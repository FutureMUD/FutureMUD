using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Implementations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Construction.Boundary;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Discord;
using MudSharp.Effects.Concrete;
using MudSharp.Email;
using MudSharp.Events.Hooks;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Groups;
using MudSharp.Health.Corpses;
using MudSharp.Health.Strategies;
using MudSharp.Health.Surgery;
using MudSharp.Help;
using MudSharp.Logging;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Powers;
using MudSharp.Magic.Resources;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Light;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using AmmunitionType = MudSharp.Combat.AmmunitionType;
using ArmourType = MudSharp.Combat.ArmourType;
using BodypartShape = MudSharp.Form.Shape.BodypartShape;
using Calendar = MudSharp.TimeAndDate.Date.Calendar;
using Cell = MudSharp.Construction.Cell;
using CellOverlayPackage = MudSharp.Construction.CellOverlayPackage;
using Channel = MudSharp.Communication.Channel;
using CharacteristicDefinition = MudSharp.Form.Characteristics.CharacteristicDefinition;
using CharacteristicProfile = MudSharp.Form.Characteristics.CharacteristicProfile;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;
using Chargen = MudSharp.CharacterCreation.Chargen;
using ChargenRole = MudSharp.CharacterCreation.Roles.ChargenRole;
using Clan = MudSharp.Models.Clan;
using Clock = MudSharp.TimeAndDate.Time.Clock;
using Colour = MudSharp.Form.Colour.Colour;
using Culture = MudSharp.Character.Heritage.Culture;
using Currency = MudSharp.Economy.Currency.Currency;
using DefaultHook = MudSharp.Events.Hooks.DefaultHook;
using Dream = MudSharp.RPG.Dreams.Dream;
using Drug = MudSharp.Health.Drug;
using Effect = MudSharp.Effects.Effect;
using EntityDescriptionPattern = MudSharp.Form.Shape.EntityDescriptionPattern;
using Ethnicity = MudSharp.Character.Heritage.Ethnicity;
using ExternalClanControl = MudSharp.Community.ExternalClanControl;
using Foragable = MudSharp.Work.Foraging.Foragable;
using ForagableProfile = MudSharp.Work.Foraging.ForagableProfile;
using GameItemComponentProto = MudSharp.GameItems.GameItemComponentProto;
using GameItemProto = MudSharp.GameItems.GameItemProto;
using Gas = MudSharp.Form.Material.Gas;
using HeightWeightModel = MudSharp.NPC.Templates.HeightWeightModel;
using Knowledge = MudSharp.RPG.Knowledge.Knowledge;
using Language = MudSharp.Communication.Language.Language;
using LanguageDifficultyModel = MudSharp.Communication.Language.DifficultyModels.LanguageDifficultyModel;
using Limb = MudSharp.Body.PartProtos.Limb;
using Liquid = MudSharp.Form.Material.Liquid;
using Material = MudSharp.Models.Material;
using NameCulture = MudSharp.Character.Name.NameCulture;
using NonCardinalExitTemplate = MudSharp.Construction.Boundary.NonCardinalExitTemplate;
using ProgSchedule = MudSharp.FutureProg.ProgSchedule;
using Race = MudSharp.Character.Heritage.Race;
using RangedCover = MudSharp.Combat.RangedCover;
using Room = MudSharp.Construction.Room;
using Script = MudSharp.Communication.Language.Script;
using Shard = MudSharp.Construction.Shard;
using ShieldType = MudSharp.Combat.ShieldType;
using SkyDescriptionTemplate = MudSharp.Construction.SkyDescriptionTemplate;
using Social = MudSharp.Commands.Socials.Social;
using StackDecorator = MudSharp.GameItems.Decorators.StackDecorator;
using Terrain = MudSharp.Construction.Terrain;
using TraitDefinition = MudSharp.Body.Traits.TraitDefinition;
using WeaponType = MudSharp.Combat.WeaponType;
using WearableSize = MudSharp.GameItems.Inventory.Size.WearableSize;
using WearProfile = MudSharp.GameItems.Inventory.WearProfile;
using Zone = MudSharp.Construction.Zone;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Employment;
using MudSharp.Effects;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Network;
using MudSharp.NPC;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Butchering;
using MudSharp.Work.Projects;
using Craft = MudSharp.Work.Crafts.Craft;
using GameItem = MudSharp.Models.GameItem;
using Helpfile = MudSharp.Help.Helpfile;
using RaceButcheryProfile = MudSharp.Work.Butchering.RaceButcheryProfile;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;
using MudSharp.RPG.Hints;

namespace MudSharp.Framework;

public sealed partial class Futuremud : IFuturemudLoader, IFuturemud, IDisposable
{
	protected List<ICharacterCommandTree> _actorCommandTrees = new();
	protected List<IPlayerConnection> _connections = new();
	public IServer Server { get; protected set; }
	public IScheduler Scheduler { get; protected set; }
	public IEffectScheduler EffectScheduler { get; protected set; }
	public ISaveManager SaveManager { get; protected set; }
	public IGameItemComponentManager GameItemComponentManager { get; protected set; }
	public IClockManager ClockManager { get; protected set; }
	public IUnitManager UnitManager { get; protected set; }
	public IHeartbeatManager HeartbeatManager { get; protected set; }
	public IEnumerable<IPlayerConnection> Connections => _connections;

