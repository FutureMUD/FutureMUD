#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Economy.Property;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class FrameworkItemLookupFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly FrameworkItemLookupType _lookupType;
	private readonly ProgVariableTypes _returnType;
	private readonly bool _useId;

	private FrameworkItemLookupFunction(IList<IFunction> parameters, IFuturemud gameworld,
		FrameworkItemLookupType lookupType, ProgVariableTypes returnType, bool useId)
		: base(parameters)
	{
		_gameworld = gameworld;
		_lookupType = lookupType;
		_returnType = returnType;
		_useId = useId;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var value = ParameterFunctions[0].Result?.GetObject;
		if (value is null)
		{
			Result = new NullVariable(_returnType);
			return StatementResult.Normal;
		}

		var result = _useId
			? LookupById(Convert.ToInt64(value))
			: LookupByName(Convert.ToString(value) ?? string.Empty);
		Result = result ?? new NullVariable(_returnType);
		return StatementResult.Normal;
	}

	private IProgVariable? LookupById(long id)
	{
		return _lookupType switch
		{
			FrameworkItemLookupType.Tag => _gameworld.Tags.Get(id),
			FrameworkItemLookupType.ItemPrototype => _gameworld.ItemProtos.Get(id),
			FrameworkItemLookupType.NPCTemplate => _gameworld.NpcTemplates.Get(id),
			FrameworkItemLookupType.OutfitTemplate => _gameworld.OutfitTemplates.Get(id),
			FrameworkItemLookupType.Vehicle => _gameworld.Vehicles.Get(id),
			FrameworkItemLookupType.CelestialObject => _gameworld.CelestialObjects.Get(id),
			FrameworkItemLookupType.Grid => _gameworld.Grids.Get(id),
			FrameworkItemLookupType.CharacteristicDefinition => _gameworld.Characteristics.Get(id),
			FrameworkItemLookupType.AgricultureFieldProfile => _gameworld.AgricultureFieldProfiles.Get(id),
			FrameworkItemLookupType.AgricultureCropDefinition => _gameworld.AgricultureCropDefinitions.Get(id),
			FrameworkItemLookupType.AgricultureHerdDefinition => _gameworld.AgricultureHerdDefinitions.Get(id),
			FrameworkItemLookupType.AgricultureWoodlandDefinition => _gameworld.AgricultureWoodlandDefinitions.Get(id),
			FrameworkItemLookupType.AgricultureOperation => _gameworld.AgricultureOperations.Get(id),
			FrameworkItemLookupType.Property => _gameworld.Properties.Get(id),
			FrameworkItemLookupType.PropertyKey => PropertyReferenceLookup.GetPropertyKey(_gameworld, id),
			FrameworkItemLookupType.PropertyLease => PropertyReferenceLookup.GetPropertyLease(_gameworld, id),
			FrameworkItemLookupType.PropertyLeaseOrder => PropertyReferenceLookup.GetPropertyLeaseOrder(_gameworld, id),
			FrameworkItemLookupType.PropertySaleOrder => PropertyReferenceLookup.GetPropertySaleOrder(_gameworld, id),
			FrameworkItemLookupType.EconomicZone => _gameworld.EconomicZones.Get(id),
			FrameworkItemLookupType.Channel => _gameworld.Channels.Get(id),
			FrameworkItemLookupType.Calendar => _gameworld.Calendars.Get(id),
			FrameworkItemLookupType.Clock => _gameworld.Clocks.Get(id),
			FrameworkItemLookupType.WeatherEvent => _gameworld.WeatherEvents.Get(id),
			FrameworkItemLookupType.Shop => _gameworld.Shops.Get(id),
			FrameworkItemLookupType.Solid => _gameworld.Materials.Get(id),
			FrameworkItemLookupType.Liquid => _gameworld.Liquids.Get(id),
			FrameworkItemLookupType.Gas => _gameworld.Gases.Get(id),
			FrameworkItemLookupType.MagicSpell => _gameworld.MagicSpells.Get(id),
			FrameworkItemLookupType.MagicSchool => _gameworld.MagicSchools.Get(id),
			FrameworkItemLookupType.MagicCapability => _gameworld.MagicCapabilities.Get(id),
			FrameworkItemLookupType.Market => _gameworld.Markets.Get(id),
			FrameworkItemLookupType.MarketCategory => _gameworld.MarketCategories.Get(id),
			FrameworkItemLookupType.AgricultureField => _gameworld.AgricultureFields.Get(id),
			FrameworkItemLookupType.NPCSkillPackage => _gameworld.NpcSkillPackages.Get(id),
			FrameworkItemLookupType.Project => _gameworld.ActiveProjects.Get(id),
			FrameworkItemLookupType.Crime => _gameworld.Crimes.Get(id),
			_ => null
		};
	}

	private IProgVariable? LookupByName(string value)
	{
		return _lookupType switch
		{
			FrameworkItemLookupType.Tag => _gameworld.Tags.GetByIdOrName(value),
			FrameworkItemLookupType.ItemPrototype => _gameworld.ItemProtos.GetByIdOrName(value),
			FrameworkItemLookupType.NPCTemplate => _gameworld.NpcTemplates.GetByIdOrName(value),
			FrameworkItemLookupType.OutfitTemplate => _gameworld.OutfitTemplates.GetByIdOrName(value),
			FrameworkItemLookupType.Vehicle => _gameworld.Vehicles.GetByIdOrName(value),
			FrameworkItemLookupType.CelestialObject => _gameworld.CelestialObjects.GetByIdOrName(value),
			FrameworkItemLookupType.Grid => _gameworld.Grids.GetByIdOrName(value),
			FrameworkItemLookupType.CharacteristicDefinition => _gameworld.Characteristics.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureFieldProfile => _gameworld.AgricultureFieldProfiles.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureCropDefinition => _gameworld.AgricultureCropDefinitions.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureHerdDefinition => _gameworld.AgricultureHerdDefinitions.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureWoodlandDefinition => _gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureOperation => _gameworld.AgricultureOperations.GetByIdOrName(value),
			FrameworkItemLookupType.Property => _gameworld.Properties.GetByIdOrName(value),
			FrameworkItemLookupType.EconomicZone => _gameworld.EconomicZones.GetByIdOrName(value),
			FrameworkItemLookupType.Channel => _gameworld.Channels.GetByIdOrName(value),
			FrameworkItemLookupType.Calendar => _gameworld.Calendars.GetByIdOrName(value),
			FrameworkItemLookupType.Clock => _gameworld.Clocks.GetByIdOrName(value),
			FrameworkItemLookupType.WeatherEvent => _gameworld.WeatherEvents.GetByIdOrName(value),
			FrameworkItemLookupType.Shop => _gameworld.Shops.GetByIdOrName(value),
			FrameworkItemLookupType.Solid => _gameworld.Materials.GetByIdOrName(value),
			FrameworkItemLookupType.Liquid => _gameworld.Liquids.GetByIdOrName(value),
			FrameworkItemLookupType.Gas => _gameworld.Gases.GetByIdOrName(value),
			FrameworkItemLookupType.MagicSpell => _gameworld.MagicSpells.GetByIdOrName(value),
			FrameworkItemLookupType.MagicSchool => _gameworld.MagicSchools.GetByIdOrName(value),
			FrameworkItemLookupType.MagicCapability => _gameworld.MagicCapabilities.GetByIdOrName(value),
			FrameworkItemLookupType.Market => _gameworld.Markets.GetByIdOrName(value),
			FrameworkItemLookupType.MarketCategory => _gameworld.MarketCategories.GetByIdOrName(value),
			FrameworkItemLookupType.AgricultureField => _gameworld.AgricultureFields.GetByIdOrName(value),
			FrameworkItemLookupType.NPCSkillPackage => _gameworld.NpcSkillPackages.GetByIdOrName(value),
			_ => null
		};
	}

	public static void RegisterFunctionCompiler()
	{
		Register("tag", ProgVariableTypes.Tag, FrameworkItemLookupType.Tag, "tag", "Returns a tag by ID or name, or null.");
		Register("itemprototype", ProgVariableTypes.ItemPrototype, FrameworkItemLookupType.ItemPrototype, "item prototype", "Returns the current item prototype by ID or name, or null.");
		Register("npctemplate", ProgVariableTypes.NPCTemplate, FrameworkItemLookupType.NPCTemplate, "NPC template", "Returns the current NPC template by ID or name, or null.");
		Register("outfittemplate", ProgVariableTypes.OutfitTemplate, FrameworkItemLookupType.OutfitTemplate, "outfit template", "Returns an outfit template by ID or name, or null.");
		Register("vehicle", ProgVariableTypes.Vehicle, FrameworkItemLookupType.Vehicle, "vehicle", "Returns a vehicle by ID or name, or null.");
		Register("celestial", ProgVariableTypes.CelestialObject, FrameworkItemLookupType.CelestialObject, "celestial object", "Returns a celestial object by ID or name, or null.");
		Register("grid", ProgVariableTypes.Grid, FrameworkItemLookupType.Grid, "grid", "Returns a grid by ID or name, or null.");
		Register("characteristicdefinition", ProgVariableTypes.CharacteristicDefinition, FrameworkItemLookupType.CharacteristicDefinition, "characteristic definition", "Returns a characteristic definition by ID or name, or null.");
		Register("fieldprofile", ProgVariableTypes.AgricultureFieldProfile, FrameworkItemLookupType.AgricultureFieldProfile, "field profile", "Returns an agriculture field profile by ID or name, or null.");
		Register("cropdefinition", ProgVariableTypes.AgricultureCropDefinition, FrameworkItemLookupType.AgricultureCropDefinition, "crop definition", "Returns an agriculture crop definition by ID or name, or null.");
		Register("herddefinition", ProgVariableTypes.AgricultureHerdDefinition, FrameworkItemLookupType.AgricultureHerdDefinition, "herd definition", "Returns an agriculture herd definition by ID or name, or null.");
		Register("woodlanddefinition", ProgVariableTypes.AgricultureWoodlandDefinition, FrameworkItemLookupType.AgricultureWoodlandDefinition, "woodland definition", "Returns an agriculture woodland definition by ID or name, or null.");
		Register("agricultureoperation", ProgVariableTypes.AgricultureOperation, FrameworkItemLookupType.AgricultureOperation, "agriculture operation", "Returns an agriculture operation by ID or name, or null.");
		Register("property", ProgVariableTypes.Property, FrameworkItemLookupType.Property, "property", "Returns a property by ID or name, or null.");
		RegisterIdOnly("propertykey", ProgVariableTypes.PropertyKey, FrameworkItemLookupType.PropertyKey, "property key", "Returns a property key by durable ID, or null.");
		RegisterIdOnly("propertylease", ProgVariableTypes.PropertyLease, FrameworkItemLookupType.PropertyLease, "property lease", "Returns a property lease by durable ID, or null.");
		RegisterIdOnly("propertyleaseorder", ProgVariableTypes.PropertyLeaseOrder, FrameworkItemLookupType.PropertyLeaseOrder, "property lease order", "Returns a property lease order by durable ID, or null.");
		RegisterIdOnly("propertysaleorder", ProgVariableTypes.PropertySaleOrder, FrameworkItemLookupType.PropertySaleOrder, "property sale order", "Returns a property sale order by durable ID, or null.");
		Register("economiczone", ProgVariableTypes.EconomicZone, FrameworkItemLookupType.EconomicZone, "economic zone", "Returns an economic zone by ID or name, or null.");
		Register("channel", ProgVariableTypes.Channel, FrameworkItemLookupType.Channel, "channel", "Returns a channel by ID or name, or null.");
		Register("tocalendar", ProgVariableTypes.Calendar, FrameworkItemLookupType.Calendar, "calendar", "Returns a calendar by ID or name, or null.");
		Register("toclock", ProgVariableTypes.Clock, FrameworkItemLookupType.Clock, "clock", "Returns a clock by ID or name, or null.");
		Register("toweatherevent", ProgVariableTypes.WeatherEvent, FrameworkItemLookupType.WeatherEvent, "weather event", "Returns a weather event by ID or name, or null.");
		Register("toshop", ProgVariableTypes.Shop, FrameworkItemLookupType.Shop, "shop", "Returns a shop by ID or name, or null.");
		Register("tosolid", ProgVariableTypes.Solid, FrameworkItemLookupType.Solid, "solid", "Returns a solid by ID or name, or null.");
		Register("toliquid", ProgVariableTypes.Liquid, FrameworkItemLookupType.Liquid, "liquid", "Returns a liquid by ID or name, or null.");
		Register("togas", ProgVariableTypes.Gas, FrameworkItemLookupType.Gas, "gas", "Returns a gas by ID or name, or null.");
		Register("tomagicspell", ProgVariableTypes.MagicSpell, FrameworkItemLookupType.MagicSpell, "magic spell", "Returns a magic spell by ID or name, or null.");
		Register("tomagicschool", ProgVariableTypes.MagicSchool, FrameworkItemLookupType.MagicSchool, "magic school", "Returns a magic school by ID or name, or null.");
		Register("tomagiccapability", ProgVariableTypes.MagicCapability, FrameworkItemLookupType.MagicCapability, "magic capability", "Returns a magic capability by ID or name, or null.");
		Register("tomarket", ProgVariableTypes.Market, FrameworkItemLookupType.Market, "market", "Returns a market by ID or name, or null.");
		Register("tomarketcategory", ProgVariableTypes.MarketCategory, FrameworkItemLookupType.MarketCategory, "market category", "Returns a market category by ID or name, or null.");
		Register("toagriculturefield", ProgVariableTypes.AgricultureField, FrameworkItemLookupType.AgricultureField, "agriculture field", "Returns an agriculture field by ID or name, or null.");
		Register("tonpcskillpackage", ProgVariableTypes.NPCSkillPackage, FrameworkItemLookupType.NPCSkillPackage, "NPC skill package", "Returns an NPC skill package by ID or name, or null.");
		RegisterIdOnly("toproject", ProgVariableTypes.Project, FrameworkItemLookupType.Project, "Project", "Returns an active project by ID, or null.");
		Register("tocelestialobject", ProgVariableTypes.CelestialObject, FrameworkItemLookupType.CelestialObject, "celestial object", "Returns a celestial object by ID or name, or null.");
		Register("toagriculturefieldprofile", ProgVariableTypes.AgricultureFieldProfile, FrameworkItemLookupType.AgricultureFieldProfile, "agriculture field profile", "Returns an agriculture field profile by ID or name, or null.");
		Register("toagriculturecropdefinition", ProgVariableTypes.AgricultureCropDefinition, FrameworkItemLookupType.AgricultureCropDefinition, "agriculture crop definition", "Returns an agriculture crop definition by ID or name, or null.");
		Register("toagricultureherddefinition", ProgVariableTypes.AgricultureHerdDefinition, FrameworkItemLookupType.AgricultureHerdDefinition, "agriculture herd definition", "Returns an agriculture herd definition by ID or name, or null.");
		Register("toagriculturewoodlanddefinition", ProgVariableTypes.AgricultureWoodlandDefinition, FrameworkItemLookupType.AgricultureWoodlandDefinition, "agriculture woodland definition", "Returns an agriculture woodland definition by ID or name, or null.");
		RegisterIdOnly("tocrime", ProgVariableTypes.Crime, FrameworkItemLookupType.Crime, "crime", "Returns a crime by ID, or null.");
	}

	private static void Register(string name, ProgVariableTypes returnType, FrameworkItemLookupType lookupType,
		string itemName, string functionHelp)
	{
		if (!name.StartsWith("to", StringComparison.Ordinal))
		{
			Register("to" + name, returnType, lookupType, itemName, functionHelp);
		}

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Number],
			(parameters, gameworld) => new FrameworkItemLookupFunction(parameters, gameworld, lookupType, returnType, true),
			["id"],
			[$"The ID of the {itemName} to find."],
			functionHelp,
			"World Lookup",
			returnType));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Text],
			(parameters, gameworld) => new FrameworkItemLookupFunction(parameters, gameworld, lookupType, returnType, false),
			["name"],
			[$"The name or ID of the {itemName} to find."],
			functionHelp,
			"World Lookup",
			returnType));
	}

	private static void RegisterIdOnly(string name, ProgVariableTypes returnType, FrameworkItemLookupType lookupType,
		string itemName, string functionHelp)
	{
		if (!name.StartsWith("to", StringComparison.Ordinal))
		{
			RegisterIdOnly("to" + name, returnType, lookupType, itemName, functionHelp);
		}

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Number],
			(parameters, gameworld) => new FrameworkItemLookupFunction(parameters, gameworld, lookupType, returnType, true),
			["id"],
			[$"The durable ID of the {itemName} to find."],
			functionHelp,
			"World Lookup",
			returnType));
	}

	private enum FrameworkItemLookupType
	{
		Tag,
		ItemPrototype,
		NPCTemplate,
		OutfitTemplate,
		Vehicle,
		CelestialObject,
		Grid,
		CharacteristicDefinition,
		AgricultureFieldProfile,
		AgricultureCropDefinition,
		AgricultureHerdDefinition,
		AgricultureWoodlandDefinition,
		AgricultureOperation,
		Property,
		PropertyKey,
		PropertyLease,
		PropertyLeaseOrder,
		PropertySaleOrder,
		EconomicZone,
		Calendar,
		Clock,
		WeatherEvent,
		Shop,
		Solid,
		Liquid,
		Gas,
		MagicSpell,
		MagicSchool,
		MagicCapability,
		Market,
		MarketCategory,
		AgricultureField,
		NPCSkillPackage,
		Project,
		Crime,
		Channel
	}
}
