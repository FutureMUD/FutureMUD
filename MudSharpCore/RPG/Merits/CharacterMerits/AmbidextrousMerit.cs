using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AmbidextrousMerit : CharacterMeritBase
{
	public AmbidextrousMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Ambidextrous",
			(merit, gameworld) => new AmbidextrousMerit(merit, gameworld));
	}
}