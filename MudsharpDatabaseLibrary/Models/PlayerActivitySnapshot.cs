using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class PlayerActivitySnapshot
	{
		public long Id { get; set; }
		public DateTime DateTime { get; set; }
		public int OnlinePlayers { get; set; }
		public int OnlineAdmins { get; set; }
		public int AvailableAdmins { get; set; }
		public int IdlePlayers { get; set; }
		public int UniquePCLocations { get; set; }
		public int OnlineGuests { get; set; }
	}
}
