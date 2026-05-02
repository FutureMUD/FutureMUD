#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

internal static class WindSpellEffectTemplateHelper
{
	public static readonly string[] PerceivableTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsPerceivableTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static readonly string[] CharacterExitTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "character&exit")
		.ToArray();

	public static readonly string[] SingleCharacterTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "character")
		.ToArray();

	public static bool IsPerceivableTarget(string types)
	{
		return types is "character" or "characters" or "item" or "items" or "perceivable" or "perceivables";
	}
}

public class LevitationEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("levitate", (root, spell) => new LevitationEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("levitate", BuilderFactory,
			"Suspends a character or item and prevents falling",
			HelpText,
			false,
			true,
			WindSpellEffectTemplateHelper.PerceivableTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new LevitationEffect(new XElement("Effect",
			new XAttribute("type", "levitate"),
			new XElement("PreserveLayer", true),
			new XElement("TargetLayer", (int)RoomLayer.InAir),
			new XElement("SDescAddendum", new XCData("(levitating)")),
			new XElement("DescAddendum", new XCData("@ appears to be suspended in the air.")),
			new XElement("AddendumColour", "bold cyan")
		), spell), string.Empty);
	}

	protected LevitationEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		PreserveLayer = bool.Parse(root.Element("PreserveLayer")?.Value ?? "true");
		TargetLayer = (RoomLayer)int.Parse(root.Element("TargetLayer")?.Value ?? ((int)RoomLayer.InAir).ToString());
		SDescAddendum = root.Element("SDescAddendum")?.Value ?? "(levitating)";
		DescAddendum = root.Element("DescAddendum")?.Value ?? "@ appears to be suspended in the air.";
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool PreserveLayer { get; private set; }
	public RoomLayer TargetLayer { get; private set; }
	public string SDescAddendum { get; private set; }
	public string DescAddendum { get; private set; }
	public ANSIColour AddendumColour { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "levitate"),
			new XElement("PreserveLayer", PreserveLayer),
			new XElement("TargetLayer", (int)TargetLayer),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return WindSpellEffectTemplateHelper.IsPerceivableTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is ICharacter or IGameItem
			? new SpellLevitationEffect(target, parent, null, PreserveLayer, TargetLayer, SDescAddendum, DescAddendum,
				AddendumColour)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new LevitationEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "layer":
				return BuildingCommandLayer(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandLayer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify either #3same#0 or a room layer.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("same"))
		{
			PreserveLayer = true;
			Spell.Changed = true;
			actor.OutputHandler.Send("This levitation effect will preserve the target's current room layer.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out RoomLayer layer))
		{
			actor.OutputHandler.Send($"Valid room layers are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		PreserveLayer = false;
		TargetLayer = layer;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This levitation effect will move targets {layer.LocativeDescription().ColourName()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short-description addendum should levitating targets have?");
			return false;
		}

		SDescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The levitation short-description addendum is now {SDescAddendum.Colour(AddendumColour)}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What full-description addendum should levitating targets have?");
			return false;
		}

		DescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send("The levitation full-description addendum has been updated.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Colours: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"Invalid colour. Options: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		AddendumColour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Levitation addenda will now use {colour.Name.Colour(colour)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return PreserveLayer
			? $"Levitate - current layer - {SDescAddendum.Colour(AddendumColour)}"
			: $"Levitate - {TargetLayer.DescribeEnum().ColourName()} - {SDescAddendum.Colour(AddendumColour)}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3layer same#0 - preserves the target's current room layer
	#3layer <which>#0 - moves the target to a room layer when applied
	#3sdesc <text>#0 - sets the short-description addendum
	#3desc <text>#0 - sets the full-description addendum
	#3colour <colour>#0 - sets the addendum colour";
}

public class FeatherFallEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("featherfall", (root, spell) => new FeatherFallEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("featherfall", BuilderFactory,
			"Reduces falling distance and damage for a character or item",
			HelpText,
			false,
			true,
			WindSpellEffectTemplateHelper.PerceivableTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new FeatherFallEffect(new XElement("Effect",
			new XAttribute("type", "featherfall"),
			new XElement("FallDistanceMultiplier", 0.0),
			new XElement("FallDamageMultiplier", 0.0),
			new XElement("SDescAddendum", new XCData("(falling lightly)")),
			new XElement("DescAddendum", new XCData("@ seems to drift lightly through the air.")),
			new XElement("AddendumColour", "bold cyan")
		), spell), string.Empty);
	}

	protected FeatherFallEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FallDistanceMultiplier = Math.Max(0.0, double.Parse(root.Element("FallDistanceMultiplier")?.Value ?? "0"));
		FallDamageMultiplier = Math.Max(0.0, double.Parse(root.Element("FallDamageMultiplier")?.Value ?? "0"));
		SDescAddendum = root.Element("SDescAddendum")?.Value ?? "(falling lightly)";
		DescAddendum = root.Element("DescAddendum")?.Value ?? "@ seems to drift lightly through the air.";
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public double FallDistanceMultiplier { get; private set; }
	public double FallDamageMultiplier { get; private set; }
	public string SDescAddendum { get; private set; }
	public string DescAddendum { get; private set; }
	public ANSIColour AddendumColour { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "featherfall"),
			new XElement("FallDistanceMultiplier", FallDistanceMultiplier),
			new XElement("FallDamageMultiplier", FallDamageMultiplier),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return WindSpellEffectTemplateHelper.IsPerceivableTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is ICharacter or IGameItem
			? new SpellFeatherFallEffect(target, parent, null, FallDistanceMultiplier, FallDamageMultiplier,
				SDescAddendum, DescAddendum, AddendumColour)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new FeatherFallEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "distance":
			case "dist":
				return BuildingCommandDistance(actor, command);
			case "damage":
			case "dmg":
				return BuildingCommandDamage(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDistance(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a non-negative distance multiplier.");
			return false;
		}

		FallDistanceMultiplier = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Effective fall distance is now multiplied by {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a non-negative damage multiplier.");
			return false;
		}

		FallDamageMultiplier = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Fall damage is now multiplied by {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short-description addendum should feather-falling targets have?");
			return false;
		}

		SDescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The feather fall short-description addendum is now {SDescAddendum.Colour(AddendumColour)}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What full-description addendum should feather-falling targets have?");
			return false;
		}

		DescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send("The feather fall full-description addendum has been updated.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Colours: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"Invalid colour. Options: {Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		AddendumColour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Feather fall addenda will now use {colour.Name.Colour(colour)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"Feather Fall - distance x{FallDistanceMultiplier.ToString("N2", actor).ColourValue()} damage x{FallDamageMultiplier.ToString("N2", actor).ColourValue()} - {SDescAddendum.Colour(AddendumColour)}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3distance <multiplier>#0 - sets the effective fall distance multiplier
	#3damage <multiplier>#0 - sets the fall damage multiplier
	#3sdesc <text>#0 - sets the short-description addendum
	#3desc <text>#0 - sets the full-description addendum
	#3colour <colour>#0 - sets the addendum colour";
}

public class RemoveInvisibilityEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeinvisibility", (root, spell) => new RemoveInvisibilityEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("dispelinvisibility", (root, spell) => new RemoveInvisibilityEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removeinvisibility", BuilderFactory,
			"Removes magical invisibility from a target",
			HelpText,
			true,
			true,
			WindSpellEffectTemplateHelper.PerceivableTriggerTypes);
		SpellEffectFactory.RegisterBuilderFactory("dispelinvisibility", BuilderFactory,
			"Removes magical invisibility from a target",
			HelpText,
			true,
			true,
			WindSpellEffectTemplateHelper.PerceivableTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RemoveInvisibilityEffect(new XElement("Effect", new XAttribute("type", "removeinvisibility")),
			spell), string.Empty);
	}

	protected RemoveInvisibilityEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "removeinvisibility"));
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return WindSpellEffectTemplateHelper.IsPerceivableTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		target?.RemoveAllEffects<SpellInvisibilityEffect>(null, true);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RemoveInvisibilityEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return "Remove Invisibility";
	}

	public const string HelpText = "This effect has no additional builder options.";
}

public class ForcedPathMovementEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("forcedpathmovement", (root, spell) => new ForcedPathMovementEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("handsofwind", (root, spell) => new ForcedPathMovementEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("forcedpathmovement", BuilderFactory,
			"Forces a character through one or more exits in the same direction",
			HelpText,
			true,
			true,
			WindSpellEffectTemplateHelper.CharacterExitTriggerTypes);
		SpellEffectFactory.RegisterBuilderFactory("handsofwind", BuilderFactory,
			"Forces a character through one or more exits in the same direction",
			HelpText,
			true,
			true,
			WindSpellEffectTemplateHelper.CharacterExitTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new ForcedPathMovementEffect(new XElement("Effect",
			new XAttribute("type", "forcedpathmovement"),
			new XElement("Steps", 1),
			new XElement("AllowFallExits", false)
		), spell), string.Empty);
	}

	protected ForcedPathMovementEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Steps = Math.Max(1, int.Parse(root.Element("Steps")?.Value ?? "1"));
		AllowFallExits = bool.Parse(root.Element("AllowFallExits")?.Value ?? "false");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public int Steps { get; private set; }
	public bool AllowFallExits { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "forcedpathmovement"),
			new XElement("Steps", Steps),
			new XElement("AllowFallExits", AllowFallExits)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return trigger.TargetTypes == "character&exit";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter characterTarget)
		{
			return null;
		}

		var exit = additionalParameters
		           .FirstOrDefault(x => x.ParameterName.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
		           ?.Item as ICellExit;
		if (exit is null)
		{
			return null;
		}

		var direction = exit.OutboundDirection;
		for (var i = 0; i < Steps; i++)
		{
			if (!TryMoveTarget(caster, characterTarget, exit, out var why))
			{
				if (!string.IsNullOrWhiteSpace(why))
				{
					characterTarget.OutputHandler.Send(why);
				}

				break;
			}

			if (direction == CardinalDirection.Unknown)
			{
				break;
			}

			exit = characterTarget.Location.ExitsFor(characterTarget)
			                      .FirstOrDefault(x =>
				                      x.OutboundDirection == direction && (AllowFallExits || !x.IsFallExit));
			if (exit is null)
			{
				break;
			}
		}

		return null;
	}

	private bool TryMoveTarget(ICharacter caster, ICharacter target, ICellExit exit, out string why)
	{
		why = string.Empty;
		if (!AllowFallExits && exit.IsFallExit)
		{
			why = "The wind cannot force you over that fall exit.";
			return false;
		}

		if (exit.Origin != target.Location)
		{
			why = "That exit is not present where the target is.";
			return false;
		}

		if (!exit.WhichLayersExitAppears().Contains(target.RoomLayer))
		{
			why = "That exit is not accessible from the target's current layer.";
			return false;
		}

		var flags = CanMoveFlags.IgnoreWhetherExitCanBeCrossed |
		            CanMoveFlags.IgnoreCancellableActionBlockers |
		            CanMoveFlags.IgnoreSafeMovement;
		var canMove = target.CanMove(exit, flags);
		if (!canMove)
		{
			why = canMove.ErrorMessage;
			return false;
		}

		var canCross = target.CanCross(exit);
		if (!canCross.Success)
		{
			if (canCross.FailureOutput is not null)
			{
				target.OutputHandler.Handle(canCross.FailureOutput);
			}

			return false;
		}

		var transition = exit.MovementTransition(target);
		if (transition.TransitionType == CellMovementTransition.NoViableTransition)
		{
			why = "The target cannot be forced through that exit from this layer.";
			return false;
		}

		target.Teleport(exit.Destination, transition.TargetLayer, false, true,
			"@ is swept away by a sudden rush of wind.",
			"@ is swept in by a sudden rush of wind.",
			"A sudden rush of wind sweeps you away.");
		ZeroGravityMovementHelper.EnsureFloating(target);
		if (target.ShouldFall())
		{
			target.FallToGround();
		}

		return true;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ForcedPathMovementEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "steps":
			case "rooms":
				return BuildingCommandSteps(actor, command);
			case "fall":
			case "fallexits":
				AllowFallExits = !AllowFallExits;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {AllowFallExits.NowNoLonger()} force targets over fall exits.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandSteps(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a positive number of steps.");
			return false;
		}

		Steps = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will force targets up to {Steps.ToString("N0", actor).ColourValue()} steps.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"Forced Path Movement - {Steps.ToString("N0", actor).ColourValue()} steps - Fall Exits {AllowFallExits.ToColouredString()}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3steps <number>#0 - sets the maximum number of exits the target is forced through
	#3fall#0 - toggles whether fall exits can be used";
}

public class TransferenceEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("transference", (root, spell) => new TransferenceEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("transference", BuilderFactory,
			"Swaps the caster's location with the target character",
			HelpText,
			true,
			true,
			WindSpellEffectTemplateHelper.SingleCharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new TransferenceEffect(new XElement("Effect",
			new XAttribute("type", "transference"),
			new XElement("IncludeFollowers", false),
			new XElement("SwapLayers", true)
		), spell), string.Empty);
	}

	protected TransferenceEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		IncludeFollowers = bool.Parse(root.Element("IncludeFollowers")?.Value ?? "false");
		SwapLayers = bool.Parse(root.Element("SwapLayers")?.Value ?? "true");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IncludeFollowers { get; private set; }
	public bool SwapLayers { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "transference"),
			new XElement("IncludeFollowers", IncludeFollowers),
			new XElement("SwapLayers", SwapLayers)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return trigger.TargetTypes == "character";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter characterTarget || characterTarget == caster ||
		    caster.Location is null || characterTarget.Location is null)
		{
			return null;
		}

		var casterLocation = caster.Location;
		var casterLayer = caster.RoomLayer;
		var targetLocation = characterTarget.Location;
		var targetLayer = characterTarget.RoomLayer;
		if (casterLocation == targetLocation && casterLayer == targetLayer)
		{
			return null;
		}

		caster.Teleport(targetLocation, SwapLayers ? targetLayer : casterLayer, IncludeFollowers, true,
			"@ vanishes in a rush of spiralling wind.",
			"@ appears in a rush of spiralling wind.",
			"A rush of wind steals you away.");
		characterTarget.Teleport(casterLocation, SwapLayers ? casterLayer : targetLayer, IncludeFollowers, true,
			"@ vanishes in a rush of spiralling wind.",
			"@ appears in a rush of spiralling wind.",
			"A rush of wind steals you away.");
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new TransferenceEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "followers":
			case "party":
				IncludeFollowers = !IncludeFollowers;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This transference will {IncludeFollowers.NowNoLonger()} include followers, dragged targets and riders.");
				return true;
			case "layers":
			case "layer":
				SwapLayers = !SwapLayers;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This transference will {SwapLayers.NowNoLonger()} swap room layers.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return $"Transference - Followers {IncludeFollowers.ToColouredString()} - Swap Layers {SwapLayers.ToColouredString()}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3followers#0 - toggles whether followers, dragged targets and riders come along
	#3layers#0 - toggles whether the caster and target exchange room layers as well as rooms";
}
