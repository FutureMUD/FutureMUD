using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Body.Needs;

public static class NeedsExtensions
{
	public static double GeneralBonusLevel(this NeedsResult result)
	{
		var bonus = 0.0;
		switch (result & NeedsResult.HungerOnly)
		{
			case NeedsResult.Starving:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelStarving");
				break;
			case NeedsResult.Hungry:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelHungry");
				break;
			case NeedsResult.Peckish:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelPeckish");
				break;
		}

		switch (result & NeedsResult.ThirstOnly)
		{
			case NeedsResult.Parched:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelParched");
				break;
			case NeedsResult.Thirsty:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelThirsty");
				break;
		}

		switch (result & NeedsResult.DrunkOnly)
		{
			case NeedsResult.Drunk:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelDrunk");
				break;
			case NeedsResult.VeryDrunk:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelVeryDrunk");
				break;
			case NeedsResult.BlackoutDrunk:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelBlackoutDrunk");
				break;
			case NeedsResult.Paralytic:
				bonus += Futuremud.Games.First().GetStaticDouble("GeneralBonusLevelParalytic");
				break;
		}

		return bonus;
	}
}