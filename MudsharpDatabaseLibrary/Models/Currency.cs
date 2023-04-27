using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Currency
    {
        public Currency()
        {
            Characters = new HashSet<Character>();
            ChargenRolesCurrencies = new HashSet<ChargenRolesCurrency>();
            ClanMembershipsBackpay = new HashSet<ClanMembershipBackpay>();
            Coins = new HashSet<Coin>();
            CurrencyDescriptionPatterns = new HashSet<CurrencyDescriptionPattern>();
            CurrencyDivisions = new HashSet<CurrencyDivision>();
            EconomicZones = new HashSet<EconomicZone>();
            LegalAuthorities = new HashSet<LegalAuthority>();
            Paygrades = new HashSet<Paygrade>();
            ShopTransactionRecords = new HashSet<ShopTransactionRecord>();
            Shops = new HashSet<Shop>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public decimal BaseCurrencyToGlobalBaseCurrencyConversion { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<ChargenRolesCurrency> ChargenRolesCurrencies { get; set; }
        public virtual ICollection<ClanMembershipBackpay> ClanMembershipsBackpay { get; set; }
        public virtual ICollection<Coin> Coins { get; set; }
        public virtual ICollection<CurrencyDescriptionPattern> CurrencyDescriptionPatterns { get; set; }
        public virtual ICollection<CurrencyDivision> CurrencyDivisions { get; set; }
        public virtual ICollection<EconomicZone> EconomicZones { get; set; }
        public virtual ICollection<LegalAuthority> LegalAuthorities { get; set; }
        public virtual ICollection<Paygrade> Paygrades { get; set; }
        public virtual ICollection<ShopTransactionRecord> ShopTransactionRecords { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }
    }
}
