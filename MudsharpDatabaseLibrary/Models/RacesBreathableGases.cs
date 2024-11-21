using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class RacesBreathableGases
	{
		public long RaceId { get; set; }
		public long GasId { get; set; }
		public double Multiplier { get; set; }

		public virtual Gas Gas { get; set; }
		public virtual Race Race { get; set; }
	}
}
