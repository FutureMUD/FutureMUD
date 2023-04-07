using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class WeeklyStatistic
	{
		public long Id { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		
		public int TotalAccounts { get; set; }
		public int ActiveAccounts { get; set; }
		public int NewAccounts { get; set; }

		public int ApplicationsSubmitted { get; set; }
		public int ApplicationsApproved { get; set; }

		public int PlayerDeaths { get; set; }
		public int NonPlayerDeaths { get; set; }
	}
}
