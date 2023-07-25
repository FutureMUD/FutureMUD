using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class RacesCombatActions
	{
		public long RaceId { get; set; }
		public long CombatActionId { get; set; }
		public virtual Race Race { get; set; }
		public virtual CombatAction CombatAction { get; set; }
	}
}
