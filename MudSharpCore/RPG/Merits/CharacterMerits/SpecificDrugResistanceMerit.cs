using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class SpecificDrugResistanceMerit : CharacterMeritBase, ISpecificDrugResistanceMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Drug Resistance",
			(merit, gameworld) => new SpecificDrugResistanceMerit(merit, gameworld));
	}

	protected SpecificDrugResistanceMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var item in definition.Element("Resistances").Elements("Resistance"))
		{
			_drugMultipliers[long.Parse(item.Attribute("drug").Value)] = double.Parse(item.Attribute("multiplier").Value);
		}
	}

	private readonly DictionaryWithDefault<long, double> _drugMultipliers = new();

	public double MultiplierForDrug(IDrug drug)
	{
		if (_drugMultipliers.ContainsKey(drug.Id))
		{
			return _drugMultipliers[drug.Id];
		}

		return 1.0;
	}
}
