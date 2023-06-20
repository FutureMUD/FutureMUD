using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Commands.Socials;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Communication.Language.Scramblers;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Boundary;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Discord;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Health;
using MudSharp.Help;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.AI.Groups;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Light;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Dreams;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Law;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Butchering;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Framework;

public sealed partial class Futuremud : IDisposable
{
	#region All<T> Declarations

	private static readonly List<Futuremud> _allgames = new();
	private readonly All<IAccent> _accents = new();
	private readonly All<IAccount> _accounts = new();
	private readonly All<ICharacter> _actors = new();
	private readonly All<ICharacter> _cachedActors = new();
	private readonly All<IAmmunitionType> _ammunitionTypes = new();
	private readonly All<IArea> _areas = new();
	private readonly All<IArmourType> _armourTypes = new();
	private readonly All<IAuctionHouse> _auctionHouses = new();
	private readonly All<IAuthority> _authorities = new();
	private readonly All<IAutobuilderArea> _autobuilderAreas = new();
	private readonly All<IAutobuilderRoom> _autobuilderRooms = new();
	private readonly All<IBank> _banks = new();
	private readonly All<IBankAccount> _bankAccounts = new();
	private readonly All<IBankAccountType> _bankAccountTypes = new();
	private readonly All<IBloodtype> _bloodtypes = new();
	private readonly All<IBloodtypeAntigen> _bloodtypeAntigens = new();
	private readonly All<IBloodModel> _bloodModels = new();
	private readonly All<IBoard> _boards = new();
	private readonly All<IBody> _bodies = new();
	private readonly All<IBodypart> _bodypartPrototypes = new();

	private readonly All<IBodypartGroupDescriber> _bodypartGroupDescriptionRules =
		new();

	private readonly All<IBodypartShape> _bodypartShapes = new();
	private readonly All<IBodyPrototype> _bodyPrototypes = new();
	private readonly All<IButcheryProduct> _butcheryProducts = new();
	private readonly All<IRaceButcheryProfile> _raceButcheryProfiles = new();
	private readonly All<ICalendar> _calendars = new();
	private readonly All<ICelestialObject> _celestialObjects = new();

	private readonly RevisableAll<ICellOverlayPackage> _cellOverlayPackages =
		new();

	private readonly All<ICell> _cells = new();
	private readonly All<IChannel> _channels = new();
	private readonly All<ICharacteristicProfile> _characteristicProfiles = new();
	private readonly All<ICharacteristicDefinition> _characteristics = new();
	private readonly All<ICharacteristicValue> _characteristicValues = new();
	private readonly All<ICharacterIntroTemplate> _characterIntroTemplates = new();
	private readonly All<ICharacter> _guests = new();
	private readonly All<ICharacter> _characters = new();
	private readonly All<IChargenAdvice> _chargenAdvices = new();
	private readonly All<IChargenResource> _chargenResources = new();
	private readonly All<ICheck> _checks = new();
	private readonly All<IClan> _clans = new();
	private readonly All<IClimateModel> _climateModels = new();
	private readonly RevisableAll<IDisfigurementTemplate> _disfigurementTemplates = new();
	private readonly All<IRegionalClimate> _regionalClimates = new();
	private readonly All<ISeason> _seasons = new();
	private readonly All<IWeatherController> _weatherControllers = new();
	private readonly All<IWeatherEvent> _weatherEvents = new();
	private readonly All<IClock> _clocks = new();
	private readonly All<IColour> _colours = new();
	private readonly All<ICharacterCombatSettings> _characterCombatSettings = new();
	private readonly All<ICorpseModel> _corpseModels = new();
	private readonly RevisableAll<ICraft> _crafts = new();
	private readonly All<ICulture> _cultures = new();
	private readonly All<ICurrency> _currencies = new();
	private readonly List<IDefaultHook> _defaultHooks = new();
	private readonly All<IDrug> _drugs = new();
	private readonly All<IDream> _dreams = new();
	private readonly All<IElection> _elections = new();

	private readonly All<IEntityDescriptionPattern> _entityDescriptionPatterns =
		new();

	private readonly All<IEnforcementAuthority> _enforcementAuthorities = new();
	private readonly All<ILegalAuthority> _legalAuthorities = new();
	private readonly All<ILegalClass> _legalClasses = new();
	private readonly All<IWitnessProfile> _witnessProfiles = new();
	private readonly All<ILaw> _laws = new();
	private readonly All<ICrime> _crimes = new();

