#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Community;

public static class ClanCommandUtilities
{
	public static bool HoldsOrControlsAppointment(IClanMembership? membership, IAppointment? appointment)
	{
		if (membership is null || appointment is null)
		{
			return false;
		}

		if (membership.Appointments.Contains(appointment))
		{
			return true;
		}

		return HoldsOrControlsAppointment(membership, appointment.ParentPosition);
	}

	public static bool HasReachedConsecutiveTermLimit(IEnumerable<IElection> elections, long characterId,
		int maximumConsecutiveTerms)
	{
		if (maximumConsecutiveTerms <= 0)
		{
			return false;
		}

		var pastElections = elections
			.Where(x => x.IsFinalised && !x.IsByElection)
			.OrderByDescending(x => x.ResultsInEffectDate)
			.Take(maximumConsecutiveTerms)
			.ToList();

		return pastElections.Count >= maximumConsecutiveTerms &&
		       pastElections.All(x => x.Victors.Any(y => y.MemberId == characterId));
	}

	public static bool HasReachedTotalTermLimit(IEnumerable<IElection> elections, long characterId, int maximumTotalTerms)
	{
		if (maximumTotalTerms <= 0)
		{
			return false;
		}

		return elections
			.Where(x => x.IsFinalised && !x.IsByElection)
			.Count(x => x.Victors.Any(y => y.MemberId == characterId)) >= maximumTotalTerms;
	}

	public static bool HasFreePosition(IAppointment appointment, IEnumerable<IClanMembership> memberships,
		IEnumerable<IExternalClanControl> externalControls)
	{
		return GetVacantAppointmentSlots(appointment, memberships, externalControls) >= 1;
	}

	public static int GetActiveAppointmentHolderCount(IAppointment appointment,
		IEnumerable<IClanMembership> memberships)
	{
		return memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(appointment));
	}

	public static int GetReservedExternalAppointmentSlots(IAppointment appointment,
		IEnumerable<IExternalClanControl> externalControls)
	{
		return externalControls
			.Where(x => x.VassalClan == appointment.Clan && x.ControlledAppointment == appointment)
			.Sum(x => x.NumberOfAppointments > x.Appointees.Count ? x.NumberOfAppointments - x.Appointees.Count : 0);
	}

	public static int GetVacantAppointmentSlots(IAppointment appointment, IEnumerable<IClanMembership> memberships,
		IEnumerable<IExternalClanControl> externalControls)
	{
		if (appointment.MaximumSimultaneousHolders < 1)
		{
			return 0;
		}

		return Math.Max(0,
			appointment.MaximumSimultaneousHolders -
			GetActiveAppointmentHolderCount(appointment, memberships) -
			GetReservedExternalAppointmentSlots(appointment, externalControls));
	}

	public static int GetOpenByElectionAppointmentSlots(IAppointment appointment, IElection? excludingElection = null)
	{
		return appointment.Elections
			.Where(x => x != excludingElection && !x.IsFinalised && x.IsByElection)
			.Sum(x => x.NumberOfAppointments);
	}

	public static int GetUncoveredAppointmentVacancies(IAppointment appointment,
		IEnumerable<IClanMembership> memberships, IEnumerable<IExternalClanControl> externalControls,
		IElection? excludingElection = null)
	{
		return Math.Max(0,
			GetVacantAppointmentSlots(appointment, memberships, externalControls) -
			GetOpenByElectionAppointmentSlots(appointment, excludingElection));
	}

	public static IElection? GetPrimaryOpenElection(IAppointment appointment)
	{
		return appointment.Elections
			.Where(x => !x.IsFinalised && !x.IsByElection)
			.OrderBy(x => x.ResultsInEffectDate)
			.FirstOrDefault();
	}

	public static IElection? GetFirstOpenByElection(IAppointment appointment)
	{
		return GetOpenByElections(appointment)
			.FirstOrDefault();
	}

	public static IEnumerable<IElection> GetOpenByElections(IAppointment appointment)
	{
		return appointment.Elections
			.Where(x => !x.IsFinalised && x.IsByElection)
			.OrderBy(x => x.ResultsInEffectDate)
			.ToList();
	}

	public static IElection? GetNextOpenElection(IAppointment appointment)
	{
		return appointment.Elections
			.Where(x => !x.IsFinalised)
			.OrderBy(x => x.ResultsInEffectDate)
			.FirstOrDefault();
	}

	public static bool ElectionNeedsContestedVote(IElection election)
	{
		return election.Nominees.Count() > election.NumberOfAppointments;
	}
}
#nullable restore
