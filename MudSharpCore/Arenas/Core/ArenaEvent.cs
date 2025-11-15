#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;

namespace MudSharp.Arenas;

public sealed class ArenaEvent : SaveableItem, IArenaEvent
{
	private readonly List<ArenaParticipant> _participants = new();
	private readonly List<ArenaReservation> _reservations = new();
	private ArenaEventState _state;
	private ArenaOutcome? _outcome;
	private IReadOnlyCollection<int>? _winningSides;

	public ArenaEvent(MudSharp.Models.ArenaEvent model, CombatArena arena, ArenaEventType eventType)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		EventType = eventType;
		_id = model.Id;
		_name = $"{eventType.Name} Event #{model.Id:N0}";
		_state = (ArenaEventState)model.State;
		CreatedAt = model.CreatedAt;
		ScheduledAt = model.ScheduledAt;
		RegistrationOpensAt = model.RegistrationOpensAt;
		StartedAt = model.StartedAt;
		ResolvedAt = model.ResolvedAt;
		CompletedAt = model.CompletedAt;
		BringYourOwn = model.BringYourOwn;
		RegistrationDuration = TimeSpan.FromSeconds(model.RegistrationDurationSeconds);
		PreparationDuration = TimeSpan.FromSeconds(model.PreparationDurationSeconds);
		TimeLimit = model.TimeLimitSeconds.HasValue ? TimeSpan.FromSeconds(model.TimeLimitSeconds.Value) : null;
		BettingModel = (BettingModel)model.BettingModel;
		AppearanceFee = model.AppearanceFee;
		VictoryFee = model.VictoryFee;

		foreach (var signup in model.ArenaSignups)
		{
			_participants.Add(new ArenaParticipant(signup, this));
		}

