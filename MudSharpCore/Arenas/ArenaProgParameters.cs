#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Arenas;

internal static class ArenaProgParameters
{
    internal static readonly IReadOnlyList<ProgVariableTypes> EventProgParameters =
    [
        ProgVariableTypes.Character | ProgVariableTypes.Collection,
        ProgVariableTypes.Number | ProgVariableTypes.Collection,
        ProgVariableTypes.Location,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
        ProgVariableTypes.Character | ProgVariableTypes.Collection,
        ProgVariableTypes.Character | ProgVariableTypes.Collection,
        ProgVariableTypes.Number | ProgVariableTypes.Collection,
        ProgVariableTypes.Number | ProgVariableTypes.Collection,
        ProgVariableTypes.Number | ProgVariableTypes.Collection,
        ProgVariableTypes.Number | ProgVariableTypes.Collection,
        ProgVariableTypes.Text | ProgVariableTypes.Collection,
        ProgVariableTypes.Text | ProgVariableTypes.Collection,
    ];

    internal static readonly IReadOnlyList<ProgVariableTypes> SideOutfitParameters =
    [
        ProgVariableTypes.Character | ProgVariableTypes.Collection,
        ProgVariableTypes.Number,
        ProgVariableTypes.Location,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
    ];

    internal static readonly IReadOnlyList<ProgVariableTypes> NpcLoaderParameters =
    [
        ProgVariableTypes.Number,
        ProgVariableTypes.Number,
        ProgVariableTypes.Location,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text,
    ];

    internal static readonly IReadOnlyList<ProgVariableTypes> PhaseTransitionParameters =
    [
        ProgVariableTypes.Number,
        ProgVariableTypes.Number,
        ProgVariableTypes.Text,
        ProgVariableTypes.Text
    ];

    internal static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> EventProgParameterSets { get; } =
        BuildParameterSets(EventProgParameters);

    internal static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> SideOutfitParameterSets { get; } =
        BuildParameterSets(SideOutfitParameters);

    internal static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> NpcLoaderParameterSets { get; } =
        BuildParameterSets(NpcLoaderParameters);

    internal static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> PhaseTransitionProgParameterSets { get; } =
        BuildParameterSets(PhaseTransitionParameters);

    internal static object[] BuildEventProgArguments(IArenaEvent arenaEvent)
    {
        List<IArenaParticipant> roster = arenaEvent.Participants
                               .Where(x => x.Character is not null)
                               .ToList();
        List<ICharacter> participants = roster.Select(x => x.Character!).ToList();
        List<int> sideIndices = roster.Select(x => x.SideIndex).ToList();
        IReadOnlyList<ArenaScoringSnapshot> snapshots = arenaEvent is ArenaEvent concreteEvent
            ? concreteEvent.ScoringSnapshots
            : [];

        return
        [
            participants,
            sideIndices,
            SelectArenaCell(arenaEvent.Arena.ArenaCells),
            arenaEvent.EventType.Name,
            arenaEvent.Arena.Name,
            arenaEvent.Name,
            snapshots.Select(x => x.Attacker).ToList(),
            snapshots.Select(x => x.Defender).ToList(),
            snapshots.Select(x => x.AttackerSideIndex).ToList(),
            snapshots.Select(x => x.DefenderSideIndex).ToList(),
            snapshots.Select(x => x.LandedHit).ToList(),
            snapshots.Select(x => x.UndefendedHit).ToList(),
            snapshots.Select(x => x.ImpactLocationKey).ToList(),
            snapshots.Select(x => x.ImpactBodypartIdentity).ToList(),
        ];
    }

    internal static object[] BuildSideOutfitArguments(IArenaEvent arenaEvent, int sideIndex,
        IReadOnlyList<ICharacter> participants)
    {
        return
        [
            participants,
            sideIndex,
            SelectWaitingCell(arenaEvent.Arena, sideIndex),
            arenaEvent.EventType.Name,
            arenaEvent.Arena.Name,
            arenaEvent.Name,
        ];
    }

    internal static object[] BuildNpcLoaderArguments(IArenaEvent arenaEvent, int sideIndex, int slotsNeeded)
    {
        return
        [
            sideIndex,
            slotsNeeded,
            SelectWaitingCell(arenaEvent.Arena, sideIndex),
            arenaEvent.EventType.Name,
            arenaEvent.Arena.Name,
            arenaEvent.Name,
        ];
    }

    internal static object[] BuildPhaseTransitionArguments(IArenaEvent arenaEvent, ArenaEventState phase)
    {
        return
        [
            arenaEvent.Arena.Id,
            arenaEvent.EventType.Id,
            arenaEvent.EventType.Name,
            phase.DescribeEnum()
        ];
    }

    private static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> BuildParameterSets(
        IReadOnlyList<ProgVariableTypes> parameters)
    {
        List<IReadOnlyList<ProgVariableTypes>> results = new(parameters.Count + 1);

        for (int i = 1; i <= parameters.Count; i++)
        {
            results.Add(parameters.Take(i).ToArray());
        }

        return results;
    }

    private static ICell? SelectWaitingCell(ICombatArena arena, int sideIndex)
    {
        return SelectIndexedCell(arena.WaitingCells, sideIndex);
    }

    private static ICell? SelectArenaCell(IEnumerable<ICell> arenaCells)
    {
        return SelectIndexedCell(arenaCells, 0);
    }

    private static ICell? SelectIndexedCell(IEnumerable<ICell> cells, int index)
    {
        List<ICell> list = cells?.ToList() ?? [];
        if (list.Count == 0)
        {
            return null;
        }

        return list.ElementAtOrDefault(index) ?? list[0];
    }
}
