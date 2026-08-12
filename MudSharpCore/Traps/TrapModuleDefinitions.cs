#nullable enable

using System.Globalization;
using MudSharp.Traps;
using MudSharp.Movement;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp.Traps;

public sealed class TrapComponentRequirementDefinition : ITrapComponentRequirement
{
	private readonly IFuturemud _gameworld;

	public TrapComponentRequirementDefinition(IFuturemud gameworld, long tagId, TrapComponentRole role,
		double spentRecoveryChance = 75.0, double qualityWeight = 1.0)
	{
		_gameworld = gameworld;
		TagId = tagId;
		Role = role;
		SpentRecoveryChance = spentRecoveryChance;
		QualityWeight = qualityWeight;
	}

	public long TagId { get; }
	public ITag? Tag => _gameworld.Tags.Get(TagId);
	public TrapComponentRole Role { get; }
	public double SpentRecoveryChance { get; }
	public double QualityWeight { get; }

	public static TrapComponentRequirementDefinition LoadFromXml(XElement root, IFuturemud gameworld)
	{
		return new TrapComponentRequirementDefinition(
			gameworld,
			long.Parse(root.Attribute("tag")?.Value ?? "0"),
			Enum.TryParse(root.Attribute("role")?.Value, true, out TrapComponentRole role)
				? role
				: TrapComponentRole.None,
			double.TryParse(root.Attribute("recovery")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture,
				out var recovery) ? recovery : 75.0,
			double.TryParse(root.Attribute("qualityweight")?.Value, NumberStyles.Float,
				CultureInfo.InvariantCulture, out var qualityWeight) ? qualityWeight : 1.0);
	}

	public XElement SaveToXml()
	{
		return new XElement("Component",
			new XAttribute("tag", TagId),
			new XAttribute("role", Role),
			new XAttribute("recovery", SpentRecoveryChance),
			new XAttribute("qualityweight", QualityWeight));
	}
}

/// <summary>
/// The persisted, builder-configurable implementation shared by the first-party trap modules.
/// Specific runtime handlers consume the named parameters that apply to their module type.
/// </summary>
public sealed class TrapTriggerDefinition : ITrapTrigger
{
	public sealed record ParameterHelp(string Name, string Description, string DefaultValue);

	private static readonly IReadOnlyList<ParameterHelp> CommonParameters =
	[
		new("chance", "percentage chance that this trigger fires (0-100)", "100"),
		new("spotdifficulty", "difficulty to notice the trap when it triggers", "Hard"),
		new("avoiddifficulty", "difficulty to avoid the trap once triggered", "Normal"),
		new("filterprog", "optional boolean FutureProg ID used to filter targets", "none"),
		new("triggerecho", "optional emote shown when this trigger fires", "none")
	];

	private static readonly IReadOnlyDictionary<TrapTriggerType, IReadOnlyList<ParameterHelp>> TypeParameters =
		new Dictionary<TrapTriggerType, IReadOnlyList<ParameterHelp>>
		{
			[TrapTriggerType.ExitTraversal] =
			[
				new("movementtypes", $"comma-separated movement types ({string.Join(", ", Enum.GetNames<MovementType>().Where(x => x is not "None" and not "All"))})", "All"),
				new("minimumsize", "smallest character size that can trigger it", "Miniscule"),
				new("maximumsize", "largest character size that can trigger it", "Titanic")
			],
			[TrapTriggerType.Proximity] =
			[
				new("maximumproximity", "proximity threshold crossed to fire the trap", "Immediate")
			],
			[TrapTriggerType.Signal] =
			[
				new("minimumvalue", "minimum signal value that fires the trap", "unbounded"),
				new("maximumvalue", "maximum signal value that fires the trap", "unbounded")
			]
		};

	public static IEnumerable<ParameterHelp> ParametersFor(TrapTriggerType type) =>
		CommonParameters.Concat(TypeParameters.GetValueOrDefault(type) ?? []);

