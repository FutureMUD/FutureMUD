using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC;

namespace MudSharp.Work.Agriculture;

public class AgricultureField : SaveableItem, IAgricultureField
{
	private readonly List<AgricultureFieldHerd> _herds = new();
	private readonly Dictionary<AgricultureScoreType, int> _customScores = new();
	private long _cellId;
	private long _profileId;
	private IAgricultureFieldProfile _profile;
	private long _cropDefinitionId;
	private IAgricultureCropDefinition _cropDefinition;
	private int _cropGrowthDays;
	private int _cropHarvestCount;
	private int _cropHealth;
	private int _cropYieldPotential;
	private long _woodlandDefinitionId;
	private IAgricultureWoodlandDefinition _woodlandDefinition;
	private int _woodlandGrowthDays;
	private int _woodlandHealth;
	private int _woodlandYieldPotential;

	public AgricultureField(Models.AgricultureField field, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDb(field);
	}

	public AgricultureField(ICell cell, IAgricultureFieldProfile profile)
	{
		Gameworld = cell.Gameworld;
		Cell = cell;
		_cellId = cell.Id;
		Profile = profile;
		CurrentUse = AgricultureFieldUse.Fallow;
		foreach (var score in AgricultureScoreTypeExtensions.ActiveScoreTypes(Gameworld))
		{
			SetScore(score, profile.DefaultScores.TryGetValue(score, out var value) ? value : 50);
		}

		using (new FMDB())
		{
			var dbitem = new Models.AgricultureField
			{
				CellId = cell.Id,
				ProfileId = profile.Id,
				CurrentUse = (int)CurrentUse,
				Moisture = Moisture,
				Drainage = Drainage,
				Nutrients = Nutrients,
				Salinity = Salinity,
				Topsoil = Topsoil,
				Tilth = Tilth,
				Rockiness = Rockiness,
				Weeds = Weeds,
				Pests = Pests,
				Fence = Fence,
				Pasture = Pasture,
				Condition = Condition,
				Definition = SaveFieldDefinition().ToString()
			};
			FMDB.Context.AgricultureFields.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureField";
	public ICell Cell { get; private set; }

	public IAgricultureFieldProfile Profile
	{
		get
		{
			if (_profile == null && _profileId != 0)
			{
				_profile = Gameworld.AgricultureFieldProfiles.Get(_profileId);
			}

			return _profile;
		}
		set
		{
			_profile = value;
			_profileId = value?.Id ?? 0;
			Changed = true;
		}
	}

	public AgricultureFieldUse CurrentUse { get; private set; }
	public AgricultureCropStage CropStage { get; private set; }
	public int CropGrowthDays => _cropGrowthDays;
	public int CropHarvestCount => _cropHarvestCount;
	public int CropHealth => _cropHealth;
	public int CropYieldPotential => _cropYieldPotential;
	public int WoodlandGrowthDays => _woodlandGrowthDays;
	public int WoodlandHealth => _woodlandHealth;
	public int WoodlandYieldPotential => _woodlandYieldPotential;

	public IAgricultureCropDefinition CurrentCrop
	{
		get
		{
			if (_cropDefinition == null && _cropDefinitionId != 0)
			{
				_cropDefinition = Gameworld.AgricultureCropDefinitions.Get(_cropDefinitionId);
			}

			return _cropDefinition;
		}
	}

	public IAgricultureWoodlandDefinition CurrentWoodland
	{
		get
		{
			if (_woodlandDefinition == null && _woodlandDefinitionId != 0)
			{
				_woodlandDefinition = Gameworld.AgricultureWoodlandDefinitions.Get(_woodlandDefinitionId);
			}

			return _woodlandDefinition;
		}
	}

	public IEnumerable<IAgricultureFieldHerd> Herds => _herds;
	public int Moisture { get; set; }
	public int Drainage { get; set; }
	public int Nutrients { get; set; }
	public int Salinity { get; set; }
	public int Topsoil { get; set; }
	public int Tilth { get; set; }
	public int Rockiness { get; set; }
	public int Weeds { get; set; }
	public int Pests { get; set; }
	public int Fence { get; set; }
	public int Pasture { get; set; }
	public int Condition { get; set; }

	private void LoadFromDb(Models.AgricultureField field)
	{
		_id = field.Id;
		_name = $"Field #{field.Id}";
		_cellId = field.CellId;
		Cell = Gameworld.Cells.Get(_cellId);
		_profileId = field.ProfileId;
		CurrentUse = (AgricultureFieldUse)field.CurrentUse;
		Moisture = field.Moisture.ClampScore();
		Drainage = field.Drainage.ClampScore();
		Nutrients = field.Nutrients.ClampScore();
		Salinity = field.Salinity.ClampScore();
		Topsoil = field.Topsoil.ClampScore();
		Tilth = field.Tilth.ClampScore();
		Rockiness = field.Rockiness.ClampScore();
		Weeds = field.Weeds.ClampScore();
		Pests = field.Pests.ClampScore();
		Fence = field.Fence.ClampScore();
		Pasture = field.Pasture.ClampScore();
		Condition = field.Condition.ClampScore();
		var fieldRoot = AgricultureXmlExtensions.RootOrDefault(field.Definition, "Field");
		foreach (var score in fieldRoot.Element("CustomScores")?.LoadScores() ?? new Dictionary<AgricultureScoreType, int>())
		{
			if (score.Key.IsCustomScore())
			{
				_customScores[score.Key] = score.Value;
			}
		}

		if (field.AgricultureFieldCrop != null)
		{
			_cropDefinitionId = field.AgricultureFieldCrop.CropDefinitionId;
			CropStage = (AgricultureCropStage)field.AgricultureFieldCrop.Stage;
			_cropGrowthDays = field.AgricultureFieldCrop.GrowthDays;
			_cropHealth = field.AgricultureFieldCrop.Health.ClampScore();
			_cropYieldPotential = field.AgricultureFieldCrop.YieldPotential.ClampScore();
			var cropRoot = AgricultureXmlExtensions.RootOrDefault(field.AgricultureFieldCrop.Definition, "Crop");
			_cropHarvestCount = Math.Max(0, (int?)cropRoot.Attribute("harvestCount") ?? 0);
		}

		if (field.AgricultureFieldWoodland != null)
		{
			_woodlandDefinitionId = field.AgricultureFieldWoodland.WoodlandDefinitionId;
			_woodlandGrowthDays = field.AgricultureFieldWoodland.GrowthDays;
			_woodlandHealth = field.AgricultureFieldWoodland.Health.ClampScore();
			_woodlandYieldPotential = field.AgricultureFieldWoodland.YieldPotential.ClampScore();
		}

		foreach (var herd in field.AgricultureFieldHerds)
		{
			var definition = Gameworld.AgricultureHerdDefinitions.Get(herd.HerdDefinitionId);
			if (definition != null)
			{
				_herds.Add(new AgricultureFieldHerd(herd.Id, definition, herd.HeadCount, herd.Condition));
			}
		}
	}

	public int Score(AgricultureScoreType score)
	{
		return score switch
		{
			AgricultureScoreType.Moisture => Moisture,
			AgricultureScoreType.Drainage => Drainage,
			AgricultureScoreType.Nutrients => Nutrients,
			AgricultureScoreType.Salinity => Salinity,
			AgricultureScoreType.Topsoil => Topsoil,
			AgricultureScoreType.Tilth => Tilth,
			AgricultureScoreType.Rockiness => Rockiness,
			AgricultureScoreType.Weeds => Weeds,
			AgricultureScoreType.Pests => Pests,
			AgricultureScoreType.Fence => Fence,
			AgricultureScoreType.Pasture => Pasture,
			AgricultureScoreType.Condition => Condition,
			_ when score.IsCustomScore() => _customScores.TryGetValue(score, out var value)
				? value
				: Profile?.DefaultScores.TryGetValue(score, out value) == true
					? value
					: 50,
			_ => 0
		};
	}

	public void SetScore(AgricultureScoreType score, int value)
	{
		value = value.ClampScore();
		switch (score)
		{
			case AgricultureScoreType.Moisture:
				Moisture = value;
				break;
			case AgricultureScoreType.Drainage:
				Drainage = value;
				break;
			case AgricultureScoreType.Nutrients:
				Nutrients = value;
				break;
			case AgricultureScoreType.Salinity:
				Salinity = value;
				break;
			case AgricultureScoreType.Topsoil:
				Topsoil = value;
				break;
			case AgricultureScoreType.Tilth:
				Tilth = value;
				break;
			case AgricultureScoreType.Rockiness:
				Rockiness = value;
				break;
			case AgricultureScoreType.Weeds:
				Weeds = value;
				break;
			case AgricultureScoreType.Pests:
				Pests = value;
				break;
			case AgricultureScoreType.Fence:
				Fence = value;
				break;
			case AgricultureScoreType.Pasture:
				Pasture = value;
				break;
			case AgricultureScoreType.Condition:
				Condition = value;
				break;
			default:
				if (score.IsCustomScore())
				{
					_customScores[score] = value;
				}

				break;
		}
	}

	public void AdjustScore(AgricultureScoreType score, int delta)
	{
		SetScore(score, Score(score) + delta);
		Changed = true;
	}

	public string DescribeTo(ICharacter voyeur, bool exact)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Agriculture Field #{Id.ToString("N0", voyeur)} - {Cell.HowSeen(voyeur)}".GetLineWithTitleInner(voyeur, Telnet.Green, Telnet.BoldWhite));
		sb.AppendLine($"Profile: {Profile?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Use: {CurrentUse.DescribeEnum().ColourName()}");
		if (CurrentCrop != null)
		{
			var harvestText = CurrentCrop.IsPerennial
				? $", harvests {_cropHarvestCount.ToString("N0", voyeur).ColourValue()}"
				: string.Empty;
			sb.AppendLine($"Crop: {CurrentCrop.Name.ColourName()} ({CropStage.DescribeEnum().ColourValue()}, health {_cropHealth.ToStringN0Colour(voyeur)}, yield {_cropYieldPotential.ToStringN0Colour(voyeur)}{harvestText})");
		}

		if (CurrentWoodland != null)
		{
			sb.AppendLine($"Woodland: {CurrentWoodland.Name.ColourName()} ({_woodlandGrowthDays.ToString("N0", voyeur).ColourValue()} managed days, health {_woodlandHealth.ToStringN0Colour(voyeur)}, yield {_woodlandYieldPotential.ToStringN0Colour(voyeur)})");
		}

		if (_herds.Any())
		{
			sb.AppendLine("Herds:");
			foreach (var herd in _herds)
			{
				sb.AppendLine($"\t{herd.Definition.Name.ColourName()} - {herd.HeadCount.ToString("N0", voyeur).ColourValue()} head, condition {herd.Condition.ToString("N0", voyeur).ColourValue()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Field State:");
		foreach (var score in AgricultureScoreTypeExtensions.ActiveScoreTypes(Gameworld))
		{
			var value = Score(score);
			var display = exact ? $"{value.ToString("N0", voyeur).ColourValue()} ({value.DescribeBandColoured()})" : value.DescribeBandColoured();
			sb.AppendLine($"\t{score.DescribeFor(Gameworld).ColourName()}: {display}");
		}

		return sb.ToString();
	}

	public void DailyTick()
	{
		var weather = Cell.CurrentWeather(null);
		switch (weather?.Precipitation ?? PrecipitationLevel.Dry)
		{
			case PrecipitationLevel.Parched:
			case PrecipitationLevel.Dry:
				AdjustScore(AgricultureScoreType.Moisture, -3);
				break;
			case PrecipitationLevel.Humid:
				AdjustScore(AgricultureScoreType.Moisture, 1);
				break;
			case PrecipitationLevel.LightRain:
			case PrecipitationLevel.Rain:
				AdjustScore(AgricultureScoreType.Moisture, 4);
				break;
			case PrecipitationLevel.HeavyRain:
			case PrecipitationLevel.TorrentialRain:
				AdjustScore(AgricultureScoreType.Moisture, 7 - Drainage / 25);
				AdjustScore(AgricultureScoreType.Topsoil, Drainage < 35 ? -1 : 0);
				break;
			case PrecipitationLevel.LightSnow:
			case PrecipitationLevel.Snow:
			case PrecipitationLevel.HeavySnow:
			case PrecipitationLevel.Blizzard:
			case PrecipitationLevel.Sleet:
				AdjustScore(AgricultureScoreType.Moisture, 2);
				break;
		}

		if (Drainage > 65 && Moisture > 50)
		{
			AdjustScore(AgricultureScoreType.Moisture, -1);
		}
		else if (Drainage < 35 && Moisture > 75)
		{
			AdjustScore(AgricultureScoreType.Condition, -1);
		}

		TickCrop();
		TickHerds();
		TickWoodland();
	}

	private void TickCrop()
	{
		var crop = CurrentCrop;
		if (crop == null || CropStage is AgricultureCropStage.Failed or AgricultureCropStage.Overripe)
		{
			return;
		}

		var temperature = Cell.CurrentTemperature(null);
		var stressed = Moisture < crop.MinimumMoisture || Moisture > crop.MaximumMoisture ||
		               temperature < crop.MinimumTemperature || temperature > crop.MaximumTemperature ||
		               Weeds > 75 || Pests > 75 || Salinity > 80 ||
		               crop.ScoreRanges.Any(x => x.Score.IsEnabledScore(Gameworld) && !x.Contains(Score(x.Score)));
		if (stressed)
		{
			_cropHealth = (_cropHealth - 4).ClampScore();
			_cropYieldPotential = (_cropYieldPotential - 3).ClampScore();
		}
		else
		{
			_cropHealth = (_cropHealth + 1).ClampScore();
			_cropYieldPotential = (_cropYieldPotential + Math.Sign(Nutrients - 50)).ClampScore();
			_cropGrowthDays++;
		}

		AdjustScore(AgricultureScoreType.Nutrients, -1);
		AdjustScore(AgricultureScoreType.Weeds, 1);
		AdjustScore(AgricultureScoreType.Pests, _cropHealth < 40 ? 2 : 1);

		if (_cropHealth <= 0)
		{
			CropStage = AgricultureCropStage.Failed;
			return;
		}

		var harvestDays = crop.IsPerennial && _cropHarvestCount > 0 ? crop.HarvestCycleDays : crop.BaseGrowthDays;
		if (_cropGrowthDays >= harvestDays + crop.HarvestWindowDays)
		{
			CropStage = AgricultureCropStage.Overripe;
		}
		else if (_cropGrowthDays >= harvestDays)
		{
			CropStage = AgricultureCropStage.Harvestable;
		}
		else if (_cropGrowthDays >= harvestDays * 2 / 3)
		{
			CropStage = AgricultureCropStage.Setting;
		}
		else if (_cropGrowthDays >= harvestDays / 3)
		{
			CropStage = AgricultureCropStage.Growing;
		}
		else if (_cropGrowthDays >= 3)
		{
			CropStage = AgricultureCropStage.Germinating;
		}

		Changed = true;
	}

	private void TickHerds()
	{
		if (!_herds.Any())
		{
			return;
		}

		var demand = _herds.Sum(x => x.HeadCount * x.Definition.DailyGraze * x.Definition.AnimalUnits);
		if (demand <= 0.0)
		{
			return;
		}

		if (Pasture >= demand)
		{
			AdjustScore(AgricultureScoreType.Pasture, -(int)Math.Ceiling(demand));
			AdjustScore(AgricultureScoreType.Nutrients, demand < 8.0 ? 1 : 0);
			foreach (var herd in _herds)
			{
				herd.Condition = Math.Min(herd.Definition.MaximumCondition, herd.Condition + 1.0);
			}
		}
		else
		{
			AdjustScore(AgricultureScoreType.Pasture, -Math.Max(1, Pasture / 2));
			AdjustScore(AgricultureScoreType.Condition, -2);
			AdjustScore(AgricultureScoreType.Topsoil, -1);
			AdjustScore(AgricultureScoreType.Tilth, -1);
			AdjustScore(AgricultureScoreType.Fence, _herds.Sum(x => x.HeadCount) > 0 ? -1 : 0);
			foreach (var herd in _herds)
			{
				herd.Condition = Math.Max(0.0, herd.Condition - 3.0);
			}
		}

		Changed = true;
	}

	private void TickWoodland()
	{
		var woodland = CurrentWoodland;
		if (woodland == null)
		{
			return;
		}

		_woodlandGrowthDays++;
		var stressed = Moisture < 15 || Moisture > 90 || Topsoil < 25 || Pests > 80;
		_woodlandHealth = (_woodlandHealth + (stressed ? -2 : 1)).ClampScore();
		if (_woodlandGrowthDays > woodland.EstablishmentDays)
		{
			_woodlandYieldPotential = (_woodlandYieldPotential + (stressed ? 0 : 1)).ClampScore();
		}

		AdjustScore(AgricultureScoreType.Nutrients, _woodlandGrowthDays % 10 == 0 ? -1 : 0);
		Changed = true;
	}

	public bool CanBeginOperation(ICharacter actor, IAgricultureOperation operation, IFrameworkItem target, out string reason)
	{
		return CanBeginOperation(actor, operation, target, true, out reason);
	}

	private bool CanBeginOperation(ICharacter actor, IAgricultureOperation operation, IFrameworkItem target, bool enforceActorAccess, out string reason)
	{
		if (operation == null)
		{
			reason = "There is no such agriculture operation.";
			return false;
		}

		if (!Profile.AllowsUse(operation.ResultUse) && operation.OperationType != AgricultureOperationType.Clear)
		{
			reason = $"The {Profile.Name} profile does not support {operation.ResultUse.DescribeEnum().ToLowerInvariant()} use.";
			return false;
		}

		if (!string.IsNullOrEmpty(operation.WhyCannotApply(this, target)))
		{
			reason = operation.WhyCannotApply(this, target);
			return false;
		}

		var property = Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(Cell));
		if (enforceActorAccess && property != null &&
		    (actor == null || !actor.IsAdministrator() && !property.IsAuthorisedOwner(actor) && !property.IsAuthorisedLeaseHolder(actor)))
		{
			reason = "You are not authorised to work this property.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, out string result)
	{
		return ApplyOperation(operation, target, actor, true, out result);
	}

	public bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, bool enforceActorAccess, out string result)
	{
		return ApplyOperation(operation, target, actor, enforceActorAccess, AgricultureWorkOutcome.Neutral, out result);
	}

	public bool ApplyOperation(IAgricultureOperation operation, IFrameworkItem target, ICharacter actor, bool enforceActorAccess,
		AgricultureWorkOutcome outcome, out string result)
	{
		if (!CanBeginOperation(actor, operation, target, enforceActorAccess, out result))
		{
			return false;
		}

		var completionProblem = WhyCannotCompleteOperation(operation, target);
		if (!string.IsNullOrEmpty(completionProblem))
		{
			result = completionProblem;
			return false;
		}

		outcome ??= AgricultureWorkOutcome.Neutral;
		foreach (var delta in operation.ScoreDeltas)
		{
			if (!delta.Key.IsEnabledScore(Gameworld))
			{
				continue;
			}

			ApplySkillAdjustedScoreDelta(delta.Key, delta.Value, outcome);
		}

		switch (operation.OperationType)
		{
			case AgricultureOperationType.Sow:
				var crop = (IAgricultureCropDefinition)target;
				_cropDefinition = crop;
				_cropDefinitionId = crop.Id;
				CropStage = AgricultureCropStage.Planted;
				_cropGrowthDays = 0;
				_cropHarvestCount = 0;
				_cropHealth = (Condition + outcome.CropHealthDelta).ClampScore();
				_cropYieldPotential = (Condition + Nutrients + Topsoil - Weeds - Pests + outcome.CropYieldDelta).ClampScore();
				CurrentUse = AgricultureFieldUse.Crop;
				result = $"The field has been sown with {crop.Name}.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.PlantOrchard:
				var orchardCrop = (IAgricultureCropDefinition)target;
				_cropDefinition = orchardCrop;
				_cropDefinitionId = orchardCrop.Id;
				CropStage = AgricultureCropStage.Planted;
				_cropGrowthDays = 0;
				_cropHarvestCount = 0;
				_cropHealth = (Condition + outcome.CropHealthDelta).ClampScore();
				_cropYieldPotential = ((Condition + Nutrients + Topsoil - Weeds - Pests + outcome.CropYieldDelta).ClampScore() / 2).ClampScore();
				CurrentUse = AgricultureFieldUse.Orchard;
				result = $"The field has been planted as {orchardCrop.Name}.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.Harvest:
				if (CurrentCrop == null || CropStage is not (AgricultureCropStage.Harvestable or AgricultureCropStage.Overripe))
				{
					result = "There is no harvest-ready crop in this field.";
					return false;
				}

				var cropName = CurrentCrop.Name;
				var perennialHarvest = CurrentUse == AgricultureFieldUse.Orchard && CurrentCrop.IsPerennial;
				var cropOutputs = ReleaseCommodityOutputs(CurrentCrop.YieldOutputs, _cropHealth, _cropYieldPotential,
					CropStage == AgricultureCropStage.Overripe ? 0.75 : 1.0, outcome);
				result = $"The {cropName} crop is harvested with an estimated yield quality of {_cropYieldPotential.DescribeBand()}.{outcome.DescribeEffect()}{DescribeOutputResult(cropOutputs)}";
				if (perennialHarvest)
				{
					_cropHarvestCount++;
					_cropGrowthDays = 0;
					_cropHealth = (_cropHealth + outcome.CropHealthDelta).ClampScore();
					_cropYieldPotential = (_cropYieldPotential - 20 + outcome.CropYieldDelta).ClampScore();
					CropStage = AgricultureCropStage.Growing;
					CurrentUse = AgricultureFieldUse.Orchard;
				}
				else
				{
					ClearCrop();
					CurrentUse = AgricultureFieldUse.Fallow;
				}
				break;
			case AgricultureOperationType.Graze:
			case AgricultureOperationType.Herd:
				CurrentUse = AgricultureFieldUse.Pasture;
				if (target is IAgricultureHerdDefinition herdDefinition && _herds.All(x => x.Definition.Id != herdDefinition.Id))
				{
					_herds.Add(new AgricultureFieldHerd(0, herdDefinition, 0, Condition));
				}

				result = "The field is now being managed as pasture.";
				break;
			case AgricultureOperationType.Woodland:
				var woodland = (IAgricultureWoodlandDefinition)target;
				_woodlandDefinition = woodland;
				_woodlandDefinitionId = woodland.Id;
				_woodlandGrowthDays = 0;
				_woodlandHealth = Condition;
				_woodlandYieldPotential = 0;
				CurrentUse = AgricultureFieldUse.Woodland;
				result = $"The field is now being managed as {woodland.Name}.";
				break;
			case AgricultureOperationType.Clear:
				var clearOutputs = ReleaseWoodlandOutputs(operation, outcome);
				ClearCrop();
				ClearWoodland();
				_herds.Clear();
				CurrentUse = AgricultureFieldUse.Fallow;
				result = $"The field has been cleared back to fallow land.{outcome.DescribeEffect()}{DescribeOutputResult(clearOutputs)}";
				break;
			default:
				CurrentUse = operation.ResultUse;
				var woodlandOutputs = ReleaseWoodlandOutputs(operation, outcome);
				result = $"The {operation.Name} operation has been applied to the field.{outcome.DescribeEffect()}{DescribeOutputResult(woodlandOutputs)}";
				break;
		}

		RunCompletionProg(operation, actor);
		Changed = true;
		return true;
	}

	private void ApplySkillAdjustedScoreDelta(AgricultureScoreType score, int delta, AgricultureWorkOutcome outcome)
	{
		if (delta == 0)
		{
			return;
		}

		var beneficial = IsBeneficialDelta(score, delta);
		var multiplier = beneficial ? outcome.BeneficialScoreMultiplier : outcome.HarmfulScoreMultiplier;
		var adjusted = (int)Math.Round(delta * multiplier);
		if (adjusted == 0)
		{
			adjusted = Math.Sign(delta);
		}

		AdjustScore(score, adjusted);
	}

	private bool IsBeneficialDelta(AgricultureScoreType score, int delta)
	{
		return score.HigherIsGood(Gameworld) ? delta > 0 : delta < 0;
	}

	private string WhyCannotCompleteOperation(IAgricultureOperation operation, IFrameworkItem target)
	{
		if (operation.OperationType == AgricultureOperationType.Harvest &&
		    (CurrentCrop == null ||
		     CropStage is not (AgricultureCropStage.Harvestable or AgricultureCropStage.Overripe)))
		{
			return "There is no harvest-ready crop in this field.";
		}

		return string.Empty;
	}

	private void RunCompletionProg(IAgricultureOperation operation, ICharacter actor)
	{
		var output = operation.CompletionProg?.Execute(this, actor);
		foreach (var item in CompletionOutputItems(output).Distinct())
		{
			ReleaseCompletionOutputToField(item);
		}
	}

	private IReadOnlyList<IGameItem> ReleaseWoodlandOutputs(IAgricultureOperation operation, AgricultureWorkOutcome outcome)
	{
		if (operation.WoodlandYieldMultiplier <= 0.0 || CurrentWoodland == null)
		{
			return Array.Empty<IGameItem>();
		}

		var outputs = ReleaseCommodityOutputs(CurrentWoodland.YieldOutputs, _woodlandHealth, _woodlandYieldPotential,
			operation.WoodlandYieldMultiplier, outcome);
		if (operation.WoodlandYieldCost > 0)
		{
			_woodlandYieldPotential = (_woodlandYieldPotential - Math.Min(_woodlandYieldPotential, operation.WoodlandYieldCost)).ClampScore();
		}

		return outputs;
	}

	private IReadOnlyList<IGameItem> ReleaseCommodityOutputs(IEnumerable<AgricultureCommodityYield> outputs, int health,
		int yieldPotential, double multiplier, AgricultureWorkOutcome outcome)
	{
		var outputList = outputs?.ToList() ?? [];
		if (outputList.Count == 0 || multiplier <= 0.0 || health <= 0 || yieldPotential <= 0)
		{
			return Array.Empty<IGameItem>();
		}

		if (CommodityGameItemComponentProto.ItemPrototype == null)
		{
			CommodityGameItemComponentProto.InitialiseItemType(Gameworld);
		}

		var items = new List<IGameItem>();
		var healthFactor = health.ClampScore() / 100.0;
		var yieldFactor = yieldPotential.ClampScore() / 100.0;
		foreach (var output in outputList)
		{
			var material = Gameworld.Materials.GetByName(output.MaterialName);
			if (material == null)
			{
				continue;
			}

			var skillMultiplier = output.TagName.EqualTo("Seeds") ? outcome.SeedYieldMultiplier : outcome.CropYieldMultiplier;
			var weight = output.BaseWeight * healthFactor * yieldFactor * multiplier * skillMultiplier;
			if (weight < 1.0)
			{
				continue;
			}

			var tag = string.IsNullOrWhiteSpace(output.TagName) ? null : Gameworld.Tags.GetByName(output.TagName);
			var item = CommodityGameItemComponentProto.CreateNewCommodity(material, weight, tag);
			item.Quality = outcome.OutputQuality;
			item.RoomLayer = RoomLayer.GroundLevel;
			Gameworld.Add(item);
			Cell.Insert(item, true);
			items.Add(item);
		}

		return items;
	}

	private static string DescribeOutputResult(IReadOnlyCollection<IGameItem> outputs)
	{
		return outputs.Count == 0 ? string.Empty : " Harvested commodities are left in the field.";
	}

	private IEnumerable<IGameItem> CompletionOutputItems(object output)
	{
		if (output == null)
		{
			yield break;
		}

		if (output is IGameItem item)
		{
			yield return item;
			yield break;
		}

		if (output is IProgVariable variable)
		{
			var inner = variable.GetObject;
			if (ReferenceEquals(inner, output))
			{
				yield break;
			}

			foreach (var nestedItem in CompletionOutputItems(inner))
			{
				yield return nestedItem;
			}

			yield break;
		}

		if (output is not string && output is IEnumerable enumerable)
		{
			foreach (var value in enumerable)
			{
				foreach (var nestedItem in CompletionOutputItems(value))
				{
					yield return nestedItem;
				}
			}
		}
	}

	private void ReleaseCompletionOutputToField(IGameItem item)
	{
		item.InInventoryOf?.Take(item);
		item.ContainedIn?.GetItemType<IContainer>()?.Take(null, item, 0);
		item.Location?.Extract(item);
		item.RoomLayer = RoomLayer.GroundLevel;
		Cell.Insert(item, true);
	}

	private void ClearCrop()
	{
		_cropDefinition = null;
		_cropDefinitionId = 0;
		CropStage = AgricultureCropStage.None;
		_cropGrowthDays = 0;
		_cropHarvestCount = 0;
		_cropHealth = 0;
		_cropYieldPotential = 0;
	}

	private void ClearWoodland()
	{
		_woodlandDefinition = null;
		_woodlandDefinitionId = 0;
		_woodlandGrowthDays = 0;
		_woodlandHealth = 0;
		_woodlandYieldPotential = 0;
	}

	public bool ConsumeCropYield(int amount, out string reason)
	{
		amount = Math.Max(0, amount);
		if (amount == 0)
		{
			reason = string.Empty;
			return true;
		}

		if (CurrentCrop == null)
		{
			reason = "There is no crop in this field.";
			return false;
		}

		if (_cropYieldPotential < amount)
		{
			reason = "The crop does not have enough remaining yield.";
			return false;
		}

		_cropYieldPotential = (_cropYieldPotential - amount).ClampScore();
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public bool ConsumeWoodlandYield(int amount, out string reason)
	{
		amount = Math.Max(0, amount);
		if (amount == 0)
		{
			reason = string.Empty;
			return true;
		}

		if (CurrentWoodland == null)
		{
			reason = "There is no woodland in this field.";
			return false;
		}

		if (_woodlandYieldPotential < amount)
		{
			reason = "The woodland does not have enough remaining yield.";
			return false;
		}

		_woodlandYieldPotential = (_woodlandYieldPotential - amount).ClampScore();
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public bool DrawDownHerd(IAgricultureHerdDefinition definition, int count, ICharacter actor, out string result)
	{
		count = Math.Max(1, count);
		var herd = _herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (herd == null || herd.HeadCount < count)
		{
			result = "There are not enough animals in that herd.";
			return false;
		}

		if (!definition.CanMaterialise)
		{
			result = "That herd definition does not have an NPC template for drawdown.";
			return false;
		}

		for (var i = 0; i < count; i++)
		{
			var npc = definition.NpcTemplate.CreateNewCharacter(Cell);
			Gameworld.Add(npc, true);
			definition.NpcTemplate.OnLoadProg?.Execute(npc);
			Cell.Login(npc);
			npc.HandleEvent(EventType.NPCOnGameLoadFinished, npc);
		}

		herd.HeadCount -= count;
		Changed = true;
		result = $"You draw {count.ToString("N0", actor)} {definition.Name} from the abstract herd into live NPCs.";
		return true;
	}

	public bool AbsorbNpcIntoHerd(ICharacter npc, IAgricultureHerdDefinition definition, ICharacter actor, out string result)
	{
		if (npc == null || npc.IsPlayerCharacter || npc.Location != Cell)
		{
			result = "You can only absorb a non-player animal in this field.";
			return false;
		}

		var herd = _herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (herd == null)
		{
			herd = new AgricultureFieldHerd(0, definition, 0, Condition);
			_herds.Add(herd);
		}

		npc.Quit(silent: true);
		herd.HeadCount++;
		herd.Condition = Math.Min(definition.MaximumCondition, (herd.Condition * (herd.HeadCount - 1) + Condition) / herd.HeadCount);
		CurrentUse = AgricultureFieldUse.Pasture;
		Changed = true;
		result = $"You add {npc.HowSeen(actor)} into the {definition.Name} herd.";
		return true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureFields
		                 .Include(x => x.AgricultureFieldCrop)
		                 .Include(x => x.AgricultureFieldHerds)
		                 .Include(x => x.AgricultureFieldWoodland)
		                 .First(x => x.Id == Id);
		dbitem.ProfileId = Profile.Id;
		dbitem.CurrentUse = (int)CurrentUse;
		dbitem.Moisture = Moisture;
		dbitem.Drainage = Drainage;
		dbitem.Nutrients = Nutrients;
		dbitem.Salinity = Salinity;
		dbitem.Topsoil = Topsoil;
		dbitem.Tilth = Tilth;
		dbitem.Rockiness = Rockiness;
		dbitem.Weeds = Weeds;
		dbitem.Pests = Pests;
		dbitem.Fence = Fence;
		dbitem.Pasture = Pasture;
		dbitem.Condition = Condition;
		dbitem.Definition = SaveFieldDefinition().ToString();

		FMDB.Context.AgricultureFieldCrops.RemoveRange(dbitem.AgricultureFieldCrop != null ? new[] { dbitem.AgricultureFieldCrop } : Array.Empty<Models.AgricultureFieldCrop>());
		if (CurrentCrop != null)
		{
			dbitem.AgricultureFieldCrop = new Models.AgricultureFieldCrop
			{
				AgricultureFieldId = Id,
				CropDefinitionId = CurrentCrop.Id,
				Stage = (int)CropStage,
				GrowthDays = _cropGrowthDays,
				Health = _cropHealth,
				YieldPotential = _cropYieldPotential,
				Definition = new XElement("Crop",
					new XAttribute("harvestCount", _cropHarvestCount)).ToString()
			};
		}

		FMDB.Context.AgricultureFieldHerds.RemoveRange(dbitem.AgricultureFieldHerds);
		foreach (var herd in _herds.Where(x => x.HeadCount > 0))
		{
			dbitem.AgricultureFieldHerds.Add(new Models.AgricultureFieldHerd
			{
				AgricultureFieldId = Id,
				HerdDefinitionId = herd.Definition.Id,
				HeadCount = herd.HeadCount,
				Condition = herd.Condition,
				Definition = "<Herd />"
			});
		}

		FMDB.Context.AgricultureFieldWoodlands.RemoveRange(dbitem.AgricultureFieldWoodland != null ? new[] { dbitem.AgricultureFieldWoodland } : Array.Empty<Models.AgricultureFieldWoodland>());
		if (CurrentWoodland != null)
		{
			dbitem.AgricultureFieldWoodland = new Models.AgricultureFieldWoodland
			{
				AgricultureFieldId = Id,
				WoodlandDefinitionId = CurrentWoodland.Id,
				GrowthDays = _woodlandGrowthDays,
				Health = _woodlandHealth,
				YieldPotential = _woodlandYieldPotential,
				Definition = "<Woodland />"
			};
		}

		Changed = false;
	}

	private XElement SaveFieldDefinition()
	{
		return new XElement("Field",
			new XElement("CustomScores",
				_customScores
					.Where(x => x.Key.IsCustomScore())
					.OrderBy(x => x.Key)
					.Select(x => new XElement("Score",
						new XAttribute("type", x.Key.ToString()),
						new XAttribute("value", x.Value.ClampScore())))));
	}

	#region FutureProgs

	public ProgVariableTypes Type => ProgVariableTypes.AgricultureField;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"location" => Cell,
			"profile" => new TextVariable(Profile?.Name ?? string.Empty),
			"use" => new TextVariable(CurrentUse.DescribeEnum()),
			"crop" => new TextVariable(CurrentCrop?.Name ?? string.Empty),
			"cropstage" => new TextVariable(CropStage.DescribeEnum()),
			"cropharvests" => new NumberVariable(_cropHarvestCount),
			"crophealth" => new NumberVariable(_cropHealth),
			"cropyield" => new NumberVariable(_cropYieldPotential),
			"woodland" => new TextVariable(CurrentWoodland?.Name ?? string.Empty),
			"woodlandhealth" => new NumberVariable(_woodlandHealth),
			"woodlandyield" => new NumberVariable(_woodlandYieldPotential),
			"moisture" => new NumberVariable(Moisture),
			"drainage" => new NumberVariable(Drainage),
			"nutrients" => new NumberVariable(Nutrients),
			"salinity" => new NumberVariable(Salinity),
			"topsoil" => new NumberVariable(Topsoil),
			"tilth" => new NumberVariable(Tilth),
			"rockiness" => new NumberVariable(Rockiness),
			"weeds" => new NumberVariable(Weeds),
			"pests" => new NumberVariable(Pests),
			"fence" => new NumberVariable(Fence),
			"pasture" => new NumberVariable(Pasture),
			"condition" => new NumberVariable(Condition),
			"harvestready" => new BooleanVariable(CropStage is AgricultureCropStage.Harvestable or AgricultureCropStage.Overripe),
			_ => throw new ApplicationException($"There was an invalid property requested in AgricultureField.GetProperty: {property}")
		};
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "location", ProgVariableTypes.Location },
			{ "profile", ProgVariableTypes.Text },
			{ "use", ProgVariableTypes.Text },
			{ "crop", ProgVariableTypes.Text },
			{ "cropstage", ProgVariableTypes.Text },
			{ "cropharvests", ProgVariableTypes.Number },
			{ "crophealth", ProgVariableTypes.Number },
			{ "cropyield", ProgVariableTypes.Number },
			{ "woodland", ProgVariableTypes.Text },
			{ "woodlandhealth", ProgVariableTypes.Number },
			{ "woodlandyield", ProgVariableTypes.Number },
			{ "moisture", ProgVariableTypes.Number },
			{ "drainage", ProgVariableTypes.Number },
			{ "nutrients", ProgVariableTypes.Number },
			{ "salinity", ProgVariableTypes.Number },
			{ "topsoil", ProgVariableTypes.Number },
			{ "tilth", ProgVariableTypes.Number },
			{ "rockiness", ProgVariableTypes.Number },
			{ "weeds", ProgVariableTypes.Number },
			{ "pests", ProgVariableTypes.Number },
			{ "fence", ProgVariableTypes.Number },
			{ "pasture", ProgVariableTypes.Number },
			{ "condition", ProgVariableTypes.Number },
			{ "harvestready", ProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return DotReferenceHandler().ToDictionary(x => x.Key, x => $"Agriculture field {x.Key} value.", StringComparer.InvariantCultureIgnoreCase);
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.AgricultureField, DotReferenceHandler(), DotReferenceHelp());
	}

	#endregion
}
