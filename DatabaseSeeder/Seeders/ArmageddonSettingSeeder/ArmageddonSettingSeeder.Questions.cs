#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>ID-only selections. No name guessing, fixture defaults, file access or player options.</summary>
public sealed record ArmageddonPreparedWorldBindings(ArmageddonMagicInstallPlan Utilities, long ReserveResource,
	long Decorator, long AlwaysTrueProg, long GatheringTemplate, IReadOnlyDictionary<string, long> SupportSkills,
	long CapacityAttribute, long CapacityExpression, string CapacityBasis,
	IReadOnlyDictionary<string, IReadOnlyList<MagicGatheringMethodKind>> AllowedMethods,
	ArmageddonProvisionInstallPlan? Provisions = null)
{
	public ArmageddonWaterSeeBindings? WaterSee { get; init; }
	public ArmageddonEmotionalBindings? Emotions { get; init; }
}

public sealed partial class ArmageddonSettingSeeder
{
	public const string InstallQuestion = "install-armageddon-setting-prepared";
	public const string BindingsQuestion = "armageddon-setting-prepared-bindings";
	public const string FullInstallQuestion = "install-armageddon-setting";
	public const string WeatherQuestion = "armageddon-weather";
	public const string ModeQuestion = "armageddon-progression-mode";
	public const string CapacityQuestion = "armageddon-setting-capacity-attribute";
	public const string NativeBindingsQuestion = "armageddon-setting-native-bindings";
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		Converters = { new JsonStringEnumConverter<MagicGatheringMethodKind>(allowIntegerValues: false),
			new JsonStringEnumConverter<Difficulty>(allowIntegerValues: false) }
	};
	private static bool IsYes(string answer) => answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
	public IEnumerable<SeederQuestion> Questions =>
	[
		new(FullInstallQuestion, "Install or update the Armageddon setting content? yes/no (default no). Existing characters and builder customisations are preserved.",
			(_, _) => true, (answer, _) => answer.Trim().Equals("no", StringComparison.OrdinalIgnoreCase) || IsYes(answer) ? (true, "") : (false, "Answer yes or no."), DefaultAnswerResolver: (_, _) => "no", PersistAnswer: false),
        new(WeatherQuestion, "Install or update all four authored Armageddon regional weather templates? yes/no. Requires Armageddon time, Suk-Krath and shared Weather Seeder dust foundations.",
            (_, answers) => FullConsent(answers), ArmageddonWeatherInstaller.ValidateAnswer,
            DefaultAnswerResolver: ArmageddonWeatherInstaller.DefaultAnswer),
		new(ModeQuestion, "Choose the magic progression style: Classic or Simple. This choice cannot be changed by rerunning the seeder.",
			(db, answers) => FullConsent(answers) && ArmageddonInstallation.Read(db) is null,
			(answer, _) => Enum.GetNames<ArmageddonProgressionMode>().Contains(answer.Trim(), StringComparer.OrdinalIgnoreCase) ? (true, "") : (false, "Choose Classic or Simple."),
			SharedAnswerKey: ArmageddonInstallation.ModeAnswerKey, DefaultAnswerResolver: (_, _) => "Classic", PersistAnswer: false),
		new(CapacityQuestion, "Which mental or spiritual attribute should determine magical energy capacity? Enter its attribute ID; no unique default was found.",
			(db, answers) => FullConsent(answers) && ArmageddonInstallation.Read(db) is null && ArmageddonInstallation.ResolveCapacityAttribute(db) == 0,
			(answer, db) => long.TryParse(answer, out var id) && db.TraitDefinitions.Any(x => x.Id == id && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && (x.TraitGroup == "Mental" || x.TraitGroup == "Spiritual")) ? (true, "") : (false, "Select an existing ordinary Mental/Spiritual body attribute."), PersistAnswer: false),
		new(NativeBindingsQuestion, "Some required world settings could not be selected automatically. Enter their IDs as JSON, for example {\"SkillDecorator\":123}. Creature and item templates are created automatically.",
			(db, answers) => FullConsent(answers) && ArmageddonInstallation.Read(db) is null && NeedsNativeBindings(db),
			ValidateNativeOverrides, DefaultAnswerResolver: (_, _) => "{}", PersistAnswer: false),
		new(InstallQuestion, "Install/reconcile the PARTIAL Armageddon package in this prepared world? yes/no (default no)",
			(_, _) => false, (answer, _) => answer.Trim().Equals("no", StringComparison.OrdinalIgnoreCase) || IsYes(answer)
				? (true, "") : (false, "Answer yes or no."), DefaultAnswerResolver: (_, _) => "no", PersistAnswer: false),
		new(BindingsQuestion, "Paste the explicit ID-only binding JSON from the builder guide. Self-only is the conservative qualified suggestion; validated selected routes remain configurable.",
			(_, answers) => answers.TryGetValue(InstallQuestion, out var answer) && IsYes(answer), ValidateBindings)
	];
	private static bool FullConsent(IReadOnlyDictionary<string, string> answers) => answers.TryGetValue(FullInstallQuestion, out var answer) && IsYes(answer);
	private static bool NeedsNativeBindings(FuturemudDatabaseContext db) => ArmageddonInstallation.ValidateFoundations(db,
		new(1, ArmageddonProgressionMode.Classic, false, 0, ArmageddonInstallation.ResolveNativeBindings(db)), checkCapacity: false).Count > 0;
	private static (bool Success, string error) ValidateNativeOverrides(string json, FuturemudDatabaseContext db)
	{
		try
		{
			var bindings = ArmageddonInstallation.ResolveNativeBindings(db, ArmageddonInstallation.ParseOverrides(json));
			var errors = ArmageddonInstallation.ValidateFoundations(db, new(1, ArmageddonProgressionMode.Classic, false, 0, bindings), checkCapacity: false);
			return errors.Count == 0 ? (true, "") : (false, string.Join(Environment.NewLine, errors));
		}
		catch (Exception error) when (error is JsonException or FormatException or ArgumentException or InvalidOperationException) { return (false, error.Message); }
	}
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
