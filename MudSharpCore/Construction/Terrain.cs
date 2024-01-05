using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction;

public class Terrain : SaveableItem, ITerrain
{
	private readonly List<IRangedCover> _terrainCovers = new();
	private IForagableProfile _foragableProfile;

	private long _foragableProfileId;
	public string TerrainBehaviourString { get; private set; }

	public Terrain(Models.Terrain terrain, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDB(terrain);
	}

	protected void LoadFromDB(Models.Terrain terrain)
	{
		_name = terrain.Name.Trim();
		_id = terrain.Id;
		MovementRate = terrain.MovementRate;
		DefaultTerrain = terrain.DefaultTerrain;
		HideDifficulty = (Difficulty)terrain.HideDifficulty;
		SpotDifficulty = (Difficulty)terrain.SpotDifficulty;
		StaminaCost = terrain.StaminaCost;
		_foragableProfileId = terrain.ForagableProfileId;
		InfectionMultiplier = terrain.InfectionMultiplier;
		InfectionVirulence = (Difficulty)terrain.InfectionVirulence;
		PrimaryInfection = (InfectionType)terrain.InfectionType;
		DefaultCellOutdoorsType = (CellOutdoorsType)terrain.DefaultCellOutdoorsType;
		TerrainEditorText = terrain.TerrainEditorText;
		TerrainEditorColour = terrain.TerrainEditorColour;
		TerrainANSIColour = terrain.TerrainANSIColour;
		if (terrain.AtmosphereId != null)
		{
			Atmosphere = terrain.AtmosphereType.Equals("gas", StringComparison.InvariantCultureIgnoreCase)
				? (IFluid)Gameworld.Gases.Get(terrain.AtmosphereId.Value)
				: Gameworld.Liquids.Get(terrain.AtmosphereId.Value);
		}

		foreach (var cover in terrain.TerrainsRangedCovers)
		{
			_terrainCovers.Add(Gameworld.RangedCovers.Get(cover.RangedCoverId));
		}

		_overrideWeatherControllerId = terrain.WeatherControllerId;
		TerrainBehaviourString = terrain.TerrainBehaviourMode;
		var ss = new StringStack(terrain.TerrainBehaviourMode);
		switch (ss.Pop().ToLowerInvariant())
		{
			case "outdoors":
				SetAsOutdoorsTerrain();
				break;
			case "trees":
				SetAsTreesTerrain();
				break;
			case "talltrees":
				SetAsTallTreesTerrain();
				break;
			case "cavetrees":
				SetAsUndergroundTreesTerrain();
				break;
			case "shallowwater":
			case "water":
				SetAsShallowWaterTerrain(ss);
				break;
			case "shallowwatercave":
				SetAsShallowWaterCaveTerrain(ss);
				break;
			case "shallowwatertrees":
				SetAsShallowWaterTreesTerrain(ss);
				break;
			case "deepwater":
				SetAsDeepWaterTerrain(ss);
				break;
			case "verydeepwater":
				SetAsVeryDeepWaterTerrain(ss);
				break;
			case "deepwatercave":
				SetAsDeepWaterCaveTerrain(ss);
				break;
			case "verydeepwatercave":
				SetAsVeryDeepWaterCaveTerrain(ss);
				break;
			case "rooftops":
				SetAsRooftopsTerrain();
				break;
			case "underwater":
				SetAsUnderwater(ss);
				break;
			case "deepunderwater":
				SetAsDeepUnderwater(ss);
				break;
			case "verydeepunderwater":
				SetAsVeryDeepUnderWater(ss);
				break;
			case "cave":
				SetAsCaveTerrain();
				break;
			case "cliff":
				SetAsCliffTerrain();
				break;
			default:
				SetAsIndoors();
				break;
		}
	}