	public static bool IsSupportedParameter(TrapTriggerType type, string name) =>
		ParametersFor(type).Any(x => x.Name.EqualTo(name));
	private static readonly IReadOnlySet<TrapSourceKind> AllDomains = new HashSet<TrapSourceKind>(Enum.GetValues<TrapSourceKind>());
	private static readonly IReadOnlySet<TrapSourceKind> MechanicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Mechanical };
	private static readonly IReadOnlyDictionary<TrapTriggerType, IReadOnlySet<TrapSourceKind>> Compatibility =
		new Dictionary<TrapTriggerType, IReadOnlySet<TrapSourceKind>>
		{
			[TrapTriggerType.ExitTraversal] = AllDomains,
			[TrapTriggerType.Openable] = AllDomains,
			[TrapTriggerType.Proximity] = AllDomains,
			[TrapTriggerType.CellEntry] = AllDomains,
			[TrapTriggerType.Signal] = MechanicalOnly,
			[TrapTriggerType.Manual] = AllDomains
		};

	public TrapTriggerDefinition(TrapTriggerType triggerType)
	{
		TriggerType = triggerType;
	}

	public TrapTriggerType TriggerType { get; }
	public IReadOnlySet<TrapSourceKind> CompatibleSourceKinds => Compatibility[TriggerType];
	private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);
	public IReadOnlyDictionary<string, string> Parameters => _parameters;

	public void SetParameter(string name, string value) => _parameters[name] = value;

	public static TrapTriggerDefinition LoadFromXml(XElement root)
	{
		if (!Enum.TryParse(root.Attribute("type")?.Value, true, out TrapTriggerType triggerType))
		{
			throw new ApplicationException($"Unknown trap trigger type '{root.Attribute("type")?.Value}'.");
		}

		var result = new TrapTriggerDefinition(triggerType);
		foreach (var parameter in root.Elements("Parameter"))
		{
			var name = parameter.Attribute("name")?.Value;
			if (!string.IsNullOrWhiteSpace(name))
			{
				result.SetParameter(name, parameter.Value);
			}
		}

		return result;
	}

	public string SaveToXml()
	{
		return new XElement("Trigger",
			new XAttribute("type", TriggerType),
			_parameters.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
				.Select(x => new XElement("Parameter", new XAttribute("name", x.Key), new XCData(x.Value))))
			.ToString();
	}
}

public sealed class TrapPayloadDefinition : ITrapPayload
{
	public sealed record ParameterHelp(string Name, string Description, string DefaultValue);

	private static readonly IReadOnlyList<ParameterHelp> CommonParameters =
	[
		new("echo", "optional emote shown to each target when this payload resolves", "none")
	];

	private static readonly IReadOnlyDictionary<TrapPayloadType, IReadOnlyList<ParameterHelp>> TypeParameters =
		new Dictionary<TrapPayloadType, IReadOnlyList<ParameterHelp>>
		{
			[TrapPayloadType.CastSpell] =
			[
				new("spell", "required ready magic spell ID to resolve when the trap fires", "required"),
				new("power", $"spell power ({string.Join(", ", Enum.GetNames<SpellPower>())})", "Standard")
			],
			[TrapPayloadType.EmitSignal] =
			[
				new("targetitem", "optional item ID with a signal sink; defaults to a matched payload component", "matched payload component"),
				new("value", "numeric signal value sent to the signal sink", "1")
			],
			[TrapPayloadType.ExecuteProg] =
			[
				new("prog", "required FutureProg ID with a supported target and/or anchor signature", "required")
			],
			[TrapPayloadType.DirectDamage] =
			[
				new("damage", "required positive damage, pain and stun amount", "required"),
				new("damagetype", $"damage type ({string.Join(", ", Enum.GetNames<DamageType>())})", "Piercing")
			],
			[TrapPayloadType.LiquidDischarge] =
			[
				new("liquid", "required liquid ID to expose targets to", "required"),
				new("amount", "positive liquid amount in litres", "0.1")
			],
			[TrapPayloadType.GasCloud] =
			[
				new("gas", "required gas ID to release", "required"),
				new("dose", "positive inhaled drug dose per unit volume when applicable", "the gas default"),
				new("duration", "positive cloud duration as a timespan", "00:00:30"),
				new("cloudecho", "room text shown when the cloud is created", "A cloud of gas billows out.")
			],
			[TrapPayloadType.Restraint] =
			[
				new("duration", "positive restraint duration as a timespan", "00:00:30"),
				new("description", "description used for the target's restraint", "caught by a trap")
			]
		};

