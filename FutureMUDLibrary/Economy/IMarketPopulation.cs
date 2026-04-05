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
    /// The market this market population belongs to
    /// </summary>
    IMarket Market { get; }

    string Description { get; }

    /// <summary>
    /// Expressed as a percentage of base expenditure at base prices. Positive stresses represent debt, negative savings
    /// </summary>
    decimal CurrentStress { get; }

    MarketStressPoint? CurrentStressPoint { get; }

    IEnumerable<MarketPopulationNeed> MarketPopulationNeeds { get; }
    IEnumerable<MarketStressPoint> MarketStressPoints { get; }
    void MarketPopulationHeartbeat();
    IMarketPopulation Clone(string name);
}