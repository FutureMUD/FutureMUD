using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class JobListing
    {
        public JobListing()
        {
            ActiveJobs = new HashSet<ActiveJob>();
        }

        public long Id { get; set; }
        public long PosterId { get; set; }
        public bool IsReadyToBePosted { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        public string PosterType { get; set; }
        public string JobListingType { get; set; }
        public string Definition { get; set; }
        public string Description { get; set; }
        public string MoneyPaidIn { get; set; }
        public string MaximumDuration { get; set; }
        public long EligibilityProgId { get; set; }
        public long? ClanId { get; set; }
        public long? RankId { get; set; }
        public long? PaygradeId { get; set; }
        public long? AppointmentId { get; set; }
        public long? PersonalProjectId { get; set; }
        public int? PersonalProjectRevisionNumber { get; set; }
        public long? RequiredProjectId { get; set; }
        public long? RequiredProjectLabourId { get; set; }
        public long? BankAccountId { get; set; }
        public long EconomicZoneId { get; set; }
        public int MaximumNumberOfSimultaneousEmployees { get; set; }
        public double FullTimeEquivalentRatio { get; set; }

        public virtual FutureProg EligibilityProg { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Clan Clan { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual Paygrade Paygrade { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual Project PersonalProject { get; set; }
        public virtual ActiveProject RequiredProject { get; set; }
        public virtual ActiveProjectLabour RequiredProjectLabour { get; set; }
        public virtual EconomicZone EconomicZone { get; set; }
        public virtual ICollection<ActiveJob> ActiveJobs { get; set; }
    }
}