		foreach (var reservation in model.ArenaReservations)
		{
			_reservations.Add(new ArenaReservation(reservation, this));
		}
	}

	public ArenaEvent(ArenaEventType template, CombatArena arena, DateTime scheduledFor,
		IEnumerable<IArenaReservation>? reservations)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		EventType = template;
		_name = $"{template.Name} Event";
		_state = ArenaEventState.Draft;
		CreatedAt = DateTime.UtcNow;
		ScheduledAt = scheduledFor;
		BringYourOwn = template.BringYourOwn;
		RegistrationDuration = template.RegistrationDuration;
		PreparationDuration = template.PreparationDuration;
		TimeLimit = template.TimeLimit;
		BettingModel = template.BettingModel;
		AppearanceFee = template.AppearanceFee;
		VictoryFee = template.VictoryFee;
		var opensAt = scheduledFor - PreparationDuration - RegistrationDuration;
		RegistrationOpensAt = opensAt > CreatedAt ? opensAt : CreatedAt;

		using (new FMDB())
		{
			var dbEvent = new MudSharp.Models.ArenaEvent
			{
				ArenaId = arena.Id,
				ArenaEventTypeId = template.Id,
				State = (int)_state,
				BringYourOwn = BringYourOwn,
				RegistrationDurationSeconds = (int)RegistrationDuration.TotalSeconds,
				PreparationDurationSeconds = (int)PreparationDuration.TotalSeconds,
				TimeLimitSeconds = TimeLimit.HasValue ? (int)TimeLimit.Value.TotalSeconds : null,
				BettingModel = (int)BettingModel,
				AppearanceFee = AppearanceFee,
				VictoryFee = VictoryFee,
				CreatedAt = CreatedAt,
				ScheduledAt = ScheduledAt,
				RegistrationOpensAt = RegistrationOpensAt,
				StartedAt = StartedAt,
				ResolvedAt = ResolvedAt,
				CompletedAt = CompletedAt
			};
			FMDB.Context.ArenaEvents.Add(dbEvent);
			FMDB.Context.SaveChanges();
			_id = dbEvent.Id;
			_name = $"{template.Name} Event #{Id:N0}";
		}

		if (reservations != null)
		{
			foreach (var reservation in reservations)
			{
				AddReservation(reservation);
			}
		}
	}

	public CombatArena Arena { get; }
	ICombatArena IArenaEvent.Arena => Arena;
	public ArenaEventType EventType { get; }
	IArenaEventType IArenaEvent.EventType => EventType;
	public ArenaEventState State => _state;
	public DateTime CreatedAt { get; private set; }
	public DateTime ScheduledAt { get; private set; }
	public DateTime? RegistrationOpensAt { get; private set; }
	public DateTime? StartedAt { get; private set; }
	public DateTime? ResolvedAt { get; private set; }
	public DateTime? CompletedAt { get; private set; }
	public ArenaOutcome? Outcome => _outcome;
	public IReadOnlyCollection<int>? WinningSides => _winningSides;
	public bool BringYourOwn { get; }
	public TimeSpan RegistrationDuration { get; }
	public TimeSpan PreparationDuration { get; }
	public TimeSpan? TimeLimit { get; }
	public BettingModel BettingModel { get; }
	public decimal AppearanceFee { get; }
	public decimal VictoryFee { get; }

	public IEnumerable<IArenaParticipant> Participants => _participants;
	public IEnumerable<IArenaReservation> Reservations => _reservations;

	public void OpenRegistration()
	{
		EnforceState(ArenaEventState.RegistrationOpen);
		RegistrationOpensAt ??= DateTime.UtcNow;
		Changed = true;
	}

	public void CloseRegistration()
	{
		EnforceState(ArenaEventState.Preparing);
		Changed = true;
	}

	public void StartPreparation()
	{
		EnforceState(ArenaEventState.Preparing);
		Changed = true;
	}

	public void Stage()
	{
		EnforceState(ArenaEventState.Staged);
		Changed = true;
	}

	public void StartLive()
	{
		StartedAt ??= DateTime.UtcNow;
		EnforceState(ArenaEventState.Live);
		Changed = true;
	}

	public void MercyStop()
	{
		if (_state != ArenaEventState.Live)
		{
			return;
		}

		if (!CanMercyStopNow())
		{
			return;
		}

		Resolve();
	}

	private bool CanMercyStopNow()
	{
		if (EventType.EliminationStrategy is { } strategy)
		{
			try
			{
				return strategy.MercyStopAllowed(this);
			}
			catch
			{
			}
		}

		var activeSides = _participants
			.GroupBy(x => x.SideIndex)
			.Count(group => group.Any(IsParticipantActive));
		return activeSides <= 1;
	}

	private static bool IsParticipantActive(ArenaParticipant participant)
	{
		var character = participant.Character;
		return character != null && character.State.IsAble();
	}

	public void Resolve()
	{
		if (_state >= ArenaEventState.Resolving)
		{
			return;
		}

		ResolvedAt = DateTime.UtcNow;
		EnforceState(ArenaEventState.Resolving);
		Changed = true;
	}

	public void Cleanup()
	{
		EnforceState(ArenaEventState.Cleanup);
		Changed = true;
	}

	public void Complete()
	{
		CompletedAt = DateTime.UtcNow;
		EnforceState(ArenaEventState.Completed);
		Changed = true;
	}

	public void Abort(string reason)
	{
		RecordOutcome(ArenaOutcome.Aborted, null);
		CompletedAt = DateTime.UtcNow;
		EnforceState(ArenaEventState.Aborted);
		Changed = true;
	}

	public (bool Truth, string Reason) CanSignUp(ICharacter character, int sideIndex,
		ICombatantClass combatantClass)
	{
		if (character == null)
		{
			return (false, "You cannot sign up without a character.");
		}

		if (_state != ArenaEventState.RegistrationOpen)
		{
			return (false, "Registration is not open for that event.");
		}

		var side = EventType.Sides.FirstOrDefault(x => x.Index == sideIndex);
		if (side == null)
		{
			return (false, "That side does not exist for this event type.");
		}

		if (!side.EligibleClasses.Contains(combatantClass))
		{
			return (false, "That combatant class is not eligible for this side.");
		}

		if (!character.IsPlayerCharacter && !side.AllowNpcSignup)
		{
			return (false, "NPC signups are not allowed for that side.");
		}

		if (EligibilityFailed(combatantClass, character))
		{
			return (false, "You are not eligible for that combatant class.");
		}

		if (_participants.Any(x => x.Character?.Id == character.Id))
		{
			return (false, "You are already signed up for this event.");
		}

		if (_participants.Count(x => x.SideIndex == sideIndex) >= side.Capacity)
		{
			return (false, "That side is already full.");
		}

		return side.Policy switch
		{
			ArenaSidePolicy.Closed => (false, "That side is closed to new participants."),
			   ArenaSidePolicy.ManagersOnly when !Arena.IsManager(character) =>
				   (false, "Only arena managers may sign up for that side."),
			   ArenaSidePolicy.ReservedOnly when !HasReservation(character) =>
				   (false, "Only reserved participants may sign up for that side."),
			_ => (true, string.Empty)
		};
	}

	public void SignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass)
	{
		var result = CanSignUp(character, sideIndex, combatantClass);
		if (!result.Truth)
		{
			throw new InvalidOperationException(result.Reason);
		}

		var startingRating = Gameworld.ArenaRatingsService.GetRating(character, combatantClass);
		using (new FMDB())
		{
			var signup = new MudSharp.Models.ArenaSignup
			{
				ArenaEventId = Id,
				CharacterId = character.Id,
				CombatantClassId = combatantClass.Id,
				SideIndex = sideIndex,
				IsNpc = !character.IsPlayerCharacter,
				StageName = combatantClass.DefaultStageNameTemplate,
				SignatureColour = combatantClass.DefaultSignatureColour,
				StartingRating = startingRating,
				SignedUpAt = DateTime.UtcNow
			};

			var priorReservation = _reservations.FirstOrDefault(x => x.CharacterId == character.Id);
			if (priorReservation != null)
			{
				signup.ArenaReservationId = priorReservation.ReservationId;
				_reservations.Remove(priorReservation);
			}

			FMDB.Context.ArenaSignups.Add(signup);
			FMDB.Context.SaveChanges();
			_participants.Add(new ArenaParticipant(signup, this));
		}
	}

	public void Withdraw(ICharacter character)
	{
		if (character == null)
		{
			return;
		}

		if (_state > ArenaEventState.RegistrationOpen)
		{
			throw new InvalidOperationException("You can no longer withdraw from that event.");
		}

		var participant = _participants.FirstOrDefault(x => x.Character?.Id == character.Id);
		if (participant == null)
		{
			return;
		}

		using (new FMDB())
		{
			var dbSignup = FMDB.Context.ArenaSignups.Find(participant.SignupId);
			if (dbSignup != null)
			{
				FMDB.Context.ArenaSignups.Remove(dbSignup);
				FMDB.Context.SaveChanges();
			}
		}

		_participants.Remove(participant);
	}

	public void AddReservation(IArenaReservation reservation)
	{
		if (reservation == null)
		{
			return;
		}

		using (new FMDB())
		{
			var record = new MudSharp.Models.ArenaReservation
			{
				ArenaEventId = Id,
				SideIndex = reservation.SideIndex,
				CharacterId = reservation.CharacterId,
				ClanId = reservation.ClanId,
				ReservedAt = DateTime.UtcNow,
				ExpiresAt = reservation.ExpiresAt
			};
			FMDB.Context.ArenaReservations.Add(record);
			FMDB.Context.SaveChanges();
			_reservations.Add(new ArenaReservation(record, this));
		}
	}

	public void RemoveReservation(IArenaReservation reservation)
	{
		if (reservation is not ArenaReservation concrete)
		{
			return;
		}

		using (new FMDB())
		{
			var record = FMDB.Context.ArenaReservations.Find(concrete.ReservationId);
			if (record != null)
			{
				FMDB.Context.ArenaReservations.Remove(record);
				FMDB.Context.SaveChanges();
			}
		}

		_reservations.Remove(concrete);
	}

	public void RecordOutcome(ArenaOutcome outcome, IEnumerable<int>? winningSides)
	{
		_outcome = outcome;
		if (winningSides is null)
		{
			_winningSides = null;
			return;
		}

		var validSides = winningSides
			.Distinct()
			.Where(index => EventType.Sides.Any(side => side.Index == index))
			.OrderBy(x => x)
			.ToArray();

		_winningSides = validSides.Length == 0 ? Array.Empty<int>() : validSides;
	}

	public void EnforceState(ArenaEventState state)
	{
		if (_state == state)
		{
			return;
		}

		_state = state;
		Changed = true;
	}

	public override string FrameworkItemType => "ArenaEvent";

	public override void Save()
	{
		if (!Changed)
		{
			return;
		}

		using (new FMDB())
		{
			var dbEvent = FMDB.Context.ArenaEvents.Find(Id);
			if (dbEvent == null)
			{
				return;
			}

			dbEvent.State = (int)_state;
			dbEvent.ScheduledAt = ScheduledAt;
			dbEvent.RegistrationOpensAt = RegistrationOpensAt;
			dbEvent.StartedAt = StartedAt;
			dbEvent.ResolvedAt = ResolvedAt;
			dbEvent.CompletedAt = CompletedAt;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	private bool HasReservation(ICharacter character)
	{
		var now = DateTime.UtcNow;
		_reservations.RemoveAll(x => x.ExpiresAt < now);
		if (_reservations.Any(x => x.CharacterId == character.Id))
		{
			return true;
		}

		var clanIds = character.ClanMemberships
							   .Select(x => x.Clan?.Id)
							   .Where(x => x.HasValue)
							   .Select(x => x.Value)
							   .ToHashSet();

		if (!clanIds.Any())
		{
			return false;
		}

		return _reservations.Any(x => x.ClanId.HasValue && clanIds.Contains(x.ClanId.Value));
	}

	private static bool EligibilityFailed(ICombatantClass combatantClass, ICharacter character)
	{
		try
		{
			return combatantClass.EligibilityProg.Execute<bool?>(character) == false;
		}
		catch
		{
			return true;
		}
	}
}

internal sealed class ArenaParticipant : IArenaParticipant
{
	private readonly ArenaEvent _event;
	private readonly long _characterId;
	private ICharacter? _characterCache;

	public ArenaParticipant(MudSharp.Models.ArenaSignup signup, ArenaEvent parent)
	{
		_event = parent;
		SignupId = signup.Id;
		_characterId = signup.CharacterId;
		CombatantClass = parent.Arena.GetCombatantClass(signup.CombatantClassId)!;
		SideIndex = signup.SideIndex;
		IsNpc = signup.IsNpc;
		StageName = string.IsNullOrWhiteSpace(signup.StageName) ? null : signup.StageName;
		SignatureColour = string.IsNullOrWhiteSpace(signup.SignatureColour) ? null : signup.SignatureColour;
		StartingRating = signup.StartingRating;
	}

	public long SignupId { get; }
	public ICharacter? Character => _characterCache ??= _event.Gameworld.TryGetCharacter(_characterId, true);
	public ICombatantClass CombatantClass { get; }
	public int SideIndex { get; }
	public bool IsNpc { get; }
	public string? StageName { get; }
	public string? SignatureColour { get; }
	public decimal? StartingRating { get; }

}

internal sealed class ArenaReservation : IArenaReservation
{
	private readonly ArenaEvent _event;

	public ArenaReservation(MudSharp.Models.ArenaReservation reservation, ArenaEvent parent)
	{
		_event = parent;
		ReservationId = reservation.Id;
		SideIndex = reservation.SideIndex;
		CharacterId = reservation.CharacterId;
		ClanId = reservation.ClanId;
		ExpiresAt = reservation.ExpiresAt;
	}

	public long ReservationId { get; }
	public int SideIndex { get; }
	public long? CharacterId { get; }
	public long? ClanId { get; }
	public DateTime ExpiresAt { get; }

}
