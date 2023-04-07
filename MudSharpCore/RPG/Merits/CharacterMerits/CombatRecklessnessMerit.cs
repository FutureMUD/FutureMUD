using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class CombatRecklessnessMerit : CharacterMeritBase, ICombatRecklessnessMerit
{
	protected CombatRecklessnessMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Combat Recklessness",
			(merit, gameworld) => new CombatRecklessnessMerit(merit, gameworld));
	}
}