using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class CombatMessage : SaveableItem, ICombatMessage
{
	public CombatMessage(IFuturemud gameworld)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.CombatMessage
			{
				Priority = 0,
				Type = (int)BuiltInCombatMoveType.UseWeaponAttack,
				Message = "$0 attack|attacks $1 with $2",
				FailureMessage = "$0 attack|attacks $1 with $2"
			};
			FMDB.Context.CombatMessages.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	public CombatMessage(MudSharp.Models.CombatMessage message, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(message);
	}

	public CombatMessage(CombatMessage rhs)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.CombatMessage
			{
				Priority = rhs.Priority,
				Type = (int)rhs.Type,
				Message = rhs.Message,
				FailureMessage = rhs.FailMessage,
				Outcome = (int?)rhs.Outcome,
				Verb = (int?)rhs.Verb,
				Chance = rhs.Chance,
				ProgId = rhs.Prog?.Id
			};
			foreach (var item in rhs.WeaponAttackIds)
			{
				dbitem.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
					{ CombatMessage = dbitem, WeaponAttackId = item });
			}

			FMDB.Context.CombatMessages.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	private void LoadFromDatabase(MudSharp.Models.CombatMessage message)
	{
		Id = message.Id;
		_name = $"Combat Message {Id}";
		Type = (BuiltInCombatMoveType)message.Type;
		Outcome = message.Outcome.HasValue ? (Outcome)message.Outcome : default(Outcome?);
		Message = message.Message;
		FailMessage = message.FailureMessage;
		Prog = Gameworld.FutureProgs.Get(message.ProgId ?? 0);
		Priority = message.Priority;
		Verb = message.Verb.HasValue ? (MeleeWeaponVerb)message.Verb : default(MeleeWeaponVerb?);
		Chance = message.Chance;
		foreach (var attack in message.CombatMessagesWeaponAttacks.Select(x => x.WeaponAttackId).ToList())
		{
			WeaponAttackIds.Add(attack);
		}
	}

	public double Chance { get; set; }
	public BuiltInCombatMoveType Type { get; set; }
	public Outcome? Outcome { get; set; }
	public string Message { get; set; }
	public string FailMessage { get; set; }
	public IFutureProg Prog { get; set; }
	public int Priority { get; set; }
	public MeleeWeaponVerb? Verb { get; set; }
	public HashSet<long> WeaponAttackIds { get; } = new();

	public bool Applies(ICharacter character, IPerceiver target, IGameItem weapon,
		IWeaponAttack attack, BuiltInCombatMoveType type, Outcome outcome,
		IBodypart bodypart)
	{
		if (Outcome.HasValue && outcome != Outcome && Outcome != RPG.Checks.Outcome.None)
		{
			return false;
		}

		if (WeaponAttackIds.Any())
		{
			if (!WeaponAttackIds.Contains(attack?.Id ?? 0))
			{
				return false;
			}
		}

		if (Verb.HasValue && attack?.Verb != Verb)
		{
			return false;
		}

		if (Type != type)
		{
			return false;
		}

		if ((bool?)Prog?.Execute(character, target, weapon, attack?.Id ?? 0, attack?.Verb.Describe(),
			    bodypart?.Shape.Name ?? "") == false)
		{
			return false;
		}

		return true;
	}

	public bool CouldApply(IWeaponAttack attack)
	{
		if (WeaponAttackIds.Any())
		{
			if (!WeaponAttackIds.Contains(attack?.Id ?? 0))
			{
				return false;
			}
		}

		if (Verb.HasValue && attack?.Verb != Verb)
		{
			return false;
		}

		if (Type != attack?.MoveType)
		{
			return false;
		}

		return true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CombatMessages.Find(Id);
		dbitem.Priority = Priority;
		dbitem.Type = (int)Type;
		dbitem.Message = Message;
		dbitem.FailureMessage = FailMessage;
		dbitem.Outcome = (int?)Outcome;
		dbitem.Verb = (int?)Verb;
		dbitem.Chance = Chance;
		dbitem.ProgId = Prog?.Id;
		FMDB.Context.CombatMessagesWeaponAttacks.RemoveRange(dbitem.CombatMessagesWeaponAttacks);
		foreach (var item in WeaponAttackIds)
		{
			dbitem.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
				{ CombatMessage = dbitem, WeaponAttackId = item });
		}

		Changed = false;
	}

	public override string FrameworkItemType => "CombatMessage";

	public string ShowBuilder(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Combat Message #{Id}");
		sb.AppendLine($"Type: {Type.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine($"Verb: {Verb?.Describe().Colour(Telnet.Cyan) ?? "Any".Colour(Telnet.BoldWhite)}");
		sb.AppendLine($"Outcome: {Outcome?.DescribeColour() ?? "Any".Colour(Telnet.BoldWhite)}");
		sb.AppendLine($"Priority: {Priority.ToString("N0", actor).Colour(Telnet.Green)}");
		sb.AppendLine($"Chance: {Chance.ToString("P3", actor).Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Prog: {(Prog != null ? $"{Prog.FunctionName} (#{Prog.Id})".FluentTagMXP("send", $"href='show futureprog {Prog.Id}'") : "None".Colour(Telnet.Red))}");
		sb.AppendLine();
		sb.AppendLine($"Message: {Message.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Fail Message: {FailMessage.Colour(Telnet.Yellow)}");
		if (WeaponAttackIds.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Weapon Attacks:");
			foreach (var attack in WeaponAttackIds.Select(x => Gameworld.WeaponAttacks.Get(x)))
			{
				var weapon = Gameworld.WeaponTypes.FirstOrDefault(x => x.Attacks.Contains(attack));
				sb.AppendLine($"\t{attack.Name} (#{attack.Id}){(weapon != null ? $" [{weapon.Name}]" : "")}");
			}
		}

		return sb.ToString();
	}

	#region Overrides of Object

	public override string ToString()
	{
		return $"{Id}: {Message}";
	}

	#endregion

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "chance":
				return BuildingCommandChance(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "outcome":
				return BuildingCommandOutcome(actor, command);
			case "message":
				return BuildingCommandMessage(actor, command);
			case "fail":
			case "failmessage":
			case "fail message":
				return BuildingCommandFail(actor, command);
			case "prog":
			case "futureprog":
				return BuildingCommandProg(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "verb":
				return BuildingCommandVerb(actor, command);
			case "attack":
				return BuildingCommandAttack(actor, command);
		}

		actor.OutputHandler.Send(
			"You can use the following options with this command:\n\tchance <0...1> - the chance between 0 and 1 of this message being chosen\n\tpriority <number> - the higher the number, the earlier the message will be evaluated to see if it applies\n\tverb <verb> - the verb this attack applies to, or NONE for all\n\toutcome <outcome> - the outcome this attack applies to, or NONE for all\n\ttype <type> - the BuiltInCombatType this combat message is for\n\tprog <prog id|name> - the prog that controls whether this attack applies or NONE to clear.\n\tmessage <message> - the message for this attack\n\tfail <message> - the fail message for this attack\n\tattack <id> - toggles a specific weapon attack on or off for this message");
		return false;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value < 0 || value > 1)
		{
			actor.OutputHandler.Send("You must enter a chance between 0 (no chance) and 1 (guaranteed).");
			return false;
		}

		Chance = value;
		Changed = true;
		actor.OutputHandler.Send($"This combat message now has a {Chance.ToString("P3", actor)} chance to apply.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which built-in combat move type would you like this message to apply to? Valid types are {Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		if (!Utilities.TryParseEnum<BuiltInCombatMoveType>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid built-in combat move type. Valid types are {Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().Select(x => x.DescribeEnum().Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		Type = value;
		Changed = true;
		actor.OutputHandler.Send($"This combat message now applies to {Type.Describe().Colour(Telnet.Cyan)} moves.");
		if (WeaponAttackIds.Any())
		{
			var removed = new HashSet<long>();
			foreach (var item in WeaponAttackIds)
			{
				if (actor.Gameworld.WeaponAttacks.Get(item).MoveType != Type)
				{
					removed.Add(item);
				}
			}

			WeaponAttackIds.RemoveWhere(x => removed.Contains(x));
			actor.OutputHandler.Send(
				$"Warning, the following weapon attacks were removed as they were no longer valid: {removed.Select(x => x.ToString("N0", actor)).ListToString()}"
					.Colour(Telnet.Red));
		}

		return true;
	}

	private bool BuildingCommandOutcome(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What outcome should this combat message be associated with? Use NONE to clear.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			Outcome = null;
			Changed = true;
			actor.OutputHandler.Send("This combat message will now apply to all roll outcomes.");
			return true;
		}

		if (!CheckExtensions.GetOutcome(command.PopSpeech(), out var outcome))
		{
			actor.OutputHandler.Send(
				$"That is not a valid outcome. Valid outcomes are: {Enum.GetValues(typeof(Outcome)).Cast<Outcome>().Where(x => x != RPG.Checks.Outcome.None && x != RPG.Checks.Outcome.NotTested).Select(x => x.DescribeColour()).ListToString()}");
			return false;
		}

		Outcome = outcome;
		Changed = true;
		actor.OutputHandler.Send(
			$"This combat message will now only apply when the check's outcome is {Outcome.Value.DescribeColour()}.");
		return true;
	}

	private string GetHelpText(BuiltInCombatMoveType type)
	{
		switch (type)
		{
			case BuiltInCombatMoveType.UseWeaponAttack:
			case BuiltInCombatMoveType.RangedWeaponAttack:
			case BuiltInCombatMoveType.ClinchAttack:
			case BuiltInCombatMoveType.AimRangedWeapon:
			case BuiltInCombatMoveType.MeleeWeaponSmashItem:
			case BuiltInCombatMoveType.StaggeringBlow:
			case BuiltInCombatMoveType.UnbalancingBlow:
			case BuiltInCombatMoveType.DownedAttack:
				return
					"Valid tokens for this message: \n\t$0 - the attacker\n\t$1 - the defender\n\t$2 - the attack weapon\n\t$3 - the defense weapon, if any. This can be null, so use $?3 to check for null.\n\t$4 - the ward weapon, if any. This can be null, so use $?4 to check for null.\n\t{1} - the bodypart the attack targets";
			case BuiltInCombatMoveType.NaturalWeaponAttack:
			case BuiltInCombatMoveType.ClinchUnarmedAttack:
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
			case BuiltInCombatMoveType.SwoopAttackUnarmed:
			case BuiltInCombatMoveType.EnvenomingAttack:
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
			case BuiltInCombatMoveType.StaggeringBlowClinch:
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
			case BuiltInCombatMoveType.DownedAttackUnarmed:
				return
					"Valid tokens for this message: \n\t$0 - the attacker\n\t$1 - the defender\n\t$3 - the defense weapon, if any. This can be null, so use $?3 to check for null.\n\t$4 - the ward weapon, if any. This can be null, so use $?4 to check for null.\n\t\n\t{0} - the bodypart the attacker is using to make the attack\n\t{1} - the bodypart the attack targets\n\t@hand - left|right|front|back depending on which side the bodypart is on";
			case BuiltInCombatMoveType.Dodge:
			case BuiltInCombatMoveType.DesperateDodge:
				return
					"Valid tokens for this message: \n\t$0 - the attacker\n\t$1 - the defender\n\t$4 - the ward weapon, if any. This can be null, so use $?4 to check for null.\n\t\n\t{0} - the bodypart the attacker is using to make the attack\n\t{1} - the bodypart the attack targets";
			case BuiltInCombatMoveType.Parry:
			case BuiltInCombatMoveType.Block:
			case BuiltInCombatMoveType.DesperateParry:
			case BuiltInCombatMoveType.DesperateBlock:
				return
					"Valid tokens for this message: \n\t$0 - the attacker\n\t$1 - the defender\n\t$3 - the defense weapon, if any. This can be null, so use $?3 to check for null.\n\t$4 - the ward weapon, if any. This can be null, so use $?4 to check for null.\n\t\n\t{0} - the bodypart the attacker is using to make the attack\n\t{1} - the bodypart the attack targets";
			case BuiltInCombatMoveType.Disarm:
				break;
			case BuiltInCombatMoveType.Flee:
				break;
			case BuiltInCombatMoveType.RetrieveItem:
				break;
			case BuiltInCombatMoveType.ChargeToMelee:
				break;
			case BuiltInCombatMoveType.MoveToMelee:
				break;
			case BuiltInCombatMoveType.AdvanceAndFire:
				return
					"Valid tokens for this message: \n\t$0 - the attacker\n\t$1 - the defender\n\t$2 - the attack weapon";
			case BuiltInCombatMoveType.ReceiveCharge:
				break;
			case BuiltInCombatMoveType.WardDefense:
				break;
			case BuiltInCombatMoveType.WardCounter:
				break;
			case BuiltInCombatMoveType.WardFreeAttack:
				break;
			case BuiltInCombatMoveType.WardFreeUnarmedAttack:
				break;
			case BuiltInCombatMoveType.StartClinch:
				break;
			case BuiltInCombatMoveType.ResistClinch:
				break;
			case BuiltInCombatMoveType.BreakClinch:
				break;
			case BuiltInCombatMoveType.ResistBreakClinch:
				break;
			case BuiltInCombatMoveType.ClinchDodge:
				break;
			case BuiltInCombatMoveType.DodgeRange:
				break;
			case BuiltInCombatMoveType.BlockRange:
				break;
			case BuiltInCombatMoveType.StandAndFire:
				break;
			case BuiltInCombatMoveType.SkirmishAndFire:
				break;
			case BuiltInCombatMoveType.CoupDeGrace:
				break;
			case BuiltInCombatMoveType.Rescue:
				break;
			case BuiltInCombatMoveType.UnarmedSmashItem:
				break;
			case BuiltInCombatMoveType.InitiateGrapple:
				break;
			case BuiltInCombatMoveType.DodgeGrapple:
				break;
			case BuiltInCombatMoveType.CounterGrapple:
				break;
			case BuiltInCombatMoveType.ExtendGrapple:
				break;
			case BuiltInCombatMoveType.WrenchAttack:
				break;
			case BuiltInCombatMoveType.StrangleAttack:
				break;
			case BuiltInCombatMoveType.DodgeExtendGrapple:
				break;
			case BuiltInCombatMoveType.BeamAttack:
				break;
			case BuiltInCombatMoveType.ScreechAttack:
				break;
			case BuiltInCombatMoveType.OverpowerGrapple:
				break;
			case BuiltInCombatMoveType.StrangleAttackExtendGrapple:
				break;
		}

		return "No specific help for this type.";
	}

	private static Regex FormatStringRegex = new("{(?<number>\\d+)}");

	private (string HelpText, bool isValid) VetEmote(BuiltInCombatMoveType type, string text)
	{
		var help = GetHelpText(type);
		Emote emote;
		switch (type)
		{
			// 1 perceiver
			case BuiltInCombatMoveType.ScreechAttack:
			case BuiltInCombatMoveType.Breakout:
			case BuiltInCombatMoveType.Flee:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x != 0))
				{
					return (help, false);
				}

				break;
			// 2 perceivers
			case BuiltInCombatMoveType.RetrieveItem:
			case BuiltInCombatMoveType.NaturalWeaponAttack:
			case BuiltInCombatMoveType.ClinchUnarmedAttack:
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
			case BuiltInCombatMoveType.StaggeringBlowClinch:
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
			case BuiltInCombatMoveType.DownedAttackUnarmed:
			case BuiltInCombatMoveType.MagicPowerAttack:
			case BuiltInCombatMoveType.ChargeToMelee:
			case BuiltInCombatMoveType.MoveToMelee:
			case BuiltInCombatMoveType.WardCounter:
			case BuiltInCombatMoveType.StartClinch:
			case BuiltInCombatMoveType.ResistClinch:
			case BuiltInCombatMoveType.BreakClinch:
			case BuiltInCombatMoveType.ResistBreakClinch:
			case BuiltInCombatMoveType.ClinchDodge:
			case BuiltInCombatMoveType.InitiateGrapple:
			case BuiltInCombatMoveType.DodgeGrapple:
			case BuiltInCombatMoveType.CounterGrapple:
			case BuiltInCombatMoveType.ExtendGrapple:
			case BuiltInCombatMoveType.WrenchAttack:
			case BuiltInCombatMoveType.StrangleAttack:
			case BuiltInCombatMoveType.SwoopAttackUnarmed:
			case BuiltInCombatMoveType.EnvenomingAttack:
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 1))
				{
					return (help, false);
				}

				break;
			// 3 perceivers
			case BuiltInCombatMoveType.UseWeaponAttack:
			case BuiltInCombatMoveType.RangedWeaponAttack:
			case BuiltInCombatMoveType.ClinchAttack:
			case BuiltInCombatMoveType.AimRangedWeapon:
			case BuiltInCombatMoveType.MeleeWeaponSmashItem:
			case BuiltInCombatMoveType.StaggeringBlow:
			case BuiltInCombatMoveType.UnbalancingBlow:
			case BuiltInCombatMoveType.DownedAttack:
			case BuiltInCombatMoveType.Rescue:
			case BuiltInCombatMoveType.Disarm:
			case BuiltInCombatMoveType.DodgeRange:
			case BuiltInCombatMoveType.StandAndFire:
			case BuiltInCombatMoveType.SkirmishAndFire:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
					new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 2))
				{
					return (help, false);
				}

				break;
			// 4 perceivers
			case BuiltInCombatMoveType.BlockRange:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
					new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 3))
				{
					return (help, false);
				}

				break;
			// 4 perceivers with a null
			case BuiltInCombatMoveType.Dodge:
			case BuiltInCombatMoveType.DesperateDodge:
			case BuiltInCombatMoveType.AdvanceAndFire:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
					new DummyPerceivable(), null, new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 3))
				{
					return (help, false);
				}

				break;
			// 5 perceivers
			case BuiltInCombatMoveType.Parry:
			case BuiltInCombatMoveType.Block:
			case BuiltInCombatMoveType.DesperateParry:
			case BuiltInCombatMoveType.DesperateBlock:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
					new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 4))
				{
					return (help, false);
				}

				break;
			case BuiltInCombatMoveType.ReceiveCharge:
				break;
			// 5 perceivers with a null
			case BuiltInCombatMoveType.WardDefense:
			case BuiltInCombatMoveType.WardFreeAttack:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
					new DummyPerceivable(), new DummyPerceivable(), null);
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 4))
				{
					return (help, false);
				}

				break;
			// 5 perceivers with 3 nulls
			case BuiltInCombatMoveType.WardFreeUnarmedAttack:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), null,
					null, null);
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 4))
				{
					return (help, false);
				}

				break;
			case BuiltInCombatMoveType.CoupDeGrace:
				break;
			// 3 perceivers with a null
			case BuiltInCombatMoveType.UnarmedSmashItem:
				emote = new Emote(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), null);
				if (!emote.Valid)
				{
					return (help, false);
				}

				if (FormatStringRegex.Matches(text).OfType<Match>().Select(x => int.Parse(x.Groups["number"].Value))
				                     .Any(x => x < 0 || x > 2))
				{
					return (help, false);
				}

				break;
			case BuiltInCombatMoveType.DodgeExtendGrapple:
				break;
			case BuiltInCombatMoveType.BeamAttack:
				break;
			case BuiltInCombatMoveType.OverpowerGrapple:
				break;
			case BuiltInCombatMoveType.StrangleAttackExtendGrapple:
				break;

			default:
				return ("That type has not been set up properly to work with the building commands, sorry.", false);
		}

		return (GetHelpText(type), true);
	}

	private bool BuildingCommandMessage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a combat message.\n{VetEmote(Type, string.Empty)}");
			return false;
		}

		var (help, valid) = VetEmote(Type, command.SafeRemainingArgument);
		if (!valid)
		{
			actor.OutputHandler.Send($"Your supplied combat message is not valid.\n{help}");
			return false;
		}

		Message = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This combat message's success message is now {Message.Colour(Telnet.Yellow)}");
		return true;
	}

	private bool BuildingCommandFail(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a combat message.\n{VetEmote(Type, string.Empty)}");
			return false;
		}

		var (help, valid) = VetEmote(Type, command.SafeRemainingArgument);
		if (!valid)
		{
			actor.OutputHandler.Send($"Your supplied combat message is not valid.\n{help}");
			return false;
		}

		FailMessage = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This combat message's failure message is now {Message.Colour(Telnet.Yellow)}");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog to use to filter this combat message, or use 'none' to clear an existing one.");
			return false;
		}

		var prog = long.TryParse(command.PopSafe(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns boolean.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes>
		    {
			    FutureProgVariableTypes.Character,
			    FutureProgVariableTypes.Perceiver,
			    FutureProgVariableTypes.Item,
			    FutureProgVariableTypes.Number,
			    FutureProgVariableTypes.Text,
			    FutureProgVariableTypes.Text
		    }))
		{
			actor.OutputHandler.Send(
				"The prog must use the following parameter signature: character, perceiver, item, number, text, text");
			return false;
		}

		Prog = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This combat message will now use the {prog.MXPClickableFunctionNameWithId()} prog to filter potential use.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter an integer value zero or higher.");
			return false;
		}

		Priority = value;
		Changed = true;
		actor.OutputHandler.Send($"This combat message now has a priority of {Priority.ToString("N0", actor)}.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What attack verb should this combat message be associated with? Use NONE to clear.");
			return false;
		}

		if (!Type.IsWeaponAttackType())
		{
			actor.OutputHandler.Send(
				$"This combat message currently applies to {Type.Describe().Colour(Telnet.Cyan)} attacks, and they do not use verbs.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			Verb = null;
			Changed = true;
			actor.OutputHandler.Send("This combat message will now apply to all attack verbs.");
			return true;
		}

		if (!Enum.TryParse<MeleeWeaponVerb>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid verb. Valid verbs are: {Enum.GetValues(typeof(MeleeWeaponVerb)).Cast<MeleeWeaponVerb>().Select(x => x.Describe().Colour(Telnet.Cyan)).ListToString()}");
			return false;
		}

		Verb = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This combat message will now only apply when the attack's verb is {Verb.Value.Describe().Colour(Telnet.Cyan)}.");
		if (WeaponAttackIds.Any())
		{
			var removed = new HashSet<long>();
			foreach (var item in WeaponAttackIds)
			{
				if (actor.Gameworld.WeaponAttacks.Get(item).Verb != Verb)
				{
					removed.Add(item);
				}
			}

			WeaponAttackIds.RemoveWhere(x => removed.Contains(x));
			actor.OutputHandler.Send(
				$"Warning, the following weapon attacks were removed as they were no longer valid: {removed.Select(x => x.ToString("N0", actor)).ListToString()}"
					.Colour(Telnet.Red));
		}

		return true;
	}

	private bool BuildingCommandAttack(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which attack ID do you want to add or remove from this combat message?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid ID.");
			return false;
		}

		var attack = actor.Gameworld.WeaponAttacks.Get(value);
		if (attack == null)
		{
			actor.OutputHandler.Send("That is not a valid weapon attack.");
			return false;
		}

		if (WeaponAttackIds.Contains(value))
		{
			WeaponAttackIds.Remove(value);
			actor.OutputHandler.Send(
				$"This combat message will no longer specifically apply to weapon attack #{value.ToString("N0", actor)} ({attack.Name.Colour(Telnet.Cyan)})");
		}
		else
		{
			WeaponAttackIds.Add(value);
			actor.OutputHandler.Send(
				$"This combat message will now specifically apply to weapon attack #{value.ToString("N0", actor)} ({attack.Name.Colour(Telnet.Cyan)})");
		}

		Changed = true;
		return true;
	}
}