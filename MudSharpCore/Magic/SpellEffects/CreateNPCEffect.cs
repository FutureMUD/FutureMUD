using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Events;

namespace MudSharp.Magic.SpellEffects;

class CreateNPCEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createnpc", (root, spell) => new CreateNPCEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createnpc", BuilderFactory,
			"Loads a new NPC",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CreateNPCEffect(new XElement("Effect",
			new XAttribute("type", "createnpc"),
			new XElement("NPCPrototypeId", 0),
			new XElement("OnLoadProg", 0)
		), spell), string.Empty);
	}

	protected CreateNPCEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_npcPrototypeId = long.Parse(root.Element("NPCPrototypeId").Value);
		_onLoadProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnLoadProg").Value));
	}

	public IFuturemud Gameworld => Spell.Gameworld;

	public IMagicSpell Spell { get; }

	private long _npcPrototypeId;

	public INPCTemplate NPCTemplate => Gameworld.NpcTemplates.Get(_npcPrototypeId);

	private IFutureProg _onLoadProg;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createnpc"),
			new XElement("NPCPrototypeId", _npcPrototypeId),
			new XElement("OnLoadProg", _onLoadProg?.Id ?? 0)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
public static bool IsCompatibleWithTrigger(string types)
	{
		switch (types)
		{
			case "room":
			case "rooms":
				return true;
			default:
				return false;
		}
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var template = NPCTemplate;
		if (template is null)
		{
			return null;
		}

		var cell = target as ICell ?? caster.Location;

		var newCharacter = template.CreateNewCharacter(cell);
		Gameworld.Add(newCharacter, true);
		if (cell == caster.Location)
		{
			newCharacter.RoomLayer = caster.RoomLayer;
		}
		
		template.OnLoadProg?.Execute(newCharacter);
		_onLoadProg?.Execute(newCharacter, caster, Spell);

		if (newCharacter.Location.IsSwimmingLayer(newCharacter.RoomLayer) && newCharacter.Race.CanSwim)
		{
			newCharacter.PositionState = PositionSwimming.Instance;
		}
		else if (newCharacter.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && newCharacter.CanFly().Truth)
		{
			newCharacter.PositionState = PositionFlying.Instance;
		}

		newCharacter.Location.Login(newCharacter);
		newCharacter.HandleEvent(EventType.NPCOnGameLoadFinished, newCharacter);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CreateNPCEffect(SaveToXml(), Spell);
	}

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this effect:

	#3npc <proto>#0 - sets the item proto to be loaded
	#3prog <which>#0 - sets the item skin for the item to be loaded
	#3prog none#0 - clears the item skin";

	public string Show(ICharacter actor)
	{
		return
			$"CreateNPC - {NPCTemplate?.EditHeader() ?? "nothing".ColourError()} - OnLoad: {_onLoadProg?.MXPClickableFunctionName() ?? "none".ColourError()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "npc":
				return BuildingCommandNPC(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog to execute when the NPC is loaded or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void, [
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_onLoadProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now execute the {prog.MXPClickableFunctionName()} prog when loading the NPC.");
		return true;
	}

	private bool BuildingCommandNPC(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which NPC prototype should this spell effect load?");
			return false;
		}

		var proto = Gameworld.NpcTemplates.GetByIdOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid NPC prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"The NPC prototype {proto.EditHeader()} is not approved for use.");
			return false;
		}

		_npcPrototypeId = proto.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now load the NPC {proto.EditHeader()}.");
		return true;
	}
	#endregion
}