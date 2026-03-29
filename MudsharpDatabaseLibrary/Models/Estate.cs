using System.Collections.Generic;

namespace MudSharp.Models;

public partial class Estate
{
	public Estate()
	{
		EstateAssets = new HashSet<EstateAsset>();
		EstateClaims = new HashSet<EstateClaim>();
		EstatePayouts = new HashSet<EstatePayout>();
	}

	public long Id { get; set; }
	public long EconomicZoneId { get; set; }
	public long CharacterId { get; set; }
	public int EstateStatus { get; set; }
	public string EstateStartTime { get; set; }
	public string FinalisationDate { get; set; }
	public long? InheritorId { get; set; }
	public string InheritorType { get; set; }

	public virtual Character Character { get; set; }
	public virtual EconomicZone EconomicZone { get; set; }
	public virtual ICollection<EstateAsset> EstateAssets { get; set; }
	public virtual ICollection<EstateClaim> EstateClaims { get; set; }
	public virtual ICollection<EstatePayout> EstatePayouts { get; set; }
}
