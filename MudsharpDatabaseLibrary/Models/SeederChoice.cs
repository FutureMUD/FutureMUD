using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class SeederChoice
	{
		public long Id {get;set; }
		public string Version {get; set; }
		public string Seeder {get; set;}
		public string Choice {get; set;}
		public string Answer {get; set; }
		public DateTime DateTime {get; set; }
	}
}
