using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class FirearmAttachmentGameItemComponentProto : GameItemComponentProto, IFirearmAttachmentPrototype,
	IGameItemComponentPrototypeRequirementProvider
{
	private readonly List<GameItemComponentPrototypeRequirement> _requiredSiblingComponents = [];

	protected FirearmAttachmentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "FirearmAttachment")
	{
		SlotType = FirearmAttachmentSlotType.Other;
		FormFactors = ["universal"];
		Modifiers = new FirearmAttachmentModifiers();
	}

	protected FirearmAttachmentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "FirearmAttachment";
	public FirearmAttachmentSlotType SlotType { get; private set; }
	public IReadOnlyCollection<string> FormFactors { get; private set; } = [];
	public FirearmAttachmentModifiers Modifiers { get; private set; } = new();
	public string? FireEmote { get; private set; }
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents =>
		_requiredSiblingComponents;

	protected override void LoadFromXml(XElement root)
	{
		SlotType = root.Element("SlotType")?.Value.TryParseEnum<FirearmAttachmentSlotType>(out var slotType) == true
			? slotType
			: FirearmAttachmentSlotType.Other;
		FormFactors = root.Element("FormFactors")?
			.Elements("FormFactor")
			.Select(x => x.Value)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray() ?? ["universal"];
		var modifiers = root.Element("Modifiers");
		Modifiers = new FirearmAttachmentModifiers(
			double.TryParse(modifiers?.Attribute("accuracy")?.Value, out var accuracy) ? accuracy : 0.0,
			double.TryParse(modifiers?.Attribute("aim")?.Value, out var aim) ? aim : 0.0,
			double.TryParse(modifiers?.Attribute("damage")?.Value, out var damage) ? damage : 1.0,
			double.TryParse(modifiers?.Attribute("range")?.Value, out var range) ? range : 1.0,
			double.TryParse(modifiers?.Attribute("recoil")?.Value, out var recoil) ? recoil : 1.0,
			double.TryParse(modifiers?.Attribute("stamina")?.Value, out var stamina) ? stamina : 1.0,
			double.TryParse(modifiers?.Attribute("delay")?.Value, out var delay) ? delay : 1.0,
			double.TryParse(modifiers?.Attribute("aimloss")?.Value, out var aimLoss) ? aimLoss : 1.0,
			int.TryParse(modifiers?.Attribute("loudness")?.Value, out var loudness) ? loudness : 0);
		FireEmote = root.Element("FireEmote")?.Value;
		_requiredSiblingComponents.Clear();
		foreach (var element in root.Element("Requirements")?.Elements("Requirement") ?? [])
		{
			var capability = GameItemComponentPrototypeRequirements.ResolveComponentCapability(
				element.Attribute("capability")?.Value ?? element.Value);
			if (capability is null)
			{
				continue;
			}

			_requiredSiblingComponents.Add(new GameItemComponentPrototypeRequirement(capability,
				element.Attribute("reason")?.Value));
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SlotType", SlotType),
			new XElement("FormFactors", FormFactors.Select(x => new XElement("FormFactor", new XCData(x)))),
			new XElement("Modifiers",
				new XAttribute("accuracy", Modifiers.AccuracyBonus),
				new XAttribute("aim", Modifiers.AimBonus),
				new XAttribute("damage", Modifiers.DamageMultiplier),
				new XAttribute("range", Modifiers.RangeMultiplier),
				new XAttribute("recoil", Modifiers.RecoilMultiplier),
				new XAttribute("stamina", Modifiers.StaminaMultiplier),
				new XAttribute("delay", Modifiers.DelayMultiplier),
				new XAttribute("aimloss", Modifiers.AimLossMultiplier),
				new XAttribute("loudness", Modifiers.LoudnessOffset)),
			new XElement("FireEmote", new XCData(FireEmote ?? string.Empty)),
			new XElement("Requirements",
				_requiredSiblingComponents.Select(x => new XElement("Requirement",
					new XAttribute("capability", x.Capability.FullName ?? x.Capability.Name),
					string.IsNullOrWhiteSpace(x.Reason) ? null : new XAttribute("reason", x.Reason)))))
			.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new FirearmAttachmentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FirearmAttachmentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new FirearmAttachmentGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("firearmattachment", true,
			(gameworld, account) => new FirearmAttachmentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("firearm attachment", false,
			(gameworld, account) => new FirearmAttachmentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("FirearmAttachment",
			(proto, gameworld) => new FirearmAttachmentGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("FirearmAttachment",
			"Makes an item a modular attachment for modern firearms",
			BuildingHelpText);
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3category <category>#0 - sets the attachment slot category
	#3formfactor <name>#0 - toggles a compatible form factor
	#3require add <capability> [reason]#0 - requires a sibling component capability
	#3require remove <capability>#0 - removes a sibling component requirement
	#3require clear#0 - removes all sibling component requirements
	#3accuracy <bonus>#0 - sets the accuracy bonus
	#3aim <bonus>#0 - sets the aiming bonus
	#3damage <multiplier>#0 - sets the damage multiplier
	#3range <multiplier>#0 - sets the range multiplier
	#3recoil <multiplier>#0 - sets the recoil multiplier
	#3stamina <multiplier>#0 - sets the stamina multiplier
	#3delay <multiplier>#0 - sets the firing-delay multiplier
	#3aimloss <multiplier>#0 - sets the post-shot aim-loss multiplier
	#3loudness <offset>#0 - shifts the ammunition audio volume by enum steps
	#3fireemote <emote|none>#0 - optionally overrides the host's firing emote";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		switch (option)
		{
			case "category":
			case "slot":
			case "type":
				return BuildingCommandCategory(actor, command);
			case "formfactor":
			case "form":
			case "factor":
				return BuildingCommandFormFactor(actor, command);
			case "fireemote":
			case "fire":
				return BuildingCommandFireEmote(actor, command);
			case "require":
			case "requires":
			case "requirement":
				return BuildingCommandRequirement(actor, command);
			case "accuracy":
			case "aim":
			case "damage":
			case "range":
			case "recoil":
			case "stamina":
			case "delay":
			case "aimloss":
			case "loudness":
				return BuildingCommandModifier(actor, command, option);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandRequirement(ICharacter actor, StringStack command)
	{
		var action = command.PopForSwitch();
		switch (action)
		{
			case "add":
			{
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"Which item component capability should this attachment require from a sibling component?");
					return false;
				}

				var capabilityText = command.PopSpeech();
				var capability =
					GameItemComponentPrototypeRequirements.ResolveComponentCapability(capabilityText);
				if (capability is null)
				{
					actor.OutputHandler.Send(
						$"{capabilityText.ColourCommand()} is not a recognised item component capability. Use names such as {"MeleeWeapon".ColourCommand()}, {"RangedWeapon".ColourCommand()}, {"ProduceLight".ColourCommand()}, or {"ProducePower".ColourCommand()}.");
					return false;
				}

				if (_requiredSiblingComponents.Any(x => x.Capability == capability))
				{
					actor.OutputHandler.Send(
						$"This attachment already requires a sibling component providing {GameItemComponentPrototypeRequirements.DescribeComponentCapability(capability).ColourName()}.");
					return false;
				}

				var reason = command.IsFinished ? null : command.SafeRemainingArgument;
				_requiredSiblingComponents.Add(new GameItemComponentPrototypeRequirement(capability, reason));
				Changed = true;
				actor.OutputHandler.Send(
					$"This attachment now requires a sibling component providing {GameItemComponentPrototypeRequirements.DescribeComponentCapability(capability).ColourName()}{(string.IsNullOrWhiteSpace(reason) ? "." : $": {reason}")}");
				return true;
			}
			case "remove":
			case "delete":
			{
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which sibling component requirement do you want to remove?");
					return false;
				}

				var capability =
					GameItemComponentPrototypeRequirements.ResolveComponentCapability(command.SafeRemainingArgument);
				var requirement = _requiredSiblingComponents.FirstOrDefault(x => x.Capability == capability);
				if (requirement is null)
				{
					actor.OutputHandler.Send("This attachment has no such sibling component requirement.");
					return false;
				}

				_requiredSiblingComponents.Remove(requirement);
				Changed = true;
				actor.OutputHandler.Send(
					$"This attachment no longer requires a sibling component providing {GameItemComponentPrototypeRequirements.DescribeComponentCapability(requirement.Capability).ColourName()}.");
				return true;
			}
			case "clear":
				_requiredSiblingComponents.Clear();
				Changed = true;
				actor.OutputHandler.Send("This attachment no longer has any sibling component requirements.");
				return true;
			default:
				actor.OutputHandler.Send(
					"Use #3require add <capability> [reason]#0, #3require remove <capability>#0, or #3require clear#0."
						.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCategory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParseEnum<FirearmAttachmentSlotType>(out var value))
		{
			actor.OutputHandler.Send(
				$"You must specify one of {Enum.GetValues<FirearmAttachmentSlotType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		SlotType = value;
		Changed = true;
		actor.OutputHandler.Send($"This is now {SlotType.DescribeEnum().A_An(colour: Telnet.Cyan)} attachment.");
		return true;
	}

	private bool BuildingCommandFormFactor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which form factor do you want to toggle?");
			return false;
		}

		var value = command.SafeRemainingArgument;
		if (FormFactors.Any(x => x.EqualTo(value)))
		{
			FormFactors = FormFactors.Where(x => !x.EqualTo(value)).ToArray();
			actor.OutputHandler.Send($"This attachment no longer supports the {value.ColourValue()} form factor.");
		}
		else
		{
			FormFactors = FormFactors.Append(value).ToArray();
			actor.OutputHandler.Send($"This attachment now supports the {value.ColourValue()} form factor.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandFireEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Specify an emote, or #3none#0 to remove the override.".SubstituteANSIColour());
			return false;
		}

		FireEmote = command.SafeRemainingArgument.EqualTo("none")
			? null
			: command.SafeRemainingArgument.Fullstop();
		Changed = true;
		actor.OutputHandler.Send(FireEmote is null
			? "This attachment no longer overrides the host's firing emote."
			: $"This attachment now uses the following firing emote:\n\n{FireEmote}");
		return true;
	}

	private bool BuildingCommandModifier(ICharacter actor, StringStack command, string option)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must specify a valid number.");
			return false;
		}

		if (option is "damage" or "range" or "recoil" or "stamina" or "delay" or "aimloss" && value < 0.0)
		{
			actor.OutputHandler.Send("Multipliers cannot be negative.");
			return false;
		}

		Modifiers = option switch
		{
			"accuracy" => Modifiers with { AccuracyBonus = value },
			"aim" => Modifiers with { AimBonus = value },
			"damage" => Modifiers with { DamageMultiplier = value },
			"range" => Modifiers with { RangeMultiplier = value },
			"recoil" => Modifiers with { RecoilMultiplier = value },
			"stamina" => Modifiers with { StaminaMultiplier = value },
			"delay" => Modifiers with { DelayMultiplier = value },
			"aimloss" => Modifiers with { AimLossMultiplier = value },
			"loudness" => Modifiers with { LoudnessOffset = (int)Math.Round(value) },
			_ => Modifiers
		};
		Changed = true;
		actor.OutputHandler.Send(
			$"The {option.ColourName()} modifier is now {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Firearm Attachment Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nCategory: {SlotType.DescribeEnum().ColourName()}\nForm Factors: {FormFactors.Select(x => x.ColourValue()).ListToString()}\nRequired Sibling Capabilities: {(_requiredSiblingComponents.Any() ? _requiredSiblingComponents.Select(x => GameItemComponentPrototypeRequirements.DescribeComponentCapability(x.Capability).ColourName()).ListToString() : "None".ColourError())}\nAccuracy: {Modifiers.AccuracyBonus.ToBonusString(actor)}\nAim: {Modifiers.AimBonus.ToBonusString(actor)}\nDamage: {Modifiers.DamageMultiplier.ToString("N2", actor).ColourValue()}\nRange: {Modifiers.RangeMultiplier.ToString("N2", actor).ColourValue()}\nRecoil: {Modifiers.RecoilMultiplier.ToString("N2", actor).ColourValue()}\nStamina: {Modifiers.StaminaMultiplier.ToString("N2", actor).ColourValue()}\nDelay: {Modifiers.DelayMultiplier.ToString("N2", actor).ColourValue()}\nAim Loss: {Modifiers.AimLossMultiplier.ToString("N2", actor).ColourValue()}\nLoudness Offset: {Modifiers.LoudnessOffset.ToBonusString(actor)}\nFire Emote: {FireEmote?.ColourCommand() ?? "None".ColourError()}";
	}
}
