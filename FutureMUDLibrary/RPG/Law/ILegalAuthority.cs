using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg;
using MudSharp.Economy;

namespace MudSharp.RPG.Law
{
    public interface ILegalAuthority : IFrameworkItem, ISaveable, IEditableItem, IFutureProgVariable
    {
        void Delete();
        ICurrency Currency { get; }
        IEnumerable<ILaw> Laws { get; }
        void AddLaw(ILaw law);
        void RemoveLaw(ILaw law);

        IEnumerable<IEnforcementAuthority> EnforcementAuthorities { get; }
        void AddEnforcementAuthority(IEnforcementAuthority authority);
        void RemoveEnforcementAuthority(IEnforcementAuthority authority);

        IEnumerable<IZone> EnforcementZones { get; }
        void AddEnforcementZone(IZone zone);
        void RemoveEnforcementZone(IZone zone);

        IEnumerable<ILegalClass> LegalClasses { get; }
        void AddLegalClass(ILegalClass item);
        void RemoveLegalClass(ILegalClass item);

        IEnumerable<IPatrolRoute> PatrolRoutes { get; }
        void AddPatrolRoute(IPatrolRoute route);
        void RemovePatrolRoute(IPatrolRoute route);

        IEnumerable<ICrime> KnownCrimes { get; }
        IEnumerable<ICrime> StaleCrimes { get; }
        IEnumerable<ICrime> UnknownCrimes { get; }
        IEnumerable<ICrime> ResolvedCrimes { get; }

        IEnumerable<ICrime> KnownCrimesForIndividual(ICharacter individual);
        IEnumerable<ICrime> UnknownCrimesForIndividual(ICharacter individual);
        IEnumerable<ICrime> ResolvedCrimesForIndividual(ICharacter individual);

        IEnumerable<ICrime> CheckPossibleCrime(ICharacter criminal, CrimeTypes crime, ICharacter victim, IGameItem item, string additionalInformation);
        bool WouldBeACrime(ICharacter criminal, CrimeTypes crime, ICharacter victim, IGameItem item,
	        string additionalInformation);
        ILegalClass GetLegalClass(ICharacter character);
        IEnforcementAuthority GetEnforcementAuthority(ICharacter character);
        void ReportCrime(ICrime crime, ICharacter witness, bool identityKnown, double reliability);
        void AccuseCrime(ICrime crime);
        void RemoveCrime(ICrime crime);

        bool PlayersKnowTheirCrimes { get; }
        TimeSpan AutomaticConvictionTime { get; }

        ICell PreparingLocation { get; }
        ICell MarshallingLocation { get; }
        ICell EnforcerStowingLocation { get; }
        ICell PrisonLocation { get; }
        ICell PrisonReleaseLocation { get; }
        ICell PrisonerBelongingsStorageLocation { get; }
        ICell JailLocation { get; }
        ICell CourtLocation { get; }
        IEnumerable<ICell> CellLocations { get; }
        IEnumerable<ICell> JailLocations { get; }

        IFutureProg OnPrisonerImprisoned { get; }
        IFutureProg OnPrisonerReleased { get; }
        IFutureProg BailCalculationProg { get; }

        IPatrolController PatrolController { get; }
        IEnumerable<IPatrol> Patrols { get; }
        IBankAccount BankAccount { get; }

        void AddPatrol(IPatrol patrol);
        void RemovePatrol(IPatrol patrol);
        void LoadPatrols();
        void IncarcerateCriminal(ICharacter who);
        void HandleDiscordNotification(ICrime crime);
        void HandleDiscordNotificationOfEnforcement(ICrime crime, IPatrol patrol);
        void HandleDiscordNotificationOfConviction(ICharacter criminal, ICrime crime, PunishmentResult result, ICharacter enforcer);
        void HandleDiscordNotificationOfForgiveness(ICrime crime, ICharacter enforcer);
        void ConvictAllKnownCrimes(ICharacter criminal, ICharacter judge);
        void SendCharacterToPrison(ICharacter criminal);
        void ReleaseCharacterToFreedom(ICharacter criminal);
        void CheckCharacterForCustodyChanges(ICharacter criminal);
        void CalculateAndSetBail(ICharacter criminal);
    }
}
