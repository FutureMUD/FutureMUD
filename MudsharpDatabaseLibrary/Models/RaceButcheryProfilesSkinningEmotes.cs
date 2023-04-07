using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RaceButcheryProfilesSkinningEmotes
    {
        public long RaceButcheryProfileId { get; set; }
        public string Subcategory { get; set; }
        public string Emote { get; set; }
        public double Delay { get; set; }
        public int Order { get; set; }

        public virtual RaceButcheryProfile RaceButcheryProfile { get; set; }
    }
}
