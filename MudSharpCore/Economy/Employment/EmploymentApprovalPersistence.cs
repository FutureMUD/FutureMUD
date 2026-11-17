#nullable enable

using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Economy.Employment;

internal static class EmploymentApprovalJson
{
	private sealed class ReadOnlySetConverter<T> : JsonConverter<IReadOnlySet<T>>
	{
		public override IReadOnlySet<T> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) =>
			new HashSet<T>(JsonSerializer.Deserialize<List<T>>(ref reader, options) ?? throw new JsonException("Missing set."));
		public override void Write(Utf8JsonWriter writer, IReadOnlySet<T> value, JsonSerializerOptions options) =>
			JsonSerializer.Serialize(writer, value.ToArray(), options);
	}

	internal static readonly JsonSerializerOptions Options = new()
	{
		RespectRequiredConstructorParameters = true,
		Converters = { new ReadOnlySetConverter<string>(), new ReadOnlySetConverter<int>() }
	};

	internal sealed record Document(int Version, Guid TaskId, string HostType, long HostId,
		string PlanBinding, EmploymentTaskProvenance Provenance, bool LegacyApproval = false);
	internal sealed record GoalDocument(int Version, EmploymentPrincipal? Principal,
		EmploymentConditionExpression? Expression, ManagerGoalRiskLimits RiskLimits,
		IReadOnlyList<EmploymentAuthorisationLimit> Budgets);
	internal sealed record RuleDocument(int Version, EmploymentPrincipal? Principal, EmploymentAuthority IssuanceAuthority);

	internal static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);
	internal static T? Read<T>(string? json) where T : class
	{
		if (string.IsNullOrWhiteSpace(json) || json.Length > 1024 * 1024)
		{
			return null;
		}
		try { return JsonSerializer.Deserialize<T>(json, Options); }
		catch (Exception ex) when (ex is JsonException or ArgumentException or NotSupportedException) { return null; }
	}

	internal static bool Valid(Document? document, Guid taskId, IEmploymentHost host, Guid correlationId,
		string planBinding)
	{
		var p = document?.Provenance;
		var g = p?.AuthorisationGrant;
		return document is { Version: 1 } && document.TaskId == taskId && document.HostId == host.Id &&
		       document.HostType == host.EmploymentHostType.ToString() && document.PlanBinding == planBinding &&
		       p is not null && Enum.IsDefined(p.SourceKind) && p.CreatedByPrincipal is not null && p.AuthorisedByPrincipal is not null &&
		       Enum.IsDefined(p.CreatedByPrincipal.Kind) && Enum.IsDefined(p.AuthorisedByPrincipal.Kind) &&
		       g is not null && g.Id != Guid.Empty && g.Revision > 0 && g.CorrelationId == correlationId &&
		       g.IssuerPrincipal is not null && Enum.IsDefined(g.IssuerPrincipal.Kind) &&
		       (g.AuthorityPrincipal is null || Enum.IsDefined(g.AuthorityPrincipal.Kind)) &&
		       g.AmountLimits is not null && g.AmountLimits.All(x => x.Key > 0 && x.Value >= 0) &&
		       g.PaymentSourceScopes is not null && g.CounterpartyScopes is not null &&
		       (g.ExpiresAt is null || g.ExpiresAt > g.IssuedAt);
	}
}

public sealed partial class EmploymentPersistenceStore
{
	private IEmploymentActionStep LoadBoundActionStep(EmploymentActionStepRecord record)
	{
		try { return ToActionStep(record) ?? new UnresolvedEmploymentStep(); }
		catch (Exception ex) when (ex is JsonException or FormatException or ArgumentException or InvalidOperationException)
		{
			return new UnresolvedEmploymentStep();
		}
	}

	private static string GoalApprovalDocument(ManagerGoal goal) => EmploymentApprovalJson.Write(
		new EmploymentApprovalJson.GoalDocument(1, goal.AuthorisingPrincipal, goal.Configuration.ConditionExpression,
			goal.Policy.RiskLimits, goal.Policy.BudgetLimits.Select(x => new EmploymentAuthorisationLimit(x.Currency.Id, x.Amount)).ToList()));

	private static readonly PropertyInfo[] ApprovalStepProperties = typeof(EmploymentActionStepRecord).GetProperties()
		.Where(x => (x.PropertyType.IsValueType || x.PropertyType == typeof(string)) &&
		            x.Name is not nameof(EmploymentActionStepRecord.Id) and not nameof(EmploymentActionStepRecord.EmploymentActionPlanId))
		.OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();

	private static string PlanBinding(IEnumerable<EmploymentActionStepRecord> steps)
	{
		var values = steps.OrderBy(x => x.SortOrder)
			.Select(x => ApprovalStepProperties.ToDictionary(p => p.Name, p => p.GetValue(x))).ToList();
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(EmploymentApprovalJson.Write(values))));
	}

	private static string StoredPlanBinding(FuturemudDatabaseContext context, long planId) =>
		PlanBinding(context.EmploymentActionSteps.AsNoTracking().Where(x => x.EmploymentActionPlanId == planId).ToList());

	private static string ApprovalDocument(EmploymentActiveTask task) => EmploymentApprovalJson.Write(
		new EmploymentApprovalJson.Document(1, task.Id, task.Employer.EmploymentHostType.ToString(), task.Employer.Id,
			task.ApprovalPlanBinding ?? string.Empty, task.Provenance,
			task.LegacyApproval && task.AuthorisationGrant.Authority.Authorities == EmploymentAuthority.None));

	public void SaveApprovalTransition(EmploymentActiveTask task, EmploymentRegisterEntry entry,
		Guid expectedGrantId, int expectedRevision)
	{
		WithContext(caller =>
		{
			// Isolate immediate approval mutations from the engine's deferred save context.
			using var independent = caller.Database.IsRelational() ? FMDB.BeginIndependentScope(requireWrites: true) : null;
			var context = FMDB.Context;
			using var transaction = context.Database.IsRelational() ? context.Database.BeginTransaction() : null;
			var row = context.EmploymentActiveTasks.AsNoTracking().Single(x => x.PublicId == task.Id.ToString("D") && x.EmploymentHostStateId == StateId);
			var stored = EmploymentApprovalJson.Read<EmploymentApprovalJson.Document>(row.ProvenanceJson);
			if (row.ProvenanceJson is not null && (stored?.Version != 1 || stored.Provenance?.AuthorisationGrant is not { } grant ||
				grant.Id != expectedGrantId || grant.Revision != expectedRevision) ||
			    row.ProvenanceJson is null && !task.LegacyApproval)
			{
				throw new InvalidOperationException("Task approval changed; show it again before making a recovery decision.");
			}
			if (task.ApprovalPlanBinding != StoredPlanBinding(context, row.EmploymentActionPlanId))
			{
				throw new InvalidOperationException("The stored task plan changed; approval recovery is quarantined.");
			}
			SaveActiveTaskState(task, expectedGrantId, expectedRevision);
			SaveRegisterEntry(entry);
			transaction?.Commit();
			if (independent is not null)
			{
				foreach (var tracked in caller.ChangeTracker.Entries<EmploymentActiveTaskStepStateRecord>()
					.Where(x => x.Entity.EmploymentActiveTaskId == row.Id).ToList()) tracked.State = EntityState.Detached;
				foreach (var tracked in caller.ChangeTracker.Entries<EmploymentActiveTaskRecord>()
					.Where(x => x.Entity.Id == row.Id).ToList()) tracked.State = EntityState.Detached;
			}
		});
	}
}
