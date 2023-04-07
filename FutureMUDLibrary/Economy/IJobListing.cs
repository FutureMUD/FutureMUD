using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.Work.Projects;

namespace MudSharp.Economy
{
    #nullable enable
    public interface IJobListing : ISaveable, IEditableItem
    {
        IEconomicZone EconomicZone { get; }
        string Description { get; }
        IFrameworkItem Employer { get; }
        bool IsReadyToBePosted { get; set; }
        bool IsArchived { get; set; }
        (bool Truth, string Error) IsEligibleForJob(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
        MudTimeSpan? MaximumDuration { get; }
        int MaximumNumberOfSimultaneousEmployees { get; }
        IClan? ClanMembership { get; }
        IRank? ClanRank { get; }
        IAppointment? ClanAppointment { get; }
        IPaygrade? ClanPaygrade { get; }
        IProject? PersonalProject { get; }
        ILocalProject? RequiredProject { get; }
        IProjectLabourRequirement? RequiredProjectLabour { get; }
        void ProjectTick();
        IEnumerable<IActiveJob> ActiveJobs { get; }
        DecimalCounter<ICurrency> MoneyPaidIn { get; }
        IBankAccount? BankAccount { get; }
        DecimalCounter<ICurrency> NetFinancialPosition { get; }
        double DaysOfSolvency { get; }
        string PayDescriptionForJobListing();
        bool IsAuthorisedToEdit(ICharacter actor);
        double FullTimeEquivalentRatio { get; }

        IActiveJob ApplyForJob(ICharacter actor);
        void FinishJob();
        void RemoveJob(IActiveJob job);
    }
}
