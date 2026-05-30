using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class MagicPortalNetwork
{
	public MagicPortalNetwork()
	{
		MagicPortalEndpoints = new HashSet<MagicPortalEndpoint>();
		MagicPortalLinks = new HashSet<MagicPortalLink>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public long? MagicSchoolId { get; set; }
	public bool IsActive { get; set; }
	public bool AllowCrossZone { get; set; }
	public string Verb { get; set; }
	public string OutboundKeyword { get; set; }
	public string InboundKeyword { get; set; }
	public string OutboundTarget { get; set; }
	public string InboundTarget { get; set; }
	public string OutboundDescription { get; set; }
	public string InboundDescription { get; set; }
	public double TimeMultiplier { get; set; }
	public long? CreatedByCharacterId { get; set; }
	public long? CreatedBySpellId { get; set; }
	public DateTime CreatedDateTime { get; set; }

	public virtual MagicSchool MagicSchool { get; set; }
	public virtual Character CreatedByCharacter { get; set; }
	public virtual MagicSpell CreatedBySpell { get; set; }
	public virtual ICollection<MagicPortalEndpoint> MagicPortalEndpoints { get; set; }
	public virtual ICollection<MagicPortalLink> MagicPortalLinks { get; set; }
}
