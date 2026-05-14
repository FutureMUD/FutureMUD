using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManualCombatCommandModel = MudSharp.Models.ManualCombatCommand;

namespace MudSharp.Combat;

#nullable enable
public class ManualCombatCommand : SaveableItem, IManualCombatCommand
{
	private readonly List<string> _aliases = new();

	public ManualCombatCommand(ManualCombatCommandModel dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(dbitem);
	}

	public ManualCombatCommand(IFuturemud gameworld, string name, string? primaryVerb = null)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			primaryVerb ??= name;
			var dbitem = new ManualCombatCommandModel
			{
				Name = name,
				PrimaryVerb = NormaliseCommandWord(primaryVerb),
				AdditionalVerbs = string.Empty,
				ActionKind = (int)ManualCombatActionKind.AuxiliaryAction,
				CombatActionId = null,
				WeaponAttackId = null,
				PlayerUsable = true,
				NpcUsable = true,
				UsabilityProgId = null,
				CooldownSeconds = 0.0,
				CooldownMessage = "You must wait a short time before doing that again.",
				DefaultAiWeightMultiplier = 1.0
			};
			FMDB.Context.ManualCombatCommands.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	private ManualCombatCommand(IManualCombatCommand rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new ManualCombatCommandModel
			{
				Name = name,
				PrimaryVerb = name.ToLowerInvariant(),
				AdditionalVerbs = string.Join(" ", rhs.CommandWords
				                                  .Where(x => !x.EqualTo(rhs.PrimaryVerb))
				                                  .Select(NormaliseCommandWord)),
				ActionKind = (int)rhs.ActionKind,
				CombatActionId = rhs.AuxiliaryAction?.Id,
				WeaponAttackId = rhs.WeaponAttack?.Id,
				PlayerUsable = rhs.PlayerUsable,
				NpcUsable = rhs.NpcUsable,
				UsabilityProgId = rhs.UsabilityProg?.Id,
				CooldownSeconds = rhs.CooldownSeconds,
				CooldownMessage = rhs.CooldownMessage,
				DefaultAiWeightMultiplier = rhs.DefaultAiWeightMultiplier
			};
			FMDB.Context.ManualCombatCommands.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	private void LoadFromDatabase(ManualCombatCommandModel dbitem)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		PrimaryVerb = NormaliseCommandWord(dbitem.PrimaryVerb);
		_aliases.Clear();
		_aliases.AddRange(ParseAliasText(dbitem.AdditionalVerbs)
		                  .Where(x => !x.EqualTo(PrimaryVerb))
		                  .Distinct(StringComparer.InvariantCultureIgnoreCase));
		ActionKind = (ManualCombatActionKind)dbitem.ActionKind;
		WeaponAttack = Gameworld.WeaponAttacks.Get(dbitem.WeaponAttackId ?? 0);
		AuxiliaryAction = Gameworld.AuxiliaryCombatActions.Get(dbitem.CombatActionId ?? 0);
		PlayerUsable = dbitem.PlayerUsable;
		NpcUsable = dbitem.NpcUsable;
		UsabilityProg = Gameworld.FutureProgs.Get(dbitem.UsabilityProgId ?? 0);
		CooldownSeconds = dbitem.CooldownSeconds;
		CooldownMessage = dbitem.CooldownMessage;
		DefaultAiWeightMultiplier = dbitem.DefaultAiWeightMultiplier;
	}

	public override string FrameworkItemType => "ManualCombatCommand";

	public string PrimaryVerb { get; private set; } = string.Empty;

	public IEnumerable<string> CommandWords => new[] { PrimaryVerb }
		.Concat(_aliases)
		.Where(x => !string.IsNullOrWhiteSpace(x))
		.Distinct(StringComparer.InvariantCultureIgnoreCase);

	public IEnumerable<string> Keywords => CommandWords.Concat(new ExplodedString(Name).Words).Distinct(StringComparer.InvariantCultureIgnoreCase);

	public ManualCombatActionKind ActionKind { get; private set; }

