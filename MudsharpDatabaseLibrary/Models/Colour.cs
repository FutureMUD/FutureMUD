using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Colour
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Basic { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public string Fancy { get; set; }
    }
}
