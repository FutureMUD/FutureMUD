#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MudSharp.Database;
using MudSharp.Magic;

namespace DatabaseSeeder.Seeders;

/// <summary>ID-only selections. No name guessing, fixture defaults, file access or player options.</summary>
public sealed record ArmageddonPreparedWorldBindings(ArmageddonMagicInstallPlan Utilities, long ReserveResource,
	long Decorator, long AlwaysTrueProg, long GatheringTemplate, IReadOnlyDictionary<string, long> SupportSkills,
	long CapacityAttribute, long CapacityExpression, string CapacityBasis,
	IReadOnlyDictionary<string, IReadOnlyList<MagicGatheringMethodKind>> AllowedMethods,
	ArmageddonProvisionInstallPlan? Provisions = null)
{
	public ArmageddonWaterSeeBindings? WaterSee { get; init; }
}

public sealed partial class ArmageddonMagicSeeder
{
	public const string InstallQuestion = "install-armageddon-partial";
	public const string BindingsQuestion = "armageddon-prepared-bindings";
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		Converters = { new JsonStringEnumConverter<MagicGatheringMethodKind>(allowIntegerValues: false) }
	};
	private static bool IsYes(string answer) => answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
	public IEnumerable<SeederQuestion> Questions => !Enabled ? Array.Empty<SeederQuestion>() :
	[
		new(InstallQuestion, "Install/reconcile the PARTIAL Armageddon package in this prepared world? yes/no (default no)",
			(_, _) => true, (answer, _) => answer.Trim().Equals("no", StringComparison.OrdinalIgnoreCase) || IsYes(answer)
				? (true, "") : (false, "Answer yes or no."), DefaultAnswerResolver: (_, _) => "no", PersistAnswer: false),
		new(BindingsQuestion, "Paste the explicit ID-only binding JSON from the builder guide. Self-only is the conservative qualified suggestion; validated selected routes remain configurable.",
			(_, answers) => answers.TryGetValue(InstallQuestion, out var answer) && IsYes(answer), ValidateBindings)
	];
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		Questions.Select(x => (x.Id, x.Question, x.Filter, x.Validator));

	public static string SerializeBindings(ArmageddonPreparedWorldBindings bindings) => JsonSerializer.Serialize(bindings, JsonOptions);
	public static ArmageddonPreparedWorldBindings ParseBindings(string json)
	{
		if (json.Length > 32768) throw new FormatException("Bindings exceed 32768 characters.");
		using var document = JsonDocument.Parse(json);
		void Unique(JsonElement element)
		{
			if (element.ValueKind == JsonValueKind.Object)
			{
				var keys = new HashSet<string>(StringComparer.Ordinal);
				foreach (var property in element.EnumerateObject())
				{
					if (!keys.Add(property.Name)) throw new FormatException($"Duplicate binding property {property.Name}.");
					Unique(property.Value);
				}
			}
			else if (element.ValueKind == JsonValueKind.Array) foreach (var child in element.EnumerateArray()) Unique(child);
		}
		Unique(document.RootElement);
		return JsonSerializer.Deserialize<ArmageddonPreparedWorldBindings>(json, JsonOptions) ?? throw new FormatException("Bindings cannot be null.");
	}
	public static (bool Success, string error) ValidateBindings(string json, FuturemudDatabaseContext context)
	{
		try
		{
			var errors = ArmageddonPreparedWorldInstaller.Validate(context, ParseBindings(json));
			return errors.Count == 0 ? (true, "") : (false, string.Join(Environment.NewLine, errors));
		}
		catch (Exception error) when (error is JsonException or FormatException or ArgumentException or InvalidOperationException or NullReferenceException or System.Xml.XmlException)
		{ return (false, "Invalid explicit bindings: " + error.Message); }
	}
}
