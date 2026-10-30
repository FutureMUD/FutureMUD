using MudSharp.Body;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Effects.Concrete;

namespace MudSharp.Body.Needs;

public abstract class ChangingNeedsModelBase : INeedsModel
{
    public const double MinimumSatiationLimitHours = 0.1;
    public static double StaticRealSecondsToInGameSeconds { get; set; }
    protected double RealSecondsToInGameSeconds => StaticRealSecondsToInGameSeconds;

    public ICharacter Owner { get; init; }
    protected double FoodSatiationLimit => GetEffectiveFoodSatiationLimit(
        Owner?.Race?.MaximumFoodSatiatedHours ?? RacialSatiationDefaults.MaximumFoodSatiatedHours);
    protected double DrinkSatiationLimit => GetEffectiveDrinkSatiationLimit(
        Owner?.Race?.MaximumDrinkSatiatedHours ?? RacialSatiationDefaults.MaximumDrinkSatiatedHours);

    /// <summary>
    ///     Whether this model tracks thirst and hydration. Aquatic active-needs models still use
    ///     hunger and alcohol mechanics but deliberately remain sated.
    /// </summary>
    protected virtual bool TracksThirst => true;

    #region INeedsModel Members

    public NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
    {
        return FulfilNeedsCore(fulfiller, ignoreDelays, null, null);
    }

    internal (double Hunger, double Thirst, double Drunkenness) GetFulfilmentMultipliers()
    {
        var effects = Owner.CombinedEffectsOfType<INeedRateEffect>()
            .Where(x => x.Applies(Owner))
            .Where(x => x.AppliesToActive)
            .ToList();
        return (
            effects.Aggregate(1.0, (multiplier, effect) => multiplier * effect.HungerMultiplier),
            effects.Aggregate(1.0, (multiplier, effect) => multiplier * effect.ThirstMultiplier),
            effects.Aggregate(1.0, (multiplier, effect) => multiplier * effect.DrunkennessMultiplier));
    }

    public NeedsResult FulfilNeedsOnSelectedBody(INeedFulfiller fulfiller, IBody selectedBody, Func<bool> isCurrent)
    {
        ArgumentNullException.ThrowIfNull(selectedBody);
        ArgumentNullException.ThrowIfNull(isCurrent);
        return FulfilNeedsCore(fulfiller, false, selectedBody, isCurrent);
    }

    private NeedsResult FulfilNeedsCore(INeedFulfiller fulfiller, bool ignoreDelays, IBody selectedBody,
        Func<bool> isCurrent)
    {
        void EnsureSelected()
        {
            if (selectedBody is not null && (!ReferenceEquals(Owner.Body, selectedBody) ||
                !ReferenceEquals(selectedBody.NeedsModel, this) || !isCurrent()))
                throw new InvalidOperationException("The selected needs body or model changed during fulfilment.");
        }

        EnsureSelected();
        NeedsResult oldStatus = Status;

        var (hungerMult, thirstMult, drunkMult) = GetFulfilmentMultipliers();
        double satiationDelta = fulfiller.SatiationPoints * hungerMult;
        double thirstDelta = fulfiller.ThirstPoints * thirstMult;
        double alcoholDelta = fulfiller.AlcoholLitres * drunkMult;
        if (selectedBody is not null)
        {
            if (!double.IsFinite(satiationDelta) || !double.IsFinite(thirstDelta) || !double.IsFinite(alcoholDelta))
                throw new InvalidOperationException("The selected native needs multiplier is not finite.");
            if (fulfiller.ThirstPoints < 0)
                thirstDelta = Math.Clamp(thirstDelta, fulfiller.ThirstPoints, 0.0);
            if (fulfiller.AlcoholLitres > 0)
                alcoholDelta = Math.Clamp(alcoholDelta, 0.0, fulfiller.AlcoholLitres);
        }
        double previousFoodSatiatedHours = FoodSatiatedHours;

        EnsureSelected();
        if (TracksThirst)
        {
            WaterLitres += fulfiller.WaterLitres;
        }

        FoodSatiatedHours += satiationDelta;
        if (TracksThirst)
        {
            DrinkSatiatedHours += thirstDelta;
        }
        SatiationReserve =
            ApplySatiationReserveFromFulfiller(SatiationReserve, previousFoodSatiatedHours, satiationDelta,
                FoodSatiationLimit);
        EnsureSelected();
        if (!ignoreDelays && (selectedBody is null ? fulfiller.AlcoholLitres > 0.0 : alcoholDelta > 0.0))
        {
            AlcoholLitres += alcoholDelta * 0.25;
            EnsureSelected();
            TimeSpan timespan = TimeSpan.FromMinutes(Math.Max(1.0,
                15.0 * (1.0 + FoodSatiatedHours / GetFoodAbsolutelyStuffedThreshold(FoodSatiationLimit)) *
                RealSecondsToInGameSeconds));
            for (var i = 1; i <= 3; i++)
            {
                EnsureSelected();
                var body = selectedBody ?? Owner.Body;
                body.AddEffect(new DelayedNeedsFulfillment(body,
                    new NeedFulfiller { AlcoholLitres = alcoholDelta * 0.25 }),
                    TimeSpan.FromTicks(checked(timespan.Ticks * i)));
                EnsureSelected();
            }
        }
        else
        {
            AlcoholLitres += alcoholDelta;
            EnsureSelected();
        }

        EnsureSelected();
        NormaliseValues(selectedBody);
        EnsureSelected();

        var result = NeedsChanged(oldStatus, fulfiller.SatiationPoints < 0, TracksThirst && fulfiller.ThirstPoints < 0,
            selectedBody is null ? fulfiller.AlcoholLitres > 0 : alcoholDelta > 0);
        EnsureSelected();
        return result;
    }

