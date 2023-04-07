using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Foragable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ForagableTypes { get; set; }
        public int ForageDifficulty { get; set; }
        public int RelativeChance { get; set; }
        public int MinimumOutcome { get; set; }
        public int MaximumOutcome { get; set; }
        public string QuantityDiceExpression { get; set; }
        public long ItemProtoId { get; set; }
        public long? OnForageProgId { get; set; }
        public long? CanForageProgId { get; set; }
        public long EditableItemId { get; set; }
        public int RevisionNumber { get; set; }

        public virtual EditableItem EditableItem { get; set; }
    }
}
