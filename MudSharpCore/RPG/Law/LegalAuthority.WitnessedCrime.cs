#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.NPC.AI;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority
{
	public ICrime? ReportWitnessedCrime(ILaw law, ICharacter criminal, ICharacter witness,
		ICharacter? victim, IGameItem? item, string context)
	{
		var location = criminal.Location;
		bool ValidScene() => CommandExecutionScope.TryContinue() && location is not null &&
			ReferenceEquals(criminal.Gameworld, Gameworld) && ReferenceEquals(witness.Gameworld, Gameworld) &&
			ReferenceEquals(criminal.Location, location) && witness.ColocatedWith(criminal) &&
			!CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(witness, criminal) &&
			witness.CanSee(criminal, PerceiveIgnoreFlags.None);
		if (RuntimeSideEffectContext.IsCrimeCreationSuppressed || criminal.IsAdministrator() ||
			!ReferenceEquals(law.Authority, this) || !Laws.Any(x => ReferenceEquals(x, law)) ||
			!ValidScene() || !EnforcementZones.Contains(location.Zone) ||
			!law.IsCrime(criminal, victim!, item!, context) || !ValidScene() ||
			!Laws.Any(x => ReferenceEquals(x, law)) || !EnforcementZones.Contains(location.Zone)) return null;
		var identity = CharacterInstanceIdentityComparer.IdentityId(criminal);
		if (ShouldSuppressAutomaticRepeatCrime(law,
			_knownCrimesLookup[identity].Concat(_unknownCrimesLookup[identity]),
			victim!, item!, location, RuntimeClock.UtcNow)) return null;

		CommandExecutionScope.MarkCommitted();
		var crime = new Crime(criminal, victim, [witness], law, item, context, location);
		_unknownCrimes.Add(crime);
		_unknownCrimesLookup.Add(identity, crime);
		Gameworld.Add(crime);
		// This action is an explicit witnessed report. Commit the legal record before
		// dispatching narrative/event callbacks, which can move or replace characters.
		ReportCrime(crime, witness, false, 1.0);
		Gameworld.MediaChannelService?.CaptureCrime(crime);
		MudSharp.RPG.AIStorytellers.AIStoryteller.HandleCrimeCommittedInRoomEvent(crime);
		witness.HandleEvent(EventType.WitnessedCrime, criminal, victim!, witness, crime);
		victim?.HandleEvent(EventType.VictimOfCrime, criminal, victim, crime);
		Changed = true;
		return crime;
	}
}
