using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class CombatRecklessnessMerit : CharacterMeritBase, ICombatRecklessnessMerit
{
	protected CombatRecklessnessMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	protected CombatRecklessnessMerit()
	{
	}

	protected CombatRecklessnessMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Combat Recklessness", "@ are|is recklessly disregarding &0's own safety")
	{
		DoDatabaseInsert();
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Combat Recklessness",
			(merit, gameworld) => new CombatRecklessnessMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Combat Recklessness", (gameworld, name) => new CombatRecklessnessMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Combat Recklessness", "Makes a character unable to flee", new CombatRecklessnessMerit().HelpText);
	}
}