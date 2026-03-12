#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static bool TryGetRaceTemplate(string raceName, out AnimalRaceTemplate template)
	{
		return RaceTemplates.TryGetValue(raceName, out template!);
	}

	private bool TryApplyTemplateRaceAttacks(Race race)
	{
		if (!TryGetRaceTemplate(race.Name, out var template))
		{
			return false;
		}

		var loadout = AttackLoadouts[template.AttackLoadoutKey];
		foreach (var attack in loadout.ShapeMatchedAttacks)
		{
			AddAttackToRace(attack.AttackKey, race, attack.Quality);
		}

		if (loadout.AliasAttacks is not null)
		{
			foreach (var attack in loadout.AliasAttacks)
			{
				AddAttackToRaceAliases(attack.AttackKey, race, attack.Quality, attack.BodypartAliases.ToArray());
			}
		}

		if (loadout.VenomAttacks is not null)
		{
			foreach (var venomAttack in loadout.VenomAttacks)
			{
				AddVenomAttackToRace(race, venomAttack);
			}
		}

		return true;
	}

	private void AddAttackToRaceAliases(string whichAttack, Race race, ItemQuality quality, params string[] aliases)
	{
		var bodies = new List<long> { race.BaseBodyId };
		var body = race.BaseBody.CountsAs;
		while (body != null)
		{
			bodies.Add(body.Id);
			body = body.CountsAs;
		}

		var attack = _attacks[whichAttack];
		foreach (var bodypart in _context.BodypartProtos.Where(x =>
					 bodies.Contains(x.BodyId) && aliases.Contains(x.Name)))
		{
			_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{
				Bodypart = bodypart,
				Race = race,
				WeaponAttack = attack,
				Quality = (int)quality
			});
		}
	}

	private void AddVenomAttackToRace(Race race, AnimalVenomAttackTemplate template)
	{
		var attackAddendum = _questionAnswers["messagestyle"].ToLowerInvariant() switch
		{
			"sentences" or "sparse" => ".",
			_ => ""
		};

		var venomProfile = VenomProfiles[template.VenomProfileKey];
		var drug = new Drug
		{
			Name = $"{race.Name} Venom",
			IntensityPerGram = venomProfile.IntensityPerGram,
			DrugVectors = (int)venomProfile.DrugVectors,
			RelativeMetabolisationRate = venomProfile.RelativeMetabolisationRate
		};
		_context.Drugs.Add(drug);
		_context.SaveChanges();

		foreach (var effect in venomProfile.Effects)
		{
			_context.DrugsIntensities.Add(new DrugIntensity
			{
				DrugId = drug.Id,
				DrugType = (int)effect.DrugType,
				RelativeIntensity = effect.RelativeIntensity,
				AdditionalEffects = effect.AdditionalEffects
			});
		}

		var liquid = new Liquid
		{
			Name = $"{race.Name} Venom".ToLowerInvariant(),
			Description = venomProfile.Description,
			LongDescription = venomProfile.LongDescription,
			TasteText = venomProfile.TasteText,
			VagueTasteText = venomProfile.VagueTasteText,
			SmellText = venomProfile.SmellText,
			VagueSmellText = venomProfile.VagueSmellText,
			TasteIntensity = 2000,
			SmellIntensity = 2000,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.5,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = venomProfile.DisplayColour,
			DampDescription = "It is damp",
			WetDescription = "It is wet",
			DrenchedDescription = "It is drenched",
			DampShortDescription = "(damp)",
			WetShortDescription = "(wet)",
			DrenchedShortDescription = "(drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)venomProfile.InjectionConsequence,
			ResidueVolumePercentage = 0.05,
			Drug = drug,
			DrugGramsPerUnitVolume = 1000
		};
		_context.Liquids.Add(liquid);
		_context.SaveChanges();

		var attackShape = _context.BodypartShapes.First(x => x.Name == template.AttackShapeName);
		var attack = AddAttack(
			$"{race.Name} {template.AttackNameSuffix}",
			template.MoveType,
			template.Verb,
			Difficulty.Normal,
			Difficulty.Hard,
			Difficulty.Hard,
			Difficulty.Hard,
			template.Alignment,
			template.Orientation,
			template.StaminaCost,
			template.BaseDelay,
			attackShape,
			_snakeBiteDamage,
			$"{template.CombatMessage}{attackAddendum}",
			template.DamageType,
			additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>{template.MaximumQuantity:R}</MaximumQuantity>
   <MinimumWoundSeverity>{template.MinimumWoundSeverity}</MinimumWoundSeverity>
 </Data>"
		);

		var bodyparts = template.TargetBodypartAliases.Any()
			? _context.BodypartProtos.Where(x => x.BodyId == race.BaseBodyId && template.TargetBodypartAliases.Contains(x.Name)).ToList()
			: _context.BodypartProtos.Where(x => x.BodyId == race.BaseBodyId && x.BodypartShapeId == attackShape.Id).ToList();

		foreach (var bodypart in bodyparts)
		{
			_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{
				Bodypart = bodypart,
				Race = race,
				WeaponAttack = attack,
				Quality = (int)ItemQuality.Standard
			});
		}

		_context.SaveChanges();
	}

	private void CreateDescriptionsFromPack(
		Race race,
		AnimalDescriptionPack pack,
		FutureProg isAdultMaleRaceProg,
		FutureProg isAdultFemaleRaceProg,
		FutureProg isJuvenileMaleRaceProg,
		FutureProg isJuvenileFemaleRaceProg,
		FutureProg isBabyMaleRaceProg,
		FutureProg isBabyFemaleRaceProg)
	{
		void AddVariants(IEnumerable<AnimalDescriptionVariant> variants, FutureProg prog)
		{
			foreach (var variant in variants)
			{
				CreateDescription(EntityDescriptionType.ShortDescription, variant.ShortDescription, prog);
				CreateDescription(EntityDescriptionType.FullDescription, variant.FullDescription, prog);
			}
		}

		if (pack.UseCatCoatDescriptions)
		{
			CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten", isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten", isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat", isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat", isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] tomcat", isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] cat", isAdultFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This animal is a young &male common domestic cat. $catcoatfancy", isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This animal is a young &male common domestic cat. $catcoatfancy", isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				"This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultFemaleRaceProg);
			return;
		}

		AddVariants(pack.BabyMale, isBabyMaleRaceProg);
		AddVariants(pack.BabyFemale, isBabyFemaleRaceProg);
		AddVariants(pack.JuvenileMale, isJuvenileMaleRaceProg);
		AddVariants(pack.JuvenileFemale, isJuvenileFemaleRaceProg);
		AddVariants(pack.AdultMale, isAdultMaleRaceProg);
		AddVariants(pack.AdultFemale, isAdultFemaleRaceProg);

		if (pack.AddDogBreedDescriptions)
		{
			DogLongDescriptions(race);
		}
	}

	private (BloodModel BloodModel, PopulationBloodModel PopulationBloodModel)? CreateBloodProfile(string bloodProfileKey)
	{
		return bloodProfileKey.ToLowerInvariant() switch
		{
			"equine" => SetupBloodModel("Equine", ["Equine A-Antigen", "Equine C-Antigen", "Equine Q-Antigen"],
				[
					("Equine ACQ", ["Equine A-Antigen", "Equine C-Antigen", "Equine Q-Antigen"], 10.0),
					("Equine AC", ["Equine A-Antigen", "Equine C-Antigen"], 10.0),
					("Equine A", ["Equine A-Antigen"], 10.0),
					("Equine CQ", ["Equine C-Antigen", "Equine Q-Antigen"], 10.0),
					("Equine Q", ["Equine Q-Antigen"], 10.0),
					("Equine C", ["Equine C-Antigen"], 10.0)
				]),
			"feline" => SetupBloodModel("Feline", ["Feline A-Antigen", "Feline B-Antigen"],
				[
					("Feline A", ["Feline A-Antigen"], 87.0),
					("Feline B", ["Feline B-Antigen"], 10.0),
					("Feline AB", ["Feline A-Antigen", "Feline B-Antigen"], 3.0)
				]),
			"canine" => SetupBloodModel("Canine", ["DEA 1.1"],
				[
					("Canine DEA 1.1 Positive", ["DEA 1.1"], 40.0),
					("Canine DEA 1.1 Negative", [], 60.0)
				]),
			"bovine" => SetupBloodModel("Bovine", ["Bovine B", "Bovine J"],
				[
					("Bovine BJ", ["Bovine B", "Bovine J"], 20.0),
					("Bovine B", ["Bovine B"], 40.0),
					("Bovine J", ["Bovine J"], 40.0)
				]),
			"ovine" => SetupBloodModel("Ovine", ["Ovine B", "Ovine R"],
				[
					("Ovine BR", ["Ovine B", "Ovine R"], 20.0),
					("Ovine B", ["Ovine B"], 40.0),
					("Ovine R", ["Ovine R"], 40.0)
				]),
			_ => null
		};
	}
}
