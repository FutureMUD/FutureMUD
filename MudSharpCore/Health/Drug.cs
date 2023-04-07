using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Health;

public class Drug : SaveableItem, IDrug
{
	public Drug(MudSharp.Models.Drug drug, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = drug.Id;
		_name = drug.Name;
		DrugVectors = (DrugVector)drug.DrugVectors;
		IntensityPerGram = drug.IntensityPerGram;
		RelativeMetabolisationRate = drug.RelativeMetabolisationRate;
		foreach (var item in drug.DrugsIntensities)
		{
			DrugTypeMulipliers[(DrugType)item.DrugType] = (item.RelativeIntensity, item.AdditionalEffects);
		}
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Drug";

	#endregion

	#region Overrides of SaveableItem

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.Drugs.Find(Id);
		dbitem.Name = Name;
		dbitem.DrugVectors = (int)DrugVectors;
		dbitem.IntensityPerGram = IntensityPerGram;
		dbitem.RelativeMetabolisationRate = RelativeMetabolisationRate;
		FMDB.Context.DrugsIntensities.RemoveRange(dbitem.DrugsIntensities);
		foreach (var item in DrugTypeMulipliers)
		{
			var dbmult = new Models.DrugIntensity();
			dbitem.DrugsIntensities.Add(dbmult);
			dbmult.DrugType = (int)item.Key;
			dbmult.RelativeIntensity = item.Value.Multiplier;
			dbmult.AdditionalEffects = item.Value.ExtraInfo;
		}
	}

	#endregion

	#region Implementation of IDrug

	public IEnumerable<DrugType> DrugTypes => DrugTypeMulipliers.Select(x => x.Key).AsEnumerable();
	public DrugVector DrugVectors { get; set; }
	public double IntensityPerGram { get; set; }
	public double RelativeMetabolisationRate { get; set; }
	public Dictionary<DrugType, (double Multiplier, string ExtraInfo)> DrugTypeMulipliers { get; } = new();

	public string ExtraInfoFor(DrugType type)
	{
		return DrugTypeMulipliers.ValueOrDefault(type, default).ExtraInfo;
	}

	public virtual string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Drug #{Id:N0} - {Name}");
		sb.AppendLine($"Intensity per Gram: {IntensityPerGram}");
		sb.AppendLine($"Relative Metabolisation Rate: {RelativeMetabolisationRate}");
		sb.AppendLine($"Vectors: {DrugVectors.Describe()}");
		sb.AppendLine(
			$"Effects: \n{DrugTypes.Select(y => DescribeEffect(y, voyeur)).ListToLines(true)}");

		return sb.ToString();
	}

	public string DescribeEffect(DrugType type, IPerceiver voyeur)
	{
		switch (type)
		{
			case DrugType.NeutraliseDrugEffect:
				var types = ExtraInfoFor(type)
				            .Split(' ')
				            .Select(x => (DrugType)int.Parse(x))
				            .Select(x => x.DescribeEnum().ColourValue())
				            .ToList();
				return $"Neutralising {types.ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.BodypartDamage:
				return
					$"Damaging {ExtraInfoFor(type).Split(' ').Select(x => (BodypartTypeEnum)int.Parse(x)).Select(x => x.DescribeEnum().Pluralise().ColourValue()).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.HealingRate:
				var split = ExtraInfoFor(DrugType.HealingRate).Split(' ');
				return
					$"HealingRate Mult ({double.Parse(split[0]).ToString("N4", voyeur)}) Diff ({double.Parse(split[1]).ToString("N4", voyeur)}) @ {IntensityForType(type).ToString("N4", voyeur)}";
			case DrugType.MagicAbility:
				var capabilityString = ExtraInfoFor(DrugType.MagicAbility);
				var capabilities = capabilityString.Split(' ')
				                                   .SelectNotNull(x => Gameworld.MagicCapabilities.Get(int.Parse(x)))
				                                   .ToList();
				return
					$"MagicAbility of {capabilities.Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
		}

		return $"{type.DescribeEnum()} @ {IntensityForType(type).ToString("N4", voyeur)}";
	}

	public double IntensityForType(DrugType type)
	{
		return DrugTypeMulipliers.ValueOrDefault(type, default).Multiplier * IntensityPerGram;
	}

	#endregion

	#region IFutureProgVariable Implementation

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Drug;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "intensitypergram":
				return new NumberVariable(IntensityPerGram);
			case "metabolisationrate":
			case "metabolizationrate":
				return new NumberVariable(RelativeMetabolisationRate);
			case "vectors":
				return new CollectionVariable(
					DrugVectors.GetFlags().OfType<DrugVector>().Select(x => new TextVariable(x.Describe())).ToList(),
					FutureProgVariableTypes.Text);
			case "types":
				return new CollectionVariable(DrugTypes.Select(x => new TextVariable(x.DescribeEnum())).ToList(),
					FutureProgVariableTypes.Text);
			case "itensities":
				return new CollectionVariable(DrugTypes.Select(x => new NumberVariable(IntensityForType(x))).ToList(),
					FutureProgVariableTypes.Number);
		}

		throw new NotImplementedException();
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "intensitypergram", FutureProgVariableTypes.Number },
			{ "metabolisationrate", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "metabolizationrate", FutureProgVariableTypes.Number },
			{ "vectors", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "types", FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection },
			{ "intensities", FutureProgVariableTypes.Number | FutureProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The unique database id of the drug" },
			{ "name", "The name of the drug" },
			{
				"intensitypergram",
				"The hard-coded intensity of the drug per gram ingested. Has different effects per drug type"
			},
			{ "metabolisationrate", "The relative rate at which the drug is metabolised (removed) by the body" },
			{ "metabolizationrate", "An alias for the metabolisationrate property" },
			{ "vectors", "The potential vectors for the drug affecting someone" },
			{ "types", "The drug effect types contained in this drug" },
			{ "intensities", "The intensities for each drug type, ordered the same way as the 'types' property" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Drug, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}