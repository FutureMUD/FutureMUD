using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SimpleCharacterMerit : CharacterMeritBase
{
	protected SimpleCharacterMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	protected SimpleCharacterMerit(){}

	protected SimpleCharacterMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Simple Character", "@ have|has a miscellaneous quirk")
	{
		DoDatabaseInsert();
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Simple Character",
			(merit, gameworld) => new SimpleCharacterMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Simple Character", (gameworld, name) => new SimpleCharacterMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Simple Character", "Has no coded effect, but can be checked in progs or used by storytellers", new SimpleCharacterMerit().HelpText);
	}
}