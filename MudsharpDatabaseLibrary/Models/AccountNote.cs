using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AccountNote
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public string InGameTimeStamp { get; set;}
        public DateTime TimeStamp { get; set; }
        public long? AuthorId { get; set; }
        public bool IsJournalEntry { get; set; }
        public long? CharacterId {get;set;}

        public virtual Account Account { get; set; }
        public virtual Account Author { get; set; }
        public virtual Character Character { get; set; }
    }
}
