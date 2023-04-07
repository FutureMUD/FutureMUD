using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Drawing
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public int ImplementType { get; set; }
        public double DrawingSkill { get; set; }
        public int DrawingSize { get; set; }

        public virtual Character Author { get; set; }
    }
}
