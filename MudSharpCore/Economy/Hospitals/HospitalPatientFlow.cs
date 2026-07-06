using System;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
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
		if (patient.Location?.Id == treatmentLocation.Id && employee.Location?.Id == treatmentLocation.Id)
		{
			return true;
		}

		var origin = patient.Location ?? employee.Location;
		if (origin is not null && origin.Id != treatmentLocation.Id)
		{
			origin.HandleRoomEcho(new EmoteOutput(new Emote(
				"@ take|takes $0 into an operating theatre for treatment.", employee, patient)));
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

		var destination = patient.IsHelpless
			? hospital.RecoveryRooms.FirstOrDefault()
			: hospital.WaitingRooms.FirstOrDefault();
		if (destination is null)
		{
			request.MarkStatus(request.Status,
				$"{auditPrefix}: no {(patient.IsHelpless ? "recovery" : "waiting")} room was available, so the patient was left in place.");
			return;
		}

		var origin = patient.Location;
		if (origin is not null && origin.Id != destination.Id)
		{
			origin.HandleRoomEcho(new EmoteOutput(new Emote(
				patient.IsHelpless
					? "$0 $0|are|is taken to recovery after treatment."
					: "$0 $0|are|is escorted back to the waiting room after treatment.", patient, patient)));
		}

		MoveCharacter(patient, destination);
		if (patient.IsHelpless)
		{
			request.RecoveryRoomCellId = destination.Id;
			PlaceOnRecoveryBed(patient, destination);
			destination.HandleRoomEcho(new EmoteOutput(new Emote("$0 $0|are|is brought in for recovery.", patient, patient)));
			return;
		}

		request.ReturnCellId = destination.Id;
		destination.HandleRoomEcho(new EmoteOutput(new Emote("$0 $0|return|returns from hospital treatment.", patient, patient)));
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
