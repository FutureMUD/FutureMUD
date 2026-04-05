using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Merits.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Needs;

/// <summary>
///     An active needs model has hunger and thirst, like a PC
/// </summary>
public class ActiveNeedsModel : ChangingNeedsModelBase
{
    public ActiveNeedsModel(MudSharp.Models.Character dbcharacter, ICharacter character)
    {
        Owner = character;
        DrinkSatiatedHours = dbcharacter.DrinkSatiatedHours;
        FoodSatiatedHours = dbcharacter.FoodSatiatedHours;
        AlcoholLitres = dbcharacter.AlcoholLitres;
        WaterLitres = dbcharacter.WaterLitres;
        SatiationReserve = dbcharacter.SatiationReserve;
    }

    public ActiveNeedsModel(ICharacter character)
    {
        Owner = character;
        DrinkSatiatedHours = 24;
        FoodSatiatedHours = 24;
        AlcoholLitres = 0;
        WaterLitres = 3.0;
        SatiationReserve = 0.0;
    }

    public override void NeedsHeartbeat()
    {
        List<INeedRateChangingMerit> ownerMerits = Owner.Merits.OfType<INeedRateChangingMerit>().Where(x => x.Applies(Owner)).ToList();
        List<INeedRateEffect> effects = Owner.CombinedEffectsOfType<INeedRateEffect>().Where(x => x.AppliesToPassive).ToList();
        double hungerMult = effects.Aggregate(1.0, (x, y) => x * y.HungerMultiplier);
        double thirstMult = effects.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier);
        double drunkMult = effects.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier);
        NeedsResult oldStatus = Status;
        double hoursPassed = 1 / 60.0 * RealSecondsToInGameSeconds;

        DrinkSatiatedHours -= hoursPassed * Owner.Race.ThirstRate *
                              ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier) * thirstMult;
        FoodSatiatedHours -= hoursPassed * Owner.Race.HungerRate *
                             ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier) * hungerMult;
        AlcoholLitres -= hoursPassed * Owner.Body.LiverAlcoholRemovalKilogramsPerHour *
                         ownerMerits.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier) * drunkMult;
        WaterLitres -= hoursPassed * Owner.Body.WaterLossLitresPerHour *
                       ownerMerits.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier) * thirstMult;

        double satiationUse = hoursPassed * Owner.Race.HungerRate *
                           ownerMerits.Aggregate(1.0, (x, y) => x * y.HungerMultiplier) * hungerMult;
        SpendSatiationReserve(satiationUse, false);

        double starvationMultiplier = GetStarvationSatiationDeficitMultiplier(StarvationLevel);
        if (starvationMultiplier > 0.0)
        {
            SpendSatiationReserve(satiationUse * starvationMultiplier, true);
        }

        double exertionMultiplier = GetExertionSatiationBurnMultiplier(Owner.Body.LongtermExertion);
        if (exertionMultiplier > 0.0)
        {
            SpendSatiationReserve(satiationUse * exertionMultiplier, false);
        }

        NeedsChanged(oldStatus, true, true, false);
    }
}