	private readonly All<IEthnicity> _ethnicities = new();
	private readonly RevisableAll<IForagable> _foragables = new();
	private readonly RevisableAll<IForagableProfile> _foragableProfiles = new();
	private readonly All<IFutureProg> _futureProgs = new();
	private readonly All<IGrid> _grids = new();
	private readonly All<IHealthStrategy> _healthStrategies = new();
	private readonly All<IHearingProfile> _hearingProfiles = new();
	private readonly All<IHeightWeightModel> _heightWeightModels = new();
	private readonly All<IHelpfile> _helpfiles = new();
	private readonly All<IHook> _hooks = new();
	private readonly All<IKnowledge> _knowledges = new();
	private readonly All<ISolid> _materials = new();
	private readonly All<IArtificialIntelligence> _AIs = new();
	private readonly RevisableAll<INPCTemplate> _npcTemplates = new();
	private readonly All<IImprovementModel> _improvementModels = new();

	private readonly RevisableAll<IGameItemComponentProto> _itemComponentProtos =
		new();

	private readonly All<IGameItemGroup> _itemGroups = new();
	private readonly RevisableAll<IGameItemProto> _itemProtos = new();
	private readonly All<IGameItem> _items = new();
	private readonly RevisableAll<IGameItemSkin> _itemSkins = new();
	private readonly All<IJobListing> _jobListings = new();
	private readonly All<IActiveJob> _activeJobs = new();
	private readonly All<IGas> _gases = new();
	private readonly All<ILanguageDifficultyModel> _languageDifficultyModels = new();
	private readonly All<ILanguage> _languages = new();
	private readonly All<ILimb> _limbs = new();
	private readonly All<ILiquid> _liquids = new();
	private readonly All<ITemporalListener> _listeners = new();

	private readonly All<IMagicSchool> _magicSchools = new();
	private readonly All<IMagicCapability> _magicCapabilities = new();
	private readonly All<IMagicPower> _magicPowers = new();
	private readonly All<IMagicResource> _magicResources = new();
	private readonly All<IMagicResourceRegenerator> _magicResourceRegenerators = new();
	private readonly All<IMagicSpell> _magicSpells = new();

	private readonly RevisableAll<IProject> _projects = new();
	private readonly All<IActiveProject> _activeProjects = new();

	private readonly All<IMerit> _merits = new();
	private readonly All<INameCulture> _nameCultures = new();
	private readonly All<INPCSpawner> _npcSpawners = new();
	private readonly All<IRandomNameProfile> _randomNameProfiles = new();
	private readonly All<IProperty> _properties = new();
	private readonly All<IPatrol> _patrols = new();
	private readonly All<IPopulationBloodModel> _populationBloodModels = new();
	private readonly All<IProgSchedule> _progSchedules = new();
	private readonly All<IRace> _races = new();
	private readonly All<IRangedCover> _rangedCovers = new();
	private readonly All<IRangedWeaponType> _rangedWeaponTypes = new();
	private readonly All<IRoom> _rooms = new();
	private readonly All<IScript> _scripts = new();
	private readonly All<IShard> _shards = new();
	private readonly All<IShieldType> _shieldTypes = new();
	private readonly All<ISkyDescriptionTemplate> _skyDescriptionTemplates = new();
	private readonly All<IStackDecorator> _stackDecorators = new();
	private readonly All<ISurgicalProcedure> _surgicalProcedures = new();
	private readonly All<ITag> _tags = new();
	private readonly All<ITerrain> _terrains = new();
	private readonly All<ITraitValueDecorator> _traitDecorators = new();
	private readonly All<ITraitDefinition> _traits = new();
	private readonly All<ITraitExpression> _traitExpressions = new();
	private readonly All<IWearProfile> _wearProfiles = new();
	private readonly All<IZone> _zones = new();
	private readonly All<INonCardinalExitTemplate> _nonCardinalExitTemplates = new();
	private readonly All<ICharacter> _NPCs = new();
	private readonly List<ISocial> _socials = new();
	private readonly All<IChargenRole> _roles = new();
	private readonly All<IWeaponType> _weaponTypes = new();
	private readonly All<IWearableSize> _wearableSizes = new();
	private readonly All<IWriting> _writings = new();
	private readonly All<IDrawing> _drawings = new();
	private readonly All<IWeaponAttack> _weaponAttacks = new();
	private readonly All<IAuxillaryCombatAction> _auxillaryCombatActions = new();

