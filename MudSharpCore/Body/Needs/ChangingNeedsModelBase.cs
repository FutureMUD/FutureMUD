using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Needs;

public abstract class ChangingNeedsModelBase : INeedsModel
{
    public static double StaticRealSecondsToInGameSeconds { get; set; }
    protected double RealSecondsToInGameSeconds => StaticRealSecondsToInGameSeconds;

    public ICharacter Owner { get; init; }
    protected double FoodSatiationLimit => GetEffectiveFoodSatiationLimit(
        Owner?.Race?.MaximumFoodSatiatedHours ?? RacialSatiationDefaults.MaximumFoodSatiatedHours);
    protected double DrinkSatiationLimit => GetEffectiveDrinkSatiationLimit(
        Owner?.Race?.MaximumDrinkSatiatedHours ?? RacialSatiationDefaults.MaximumDrinkSatiatedHours);

    #region INeedsModel Members

    public NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
    {
        NeedsResult oldStatus = Status;

        List<INeedRateEffect> effects = Owner.CombinedEffectsOfType<INeedRateEffect>().Where(x => x.AppliesToActive).ToList();
        double hungerMult = effects.Aggregate(1.0, (x, y) => x * y.HungerMultiplier);
        double thirstMult = effects.Aggregate(1.0, (x, y) => x * y.ThirstMultiplier);
        double drunkMult = effects.Aggregate(1.0, (x, y) => x * y.DrunkennessMultiplier);
        double satiationDelta = fulfiller.SatiationPoints * hungerMult;
        double thirstDelta = fulfiller.ThirstPoints * thirstMult;
        double previousFoodSatiatedHours = FoodSatiatedHours;

        WaterLitres += fulfiller.WaterLitres;
        FoodSatiatedHours += satiationDelta;
        DrinkSatiatedHours += thirstDelta;
        SatiationReserve =
            ApplySatiationReserveFromFulfiller(SatiationReserve, previousFoodSatiatedHours, satiationDelta,
                FoodSatiationLimit);
        if (!ignoreDelays && fulfiller.AlcoholLitres > 0.0)
        {
            AlcoholLitres += fulfiller.AlcoholLitres * 0.25 * drunkMult;
            TimeSpan timespan = TimeSpan.FromMinutes(Math.Max(1.0,
                15.0 * (1.0 + FoodSatiatedHours / GetFoodAbsolutelyStuffedThreshold(FoodSatiationLimit)) *
                RealSecondsToInGameSeconds));
            Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
                new NeedFulfiller
                {
                    AlcoholLitres = fulfiller.AlcoholLitres * 0.25 * drunkMult
                }
            ), timespan);
            Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
                new NeedFulfiller
                {
                    AlcoholLitres = fulfiller.AlcoholLitres * 0.25 * drunkMult
                }
            ), timespan + timespan);
            Owner.Body.AddEffect(new DelayedNeedsFulfillment(Owner.Body,
                new NeedFulfiller
                {
                    AlcoholLitres = fulfiller.AlcoholLitres * 0.25 * drunkMult
                }
            ), timespan + timespan + timespan);
        }
        else
        {
            AlcoholLitres += fulfiller.AlcoholLitres * drunkMult;
        }

        NormaliseValues();

        return NeedsChanged(oldStatus, fulfiller.SatiationPoints < 0, fulfiller.ThirstPoints < 0,
            fulfiller.AlcoholLitres > 0);
    }

    protected void NormaliseValues()
    {
        double foodLimit = FoodSatiationLimit;
        if (FoodSatiatedHours > foodLimit)
        {
            FoodSatiatedHours = foodLimit;
        }

        double foodMinimum = -0.25 * foodLimit;
        if (FoodSatiatedHours < foodMinimum)
        {
            FoodSatiatedHours = foodMinimum;
        }

        double drinkLimit = DrinkSatiationLimit;
        if (DrinkSatiatedHours > drinkLimit)
        {
            DrinkSatiatedHours = drinkLimit;
        }

        double drinkMinimum = -0.5 * drinkLimit;
        if (DrinkSatiatedHours < drinkMinimum)
        {
            DrinkSatiatedHours = drinkMinimum;
        }

        if (AlcoholLitres < 0)
        {
            AlcoholLitres = 0;
        }

        if (WaterLitres > Owner.Body.CurrentBloodVolumeLitres / 6.0)
        {
            WaterLitres = Owner.Body.CurrentBloodVolumeLitres / 6.0;
        }
    }

    protected NeedsResult NeedsChanged(NeedsResult oldStatus, bool hungrier, bool thirstier, bool drunker)
    {
        NeedsResult newStatus = Status;

        // Food Messages
        bool foodChanged = (oldStatus & NeedsResult.HungerOnly) != (newStatus & NeedsResult.HungerOnly);
        switch (newStatus & NeedsResult.HungerOnly)
        {
            case NeedsResult.Starving:
                if (foodChanged)
                {
                    Owner.Send("You are starving!".ColourBold(Telnet.Red));
                }

                break;
            case NeedsResult.Hungry:
                if (foodChanged)
                {
                    Owner.Send(hungrier
                        ? "You are starting to feel quite hungry.".Colour(Telnet.Red)
                        : "You are not starving any more, but still quite hungry.".Colour(Telnet.Red));
                }

                break;
            case NeedsResult.Peckish:
                if (foodChanged)
                {
                    Owner.Send(hungrier
                        ? "You are starting to feel a bit peckish."
                        : "You now feel merely a little peckish.");
                }

                break;
            case NeedsResult.Full:
                if (foodChanged)
                {
                    Owner.Send(hungrier ? "You no longer feel absolutely stuffed." : "You feel that you are full.");
                }

                break;
            case NeedsResult.AbsolutelyStuffed:
                if (foodChanged)
                {
                    Owner.Send("You feel absolutely stuffed!");
                }

                break;
        }

        // Drink Messages
        bool drinkChanged = (oldStatus & NeedsResult.ThirstOnly) != (newStatus & NeedsResult.ThirstOnly);
        switch (newStatus & NeedsResult.ThirstOnly)
        {
            case NeedsResult.Parched:
                if (drinkChanged)
                {
                    Owner.Send("You are extremely parched!".ColourBold(Telnet.Red));
                }

                break;
            case NeedsResult.Thirsty:
                if (drinkChanged)
                {
                    Owner.Send(thirstier
                        ? "You are starting to feel quite thirsty.".Colour(Telnet.Red)
                        : "You are not parched any more, but still quite thirsty.".Colour(Telnet.Red));
                }

                break;
            case NeedsResult.NotThirsty:
                if (drinkChanged)
                {
                    Owner.Send(thirstier ? "You no longer feel totally sated." : "You no longer feel thirsty.");
                }

                break;
            case NeedsResult.Sated:
                if (drinkChanged)
                {
                    Owner.Send("You feel completely sated!");
                }

                break;
        }

        // Alcohol Messages
        bool alcoholChanged = (oldStatus & NeedsResult.DrunkOnly) != (newStatus & NeedsResult.DrunkOnly);
        switch (newStatus & NeedsResult.DrunkOnly)
        {
            case NeedsResult.Sober:
                if (alcoholChanged)
                {
                    Owner.Send("You feel as if you are now completely sober.");
                }

                break;
            case NeedsResult.Buzzed:
                if (alcoholChanged)
                {
                    Owner.Send(drunker
                        ? "You are starting to feel pleasantly buzzed."
                        : "You no longer feel tipsy and feel only mildly buzzed.");
                }

                break;
            case NeedsResult.Tipsy:
                if (alcoholChanged)
                {
                    Owner.Send(drunker
                        ? "You are starting to feel a little tipsy."
                        : "You feel as if you have sobered up to the point where you are no longer drunk.");
                }

                break;
            case NeedsResult.Drunk:
                if (alcoholChanged)
                {
                    Owner.Send(drunker
                        ? "You are starting to feel comfortably drunk."
                        : "You feel as if you have sobered up to the point where you are no longer extremely drunk.");
                }

                break;
            case NeedsResult.VeryDrunk:
                if (alcoholChanged)
                {
                    Owner.Send(drunker
                        ? "You are starting to feel extremely drunk."
                        : "You feel as if you have sobered up to the point where you are no longer blackout drunk.");
                }

                break;
            case NeedsResult.BlackoutDrunk:
                if (alcoholChanged)
                {
                    Owner.Send(drunker
                        ? "You are now in the range of blackout drunk, and are unlikely to remember much of what is going on."
                        : "You feel as if you have sobered up to the point where you are no longer paralytic.");
                }

                break;
            case NeedsResult.Paralytic:
                if (alcoholChanged)
                {
                    Owner.Send("You are absolutely paralytic!");
                }

                break;
        }

        return newStatus;
    }

    internal static double CalculateStarvationLevel(double foodSatiatedHours)
    {
        return Math.Max(0.0, -foodSatiatedHours);
    }

    internal static double GetEffectiveFoodSatiationLimit(double configuredLimit)
    {
        return GetEffectiveSatiationLimit(configuredLimit, RacialSatiationDefaults.MaximumFoodSatiatedHours);
    }

    internal static double GetEffectiveDrinkSatiationLimit(double configuredLimit)
    {
        return GetEffectiveSatiationLimit(configuredLimit, RacialSatiationDefaults.MaximumDrinkSatiatedHours);
    }

    private static double GetEffectiveSatiationLimit(double configuredLimit, double fallbackLimit)
    {
        if (double.IsNaN(configuredLimit) || double.IsInfinity(configuredLimit) || configuredLimit <= 0.0)
        {
            return fallbackLimit;
        }

        return configuredLimit;
    }

    internal static double GetFoodAbsolutelyStuffedThreshold(double maximumFoodSatiatedHours =
        RacialSatiationDefaults.MaximumFoodSatiatedHours)
    {
        return GetEffectiveFoodSatiationLimit(maximumFoodSatiatedHours) * 0.75;
    }

    internal static NeedsResult GetHungerStatus(double foodSatiatedHours,
        double maximumFoodSatiatedHours = RacialSatiationDefaults.MaximumFoodSatiatedHours)
    {
        double foodLimit = GetEffectiveFoodSatiationLimit(maximumFoodSatiatedHours);
        if (foodSatiatedHours >= foodLimit * 0.75)
        {
            return NeedsResult.AbsolutelyStuffed;
        }

        if (foodSatiatedHours >= foodLimit * 0.5)
        {
            return NeedsResult.Full;
        }

        if (foodSatiatedHours >= foodLimit * 0.25)
        {
            return NeedsResult.Peckish;
        }

        return foodSatiatedHours > 0.0 ? NeedsResult.Hungry : NeedsResult.Starving;
    }

    internal static NeedsResult GetThirstStatus(double drinkSatiatedHours,
        double maximumDrinkSatiatedHours = RacialSatiationDefaults.MaximumDrinkSatiatedHours)
    {
        double drinkLimit = GetEffectiveDrinkSatiationLimit(maximumDrinkSatiatedHours);
        if (drinkSatiatedHours >= drinkLimit * 0.75)
        {
            return NeedsResult.Sated;
        }

        if (drinkSatiatedHours >= drinkLimit * 0.5)
        {
            return NeedsResult.NotThirsty;
        }

        return drinkSatiatedHours > 0.0 ? NeedsResult.Thirsty : NeedsResult.Parched;
    }

    internal static double CalculateOversatiationLevel(double foodSatiatedHours,
        double maximumFoodSatiatedHours = RacialSatiationDefaults.MaximumFoodSatiatedHours)
    {
        return Math.Max(0.0, foodSatiatedHours - GetFoodAbsolutelyStuffedThreshold(maximumFoodSatiatedHours));
    }

    internal static double ApplySatiationReserveFromFulfiller(double currentReserve, double previousFoodSatiatedHours,
        double satiationDelta,
        double maximumFoodSatiatedHours = RacialSatiationDefaults.MaximumFoodSatiatedHours)
    {
        if (satiationDelta <= 0.0)
        {
            return currentReserve + satiationDelta;
        }

        double originalSatiationDelta = satiationDelta;
        if (currentReserve < 0.0)
        {
            double deficitRecovery = Math.Min(-currentReserve, satiationDelta);
            currentReserve += deficitRecovery;
            satiationDelta -= deficitRecovery;
        }

        if (satiationDelta <= 0.0)
        {
            return currentReserve;
        }

        double oversatiationBefore = CalculateOversatiationLevel(previousFoodSatiatedHours, maximumFoodSatiatedHours);
        double oversatiationAfter =
            CalculateOversatiationLevel(previousFoodSatiatedHours + originalSatiationDelta, maximumFoodSatiatedHours);
        double oversatiationGain = Math.Max(0.0, oversatiationAfter - oversatiationBefore);
        if (oversatiationGain <= 0.0)
        {
            return currentReserve;
        }

        double oversatiationFraction = Math.Min(1.0, oversatiationGain / originalSatiationDelta);
        return currentReserve + satiationDelta * oversatiationFraction;
    }

    internal static double GetStarvationSatiationDeficitMultiplier(double starvationLevel)
    {
        if (starvationLevel <= 0.0)
        {
            return 0.0;
        }

        return Math.Max(0.25, Math.Min(1.0, starvationLevel));
    }

    internal static double GetExertionSatiationBurnMultiplier(ExertionLevel exertion)
    {
        return exertion switch
        {
            ExertionLevel.Heavy => 0.5,
            ExertionLevel.VeryHeavy => 1.0,
            ExertionLevel.ExtremelyHeavy => 1.5,
            _ => 0.0
        };
    }

    protected void SpendSatiationReserve(double amount, bool allowDeficit)
    {
        if (amount <= 0.0)
        {
            return;
        }

        if (allowDeficit)
        {
            SatiationReserve -= amount;
            return;
        }

        if (SatiationReserve <= 0.0)
        {
            return;
        }

        SatiationReserve = Math.Max(0.0, SatiationReserve - amount);
    }

    public NeedsResult Status
    {
        get
        {
            NeedsResult result = NeedsResult.None;
            result |= GetHungerStatus(FoodSatiatedHours, FoodSatiationLimit);
            result |= GetThirstStatus(DrinkSatiatedHours, DrinkSatiationLimit);

            double bac = 10.0 * AlcoholLitres / Owner.Body.CurrentBloodVolumeLitres;
            if (bac >= 0.25)
            {
                result |= NeedsResult.Paralytic;
            }
            else if (bac >= 0.16)
            {
                result |= NeedsResult.BlackoutDrunk;
            }
            else if (bac >= 0.12)
            {
                result |= NeedsResult.VeryDrunk;
            }
            else if (bac >= 0.08)
            {
                result |= NeedsResult.Drunk;
            }
            else if (bac >= 0.04)
            {
                result |= NeedsResult.Tipsy;
            }
            else if (bac >= 0.01)
            {
                result |= NeedsResult.Buzzed;
            }
            else
            {
                result |= NeedsResult.Sober;
            }

            return result;
        }
    }

    public abstract void NeedsHeartbeat();

    private double _alcoholLitres;

    public double AlcoholLitres
    {
        get => _alcoholLitres;
        set
        {
            _alcoholLitres = Math.Max(0.0, value);
            NeedsChanged(Status, false, false, false);
        }
    }

    public double WaterLitres { get; protected set; }

    public double FoodSatiatedHours { get; protected set; }

    public double DrinkSatiatedHours { get; protected set; }

    public double SatiationReserve { get; protected set; }

    public double StarvationLevel => CalculateStarvationLevel(FoodSatiatedHours);

    public double OversatiationLevel => CalculateOversatiationLevel(FoodSatiatedHours, FoodSatiationLimit);

    public double SatiationExcess => Math.Max(0.0, SatiationReserve);

    public double SatiationDeficit => Math.Max(0.0, -SatiationReserve);

    public virtual bool NeedsSave => true;

    #endregion
}
