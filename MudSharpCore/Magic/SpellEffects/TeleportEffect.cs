using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class TeleportEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("teleport", (root, spell) => new TeleportEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("teleport", BuilderFactory,
			"Teleports a character and/or their party",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new TeleportEffect(new XElement("Effect",
			new XAttribute("type", "teleport"),
			new XElement("TeleportParty", false),
			new XElement("PreserveLayer", true),
			new XElement("TargetLayer", (int)RoomLayer.GroundLevel)
		), spell), string.Empty);
	}

	protected TeleportEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		TeleportParty = bool.Parse(root.Element("TeleportParty").Value);
		PreserveLayer = bool.Parse(root.Element("PreserveLayer").Value);
		TargetLayer = (RoomLayer)int.Parse(root.Element("TargetLayer").Value);
	}

	public IFuturemud Gameworld => Spell.Gameworld;

	public IMagicSpell Spell { get; }

	public bool TeleportParty { get; private set; }

	public bool PreserveLayer { get; private set; }
	public RoomLayer TargetLayer { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "teleport"),
			new XElement("TeleportParty", TeleportParty),
			new XElement("PreserveLayer", PreserveLayer),
			new XElement("TargetLayer", (int)TargetLayer)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
public static bool IsCompatibleWithTrigger(string types)
	{
		switch (types)
		{
			case "character":
			case "characters":
				return true;
			default:
				return false;
		}
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICell cell)
		{
			return null;
		}

		caster.Teleport(cell, PreserveLayer ? caster.RoomLayer : TargetLayer, TeleportParty, true);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new TeleportEffect(SaveToXml(), Spell);
	}

	#region Implementation of IEditableItem
	public string Show(ICharacter actor)
	{
		return
			$"Teleport {(TeleportParty ? "Self and Party".ColourCharacter() : "Self".ColourCharacter())} {(PreserveLayer ? "to same layer" : $"to {TargetLayer.DescribeEnum()}").ColourName()}";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3party#0 - toggles including the party in the teleport
	#3layer same#0 - preserves current room layer
	#3layer <which>#0 - teleports to a specific layer";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "party":
				return BuildingCommandParty(actor);
			case "layer":
				return BuildingCommandLayer(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandLayer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a room layer, or the keyword #3same#0 to preserve the room layer.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("same"))
		{
			PreserveLayer = true;
			TargetLayer = RoomLayer.GroundLevel;
			Spell.Changed = true;
			actor.OutputHandler.Send("This teleport effect will now teleport the target to the same room layer they're currently in (if possible).");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<RoomLayer>(out var layer))
		{
			actor.OutputHandler.Send($"There is no room layer identified by the text {command.SafeRemainingArgument.ColourCommand()}.\nValid values include: {Enum.GetValues<RoomLayer>().ListToColouredString()}");
			return false;
		}

		PreserveLayer = false;
		TargetLayer = layer;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell will now teleport position the target {layer.LocativeDescription().ColourName()}.");
		return true;
	}

	private bool BuildingCommandParty(ICharacter actor)
	{
		TeleportParty = !TeleportParty;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			TeleportParty ?
				"This spell will now also teleport the caster's party, as well as anything or anyone they're dragging or grappling.": 
				"This spell will now only teleport the caster"
		);
		return true;
	}

	#endregion
}