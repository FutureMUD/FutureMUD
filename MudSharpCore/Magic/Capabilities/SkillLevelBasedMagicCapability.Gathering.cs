#nullable enable

using System.Globalization;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic.Gathering;

namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private readonly List<MagicGatheringMethodDefinition> _gatheringMethods = [];
	private string? _gatheringConfigurationLoadError;
	private XElement? _unreadableGatheringDefinition;

	public IReadOnlyList<MagicGatheringMethodDefinition> GatheringMethods => _gatheringMethods.AsReadOnly();

	protected void CloneGatheringMethodsFrom(SkillLevelBasedMagicCapability source)
	{
		_gatheringConfigurationLoadError = source._gatheringConfigurationLoadError;
		_unreadableGatheringDefinition = source._unreadableGatheringDefinition is null
			? null
			: new XElement(source._unreadableGatheringDefinition);
		_gatheringMethods.AddRange(source._gatheringMethods.Select(x => x with { Key = Guid.NewGuid() }));
	}

	protected void LoadGatheringDefinition(XElement root)
	{
		_gatheringMethods.Clear();
		_gatheringConfigurationLoadError = null;
		_unreadableGatheringDefinition = null;
		XElement? gathering = root.Element("Gathering");
		if (gathering is null)
		{
			return;
		}

		try
		{
			if ((int?)gathering.Attribute("version") is not (null or 1))
			{
				throw new FormatException("Unsupported gathering definition version.");
			}

			foreach (XElement method in gathering.Elements("Method"))
			{
				_gatheringMethods.Add(new MagicGatheringMethodDefinition(
					Guid.Parse(Required(method, "key")),
					Required(method, "alias"),
					Required(method, "name"),
					Enum.Parse<MagicGatheringMethodKind>(Required(method, "kind"), true),
					Long(method, "destination"),
					NullableLong(method, "source"),
					Number(method, "min"),
					Number(method, "max"),
					Number(method, "duration"),
					Number(method, "ratio", 1.0),
					Number(method, "stamina", 0.0),
					Number(method, "minimumStamina", 0.0),
					Number(method, "damage", 0.0),
					Number(method, "pain", 0.0),
					Number(method, "stun", 0.0),
					Enum.Parse<WoundSeverity>(method.Attribute("maximumHealthSeverity")?.Value ?? nameof(WoundSeverity.None), true),
					Long(method, "permission", 0),
					Long(method, "durationProg", 0),
					Long(method, "staminaProg", 0),
					Long(method, "damageProg", 0),
					Long(method, "painProg", 0),
					Long(method, "stunProg", 0),
					Long(method, "onGathered", 0),
					(int)Long(method, "structuralVersion", 1)));
			}
		}
		catch (Exception ex)
		{
			_gatheringMethods.Clear();
			_gatheringConfigurationLoadError = $"Gathering configuration: {ex.Message}";
			_unreadableGatheringDefinition = new XElement(gathering);
		}
	}

	protected void SaveGatheringDefinition(XElement root)
	{
		if (_gatheringConfigurationLoadError is not null && _unreadableGatheringDefinition is not null)
		{
			// Preserve malformed authored XML until it is explicitly repaired instead of converting it into free gathering.
			root.Add(new XElement(_unreadableGatheringDefinition));
			return;
		}

		if (_gatheringMethods.Count == 0)
		{
			return;
		}

		root.Add(new XElement("Gathering", new XAttribute("version", 1),
			_gatheringMethods.OrderBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Select(SaveGatheringMethod)));
	}

	protected static void RemapGatheringKeysForClone(XElement root)
	{
		XElement? gathering = root.Element("Gathering");
		if (gathering is null)
		{
			return;
		}

		Dictionary<Guid, Guid> keys = gathering.Elements("Method")
			.Select(x => Guid.Parse(Required(x, "key")))
			.ToDictionary(x => x, _ => Guid.NewGuid());
		foreach (XElement method in gathering.Elements("Method"))
		{
			XAttribute? key = method.Attribute("key");
			if (key is not null)
			{
				key.Value = keys[Guid.Parse(key.Value)].ToString();
			}
		}
	}

	public IReadOnlyList<string> GatheringConfigurationErrors()
	{
		List<string> errors = [];
		if (_gatheringConfigurationLoadError is not null)
		{
			errors.Add(_gatheringConfigurationLoadError);
			return errors.AsReadOnly();
		}

		if (_gatheringMethods.Count > 64)
		{
			errors.Add("At most 64 gathering methods are supported per capability.");
		}

		if (_gatheringMethods.GroupBy(x => x.Key).Any(x => x.Key == Guid.Empty || x.Count() > 1))
		{
			errors.Add("Gathering method keys must be present and unique.");
		}

		if (_gatheringMethods.GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Count() > 1))
		{
			errors.Add("Gathering method aliases must be non-empty and unique.");
		}

		foreach (MagicGatheringMethodDefinition method in _gatheringMethods)
		{
			errors.AddRange(MagicGatheringPolicy.ConfigurationErrors(Gameworld, method));
		}

		return errors.AsReadOnly();
	}

	private static XElement SaveGatheringMethod(MagicGatheringMethodDefinition method) => new("Method",
		new XAttribute("key", method.Key),
		new XAttribute("alias", method.Alias),
		new XAttribute("name", method.Name),
		new XAttribute("kind", method.Kind),
		new XAttribute("destination", method.DestinationResourceId),
		new XAttribute("source", method.SourceResourceId ?? 0),
		new XAttribute("min", NumberText(method.MinimumAmount)),
		new XAttribute("max", NumberText(method.MaximumAmount)),
		new XAttribute("duration", NumberText(method.DurationSeconds)),
		new XAttribute("ratio", NumberText(method.SourceUnitsPerDestinationUnit)),
		new XAttribute("stamina", NumberText(method.StaminaCost)),
		new XAttribute("minimumStamina", NumberText(method.MinimumStamina)),
		new XAttribute("damage", NumberText(method.DamageCost)),
		new XAttribute("pain", NumberText(method.PainCost)),
		new XAttribute("stun", NumberText(method.StunCost)),
		new XAttribute("maximumHealthSeverity", method.MaximumHealthSeverity),
		new XAttribute("permission", method.PermissionProgId),
		new XAttribute("durationProg", method.DurationProgId),
		new XAttribute("staminaProg", method.StaminaCostProgId),
		new XAttribute("damageProg", method.DamageCostProgId),
		new XAttribute("painProg", method.PainCostProgId),
		new XAttribute("stunProg", method.StunCostProgId),
		new XAttribute("onGathered", method.OnGatheredProgId),
		new XAttribute("structuralVersion", method.StructuralVersion));

	private static string Required(XElement element, string attribute) =>
		element.Attribute(attribute)?.Value ?? throw new FormatException($"Method is missing the {attribute} attribute.");

	private static long Long(XElement element, string attribute, long defaultValue = 0) =>
		long.Parse(element.Attribute(attribute)?.Value ?? defaultValue.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

	private static long? NullableLong(XElement element, string attribute)
	{
		long value = Long(element, attribute, 0);
		return value > 0 ? value : null;
	}

	private static double Number(XElement element, string attribute, double? defaultValue = null)
	{
		string? text = element.Attribute(attribute)?.Value;
		if (text is null && !defaultValue.HasValue)
		{
			throw new FormatException($"Method is missing the {attribute} attribute.");
		}

		return double.Parse(text ?? NumberText(defaultValue!.Value), CultureInfo.InvariantCulture);
	}

	private static string NumberText(double value) => value.ToString("R", CultureInfo.InvariantCulture);

	private bool BuildingCommandGathering(ICharacter actor, StringStack command)
	{
		string verb = command.PopForSwitch();
		switch (verb)
		{
			case "list":
				actor.OutputHandler.Send(GatheringListText(actor));
				return true;
			case "add":
				return BuildingCommandGatheringAdd(actor, command);
			case "remove":
				return BuildingCommandGatheringRemove(actor, command);
			case "show":
				return BuildingCommandGatheringShow(actor, command);
			case "set":
				return BuildingCommandGatheringSet(actor, command);
			default:
				actor.OutputHandler.Send(GatheringHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandGatheringAdd(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().TryParseEnum<MagicGatheringMethodKind>(out MagicGatheringMethodKind kind))
		{
			actor.OutputHandler.Send("Use gather add self|gentle <alias> <presentation name>.");
			return false;
		}

		string alias = command.PopSpeech();
		string name = command.SafeRemainingArgument;
		if (!ValidAlias(alias) || string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("Specify a unique single-word alias and a presentation name.");
			return false;
		}

		if (_gatheringMethods.Any(x => x.Alias.EqualTo(alias)))
		{
			actor.OutputHandler.Send("That gathering alias is already in use by this capability.");
			return false;
		}

		_gatheringMethods.Add(new MagicGatheringMethodDefinition(Guid.NewGuid(), alias, name, kind, 0,
			null, 1.0, 1.0, 10.0, StaminaCost: kind == MagicGatheringMethodKind.Self ? 1.0 : 0.0));
		Changed = true;
		actor.OutputHandler.Send($"Added the {name.ColourName()} gathering method. Set its destination resource before it can be used.");
		return true;
	}

	private bool BuildingCommandGatheringRemove(ICharacter actor, StringStack command)
	{
		MagicGatheringMethodDefinition? method = FindGatheringMethod(command.PopSpeech());
		if (method is null)
		{
			actor.OutputHandler.Send("There is no such gathering method.");
			return false;
		}

		_gatheringMethods.Remove(method);
		Changed = true;
		actor.OutputHandler.Send($"Removed the {method.Name.ColourName()} gathering method. Any live precommit action using it will cancel at validation.");
		return true;
	}

	private bool BuildingCommandGatheringShow(ICharacter actor, StringStack command)
	{
		MagicGatheringMethodDefinition? method = FindGatheringMethod(command.PopSpeech());
		if (method is null)
		{
			actor.OutputHandler.Send("There is no such gathering method.");
			return false;
		}

		actor.OutputHandler.Send(GatheringMethodText(actor, method));
		return true;
	}

	private bool BuildingCommandGatheringSet(ICharacter actor, StringStack command)
	{
		MagicGatheringMethodDefinition? method = FindGatheringMethod(command.PopSpeech());
		string setting = command.PopForSwitch();
		if (method is null || string.IsNullOrEmpty(setting))
		{
			actor.OutputHandler.Send("Use gather set <method> <setting> <value>.");
			return false;
		}

		MagicGatheringMethodDefinition replacement;
		bool structural = true;
		try
		{
			switch (setting)
			{
				case "alias":
					string alias = command.SafeRemainingArgument;
					if (!ValidAlias(alias) || _gatheringMethods.Any(x => x.Key != method.Key && x.Alias.EqualTo(alias))) throw new InvalidOperationException("Aliases must be unique single words.");
					replacement = method with { Alias = alias };
					structural = false;
					break;
				case "name":
					string name = command.SafeRemainingArgument;
					if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Specify a presentation name.");
					replacement = method with { Name = name };
					structural = false;
					break;
				case "destination":
					replacement = method with { DestinationResourceId = ResourceId(command) };
					break;
				case "source":
					replacement = method with { SourceResourceId = ResourceId(command) };
					break;
				case "min":
					replacement = method with { MinimumAmount = PositiveNumber(command, "minimum amount") };
					break;
				case "max":
					replacement = method with { MaximumAmount = PositiveNumber(command, "maximum amount") };
					break;
				case "duration":
					replacement = method with { DurationSeconds = PositiveNumber(command, "duration") };
					break;
				case "ratio":
					replacement = method with { SourceUnitsPerDestinationUnit = PositiveNumber(command, "source ratio") };
					break;
				case "stamina":
					replacement = method with { StaminaCost = NonNegativeNumber(command, "stamina cost") };
					break;
				case "minimumstamina":
				case "minstamina":
					replacement = method with { MinimumStamina = NonNegativeNumber(command, "minimum stamina") };
					break;
				case "damage":
					replacement = method with { DamageCost = NonNegativeNumber(command, "damage cost") };
					break;
				case "pain":
					replacement = method with { PainCost = NonNegativeNumber(command, "pain cost") };
					break;
				case "stun":
					replacement = method with { StunCost = NonNegativeNumber(command, "stun cost") };
					break;
				case "maximumhealthseverity":
				case "healthseverity":
					if (!command.SafeRemainingArgument.TryParseEnum<WoundSeverity>(out WoundSeverity severity)) throw new InvalidOperationException("Specify a valid wound severity.");
					replacement = method with { MaximumHealthSeverity = severity };
					break;
				case "permission":
					replacement = method with { PermissionProgId = ProgId(command, "permission") };
					break;
				case "durationprog":
					replacement = method with { DurationProgId = ProgId(command, "duration") };
					break;
				case "staminaprog":
					replacement = method with { StaminaCostProgId = ProgId(command, "stamina cost") };
					break;
				case "damageprog":
					replacement = method with { DamageCostProgId = ProgId(command, "damage cost") };
					break;
				case "painprog":
					replacement = method with { PainCostProgId = ProgId(command, "pain cost") };
					break;
				case "stunprog":
					replacement = method with { StunCostProgId = ProgId(command, "stun cost") };
					break;
				case "onsuccess":
				case "ongathered":
					replacement = method with { OnGatheredProgId = ProgId(command, "post-gather") };
					break;
				default:
					throw new InvalidOperationException("Unknown gathering setting. See gather help.");
			}
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send(ex.Message);
			return false;
		}

		if (structural)
		{
			replacement = replacement with { StructuralVersion = checked(method.StructuralVersion + 1) };
		}

		ReplaceGatheringMethod(replacement);
		Changed = true;
		actor.OutputHandler.Send($"Updated gathering method {replacement.Name.ColourName()}{(structural ? "; live precommit work using it will revalidate." : ".")}");
		return true;
	}

	private void ReplaceGatheringMethod(MagicGatheringMethodDefinition replacement)
	{
		int index = _gatheringMethods.FindIndex(x => x.Key == replacement.Key);
		if (index >= 0)
		{
			_gatheringMethods[index] = replacement;
		}
	}

	private MagicGatheringMethodDefinition? FindGatheringMethod(string text)
	{
		if (Guid.TryParse(text, out Guid key))
		{
			return _gatheringMethods.FirstOrDefault(x => x.Key == key);
		}

		MagicGatheringMethodDefinition[] matches = _gatheringMethods.Where(x => x.Alias.EqualTo(text) || x.Alias.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToArray();
		return matches.Length == 1 ? matches[0] : null;
	}

	private long ResourceId(StringStack command)
	{
		IMagicResource? resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
		return resource?.Id ?? throw new InvalidOperationException("There is no such magic resource.");
	}

	private long ProgId(StringStack command, string description)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear"))
		{
			return 0;
		}

		IFutureProg? prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			throw new InvalidOperationException($"There is no such {description} prog.");
		}

		return prog.Id;
	}

	private static bool ValidAlias(string alias) => !string.IsNullOrWhiteSpace(alias) && alias.All(x => char.IsLetterOrDigit(x) || x is '-' or '_');

	private static double PositiveNumber(StringStack command, string label)
	{
		double value = ParseFinite(command, label);
		if (value <= 0.0) throw new InvalidOperationException($"The {label} must be finite and greater than zero.");
		return value;
	}

	private static double NonNegativeNumber(StringStack command, string label)
	{
		double value = ParseFinite(command, label);
		if (value < 0.0) throw new InvalidOperationException($"The {label} must be finite and non-negative.");
		return value;
	}

	private static double ParseFinite(StringStack command, string label)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) || !double.IsFinite(value))
		{
			throw new InvalidOperationException($"Specify a finite {label}.");
		}

		return value;
	}

	private void AppendGatheringShow(StringBuilder sb, ICharacter actor)
	{
		sb.AppendLine();
		sb.AppendLine("Gathering Methods:");
		sb.AppendLine();
		if (_gatheringMethods.Count == 0)
		{
			sb.AppendLine("\tNone configured.");
			return;
		}

		foreach (MagicGatheringMethodDefinition method in _gatheringMethods.OrderBy(x => x.Alias))
		{
			sb.AppendLine($"\t{method.Alias.ColourCommand()} ({method.Name.ColourName()}): {method.Kind.DescribeEnum().ColourValue()}, {method.MinimumAmount.ToString("N2", actor)}-{method.MaximumAmount.ToString("N2", actor)}, version {method.StructuralVersion.ToString("N0", actor)}");
		}
		foreach (string error in GatheringConfigurationErrors())
		{
			sb.AppendLine($"\t{error.ColourError()}");
		}
	}

	private string GatheringListText(ICharacter actor)
	{
		StringBuilder sb = new("Gathering Methods".GetLineWithTitle(actor, Telnet.Magenta, Telnet.BoldWhite));
		AppendGatheringShow(sb, actor);
		return sb.ToString();
	}

	private string GatheringMethodText(ICharacter actor, MagicGatheringMethodDefinition method)
	{
		IMagicResource? destination = Gameworld.MagicResources.Get(method.DestinationResourceId);
		IMagicResource? source = method.SourceResourceId is { } id ? Gameworld.MagicResources.Get(id) : null;
		return $"Gathering Method {method.Name}".GetLineWithTitle(actor, Telnet.Magenta, Telnet.BoldWhite) + "\n\n" +
			$"Identity: {method.Key}\nAlias: {method.Alias}\nKind: {method.Kind}\nDestination: {destination?.Name ?? $"missing #{method.DestinationResourceId}"}\n" +
			$"Source: {source?.Name ?? (method.Kind == MagicGatheringMethodKind.Gentle ? "missing" : "not applicable")}\n" +
			$"Amounts: {method.MinimumAmount.ToString("N2", actor)} - {method.MaximumAmount.ToString("N2", actor)}\nDuration: {method.DurationSeconds.ToString("N2", actor)} seconds\n" +
			$"Gentle source units per destination unit: {method.SourceUnitsPerDestinationUnit.ToString("N2", actor)}\n" +
			$"Bodily costs: stamina {method.StaminaCost.ToString("N2", actor)} (minimum remaining {method.MinimumStamina.ToString("N2", actor)}), damage {method.DamageCost.ToString("N2", actor)}, pain {method.PainCost.ToString("N2", actor)}, stun {method.StunCost.ToString("N2", actor)}\n" +
			$"Maximum health severity: {method.MaximumHealthSeverity.DescribeEnum()}\nPermission: {Gameworld.FutureProgs.Get(method.PermissionProgId)?.MXPClickableFunctionName() ?? "none"}\n" +
			$"Duration prog: {Gameworld.FutureProgs.Get(method.DurationProgId)?.MXPClickableFunctionName() ?? "static"}\n" +
			$"On success: {Gameworld.FutureProgs.Get(method.OnGatheredProgId)?.MXPClickableFunctionName() ?? "none"}\n" +
			$"Structural version: {method.StructuralVersion}";
	}

	private const string GatheringHelp = @"Gathering capability configuration:

	#3gather list#0 - lists configured methods
	#3gather add self|gentle <alias> <presentation name>#0 - adds a disabled-by-default method
	#3gather remove <method>#0 - removes a method
	#3gather show <method>#0 - shows one method
	#3gather set <method> destination|source <resource>#0 - chooses the personal destination and Gentle room source
	#3gather set <method> min|max|duration|ratio <number>#0 - amount bounds, action seconds, or source units consumed per destination unit
	#3gather set <method> stamina|minstamina|damage|pain|stun <number>#0 - independent bodily prices
	#3gather set <method> healthseverity <severity>#0 - maximum strategy-derived severity allowed for a health price
	#3gather set <method> permission|durationprog|staminaprog|damageprog|painprog|stunprog|onsuccess <prog|none>#0 - policy hooks
	#3gather set <method> alias|name <text>#0 - presentation-only changes that keep the stable method identity

Gentle's ratio is source units consumed per destination unit credited. Structural settings invalidate live precommit work; aliases and names do not.";
}
