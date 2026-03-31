using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MaterialAlias
    {
        public long MaterialId { get; set; }
        public string Alias { get; set; }

        public virtual Material Material { get; set; }
    }
}
