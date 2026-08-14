#nullable enable

using System.Globalization;
using ExpressionEngine;
using MudSharp.Traps;
using MudSharp.Movement;
using MudSharp.Framework;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

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
	public sealed record ParameterHelp(string Name, string Syntax, string Description, string DefaultValue,
		bool Optional = true);

	private static readonly IReadOnlyList<ParameterHelp> CommonParameters =
	[
		new("chance", "<0-100>", "percentage chance that this trigger fires", "100"),
		new("spotdifficulty", "<difficulty>", "difficulty to notice the trap when it triggers", "Hard"),
		new("avoiddifficulty", "<difficulty>", "difficulty to avoid the trap once triggered", "Normal"),
		new("filterprog", "<FutureProg ID|none>", "boolean FutureProg used to filter targets", "none"),
		new("triggerecho", "<emote|none>", "emote shown when this trigger fires", "none")
	];

	private static readonly IReadOnlyDictionary<TrapTriggerType, IReadOnlyList<ParameterHelp>> TypeParameters =
		new Dictionary<TrapTriggerType, IReadOnlyList<ParameterHelp>>
		{
			[TrapTriggerType.ExitTraversal] =
			[
				new("movementtypes", "<All|type[,type...]>", $"movement types: {string.Join(", ", Enum.GetNames<MovementType>().Where(x => x is not "None" and not "All"))}", "All"),
				new("minimumsize", "<size category>", "smallest character size that can trigger it", "Miniscule"),
				new("maximumsize", "<size category>", "largest character size that can trigger it", "Titanic")
			],
			[TrapTriggerType.Proximity] =
			[
				new("maximumproximity", "<proximity>", "proximity threshold crossed to fire the trap", "Immediate")
			],
			[TrapTriggerType.Signal] =
			[
				new("minimumvalue", "<number|none>", "minimum signal value that fires the trap", "unbounded"),
				new("maximumvalue", "<number|none>", "maximum signal value that fires the trap", "unbounded")
			]
		};

	public static IEnumerable<ParameterHelp> ParametersFor(TrapTriggerType type) =>
		CommonParameters.Concat(TypeParameters.GetValueOrDefault(type) ?? []);

	public static bool IsSupportedParameter(TrapTriggerType type, string name) =>
		ParametersFor(type).Any(x => x.Name.EqualTo(name));

	public static ParameterHelp? ParameterFor(TrapTriggerType type, string name) =>
		ParametersFor(type).FirstOrDefault(x => x.Name.EqualTo(name));

	public static bool TryValidateParameter(TrapTriggerType type, string name, string value, out string error)
	{
		var parameter = ParameterFor(type, name);
		if (parameter is null)
		{
			error = $"{name} is not supported by {type.DescribeEnum()} triggers.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(value))
		{
			error = $"The {parameter.Name} parameter requires {parameter.Syntax}.";
			return false;
		}

		if (parameter.Optional && value.EqualTo("none"))
		{
			error = string.Empty;
			return true;
		}

		var valid = parameter.Name.CollapseString() switch
		{
			"chance" => TrapParameterValidation.TryParseFiniteDouble(value, out var chance) && chance is >= 0.0 and <= 100.0,
			"spotdifficulty" or "avoiddifficulty" => TrapParameterValidation.TryParseDefinedEnum<Difficulty>(value, out _),
			"filterprog" => TrapParameterValidation.TryParsePositiveLong(value, out _),
			"movementtypes" => TrapParameterValidation.TryParseMovementTypes(value),
			"minimumsize" or "maximumsize" => TrapParameterValidation.TryParseDefinedEnum<SizeCategory>(value, out _),
			"maximumproximity" => TrapParameterValidation.TryParseDefinedEnum<Proximity>(value, out _),
			"minimumvalue" or "maximumvalue" => TrapParameterValidation.TryParseFiniteDouble(value, out _),
			_ => true
		};
		if (valid)
		{
			error = string.Empty;
			return true;
		}

		error = $"The {parameter.Name} parameter requires {parameter.Syntax}.";
		return false;
	}

	public static bool TryValidateParameters(TrapTriggerType type, IReadOnlyDictionary<string, string> parameters,
		out string error)
	{
		if (!Compatibility.ContainsKey(type))
		{
			error = $"{type} is not a supported trap trigger type.";
			return false;
		}

		foreach (var parameter in parameters)
		{
			if (!TryValidateParameter(type, parameter.Key, parameter.Value, out error))
			{
				return false;
			}
		}

		if (parameters.TryGetValue("minimumvalue", out var minimumValue) &&
		    parameters.TryGetValue("maximumvalue", out var maximumValue) &&
		    TrapParameterValidation.TryParseFiniteDouble(minimumValue, out var minimum) &&
		    TrapParameterValidation.TryParseFiniteDouble(maximumValue, out var maximum) && minimum > maximum)
		{
			error = "The minimumvalue parameter cannot exceed maximumvalue.";
			return false;
		}

		if (parameters.TryGetValue("minimumsize", out var minimumSize) &&
		    parameters.TryGetValue("maximumsize", out var maximumSize) &&
		    TrapParameterValidation.TryParseDefinedEnum<SizeCategory>(minimumSize, out var minimumCategory) &&
		    TrapParameterValidation.TryParseDefinedEnum<SizeCategory>(maximumSize, out var maximumCategory) &&
		    minimumCategory > maximumCategory)
		{
			error = "The minimumsize parameter cannot exceed maximumsize.";
			return false;
		}

		error = string.Empty;
		return true;
	}
	private static readonly IReadOnlySet<TrapSourceKind> AllDomains = new HashSet<TrapSourceKind>(Enum.GetValues<TrapSourceKind>());
	private static readonly IReadOnlySet<TrapSourceKind> MechanicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Mechanical };
	private static readonly IReadOnlySet<TrapSourceKind> NoDomains = new HashSet<TrapSourceKind>();
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
	public IReadOnlySet<TrapSourceKind> CompatibleSourceKinds => Compatibility.GetValueOrDefault(TriggerType) ?? NoDomains;
	private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);
	public IReadOnlyDictionary<string, string> Parameters => _parameters;

	public bool SetParameter(string name, string value)
	{
		if (ParameterFor(TriggerType, name)?.Optional == true && value.EqualTo("none"))
		{
			_parameters.Remove(name);
			return true;
		}

		if (!TryValidateParameter(TriggerType, name, value, out _))
		{
			return false;
		}

		var parameters = _parameters.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
		parameters[name] = value;
		if (!TryValidateParameters(TriggerType, parameters, out _))
		{
			return false;
		}

		_parameters[name] = value;
		return true;
	}

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
				if (!result.SetParameter(name, parameter.Value))
				{
					result._parameters[name] = parameter.Value;
				}
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
	public sealed record ParameterHelp(string Name, string Syntax, string Description, string DefaultValue,
		bool Optional = true);

	private static readonly IReadOnlyList<ParameterHelp> CommonParameters =
	[
		new("echo", "<emote|none>", "emote shown to each target when this payload resolves", "none")
	];

	private static readonly IReadOnlyDictionary<TrapPayloadType, IReadOnlyList<ParameterHelp>> TypeParameters =
		new Dictionary<TrapPayloadType, IReadOnlyList<ParameterHelp>>
		{
			[TrapPayloadType.CastSpell] =
			[
				new("spell", "<ready spell ID>", "magic spell to resolve when the trap fires", "required", false),
				new("power", "<spell power>", $"spell power: {string.Join(", ", Enum.GetNames<SpellPower>())}", "Standard")
			],
			[TrapPayloadType.EmitSignal] =
			[
				new("targetitem", "<signal-sink item ID|none>", "item with a signal sink; none uses a matched payload component", "matched payload component"),
				new("value", "<number|none>", "signal value sent to the signal sink", "1")
			],
			[TrapPayloadType.ExecuteProg] =
			[
				new("prog", "<FutureProg ID>", "FutureProg with a supported target and/or anchor signature", "required", false)
			],
			[TrapPayloadType.DirectDamage] =
			[
				new("damage", "<expression>", "physical wound damage formula; may use quality and power", "required", false),
				new("pain", "<expression|none>", "pain formula; may use damage, quality and power; none uses the resolved damage", "damage"),
				new("stun", "<expression|none>", "stun formula; may use damage, quality and power; none uses the resolved damage", "damage"),
				new("damagetype", "<damage type|none>", $"damage type: {string.Join(", ", Enum.GetNames<DamageType>())}", "Piercing")
			],
			[TrapPayloadType.ExplosiveDamage] =
			[
				new("damage", "<expression>", "explosion wound damage formula; may use quality and power", "required", false),
				new("pain", "<expression|none>", "pain formula; may use damage, quality and power; none uses the resolved damage", "damage"),
				new("stun", "<expression|none>", "stun formula; may use damage, quality and power; none uses the resolved damage", "damage"),
				new("damagetype", "<damage type|none>", $"damage type: {string.Join(", ", Enum.GetNames<DamageType>())}", "Shockwave"),
				new("explosionsize", "<size category|none>", $"explosion coverage size: {string.Join(", ", Enum.GetNames<SizeCategory>())}", "Normal"),
				new("maximumproximity", "<proximity|none>", $"furthest affected proximity: {string.Join(", ", Enum.GetNames<Proximity>().Where(x => x != nameof(Proximity.Unapproximable)))}", "Proximate"),
				new("elevation", "<finite metres|none>", "explosion height relative to the anchor, used to select affected body orientations", "0")
			],
			[TrapPayloadType.LiquidDischarge] =
			[
				new("liquid", "<liquid ID>", "liquid to expose targets to", "required", false),
				new("amount", "<positive litres|none>", "liquid amount in litres", "0.1")
			],
			[TrapPayloadType.GasCloud] =
			[
				new("gas", "<gas ID>", "gas to release", "required", false),
				new("dose", "<positive number|none>", "inhaled drug dose per unit volume; none uses the gas default", "the gas default"),
				new("duration", "<positive timespan|none>", "cloud duration; none uses 30 seconds", "00:00:30"),
				new("cloudecho", "<room text|none>", "room text shown when the cloud is created", "A cloud of gas billows out.")
			],
			[TrapPayloadType.Restraint] =
			[
				new("duration", "<positive timespan|none>", "restraint duration; none uses 30 seconds", "00:00:30"),
				new("description", "<description|none>", "description used for the target's restraint", "caught by a trap")
			]
		};

	private static readonly IReadOnlySet<TrapSourceKind> AllDomains = new HashSet<TrapSourceKind>(Enum.GetValues<TrapSourceKind>());
	private static readonly IReadOnlySet<TrapSourceKind> MechanicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Mechanical };
	private static readonly IReadOnlySet<TrapSourceKind> MagicalOnly = new HashSet<TrapSourceKind> { TrapSourceKind.Magical };
	private static readonly IReadOnlySet<TrapSourceKind> NoDomains = new HashSet<TrapSourceKind>();
	private static readonly IReadOnlyDictionary<TrapPayloadType, IReadOnlySet<TrapSourceKind>> Compatibility =
		new Dictionary<TrapPayloadType, IReadOnlySet<TrapSourceKind>>
		{
			[TrapPayloadType.DetonateItem] = MechanicalOnly,
			[TrapPayloadType.CastSpell] = MagicalOnly,
			[TrapPayloadType.EmitSignal] = MechanicalOnly,
			[TrapPayloadType.ExecuteProg] = AllDomains,
			[TrapPayloadType.DirectDamage] = AllDomains,
			[TrapPayloadType.ExplosiveDamage] = AllDomains,
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
	public IReadOnlySet<TrapSourceKind> CompatibleSourceKinds => Compatibility.GetValueOrDefault(PayloadType) ?? NoDomains;
	public TimeSpan Delay { get; private set; }
	public TrapTargetSelector TargetSelector { get; private set; }
	private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);
	public IReadOnlyDictionary<string, string> Parameters => _parameters;

	public static IEnumerable<ParameterHelp> ParametersFor(TrapPayloadType type) =>
		CommonParameters.Concat(TypeParameters.GetValueOrDefault(type) ?? []);

	public static bool IsSupportedParameter(TrapPayloadType type, string name) =>
		ParametersFor(type).Any(x => x.Name.EqualTo(name));

	public static ParameterHelp? ParameterFor(TrapPayloadType type, string name) =>
		ParametersFor(type).FirstOrDefault(x => x.Name.EqualTo(name));

	public static bool TryValidateParameter(TrapPayloadType type, string name, string value, out string error)
	{
		var parameter = ParameterFor(type, name);
		if (parameter is null)
		{
			error = $"{name} is not supported by {type.DescribeEnum()} payloads.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(value))
		{
			error = $"The {parameter.Name} parameter requires {parameter.Syntax}.";
			return false;
		}

		if (parameter.Optional && value.EqualTo("none"))
		{
			error = string.Empty;
			return true;
		}

		var collapsedName = parameter.Name.CollapseString();
		if (collapsedName is "damage" or "pain" or "stun")
		{
			return TrapParameterValidation.TryValidateDamageExpression(collapsedName, value, out error);
		}

		var valid = collapsedName switch
		{
			"spell" or "prog" or "targetitem" or "liquid" or "gas" => TrapParameterValidation.TryParsePositiveLong(value, out _),
			"power" => TrapParameterValidation.TryParseDefinedEnum<SpellPower>(value, out _),
			"value" => TrapParameterValidation.TryParseFiniteDouble(value, out _),
			"amount" or "dose" => TrapParameterValidation.TryParseFiniteDouble(value, out var number) && number > 0.0,
			"damagetype" => TrapParameterValidation.TryParseDefinedEnum<DamageType>(value, out _),
			"explosionsize" => TrapParameterValidation.TryParseDefinedEnum<SizeCategory>(value, out _),
			"maximumproximity" => TrapParameterValidation.TryParseExplosionMaximumProximity(value, out _),
			"elevation" => TrapParameterValidation.TryParseFiniteDouble(value, out _),
			"duration" => TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var duration) && duration > TimeSpan.Zero,
			_ => true
		};
		if (valid)
		{
			error = string.Empty;
			return true;
		}

		error = $"The {parameter.Name} parameter requires {parameter.Syntax}.";
		return false;
	}

	public static bool TryValidateParameters(TrapPayloadType type, IReadOnlyDictionary<string, string> parameters,
		out string error)
	{
		if (!Compatibility.ContainsKey(type))
		{
			error = $"{type} is not a supported trap payload type.";
			return false;
		}

		foreach (var parameter in parameters)
		{
			if (!TryValidateParameter(type, parameter.Key, parameter.Value, out error))
			{
				return false;
			}
		}

		error = string.Empty;
		return true;
	}

	public bool SetParameter(string name, string value)
	{
		if (ParameterFor(PayloadType, name)?.Optional == true && value.EqualTo("none"))
		{
			_parameters.Remove(name);
			return true;
		}

		if (!TryValidateParameter(PayloadType, name, value, out _))
		{
			return false;
		}

		_parameters[name] = value;
		return true;
	}

	public void SetDelay(TimeSpan delay) => Delay = delay;
	public void SetTargetSelector(TrapTargetSelector selector) => TargetSelector = selector;

	public static TrapPayloadDefinition LoadFromXml(XElement root)
	{
		if (!Enum.TryParse(root.Attribute("type")?.Value, true, out TrapPayloadType payloadType))
		{
			throw new ApplicationException($"Unknown trap payload type '{root.Attribute("type")?.Value}'.");
		}

		var delayText = root.Attribute("delay")?.Value;
		var delay = string.IsNullOrWhiteSpace(delayText)
			? TimeSpan.Zero
			: TimeSpan.TryParse(delayText, out var parsedDelay)
				? parsedDelay
				: TimeSpan.MinValue;
		var targetText = root.Attribute("target")?.Value;
		var targetSelector = string.IsNullOrWhiteSpace(targetText)
			? TrapTargetSelector.Triggerer
			: TrapParameterValidation.TryParseDefinedEnum(targetText, out TrapTargetSelector parsedSelector)
				? parsedSelector
				: (TrapTargetSelector)(-1);
		var result = new TrapPayloadDefinition(payloadType, delay, targetSelector);
		foreach (var parameter in root.Elements("Parameter"))
		{
			var name = parameter.Attribute("name")?.Value;
			if (!string.IsNullOrWhiteSpace(name))
			{
				if (!result.SetParameter(name, parameter.Value))
				{
					result._parameters[name] = parameter.Value;
				}
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

internal static class TrapParameterValidation
{
	private static readonly IReadOnlySet<string> DamageFormulaParameters =
		new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quality", "power" };

	private static readonly IReadOnlySet<string> PainAndStunFormulaParameters =
		new HashSet<string>(DamageFormulaParameters, StringComparer.OrdinalIgnoreCase) { "damage" };

	public static bool TryParseFiniteDouble(string value, out double number)
	{
		return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number) &&
		       double.IsFinite(number);
	}

	public static bool TryParsePositiveLong(string value, out long number)
	{
		return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number) && number > 0L;
	}

	public static bool TryParseDefinedEnum<TEnum>(string value, out TEnum parsed)
		where TEnum : struct, Enum
	{
		return Enum.TryParse(value, true, out parsed) && Enum.IsDefined(parsed);
	}

	public static bool TryParseExplosionMaximumProximity(string value, out Proximity proximity)
	{
		return TryParseDefinedEnum(value, out proximity) && proximity != Proximity.Unapproximable;
	}

	public static bool TryValidateDamageExpression(string parameterName, string value, out string error)
	{
		var expression = new Expression(value);
		if (expression.HasErrors())
		{
			error = $"The {parameterName} expression is invalid: {expression.Error}";
			return false;
		}

		var allowedParameters = parameterName is "pain" or "stun"
			? PainAndStunFormulaParameters
			: DamageFormulaParameters;
		var unsupportedParameters = expression.ParameterNames
			.Where(x => !allowedParameters.Contains(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		if (unsupportedParameters.Any())
		{
			error = $"The {parameterName} expression may only use {allowedParameters.OrderBy(x => x).ListToString()}; it also uses {unsupportedParameters.ListToString()}.";
			return false;
		}

		if (!TryEvaluateDamageExpression(value, (int)ItemQuality.Standard, SpellPower.Standard, 0.0,
			out _, out var evaluationError))
		{
			error = $"The {parameterName} expression is invalid: {evaluationError}";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public static bool TryEvaluateDamageExpression(string value, double quality, SpellPower power, double damage,
		out double result, out string error)
	{
		result = 0.0;
		error = string.Empty;
		var expression = new Expression(value);
		if (expression.HasErrors())
		{
			error = expression.Error;
			return false;
		}

		string? expressionError = null;
		EventHandler<string> errorHandler = (sender, message) =>
		{
			if (ReferenceEquals(sender, expression))
			{
				expressionError = message;
			}
		};
		Expression.ExpressionError += errorHandler;
		try
		{
			result = expression.EvaluateDoubleWith(("quality", quality), ("power", (int)power), ("damage", damage));
		}
		catch (Exception ex) when (ex is ArgumentException or OverflowException or InvalidOperationException or FormatException)
		{
			error = ex.Message;
			return false;
		}
		finally
		{
			Expression.ExpressionError -= errorHandler;
		}

		if (expressionError is not null)
		{
			error = expressionError;
			return false;
		}

		if (!double.IsFinite(result) || result < 0.0)
		{
			error = "it must evaluate to a finite, non-negative number";
			return false;
		}

		return true;
	}

	public static bool TryParseMovementTypes(string text)
	{
		var movementTypes = MovementType.None;
		foreach (var value in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (!TryParseDefinedEnum<MovementType>(value, out var parsed) || parsed == MovementType.None)
			{
				return false;
			}

			movementTypes |= parsed;
		}

		return movementTypes != MovementType.None;
	}
}
