#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Immutable template describing the structure of a combat event.
/// </summary>
public interface IArenaEventType : IFrameworkItem, ISaveable {
	ICombatArena Arena { get; }
	IEnumerable<IArenaEventTypeSide> Sides { get; }
	bool BringYourOwn { get; }
	TimeSpan RegistrationDuration { get; }
	TimeSpan PreparationDuration { get; }
	TimeSpan? TimeLimit { get; }
	BettingModel BettingModel { get; }
	decimal AppearanceFee { get; }
	decimal VictoryFee { get; }
	IFutureProg? IntroProg { get; }
	IFutureProg? ScoringProg { get; }
	IFutureProg? ResolutionOverrideProg { get; }
	IArenaEliminationStrategy? EliminationStrategy { get; }

	IArenaEvent CreateInstance(DateTime scheduledTime, IEnumerable<IArenaReservation>? reservations = null);
	IArenaEventType Clone(string newName, ICharacter originator);
}
