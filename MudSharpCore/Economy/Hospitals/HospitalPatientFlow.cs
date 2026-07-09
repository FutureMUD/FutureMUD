using System;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public static class HospitalPatientFlow
{
	public static bool TryReserveTreatmentLocation(IHospital hospital, IHospitalServiceRequest request,
		out ICell? location, out string reason)
	{
		location = null;
		reason = string.Empty;

		if (!HospitalMedicalServiceRunner.ShouldUseTreatmentTheatre(request.Service))
		{
			location = request.Patient?.Location ?? hospital.WaitingRooms.FirstOrDefault();
			return location is not null;
		}

		if (request.OperatingTheatreCellId is { } theatreId)
		{
			var reserved = (hospital.OperatingTheatres ?? Array.Empty<ICell>()).FirstOrDefault(x => x.Id == theatreId);
			if (reserved is null)
			{
				reason = "The reserved operating theatre is no longer configured for this hospital.";
				return false;
			}

			if (!IsTheatreAvailable(hospital, request, reserved, out reason))
			{
				return false;
			}

			location = reserved;
			return true;
		}

		foreach (var theatre in hospital.OperatingTheatres ?? Array.Empty<ICell>())
		{
			if (!IsTheatreAvailable(hospital, request, theatre, out _))
			{
				continue;
			}

			request.OperatingTheatreCellId = theatre.Id;
			request.UsedInPlaceFallback = false;
			location = theatre;
			return true;
		}

		reason = $"{hospital.Name} has no empty operating theatre for request #{request.Id.ToString("N0")}.";
		return false;
	}

	public static bool TransferForTreatment(IHospital hospital, IHospitalServiceRequest request, ICharacter employee,
		ICharacter patient, ICell treatmentLocation, out string reason)
	{
		reason = string.Empty;
		request.ReturnCellId ??= patient.Location?.Id;
		var patientAlreadyThere = patient.Location?.Id == treatmentLocation.Id;
		if (patientAlreadyThere && employee.Location?.Id == treatmentLocation.Id)
		{
			return true;
		}

		var origin = patient.Location ?? employee.Location;
		if (origin is not null && origin.Id != treatmentLocation.Id)
		{
			origin.HandleRoomEcho(new EmoteOutput(new Emote(
				"@ take|takes $0 into an operating theatre for treatment.", employee, patient)));
		}

		if (!patientAlreadyThere)
		{
			patient.OutputHandler.Send(
				$"You are taken into {treatmentLocation.Name.ColourName()} for hospital treatment.");
		}

		MoveCharacter(patient, treatmentLocation);
		MoveCharacter(employee, treatmentLocation);
		treatmentLocation.HandleRoomEcho(new EmoteOutput(new Emote(
			"@ arrive|arrives with $0 for hospital treatment.", employee, patient)));
		return true;
	}

	public static void TransferAfterTreatment(IHospital hospital, IHospitalServiceRequest request, ICharacter? employee,
		string auditPrefix)
	{
		if (!request.Service.RequiresRecovery || request.Patient is not { } patient)
		{
			return;
		}

		var recoveryRoom = hospital.RecoveryRooms.FirstOrDefault();
		var destination = recoveryRoom ?? hospital.WaitingRooms.FirstOrDefault();
		if (destination is null)
		{
			request.MarkStatus(request.Status,
				$"{auditPrefix}: no recovery or waiting room was available, so the patient was left in place.");
			return;
		}

		var destinationIsRecovery = recoveryRoom is not null;
		var origin = patient.Location;
		var employeeCanEscort = employee is not null &&
		                        origin is not null &&
		                        employee.Location?.Id == origin.Id;
		if (origin is not null && origin.Id != destination.Id)
		{
			if (employeeCanEscort)
			{
				origin.HandleRoomEcho(new EmoteOutput(new Emote(
					destinationIsRecovery
						? "@ escort|escorts $0 to recovery after treatment."
						: "@ escort|escorts $0 back to the waiting room after treatment.", employee!, patient)));
			}
			else
			{
				origin.HandleRoomEcho(new EmoteOutput(new Emote(
					destinationIsRecovery
						? "$0 $0|are|is taken to recovery after treatment."
						: "$0 $0|are|is escorted back to the waiting room after treatment.", patient, patient)));
			}
		}

		MoveCharacter(patient, destination);
		if (employeeCanEscort)
		{
			MoveCharacter(employee!, destination);
		}

		if (destinationIsRecovery)
		{
			request.RecoveryRoomCellId = destination.Id;
			if (patient.IsHelpless)
			{
				PlaceOnRecoveryBed(patient, destination);
			}

			patient.OutputHandler.Send(
				$"Your hospital treatment is complete, and you are {(patient.IsHelpless ? "brought" : "escorted")} to {destination.Name.ColourName()} for recovery.");
			destination.HandleRoomEcho(new EmoteOutput(employeeCanEscort
				? new Emote("@ arrive|arrives with $0 for recovery.", employee!, patient)
				: new Emote("$0 $0|are|is brought in for recovery.", patient, patient)));
			return;
		}

		request.ReturnCellId = destination.Id;
		patient.OutputHandler.Send(
			$"Your hospital treatment is complete, and you are returned to {destination.Name.ColourName()}.");
		destination.HandleRoomEcho(new EmoteOutput(employeeCanEscort
			? new Emote("@ arrive|arrives with $0 after hospital treatment.", employee!, patient)
			: new Emote("$0 $0|return|returns from hospital treatment.", patient, patient)));
	}

	public static bool TryGetUnavailablePatientReason(IHospital hospital, IHospitalServiceRequest request,
		ICharacter? employee, bool requirePreparedTreatmentLocation, out string reason)
	{
		reason = string.Empty;
		if (request.Patient is not { } patient)
		{
			reason = "The patient is no longer loaded or available for treatment.";
			return true;
		}

		var patientName = patient.HowSeen(employee ?? patient, colour: false);
		if (patient.State.HasFlag(CharacterState.Dead))
		{
			reason = $"{patientName} has died and can no longer receive hospital treatment.";
			return true;
		}

		if (patient.State.HasFlag(CharacterState.Stasis))
		{
			reason = $"{patientName} is no longer present and can no longer receive hospital treatment.";
			return true;
		}

		var patientLocation = patient.Location;
		if (patientLocation is null)
		{
			reason = $"{patientName} is not presently in a known location.";
			return true;
		}

		var hospitalLocations = (hospital.Locations ?? Enumerable.Empty<ICell>()).ToList();
		if (hospitalLocations.Any() && hospitalLocations.All(x => x.Id != patientLocation.Id))
		{
			reason = $"{patientName} is no longer in a configured location for {hospital.Name}.";
			return true;
		}

		if (requirePreparedTreatmentLocation &&
		    request.ReturnCellId.HasValue &&
		    request.OperatingTheatreCellId is { } theatreId &&
		    patientLocation.Id != theatreId)
		{
			var theatre = (hospital.OperatingTheatres ?? Enumerable.Empty<ICell>()).FirstOrDefault(x => x.Id == theatreId);
			reason = theatre is null
				? $"{patientName} is no longer in the reserved operating theatre."
				: $"{patientName} is no longer in the reserved operating theatre {theatre.Name}.";
			return true;
		}

		if (employee is not null &&
		    employee.ColocatedWith(patient) &&
		    !employee.CanSee(patient))
		{
			reason = $"{employee.HowSeen(employee, colour: false)} can no longer see {patientName} to continue hospital treatment.";
			return true;
		}

		return false;
	}

	public static bool IsTheatreAvailable(IHospital hospital, IHospitalServiceRequest request, ICell theatre,
		out string reason)
	{
		var conflictingRequest = (hospital.ActiveServiceRequests ?? Array.Empty<IHospitalServiceRequest>())
		                                 .FirstOrDefault(x =>
			                                 !IsSameRequest(x, request) &&
			                                 x.OperatingTheatreCellId == theatre.Id);
		if (conflictingRequest is not null)
		{
			reason = $"Operating theatre {theatre.Name} is already reserved for active hospital request #{conflictingRequest.Id.ToString("N0")}.";
			return false;
		}

		if ((theatre.Characters ?? Array.Empty<ICharacter>()).Any(x => !x.IsAdministrator() && !hospital.IsEmployee(x) && !IsRequestPatient(request, x)))
		{
			reason = $"Operating theatre {theatre.Name} is occupied by someone unrelated to this request.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool IsSameRequest(IHospitalServiceRequest request, IHospitalServiceRequest other)
	{
		if (request.Id != 0 && request.Id == other.Id)
		{
			return true;
		}

		return request.EmploymentTaskId is { } requestTaskId &&
		       other.EmploymentTaskId is { } otherTaskId &&
		       requestTaskId == otherTaskId;
	}

	private static bool IsRequestPatient(IHospitalServiceRequest request, ICharacter occupant)
	{
		if (request.Patient is { } patient &&
		    (CharacterInstanceIdentityComparer.SamePhysicalInstance(occupant, patient) ||
		     CharacterInstanceIdentityComparer.SameIdentity(occupant, patient)))
		{
			return true;
		}

		return request.PatientId > 0 &&
		       CharacterInstanceIdentityComparer.IdentityId(occupant) == request.PatientId;
	}

	private static void MoveCharacter(ICharacter character, ICell destination)
	{
		if (character.Location?.Id == destination.Id)
		{
			return;
		}

		character.Location?.Leave(character);
		destination.Enter(character, noSave: true, roomLayer: character.RoomLayer);
	}

	private static void PlaceOnRecoveryBed(ICharacter patient, ICell destination)
	{
		var bed = destination.GameItems
		                     .SelectMany(x => x.DeepItems.Append(x))
		                     .DistinctBy(x => x.Id)
		                     .FirstOrDefault(x => x.CanBePositionedAgainst(PositionLyingDown.Instance, PositionModifier.On) &&
		                                          patient.CanMovePosition(PositionLyingDown.Instance, PositionModifier.On, x,
			                                          true, true));
		if (bed is null)
		{
			return;
		}

		patient.MovePosition(PositionLyingDown.Instance, PositionModifier.On, bed, null, null, true, true);
	}
}
