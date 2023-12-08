using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class DrugEffectResistanceMerit : CharacterMeritBase, IDrugEffectResistanceMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Drug Effect Resistance",
			(merit, gameworld) => new DrugEffectResistanceMerit(merit, gameworld));
	}

	protected DrugEffectResistanceMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var item in definition.Element("Resistances").Elements("Resistance"))
		{
			DrugResistances[(DrugType)int.Parse(item.Attribute("type").Value)] = double.Parse(item.Attribute("value").Value);
		}
	}

	public Dictionary<DrugType, double> DrugResistances { get; } = new();
	IReadOnlyDictionary<DrugType, double> IDrugEffectResistanceMerit.DrugResistances => DrugResistances;
	public double ModifierForDrugType(DrugType drugType)
	{
		if (!DrugResistances.ContainsKey(drugType))
		{
			return 1.0;
		}

		return Math.Max(0.0, 1.0 - DrugResistances[drugType]);
	}
}
