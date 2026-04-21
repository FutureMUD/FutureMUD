#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public enum RoomFlagKind
{
	Peaceful = 0,
	NoDream = 1,
	Alarm = 2,
	Darkness = 3,
	WardTag = 4
}

public abstract class RoomSpellEffectTemplateBase : IMagicSpellEffectTemplate
{
	protected RoomSpellEffectTemplateBase(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		LoadFromXml(root);
	}

	protected virtual void LoadFromXml(XElement root)
	{
	}

	protected abstract string BuilderEffectType { get; }

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public virtual bool IsInstantaneous => false;
	public virtual bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		var root = new XElement("Effect", new XAttribute("type", BuilderEffectType));
		SaveToXml(root);
		return root;
	}

	protected virtual void SaveToXml(XElement root)
	{
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return StandaloneSpellEffectTemplateHelper.IsRoomTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICell cell)
		{
			return null;
		}

		return CreateEffect(caster, cell, outcome, power, parent, additionalParameters);
	}

	protected abstract IMagicSpellEffect? CreateEffect(ICharacter caster, ICell target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters);

	public abstract bool BuildingCommand(ICharacter actor, StringStack command);
	public abstract string Show(ICharacter actor);
	public abstract IMagicSpellEffectTemplate Clone();
}

public class RoomFlagEffect : RoomSpellEffectTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:

	#3type <flag>#0 - sets the room flag type
	#3lux <amount>#0 - sets the darkness lux penalty for darkness flags
	#3echo <emote>#0 - sets the alarm echo for alarm flags
	#3prog <prog>#0 - sets the optional alarm callback prog
	#3prog clear#0 - clears the alarm callback prog
	#3tag <text>#0 - sets the ward tag text for wardtag flags";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomflag", (root, spell) => new RoomFlagEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomflag", BuilderFactory,
			"Applies a persistent magical room flag",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.RoomTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RoomFlagEffect(new XElement("Effect",
			new XAttribute("type", "roomflag"),
			new XElement("FlagType", (int)RoomFlagKind.Peaceful),
			new XElement("DarknessLux", 100.0),
			new XElement("AlarmEcho", new XCData("@ trigger|triggers a magical alarm.")),
			new XElement("AlarmProg", 0L),
			new XElement("WardTag", new XCData("ward"))
		), spell), string.Empty);
	}

	protected RoomFlagEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	public RoomFlagKind FlagType { get; private set; }
	public double DarknessLux { get; private set; }
	public string AlarmEcho { get; private set; } = string.Empty;
	public IFutureProg? AlarmProg { get; private set; }
	public string WardTag { get; private set; } = "ward";

	protected override string BuilderEffectType => "roomflag";

	protected override void LoadFromXml(XElement root)
	{
		FlagType = (RoomFlagKind)int.Parse(root.Element("FlagType")?.Value ?? "0");
		DarknessLux = Math.Abs(double.Parse(root.Element("DarknessLux")?.Value ?? "100"));
		AlarmEcho = root.Element("AlarmEcho")?.Value ?? "@ trigger|triggers a magical alarm.";
		AlarmProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("AlarmProg")?.Value ?? "0"));
		WardTag = root.Element("WardTag")?.Value ?? "ward";
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("FlagType", (int)FlagType));
		root.Add(new XElement("DarknessLux", DarknessLux));
		root.Add(new XElement("AlarmEcho", new XCData(AlarmEcho)));
		root.Add(new XElement("AlarmProg", AlarmProg?.Id ?? 0L));
		root.Add(new XElement("WardTag", new XCData(WardTag)));
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICell target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return FlagType switch
		{
			RoomFlagKind.Peaceful => new SpellRoomPeacefulEffect(target, parent),
			RoomFlagKind.NoDream => new SpellRoomNoDreamEffect(target, parent),
			RoomFlagKind.Alarm => new SpellRoomAlarmEffect(target, parent, AlarmEcho, AlarmProg),
			RoomFlagKind.Darkness => new SpellRoomDarknessEffect(target, parent, DarknessLux),
			RoomFlagKind.WardTag => new SpellRoomWardTagEffect(target, parent, WardTag),
			_ => null
		};
	}

	public override string Show(ICharacter actor)
	{
		return FlagType switch
		{
			RoomFlagKind.Peaceful => "Room Flag - Peaceful",
			RoomFlagKind.NoDream => "Room Flag - NoDream",
			RoomFlagKind.Alarm => $"Room Flag - Alarm [{AlarmEcho.ColourCommand()}]{(AlarmProg is null ? "" : $" ({AlarmProg.MXPClickableFunctionName()})")}",
			RoomFlagKind.Darkness => $"Room Flag - Darkness [{DarknessLux.ToString("N2", actor).ColourValue()} lux]",
			RoomFlagKind.WardTag => $"Room Flag - WardTag [{WardTag.ColourCommand()}]",
			_ => "Room Flag - Unknown"
		};
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "lux":
				return BuildingCommandLux(actor, command);
			case "echo":
				return BuildingCommandEcho(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out RoomFlagKind value))
		{
			actor.OutputHandler.Send(
				$"You must specify a valid room flag type. Valid options are {Enum.GetValues<RoomFlagKind>().ListToColouredString()}.");
			return false;
		}

		FlagType = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now apply the {value.DescribeEnum().ColourValue()} room flag.");
		return true;
	}

	private bool BuildingCommandLux(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative lux penalty.");
			return false;
		}

		DarknessLux = value;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This darkness room flag will now subtract {DarknessLux.ToString("N2", actor).ColourValue()} lux.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should the alarm flag echo when someone enters?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		AlarmEcho = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This alarm room flag now uses the echo {AlarmEcho.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should this alarm room flag invoke?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			AlarmProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This alarm room flag will no longer invoke any prog.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts either a character, or a character and a location, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		AlarmProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This alarm room flag will now invoke {prog.MXPClickableFunctionNameWithId()}.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What ward tag should this room flag apply?");
			return false;
		}

		WardTag = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This room flag will now apply the ward tag {WardTag.ColourCommand()}.");
		return true;
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new RoomFlagEffect(SaveToXml(), Spell);
	}
}

