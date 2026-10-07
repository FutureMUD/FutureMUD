#nullable enable

using MudSharp.Construction;

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
                               .Where(x => x.ActiveCharacter is not null)
                               .ToList();
        List<ICharacter> participants = roster.Select(x => x.ActiveCharacter!).ToList();
        List<int> sideIndices = roster.Select(x => x.SideIndex).ToList();
        IReadOnlyList<ArenaScoringSnapshot> snapshots = arenaEvent is ArenaEvent concreteEvent
            ? concreteEvent.ScoringSnapshots
            : [];

        return
        [
            participants,
            sideIndices,
            SelectArenaRoom(arenaEvent.Arena.ArenaRooms)!,
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
            SelectWaitingRoom(arenaEvent.Arena, sideIndex)!,
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
            SelectWaitingRoom(arenaEvent.Arena, sideIndex)!,
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

    private static IRoom? SelectWaitingRoom(ICombatArena arena, int sideIndex)
    {
        return SelectIndexedRoom(arena.WaitingRooms, sideIndex);
    }

    private static IRoom? SelectArenaRoom(IEnumerable<IRoom> arenaRooms)
    {
        return SelectIndexedRoom(arenaRooms, 0);
    }

    private static IRoom? SelectIndexedRoom(IEnumerable<IRoom> rooms, int index)
    {
        List<IRoom> list = rooms?.ToList() ?? [];
        if (list.Count == 0)
        {
            return null;
        }

        return list.ElementAtOrDefault(index) ?? list[0];
    }
}
