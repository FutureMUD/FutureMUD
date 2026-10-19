#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Fresh early Combat content. Source outcome semantics with explicit authored native tuning.</summary>
internal static class ArmageddonSourceImpacts
{
	internal static void InstallFresh(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var always = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var humanoid = db.Races.Single(x => x.Name == "Organic Humanoid");
		var strength = db.TraitDefinitions.Single(x => x.Name == "Strength" && x.Type == 1).Id;
		var agility = db.TraitDefinitions.Single(x => x.Name == "Agility" && x.Type == 1).Id;
		foreach (var verb in new[] { "bash", "kick" })
		{
			var bash = verb == "bash";
			var trait = skills["arm.skill." + verb];
			var damage = bash ? $"2 + strength:{strength} * 0.1 + degree" : $"(1 + kick:{trait.Id} * 0.04) * (1 + degree * 0.2)";
			var counter = bash ? $"2 + strength:{strength} * 0.1" : "0";
			var profile = new XElement("PhysicalImpact",
				new XAttribute("countertrait", trait.Id), new XAttribute("guardtrait", skills["arm.skill.guarding"].Id),
				new XAttribute("agilitytrait", agility), new XAttribute("minimumstamina", 10), new XAttribute("sizebonus", 10),
				new XAttribute("counterdegree", 4), new XAttribute("uprighttarget", bash), new XAttribute("seatonmiss", bash),
				new XAttribute("seatonhit", bash), new XAttribute("seatoncounter", true), new XAttribute("shieldblock", !bash),
				new XAttribute("initiatecombat", true), new XAttribute("allowuntrained", !bash), new XAttribute("usableleg", !bash), new XAttribute("successdelay", bash ? 2 : 0),
				new XElement("Damage", damage), new XElement("Stun", damage), new XElement("CounterDamage", counter), new XElement("CounterStun", counter));
			var action = new CombatAction
			{
				Name = "Armageddon " + (bash ? "Bash" : "Kick"), MoveType = (int)BuiltInCombatMoveType.AuxiliaryMove,
				TraitDefinitionId = trait.Id, UsabilityProgId = always, MaximumTargets = 1,
				Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun | (bash ? CombatMoveIntentions.Disadvantage : CombatMoveIntentions.None)),
				MoveDifficulty = (int)Difficulty.Normal, RecoveryDifficultySuccess = (int)Difficulty.Easy,
				RecoveryDifficultyFailure = (int)Difficulty.Hard, ExertionLevel = (int)ExertionLevel.Heavy,
				StaminaCost = 5, BaseDelay = bash ? 1.2 : 1.0, Weighting = 100, RequiredPositionStateIds = "1 18",
				AdditionalInfo = new XElement("Root", profile).ToString()
			};
			db.CombatActions.Add(action); db.SaveChanges();
			ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.action." + verb, action,
				nameof(CombatAction.Name), nameof(CombatAction.TraitDefinitionId), nameof(CombatAction.AdditionalInfo), nameof(CombatAction.UsabilityProgId),
				nameof(CombatAction.MaximumTargets), nameof(CombatAction.BaseDelay), nameof(CombatAction.Weighting));
			db.RacesCombatActions.Add(new RacesCombatActions { RaceId = humanoid.Id, CombatActionId = action.Id });
			var message = new CombatMessage
			{
				Type = (int)BuiltInCombatMoveType.AuxiliaryMove, Priority = 10, Chance = 1.0,
				Message = bash ? "@ ram|rams into $1 with a forceful bash." : "@ land|lands a sharp kick on $1.",
				FailureMessage = bash ? "@ miss|misses $1 with a bash and loses footing." : "@ miss|misses $1 with a kick."
			};
			db.CombatMessages.Add(message); db.SaveChanges();
			db.CombatMessagesCombatActions.Add(new CombatMessagesCombatActions { CombatMessageId = message.Id, CombatActionId = action.Id });
			ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.action." + verb + ".message", message,
				nameof(CombatMessage.Message), nameof(CombatMessage.FailureMessage), nameof(CombatMessage.Priority), nameof(CombatMessage.Chance));
			var command = db.ManualCombatCommands.Single(x => x.PrimaryVerb == verb);
			command.ActionKind = (int)ManualCombatActionKind.AuxiliaryAction;
			command.CombatActionId = action.Id; command.WeaponAttackId = null;
			command.PlayerUsable = true; command.NpcUsable = true; command.UsabilityProgId = always;
			ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.command." + verb, command,
				nameof(ManualCombatCommand.ActionKind), nameof(ManualCombatCommand.CombatActionId), nameof(ManualCombatCommand.WeaponAttackId),
				nameof(ManualCombatCommand.PlayerUsable), nameof(ManualCombatCommand.NpcUsable), nameof(ManualCombatCommand.UsabilityProgId));
		}
		db.SaveChanges();
	}
}
