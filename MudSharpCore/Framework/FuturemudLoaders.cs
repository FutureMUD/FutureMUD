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
using System.Numerics;
using MudSharp.Economy.Shoppers;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using Track = MudSharp.Movement.Track;

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
		var sw = new Stopwatch();
		sw.Start();
		GameStatistics = new GameStatistics(this) { LastBootTime = DateTime.UtcNow };
		ConsoleUtilities.WriteLine("#DBooting Futuremud Server...#0\n");

		ConsoleUtilities.WriteLine("#C========================================#0");
		ConsoleUtilities.WriteLine("#5Constructing FutureMUD...#0");

		ConsoleUtilities.WriteLine("\n#EInitialising Non-Database Statics.#0");
		PositionState.SetupPositions();
		FutureProg.FutureProg.Initialise();

#if DEBUG
		ConsoleUtilities.WriteLine("\n#EWriting prog help text htmls...#0");
		var infos = FutureProg.FutureProg.GetFunctionCompilerInformations().ToList();
		ImplementorModule.WriteProgParametersByCategory(this, infos);
		ImplementorModule.WriteProgParametersAlphabetically(this, infos);
		ImplementorModule.WriteTypeHelps(this);
		ImplementorModule.WriteCollectionHelps(this);
		ConsoleUtilities.WriteLine("#ADone.#0");
