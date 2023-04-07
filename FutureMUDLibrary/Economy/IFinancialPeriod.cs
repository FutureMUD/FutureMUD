using System;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy
{
    public interface IFinancialPeriod : IComparable<IFinancialPeriod>, ILateInitialisingItem
    {
        IEconomicZone EconomicZone { get; }
        DateTime FinancialPeriodStart { get; }
        DateTime FinancialPeriodEnd { get; set; }
        MudDateTime FinancialPeriodStartMUD { get; }
        MudDateTime FinancialPeriodEndMUD { get; }
        bool InPeriod(DateTime compare);
        bool InPeriod(MudDateTime compare);
        void Delete();
    }
}