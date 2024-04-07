using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Combat;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.Scramblers;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Events.Hooks;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Health;
using MudSharp.Help;
using MudSharp.Network;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using System.Diagnostics;
using MudSharp.Body.Disfigurements;
using MudSharp.CharacterCreation;
using MudSharp.Climate;
using MudSharp.Magic;
using MudSharp.Economy;
using MudSharp.Construction.Grids;
using MudSharp.NPC.AI.Groups;
using MudSharp.RPG.Law;
using MudSharp.Communication;
using MudSharp.Effects;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Community.Boards;
using MudSharp.Economy.Property;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Dreams;
using MudSharp.Work.Butchering;
using MudSharp.Work.Crafts;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;
using MudSharp.RPG.ScriptedEvents;
using System.Xml.Linq;
using MudSharp.RPG.Hints;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine.Handlers;
using System.Numerics;
using MudSharp.Economy.Currency;

namespace MudSharp.Framework;

public sealed partial class Futuremud : IFuturemud, IDisposable
{
	public static IEnumerable<Type> GetAllTypes()
	{
		var databaseAssembly = Assembly.GetAssembly(typeof(FuturemudDatabaseContext));
		var libraryAssembly = Assembly.GetAssembly(typeof(IFuturemud));
		return new[] { Assembly.GetExecutingAssembly(), databaseAssembly, libraryAssembly }.SelectMany(x =>
			x.GetTypes());
	}

	private readonly Dictionary<string, Func<long, IPerceivable>> _perceivableTypeDictionary =
		new();

	private readonly Dictionary<string, string> _staticConfigurations = new();

	private readonly Dictionary<string, string> _staticStrings = new();

	private MaintenanceModeSetting _maintenanceMode;

	private bool _needsToHalt;

	public Futuremud(IServer server)
	{
		Server = server;
		EffectScheduler = new EffectScheduler(this);

		_allgames.Add(this);

		_actorCommandTrees.Add(PlayerCommandTree.Instance);
		_actorCommandTrees.Add(GuestCommandTree.Instance);
		_actorCommandTrees.Add(GuideCommandTree.Instance);
		_actorCommandTrees.Add(AdminCommandTree.JuniorAdminCommandTree);
		_actorCommandTrees.Add(AdminCommandTree.StandardAdminCommandTree);
		_actorCommandTrees.Add(AdminCommandTree.SeniorAdminCommandTree);
		_actorCommandTrees.Add(AdminCommandTree.HighAdminCommandTree);
		_actorCommandTrees.Add(FounderCommandTree.Instance);
		_actorCommandTrees.Add(NPCCommandTree.Instance);

		#region Test / Development Overrides

		LanguageScrambler = WordMaskScrambler.Instance; // TODO - load from config file
		ElectronicLanguageScrambler = ElectronicWordMaskScrambler.Instance;

		#endregion Test / Development Overrides

		ClockManager = new ClockManager(this);
		GameItemComponentManager = new GameItemComponentManager();
		Scheduler = new Scheduler();
		SaveManager = new SaveManager();
		HeartbeatManager = new HeartbeatManager(this);

		server?.Bind(_connections, AddConnection);
	}

	public string Name => GetStaticString("MudName");

	/// <summary>
	///     Determines whether or not players can log into the game
	/// </summary>
	public MaintenanceModeSetting MaintenanceMode
	{
		get => _maintenanceMode;
		set
		{
			_maintenanceMode = value;
			using (new FMDB())
			{
				var dbitem = FMDB.Context.StaticConfigurations.Find("MaintenanceMode");
				if (dbitem == null)
				{
					dbitem = new Models.StaticConfiguration();
					FMDB.Context.StaticConfigurations.Add(dbitem);
				}

				dbitem.Definition = ((int)_maintenanceMode).ToString();
				FMDB.Context.SaveChanges();
			}
		}
	}

	public static IEnumerable<IFuturemud> Games => _allgames;

	public void InitialiseTypes()
	{
		var pType = typeof(IPerceivable);
		foreach (
			var type in GetAllTypes().Where(x => x.GetInterfaces().Contains(pType)))
		{
			var method = type.GetMethod("RegisterPerceivableType", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { this });
		}
	}

	public IPerceivable GetPerceivable(string type, long id)
	{
		if (!_perceivableTypeDictionary.ContainsKey(type))
		{
			throw new ApplicationException(
				$"Perceivable Type {type} requested in Futuremud.GetPerceivable with ID {id}");
		}

		return _perceivableTypeDictionary[type](id);
	}

	public T GetPerceivable<T>(long id) where T : class, IPerceivable
	{
		return (T)_perceivableTypeDictionary[typeof(T).Name](id);
	}

	public void RegisterPerceivableType(string type, Func<long, IPerceivable> func)
	{
		_perceivableTypeDictionary.Add(type, func);
	}

	public void HaltGameLoop()
	{
		_needsToHalt = true;
	}

	public IAccount LogIn(string loginName, string password, IPlayerController controller)
	{
		var nameLower = loginName.ToLowerInvariant();

		using (new FMDB())
		{
			var accountLookup = (from dbAccount in FMDB.Context.Accounts
			                     where dbAccount.Name == nameLower
			                     select dbAccount).FirstOrDefault();
#if DEBUG
			if (accountLookup == null)
			{
				return null;
			}
#else
				if ((accountLookup == null) ||
				    !SecurityUtilities.VerifyPassword(password, accountLookup.Password, accountLookup.Salt))
				{
					return null;
				}
#endif


			var account = Accounts.Get(accountLookup.Id);

			Output output = null;
			if (account != null)
			{
				Console.WriteLine("Account " + account.Name.Proper() + " has reconnected.");
				output = new EmoteOutput(new Emote("Account " + account.Name.Proper() + " has logged in.", null));
			}
			else
			{
				account = new Account(accountLookup, this,
					Authorities.FirstOrDefault(
						x => x.Id == accountLookup.AuthorityGroupId));
				_accounts.Add(account);

				if (accountLookup.LastLoginIp == null)
				{
					Console.WriteLine("Account " + account.Name.Proper() + " has been created.");
					output = new EmoteOutput("Account " + account.Name.Proper() + " has been created.", null);
				}
				else
				{
					Console.WriteLine("Account " + account.Name.Proper() + " has logged in.");
					output = new EmoteOutput(
						new Emote("Account " + account.Name.Proper() + " has logged in.", null));
				}
			}

			SystemMessage(output, true);

			account.PreviousLastLoginTime = accountLookup.LastLoginTime ?? DateTime.MinValue;
			accountLookup.LastLoginTime = DateTime.UtcNow;
			account.LastLoginTime = DateTime.UtcNow;

			accountLookup.LastLoginIp = controller.IPAddress;
			GameStatistics.UpdateActiveAccount(accountLookup);
			//FMDB.Context.sp_LoginIP(controller.IPAddress, accountLookup.Id); // Having issues in Mysql80 with this for some reason
			// TODO: Missing first seen IP date/time
			FMDB.Context.SaveChanges();
			return account;
		}
	}

