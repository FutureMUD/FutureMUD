#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Arenas;

/// <summary>
/// Reservation for a particular side or entity during registration.
/// </summary>
public interface IArenaReservation {
	int SideIndex { get; }
	long? CharacterId { get; }
	long? ClanId { get; }
	DateTime ExpiresAt { get; }
}

/// <summary>
/// Represents a scheduled combat event instance.
/// </summary>
public interface IArenaEvent : IFrameworkItem, ISaveable {
	ICombatArena Arena { get; }
	IArenaEventType EventType { get; }
	ArenaEventState State { get; }
	DateTime CreatedAt { get; }
	DateTime ScheduledAt { get; }
	DateTime? RegistrationOpensAt { get; }
	DateTime? StartedAt { get; }
	DateTime? ResolvedAt { get; }
	DateTime? CompletedAt { get; }
	ArenaOutcome? Outcome { get; }
	IReadOnlyCollection<int>? WinningSides { get; }

	IEnumerable<IArenaParticipant> Participants { get; }
	IEnumerable<IArenaReservation> Reservations { get; }

	void OpenRegistration();
	void CloseRegistration();
	void StartPreparation();
	void Stage();
	void StartLive();
	void MercyStop();
	void Resolve();
	void Cleanup();
	void Complete();
	void Abort(string reason);

	(bool Truth, string Reason) CanSignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass);
	void SignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass);
	void Withdraw(ICharacter character);

	void AddReservation(IArenaReservation reservation);
	void RemoveReservation(IArenaReservation reservation);

	void RecordOutcome(ArenaOutcome outcome, IEnumerable<int>? winningSides);
	void EnforceState(ArenaEventState state);
}
