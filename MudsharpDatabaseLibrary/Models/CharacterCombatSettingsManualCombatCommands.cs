namespace MudSharp.Models;

public partial class CharacterCombatSettingsManualCombatCommands
{
	public long CharacterCombatSettingId { get; set; }
	public long ManualCombatCommandId { get; set; }
	public double WeightMultiplier { get; set; }

	public virtual CharacterCombatSetting CharacterCombatSetting { get; set; }
	public virtual ManualCombatCommand ManualCombatCommand { get; set; }
}
