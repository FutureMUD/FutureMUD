using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LegalAuthority
    {
        public LegalAuthority()
        {
            EnforcementAuthorities = new HashSet<EnforcementAuthority>();
            Laws = new HashSet<Law>();
            LegalAuthoritiesZones = new HashSet<LegalAuthoritiesZones>();
            LegalClasses = new HashSet<LegalClass>();
            WitnessProfilesCooperatingAuthorities = new HashSet<WitnessProfilesCooperatingAuthorities>();
            PatrolRoutes = new HashSet<PatrolRoute>();
            LegalAuthorityCells = new HashSet<LegalAuthorityCells>();
            LegalAuthorityJailCells = new HashSet<LegalAuthorityJailCell>();
            Patrols = new HashSet<Patrol>();
            Fines = new HashSet<LegalAuthorityFine>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long CurrencyId { get; set; }
        public bool AutomaticallyConvict { get; set; }
        public double AutomaticConvictionTime { get; set; }
        public bool PlayersKnowTheirCrimes { get; set; }
        public long? PreparingLocationId { get; set; }
        public long? MarshallingLocationId { get; set; }
        public long? EnforcerStowingLocationId { get; set; }
        public long? PrisonLocationId { get; set; }
        public long? PrisonReleaseLocationId { get; set; }
        public long? PrisonBelongingsLocationId { get; set; }
        public long? JailLocationId { get; set; }
        public long? CourtLocationId { get; set; }
        public long? OnReleaseProgId { get; set; }
        public long? OnImprisonProgId { get; set; }
        public long? OnHoldProgId { get; set; }
        public long? BailCalculationProgId { get; set; }
        public ulong? GuardianDiscordChannel { get; set; }
        public long? BankAccountId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<EnforcementAuthority> EnforcementAuthorities { get; set; }
        public virtual ICollection<Law> Laws { get; set; }
        public virtual ICollection<LegalAuthoritiesZones> LegalAuthoritiesZones { get; set; }
        public virtual ICollection<LegalClass> LegalClasses { get; set; }
        public virtual ICollection<WitnessProfilesCooperatingAuthorities> WitnessProfilesCooperatingAuthorities { get; set; }
        public virtual ICollection<PatrolRoute> PatrolRoutes { get; set; }
        public virtual ICollection<LegalAuthorityCells> LegalAuthorityCells { get; set; }
        public virtual ICollection<LegalAuthorityJailCell> LegalAuthorityJailCells { get; set; }
        public virtual ICollection<Patrol> Patrols { get; set; }
        public virtual ICollection<LegalAuthorityFine> Fines { get; set; }
        public virtual Cell PreparingLocation { get; set; }
        public virtual Cell MarshallingLocation { get; set; }
        public virtual Cell EnforcerStowingLocation { get; set; }
        public virtual Cell PrisonLocation { get; set; }
        public virtual Cell PrisonReleaseLocation { get; set; }
        public virtual Cell PrisonBelongingsLocation { get; set; }
        public virtual Cell JailLocation { get; set; }
        public virtual Cell CourtLocation { get; set;}
        public virtual FutureProg OnReleaseProg { get; set; }
        public virtual FutureProg OnImprisonProg { get; set; }
        public virtual FutureProg OnHoldProg { get; set; }
        public virtual FutureProg BailCalculationProg { get; set; }
        public virtual BankAccount BankAccount { get; set; }
    }
}
