using System;
using System.Collections.Generic;
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
using MudSharp.Commands.Trees;
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
using MudSharp.Discord;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Effects;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Health;
using MudSharp.Help;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Movement;
using MudSharp.Network;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.AI.Groups;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Light;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Dreams;
using MudSharp.RPG.Hints;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Law;
using MudSharp.RPG.Merits;
using MudSharp.RPG.ScriptedEvents;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Butchering;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Framework
{
    [Flags]
    public enum MaintenanceModeSetting {
        None = 0,
        NoLogin = 1 << 0,
        NoChargen = 1 << 1,
        NoAccountLogin = 1 << 2,
        NoGuests = 1 << 3
    }

    public interface IFuturemud
    {
        void ReleasePrimedGameItems();
        void PrimeGameItems();
        /// <summary>
        /// Loads all Player Characters that are not currently in the game, adds them to the game world, and returns a list of the PCs that were loaded
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICharacter> LoadAllPlayerCharacters();
        IUneditableAll<IAccent> Accents { get; }
        IUneditableAll<IAccount> Accounts { get; }

        /// <summary>
        /// This collection contains all Actor (NPCs and Characters) that have been loaded into the game
        /// </summary>
        IUneditableAll<ICharacter> Actors { get; }

        IUneditableAll<ICharacter> CachedActors { get; }
        IUneditableAll<IAmmunitionType> AmmunitionTypes { get; }
        IUneditableAll<IArea> Areas { get; }
        IUneditableAll<IAuctionHouse> AuctionHouses { get; }
        IUneditableAll<IAuthority> Authorities { get; }
        IUneditableAll<IArmourType> ArmourTypes { get; }
        IUneditableAll<IAutobuilderRoom> AutobuilderRooms { get; }
        IUneditableAll<IAutobuilderArea> AutobuilderAreas { get; }
        IUneditableAll<IBank> Banks { get; }
        IUneditableAll<IBankAccount> BankAccounts { get; }
        IUneditableAll<IBankAccountType> BankAccountTypes { get; }
        IUneditableAll<IBloodtype> Bloodtypes { get; }
        IUneditableAll<IBloodtypeAntigen> BloodtypeAntigens { get; }
        IUneditableAll<IBloodModel> BloodModels { get; }
        IUneditableAll<IBoard> Boards { get; }
        IUneditableAll<IBody> Bodies { get; }
        IUneditableAll<IBodypart> BodypartPrototypes { get; }
        IUneditableAll<IBodypartGroupDescriber> BodypartGroupDescriptionRules { get; }
        IUneditableAll<IBodypartShape> BodypartShapes { get; }
        IUneditableAll<IBodyPrototype> BodyPrototypes { get; }
        IUneditableAll<IButcheryProduct> ButcheryProducts { get; }
        IUneditableAll<IRaceButcheryProfile> RaceButcheryProfiles { get; }
        IUneditableAll<ICalendar> Calendars { get; }
        IUneditableAll<ICelestialObject> CelestialObjects { get; }
        IUneditableRevisableAll<ICellOverlayPackage> CellOverlayPackages { get; }
        IUneditableAll<ICell> Cells { get; }
        IUneditableAll<IChannel> Channels { get; }
        IUneditableAll<ICharacteristicProfile> CharacteristicProfiles { get; }
        IUneditableAll<ICharacteristicDefinition> Characteristics { get; }
        IUneditableAll<ICharacteristicValue> CharacteristicValues { get; }
        IUneditableAll<IChargenAdvice> ChargenAdvices { get; }
        IUneditableAll<ICoin> Coins { get; }
        IUneditableAll<ICombatArena> CombatArenas { get; }
        IUneditableRevisableAll<ICraft> Crafts { get; }
        IUneditableAll<IDrug> Drugs { get; }
        IUneditableAll<ICharacter> Guests { get; }

        /// <summary>
        /// This collection contains all PC Characters that have been loaded into the game.
        /// </summary>
        IUneditableAll<ICharacter> Characters { get; }

        IUneditableAll<ICharacterCombatSettings> CharacterCombatSettings { get; }
        IUneditableAll<IChargenResource> ChargenResources { get; }
        IUneditableAll<ICharacterIntroTemplate> CharacterIntroTemplates { get; }
        IUneditableAll<ICheck> Checks { get; }
        IUneditableAll<IClan> Clans { get; }
        IUneditableAll<IClimateModel> ClimateModels { get; }
        IUneditableRevisableAll<IDisfigurementTemplate> DisfigurementTemplates { get; }
        IUneditableAll<IElection> Elections { get; }
        IUneditableAll<IRegionalClimate> RegionalClimates { get; }
        IUneditableAll<ISeason> Seasons { get; }
        IUneditableAll<IWeatherController> WeatherControllers { get; }
        IUneditableAll<IWeatherEvent> WeatherEvents { get; }
        IUneditableAll<IDrawing> Drawings { get; }
        IUneditableAll<IClock> Clocks { get; }
        IUneditableAll<IColour> Colours { get; }
        IUneditableAll<ICorpseModel> CorpseModels { get; }
        IUneditableAll<ICulture> Cultures { get; }
        IUneditableAll<ICurrency> Currencies { get; }
        IEnumerable<IDefaultHook> DefaultHooks { get; }
        IUneditableAll<IDream> Dreams { get; }
        IUneditableAll<IEntityDescriptionPattern> EntityDescriptionPatterns { get; }
        IUneditableAll<IEthnicity> Ethnicities { get; }
        IUneditableAll<IEnforcementAuthority> EnforcementAuthorities { get; }
        IUneditableAll<ILegalAuthority> LegalAuthorities { get; }
        IUneditableAll<ILegalClass> LegalClasses { get; }
        IUneditableAll<IWitnessProfile> WitnessProfiles { get; }
        IUneditableAll<ILaw> Laws { get; }
        IUneditableAll<ICrime> Crimes { get; }
        IUneditableRevisableAll<IForagable> Foragables { get; }
        IUneditableRevisableAll<IForagableProfile> ForagableProfiles { get; }
        IUneditableAll<IFutureProg> FutureProgs { get; }
        IUneditableAll<IGas> Gases { get; }
        IUneditableAll<IGrid> Grids { get; }
        IUneditableAll<IHealthStrategy> HealthStrategies { get; }
        IUneditableAll<IHearingProfile> HearingProfiles { get; }
        IUneditableAll<IHeightWeightModel> HeightWeightModels { get; }
        IUneditableAll<IHelpfile> Helpfiles { get; }
        IUneditableAll<IHook> Hooks { get; }
        IUneditableAll<IArtificialIntelligence> AIs { get; }
        IUneditableRevisableAll<INPCTemplate> NpcTemplates { get; }
        IUneditableAll<IImprovementModel> ImprovementModels { get; }
        IUneditableAll<IGameItemGroup> ItemGroups { get; }
        IUneditableRevisableAll<IGameItemComponentProto> ItemComponentProtos { get; }
        IUneditableRevisableAll<IGameItemProto> ItemProtos { get; }
        IUneditableAll<IGameItem> Items { get; }
        IUneditableRevisableAll<IGameItemSkin> ItemSkins { get; }
        IUneditableAll<IJobListing> JobListings { get; }
        IUneditableAll<IActiveJob> ActiveJobs { get; }
        IUneditableAll<IKnowledge> Knowledges { get; }
        IUneditableAll<ILanguageDifficultyModel> LanguageDifficultyModels { get; }
        IUneditableAll<ILanguage> Languages { get; }
        IUneditableAll<ILimb> Limbs { get; }
        IUneditableAll<ILiquid> Liquids { get; }
        IUneditableAll<ITemporalListener> Listeners { get; }
        IUneditableAll<IMagicSchool> MagicSchools { get; }
        IUneditableAll<IMagicCapability> MagicCapabilities { get; }
        IUneditableAll<IMagicPower> MagicPowers { get; }
        IUneditableAll<IMagicResource> MagicResources { get; }
        IUneditableAll<IMagicResourceRegenerator> MagicResourceRegenerators { get; }
        IUneditableAll<IMagicSpell> MagicSpells { get; }

        IUneditableAll<IMarket> Markets { get; }
        IUneditableAll<IMarketCategory> MarketCategories { get; }
        IUneditableAll<IMarketInfluenceTemplate> MarketInfluenceTemplates { get; }
        IUneditableAll<IMarketInfluence> MarketInfluences { get; }
        IUneditableAll<IMarketPopulation> MarketPopulations { get; }

        IUneditableAll<ISolid> Materials { get; }
        IUneditableAll<IMerit> Merits { get; }
        IUneditableAll<IMoveSpeed> MoveSpeeds { get; }
        IUneditableAll<INameCulture> NameCultures { get; }
        IUneditableAll<INewPlayerHint> NewPlayerHints { get; }

		IUneditableAll<IRandomNameProfile> RandomNameProfiles { get; }
        IUneditableAll<INonCardinalExitTemplate> NonCardinalExitTemplates { get; }

        /// <summary>
        /// This collection contains all NPCs that have been loaded into the game.
        /// </summary>
        IUneditableAll<ICharacter> NPCs { get; }
        IUneditableAll<INPCSpawner> NPCSpawners { get; }
        IUneditableAll<IPatrol> Patrols { get; }
        IUneditableAll<IPopulationBloodModel> PopulationBloodModels { get; }
        IUneditableAll<IProgSchedule> ProgSchedules { get; }
        IUneditableRevisableAll<IProject> Projects { get; }
        IUneditableAll<IActiveProject> ActiveProjects { get; }
        IUneditableAll<IProperty> Properties { get; }
        IUneditableAll<IRace> Races { get; }
        IUneditableAll<IRangedCover> RangedCovers { get; }
        IUneditableAll<IRangedWeaponType> RangedWeaponTypes { get; }
        IUneditableAll<IRoom> Rooms { get; }
        IUneditableAll<IScript> Scripts { get; }
        IUneditableAll<IScriptedEvent> ScriptedEvents { get; }
        IUneditableAll<IShard> Shards { get; }
        IUneditableAll<IShieldType> ShieldTypes { get; }
        IUneditableAll<ISkyDescriptionTemplate> SkyDescriptionTemplates { get; }
        IUneditableAll<IStackDecorator> StackDecorators { get; }
        IUneditableAll<ISurgicalProcedure> SurgicalProcedures { get; }
        IUneditableAll<ITag> Tags { get; }
        IUneditableAll<ITerrain> Terrains { get; }
        IUneditableAll<ITraitValueDecorator> TraitDecorators { get; }
        IUneditableAll<ITraitDefinition> Traits { get; }
        IUneditableAll<ITraitExpression> TraitExpressions { get; }
        IUneditableAll<IWearProfile> WearProfiles { get; }
        IUneditableAll<IZone> Zones { get; }
        IEnumerable<ISocial> Socials { get; }
        IUneditableAll<IChargenRole> Roles { get; }
        IUneditableAll<IWeaponType> WeaponTypes { get; }
        IUneditableAll<IWearableSize> WearableSizes { get; }
        IUneditableAll<IWriting> Writings { get; }
        IUneditableAll<IWeaponAttack> WeaponAttacks { get; }
		IUneditableAll<IAuxiliaryCombatAction> AuxiliaryCombatActions { get; }
		IUneditableAll<IShop> Shops { get; }
        IUneditableAll<IEconomicZone> EconomicZones { get; }
        IUneditableAll<IGroupAITemplate> GroupAITemplates { get; }
        IUneditableAll<IGroupAI> GroupAIs { get; }
        IUneditableAll<ILineOfCreditAccount> LineOfCreditAccounts { get; }
        IChargenStoryboard ChargenStoryboard { get; }
        RankedRange<ICharacteristicValue> RelativeHeightDescriptors { get; }
        ILanguageScrambler LanguageScrambler { get; }
        ILanguageScrambler ElectronicLanguageScrambler { get; }
        IExitManager ExitManager { get; }
        IGameStatistics GameStatistics { get; }
        ILightModel LightModel { get; }
        IVariableRegister VariableRegister { get; }
        ILogManager LogManager { get; }
        ICombatMessageManager CombatMessageManager { get; }
        IDiscordConnection DiscordConnection { get; }
        Dictionary<long, List<ICharacter>> CachedBodyguards { get; }
        IInventoryPlanTemplate BindInventoryPlanTemplate { get; }
        IInventoryPlanTemplate CleanWoundInventoryPlanTemplate { get; }
        IInventoryPlanTemplate SutureInventoryPlanTemplate { get; }
        IInventoryPlanTemplate TendInventoryPlanTemplate { get; }
        string Name { get; }
        IFutureProg AlwaysTrueProg { get; }
        IFutureProg AlwaysFalseProg { get; }
		IFutureProg AlwaysZeroProg { get; }
		IFutureProg AlwaysOneProg { get; }
		IFutureProg AlwaysOneHundredProg { get; }
		IFutureProg AlwaysOneThousandProg { get; }
		IFutureProg AlwaysTenThousandProg { get; }
        IFutureProg UniversalErrorTextProg { get; }

		/// <summary>
		///     Determines whether or not players can log into the game
		/// </summary>
		MaintenanceModeSetting MaintenanceMode { get; set; }

        IServer Server { get; }
        IScheduler Scheduler { get; }
        IEffectScheduler EffectScheduler { get; }
        ISaveManager SaveManager { get; }
        IGameItemComponentManager GameItemComponentManager { get; }
        IClockManager ClockManager { get; }
        IUnitManager UnitManager { get; }
        IHeartbeatManager HeartbeatManager { get; }
        IEnumerable<IPlayerConnection> Connections { get; }

        string GetStaticConfiguration(string whichConfiguration);
        string GetStaticString(string whichString);
        bool GetStaticBool(string whichConfiguration);
        double GetStaticDouble(string whichConfiguration);
        decimal GetStaticDecimal(string whichConfiguration);
        int GetStaticInt(string whichConfiguration);
        long GetStaticLong(string whichConfiguration);
        IEnumerable<string> StaticStringNames { get; }
        IEnumerable<string> StaticConfigurationNames { get; }
        void UpdateStaticString(string whichString, string newValue);
        void UpdateStaticConfiguration(string whichConfiguration, string newValue);
        void InitialiseTypes();
        IPerceivable GetPerceivable(string type, long id);
        T GetPerceivable<T>(long id) where T : class, IPerceivable;
        void RegisterPerceivableType(string type, Func<long, IPerceivable> func);
        void HaltGameLoop();
        IAccount LogIn(string loginName, string password, IPlayerController controller);
        void StartGameLoop();
        IAccount TryAccount(Models.Account account);
        IAccount TryAccount(long accountid);
		void PreloadAccounts();
        void PreloadCharacterNames();

		/// <summary>
		///     Send a text message to every player character in game who meets the criteria specified in the filter function
		/// </summary>
		/// <param name="message">The message to be sent</param>
		/// <param name="filterFunc">The filter function to be applied to characters</param>
		void SystemMessage(string message, Func<ICharacter, bool> filterFunc);

        /// <summary>
        ///     Send a text message to every player character in game, with a filter for admin-only messages
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="adminonly">If true, only sends to admin avatars. Otherwise sends to all players</param>
        void SystemMessage(string message, bool adminonly = false);

        /// <summary>
        ///     Send an Output to the OutputHandler every player character in game who meets the criteria specified in the filter
        ///     function
        /// </summary>
        /// <param name="message">An Output to be handled by the character's OutputHandler</param>
        /// <param name="filterFunc">The filter function to be applied to characters</param>
        void SystemMessage(IOutput message, Func<ICharacter, bool> filterFunc);

        /// <summary>
        ///     Send an Output to the OutputHandler every player character in game, with a filter for admin-only messages
        /// </summary>
        /// <param name="message">An Output to be handled by the character's OutputHandler</param>
        /// <param name="adminonly">If true, only sends to admin avatars. Otherwise sends to all players</param>
        void SystemMessage(IOutput message, bool adminonly = false);

        void AddGuest(ICharacter character);
        void Add(IMoveSpeed speed);
        void Add(IImprovementModel model);
        void Add(ICurrency currency);
        void Add(ICoin coin);
        void Add(ICombatArena arena);
        void Add(IChannel channel);
        void Add(IAuxiliaryCombatAction action);
        void Add(IBoard board);
        void Add(IBodypartShape shape);
        void Add(INPCSpawner spawner);
        void Add(IJobListing listing);
        void Add(IActiveJob job);
        void Add(IAuctionHouse auctionHouse);
        void Add(IProperty property);
        void Add(IGameItemSkin skin);
        void Add(IBank bank);
        void Add(IBankAccount account);
        void Add(IBankAccountType type);
        void Add(IDream dream);
        void Add(IPatrol patrol);
        void Add(IMagicSpell spell);
        void Add(INameCulture culture);
        void Add(ICulture culture);
        void Add(IRace race);
        void Add(IRandomNameProfile profile);
        void Add(ILineOfCreditAccount account);
        void Add(IElection election);
        void Add(ILanguage language);
        void Add(IAccent accent);
        void Add(IDrawing drawing);
        void Add(ILegalAuthority item);
        void Add(ILegalClass item);
        void Add(IEnforcementAuthority item);
        void Add(ILaw law);
        void Add(IWitnessProfile profile);
        void Add(ICrime crime);
        void Add(IGroupAITemplate item);
        void Add(IGroupAI item);
        void Add(ITerrain terrain);
        void Add(IGrid grid);
        void Add(IDisfigurementTemplate template);
        void Add(IActiveProject project);
        void Add(IProject project);
        void Add(IShop shop);
        void Add(IEconomicZone zone);
        void Add(IArea area);
        void Add(IClimateModel model);
        void Add(IRegionalClimate climate);
        void Add(ISeason season);
        void Add(IWeatherEvent weather);
        void Add(IWeatherController controller);
        void Add(ITraitExpression item);
        void Add(IWeaponAttack attack);
        void Add(IMagicSchool school);
        void Add(IMagicCapability capability);
        void Add(IMagicPower power);
        void Add(IMagicResource resource);
        void Add(IMagicResourceRegenerator regenerator);
        void Add(IChargenAdvice advice);
        void Add(ICharacterIntroTemplate template);
        void Add(ICraft craft);
        void Add(IPopulationBloodModel model);
        void Add(IBloodtype type);
        void Add(IBloodtypeAntigen antigen);
        void Add(IBloodModel model);
        void Add(IAutobuilderRoom room);
        void Add(IAutobuilderArea area);
        void Add(IRaceButcheryProfile profile);
        void Add(IButcheryProduct product);
        void Add(IWriting writing);
        void Add(IScript script);
        void Add(ISurgicalProcedure procedure);
        void Add(IWearProfile profile);
        void Add(ILimb limb);
        void Add(IWeaponType type);
        void Add(IRangedWeaponType type);
        void Add(IAmmunitionType type);
        void Add(IWearableSize size);
        void Add(ICharacterCombatSettings setting);
        void Add(IProgSchedule schedule);
        void Add(IMerit merit);
        void Add(IHealthStrategy strategy);
        void Add(IBodypart proto);
        void Add(IForagableProfile foragableProfile);
        void Add(IForagable foragable);
        void Add(IGameItemGroup group);
        void Add(ICorpseModel model);
        void Add(IGas gas);
        void Add(ILiquid liquid);
        void Add(IDefaultHook hook);
        void Add(IKnowledge knowledge);
        void Add(ITag tag);
        void Add(IChargenRole role);
        void Add(ISkyDescriptionTemplate template);
        void Add(IMaterial material);
        void Add(ICell cell);
        void Add(IRoom room);
        void Add(IZone zone);
        void Add(IShard shard);
        void Add(IArtificialIntelligence ai);
        void Add(INPCTemplate template);
        void Add(IHook hook);
        void Add(IEthnicity ethnicity);
        void Add(IEntityDescriptionPattern pattern);
        void Add(INonCardinalExitTemplate template);
        void Add(ICharacteristicDefinition definition);
        void Add(ICharacteristicProfile profile);
        void Add(ICharacteristicValue value);
        void Add(IBody body);
        void Add(ICharacter actor, bool isNPC);
        void Add(IGameItem item);
        void Add(ICellOverlayPackage package);
        void Add(IGameItemProto proto);
        void Add(IGameItemComponentProto proto);
        void Add(ITemporalListener listener);
        void Add(IHelpfile helpfile);
        void Add(ITraitDefinition trait);
        void Add(IClock clock);
        void Add(ICalendar calendar);
        void Add(ICelestialObject celestial);
        void Add(IFutureProg prog);
        void Add(IClan clan);
        void Add(IArmourType type);
        void Add(IScriptedEvent item);
        void Add(INewPlayerHint hint);
        void Add(IMarket market);
        void Add(IMarketCategory category);
        void Add(IMarketInfluenceTemplate item);
        void Add(IMarketInfluence item);
        void Add(IMarketPopulation item);

		ICheck GetCheck(CheckType type);

        /// <summary>
        ///     Returns a character if they exist, but if they do not, loads them but does not add them to the game world
        /// </summary>
        /// <param name="id"></param>
        /// <param name="useCachedValues"></param>
        /// <returns></returns>
        ICharacter TryGetCharacter(long id, bool useCachedValues = false);

        /// <summary>
        /// Tries to find a player character by personal name, and loads them if not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ICharacter TryPlayerCharacterByName(string name);


		IGameItem TryGetItem(Models.GameItem dbitem, bool addToGameworld);
        IGameItem TryGetItem(long id, bool addToGameworld = false);

        void Destroy(object obj);
		void Destroy(ICombatArena arena);
        void Destroy(ICurrency currency);
		void Destroy(ICoin coin);
        void Destroy(ICrime crime);
        void Destroy(INPCSpawner spawner);
        void Destroy(IJobListing listing);
        void Destroy(IActiveJob job);
        void Destroy(IAuctionHouse auctionHouse);
        void Destroy(IProperty property);
        void Destroy(IPatrol patrol);
        void Destroy(IMagicSpell spell);
        void Destroy(ILineOfCreditAccount account);
        void Destroy(IElection election);
        void Destroy(IDrawing drawing);
        void Destroy(IGroupAI item);
        void Destroy(IGroupAITemplate item);
        void Destroy(ITerrain terrain);
        void Destroy(IGrid grid);
        void Destroy(IProject project);
        void Destroy(IDisfigurementTemplate template);
        void Destroy(IActiveProject project);
        void Destroy(IShop shop);
        void Destroy(IEconomicZone zone);
        void Destroy(IArea area);
        void Destroy(IClimateModel model);
        void Destroy(ITraitExpression expression);
        void Destroy(IWeaponAttack attack);
        void Destroy(ICombatMessage message);
        void Destroy(IChargenAdvice advice);
        void Destroy(ICharacterIntroTemplate template);
        void Destroy(ICraft craft);
        void Destroy(IPopulationBloodModel model);
        void Destroy(IBloodtype type);
        void Destroy(IBloodtypeAntigen antigen);
        void Destroy(IBloodModel model);
        void Destroy(IAutobuilderArea area);
        void Destroy(IAutobuilderRoom room);
        void Destroy(IGameItemSkin skin);
        void Destroy(IRaceButcheryProfile profile);
        void Destroy(IButcheryProduct product);
        void Destroy(ISurgicalProcedure procedure);
        void Destroy(IWearProfile profile);
        void Destroy(IWeaponType type);
        void Destroy(ILimb limb);
        void Destroy(IRangedWeaponType type);
        void Destroy(IAmmunitionType type);
        void Destroy(IWearableSize size);
        void Destroy(ICharacterCombatSettings setting);
        void Destroy(IProgSchedule schedule);
        void Destroy(IArmourType type);
        void Destroy(IMerit merit);
        void Destroy(IHealthStrategy strategy);
        void Destroy(IBodypart proto);
        void Destroy(IForagableProfile foragableProfile);
        void Destroy(IForagable foragable);
        void Destroy(ICorpseModel model);
        void Destroy(IGameItemGroup group);
        void Destroy(IKnowledge knowledge);
        void Destroy(IDefaultHook hook);
        void Destroy(ILiquid item);
        void Destroy(IGas gas);
        void Destroy(ITag tag);
        void Destroy(ISkyDescriptionTemplate template);
        void Destroy(IChargenRole role);
        void Destroy(ISolid material);
        void Destroy(INPCTemplate template);
        void Destroy(IArtificialIntelligence ai);
        void Destroy(IHook hook);
        void Destroy(IEthnicity ethnicity);
        void Destroy(IEntityDescriptionPattern pattern);
        void Destroy(IPlayerConnection connection);
        void Destroy(INonCardinalExitTemplate template);
        void Destroy(ICellOverlayPackage package);
        void Destroy(ICharacteristicDefinition definition);
        void Destroy(ICharacteristicProfile profile);
        void Destroy(ICharacteristicValue value);
        void Destroy(IBody body);
        void Destroy(ICharacter actor);
        void Destroy(IHelpfile helpfile);
        void Destroy(IGameItem item);
        void Destroy(IGameItemProto proto);
        void Destroy(IGameItemComponentProto proto);
        void Destroy(IAccount account);
        void Destroy(ICell cell);
        void Destroy(IRoom room);
        void Destroy(IZone zone);
        void Destroy(IShard plane);
        void Destroy(ICelestialObject celestial);
        void Destroy(ITemporalListener listener);
        void Destroy(ICalendar calendar);
        void Destroy(IClock clock);
        void Destroy(ICulture culture);
        void Destroy(INameCulture culture);
        void Destroy(IRace race);
        void Destroy(IBodyPrototype body);
        void Destroy(IBodypartGroupDescriber rule);
        void Destroy(IBodypartShape shape);
        void Destroy(IColour colour);
        void Destroy(ITraitDefinition trait);
        void Destroy(ITraitValueDecorator decorator);
        void Destroy(ICheck check);
        void Destroy(IFutureProg prog);
        void Destroy(IClan clan);
        void Destroy(IWriting writing);
        void Destroy(IScript script);
		void Destroy(IScriptedEvent item);
        void Destroy(INewPlayerHint hint);
        void Destroy(IMarket market);
        void Destroy(IMarketCategory category);
        void Destroy(IMarketInfluenceTemplate template);
        void Destroy(IMarketInfluence influence);
		void Dispose();
        void ForceOutgoingMessages();
        string ToString();
        void Broadcast(string text);
        ICharacterCommandTree RetrieveAppropriateCommandTree(ICharacter character);
    }
}