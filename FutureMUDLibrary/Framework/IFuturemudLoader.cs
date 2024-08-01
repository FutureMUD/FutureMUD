namespace MudSharp.Framework {
    public interface IFuturemudLoader {
        void LoadFromDatabase();
        void LoadClimate();
        void LoadAuthorityGroups();
        void LoadTerrains();
        void LoadNonCardinalExitTemplates();
        void LoadCellOverlayPackages();
        void LoadSkyDescriptionTemplates();
        void LoadWorld();
        void LoadWorldItems();
        void LoadEntityDescriptions();
        void LoadClans();
        void LoadCurrencies();
        void LoadMaterials();
        void LoadForagables();
        void LoadCharacteristics();
        void LoadBodies();
        void LoadBodypartShapes();
        void LoadChannels();
        void LoadHooks();
        void LoadAIs();
        void LoadGroupAIs();
        void LoadNPCTemplates();
        void LoadChargenResources();
        void LoadHeightWeightModels();
        void LoadHearingProfiles();
        void LoadEthnicities();
        void LoadCultures();
        void LoadRaces();
        void LoadLanguages();
        void LoadScripts();
        void LoadWritings();
        void LoadDrawings();
        void LoadLanguageDifficultyModels();
        void LoadEntityDescriptionPatterns();
        void LoadColours();
        void LoadTags();
        void LoadWearProfiles();
        void LoadStackDecorators();
        void LoadGameItemComponentProtos();
        void LoadGameItemProtos();
        void LoadGameItemSkins();
        void LoadGameItemGroups();
        void LoadTraits();
        void LoadTraitDecorators();

        void LoadTraitExpressions();
        void LoadImprovementModels();
        void LoadChecks();
        void LoadUnits();
        void LoadCalendars();
        void LoadClocks();
        void LoadTemporalListeners();
        void LoadCelestials();
        void LoadHelpFiles();
        void LoadFutureProgs();
        void LoadScriptedEvents();
        void LoadRoles();
        void LoadChargen();
        void LoadNPCs();
        void LoadStaticValues();
        void LoadSocials();
        void LoadCorpseModels();
        void LoadHealthStrategies();
        void LoadMerits();
        void LoadDreams();
        void LoadProgSchedules();
        void LoadArmourTypes();
        void LoadWeaponTypes();
        void LoadShieldTypes();
        void LoadCharacterCombatSettings();
        void LoadDrugs();
        void LoadKnowledges();
        void LoadSurgery();
        void LoadCover();
        void LoadCrafts();
        void LoadButchering();
        void LoadAutobuilderTemplates();
        void LoadBloodtypes();
        void LoadCharacterIntroTemplates();
        void LoadChargenAdvices();
        void LoadMagic();
        void LoadDisfigurements();

        public void ReleasePrimedGameItems();
        public void PrimeGameItems();

        void LoadBoards();

        void LoadEconomy();

        void LoadProjects();
        void LoadJobs();
        void LoadGrids();
        void LoadLegal();
        void LoadNewPlayerHints();
        void LoadMarkets();
        void LoadTracks();

        public string GetStaticConfiguration(string whichConfiguration);
    }
}