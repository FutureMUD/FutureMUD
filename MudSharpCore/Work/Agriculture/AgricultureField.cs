using System.Collections;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Environment;
using MudSharp.NPC;

namespace MudSharp.Work.Agriculture;

public partial class AgricultureField : SaveableItem, IAgricultureField
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
	private AgricultureFieldApiary _apiary;
	private int _pasture;
	private int _condition;
	private int _environmentalInputChangeDepth;
	private bool _environmentalInputChangePending;
	private bool _environmentalInputNotificationsEnabled;

	public AgricultureField(Models.AgricultureField field, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDb(field);
		_environmentalInputNotificationsEnabled = true;
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
		_pendingPastureAssessment = true;

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

		InitialiseLegacyNativeOrganicAccounting();
		_environmentalInputNotificationsEnabled = true;
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
			var changed = _profileId != (value?.Id ?? 0) ||
			              _profile != null && !ReferenceEquals(_profile, value);
			_profile = value;
			_profileId = value?.Id ?? 0;
			Changed = true;
			if (changed)
			{
				NotifyEnvironmentalInputsChanged();
			}
		}
	}

	public AgricultureFieldUse CurrentUse { get; private set; }
	public AgricultureCropStage CropStage { get; private set; }
	public int CropGrowthDays
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _cropGrowthDays;
			}
		}
	}
	public int CropHarvestCount
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _cropHarvestCount;
			}
		}
	}
	public int CropHealth
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _cropHealth;
			}
		}
	}
	public int CropYieldPotential
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _cropYieldPotential;
			}
		}
	}
	public int WoodlandGrowthDays
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _woodlandGrowthDays;
			}
		}
	}
	public int WoodlandHealth
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _woodlandHealth;
			}
		}
	}
	public int WoodlandYieldPotential
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _woodlandYieldPotential;
			}
		}
	}
	public IAgricultureFieldApiary Apiary => _apiary;
	public bool HasActiveApiary => _apiary?.HiveCount > 0;
	public bool IsApiaryHappy => IsApiaryHappyForPollination();
	public int PollinationStrength => IsApiaryHappy ? _apiary.PollinationStrength : 0;

	public IAgricultureCropDefinition CurrentCrop
	{
		get
		{
			IAgricultureCropDefinition cached;
			long definitionId;
			lock (_nativeOrganicOwnerSync)
			{
				cached = _cropDefinition;
				definitionId = _cropDefinitionId;
			}

			if (cached != null || definitionId == 0L)
			{
				return cached;
			}

			var resolved = Gameworld.AgricultureCropDefinitions.Get(definitionId);
			lock (_nativeOrganicOwnerSync)
			{
				if (_cropDefinitionId == definitionId && _cropDefinition == null)
				{
					_cropDefinition = resolved;
				}

				return _cropDefinitionId == definitionId ? _cropDefinition : null;
			}
		}
	}

	public IAgricultureWoodlandDefinition CurrentWoodland
	{
		get
		{
			IAgricultureWoodlandDefinition cached;
			long definitionId;
			lock (_nativeOrganicOwnerSync)
			{
				cached = _woodlandDefinition;
				definitionId = _woodlandDefinitionId;
			}

			if (cached != null || definitionId == 0L)
			{
				return cached;
			}

			var resolved = Gameworld.AgricultureWoodlandDefinitions.Get(definitionId);
			lock (_nativeOrganicOwnerSync)
			{
				if (_woodlandDefinitionId == definitionId && _woodlandDefinition == null)
				{
					_woodlandDefinition = resolved;
				}

				return _woodlandDefinitionId == definitionId ? _woodlandDefinition : null;
			}
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
	public int Pasture
	{
		get
		{
			lock (_nativeOrganicOwnerSync)
			{
				return _pasture;
			}
		}
		set
		{
			SynchronizeNativeOrganicOwner(() =>
			{
				if (_pasture == value)
				{
					return;
				}

				_pasture = value;
				if (_nativeOrganicAccountingLoaded)
				{
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Pasture);
				}
				else
				{
					RequestEnvironmentalInputNotificationUnsafe();
				}
			});
		}
	}

	public int Condition
	{
		get => _condition;
		set
		{
			if (_condition == value)
			{
				return;
			}

			_condition = value;
			NotifyEnvironmentalInputsChanged();
		}
	}

	private void NotifyEnvironmentalInputsChanged()
	{
		var notify = false;
		lock (_nativeOrganicOwnerSync)
		{
			RequestEnvironmentalInputNotificationUnsafe();
			notify = TryTakeEnvironmentalInputNotificationUnsafe();
		}

		if (notify)
		{
			DispatchEnvironmentalInputsChanged();
		}
	}

	private void DispatchEnvironmentalInputsChanged()
	{
		Gameworld.EnvironmentalMagic?.MarkDirty(Cell, EnvironmentalMagicDirtyReason.Agriculture);
	}

	private bool TryTakeEnvironmentalInputNotificationUnsafe()
	{
		if (!_environmentalInputNotificationsEnabled || !_environmentalInputChangePending ||
		    _environmentalInputChangeDepth > 0 || _nativeOrganicOwnerSyncDepth > 0)
		{
			return false;
		}

		_environmentalInputChangePending = false;
		return true;
	}

	private EnvironmentalInputChange BeginEnvironmentalInputChange()
	{
		lock (_nativeOrganicOwnerSync)
		{
			_environmentalInputChangeDepth++;
			return new EnvironmentalInputChange(this, CaptureEnvironmentalInputsUnsafe());
		}
	}

	private EnvironmentalInputs CaptureEnvironmentalInputsUnsafe()
	{
		return new EnvironmentalInputs(_profileId, CurrentUse, _cropDefinitionId, _cropHealth,
			_cropYieldPotential, _woodlandDefinitionId, _woodlandHealth, _woodlandYieldPotential, _pasture, _condition,
			_cropNativeAccounting.Revision, _woodlandNativeAccounting.Revision, _pastureNativeAccounting.Revision);
	}

	private void EndEnvironmentalInputChange(EnvironmentalInputs before)
	{
		var notify = false;
		lock (_nativeOrganicOwnerSync)
		{
			if (before != CaptureEnvironmentalInputsUnsafe())
			{
				RequestEnvironmentalInputNotificationUnsafe();
			}

			_environmentalInputChangeDepth--;
			notify = TryTakeEnvironmentalInputNotificationUnsafe();
		}

		if (notify)
		{
			DispatchEnvironmentalInputsChanged();
		}
	}

	private readonly record struct EnvironmentalInputs(long ProfileId, AgricultureFieldUse Use, long CropId,
		int CropHealth, int CropYield, long WoodlandId, int WoodlandHealth, int WoodlandYield, int Pasture, int Condition,
		long CropRevision, long WoodlandRevision, long PastureRevision);

	private readonly struct EnvironmentalInputChange(AgricultureField field, EnvironmentalInputs before) : IDisposable
	{
		private readonly EnvironmentalInputs _before = before;

		public void Dispose()
		{
			field.EndEnvironmentalInputChange(_before);
		}
	}

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

		var apiaryRoot = fieldRoot.Element("Apiary");
		if (apiaryRoot != null)
		{
			_apiary = AgricultureFieldApiary.LoadFromXml(apiaryRoot);
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
				var herdRoot = AgricultureXmlExtensions.RootOrDefault(herd.Definition, "Herd");
				var secondaryYieldPotential = ((int?)herdRoot.Attribute("secondaryYield") ?? 0).ClampScore();
				_herds.Add(new AgricultureFieldHerd(herd.Id, definition, herd.HeadCount, herd.Condition,
					secondaryYieldPotential));
			}
		}

		LoadNativeOrganicAccounting(fieldRoot);
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
		if (score == AgricultureScoreType.Pasture)
		{
			SynchronizeNativeOrganicOwner(() =>
			{
				var value = (_pasture + delta).ClampScore();
				if (_pasture == value)
				{
					return;
				}

				_pasture = value;
				if (_nativeOrganicAccountingLoaded)
				{
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Pasture);
				}
				else
				{
					RequestEnvironmentalInputNotificationUnsafe();
				}
			});
			Changed = true;
			return;
		}

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
				sb.AppendLine($"\t{herd.Definition.Name.ColourName()} - {herd.HeadCount.ToString("N0", voyeur).ColourValue()} head, condition {herd.Condition.ToString("N0", voyeur).ColourValue()}, secondary yield {herd.SecondaryYieldPotential.ToStringN0Colour(voyeur)}");
			}
		}

		if (_apiary != null)
		{
			sb.AppendLine($"Apiary: {_apiary.HiveCount.ToString("N0", voyeur).ColourValue()} hives, colony health {_apiary.ColonyHealth.ToStringN0Colour(voyeur)}, stores {_apiary.Stores.ToStringN0Colour(voyeur)}, yield {_apiary.YieldPotential.ToStringN0Colour(voyeur)}, pollination radius {_apiary.PollinationRadius.ToStringN0Colour(voyeur)}");
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
		using var environmentalChange = BeginEnvironmentalInputChange();
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

		TickApiary();
		TickCrop();
		TickHerds();
		TickWoodland();
	}

	private void TickApiary()
	{
		if (_apiary == null)
		{
			return;
		}

		var temperature = Cell.CurrentTemperature(null);
		var temperate = temperature >= 5.0 && temperature <= 40.0;
		var wellSituated = Condition >= 40 && Pests <= 70 && temperate;
		var healthDelta = wellSituated ? 1 : -3;
		var storesDelta = temperate && _apiary.ColonyHealth >= 40
			? Math.Max(1, _apiary.HiveCount * 2)
			: -2;
		var yieldDelta = IsApiaryHappyForPollination() ? 2 : (_apiary.ColonyHealth < 40 ? -1 : 0);
		_apiary.Adjust(healthDelta, storesDelta, yieldDelta);
		Changed = true;
	}

	private void TickCrop()
	{
		var crop = CurrentCrop;
		if (crop == null)
		{
			return;
		}

		var cropIdentity = SynchronizeNativeOrganicOwner(() =>
			(DefinitionId: _cropDefinitionId, Generation: _cropNativeAccounting.Generation, Stage: CropStage,
				HarvestCount: _cropHarvestCount));
		if (cropIdentity.Stage is AgricultureCropStage.Failed or AgricultureCropStage.Overripe)
		{
			return;
		}

		var harvestDays = crop.IsPerennial && cropIdentity.HarvestCount > 0
			? crop.HarvestCycleDays
			: crop.BaseGrowthDays;
		var harvestWindowDays = crop.HarvestWindowDays;
		var (stressed, pollinationHealth, pollinationYield) = CurrentCropTickContributions(crop,
			cropIdentity.Stage);
		if (stressed)
		{
			SynchronizeNativeOrganicOwner(() =>
			{
				if (_cropDefinitionId != cropIdentity.DefinitionId ||
				    _cropNativeAccounting.Generation != cropIdentity.Generation)
				{
					return;
				}

				var oldHealth = _cropHealth;
				var oldYield = _cropYieldPotential;
				_cropHealth = (_cropHealth - 4).ClampScore();
				_cropYieldPotential = (_cropYieldPotential - 3).ClampScore();
				if (oldHealth != _cropHealth || oldYield != _cropYieldPotential)
				{
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
				}
			});
		}
		else
		{
			var nutrientYield = Math.Sign(Nutrients - 50);
			// Native crop ticks clamp the nutrient step before applying pollination. A later
			// loss cannot create headroom for an earlier positive contribution.
			var healthIncrease = ApplyCropHealthIncrease(1 + Math.Max(0, pollinationHealth));
			var yieldIncrease = ApplyCropYieldIncrease(
				Math.Max(0, nutrientYield) + Math.Max(0, pollinationYield),
				sameOperationLoss: Math.Min(0, nutrientYield));
			SynchronizeNativeOrganicOwner(() =>
			{
				if (_cropDefinitionId != cropIdentity.DefinitionId ||
				    _cropNativeAccounting.Generation != cropIdentity.Generation)
				{
					return;
				}

				var oldHealth = _cropHealth;
				var oldYield = _cropYieldPotential;
				_cropHealth = ((_cropHealth + Math.Min(healthIncrease, 1)).ClampScore() +
				               Math.Min(0, pollinationHealth) + Math.Max(0, healthIncrease - 1)).ClampScore();
				_cropYieldPotential = ((_cropYieldPotential + Math.Min(0, nutrientYield) +
				                        (nutrientYield > 0 ? yieldIncrease : 0)).ClampScore() +
				                       Math.Min(0, pollinationYield) +
				                       (nutrientYield > 0 ? 0 : yieldIncrease)).ClampScore();
				_cropGrowthDays++;
				if (oldHealth != _cropHealth || oldYield != _cropYieldPotential)
				{
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
				}
			});
		}

		AdjustScore(AgricultureScoreType.Nutrients, -1);
		AdjustScore(AgricultureScoreType.Weeds, 1);
		AdjustScore(AgricultureScoreType.Pests, _cropHealth < 40 ? 2 : 1);

		var failed = SynchronizeNativeOrganicOwner(() =>
		{
			if (_cropDefinitionId != cropIdentity.DefinitionId ||
			    _cropNativeAccounting.Generation != cropIdentity.Generation)
			{
				return false;
			}

			var oldStage = CropStage;
			if (_cropHealth <= 0)
			{
				CropStage = AgricultureCropStage.Failed;
				EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
				return true;
			}

			if (_cropGrowthDays >= harvestDays + harvestWindowDays)
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

			if (oldStage != CropStage)
			{
				MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
			}

			return false;
		});
		if (failed)
		{
			Changed = true;
			return;
		}

		Changed = true;
	}

	private (bool Stressed, int PollinationHealth, int PollinationYield) CurrentCropTickContributions(
		IAgricultureCropDefinition crop, AgricultureCropStage stage)
	{
		var temperature = Cell.CurrentTemperature(null);
		var pollinationSupport = CurrentPollinationSupport(crop);
		var lacksRequiredPollination = crop.PollinationDependency == AgriculturePollinationDependency.Required &&
		                                stage == AgricultureCropStage.Setting && pollinationSupport <= 0;
		var stressed = Moisture < crop.MinimumMoisture || Moisture > crop.MaximumMoisture ||
		               temperature < crop.MinimumTemperature || temperature > crop.MaximumTemperature ||
		               Weeds > 75 || Pests > 75 || Salinity > 80 ||
		               crop.ScoreRanges.Any(x => x.Score.IsEnabledScore(Gameworld) && !x.Contains(Score(x.Score))) ||
		               lacksRequiredPollination;
		return (stressed, pollinationSupport > 0 ? crop.PollinationHealthBonus : 0,
			pollinationSupport > 0 ? crop.PollinationYieldBonus : 0);
	}

	private int CurrentPollinationSupport(IAgricultureCropDefinition crop)
	{
		if (crop.PollinationDependency == AgriculturePollinationDependency.None ||
		    CropStage is not (AgricultureCropStage.Growing or AgricultureCropStage.Setting))
		{
			return 0;
		}

		var best = 0;
		var candidates = Gameworld.EnvironmentalMagic is EnvironmentalMagicCoordinator coordinator
			? coordinator.PollinationCandidates()
			: Gameworld.AgricultureFields;
		foreach (var field in candidates)
		{
			if (field?.HasActiveApiary != true || !field.IsApiaryHappy || field.Apiary == null)
			{
				continue;
			}

			var distance = DistanceBetweenCells(Cell, field.Cell, field.Apiary.PollinationRadius);
			if (distance < 0)
			{
				continue;
			}

			best = Math.Max(best, Math.Max(1, field.Apiary.PollinationStrength - distance * 15));
		}

		return best;
	}

	private static int DistanceBetweenCells(ICell origin, ICell destination, int maximumDistance)
	{
		if (origin == null || destination == null)
		{
			return -1;
		}

		if (origin.Id == destination.Id)
		{
			return 0;
		}

		if (maximumDistance <= 0)
		{
			return -1;
		}

		var seen = new HashSet<long> { origin.Id };
		var queue = new Queue<(ICell Cell, int Distance)>();
		queue.Enqueue((origin, 0));
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			if (current.Distance >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in current.Cell.ExitsFor(null, true))
			{
				var next = exit.Destination;
				if (next == null || !seen.Add(next.Id))
				{
					continue;
				}

				var distance = current.Distance + 1;
				if (next.Id == destination.Id)
				{
					return distance;
				}

				queue.Enqueue((next, distance));
			}
		}

		return -1;
	}

	private bool IsApiaryHappyForPollination()
	{
		if (_apiary == null || _apiary.HiveCount <= 0)
		{
			return false;
		}

		var temperature = Cell.CurrentTemperature(null);
		return _apiary.ColonyHealth >= 50 &&
		       _apiary.Stores >= 25 &&
		       Condition >= 40 &&
		       Pests <= 70 &&
		       temperature >= 5.0 &&
		       temperature <= 40.0;
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
				if (herd.HeadCount > 0 && herd.Definition.SecondaryOutputs.Count > 0 && herd.Condition >= 35.0)
				{
					var yieldDelta = Math.Max(1, (int)Math.Ceiling(herd.Condition / 25.0));
					herd.SecondaryYieldPotential = (herd.SecondaryYieldPotential + yieldDelta).ClampScore();
				}
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
				herd.SecondaryYieldPotential = (herd.SecondaryYieldPotential - 8).ClampScore();
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

		var woodlandIdentity = SynchronizeNativeOrganicOwner(() =>
			(DefinitionId: _woodlandDefinitionId, Generation: _woodlandNativeAccounting.Generation,
				GrowthDays: _woodlandGrowthDays, Health: _woodlandHealth));
		var stressed = Moisture < 15 || Moisture > 90 || Topsoil < 25 || Pests > 80;
		var woodlandEstablishmentDays = woodland.EstablishmentDays;
		var healthIncrease = !stressed && woodlandIdentity.Health > 0 ? ApplyWoodlandHealthIncrease(1) : 0;
		var yieldIncrease = !stressed && woodlandIdentity.Health > 0 &&
		                    woodlandIdentity.GrowthDays + 1 > woodlandEstablishmentDays
			? ApplyWoodlandYieldIncrease(1)
			: 0;
		SynchronizeNativeOrganicOwner(() =>
		{
			if (_woodlandDefinitionId != woodlandIdentity.DefinitionId ||
			    _woodlandNativeAccounting.Generation != woodlandIdentity.Generation)
			{
				return;
			}

			var oldHealth = _woodlandHealth;
			var oldYield = _woodlandYieldPotential;
			_woodlandGrowthDays++;
			_woodlandHealth = stressed
				? (_woodlandHealth - 2).ClampScore()
				: (_woodlandHealth + healthIncrease).ClampScore();
			if (_woodlandGrowthDays > woodlandEstablishmentDays && !stressed && _woodlandHealth > 0)
			{
				_woodlandYieldPotential = (_woodlandYieldPotential + yieldIncrease).ClampScore();
			}

			if (_woodlandHealth <= 0)
			{
				EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
			}
			else if (oldHealth != _woodlandHealth || oldYield != _woodlandYieldPotential)
			{
				MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Woodland);
			}
		});

		AdjustScore(AgricultureScoreType.Nutrients, WoodlandGrowthDays % 10 == 0 ? -1 : 0);

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

		using var environmentalChange = BeginEnvironmentalInputChange();
		outcome ??= AgricultureWorkOutcome.Neutral;
		var initialisingPasture = (CurrentUse != AgricultureFieldUse.Pasture || _pendingPastureAssessment) &&
		                         operation.ResultUse == AgricultureFieldUse.Pasture;
		var assessingStagedPasture = initialisingPasture && _pendingPastureAssessment;
		var establishmentIncrease = assessingStagedPasture &&
			operation.ScoreDeltas.TryGetValue(AgricultureScoreType.Pasture, out var pastureDelta) &&
			AgricultureScoreType.Pasture.IsEnabledScore(Gameworld)
			? Math.Max(0, SkillAdjustedScoreDelta(AgricultureScoreType.Pasture, pastureDelta, outcome))
			: 0;
		if (initialisingPasture)
		{
			TransitionNativeOrganicUse(AgricultureFieldUse.Pasture, establishmentIncrease);
		}

		foreach (var delta in operation.ScoreDeltas)
		{
			if (!delta.Key.IsEnabledScore(Gameworld))
			{
				continue;
			}
			if (assessingStagedPasture && delta.Key == AgricultureScoreType.Pasture &&
			    SkillAdjustedScoreDelta(delta.Key, delta.Value, outcome) > 0)
			{
				continue;
			}

			ApplySkillAdjustedScoreDelta(delta.Key, delta.Value, outcome, initialisingPasture);
		}

		switch (operation.OperationType)
		{
			case AgricultureOperationType.Sow:
				var crop = (IAgricultureCropDefinition)target;
				var cropId = crop.Id;
				var baselineCropHealth = Condition.ClampScore();
				var outcomeCropHealth = (Condition + outcome.CropHealthDelta).ClampScore();
				var cropHealthOutcomeDelta = outcomeCropHealth - baselineCropHealth;
				var baselineCropYield = (Condition + Nutrients + Topsoil - Weeds - Pests).ClampScore();
				var outcomeCropYield = (Condition + Nutrients + Topsoil - Weeds - Pests +
				                        outcome.CropYieldDelta).ClampScore();
				var cropYieldOutcomeDelta = outcomeCropYield - baselineCropYield;
				var cropGeneration = SynchronizeNativeOrganicOwner(() =>
				{
					BeginNativeOrganicReplacementUnsafe(NativeOrganicSourceKind.Crop, AgricultureFieldUse.Crop);
					_cropDefinition = crop;
					_cropDefinitionId = cropId;
					CropStage = AgricultureCropStage.Planted;
					_cropGrowthDays = 0;
					_cropHarvestCount = 0;
					_cropHealth = 0;
					_cropYieldPotential = 0;
					CurrentUse = AgricultureFieldUse.Crop;
					return _cropNativeAccounting.Generation;
				});
				var initialCropHealth = (ApplyCropHealthIncrease(
					baselineCropHealth + Math.Max(0, cropHealthOutcomeDelta),
					NativeOrganicPenaltyChannel.CropInitialisation) + Math.Min(0, cropHealthOutcomeDelta)).ClampScore();
				var initialCropYield = initialCropHealth > 0
					? (ApplyCropYieldIncrease(
						baselineCropYield + Math.Max(0, cropYieldOutcomeDelta),
						NativeOrganicPenaltyChannel.CropInitialisation) +
						Math.Min(0, cropYieldOutcomeDelta)).ClampScore()
					: 0;
				SynchronizeNativeOrganicOwner(() =>
				{
					if (_cropDefinitionId != cropId || _cropNativeAccounting.Generation != cropGeneration)
					{
						return;
					}

					_cropHealth = initialCropHealth;
					_cropYieldPotential = initialCropYield;
					if (_cropHealth <= 0)
					{
						CropStage = AgricultureCropStage.Failed;
						EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
					}
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
				});
				result = $"The field has been sown with {crop.Name}.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.PlantOrchard:
				var orchardCrop = (IAgricultureCropDefinition)target;
				var orchardCropId = orchardCrop.Id;
				var baselineOrchardHealth = Condition.ClampScore();
				var outcomeOrchardHealth = (Condition + outcome.CropHealthDelta).ClampScore();
				var orchardHealthOutcomeDelta = outcomeOrchardHealth - baselineOrchardHealth;
				var baselineOrchardYield = ((Condition + Nutrients + Topsoil - Weeds - Pests).ClampScore() / 2)
					.ClampScore();
				var outcomeOrchardYield = ((Condition + Nutrients + Topsoil - Weeds - Pests +
				                           outcome.CropYieldDelta).ClampScore() / 2).ClampScore();
				var orchardYieldOutcomeDelta = outcomeOrchardYield - baselineOrchardYield;
				var orchardGeneration = SynchronizeNativeOrganicOwner(() =>
				{
					BeginNativeOrganicReplacementUnsafe(NativeOrganicSourceKind.Crop, AgricultureFieldUse.Orchard);
					_cropDefinition = orchardCrop;
					_cropDefinitionId = orchardCropId;
					CropStage = AgricultureCropStage.Planted;
					_cropGrowthDays = 0;
					_cropHarvestCount = 0;
					_cropHealth = 0;
					_cropYieldPotential = 0;
					CurrentUse = AgricultureFieldUse.Orchard;
					return _cropNativeAccounting.Generation;
				});
				var initialOrchardHealth = (ApplyCropHealthIncrease(
					baselineOrchardHealth + Math.Max(0, orchardHealthOutcomeDelta),
					NativeOrganicPenaltyChannel.CropInitialisation) +
					Math.Min(0, orchardHealthOutcomeDelta)).ClampScore();
				var initialOrchardYield = initialOrchardHealth > 0
					? (ApplyCropYieldIncrease(
						baselineOrchardYield + Math.Max(0, orchardYieldOutcomeDelta),
						NativeOrganicPenaltyChannel.CropInitialisation) +
						Math.Min(0, orchardYieldOutcomeDelta)).ClampScore()
					: 0;
				SynchronizeNativeOrganicOwner(() =>
				{
					if (_cropDefinitionId != orchardCropId || _cropNativeAccounting.Generation != orchardGeneration)
					{
						return;
					}

					_cropHealth = initialOrchardHealth;
					_cropYieldPotential = initialOrchardYield;
					if (_cropHealth <= 0)
					{
						CropStage = AgricultureCropStage.Failed;
						EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
					}
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
				});
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
					CropStage == AgricultureCropStage.Overripe ? 0.75 : 1.0, outcome, actor);
				result = $"The {cropName} crop is harvested with an estimated yield quality of {_cropYieldPotential.DescribeBand()}.{outcome.DescribeEffect()}{DescribeOutputResult(cropOutputs)}";
				if (perennialHarvest)
				{
					var harvestHealthIncrease = outcome.CropHealthDelta > 0
						? ApplyCropHealthIncrease(outcome.CropHealthDelta)
						: outcome.CropHealthDelta;
					var harvestYieldIncrease = outcome.CropYieldDelta > 0
						? ApplyCropYieldIncrease(outcome.CropYieldDelta, sameOperationLoss: -20)
						: outcome.CropYieldDelta;
					SynchronizeNativeOrganicOwner(() =>
					{
						var oldCropHealth = _cropHealth;
						var oldCropYield = _cropYieldPotential;
						_cropHarvestCount++;
						_cropGrowthDays = 0;
						_cropHealth = (_cropHealth + harvestHealthIncrease).ClampScore();
						_cropYieldPotential = (_cropYieldPotential - 20 + harvestYieldIncrease).ClampScore();
						if (_cropHealth <= 0)
						{
							CropStage = AgricultureCropStage.Failed;
							EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
						}
						else
						{
							CropStage = AgricultureCropStage.Growing;
						}

						CurrentUse = AgricultureFieldUse.Orchard;
						if (oldCropHealth != _cropHealth || oldCropYield != _cropYieldPotential)
						{
							MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
						}
					});
				}
				else
				{
					SynchronizeNativeOrganicOwner(() =>
					{
						ClearCrop();
						CurrentUse = AgricultureFieldUse.Fallow;
					});
				}
				break;
			case AgricultureOperationType.InstallApiary:
				_apiary = new AgricultureFieldApiary(
					Math.Max(1, operation.ApiaryInstallHiveCount),
					(Condition + outcome.CropHealthDelta).ClampScore(),
					(35 + outcome.CropYieldDelta).ClampScore(),
					(40 + outcome.CropYieldDelta).ClampScore(),
					operation.ApiaryPollinationRadius <= 0 ? 2 : operation.ApiaryPollinationRadius);
				result = $"The field has been fitted with an apiary installation of {_apiary.HiveCount.ToString("N0", actor)} hives.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.TendApiary:
				_apiary.Adjust(operation.ApiaryTendHealthDelta + outcome.CropHealthDelta,
					operation.ApiaryTendStoresDelta,
					operation.ApiaryTendYieldDelta + outcome.CropYieldDelta);
				result = $"The apiary has been tended. Colony health is now {_apiary.ColonyHealth.DescribeBand()}, stores are {_apiary.Stores.DescribeBand()}, and yield potential is {_apiary.YieldPotential.DescribeBand()}.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.HarvestApiary:
				var apiaryOutputs = ReleaseCommodityOutputs(operation.ApiaryYieldOutputs, _apiary.ColonyHealth,
					Math.Min(_apiary.Stores, _apiary.YieldPotential), operation.ApiaryYieldMultiplier, outcome, actor);
				_apiary.Adjust(0, -operation.ApiaryYieldCost, -operation.ApiaryYieldCost);
				result = $"The apiary is harvested with an estimated stores quality of {_apiary.Stores.DescribeBand()}.{outcome.DescribeEffect()}{DescribeOutputResult(apiaryOutputs)}";
				break;
			case AgricultureOperationType.RemoveApiary:
				_apiary = null;
				result = $"The apiary installation has been removed from the field.{outcome.DescribeEffect()}";
				break;
			case AgricultureOperationType.HarvestHerdProducts:
				var herdTarget = (IAgricultureHerdDefinition)target;
				var herdOutputs = ReleaseHerdOutputs(operation, herdTarget, outcome, actor);
				var harvestedHerd = _herds.First(x => x.Definition.Id == herdTarget.Id);
				result = $"The {harvestedHerd.Definition.Name} herd products are collected with an estimated condition of {((int)Math.Round(harvestedHerd.Condition)).DescribeBand()}.{outcome.DescribeEffect()}{DescribeOutputResult(herdOutputs)}";
				break;
			case AgricultureOperationType.Graze:
			case AgricultureOperationType.Herd:
				TransitionNativeOrganicUse(AgricultureFieldUse.Pasture);
				if (target is IAgricultureHerdDefinition herdDefinition && _herds.All(x => x.Definition.Id != herdDefinition.Id))
				{
					_herds.Add(new AgricultureFieldHerd(0, herdDefinition, 0, Condition));
				}

				result = "The field is now being managed as pasture.";
				break;
			case AgricultureOperationType.Woodland:
				var woodland = (IAgricultureWoodlandDefinition)target;
				var woodlandId = woodland.Id;
				var initialWoodlandHealth = Condition;
				var woodlandGeneration = SynchronizeNativeOrganicOwner(() =>
				{
					BeginNativeOrganicReplacementUnsafe(NativeOrganicSourceKind.Woodland,
						AgricultureFieldUse.Woodland);
					_woodlandDefinition = woodland;
					_woodlandDefinitionId = woodlandId;
					_woodlandGrowthDays = 0;
					_woodlandHealth = 0;
					_woodlandYieldPotential = 0;
					CurrentUse = AgricultureFieldUse.Woodland;
					return _woodlandNativeAccounting.Generation;
				});
				var establishedWoodlandHealth = ApplyWoodlandHealthIncrease(initialWoodlandHealth,
					NativeOrganicPenaltyChannel.WoodlandInitialisation).ClampScore();
				SynchronizeNativeOrganicOwner(() =>
				{
					if (_woodlandDefinitionId != woodlandId ||
					    _woodlandNativeAccounting.Generation != woodlandGeneration)
					{
						return;
					}

					_woodlandHealth = establishedWoodlandHealth;
					if (_woodlandHealth <= 0)
					{
						EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
					}
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Woodland);
				});
				result = $"The field is now being managed as {woodland.Name}.";
				break;
			case AgricultureOperationType.Clear:
				var clearOutputs = ReleaseWoodlandOutputs(operation, outcome, actor);
				SynchronizeNativeOrganicOwner(() =>
				{
					ClearCrop();
					ClearWoodland();
					_herds.Clear();
					if (CurrentUse == AgricultureFieldUse.Pasture)
					{
						EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Pasture);
					}

					CurrentUse = AgricultureFieldUse.Fallow;
				});
				result = $"The field has been cleared back to fallow land.{outcome.DescribeEffect()}{DescribeOutputResult(clearOutputs)}";
				break;
			default:
				TransitionNativeOrganicUse(operation.ResultUse);
				var woodlandOutputs = ReleaseWoodlandOutputs(operation, outcome, actor);
				result = $"The {operation.Name} operation has been applied to the field.{outcome.DescribeEffect()}{DescribeOutputResult(woodlandOutputs)}";
				break;
		}

		RunCompletionProg(operation, actor);
		if (operation.OperationType is AgricultureOperationType.InstallApiary or
		    AgricultureOperationType.RemoveApiary)
		{
			Gameworld.EnvironmentalMagic?.RefreshPollinationCandidate(this);
		}
		Changed = true;
		return true;
	}

	private void ApplySkillAdjustedScoreDelta(AgricultureScoreType score, int delta, AgricultureWorkOutcome outcome,
		bool initialisingPasture)
	{
		if (delta == 0)
		{
			return;
		}

		var adjusted = SkillAdjustedScoreDelta(score, delta, outcome);

		if (score == AgricultureScoreType.Pasture && adjusted > 0 && CurrentUse == AgricultureFieldUse.Pasture)
		{
			var increase = ApplyPastureIncrease(adjusted, initialisingPasture
				? NativeOrganicPenaltyChannel.PastureInitialisation
				: NativeOrganicPenaltyChannel.PastureRecovery);
			if (increase > 0)
			{
				AdjustScore(score, increase);
			}

			return;
		}

		AdjustScore(score, adjusted);
	}

	private int SkillAdjustedScoreDelta(AgricultureScoreType score, int delta, AgricultureWorkOutcome outcome)
	{
		if (delta == 0)
		{
			return 0;
		}
		var multiplier = IsBeneficialDelta(score, delta)
			? outcome.BeneficialScoreMultiplier
			: outcome.HarmfulScoreMultiplier;
		var adjusted = (int)Math.Round(delta * multiplier);
		return adjusted == 0 ? Math.Sign(delta) : adjusted;
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

		if (operation.OperationType == AgricultureOperationType.HarvestApiary &&
		    (_apiary == null || _apiary.Stores <= 0 || _apiary.YieldPotential <= 0))
		{
			return "The apiary does not have harvestable stores.";
		}

		if (operation.OperationType == AgricultureOperationType.HarvestHerdProducts &&
		    target is IAgricultureHerdDefinition herdDefinition)
		{
			var herd = _herds.FirstOrDefault(x => x.Definition.Id == herdDefinition.Id);
			if (herd == null || herd.SecondaryYieldPotential <= 0)
			{
				return "That herd does not have any secondary products ready to collect.";
			}
		}

		return string.Empty;
	}

	private void RunCompletionProg(IAgricultureOperation operation, ICharacter actor)
	{
		var output = operation.CompletionProg?.Execute(this, actor);
		foreach (var item in CompletionOutputItems(output).Distinct())
		{
			item.SetOwner(actor);
			ReleaseCompletionOutputToField(item);
		}
	}

	private IReadOnlyList<IGameItem> ReleaseWoodlandOutputs(IAgricultureOperation operation,
		AgricultureWorkOutcome outcome, ICharacter owner)
	{
		if (operation.WoodlandYieldMultiplier <= 0.0 || CurrentWoodland == null)
		{
			return Array.Empty<IGameItem>();
		}

		var outputs = ReleaseCommodityOutputs(CurrentWoodland.YieldOutputs, _woodlandHealth, _woodlandYieldPotential,
			operation.WoodlandYieldMultiplier, outcome, owner);
		var woodlandYieldCost = operation.WoodlandYieldCost;
		if (woodlandYieldCost > 0)
		{
			SynchronizeNativeOrganicOwner(() =>
			{
				var oldYield = _woodlandYieldPotential;
				_woodlandYieldPotential = (_woodlandYieldPotential -
				                            Math.Min(_woodlandYieldPotential, woodlandYieldCost)).ClampScore();
				if (oldYield != _woodlandYieldPotential)
				{
					MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Woodland);
				}
			});
		}

		return outputs;
	}

	private IReadOnlyList<IGameItem> ReleaseHerdOutputs(IAgricultureOperation operation,
		IAgricultureHerdDefinition definition, AgricultureWorkOutcome outcome, ICharacter owner)
	{
		if (operation.HerdYieldMultiplier <= 0.0 || definition == null)
		{
			return Array.Empty<IGameItem>();
		}

		var herd = _herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (herd == null || herd.HeadCount <= 0 || herd.SecondaryYieldPotential <= 0)
		{
			return Array.Empty<IGameItem>();
		}

		var perHerdOutputs = herd.Definition.SecondaryOutputs
		                         .Select(x => new AgricultureCommodityYield(x.MaterialName,
			                         x.BaseWeight * herd.HeadCount, x.TagName))
		                         .ToList();
		var outputs = ReleaseCommodityOutputs(perHerdOutputs, (int)Math.Round(herd.Condition),
			herd.SecondaryYieldPotential, operation.HerdYieldMultiplier, outcome, owner);
		if (operation.HerdYieldCost > 0)
		{
			herd.SecondaryYieldPotential = (herd.SecondaryYieldPotential -
			                                Math.Min(herd.SecondaryYieldPotential, operation.HerdYieldCost)).ClampScore();
		}

		return outputs;
	}

	private IReadOnlyList<IGameItem> ReleaseCommodityOutputs(IEnumerable<AgricultureCommodityYield> outputs, int health,
		int yieldPotential, double multiplier, AgricultureWorkOutcome outcome, ICharacter owner)
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
			item.SetOwner(owner);
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
		SynchronizeNativeOrganicOwner(() =>
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
			_cropDefinition = null;
			_cropDefinitionId = 0;
			CropStage = AgricultureCropStage.None;
			_cropGrowthDays = 0;
			_cropHarvestCount = 0;
			_cropHealth = 0;
			_cropYieldPotential = 0;
		});
	}

	private void ClearWoodland()
	{
		SynchronizeNativeOrganicOwner(() =>
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
			_woodlandDefinition = null;
			_woodlandDefinitionId = 0;
			_woodlandGrowthDays = 0;
			_woodlandHealth = 0;
			_woodlandYieldPotential = 0;
		});
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

		using var environmentalChange = BeginEnvironmentalInputChange();
		var consumeResult = SynchronizeNativeOrganicOwner(() =>
		{
			if (_cropDefinitionId <= 0L)
			{
				return (false, "There is no crop in this field.");
			}

			if (_cropYieldPotential < amount)
			{
				return (false, "The crop does not have enough remaining yield.");
			}

			_cropYieldPotential = (_cropYieldPotential - amount).ClampScore();
			MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Crop);
			return (true, string.Empty);
		});
		reason = consumeResult.Item2;
		return consumeResult.Item1;
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

		using var environmentalChange = BeginEnvironmentalInputChange();
		var consumeResult = SynchronizeNativeOrganicOwner(() =>
		{
			if (_woodlandDefinitionId <= 0L)
			{
				return (false, "There is no woodland in this field.");
			}

			if (_woodlandYieldPotential < amount)
			{
				return (false, "The woodland does not have enough remaining yield.");
			}

			_woodlandYieldPotential = (_woodlandYieldPotential - amount).ClampScore();
			MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Woodland);
			return (true, string.Empty);
		});
		reason = consumeResult.Item2;
		return consumeResult.Item1;
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
			var actorLocation = RouteSpatialService.Instance.GetEffectiveLocation(actor);
			var spawnLocation = ReferenceEquals(actorLocation.Cell, Cell)
				? actorLocation
				: CharacterInstanceService.CreateDefaultSpawnLocation(Cell, RoomLayer.GroundLevel);
            var npc = definition.NpcTemplate.CreateNewCharacter(spawnLocation);
            Gameworld.Add(npc, true);
            definition.NpcTemplate.ApplyTemplateLoadAdditions(npc);
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
		if (npc == null || npc.IsPlayerCharacter || npc.Location != Cell || !AnimalLineageHelper.IsAnimal(npc))
		{
			result = "You can only absorb a non-player animal in this field.";
			return false;
		}

		if (definition.NpcTemplate != null && npc is INPC concreteNpc &&
		    concreteNpc.Template?.Id != definition.NpcTemplate.Id)
		{
			result = $"You can only absorb animals that match the {definition.Name} herd definition.";
			return false;
		}

		using var environmentalChange = BeginEnvironmentalInputChange();
		var herd = _herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (herd == null)
		{
			herd = new AgricultureFieldHerd(0, definition, 0, Condition);
			_herds.Add(herd);
		}

		npc.Quit(silent: true);
		herd.HeadCount++;
		herd.Condition = Math.Min(definition.MaximumCondition, (herd.Condition * (herd.HeadCount - 1) + Condition) / herd.HeadCount);
		TransitionNativeOrganicUse(AgricultureFieldUse.Pasture);
		Changed = true;
		result = $"You add {npc.HowSeen(actor)} into the {definition.Name} herd.";
		return true;
	}

	public bool DriveHerdTo(IAgricultureField destinationField, IAgricultureHerdDefinition definition, int count,
		ICharacter actor, out string result)
	{
		if (destinationField is not AgricultureField destination)
		{
			result = "There is no destination agriculture field.";
			return false;
		}

		if (destination.Id == Id)
		{
			result = "The herd is already in this field.";
			return false;
		}

		if (!CanActorUseField(actor, this, "drive herds from", out result) ||
		    !CanActorUseField(actor, destination, "drive herds into", out result))
		{
			return false;
		}

		if (!destination.Profile.AllowsUse(AgricultureFieldUse.Pasture))
		{
			result = $"The {destination.Profile.Name} profile does not support pasture use.";
			return false;
		}

		if (destination.CurrentUse is not (AgricultureFieldUse.Fallow or AgricultureFieldUse.Pasture))
		{
			result = "You can only drive herds into fallow or pasture fields.";
			return false;
		}

		var sourceHerd = _herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (sourceHerd == null || sourceHerd.HeadCount <= 0)
		{
			result = "There are no animals of that herd in this field.";
			return false;
		}

		var moveCount = count <= 0 ? sourceHerd.HeadCount : count;
		if (moveCount > sourceHerd.HeadCount)
		{
			result = "There are not enough animals in that herd.";
			return false;
		}

		using var destinationEnvironmentalChange = destination.BeginEnvironmentalInputChange();
		var destinationHerd = destination._herds.FirstOrDefault(x => x.Definition.Id == definition.Id);
		if (destinationHerd == null)
		{
			destinationHerd = new AgricultureFieldHerd(0, definition, 0, destination.Condition);
			destination._herds.Add(destinationHerd);
		}

		destinationHerd.Condition = Math.Min(definition.MaximumCondition,
			(destinationHerd.Condition * destinationHerd.HeadCount + sourceHerd.Condition * moveCount) /
			(destinationHerd.HeadCount + moveCount));
		destinationHerd.HeadCount += moveCount;
		sourceHerd.HeadCount -= moveCount;
		if (sourceHerd.HeadCount <= 0)
		{
			_herds.Remove(sourceHerd);
		}

		destination.TransitionNativeOrganicUse(AgricultureFieldUse.Pasture);
		Changed = true;
		destination.Changed = true;
		var destinationName = actor == null ? destination.Cell.Name : destination.Cell.GetFriendlyReference(actor);
		result = $"You drive {moveCount.ToString("N0", actor)} {definition.Name} to {destinationName}.";
		return true;
	}

	private bool CanActorUseField(ICharacter actor, IAgricultureField field, string action, out string reason)
	{
		var property = Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(field.Cell));
		if (property != null &&
		    (actor == null || !actor.IsAdministrator() && !property.IsAuthorisedOwner(actor) && !property.IsAuthorisedLeaseHolder(actor)))
		{
			reason = $"You can only {action} unowned fields or fields you are authorised to use.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureFields
		                 .Include(x => x.AgricultureFieldCrop)
		                 .Include(x => x.AgricultureFieldHerds)
		                 .Include(x => x.AgricultureFieldWoodland)
		                 .First(x => x.Id == Id);
		var native = SynchronizeNativeOrganicOwner(() =>
		{
			var snapshot = (Use: CurrentUse, Pasture: _pasture, CropDefinitionId: _cropDefinitionId, CropStage,
				CropGrowthDays: _cropGrowthDays, CropHarvestCount: _cropHarvestCount, CropHealth: _cropHealth,
				CropYield: _cropYieldPotential, WoodlandDefinitionId: _woodlandDefinitionId,
				WoodlandGrowthDays: _woodlandGrowthDays, WoodlandHealth: _woodlandHealth,
				WoodlandYield: _woodlandYieldPotential, Definition: SaveFieldDefinitionUnsafe());
			// Clear the staged revision under the owner gate. A later debit must requeue this field,
			// rather than having its dirty mark erased after the EF rows are assembled.
			Changed = false;
			return snapshot;
		});
		dbitem.ProfileId = Profile.Id;
		dbitem.CurrentUse = (int)native.Use;
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
		dbitem.Pasture = native.Pasture;
		dbitem.Condition = Condition;
		dbitem.Definition = native.Definition.ToString();

		if (native.CropDefinitionId > 0L)
		{
			var crop = dbitem.AgricultureFieldCrop ?? new Models.AgricultureFieldCrop { AgricultureFieldId = Id };
			crop.CropDefinitionId = native.CropDefinitionId;
			crop.Stage = (int)native.CropStage;
			crop.GrowthDays = native.CropGrowthDays;
			crop.Health = native.CropHealth;
			crop.YieldPotential = native.CropYield;
			crop.Definition = new XElement("Crop",
				new XAttribute("harvestCount", native.CropHarvestCount)).ToString();
			dbitem.AgricultureFieldCrop = crop;
		}
		else if (dbitem.AgricultureFieldCrop != null)
		{
			FMDB.Context.AgricultureFieldCrops.Remove(dbitem.AgricultureFieldCrop);
			dbitem.AgricultureFieldCrop = null;
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
				Definition = new XElement("Herd",
					new XAttribute("secondaryYield", herd.SecondaryYieldPotential)).ToString()
			});
		}

		if (native.WoodlandDefinitionId > 0L)
		{
			var woodland = dbitem.AgricultureFieldWoodland ?? new Models.AgricultureFieldWoodland { AgricultureFieldId = Id };
			woodland.WoodlandDefinitionId = native.WoodlandDefinitionId;
			woodland.GrowthDays = native.WoodlandGrowthDays;
			woodland.Health = native.WoodlandHealth;
			woodland.YieldPotential = native.WoodlandYield;
			woodland.Definition = "<Woodland />";
			dbitem.AgricultureFieldWoodland = woodland;
		}
		else if (dbitem.AgricultureFieldWoodland != null)
		{
			FMDB.Context.AgricultureFieldWoodlands.Remove(dbitem.AgricultureFieldWoodland);
			dbitem.AgricultureFieldWoodland = null;
		}

	}

	private XElement SaveFieldDefinition()
	{
		return SynchronizeNativeOrganicOwner(SaveFieldDefinitionUnsafe);
	}

	private XElement SaveFieldDefinitionUnsafe()
	{
		return new XElement("Field",
			_apiary?.SaveToXml(),
			SaveNativeOrganicAccountingUnsafe(),
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
			"profiledefinition" => Profile is IProgVariable profile
				? profile
				: new NullVariable(ProgVariableTypes.AgricultureFieldProfile),
			"use" => new TextVariable(CurrentUse.DescribeEnum()),
			"crop" => new TextVariable(CurrentCrop?.Name ?? string.Empty),
			"cropdefinition" => CurrentCrop is IProgVariable crop
				? crop
				: new NullVariable(ProgVariableTypes.AgricultureCropDefinition),
			"cropstage" => new TextVariable(CropStage.DescribeEnum()),
			"cropharvests" => new NumberVariable(_cropHarvestCount),
			"crophealth" => new NumberVariable(_cropHealth),
			"cropyield" => new NumberVariable(_cropYieldPotential),
			"woodland" => new TextVariable(CurrentWoodland?.Name ?? string.Empty),
			"woodlanddefinition" => CurrentWoodland is IProgVariable woodland
				? woodland
				: new NullVariable(ProgVariableTypes.AgricultureWoodlandDefinition),
			"woodlandhealth" => new NumberVariable(_woodlandHealth),
			"woodlandyield" => new NumberVariable(_woodlandYieldPotential),
			"hasapiary" => new BooleanVariable(HasActiveApiary),
			"apiaryhappy" => new BooleanVariable(IsApiaryHappy),
			"apiaryhives" => new NumberVariable(_apiary?.HiveCount ?? 0),
			"apiaryhealth" => new NumberVariable(_apiary?.ColonyHealth ?? 0),
			"apiarystores" => new NumberVariable(_apiary?.Stores ?? 0),
			"apiaryyield" => new NumberVariable(_apiary?.YieldPotential ?? 0),
			"pollinationstrength" => new NumberVariable(PollinationStrength),
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
			{ "profiledefinition", ProgVariableTypes.AgricultureFieldProfile },
			{ "use", ProgVariableTypes.Text },
			{ "crop", ProgVariableTypes.Text },
			{ "cropdefinition", ProgVariableTypes.AgricultureCropDefinition },
			{ "cropstage", ProgVariableTypes.Text },
			{ "cropharvests", ProgVariableTypes.Number },
			{ "crophealth", ProgVariableTypes.Number },
			{ "cropyield", ProgVariableTypes.Number },
			{ "woodland", ProgVariableTypes.Text },
			{ "woodlanddefinition", ProgVariableTypes.AgricultureWoodlandDefinition },
			{ "woodlandhealth", ProgVariableTypes.Number },
			{ "woodlandyield", ProgVariableTypes.Number },
			{ "hasapiary", ProgVariableTypes.Boolean },
			{ "apiaryhappy", ProgVariableTypes.Boolean },
			{ "apiaryhives", ProgVariableTypes.Number },
			{ "apiaryhealth", ProgVariableTypes.Number },
			{ "apiarystores", ProgVariableTypes.Number },
			{ "apiaryyield", ProgVariableTypes.Number },
			{ "pollinationstrength", ProgVariableTypes.Number },
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
