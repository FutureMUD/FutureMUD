using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.Work.Projects;

namespace MudSharp.Economy
{
    #nullable enable
    public interface IActiveJob : IFrameworkItem, ISaveable
    {
        IJobListing Listing { get; }
        ICharacter Character { get; }
        MudDateTime JobCommenced { get; }
        MudDateTime? JobDueToEnd { get; }
        MudDateTime? JobEnded { get; }
        bool IsJobComplete { get; }
        bool AlreadyHadClanPosition { get; }
        double FullTimeEquivalentRatio { get; }
        double CurrentPerformance { get; set; }
        DecimalCounter<ICurrency> BackpayOwed { get; }
        DecimalCounter<ICurrency> RevenueEarned { get; }
        IActiveProject? ActiveProject { get; }
        void QuitJob();
        void FireFromJob();
        void FinishFixedTerm();
        void BeginJob();
        DecimalCounter<ICurrency> DailyPay();
        void Delete();
    }
}