	public void StartGameLoop()
	{
		_needsToHalt = false;

		if (!Server.IsListeningAndResponding)
		{
			Server.Start();
		}

		EmailHelper.Instance.StartEmailThread();
		Thread.Sleep(50);
		try
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[SUCCESS] - MUD is now ready to connect");
			Console.ResetColor();

			var totalTime = new Stopwatch();
			var sw = new Stopwatch();
			var saveStopWatch = new Stopwatch();
			saveStopWatch.Start();
			while (!_needsToHalt)
			{
				totalTime.Restart();

				sw.Restart();
				ProcessPendingCommands();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - ProcessPendingCommands took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				WarnIdlers();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - WarnIdlers took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				ClockManager.UpdateClocks();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - ProcessPendingCommands took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				Scheduler.CheckSchedules();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - Scheduler.CheckSchedules() took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				EffectScheduler.CheckSchedules();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - EffectScheduler.CheckSchedules() took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				LogManager.FlushLog();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - LogManager.FlushLog() took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				ClearDeadConnections();
				if (sw.ElapsedMilliseconds > 250)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PERF] - ClearDeadConnections took {sw.ElapsedMilliseconds}ms");
					Console.ResetColor();
				}

				sw.Restart();
				if (GetStaticBool("UseDiscordBot"))
				{
					DiscordConnection.HandleMessages();

					if (sw.ElapsedMilliseconds > 250)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(
							$"[PERF] - DiscordConnection.HandleMessages() took {sw.ElapsedMilliseconds}ms");
						Console.ResetColor();
					}
				}

				if (saveStopWatch.ElapsedMilliseconds > 10000)
				{
					sw.Restart();
					SaveManager.Flush();
					if (sw.ElapsedMilliseconds > 250)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"[PERF] - SaveManager.Flush() took {sw.ElapsedMilliseconds}ms");
						Console.ResetColor();
					}

					saveStopWatch.Restart();
				}

				sw.Stop();
				totalTime.Stop();
//#if DEBUG
//#else
				if (totalTime.ElapsedMilliseconds < 250)
				{
					var milliseconds = 250 - totalTime.ElapsedMilliseconds;
					totalTime.Start();
					SaveManager.FlushLazyLoad(TimeSpan.FromMilliseconds(milliseconds));
				}

				totalTime.Stop();