	private static readonly IReadOnlySet<TrapSourceKind> AllDomains = new HashSet<TrapSourceKind>(Enum.GetValues<TrapSourceKind>());
	private static readonly IReadOnlySet<TrapSourceKind> MechanicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Mechanical };
	private static readonly IReadOnlySet<TrapSourceKind> MagicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Magical };
	private static readonly IReadOnlyDictionary<TrapPayloadType, IReadOnlySet<TrapSourceKind>> Compatibility =
		new Dictionary<TrapPayloadType, IReadOnlySet<TrapSourceKind>>
		{
			[TrapPayloadType.DetonateItem] = MechanicalOnly,
			[TrapPayloadType.CastSpell] = MagicalOnly,
			[TrapPayloadType.EmitSignal] = MechanicalOnly,
			[TrapPayloadType.ExecuteProg] = AllDomains,
			[TrapPayloadType.DirectDamage] = AllDomains,
			[TrapPayloadType.LiquidDischarge] = AllDomains,
			[TrapPayloadType.GasCloud] = AllDomains,
			[TrapPayloadType.Restraint] = AllDomains
		};

	public TrapPayloadDefinition(TrapPayloadType payloadType, TimeSpan delay = default,
		TrapTargetSelector targetSelector = TrapTargetSelector.Triggerer)
	{
		PayloadType = payloadType;
		Delay = delay;
		TargetSelector = targetSelector;
	}

	public TrapPayloadType PayloadType { get; }
	public IReadOnlySet<TrapSourceKind> CompatibleSourceKinds => Compatibility[PayloadType];
	public TimeSpan Delay { get; private set; }
	public TrapTargetSelector TargetSelector { get; private set; }
	private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);
	public IReadOnlyDictionary<string, string> Parameters => _parameters;

	public void SetParameter(string name, string value) => _parameters[name] = value;

	public static IEnumerable<ParameterHelp> ParametersFor(TrapPayloadType type) =>
		CommonParameters.Concat(TypeParameters.GetValueOrDefault(type) ?? []);

	public static bool IsSupportedParameter(TrapPayloadType type, string name) =>
		ParametersFor(type).Any(x => x.Name.EqualTo(name));

	public void SetDelay(TimeSpan delay) => Delay = delay;
	public void SetTargetSelector(TrapTargetSelector selector) => TargetSelector = selector;

	public static TrapPayloadDefinition LoadFromXml(XElement root)
	{
		if (!Enum.TryParse(root.Attribute("type")?.Value, true, out TrapPayloadType payloadType))
		{
			throw new ApplicationException($"Unknown trap payload type '{root.Attribute("type")?.Value}'.");
		}

		var delay = TimeSpan.TryParse(root.Attribute("delay")?.Value, out var parsedDelay) ? parsedDelay : TimeSpan.Zero;
		var targetSelector = Enum.TryParse(root.Attribute("target")?.Value, true, out TrapTargetSelector parsedSelector)
			? parsedSelector
			: TrapTargetSelector.Triggerer;
		var result = new TrapPayloadDefinition(payloadType, delay, targetSelector);
		foreach (var parameter in root.Elements("Parameter"))
		{
			var name = parameter.Attribute("name")?.Value;
			if (!string.IsNullOrWhiteSpace(name))
			{
				result.SetParameter(name, parameter.Value);
			}
		}

		return result;
	}

	public string SaveToXml()
	{
		return new XElement("Payload",
			new XAttribute("type", PayloadType),
			new XAttribute("delay", Delay.ToString("c")),
			new XAttribute("target", TargetSelector),
			_parameters.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
				.Select(x => new XElement("Parameter", new XAttribute("name", x.Key), new XCData(x.Value))))
			.ToString();
	}
}
