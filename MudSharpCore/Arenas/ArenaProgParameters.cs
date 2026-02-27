#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;

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
		ProgVariableTypes.Number,
		ProgVariableTypes.DateTime,
		ProgVariableTypes.TimeSpan
	];

	internal static readonly IReadOnlyList<ProgVariableTypes> SideOutfitParameters =
	[
		ProgVariableTypes.Character | ProgVariableTypes.Collection,
		ProgVariableTypes.Number,
		ProgVariableTypes.Location,
		ProgVariableTypes.Text,
		ProgVariableTypes.Text,
		ProgVariableTypes.Text,
		ProgVariableTypes.Number,
		ProgVariableTypes.DateTime,
		ProgVariableTypes.TimeSpan
	];

	internal static readonly IReadOnlyList<ProgVariableTypes> NpcLoaderParameters =
	[
		ProgVariableTypes.Number,
		ProgVariableTypes.Number,
		ProgVariableTypes.Location,
		ProgVariableTypes.Text,
		ProgVariableTypes.Text,
		ProgVariableTypes.Text,
		ProgVariableTypes.Number,
		ProgVariableTypes.DateTime,
		ProgVariableTypes.TimeSpan
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
		var roster = arenaEvent.Participants
		                       .Where(x => x.Character is not null)
		                       .ToList();
		var participants = roster.Select(x => x.Character!).ToList();
		var sideIndices = roster.Select(x => x.SideIndex).ToList();

		return
		[
			participants,
			sideIndices,
			SelectArenaCell(arenaEvent.Arena.ArenaCells),
			arenaEvent.Name,
			arenaEvent.EventType.Name,
			arenaEvent.Arena.Name,
			arenaEvent.Id,
			arenaEvent.ScheduledAt,
			arenaEvent.EventType.TimeLimit
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
			arenaEvent.Name,
			arenaEvent.EventType.Name,
			arenaEvent.Arena.Name,
			arenaEvent.Id,
			arenaEvent.ScheduledAt,
			arenaEvent.EventType.TimeLimit
		];
	}

	internal static object[] BuildNpcLoaderArguments(IArenaEvent arenaEvent, int sideIndex, int slotsNeeded)
	{
		return
		[
			sideIndex,
			slotsNeeded,
			SelectWaitingCell(arenaEvent.Arena, sideIndex),
			arenaEvent.Name,
			arenaEvent.EventType.Name,
			arenaEvent.Arena.Name,
			arenaEvent.Id,
			arenaEvent.ScheduledAt,
			arenaEvent.EventType.TimeLimit
		];
	}

	internal static object[] BuildPhaseTransitionArguments(IArenaEvent arenaEvent, ArenaEventState phase)
	{
		return
		[
			arenaEvent.Arena.Id,
			arenaEvent.Id,
			arenaEvent.Name,
			phase.DescribeEnum()
		];
	}

	private static IReadOnlyCollection<IReadOnlyList<ProgVariableTypes>> BuildParameterSets(
		IReadOnlyList<ProgVariableTypes> parameters)
	{
		var results = new List<IReadOnlyList<ProgVariableTypes>>(parameters.Count + 1);

		for (var i = 1; i <= parameters.Count; i++)
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
		var list = cells?.ToList() ?? [];
		if (list.Count == 0)
		{
			return null;
		}

		return list.ElementAtOrDefault(index) ?? list[0];
	}
}
