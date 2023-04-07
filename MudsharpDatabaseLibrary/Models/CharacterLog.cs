using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacterLog
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long CharacterId { get; set; }
        public long CellId { get; set; }
        public string Command { get; set; }
        public DateTime Time { get; set; }
        public bool IsPlayerCharacter { get; set; }

        public virtual Account Account { get; set; }
        public virtual Cell Cell { get; set; }
        public virtual Character Character { get; set; }
    }
}
