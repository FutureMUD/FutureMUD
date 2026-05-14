using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ManualCombatCommand
{
	public ManualCombatCommand()
	{
		CharacterCombatSettingsManualCombatCommands = new HashSet<CharacterCombatSettingsManualCombatCommands>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public string PrimaryVerb { get; set; }
	public string AdditionalVerbs { get; set; }
	public int ActionKind { get; set; }
	public long? WeaponAttackId { get; set; }
	public long? CombatActionId { get; set; }
	public bool PlayerUsable { get; set; }
	public bool NpcUsable { get; set; }
	public long? UsabilityProgId { get; set; }
	public double CooldownSeconds { get; set; }
	public string CooldownMessage { get; set; }
	public double DefaultAiWeightMultiplier { get; set; }

	public virtual WeaponAttack WeaponAttack { get; set; }
	public virtual CombatAction CombatAction { get; set; }
	public virtual FutureProg UsabilityProg { get; set; }
	public virtual ICollection<CharacterCombatSettingsManualCombatCommands> CharacterCombatSettingsManualCombatCommands { get; set; }
}
