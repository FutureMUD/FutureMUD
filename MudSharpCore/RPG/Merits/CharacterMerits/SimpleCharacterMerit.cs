using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SimpleCharacterMerit : CharacterMeritBase
{
	protected SimpleCharacterMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Simple Character",
			(merit, gameworld) => new SimpleCharacterMerit(merit, gameworld));
	}
}