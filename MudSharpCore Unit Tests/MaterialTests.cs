using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Climate;
using MudSharp.GameItems.Components;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MaterialTests
{
	private int SecondsToEvaporate(double initial)
	{
		for (var i = 0; i < 864000; i++)
		{
			initial -= PuddleGameItemComponent.EvaporationRatePerSecond(initial, 1.0, 25.0, PrecipitationLevel.Dry,
				WindLevel.Breeze);
			if (initial <= 0.0)
			{
				return i;
			}
		}

		return 864000;
	}

	[TestMethod]
	public void TestEvaporation()
	{
		var evaporation0 =
			PuddleGameItemComponent.EvaporationRatePerSecond(1000.0, 1.0, 25.0, PrecipitationLevel.Dry,
				WindLevel.Breeze);
		var evaporation1 =
			PuddleGameItemComponent.EvaporationRatePerSecond(10.0, 1.0, 25.0, PrecipitationLevel.Dry,
				WindLevel.Breeze);
		var evaporation2 =
			PuddleGameItemComponent.EvaporationRatePerSecond(1.0, 1.0, 25.0, PrecipitationLevel.Dry,
				WindLevel.Breeze);
		var evaporation3 =
			PuddleGameItemComponent.EvaporationRatePerSecond(0.011, 1.0, 25.0, PrecipitationLevel.Dry,
				WindLevel.Breeze);

		var seconds0 = 1000.0 / evaporation0;
		var seconds1 = 10.0 / evaporation1;
		var seconds2 = 1.0 / evaporation2;
		var seconds3 = 0.011 / evaporation3;

		Dictionary<double, int> secondsDictionary = new();
		secondsDictionary[0.011] = SecondsToEvaporate(0.011);
		secondsDictionary[0.1] = SecondsToEvaporate(0.1);
		secondsDictionary[0.4] = SecondsToEvaporate(0.4);
		secondsDictionary[1.0] = SecondsToEvaporate(1.0);
		secondsDictionary[3.5] = SecondsToEvaporate(3.5);
		secondsDictionary[15] = SecondsToEvaporate(15);
		secondsDictionary[150] = SecondsToEvaporate(150);
		secondsDictionary[1500] = SecondsToEvaporate(1500);

		Assert.IsTrue(true);
	}
}