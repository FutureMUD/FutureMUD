using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class NPCSpawnerZone
	{
		public long NPCSpawnerId { get; set; }
		public long ZoneId { get; set; }

		public virtual NPCSpawner NPCSpawner { get; set; }
		public virtual Zone Zone { get; set; }
	}
}
