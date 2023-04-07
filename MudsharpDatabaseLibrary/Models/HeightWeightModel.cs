using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class HeightWeightModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double MeanHeight { get; set; }
        public double MeanBmi { get; set; }
        public double StddevHeight { get; set; }
        public double StddevBmi { get; set; }
        public double Bmimultiplier { get; set; }
    }
}
