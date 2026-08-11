#nullable enable

using MudSharp.Traps;

namespace MudSharp.Traps;

/// <summary>
/// The persisted, builder-configurable implementation shared by the first-party trap modules.
/// Specific runtime handlers consume the named parameters that apply to their module type.
/// </summary>
public sealed class TrapTriggerDefinition : ITrapTrigger
{
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