	public IWeaponAttack? WeaponAttack { get; private set; }

	public IAuxiliaryCombatAction? AuxiliaryAction { get; private set; }

	public bool PlayerUsable { get; set; }

	public bool NpcUsable { get; set; }

	public IFutureProg? UsabilityProg { get; set; }

	public double CooldownSeconds { get; set; }

	public string CooldownMessage { get; set; } = "You must wait a short time before doing that again.";

	public double DefaultAiWeightMultiplier { get; set; }

	private static IEnumerable<string> ParseAliasText(string? text)
	{
		return (text ?? string.Empty)
		       .Split(new[] { ' ', ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
		       .Select(NormaliseCommandWord)
		       .Where(x => !string.IsNullOrWhiteSpace(x));
	}

	private static string NormaliseCommandWord(string text)
	{
		return text.Trim().ToLowerInvariant();
	}

	public bool IsUsableBy(ICharacter actor, ICharacter target)
	{
		return UsabilityProg?.ExecuteBool(actor, null, target) ?? true;
	}

	public IManualCombatCommand Clone(string name)
	{
		return new ManualCombatCommand(this, name);
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ManualCombatCommands.Find(Id);
			if (dbitem is null)
			{
				return;
			}

			dbitem.Name = Name;
			dbitem.PrimaryVerb = PrimaryVerb;
			dbitem.AdditionalVerbs = string.Join(" ", _aliases);
			dbitem.ActionKind = (int)ActionKind;
			dbitem.WeaponAttackId = WeaponAttack?.Id;
			dbitem.CombatActionId = AuxiliaryAction?.Id;
			dbitem.PlayerUsable = PlayerUsable;
			dbitem.NpcUsable = NpcUsable;
			dbitem.UsabilityProgId = UsabilityProg?.Id;
			dbitem.CooldownSeconds = CooldownSeconds;
			dbitem.CooldownMessage = CooldownMessage;
			dbitem.DefaultAiWeightMultiplier = DefaultAiWeightMultiplier;
			FMDB.Context.SaveChanges();
			Changed = false;
		}
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Manual Combat Command #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLineColumns((uint)actor.LineFormatLength, 2,
			$"Primary Verb: {PrimaryVerb.ColourCommand()}",
			$"Aliases: {(_aliases.Any() ? _aliases.Select(x => x.ColourCommand()).ListToString() : "None".ColourError())}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 2,
			$"Players: {PlayerUsable.ToColouredString()}",
			$"NPCs: {NpcUsable.ToColouredString()}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 2,
			$"Action Kind: {ActionKind.DescribeEnum().ColourName()}",
			$"Target Action: {DescribeActionTarget(actor)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 2,
			$"Cooldown: {CooldownSeconds.ToString("N2", actor).ColourValue()}s",
			$"AI Multiplier: {DefaultAiWeightMultiplier.ToString("N2", actor).ColourValue()}x"
		);
		sb.AppendLine($"Cooldown Message: {CooldownMessage}");
		sb.AppendLine($"Usability Prog: {(UsabilityProg is not null ? $"{UsabilityProg.FunctionName} (#{UsabilityProg.Id.ToString("N0", actor)})".ColourName() : "None".ColourError())}");
		return sb.ToString();
	}

	private string DescribeActionTarget(ICharacter actor)
	{
		return ActionKind switch
		{
			ManualCombatActionKind.WeaponAttack => WeaponAttack is not null
				? $"{WeaponAttack.Name.ColourName()} (#{WeaponAttack.Id.ToString("N0", actor)})"
				: "None".ColourError(),
			ManualCombatActionKind.AuxiliaryAction => AuxiliaryAction is not null
				? $"{AuxiliaryAction.Name.ColourName()} (#{AuxiliaryAction.Id.ToString("N0", actor)})"
				: "None".ColourError(),
			_ => "Unknown".ColourError()
		};
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "verb":
			case "primary":
				return BuildingCommandVerb(actor, command);
			case "alias":
			case "aliases":
				return BuildingCommandAlias(actor, command);
			case "removealias":
			case "remalias":
			case "deletealias":
			case "delalias":
				return BuildingCommandRemoveAlias(actor, command);
			case "clearaliases":
			case "clearalias":
				return BuildingCommandClearAliases(actor);
			case "action":
			case "target":
				return BuildingCommandAction(actor, command);
			case "players":
			case "player":
				return BuildingCommandPlayers(actor);
			case "npcs":
			case "npc":
				return BuildingCommandNpcs(actor);
			case "cooldown":
			case "delay":
				return BuildingCommandCooldown(actor, command);
			case "message":
			case "cooldownmessage":
			case "cooldownmsg":
				return BuildingCommandCooldownMessage(actor, command);
			case "prog":
			case "usability":
			case "usabilityprog":
			case "futureprog":
				return BuildingCommandProg(actor, command);
			case "aimultiplier":
			case "multiplier":
			case "ai":
			case "weight":
				return BuildingCommandAiMultiplier(actor, command);
			default:
				actor.OutputHandler.Send(CombatBuilderModule.ManualCombatCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give this manual combat command?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ManualCombatCommands.Any(x => x.Name.EqualTo(name) && x.Id != Id))
		{
			actor.OutputHandler.Send($"There is already a manual combat command named {name.ColourName()}.");
			return false;
		}

		var oldName = Name;
		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"You rename manual combat command {oldName.ColourName()} to {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which command verb should invoke this manual combat command?");
			return false;
		}

		var verb = NormaliseCommandWord(command.PopSpeech());
		if (!ManualCombatCommandRegistry.IsValidCommandWord(verb))
		{
			actor.OutputHandler.Send("Manual combat verbs must be a single alphabetic command word.");
			return false;
		}

		if (ManualCombatCommandRegistry.HasReservedCommandCollision(verb) ||
		    Gameworld.ManualCombatCommands.Any(x => x.Id != Id && x.CommandWords.Any(y => y.EqualTo(verb))))
		{
			actor.OutputHandler.Send($"The verb {verb.ColourCommand()} is already used by another command.");
			return false;
		}

		PrimaryVerb = verb;
		_aliases.RemoveAll(x => x.EqualTo(verb));
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"This manual combat command now uses {verb.ColourCommand()} as its primary verb.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which alias do you want to toggle for this manual combat command?");
			return false;
		}

		var alias = NormaliseCommandWord(command.PopSpeech());
		if (!ManualCombatCommandRegistry.IsValidCommandWord(alias))
		{
			actor.OutputHandler.Send("Manual combat aliases must be single alphabetic command words.");
			return false;
		}

		if (alias.EqualTo(PrimaryVerb))
		{
			actor.OutputHandler.Send("The primary verb is already a command word for this manual combat command.");
			return false;
		}

		if (_aliases.Any(x => x.EqualTo(alias)))
		{
			_aliases.RemoveAll(x => x.EqualTo(alias));
			Changed = true;
			ManualCombatCommandRegistry.Rebuild(Gameworld);
			actor.OutputHandler.Send($"The alias {alias.ColourCommand()} will no longer invoke this manual combat command.");
			return true;
		}

		if (ManualCombatCommandRegistry.HasReservedCommandCollision(alias) ||
		    Gameworld.ManualCombatCommands.Any(x => x.Id != Id && x.CommandWords.Any(y => y.EqualTo(alias))))
		{
			actor.OutputHandler.Send($"The alias {alias.ColourCommand()} is already used by another command.");
			return false;
		}

		_aliases.Add(alias);
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"The alias {alias.ColourCommand()} will now invoke this manual combat command.");
		return true;
	}

	private bool BuildingCommandRemoveAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which alias do you want to remove from this manual combat command?");
			return false;
		}

		var alias = NormaliseCommandWord(command.PopSpeech());
		if (!_aliases.Any(x => x.EqualTo(alias)))
		{
			actor.OutputHandler.Send($"This manual combat command does not have an alias {alias.ColourCommand()}.");
			return false;
		}

		_aliases.RemoveAll(x => x.EqualTo(alias));
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"The alias {alias.ColourCommand()} will no longer invoke this manual combat command.");
		return true;
	}

	private bool BuildingCommandClearAliases(ICharacter actor)
	{
		_aliases.Clear();
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send("This manual combat command no longer has any aliases.");
		return true;
	}

	private bool BuildingCommandAction(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to bind a weapon attack or an auxiliary action?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "weapon":
			case "weaponattack":
			case "attack":
				return BuildingCommandWeaponAction(actor, command);
			case "aux":
			case "auxiliary":
			case "auxiliaryaction":
			case "action":
				return BuildingCommandAuxiliaryAction(actor, command);
		}

		actor.OutputHandler.Send("You must specify either weapon or auxiliary as the action kind.");
		return false;
	}

	private bool BuildingCommandWeaponAction(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weapon attack should this manual combat command execute?");
			return false;
		}

		var attack = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.WeaponAttacks.Get(value)
			: Gameworld.WeaponAttacks.GetByName(command.SafeRemainingArgument);
		if (attack is null)
		{
			actor.OutputHandler.Send("There is no such weapon attack.");
			return false;
		}

		ActionKind = ManualCombatActionKind.WeaponAttack;
		WeaponAttack = attack;
		AuxiliaryAction = null;
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"This manual combat command will now execute the {attack.Name.ColourName()} weapon/natural attack.");
		return true;
	}

	private bool BuildingCommandAuxiliaryAction(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which auxiliary combat action should this manual combat command execute?");
			return false;
		}

		var action = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.AuxiliaryCombatActions.Get(value)
			: Gameworld.AuxiliaryCombatActions.GetByName(command.SafeRemainingArgument);
		if (action is null)
		{
			actor.OutputHandler.Send("There is no such auxiliary combat action.");
			return false;
		}

		ActionKind = ManualCombatActionKind.AuxiliaryAction;
		AuxiliaryAction = action;
		WeaponAttack = null;
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"This manual combat command will now execute the {action.Name.ColourName()} auxiliary action.");
		return true;
	}

	private bool BuildingCommandPlayers(ICharacter actor)
	{
		PlayerUsable = !PlayerUsable;
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"Players can {PlayerUsable.NowNoLonger()} use this manual combat command.");
		return true;
	}

	private bool BuildingCommandNpcs(ICharacter actor)
	{
		NpcUsable = !NpcUsable;
		Changed = true;
		ManualCombatCommandRegistry.Rebuild(Gameworld);
		actor.OutputHandler.Send($"NPCs can {NpcUsable.NowNoLonger()} use this manual combat command.");
		return true;
	}

	private bool BuildingCommandCooldown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative cooldown in seconds.");
			return false;
		}

		CooldownSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This manual combat command now has a command cooldown of {CooldownSeconds.ToString("N2", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandCooldownMessage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What message should players see when this command is still cooling down?");
			return false;
		}

		CooldownMessage = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The cooldown message is now: {CooldownMessage}");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You can specify a boolean prog, or none to clear the current prog.");
			return false;
		}

		if (command.Peek().EqualToAny("none", "clear", "remove"))
		{
			UsabilityProg = null;
			Changed = true;
			actor.OutputHandler.Send("This manual combat command no longer has a usability prog.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != ProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send($"The usability prog must return boolean, but {prog.FunctionName.ColourName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send("The usability prog must accept parameters character, item, character.");
			return false;
		}

		UsabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This manual combat command will now use {prog.FunctionName.ColourName()} as its usability prog.");
		return true;
	}

	private bool BuildingCommandAiMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a non-negative AI weight multiplier.");
			return false;
		}

		DefaultAiWeightMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"NPC combat settings without an override now apply a {DefaultAiWeightMultiplier.ToString("N2", actor).ColourValue()}x multiplier for this command's underlying action.");
		return true;
	}
}
#nullable restore
