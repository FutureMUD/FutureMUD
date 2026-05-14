using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System.Collections.Generic;

namespace MudSharp.Combat;

public enum ManualCombatActionKind
{
	WeaponAttack = 0,
	AuxiliaryAction = 1
}

public interface IManualCombatCommand : ISaveable, IEditableItem, IKeywordedItem
{
	string PrimaryVerb { get; }
	IEnumerable<string> CommandWords { get; }
	ManualCombatActionKind ActionKind { get; }
	IWeaponAttack WeaponAttack { get; }
	IAuxiliaryCombatAction AuxiliaryAction { get; }
	bool PlayerUsable { get; set; }
	bool NpcUsable { get; set; }
	IFutureProg UsabilityProg { get; set; }
	double CooldownSeconds { get; set; }
	string CooldownMessage { get; set; }
	double DefaultAiWeightMultiplier { get; set; }
	bool IsUsableBy(ICharacter actor, ICharacter target);
	IManualCombatCommand Clone(string name);
}
