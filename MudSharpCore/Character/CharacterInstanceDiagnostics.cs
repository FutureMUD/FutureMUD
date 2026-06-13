using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Character;

public enum CharacterInstanceDiagnosticSeverity
{
	Info = 0,
	Warning = 1,
	Error = 2
}

public sealed record CharacterInstanceDiagnostic(
	CharacterInstanceDiagnosticSeverity Severity,
	CharacterInstanceStateScope Scope,
	string Code,
	string Message,
	CharacterInstanceDiagnosticSubject? Subject = null
);

public sealed record CharacterInstanceDiagnosticSubject(
	long? CharacterId = null,
	long? InstanceId = null,
	long? BodyId = null,
	long? LocationId = null
);

public sealed record CharacterInstanceDiagnosticReport(
	string Title,
	IReadOnlyList<CharacterInstanceDiagnostic> Diagnostics
)
{
	public bool HasErrors => Diagnostics.Any(x => x.Severity == CharacterInstanceDiagnosticSeverity.Error);
	public bool HasWarnings => Diagnostics.Any(x => x.Severity == CharacterInstanceDiagnosticSeverity.Warning);
}

public sealed record CharacterInstanceReferenceSets(
	IReadOnlySet<long> BodyIds,
	IReadOnlySet<long> LocationIds
);

