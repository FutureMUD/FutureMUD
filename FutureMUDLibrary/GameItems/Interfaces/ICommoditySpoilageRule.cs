#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Revision;

namespace MudSharp.GameItems.Interfaces;

public interface ICommoditySpoilageRule : IEditableItem, ISaveable
{
	string Description { get; set; }
	bool Enabled { get; set; }
	int Priority { get; set; }
	ISolid? Material { get; set; }
	ITag? MaterialTag { get; set; }
	ITag? CommodityTag { get; set; }
	ISolid ResultMaterial { get; set; }
	ITag? ResultCommodityTag { get; set; }
	TimeSpan SecondsUntilSpoiled { get; set; }
	string? SpoilEcho { get; set; }

	int MatchSpecificity(ICommodity commodity);
	bool HasCompatibleResult(ICommoditySpoilageRule? other);
	ICommoditySpoilageRule Clone(string newName);
	IEnumerable<string> ValidationWarnings { get; }
}
