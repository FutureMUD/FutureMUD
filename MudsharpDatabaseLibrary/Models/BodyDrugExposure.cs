using System;

namespace MudSharp.Models
{
	public partial class BodyDrugExposure
	{
		public long BodyId { get; set; }
		public long DrugId { get; set; }
		public double Exposure { get; set; }
		public double PeakExposure { get; set; }
		public double WithdrawalIntensity { get; set; }
		public DateTime LastUpdatedAtUtc { get; set; }

		public virtual Body Body { get; set; }
		public virtual Drug Drug { get; set; }
	}
}
