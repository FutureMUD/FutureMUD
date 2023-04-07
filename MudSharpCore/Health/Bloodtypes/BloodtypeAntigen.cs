using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health.Bloodtypes;

public class BloodtypeAntigen : FrameworkItem, IBloodtypeAntigen
{
	public BloodtypeAntigen(MudSharp.Models.BloodtypeAntigen dbitem, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
	}

	#region Overrides of Item

	public override string FrameworkItemType => "BloodtypeAntigen";

	#endregion
}