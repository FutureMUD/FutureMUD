#nullable enable
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.Economy;

public interface IMarketPopulation : ISaveable, IEditableItem
{
    /// <summary>
    /// Approximate size of this population in individuals
    /// </summary>
    int PopulationScale { get; }

    /// <summary>
    /// A multiplier applied to this population's base budget when working out how much income it has available.
    /// </summary>
    decimal IncomeFactor { get; }

    /// <summary>
    /// Expressed in multiples of this population's base budget, representing budget cycles of savings currently on hand.
    /// </summary>
    decimal Savings { get; }

    /// <summary>
    /// Expressed in multiples of this population's base budget, representing the largest savings reserve this population can accumulate.
    /// </summary>
    decimal SavingsCap { get; }

    /// <summary>
    /// The absolute hysteresis buffer applied when stress is falling so thresholds do not rapidly toggle on and off.
    /// </summary>
    decimal StressFlickerThreshold { get; }

    /// <summary>
    /// The market this market population belongs to
    /// </summary>
    IMarket Market { get; }

    string Description { get; }

    /// <summary>
    /// Expressed as a percentage of base expenditure at base prices, after current income and accumulated savings have both been applied.
    /// </summary>
    decimal CurrentStress { get; }

    MarketStressPoint? CurrentStressPoint { get; }

    IEnumerable<MarketPopulationNeed> MarketPopulationNeeds { get; }
    IEnumerable<MarketStressPoint> MarketStressPoints { get; }
    void MarketPopulationHeartbeat();
    IMarketPopulation Clone(string name);
}
