using System;

namespace MudSharp.Models;

public class MagicPortalLink
{
	public long Id { get; set; }
	public long MagicPortalNetworkId { get; set; }
	public long SourceEndpointId { get; set; }
	public long DestinationEndpointId { get; set; }
	public bool IsActive { get; set; }
	public long? CreatedByCharacterId { get; set; }
	public long? CreatedBySpellId { get; set; }
	public DateTime CreatedDateTime { get; set; }

	public virtual MagicPortalNetwork MagicPortalNetwork { get; set; }
	public virtual MagicPortalEndpoint SourceEndpoint { get; set; }
	public virtual MagicPortalEndpoint DestinationEndpoint { get; set; }
	public virtual Character CreatedByCharacter { get; set; }
	public virtual MagicSpell CreatedBySpell { get; set; }
}
