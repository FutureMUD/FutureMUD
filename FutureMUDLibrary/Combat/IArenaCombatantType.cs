using MudSharp.Character.Name;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using System.Collections.Generic;

namespace MudSharp.Combat;

public interface IArenaCombatantType : IFrameworkItem, ISaveable, IEditableItem
{
	int MinimumNumberOfNPCs { get; }
	int MaximumNumberOfPCs { get; }
	INPCTemplate? NPCTemplate { get; }
	IFutureProg PlayerQualificationProg { get; }
	IFutureProg? OnCombatantJoinsTypeProg { get; }
	IFutureProg? OnCombatantWinProg { get; }
	IFutureProg? OnCombatantLoseProg { get; }
	IFutureProg? OnCombatantLeavesTypeProg { get; }
	INameCulture? StageNameCulture { get; }
	IEnumerable<IRandomNameProfile> StageNameRandomProfiles { get; }
	bool AutoHealNPCs { get; }
}
