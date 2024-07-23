using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AmbidextrousMerit : CharacterMeritBase
{
	public AmbidextrousMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	protected AmbidextrousMerit()
	{
	}

	protected AmbidextrousMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Ambidextrous", "@ are|is ambidextrous")
	{
		DoDatabaseInsert();
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Ambidextrous",
			(merit, gameworld) => new AmbidextrousMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Ambidextrous", (gameworld, name) => new AmbidextrousMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Ambidextrous", "Has no penalty for off-hand use", new AmbidextrousMerit().HelpText);
	}
}