    protected void NormaliseValues(IBody selectedBody = null)
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

        if (TracksThirst)
        {
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

            if (WaterLitres > (selectedBody ?? Owner.Body).CurrentBloodVolumeLitres / 6.0)
            {
                WaterLitres = (selectedBody ?? Owner.Body).CurrentBloodVolumeLitres / 6.0;
            }
        }
        else
        {
            DrinkSatiatedHours = DrinkSatiationLimit;
        }

        if (AlcoholLitres < 0)
        {
            AlcoholLitres = 0;
        }
    }

    protected NeedsResult NeedsChanged(NeedsResult oldStatus, bool hungrier, bool thirstier, bool drunker)
    {
        NeedsResult newStatus = Status;
        var hungerTransition = (oldStatus & NeedsResult.HungerOnly) != (newStatus & NeedsResult.HungerOnly);
        var thirstTransition = (oldStatus & NeedsResult.ThirstOnly) != (newStatus & NeedsResult.ThirstOnly);
        var actor = Owner;
        var location = actor.Location;

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

        DispatchNeedsTransitions(actor, location, oldStatus, newStatus);

        return newStatus;
    }

    internal static void DispatchNeedsTransitions(ICharacter actor, ILocation? location, NeedsResult oldStatus,
        NeedsResult newStatus)
    {
        var hungerEvent = GetHungerEntryEvent(oldStatus, newStatus);
        var thirstEvent = GetThirstEntryEvent(oldStatus, newStatus);
        if (!hungerEvent.HasValue && !thirstEvent.HasValue)
        {
            return;
        }

        var witnesses = location?.EventHandlersFor(actor).Where(x => !ReferenceEquals(x, actor)).ToArray()
                        ?? Array.Empty<IHandleEvents>();

        // Snapshot both actor location (passed by caller) and handlers before callbacks. A self
        // hook that moves the actor or recursively changes needs cannot redirect this transition.
        if (hungerEvent.HasValue)
        {
            actor.HandleEvent(hungerEvent.Value, actor, location);
            var witnessEvent = hungerEvent == EventType.CharacterHungry
                ? EventType.CharacterInRoomHungry
                : EventType.CharacterInRoomStarving;
            foreach (var witness in witnesses)
            {
                witness.HandleEvent(witnessEvent, actor, location, witness);
            }
        }

        if (thirstEvent.HasValue)
        {
            actor.HandleEvent(thirstEvent.Value, actor, location);
            var witnessEvent = thirstEvent == EventType.CharacterThirsty
                ? EventType.CharacterInRoomThirsty
                : EventType.CharacterInRoomParched;
            foreach (var witness in witnesses)
            {
                witness.HandleEvent(witnessEvent, actor, location, witness);
            }
        }
    }

    internal static EventType? GetHungerEntryEvent(NeedsResult oldStatus, NeedsResult newStatus)
    {
        if ((oldStatus & NeedsResult.HungerOnly) == (newStatus & NeedsResult.HungerOnly))
        {
            return null;
        }

        return (newStatus & NeedsResult.HungerOnly) switch
        {
            NeedsResult.Hungry => EventType.CharacterHungry,
            NeedsResult.Starving => EventType.CharacterStarving,
            _ => null
        };
    }

    internal static EventType? GetThirstEntryEvent(NeedsResult oldStatus, NeedsResult newStatus)
    {
        if ((oldStatus & NeedsResult.ThirstOnly) == (newStatus & NeedsResult.ThirstOnly))
        {
            return null;
        }

        return (newStatus & NeedsResult.ThirstOnly) switch
        {
            NeedsResult.Thirsty => EventType.CharacterThirsty,
            NeedsResult.Parched => EventType.CharacterParched,
            _ => null
        };
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
        if (!double.IsFinite(configuredLimit) || configuredLimit < MinimumSatiationLimitHours)
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
            result |= TracksThirst
                ? GetThirstStatus(DrinkSatiatedHours, DrinkSatiationLimit)
                : NeedsResult.Sated;

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

    public abstract string ModelName { get; }

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
