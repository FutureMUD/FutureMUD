using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models;

public class CombatAction
{
	public CombatAction()
	{
		CombatMessagesCombatActions = new HashSet<CombatMessagesCombatActions>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public long? UsabilityProgId { get; set; }
	public int RecoveryDifficultySuccess { get; set; }
	public int RecoveryDifficultyFailure { get; set; }
	public int MoveType { get; set; }
	public long Intentions { get; set; }
	public int ExertionLevel { get; set; }
	public double Weighting { get; set; }
	public double StaminaCost { get; set; }
	public double BaseDelay { get; set; }
	public string AdditionalInfo { get; set; }
	public string RequiredPositionStateIds { get; set; }

	public virtual FutureProg UsabilityProg { get; set; }
	public virtual ICollection<CombatMessagesCombatActions> CombatMessagesCombatActions { get; set; }
}