public static class CharacterInstanceDiagnostics
{
	public static IReadOnlyList<CharacterInstanceDiagnostic> AuditPrimaryInstance(ICharacter character)
	{
		var diagnostics = new List<CharacterInstanceDiagnostic>();
		if (character is null)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"character-null",
				"Cannot audit a null character reference."
			));
			return diagnostics;
		}

		if (character.CurrentBody is null)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"current-body-null",
				$"Character #{character.Id} has no current body.",
				new CharacterInstanceDiagnosticSubject(character.Id)
			));
			return diagnostics;
		}

		if (!ReferenceEquals(character.Body, character.CurrentBody))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.CompatibilityMirror,
				"body-currentbody-diverged",
				$"Character #{character.Id} has Body #{character.Body?.Id ?? 0} but CurrentBody #{character.CurrentBody.Id}.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		if (!ReferenceEquals(character.CurrentBody.Actor, character))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-actor-diverged",
				$"Body #{character.CurrentBody.Id} is attached to actor #{character.CurrentBody.Actor?.Id ?? 0} instead of character #{character.Id}.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		if (character.CurrentBody.Location != character.Location)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-location-diverged",
				$"Character #{character.Id} is in location #{character.Location?.Id ?? 0} but body #{character.CurrentBody.Id} reports location #{character.CurrentBody.Location?.Id ?? 0}.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		if (character.CurrentBody.RoomLayer != character.RoomLayer)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Instance,
				"body-layer-diverged",
				$"Character #{character.Id} is on layer {(int)character.RoomLayer} but body #{character.CurrentBody.Id} reports layer {(int)character.CurrentBody.RoomLayer}.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		if (!character.Bodies.Contains(character.CurrentBody))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"current-body-not-owned",
				$"Character #{character.Id} has current body #{character.CurrentBody.Id}, but it is not present in the owned body list.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		if (!character.Forms.Any(x => ReferenceEquals(x.Body, character.CurrentBody)))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Warning,
				CharacterInstanceStateScope.Body,
				"current-body-no-form",
				$"Character #{character.Id} has current body #{character.CurrentBody.Id}, but no form metadata points at it.",
				new CharacterInstanceDiagnosticSubject(character.Id, character.InstanceId, character.CurrentBody.Id,
					character.Location?.Id)
			));
		}

		diagnostics.AddRange(AuditLoadedIdentity(character.Identity));

		return diagnostics;
	}

	public static IReadOnlyList<CharacterInstanceDiagnostic> AuditLoadedIdentity(ICharacterIdentity identity)
	{
		var diagnostics = new List<CharacterInstanceDiagnostic>();
		if (identity is null)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"identity-null",
				"Cannot audit a null identity reference."
			));
			return diagnostics;
		}

		var instances = identity.Instances?.ToList() ?? new List<ICharacterInstance>();
		var primaryInstances = instances
		                       .Where(x => x.IsPrimaryInstance)
		                       .ToList();
		if (primaryInstances.Count == 0)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"missing-primary-instance",
				$"Identity #{identity.Id} has no loaded primary instance.",
				new CharacterInstanceDiagnosticSubject(identity.Id)
			));
		}

		if (primaryInstances.Count > 1)
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"duplicate-primary-instance",
				$"Identity #{identity.Id} has {primaryInstances.Count} loaded primary instances.",
				new CharacterInstanceDiagnosticSubject(identity.Id)
			));
		}

		foreach (var instance in instances)
		{
			var subject = new CharacterInstanceDiagnosticSubject(
				identity.Id,
				instance.InstanceId,
				instance.Body?.Id,
				instance.Location?.Id);

			if (instance.Identity.Id != identity.Id)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Identity,
					"instance-identity-diverged",
					$"Instance #{instance.InstanceId} reports identity #{instance.Identity.Id} instead of identity #{identity.Id}.",
					subject
				));
			}

			if (instance.IsPrimaryInstance &&
			    (instance.InstanceKind != CharacterInstanceKind.Primary ||
			     instance.DeathPolicy != CharacterInstanceDeathPolicy.FinalCharacterDeath ||
			     instance.PerceptionPolicy != CharacterInstancePerceptionPolicy.OrdinaryEmbodied ||
			     instance.PersistencePolicy != CharacterInstancePersistencePolicy.Persistent ||
			     !instance.IsEmbodied))
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.CompatibilityMirror,
					"primary-policy-mismatch",
					$"Primary instance #{instance.InstanceId} has kind {instance.InstanceKind}, death {instance.DeathPolicy}, perception {instance.PerceptionPolicy}, persistence {instance.PersistencePolicy}, embodied {instance.IsEmbodied}.",
					subject
				));
			}

			if (!instance.IsPrimaryInstance && instance.InstanceKind == CharacterInstanceKind.Primary)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"secondary-primary-flag-mismatch",
					$"Secondary instance #{instance.InstanceId} is marked with primary instance kind.",
					subject
				));
			}

			if (instance.IsEmbodied &&
			    instance.Location is null &&
			    !instance.State.HasFlag(CharacterState.Dead) &&
			    instance.Status != CharacterStatus.Deceased)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"embodied-without-location",
					$"Embodied live instance #{instance.InstanceId} has no loaded location.",
					subject
				));
			}

			if (instance.IsControllable && instance.ControlPolicy == CharacterInstanceControlPolicy.NotControllable)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"controllable-with-not-controllable-policy",
					$"Instance #{instance.InstanceId} is controllable but has NotControllable policy.",
					subject
				));
			}
		}

		foreach (var bodyGroup in instances
		                          .Where(x => x.IsEmbodied &&
		                                      x.Body is not null &&
		                                      !x.State.HasFlag(CharacterState.Dead) &&
		                                      x.Status != CharacterStatus.Deceased)
		                          .GroupBy(x => x.Body.Id)
		                          .Where(x => x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"duplicate-embodied-body",
				$"Body #{bodyGroup.Key} is embodied by {bodyGroup.Count()} loaded instances for identity #{identity.Id}.",
				new CharacterInstanceDiagnosticSubject(identity.Id, BodyId: bodyGroup.Key)
			));
		}

		return diagnostics;
	}

	public static IReadOnlyList<CharacterInstanceDiagnostic> AuditPersistedInstances(
		IEnumerable<MudSharp.Models.CharacterInstance> instances,
		bool includeReferenceChecks = true,
		CharacterInstanceReferenceSets? references = null)
	{
		var diagnostics = new List<CharacterInstanceDiagnostic>();
		var list = instances?.ToList() ?? new List<MudSharp.Models.CharacterInstance>();

		foreach (var group in list.Where(x => x.IsPrimary)
		                          .GroupBy(x => x.CharacterId)
		                          .Where(x => x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Identity,
				"duplicate-primary-instance",
				$"Identity #{group.Key} has {group.Count()} persisted primary instance rows.",
				new CharacterInstanceDiagnosticSubject(group.Key)
			));
		}

		foreach (var group in list.Where(x =>
			                          x.IsEmbodied &&
			                          !((CharacterState)x.State).HasFlag(CharacterState.Dead) &&
			                          (CharacterStatus)x.Status != CharacterStatus.Deceased)
		                          .GroupBy(x => x.BodyId)
		                          .Where(x => x.Count() > 1))
		{
			diagnostics.Add(new CharacterInstanceDiagnostic(
				CharacterInstanceDiagnosticSeverity.Error,
				CharacterInstanceStateScope.Body,
				"duplicate-embodied-body",
				$"Body #{group.Key} has {group.Count()} live embodied instance rows.",
				new CharacterInstanceDiagnosticSubject(BodyId: group.Key)
			));
		}

		foreach (var instance in list)
		{
			var subject = new CharacterInstanceDiagnosticSubject(
				instance.CharacterId,
				instance.Id,
				instance.BodyId,
				instance.LocationId);

			if (includeReferenceChecks &&
			    references?.BodyIds is not null &&
			    !references.BodyIds.Contains(instance.BodyId))
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Body,
					"stale-body-reference",
					$"Persisted instance #{instance.Id} references missing body #{instance.BodyId}.",
					subject
				));
			}

			if (includeReferenceChecks &&
			    references?.LocationIds is not null &&
			    instance.LocationId.HasValue &&
			    !references.LocationIds.Contains(instance.LocationId.Value))
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"stale-location-reference",
					$"Persisted instance #{instance.Id} references missing location #{instance.LocationId.Value}.",
					subject
				));
			}

			if (!HasValidEffectData(instance.EffectData))
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"malformed-effect-data",
					$"Persisted instance #{instance.Id} has malformed EffectData XML.",
					subject
				));
			}

			if (instance.IsPrimary &&
			    ((CharacterInstanceKind)instance.InstanceKind != CharacterInstanceKind.Primary ||
			     (CharacterInstanceDeathPolicy)instance.DeathPolicy != CharacterInstanceDeathPolicy.FinalCharacterDeath ||
			     (CharacterInstancePerceptionPolicy)instance.PerceptionPolicy !=
			     CharacterInstancePerceptionPolicy.OrdinaryEmbodied ||
			     (CharacterInstancePersistencePolicy)instance.PersistencePolicy !=
			     CharacterInstancePersistencePolicy.Persistent ||
			     !instance.IsEmbodied))
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.CompatibilityMirror,
					"primary-policy-mismatch",
					$"Persisted primary instance #{instance.Id} has kind {(CharacterInstanceKind)instance.InstanceKind}, death {(CharacterInstanceDeathPolicy)instance.DeathPolicy}, perception {(CharacterInstancePerceptionPolicy)instance.PerceptionPolicy}, persistence {(CharacterInstancePersistencePolicy)instance.PersistencePolicy}, embodied {instance.IsEmbodied}.",
					subject
				));
			}

			if (!instance.IsPrimary && (CharacterInstanceKind)instance.InstanceKind == CharacterInstanceKind.Primary)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"secondary-primary-flag-mismatch",
					$"Persisted secondary instance #{instance.Id} is marked with primary instance kind.",
					subject
				));
			}

			if (instance.IsEmbodied &&
			    !((CharacterState)instance.State).HasFlag(CharacterState.Dead) &&
			    (CharacterStatus)instance.Status != CharacterStatus.Deceased &&
			    instance.LocationId is null)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"embodied-without-location",
					$"Persisted embodied live instance #{instance.Id} has no location.",
					subject
				));
			}

			if (instance.IsControllable &&
			    (CharacterInstanceControlPolicy)instance.ControlPolicy == CharacterInstanceControlPolicy.NotControllable)
			{
				diagnostics.Add(new CharacterInstanceDiagnostic(
					CharacterInstanceDiagnosticSeverity.Error,
					CharacterInstanceStateScope.Instance,
					"controllable-with-not-controllable-policy",
					$"Persisted instance #{instance.Id} is controllable but has NotControllable policy.",
					subject
				));
			}
		}

		return diagnostics;
	}

	public static string RenderDiagnosticsTable(IEnumerable<CharacterInstanceDiagnostic> diagnostics,
		int lineFormatLength = 120, bool unicodeTable = false)
	{
		var list = diagnostics?
		           .OrderByDescending(x => x.Severity)
		           .ThenBy(x => x.Code)
		           .ThenBy(x => x.Subject?.CharacterId ?? 0)
		           .ThenBy(x => x.Subject?.InstanceId ?? 0)
		           .ToList() ?? new List<CharacterInstanceDiagnostic>();
		if (!list.Any())
		{
			return "No character instance diagnostics were found.";
		}

		return StringUtilities.GetTextTable(
			list.Select(x => new[]
			{
				x.Severity.DescribeEnum(),
				x.Code,
				x.Scope.DescribeEnum(),
				FormatSubject(x.Subject),
				x.Message
			}),
			new[] { "Severity", "Code", "Scope", "Subject", "Message" },
			lineFormatLength,
			colour: Telnet.Cyan,
			truncatableColumnIndex: 4,
			unicodeTable: unicodeTable
		);
	}

	private static bool HasValidEffectData(string effectData)
	{
		if (string.IsNullOrWhiteSpace(effectData))
		{
			return false;
		}

		try
		{
			XElement.Parse(effectData);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static string FormatSubject(CharacterInstanceDiagnosticSubject? subject)
	{
		if (subject is null)
		{
			return string.Empty;
		}

		var parts = new List<string>();
		if (subject.CharacterId.HasValue)
		{
			parts.Add($"C#{subject.CharacterId.Value.ToString("N0", CultureInfo.InvariantCulture)}");
		}

		if (subject.InstanceId.HasValue)
		{
			parts.Add($"I#{subject.InstanceId.Value.ToString("N0", CultureInfo.InvariantCulture)}");
		}

		if (subject.BodyId.HasValue)
		{
			parts.Add($"B#{subject.BodyId.Value.ToString("N0", CultureInfo.InvariantCulture)}");
		}

		if (subject.LocationId.HasValue)
		{
			parts.Add($"L#{subject.LocationId.Value.ToString("N0", CultureInfo.InvariantCulture)}");
		}

		return string.Join(" ", parts);
	}
}
