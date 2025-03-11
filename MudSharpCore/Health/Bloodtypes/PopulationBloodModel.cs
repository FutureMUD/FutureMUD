using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health.Bloodtypes;

public class PopulationBloodModel : FrameworkItem, IPopulationBloodModel
{
	public PopulationBloodModel(MudSharp.Models.PopulationBloodModel model, IFuturemud gameworld)
	{
		_id = model.Id;
		_name = model.Name;
		foreach (var type in model.PopulationBloodModelsBloodtypes)
		{
			_bloodTypes.Add((gameworld.Bloodtypes.Get(type.BloodtypeId), type.Weight));
		}

		BloodModel = gameworld.BloodModels.First(x => x.Bloodtypes.Any(y => _bloodTypes.Any(z => z.Bloodtype == y)));
	}

	private readonly List<(IBloodtype Bloodtype, double Weight)> _bloodTypes = new();

	public IEnumerable<(IBloodtype Bloodtype, double Weight)> BloodTypes => _bloodTypes;

	public IBloodModel BloodModel { get; }

	public IBloodtype GetBloodType(ICharacterTemplate character)
	{
		if (character?.SelectedMerits.OfType<IFixedBloodTypeMerit>().Any() == true)
		{
			var bloodtype = character.SelectedMerits.OfType<IFixedBloodTypeMerit>().First().Bloodtype;
			if (_bloodTypes.Any(x => x.Bloodtype == bloodtype))
			{
				return bloodtype;
			}
		}

		return _bloodTypes.GetWeightedRandom(x => x.Weight).Bloodtype;
	}

	public override string FrameworkItemType => "PopulationBloodModel";
}