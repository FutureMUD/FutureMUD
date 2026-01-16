#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using System.Text;
using MudSharp.Community;
using MudSharp.Construction;
using System.Globalization;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.TimeAndDate;
using MudSharp.PerceptionEngine.Parsers;

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
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Arena Event #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Arena: {Arena.Name.ColourName()}");
		sb.AppendLine($"Event Type: {EventType.Name.ColourName()}");
		sb.AppendLine($"State: {State.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Created: {CreatedAt.ToString("g", actor).ColourValue()}");
		sb.AppendLine($"Scheduled: {ScheduledAt.ToString("g", actor).ColourValue()}");
		sb.AppendLine($"Registration Opens: {(RegistrationOpensAt?.ToString("g", actor) ?? "Not Set").ColourValue()}");
		sb.AppendLine($"Started: {(StartedAt?.ToString("g", actor) ?? "Not Started").ColourValue()}");
		sb.AppendLine($"Resolved: {(ResolvedAt?.ToString("g", actor) ?? "Not Resolved").ColourValue()}");
		sb.AppendLine($"Completed: {(CompletedAt?.ToString("g", actor) ?? "Not Completed").ColourValue()}");
		sb.AppendLine($"Outcome: {(Outcome?.DescribeEnum() ?? "Unknown").ColourName()}");
		if (WinningSides != null)
		{
			sb.AppendLine($"Winning Sides: {WinningSides.Select(x => ArenaSideIndexUtilities.ToDisplayString(actor, x)).ListToCommaSeparatedValues(", ").ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine("Participants:");
		var grouped = Participants.GroupBy(x => x.SideIndex).OrderBy(x => x.Key);
		foreach (var group in grouped)
		{
			sb.AppendLine($"\tSide {ArenaSideIndexUtilities.ToDisplayString(actor, group.Key).ColourValue()}");
			foreach (var participant in group)
			{
				var who = participant.Character?.HowSeen(actor) ?? participant.StageName ?? "NPC";
				sb.AppendLine($"\t\t{who.ColourName()} ({participant.CombatantClass.Name.ColourName()})" +
				              (participant.IsNpc ? " [NPC]".Colour(Telnet.Yellow) : string.Empty));
			}
		}

		if (Reservations.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Reservations:");
			foreach (var reservation in Reservations)
			{
				var who = reservation.CharacterId.HasValue
					? Gameworld.TryGetCharacter(reservation.CharacterId.Value, true)?.HowSeen(actor) ?? $"Character #{reservation.CharacterId.Value}"
					: reservation.ClanId.HasValue
						? Gameworld.Clans.Get(reservation.ClanId.Value)?.FullName ?? $"Clan #{reservation.ClanId.Value}"
						: "Unknown";
				sb.AppendLine(
					$"\tSide {ArenaSideIndexUtilities.ToDisplayString(actor, reservation.SideIndex).ColourValue()} - {who.ColourName()} (expires {reservation.ExpiresAt.ToString("g", actor).ColourValue()})");
			}
		}

		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this arena event
	#3schedule <datetime>#0 - sets the scheduled start time
	#3registration <datetime|none>#0 - sets or clears the registration open time
	#3state <state>#0 - forcibly sets the state
	#3abort <reason>#0 - aborts the event with a reason";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "schedule":
			case "time":
				return BuildingCommandSchedule(actor, command);
			case "registration":
			case "reg":
				return BuildingCommandRegistration(actor, command);
			case "state":
				return BuildingCommandState(actor, command);
			case "abort":
				return BuildingCommandAbort(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	public void OpenRegistration()
	{
		EnforceState(ArenaEventState.RegistrationOpen);
		RegistrationOpensAt ??= DateTime.UtcNow;
		Changed = true;
	}

	public void CloseRegistration()
	{
		StartPreparation();
	}

	public void StartPreparation()
	{
		if (_state >= ArenaEventState.Preparing)
		{
			EnforceState(ArenaEventState.Preparing);
			Changed = true;
			ApplyPreparationPhaseEffects();
			return;
		}

		AutoFillNpcParticipants();

		EnforceState(ArenaEventState.Preparing);
		Changed = true;
		ApplyPreparationPhaseEffects();
		PrepareNpcParticipants();
		PreparePlayerParticipants();
		ExecuteOutfitProgs();
	}

	public void Stage()
	{
		if (_state >= ArenaEventState.Staged)
		{
			EnforceState(ArenaEventState.Staged);
			Changed = true;
			ApplyCombatPhaseEffects();
			return;
		}

		EnforceState(ArenaEventState.Staged);
		Changed = true;
		ApplyCombatPhaseEffects();
		MoveParticipantsToArena();
		ExecuteIntroProg();
	}

	public void StartLive()
	{
		StartedAt ??= DateTime.UtcNow;
		if (_state >= ArenaEventState.Live)
		{
			EnforceState(ArenaEventState.Live);
			Changed = true;
			ApplyCombatPhaseEffects();
			return;
		}

		EnforceState(ArenaEventState.Live);
		Changed = true;
		ApplyCombatPhaseEffects();
		MoveParticipantsToArena();
		ExecuteScoringProg();
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
		ExecuteResolutionOverrideProg();
	}

	public void Cleanup()
	{
		EnforceState(ArenaEventState.Cleanup);
		Changed = true;
		FinalizeParticipants();
	}

	public void Complete()
	{
		CompletedAt = DateTime.UtcNow;
		EnforceState(ArenaEventState.Completed);
		Changed = true;
		FinalizeParticipants();
	}

	public void Abort(string reason)
	{
		RecordOutcome(ArenaOutcome.Aborted, null);
		CompletedAt = DateTime.UtcNow;
		EnforceState(ArenaEventState.Aborted);
		Changed = true;
		FinalizeParticipants();
	}

	public (bool Truth, string Reason) CanSignUp(ICharacter character, int sideIndex,
		ICombatantClass combatantClass)
	{
		return CanSignUpInternal(character, sideIndex, combatantClass, false, false);
	}

	public void SignUp(ICharacter character, int sideIndex, ICombatantClass combatantClass)
	{
		var result = CanSignUpInternal(character, sideIndex, combatantClass, false, false);
		if (!result.Truth)
		{
			throw new InvalidOperationException(result.Reason);
		}

		CompleteSignup(character, sideIndex, combatantClass, true);
	}

	private (bool Truth, string Reason) CanSignUpInternal(ICharacter character, int sideIndex,
		ICombatantClass combatantClass, bool ignoreNpcRestriction, bool ignoreSidePolicy)
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

		if (!ignoreNpcRestriction && !character.IsPlayerCharacter && !side.AllowNpcSignup)
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

		if (ignoreSidePolicy)
		{
			return (true, string.Empty);
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

	private void CompleteSignup(ICharacter character, int sideIndex, ICombatantClass combatantClass,
		bool checkForEarlyPreparation)
	{
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

		HandleSignupStaging(character, sideIndex);
		if (checkForEarlyPreparation)
		{
			TryAdvanceToPreparation();
		}
	}

	private void TryAdvanceToPreparation()
	{
		if (_state != ArenaEventState.RegistrationOpen)
		{
			return;
		}

		if (!IsEventFull())
		{
			return;
		}

		Gameworld.ArenaLifecycleService.Transition(this, ArenaEventState.Preparing);
	}

	private bool IsEventFull()
	{
		var sides = EventType.Sides.ToList();
		if (sides.Count == 0)
		{
			return false;
		}

		var sideCounts = _participants
			.GroupBy(x => x.SideIndex)
			.ToDictionary(x => x.Key, x => x.Count());

		return sides.All(side =>
			sideCounts.TryGetValue(side.Index, out var count) &&
			count >= side.Capacity);
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
		ClearStagingEffect(character);
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
		AnnounceStateChange(state);
	}

	private void AnnounceStateChange(ArenaEventState currentState)
	{
		var message = BuildStateChangeMessage(currentState);
		if (string.IsNullOrWhiteSpace(message))
		{
			return;
		}

		foreach (var cell in GetAnnouncementCells())
		{
			cell.Handle(message);
		}
	}

	private IEnumerable<ICell> GetAnnouncementCells()
	{
		return Arena.WaitingCells
		            .Concat(Arena.ArenaCells)
		            .Concat(Arena.ObservationCells)
		            .Concat(Arena.InfirmaryCells)
		            .Concat(Arena.AfterFightCells)
		            .Concat(Arena.NpcStablesCells)
		            .Where(cell => cell != null)
		            .Distinct();
	}

	private string? BuildStateChangeMessage(ArenaEventState state)
	{
		var eventName = EventType.Name.ColourName();
		var arenaName = Arena.Name.ColourName();

		return state switch
		{
			ArenaEventState.RegistrationOpen => $"Registration is now open for the {eventName} event in {arenaName}.",
			ArenaEventState.Preparing => $"Registration is now closed for the {eventName} event in {arenaName}.",
			ArenaEventState.Staged => $"Combatants are taking their places for the {eventName} event in {arenaName}.",
			ArenaEventState.Live => $"The {eventName} event is now underway in {arenaName}.",
			ArenaEventState.Resolving => $"The {eventName} event has concluded in {arenaName}.",
			ArenaEventState.Cleanup => $"Cleanup has begun after the {eventName} event in {arenaName}.",
			ArenaEventState.Completed => $"The {eventName} event is complete in {arenaName}.",
			ArenaEventState.Aborted => $"The {eventName} event in {arenaName} has been aborted.",
			_ => null
		};
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

	private void ExecuteIntroProg()
	{
		EventType.IntroProg?.Execute(ArenaProgParameters.BuildEventProgArguments(this));
	}

	private void ExecuteScoringProg()
	{
		EventType.ScoringProg?.Execute(ArenaProgParameters.BuildEventProgArguments(this));
	}

	private void ExecuteOutfitProgs()
	{
		foreach (var side in EventType.Sides)
		{
			var prog = side.OutfitProg;
			if (prog is null)
			{
				continue;
			}

			var participants = _participants
				.Where(x => x.SideIndex == side.Index)
				.Select(x => x.Character)
				.OfType<ICharacter>()
				.ToList();

			prog.Execute(ArenaProgParameters.BuildSideOutfitArguments(this, side.Index, participants));
		}
	}

	private void ExecuteResolutionOverrideProg()
	{
		var prog = EventType.ResolutionOverrideProg;
		if (prog is null)
		{
			return;
		}

		var values = prog.ExecuteCollection<decimal>(ArenaProgParameters.BuildEventProgArguments(this)).ToList();
		if (values.Count == 0)
		{
			return;
		}

		var firstValue = Convert.ToInt32(values[0]);
		var usesOutcome = Enum.IsDefined(typeof(ArenaOutcome), firstValue);
		var outcome = usesOutcome ? (ArenaOutcome)firstValue : ArenaOutcome.Win;
		var winningSides = usesOutcome
			? values.Skip(1).Select(Convert.ToInt32).ToList()
			: values.Select(Convert.ToInt32).ToList();

		if (outcome == ArenaOutcome.Win && winningSides.Count == 0)
		{
			return;
		}

		RecordOutcome(outcome, outcome == ArenaOutcome.Win ? winningSides : null);
	}

	private void AutoFillNpcParticipants()
	{
		if (_state >= ArenaEventState.Preparing)
		{
			return;
		}

		var existingIds = _participants
			.Select(x => x.Character?.Id)
			.Where(x => x.HasValue)
			.Select(x => x.Value)
			.ToHashSet();

		foreach (var side in EventType.Sides)
		{
			if (!side.AutoFillNpc)
			{
				continue;
			}

			var slotsNeeded = side.Capacity - _participants.Count(x => x.SideIndex == side.Index);
			if (slotsNeeded <= 0)
			{
				continue;
			}

			var npcs = Gameworld.ArenaNpcService.AutoFill(this, side.Index, slotsNeeded);
			foreach (var npc in npcs)
			{
				if (npc is null)
				{
					continue;
				}

				if (!existingIds.Add(npc.Id))
				{
					continue;
				}

				var combatantClass = side.EligibleClasses.FirstOrDefault(x => !EligibilityFailed(x, npc));
				if (combatantClass is null)
				{
					continue;
				}

				var signUpCheck = CanSignUpInternal(npc, side.Index, combatantClass, true, true);
				if (!signUpCheck.Truth)
				{
					continue;
				}

				CompleteSignup(npc, side.Index, combatantClass, false);
				slotsNeeded--;
				if (slotsNeeded <= 0)
				{
					break;
				}
			}
		}
	}

	private void PrepareNpcParticipants()
	{
		foreach (var participant in _participants.Where(x => x.IsNpc))
		{
			if (participant.Character is not ICharacter npc)
			{
				continue;
			}

			Gameworld.ArenaNpcService.PrepareNpc(npc, this, participant.SideIndex, participant.CombatantClass);
		}
	}

	private void PreparePlayerParticipants()
	{
		foreach (var participant in _participants)
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			var waitingCell = Arena.GetWaitingCell(participant.SideIndex);
			if (waitingCell is not null && !ReferenceEquals(character.Location, waitingCell))
			{
				character.Teleport(waitingCell, RoomLayer.GroundLevel, false, false);
			}

			if (BringYourOwn || character.Body is null)
			{
				continue;
			}

			var effect = character.CombinedEffectsOfType<ArenaParticipantPreparationEffect>()
				.FirstOrDefault(x => x.EventId == Id);
			if (effect is null)
			{
				effect = new ArenaParticipantPreparationEffect(character, Id);
				character.AddEffect(effect);
			}
			else
			{
				effect.ClearCapturedItems();
			}

			StripParticipantLoadout(character.Body, effect);
		}
	}

	private void FinalizeParticipants()
	{
		ReturnNpcParticipants();
		MovePlayerParticipantsToAfterFight();
		RestorePlayerParticipants();
		ClearParticipantPhaseEffects();
		ClearObservationEffects();
	}

	private void ReturnNpcParticipants()
	{
		var stableCells = Arena.NpcStablesCells.ToList();
		var afterFightCells = Arena.AfterFightCells.ToList();

		foreach (var participant in _participants.Where(x => x.IsNpc))
		{
			var npc = participant.Character;
			if (npc is null)
			{
				continue;
			}

			var effect = npc.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
				.FirstOrDefault(x => x.EventId == Id);
			if (effect is not null)
			{
				Gameworld.ArenaNpcService.ReturnNpc(npc, this, participant.CombatantClass.ResurrectNpcOnDeath);
				continue;
			}

			var destination = stableCells.Count > 0
				? SelectIndexedCell(stableCells, participant.SideIndex)
				: SelectIndexedCell(afterFightCells, participant.SideIndex);
			if (destination is null)
			{
				continue;
			}

			npc.Teleport(destination, RoomLayer.GroundLevel, false, false);
		}
	}

	private void RestorePlayerParticipants()
	{
		foreach (var participant in _participants)
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			var effect = character.CombinedEffectsOfType<ArenaParticipantPreparationEffect>()
				.FirstOrDefault(x => x.EventId == Id);
			if (effect is null)
			{
				continue;
			}

			RestoreParticipantInventory(character, effect);
			character.RemoveEffect(effect, true);
		}
	}

	private void MovePlayerParticipantsToAfterFight()
	{
		var afterFightCells = Arena.AfterFightCells.ToList();
		if (afterFightCells.Count == 0)
		{
			afterFightCells = Arena.WaitingCells.ToList();
		}

		if (afterFightCells.Count == 0)
		{
			afterFightCells = Arena.ArenaCells.ToList();
		}

		if (afterFightCells.Count == 0)
		{
			return;
		}

		foreach (var participant in _participants.Where(x => !x.IsNpc))
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			if (character.Location is not null &&
			    !Arena.ArenaCells.Contains(character.Location) &&
			    !Arena.WaitingCells.Contains(character.Location))
			{
				continue;
			}

			var destination = SelectIndexedCell(afterFightCells, participant.SideIndex);
			if (destination is null)
			{
				continue;
			}

			character.Teleport(destination, RoomLayer.GroundLevel, false, false);
		}
	}

	private void MoveParticipantsToArena()
	{
		var arenaCells = Arena.ArenaCells.ToList();
		if (arenaCells.Count == 0)
		{
			return;
		}

		var sideOffsets = new Dictionary<int, int>();
		foreach (var participant in _participants)
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			if (character.Location is not null && arenaCells.Contains(character.Location))
			{
				continue;
			}

			var offset = sideOffsets.TryGetValue(participant.SideIndex, out var value) ? value : 0;
			var index = (participant.SideIndex + offset) % arenaCells.Count;
			sideOffsets[participant.SideIndex] = offset + 1;

			character.Teleport(arenaCells[index], RoomLayer.GroundLevel, false, false);
		}
	}

	private static void StripParticipantLoadout(IBody body, ArenaParticipantPreparationEffect effect)
	{
		var directItems = body.DirectItems?.OfType<IGameItem>().ToList();
		if (directItems is null || directItems.Count == 0)
		{
			return;
		}

		foreach (var item in directItems)
		{
			var state = DetermineState(body, item);
			var wearProfileId = item.GetItemType<IWearable>()?.CurrentProfile?.Id;
			var bodypartId = body.BodypartLocationOfInventoryItem(item)?.Id;
			effect.CaptureItem(item, state, wearProfileId, bodypartId);
			body.Take(item);
		}
	}

	private static void RestoreParticipantInventory(ICharacter participant, ArenaParticipantPreparationEffect effect)
	{
		if (participant.Body is null)
		{
			DropCapturedItems(participant, effect);
			return;
		}

		foreach (var snapshot in effect.Items)
		{
			var item = snapshot.Item;
			if (item is null || item.Deleted)
			{
				continue;
			}

			participant.Body.Get(item, silent: true,
				ignoreFlags: ItemCanGetIgnore.IgnoreWeight | ItemCanGetIgnore.IgnoreFreeHands);

			switch (snapshot.State)
			{
				case InventoryState.Worn:
					TryWearItem(participant.Body, item, snapshot.WearProfileId);
					break;
				case InventoryState.Wielded:
					TryWieldItem(participant.Body, item, snapshot.BodypartId);
					break;
			}
		}
	}

	private static void DropCapturedItems(ICharacter participant, ArenaParticipantPreparationEffect effect)
	{
		var location = participant.Location;
		if (location is null)
		{
			return;
		}

		foreach (var snapshot in effect.Items)
		{
			var item = snapshot.Item;
			if (item is null || item.Deleted)
			{
				continue;
			}

			if (item.InInventoryOf is not null || item.Location is not null || item.ContainedIn is not null)
			{
				continue;
			}

			item.RoomLayer = participant.RoomLayer;
			location.Insert(item, true);
		}
	}

	private static void TryWearItem(IBody body, IGameItem item, long? wearProfileId)
	{
		var wearable = item.GetItemType<IWearable>();
		if (wearable is null)
		{
			return;
		}

		var profile = wearProfileId.HasValue
			? wearable.Profiles.FirstOrDefault(x => x.Id == wearProfileId.Value)
			: wearable.CurrentProfile;

		if (profile is not null)
		{
			body.Wear(item, profile, null, true);
		}
		else
		{
			body.Wear(item, null, true);
		}
	}

	private static void TryWieldItem(IBody body, IGameItem item, long? bodypartId)
	{
		var hand = bodypartId.HasValue
			? body.Bodyparts.FirstOrDefault(x => x.Id == bodypartId.Value) as IWield
			: null;

		body.Wield(item, hand, null, true, ItemCanWieldFlags.IgnoreFreeHands);
	}

	private static InventoryState DetermineState(IBody body, IGameItem item)
	{
		if (body.WornItems.Contains(item))
		{
			return InventoryState.Worn;
		}

		if (body.WieldedItems.Contains(item))
		{
			return InventoryState.Wielded;
		}

		return InventoryState.Held;
	}

	private static ICell? SelectIndexedCell(IReadOnlyList<ICell> cells, int index)
	{
		if (cells.Count == 0)
		{
			return null;
		}

		if (index >= 0 && index < cells.Count)
		{
			return cells[index];
		}

		return cells[0];
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should this arena event have?".SubstituteANSIColour());
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This arena event is now called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSchedule(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("When should this event be scheduled for?".SubstituteANSIColour());
			return false;
		}

		if (!DateUtilities.TryParseDateTimeOrRelative(command.SafeRemainingArgument, actor.Account, false, out var when))
		{
			actor.OutputHandler.Send("That is not a valid date/time.".ColourError());
			return false;
		}

		ScheduledAt = when;
		var regOpens = ScheduledAt - PreparationDuration - RegistrationDuration;
		if (regOpens > CreatedAt)
		{
			RegistrationOpensAt = regOpens;
		}

		Changed = true;
		actor.OutputHandler.Send($"This event is now scheduled for {ScheduledAt.ToString("f", actor).ColourValue()}.");
		RescheduleTransitions();
		return true;
	}

	private bool BuildingCommandRegistration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("When should registration open? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			RegistrationOpensAt = null;
			Changed = true;
			actor.OutputHandler.Send("Registration open time cleared.".SubstituteANSIColour());
			RescheduleTransitions();
			return true;
		}

		if (!DateUtilities.TryParseDateTimeOrRelative(command.SafeRemainingArgument, actor.Account, false, out var when))
		{
			actor.OutputHandler.Send("That is not a valid date/time.".ColourError());
			return false;
		}

		RegistrationOpensAt = when;
		Changed = true;
		actor.OutputHandler.Send($"Registration will open at {RegistrationOpensAt.Value.ToString("f", actor).ColourValue()}.");
		RescheduleTransitions();
		return true;
	}

	private bool BuildingCommandState(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which state should this event be forced into? Valid options are {Enum.GetValues<ArenaEventState>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ArenaEventState>(out var state))
		{
			actor.OutputHandler.Send(
				$"That is not a valid state. Valid options are {Enum.GetValues<ArenaEventState>().ListToColouredString()}.");
			return false;
		}

		ApplyForcedState(state);
		Changed = true;
		actor.OutputHandler.Send($"State set to {State.DescribeEnum().ColourValue()}.");
		RescheduleTransitions();
		return true;
	}

	private bool BuildingCommandAbort(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What reason do you want to give for aborting this event?".SubstituteANSIColour());
			return false;
		}

		var reason = command.SafeRemainingArgument;
		Abort(reason);
		actor.OutputHandler.Send($"You abort this event: {reason.ColourError()}");
		RescheduleTransitions();
		return true;
	}

	private void RescheduleTransitions()
	{
		Gameworld.ArenaScheduler.Schedule(this);
	}

	private void ApplyForcedState(ArenaEventState state)
	{
		switch (state)
		{
			case ArenaEventState.RegistrationOpen:
				OpenRegistration();
				break;
			case ArenaEventState.Preparing:
				StartPreparation();
				break;
			case ArenaEventState.Staged:
				Stage();
				break;
			case ArenaEventState.Live:
				StartLive();
				break;
			case ArenaEventState.Resolving:
				Resolve();
				break;
			case ArenaEventState.Cleanup:
				Cleanup();
				break;
			case ArenaEventState.Completed:
				Complete();
				break;
			case ArenaEventState.Aborted:
				Abort("Event aborted.");
				break;
			default:
				EnforceState(state);
				break;
		}
	}

	private void HandleSignupStaging(ICharacter character, int sideIndex)
	{
		if (character is null)
		{
			return;
		}

		var waitingCell = Arena.GetWaitingCell(sideIndex);
		if (waitingCell is null)
		{
			return;
		}

		var signupEcho = Arena.SignupEcho;
		var originCell = character.Location;
		if (!string.IsNullOrWhiteSpace(signupEcho) && originCell is not null)
		{
			originCell.Handle(new EmoteOutput(new Emote(signupEcho, character)));
		}

		character.Teleport(waitingCell, RoomLayer.GroundLevel, false, false);

		if (!string.IsNullOrWhiteSpace(signupEcho) && !ReferenceEquals(originCell, waitingCell))
		{
			waitingCell.Handle(new EmoteOutput(new Emote(signupEcho, character)));
		}

		if (!character.IsPlayerCharacter)
		{
			return;
		}

		var existing = character.CombinedEffectsOfType<ArenaStagingEffect>()
			.FirstOrDefault(x => x.Matches(this));
		if (existing is not null)
		{
			existing.AttachToEvent(this);
			return;
		}

		character.AddEffect(new ArenaStagingEffect(character, this));
	}

	private void ClearStagingEffect(ICharacter character)
	{
		if (character is null || !character.IsPlayerCharacter)
		{
			return;
		}

		var effect = character.CombinedEffectsOfType<ArenaStagingEffect>()
			.FirstOrDefault(x => x.Matches(this));
		if (effect is null)
		{
			return;
		}

		character.RemoveEffect(effect, true);
	}

	private void ClearStagingEffects()
	{
		foreach (var participant in _participants)
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			ClearStagingEffect(character);
		}
	}

	private void ApplyPreparationPhaseEffects()
	{
		foreach (var participant in _participants.Where(x => !x.IsNpc))
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			EnsurePreparingEffect(character);
			ClearCombatantEffect(character);
		}

		ClearStagingEffects();
	}

	private void ApplyCombatPhaseEffects()
	{
		foreach (var participant in _participants.Where(x => !x.IsNpc))
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			EnsureParticipantOutputHandler(character);
			ClearPreparingEffect(character);
			EnsureCombatantEffect(character);
		}

		ClearStagingEffects();
	}

	private void EnsurePreparingEffect(ICharacter character)
	{
		if (!character.IsPlayerCharacter)
		{
			return;
		}

		character.RemoveAllEffects(effect => effect.IsEffectType<LinkdeadLogout>());
		var existing = character.CombinedEffectsOfType<ArenaPreparingEffect>()
			.FirstOrDefault(x => x.Matches(this));
		if (existing is not null)
		{
			existing.AttachToEvent(this);
			return;
		}

		character.AddEffect(new ArenaPreparingEffect(character, this));
	}

	private void ClearPreparingEffect(ICharacter character)
	{
		if (!character.IsPlayerCharacter)
		{
			return;
		}

		var effect = character.CombinedEffectsOfType<ArenaPreparingEffect>()
			.FirstOrDefault(x => x.Matches(this));
		if (effect is null)
		{
			return;
		}

		character.RemoveEffect(effect, true);
	}

	private void ClearPreparationEffects()
	{
		foreach (var participant in _participants.Where(x => !x.IsNpc))
		{
			var character = participant.Character;
			if (character is null)
			{
				continue;
			}

			ClearPreparingEffect(character);
		}
	}

	private void EnsureCombatantEffect(ICharacter character)
	{
		Gameworld.ArenaParticipationService.EnsureParticipation(character, this);
	}

	private void ClearCombatantEffect(ICharacter character)
	{
		Gameworld.ArenaParticipationService.ClearParticipation(character, this);
	}

	private void ClearParticipantPhaseEffects()
	{
		ClearPreparationEffects();
		Gameworld.ArenaParticipationService.ClearParticipation(this);
		ClearStagingEffects();
	}

	private void ClearObservationEffects()
	{
		foreach (var cell in Arena.ArenaCells)
		{
			var effects = cell.EffectsOfType<ArenaWatcherEffect>()
				.Where(x => ReferenceEquals(x.ArenaEvent, this))
				.ToList();

			foreach (var effect in effects)
			{
				cell.RemoveEffect(effect, true);
			}
		}
	}

	private static void EnsureParticipantOutputHandler(ICharacter character)
	{
		if (character.OutputHandler is not null)
		{
			return;
		}

		character.Register(new NonPlayerOutputHandler());
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
	public IFuturemud Gameworld => _event.Gameworld;
	public string Name => StageName ?? Character?.Name ?? $"Participant {SignupId:N0}";
	public long Id => SignupId;
	public string FrameworkItemType => "ArenaParticipant";

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Arena Participant #{SignupId.ToStringN0(actor)}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Character: {(Character?.HowSeen(actor) ?? "NPC".Colour(Telnet.Yellow))}");
		sb.AppendLine($"Combatant Class: {CombatantClass.Name.ColourName()}");
		sb.AppendLine($"Side: {ArenaSideIndexUtilities.ToDisplayString(actor, SideIndex).ColourValue()}");
		sb.AppendLine($"NPC Signup: {IsNpc.ToColouredString()}");
		sb.AppendLine($"Stage Name: {(string.IsNullOrWhiteSpace(StageName) ? "None".ColourError() : StageName.ColourName())}");
		sb.AppendLine($"Signature Colour: {(string.IsNullOrWhiteSpace(SignatureColour) ? "None".ColourError() : SignatureColour.ColourValue())}");
		if (StartingRating.HasValue)
		{
			sb.AppendLine($"Starting Rating: {StartingRating.Value.ToString("N2", actor).ColourValue()}");
		}

		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("Arena participants are informational only and cannot be edited directly.");
		return false;
	}

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