	private void SetAsCliffTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.InAir);
		TerrainBehaviourString = "cliff";
	}

	private void SetAsCaveTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		TerrainBehaviourString = "cave";
	}

	private void SetAsShallowWaterTreesTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InTrees);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.Underwater);
		TerrainBehaviourString = $"shallowwatertrees {fluid.Id}";
	}

	private void SetAsVeryDeepUnderWater(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		_terrainLayers.Add(RoomLayer.VeryDeepUnderwater);
		TerrainBehaviourString = $"verydeepunderwater {fluid.Id}";
	}

	private void SetAsVeryDeepWaterCaveTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		_terrainLayers.Add(RoomLayer.VeryDeepUnderwater);
		TerrainBehaviourString = $"verydeepwatercave {fluid.Id}";
	}

	private void SetAsVeryDeepWaterTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		_terrainLayers.Add(RoomLayer.VeryDeepUnderwater);
		TerrainBehaviourString = $"verydeepwater {fluid.Id}";
	}

	private void SetAsIndoors()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		TerrainBehaviourString = "indoors";
	}

	private void SetAsUnderwater(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.Underwater);
		TerrainBehaviourString = $"underwater {fluid.Id}";
	}

	private void SetAsDeepUnderwater(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		TerrainBehaviourString = $"deepunderwater {fluid.Id}";
	}

	private void SetAsRooftopsTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.OnRooftops);
		TerrainBehaviourString = $"rooftops";
	}

	private void SetAsOutdoorsTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		TerrainBehaviourString = $"outdoors";
	}

	private void SetAsTreesTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.InTrees);
		TerrainBehaviourString = "trees";
	}

	private void SetAsTallTreesTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.InTrees);
		_terrainLayers.Add(RoomLayer.HighInTrees);
		TerrainBehaviourString = "talltrees";
	}

	private void SetAsUndergroundTreesTerrain()
	{
		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InTrees);
		TerrainBehaviourString = "cavetrees";
	}

	private void SetAsShallowWaterTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.Underwater);
		TerrainBehaviourString = $"shallowwater {fluid.Id}";
	}

	private void SetAsDeepWaterTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.InAir);
		_terrainLayers.Add(RoomLayer.HighInAir);
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		TerrainBehaviourString = $"deepwater {fluid.Id}";
	}

	private void SetAsShallowWaterCaveTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.Underwater);
		TerrainBehaviourString = $"shallowwatercave {fluid.Id}";
	}

	private void SetAsDeepWaterCaveTerrain(StringStack ss)
	{
		var fluid = long.TryParse(ss.PopSpeech(), out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(ss.Last);
		if (fluid == null)
		{
			return;
		}

		WaterFluid = fluid;

		_terrainLayers.Clear();
		_terrainLayers.Add(RoomLayer.GroundLevel);
		_terrainLayers.Add(RoomLayer.Underwater);
		_terrainLayers.Add(RoomLayer.DeepUnderwater);
		TerrainBehaviourString = $"deepwatercave {fluid.Id}";
	}

	public Terrain(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Terrain();
			FMDB.Context.Terrains.Add(dbitem);

			dbitem.Name = name;
			dbitem.AtmosphereId = Gameworld.Gases.Get(Gameworld.GetStaticLong("DefaultAtmosphereId"))?.Id;
			dbitem.AtmosphereType = "gas";
			dbitem.DefaultTerrain = false;
			dbitem.HideDifficulty = (int)Difficulty.Normal;
			dbitem.SpotDifficulty = (int)Difficulty.Automatic;
			dbitem.StaminaCost = Gameworld.GetStaticDouble("DefaultTerrainStaminaCost");
			dbitem.InfectionMultiplier = 1.0;
			dbitem.InfectionVirulence = (int)Difficulty.Normal;
			dbitem.InfectionType = (int)InfectionType.Simple;
			dbitem.TerrainBehaviourMode = "Standard";
			dbitem.TerrainEditorColour = "#FF7CFC00";
			dbitem.TerrainEditorText = null;
			dbitem.DefaultCellOutdoorsType = (int)CellOutdoorsType.Outdoors;

			FMDB.Context.SaveChanges();
			LoadFromDB(dbitem);
		}
	}

	public Terrain(ITerrain rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.Terrain();
			FMDB.Context.Terrains.Add(dbitem);

			var rhsItem = FMDB.Context.Terrains.Find(rhs.Id);

			dbitem.Name = name;
			dbitem.AtmosphereId = rhs.Atmosphere?.Id;
			dbitem.AtmosphereType = rhs.Atmosphere is IGas ? "gas" : "liquid";
			dbitem.DefaultTerrain = false;
			dbitem.HideDifficulty = (int)rhs.HideDifficulty;
			dbitem.SpotDifficulty = (int)rhs.SpotDifficulty;
			dbitem.StaminaCost = rhs.StaminaCost;
			dbitem.InfectionMultiplier = rhs.InfectionMultiplier;
			dbitem.InfectionVirulence = (int)rhs.InfectionVirulence;
			dbitem.InfectionType = (int)rhs.PrimaryInfection;
			dbitem.WeatherControllerId = rhs.OverrideWeatherController?.Id;
			dbitem.ForagableProfileId = rhs.ForagableProfile?.Id ?? 0;
			dbitem.TerrainBehaviourMode = rhsItem.TerrainBehaviourMode;
			dbitem.TerrainEditorColour = rhsItem.TerrainEditorColour;
			dbitem.DefaultCellOutdoorsType = rhsItem.DefaultCellOutdoorsType;
			dbitem.TerrainEditorText = rhsItem.TerrainEditorText;
			dbitem.TerrainANSIColour = rhsItem.TerrainANSIColour;
			foreach (var cover in rhs.TerrainCovers)
			{
				dbitem.TerrainsRangedCovers.Add(new TerrainsRangedCovers
					{ Terrain = dbitem, RangedCoverId = cover.Id });
			}

			FMDB.Context.SaveChanges();
			LoadFromDB(dbitem);
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Terrains.Find(Id);
		dbitem.Name = Name;
		dbitem.AtmosphereId = Atmosphere?.Id;
		dbitem.AtmosphereType = Atmosphere is IGas ? "gas" : "liquid";
		dbitem.DefaultTerrain = DefaultTerrain;
		dbitem.HideDifficulty = (int)HideDifficulty;
		dbitem.SpotDifficulty = (int)SpotDifficulty;
		dbitem.StaminaCost = StaminaCost;
		dbitem.InfectionMultiplier = InfectionMultiplier;
		dbitem.InfectionVirulence = (int)InfectionVirulence;
		dbitem.InfectionType = (int)PrimaryInfection;
		dbitem.WeatherControllerId = _overrideWeatherControllerId;
		dbitem.ForagableProfileId = ForagableProfile?.Id ?? 0;
		dbitem.TerrainBehaviourMode = TerrainBehaviourString;
		dbitem.TerrainANSIColour = TerrainANSIColour;
		dbitem.DefaultCellOutdoorsType = (int)DefaultCellOutdoorsType;
		FMDB.Context.TerrainsRangedCovers.RemoveRange(dbitem.TerrainsRangedCovers);
		foreach (var cover in _terrainCovers)
		{
			dbitem.TerrainsRangedCovers.Add(new TerrainsRangedCovers { Terrain = dbitem, RangedCoverId = cover.Id });
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Terrain";

	public CellOutdoorsType DefaultCellOutdoorsType { get; private set; }

	public double MovementRate { get; private set; }

	public double StaminaCost { get; private set; }

	public double InfectionMultiplier { get; private set; }

	public InfectionType PrimaryInfection { get; private set; }

	public Difficulty InfectionVirulence { get; private set; }

	public IForagableProfile ForagableProfile
	{
		get
		{
			if (_foragableProfileId != 0)
			{
				_foragableProfile = Gameworld.ForagableProfiles.Get(_foragableProfileId);
				_foragableProfileId = 0;
			}

			return _foragableProfile;
		}
		private set
		{
			_foragableProfile = value;
			_foragableProfileId = 0;
		}
	}

	public bool DefaultTerrain { get; set; }

	public IFluid Atmosphere { get; set; }

	/// <summary>
	///     Base difficulty of making a hide check in this terrain
	/// </summary>
	public Difficulty HideDifficulty { get; private set; }

	/// <summary>
	///     Base difficulty of making a spot check in this terrain
	/// </summary>
	public Difficulty SpotDifficulty { get; private set; }

	private long? _overrideWeatherControllerId;
	private IWeatherController _overrideWeatherController;

	public IWeatherController OverrideWeatherController
	{
		get
		{
			if (_overrideWeatherController == null && _overrideWeatherControllerId.HasValue)
			{
				_overrideWeatherController = Gameworld.WeatherControllers.Get(_overrideWeatherControllerId.Value);
			}

			return _overrideWeatherController;
		}
	}

	public IEnumerable<IRangedCover> TerrainCovers => _terrainCovers;

	private readonly List<RoomLayer> _terrainLayers = new();
	public IEnumerable<RoomLayer> TerrainLayers => _terrainLayers;

	public string RoomNameForLayer(string baseRoomName, RoomLayer layer)
	{
		switch (layer)
		{
			case RoomLayer.GroundLevel:
				return baseRoomName;
			case RoomLayer.Underwater:
				return string.Format(Gameworld.GetStaticString("RoomNameUnderwater"), baseRoomName);
			case RoomLayer.DeepUnderwater:
				return string.Format(Gameworld.GetStaticString("RoomNameDeepUnderwater"), baseRoomName);
			case RoomLayer.VeryDeepUnderwater:
				return string.Format(Gameworld.GetStaticString("RoomNameVeryDeepUnderwater"), baseRoomName);
			case RoomLayer.InTrees:
				return string.Format(Gameworld.GetStaticString("RoomNameInTrees"), baseRoomName);
			case RoomLayer.HighInTrees:
				return string.Format(Gameworld.GetStaticString("RoomNameHighInTrees"), baseRoomName);
			case RoomLayer.InAir:
				return string.Format(Gameworld.GetStaticString("RoomNameInAir"), baseRoomName);
			case RoomLayer.HighInAir:
				return string.Format(Gameworld.GetStaticString("RoomNameHighInAir"), baseRoomName);
			case RoomLayer.OnRooftops:
				return string.Format(Gameworld.GetStaticString("RoomNameOnRooftops"), baseRoomName);
		}

		return baseRoomName;
	}

	public IFluid WaterFluid { get; private set; }

	public string TerrainEditorColour { get; private set; }
	public string TerrainEditorText { get; private set; }
	public string TerrainANSIColour { get; private set; }

	#region Implementation of IFutureProgVariable

	/// <summary>
	///     The FutureProgVariableType that represents this IFutureProgVariable
	/// </summary>
	public FutureProgVariableTypes Type => FutureProgVariableTypes.Terrain;

	/// <summary>
	///     Returns an object representing the underlying variable wrapped in this IFutureProgVariable
	/// </summary>
	public object GetObject => this;

	/// <summary>
	///     Requests an IFutureProgVariable representing the property referenced by the given string.
	/// </summary>
	/// <param name="property">A string representing the property to be retrieved</param>
	/// <returns>An IFutureProgVariable representing the desired property</returns>
	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "staminacost":
				return new NumberVariable(StaminaCost);
			case "infectionmultiplier":
				return new NumberVariable(InfectionMultiplier);
			case "primaryinfection":
				return new TextVariable(PrimaryInfection.Describe());
			case "infectionvirulence":
				return new NumberVariable((int)InfectionVirulence);
			case "foragableprofile":
				return new NumberVariable(_foragableProfileId);
			case "atmosphereid":
				return Atmosphere == null
					? (IFutureProgVariable)new NullVariable(FutureProgVariableTypes.Number)
					: new NumberVariable(Atmosphere.Id);
			case "default":
				return new BooleanVariable(DefaultTerrain);
			case "hidedifficulty":
				return new NumberVariable((int)HideDifficulty);
			case "spotdifficulty":
				return new NumberVariable((int)SpotDifficulty);
		}

		throw new NotSupportedException($"Unsupported property type {property} in {FrameworkItemType}.GetProperty");
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "staminacost", FutureProgVariableTypes.Number },
			{ "infectionmultiplier", FutureProgVariableTypes.Number },
			{ "primaryinfection", FutureProgVariableTypes.Text },
			{ "infectionvirulence", FutureProgVariableTypes.Number },
			{ "foragableprofile", FutureProgVariableTypes.Number },
			{ "atmosphereid", FutureProgVariableTypes.Number },
			{ "default", FutureProgVariableTypes.Boolean },
			{ "hidedifficulty", FutureProgVariableTypes.Number },
			{ "spotdifficulty", FutureProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "staminacost", "" },
			{ "infectionmultiplier", "" },
			{ "primaryinfection", "" },
			{ "infectionvirulence", "" },
			{ "foragableprofile", "" },
			{ "atmosphereid", "" },
			{ "default", "" },
			{ "hidedifficulty", "" },
			{ "spotdifficulty", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Terrain, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{Name} (#{Id.ToString("N0", actor)})".GetLineWithTitle(actor.LineFormatLength,
			actor.Account.UseUnicode, Telnet.Yellow, Telnet.Cyan));
		sb.AppendLine($"Default: {DefaultTerrain.ToColouredString()}");
		sb.AppendLine($"Stamina: {StaminaCost.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Movement: {MovementRate.ToString("P3", actor).ColourValue()}");
		sb.AppendLine($"Default Outdoors: {DefaultCellOutdoorsType.Describe().ColourValue()}");
		sb.AppendLine($"Hide Difficulty: {HideDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Spot Difficulty: {SpotDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Atmosphere: {Atmosphere?.Name.Colour(Atmosphere.DisplayColour) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Weather: {OverrideWeatherController?.Name.Colour(Telnet.BoldCyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Foragable Profile: {ForagableProfile?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Infection: {PrimaryInfection.Describe().Colour(Telnet.Magenta)} @ {InfectionVirulence.Describe().ColourValue()} {InfectionMultiplier.ToString("P2", actor).ColourValue()} Intensity");
		sb.AppendLine($"Model: {TerrainBehaviourString.ColourCommand()}");
		sb.AppendLine($"Editor Colour: {TerrainEditorColour.FluentTagMXP("Color", $"FORE={TerrainEditorColour}")}");
		sb.AppendLine($"Editor Text: {TerrainEditorText ?? ""}");
		sb.AppendLine($"Map Colour: {TerrainANSIColour.ColourForegroundCustom(TerrainANSIColour)}");
		sb.AppendLine($"Covers:");
		sb.AppendLine(CommonStringUtilities.ArrangeStringsOntoLines(
			TerrainCovers.Select(x => $"[{x.Id.ToString("N0", actor)}] {x.Name}"), 3, (uint)actor.LineFormatLength));
		return sb.ToString();
	}

	#region Building Commands

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "atmosphere":
				return BuildingCommandAtmosphere(actor, command);
			case "movement":
				return BuildingCommandMovement(actor, command);
			case "stamina":
				return BuildingCommandStamina(actor, command);
			case "hide":
				return BuildingCommandHide(actor, command);
			case "spot":
				return BuildingCommandSpot(actor, command);
			case "forage":
				return BuildingCommandForage(actor, command);
			case "weather":
				return BuildingCommandWeather(actor, command);
			case "cover":
				return BuildingCommandCover(actor, command);
			case "default":
				return BuildingCommandDefault(actor, command);
			case "infection":
				return BuildingCommandInfection(actor, command);
			case "outdoor":
			case "outdoors":
			case "outside":
				return BuildingCommandOutdoors(actor, CellOutdoorsType.Outdoors);
			case "indoors":
			case "indoor":
			case "inside":
				return BuildingCommandOutdoors(actor, CellOutdoorsType.Indoors);
			case "windows":
				return BuildingCommandOutdoors(actor, CellOutdoorsType.IndoorsWithWindows);
			case "cave":
			case "nolight":
			case "no light":
				return BuildingCommandOutdoors(actor, CellOutdoorsType.IndoorsNoLight);
			case "shelter":
			case "climate":
			case "sheltered":
			case "exposed":
				return BuildingCommandOutdoors(actor, CellOutdoorsType.IndoorsClimateExposed);
			case "mapcolour":
			case "mapcolor":
				return BuildingCommandMapColour(actor, command);
			case "editorcolour":
			case "editorcolor":
				return BuildingCommandEditorColour(actor, command);
			case "editortext":
				return BuildingCommandEditorText(actor, command);
			case "model":
			case "behaviour":
			case "behavior":
				return BuildingCommandModel(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with this building command:

	#3name <name>#0 - renames this terrain type
	#3atmosphere none#0 - sets the terrain to have no atmosphere
	#3atmosphere gas <gas>#0 - sets the atmosphere to a specified gas
	#3atmosphere liquid <liquid>#0 - sets the atmosphere to specified liquid
	#3movement <multiplier>#0 - sets the movement speed multiplier
	#3stamina <cost>#0 - sets the stamina cost for movement
	#3hide <difficulty>#0 - sets the hide difficulty
	#3spot <difficulty>#0 - sets the minimum spot difficulty
	#3forage none#0 - removes the forage profile from this terrain
	#3forage <profile>#0 - sets the foragable profile
	#3weather none#0 - removes a weather controller
	#3weather <controller>#0 - sets the weather controller
	#3cover <cover>#0 - toggles a ranged cover
	#3default#0 - sets this terrain as the default for new rooms
	#3infection <type> <difficulty> <virulence>#0 - sets the infection for this terrain
	#3outdoors|indoors|exposed|cave|windows#0 - sets the default behaviour type
	#3model <model>#0 - sets the layer model. See TERRAIN SET MODEL for a list of valid values.
    #3mapcolour <0-255>#0 - sets the ANSI colour for the MAP command
    #3editorcolour <#00000000>#0 - sets the hexadecimal colour for the terrain planner
    #3editortext <1 or 2 letter code>#0 - sets a code to appear on the terrain planner tile".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandModel(ICharacter actor, StringStack command)
	{
		var model = command.PopForSwitch();
		ILiquid liquid = null;
		switch (model)
		{
			case "outdoors":
			case "indoors":
			case "trees":
			case "talltrees":
			case "cavetrees":
			case "rooftops":
			case "cave":
			case "cliff":
				break;
			case "shallowwater":
			case "water":
			case "shallowwatercave":
			case "shallowwatertrees":
			case "deepwater":
			case "verydeepwater":
			case "deepwatercave":
			case "verydeepwatercave":
			case "underwater":
			case "deepunderwater":
			case "verydeepunderwater":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						$"That terrain model requires you to specify a liquid. See {"show liquids".MXPSend("show liquids")} for a complete list.");
					return false;
				}

				liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
				if (liquid is null)
				{
					actor.OutputHandler.Send(
						$"There is no such liquid liquid. See {"show liquids".MXPSend("show liquids")} for a complete list.");
					return false;
				}

				break;
			default:
				actor.OutputHandler.Send(@"The valid models are as listed below:

	#3outdoors#0 - Ground Level + 2 air levels
	#3indoors#0 - Ground Level only
	#3cave#0 - Ground Level + 1 air level
	#3cliff#0 - Air Levels only - i.e. rockface
	#3rooftop#0 - Ground level + rooftops
	#3trees#0 - Ground level + trees + 2 air levels
	#3talltrees#0 - Ground level + 2 tree levels + 2 air levels
	#3cavetrees#0 - Ground level + 2 tree levels

The following additional models require you to specify a liquid to go with them:

	#3shallowwater <liquid>#0 - underwater + water surface + 2 air levels
	#3deepwater <liquid>#0 - 2 underwater layers + water surface + 2 air levels
	#3verydeepwater <liquid>#0 - 3 underwater layers + water surface + 2 air levels
	#3underwater <liquid>#0 - 1 underwater layer only
	#3deepunderwater <liquid>#0 - 2 underwater layers only
	#3verydeepunderwater <liquid>#0 - 3 underwater layers only
	#3shallowwatertrees <liquid>#0 - underwater + water surface + trees + 2 air levels
	#3shallowwatercave <liquid>#0 - underwater + water surface only
	#3deepwatercave <liquid>#0 - 2 underwater layers + water surface only".SubstituteANSIColour());
				return false;
		}

		_terrainLayers.Clear();
		switch (model)
		{
			case "outdoors":
				SetAsOutdoorsTerrain();
				break;
			case "indoors":
				SetAsIndoors();
				break;
			case "trees":
				SetAsTreesTerrain();
				break;
			case "talltrees":
				SetAsTallTreesTerrain();
				break;
			case "cavetrees":
				SetAsUndergroundTreesTerrain();
				break;
			case "rooftops":
				SetAsRooftopsTerrain();
				break;
			case "cave":
				SetAsCaveTerrain();
				break;
			case "cliff":
				SetAsCliffTerrain();
				break;
			case "shallowwater":
			case "water":
				SetAsShallowWaterTerrain(command);
				break;
			case "shallowwatercave":
				SetAsShallowWaterCaveTerrain(command);
				break;
			case "shallowwatertrees":
				SetAsShallowWaterTreesTerrain(command);
				break;
			case "deepwater":
				SetAsDeepWaterTerrain(command);
				break;
			case "verydeepwater":
				SetAsVeryDeepWaterTerrain(command);
				break;
			case "deepwatercave":
				SetAsDeepWaterCaveTerrain(command);
				break;
			case "verydeepwatercave":
				SetAsVeryDeepWaterCaveTerrain(command);
				break;
			case "underwater":
				SetAsUnderwater(command);
				break;
			case "deepunderwater":
				SetAsDeepUnderwater(command);
				break;
			case "verydeepunderwater":
				SetAsVeryDeepUnderWater(command);
				break;
			default:
				return false;
		}


		var text = liquid is null ? model : $"{model} {liquid.Id}";
		actor.OutputHandler.Send(
			$"You change the terrain model to {text}. The layers are now:\n\n{_terrainLayers.Select(x => x.LocativeDescription().ColourName()).ListToLines(true)}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEditorText(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			TerrainEditorText = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This terrain will no longer have any text on the terrain editor.");
			return true;
		}

		var text = command.SafeRemainingArgument;
		if (text.Length > 2)
		{
			actor.OutputHandler.Send("The terrain editor text must be two characters in length or less.");
			return false;
		}

		TerrainEditorText = text;
		Changed = true;
		actor.OutputHandler.Send($"The terrain editor text for this terrain is now {TerrainEditorText.ColourValue()}.");
		return true;
	}

	private static readonly Regex EditorColourRegex = new("#[0-9a-f]{8}", RegexOptions.IgnoreCase);

	private bool BuildingCommandEditorColour(ICharacter actor, StringStack command)
	{
		if (!EditorColourRegex.IsMatch(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send(
				$"The colour for the editor must be in the format {"#00000000".Colour(Telnet.BoldCyan)}, where each 0 is a hexadecimal digit.");
			return false;
		}

		TerrainEditorColour = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This terrain will now use the hexadecimal colour code {TerrainEditorColour.Colour(Telnet.BoldCyan)}.");
		return true;
	}

	private bool BuildingCommandMapColour(ICharacter actor, StringStack command)
	{
		var text = command.SafeRemainingArgument;
		if (!int.TryParse(text, out var value) || value < 0 || value > 255)
		{
			actor.OutputHandler.Send(
				$"The colour must be a number between 0 and 255. See {"https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit".FluentTagMXP("A", "href=\"https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit\"")} for a list of colour numbers.");
			return false;
		}

		TerrainANSIColour = value.ToString("F0");
		actor.OutputHandler.Send(
			$"This terrain will now use the colour sequence {TerrainANSIColour.ColourForegroundCustom(TerrainANSIColour)} for display on the in-game map.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandOutdoors(ICharacter actor, CellOutdoorsType outdoors)
	{
		DefaultCellOutdoorsType = outdoors;
		Changed = true;
		switch (outdoors)
		{
			case CellOutdoorsType.Indoors:
				actor.OutputHandler.Send($"This terrain is now indoors by default.");
				break;
			case CellOutdoorsType.IndoorsWithWindows:
				actor.OutputHandler.Send(
					$"This terrain is now indoors with windows by default. This means those inside will be able to see the weather and the position of the sun, even though sheltered.");
				break;
			case CellOutdoorsType.Outdoors:
				actor.OutputHandler.Send(
					$"This terrain is now outdoors by default; exposed to the elements with full visibility.");
				break;
			case CellOutdoorsType.IndoorsNoLight:
				actor.OutputHandler.Send(
					$"This terrain is now indoors with no light (i.e. cave) by default. They cannot see outside and there is no natural light from celestial objects.");
				break;
			case CellOutdoorsType.IndoorsClimateExposed:
				actor.OutputHandler.Send(
					$"This terrain is now indoors but climate exposed (i.e. a shelter or a bluff). It is safe from the rain but not from the wind.");
				break;
		}

		return true;
	}

	private bool BuildingCommandInfection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of infection do you want to set for this terrain? The valid types are: {Enum.GetValues(typeof(InfectionType)).OfType<InfectionType>().Select(x => x.Describe().Colour(Telnet.Magenta)).ListToString()}.");
			return false;
		}

		if (!Utilities.TryParseEnum<InfectionType>(command.PopSpeech(), out var theEnum))
		{
			actor.OutputHandler.Send(
				$"That is not a valid infection type. The valid types are: {Enum.GetValues(typeof(InfectionType)).OfType<InfectionType>().Select(x => x.Describe().Colour(Telnet.Magenta)).ListToString()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the difficulty to resist this infection?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the virulence multiplier for this terrain?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number for the infection virulence.");
			return false;
		}

		PrimaryInfection = theEnum;
		InfectionMultiplier = value;
		InfectionVirulence = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This terrain now harbours the {PrimaryInfection.Describe().Colour(Telnet.Magenta)} infection at difficulty {InfectionVirulence.Describe().ColourValue()} and virulence {InfectionMultiplier.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDefault(ICharacter actor, StringStack command)
	{
		if (DefaultTerrain)
		{
			actor.OutputHandler.Send(
				"This terrain is already the default terrain. There must always be a default terrain, so in order to change it you must edit the new terrain and use this same command.");
			return false;
		}

		DefaultTerrain = true;
		Changed = true;
		foreach (var terrain in Gameworld.Terrains.Except(this))
		{
			terrain.DefaultTerrain = false;
			terrain.Changed = true;
		}

		actor.OutputHandler.Send(
			$"You set the {Name.Colour(Telnet.Cyan)} terrain to be the default terrain for new rooms.");
		return true;
	}

	private bool BuildingCommandCover(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which cover do you want to toggle on or off for this terrain?");
			return false;
		}

		var cover = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.RangedCovers.Get(value)
			: Gameworld.RangedCovers.GetByName(command.Last);
		if (cover == null)
		{
			actor.OutputHandler.Send("There is no such ranged cover.");
			return false;
		}

		if (_terrainCovers.Contains(cover))
		{
			_terrainCovers.Remove(cover);
			actor.OutputHandler.Send(
				$"The {Name.Colour(Telnet.Cyan)} terrain no longer contains the {cover.Name.ColourValue()} cover.");
		}
		else
		{
			_terrainCovers.Add(cover);
			actor.OutputHandler.Send(
				$"The {Name.Colour(Telnet.Cyan)} terrain now contains the {cover.Name.ColourValue()} cover.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandWeather(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a weather controller to use for this terrain, or 'none' to remove an existing weather controller.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			_overrideWeatherController = null;
			_overrideWeatherControllerId = null;
			Changed = true;
			actor.OutputHandler.Send($"The {Name.Colour(Telnet.Cyan)} terrain no longer has any weather controller.");
			return true;
		}

		var controller = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.WeatherControllers.Get(value)
			: Gameworld.WeatherControllers.GetByName(command.Last);
		if (controller == null)
		{
			actor.OutputHandler.Send("There is no such weather controller.");
			return false;
		}

		_overrideWeatherControllerId = controller.Id;
		_overrideWeatherController = controller;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} terrain will now use the {controller.Name.Colour(Telnet.BoldCyan)} weather controller.");
		return true;
	}

	private bool BuildingCommandForage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a foragable profile to use for this terrain, or 'none' to remove an existing foreable profile.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			_foragableProfile = null;
			_foragableProfileId = 0;
			Changed = true;
			actor.OutputHandler.Send($"The {Name.Colour(Telnet.Cyan)} terrain no longer has any foragable profile.");
			return true;
		}

		var profile = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ForagableProfiles.Get(value)
			: Gameworld.ForagableProfiles.GetByName(command.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such foragable profile.");
			return false;
		}

		if (profile.Status != Framework.Revision.RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"You cannot use the {profile.Name.ColourValue()} foragable profile because it has no approved version.");
			return false;
		}

		_foragableProfile = profile;
		_foragableProfileId = profile.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} terrain now uses the {profile.Name.ColourValue()} foragable profile.");
		return true;
	}

	private bool BuildingCommandSpot(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the minimum difficulty to all spot checks in this terrain?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		SpotDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"It will now always be at least {SpotDifficulty.Describe().ColourValue()} to spot in the {Name.Colour(Telnet.Cyan)} terrain.");
		return true;
	}

	private bool BuildingCommandHide(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the minimum difficulty to all hide checks in this terrain?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		HideDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"It will now always be at least {HideDifficulty.Describe().ColourValue()} to hide in the {Name.Colour(Telnet.Cyan)} terrain.");
		return true;
	}

	private bool BuildingCommandStamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much stamina should the base movement cost for this terrain be?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		StaminaCost = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} terrain will now have a base stamina cost for movement of {StaminaCost.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMovement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the movement speed percentage be for this terrain?");
			return false;
		}

		if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		MovementRate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.Colour(Telnet.Cyan)} terrain will now have a movement rate of {MovementRate.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAtmosphere(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify none, gas or liquid for the atmosphere.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "none":
				Atmosphere = null;
				Changed = true;
				actor.OutputHandler.Send($"The {Name.Colour(Telnet.Cyan)} terrain will no longer have any atmosphere.");
				return true;
			case "gas":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a specific gas after the gas keyword.");
					return false;
				}

				var gas = long.TryParse(command.PopSpeech(), out var value)
					? Gameworld.Gases.Get(value)
					: Gameworld.Gases.GetByName(command.Last);
				if (gas == null)
				{
					actor.OutputHandler.Send("There is no such gas.");
					return false;
				}

				Atmosphere = gas;
				Changed = true;
				actor.OutputHandler.Send(
					$"The {Name.Colour(Telnet.Cyan)} terrain will now have a gas atmosphere consisting of {gas.Name.Colour(gas.DisplayColour)}.");
				return true;
			case "liquid":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a specific liquid after the liquid keyword.");
					return false;
				}

				var liquid = long.TryParse(command.PopSpeech(), out value)
					? Gameworld.Liquids.Get(value)
					: Gameworld.Liquids.GetByName(command.Last);
				if (liquid == null)
				{
					actor.OutputHandler.Send("There is no such liquid.");
					return false;
				}

				Atmosphere = liquid;
				Changed = true;
				actor.OutputHandler.Send(
					$"The {Name.Colour(Telnet.Cyan)} terrain will now have a liquid atmosphere consisting of {liquid.Name.Colour(liquid.DisplayColour)}.");
				return true;
			default:
				actor.OutputHandler.Send("You must either specify none, gas or liquid for the atmosphere.");
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to set for this terrain?");
			return false;
		}

		var name = command.PopSpeech().Trim().TitleCase();
		if (Gameworld.Terrains.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a terrain with that name. Names must be unique.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This terrain is now called {name.Colour(Telnet.Cyan)}.");
		return true;
	}

	#endregion
}