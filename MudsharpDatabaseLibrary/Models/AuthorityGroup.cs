using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AuthorityGroup
    {
        public AuthorityGroup()
        {
            Accounts = new HashSet<Account>();
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public int AuthorityLevel { get; set; }
        public int InformationLevel { get; set; }
        public int AccountsLevel { get; set; }
        public int CharactersLevel { get; set; }
        public int CharacterApprovalLevel { get; set; }
        public int CharacterApprovalRisk { get; set; }
        public int ItemsLevel { get; set; }
        public int PlanesLevel { get; set; }
        public int RoomsLevel { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
