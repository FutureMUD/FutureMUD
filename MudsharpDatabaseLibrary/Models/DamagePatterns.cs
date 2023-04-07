using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DamagePatterns
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public int DamageType { get; set; }
        public int Dice { get; set; }
        public int Sides { get; set; }
        public int Bonus { get; set; }
    }
}