	private readonly All<IShop> _shops = new();
	private readonly All<IEconomicZone> _economicZones = new();

	private readonly All<IGroupAITemplate> _groupAITemplates = new();
	private readonly All<IGroupAI> _groupAIs = new();

	private readonly All<ILineOfCreditAccount> _lineOfCreditAccounts = new();

	#endregion All<T> Declarations

	#region IUneditableAll<T> Declarations

	public IUneditableAll<IAccent> Accents => _accents;

	public IUneditableAll<IAccount> Accounts => _accounts;

	/// <summary>
	/// This collection contains all Actor (NPCs and Characters) that have been loaded into the game
	/// </summary>
	public IUneditableAll<ICharacter> Actors => _actors;

	public IUneditableAll<ICharacter> CachedActors => _cachedActors;

	public IUneditableAll<IAmmunitionType> AmmunitionTypes => _ammunitionTypes;

	public IUneditableAll<IArea> Areas => _areas;

	public IUneditableAll<IAuthority> Authorities => _authorities;

	public IUneditableAll<IArmourType> ArmourTypes => _armourTypes;

	public IUneditableAll<IAuctionHouse> AuctionHouses => _auctionHouses;

	public IUneditableAll<IAutobuilderRoom> AutobuilderRooms => _autobuilderRooms;

	public IUneditableAll<IAutobuilderArea> AutobuilderAreas => _autobuilderAreas;

	public IUneditableAll<IBank> Banks => _banks;

	public IUneditableAll<IBankAccount> BankAccounts => _bankAccounts;
	public IUneditableAll<IBankAccountType> BankAccountTypes => _bankAccountTypes;

	public IUneditableAll<IBloodtype> Bloodtypes => _bloodtypes;

	public IUneditableAll<IBloodtypeAntigen> BloodtypeAntigens => _bloodtypeAntigens;

	public IUneditableAll<IBloodModel> BloodModels => _bloodModels;

	public IUneditableAll<IBoard> Boards => _boards;

	public IUneditableAll<IBody> Bodies => _bodies;

	public IUneditableAll<IBodypart> BodypartPrototypes => _bodypartPrototypes;

	public IUneditableAll<IBodypartGroupDescriber> BodypartGroupDescriptionRules => _bodypartGroupDescriptionRules;

	public IUneditableAll<IBodypartShape> BodypartShapes => _bodypartShapes;

	public IUneditableAll<IBodyPrototype> BodyPrototypes => _bodyPrototypes;

	public IUneditableAll<IButcheryProduct> ButcheryProducts => _butcheryProducts;

	public IUneditableAll<IRaceButcheryProfile> RaceButcheryProfiles => _raceButcheryProfiles;

	public IUneditableAll<ICalendar> Calendars => _calendars;

	public IUneditableAll<ICelestialObject> CelestialObjects => _celestialObjects;

	public IUneditableRevisableAll<ICellOverlayPackage> CellOverlayPackages => _cellOverlayPackages;

	public IUneditableAll<ICell> Cells => _cells;

	public IUneditableAll<IChannel> Channels => _channels;

	public IUneditableAll<ICharacteristicProfile> CharacteristicProfiles => _characteristicProfiles;

	public IUneditableAll<ICharacteristicDefinition> Characteristics => _characteristics;

	public IUneditableAll<ICharacteristicValue> CharacteristicValues => _characteristicValues;

	public IUneditableAll<IChargenAdvice> ChargenAdvices => _chargenAdvices;

	public IUneditableRevisableAll<ICraft> Crafts => _crafts;

	public IUneditableAll<IDrug> Drugs => _drugs;

	public IUneditableAll<ICharacter> Guests => _guests;

	/// <summary>
	/// This collection contains all PC Characters that have been loaded into the game.
	/// </summary>
	public IUneditableAll<ICharacter> Characters => _characters;

	public IUneditableAll<ICharacterCombatSettings> CharacterCombatSettings => _characterCombatSettings;

	public IUneditableAll<IChargenResource> ChargenResources => _chargenResources;

