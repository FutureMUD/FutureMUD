using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using Drug = MudSharp.Models.Drug;

#nullable enable
namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal const double PoisonGramsPerDurationUnit = 0.001;
	private readonly Dictionary<int, FutureProg> _poisonDoseProgs = new();
	private readonly Dictionary<int, long> _poisonDrugIds = new();
	private static readonly string[] LegacyPoisonNames =
		["Generic Poison", "Grishen", "Skellebain", "Methelinoc Toxin", "Terradin", "Peraine", "Heramide", "Plague Toxin", "Skellebain (second stage)", "Skellebain (third stage)"];

	private void InstallLegacyPoisonEffects()
	{
		var traits = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		long Trait(params string[] names) => names.Select(name => traits.FirstOrDefault(x =>
			x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault(x => x is not null)?.Id
			?? throw new InvalidOperationException($"Legacy poison requires a native attribute matching {string.Join("/", names)}.");
		var endurance = Trait("Endurance", "Constitution", "Hardiness", "Resilience", "Stamina");
		var agility = Trait("Agility", "Dexterity");
		var wisdom = Trait("Wisdom", "Willpower", "Intelligence", "Mind");
		var halfGiant = _races.Values.FirstOrDefault(x =>
			x.Name.Equals("Half-Giant", StringComparison.OrdinalIgnoreCase) || x.Name.Equals("Half Giant", StringComparison.OrdinalIgnoreCase));
		PoisonResistanceSeederHelper.Ensure(_db, traits.Single(x => x.Id == endurance),
			() => Next<TraitExpression>(), () => Next<CheckTemplate>());
		var feces = _db.Materials.FirstOrDefault(x => x.Name == "feces")
			?? throw new InvalidOperationException("Skellebain requires the core feces material.");
		var residue = (Material)_db.Entry(feces).CurrentValues.ToObject();
		residue.Id = Next<Material>();
		residue.Name = "Worldfile bowel residue";
		residue.MaterialDescription = "foul bowel residue";
		residue.ResidueSdesc = "(soiled)";
		residue.ResidueDesc = "It is soiled with foul bowel residue.";
		residue.ResidueColour = "brown";
		residue.SolventId = _db.Liquids.First(x => x.Name == "water").Id;
		residue.SolventVolumeRatio = 1;
		_db.Materials.Add(residue);
		Record("poison.bowel-residue", nameof(Material), residue.Id, null, new { SourceMaterial = feces.Id, AuthoredKilogramsPerEpisode = 0.01 });
		for (var index = 1; index <= 10; index++)
		{
			var drug = CreateLegacyPoisonDrug(index, endurance, agility, wisdom,
				residue.Id);
			if (index == 1 && _races.TryGetValue(18, out var vampire))
			{
				var symptoms = DrugSymptomsAdditionalInfo.Parse(drug.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Symptoms).AdditionalEffects);
				symptoms.ImmuneRaceIds.Add(vampire.Id);
				drug.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Symptoms).AdditionalEffects = symptoms.DatabaseString;
			}
			if (index == 8)
			{
				var profile = CreateLegacyPoisonDrug(8, endurance, agility, wisdom);
				profile.Id = Next<Drug>();
				profile.Name = "Plague Symptoms";
				profile.DrugVectors = 0;
				profile.DoseBehaviour = (int)DrugDoseBehaviour.Normal;
				profile.FixedDoseGrams = 0;
				foreach (var effect in profile.DrugsIntensities) effect.DrugId = profile.Id;
				_db.Drugs.Add(profile);
				drug.DrugsIntensities.Clear();
				drug.DrugsIntensities.Add(new DrugIntensity
				{
					DrugType = (int)DrugType.Infection, RelativeIntensity = 1.0,
					AdditionalEffects = new DrugInfectionAdditionalInfo
					{
						Name = "plague", SymptomDrugId = profile.Id, RecoveryMode = InfectionRecoveryMode.FixedDuration,
						DurationSeconds = 30000, InitialIntensity = 1, SpreadIntervalSeconds = 3,
						SpreadOnSymptomEpisode = true, SpreadChance = 0.0001, CureWithDrug = true
					}.DatabaseString
				});
				Record("poison.plague-symptoms", nameof(Drug), profile.Id, null,
					new { DurationSeconds = 30000, SourceSpreadRoll = "1 in 10000 per room occupant during an episode", SymptomProfile = true });
			}
			drug.Id = Next<Drug>();
			foreach (var effect in drug.DrugsIntensities) effect.DrugId = drug.Id;
			_db.Drugs.Add(drug);
			_poisonDrugIds.Add(index, drug.Id);
			Record("poison.drug." + index, nameof(Drug), drug.Id, null, new
			{
				SourcePoisonIndex = index, SourceName = LegacyPoisonNames[index - 1],
				GramsPerDurationUnit = PoisonGramsPerDurationUnit, SourceSecondsPerDurationUnit = 600,
				ReferenceBodyKilograms = 70, ReferenceLiverKilogramsPerHour = 0.01,
				Disposition = "Approved authored mass/time calibration through native absorption and metabolism, with source-specific default duration, peraine halving and plague duplicate refusal. No historical dose or expanded healer eligibility is inferred."
			});
			var dose = Prog("ArmageddonWorldfilePoisonDose" + index, LegacyPoisonDoseProg(index, drug.Id, halfGiant?.Id ?? 0),
				ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("units", ProgVariableTypes.Number),
				("vector", ProgVariableTypes.Text), ("save", ProgVariableTypes.Boolean));
			_poisonDoseProgs.Add(index, dose);
		}
	}

	internal static Drug CreateLegacyPoisonDrug(int index, long endurance, long agility, long wisdom, long fecesMaterialId = 0)
	{
		if (index is < 1 or > 10) throw new ArgumentOutOfRangeException(nameof(index));
		var drug = new Drug
		{
			Name = LegacyPoisonNames[index - 1], IntensityPerGram = 70.0, RelativeMetabolisationRate = 0.0006,
			DrugVectors = (int)(DrugVector.Ingested | DrugVector.Injected | DrugVector.Inhaled)
		};
		void Effect(DrugType type, double intensity, DrugAdditionalInfo? info = null) => drug.DrugsIntensities.Add(new DrugIntensity
		{ DrugType = (int)type, RelativeIntensity = intensity, AdditionalEffects = info?.DatabaseString });
		var symptoms = new DrugSymptomsAdditionalInfo { ResistanceTraitId = endurance, TickIntervalSeconds = 3.0 };
		var stage = symptoms.Stages[0];
		switch (index)
		{
			case 1:
				stage.Chance = "1.0 / Max(1, resistance + 6)";
				stage.Damage = "rand(1, 5)";
				stage.MaximumHitPointsBonus = "-damage";
				stage.MaximumHitPointsDurationSeconds = 5400;
				stage.MaximumHitPointsCapacityOnly = true;
				stage.Echoes = ["A wave of pain wracks your body."];
				break;
			case 2:
				stage.Chance = "1.0 / Max(1, resistance + 1)";
				stage.Stamina = "5";
				stage.SleepAtZeroStamina = true;
				stage.Echoes = ["You break into a feverish sweat.", "Your muscles feel weak and exhausted."];
				break;
			case 3: case 9: case 10:
				if (index == 3)
				{
					symptoms.OnsetPerDose = true;
					symptoms.OnsetTraitChanges.Add(new DrugPermanentTraitChange
					{ TraitId = wisdom, Chance = "0.001", Amount = -1.0, MinimumValue = 1.0 });
				}
				symptoms.InitialStage = index == 3 ? 0 : index == 9 ? 1 : 2;
				symptoms.Stages =
				[
					new() { AwakeOnly = true, TransitionChance = "1.0 / Max(1, resistance + 29)", NextStages = [1] },
					new() { AwakeOnly = true, TransitionChance = "1.0 / Max(1, resistance + 21)", NextStages = [0, 2],
						NextStageWeights = new() { [0] = "Max(0, resistance + 40)", [2] = "Max(0, 60 - resistance)" },
						Chance = "1.0 / 23", Outcomes = [new() { Weight = 49 }, new() { Action = DrugSymptomAction.Sit, PrivateEcho = "You feel the world lurch beneath you." }, new() { PrivateEcho = "Your hand seems to burst into flames!", RoomEmote = "$0 look|looks at &0's hand and yelp|yelps." }], Echoes = ["The ground seems to lurch beneath you.", "You hear mocking laughter just behind you.", "For a moment, everything around you seems covered in coarse hair."] },
					new() { AwakeOnly = true, TransitionChance = "1.0 / Max(1, resistance + 26)", NextStages = [1],
						Chance = "1.0 / 23", Outcomes =
						[
							new() { Weight = 34 },
							new() { Action = DrugSymptomAction.Sit, PrivateEcho = "The ground spins in circles. You feel terribly dizzy." },
							new() { PrivateEcho = "You vomit uncontrollably over your chest.", RoomEmote = "$0 make|makes a gagging sound and vomit|vomits loudly." },
							new() { Action = DrugSymptomAction.Residue, PrivateEcho = "You lose control of your bowels, making a nasty mess.", ResidueMaterialId = fecesMaterialId, ResidueKilograms = 0.01, ClothingBodypart = "leg" },
							new() { Action = DrugSymptomAction.Babble, PrivateEcho = "Your tongue feels numb and fat in your mouth.", DurationSeconds = 600 },
							new() { Action = DrugSymptomAction.DropReadiedItems, ReleaseGrapple = true, PrivateEcho = "Your hands seem to melt before your very eyes into little piles of goo." }
						], Echoes = ["Someone swings at your neck! You flinch from a blow that never lands.", "You are suddenly certain that everyone nearby wants you dead.", "The room twists and spins around you."] }
				];
				break;
			case 4:
				stage.Chance = "1.0 / Max(1, resistance + 1)";
				stage.Damage = "rand(5, Max(5, resistance))";
				stage.Echoes = ["Burning pain courses through your body."];
				stage.KillSleeping = true;
				break;
			case 5:
				Effect(DrugType.Nausea, 5.0);
				stage.Chance = "if(fullness <= 0, 1.0 / Max(1, resistance + 1), 0)";
				stage.Damage = "rand(1, Max(1, 105 - resistance))";
				stage.Echoes = ["Your empty stomach cramps painfully."];
				break;
			case 6:
				// A reference body's final ten-second metabolism quantum still exceeds the native 0.75 threshold.
				Effect(DrugType.Paralysis, 60.0);
				stage.CeaseCombatOnParalysis = true;
				break;
			case 7:
				stage.PreventNaturalStunRecovery = true;
				stage.RecoveryBlockOnlyWhenUnconscious = true;
				stage.Chance = "if(unconscious = 1, 0, 1.0 / Max(1, Floor(resistance / 5) + 1))";
				stage.Stun = "rand(1, 4) * rand(20, 25)";
				stage.Echoes = ["A dizzy spell blurs your vision.", "Your head swims and your surroundings grow distant."];
				break;
			case 8:
				drug.DoseBehaviour = (int)(DrugDoseBehaviour.FixedDose | DrugDoseBehaviour.IgnoreAdditionalExposure);
				drug.FixedDoseGrams = 0.05;
				symptoms.ExpiryTraitChanges.Add(new DrugPermanentTraitChange
				{ TraitId = wisdom, Chance = "1", Amount = -1.0, MinimumValue = 0.0 });
				Effect(DrugType.Modifier, 1.0, new DrugModifierAdditionalInfo
				{ Steps = [new DrugModifierStep(0.0, TraitId: agility, TraitBonus: -4)] });
				stage.Chance = "1.0 / (Min(Abs(stamina), 10) + 1)";
				stage.Echoes = ["You shiver with fever.", "An irritating cough catches in your throat.", "You sneeze and wipe sweat from your brow."];
				break;
		}
		Effect(DrugType.Symptoms, 1.0, symptoms);
		return drug;
	}

	internal static double LegacyPoisonDoseGrams(int index, double units)
	{
		if (index is < 1 or > 10 || !double.IsFinite(units) || units < 0.0) return 0.0;
		units = Math.Floor(units);
		if (index == 8 || index == 4 && units == 0.0) units = 50.0;
		if (index == 6) units = Math.Max(1.0 / 60.0, Math.Floor(units / 2.0));
		return units * PoisonGramsPerDurationUnit;
	}

	internal static string LegacyPoisonDoseProg(int index, long drugId, long halfGiantRaceId = 0)
	{
		var body = new StringBuilder($"if (@units < 0)\nreturn false\nend if\nunits = floor(@units)\nvar drug = todrug({drugId})\n");
		if (index is 3 or 9 or 10 && halfGiantRaceId > 0)
			body.AppendLine($"if (@actor.race == torace({halfGiantRaceId}))\nreturn false\nend if");
		body.AppendLine("if (@units == 0 and @drug.fixedadministration)\nunits = @drug.fixeddosegrams / 0.001\nend if");
		if (index == 4) body.AppendLine("if (@units == 0)\nunits = 50\nend if");
		if (index == 6) body.AppendLine("units = floor(@units / 2)\nif (@units < (1.0 / 60.0))\nunits = 1.0 / 60.0\nend if");
		body.AppendLine("if (@units <= 0)\nreturn false\nend if\nif (@save)\nif (resistpoison(@actor, 5))\nreturn false\nend if\nend if\nreturn dosedrug(@actor, @drug, @units * 0.001, @vector)");
		return body.ToString();
	}
}
