using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class NPCSpawnerCell
	{
		public long NPCSpawnerId { get; set; }
		public long CellId { get; set; }

		public virtual NPCSpawner NPCSpawner { get; set; }
		public virtual Cell Cell { get; set; }
	}
}
