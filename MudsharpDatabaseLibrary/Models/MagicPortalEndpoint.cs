using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class MagicPortalEndpoint
{
	public MagicPortalEndpoint()
	{
		MagicPortalLinksAsDestination = new HashSet<MagicPortalLink>();
		MagicPortalLinksAsSource = new HashSet<MagicPortalLink>();
	}

	public long Id { get; set; }
	public long MagicPortalNetworkId { get; set; }
	public string Key { get; set; }
	public string Name { get; set; }
	public int AnchorType { get; set; }
	public long? CellId { get; set; }
	public long? GameItemId { get; set; }
	public bool IsActive { get; set; }
	public long? CreatedByCharacterId { get; set; }
	public long? CreatedBySpellId { get; set; }
	public DateTime CreatedDateTime { get; set; }

	public virtual MagicPortalNetwork MagicPortalNetwork { get; set; }
	public virtual Cell Cell { get; set; }
	public virtual GameItem GameItem { get; set; }
	public virtual Character CreatedByCharacter { get; set; }
	public virtual MagicSpell CreatedBySpell { get; set; }
	public virtual ICollection<MagicPortalLink> MagicPortalLinksAsDestination { get; set; }
	public virtual ICollection<MagicPortalLink> MagicPortalLinksAsSource { get; set; }
}