	public IUneditableAll<ICharacterIntroTemplate> CharacterIntroTemplates => _characterIntroTemplates;

	public IUneditableAll<ICheck> Checks => _checks;

	public IUneditableAll<IClan> Clans => _clans;

	public IUneditableAll<IClimateModel> ClimateModels => _climateModels;

	public IUneditableRevisableAll<IDisfigurementTemplate> DisfigurementTemplates => _disfigurementTemplates;

	public IUneditableAll<IElection> Elections => _elections;

	public IUneditableAll<IRegionalClimate> RegionalClimates => _regionalClimates;

	public IUneditableAll<ISeason> Seasons => _seasons;

	public IUneditableAll<IWeatherController> WeatherControllers => _weatherControllers;

	public IUneditableAll<IWeatherEvent> WeatherEvents => _weatherEvents;

	public IUneditableAll<IDrawing> Drawings => _drawings;

	public IUneditableAll<IClock> Clocks => _clocks;

	public IUneditableAll<IColour> Colours => _colours;

	public IUneditableAll<ICorpseModel> CorpseModels => _corpseModels;

	public IUneditableAll<ICulture> Cultures => _cultures;

	public IUneditableAll<ICurrency> Currencies => _currencies;

	public IEnumerable<IDefaultHook> DefaultHooks => _defaultHooks;

	public IUneditableAll<IDream> Dreams => _dreams;

	public IUneditableAll<IEntityDescriptionPattern> EntityDescriptionPatterns => _entityDescriptionPatterns;

	public IUneditableAll<IEthnicity> Ethnicities => _ethnicities;

	public IUneditableAll<IEnforcementAuthority> EnforcementAuthorities => _enforcementAuthorities;
	public IUneditableAll<ILegalAuthority> LegalAuthorities => _legalAuthorities;
	public IUneditableAll<ILegalClass> LegalClasses => _legalClasses;
	public IUneditableAll<IWitnessProfile> WitnessProfiles => _witnessProfiles;
	public IUneditableAll<ILaw> Laws => _laws;
	public IUneditableAll<ICrime> Crimes => _crimes;

	public IUneditableRevisableAll<IForagable> Foragables => _foragables;

	public IUneditableRevisableAll<IForagableProfile> ForagableProfiles => _foragableProfiles;

	public IUneditableAll<IFutureProg> FutureProgs => _futureProgs;

	public IUneditableAll<IGas> Gases => _gases;

	public IUneditableAll<IGrid> Grids => _grids;

	public IUneditableAll<IHealthStrategy> HealthStrategies => _healthStrategies;

	public IUneditableAll<IHearingProfile> HearingProfiles => _hearingProfiles;

	public IUneditableAll<IHeightWeightModel> HeightWeightModels => _heightWeightModels;

	public IUneditableAll<IHelpfile> Helpfiles => _helpfiles;

	public IUneditableAll<IHook> Hooks => _hooks;

	public IUneditableAll<IArtificialIntelligence> AIs => _AIs;

	public IUneditableRevisableAll<INPCTemplate> NpcTemplates => _npcTemplates;
	public IUneditableAll<INPCSpawner> NPCSpawners => _npcSpawners;

	public IUneditableAll<IImprovementModel> ImprovementModels => _improvementModels;

	public IUneditableAll<IGameItemGroup> ItemGroups => _itemGroups;

	public IUneditableRevisableAll<IGameItemComponentProto> ItemComponentProtos => _itemComponentProtos;

	public IUneditableRevisableAll<IGameItemProto> ItemProtos => _itemProtos;

	public IUneditableAll<IGameItem> Items => _items;
	public IUneditableRevisableAll<IGameItemSkin> ItemSkins => _itemSkins;

	public IUneditableAll<IJobListing> JobListings => _jobListings;
	public IUneditableAll<IActiveJob> ActiveJobs => _activeJobs;

	public IUneditableAll<IKnowledge> Knowledges => _knowledges;

	public IUneditableAll<ILanguageDifficultyModel> LanguageDifficultyModels => _languageDifficultyModels;

	public IUneditableAll<ILanguage> Languages => _languages;

	public IUneditableAll<ILimb> Limbs => _limbs;

	public IUneditableAll<ILiquid> Liquids => _liquids;

	public IUneditableAll<ITemporalListener> Listeners => _listeners;

