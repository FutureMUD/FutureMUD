using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Health.Wounds;
using System;

namespace MudSharp.Health;

public static class WoundFactory
{
	public static IWound LoadWound(Wound wound, IHaveWounds owner, IFuturemud gameworld)
	{
		switch (wound.WoundType)
		{
			case "HealingSimple":
				return new HealingSimpleWound(owner, wound, gameworld);
			case "Simple":
				return new SimpleWound(owner, wound, gameworld);
			case "SimpleOrganic":
				return new SimpleOrganicWound(owner, wound, gameworld);
			case "BoneFracture":
				return new BoneFracture(owner, wound, gameworld);
			case "Robot":
				return new RobotWound(owner, wound, gameworld);
		}

		throw new ApplicationException("Unknown WoundType in WoundFactory.LoadWound - " + wound.WoundType);
	}
}