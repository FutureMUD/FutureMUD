#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Immutable template describing the structure of a combat event.
/// </summary>
public interface IArenaEventType : IEditableItem
{
	ICombatArena Arena { get; }
	IEnumerable<IArenaEventTypeSide> Sides { get; }
	bool BringYourOwn { get; }
	TimeSpan RegistrationDuration { get; }
	TimeSpan PreparationDuration { get; }
	TimeSpan? TimeLimit { get; }
	TimeSpan? AutoScheduleInterval { get; }
	DateTime? AutoScheduleReferenceTime { get; }
	bool AutoScheduleEnabled { get; }
	BettingModel BettingModel { get; }
	decimal AppearanceFee { get; }
	decimal VictoryFee { get; }
	IFutureProg? IntroProg { get; }
	IFutureProg? ScoringProg { get; }
	IFutureProg? ResolutionOverrideProg { get; }
	ArenaEloStyle EloStyle { get; }
	decimal EloKFactor { get; }
	IArenaEliminationStrategy? EliminationStrategy { get; }
	ArenaEliminationMode EliminationMode { get; }
	bool AllowSurrender { get; }

	IArenaEvent CreateInstance(DateTime scheduledTime, IEnumerable<IArenaReservation>? reservations = null);
	void ConfigureAutoSchedule(TimeSpan? interval, DateTime? referenceTime);
	IArenaEventType Clone(string newName, ICharacter originator);
}
