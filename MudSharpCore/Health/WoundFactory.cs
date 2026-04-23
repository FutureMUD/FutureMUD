using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using System;

namespace MudSharp.Health;

public static class WoundFactory
{
    public static IWound LoadWound(Wound wound, IHaveWounds owner, IFuturemud gameworld, IBody ownerBody = null)
    {
        switch (wound.WoundType)
        {
            case "HealingSimple":
                return new HealingSimpleWound(owner, wound, gameworld, ownerBody);
            case "Simple":
                return new SimpleWound(owner, wound, gameworld, ownerBody);
            case "SimpleOrganic":
                return new SimpleOrganicWound(owner, wound, gameworld, ownerBody);
            case "BoneFracture":
                return new BoneFracture(owner, wound, gameworld, ownerBody);
            case "Robot":
                return new RobotWound(owner, wound, gameworld, ownerBody);
        }

        throw new ApplicationException("Unknown WoundType in WoundFactory.LoadWound - " + wound.WoundType);
    }
}
