using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodyDrugDose
    {
        public long Id { get; set; }
        public long BodyId { get; set; }
        public long DrugId { get; set; }
        public double Grams { get; set; }
        public int OriginalVector { get; set; }
        public bool Active { get; set; }
        public Guid? SpellOriginId { get; set; }
        public string SpellOriginFamily { get; set; }
        public string SpellOriginFormula { get; set; }

        public virtual Body Body { get; set; }
        public virtual Drug Drug { get; set; }
    }
}
