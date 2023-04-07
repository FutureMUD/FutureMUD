using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Clan
    {
        public Clan()
        {
            Appointments = new HashSet<Appointment>();
            ChargenRolesClanMemberships = new HashSet<ChargenRolesClanMemberships>();
            ClanMemberships = new HashSet<ClanMembership>();
            ClansAdministrationCells = new HashSet<ClanAdministrationCell>();
            ClansTreasuryCells = new HashSet<ClanTreasuryCell>();
            ExternalClanControlsLiegeClan = new HashSet<ExternalClanControl>();
            ExternalClanControlsVassalClan = new HashSet<ExternalClanControl>();
            InverseParentClan = new HashSet<Clan>();
            Paygrades = new HashSet<Paygrade>();
            Ranks = new HashSet<Rank>();
            EconomicZones = new HashSet<EconomicZone>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public int PayIntervalType { get; set; }
        public int PayIntervalModifier { get; set; }
        public int PayIntervalOther { get; set; }
        public long CalendarId { get; set; }
        public string PayIntervalReferenceDate { get; set; }
        public string PayIntervalReferenceTime { get; set; }
        public bool IsTemplate { get; set; }
        public bool ShowClanMembersInWho { get; set; }
        public long? PaymasterId { get; set; }
        public long? PaymasterItemProtoId { get; set; }
        public long? OnPayProgId { get; set; }
        public int? MaximumPeriodsOfUncollectedBackPay { get; set; }
        public string Sphere { get; set; }
        public bool ShowFamousMembersInNotables { get; set; }
        public long? BankAccountId { get; set; }
        public ulong? DiscordChannelId { get; set; }

        public virtual Calendar Calendar { get; set; }
        public virtual FutureProg OnPayProg { get; set; }
        public virtual Character Paymaster { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<ChargenRolesClanMemberships> ChargenRolesClanMemberships { get; set; }
        public virtual ICollection<ClanMembership> ClanMemberships { get; set; }
        public virtual ICollection<ClanAdministrationCell> ClansAdministrationCells { get; set; }
        public virtual ICollection<ClanTreasuryCell> ClansTreasuryCells { get; set; }
        public virtual ICollection<ExternalClanControl> ExternalClanControlsLiegeClan { get; set; }
        public virtual ICollection<ExternalClanControl> ExternalClanControlsVassalClan { get; set; }
        public virtual ICollection<Clan> InverseParentClan { get; set; }
        public virtual ICollection<Paygrade> Paygrades { get; set; }
        public virtual ICollection<Rank> Ranks { get; set; }
        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
    }
}