	public IUneditableAll<IMagicSchool> MagicSchools => _magicSchools;
	public IUneditableAll<IMagicCapability> MagicCapabilities => _magicCapabilities;
	public IUneditableAll<IMagicPower> MagicPowers => _magicPowers;
	public IUneditableAll<IMagicResource> MagicResources => _magicResources;
	public IUneditableAll<IMagicResourceRegenerator> MagicResourceRegenerators => _magicResourceRegenerators;
	public IUneditableAll<IMagicSpell> MagicSpells => _magicSpells;

	public IUneditableAll<ISolid> Materials => _materials;

	public IUneditableAll<IMerit> Merits => _merits;

	public IUneditableAll<INameCulture> NameCultures => _nameCultures;

	public IUneditableAll<IRandomNameProfile> RandomNameProfiles => _randomNameProfiles;

	public IUneditableAll<INonCardinalExitTemplate> NonCardinalExitTemplates => _nonCardinalExitTemplates;

	/// <summary>
	/// This collection contains all NPCs that have been loaded into the game.
	/// </summary>
	public IUneditableAll<ICharacter> NPCs => _NPCs;

	public IUneditableAll<IPatrol> Patrols => _patrols;

	public IUneditableAll<IPopulationBloodModel> PopulationBloodModels => _populationBloodModels;

	public IUneditableAll<IProgSchedule> ProgSchedules => _progSchedules;

	public IUneditableRevisableAll<IProject> Projects => _projects;
	public IUneditableAll<IActiveProject> ActiveProjects => _activeProjects;
	public IUneditableAll<IProperty> Properties => _properties;
	public IUneditableAll<IRace> Races => _races;

	public IUneditableAll<IRangedCover> RangedCovers => _rangedCovers;

	public IUneditableAll<IRangedWeaponType> RangedWeaponTypes => _rangedWeaponTypes;

	public IUneditableAll<IRoom> Rooms => _rooms;

	public IUneditableAll<IScript> Scripts => _scripts;

	public IUneditableAll<IShard> Shards => _shards;

	public IUneditableAll<IShieldType> ShieldTypes => _shieldTypes;

	public IUneditableAll<ISkyDescriptionTemplate> SkyDescriptionTemplates => _skyDescriptionTemplates;

	public IUneditableAll<IStackDecorator> StackDecorators => _stackDecorators;

	public IUneditableAll<ISurgicalProcedure> SurgicalProcedures => _surgicalProcedures;

	public IUneditableAll<ITag> Tags => _tags;

	public IUneditableAll<ITerrain> Terrains => _terrains;

	public IUneditableAll<ITraitValueDecorator> TraitDecorators => _traitDecorators;

	public IUneditableAll<ITraitDefinition> Traits => _traits;

	public IUneditableAll<ITraitExpression> TraitExpressions => _traitExpressions;

	public IUneditableAll<IWearProfile> WearProfiles => _wearProfiles;

	public IUneditableAll<IZone> Zones => _zones;

	public IEnumerable<ISocial> Socials => _socials;

	public IUneditableAll<IChargenRole> Roles => _roles;

	public IUneditableAll<IWeaponType> WeaponTypes => _weaponTypes;

	public IUneditableAll<IWearableSize> WearableSizes => _wearableSizes;

	public IUneditableAll<IWriting> Writings => _writings;

	public IUneditableAll<IWeaponAttack> WeaponAttacks => _weaponAttacks;
	public IUneditableAll<IAuxillaryCombatAction> AuxillaryCombatActions => _auxillaryCombatActions;

	public IUneditableAll<IShop> Shops => _shops;

	public IUneditableAll<IEconomicZone> EconomicZones => _economicZones;

	public IUneditableAll<IGroupAITemplate> GroupAITemplates => _groupAITemplates;

	public IUneditableAll<IGroupAI> GroupAIs => _groupAIs;

	public IUneditableAll<ILineOfCreditAccount> LineOfCreditAccounts => _lineOfCreditAccounts;

	#endregion IUneditableAll<T> Declarations

	#region Miscellaneous Game Level Variables

	public IChargenStoryboard ChargenStoryboard { get; private set; }

	public RankedRange<ICharacteristicValue> RelativeHeightDescriptors { get; private set; }

	public ILanguageScrambler LanguageScrambler { get; private set; }
	public ILanguageScrambler ElectronicLanguageScrambler { get; private set; }

