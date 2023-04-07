using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Inventory;

public class WearlocProfile : IWearlocProfile
{
	public WearlocProfile(XElement root)
	{
		Transparent = Convert.ToBoolean(root.Attribute("Transparent").Value);
		NoArmour = Convert.ToBoolean(root.Attribute("NoArmour").Value);
		PreventsRemoval = Convert.ToBoolean(root.Attribute("PreventsRemoval").Value);
		Mandatory = Convert.ToBoolean(root.Attribute("Mandatory").Value);
		HidesSeveredBodyparts = Convert.ToBoolean(root.Attribute("HidesSevered")?.Value ?? "false");
	}

	public WearlocProfile(bool preventsremoval = true, bool transparent = false, bool noarmour = false,
		bool mandatory = false, bool hidessevered = false)
	{
		Transparent = transparent;
		PreventsRemoval = preventsremoval;
		NoArmour = noarmour;
		Mandatory = mandatory;
		HidesSeveredBodyparts = hidessevered;
	}

	public bool Transparent { get; set; }

	public bool NoArmour { get; set; }

	public bool PreventsRemoval { get; set; }

	public bool Mandatory { get; set; }

	public bool HidesSeveredBodyparts { get; set; }
}