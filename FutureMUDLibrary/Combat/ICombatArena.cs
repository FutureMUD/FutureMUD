using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat;
#nullable enable

public interface ICombatArena : IFrameworkItem, ISaveable
{
	IEnumerable<ICell> ArenaCells { get; }
	IEnumerable<ICell> StagingCells { get; }
	IEnumerable<ICell> StableCells { get; }
	IEnumerable<IArenaCombatantType> ArenaCombatantTypes { get; }
	IEnumerable<IArenaCombatantProfile> ArenaCombatantProfiles { get; }
}

public interface IArenaCombatantType : IFrameworkItem, ISaveable
{
	int MinimumNumberOfNPCs { get; }
	int MaximumNumberOfPCs { get; }
	INPCTemplate? NPCTemplate { get; }
	IFutureProg PlayerQualificationProg { get; }
	IFutureProg? OnCombatantJoinsTypeProg { get; }
	IFutureProg? OnCombatantWinProg { get; }
	IFutureProg? OnCombatantLoseProg { get; }
	IFutureProg? OnCombatantLeavesTypeProg { get; }
}

public interface IArenaCombatantProfile : IFrameworkItem, ISaveable
{
	bool IsArchived { get; }
	bool IsPC { get; }
	ICharacter Character { get; }
	IArenaCombatantType ArenaCombatantType { get; }
}
