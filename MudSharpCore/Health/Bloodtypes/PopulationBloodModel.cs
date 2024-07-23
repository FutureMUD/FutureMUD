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
			BloodTypes.Add((gameworld.Bloodtypes.Get(type.BloodtypeId), type.Weight));
		}

		BloodModel = gameworld.BloodModels.First(x => x.Bloodtypes.Any(y => BloodTypes.Any(z => z.Bloodtype == y)));
	}

	public List<(IBloodtype Bloodtype, double Weight)> BloodTypes { get; } = new();

	public IBloodModel BloodModel { get; }

	public IBloodtype GetBloodType(ICharacterTemplate character)
	{
		if (character?.SelectedMerits.OfType<IFixedBloodTypeMerit>().Any() == true)
		{
			var bloodtype = character.SelectedMerits.OfType<IFixedBloodTypeMerit>().First().Bloodtype;
			if (BloodTypes.Any(x => x.Bloodtype == bloodtype))
			{
				return bloodtype;
			}
		}

		return BloodTypes.GetWeightedRandom(x => x.Weight).Bloodtype;
	}

	public override string FrameworkItemType => "PopulationBloodModel";
}