	void IFuturemudLoader.LoadFromDatabase()
	{
		SaveManager.MudBootingMode = true;
		// TODO: Consider remaking context every 100 - 500 entities for the performance boost
		var sw = new Stopwatch();
		sw.Start();
		GameStatistics = new GameStatistics(this) { LastBootTime = DateTime.UtcNow };
		Console.WriteLine("Booting Futuremud Server...\n");

		Console.WriteLine("========================================");
		Console.WriteLine("Constructing FutureMud...");

		Console.WriteLine("Initialising Non-Database Statics.");
		PositionState.SetupPositions();
		FutureProg.FutureProg.Initialise();

		var game = (IFuturemudLoader)this;

		Console.WriteLine("Ensuring that Database migrations are applied...");
		using (var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		                                                  .UseMySql(FMDB.ConnectionString,
			                                                  ServerVersion.AutoDetect(FMDB.ConnectionString)).Options))
		{
			var migrator = context.GetService<IMigrator>();
			var migrations = context.Database.GetPendingMigrations().ToList();
			var i = 1;
			foreach (var migration in migrations)
			{
				Console.Write($"...Applying migration {i++} of {migrations.Count}: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(migration);
				Console.ForegroundColor = ConsoleColor.White;
				try
				{
					migrator.Migrate(migration);
				}
				catch (Exception e)
				{
					throw new ApplicationException($"Encountered an exception while applying the {migration} migration",
						e);
				}
			}
		}

		Console.WriteLine("Database is up to date.");

		Console.WriteLine("Connecting to Database...");

		using (new FMDB())
		{
#if DEBUG
			var sqlwriter = new StreamWriter("SqlDebugLog.txt");
#endif
			Console.WriteLine("Database connection opened.");

			Console.WriteLine("Setting up Email Server...");
			Console.WriteLine(EmailHelper.SetupEmailClient()
				? "Email Server successfully setup."
				: "Warning! Email Server did not successfully setup!");

			game.LoadStaticValues(); // Definitely call this before all other Loader methods as it caches values that many of them look up later.
			LogManager = new LogManager(this); // Should go as early as possible after LoadStaticValues
			game.LoadAuthorityGroups();
			game.LoadChargenResources();

			VariableRegister = new VariableRegister(this); // must come before LoadFutureProgs

			game.LoadFutureProgs(); // Needs to come after VariableRegister is initialised
			game.LoadScriptedEvents();
			game.LoadTraitExpressions();
			game.LoadTags();
			game.LoadChargenAdvices(); // Needs to come after LoadFutureProgs
			game.LoadGrids();
			game.LoadDrugs(); // Needs to come after LoadFutureProgs
			game.LoadKnowledges(); // Needs to come after LoadFutureProgs
			game.LoadMaterials(); // Needs to come after LoadFutureProgs, LoadTags and LoadDrugs
			game.LoadChannels(); // Needs to come after LoadFutureProgs
			game.LoadHooks(); // Needs to come after LoadFutureProgs
			game.LoadCurrencies(); // Needs to come after LoadFutureProgs
			game.LoadEntityDescriptionPatterns(); // Needs to come after LoadFutureProgs
			game.LoadHelpFiles(); // Needs to come after LoadFutureProgs
			game.LoadEntityDescriptions(); // Needs to come after LoadEntityDescriptionPatterns
			game.LoadColours();
			game.LoadCharacterCombatSettings();
			game.LoadBodypartShapes(); // Must come before LoadCharacteristics
			game.LoadCharacteristics();

			game.LoadDreams();

			// Body Related Loads
			game.LoadArmourTypes(); // Should come before GameItemComponentProtos and LoadBodies

			game.LoadBodies();
			// End Body Related Loads

			// World File Loads

			game.LoadClocks(); // Must come before calendars/celestials/listeners
			game.LoadCalendars();
			game.LoadCelestials();

			game.LoadHearingProfiles(); // must come before LoadWorld(), and after LoadClocks() / LoadCalendars()
			game.LoadCellOverlayPackages();
			game.LoadCover(); // Must come before LoadTerrains
			game.LoadTerrains();
			game.LoadNonCardinalExitTemplates();
			game.LoadSkyDescriptionTemplates();

			InitialiseTypes(); // Presume this needs to happen before LoadWorld but not sure
			Effect.InitialiseEffects(); // Should come before LoadWorld
			PrimeGameItems();
			game.LoadClimate(); // Should come before LoadWorld and after LoadCelestials

			game.LoadWorld();
			// Needs to come after LoadCellOverlayPackages, LoadSkyDescriptionTemplates and LoadTerrains
			// End World File Loads

			game.LoadImprovementModels();
			game.LoadTraitDecorators();
			game.LoadTraits(); // Depends on LoadImprovementModels and LoadTraitDecorators
			game.LoadChecks(); // Depends on LoadTraits

			game.LoadSurgery(); // Needs to come after LoadFutureProgs, LoadKnowledges and LoadTags, and LoadBodies

			game.LoadHeightWeightModels();
			game.LoadHealthStrategies(); // Depends on LoadTraits
			game.LoadCorpseModels(); // Depends on LoadTerrains
			game.LoadButchering(); // Depends on LoadTags, LoadFutureProgs, LoadBodies
			game.LoadBloodtypes();
			game.LoadWeaponTypes(); // Should come before GameItemComponentProtos
			game.LoadRaces(); // Depends on LoadBloodtypes, LoadCorpseModels, LoadHealthStrategies and LoadTraits, LoadArmourTypes, LoadWeaponTypes
			game.LoadBoards(); // Must come before GameItemComponentProtos
			// Game Item Related Loads
			game.LoadWearProfiles(); // Depends on LoadBodies
			game.LoadStackDecorators();

			game.LoadShieldTypes(); // Should come before GameItemComponentProtos and after LoadArmourTypes and LoadTraits
			game.LoadGameItemComponentProtos(); // Depends on LoadWearProfiles and LoadStackDecorators
			game.LoadGameItemGroups(); // Depends on LoadWorld
			game.LoadGameItemProtos(); // Depends on LoadHealthStrategies, LoadGameItemComponentProtos and LoadGameItemGroups
			game.LoadGameItemSkins();

			// End Game Item Related Loads

			game.LoadForagables(); // Depends on LoadGameItemProtos

			game.LoadEthnicities(); // depends on LoadCharacteristics
			game.LoadCultures();

			game.LoadClans(); // Needs to come after LoadCalendars, LoadFutureProgs, LoadCultures and LoadCurrencies

			game.LoadLanguageDifficultyModels(); // Languages need to come after traits
			game.LoadLanguages();
			game.LoadScripts(); // Must come after LoadLanguages

			game.LoadUnits();
			game.LoadAIs(); // Needs to come after LoadFutureProgs and LoadBodies
			game.LoadDisfigurements();
			game.LoadNPCTemplates(); // Needs to come after LoadAIs and LoadHeightWeightModels
			game.LoadWritings(); // Needs to come after LoadScripts and before LoadWorldItems
			game.LoadDrawings(); // Needs to come before LoadWorldItems

			game.LoadMagic(); // Needs to come before LoadMerits
			game.LoadMerits(); // ToDO - where should this be loaded?
			game.LoadProjects(); // Needs to come before LoadNPCs

			Character.Character.InitialiseCharacterClass(this);
			LightModel = PerceptionEngine.Light.LightModel.LoadLightModel(this);

			game.LoadRoles();
			// Needs to come after LoadFutureprogs, LoadClans, LoadCurrencies, LoadMerits and LoadGameItemProtos
			game.LoadCrafts(); // Needs to come after LoadFutureProgs and before LoadWorldItems
			game.LoadEconomy(); // Should come before LoadWorldItems as late as possible
			game.LoadLegal(); // Should come after LoadWorld, LoadEconomy and after LoadFutureProgs
			game.LoadJobs(); // Needs to come after LoadEconomy
			game.LoadWorldItems(); // Depends on LoadWorld and LoadGameItemProtos and LoadRaces
			game.LoadNPCs(); // Needs to come after InitialiseCharacterClass and LightModel loading
			game.LoadGroupAIs(); // Needs to come after LoadNPCs
			ExitManager.PreloadCriticalExits(); // Needs to come after LoadGameItemProtos
			ReleasePrimedGameItems();
			game.LoadSocials();
			game.LoadAutobuilderTemplates();
			game.LoadCharacterIntroTemplates(); // Needs to come after LoadFutureProgs
			game.LoadNewPlayerHints(); // Needs to come after LoadFutureProgs
			game.LoadChargen(); // Needs to come after LoadFutureProgs and LoadRoles

			game.LoadTemporalListeners(); // Should come after everything else is loaded
			game.LoadProgSchedules(); // Should come after everything else is loaded

			SaveManager.MudBootingMode = false;

			// Cells need to do a few things that have to happen outside the boot order. Todo - use reflection to solve problems like this
			Console.WriteLine("Finalising Cells...");
			var cells = FMDB.Context.Cells.Include(x => x.CellsForagableYields).ToList();
			foreach (var cell in _cells)
			{
				cell.PostLoadTasks(cells.FirstOrDefault(x => x.Id == cell.Id));
			}

			Console.WriteLine("Done Finalising Cells.");
			Console.WriteLine("Loading Statistics...");
			GameStatistics.LoadStatistics(game);
			Console.WriteLine("Done Loading Statistics.");
			Console.WriteLine("Loading Maintenance Mode...");
			var maintenanceSetting = FMDB.Context.StaticConfigurations.Find("MaintenanceMode");
			if (maintenanceSetting == null || !maintenanceSetting.Definition.IsInteger())
			{
				MaintenanceMode = MaintenanceModeSetting.None;
			}
			else
			{
				_maintenanceMode = (MaintenanceModeSetting)int.Parse(maintenanceSetting.Definition);
			}

			Console.WriteLine("Done Loading Maintenance Mode.");

			Console.WriteLine("Loading Combat Messages...");
			CombatMessageManager = new CombatMessageManager(this);
			Console.WriteLine("Done Loading Combat Messages.");
			// Setup Combat - todo - where should this live?
			var pType = typeof(ICombat);
			foreach (
				var type in GetAllTypes().Where(x => x.GetInterfaces().Contains(pType))
			)
			{
				var method = type.GetMethod("SetupCombat", BindingFlags.Static | BindingFlags.Public);
				method?.Invoke(null, new object[] { this });
			}
			PreloadAccounts();
			PreloadCharacterNames();
#if DEBUG
			sqlwriter.Close();
#endif
		}

		Console.WriteLine("\nDatabase connection closed.");
		Console.WriteLine("FutureMud constructed.");
		Console.WriteLine("========================================");
		Console.WriteLine("\nScheduling core system tasks...");
		ClockManager.Initialise();
		// Scheduler.AddSchedule(new RepeatingSchedule<Game>(this, this, fm => fm.SaveManager.Flush(), ScheduleType.System, new TimeSpan(0, 0, 10), "Main Save Loop"));
		//Scheduler.AddSchedule(new RepeatingSchedule<Game>(this, this, fm => new Monitoring.DuplicationMonitor(this).AuditCharacters(), ScheduleType.System, TimeSpan.FromHours(1)));
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this, fm =>
			{
				using (new FMDB())
				{
					foreach (var ch in Characters)
					{
						ch.SaveMinutes(null);
					}

					FMDB.Context.SaveChanges();
				}
			},
			ScheduleType.System, new TimeSpan(0, 5, 0), "Main Character Minutes Loop"));
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this, fm =>
			{
				foreach (var project in ActiveProjects.ToList())
				{
					project.DoProjectsTick();
				}
			}, ScheduleType.System, TimeSpan.FromMinutes(GetStaticDouble("ProjectTickMinutes")),
			"Main ActiveProjects Tick"));
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this,
			fm => { fm.GameStatistics.DoPlayerActivitySnapshot(); }, ScheduleType.System, TimeSpan.FromMinutes(15),
			"Update Online Snapshot"));
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this, fm =>
		{
			foreach (var spawner in fm.NPCSpawners)
			{
				spawner.CheckSpawn();
			}
		}, ScheduleType.System, TimeSpan.FromMinutes(5), "Check NPC Spawners"));
		Chargen.SetupChargen(this);
		HeartbeatManager.StartHeartbeatTick();
		EffectScheduler.SetupEffectSaver();
		sw.Stop();
		Console.WriteLine("Total Boot Time: " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString());
		foreach (var npc in NPCs)
		{
			npc.HandleEvent(Events.EventType.NPCOnGameLoadFinished, npc);
		}

		GameStatistics.LastStartupSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
		Console.WriteLine("\nAttempting to connect to Discord Server...");
		DiscordConnection = new DiscordConnection(this);
		if (DiscordConnection.OpenTcpConnection())
		{
			Console.WriteLine("Successfully connected to the Discord Server.");
		}
		else
		{
			Console.WriteLine("Was unable to connect to the Discord Server.");
		}

		Console.WriteLine("\nAttempting to create listening server thread...");
	}

	void IFuturemudLoader.LoadNewPlayerHints()
	{
		Console.WriteLine("\nLoading New Player Hints...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif

		var hints = FMDB.Context.NewPlayerHints
			.AsNoTracking()
			.ToList();
		foreach (var hint in hints)
		{
			_newPlayerHints.Add(new RPG.Hints.NewPlayerHint(hint, this));
		}

#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _newPlayerHints.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Hint" : "Hints");

	}

	void IFuturemudLoader.LoadLegal()
	{
		Console.WriteLine("\nLoading Legal System...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var authorities = FMDB.Context.LegalAuthorities
		                      .Include(x => x.Laws)
		                      .ThenInclude(x => x.Crimes)
		                      .Include(x => x.Laws)
		                      .ThenInclude(x => x.LawsOffenderClasses)
		                      .Include(x => x.Laws)
		                      .ThenInclude(x => x.LawsVictimClasses)
		                      .Include(x => x.LegalClasses)
		                      .Include(x => x.EnforcementAuthorities)
		                      .ThenInclude(x => x.EnforcementAuthoritiesParentAuthoritiesChild)
		                      .Include(x => x.LegalAuthoritiesZones)
		                      .Include(x => x.PatrolRoutes)
		                      .ThenInclude(x => x.PatrolRouteNodes)
		                      .Include(x => x.PatrolRoutes)
		                      .ThenInclude(x => x.PatrolRouteNumbers)
		                      .Include(x => x.PatrolRoutes)
		                      .ThenInclude(x => x.TimesOfDay)
		                      .Include(x => x.Patrols)
		                      .ThenInclude(x => x.PatrolMembers)
		                      .Include(x => x.LegalAuthorityCells)
		                      .Include(x => x.LegalAuthorityJailCells)
		                      .Include(x => x.Fines)
		                      .AsSplitQuery()
		                      .AsNoTracking()
		                      .ToList();
		foreach (var item in authorities)
		{
			_legalAuthorities.Add(new RPG.Law.LegalAuthority(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _legalAuthorities.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Legal Authority" : "Legal Authorities");

#if DEBUG
		sw.Restart();
#endif
		var profiles = FMDB.Context.WitnessProfiles
		                   .Include(x => x.WitnessProfilesCooperatingAuthorities)
		                   .Include(x => x.WitnessProfilesIgnoredCriminalClasses)
		                   .ThenInclude(x => x.LegalClass)
		                   .Include(x => x.WitnessProfilesIgnoredVictimClasses)
		                   .ThenInclude(x => x.LegalClass)
		                   .AsNoTracking().ToList();
		foreach (var item in profiles)
		{
			_witnessProfiles.Add(new RPG.Law.WitnessProfile(item, this));
		}

#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = _witnessProfiles.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Witness Profile" : "Witness Profiles");
	}

	void IFuturemudLoader.LoadGrids()
	{
		Console.WriteLine("\nLoading Grids...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var grids = FMDB.Context.Grids.AsNoTracking().ToList();
		foreach (var grid in grids)
		{
			_grids.Add(GridFactory.LoadGrid(grid, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = grids.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Grid" : "Grids");
	}

	void IFuturemudLoader.LoadDisfigurements()
	{
		Console.WriteLine("\nLoading Disfigurement Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = FMDB.Context.DisfigurementTemplates.Include(x => x.EditableItem).AsNoTracking().ToList();
		foreach (var template in templates)
		{
			_disfigurementTemplates.Add(DisfigurementFactory.LoadTemplate(template, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Disfigurement Template" : "Disfigurement Templates");
	}

	void IFuturemudLoader.LoadEconomy()
	{
		Console.WriteLine("\nLoading Economic Zones...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var zones = FMDB.Context.EconomicZones
		                .Include(x => x.FinancialPeriods)
		                .Include(x => x.EconomicZoneRevenues)
		                .Include(x => x.EconomicZoneShopTaxes)
		                .Include(x => x.ShopFinancialPeriodResults)
		                .Include(x => x.EconomicZoneTaxes)
		                .Include(x => x.ConveyancingLocations)
		                .Include(x => x.JobFindingLocations)
		                .AsNoTracking().ToList();
		foreach (var zone in zones)
		{
			_economicZones.Add(new MudSharp.Economy.EconomicZone(zone, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = zones.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Economic Zone" : "Economic Zones");

		Console.WriteLine("\nLoading Banks...");
#if DEBUG
		sw.Restart();
#endif
		var banks = FMDB.Context.Banks
		                .Include(x => x.BankAccountTypes)
		                .Include(x => x.BankAccounts)
		                .Include(x => x.BankCurrencyReserves)
		                .Include(x => x.BankExchangeRates)
		                .Include(x => x.BankBranches)
		                .Include(x => x.BankManagers)
		                .AsNoTracking()
		                .ToList();
		foreach (var bank in banks)
		{
			_banks.Add(new Economy.Banking.Bank(bank, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = banks.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Bank" : "Banks");

		Console.WriteLine("\nLoading Properties...");
#if DEBUG
		sw.Restart();
#endif
		var properties = FMDB.Context.Properties
		                     .Include(x => x.PropertyLocations)
		                     .Include(x => x.LeaseOrders)
		                     .Include(x => x.SaleOrder)
		                     .Include(x => x.PropertyLeases)
		                     .Include(x => x.PropertyOwners)
		                     .Include(x => x.PropertyKeys)
		                     .Include(x => x.LeaseOrder)
		                     .AsNoTracking()
		                     .ToList();
		foreach (var property in properties)
		{
			_properties.Add(new Economy.Property.Property(property, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = properties.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Property" : "Properties");

		Console.WriteLine("\nLoading Auction Houses...");
#if DEBUG
		sw.Restart();
#endif
		var auctionhouses = FMDB.Context.AuctionHouses
		                        .AsNoTracking()
		                        .ToList();
		foreach (var auctionhouse in auctionhouses)
		{
			_auctionHouses.Add(new Economy.Auctions.AuctionHouse(auctionhouse, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = auctionhouses.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Auction House" : "Auction Houses");

		Console.WriteLine("\nLoading Shops...");
#if DEBUG
		sw.Restart();
#endif
		var shops = FMDB.Context.Shops
		                .Include(x => x.Merchandises)
		                .Include(x => x.ShopTransactionRecords)
		                .Include(x => x.ShopsTills)
		                .Include(x => x.ShopsStoreroomCells)
		                .Include(x => x.LineOfCreditAccounts)
		                .ThenInclude(x => x.AccountUsers)
		                .AsNoTracking()
		                .ToList();
		foreach (var shop in shops)
		{
			_shops.Add(Economy.Shop.LoadShop(shop, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = shops.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Shop" : "Shops");
	}


	void IFuturemudLoader.LoadClimate()
	{
		Console.WriteLine("\nLoading Seasons...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var seasons = FMDB.Context.Seasons.AsNoTracking().ToList();
		foreach (var season in seasons)
		{
			_seasons.Add(new Climate.Season(season, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = seasons.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Season" : "Seasons");

		Console.WriteLine("\nLoading Weather Events...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var weatherevents = FMDB.Context.WeatherEvents.AsNoTracking().ToList();
		foreach (var weatherevent in weatherevents)
		{
			_weatherEvents.Add(Climate.WeatherEventFactory.LoadWeatherEvent(weatherevent, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = seasons.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Weather Event" : "Weather Events");

		Console.WriteLine("\nLoading Climate Models...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var models = FMDB.Context.ClimateModels.AsNoTracking().ToList();
		foreach (var model in models)
		{
			_climateModels.Add(Climate.ClimateModelFactory.LoadClimateModel(model, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = models.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Climate Model" : "Climate Models");

		Console.WriteLine("\nLoading Regional Climates...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var regionals = FMDB.Context.RegionalClimates
		                    .Include(x => x.RegionalClimatesSeasons)
		                    .AsNoTracking().ToList();
		foreach (var regional in regionals)
		{
			_regionalClimates.Add(new Climate.RegionalClimate(regional, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = regionals.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Regional Climate" : "Regional Climates");

		Console.WriteLine("\nLoading Weather Controllers...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var controllers = FMDB.Context.WeatherControllers
		                      .AsNoTracking().ToList();
		foreach (var controller in controllers)
		{
			_weatherControllers.Add(new Climate.WeatherController(controller, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = controllers.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Weather Controller" : "Weather Controllers");
	}


	void IFuturemudLoader.LoadMagic()
	{
		Console.WriteLine("\nLoading Magic Schools...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var schools = FMDB.Context.MagicSchools.AsNoTracking().ToList();
		foreach (var school in schools)
		{
			_magicSchools.Add(new Magic.MagicSchool(school, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = schools.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "School" : "Schools");

		Console.WriteLine("\nLoading Magic Resources...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var resources = FMDB.Context.MagicResources.AsNoTracking().ToList();
		foreach (var resource in resources)
		{
			_magicResources.Add(BaseMagicResource.LoadResource(resource, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = resources.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Resource" : "Resources");

		Console.WriteLine("\nLoading Magic Resource Generators...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var generators = FMDB.Context.MagicGenerators.AsNoTracking().ToList();
		foreach (var generator in generators)
		{
			_magicResourceRegenerators.Add(BaseMagicResourceGenerator.LoadFromDatabase(generator, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = generators.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Generator" : "Generators");

		Console.WriteLine("\nLoading Magic Powers...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var powers = FMDB.Context.MagicPowers.AsNoTracking().ToList();
		foreach (var power in powers)
		{
			_magicPowers.Add(MagicPowerFactory.LoadPower(power, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		Console.WriteLine("\nLoading Magic Spells...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var spells = FMDB.Context.MagicSpells.AsNoTracking().ToList();
		foreach (var spell in spells)
		{
			_magicSpells.Add(new Magic.MagicSpell(spell, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = spells.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Spell" : "Spells");

		Console.WriteLine("\nLoading Magic Capabilities...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var capabilities =
			FMDB.Context.MagicCapabilities
			    .AsNoTracking().ToList();
		foreach (var capability in capabilities)
		{
			_magicCapabilities.Add(Magic.Capabilities.MagicCapabilityFactory.LoadCapability(capability, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = capabilities.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Capability" : "Capabilities");


		foreach (var verb in _magicSchools.Select(x => x.SchoolVerb).Distinct())
		{
			var command = new Command<ICharacter>(MagicModule.MagicGeneric, CharacterState.Conscious,
				PermissionLevel.Any, verb,
				CommandDisplayOptions.None,
				condition: MagicModule.MagicFilterFunction);
			PlayerCommandTree.Instance.Commands.Add(verb, command);
			GuideCommandTree.Instance.Commands.Add(verb, command);
			AdminCommandTree.JuniorAdminCommandTree.Commands.Add(verb, command);
			AdminCommandTree.StandardAdminCommandTree.Commands.Add(verb, command);
			AdminCommandTree.SeniorAdminCommandTree.Commands.Add(verb, command);
			AdminCommandTree.HighAdminCommandTree.Commands.Add(verb, command);
			FounderCommandTree.Instance.Commands.Add(verb, command);
		}
	}

	void IFuturemudLoader.LoadChargenAdvices()
	{
		Console.WriteLine("\nLoading Chargen Advices...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var advices = FMDB.Context.ChargenAdvices.AsNoTracking().ToList();
		foreach (var item in advices)
		{
			_chargenAdvices.Add(new CharacterCreation.ChargenAdvice(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = advices.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Advice" : "Advices");
	}

	void IFuturemudLoader.LoadCharacterIntroTemplates()
	{
		Console.WriteLine("\nLoading Character Intro Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = FMDB.Context.CharacterIntroTemplates
		                    .AsNoTracking().ToList();
		foreach (var item in templates)
		{
			_characterIntroTemplates.Add(new CharacterCreation.CharacterIntroTemplate(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Template" : "Templates");
	}

	void IFuturemudLoader.LoadBloodtypes()
	{
		Console.WriteLine("\nLoading Bloodtype Antigens...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var antigens = FMDB.Context.BloodtypeAntigens.AsNoTracking().ToList();
		foreach (var item in antigens)
		{
			_bloodtypeAntigens.Add(new Health.Bloodtypes.BloodtypeAntigen(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = antigens.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Antigen" : "Antigens");
		Console.WriteLine("\n\nLoading Bloodtypes...");
#if DEBUG
		sw.Restart();
#endif
		var bloodtypes = FMDB.Context.Bloodtypes
		                     .Include(x => x.BloodtypesBloodtypeAntigens)
		                     .AsNoTracking().ToList();
		foreach (var item in bloodtypes)
		{
			_bloodtypes.Add(new Health.Bloodtypes.Bloodtype(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = bloodtypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Bloodtype" : "Bloodtypes");

		Console.WriteLine("\n\nLoading Blood Models...");
#if DEBUG
		sw.Restart();
#endif
		var bloodModels = FMDB.Context.BloodModels
		                      .Include(x => x.BloodModelsBloodtypes)
		                      .AsNoTracking().ToList();
		foreach (var item in bloodModels)
		{
			_bloodModels.Add(new Health.Bloodtypes.BloodModel(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = bloodModels.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Blood Model" : "Blood Models");

		Console.WriteLine("\n\nLoading Population Blood Models...");
#if DEBUG
		sw.Restart();
#endif
		var popBloodModels = FMDB.Context.PopulationBloodModels
		                         .Include(x => x.PopulationBloodModelsBloodtypes)
		                         .AsNoTracking().ToList();
		foreach (var item in popBloodModels)
		{
			_populationBloodModels.Add(new Health.Bloodtypes.PopulationBloodModel(item, this));
		}

#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = antigens.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Population Blood Model" : "Population Blood Models");
	}

	void IFuturemudLoader.LoadAutobuilderTemplates()
	{
		Console.WriteLine("\nLoading Autobuilder Templates...");
		Construction.Autobuilder.AutobuilderFactory.InitialiseAutobuilders();
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		Console.WriteLine("Loading Room Templates...");
		var roomTemplates = FMDB.Context.AutobuilderRoomTemplates.AsNoTracking().ToList();
		foreach (var item in roomTemplates)
		{
			_autobuilderRooms.Add(Construction.Autobuilder.AutobuilderFactory.LoadRoom(item, this));
		}

		var count = _autobuilderRooms.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Room Template" : "Room Templates");

		Console.WriteLine("Loading Area Templates...");
		var areaTemplates = FMDB.Context.AutobuilderAreaTemplates.AsNoTracking().ToList();
		foreach (var item in areaTemplates)
		{
			_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(item, this));
		}

		count = _autobuilderAreas.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Area Template" : "Area Templates");
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		if (_autobuilderAreas.Count < 6)
		{
			if (!_autobuilderAreas.Any(x => x.Name == "Rectangle"))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Rectangle",
					TemplateType = "rectangle",
					Definition =
						@"<Template><ShowCommandByLine><![CDATA[A simple rectangle with a single terrain type]]></ShowCommandByLine></Template>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}

			if (!_autobuilderAreas.Any(x => x.Name.CollapseString().EqualTo("RectangleDiagonals")))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Rectangle Diagonals",
					TemplateType = "rectangle diagonals",
					Definition =
						@"<Template><ShowCommandByLine><![CDATA[A simple rectangle with a single terrain type and diagonal connections]]></ShowCommandByLine></Template>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}

			if (!_autobuilderAreas.Any(x => x.Name.CollapseString().EqualTo("TerrainRectangle")))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Terrain Rectangle",
					TemplateType = "terrain rectangle",
					Definition =
						@"<Definition connect_diagonals=""false""><ShowCommandByLine><![CDATA[A simple rectangle with supplied terrain mask]]></ShowCommandByLine></Definition>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}

			if (!_autobuilderAreas.Any(x => x.Name.CollapseString().EqualTo("TerrainRectangleDiagonals")))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Terrain Rectangle Diagonals",
					TemplateType = "terrain rectangle",
					Definition =
						@"<Definition connect_diagonals=""true""><ShowCommandByLine><![CDATA[A simple rectangle with supplied terrain mask and diagonal exits]]></ShowCommandByLine></Definition>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}

			if (!_autobuilderAreas.Any(x => x.Name.CollapseString().EqualTo("FeatureRectangle")))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Feature Rectangle",
					TemplateType = "terrain feature rectangle",
					Definition =
						@"<Definition connect_diagonals=""false""><ShowCommandByLine><![CDATA[A simple rectangle with a supplied terrain and feature mask]]></ShowCommandByLine></Definition>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}

			if (!_autobuilderAreas.Any(x => x.Name.CollapseString().EqualTo("FeatureRectangleDiagonals")))
			{
				var dbitem = new AutobuilderAreaTemplate
				{
					Name = "Feature Rectangle Diagonals",
					TemplateType = "terrain feature rectangle",
					Definition =
						@"<Definition connect_diagonals=""true""><ShowCommandByLine><![CDATA[A simple rectangle with a supplied terrain and feature mask and diagonal exits]]></ShowCommandByLine></Definition>"
				};
				FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
				FMDB.Context.SaveChanges();
				_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(dbitem, this));
			}
		}
	}

	void IFuturemudLoader.LoadButchering()
	{
		Console.WriteLine("\nLoading Butchery Products...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var products = FMDB.Context.ButcheryProducts
		                   .Include(x => x.ButcheryProductsBodypartProtos)
		                   .Include(x => x.ButcheryProductItems)
		                   .AsNoTracking()
		                   .ToList();
		foreach (var product in products)
		{
			_butcheryProducts.Add(new ButcheryProduct(product, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = products.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Product" : "Products");

#if DEBUG
		sw.Restart();
#endif
		var profiles = FMDB.Context.RaceButcheryProfiles
		                   .Include(x => x.RaceButcheryProfilesBreakdownChecks)
		                   .Include(x => x.RaceButcheryProfilesBreakdownEmotes)
		                   .Include(x => x.RaceButcheryProfilesSkinningEmotes)
		                   .Include(x => x.RaceButcheryProfilesButcheryProducts)
		                   .AsNoTracking()
		                   .ToList();
		foreach (var profile in profiles)
		{
			_raceButcheryProfiles.Add(new RaceButcheryProfile(profile, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = products.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Profile" : "Profiles");
	}

	void IFuturemudLoader.LoadBoards()
	{
		Console.WriteLine("\nLoading boards...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var boards =
			FMDB.Context.Boards
			    .Include(x => x.BoardPosts)
			    .AsNoTracking()
			    .ToList();
		foreach (var board in boards)
		{
			_boards.Add(new Community.Boards.Board(board, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = boards.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Board" : "Boards");
	}

	void IFuturemudLoader.LoadTraitExpressions()
	{
		Console.WriteLine("\nLoading Trait Expressions...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var expressions = FMDB.Context.TraitExpressions
		                      .Include(x => x.TraitExpressionParameters)
		                      .AsNoTracking().ToList();
		foreach (var item in expressions)
		{
			_traitExpressions.Add(new TraitExpression(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = expressions.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Trait Expression" : "Trait Expressions");
	}

	void IFuturemudLoader.LoadCrafts()
	{
		Console.WriteLine("\nLoading Crafts...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var crafts = FMDB.Context.Crafts
		                 .Include(x => x.EditableItem)
		                 .Include(x => x.CraftPhases)
		                 .Include(x => x.CraftInputs)
		                 .Include(x => x.CraftProducts)
		                 .Include(x => x.CraftTools)
		                 .AsNoTracking().ToList();
		foreach (var item in crafts)
		{
			_crafts.Add(new Craft(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = crafts.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Craft" : "Crafts");
	}

	void IFuturemudLoader.LoadScripts()
	{
		Console.WriteLine("\nLoading Scripts...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var scripts = FMDB.Context.Scripts
		                  .Include(x => x.ScriptsDesignedLanguages)
		                  .AsNoTracking().ToList();
		foreach (var item in scripts)
		{
			_scripts.Add(new Script(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = scripts.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Script" : "Scripts");
	}

	void IFuturemudLoader.LoadDrawings()
	{
		Console.WriteLine("\nLoading Drawings...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var drawings = FMDB.Context.Drawings.AsNoTracking().ToList();
		foreach (var item in drawings)
		{
			_drawings.Add(new Communication.Drawing(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = drawings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Drawing" : "Drawings");
	}

	void IFuturemudLoader.LoadWritings()
	{
		Console.WriteLine("\nLoading Writings...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var writings = FMDB.Context.Writings.AsNoTracking().ToList();
		foreach (var item in writings)
		{
			_writings.Add(WritingFactory.LoadWriting(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = writings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Writing" : "Writings");
	}

	void IFuturemudLoader.LoadCover()
	{
		Console.WriteLine("\nLoading Ranged Cover...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var covers = FMDB.Context.RangedCovers.AsNoTracking().ToList();
		foreach (var cover in covers)
		{
			_rangedCovers.Add(new RangedCover(cover));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = covers.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Cover" : "Covers");
	}

	void IFuturemudLoader.LoadSurgery()
	{
		Console.WriteLine("\nLoading Surgical Procedures...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var surgeries = FMDB.Context.SurgicalProcedures
		                    .Include(x => x.SurgicalProcedurePhases)
		                    .AsNoTracking().ToList();
		foreach (var item in surgeries)
		{
			_surgicalProcedures.Add(SurgicalProcedureFactory.Instance.LoadProcedure(this, item));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = surgeries.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Procedure" : "Procedures");
	}

	void IFuturemudLoader.LoadKnowledges()
	{
		Console.WriteLine("\nLoading Knowledges...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var knowledges = FMDB.Context.Knowledges.Include(x => x.KnowledgesCosts).AsNoTracking().ToList();
		foreach (var item in knowledges)
		{
			_knowledges.Add(new Knowledge(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = knowledges.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Knowledge" : "Knowledges");
	}

	void IFuturemudLoader.LoadDrugs()
	{
		Console.WriteLine("\nLoading Drugs...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var drugs = FMDB.Context.Drugs
		                .Include(x => x.DrugsIntensities)
		                .AsNoTracking().ToList();
		foreach (var drug in drugs)
		{
			_drugs.Add(new Drug(drug, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = drugs.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Drug" : "Drugs");
	}

	void IFuturemudLoader.LoadCharacterCombatSettings()
	{
		Console.WriteLine("\nLoading Character Combat Settings...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var combatSettings = FMDB.Context.CharacterCombatSettings.AsNoTracking().ToList();
		foreach (var item in combatSettings)
		{
			_characterCombatSettings.Add(new CharacterCombatSettings(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = combatSettings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Character Combat Setting" : "Character Combat Settings");
	}

	void IFuturemudLoader.LoadArmourTypes()
	{
		Console.WriteLine("\nLoading Armour Types...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var armourTypes = (from type in FMDB.Context.ArmourTypes.AsNoTracking() select type).ToList();
		foreach (var type in armourTypes)
		{
			_armourTypes.Add(new ArmourType(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = armourTypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Armour Type" : "Armour Types");
	}

	void IFuturemudLoader.LoadWeaponTypes()
	{
		Console.WriteLine("\nLoading Weapon Attacks...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var attacks = (from type in FMDB.Context.WeaponAttacks
		                                .AsNoTracking()
		               select type).ToList();
		foreach (var type in attacks)
		{
			_weaponAttacks.Add(Combat.WeaponAttack.LoadWeaponAttack(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = attacks.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Weapon Attack" : "Weapon Attacks");

		Console.WriteLine("\nLoading Auxiliary Combat Action Types...");
		var actions = (from action in FMDB.Context.CombatActions.AsNoTracking() select action).ToList();
		foreach (var action in actions)
		{
			_auxiliaryCombatActions.Add(new AuxiliaryCombatAction(action, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		count = attacks.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Auxiliary Combat Action" : "Auxiliary Combat Actions");

#if DEBUG
		sw.Restart();
#endif

		Console.WriteLine("\nLoading Melee Weapon Types...");
#if DEBUG
		sw.Restart();
#endif
		var weaponTypes = (from type in FMDB.Context.WeaponTypes
		                                    .Include(x => x.WeaponAttacks)
		                                    .AsNoTracking()
		                   select type).ToList();
		foreach (var type in weaponTypes)
		{
			_weaponTypes.Add(new WeaponType(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		count = weaponTypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Weapon Type" : "Weapon Types");

		Console.WriteLine("\nLoading Ranged Weapon Types...");
#if DEBUG
		sw.Restart();
#endif
		var rangedTypes = (from type in FMDB.Context.RangedWeaponTypes.AsNoTracking() select type).ToList();
		foreach (var type in rangedTypes)
		{
			_rangedWeaponTypes.Add(new RangedWeaponTypeDefinition(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = rangedTypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Weapon Type" : "Weapon Types");

		Console.WriteLine("\nLoading Ammunition Types...");
#if DEBUG
		sw.Restart();
#endif
		var ammoTypes = (from type in FMDB.Context.AmmunitionTypes.AsNoTracking() select type).ToList();
		foreach (var type in ammoTypes)
		{
			_ammunitionTypes.Add(new AmmunitionType(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = ammoTypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Ammo Type" : "Ammo Types");
	}

	void IFuturemudLoader.LoadShieldTypes()
	{
		Console.WriteLine("\nLoading Shield Types...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var shieldTypes = (from type in FMDB.Context.ShieldTypes.AsNoTracking() select type).ToList();
		foreach (var type in shieldTypes)
		{
			_shieldTypes.Add(new ShieldType(type, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = shieldTypes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Shield Type" : "Shield Types");
	}

	void IFuturemudLoader.LoadDreams()
	{
		Console.WriteLine("\nLoading Dreams...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var dreams = (from dream in FMDB.Context.Dreams
		                                .Include(x => x.DreamPhases)
		                                .Include(x => x.DreamsAlreadyDreamt)
		                                .Include(x => x.DreamsCharacters)
		                                .AsNoTracking()
		              select dream).ToList();
		foreach (var dream in dreams)
		{
			_dreams.Add(new Dream(dream, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = dreams.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Dream" : "Dreams");
	}

	void IFuturemudLoader.LoadMerits()
	{
		Console.WriteLine("\nLoading Merits and Flaws...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		MeritFactory.InitialiseMerits();
		var merits = (from merit in FMDB.Context.Merits
		                                .Include(x => x.MeritsChargenResources)
		                                .AsNoTracking()
		              select merit).ToList();
		foreach (var merit in merits)
		{
			_merits.Add(MeritFactory.LoadMerit(merit, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _merits.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Merit or Flaw" : "Merits and Flaws");
	}

	void IFuturemudLoader.LoadHealthStrategies()
	{
		Console.WriteLine("\nLoading Health Strategies...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		BaseHealthStrategy.SetupHealthStrategies();
		var strategies = (from strategy in FMDB.Context.HealthStrategies
		                                       .AsNoTracking()
		                  select strategy).ToList();
		foreach (var strategy in strategies)
		{
			_healthStrategies.Add(BaseHealthStrategy.LoadStrategy(strategy, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = strategies.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Health Strategy" : "Health Strategies");
	}

	void IFuturemudLoader.LoadForagables()
	{
		Console.WriteLine("\nLoading Foragables...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var foragables = (from foragable in FMDB.Context.Foragables
		                                        .Include(x => x.EditableItem)
		                                        .AsNoTracking()
		                  select foragable).ToList();
		foreach (var foragable in foragables)
		{
			_foragables.Add(new Foragable(foragable, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = foragables.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Foragable" : "Foragables");

		Console.WriteLine("\nLoading Foragable Profiles...");
#if DEBUG
		sw.Restart();
#endif
		var foragableProfiles =
			(from profile in FMDB.Context.ForagableProfiles
			                     .Include(x => x.EditableItem)
			                     .Include(x => x.ForagableProfilesForagables)
			                     .Include(x => x.ForagableProfilesHourlyYieldGains)
			                     .Include(x => x.ForagableProfilesMaximumYields)
			                     .AsNoTracking()
			 select profile).ToList();
		foreach (var profile in foragableProfiles)
		{
			_foragableProfiles.Add(new ForagableProfile(profile, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = foragableProfiles.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Foragable Profile" : "Foragable Profiles");
	}

	void IFuturemudLoader.LoadCorpseModels()
	{
		Console.WriteLine("\nLoading Corpse Models...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		CorpseModelFactory.RegisterCorpseModelTypeLoaders();
		var models = (from model in FMDB.Context.CorpseModels.AsNoTracking() select model).ToList();
		foreach (var model in models)
		{
			_corpseModels.Add(CorpseModelFactory.LoadCorpseModel(model, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = models.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Corpse Model" : "Corpse Models");
	}

	void IFuturemudLoader.LoadWorldItems()
	{
		Console.WriteLine("\nLoading World Items...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
#if DEBUG
		//FMDB.Context.Database.Log = Console.WriteLine;
#endif
		var cellItems = _bootTimeCachedGameItems.Values.GroupBy(x => x.CellsGameItems.FirstOrDefault());
		var count = 0;
		foreach (var cell in cellItems)
		{
			if (cell.Key == null)
			{
				continue;
			}

			var gcell = _cells.Get(cell.Key.CellId);
			count += gcell?.LoadItems(cell) ?? 0;
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
#if DEBUG
		//FMDB.Context.Database.Log = null;
#endif
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Item" : "Items");

		// Initialise all the grids after the items are definitely loaded and in game
		Console.WriteLine("Initialising Grids...");
		foreach (var grid in _grids)
		{
			grid.LoadTimeInitialise();
		}

		Console.WriteLine("Initialising Shops...");
		foreach (var shop in _shops)
		{
			shop.PostLoadInitialisation();
		}

		Console.WriteLine("Logging in world game items...");
		foreach (var cell in cellItems)
		{
			if (cell.Key == null)
			{
				continue;
			}

			var gcell = _cells.Get(cell.Key.CellId);
			foreach (var item in gcell.GameItems)
			{
				item.Login();
			}
		}
	}

	void IFuturemudLoader.LoadGameItemGroups()
	{
		Console.WriteLine("\nLoading Game Item Groups...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var groups = (from item in FMDB.Context.ItemGroups.Include(x => x.ItemGroupForms).AsNoTracking()
		              select item).ToList();

		foreach (var item in groups)
		{
			_itemGroups.Add(GameItemGroupFactory.CreateGameItemGroup(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = groups.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Item Group" : "Item Groups");
	}

	void IFuturemudLoader.LoadSkyDescriptionTemplates()
	{
		Console.WriteLine("\nLoading Sky Description Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = (from item in FMDB.Context.SkyDescriptionTemplates
		                                  .Include(x => x.SkyDescriptionTemplatesValues)
		                                  .AsNoTracking()
		                 select item).ToList();

		foreach (var item in templates)
		{
			_skyDescriptionTemplates.Add(new SkyDescriptionTemplate(item));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Template" : "Templates");
	}

	void IFuturemudLoader.LoadRoles()
	{
		Console.WriteLine("\nLoading Roles...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var roles = (from item in FMDB.Context.ChargenRoles
		                              .Include(x => x.ChargenAdvicesChargenRoles)
		                              .Include(x => x.ChargenRolesApprovers)
		                              .ThenInclude(x => x.Approver)
		                              .Include(x => x.ChargenRolesClanMemberships)
		                              .ThenInclude(x => x.ChargenRolesClanMembershipsAppointments)
		                              .Include(x => x.ChargenRolesCosts)
		                              .Include(x => x.ChargenRolesCurrencies)
		                              .Include(x => x.ChargenRolesMerits)
		                              .Include(x => x.ChargenRolesTraits)
		                              .AsNoTracking()
		             select item).ToList();

		foreach (var item in roles)
		{
			_roles.Add(new ChargenRole(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = roles.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Role" : "Roles");
	}

	void IFuturemudLoader.LoadMaterials()
	{
		Console.WriteLine("\nLoading Materials...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		Console.WriteLine("\nLoading Solids...");
		var materials = (from item in FMDB.Context.Materials
		                                  .Include(x => x.MaterialsTags)
		                                  .AsNoTracking()
		                 where item.Type == (int)MaterialType.Solid
		                 select item).ToList();

		foreach (var item in materials)
		{
			var newItem = new Solid(item, this);
			_materials.Add(newItem);
		}

		var count = materials.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Solid" : "Solids");

		Console.WriteLine("\nLoading Liquids...");
		var liquids = (from item in FMDB.Context.Liquids
		                                .Include(x => x.LiquidsTags)
		                                .AsNoTracking()
		               select item).ToList();

		foreach (var item in liquids)
		{
			_liquids.Add(new Liquid(item, this));
		}

		count = liquids.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Liquid" : "Liquids");

		Console.WriteLine("\nLoading Gases...");
		var gases = (from item in FMDB.Context.Gases
		                              .Include(x => x.GasesTags)
		                              .AsNoTracking()
		             select item).ToList();

		foreach (var item in gases)
		{
			_gases.Add(new Gas(item, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = gases.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Gas" : "Gases");
	}

	void IFuturemudLoader.LoadSocials()
	{
		Console.WriteLine("\nLoading Socials...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var socials = (from item in FMDB.Context.Socials.AsNoTracking()
		               select item).ToList();

		foreach (var item in socials)
		{
			_socials.Add(new Social(item, this));
		}

		foreach (var tree in _actorCommandTrees)
		{
			tree.Commands.AddSocials(Socials);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = socials.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Social" : "Socials");
	}

	void IFuturemudLoader.LoadChargenResources()
	{
		Console.WriteLine("\nLoading Chargen Resources...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var resources = (from item in FMDB.Context.ChargenResources.AsNoTracking()
		                 select item).ToList();

		var stagingTable = new Dictionary<IChargenResource, ChargenResource>();
		foreach (var item in resources)
		{
			var newItem = ChargenResourceBase.LoadFromDatabase(item);
			stagingTable.Add(newItem, item);
			_chargenResources.Add(newItem);
		}

		foreach (var item in stagingTable)
		{
			item.Key.PerformPostLoadUpdate(item.Value, this);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = resources.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Chargen Resource" : "Chargen Resources");
	}

	void IFuturemudLoader.LoadHeightWeightModels()
	{
		Console.WriteLine("\nLoading Height Weight Models...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var models = (from item in FMDB.Context.HeightWeightModels.AsNoTracking()
		              select item).ToList();
		foreach (var item in models)
		{
			_heightWeightModels.Add(new HeightWeightModel(item));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = models.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Height Weight Model" : "Height Weight Models");
	}


	void IFuturemudLoader.LoadStaticValues()
	{
		Console.WriteLine("\nLoading Static Values...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		_staticStrings.Clear();
		var strings = (from item in FMDB.Context.StaticStrings.AsNoTracking()
		               select item).ToList();

		_staticStrings.Clear();
		foreach (var item in strings)
		{
			_staticStrings.Add(item.Id, item.Text);
		}

		var count = strings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Static String" : "Static Strings");

		var settings = (from item in FMDB.Context.StaticConfigurations.AsNoTracking()
		                select item).ToList();

		_staticConfigurations.Clear();
		foreach (var item in settings)
		{
			_staticConfigurations.Add(item.SettingName, item.Definition);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = settings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Static Setting" : "Static Settings");
	}

	void IFuturemudLoader.LoadChannels()
	{
		Console.WriteLine("\nLoading Channels...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var channels = (from channel in FMDB.Context.Channels
		                                    .Include(x => x.ChannelCommandWords)
		                                    .AsNoTracking()
		                select channel).ToList();

		foreach (var channel in channels)
		{
			_channels.Add(new Channel(channel, this));
		}

		PlayerCommandTree.Instance.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords
			    .Where(x => x.Channel.AddToPlayerCommandTree)
			    .Select(x => x.Word)
			    .AsEnumerable()
			    .Concat(new[] { "channel" })
			    .ToList(), Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		GuestCommandTree.Instance.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Where(x => x.Channel.AddToPlayerCommandTree)
			    .Select(x => x.Word)
			    .AsEnumerable()
			    .Concat(new[] { "channel" })
			    .ToList(), Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		GuideCommandTree.Instance.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Where(x => x.Channel.AddToGuideCommandTree)
			    .Select(x => x.Word)
			    .AsEnumerable()
			    .Concat(new[] { "channel" })
			    .ToList(), Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		AdminCommandTree.StandardAdminCommandTree.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Select(x => x.Word).AsEnumerable().Concat(new[] { "channel" }).ToList(),
			Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		AdminCommandTree.SeniorAdminCommandTree.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Select(x => x.Word).AsEnumerable().Concat(new[] { "channel" }).ToList(),
			Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		AdminCommandTree.JuniorAdminCommandTree.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Select(x => x.Word).AsEnumerable().Concat(new[] { "channel" }).ToList(),
			Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		AdminCommandTree.HighAdminCommandTree.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Select(x => x.Word).AsEnumerable().Concat(new[] { "channel" }).ToList(),
			Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
		FounderCommandTree.Instance.Commands.Add("Channel",
			FMDB.Context.ChannelCommandWords.Select(x => x.Word).AsEnumerable().Concat(new[] { "channel" }).ToList(),
			Channel.ChannelCommandDelegate, options: CommandDisplayOptions.DisplayCommandWords);
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
	}

	void IFuturemudLoader.LoadNonCardinalExitTemplates()
	{
		Console.WriteLine("\nLoading Non Cardinal Exit Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = (from template in FMDB.Context.NonCardinalExitTemplates.AsNoTracking()
		                 select template).ToList();

		foreach (var template in templates)
		{
			_nonCardinalExitTemplates.Add(new NonCardinalExitTemplate(template, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0} Non Cardinal Exit Template{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadNPCTemplates()
	{
		Console.WriteLine("\nLoading NPC Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = (from template in FMDB.Context.NpcTemplates
		                                      .Include(x => x.NpctemplatesArtificalIntelligences)
		                                      .Include(x => x.EditableItem)
		                                      .AsNoTracking()
		                 select template).ToList();

		foreach (var template in templates)
		{
			_npcTemplates.Add(NPCTemplateBase.LoadTemplate(template, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0} NPC Template{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadNPCs()
	{
		Console.WriteLine("\nLoading NPCs...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		DummyAccount.Instance.SetupGameworld(this);
		var npcs = (from npc in FMDB.Context.Npcs
		                            .Include(x => x.NpcsArtificialIntelligences)
		                            .Include(x => x.Character)
		            /*            .Include(x => x.Character.Body.BodiesGameItems)
		                        .Include(x => x.Character.CharactersAccents)
		                        .Include(x => x.Character.CharactersLanguages)
		                        .Include(x => x.Character.Body.BodiesSeveredParts)
		                        .Include(x => x.Character.Body.Characteristics)
		                        .Include(x => x.Character.AlliesCharacter)
		                        .Include(x => x.Character.Body.Traits)
		                        .Include(x => x.Character.PerceiverMerits)
		                        .Include(x => x.Character.Body.PerceiverMerits)
		                        .Include(x => x.Character.Body.BodiesDrugDoses)
		                        .Include(x => x.Character.Body.BodiesProsthetics)
		                        .Include(x => x.Character.Body.BodiesImplants)
		                        .Include(x => x.Character.HooksPerceivables)
		                        .Include(x => x.Character.CharactersChargenRoles)
		                        .Include(x => x.Character.Dubs)
		                        .Include(x => x.Character.CharactersScripts)
		                        .Include(x => x.Character.ActiveProjects)
		                        .Include(x => x.Character.CharacterKnowledges)
		                        .Include(x => x.Character.CharactersMagicResources)
		                        .Include(x => x.Character.Body.Wounds)
		                        .ThenInclude(x => x.Infections)
		            */
		            where npc.Character.State != (int)CharacterState.Dead
		            select npc).ToList();

		foreach (var npc in npcs)
		{
			var newNpc = TryGetCharacter(npc.CharacterId);
			if (npc.BodyguardCharacterId != null)
			{
				if (!CachedBodyguards.ContainsKey(npc.BodyguardCharacterId.Value))
				{
					CachedBodyguards[npc.BodyguardCharacterId.Value] = new List<ICharacter>();
				}

				CachedBodyguards[npc.BodyguardCharacterId.Value].Add(newNpc);
				continue;
			}

			Add(newNpc, true);
			if (newNpc.Location != null)
			{
				(newNpc.Location as Cell)?.Login(newNpc);
			}
			else
			{
				Cells.First().Login(newNpc);
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = npcs.Count;
		Console.WriteLine("Loaded {0} NPC{1}.", count, count == 1 ? "" : "s");

		Console.WriteLine("\nLoading Guests...");
#if DEBUG
		sw.Restart();
#endif
		var guests = (from guest in FMDB.Context.Guests select guest).ToList();


		foreach (var guest in guests)
		{
			var newGuest = TryGetCharacter(guest.CharacterId);
			_guests.Add(newGuest);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		count = guests.Count;
		Console.WriteLine("Loaded {0} Guest{1}.", count, count == 1 ? "" : "s");

		foreach (var authority in _legalAuthorities)
		{
			authority.LoadPatrols();
		}

#if DEBUG
		sw.Restart();
#endif
		Console.WriteLine("\nLoading NPC Spawners...");

		var spawners = (from spawner in FMDB.Context.NpcSpawners select spawner).ToList();

		foreach (var spawner in spawners)
		{
			_npcSpawners.Add(new NPC.NPCSpawner(spawner, this));
		}

		count = spawners.Count;
		Console.WriteLine("Loaded {0} NPC Spawner{1}.", count, count == 1 ? "" : "s");
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
	}

	void IFuturemudLoader.LoadGroupAIs()
	{
		Console.WriteLine("\nLoading Group AI Templates...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var templates = (from template in FMDB.Context.GroupAiTemplates.AsNoTracking() select template).ToList();

		foreach (var template in templates)
		{
			_groupAITemplates.Add(new NPC.AI.Groups.GroupAITemplate(template, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = templates.Count;
		Console.WriteLine("Loaded {0} Template{1}.", count, count == 1 ? "" : "s");

		Console.WriteLine("\nLoading Group AIs...");
#if DEBUG
		sw.Restart();
#endif

		var ais = FMDB.Context.GroupAis.AsNoTracking().ToList();
		foreach (var ai in ais)
		{
			_groupAIs.Add(new NPC.AI.Groups.GroupAI(ai, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = templates.Count;
		Console.WriteLine("Loaded {0} AI{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadAIs()
	{
		Console.WriteLine("\nLoading AIs...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ArtificialIntelligenceBase.SetupAI();
		var ais = (from ai in FMDB.Context.ArtificialIntelligences.AsNoTracking()
		           select ai).ToList();

		foreach (var ai in ais)
		{
			_AIs.Add(ArtificialIntelligenceBase.LoadIntelligence(ai, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = ais.Count;
		Console.WriteLine("Loaded {0} AI{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHooks()
	{
		Console.WriteLine("\nLoading Hooks...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		HookBase.SetupHooks();
		var hooks = (from hook in FMDB.Context.Hooks.AsNoTracking()
		             select hook).ToList();

		foreach (var hook in hooks)
		{
			_hooks.Add(HookBase.LoadHook(hook, this));
		}

		var count = hooks.Count;
		Console.WriteLine("Loaded {0} Hook{1}.", count, count == 1 ? "" : "s");

		Console.WriteLine("\nLoading Default Hooks...");
		var defaultHooks = (from hook in FMDB.Context.DefaultHooks.AsNoTracking() select hook).ToList();
		foreach (var hook in defaultHooks)
		{
			_defaultHooks.Add(new DefaultHook(hook, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = defaultHooks.Count;
		Console.WriteLine("Loaded {0} Default Hook{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCurrencies()
	{
		Console.WriteLine("\nLoading Currencies...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var currencies = (from currency in
			                  FMDB.Context.Currencies
			                      .Include(x => x.Coins)
			                      .Include(x => x.CurrencyDivisions)
			                      .ThenInclude(x => x.CurrencyDivisionAbbreviations)
			                      .Include(x => x.CurrencyDescriptionPatterns)
			                      .ThenInclude(x => x.CurrencyDescriptionPatternElements)
			                      .ThenInclude(x => x.CurrencyDescriptionPatternElementSpecialValues)
			                      .AsNoTracking()
		                  select currency).ToList();

		foreach (var currency in currencies)
		{
			_currencies.Add(new Currency(currency, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = currencies.Count;
		Console.WriteLine("Loaded {0} Currenc{1}.", count, count == 1 ? "y" : "ies");
	}

	void IFuturemudLoader.LoadChargen()
	{
		Console.WriteLine("\nLoading Character Creation Storyboard...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ChargenStoryboard = new ChargenStoryboard(this);
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		Console.WriteLine("Done.");
	}

	void IFuturemudLoader.LoadEntityDescriptionPatterns()
	{
		Console.WriteLine("\nLoading Entity Description Patterns...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var patterns = (from pattern in FMDB.Context.EntityDescriptionPatterns.AsNoTracking()
		                select pattern).ToList();

		foreach (var pattern in patterns)
		{
			_entityDescriptionPatterns.Add(new EntityDescriptionPattern(pattern, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = patterns.Count;
		Console.WriteLine("Loaded {0} Entity Description Pattern{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCellOverlayPackages()
	{
		Console.WriteLine("\nLoading Cell Overlay Packages...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var packages = (from package in FMDB.Context.CellOverlayPackages
		                                    .Include(x => x.EditableItem)
		                                    .Include(x => x.CellOverlays)
		                                    .AsNoTracking()
		                select package).ToList();

		foreach (var package in packages)
		{
			_cellOverlayPackages.Add(new CellOverlayPackage(package, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = packages.Count;
		Console.WriteLine("Loaded {0} Cell Overlay Package{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHelpFiles()
	{
		Console.WriteLine("\nLoading Help Files...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var helpfiles = (from helpfile in FMDB.Context.Helpfiles
		                                      .Include(x => x.HelpfilesExtraTexts)
		                                      .AsNoTracking()
		                 select helpfile).ToList();

		foreach (var helpfile in helpfiles)
		{
			_helpfiles.Add(new Helpfile(helpfile, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = helpfiles.Count;
		Console.WriteLine("Loaded {0} Help File{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadFutureProgs()
	{
		Console.WriteLine("\nLoading FutureProgs...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var progs = (from prog in FMDB.Context.FutureProgs
		                              .Include(x => x.FutureProgsParameters)
		                              .AsNoTracking()
		             select prog).ToList();

		foreach (var prog in progs)
		{
			_futureProgs.Add(FutureProgFactory.CreateNew(prog, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = progs.Count;
		Console.WriteLine("Loaded {0} FutureProg{1}...Compiling...", count, count == 1 ? "" : "s");

		foreach (var prog in _futureProgs)
		{
			if (!prog.Compile())
			{
				Console.WriteLine("FutureProg {0} ({2}) failed to compile: {1}", prog.Id, prog.CompileError,
					prog.FunctionName);
			}
		}
	}

	void IFuturemudLoader.LoadScriptedEvents()
	{
		Console.WriteLine("\nLoading Scripted Events...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var scripteds = (from scripted in FMDB.Context.ScriptedEvents
									  .Include(x => x.FreeTextQuestions)
									  .Include(x => x.MultipleChoiceQuestions)
									  .ThenInclude(x => x.Answers)
									  .AsNoTracking()
					 select scripted).ToList();

		foreach (var scripted in scripteds)
		{
			_scriptedEvents.Add(new RPG.ScriptedEvents.ScriptedEvent(scripted, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = scripteds.Count;
		Console.WriteLine("Loaded {0} Scripted Event{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCharacteristics()
	{
		Console.WriteLine("\nLoading Characteristics...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif

		// Load Relative Height separately
		RelativeHeightDescriptors = new RankedRange<ICharacteristicValue>();
		var relativeHeightDefinition =
			new CharacteristicDefinition(FMDB.Context.CharacteristicDefinitions.First(
				x => (CharacteristicType)x.Type == CharacteristicType.RelativeHeight), this);
		var heightValues =
			FMDB.Context.CharacteristicValues.AsNoTracking().Where(
				x => x.DefinitionId == relativeHeightDefinition.Id).ToList();
		foreach (
			var item in
			heightValues.OrderBy(x => double.Parse(x.Value)))
		{
			var newValue = new CharacteristicValue(item.Name, relativeHeightDefinition, item.Value,
				item.AdditionalValue, true);
			RelativeHeightDescriptors.Add(newValue, double.Parse(item.Value), double.Parse(item.AdditionalValue));
			if (item.Default)
			{
				relativeHeightDefinition.SetDefaultValue(newValue);
			}
		}
		//_characteristics.Add(relativeHeightDefinition);

		var definitions =
			(from definition in
				 FMDB.Context.CharacteristicDefinitions.AsNoTracking()
				     .Where(x => (CharacteristicType)x.Type != CharacteristicType.RelativeHeight)
			 orderby definition.ParentId == null descending
			 select definition).ToList();
		foreach (var definition in definitions)
		{
			_characteristics.Add(CharacteristicDefinitionFactory.LoadDefinition(definition, this));
		}

		var count = definitions.Count;
		Console.WriteLine("Loaded {0} Characteristic Definition{1}.", count, count == 1 ? "" : "s");

		var values = (from value in FMDB.Context.CharacteristicValues.AsNoTracking()
		              where value.DefinitionId != relativeHeightDefinition.Id
		              select value).ToList();
		foreach (var value in values)
		{
			var cvalue = CharacteristicValue.LoadValue(value, this);
			if (cvalue == null)
			{
				throw new ApplicationException($"Null Characteristic Value for Definition {value.DefinitionId}.");
			}

			_characteristicValues.Add(cvalue);
		}

		count = values.Count;
		Console.WriteLine("Loaded {0} Characteristic Value{1}.", count, count == 1 ? "" : "s");

		var profiles = (from profile in FMDB.Context.CharacteristicProfiles.AsNoTracking() select profile).ToList();
		foreach (var profile in profiles)
		{
			_characteristicProfiles.Add(CharacteristicProfile.LoadProfile(profile, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = profiles.Count;
		Console.WriteLine("Loaded {0} Characteristic Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadUnits()
	{
		Console.WriteLine("\nLoading Units of Measure...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		UnitManager = new UnitManager(this);
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = UnitManager.Units.Count();
		Console.WriteLine("Loaded {0} Unit{1} of Measure.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadStackDecorators()
	{
		Console.WriteLine("\nLoading Stack Decorators...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var decorators = (from decorator in FMDB.Context.StackDecorators.AsNoTracking()
		                  select decorator).ToList();

		foreach (var decorator in decorators)
		{
			_stackDecorators.Add(StackDecorator.LoadStackDecorator(decorator));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = _stackDecorators.Count;
		Console.WriteLine("Loaded {0} Stack Decorator{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHearingProfiles()
	{
		Console.WriteLine("\nLoading Hearing Profiles...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var profiles = (from profile in FMDB.Context.HearingProfiles.AsNoTracking()
		                select profile).ToList();

		var stagingtable = new Dictionary<long, Tuple<HearingProfile, Form.Audio.HearingProfiles.HearingProfile>>();
		foreach (var profile in profiles)
		{
			var newprofile = Form.Audio.HearingProfiles.HearingProfile.LoadProfile(profile);
			stagingtable.Add(profile.Id, Tuple.Create(profile, newprofile));
			_hearingProfiles.Add(newprofile);
		}

		foreach (var profile in stagingtable)
		{
			profile.Value.Item2.Initialise(profile.Value.Item1, this);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = profiles.Count;
		Console.WriteLine("Loaded {0} Hearing Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadLanguageDifficultyModels()
	{
		Console.WriteLine("\nLoading Language Difficulty Models...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var models = (from model in FMDB.Context.LanguageDifficultyModels.AsNoTracking()
		              select model).ToList();
		foreach (var model in models)
		{
			_languageDifficultyModels.Add(LanguageDifficultyModel.LoadModel(model));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = models.Count;
		Console.WriteLine("Loaded {0} Language Difficulty Model{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadLanguages()
	{
		Console.WriteLine("\nLoading Languages and Accents...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var languages = (from language in FMDB.Context.Languages
		                                      .Include(x => x.Accents)
		                                      .Include(x => x.MutualIntelligabilitiesListenerLanguage)
		                                      .AsNoTracking()
		                 select language).ToList();

		foreach (var language in languages)
		{
			_languages.Add(new Language(language, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = languages.Count;
		var accents = _accents.Count;
		Console.WriteLine("Loaded {0} Language{1} and {2} Accent{3}.", count, count == 1 ? "" : "s", accents,
			accents == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadAuthorityGroups()
	{
		Console.WriteLine("\nLoading Authority Groups...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var authorityGroups = (from authorityGroup in FMDB.Context.AuthorityGroups.AsNoTracking()
		                       select authorityGroup).ToList();
		foreach (var authorityGroup in authorityGroups)
		{
			var newAuthority = new Authority(authorityGroup);
			_authorities.Add(newAuthority);
			if (newAuthority.Level == PermissionLevel.NPC)
			{
				DummyAccount.Instance.Authority = newAuthority;
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = authorityGroups.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Authority Group" : "Authority Groups");
	}

	void IFuturemudLoader.LoadTerrains()
	{
		Console.WriteLine("\nLoading Terrains...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var terrains = (from terrain in FMDB.Context.Terrains.Include(x => x.TerrainsRangedCovers).AsNoTracking()
		                select terrain).ToList();
		foreach (var terrain in terrains)
		{
			_terrains.Add(new Terrain(terrain, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = terrains.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Terrain" : "Terrains");
	}

	void IFuturemudLoader.LoadWorld()
	{
		Console.WriteLine("\nLoading Shards...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var shards = (from plane in FMDB.Context.Shards
		                                .Include(x => x.HooksPerceivables)
		                                .Include(x => x.ShardsCalendars)
		                                .Include(x => x.ShardsCelestials)
		                                .Include(x => x.ShardsClocks)
		                                .AsNoTracking()
		              select plane).ToList();
		foreach (var shard in shards)
		{
			_shards.Add(new Shard(shard, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = shards.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Shard" : "Shards");

		// ------------------------------------------------------------------- //

		Console.WriteLine("\nLoading Zones...");
#if DEBUG
		sw.Restart();
#endif
		var zones = (from zone in FMDB.Context.Zones
		                              .Include(x => x.HooksPerceivables)
		                              .Include(x => x.ZonesTimezones)
		                              .AsNoTracking()
		             select zone).ToList();
		foreach (var zone in zones)
		{
			_zones.Add(new Zone(zone, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = zones.Count;
		Console.WriteLine("Loaded {0} {1}.", count, count == 1 ? "Zone" : "Zones");

		// ------------------------------------------------------------------- //

		Console.WriteLine("\nLoading Rooms...");
#if DEBUG
		sw.Restart();
#endif
		var rooms = (from room in FMDB.Context.Rooms.AsNoTracking()
		             select room).ToList();
		foreach (var room in rooms)
		{
			_rooms.Add(new Room(room, _zones.FirstOrDefault(x => x.Id == room.ZoneId)));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = rooms.Count;
		Console.WriteLine("Loaded {0} {1}.", count, count == 1 ? "Room" : "Rooms");

		// ------------------------------------------------------------------- //

		Console.WriteLine("\nLoading Cells");
#if DEBUG
		sw.Restart();
#endif
		var cells = (from cell in
			             FMDB.Context.Cells
			                 .Include(x => x.CellOverlays)
			                 .ThenInclude(x => x.CellOverlaysExits)
			                 .Include(x => x.CellsForagableYields)
			                 .Include(x => x.CellsGameItems)
			                 .Include(x => x.CellsMagicResources)
			                 .Include(x => x.CellsRangedCovers)
			                 .Include(x => x.CellsTags)
			                 .Include(x => x.HooksPerceivables)
			                 .AsNoTracking()
		             select cell).ToList();
		var loadedCells = new Dictionary<MudSharp.Models.Cell, Cell>();
		foreach (var cell in cells)
		{
			var newCell = new Cell(cell, _rooms.FirstOrDefault(x => x.Id == cell.RoomId));
			loadedCells[cell] = newCell;
			_cells.Add(newCell);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = cells.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Cell" : "Cells");

		Console.WriteLine("\nLoading Areas");
#if DEBUG
		sw.Restart();
#endif
		var areas = (from cell in FMDB.Context.Areas
		                              .Include(x => x.AreasRooms)
		                              .AsNoTracking()
		             select cell).ToList();
		foreach (var area in areas)
		{
			_areas.Add(new Construction.Area(area, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = areas.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Area" : "Areas");

		// ------------------------------------------------------------------- //


		Console.WriteLine("Loading Exit Manager and Preloading Critical Cell Exits...");
#if DEBUG
		sw.Restart();
#endif
		ExitManager = new ExitManager(this);
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		Console.WriteLine("Exit Manager Complete.");

		foreach (var zone in _zones)
		{
			(zone as Zone)?.PostLoadSetup();
		}
	}

	void IFuturemudLoader.LoadEntityDescriptions()
	{
	}

	void IFuturemudLoader.LoadBodies()
	{
		Console.WriteLine("\nLoading Bodypart Group Describers...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var rules = (from rule in FMDB.Context.BodypartGroupDescribers
		                              .Include(x => x.BodypartGroupDescribersBodypartProtos)
		                              .Include(x => x.BodypartGroupDescribersShapeCount)
		                              .Include(x => x.BodypartGroupDescribersBodyProtos)
		                              .AsNoTracking()
		             select rule).ToList();

		var bodypartStagingTable = new Dictionary<IBodypartGroupDescriber, BodypartGroupDescriber>();
		foreach (var rule in rules)
		{
			var newRule = Body.Grouping.BodypartGroupDescriber.LoadDescriber(rule, this);
			_bodypartGroupDescriptionRules.Add(newRule);
			bodypartStagingTable.Add(newRule, rule);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _bodypartGroupDescriptionRules.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Bodypart Group Describer" : "Bodypart Group Describers");

		Console.WriteLine("\nLoading Bodies...");
#if DEBUG
		sw.Restart();
#endif
		Console.WriteLine("\nLoading Bodyparts...");

		var bpprotos = FMDB.Context.BodypartProtos.ToList();
		foreach (var bpproto in bpprotos)
		{
			_bodypartPrototypes.Add(BodypartPrototype.LoadFromDatabase(bpproto, this));
		}

		// Setup Organ Info
		foreach (var organ in bpprotos.Where(x => x.IsOrgan == 1).ToList())
		{
			var iorgan = (IOrganProto)BodypartPrototypes.Get(organ.Id);
			foreach (var info in organ.BodypartInternalInfosInternalPart)
			{
				var part = BodypartPrototypes.Get(info.BodypartProtoId);
				part.LinkOrgan(iorgan,
					new BodypartInternalInfo(info.HitChance, info.IsPrimaryOrganLocation, info.ProximityGroup));
			}
		}

		// Setup Bone Info
		foreach (var bone in bpprotos.Where(x => ((BodypartTypeEnum)x.BodypartType).IsBone()).ToList())
		{
			var ibone = (IBone)BodypartPrototypes.Get(bone.Id);
			ibone.PostLoadProcessing(null, bone);
			foreach (var info in bone.BodypartInternalInfosInternalPart)
			{
				var part = BodypartPrototypes.Get(info.BodypartProtoId);
				part.LinkBone(
					ibone,
					new BodypartInternalInfo(info.HitChance, info.IsPrimaryOrganLocation, info.ProximityGroup));
			}
		}

		// Link bodyparts
		foreach (var part in bpprotos.Where(x => x.IsOrgan == 0 && !((BodypartTypeEnum)x.BodypartType).IsBone())
		                             .ToList())
		{
			var thisPart = BodypartPrototypes.Get(part.Id);
			if (part.BodypartProtoBodypartProtoUpstreamChildNavigation.Count <= 0)
			{
				continue;
			}

			var targetUpstream =
				BodypartPrototypes.Get(part.BodypartProtoBodypartProtoUpstreamChildNavigation.First().Parent);
			if (targetUpstream != null)
			{
				thisPart.LinkUpstream(targetUpstream);
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		Console.WriteLine("\nLoading Body Protos...");
#if DEBUG
		sw.Restart();
#endif
		var bodyStaging = new Dictionary<IBodyPrototype, Models.BodyProto>();
		var bodies = (from body in FMDB.Context.BodyProtos
		                               .Include(x => x.BodyProtosAdditionalBodyparts)
		                               .Include(x => x.BodypartGroupDescribersBodyProtos)
		                               .Include(x => x.MoveSpeeds)
		                               .Include(x => x.WearSizeParameter)
		                               .Include(x => x.BodyProtosPositions)
		              select body)
		             .AsNoTracking()
		             .ToList();
		foreach (var dbbody in bodies)
		{
			var body = new BodyPrototype(dbbody, this);
			_bodyPrototypes.Add(body);
			bodyStaging[body] = dbbody;
		}

#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		Console.WriteLine("\nLinking Bodyparts...");
		foreach (var part in bpprotos)
		{
			BodypartPrototypes.Get(part.Id).SetBodyProto(BodyPrototypes.Get(part.BodyId));
		}

		Console.WriteLine("\nFinalising Bodyparts...");
		foreach (var body in BodyPrototypes)
		{
			body.FinaliseBodyparts(bodyStaging[body]);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = bodies.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Body Prototype" : "Body Prototypes");

		Console.WriteLine("\nLoading Limbs...");
#if DEBUG
		sw.Restart();
#endif
		var limbs = (from limb in FMDB.Context.Limbs
		                              .Include(x => x.LimbsSpinalParts)
		                              .Include(x => x.LimbsBodypartProto)
		             select limb).ToList();
		foreach (var limb in limbs)
		{
			_limbs.Add(new Limb(limb, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = limbs.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Limb" : "Limbs");

		Console.WriteLine("Finalising Bodypart Group Describers...");
		foreach (var item in bodypartStagingTable)
		{
			item.Key.FinaliseLoad(item.Value, this);
		}

		Console.WriteLine("Done finalising Bodypart Group Describers...");

		foreach (var part in BodypartPrototypes)
		{
			var partlimb = part.Body.Limbs.FirstOrDefault(x => x.Parts.Contains(part));
			if (partlimb == null)
			{
				var potentialLimbs = part.Body.Limbs.Where(x => part.DownstreamOfPart(x.RootBodypart)).ToList();
				if (potentialLimbs.Count > 1)
				{
					var bodyRootLimb = potentialLimbs.FirstOrDefault(x => x.RootBodypart.UpstreamConnection == null);
					if (bodyRootLimb == null)
					{
						partlimb = potentialLimbs.First(x => x.LimbType != LimbType.Torso);
					}
					else
					{
						partlimb = potentialLimbs.Except(bodyRootLimb).First();
					}
				}
				else
				{
					partlimb = potentialLimbs.FirstOrDefault();
				}

				partlimb?.AddBodypart(part);
			}
		}
	}

	void IFuturemudLoader.LoadBodypartShapes()
	{
		Console.WriteLine("\nLoading Bodypart Shapes...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var shapes = (from shape in FMDB.Context.BodypartShapes.AsNoTracking()
		              select shape).ToList();

		foreach (var shape in shapes)
		{
			_bodypartShapes.Add(new BodypartShape(shape, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = _bodypartShapes.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Bodypart Shape" : "Bodypart Shapes");
	}

	void IFuturemudLoader.LoadClans()
	{
		Console.WriteLine("\nLoading Clans...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var clans = (from clan in FMDB.Context.Clans
		                              .Include(x => x.ClansAdministrationCells)
		                              .Include(x => x.ClansTreasuryCells)
		                              .Include(x => x.Ranks)
		                              .ThenInclude(x => x.RanksAbbreviations)
		                              .Include(x => x.Ranks)
		                              .ThenInclude(x => x.RanksPaygrades)
		                              .Include(x => x.Ranks)
		                              .ThenInclude(x => x.RanksTitles)
		                              .Include(x => x.Paygrades)
		                              .Include(x => x.Appointments)
		                              .ThenInclude(x => x.AppointmentsAbbreviations)
		                              .Include(x => x.Appointments)
		                              .ThenInclude(x => x.AppointmentsTitles)
		                              .Include(x => x.ExternalClanControlsLiegeClan)
		                              .AsNoTracking()
		             select clan).ToList();

		var memberships = (from membership in FMDB.Context.ClanMemberships
		                                          .Include(x => x.ClanMembershipsAppointments)
		                                          .Include(x => x.ClanMembershipsBackpay)
		                                          .AsNoTracking()
		                   select membership
			).ToList();

#if DEBUG
		Console.WriteLine($"...Creating Clan objects [{sw.ElapsedMilliseconds}ms]");
#endif
		var staging = new Dictionary<IClan, Clan>();
		foreach (var clan in clans)
		{
			var newClan = Community.Clan.ClanFactory.LoadClan(clan, this);
			_clans.Add(newClan);
			staging.Add(newClan, clan);
		}

#if DEBUG
		Console.WriteLine($"...Finalising Load of Clans [{sw.ElapsedMilliseconds}ms]");
#endif
		foreach (var clan in staging)
		{
			clan.Key.FinaliseLoad(clan.Value, memberships);
		}

#if DEBUG
		Console.WriteLine($"...Loading External Controls [{sw.ElapsedMilliseconds}ms]");
#endif
		var externals = from external
			                in FMDB.Context.ExternalClanControls
			                       .Include(x => x.ExternalClanControlsAppointments)
			                       .AsNoTracking()
		                select external;

		foreach (var external in externals)
		{
			new ExternalClanControl(external, this);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = clans.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Clan" : "Clans");
		count = externals.Count();
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "External Clan Control" : "External Clan Controls");
	}

	void IFuturemudLoader.LoadColours()
	{
		Console.WriteLine("\nLoading Colours...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var colours = FMDB.Context.Colours.AsNoTracking().ToList();
		foreach (var colour in colours)
		{
			_colours.Add(new Colour(colour));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _colours.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Colour" : "Colours");
	}

	void IFuturemudLoader.LoadTags()
	{
		Console.WriteLine("\nLoading Tags...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif

		var tags = (from tag in FMDB.Context.Tags
		            select tag).ToList();

		var staging = new Dictionary<MudSharp.Models.Tag, ILoadingTag>();
		foreach (var tag in tags)
		{
			var newTag = new Tag(tag, this);
			staging.Add(tag, newTag);
			_tags.Add(newTag);
		}

		foreach (var tag in staging)
		{
			tag.Value.FinaliseLoad(tag.Key);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = tags.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Tag" : "Tags");
	}

	void IFuturemudLoader.LoadWearProfiles()
	{
		Console.WriteLine("\nLoading Wear Profiles...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var profiles = (from profile in FMDB.Context.WearProfiles.AsNoTracking()
		                select profile).ToList();
		foreach (var profile in profiles)
		{
			_wearProfiles.Add(WearProfile.LoadWearProfile(profile, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = _wearProfiles.Count;
		Console.WriteLine("Loaded {0} Wear Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadGameItemComponentProtos()
	{
		Console.WriteLine("\nLoading Game Item Component Protos...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var protos = (from proto in FMDB.Context.GameItemComponentProtos
		                                .Include(x => x.EditableItem)
		                                .AsNoTracking()
		              select proto).ToList();
		foreach (var proto in protos)
		{
			_itemComponentProtos.Add(GameItemComponentManager.GetProto(proto, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _itemComponentProtos.Count();
		Console.WriteLine("Loaded {0:N0} {1}.", count,
			count == 1 ? "Game Item Component Proto" : "Game Item Component Protos");
	}

	void IFuturemudLoader.LoadGameItemProtos()
	{
		Console.WriteLine("\nLoading Game Item Protos...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var protos = (from proto in FMDB.Context.GameItemProtos
		                                .Include(x => x.EditableItem)
		                                .Include(x => x.GameItemProtosDefaultVariables)
		                                .Include(x => x.GameItemProtosTags)
		                                .Include(x => x.GameItemProtosGameItemComponentProtos)
		                                .Include(x => x.GameItemProtosOnLoadProgs)
		                                .Include(x => x.ExtraDescriptions)
		                                .AsNoTracking()
		              select proto).ToList();
		foreach (var proto in protos)
		{
			_itemProtos.Add(new GameItemProto(proto, this));
		}

		// Special Game Item Protos
		// TODO: Make a programmatic way to do this
		foreach (
			var type in
			GetAllTypes().Where(x => x.IsSubclassOf(typeof(GameItemComponentProto))))
		{
			var method = type.GetMethod("InitialiseItemType", BindingFlags.Public | BindingFlags.Static);
			method?.Invoke(null, new object[] { this });
		}
		//CurrencyGameItemComponentProto.ItemPrototype = _itemProtos.Single(x => x.IsItemType<CurrencyGameItemComponentProto>());
		// End Special Section

		var config =
			FMDB.Context.StaticConfigurations
			    .AsNoTracking()
			    .FirstOrDefault(x => x.SettingName == "TooManyItemsGameItemGroup");

		if (config != null)
		{
			GameItemProto.TooManyItemsGroup = _itemGroups.Get(long.Parse(config.Definition));
		}

		config =
			FMDB.Context.StaticConfigurations
			    .AsNoTracking()
			    .FirstOrDefault(x => x.SettingName == "DefaultItemHealthStrategy");

		if (config != null)
		{
			GameItemProto.DefaultItemHealthStrategy = _healthStrategies.Get(long.Parse(config.Definition));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = _itemProtos.Count();
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Game Item Proto" : "Game Item Protos");
	}

	void IFuturemudLoader.LoadGameItemSkins()
	{
		Console.WriteLine("\nLoading Game Item Skins...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var skins = (from skin in FMDB.Context.GameItemSkins
		                              .Include(x => x.EditableItem)
		                              .AsNoTracking()
		             select skin).ToList();
		foreach (var skin in skins)
		{
			_itemSkins.Add(new GameItems.GameItemSkin(skin, this));
		}


#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = _itemSkins.Count();
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Game Item Skin" : "Game Item Skins");
	}

	void IFuturemudLoader.LoadImprovementModels()
	{
		Console.WriteLine("\nLoading Improvement Models...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var models = (from model in FMDB.Context.Improvers.AsNoTracking() select model).ToList();
		foreach (var model in models)
		{
			_improvementModels.Add(ImprovementModel.LoadModel(model, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _improvementModels.Count;
		Console.WriteLine("Loaded {0} Improvement Model{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadTraits()
	{
		Console.WriteLine("\nLoading Trait Definitions...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var traits = (from trait in FMDB.Context.TraitDefinitions
		                                .Include(x => x.TraitDefinitionsChargenResources)
		                                .AsNoTracking()
		              select trait).ToList();
		foreach (var trait in traits)
		{
			_traits.Add(TraitDefinition.LoadTraitDefinition(trait, this));
		}

		foreach (var trait in traits)
		{
			_traits.Get(trait.Id).Initialise(trait);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = traits.Count;
		Console.WriteLine("Loaded {0} Trait Definition{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadTraitDecorators()
	{
		Console.WriteLine("\nLoading Trait Decorators...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var decorators = (from decorator in FMDB.Context.TraitDecorators.AsNoTracking() select decorator).ToList();
		foreach (var decorator in decorators)
		{
			_traitDecorators.Add(DecoratorBase.GetDecorator(decorator));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = decorators.Count;
		Console.WriteLine("Loaded {0} Trait Decorator{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadEthnicities()
	{
		Console.WriteLine("\nLoading Ethnicities...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var ethnicities = (from ethnicity in FMDB.Context.Ethnicities
		                                         .Include(x => x.EthnicitiesCharacteristics)
		                                         .Include(x => x.EthnicitiesChargenResources)
		                                         .Include(x => x.ChargenAdvicesEthnicities)
		                                         .Include(x => x.PopulationBloodModel)
		                                         .AsNoTracking()
		                   select ethnicity).ToList();
		foreach (var ethnicity in ethnicities)
		{
			_ethnicities.Add(new Ethnicity(ethnicity, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = ethnicities.Count;
		Console.WriteLine("Loaded {0} Ethnicit{1}", count, count == 1 ? "y" : "ies");
	}

	void IFuturemudLoader.LoadCultures()
	{
		Console.WriteLine("\nLoading Cultures and Name Cultures, and Names...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var namecultures =
			(from nameculture in FMDB.Context.NameCultures
			                         .Include(x => x.RandomNameProfiles)
			                         .ThenInclude(x => x.RandomNameProfilesDiceExpressions)
			                         .Include(x => x.RandomNameProfiles)
			                         .ThenInclude(x => x.RandomNameProfilesElements)
			                         .AsNoTracking()
			 select nameculture).ToList();
		foreach (var nameculture in namecultures)
		{
			_nameCultures.Add(new NameCulture(nameculture, this));
		}

		var count = namecultures.Count;
		Console.WriteLine("Loaded {0} Name Culture{1}", count, count == 1 ? "" : "s");

		var cultures = (from culture in FMDB.Context.Cultures
		                                    .Include(x => x.CulturesChargenResources)
		                                    .Include(x => x.ChargenAdvicesCultures)
		                                    .Include(x => x.CulturesNameCultures)
		                select culture).ToList();
		foreach (var culture in cultures)
		{
			_cultures.Add(new Culture(culture, this));
		}

		count = cultures.Count;
		Console.WriteLine("Loaded {0} Culture{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadRaces()
	{
		Console.WriteLine("\nLoading Races...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var races = (from race in FMDB.Context.Races
		                              .Include(x => x.ChargenAdvicesRaces)
		                              .Include(x => x.RacesChargenResources)
		                              .Include(x => x.RacesAdditionalBodyparts)
		                              .Include(x => x.RacesAdditionalCharacteristics)
		                              .Include(x => x.RacesWeaponAttacks)
		                              .Include(x => x.RacesAttributes)
		                              .Include(x => x.RacesBreathableGases)
		                              .Include(x => x.RacesBreathableLiquids)
		                              .Include(x => x.RacesEdibleMaterials)
		                              .Include(x => x.RaceEdibleForagableYields)
		                              .Include(x => x.RacesCombatActions)
		             select race).ToList();
		var staging = new List<Tuple<Race, MudSharp.Models.Race>>();

		foreach (var race in races)
		{
			var newRace = new Race(race, this);
			staging.Add(Tuple.Create(newRace, race));
			_races.Add(newRace);
		}

		foreach (var race in staging)
		{
			race.Item1.LinkParents(race.Item2);
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = staging.Count;
		Console.WriteLine("Loaded {0} Race{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadProjects()
	{
		Console.WriteLine("\nLoading Projects...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var projects = FMDB.Context.Projects
		                   .Include(x => x.EditableItem)
		                   .Include(x => x.ProjectPhases)
		                   .ThenInclude(x => x.ProjectActions)
		                   .Include(x => x.ProjectPhases)
		                   .ThenInclude(x => x.ProjectLabourRequirements)
		                   .ThenInclude(x => x.ProjectLabourImpacts)
		                   .Include(x => x.ProjectPhases)
		                   .ThenInclude(x => x.ProjectMaterialRequirements)
		                   .AsNoTracking().ToList();

		foreach (var project in projects)
		{
			_projects.Add(ProjectFactory.LoadProject(project, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = projects.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Project" : "Projects");

		Console.WriteLine("\nLoading Active Projects...");
#if DEBUG
		sw.Restart();
#endif
		var actives = FMDB.Context.ActiveProjects
		                  .Include(x => x.ActiveProjectLabours)
		                  .Include(x => x.ActiveProjectMaterials)
		                  .Include(x => x.Characters)
		                  .AsNoTracking().ToList();
		foreach (var active in actives)
		{
			_activeProjects.Add(ProjectFactory.LoadActiveProject(active, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		count = projects.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Active Project" : "Active Projects");
	}

	void IFuturemudLoader.LoadJobs()
	{
		Console.WriteLine("\nLoading Job Listings...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var listings = FMDB.Context
		                   .JobListings
		                   .Include(x => x.ActiveJobs)
		                   .AsNoTracking()
		                   .ToList();
		foreach (var listing in listings)
		{
			_jobListings.Add(JobListingFactory.Load(listing, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = _jobListings.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Job Listing" : "Job Listings");
	}

	public void ReleasePrimedGameItems()
	{
		_bootTimeCachedGameItems = new Dictionary<long, GameItem>();
	}

	public void PrimeGameItems()
	{
		Console.WriteLine("Preparing to Cache Game Items...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		_bootTimeCachedGameItems = new Dictionary<long, GameItem>();
		var i = 0;
		var query = FMDB.Context.GameItems
		                .Include(x => x.WoundsGameItem)
		                .Include(x => x.GameItemComponents)
		                .Include(x => x.HooksPerceivables)
		                .Include(x => x.GameItemsMagicResources)
		                .OrderBy(x => x.Id);

		while (true)
		{
#if DEBUG
			Console.WriteLine($"Retrieving records {i * 250} to {(i + 1) * 250}...");
#endif
			var result = query.Skip(i++ * 250).Take(250).ToList();
			if (!result.Any())
			{
				break;
			}

			foreach (var item in result)
			{
				_bootTimeCachedGameItems.Add(item.Id, item);
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		Console.WriteLine("Done Caching Game Items.");
	}

	#region IFuturemudLoader Members

	void IFuturemudLoader.LoadCalendars()
	{
		Console.WriteLine("\nLoading Calendars...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var calendars = (from calendar in FMDB.Context.Calendars.AsNoTracking()
		                 select calendar).ToList();
		foreach (var calendar in calendars)
		{
			_calendars.Add(new Calendar(calendar, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = calendars.Count;
		Console.WriteLine("Loaded {0} Calendar{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadClocks()
	{
		Console.WriteLine("\nLoading Clocks...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		// TODO - this needs to find a new home
		ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds =
			double.Parse(FMDB.Context.StaticConfigurations.Find("RealSecondsToInGameSeconds").Definition);

		var clocks = (from clock in FMDB.Context.Clocks
		                                .Include(x => x.Timezones)
		                                .AsNoTracking()
		              select clock).ToList();
		foreach (var clock in clocks)
		{
			_clocks.Add(new Clock(clock, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = clocks.Count;
		Console.WriteLine("Loaded {0} Clock{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadChecks()
	{
		Console.WriteLine("\nLoading Checks...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var checks = (from check in FMDB.Context.Checks
		                                .Include(x => x.CheckTemplate)
		                                .ThenInclude(x => x.CheckTemplateDifficulties)
		                                .AsNoTracking()
		              select check).ToList();
		foreach (var check in checks)
		{
			var fmcheck = StandardCheck.LoadCheck(check, this);
			_checksIDs[fmcheck.Type] = fmcheck.Id;
			_checks.Add(fmcheck);
		}

		foreach (var checkType in Enum.GetValues<CheckType>())
		{
			if (checkType == CheckType.None)
			{
				continue;
			}

			if (!_checksIDs.ContainsKey(checkType))
			{
				var newCheck = new Check
				{
					CheckTemplate =
						FMDB.Context.CheckTemplates.FirstOrDefault(x =>
							x.Name == "SkillCheck" || x.Name == "Skill Check") ??
						FMDB.Context.CheckTemplates.First(),
					MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
					Type = (int)checkType,
					TraitExpression = new Models.TraitExpression
					{
						Name = $"{checkType.DescribeEnum(true)} Check",
						Expression = "50"
					}
				};
				FMDB.Context.Checks.Add(newCheck);
				FMDB.Context.TraitExpressions.Add(newCheck.TraitExpression);
				FMDB.Context.SaveChanges();
				_traitExpressions.Add(new TraitExpression(newCheck.TraitExpression, this));
				var fmcheck = StandardCheck.LoadCheck(newCheck, this);
				_checksIDs[fmcheck.Type] = fmcheck.Id;
				_checks.Add(fmcheck);
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = checks.Count;
		Console.WriteLine("Loaded {0} Check{1}", count, count == 1 ? "" : "s");
		foreach (var value in Enum.GetValues(typeof(CheckType)).OfType<CheckType>())
		{
			if (_checksIDs.ContainsKey(value))
			{
				continue;
			}

			Console.WriteLine(
				$"Warning - no check defined for check type {(int)value:N0} ({Enum.GetName(typeof(CheckType), value)})");
		}
	}

	void IFuturemudLoader.LoadTemporalListeners()
	{
		// TODO
	}

	void IFuturemudLoader.LoadProgSchedules()
	{
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		Console.WriteLine("\nLoading Prog Schedules...");
		var schedules = (from schedule in FMDB.Context.ProgSchedules.AsNoTracking() select schedule).ToList();
		foreach (var schedule in schedules)
		{
			_progSchedules.Add(new ProgSchedule(schedule, this));
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif
		var count = schedules.Count;
		Console.WriteLine("Loaded {0:N0} {1}.", count, count == 1 ? "Prog Schedule" : "Prog Schedules");
	}

	void IFuturemudLoader.LoadCelestials()
	{
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		Console.WriteLine("\nLoading Celestials...");

		var celestials = (from celestial in FMDB.Context.Celestials.AsNoTracking()
		                  select celestial).ToList();
		foreach (var celestial in celestials)
		{
			switch (celestial.CelestialType)
			{
				case "OldSun":
					_celestialObjects.Add(new Sun(celestial, this));
					continue;
				case "Sun":
					_celestialObjects.Add(new NewSun(celestial, this));
					continue;
				default:
					throw new ApplicationException($"Unknown Celestial type {celestial.CelestialType}.");
			}
		}
#if DEBUG
		sw.Stop();
		Console.WriteLine($"Duration: {sw.ElapsedMilliseconds}ms");
#endif

		var count = celestials.Count;
		Console.WriteLine("Loaded {0} Celestial Object{1}", count, count == 1 ? "" : "s");
	}

	#endregion
}