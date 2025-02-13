using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace MudSharp.Combat.Arenas
{
	public class CombatArenaCombat : SimpleMeleeCombat
	{
		private ICombatArena CombatArena { get; }
		private IArenaMatch ArenaMatch { get; }
		public CombatArenaCombat(ICombatArena combatArena, IArenaMatch arenaMatch) : base(combatArena.Gameworld)
		{
			CombatArena = combatArena;
			ArenaMatch = arenaMatch;
		}

		#region Overrides of CombatBase

		/// <inheritdoc />
		public override string CombatHeaderDescription { get; }

		/// <inheritdoc />
		protected override void HandleCombatResult(IPerceiver perceiver, ICombatMove move, ICombatMove response, CombatMoveResult result)
		{
			var ch = perceiver as ICharacter;
			var tch = response?.Assailant;
			if (ch is null || tch is null)
			{
				return;
			}

			ArenaMatch.HandleScore(ch, tch, move, result);
		}

		#endregion
	}
}
