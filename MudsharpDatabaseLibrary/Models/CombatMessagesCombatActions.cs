using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models;

public class CombatMessagesCombatActions
{
	public long CombatMessageId { get; set; }
	public long CombatActionId { get; set; }

	public virtual CombatMessage CombatMessage { get; set; }
	public virtual CombatAction CombatAction { get; set; }
}
