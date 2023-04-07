using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health.Bloodtypes;

public class Bloodtype : FrameworkItem, IBloodtype
{
	public Bloodtype(MudSharp.Models.Bloodtype dbitem, IFuturemud gameworld)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		_antigens.AddRange(gameworld.BloodtypeAntigens.Where(x =>
			dbitem.BloodtypesBloodtypeAntigens.Any(y => y.BloodtypeAntigenId == x.Id)));
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Bloodtype";

	#endregion

	#region Implementation of IBloodtype

	private readonly List<IBloodtypeAntigen> _antigens = new();
	public IEnumerable<IBloodtypeAntigen> Antigens => _antigens;

	public bool IsCompatibleWithDonorBlood(IBloodtype donorBloodtype)
	{
		return donorBloodtype.Antigens.All(x => Antigens.Contains(x));
	}

	#endregion
}