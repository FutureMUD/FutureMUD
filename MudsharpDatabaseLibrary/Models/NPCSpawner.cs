using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class NPCSpawner
	{
		public NPCSpawner()
		{
			Cells = new HashSet<NPCSpawnerCell>();
			Zones = new HashSet<NPCSpawnerZone>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long? TargetTemplateId { get; set; }
		public int TargetCount { get; set; }
		public int MinimumCount { get; set; }
		public long? OnSpawnProgId { get; set; }
		public long? CountsAsProgId { get; set; }
		public long? IsActiveProgId { get; set; }
		public int SpawnStrategy { get; set; }
		public string Definition { get; set; }
		public virtual FutureProg OnSpawnProg { get; set; }
		public virtual FutureProg CountsAsProg { get; set; }
		public virtual FutureProg IsActiveProg { get; set; }
		public virtual ICollection<NPCSpawnerCell> Cells { get; set; }
		public virtual ICollection<NPCSpawnerZone> Zones { get; set; }
	}
}
