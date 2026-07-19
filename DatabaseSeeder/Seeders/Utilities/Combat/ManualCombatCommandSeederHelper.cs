using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static class ManualCombatCommandSeederHelper
{
	public static int EnsureStockManualCombatCommands(FuturemudDatabaseContext context)
	{
		var changes = 0;
		var bash = context.CombatActions
		                  .FirstOrDefault(x => x.Name == "Bash" &&
		                                       x.MoveType == (int)BuiltInCombatMoveType.AuxiliaryMove);
		if (bash is not null)
		{
			changes += EnsureManualCommand(
				context,
				name: "Bash",
				primaryVerb: "bash",
				actionKind: ManualCombatActionKind.AuxiliaryAction,
				weaponAttackId: null,
				combatActionId: bash.Id,
				defaultAiMultiplier: 1.25);
		}

		var kick = context.WeaponAttacks
		                  .FirstOrDefault(x => x.Name == "Snap Kick" &&
		                                       x.MoveType == (int)BuiltInCombatMoveType.NaturalWeaponAttack) ??
		           context.WeaponAttacks
		                  .FirstOrDefault(x => x.Verb == (int)MeleeWeaponVerb.Kick &&
		                                       x.MoveType == (int)BuiltInCombatMoveType.NaturalWeaponAttack);
		if (kick is not null)
		{
			changes += EnsureManualCommand(
				context,
				name: "Kick",
				primaryVerb: "kick",
				actionKind: ManualCombatActionKind.WeaponAttack,
				weaponAttackId: kick.Id,
				combatActionId: null,
				defaultAiMultiplier: 1.0);
		}

		context.SaveChanges();
		return changes;
	}

	private static int EnsureManualCommand(
		FuturemudDatabaseContext context,
		string name,
		string primaryVerb,
		ManualCombatActionKind actionKind,
		long? weaponAttackId,
		long? combatActionId,
		double defaultAiMultiplier)
	{
		var command = context.ManualCombatCommands
		                     .FirstOrDefault(x => x.PrimaryVerb == primaryVerb) ??
		              context.ManualCombatCommands
		                     .FirstOrDefault(x => x.Name == name);
		if (command is null)
		{
			context.ManualCombatCommands.Add(new ManualCombatCommand
			{
				Name = name,
				PrimaryVerb = primaryVerb,
				AdditionalVerbs = string.Empty,
				ActionKind = (int)actionKind,
				WeaponAttackId = weaponAttackId,
				CombatActionId = combatActionId,
				PlayerUsable = true,
				NpcUsable = true,
				UsabilityProgId = null,
				CooldownSeconds = 0.0,
				CooldownMessage = "You must wait a short time before doing that again.",
				DefaultAiWeightMultiplier = defaultAiMultiplier
			});
			return 1;
		}

		var changed = false;
		if (command.Name != name)
		{
			command.Name = name;
			changed = true;
		}

		if (command.PrimaryVerb != primaryVerb)
		{
			command.PrimaryVerb = primaryVerb;
			changed = true;
		}

		if (command.ActionKind != (int)actionKind)
		{
			command.ActionKind = (int)actionKind;
			changed = true;
		}

		if (command.WeaponAttackId != weaponAttackId)
		{
			command.WeaponAttackId = weaponAttackId;
			changed = true;
		}

		if (command.CombatActionId != combatActionId)
		{
			command.CombatActionId = combatActionId;
			changed = true;
		}

		if (!command.PlayerUsable)
		{
			command.PlayerUsable = true;
			changed = true;
		}

		if (!command.NpcUsable)
		{
			command.NpcUsable = true;
			changed = true;
		}

		if (command.DefaultAiWeightMultiplier <= 0.0)
		{
			command.DefaultAiWeightMultiplier = defaultAiMultiplier;
			changed = true;
		}

		return changed ? 1 : 0;
	}
}
