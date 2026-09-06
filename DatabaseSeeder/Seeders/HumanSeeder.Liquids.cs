#nullable enable

using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	internal static (Liquid Blood, Liquid Sweat) SeedBloodAndSweat(FuturemudDatabaseContext context)
	{
		var water = context.Liquids.FirstOrDefault(x => x.Name == "water")
			?? throw new InvalidOperationException("The Human Seeder requires the stock water liquid. Run the Core Data Seeder first.");

		Material driedBlood = new()
		{
			Name = "Dried Human Blood",
			MaterialDescription = "dried blood",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = water.Id,
			SolventVolumeRatio = 4,
			ResidueDesc = "It is covered in {0}dried blood",
			ResidueColour = "red",
			Absorbency = 0
		};
		context.Materials.Add(driedBlood);
		Liquid blood = new()
		{
			Name = "Human Blood",
			Description = "blood",
			LongDescription = "a virtually opaque dark red fluid",
			TasteText = "It has a sharply metallic, umami taste",
			VagueTasteText = "It has a metallic taste",
			SmellText = "It has a metallic, coppery smell",
			VagueSmellText = "It has a faintly metallic smell",
			TasteIntensity = 200,
			SmellIntensity = 10,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.8,
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
			DisplayColour = "bold red",
			DampDescription = "It is damp with blood",
			WetDescription = "It is wet with blood",
			DrenchedDescription = "It is drenched with blood",
			DampShortDescription = "(blood damp)",
			WetShortDescription = "(bloody)",
			DrenchedShortDescription = "(blood drenched)",
			SolventId = water.Id,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedBlood,
			CountAsQuality = (int)ItemQuality.Legendary,
			CountAs = context.Liquids.First(x => x.Name == "Blood")
		};
		context.Liquids.Add(blood);

		Material driedSweat = new()
		{
			Name = "Dried Human Sweat",
			MaterialDescription = "dried sweat",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = water.Id,
			SolventVolumeRatio = 3,
			ResidueDesc = "It is covered in {0}dried sweat",
			ResidueColour = "yellow",
			Absorbency = 0
		};
		context.Materials.Add(driedSweat);
		Liquid sweat = new()
		{
			Name = "Human Sweat",
			Description = "sweat",
			LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
			TasteText = "It tastes like a pungent, salty lick of someone's underarms",
			VagueTasteText = "It tastes very unpleasant, like underarm stench",
			SmellText = "It has the sharp, pungent smell of body odor",
			VagueSmellText = "It has the sharp, pungent smell of body odor",
			TasteIntensity = 200,
			SmellIntensity = 200,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.95,
			DrinkSatiatedHoursPerLitre = 5,
			FoodSatiatedHoursPerLitre = 0,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "yellow",
			DampDescription = "It is damp with sweat",
			WetDescription = "It is wet and smelly with sweat",
			DrenchedDescription = "It is soaking wet and smelly with sweat",
			DampShortDescription = "(sweat-damp)",
			WetShortDescription = "(sweaty)",
			DrenchedShortDescription = "(sweat-drenched)",
			SolventId = water.Id,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedSweat,
			CountAsQuality = (int)ItemQuality.Legendary,
			CountAs = context.Liquids.First(x => x.Name == "Sweat")
		};
		context.Liquids.Add(sweat);
		return (blood, sweat);
	}
}
