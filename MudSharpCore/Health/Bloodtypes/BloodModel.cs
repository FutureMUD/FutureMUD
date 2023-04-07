using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health.Bloodtypes;

public class BloodModel : FrameworkItem, IBloodModel
{
	public BloodModel(MudSharp.Models.BloodModel model, IFuturemud gameworld)
	{
		_id = model.Id;
		_name = model.Name;
		_bloodtypes.AddRange(
			gameworld.Bloodtypes.Where(x => model.BloodModelsBloodtypes.Any(y => y.BloodtypeId == x.Id)));
	}

	#region Overrides of Item

	public override string FrameworkItemType => "BloodModel";

	#endregion

	#region Implementation of IBloodModel

	private readonly List<IBloodtype> _bloodtypes = new();
	public IEnumerable<IBloodtype> Bloodtypes => _bloodtypes;
	public IEnumerable<IBloodtypeAntigen> Antigens => _bloodtypes.SelectMany(x => x.Antigens).Distinct();

	#endregion
}