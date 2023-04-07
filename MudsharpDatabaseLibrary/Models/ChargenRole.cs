using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRole
    {
        public ChargenRole()
        {
            CharactersChargenRoles = new HashSet<CharactersChargenRoles>();
            ChargenAdvicesChargenRoles = new HashSet<ChargenAdvicesChargenRoles>();
            ChargenRolesApprovers = new HashSet<ChargenRolesApprovers>();
            ChargenRolesClanMemberships = new HashSet<ChargenRolesClanMemberships>();
            ChargenRolesCosts = new HashSet<ChargenRolesCost>();
            ChargenRolesCurrencies = new HashSet<ChargenRolesCurrency>();
            ChargenRolesMerits = new HashSet<ChargenRolesMerit>();
            ChargenRolesTraits = new HashSet<ChargenRolesTrait>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public long PosterId { get; set; }
        public int MaximumNumberAlive { get; set; }
        public int MaximumNumberTotal { get; set; }
        public string ChargenBlurb { get; set; }
        public long? AvailabilityProgId { get; set; }
        public bool Expired { get; set; }
        public int MinimumAuthorityToApprove { get; set; }
        public int MinimumAuthorityToView { get; set; }

        public virtual FutureProg AvailabilityProg { get; set; }
        public virtual Account Poster { get; set; }
        public virtual ICollection<CharactersChargenRoles> CharactersChargenRoles { get; set; }
        public virtual ICollection<ChargenAdvicesChargenRoles> ChargenAdvicesChargenRoles { get; set; }
        public virtual ICollection<ChargenRolesApprovers> ChargenRolesApprovers { get; set; }
        public virtual ICollection<ChargenRolesClanMemberships> ChargenRolesClanMemberships { get; set; }
        public virtual ICollection<ChargenRolesCost> ChargenRolesCosts { get; set; }
        public virtual ICollection<ChargenRolesCurrency> ChargenRolesCurrencies { get; set; }
        public virtual ICollection<ChargenRolesMerit> ChargenRolesMerits { get; set; }
        public virtual ICollection<ChargenRolesTrait> ChargenRolesTraits { get; set; }
    }
}
