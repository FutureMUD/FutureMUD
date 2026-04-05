using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Character.Heritage
{
    public class EdibleMaterial
    {
        public IMaterial Material { get; init; }
        public double HungerPerKilogram { get; set; }
        public double WaterPerKilogram { get; set; }
        public double ThirstPerKilogram { get; set; }
        public double AlcoholPerKilogram { get; init; }
    }
}