#endif

		var game = (IFuturemudLoader)this;

		ConsoleUtilities.WriteLine("\n#EEnsuring that Database migrations are applied...#0");
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
				ConsoleUtilities.WriteLine(migration);
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

		ConsoleUtilities.WriteLine("#ADatabase is up to date.#0");

		ConsoleUtilities.WriteLine("#EConnecting to Database...#0");

		using (new FMDB())
		{
#if DEBUG
			var sqlwriter = new StreamWriter("SqlDebugLog.txt");
#endif
			ConsoleUtilities.WriteLine("#ADatabase connection opened.#0");

			ConsoleUtilities.WriteLine("\n#ESetting up Email Server...#0");
			ConsoleUtilities.WriteLine(EmailHelper.SetupEmailClient()
				? "#AEmail Server successfully setup.#0"
				: "#9Warning! Email Server did not successfully setup!#0");

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

			game.LoadNameCultures();
			game.LoadEthnicities(); // depends on LoadCharacteristics and LoadNameCultures
			game.LoadCultures(); // depends on LoadNameCultures

			game.LoadClans(); // Needs to come after LoadCalendars, LoadFutureProgs, LoadCultures and LoadCurrencies

			game.LoadLanguageDifficultyModels(); // Languages need to come after traits
			game.LoadLanguages();
			game.LoadScripts(); // Must come after LoadLanguages

			game.LoadUnits();
			game.LoadMagic(); // Needs to come before LoadMerits
			game.LoadMerits(); // ToDO - where should this be loaded?
			game.LoadAIs(); // Needs to come after LoadFutureProgs and LoadBodies
			game.LoadDisfigurements();
			game.LoadRoles();
			game.LoadNPCTemplates(); // Needs to come after LoadRoles, LoadAIs, LoadMerits and LoadHeightWeightModels
			game.LoadWritings(); // Needs to come after LoadScripts and before LoadWorldItems
			game.LoadDrawings(); // Needs to come before LoadWorldItems

			
			game.LoadProjects(); // Needs to come before LoadNPCs

			Character.Character.InitialiseCharacterClass(this);
			LightModel = PerceptionEngine.Light.LightModel.LoadLightModel(this);

			
			// Needs to come after LoadFutureprogs, LoadClans, LoadCurrencies, LoadMerits and LoadGameItemProtos
			game.LoadCrafts(); // Needs to come after LoadFutureProgs and before LoadWorldItems
			game.LoadEconomy(); // Should come before LoadWorldItems as late as possible
			game.LoadMarkets(); // Should come after LoadEconomy as late as possible
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
			game.LoadTracks();

			game.LoadTemporalListeners(); // Should come after everything else is loaded
			game.LoadProgSchedules(); // Should come after everything else is loaded

			SaveManager.MudBootingMode = false;

			// Cells need to do a few things that have to happen outside the boot order. Todo - use reflection to solve problems like this
			ConsoleUtilities.WriteLine("#EFinalising Cells...#0");
			var cells = FMDB.Context.Cells.Include(x => x.CellsForagableYields).ToList();
			foreach (var cell in _cells)
			{
				cell.PostLoadTasks(cells.FirstOrDefault(x => x.Id == cell.Id));
			}

			ConsoleUtilities.WriteLine("#ADone Finalising Cells.#0");
			ConsoleUtilities.WriteLine("#ELoading Statistics...#0");
			GameStatistics.LoadStatistics(game);
			ConsoleUtilities.WriteLine("#ADone Loading Statistics.#0");
			ConsoleUtilities.WriteLine("#ELoading Maintenance Mode...#0");
			var maintenanceSetting = FMDB.Context.StaticConfigurations.Find("MaintenanceMode");
			if (maintenanceSetting == null || !maintenanceSetting.Definition.IsInteger())
			{
				MaintenanceMode = MaintenanceModeSetting.None;
			}
			else
			{
				_maintenanceMode = (MaintenanceModeSetting)int.Parse(maintenanceSetting.Definition);
			}

			ConsoleUtilities.WriteLine("#ADone Loading Maintenance Mode.#0");

			ConsoleUtilities.WriteLine("#ELoading Combat Messages...#0");
			CombatMessageManager = new CombatMessageManager(this);
			ConsoleUtilities.WriteLine("#ADone Loading Combat Messages.#0");
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
			EnsureCoreHelpfiles();
#if DEBUG
			sqlwriter.Close();
#endif
		}

		
		ConsoleUtilities.WriteLine("\n\n#BDatabase connection closed.#0");
		ConsoleUtilities.WriteLine("#AFutureMUD constructed.#0");
		ConsoleUtilities.WriteLine("#C========================================#0");
		ConsoleUtilities.WriteLine("\n#EScheduling core system tasks...#0");
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
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this,
			fm =>
			{
				foreach (var population in fm.MarketPopulations)
				{
					population.MarketPopulationHeartbeat();
				}
			}, ScheduleType.System, TimeSpan.FromMinutes(60),
			"Market Population Heartbeats"));
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this, Economy.Shops.Shop.DoAutopayShopTaxes, ScheduleType.System, TimeSpan.FromMinutes(60), "Shop Autopay Taxes"));
		Chargen.SetupChargen(this);
		HeartbeatManager.StartHeartbeatTick();
		Track.CreateGlobalHeartbeatEvent();
		CommodityGameItemComponentProto.CreateGlobalHeartbeatEvent();
		EffectScheduler.SetupEffectSaver();
		Scheduler.AddSchedule(new RepeatingSchedule<IFuturemud>(this, this, fm => {
			CheckNewPlayerHints();
		}, ScheduleType.System, TimeSpan.FromMinutes(1), "Check New Player Hints"));
		sw.Stop();
		ConsoleUtilities.WriteLine($"Total Boot Time: #2{TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds)}#0");
		foreach (var npc in NPCs)
		{
			npc.HandleEvent(Events.EventType.NPCOnGameLoadFinished, npc);
		}

		GameStatistics.LastStartupSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
		ConsoleUtilities.WriteLine("\n#EAttempting to connect to Discord Server...#0");
		DiscordConnection = new DiscordConnection(this);
		if (DiscordConnection.OpenTcpConnection())
		{
			ConsoleUtilities.WriteLine("#ASuccessfully connected to the Discord Server.#0");
		}
		else
		{
			ConsoleUtilities.WriteLine("#9Was unable to connect to the Discord Server.#0");
		}

		ConsoleUtilities.WriteLine("\n#ECreating listening server thread...#0");
	}

	private void EnsureCoreHelpfiles()
	{
		if (Helpfiles.Any(x => x.Name == "Communication"))
		{
			return;
		}

		var dbhelp = new Models.Helpfile
		{
			Name = "Communication",
			Category = "Communication",
			Subcategory = "General",
			PublicText = @"There are several in-character communication commands that you can use in this game. There are two important things to know about each command - the volume at which it will cause your character to speak, and whether it is a targeted or non-targeted command. You will always use the language and accent set by the #3speak#0 command.

The twelve principle in-character communication commands are (in ascending order of loudness and with non-targeted / targeted versions shown)

#3whisper#0 / #3whisperto#0
#3talk#0 / #3talkto#0
#3say#0 / #3tell#0
#3loudsay#0 / #3loudtell#0
#3yell#0 / #3yellat#0
#3shout#0 / #3shoutat#0

There is also #3sing#0 / #3singto#0 outside of this hierarchy.

Each of the commands follows the same syntax, with say/tell used as an example:

#0say <message>#0
#3tell <target> <message>#0

In addition, you may also include a short emote in your communication commands that clarifies some action that you are taking as you speak, which is done with the following syntax:

#3tell <target> (<emote>) <message>#0
e.g #3whisperto tall.man (with a conspiratorial glance towards ~short.bald) I suspect that baldy is going to try and make a move for the throne.#0

The targeted versions of the command can be used to target either people or objects, so for instance you may talk directly to an individual, or you could whisper the name of someone you are about to kill to your dagger.

Finally, the yell and shout level of volume is so loud, that it can be heard in nearby rooms (albeit at a lesser volume there).

For information on the syntax to use in emotes (such as those included in brackets after your communication command), see #3help emote#0.".SubstituteANSIColour(),
			LastEditedBy = "System",
			LastEditedDate = DateTime.UtcNow,
			Keywords = "Communication",
			TagLine = "Information about communication commands"
		};
		FMDB.Context.Helpfiles.Add(dbhelp);
		FMDB.Context.SaveChanges();
		_helpfiles.Add(new Helpfile(dbhelp, this));
	}

	void IFuturemudLoader.LoadNewPlayerHints()
	{
		ConsoleUtilities.WriteLine("\nLoading #5New Player Hints#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _newPlayerHints.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Hint" : "Hints");

	}

	void IFuturemudLoader.LoadTracks()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Tracks#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var tracks = FMDB.Context.Tracks
		                      .AsNoTracking()
		                      .ToList();
		foreach (var item in tracks)
		{
			_tracks.Add(new Track(item, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _tracks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Track" : "Tracks");

		ConsoleUtilities.WriteLine("\n#EInitialising Tracks...#0");
		var trackDictionary = _tracks.AsEnumerable().Select(x => (x.Cell, x)).ToCollectionDictionary().AsReadOnlyCollectionDictionary();
		foreach (var cell in _cells)
		{
			cell.InitialiseTracks(trackDictionary);
		}
		ConsoleUtilities.WriteLine("#ADone.#0");
	}

	void IFuturemudLoader.LoadMarkets()
	{
		#region Categories
		ConsoleUtilities.WriteLine("\nLoading #5Market Categories#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif

		var categories = FMDB.Context.MarketCategories
		                .AsNoTracking()
		                .ToList();
		foreach (var item in categories)
		{
			_marketCategories.Add(new Economy.Markets.MarketCategory(this, item));
		}

#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _marketCategories.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Category" : "Categories");
		#endregion

		#region Influence Templates
		ConsoleUtilities.WriteLine("\nLoading #5Market Influence Templates#0...");
#if DEBUG
		sw.Restart();
#endif
		var templates = FMDB.Context.MarketInfluenceTemplates.AsNoTracking().ToList();
		foreach (var item in templates)
		{
			_marketInfluenceTemplates.Add(new Economy.Markets.MarketInfluenceTemplate(this, item));
		}


#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _marketInfluenceTemplates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Template" : "Templates");
		#endregion

		#region Markets
		ConsoleUtilities.WriteLine("\nLoading #5Markets#0...");
#if DEBUG
		sw.Restart();
#endif
		var markets = FMDB.Context.Markets
		                  .Include(x => x.MarketCategories)
		                  .Include(x => x.Influences)
		                  .AsNoTracking().ToList();
		foreach (var item in markets)
		{
			_markets.Add(new Economy.Markets.Market(this, item));
		}


#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _markets.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Market" : "Markets");
		#endregion

		#region Market Populations
		ConsoleUtilities.WriteLine("\nLoading #5Market Populations#0...");
#if DEBUG
		sw.Restart();
#endif
		var populations = FMDB.Context.MarketPopulations
		                      .AsNoTracking()
		                      .ToList();
		foreach (var item in populations)
		{
			_marketPopulations.Add(new Economy.Markets.MarketPopulation(this, item));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _marketPopulations.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Market Population" : "Market Populations");

		#endregion

		#region Shoppers
		ShopperBase.RegisterShopperTypes();
		ConsoleUtilities.WriteLine("\nLoading #5Shoppers#0...");
#if DEBUG
		sw.Restart();
#endif
		var shoppers = FMDB.Context.Shoppers
		                      .AsNoTracking()
		                      .ToList();
		foreach (var item in shoppers)
		{
			_shoppers.Add(Economy.Shoppers.ShopperBase.LoadShopper(item, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _shoppers.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Shopper" : "Shoppers");

		#endregion

	}

	void IFuturemudLoader.LoadLegal()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Legal System#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _legalAuthorities.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Legal Authority" : "Legal Authorities");

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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _witnessProfiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Witness Profile" : "Witness Profiles");
	}

	void IFuturemudLoader.LoadGrids()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Grids#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = grids.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Grid" : "Grids");
	}

	void IFuturemudLoader.LoadDisfigurements()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Disfigurement Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Disfigurement Template" : "Disfigurement Templates");
	}

	void IFuturemudLoader.LoadEconomy()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Economic Zones#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var zones = FMDB.Context.EconomicZones
		                .Include(x => x.FinancialPeriods)
		                .ThenInclude(x => x.ShopFinancialPeriodResults)
		                .Include(x => x.EconomicZoneRevenues)
		                .Include(x => x.EconomicZoneShopTaxes)
		                .Include(x => x.EconomicZoneTaxes)
		                .Include(x => x.ConveyancingLocations)
		                .Include(x => x.JobFindingLocations)
		                .AsSplitQuery()
		                .AsNoTracking()
		                .ToList();
		foreach (var zone in zones)
		{
			_economicZones.Add(new MudSharp.Economy.EconomicZone(zone, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = zones.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Economic Zone" : "Economic Zones");

		ConsoleUtilities.WriteLine("\nLoading #5Banks#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = banks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Bank" : "Banks");

		ConsoleUtilities.WriteLine("\nLoading #5Properties#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = properties.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Property" : "Properties");

		ConsoleUtilities.WriteLine("\nLoading #5Auction Houses#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = auctionhouses.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Auction House" : "Auction Houses");

		ConsoleUtilities.WriteLine("\nLoading #5Shops#0...");
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
		                .AsSplitQuery()
						.AsNoTracking()
		                .ToList();
		foreach (var shop in shops)
		{
			_shops.Add(Economy.Shops.Shop.LoadShop(shop, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = shops.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Shop" : "Shops");
	}


	void IFuturemudLoader.LoadClimate()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Seasons#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _seasons.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Season" : "Seasons");

		ConsoleUtilities.WriteLine("\nLoading #5Weather Events#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _weatherEvents.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Weather Event" : "Weather Events");

		ConsoleUtilities.WriteLine("\nLoading #5Climate Models#0...");
#if DEBUG
		sw = new Stopwatch();
		sw.Start();
#endif
		var models = FMDB.Context.ClimateModels
			.Include(x => x.ClimateModelSeasons)
			.ThenInclude(x => x.SeasonEvents)
			.AsSplitQuery()
			.AsNoTracking()
			.ToList();
		foreach (var model in models)
		{
			_climateModels.Add(Climate.ClimateModelFactory.LoadClimateModel(model, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = _climateModels.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Climate Model" : "Climate Models");

		ConsoleUtilities.WriteLine("\nLoading #5Regional Climates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = regionals.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Regional Climate" : "Regional Climates");

		ConsoleUtilities.WriteLine("\nLoading #5Weather Controllers#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = controllers.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Weather Controller" : "Weather Controllers");
	}


	void IFuturemudLoader.LoadMagic()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Magic Schools#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = schools.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "School" : "Schools");

		ConsoleUtilities.WriteLine("\nLoading #5Magic Resources#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = resources.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Resource" : "Resources");

		ConsoleUtilities.WriteLine("\nLoading #5Magic Resource Generators#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = generators.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Generator" : "Generators");

		ConsoleUtilities.WriteLine("\nLoading #5Magic Powers#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");

#endif
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}", count, count == 1 ? "Magic Power" : "Magic Powers");

		ConsoleUtilities.WriteLine("\nLoading #5Magic Spells#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = spells.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Spell" : "Spells");

		ConsoleUtilities.WriteLine("\nLoading #5Magic Capabilities#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = capabilities.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Capability" : "Capabilities");


		foreach (var verb in _magicSchools.Select(x => x.SchoolVerb).Distinct())
		{
			var command = new Command<ICharacter>(MagicModule.MagicGeneric, CharacterState.Conscious,
				PermissionLevel.Any, verb,
				CommandDisplayOptions.None,
				condition: MagicModule.MagicFilterFunction);
			PlayerCommandTree.Instance.Commands.Add(verb, command);
			NPCCommandTree.Instance.Commands.Add(verb, command);
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
		ConsoleUtilities.WriteLine("\nLoading #5Chargen Advices#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = advices.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Advice" : "Advices");
	}

	void IFuturemudLoader.LoadCharacterIntroTemplates()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Character Intro Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Template" : "Templates");
	}

	void IFuturemudLoader.LoadBloodtypes()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Bloodtype Antigens#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = antigens.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Antigen" : "Antigens");
		ConsoleUtilities.WriteLine("\n\nLoading #5Bloodtypes#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = bloodtypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Bloodtype" : "Bloodtypes");

		ConsoleUtilities.WriteLine("\n\nLoading #5Blood Models#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = bloodModels.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Blood Model" : "Blood Models");

		ConsoleUtilities.WriteLine("\n\nLoading #5Population Blood Models#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = antigens.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "Population Blood Model" : "Population Blood Models");
	}

	void IFuturemudLoader.LoadAutobuilderTemplates()
	{
		ConsoleUtilities.WriteLine("\n#ELoading Autobuilder Templates...#0");
		Construction.Autobuilder.AutobuilderFactory.InitialiseAutobuilders();
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ConsoleUtilities.WriteLine("\nLoading #5Room Templates...#0");
		var roomTemplates = FMDB.Context.AutobuilderRoomTemplates.AsNoTracking().ToList();
		foreach (var item in roomTemplates)
		{
			_autobuilderRooms.Add(Construction.Autobuilder.AutobuilderFactory.LoadRoom(item, this));
		}

		var count = _autobuilderRooms.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Room Template" : "Room Templates");

		ConsoleUtilities.WriteLine("\nLoading #5Area Templates...#0");
		var areaTemplates = FMDB.Context.AutobuilderAreaTemplates.AsNoTracking().ToList();
		foreach (var item in areaTemplates)
		{
			_autobuilderAreas.Add(Construction.Autobuilder.AutobuilderFactory.LoadArea(item, this));
		}

		count = _autobuilderAreas.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Area Template" : "Area Templates");
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
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
		ConsoleUtilities.WriteLine("\nLoading #5Butchery Products#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = products.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Product" : "Products");

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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = products.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Profile" : "Profiles");
	}

	void IFuturemudLoader.LoadBoards()
	{
		ConsoleUtilities.WriteLine("\nLoading #5boards#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = boards.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Board" : "Boards");
	}

	void IFuturemudLoader.LoadTraitExpressions()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Trait Expressions#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = expressions.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Trait Expression" : "Trait Expressions");
	}

	void IFuturemudLoader.LoadCrafts()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Crafts#0...");
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
		                 .AsSplitQuery()
		                 .AsNoTracking()
		                 .ToList();
		foreach (var item in crafts)
		{
			_crafts.Add(new Craft(item, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = crafts.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Craft" : "Crafts");
	}

	void IFuturemudLoader.LoadScripts()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Scripts#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = scripts.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Script" : "Scripts");
	}

	void IFuturemudLoader.LoadDrawings()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Drawings#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = drawings.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Drawing" : "Drawings");
	}

	void IFuturemudLoader.LoadWritings()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Writings#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = writings.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Writing" : "Writings");
	}

	void IFuturemudLoader.LoadCover()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Ranged Cover#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = covers.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Cover" : "Covers");
	}

	void IFuturemudLoader.LoadSurgery()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Surgical Procedures#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = surgeries.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Procedure" : "Procedures");
	}

	void IFuturemudLoader.LoadKnowledges()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Knowledges#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = knowledges.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Knowledge" : "Knowledges");
	}

	void IFuturemudLoader.LoadDrugs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Drugs#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = drugs.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Drug" : "Drugs");
	}

	void IFuturemudLoader.LoadCharacterCombatSettings()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Character Combat Settings#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = combatSettings.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "Character Combat Setting" : "Character Combat Settings");
	}

	void IFuturemudLoader.LoadArmourTypes()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Armour Types#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = armourTypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Armour Type" : "Armour Types");
	}

	void IFuturemudLoader.LoadWeaponTypes()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Weapon Attacks#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = attacks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Weapon Attack" : "Weapon Attacks");

		ConsoleUtilities.WriteLine("\nLoading #5Auxiliary Combat Action Types#0...");
		var actions = (from action in FMDB.Context.CombatActions.AsNoTracking() select action).ToList();
		foreach (var action in actions)
		{
			_auxiliaryCombatActions.Add(new AuxiliaryCombatAction(action, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		count = attacks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Auxiliary Combat Action" : "Auxiliary Combat Actions");

#if DEBUG
		sw.Restart();
#endif

		ConsoleUtilities.WriteLine("\nLoading #5Melee Weapon Types#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		count = weaponTypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Weapon Type" : "Weapon Types");

		ConsoleUtilities.WriteLine("\nLoading #5Ranged Weapon Types#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = rangedTypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Weapon Type" : "Weapon Types");

		ConsoleUtilities.WriteLine("\nLoading #5Ammunition Types#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = ammoTypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Ammo Type" : "Ammo Types");
	}

	void IFuturemudLoader.LoadShieldTypes()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Shield Types#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = shieldTypes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Shield Type" : "Shield Types");
	}

	void IFuturemudLoader.LoadDreams()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Dreams#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = dreams.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Dream" : "Dreams");
	}

	void IFuturemudLoader.LoadMerits()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Merits and Flaws#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _merits.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Merit or Flaw" : "Merits and Flaws");
	}

	void IFuturemudLoader.LoadHealthStrategies()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Health Strategies#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = strategies.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Health Strategy" : "Health Strategies");
	}

	void IFuturemudLoader.LoadForagables()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Foragables#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = foragables.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Foragable" : "Foragables");

		ConsoleUtilities.WriteLine("\nLoading #5Foragable Profiles#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = foragableProfiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Foragable Profile" : "Foragable Profiles");
	}

	void IFuturemudLoader.LoadCorpseModels()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Corpse Models#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = models.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Corpse Model" : "Corpse Models");
	}

	void IFuturemudLoader.LoadWorldItems()
	{
		ConsoleUtilities.WriteLine("\nLoading #5World Items#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
#if DEBUG
		//FMDB.Context.Database.Log = ConsoleUtilities.WriteLine;
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
#if DEBUG
		//FMDB.Context.Database.Log = null;
#endif
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Item" : "Items");

		// Initialise all the grids after the items are definitely loaded and in game
		ConsoleUtilities.WriteLine("#EInitialising Grids...#0");
		foreach (var grid in _grids)
		{
			grid.LoadTimeInitialise();
		}

		ConsoleUtilities.WriteLine("#EInitialising Shops...#0");
		foreach (var shop in _shops)
		{
			shop.PostLoadInitialisation();
		}

		ConsoleUtilities.WriteLine("#ELogging in world game items...#0");
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
		ConsoleUtilities.WriteLine("\nLoading #5Game Item Groups#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = groups.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Item Group" : "Item Groups");
	}

	void IFuturemudLoader.LoadSkyDescriptionTemplates()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Sky Description Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Template" : "Templates");
	}

	void IFuturemudLoader.LoadRoles()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Roles#0...");
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
		                              .AsSplitQuery()
		                              .AsNoTracking()
		             select item).ToList();

		foreach (var item in roles)
		{
			_roles.Add(new ChargenRole(item, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = roles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Role" : "Roles");
	}

	void IFuturemudLoader.LoadMaterials()
	{
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ConsoleUtilities.WriteLine("\nLoading #5Solids#0...");
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
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Solid" : "Solids");

		ConsoleUtilities.WriteLine("\nLoading #5Liquids#0...");
		var liquids = (from item in FMDB.Context.Liquids
		                                .Include(x => x.LiquidsTags)
		                                .AsNoTracking()
		               select item).ToList();

		foreach (var item in liquids)
		{
			_liquids.Add(new Liquid(item, this));
		}

		count = liquids.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Liquid" : "Liquids");

		ConsoleUtilities.WriteLine("\nLoading #5Gases#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = gases.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Gas" : "Gases");
	}

	void IFuturemudLoader.LoadSocials()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Socials#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = socials.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Social" : "Socials");
	}

	void IFuturemudLoader.LoadChargenResources()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Chargen Resources#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var resources = (from item in FMDB.Context.ChargenResources.AsNoTracking()
		                 select item).ToList();

		var stagingTable = new Dictionary<IChargenResource, ChargenResource>();
		foreach (var item in resources)
		{
			var newItem = ChargenResourceBase.LoadFromDatabase(this, item);
			stagingTable.Add(newItem, item);
			_chargenResources.Add(newItem);
		}

		foreach (var item in stagingTable)
		{
			item.Key.PerformPostLoadUpdate(item.Value, this);
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = resources.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Chargen Resource" : "Chargen Resources");
	}

	void IFuturemudLoader.LoadHeightWeightModels()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Height Weight Models#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var models = (from item in FMDB.Context.HeightWeightModels.AsNoTracking()
		              select item).ToList();
		foreach (var item in models)
		{
			_heightWeightModels.Add(new HeightWeightModel(this, item));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = models.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "Height Weight Model" : "Height Weight Models");
	}


	void IFuturemudLoader.LoadStaticValues()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Static Values#0...");
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
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Static String" : "Static Strings");

		var settings = (from item in FMDB.Context.StaticConfigurations.AsNoTracking()
		                select item).ToList();

		_staticConfigurations.Clear();
		foreach (var item in settings)
		{
			_staticConfigurations.Add(item.SettingName, item.Definition);
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = settings.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Static Setting" : "Static Settings");
	}

	void IFuturemudLoader.LoadChannels()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Channels#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = channels.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Channel{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadNonCardinalExitTemplates()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Non Cardinal Exit Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Non Cardinal Exit Template{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadNPCTemplates()
	{
		ConsoleUtilities.WriteLine("\nLoading #5NPC Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 NPC Template{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadNPCs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5NPCs#0...");
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
		            where !((CharacterState)npc.Character.State).HasFlag(CharacterState.Dead)
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = npcs.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 NPC{1}.", count, count == 1 ? "" : "s");

		ConsoleUtilities.WriteLine("\nLoading #5Guests#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		count = guests.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Guest{1}.", count, count == 1 ? "" : "s");

		foreach (var authority in _legalAuthorities)
		{
			authority.LoadPatrols();
		}

#if DEBUG
		sw.Restart();
#endif
		ConsoleUtilities.WriteLine("\nLoading #5NPC Spawners#0...");

		var spawners = (from spawner in FMDB.Context.NpcSpawners select spawner).ToList();

		foreach (var spawner in spawners)
		{
			_npcSpawners.Add(new NPC.NPCSpawner(spawner, this));
		}

		count = spawners.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 NPC Spawner{1}.", count, count == 1 ? "" : "s");
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
	}

	void IFuturemudLoader.LoadGroupAIs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Group AI Templates#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Template{1}.", count, count == 1 ? "" : "s");

		ConsoleUtilities.WriteLine("\nLoading #5Group AIs#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = templates.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 AI{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadAIs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5AIs#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = ais.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 AI{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHooks()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Hooks#0...");
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
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Hook{1}.", count, count == 1 ? "" : "s");

		ConsoleUtilities.WriteLine("\nLoading #5Default Hooks#0...");
		var defaultHooks = (from hook in FMDB.Context.DefaultHooks.AsNoTracking() select hook).ToList();
		foreach (var hook in defaultHooks)
		{
			_defaultHooks.Add(new DefaultHook(hook, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = defaultHooks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Default Hook{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCurrencies()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Currencies#0...");
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
			                      .AsSplitQuery()
								  .AsNoTracking()
		                  select currency).ToList();

		foreach (var currency in currencies)
		{
			_currencies.Add(new Currency(currency, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = currencies.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Currenc{1}.", count, count == 1 ? "y" : "ies");
	}

	void IFuturemudLoader.LoadChargen()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Character Creation Storyboard#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ChargenStoryboard = new ChargenStoryboard(this);
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		ConsoleUtilities.WriteLine("Done.");
	}

	void IFuturemudLoader.LoadEntityDescriptionPatterns()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Entity Description Patterns#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = patterns.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Entity Description Pattern{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCellOverlayPackages()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Cell Overlay Packages#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = packages.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Cell Overlay Package{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHelpFiles()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Help Files#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = helpfiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Help File{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadFutureProgs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5FutureProgs#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = progs.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 FutureProg{1}...Compiling...", count, count == 1 ? "" : "s");

		foreach (var prog in _futureProgs)
		{
			if (!prog.Compile())
			{
				ConsoleUtilities.WriteLine("#9FutureProg {0} ({2}) failed to compile: \n{1}#0", prog.Id, prog.CompileError, prog.FunctionName);
			}
		}
	}

	void IFuturemudLoader.LoadScriptedEvents()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Scripted Events#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var scripteds = (from scripted in FMDB.Context.ScriptedEvents
									  .Include(x => x.FreeTextQuestions)
									  .Include(x => x.MultipleChoiceQuestions)
									  .ThenInclude(x => x.Answers)
									  .AsSplitQuery()
									  .AsNoTracking()
					 select scripted).ToList();

		foreach (var scripted in scripteds)
		{
			_scriptedEvents.Add(new RPG.ScriptedEvents.ScriptedEvent(scripted, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = scripteds.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Scripted Event{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCharacteristics()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Characteristics#0...");
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
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Characteristic Definition{1}.", count, count == 1 ? "" : "s");

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
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Characteristic Value{1}.", count, count == 1 ? "" : "s");

		var profiles = (from profile in FMDB.Context.CharacteristicProfiles.AsNoTracking() select profile).ToList();
		foreach (var profile in profiles)
		{
			_characteristicProfiles.Add(CharacteristicProfile.LoadProfile(profile, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = profiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Characteristic Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadUnits()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Units of Measure#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		UnitManager = new UnitManager(this);
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = UnitManager.Units.Count();
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Unit{1} of Measure.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadStackDecorators()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Stack Decorators#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = _stackDecorators.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Stack Decorator{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadHearingProfiles()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Hearing Profiles#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = profiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Hearing Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadLanguageDifficultyModels()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Language Difficulty Models#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = models.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Language Difficulty Model{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadLanguages()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Languages and Accents#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = languages.Count;
		var accents = _accents.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Language{1} and #2{2}#0 Accent{3}.", count, count == 1 ? "" : "s", accents,
			accents == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadAuthorityGroups()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Authority Groups#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = authorityGroups.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Authority Group" : "Authority Groups");
	}

	void IFuturemudLoader.LoadTerrains()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Terrains#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = terrains.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Terrain" : "Terrains");
	}

	void IFuturemudLoader.LoadWorld()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Shards#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = shards.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Shard" : "Shards");

		// ------------------------------------------------------------------- //

		ConsoleUtilities.WriteLine("\nLoading #5Zones#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = zones.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 {1}.", count, count == 1 ? "Zone" : "Zones");

		// ------------------------------------------------------------------- //

		ConsoleUtilities.WriteLine("\nLoading #5Rooms#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = rooms.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 {1}.", count, count == 1 ? "Room" : "Rooms");

		// ------------------------------------------------------------------- //

		ConsoleUtilities.WriteLine("\nLoading #5Cells#0...");
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
			                 .AsSplitQuery()
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = cells.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Cell" : "Cells");

		ConsoleUtilities.WriteLine("\nLoading #5Areas#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = areas.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Area" : "Areas");

		// ------------------------------------------------------------------- //


		ConsoleUtilities.WriteLine("\n#ELoading Exit Manager and Preloading Critical Cell Exits...#0");
#if DEBUG
		sw.Restart();
#endif
		ExitManager = new ExitManager(this);
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		ConsoleUtilities.WriteLine("#AExit Manager Complete.#0");

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
		ConsoleUtilities.WriteLine("\n#ELoading Bodies...#0");
		ConsoleUtilities.WriteLine("\nLoading #5#5Bodypart Group Describers#0#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _bodypartGroupDescriptionRules.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "Bodypart Group Describer" : "Bodypart Group Describers");

#if DEBUG
		sw.Restart();
#endif
		ConsoleUtilities.WriteLine("\nLoading #5Bodyparts#0...");

		var bpprotos = FMDB.Context.BodypartProtos
		                   .Include(x => x.BodypartInternalInfosBodypartProto)
		                   .Include(x => x.BodypartInternalInfosInternalPart)
		                   .Include(x => x.BodyProtosAdditionalBodyparts)
		                   .Include(x => x.BodypartProtoBodypartProtoUpstreamChildNavigation)
		                   .Include(x => x.BoneOrganCoveragesBone)
		                   .Include(x => x.BoneOrganCoveragesOrgan)
						   .AsSplitQuery()
		                   .ToList();
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		ConsoleUtilities.WriteLine("\nLoading #5Body Protos#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = bodies.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Body Prototype" : "Body Prototypes");

		ConsoleUtilities.WriteLine("\n#ELinking Bodyparts...#0");
		foreach (var part in bpprotos)
		{
			BodypartPrototypes.Get(part.Id).SetBodyProto(BodyPrototypes.Get(part.BodyId));
		}

		ConsoleUtilities.WriteLine("#EFinalising Bodyparts...#0");
		foreach (var body in BodyPrototypes)
		{
			body.FinaliseBodyparts(bodyStaging[body]);
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		ConsoleUtilities.WriteLine("\nLoading #5Limbs#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = limbs.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Limb" : "Limbs");

		ConsoleUtilities.WriteLine("#EFinalising Bodypart Group Describers...#0");
		foreach (var item in bodypartStagingTable)
		{
			item.Key.FinaliseLoad(item.Value, this);
		}

		ConsoleUtilities.WriteLine("#ADone finalising Bodypart Group Describers...#0");

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
		ConsoleUtilities.WriteLine("\nLoading #5Bodypart Shapes#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = _bodypartShapes.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Bodypart Shape" : "Bodypart Shapes");
	}

	void IFuturemudLoader.LoadClans()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Clans#0...");
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
		                              .AsSplitQuery()
									  .AsNoTracking()
		             select clan).ToList();

		var memberships = (from membership in FMDB.Context.ClanMemberships
		                                          .Include(x => x.ClanMembershipsAppointments)
		                                          .Include(x => x.ClanMembershipsBackpay)
		                                          .AsNoTracking()
		                   select membership
			).ToList();

#if DEBUG
		ConsoleUtilities.WriteLine($"#E...Creating Clan objects#0 [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		var staging = new Dictionary<IClan, Clan>();
		foreach (var clan in clans)
		{
			var newClan = Community.Clan.ClanFactory.LoadClan(clan, this);
			_clans.Add(newClan);
			staging.Add(newClan, clan);
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"#E...Finalising Load of Clans#0 [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		foreach (var clan in staging)
		{
			clan.Key.FinaliseLoad(clan.Value, memberships);
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"#E...Loading External Controls#0 [#2{sw.ElapsedMilliseconds}ms#0]");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = clans.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Clan" : "Clans");
		count = externals.Count();
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "External Clan Control" : "External Clan Controls");
	}

	void IFuturemudLoader.LoadColours()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Colours#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var colours = FMDB.Context.Colours.AsNoTracking().ToList();
		foreach (var colour in colours)
		{
			_colours.Add(new Colour(colour, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _colours.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Colour" : "Colours");
	}

	void IFuturemudLoader.LoadTags()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Tags#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = tags.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Tag" : "Tags");
	}

	void IFuturemudLoader.LoadWearProfiles()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Wear Profiles#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = _wearProfiles.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Wear Profile{1}.", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadGameItemComponentProtos()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Game Item Component Protos#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _itemComponentProtos.Count();
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count,
			count == 1 ? "Game Item Component Proto" : "Game Item Component Protos");
	}

	void IFuturemudLoader.LoadGameItemProtos()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Game Item Protos#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = _itemProtos.Count();
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Game Item Proto" : "Game Item Protos");
	}

	void IFuturemudLoader.LoadGameItemSkins()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Game Item Skins#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = _itemSkins.Count();
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Game Item Skin" : "Game Item Skins");
	}

	void IFuturemudLoader.LoadImprovementModels()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Improvement Models#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _improvementModels.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Improvement Model{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadTraits()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Trait Definitions#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = traits.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Trait Definition{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadTraitDecorators()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Trait Decorators#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = decorators.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Trait Decorator{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadEthnicities()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Ethnicities#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		var ethnicities = (from ethnicity in FMDB.Context.Ethnicities
		                                         .Include(x => x.EthnicitiesCharacteristics)
		                                         .Include(x => x.EthnicitiesChargenResources)
		                                         .Include(x => x.EthnicitiesNameCultures)
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = ethnicities.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Ethnicit{1}", count, count == 1 ? "y" : "ies");
	}

	void IFuturemudLoader.LoadNameCultures()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Name Cultures#0...");
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
			                         .AsSplitQuery()
			                         .AsNoTracking()
			 select nameculture).ToList();
		foreach (var nameculture in namecultures)
		{
			_nameCultures.Add(new NameCulture(nameculture, this));
		}

		var count = namecultures.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Name Culture{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadCultures()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Cultures#0...");
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif

		var cultures = (from culture in FMDB.Context.Cultures
		                                    .Include(x => x.CulturesChargenResources)
		                                    .Include(x => x.ChargenAdvicesCultures)
		                                    .Include(x => x.CulturesNameCultures)
		                select culture).ToList();
		foreach (var culture in cultures)
		{
			_cultures.Add(new Culture(culture, this));
		}

		var count = cultures.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Culture{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadRaces()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Races#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = staging.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Race{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadProjects()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Projects#0...");
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
		                   .AsSplitQuery()
						   .AsNoTracking().ToList();

		foreach (var project in projects)
		{
			_projects.Add(ProjectFactory.LoadProject(project, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = projects.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Project" : "Projects");

		ConsoleUtilities.WriteLine("\nLoading #5Active Projects#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		count = projects.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Active Project" : "Active Projects");
	}

	void IFuturemudLoader.LoadJobs()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Job Listings#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = _jobListings.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Job Listing" : "Job Listings");
	}

	public void ReleasePrimedGameItems()
	{
		_bootTimeCachedGameItems = new Dictionary<long, GameItem>();
	}

	public void PrimeGameItems()
	{
		ConsoleUtilities.WriteLine("Preparing to Cache Game Items...");
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
		                .AsSplitQuery()
						.OrderBy(x => x.Id);

		while (true)
		{
#if DEBUG
			ConsoleUtilities.WriteLine($"#3...Retrieving records #2{i * 250} #3to #2{(i + 1) * 250}#3...#0");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		ConsoleUtilities.WriteLine("Done Caching Game Items.");
	}

	#region IFuturemudLoader Members

	void IFuturemudLoader.LoadCalendars()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Calendars#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = calendars.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Calendar{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadClocks()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Clocks#0...");
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = clocks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Clock{1}", count, count == 1 ? "" : "s");
	}

	void IFuturemudLoader.LoadChecks()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Checks#0...");
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
						checkType switch
						{
							CheckType.WritingComprehendCheck => FMDB.Context.CheckTemplates.FirstOrDefault(x =>
								x.Name == "CapabilityCheck" || x.Name == "Capability Check"),
							_ => FMDB.Context.CheckTemplates.FirstOrDefault(x =>
								x.Name == "SkillCheck" || x.Name == "Skill Check")

						} ??
						FMDB.Context.CheckTemplates.First(),
					MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
					Type = (int)checkType,
					TraitExpression = new Models.TraitExpression
					{
						Name = $"{checkType.DescribeEnum(true)} Check",
						Expression = checkType switch
						{
							CheckType.WritingComprehendCheck => "variable",
							CheckType.ClimbTreetoTreeCheck => _checks.FirstOrDefault(x => x.Type == CheckType.ClimbCheck)?.TargetNumberExpression.OriginalFormulaText ?? "50",
							_ => "50"
						}
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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = checks.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Check{1}", count, count == 1 ? "" : "s");
		foreach (var value in Enum.GetValues(typeof(CheckType)).OfType<CheckType>())
		{
			if (_checksIDs.ContainsKey(value))
			{
				continue;
			}

			ConsoleUtilities.WriteLine(
				$"#1Warning - no check defined for check type {(int)value:N0} ({Enum.GetName(typeof(CheckType), value)})#0");
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
		ConsoleUtilities.WriteLine("\nLoading #5Prog Schedules#0...");
		var schedules = (from schedule in FMDB.Context.ProgSchedules.AsNoTracking() select schedule).ToList();
		foreach (var schedule in schedules)
		{
			_progSchedules.Add(new ProgSchedule(schedule, this));
		}
#if DEBUG
		sw.Stop();
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif
		var count = schedules.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", count, count == 1 ? "Prog Schedule" : "Prog Schedules");
	}

	void IFuturemudLoader.LoadCelestials()
	{
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
#endif
		ConsoleUtilities.WriteLine("\nLoading #5Celestials#0...");

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
		ConsoleUtilities.WriteLine($"Duration: #2{sw.ElapsedMilliseconds}ms#0");
#endif

		var count = celestials.Count;
		ConsoleUtilities.WriteLine("Loaded #2{0}#0 Celestial Object{1}", count, count == 1 ? "" : "s");
	}

	#endregion
}