//#endif
				Thread.Sleep(Math.Max(1, 250 - (int)totalTime.ElapsedMilliseconds));
			}

			SaveManager.Flush();
		}
		finally
		{
			using (new FMDB())
			{
				var time = DateTime.UtcNow;
				foreach (var dbchar in Characters.SelectNotNull(ch => FMDB.Context.Characters.Find(ch.Id)))
				{
					dbchar.LastLogoutTime = time;
				}

				FMDB.Context.SaveChanges();
			}
		}
	}

	public IAccount TryAccount(long accountid)
	{
		if (_accounts.Any(x => x.Id == accountid))
		{
			return _accounts.Get(accountid);
		}

		using (new FMDB())
		{
			var dbaccount = FMDB.Context.Accounts.Find(accountid);
			if (dbaccount is null)
			{
				return null;
			}

			var newAccount = new Account(dbaccount, this,
				Authorities.FirstOrDefault(x => x.Id == dbaccount.AuthorityGroupId));
			_accounts.Add(newAccount);

			return newAccount;
		}
	}

	public IAccount TryAccount(Models.Account account)
	{
		if (_accounts.Any(x => x.Id == account.Id))
		{
			return _accounts.Get(account.Id);
		}

		var newAccount = new Account(account, this,
			Authorities.FirstOrDefault(x => x.Id == account.AuthorityGroupId));
		_accounts.Add(newAccount);

		return newAccount;
	}

	public ICharacter TryPlayerCharacterByName(string name)
	{
		var result = 
			_cachedPersonalNames.GetByPersonalName(name);
		if (result is null)
		{
			return null;
		}

		return TryGetCharacter(result.Id, true);
	}

	private void CheckNewPlayerHints()
	{
		foreach (var character in Characters)
		{
			if (character.OutputHandler.QuietMode)
			{
				continue;
			}

			if (!character.Account.HintsEnabled)
			{
				continue;
			}

			var effect = character.CombinedEffectsOfType<NewPlayerHintsShown>().FirstOrDefault();
			if (effect is null)
			{
				effect = new NewPlayerHintsShown(character);
				effect.LastHintShown = DateTime.UtcNow;
				character.AddEffect(effect);
			}

			if ((DateTime.UtcNow - effect.LastHintShown) < TimeSpan.FromMinutes(15))
			{
				continue;
			}

			var hint = NewPlayerHints
				.Where(x => !effect.ShownHintIds.Contains(x.Id))
				.Where(x => x.FilterProg.ExecuteBool(character))
				.OrderByDescending(x => x.Priority)
				.WhereMax(x => x.Priority)
				.GetRandomElement();
			if (hint is null)
			{
				continue;
			}

			character.OutputHandler.Send($"#G[Hint]#0 {hint.Text}".SubstituteANSIColour().Wrap(character.InnerLineFormatLength));
			if (!hint.CanRepeat)
			{
				effect.ShownHintIds.Add(hint.Id);
			}
			
			effect.LastHintShown = DateTime.UtcNow;
			character.EffectsChanged = true;
			continue;
		}
	}

	public void PreloadAccounts()
	{
		ConsoleUtilities.WriteLine("\nPreloading #5Accounts#0...");
		using (new FMDB())
		{
			foreach (var dbaccount in FMDB.Context.Accounts.ToList())
			{
				if (_accounts.Has(dbaccount.Id))
				{
					continue;
				}

				var newAccount = new Account(dbaccount, this,
					Authorities.FirstOrDefault(x => x.Id == dbaccount.AuthorityGroupId));
				_accounts.Add(newAccount);
			}
		}
		ConsoleUtilities.WriteLine($"Preloaded #2{_accounts.Count:N0}#0 Accounts...");
	}

	public IEnumerable<ICharacter> LoadAllPlayerCharacters()
	{
		var loadedPCs = new List<ICharacter>();
		var onlinePCIDs = Characters.Select(x => x.Id).Concat(_cachedActors.Select(x => x.Id)).Distinct().ToHashSet();
		using (new FMDB())
		{
			var PCsToLoad =
				FMDB.Context.Characters.Where(
						x => !x.NpcsCharacter.Any() && x.Guest == null && !onlinePCIDs.Contains(x.Id))
				    .OrderBy(x => x.Id)
				    .Select(x => x.Id)
				    .ToList();
			foreach (var pc in PCsToLoad)
			{
				var character = TryGetCharacter(pc, true); // This will add to the cache
				character.Register(new NonPlayerOutputHandler());
				loadedPCs.Add(character);
			}
		}

		Console.WriteLine($"Loaded {loadedPCs.Count} offline PCs", true);
		return loadedPCs;
	}

	private record CharacterPersonalNameLookup : IHavePersonalName
	{
		public IPersonalName PersonalName { get; init; }
		public long Id { get; init; }
	}

	public void PreloadCharacterNames()
	{
		ConsoleUtilities.WriteLine("\nPreloading #5Character Name Lookups#0...");
		using (new FMDB())
		{
			foreach (var dbcharacter in FMDB.Context.Characters
				.Where(x => x.AccountId.HasValue)
				.OrderByDescending(x => x.Status == (int)CharacterStatus.Active)
				.ThenBy(x => x.TotalMinutesPlayed)
				.ToList())
			{
				_cachedPersonalNames.Add(new CharacterPersonalNameLookup { 
				PersonalName = new PersonalName(XElement.Parse(dbcharacter.NameInfo).Element("PersonalName").Element("Name"), this),
				Id = dbcharacter.Id
				});
			}
		}

		ConsoleUtilities.WriteLine($"Preloaded #2{_cachedPersonalNames.Count:N0}#0 Character Names...");
	}

	private void AddConnection(IPlayerConnection connection)
	{
		connection.Bind(new FuturemudControlContext(connection, this));
		if (connection.State == ConnectionState.Closed)
			// If this occurs, there has been an exception while initialising the player connection
		{
			return;
		}

		lock (_connections)
		{
			_connections.Add(connection);
		}
	}

	~Futuremud()
	{
		Dispose(false);
	}

	#region System Messaging

	/// <summary>
	///     Send a text message to every player character in game who meets the criteria specified in the filter function
	/// </summary>
	/// <param name="message">The message to be sent</param>
	/// <param name="filterFunc">The filter function to be applied to characters</param>
	public void SystemMessage(string message, Func<ICharacter, bool> filterFunc)
	{
		var realMessage = $"{"[System Message]".Colour(Telnet.Green)} {message}\n";
		foreach (var person in Actors.Where(filterFunc))
		{
			person.OutputHandler?.Send(realMessage, nopage: true);
		}

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"[System Message] {message.RawText()}");
		Console.ResetColor();
	}

	/// <summary>
	///     Send a text message to every player character in game, with a filter for admin-only messages
	/// </summary>
	/// <param name="message">The message to be sent</param>
	/// <param name="adminonly">If true, only sends to admin avatars. Otherwise sends to all players</param>
	public void SystemMessage(string message, bool adminonly = false)
	{
		if (adminonly)
		{
			SystemMessage(message, character => character.IsAdministrator());
		}
		else
		{
			SystemMessage(message, character => true);
		}
	}

	/// <summary>
	///     Send an Output to the OutputHandler every player character in game who meets the criteria specified in the filter
	///     function
	/// </summary>
	/// <param name="message">An Output to be handled by the character's OutputHandler</param>
	/// <param name="filterFunc">The filter function to be applied to characters</param>
	public void SystemMessage(IOutput message, Func<ICharacter, bool> filterFunc)
	{
		var systemMessage = "[System Message] ".Colour(Telnet.Green);
		foreach (var person in Actors.Where(x => filterFunc(x) && message.ShouldSee(x)))
		{
			person.OutputHandler?.Send($"{systemMessage}{message.ParseFor(person)}", nopage: true);
		}
	}

	/// <summary>
	///     Send an Output to the OutputHandler every player character in game, with a filter for admin-only messages
	/// </summary>
	/// <param name="message">An Output to be handled by the character's OutputHandler</param>
	/// <param name="adminonly">If true, only sends to admin avatars. Otherwise sends to all players</param>
	public void SystemMessage(IOutput message, bool adminonly = false)
	{
		if (adminonly)
		{
			SystemMessage(message, character => character.IsAdministrator());
		}
		else
		{
			SystemMessage(message, character => true);
		}
	}

	#endregion

	#region Special Add Methods

	public void AddGuest(ICharacter character)
	{
		_guests.Add(character);
	}

	public void Add(ICurrency currency)
	{
		_currencies.Add(currency);
	}

	public void Add(ICoin coin)
	{
		_coins.Add(coin);
	}

	public void Add(ICombatArena arena)
	{
		_combatArenas.Add(arena);
	}
	public void Add(IChannel channel)
	{
		_channels.Add(channel);
	}

	public void Add(IAuxiliaryCombatAction action)
	{
		_auxiliaryCombatActions.Add(action);
	}

	public void Add(IBoard board)
	{
		_boards.Add(board);
	}

	public void Add(IBodypartShape shape)
	{
		_bodypartShapes.Add(shape);
	}

	public void Add(INPCSpawner spawner)
	{
		_npcSpawners.Add(spawner);
	}

	public void Add(IJobListing listing)
	{
		_jobListings.Add(listing);
	}

	public void Add(IActiveJob job)
	{
		_activeJobs.Add(job);
	}

	public void Add(IGameItemSkin skin)
	{
		_itemSkins.Add(skin);
	}

	public void Add(IAuctionHouse house)
	{
		_auctionHouses.Add(house);
	}

	public void Add(IProperty property)
	{
		_properties.Add(property);
	}

	public void Add(IBank bank)
	{
		_banks.Add(bank);
	}

	public void Add(IBankAccount account)
	{
		_bankAccounts.Add(account);
	}

	public void Add(IBankAccountType type)
	{
		_bankAccountTypes.Add(type);
	}

	public void Add(IDream dream)
	{
		_dreams.Add(dream);
	}

	public void Add(IPatrol patrol)
	{
		_patrols.Add(patrol);
	}

	public void Add(IMagicSpell spell)
	{
		_magicSpells.Add(spell);
	}

	public void Add(INameCulture culture)
	{
		_nameCultures.Add(culture);
	}

	public void Add(ICulture culture)
	{
		_cultures.Add(culture);
	}

	public void Add(IRace race)
	{
		_races.Add(race);
	}

	public void Add(IRandomNameProfile profile)
	{
		_randomNameProfiles.Add(profile);
	}

	public void Add(ILineOfCreditAccount account)
	{
		_lineOfCreditAccounts.Add(account);
	}

	public void Add(IElection election)
	{
		_elections.Add(election);
	}

	public void Add(ILanguage language)
	{
		_languages.Add(language);
	}

	public void Add(IAccent accent)
	{
		_accents.Add(accent);
	}

	public void Add(IDrawing drawing)
	{
		_drawings.Add(drawing);
	}

	public void Add(ILegalAuthority item)
	{
		_legalAuthorities.Add(item);
	}

	public void Add(ILegalClass item)
	{
		_legalClasses.Add(item);
	}

	public void Add(IEnforcementAuthority item)
	{
		_enforcementAuthorities.Add(item);
	}

	public void Add(ILaw law)
	{
		_laws.Add(law);
	}

	public void Add(IWitnessProfile profile)
	{
		_witnessProfiles.Add(profile);
	}

	public void Add(ICrime crime)
	{
		_crimes.Add(crime);
	}

	public void Add(IGroupAITemplate item)
	{
		_groupAITemplates.Add(item);
	}

	public void Add(IGroupAI item)
	{
		_groupAIs.Add(item);
	}

	public void Add(ITerrain terrain)
	{
		_terrains.Add(terrain);
	}

	public void Add(IGrid grid)
	{
		_grids.Add(grid);
	}

	public void Add(IDisfigurementTemplate template)
	{
		_disfigurementTemplates.Add(template);
	}

	public void Add(IActiveProject project)
	{
		_activeProjects.Add(project);
	}

	public void Add(IProject project)
	{
		_projects.Add(project);
	}

	public void Add(IShop shop)
	{
		_shops.Add(shop);
	}

	public void Add(IEconomicZone zone)
	{
		_economicZones.Add(zone);
	}

	public void Add(IArea area)
	{
		_areas.Add(area);
	}

	public void Add(IClimateModel model)
	{
		_climateModels.Add(model);
	}

	public void Add(IRegionalClimate climate)
	{
		_regionalClimates.Add(climate);
	}

	public void Add(ISeason season)
	{
		_seasons.Add(season);
	}

	public void Add(IWeatherEvent weather)
	{
		_weatherEvents.Add(weather);
	}

	public void Add(IWeatherController controller)
	{
		_weatherControllers.Add(controller);
	}

	public void Add(ITraitExpression item)
	{
		_traitExpressions.Add(item);
	}

	public void Add(IWeaponAttack attack)
	{
		_weaponAttacks.Add(attack);
	}

	public void Add(IMagicSchool school)
	{
		_magicSchools.Add(school);
	}

	public void Add(IMagicCapability capability)
	{
		_magicCapabilities.Add(capability);
	}

	public void Add(IMagicPower power)
	{
		_magicPowers.Add(power);
	}

	public void Add(IMagicResource resource)
	{
		_magicResources.Add(resource);
	}

	public void Add(IMagicResourceRegenerator regenerator)
	{
		_magicResourceRegenerators.Add(regenerator);
	}

	public void Add(IChargenAdvice advice)
	{
		_chargenAdvices.Add(advice);
	}

	public void Add(ICharacterIntroTemplate template)
	{
		_characterIntroTemplates.Add(template);
	}

	public void Add(ICraft craft)
	{
		_crafts.Add(craft);
	}

	public void Add(IPopulationBloodModel model)
	{
		_populationBloodModels.Add(model);
	}

	public void Add(IBloodtype type)
	{
		_bloodtypes.Add(type);
	}

	public void Add(IBloodtypeAntigen antigen)
	{
		_bloodtypeAntigens.Add(antigen);
	}

	public void Add(IBloodModel model)
	{
		_bloodModels.Add(model);
	}

	public void Add(IAutobuilderRoom room)
	{
		_autobuilderRooms.Add(room);
	}

	public void Add(IAutobuilderArea area)
	{
		_autobuilderAreas.Add(area);
	}

	public void Add(IRaceButcheryProfile profile)
	{
		_raceButcheryProfiles.Add(profile);
	}

	public void Add(IButcheryProduct product)
	{
		_butcheryProducts.Add(product);
	}

	public void Add(IWriting writing)
	{
		_writings.Add(writing);
	}

	public void Add(IScript script)
	{
		_scripts.Add(script);
	}

	public void Add(ISurgicalProcedure procedure)
	{
		_surgicalProcedures.Add(procedure);
	}

	public void Add(IWearProfile profile)
	{
		_wearProfiles.Add(profile);
	}

	public void Add(ILimb limb)
	{
		_limbs.Add(limb);
	}

	public void Add(IWeaponType type)
	{
		_weaponTypes.Add(type);
	}

	public void Add(IRangedWeaponType type)
	{
		_rangedWeaponTypes.Add(type);
	}

	public void Add(IAmmunitionType type)
	{
		_ammunitionTypes.Add(type);
	}

	public void Add(IWearableSize size)
	{
		_wearableSizes.Add(size);
	}

	public void Add(ICharacterCombatSettings setting)
	{
		_characterCombatSettings.Add(setting);
	}

	public void Add(IProgSchedule schedule)
	{
		_progSchedules.Add(schedule);
	}

	public void Add(IMerit merit)
	{
		_merits.Add(merit);
	}

	public void Add(IHealthStrategy strategy)
	{
		_healthStrategies.Add(strategy);
	}

	public void Add(IBodypart proto)
	{
		_bodypartPrototypes.Add(proto);
	}

	public void Add(IForagableProfile foragableProfile)
	{
		_foragableProfiles.Add(foragableProfile);
	}

	public void Add(IForagable foragable)
	{
		_foragables.Add(foragable);
	}

	public void Add(IGameItemGroup group)
	{
		_itemGroups.Add(group);
	}

	public void Add(ICorpseModel model)
	{
		_corpseModels.Add(model);
	}

	public void Add(IGas gas)
	{
		_gases.Add(gas);
	}

	public void Add(ILiquid liquid)
	{
		_liquids.Add(liquid);
	}

	public void Add(IDefaultHook hook)
	{
		_defaultHooks.Add(hook);
	}

	public void Add(IKnowledge knowledge)
	{
		_knowledges.Add(knowledge);
	}

	public void Add(ITag tag)
	{
		_tags.Add(tag);
	}

	public void Add(IChargenRole role)
	{
		_roles.Add(role);
	}

	public void Add(ISkyDescriptionTemplate template)
	{
		_skyDescriptionTemplates.Add(template);
	}

	public void Add(IMaterial material)
	{
		_materials.Add(material);
	}

	public void Add(ICell cell)
	{
		_cells.Add(cell);
	}

	public void Add(IRoom room)
	{
		_rooms.Add(room);
	}

	public void Add(IZone zone)
	{
		_zones.Add(zone);
	}

	public void Add(IShard shard)
	{
		_shards.Add(shard);
	}

	public void Add(IArtificialIntelligence ai)
	{
		_AIs.Add(ai);
	}

	public void Add(INPCTemplate template)
	{
		_npcTemplates.Add(template);
	}

	public void Add(IHook hook)
	{
		_hooks.Add(hook);
	}

	public void Add(IEthnicity ethnicity)
	{
		_ethnicities.Add(ethnicity);
	}

	public void Add(IEntityDescriptionPattern pattern)
	{
		_entityDescriptionPatterns.Add(pattern);
	}

	public void Add(INonCardinalExitTemplate template)
	{
		_nonCardinalExitTemplates.Add(template);
	}

	public void Add(ICharacteristicDefinition definition)
	{
		_characteristics.Add(definition);
	}

	public void Add(ICharacteristicProfile profile)
	{
		_characteristicProfiles.Add(profile);
	}

	public void Add(ICharacteristicValue value)
	{
		_characteristicValues.Add(value);
	}

	public void Add(IBody body)
	{
		_bodies.Add(body);
	}

	public void Add(ICharacter actor, bool isNPC)
	{
		if (!_actors.Has(actor))
		{
			_actors.Add(actor);
		}

		if (isNPC)
		{
			if (!_NPCs.Has(actor))
			{
				_NPCs.Add(actor);
			}
		}
		else
		{
			if (!_characters.Has(actor))
			{
				_characters.Add(actor);
				_cachedPersonalNames.RemoveAll(x => x.Id == actor.Id);
				_cachedPersonalNames.Add(new CharacterPersonalNameLookup { PersonalName = actor.PersonalName, Id = actor.Id });
			}

			GameStatistics.UpdateOnlinePlayers();
		}

		if (_cachedActors.Has(actor))
		{
			_cachedActors.Remove(actor);
		}
	}

	public void Add(IGameItem item)
	{
		_items.Add(item);
	}

	public void Add(ICellOverlayPackage package)
	{
		_cellOverlayPackages.Add(package);
	}

	public void Add(IGameItemProto proto)
	{
		_itemProtos.Add(proto);
	}

	public void Add(IGameItemComponentProto proto)
	{
		_itemComponentProtos.Add(proto);
	}

	public void Add(ITemporalListener listener)
	{
		_listeners.Add(listener);
	}

	public void Add(IHelpfile helpfile)
	{
		_helpfiles.Add(helpfile);
	}

	public void Add(ITraitDefinition trait)
	{
		_traits.Add(trait);
	}

	public void Add(IClock clock)
	{
		_clocks.Add(clock);
	}

	public void Add(ICalendar calendar)
	{
		_calendars.Add(calendar);
	}

	public void Add(ICelestialObject celestial)
	{
		_celestialObjects.Add(celestial);
	}

	public void Add(IFutureProg prog)
	{
		_futureProgs.Add(prog);
	}

	public void Add(IClan clan)
	{
		_clans.Add(clan);
	}

	public void Add(IArmourType type)
	{
		_armourTypes.Add(type);
	}

	public void Add(IScriptedEvent item)
	{
		_scriptedEvents.Add(item);
	}

	public void Add(INewPlayerHint hint)
	{
		_newPlayerHints.Add(hint);
	}

	public void Add(IMarket market)
	{
		_markets.Add(market);
	}
	public void Add(IMarketCategory category)
	{
		_marketCategories.Add(category);
	}
	public void Add(IMarketInfluenceTemplate item)
	{
		_marketInfluenceTemplates.Add(item);
	}
	public void Add(IMarketInfluence item)
	{
		_marketInfluences.Add(item);
	}

	#endregion Special Add Methods

	#region Special Find

	private readonly Dictionary<CheckType, long> _checksIDs = new();

	public ICheck GetCheck(CheckType type)
	{
		return _checksIDs.ContainsKey(type) ? Checks.Get(_checksIDs[type]) : Checks.Get(_checksIDs[CheckType.None]);
	}

	/// <summary>
	///     Returns a character if they exist, but if they do not, loads them but does not add them to the game world
	/// </summary>
	/// <param name="id"></param>
	/// <param name="useCachedValues"></param>
	/// <returns></returns>
	public ICharacter TryGetCharacter(long id, bool useCachedValues = false)
	{
		if (id == 0)
		{
			return null;
		}

		if (_characters.Has(id))
		{
			return _characters.Get(id);
		}

		if (_actors.Has(id))
		{
			return _actors.Get(id);
		}

		if (_cachedActors.Has(id))
		{
			if (useCachedValues)
			{
				return _cachedActors.Get(id);
			}

			_cachedActors.RemoveAll(x => x.Id == id);
		}

		using (new FMDB())
		{
			var dbchar = (from ch in FMDB.Context.Characters
			              /*
			                        .Include(x => x.Body.BodiesGameItems)
			                        .ThenInclude(x => x.GameItem.GameItemComponents)
			                        .Include(x => x.Body.BodiesGameItems)
			                        .ThenInclude(x => x.GameItem.GameItemsMagicResources)
			                        .Include(x => x.Body.BodiesGameItems)
			                        .ThenInclude(x => x.GameItem.HooksPerceivables)
			                        .Include(x => x.Body.BodiesGameItems)
			                        .ThenInclude(x => x.GameItem.WoundsGameItem)
			                        .Include(x => x.CharactersAccents)
			                        .Include(x => x.CharactersLanguages)
			                        .Include(x => x.Body.BodiesSeveredParts)
			                        .Include(x => x.Body.Characteristics)
			                        .Include(x => x.AlliesCharacter)
			                        .Include(x => x.Body.Traits)
			                        .ThenInclude(x => x.TraitDefinition)
			                        .Include(x => x.PerceiverMerits)
			                        .Include(x => x.Body.PerceiverMerits)
			                        .Include(x => x.Body.BodiesDrugDoses)
			                        .Include(x => x.Body.BodiesProsthetics)
			                        .Include(x => x.Body.BodiesImplants)
			                        .Include(x => x.HooksPerceivables)
			                        .Include(x => x.CharactersChargenRoles)
			                        .Include(x => x.Dubs)
			                        .Include(x => x.CharactersScripts)
			                        .Include(x => x.ActiveProjects)
			                        .Include(x => x.CharacterKnowledges)
			                        .Include(x => x.Body.Wounds)
			                        .Include(x => x.NpcsCharacter)
			                        .Include(x => x.Guest)
			              */
			              where ch.Id == id
			              select ch).FirstOrDefault();
			if (dbchar == null)
			{
				return null;
			}

			Models.Npc dbnpc = null;
			if (dbchar.NpcsCharacter.Any())
			{
				dbnpc = dbchar.NpcsCharacter.First();
			}

			var dbguest = dbchar.Guest;

			ICharacter newChar;
			if (dbguest != null)
			{
				newChar = new GuestCharacter(dbchar, this);
			}
			else if (dbnpc != null)
			{
				newChar = new NPC.NPC(dbnpc, dbchar, this);
			}
			else
			{
				newChar = new Character.Character(dbchar, this, true);
			}

			newChar.SilentAssumeControl(new NPCController());
			_cachedActors.Add(newChar);
			return newChar;
		}
	}

	public IGameItem TryGetItem(Models.GameItem dbitem, bool addToGameworld)
	{
		if (dbitem == null)
		{
			return null;
		}

		if (dbitem.Id == 0)
		{
			return null;
		}

		if (_items.Has(dbitem.Id))
		{
			return _items.Get(dbitem.Id);
		}

		var newItem = new GameItem(dbitem, this);
		if (addToGameworld)
		{
			_items.Add(newItem);
		}

		return newItem;
	}

	private Dictionary<long, Models.GameItem> _bootTimeCachedGameItems;

	public IGameItem TryGetItem(long id, bool addToGameworld = false)
	{
		if (id == 0)
		{
			return null;
		}

		if (_items.Has(id))
		{
			return _items.Get(id);
		}

		var dbitem = _bootTimeCachedGameItems.ContainsKey(id) ? _bootTimeCachedGameItems[id] : null;
		if (dbitem != null)
		{
			return TryGetItem(dbitem, addToGameworld);
		}

		using (new FMDB())
		{
			dbitem = FMDB.Context.GameItems.Find(id);
			if (dbitem == null)
			{
				return null;
			}

			var newItem = new GameItem(dbitem, this);
			if (addToGameworld)
			{
				_items.Add(newItem);
			}

			return newItem;
		}
	}

	#endregion Special Find

	#region Destruction

	private void DestroyListeners(object obj)
	{
		foreach (var listener in _listeners.Where(x => x.PertainsTo(obj)).ToArray())
		{
			listener.CancelListener();
		}
	}

	

	public void Destroy(ICurrency currency)
	{
		_currencies.Remove(currency);
	}

	public void Destroy(ICoin coin)
	{
		_coins.Remove(coin);
	}
	public void Destroy(ICombatArena arena)
	{
		_combatArenas.Remove(arena);
	}
	public void Destroy(ICrime crime)
	{
		_crimes.Remove(crime);
	}

	public void Destroy(INPCSpawner spawner)
	{
		_npcSpawners.Remove(spawner);
	}

	public void Destroy(IJobListing listing)
	{
		_jobListings.Remove(listing);
	}

	public void Destroy(IActiveJob job)
	{
		_activeJobs.Remove(job);
	}

	public void Destroy(IGameItemSkin skin)
	{
		_itemSkins.Remove(skin);
	}

	public void Destroy(IAuctionHouse house)
	{
		_auctionHouses.Remove(house);
	}

	public void Destroy(IProperty property)
	{
		_properties.Remove(property);
	}

	public void Destroy(IPatrol patrol)
	{
		_patrols.Remove(patrol);
	}

	public void Destroy(IMagicSpell spell)
	{
		_magicSpells.Remove(spell);
	}

	public void Destroy(ILineOfCreditAccount account)
	{
		_lineOfCreditAccounts.Remove(account);
	}

	public void Destroy(IElection election)
	{
		_elections.Remove(election);
	}

	public void Destroy(IDrawing drawing)
	{
		_drawings.Remove(drawing);
	}

	public void Destroy(IGroupAI item)
	{
		_groupAIs.Remove(item);
	}

	public void Destroy(IGroupAITemplate item)
	{
		_groupAITemplates.Remove(item);
	}

	public void Destroy(ITerrain terrain)
	{
		_terrains.Remove(terrain);
	}

	public void Destroy(IGrid grid)
	{
		_grids.Remove(grid);
	}

	public void Destroy(IProject project)
	{
		_projects.Remove(project);
	}

	public void Destroy(IDisfigurementTemplate template)
	{
		_disfigurementTemplates.Remove(template);
	}

	public void Destroy(IActiveProject project)
	{
		_activeProjects.Remove(project);
	}

	public void Destroy(IShop shop)
	{
		_shops.Remove(shop);
	}

	public void Destroy(IEconomicZone zone)
	{
		_economicZones.Remove(zone);
	}

	public void Destroy(IArea area)
	{
		_areas.Remove(area);
	}

	public void Destroy(IClimateModel model)
	{
		_climateModels.Remove(model);
	}

	private void Destroy(IRegionalClimate climate)
	{
		_regionalClimates.Remove(climate);
	}

	private void Destroy(ISeason season)
	{
		_seasons.Remove(season);
	}

	private void Destroy(IWeatherController controller)
	{
		_weatherControllers.Remove(controller);
	}

	private void Destroy(IWeatherEvent weather)
	{
		_weatherEvents.Remove(weather);
	}

	public void Destroy(ITraitExpression expression)
	{
		_traitExpressions.Remove(expression);
	}

	public void Destroy(IWeaponAttack attack)
	{
		_weaponAttacks.Remove(attack);
	}

	public void Destroy(ICombatMessage message)
	{
		CombatMessageManager.RemoveCombatMessage(message);
	}

	public void Destroy(IChargenAdvice advice)
	{
		_chargenAdvices.Remove(advice);
	}

	public void Destroy(ICharacterIntroTemplate template)
	{
		_characterIntroTemplates.Remove(template);
	}

	public void Destroy(ICraft craft)
	{
		_crafts.Remove(craft);
	}

	public void Destroy(IPopulationBloodModel model)
	{
		_populationBloodModels.Remove(model);
	}

	public void Destroy(IBloodtype type)
	{
		_bloodtypes.Remove(type);
	}

	public void Destroy(IBloodtypeAntigen antigen)
	{
		_bloodtypeAntigens.Remove(antigen);
	}

	public void Destroy(IBloodModel model)
	{
		_bloodModels.Remove(model);
	}

	public void Destroy(IAutobuilderArea area)
	{
		_autobuilderAreas.Remove(area);
	}

	public void Destroy(IAutobuilderRoom room)
	{
		_autobuilderRooms.Remove(room);
	}

	public void Destroy(object obj)
	{
		throw new ApplicationException("No game level destroy implemented for this type.");
	}

	public void Destroy(IRaceButcheryProfile profile)
	{
		_raceButcheryProfiles.Remove(profile);
	}

	public void Destroy(IButcheryProduct product)
	{
		_butcheryProducts.Remove(product);
	}

	public void Destroy(ISurgicalProcedure procedure)
	{
		_surgicalProcedures.Remove(procedure);
	}

	public void Destroy(IWearProfile profile)
	{
		_wearProfiles.Remove(profile);
	}

	public void Destroy(IWeaponType type)
	{
		_weaponTypes.Remove(type);
	}

	public void Destroy(ILimb limb)
	{
		_limbs.Remove(limb);
	}

	public void Destroy(IRangedWeaponType type)
	{
		_rangedWeaponTypes.Remove(type);
	}

	public void Destroy(IAmmunitionType type)
	{
		_ammunitionTypes.Remove(type);
	}

	public void Destroy(IWearableSize size)
	{
		_wearableSizes.Remove(size);
	}

	public void Destroy(ICharacterCombatSettings setting)
	{
		_characterCombatSettings.Remove(setting);
	}

	public void Destroy(IProgSchedule schedule)
	{
		_progSchedules.Remove(schedule);
	}

	public void Destroy(IArmourType type)
	{
		_armourTypes.Remove(type);
	}

	public void Destroy(IMerit merit)
	{
		_merits.Remove(merit);
	}

	public void Destroy(IHealthStrategy strategy)
	{
		_healthStrategies.Remove(strategy);
	}

	public void Destroy(IBodypart proto)
	{
		_bodypartPrototypes.Remove(proto);
	}

	public void Destroy(IForagableProfile foragableProfile)
	{
		_foragableProfiles.Remove(foragableProfile);
	}

	public void Destroy(IForagable foragable)
	{
		_foragables.Remove(foragable);
	}

	public void Destroy(ICorpseModel model)
	{
		_corpseModels.Remove(model);
	}

	public void Destroy(IGameItemGroup group)
	{
		_itemGroups.Remove(group);
	}

	public void Destroy(IKnowledge knowledge)
	{
		_knowledges.Remove(knowledge);
	}

	public void Destroy(IDefaultHook hook)
	{
		_defaultHooks.Remove(hook);
	}

	public void Destroy(ILiquid item)
	{
		_liquids.Remove(item);
	}

	public void Destroy(IGas gas)
	{
		_gases.Remove(gas);
	}

	public void Destroy(ITag tag)
	{
		_tags.Remove(tag);
	}

	public void Destroy(ISkyDescriptionTemplate template)
	{
		_skyDescriptionTemplates.Remove(template);
	}

	public void Destroy(IChargenRole role)
	{
		_roles.Remove(role);
	}

	public void Destroy(ISolid material)
	{
		_materials.Remove(material);
	}

	public void Destroy(INPCTemplate template)
	{
		_npcTemplates.Remove(template);
	}

	public void Destroy(IArtificialIntelligence ai)
	{
		_AIs.Remove(ai);
	}

	public void Destroy(IHook hook)
	{
		_hooks.Remove(hook);
	}

	public void Destroy(IEthnicity ethnicity)
	{
		_ethnicities.Remove(ethnicity);
	}

	public void Destroy(IEntityDescriptionPattern pattern)
	{
		_entityDescriptionPatterns.Remove(pattern);
	}

	public void Destroy(IPlayerConnection connection)
	{
		lock (_connections)
		{
			_connections.Remove(connection);
		}
	}

	public void Destroy(INonCardinalExitTemplate template)
	{
		_nonCardinalExitTemplates.Remove(template);
	}

	public void Destroy(ICellOverlayPackage package)
	{
		_cellOverlayPackages.Remove(package);
	}

	public void Destroy(ICharacteristicDefinition definition)
	{
		_characteristics.Remove(definition);
	}

	public void Destroy(ICharacteristicProfile profile)
	{
		_characteristicProfiles.Remove(profile);
	}

	public void Destroy(ICharacteristicValue value)
	{
		_characteristicValues.Remove(value);
	}

	public void Destroy(IBody body)
	{
		_bodies.Remove(body);
		DestroyListeners(body);
	}

	public void Destroy(ICharacter actor)
	{
		_actors.Remove(actor);
		if (_characters.Remove(actor))
		{
			GameStatistics.UpdateOnlinePlayers();
		}

		_NPCs.Remove(actor);
		DestroyListeners(actor);
		if (!_cachedActors.Has(actor))
		{
			_cachedActors.Add(actor);
		}
	}

	public void Destroy(IHelpfile helpfile)
	{
		_helpfiles.Remove(helpfile);
	}

	public void Destroy(IGameItem item)
	{
		_items.Remove(item);
		DestroyListeners(item);
	}

	public void Destroy(IGameItemProto proto)
	{
		_itemProtos.Remove(proto);
	}

	public void Destroy(IGameItemComponentProto proto)
	{
		_itemComponentProtos.Remove(proto);
	}

	public void Destroy(IAccount account)
	{
		_accounts.Remove(account);
	}

	public void Destroy(ICell cell)
	{
		cell.Room.Destroy(cell);
		_cells.Remove(cell);
		DestroyListeners(cell);
	}

	public void Destroy(IRoom room)
	{
		_rooms.Remove(room);
		DestroyListeners(room);
	}

	public void Destroy(IZone zone)
	{
		_zones.Remove(zone);
		DestroyListeners(zone);
	}

	public void Destroy(IShard plane)
	{
		_shards.Remove(plane);
		DestroyListeners(plane);
	}

	public void Destroy(ICelestialObject celestial)
	{
		_celestialObjects.Remove(celestial);
		DestroyListeners(celestial);
	}

	public void Destroy(ITemporalListener listener)
	{
		_listeners.Remove(listener);
	}

	public void Destroy(ICalendar calendar)
	{
		_calendars.Remove(calendar);
	}

	public void Destroy(IClock clock)
	{
		_clocks.Remove(clock);
	}

	public void Destroy(ICulture culture)
	{
		_cultures.Remove(culture);
	}

	public void Destroy(INameCulture culture)
	{
		_nameCultures.Remove(culture);
	}

	public void Destroy(IRace race)
	{
		_races.Remove(race);
	}

	public void Destroy(IBodyPrototype body)
	{
		_bodyPrototypes.Remove(body);
	}

	public void Destroy(IBodypartGroupDescriber rule)
	{
		_bodypartGroupDescriptionRules.Remove(rule);
	}

	public void Destroy(IBodypartShape shape)
	{
		_bodypartShapes.Remove(shape);
	}

	public void Destroy(IColour colour)
	{
		_colours.Remove(colour);
	}

	public void Destroy(ITraitDefinition trait)
	{
		_traits.Remove(trait);
	}

	public void Destroy(ITraitValueDecorator decorator)
	{
		_traitDecorators.Remove(decorator);
	}

	public void Destroy(ICheck check)
	{
		_checks.Remove(check);
	}

	public void Destroy(IFutureProg prog)
	{
		_futureProgs.Remove(prog);
	}

	public void Destroy(IClan clan)
	{
		_clans.Remove(clan);
	}

	public void Destroy(IWriting writing)
	{
		_writings.Remove(writing);
	}

	public void Destroy(IScript script)
	{
		_scripts.Remove(script);
	}

	public void Destroy(IScriptedEvent item)
	{
		_scriptedEvents.Remove(item);
	}

	public void Destroy(INewPlayerHint hint)
	{
		_newPlayerHints.Remove(hint);
	}

	public void Destroy(IMarket market)
	{
		_markets.Remove(market);
	}

	public void Destroy(IMarketCategory category)
	{
		_marketCategories.Remove(category);
	}

	public void Destroy(IMarketInfluenceTemplate template)
	{
		_marketInfluenceTemplates.Remove(template);
	}

	public void Destroy(IMarketInfluence influence)
	{
		_marketInfluences.Remove(influence);
	}

	#endregion Destruction

	#region IDisposable Members

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	#endregion IDisposable Members

	private void ProcessPendingCommands()
	{
		lock (_connections)
		{
			foreach (
				var player in
				_connections.Where(
					            player => player.HasIncomingCommands && player.State == ConnectionState.Open)
				            .Shuffle())
			{
				player?.AttemptCommand();
			}

			foreach (var connection in _connections.ToList())
			{
				connection?.PrepareOutgoing();
			}
		}
	}

	public void ForceOutgoingMessages()
	{
		lock (_connections)
		{
			foreach (var connection in _connections.ToList())
			{
				connection.PrepareOutgoing();
				connection.SendOutgoing();
			}
		}
	}

	private void WarnIdlers()
	{
		lock (_connections)
		{
			foreach (var connection in _connections.Where(x => x.State == ConnectionState.Open))
			{
				connection.WarnTimeout();
			}
		}
	}

	private void ClearDeadConnections()
	{
		lock (_connections)
		{
			_connections.RemoveAll(c => c.State == ConnectionState.Closed);
		}
	}

	public override string ToString()
	{
		return Name.Proper();
	}

	private void Dispose(bool disposed)
	{
		_allgames.Remove(this);
		lock (_connections)
		{
			_connections.ForEach(t => t.Dispose());
		}
	}

	public void Broadcast(string text)
	{
		lock (_connections)
		{
			foreach (var connection in _connections)
			{
				connection.AddOutgoing(text);
			}
		}
	}

	public ICharacterCommandTree RetrieveAppropriateCommandTree(ICharacter character)
	{
		if (character == null)
		{
			return _actorCommandTrees.OrderByDescending(x => x.PermissionLevel).First();
		}

		ICharacterCommandTree biggestTree = null;

		if (character.IsGuest)
		{
			var guestPermission = _actorCommandTrees.FirstOrDefault(x => x.PermissionLevel == PermissionLevel.Guest);
			if (guestPermission != null)
			{
				return guestPermission;
			}
		}

		foreach (var tree in _actorCommandTrees.Where(x => x.PermissionLevel <= character.PermissionLevel))
		{
			if (biggestTree == null)
			{
				biggestTree = tree;
			}
			else
			{
				biggestTree = tree.PermissionLevel > biggestTree.PermissionLevel ? tree : biggestTree;
			}
		}

		return biggestTree;
	}
}