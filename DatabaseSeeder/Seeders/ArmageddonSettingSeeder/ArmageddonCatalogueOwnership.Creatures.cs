#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonCatalogueOwnership
{
	private static IReadOnlyDictionary<Type, string[]> CreatureOwnedFields(IReadOnlyDictionary<Type, string[]> existing)
	{
		var fields = existing.ToDictionary(x => x.Key, x => x.Value);
		fields.Add(typeof(Race), ["Name", "Description", "BaseBodyId", "AllowedGenders", "ParentRaceId", "AttributeTotalCap", "IndividualAttributeCap", "DiceExpression", "IlluminationPerceptionMultiplier", "AvailabilityProgId", "CorpseModelId", "DefaultHealthStrategyId", "DefaultCombatSettingId", "CanUseWeapons", "CanAttack", "CanDefend", "NaturalArmourTypeId", "NaturalArmourQuality", "NaturalArmourMaterialId", "BloodLiquidId", "NeedsToBreathe", "BreathingModel", "SweatLiquidId", "SweatRateInLitresPerMinute", "SizeStanding", "SizeProne", "SizeSitting", "CommunicationStrategyType", "DefaultAlertEmote", "DefaultDistantAlertEmote", "DefaultHandedness", "HandednessOptions", "MaximumDragWeightExpression", "MaximumLiftWeightExpression", "RaceButcheryProfileId", "BloodModelId", "RaceUsesStamina", "CanEatCorpses", "BiteWeight", "EatCorpseEmoteText", "CanEatMaterialsOptIn", "TemperatureRangeFloor", "TemperatureRangeCeiling", "BodypartSizeModifier", "BodypartHealthMultiplier", "PainToleranceMultiplier", "BreathingVolumeExpression", "HoldBreathLengthExpression", "CanClimb", "CanSwim", "MinimumSleepingPosition", "ChildAge", "YouthAge", "YoungAdultAge", "AdultAge", "ElderAge", "VenerableAge", "DefaultHeightWeightModelMaleId", "DefaultHeightWeightModelFemaleId", "DefaultHeightWeightModelNeuterId", "DefaultHeightWeightModelNonBinaryId", "HungerRate", "ThirstRate", "MaximumFoodSatiatedHours", "MaximumDrinkSatiatedHours", "TrackIntensityVisual", "TrackIntensityOlfactory", "TrackingAbilityVisual", "TrackingAbilityOlfactory"]);
		fields.Add(typeof(Culture), ["NativeLanguageId", "Name", "Description", "PersonWordMale", "PersonWordFemale", "PersonWordNeuter", "PersonWordIndeterminate", "PrimaryCalendarId", "SkillStartingValueProgId", "AvailabilityProgId", "TolerableTemperatureFloorEffect", "TolerableTemperatureCeilingEffect"]);
		fields.Add(typeof(Ethnicity), ["NativeLanguageId", "Name", "ChargenBlurb", "AvailabilityProgId", "ParentRaceId", "EthnicGroup", "EthnicSubgroup", "PopulationBloodModelId", "TolerableTemperatureFloorEffect", "TolerableTemperatureCeilingEffect"]);
		fields.Add(typeof(Merit), ["Name", "Type", "MeritType", "MeritScope", "Definition", "ParentId"]);
		fields.Add(typeof(NpcTemplate), ["Type", "Name", "UniqueName", "BuilderNotes", "Definition"]);
		fields.Add(typeof(ArtificialIntelligence), ["Type", "Name", "Definition"]);
		return fields;
	}

	private void LoadCreatureChildren(object row)
	{
		if (row is Culture culture) _db.Entry(culture).Collection(x => x.CulturesNameCultures).Load();
		if (row is Race race)
		{
			_db.Entry(race).Collection(x => x.RacesAttributes).Load();
			_db.Entry(race).Collection(x => x.RacesBreathableGases).Load();
			_db.Entry(race).Collection(x => x.RacesBreathableLiquids).Load();
			_db.Entry(race).Collection(x => x.RacesWeaponAttacks).Load();
		}
		if (row is NpcTemplate template) _db.Entry(template).Collection(x => x.NpctemplatesArtificalIntelligences).Load();
	}

	private static void CreatureFields(object row, Dictionary<string, string> fields)
	{
		if (row is Culture culture) fields["NameCultures"] = JsonSerializer.Serialize(culture.CulturesNameCultures
			.OrderBy(x => x.Gender).ThenBy(x => x.NameCultureId).Select(x => new NameCultureLink(x.Gender, x.NameCultureId)).ToArray());
		if (row is Race race)
		{
			fields["Attributes"] = JsonSerializer.Serialize(race.RacesAttributes.OrderBy(x => x.AttributeId)
				.Select(x => new RaceAttribute(x.AttributeId, x.IsHealthAttribute, x.AttributeBonus, x.DiceExpression)).ToArray());
			fields["BreathableGases"] = JsonSerializer.Serialize(race.RacesBreathableGases.OrderBy(x => x.GasId).Select(x => new FluidLink(x.GasId, x.Multiplier)).ToArray());
			fields["BreathableLiquids"] = JsonSerializer.Serialize(race.RacesBreathableLiquids.OrderBy(x => x.LiquidId).Select(x => new FluidLink(x.LiquidId, x.Multiplier)).ToArray());
			fields["NaturalAttacks"] = JsonSerializer.Serialize(race.RacesWeaponAttacks.OrderBy(x => x.WeaponAttackId).ThenBy(x => x.BodypartId)
				.Select(x => new NaturalAttackLink(x.WeaponAttackId, x.BodypartId, x.Quality)).ToArray());
		}
		if (row is NpcTemplate template) fields["AIs"] = JsonSerializer.Serialize(template.NpctemplatesArtificalIntelligences.Select(x => x.AiId).Order().ToArray());
	}

	private void ApplyCreatureChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
		bool Changed(string key) => current[key] != merged[key];
		T[] Values<T>(string key) => JsonSerializer.Deserialize<T[]>(merged[key])!;
		if (row is Culture culture && Changed("NameCultures")) Replace(culture.CulturesNameCultures,
			Values<NameCultureLink>("NameCultures").Select(x => new CulturesNameCultures { CultureId = culture.Id, Gender = x.Gender, NameCultureId = x.Id }), x => (x.Gender, x.NameCultureId));
		if (row is Race race)
		{
			if (Changed("Attributes")) Replace(race.RacesAttributes, Values<RaceAttribute>("Attributes").Select(x => new RacesAttributes
			{ RaceId = race.Id, AttributeId = x.Id, IsHealthAttribute = x.Health, AttributeBonus = x.Bonus, DiceExpression = x.Dice }), x => x.AttributeId,
				(actual, next) => { actual.IsHealthAttribute = next.IsHealthAttribute; actual.AttributeBonus = next.AttributeBonus; actual.DiceExpression = next.DiceExpression; });
			if (Changed("BreathableGases")) Replace(race.RacesBreathableGases, Values<FluidLink>("BreathableGases").Select(x => new RacesBreathableGases
			{ RaceId = race.Id, GasId = x.Id, Multiplier = x.Multiplier }), x => x.GasId, (actual, next) => actual.Multiplier = next.Multiplier);
			if (Changed("BreathableLiquids")) Replace(race.RacesBreathableLiquids, Values<FluidLink>("BreathableLiquids").Select(x => new RacesBreathableLiquids
			{ RaceId = race.Id, LiquidId = x.Id, Multiplier = x.Multiplier }), x => x.LiquidId, (actual, next) => actual.Multiplier = next.Multiplier);
			if (Changed("NaturalAttacks")) Replace(race.RacesWeaponAttacks, Values<NaturalAttackLink>("NaturalAttacks").Select(x => new RacesWeaponAttacks
			{ RaceId = race.Id, WeaponAttackId = x.Attack, BodypartId = x.Part, Quality = x.Quality }), x => (x.WeaponAttackId, x.BodypartId), (actual, next) => actual.Quality = next.Quality);
		}
		if (row is NpcTemplate template && Changed("AIs")) Replace(template.NpctemplatesArtificalIntelligences,
			Values<long>("AIs").Select(x => new NpcTemplatesArtificalIntelligences { NpcTemplateId = template.Id, NpcTemplateRevisionNumber = template.RevisionNumber, AiId = x }), x => x.AiId);
	}

	private sealed record RaceAttribute(long Id, bool Health, double Bonus, string? Dice);
	private sealed record NameCultureLink(short Gender, long Id);
	private sealed record FluidLink(long Id, double Multiplier);
	private sealed record NaturalAttackLink(long Attack, long Part, int Quality);
}
