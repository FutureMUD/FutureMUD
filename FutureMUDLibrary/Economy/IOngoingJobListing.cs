using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy
{
    public interface IOngoingJobListing : IJobListing
    {
        RecurringInterval PayInterval { get; }
        MudDateTime PayReference { get; }
        ICurrency PayCurrency { get; }
        ExpressionEngine.Expression PayExpression { get; }

        void CheckPayroll();
    }
}