public class RemoveRoomFlagEffect : RoomSpellEffectTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:

	#3type <flag>#0 - sets which room flag type is removed
	#3tag <text>#0 - sets the ward tag text to remove for wardtag flags";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeroomflag", (root, spell) => new RemoveRoomFlagEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removeroomflag", BuilderFactory,
			"Removes a persistent magical room flag",
			HelpText,
			true,
			true,
			StandaloneSpellEffectTemplateHelper.RoomTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RemoveRoomFlagEffect(new XElement("Effect",
			new XAttribute("type", "removeroomflag"),
			new XElement("FlagType", (int)RoomFlagKind.Peaceful),
			new XElement("WardTag", new XCData("ward"))
		), spell), string.Empty);
	}

	protected RemoveRoomFlagEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	public override bool IsInstantaneous => true;

	public RoomFlagKind FlagType { get; private set; }
	public string WardTag { get; private set; } = "ward";

	protected override string BuilderEffectType => "removeroomflag";

	protected override void LoadFromXml(XElement root)
	{
		FlagType = (RoomFlagKind)int.Parse(root.Element("FlagType")?.Value ?? "0");
		WardTag = root.Element("WardTag")?.Value ?? "ward";
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("FlagType", (int)FlagType));
		root.Add(new XElement("WardTag", new XCData(WardTag)));
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICell target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		switch (FlagType)
		{
			case RoomFlagKind.Peaceful:
				target.RemoveAllEffects<SpellRoomPeacefulEffect>(null, true);
				break;
			case RoomFlagKind.NoDream:
				target.RemoveAllEffects<SpellRoomNoDreamEffect>(null, true);
				break;
			case RoomFlagKind.Alarm:
				target.RemoveAllEffects<SpellRoomAlarmEffect>(null, true);
				break;
			case RoomFlagKind.Darkness:
				target.RemoveAllEffects<SpellRoomDarknessEffect>(null, true);
				break;
			case RoomFlagKind.WardTag:
				target.RemoveAllEffects<SpellRoomWardTagEffect>(x => x.WardTag.EqualTo(WardTag), true);
				break;
		}

		return null;
	}

	public override string Show(ICharacter actor)
	{
		return FlagType == RoomFlagKind.WardTag
			? $"Remove Room Flag - WardTag [{WardTag.ColourCommand()}]"
			: $"Remove Room Flag - {FlagType.DescribeEnum()}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out RoomFlagKind value))
		{
			actor.OutputHandler.Send(
				$"You must specify a valid room flag type. Valid options are {Enum.GetValues<RoomFlagKind>().ListToColouredString()}.");
			return false;
		}

		FlagType = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now remove the {value.DescribeEnum().ColourValue()} room flag.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What ward tag text should this remove-room-flag effect match?");
			return false;
		}

		WardTag = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now remove the ward tag {WardTag.ColourCommand()}.");
		return true;
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new RemoveRoomFlagEffect(SaveToXml(), Spell);
	}
}
