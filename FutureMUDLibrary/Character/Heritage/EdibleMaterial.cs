using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Form.Material;

namespace MudSharp.Character.Heritage
{
	public class EdibleMaterial
	{
		public IMaterial Material { get; init; }
		public double CaloriesPerKilogram { get; set; }
		public double HungerPerKilogram { get; set; }
		public double WaterPerKilogram { get; set; }
		public double ThirstPerKilogram { get; set; }
		public double AlcoholPerKilogram { get; init; }
	}
}
