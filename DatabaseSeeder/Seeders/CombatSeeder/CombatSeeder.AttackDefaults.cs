#nullable enable

using System.Linq;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
	internal static int EnsureSpecialAttackDefaults(FuturemudDatabaseContext context, SeedCombatMessageStyle style)
	{
		var added = 0;
		foreach (var (type, text) in new[]
		         {
			         (BuiltInCombatMoveType.MagicPowerAttack, "$0 direct|directs magical power at $1"),
			         (BuiltInCombatMoveType.CouchedLanceAttack, "$0 thrust|thrusts $2 at $1 while mounted"),
			         (BuiltInCombatMoveType.MountedWeaponAttack, "$0 attack|attacks $1 with $2 while mounted")
		         })
		{
			if (context.CombatMessages.Any(x => x.Type == (int)type && x.Outcome == null &&
			    x.Verb == null && x.ProgId == null && x.AuxiliaryProgId == null && x.Chance >= 1.0 &&
			    !x.CombatMessagesWeaponAttacks.Any() && !x.CombatMessagesCombatActions.Any())) continue;

			var message = CombatSeederMessageStyleHelper.FormatAttackMessage(text, style);
			context.CombatMessages.Add(new CombatMessage
			{
				Type = (int)type, Message = message, FailureMessage = message, Chance = 1.0, Priority = 0
			});
			added++;
		}

		context.SaveChanges();
		return added;
	}
}