	public IExitManager ExitManager { get; private set; }

	public IGameStatistics GameStatistics { get; private set; }
	public ILightModel LightModel { get; private set; }
	public IVariableRegister VariableRegister { get; private set; }
	public ILogManager LogManager { get; private set; }
	public ICombatMessageManager CombatMessageManager { get; private set; }
	public IDiscordConnection DiscordConnection { get; private set; }

	public Dictionary<long, List<ICharacter>> CachedBodyguards { get; } = new();

	private IFutureProg _alwaysTrueProg;

	public IFutureProg AlwaysTrueProg
	{
		get
		{
			if (_alwaysTrueProg is null)
			{
				_alwaysTrueProg = FutureProgs.Get(GetStaticLong("AlwaysTrueProg"));
			}

			return _alwaysTrueProg;
		}
	}

	private IFutureProg _alwaysFalseProg;

	public IFutureProg AlwaysFalseProg
	{
		get
		{
			if (_alwaysFalseProg is null)
			{
				_alwaysFalseProg = FutureProgs.Get(GetStaticLong("AlwaysFalseProg"));
			}

			return _alwaysFalseProg;
		}
	}

	#endregion

	#region Game-level InventoryPlans

	private IInventoryPlanTemplate _bindInventoryPlanTemplate;

	public IInventoryPlanTemplate BindInventoryPlanTemplate
	{
		get
		{
			return _bindInventoryPlanTemplate ??= new InventoryPlanTemplate(this, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(this,
						DesiredItemState.Held,
						0,
						0,
						item => item.GetItemType<ITreatment>()?.IsTreatmentType(TreatmentType.Trauma) ?? false,
						null,
						fitnessscorer: item =>
							(int)item.GetItemType<ITreatment>().GetTreatmentDifficulty(Difficulty.Normal),
						originalReference: "treatment")
				})
			});
		}
	}

	private IInventoryPlanTemplate _cleanWoundInventoryPlanTemplate;

	public IInventoryPlanTemplate CleanWoundInventoryPlanTemplate
	{
		get
		{
			return _cleanWoundInventoryPlanTemplate ??= new InventoryPlanTemplate(this, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(this,
						DesiredItemState.Held,
						0,
						0,
						item =>
							(item.GetItemType<ITreatment>()?.IsTreatmentType(TreatmentType.Clean) ?? false) ||
							(item.GetItemType<ITreatment>()?.IsTreatmentType(TreatmentType.Antiseptic) ?? false),
						null,
						fitnessscorer: item =>
							item.GetItemType<ITreatment>() is ITreatment tr
								? (tr.IsTreatmentType(TreatmentType.Antiseptic) ? 100.0 : 1.0) *
								  (int)tr.GetTreatmentDifficulty(Difficulty.Normal)
								: 0.0,
						originalReference: "treatment")
				})
			});
		}
	}

	private IInventoryPlanTemplate _sutureInventoryPlanTemplate;

	public IInventoryPlanTemplate SutureInventoryPlanTemplate
	{
		get
		{
			return _sutureInventoryPlanTemplate ??= new InventoryPlanTemplate(this, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(this,
						DesiredItemState.Held,
						0,
						0,
						item => item.GetItemType<ITreatment>()
						            ?.IsTreatmentType(TreatmentType.Close) ??
						        false,
						null,
						fitnessscorer: item =>
							(int)item.GetItemType<ITreatment>()
							         .GetTreatmentDifficulty(Difficulty.Normal),
						originalReference: "treatment")
				})
			});
		}
	}

	private IInventoryPlanTemplate _tendInventoryPlanTemplate;

	public IInventoryPlanTemplate TendInventoryPlanTemplate
	{
		get
		{
			return _tendInventoryPlanTemplate ??= new InventoryPlanTemplate(this, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(this,
						DesiredItemState.Held,
						0,
						0,
						item => item.GetItemType<ITreatment>()
						            ?.IsTreatmentType(TreatmentType.Tend) ??
						        false,
						null,
						fitnessscorer: item =>
							(int)item.GetItemType<ITreatment>()
							         .GetTreatmentDifficulty(Difficulty.Normal),
						originalReference: "treatment")
				})
			});
		}
	}

	#endregion

	#region Static Configs and Strings

	public IEnumerable<string> StaticStringNames => _staticStrings.Keys.AsEnumerable()
	                                                              .Concat(DefaultStaticSettings.DefaultStaticStrings
		                                                              .Keys).Distinct();

	public IEnumerable<string> StaticConfigurationNames => _staticConfigurations.Keys.AsEnumerable()
		.Concat(DefaultStaticSettings.DefaultStaticConfigurations.Keys).Distinct();

	public void UpdateStaticString(string whichString, string newValue)
	{
		_staticStrings[whichString] = newValue;
	}

	public void UpdateStaticConfiguration(string whichConfiguration, string newValue)
	{
		_staticConfigurations[whichConfiguration] = newValue;
		_staticBools.Remove(whichConfiguration);
		_staticDoubles.Remove(whichConfiguration);
		_staticInts.Remove(whichConfiguration);
		_staticLongs.Remove(whichConfiguration);

		// Special values that need other actions
		if (whichConfiguration.EqualTo("DisplayProgsInDarkMode"))
		{
			foreach (var prog in FutureProgs)
			{
				prog.ColouriseFunctionText();
			}

			return;
		}
	}

	public string GetStaticConfiguration(string whichConfiguration)
	{
		if (_staticConfigurations.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		if (DefaultStaticSettings.DefaultStaticConfigurations.TryGetValue(whichConfiguration, out returnValue))
		{
			_staticConfigurations[whichConfiguration] = returnValue;
			using (new FMDB())
			{
				FMDB.Context.StaticConfigurations.Add(new Models.StaticConfiguration
					{ SettingName = whichConfiguration, Definition = returnValue });
				FMDB.Context.SaveChanges();
			}

			return returnValue;
		}

		throw new ApplicationException(
			$"Undefined Static Configuration with no default Requested in Futuremud.GetStaticConfiguration: {whichConfiguration}");
	}

	public string GetStaticString(string whichString)
	{
		if (_staticStrings.TryGetValue(whichString, out var returnValue))
		{
			return returnValue;
		}

		if (DefaultStaticSettings.DefaultStaticStrings.TryGetValue(whichString, out returnValue))
		{
			Console.WriteLine(
				$"Warning: Had to use default value of static string '{whichString}'. Consider setting a value for this yourself.");
			_staticStrings[whichString] = returnValue;
			using (new FMDB())
			{
				FMDB.Context.StaticStrings.Add(new Models.StaticString(){ Id = whichString, Text = returnValue});
				FMDB.Context.SaveChanges();
			}
			return returnValue;
		}

		throw new ApplicationException("Undefined Static String Requested in Futuremud.GetStaticString - " +
		                               whichString);
	}

	private readonly Dictionary<string, bool> _staticBools = new();

	public bool GetStaticBool(string whichConfiguration)
	{
		if (_staticBools.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		_staticBools[whichConfiguration] = bool.Parse(GetStaticConfiguration(whichConfiguration));
		return _staticBools[whichConfiguration];
	}

	private readonly Dictionary<string, double> _staticDoubles = new();

	public double GetStaticDouble(string whichConfiguration)
	{
		if (_staticDoubles.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		_staticDoubles[whichConfiguration] = double.Parse(GetStaticConfiguration(whichConfiguration));
		return _staticDoubles[whichConfiguration];
	}

	private readonly Dictionary<string, decimal> _staticDecimals = new();

	public decimal GetStaticDecimal(string whichConfiguration)
	{
		if (_staticDecimals.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		_staticDecimals[whichConfiguration] = decimal.Parse(GetStaticConfiguration(whichConfiguration));
		return _staticDecimals[whichConfiguration];
	}

	private readonly Dictionary<string, int> _staticInts = new();

	public int GetStaticInt(string whichConfiguration)
	{
		if (_staticInts.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		_staticInts[whichConfiguration] = int.Parse(GetStaticConfiguration(whichConfiguration));
		return _staticInts[whichConfiguration];
	}

	private readonly Dictionary<string, long> _staticLongs = new();

	public long GetStaticLong(string whichConfiguration)
	{
		if (_staticLongs.TryGetValue(whichConfiguration, out var returnValue))
		{
			return returnValue;
		}

		_staticLongs[whichConfiguration] = long.Parse(GetStaticConfiguration(whichConfiguration));
		return _staticLongs[whichConfiguration];
	}

	#endregion
}