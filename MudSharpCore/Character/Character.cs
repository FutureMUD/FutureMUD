using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.Scramblers;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Menus;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.TimeAndDate.Date;
using MudSharp.Strategies.BodyStratagies;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.Combat;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.Models;
using MudSharp.Work.Projects;
using Attribute = MudSharp.Body.Traits.Subtypes.Attribute;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Character;

public partial class Character : PerceiverItem, ICharacter
{
	private IPersonalName _currentName;

	public Character(MudSharp.Models.Character character, IFuturemud gameworld, bool temporary = false)
		: base(character.Id)
	{
		if (character == null)
		{
			throw new ApplicationException("Trying to load non existent character");
		}

		EffectHandler = new EffectHandler(this);
		QueuedMoveCommands = new Queue<string>();
		Gameworld = gameworld;
		LoadFromDatabase(character);
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		LoginDateTime = character.LastLoginTime ?? DateTime.MinValue;
		LastMinutesUpdate = LoginDateTime;
		_dbTotalMinutesPlayed = character.TotalMinutesPlayed;
		LoadEffects(XElement.Parse(character.EffectData.IfNullOrWhiteSpace("<Effects/>")));

		Body.TotalBloodVolumeLitres = TotalBloodVolume(this);
		if (Body.CurrentBloodVolumeLitres <= 0 && !State.HasFlag(CharacterState.Dead))
		{
			Body.CurrentBloodVolumeLitres = Body.TotalBloodVolumeLitres;
		}

		Body.BaseLiverAlcoholRemovalKilogramsPerHour = LiverFunction(this);
		InitialiseStamina();
		LoadHooks(character.HooksPerceivables, "Character");
	}

	public Character(long characterId, IFuturemud game)
		: this(FMDB.Context.Characters.AsNoTracking().FirstOrDefault(x => characterId == x.Id), game)
	{
	}

	public Character(IFuturemud gameworld, ICharacterTemplate template)
	{
		_noSave = true;
		Gameworld = gameworld;
		Account = template.Account;
		Location = Gameworld.Cells.Get(template.SelectedStartingLocation?.Id ?? 0);
		Culture = template.SelectedCulture;
		Currency = Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID"));
		Birthday = template.SelectedBirthday;
		Handedness = template.Handedness;
		_gender = Gendering.Get(template.SelectedGender);
		_state = CharacterState.Awake;
		_status = CharacterStatus.Active;
		_personalName = new PersonalName(template.SelectedName.Culture, template.SelectedName.SaveToXml());
		Aliases = new List<IPersonalName>();
		_currentName = PersonalName;
		PermissionLevel = Account.Authority.Level >= PermissionLevel.Guide
			? PermissionLevel.Guide
			: PermissionLevel.Player;
		_dubs = new List<IDub>();

		Body = new Body.Implementations.Body(gameworld, this, template);

		NeedsModel = NeedsModelFactory.LoadNeedsModel(CharacterCreation.Chargen.NeedsModelProg != null
			? (string)CharacterCreation.Chargen.NeedsModelProg.Execute(template)
			: "NoNeeds", this);


		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		LoginDateTime = DateTime.UtcNow;
		LastMinutesUpdate = LoginDateTime;
		_dbTotalMinutesPlayed = 0;
		PositionState = PositionStanding.Instance;
		PositionModifier = PositionModifier.None;

		var comboMerits = new List<ComboMerit>();
		foreach (var merit in template.SelectedMerits)
		{
			if (merit is ComboMerit cm)
			{
				comboMerits.Add(cm);
			}

			if (merit.MeritScope != MeritScope.Character)
			{
				continue;
			}

			_merits.Add(merit);
		}

		foreach (var role in template.SelectedRoles)
		{
			_roles.Add(role);

			foreach (var clan in template.SelectedRoles.SelectMany(x => x.ClanMemberships).GroupBy(x => x.Clan))
			{
				var rank = clan.Select(x => x.Rank).FirstMax(x => x.RankNumber);
				IClanMembership newMembership;
				if (ClanMemberships.All(x => x.Clan != clan.Key))
				{
					newMembership = new Community.ClanMembership(Gameworld)
					{
						Clan = clan.Key,
						Rank = rank,
						Paygrade =
							clan.Select(x => x.Paygrade)
								.Where(x => rank.Paygrades.Contains(x))
								.FirstMax(x => x.PayAmount),
						PersonalName = CurrentName,
						JoinDate = clan.Key.Calendar.CurrentDate
					};
					clan.Key.Memberships.Add(newMembership);
					AddMembership(newMembership);
				}
				else
				{
					newMembership = ClanMemberships.First(x => x.Clan == clan.Key);
					if (rank.RankNumber > newMembership.Rank.RankNumber)
					{
						newMembership.Rank = rank;
						newMembership.Paygrade =
							clan.Select(x => x.Paygrade)
								.Where(x => rank.Paygrades.Contains(x))
								.FirstMax(x => x.PayAmount);
					}
				}

				newMembership.Appointments.AddRange(clan.SelectMany(x => x.Appointments).Distinct()
														.Where(x => !newMembership.Appointments.Contains(x)).ToList());
			}

			foreach (var adjustment in role.TraitAdjustments)
			{
				if (!adjustment.Value.giveIfMissing && !Body.HasTrait(adjustment.Key))
				{
					continue;
				}

				Body.SetTraitValue(adjustment.Key, Body.TraitRawValue(adjustment.Key) + adjustment.Value.amount);
			}

			foreach (var merit in role.AdditionalMerits)
			{
				if (Merits.Contains(merit))
				{
					continue;
				}

				if (merit is ComboMerit cm)
				{
					comboMerits.Add(cm);
				}

				if (merit.MeritScope != MeritScope.Character)
				{
					continue;
				}

				_merits.Add(merit);
			}
		}

		foreach (var merit in comboMerits)
		foreach (var included in merit.CharacterMerits.Where(x => x.MeritScope == MeritScope.Character))
		{
			_merits.Add(merit);
		}

		foreach (var knowledge in template.SelectedKnowledges)
		{
			_characterKnowledges.Add(new RPG.Knowledge.CharacterKnowledge(this, knowledge, "Chargen"));
			foreach (var script in Gameworld.Scripts.Where(x => x.ScriptKnowledge == knowledge))
			{
				if (_scripts.Contains(script))
				{
					continue;
				}

				_scripts.Add(script);
			}
		}

		Body.TotalBloodVolumeLitres = TotalBloodVolume(this);
		Body.CurrentBloodVolumeLitres = Body.TotalBloodVolumeLitres;
		//Body.BaseLiverAlcoholRemovalKilogramsPerHour = LiverFunction(this);
		InitialiseStamina();
		foreach (var language in template.SkillValues.SelectMany(skill =>
					 Gameworld.Languages.Where(x => x.LinkedTrait == skill.Item1)))
		{
			_languages.Add(language);
		}

		_currentLanguage = _languages.FirstOrDefault();
		foreach (var accent in template.SelectedAccents)
		{
			_accents[accent] = Difficulty.Trivial;
			if (!_preferredAccents.ContainsKey(accent.Language))
			{
				_preferredAccents[accent.Language] = accent;
			}
		}

		_currentAccent = _accents.Select(x => x.Key).FirstOrDefault(x => x.Language == _currentLanguage);

		// Initialise Traits
		foreach (var trait in Body.Traits)
		{
			trait.Initialise(Body);
		}

		Body.RecalculatePartsAndOrgans(); // Sometimes character merits can change these after the body already sets them
		Body.RecalculateItemHelpers();
		_noSave = false;
		CombatSettings = Gameworld.CharacterCombatSettings.FirstOrDefault(
							 x => x.GlobalTemplate && x.AvailabilityProg?.Execute<bool?>(this) == true) ??
						 Gameworld.CharacterCombatSettings.FirstOrDefault(x => x.GlobalTemplate);

		var hooks = Gameworld.DefaultHooks.Where(x => x.Applies(template, "Character")).ToList();
		foreach (var hook in hooks)
		{
			InstallHook(hook.Hook);
		}

		CurrentStamina = MaximumStamina;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public override InitialisationPhase InitialisationPhase => InitialisationPhase.First;

	public event PerceivableEvent OnStateChanged;

	public bool IsAdministrator(PermissionLevel level = PermissionLevel.JuniorAdmin)
	{
		return PermissionLevel >= level && EffectHandler.AffectedBy<IAdminSightEffect>();
	}

	public void ChangePermissionLevel(PermissionLevel newLevel)
	{
		PermissionLevel = newLevel;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
	}

	public override void Register(IOutputHandler handler)
	{
		if (handler == null)
		{
			handler = new NonPlayerOutputHandler();
		}

		OutputHandler = handler;
		handler?.Register(this);
		Body?.Register(handler);
	}

	public override bool CanHear(IPerceivable thing)
	{
		return Body.CanHear(thing);
	}

	public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return Body.CanSee(thing, flags);
	}

	public override double VisionPercentage => Body.VisionPercentage;

	public override bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
	{
		return Body.CanSense(thing, ignoreFuzzy);
	}

	public override bool CanSmell(IPerceivable thing)
	{
		return Body.CanSmell(thing);
	}

	public override PerceptionTypes NaturalPerceptionTypes => Body.NaturalPerceptionTypes;

	public override double IlluminationProvided
	{
		get
		{
			var sum = 0.0;
			foreach (var effect in CombinedEffectsOfType<IProduceIllumination>().Where(x => x.Applies()))
			{
				sum += effect.ProvidedLux;
			}

			sum += Body.ExternalItems.Sum(x => x.IlluminationProvided);
			return sum;
		}
	}

	private bool _namesChanged;

	public bool NamesChanged
	{
		get => _namesChanged;
		set
		{
			if (value && !_namesChanged)
			{
				Changed = true;
			}

			_namesChanged = value;
		}
	}

	private IPersonalName _personalName;

	public IPersonalName PersonalName
	{
		get => _personalName;
		set
		{
			_personalName = value;
			_name = value.GetName(NameStyle.GivenOnly);
			Changed = true;
			NamesChanged = true;
		}
	}

	public IList<IPersonalName> Aliases { get; protected set; }

	public IPersonalName CurrentName
	{
		get { return _currentName; }
		set
		{
#if DEBUG
#endif
			_currentName = value ??
						   throw new ApplicationException("Null CurrentName assigned in Character.CurrentName.Set");
			Changed = true;
			NamesChanged = true;
		}
	}

	public sealed override string Name => _personalName?.GetName(NameStyle.GivenOnly) ?? "Entity";

	public T EditingItem<T>() where T : class
	{
		return EffectHandler.EffectsOfType<BuilderEditingEffect<T>>().FirstOrDefault()?.EditingItem;
	}
#nullable enable
	public void SetEditingItem<T>(T? item) where T : class
	{
		EffectHandler.RemoveAllEffects(x => x is BuilderEditingEffect<T>);
		if (item is not null)
		{
			EffectHandler.AddEffect(new BuilderEditingEffect<T>(this) { EditingItem = item });
		}
	}
#nullable restore

	public bool BriefRoomDescs { get; set; }

	public Alignment Handedness { get; set; }

		public override SizeCategory Size => Body.CurrentContextualSize(SizeContext.None);

		public override bool IsSelf(IPerceivable other)
		{
			return base.IsSelf(other) || Body == other;
		}

	public override IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Body.GetKeywordsFor(voyeur);
	}

	public override bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = true, bool useContainsOverStartsWith = false)
	{
		return Body.HasKeyword(targetKeyword, voyeur, abbreviated, useContainsOverStartsWith) ||
			   (voyeur is ICharacter c && c.IsAdministrator() && PersonalName.GetName(NameStyle.FullWithNickname)
																			 .Split(new[] { ' ', '"', '-' },
																				 StringSplitOptions.RemoveEmptyEntries)
																			 .Any(x => x.StartsWith(targetKeyword,
																				 StringComparison
																					 .InvariantCultureIgnoreCase)));
		;
	}

	public override bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = true, bool useContainsOverStartsWith = false)
	{
		return Body.HasKeywords(targetKeywords, voyeur, abbreviated, useContainsOverStartsWith) ||
			   (voyeur is ICharacter c && c.IsAdministrator() && PersonalName.GetName(NameStyle.FullWithNickname)
																			 .Split(new[] { ' ', '"', '-' },
																				 StringSplitOptions.RemoveEmptyEntries)
																			 .Any(x => targetKeywords.Any(
																				 y => x.StartsWith(y,
																					 StringComparison
																						 .InvariantCultureIgnoreCase))));
		;
	}

	public override bool Sentient => true;

	public string GetConsiderString(IPerceiver voyeur)
	{
		return Body.GetConsiderString(voyeur);
	}

	public DateTime? PreviousLoginDateTime { get; set; }

	public DateTime? LastLogoutDateTime { get; set; }

	private DateTime _loginDateTime;

	public DateTime LoginDateTime
	{
		get => _loginDateTime;
		set
		{
			_loginDateTime = value;
			LoginTimeChanged = true;
		}
	}

	private bool _loginTimeChanged;

	public bool LoginTimeChanged
	{
		get => _loginTimeChanged;
		set
		{
			if (!_noSave && IsPlayerCharacter)
			{
				if (value)
				{
					Changed = true;
				}

				_loginTimeChanged = value;
			}
		}
	}

	public DateTime LastMinutesUpdate { get; set; }

	#region IHaveCulture

	public ICulture Culture { get; protected set; }

	#endregion

	public override void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false)
	{
		base.MoveTo(location, layer, exit);
		if (!noSave)
		{
			Changed = true;
			RemoveAllEffects(x => x.GetSubtype<IRemoveOnMovementEffect>()?.ShouldRemove() == true, true);
			Body.RemoveAllEffects(x => x.GetSubtype<IRemoveOnMovementEffect>()?.ShouldRemove() == true, true);
		}

		CheckAllTargets();
	}

	public override void Save()
	{
		var dbchar = FMDB.Context.Characters.Find(Id);
		if (dbchar == null)
		{
#if DEBUG

			throw new ApplicationException(
				$"Character ID {Id} could not find itself in the database to save. {HowSeen(this, colour: false)}");

#else
				Gameworld.SystemMessage(
					$"Critical Error: Character ID {Id} ({HowSeen(this)}) couldn't find itself in the database when it tried to save.",
					true);
				Changed = false;
				return;
#endif
		}

		dbchar.Location = Location?.Id ?? 1L;
		dbchar.RoomLayer = (int)RoomLayer;
		dbchar.State = (int)State;
		dbchar.Status = (int)_status;
		dbchar.CurrencyId = Currency?.Id;
		dbchar.Gender = (short)Gender.Enum;
		dbchar.DominantHandAlignment = (int)Handedness;
		dbchar.RoomBrief = BriefRoomDescs;
		dbchar.CombatBrief = BriefCombatMode;
		dbchar.NoMercy = NoMercy;

		if (NamesChanged)
		{
			dbchar.NameInfo = SaveNames().ToString();
			dbchar.Name = _name;
			_namesChanged = false;
		}

		if (NeedsModel.NeedsSave)
		{
			dbchar.AlcoholLitres = NeedsModel.AlcoholLitres;
			dbchar.WaterLitres = NeedsModel.WaterLitres;
			dbchar.Calories = NeedsModel.Calories;
			dbchar.DrinkSatiatedHours = NeedsModel.DrinkSatiatedHours;
			dbchar.FoodSatiatedHours = NeedsModel.FoodSatiatedHours;
		}

		dbchar.ShortTermPlan = ShortTermPlan;
		dbchar.LongTermPlan = LongTermPlan;
		dbchar.CurrentCombatSettingId = CombatSettings?.Id;
		dbchar.PreferredDefenseType = (int)PreferredDefenseType;

		SaveProjects(dbchar);

		if (PositionChanged)
		{
			SavePosition(dbchar);
		}

		if (LanguagesChanged)
		{
			SaveLanguages(dbchar);
		}

		if (AlliesChanged)
		{
			SaveAllies(dbchar);
		}

		base.Save();
		if (KnowledgesChanged)
		{
			SaveKnowledges(dbchar);
		}

		if (MeritsChanged)
		{
			SaveMerits(dbchar);
		}

		if (MagicChanged || ResourcesChanged)
		{
			SaveMagic(dbchar);
		}

		if (LoginTimeChanged)
		{
			dbchar.LastLoginTime = LoginDateTime;
			LoginTimeChanged = false;
		}

		if (OutfitsChanged)
		{
			SaveOutfits(dbchar);
		}

		if (EffectsChanged)
		{
			dbchar.EffectData = SaveEffects().ToString();
			EffectsChanged = false;
		}

		if (HooksChanged)
		{
			FMDB.Context.HooksPerceivables.RemoveRange(dbchar.HooksPerceivables);
			foreach (var hook in _installedHooks)
			{
				dbchar.HooksPerceivables.Add(new HooksPerceivable
				{
					Character = dbchar,
					HookId = hook.Id
				});
			}

			HooksChanged = false;
		}

		Changed = false;
	}

	#region IFormatProvider Members

	public override object GetFormat(Type formatType)
	{
		return Account.GetFormat(formatType);
	}

	#endregion

	public override int LineFormatLength => Account.LineFormatLength;

	public override int InnerLineFormatLength => Account.InnerLineFormatLength;

	public string ShowHealth(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine("You feel the following things about your health:");
		sb.AppendLine();
		if (NeedsToBreathe)
		{
			var lungs = Body.Organs.OfType<LungProto>().Select(x => Tuple.Create(x, x.OrganFunctionFactor(Body)))
							.ToList();
			var tracheas = Body.Organs.OfType<TracheaProto>().Select(x => Tuple.Create(x, x.OrganFunctionFactor(Body)))
							   .ToList();
			if (CanBreathe)
			{
				sb.AppendLine("You can breathe.".Colour(Telnet.BoldGreen));
			}
			else
			{
				sb.AppendLine("You cannot breathe.".Colour(Telnet.BoldRed));
			}

			foreach (var lung in lungs)
			{
				if (lung.Item2 > 0.95)
					//sb.AppendLine($"Your {lung.Item1.FullDescription()} feels pretty good.");
				{
					continue;
				}

				if (lung.Item2 > 0.75)
				{
					sb.AppendLine(
						$"Your {lung.Item1.FullDescription()} is a little sore, but still functional.".Colour(
							Telnet.BoldYellow));
					continue;
				}

				if (lung.Item2 > 0.5)
				{
					sb.AppendLine(
						$"Your {lung.Item1.FullDescription()} is fairly sore, and it hurts when you breathe deeply or rapidly."
							.Colour(Telnet.BoldOrange));
					continue;
				}

				if (lung.Item2 > 0.3)
				{
					sb.AppendLine(
						$"Your {lung.Item1.FullDescription()} is very sore, and is causing you to have a little bit of difficulty breathing."
							.Colour(Telnet.Orange));
					continue;
				}

				if (lung.Item2 > 0.0)
				{
					sb.AppendLine(
						$"Your {lung.Item1.FullDescription()} is extremely painful, and is hardly working at all."
							.Colour(Telnet.Red));
					continue;
				}

				sb.AppendLine($"Your {lung.Item1.FullDescription()} is extremely painful, and not working at all."
					.Colour(Telnet.BoldRed));
			}

			foreach (var trachea in tracheas)
			{
				if (trachea.Item2 > 0.95)
					//sb.AppendLine($"Your {trachea.Item1.FullDescription()} feels clear and unobstructed.");
				{
					continue;
				}

				if (trachea.Item2 > 0.75)
				{
					sb.AppendLine($"Your {trachea.Item1.FullDescription()} is a little sore, but still functional."
						.Colour(Telnet.BoldYellow));
					continue;
				}

				if (trachea.Item2 > 0.5)
				{
					sb.AppendLine(
						$"Your {trachea.Item1.FullDescription()} is fairly sore, and it hurts when you breathe deeply or rapidly."
							.Colour(Telnet.BoldOrange));
					continue;
				}

				if (trachea.Item2 > 0.3)
				{
					sb.AppendLine(
						$"Your {trachea.Item1.FullDescription()} is very sore, and is causing you to have a little bit of difficulty breathing."
							.Colour(Telnet.Orange));
					continue;
				}

				if (trachea.Item2 > 0.0)
				{
					sb.AppendLine(
						$"Your {trachea.Item1.FullDescription()} is extremely painful, and is hardly working at all."
							.Colour(Telnet.Red));
					continue;
				}

				sb.AppendLine($"Your {trachea.Item1.FullDescription()} is extremely painful, and not working at all."
					.Colour(Telnet.BoldRed));
			}
		}

		// What's your temperature like
		var (floor, ceiling) = Body.TolerableTemperatures(true);
		sb.AppendLine(
			$"You are currently feeling {TemperatureExtensions.SubjectiveTemperature(Location.CurrentTemperature(this), floor, ceiling).DescribeColour()}.");
		var tempStatus = HealthStrategy.CurrentTemperatureStatus(this);
		if (tempStatus != BodyTemperatureStatus.NormalTemperature)
		{
			sb.AppendLine($"You are {tempStatus.DescribeColour()}.");
		}

		// Sobriety level
		sb.AppendLine(
			$"You are {(NeedsModel.Status & NeedsResult.DrunkOnly).Describe().ToLowerInvariant().Colour(Telnet.Green)}.");

		// Do you have your second wind?
		if (CombinedEffectsOfType<SecondWindExhausted>().Any(x => x.Merit is null))
		{
			sb.AppendLine("You have already had your second wind.".Colour(Telnet.Yellow));
		}
		else
		{
			sb.AppendLine("You have not recently had a second wind and could have one.".Colour(Telnet.Green));
		}

		foreach (var merit in Merits.OfType<ISecondWindMerit>().Where(x => x.Applies(this)))
		{
			if (CombinedEffectsOfType<SecondWindExhausted>().Any(x => x.Merit == merit))
			{
				sb.AppendLine($"You have used the second wind from your {merit.Name.ColourName()} merit."
					.ColourIncludingReset(Telnet.Yellow));
			}
			else
			{
				sb.AppendLine($"You have not yet used the second wind from your {merit.Name.ColourName()} merit."
					.ColourIncludingReset(Telnet.Green));
			}
		}

		//Can you talk?
		if (!Body.Communications.CanVocalise(Body, Form.Audio.AudioVolume.Decent))
			//Can't talk. Why?
		{
			if (Body.Communications is HumanoidCommunicationStrategy hcs)
			{
				sb.AppendLine($"{hcs.WhyCannotVocalise(Body)}".Colour(Telnet.BoldRed));
			}
		}

		//Can you see?
		var eyes = Body.Bodyparts.OfType<EyeProto>();
		foreach (var eye in eyes.Where(x => Body.AffectedBy<IBodypartIneffectiveEffect>(x)))
		{
			sb.AppendLine($"You cannot see out of your {eye.FullDescription()}.".Colour(Telnet.BoldRed));
		}

		foreach (var cybereye in eyes.Where(x =>
					 Body.Prosthetics.Any(y => x.DownstreamOfPart(y.TargetBodypart) && !y.Functional)))
		{
			sb.AppendLine($"You cannot see out of your {cybereye.FullDescription()} because it is non-functional."
				.Colour(Telnet.BoldRed));
		}

		foreach (var blindfolded in eyes.Where(x => Body.WornItemsFor(x).Any(y => y.IsItemType<IBlindfold>())))
		{
			sb.AppendLine($"You cannot see out of your {blindfolded.FullDescription()} because it is blindfolded."
				.Colour(Telnet.BoldRed));
		}

		if (!Body.CanSee(this))
		{
			sb.AppendLine("You are unable to see at all!".Colour(Telnet.BoldRed));
		}

		//Can you hear?
		var ears = Body.Organs.OfType<EarProto>();
		foreach (var ear in ears.Where(x => Body.AffectedBy<IBodypartIneffectiveEffect>(x)))
		{
			sb.AppendLine($"You cannot hear out of your {ear.FullDescription()}.".Colour(Telnet.BoldRed));
		}

		foreach (var cyberear in ears.Where(x =>
					 Body.Prosthetics.Any(y => x.DownstreamOfPart(y.TargetBodypart) && !y.Functional)))
		{
			sb.Append(
				$"You cannot hear out of your {cyberear.FullDescription()} because it is non-funcitonal.".Colour(
					Telnet.BoldRed));
		}

		if (!Body.CanHear(this))
		{
			sb.AppendLine("You are unable to hear at all!".Colour(Telnet.BoldRed));
		}

		var bloodRatio = Body.CurrentBloodVolumeLitres / Body.TotalBloodVolumeLitres;
		if (bloodRatio <= 0.65)
		{
			sb.AppendLine($"You feel light as a feather and your head is spinning; you also feel tranquil and at peace."
				.Colour(Telnet.BoldRed));
		}
		else if (bloodRatio <= 0.725)
		{
			sb.AppendLine(
				$"You feel very weak, and your throat feels parched. You feel quite light headed."
					.Colour(Telnet.Orange));
		}
		else if (bloodRatio <= 0.825)
		{
			sb.AppendLine(
				$"You feel weak, your throat feels dry, and you feel very anxious and light headed.".Colour(
					Telnet.BoldOrange));
		}
		else if (bloodRatio <= 0.9)
		{
			sb.AppendLine($"You feel a little weak, and have a general sense of anxiety.".Colour(Telnet.Yellow));
		}

		var hearts = Body.Organs.OfType<HeartProto>().Select(x => Tuple.Create(x, x.OrganFunctionFactor(Body)))
						 .ToList();
		if (hearts.Any())
		{
			foreach (var heart in hearts)
			{
				if (heart.Item2 <= 0.0)
				{
					sb.AppendLine(
						$"There is an all-encompassing sensation of crushing right over your {heart.Item1.FullDescription()}."
							.Colour(Telnet.BoldRed));
				}
				else if (heart.Item2 < 0.3)
				{
					sb.AppendLine(
						$"There is a strong sensation of tightness and pain radiating from your {heart.Item1.FullDescription()}."
							.Colour(Telnet.BoldRed));
				}
				else if (heart.Item2 < 0.5)
				{
					sb.AppendLine(
						$"Your {heart.Item1.FullDescription()} is beating far too fast and feels quite painful.".Colour(
							Telnet.Orange));
				}
				else if (heart.Item2 < 0.7)
				{
					sb.AppendLine($"Your {heart.Item1.FullDescription()} is beating rapidly and feels mildly painful."
						.Colour(Telnet.BoldOrange));
				}
				else if (heart.Item2 < 0.95)
				{
					sb.AppendLine(
						$"Your {heart.Item1.FullDescription()} is beating a little fast and feels uncomfortable."
							.Colour(Telnet.BoldYellow));
				}
			}
		}

		sb.AppendLine();
		foreach (var part in Body.Bodyparts.Where(x => x.Organs.Any() || x.IsVital).ToList())
		{
			var wounds = Body.Wounds.Where(x => x.Bodypart == part).ToList();
			var internalWounds = Body.Wounds
									 .Where(x => part.Organs.Contains(x.Bodypart) || part.Bones.Contains(x.Bodypart))
									 .ToList();
			if (!wounds.Any() && !internalWounds.Any())
			{
				continue;
			}

			var painRatio = wounds.Sum(x => x.CurrentPain) * 0.5 / Body.HitpointsForBodypart(part);
			var internalStressRatio = internalWounds.Sum(x => Math.Max(x.CurrentDamage, x.CurrentPain)) /
									  Body.HitpointsForBodypart(part);
			var totalRatio = painRatio + internalStressRatio;
			if (double.IsNaN(totalRatio))
			{
				totalRatio = 0.0;
			}

			if (totalRatio <= 0.1)
			{
				continue;
			}

			if (totalRatio > 3.0)
			{
				sb.AppendLine($"Your {part.FullDescription()} is in excruciating agony.".Colour(Telnet.BoldMagenta));
			}
			else if (totalRatio > 2.0)
			{
				sb.AppendLine($"Your {part.FullDescription()} is in agonizing pain.".Colour(Telnet.BoldRed));
			}
			else if (totalRatio > 1.0)
			{
				sb.AppendLine($"Your {part.FullDescription()} is in a great deal of pain.".Colour(Telnet.BoldRed));
			}
			else if (totalRatio > 0.75)
			{
				sb.AppendLine($"Your {part.FullDescription()} is in in a fair amount of pain.".Colour(Telnet.Red));
			}
			else if (totalRatio > 0.5)
			{
				sb.AppendLine($"Your {part.FullDescription()} is in a moderate amount of pain.".Colour(Telnet.Orange));
			}
			else if (totalRatio > 0.35)
			{
				sb.AppendLine($"Your {part.FullDescription()} is extremely uncomfortable and perhaps a little painful."
					.Colour(Telnet.BoldOrange));
			}
			else if (totalRatio > 0.2)
			{
				sb.AppendLine($"Your {part.FullDescription()} is moderately uncomfortable.".Colour(Telnet.BoldYellow));
			}
			else
			{
				sb.AppendLine($"Your {part.FullDescription()} is a little uncomfortable.".Colour(Telnet.BoldYellow));
			}
		}

		foreach (var limb in Body.Limbs)
		{
			var canUseLimb = Body.CanUseLimb(limb);
			if ((limb.LimbType == LimbType.Head || limb.LimbType == LimbType.Torso) && !canUseLimb.In(
					CanUseLimbResult.CantUseGrappled, CanUseLimbResult.CantUseRestrained,
					CanUseLimbResult.CantUseSpinalDamage))
			{
				continue;
			}

			switch (canUseLimb)
			{
				case CanUseLimbResult.CantUseDamage:
					sb.AppendLine($"Your {limb.Name} is too damaged to function.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUsePain:
					sb.AppendLine($"Your {limb.Name} is in too much pain to function.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUseSevered:
					sb.AppendLine($"Your {limb.Name} has been severed.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUseGrappled:
					sb.AppendLine($"Your {limb.Name} is being grappled and restrained.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUseRestrained:
					sb.AppendLine($"Your {limb.Name} is being grappled and restrained.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUseMissingBone:
					sb.AppendLine(
						$"Your {limb.Name} is missing a vital bone and is totally useless.".Colour(Telnet.BoldRed));
					continue;
				case CanUseLimbResult.CantUseSpinalDamage:
					sb.AppendLine($"You can neither feel nor move your {limb.Name}.".Colour(Telnet.BoldRed));
					continue;
			}
		}

		sb.AppendLine();
		//Can you stand? 
		if (!CanMovePosition(PositionStanding.Instance, PositionModifier.None, null, true))
		{
			sb.AppendLine(
				$"You cannot stand because: {WhyCannotMovePosition(PositionStanding.Instance, PositionModifier.None, null, true)}"
					.Colour(Telnet.BoldRed));
		}

		foreach (var infection in Body.PartInfections.Select(x => (infection: x,
					 part: x.Bodypart is IOrganProto op
						 ? Body.Bodyparts.Where(y => y.Organs.Contains(op)).OrderByDescending(y => y.RelativeHitChance)
							   .FirstOrDefault()
						 : x.Bodypart)).GroupBy(x => x.part))
		{
			var intensity = infection.Sum(x => x.infection.Intensity);
			if (intensity < 100.0)
			{
				continue;
			}

			if (intensity < 250.0)
			{
				sb.AppendLine(
					$"You're pretty sure that you have an infection in your {infection.Key.FullDescription()}.".Colour(
						Telnet.BoldYellow));
				continue;
			}

			if (intensity < 300.0)
			{
				sb.AppendLine($"You're positive that you have an infection in your {infection.Key.FullDescription()}."
					.Colour(Telnet.BoldOrange));
				continue;
			}

			if (intensity < 500.0)
			{
				sb.AppendLine(
					$"You're positive that you have a pretty bad infection in your {infection.Key.FullDescription()}."
						.Colour(Telnet.Orange));
				continue;
			}

			sb.AppendLine(
				$"You have a very severe infection in your {infection.Key.FullDescription()}.".Colour(Telnet.BoldRed));
			continue;
		}

		foreach (var effect in CombinedEffectsOfType<IScoreAddendumEffect>().Where(x => x.ShowInHealth))
		{
			sb.AppendLine($"{effect.ScoreAddendum}");
		}

		return sb.ToString();
	}

	public virtual string ShowStat(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"{(IsPlayerCharacter ? IsGuest ? "Guest" : "Player" : "NPC")} Character #{Id.ToString("N0", voyeur)}".GetLineWithTitle(voyeur, Telnet.Green, Telnet.BoldWhite));
		sb.AppendLine($"Short Description: {HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreDisguises)}");
		sb.AppendLine($"Name: {PersonalName.GetName(NameStyle.FullWithNickname).ColourName()}");
		sb.AppendLine($"Aliases: {Aliases.Select(x => x.GetName(NameStyle.FullWithNickname).ColourName()).ListToString()}");
		sb.AppendLine($"Current Name: {CurrentName.GetName(NameStyle.FullWithNickname).ColourName()}");
		sb.AppendLine($"Account: {(Account is DummyAccount ? "NPC".ColourError() : Account.Name.ColourName())}");
		sb.AppendLine($"Status: {Status.Describe().ColourValue()}");
		sb.AppendLine($"Race: {Race.Name.ColourValue()}");
		sb.AppendLine($"Culture: {Culture.Name.ColourValue()}");
		sb.AppendLine($"Ethnicity: {Ethnicity.Name.ColourValue()}");
		sb.AppendLine($"Gender: {Gender.Name.ColourValue()}");
		sb.AppendLine($"Age: {AgeInYears.ToString("N0", voyeur).ColourValue()} ({AgeCategory.DescribeEnum()})");
		
		
		sb.AppendLine($"Height: {Gameworld.UnitManager.Describe(Body.Height, UnitType.Length, voyeur).ColourValue()}");
		sb.AppendLine($"Weight: {Gameworld.UnitManager.DescribeMostSignificant(Body.Weight, UnitType.Mass, voyeur).ColourValue()}");
		sb.AppendLine($"Location: {Location?.GetFriendlyReference(voyeur).ColourValue() ?? "Nowhere".ColourError()} ({RoomLayer.LocativeDescription().ColourValue()})");
		sb.AppendLine($"Short Term Plan: {ShortTermPlan?.ColourCommand() ?? "None".ColourError()}");
		sb.AppendLine($"Long Term Plan: {LongTermPlan?.ColourCommand() ?? "None".ColourError()}");
		if (CurrentProject.Project is not null)
		{
			sb.AppendLine($"Project: {CurrentProject.Project.Name.ColourName()} (#{CurrentProject.Project.Id.ToString("N0", voyeur)}) - {CurrentProject.Labour.Description.ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Project: {"None".ColourError()}");
		}
		sb.AppendLine();
		sb.AppendLine($"Stats: {TraitsOfType(TraitType.Attribute).Select(x => $"{x.Definition.Name.ColourName()} [{x.Value.ToString("N0", voyeur).ColourValue()}]").ListToString(separator: " ", conjunction: "")}");
		sb.AppendLine();
		sb.AppendLine($"Skills:");
		sb.AppendLine();
		sb.Append(TraitsOfType(TraitType.Skill).Select(x => $"{x.Definition.Name.ColourName()} [{x.Value.ToString("N2", voyeur).ColourValue()} | {x.MaxValue.ToString("N2", voyeur).ColourValue()}]")
											   .ArrangeStringsOntoLines(4,
												   (uint)voyeur.LineFormatLength)
		);
		return sb.ToString();
	}

	public int AgeInYears => Location.Date(Birthday.Calendar).YearsDifference(Birthday);
	public AgeCategory AgeCategory => Race.AgeCategory(this);

	public string ShowScore(IPerceiver voyeur)
	{
		var sb = new StringBuilder()
			.AppendLine(
				$"{PersonalName.GetName(NameStyle.FullName)} {HowSeen(voyeur, type: DescriptionType.Short, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee).Parentheses()}");

		if (Gameworld.GetStaticBool("ShowAttributesInScore"))
		{
			sb.AppendLine(
				Body.Traits
					.OfType<IAttribute>()
					.Where(x => !x.Hidden && x.AttributeDefinition.ShowInScoreCommand)
					.OrderBy(x => x.AttributeDefinition.DisplayOrder)
					.Select(x =>
						$"{x.AttributeDefinition.Alias.Proper()}: {Body.GetTraitDecorated(x.Definition).ColourIfNotColoured(Telnet.Green)}")
					.ToList()
					.ListToString("", ", ", "", ""));
		}

		sb.AppendLine(
			  CommonStringUtilities.CultureFormat(
				  $"You are {Location.Date(Birthday.Calendar).YearsDifference(Birthday).A_An(colour: Telnet.Green)} year old {Race.Name.ToLowerInvariant().Colour(Telnet.Green)} {Gender.GenderClass().Colour(Telnet.Green)}, and belong to the {Culture.Name.Colour(Telnet.Green)} culture.",
				  Account))
		  .AppendLine($"You are {Race.AgeCategory(this).DescribeEnum(true).A_An(false, Telnet.Green)}.");

		if (Account.AccountResources.Any(x => x.Value > 0 && x.Key.ShowToPlayerInScore))
		{
			sb.AppendLine(
				CommonStringUtilities.CultureFormat(
					$"Your account has {Account.AccountResources.Where(x => x.Value > 0 && x.Key.ShowToPlayerInScore).Select(x => $"{x.Value.ToString("N0", voyeur)} {(x.Value == 1 ? x.Key.Name : x.Key.PluralName)}".ColourValue()).ListToString()}.",
					Account));
		}

		if (Roles.Any(x => x.RoleType == ChargenRoleType.Class))
		{
			sb.AppendLine(
				$"You are {Roles.First(x => x.RoleType == ChargenRoleType.Class).Name.TitleCase().A_An(colour: Telnet.Green)}{(Roles.Any(x => x.RoleType == ChargenRoleType.Subclass) ? $", of subclass {Roles.First(x => x.RoleType == ChargenRoleType.Subclass).Name.TitleCase().Colour(Telnet.Green)}" : "")}.");
		}

		sb.AppendLine(
			$"You are {Gameworld.UnitManager.Describe(Body.Height, UnitType.Length, voyeur).Colour(Telnet.Green)} tall and weigh {Gameworld.UnitManager.DescribeMostSignificant(Body.Weight, UnitType.Mass, voyeur).Colour(Telnet.Green)}.");
		sb.AppendLine(
			$"You are {Handedness.DescribeAsHandedness().ToLowerInvariant().Colour(Telnet.Green)}{(Merits.OfType<IAmbidextrousMerit>().Any(x => x.Applies(this)) ? ", but are ambidextrous" : "")}.");
		sb.AppendLine(
			$"Your birthday is {Birthday.Calendar.DisplayDate(Birthday, CalendarDisplayMode.Long).Colour(Telnet.Green)}.");

		var status = NeedsModel.Status;
		sb.AppendLineFormat("You are {0}, {1} and {2}.",
			(status & NeedsResult.HungerOnly).Describe().ToLowerInvariant().Colour(Telnet.Green),
			(status & NeedsResult.ThirstOnly).Describe().ToLowerInvariant().Colour(Telnet.Green),
			(status & NeedsResult.DrunkOnly).Describe().ToLowerInvariant().Colour(Telnet.Green)
		);

		sb
			.AppendLine(CurrentLanguage != null && CurrentAccent != null
				? $"You are speaking in {CurrentLanguage.Name.Proper().Colour(Telnet.Green)} {CurrentAccent.AccentSuffix.Colour(Telnet.Green)}."
				: $"You are not speaking any languages."
			)
			.AppendLine(IsLiterate && CurrentScript != null && CurrentWritingLanguage != null
				? $"When you write, you use the {CurrentScript.Name.Colour(Telnet.Green)} script and the {CurrentWritingLanguage.Name.Colour(Telnet.Green)} language."
				: "You don't have any writing preferences set up.");


		sb.AppendLine(
			$"You {(from state in Body.CurrentSpeeds.Keys where Body.Speeds.Count(x => x.Position == state) > 1 select Body.CurrentSpeeds[state].FirstPersonVerb.Colour(Telnet.Green) + " if you are " + state.DefaultDescription()).ListToString()}.");

		sb.AppendLine(
			$"You use the {Currency?.Name.Proper().Colour(Telnet.Green) ?? "Unknown"} currency in any economic transactions.");

		if (CurrentName != PersonalName)
		{
			sb.AppendLine(
				$"You are going by the alias {CurrentName.GetName(NameStyle.FullName).Colour(Telnet.Green)}{(!CurrentName.GetName(NameStyle.SimpleFull).Equals(CurrentName.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + CurrentName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green) : "")}.");
		}

		sb.AppendLine(TotalMinutesPlayed < 1
			? "You haven't even been playing this character for a single minute."
			: CommonStringUtilities.CultureFormat(
				$"You have played this character for a total of {TimeSpan.FromMinutes(TotalMinutesPlayed).Describe().Colour(Telnet.Green)}.",
				Account));

		sb.AppendLine(
			$"You can carry {Gameworld.UnitManager.DescribeMostSignificant(Race.GetMaximumLiftWeight(this), UnitType.Mass, voyeur).Colour(Telnet.Green)}, drag {Gameworld.UnitManager.DescribeMostSignificant(Race.GetMaximumDragWeight(this), UnitType.Mass, voyeur).Colour(Telnet.Green)} and are {Body.Encumbrance.DescribeColoured()}.");
		sb.AppendLine(Body.ExternalItems.Any()
			? $"You are carrying {Gameworld.UnitManager.DescribeMostSignificant(Body.CarriedItems.Sum(x => x.Weight), UnitType.Mass, voyeur).Colour(Telnet.Green)}" +
			  $"{(Body.HeldOrWieldedItems.Any() ? $", and holding {Gameworld.UnitManager.DescribeMostSignificant(Body.HeldOrWieldedItems.Sum(x => x.Weight), UnitType.Mass, voyeur).Colour(Telnet.Green)} of that in your {Body.WielderDescriptionPlural.ToLowerInvariant()}" : "")}."
			: "You are not carrying anything at all.");

		var stateDesc = "";
		if (State.HasFlag(CharacterState.Unconscious))
		{
			stateDesc = "unconscious, ";
		}
		else if (State.HasFlag(CharacterState.Sleeping))
		{
			stateDesc = "asleep, ";
		}
		else if (EffectsOfType<HideInvis>().Any())
		{
			stateDesc = "hiding, ";
		}

		sb.AppendLine(
			$"You are {stateDesc}{PositionState.Describe(voyeur, PositionTarget, PositionModifier, PositionEmote, false).Fullstop()}");

		if (Capabilities.Any())
		{
			sb.AppendLine(
				$"You are {Capabilities.Select(x => x.Name.A_An(false, Telnet.BoldMagenta)).ListToString()}.");
		}

		foreach (var effect in CombinedEffectsOfType<IScoreAddendumEffect>().Where(x => x.ShowInScore))
		{
			sb.AppendLine(effect.ScoreAddendum);
		}

		var guardCharacter = EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
		if (guardCharacter != null)
		{
			sb.AppendLine($"You are guarding {guardCharacter.Targets.Select(x => x.HowSeen(this)).ListToString()}.");
		}

		var guardItem = EffectsOfType<IGuardItemEffect>().FirstOrDefault();
		if (guardItem != null)
		{
			sb.AppendLine(
				$"You are guarding {guardItem.TargetItem.HowSeen(this)}{(guardItem.IncludeVicinity ? " and everything in its vicinity" : "")}.");
		}

		var bondEffects = EffectsOfType<GoodBehaviourBond>().Select(x => x.Authority).Distinct().ToList();
		if (bondEffects.Any())
		{
			sb.AppendLine($"You are on a {"Good Behaviour Bond".Colour(Telnet.BoldCyan)} in the {bondEffects.Select(x => x.Name).ListToColouredString()} {"jurisdiction".Pluralise(bondEffects.Count != 1)}.");
		}

		var fines = Gameworld.LegalAuthorities.Select(x => (Legal: x, Owed: x.FinesOwed(this))).Where(x => x.Owed.Fine > 0.0M).ToList();
		foreach (var (authority, (fine,date)) in fines)
		{
			sb.AppendLine($"You owe fines of {authority.Currency.Describe(fine, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} due by {date.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()} in the {authority.Name.ColourName()} jurisdiction.");
		}

		if (ClanMemberships.Any())
		{
			sb.AppendLine();
			sb.AppendLine("You are a member of the following clans:");
			sb.AppendLine();
			foreach (var clan in ClanMemberships)
			{
				sb.AppendLine(
					$"\tYou are {clan.Rank.Title(this).TitleCase().A_An(colour: Telnet.Green)} of {clan.Clan.FullName.TitleCase().Colour(Telnet.Green)}{(clan.Appointments.Any() ? $", and are {clan.Appointments.Select(x => x.Title(this).Colour(Telnet.Green)).ListToString()}" : "")}.");
			}
		}

		return sb.ToString();
	}

	#region IEquatable<ICharacter> Members

	public bool Equals(ICharacter other)
	{
		return other?.Id == Id;
	}

	#endregion

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Character", id => gameworld.Characters.Get(id));
	}

	public static void InitialiseCharacterClass(IFuturemud gameworld)
	{
		var stringValue = gameworld.GetStaticConfiguration("MaximumNumberOfAliasProg");
		if (stringValue != null && long.TryParse(stringValue, out var value))
		{
			MaximumNumberOfAliasesProg = gameworld.FutureProgs.Get(value);
		}

		stringValue = gameworld.GetStaticConfiguration("TotalBloodVolumeProg");
		if (stringValue != null && long.TryParse(stringValue, out value))
		{
			TotalBloodVolumeProg = gameworld.FutureProgs.Get(value);
		}

		stringValue = gameworld.GetStaticConfiguration("LiverFunctionProg");
		if (stringValue != null && long.TryParse(stringValue, out value))
		{
			LiverFunctionProg = gameworld.FutureProgs.Get(value);
		}

		stringValue = gameworld.GetStaticConfiguration("MaximumStaminaProg");
		if (stringValue != null && long.TryParse(stringValue, out value))
		{
			MaximumStaminaProg = gameworld.FutureProgs.Get(value);
		}

		stringValue = gameworld.GetStaticConfiguration("WoundPenaltyToMoveSpeedPenalty");
		if (stringValue != null && double.TryParse(stringValue, out var dvalue))
		{
			WoundPenaltyToMoveSpeedPenalty = dvalue;
		}
		else
		{
			WoundPenaltyToMoveSpeedPenalty = 10.0;
		}

		stringValue = gameworld.GetStaticConfiguration("BaseSpeedExpression");
		BaseSpeedExpression = stringValue != null
			? new TraitExpression(stringValue, gameworld)
			: new TraitExpression("4000", gameworld);
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		var item = (MudSharp.Models.Character)dbitem;
		_id = item.Id;
		foreach (var cm in ClanMemberships)
		{
			cm.MemberId = item.Id;
		}

		Body.SetIDFromDatabase(item.Body);
		foreach (var ck in CharacterKnowledges)
		{
			ck.SetId(item.CharacterKnowledges.First(x => x.KnowledgeId == ck.Knowledge.Id).Id);
		}

		Body.BaseLiverAlcoholRemovalKilogramsPerHour = LiverFunction(this);
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.Character
		{
			Location = base.Location?.Id ?? 1L,
			Gender = (short)Gender.Enum,
			CultureId = Culture.Id,
			CreationTime = DateTime.UtcNow,
			CurrencyId = Currency?.Id,
			Body = (MudSharp.Models.Body)Body.DatabaseInsert(),
			DominantHandAlignment = (int)Handedness,
			BirthdayDate = Birthday.GetDateString(),
			BirthdayCalendarId = Birthday.Calendar.Id,
			AccountId = Account?.Id != 0 ? Account?.Id : default,
			NameInfo = SaveNames().ToString(),
			Name = PersonalName.GetName(NameStyle.GivenOnly),
			NeedsModel = CharacterCreation.Chargen.NeedsModelProg != null
				? (string)CharacterCreation.Chargen.NeedsModelProg.Execute(this)
				: "NoNeeds",
			PositionId = (int)PositionStanding.Instance.Id,
			PositionModifier = (int)PositionModifier.None,
			EffectData = SaveEffects().ToString(),
			CurrentCombatSettingId = CombatSettings.Id
		};

		foreach (var hook in _installedHooks)
		{
			var dbhook = new HooksPerceivable
			{
				HookId = hook.Id,
				Character = dbitem
			};
			FMDB.Context.HooksPerceivables.Add(dbhook);
		}

		foreach (var merit in _merits)
		{
			var dbmerit = new PerceiverMerit
			{
				MeritId = merit.Id,
				Character = dbitem
			};
			FMDB.Context.PerceiverMerits.Add(dbmerit);
		}

		foreach (var role in Roles.Distinct())
		{
			dbitem.CharactersChargenRoles.Add(
				new CharactersChargenRoles { ChargenRoleId = role.Id, Character = dbitem });
		}

		foreach (var clan in ClanMemberships)
		{
			var dbclan = new Models.ClanMembership
			{
				Character = dbitem,
				ClanId = clan.Clan.Id,
				RankId = clan.Rank.Id,
				PaygradeId = clan.Paygrade?.Id,
				PersonalName = CurrentName.SaveToXml().ToString(),
				JoinDate = clan.Clan.Calendar.CurrentDate.GetDateString()
			};
			foreach (var appointment in clan.Appointments)
			{
				dbclan.ClanMembershipsAppointments.Add(new ClanMembershipsAppointments
					{ AppointmentId = appointment.Id, ClanMembership = dbclan });
			}

			dbitem.ClanMembershipsCharacter.Add(dbclan);
		}

		foreach (var knowledge in _characterKnowledges)
		{
			var dbknow = new Models.CharacterKnowledge
			{
				Character = dbitem,
				KnowledgeId = knowledge.Knowledge.Id,
				WhenAcquired = DateTime.UtcNow,
				TimesTaught = 0,
				HowAcquired = "Chargen"
			};
			FMDB.Context.CharacterKnowledges.Add(dbknow);
		}

		foreach (var accent in Accents)
		{
			var dbaccent = new CharacterAccent
			{
				Character = dbitem,
				AccentId = accent.Id,
				Familiarity = (int)_accents[accent],
				IsPreferred = _preferredAccents.ContainsValue(accent)
			};
			FMDB.Context.CharactersAccents.Add(dbaccent);
		}

		foreach (var language in Languages)
		{
			dbitem.CharactersLanguages.Add(new CharactersLanguages { Character = dbitem, LanguageId = language.Id });
		}

		foreach (var script in Scripts)
		{
			dbitem.CharactersScripts.Add(new CharactersScripts { Character = dbitem, ScriptId = script.Id });
		}

		dbitem.CurrentScriptId = CurrentScript?.Id;
		dbitem.CurrentWritingLanguageId = CurrentWritingLanguage?.Id;
		dbitem.CurrentLanguageId = CurrentLanguage?.Id;
		dbitem.CurrentAccentId = CurrentAccent?.Id;

		dbitem.CurrentProjectId = CurrentProject.Project?.Id;
		dbitem.CurrentProjectLabourId = CurrentProject.Labour?.Id;
		FMDB.Context.Characters.Add(dbitem);
		return dbitem;
	}

	private void SaveKnowledges(MudSharp.Models.Character dbchar)
	{
		foreach (var knowledge in _addedKnowledges)
		{
			if (knowledge.Id == 0)
			{
				//It may have been saved on the CharacterKnowledge.cs level, so no need to add again
				var dbitem = new Models.CharacterKnowledge
				{
					CharacterId = base.Id,
					KnowledgeId = knowledge.Knowledge.Id,
					WhenAcquired = knowledge.WhenAcquired,
					HowAcquired = knowledge.HowAcquired,
					TimesTaught = knowledge.TimesTaught
				};

				FMDB.Context.CharacterKnowledges.Add(dbitem);
			}
		}

		_addedKnowledges.Clear();

		foreach (var knowledge in _removedKnowledges)
		{
			var dbitem = FMDB.Context.CharacterKnowledges.Find(knowledge.Id);
			if (dbitem == null)
			{
				continue;
			}

			FMDB.Context.CharacterKnowledges.Remove(dbitem);
		}

		_removedKnowledges.Clear();
		FMDB.Context.SaveChanges();
		foreach (var item in dbchar.CharacterKnowledges)
		{
			_characterKnowledges.FirstOrDefault(x => x.Knowledge.Id == item.KnowledgeId).SetId(item.Id);
		}

		_knowledgesChanged = false;
	}


	private void LoadFromDatabase(MudSharp.Models.Character character)
	{
		_noSave = true;
		_name = character.Name;
		_id = character.Id;
		_account = character.Account != null
			? Gameworld.TryAccount(character.Account)
			: DummyAccount.Instance;
		PreviousLoginDateTime = character.LastLoginTime;
		LastLogoutDateTime = character.LastLogoutTime;

		_gender = Gendering.Get((Gender)character.Gender);
		_currency = character.CurrencyId.HasValue
			? Gameworld.Currencies.Get(character.CurrencyId.Value)
			: Gameworld.Currencies.FirstOrDefault();

		_state = (CharacterState)character.State;
		_state |= CharacterState.Stasis;
		_status = (CharacterStatus)character.Status;
		_noMercy = character.NoMercy;
		BriefRoomDescs = character.RoomBrief;
		BriefCombatMode = character.CombatBrief;
		LoadNames(character);

		_roomLayer = (RoomLayer)character.RoomLayer;
		Handedness = (Alignment)character.DominantHandAlignment;

		Culture = Gameworld.Cultures.Get(character.CultureId);
		Birthday = Gameworld.Calendars.Get(character.BirthdayCalendarId).GetDate(character.BirthdayDate);

		//TODO: If the Account no longer belongs to an Admin level Authority Group yet this character still has
		//      the IsAdminAvatar bit set to true in the DB, we should clear the bit just to be sure, even
		//      though they have no special powers in-game due to the logic below.
		PermissionLevel = character.IsAdminAvatar
			? Account.Authority.Level
			: Account.Authority.Level >= PermissionLevel.Guide
				? PermissionLevel.Guide
				: PermissionLevel.Player;

		_clanMemberships.AddRange(Gameworld.Clans.SelectMany(x => x.Memberships.Where(y => y.MemberId == Id)));

		Location = Gameworld.Cells.Get(character.Location) ?? Gameworld.Cells.First();
		_dubs = character.Dubs.Select(x => (IDub)new Dub(x, this, Gameworld)).ToList();


		foreach (var merit in character.PerceiverMerits)
		{
			_merits.Add(Gameworld.Merits.Get(merit.MeritId));
		}

		Body = new Body.Implementations.Body(character.Body, Gameworld, this);
		LoadPosition(character.PositionId, character.PositionModifier, character.PositionEmote,
			character.PositionTargetId, character.PositionTargetType);
		if (character.CharactersChargenRoles.Any())
		{
			_roles.AddRange(character.CharactersChargenRoles.Select(x => Gameworld.Roles.Get(x.ChargenRoleId))
									 .ToList());
		}

		NeedsModel = NeedsModelFactory.LoadNeedsModel(character, this);
		LongTermPlan = character.LongTermPlan;
		ShortTermPlan = character.ShortTermPlan;

		foreach (var knowledge in character.CharacterKnowledges.ToList())
		{
			_characterKnowledges.Add(new RPG.Knowledge.CharacterKnowledge(knowledge, this));
		}

		foreach (var language in character.CharactersLanguages)
		{
			_languages.Add(Gameworld.Languages.Get(language.LanguageId));
		}

		foreach (var accent in character.CharactersAccents)
		{
			var iAccent = Gameworld.Accents.Get(accent.AccentId);
			_accents.Add(iAccent, (Difficulty)accent.Familiarity);
			if (accent.IsPreferred)
			{
				_preferredAccents[iAccent.Language] = iAccent;
			}
		}

		foreach (var language in Gameworld.Languages.Where(
					 x => Traits.Any(y => y.Definition == x.LinkedTrait) && !_languages.Contains(x)))
		{
			_languages.Add(language);
			if (_accents.All(x => x.Key.Language != language))
			{
				_accents.Add(language.DefaultLearnerAccent, Difficulty.Automatic);
			}

			LanguagesChanged = true;
		}

		CurrentLanguage = character.CurrentLanguageId.HasValue
			? Gameworld.Languages.Get(character.CurrentLanguageId.Value)
			: _languages.FirstOrDefault();
		CurrentWritingLanguage = character.CurrentWritingLanguageId.HasValue
			? Gameworld.Languages.Get(character.CurrentWritingLanguageId.Value)
			: _languages.FirstOrDefault();


		CurrentAccent = Gameworld.Accents.Get(character.CurrentAccentId ?? 0) ??
						_preferredAccents.ValueOrDefault(CurrentLanguage, null) ??
						_accents.FirstOrDefault(x => x.Key.Language == CurrentLanguage).Key;

		foreach (var script in character.CharactersScripts)
		{
			_scripts.Add(Gameworld.Scripts.Get(script.ScriptId));
		}

		foreach (var script in Gameworld.Scripts.Where(
					 x => Knowledges.Any(y => x.ScriptKnowledge == y) && !_scripts.Contains(x)))
		{
			_scripts.Add(script);
			LanguagesChanged = true;
		}

		CurrentScript = character.CurrentScriptId.HasValue
			? Gameworld.Scripts.Get(character.CurrentScriptId.Value)
			: _scripts.FirstOrDefault();

		WritingStyle = (WritingStyleDescriptors)character.WritingStyle;
		foreach (var language in _languages.Where(x => _accents.All(y => y.Key.Language != x)))
		{
			_accents.Add(language.DefaultLearnerAccent, Difficulty.Trivial);
			if (CurrentLanguage == language)
			{
				CurrentAccent = CurrentLanguage.DefaultLearnerAccent;
			}
		}

		foreach (var ally in character.AlliesCharacter.ToList())
		{
			_allyIDs.Add(ally.AllyId);
			if (ally.Trusted)
			{
				_trustedAllyIDs.Add(ally.AllyId);
			}
		}

		LoadMagic(character);
		LoadOutfits(character);
		LoadProjects(character);
		Body.LoadInventory(character.Body);
		CombatSettings = Gameworld.CharacterCombatSettings.Get(character.CurrentCombatSettingId ?? 0) ??
						 Gameworld.CharacterCombatSettings.FirstOrDefault(
							 x => x.GlobalTemplate && x.AvailabilityProg?.Execute<bool?>(this) == true) ??
						 Gameworld.CharacterCombatSettings.FirstOrDefault(x => x.GlobalTemplate);
		_preferredDefenseType = (DefenseType)character.PreferredDefenseType;

		foreach (var item in character.ActiveJobs)
		{
			_activeJobs.Add(Gameworld.ActiveJobs.Get(item.Id));
		}

		_noSave = false;
	}

	private void LoadNames(MudSharp.Models.Character character)
	{
		var root = XElement.Parse(character.NameInfo);
		_personalName = new PersonalName(root.Element("PersonalName").Element("Name"), Gameworld);
		Aliases = new List<IPersonalName>();
		foreach (var element in root.Element("Aliases").Elements())
		{
			Aliases.Add(new PersonalName(element, Gameworld));
		}

		var current = int.Parse(root.Element("CurrentName").Value);
		if (current == 0)
		{
			_currentName = PersonalName;
		}
		else
		{
			_currentName = Aliases.ElementAt(current - 1);
		}
	}

	private XElement SaveNames()
	{
		return new XElement("Names",
			new XElement("PersonalName", PersonalName.SaveToXml()),
			new XElement("Aliases",
				from alias in Aliases select alias.SaveToXml()
			),
			new XElement("CurrentName",
				CurrentName == PersonalName ? 0 : Aliases.IndexOf(CurrentName) + 1
			)
		);
	}

	public override string ToString()
	{
		return $"Character ID {Id} Desc: {HowSeen(this, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}";
	}

	public IEnumerable<T> CombinedEffectsOfType<T>() where T : class, IEffect
	{
		return EffectHandler.EffectsOfType<T>().Concat(Body.EffectsOfType<T>());
	}

	public IEnumerable<IEffect> CombinedEffects()
	{
		return EffectHandler.Effects.Concat(Body.Effects);
	}

	public override bool IdentityIsObscured =>
		// TODO
		false;

	public override bool IdentityIsObscuredTo(ICharacter observer)
	{
		// TODO
		return false;
	}

	public bool TryToDetermineIdentity(ICharacter observer)
	{
		// TODO
		return false;
	}

	public bool NoMercy
	{
		get => _noMercy;
		set
		{
			_noMercy = value;
			Changed = true;
		}
	}

	#region Properties

	#region Static

	private static IFutureProg MaximumNumberOfAliasesProg;

	public static int MaximumNumberOfAliases(ICharacter character)
	{
		return MaximumNumberOfAliasesProg?.ExecuteInt(character) ?? 0;
	}

	private static IFutureProg TotalBloodVolumeProg;

	public static double TotalBloodVolume(ICharacter character)
	{
		return (double)(decimal)TotalBloodVolumeProg.Execute(character);
	}

	private static IFutureProg LiverFunctionProg;

	public static double LiverFunction(ICharacter character)
	{
		var amount = (double)(decimal)LiverFunctionProg.Execute(character);
		if (double.IsNaN(amount))
		{
			return 0.0;
		}

		return amount;
	}

	private static IFutureProg MaximumStaminaProg;

	public static double MaximumStaminaFor(ICharacter character)
	{
		return (double)(decimal)MaximumStaminaProg.Execute(character);
	}

	public static double WoundPenaltyToMoveSpeedPenalty { get; set; }

	public static TraitExpression BaseSpeedExpression { get; set; }

	#endregion Static

	#region Interface

	public override string FrameworkItemType => "Character";

	#region ITimeout

	public int Timeout => 28800000;

	#endregion

	#region IHaveAuthority

	public IAuthority Authority => Account.Authority;

	public PermissionLevel PermissionLevel { get; protected set; }

	#endregion

	#region IHaveABody

	public IBody Body { get; protected set; }

	#endregion

	#region ICollect

	public IEnumerable<IGameItem> Inventory => Body.ExternalItems;

	public IEnumerable<IGameItem> ContextualItems => Location.GameItems.Concat(Body.ExternalItems).ToList();

	public IEnumerable<IGameItem> DeepContextualItems
	{
		get
		{
			var items = new List<IGameItem>();
			items.AddRange(Location.LayerGameItems(RoomLayer));
			items.AddRange(Body.ExternalItems);

			foreach (var item in items.ToArray())
			{
				if (item.GetItemType<IContainer>() is IContainer ic)
				{
					foreach (var subitem in ic.Contents.Where(x => ic.CanTake(this, x, 0)))
					{
						items.Add(subitem);
					}
				}
			}

			return items;
		}
	}

	#endregion

	#region IAudible

	public Communication.Language.Language Language { get; private set; }

	#endregion

	#region IControllable

	protected IControllable _nextContext;
	protected IControllable _subContext;

	IControllable IControllable.SubContext => _subContext;

	IControllable IControllable.NextContext => _nextContext;

	#endregion

	#region ISentient

	public bool Think(string thought, IEmote emote = null)
	{
		OutputHandler.Send(new MixedEmoteOutput(new Emote("", this)).Append(emote));
		return false;
	}

	public bool Feel(string feeling)
	{
		if (feeling.Length > 400)
		{
			OutputHandler.Send("You can't possibly feel so much at once.");
			return false;
		}

		// TODO - reasons why you can't feel?
		var feelEmote = new PlayerEmote(feeling, this, permitSpeech: PermitLanguageOptions.IgnoreLanguage);
		if (!feelEmote.Valid)
		{
			OutputHandler.Send(feelEmote.ErrorMessage);
			return false;
		}

		var output = new MixedEmoteOutput(new Emote("@ feel|feels", this)).Append(feelEmote);
		OutputHandler.Send(output);
		return true;
	}

	#endregion

	#endregion

	#region Local

	#region Framework

	/// <summary>
	///     These tags such as (Editing) are set when this actor loses its controller
	/// </summary>
	private string _noControllerTags;

	public void SetNoControllerTags(string text)
	{
		_noControllerTags = text;
	}

	IController IControllable.Controller => Controller;

	public ICharacterController Controller { get; protected set; }

	public ICharacterController CharacterController => Controller;

	#endregion

	private Gendering _gender;
	private CharacterStatus _status;

	public CharacterStatus Status => _status;

	private CharacterState _state;

	public CharacterState State
	{
		get => _state;
		set
		{
			if (_state != value)
			{
				_state = value;
				OnStateChanged?.Invoke(this);
				EffectHandler.RemoveAllEffects(x => x.GetSubtype<IRemoveOnStateChange>()?.ShouldRemove(value) == true, true);
				Body.RemoveAllEffects(x => x.GetSubtype<IRemoveOnStateChange>()?.ShouldRemove(value) == true, true);
				Changed = true;
				if (_status == CharacterStatus.Deceased && State != CharacterState.Dead)
				{
					throw new ApplicationException("A dead character was set to a non dead status.");
				}

				CheckCanFly();
			}
		}
	}

	public bool HasAuthority { get; private set; }
	private IAccount _account;

	public override IAccount Account
	{
		get => _account;
		set => _account = value;
	}

	public override Gendering Gender => _gender;

	public ICharacterCommandTree CommandTree { get; protected set; }

	private int _dbTotalMinutesPlayed;
	public int TotalMinutesPlayed => _dbTotalMinutesPlayed + (int)(DateTime.UtcNow - LastMinutesUpdate).TotalMinutes;

	public ILanguageScrambler LanguageScrambler { get; protected set; }

	#endregion

	#endregion

	#region Methods

	#region Control

	public void AssumeControl(IController controller)
	{
		SilentAssumeControl(controller);
		Body.Look();
	}

	public void SilentAssumeControl(IController controller)
	{
		Controller = (ICharacterController)controller;
		Controller?.UpdateControlFocus(this);
		Register(Controller?.OutputHandler);
		_noControllerTags = "";
		_nextContext = null;
	}

	bool IControllable.ExecuteCommand(string command)
	{
		Gameworld?.LogManager.LogCharacterCommand(this, command);
		// Handle Command Interupt hooks first
		var cmd = new StringStack(command).Pop();
		var ss = new StringStack(command.RemoveFirstWord());
		if (!EffectHandler.AffectedBy<IIgnoreCommandHooksEffect>())
		{
			if (HandleEvent(EventType.SelfCommandInput, this, cmd, ss))
			{
				return true;
			}

			if (Body.ExternalItems.Any(x => x.HandleEvent(EventType.CommandInput, this, x, cmd, ss)))
			{
				return true;
			}

			if (
				Location?.LayerCharacters(RoomLayer).Except(this)
						.Any(x => x.HandleEvent(EventType.CommandInput, this, x, cmd, ss)) ??
				false)
			{
				return true;
			}

			if (Location?.LayerGameItems(RoomLayer).Any(x => x.HandleEvent(EventType.CommandInput, this, x, cmd, ss)) ??
				false)
			{
				return true;
			}

			if (Location?.HandleEvent(EventType.CommandInput, this, Location, cmd, ss) ?? false)
			{
				return true;
			}
		}

		// Special overrides for non-cardinal cell exits
		if (!ss.IsFinished)
		{
			var nonCardinalExit = Location.GetExit(cmd, ss.PeekSpeech(), this);
			if (nonCardinalExit != null && CanSee(Location, nonCardinalExit))
			{
				var commandName = "north";
				return CommandTree.Commands.Execute(this,
					Commands.Modules.MovementModule.Instance.Commands.LocateCommand(this, ref commandName), command,
					State, PermissionLevel, OutputHandler);
			}
		}

		return CommandTree.Commands.Execute(this, command, State, PermissionLevel, OutputHandler);
	}

	public void EditorModeMulti(Action<IEnumerable<string>, IOutputHandler, object[]> postAction, Action<IOutputHandler, object[]> cancelAction, IEnumerable<string> editorTexts, double characterLengthMultiplier = 1.0, string recallText = null, EditorOptions options = EditorOptions.None, object[] suppliedArguments = null)
	{
		_nextContext = new MultiEditorController(this, suppliedArguments, postAction, cancelAction, options, editorTexts,
			characterLengthMultiplier, recallText ?? EffectsOfType<StoredEditorText>().FirstOrDefault()?.Text);
		_noControllerTags = " (editing)".Colour(Telnet.Red);
		var newHandler = new NonPlayerOutputHandler();
		OutputHandler.Register(null);
		Register(newHandler);
	}

	public void EditorMode(Action<string, IOutputHandler, object[]> postAction,
		Action<IOutputHandler, object[]> cancelAction, double characterLengthMultiplier, string recallText = null,
		EditorOptions options = EditorOptions.None, object[] suppliedArguments = null)
	{
		OutputHandler.Send(
			"You are now entering an editor, use @ on a blank line to exit and *help to see help.".Colour(
				Telnet.Yellow));
		OutputHandler.Send(CommonStringUtilities.GetWidthRuler(Account.LineFormatLength).Colour(Telnet.Yellow));
		Gameworld.ForceOutgoingMessages();
		_nextContext = new EditorController(this, suppliedArguments, postAction, cancelAction, options,
			characterLengthMultiplier, recallText ?? EffectsOfType<StoredEditorText>().FirstOrDefault()?.Text);
		_noControllerTags = " (editing)".Colour(Telnet.Red);
		var newHandler = new NonPlayerOutputHandler();
		OutputHandler.Register(null);
		Register(newHandler);
	}

	public void LoseControl(IController controller)
	{
		if (OutputHandler == Controller.OutputHandler)
		{
			var newHandler = new NonPlayerOutputHandler();
			OutputHandler.Register(null);
			Register(newHandler);
		}

		Controller.UpdateControlFocus(null);
		Controller = null;
	}

	public bool HandleSubContext(string command)
	{
		if (_subContext?.HandleSubContext(command) == true)
		{
			if (_subContext.NextContext != null)
			{
				_subContext.LoseControl(this);
				_subContext = _subContext.NextContext;
				_subContext.AssumeControl(this);
			}

			return true;
		}

		return false;
	}

	public void SetContext(IControllable context)
	{
		// Do nothing
	}

	public virtual void OutOfContextExecuteCommand(string command)
	{
		if (Controller != null)
		{
			Controller.HandleCommand(command);
		}
		else
		{
			((IControllable)this).ExecuteCommand(command);
		}
	}

	public void CloseSubContext()
	{
		_subContext.LoseControl(this);
		_subContext = _subContext.NextContext;
	}

	public void Close()
	{
		Quit();
	}

	#endregion

	#region Out of Character

	public bool Stop(bool force)
	{
		// Movement
		if (Movement != null)
		{
			if (!Movement.IsMovementLeader(this) && !force)
			{
				OutputHandler.Send("You are not the party leader, and so cannot stop the group.");
				return false;
			}

			if (Movement.Phase == MovementPhase.NewRoom && !QueuedMoveCommands.Any() && !force)
			{
				OutputHandler.Send("You cannot stop moving now, you must wait until you are done.");
				return false;
			}

			if (!Movement.CanBeVoluntarilyCancelled && !force)
			{
				OutputHandler.Send("You cannot voluntarily stop moving at this time.");
				return false;
			}

			if (Movement.Phase == MovementPhase.NewRoom)
			{
				QueuedMoveCommands.Clear();
				OutputHandler.Send("You clear all your pending movement commands.");
				return false;
			}

			Movement.StopMovement();
			QueuedMoveCommands.Clear();
			return true;
		}

		// Effects
		var stoppingEffects =
			Effects.Where(x => x.IsBlockingEffect(string.Empty) && x.CanBeStoppedByPlayer).ToList();
		if (!stoppingEffects.Any() && !force)
		{
			// Combat Truces
			if (Combat != null)
			{
				Combat.TruceRequested(this);
				return true;
			}

			OutputHandler.Send("You aren't doing anything that requires you to stop or that you can stop.");
			return false;
		}

		var stoppedList = new List<IEffect>();
		foreach (var effect in stoppingEffects)
		{
			stoppedList.Add(effect);
			RemoveEffect(effect);
			effect.CancelEffect();
		}

		if (stoppingEffects.Any())
		{
			OutputHandler.Handle(new SuppliedFunctionOutput(new Emote("@ stop|stops <EFFECTS>.", this),
				(text, voyeur) =>
				{
					return text.Replace(
						"<EFFECTS>",
						stoppedList.Select(
							x => x.BlockingDescription(
								string.Empty, voyeur)).ListToString());
				},
				flags: OutputFlags.SuppressObscured));
			return true;
		}

		return false;
	}

	protected override void ReleaseEvents()
	{
		base.ReleaseEvents();
		OnDeath = null;
		OnMoved = null;
		OnStartMove = null;
		OnStateChanged = null;
		OnStopMove = null;
		OnWantsToMove = null;
	}

	public virtual bool Quit(bool silent = false)
	{
		EffectsChanged = true;
		PerceivableQuit();
		Movement?.CancelForMoverOnly(this);
		LeaveParty(true);
		CeaseFollowing();
		Combat?.LeaveCombat(this);

		if (Location != null)
		{
			if (!silent)
			{
				OutputHandler?.Handle(new EmoteOutput(new Emote("@ leaves the area.", this),
					flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
			}

			foreach (var bodyguard in Location.LayerCharacters(RoomLayer).OfType<INPC>()
											  .Where(x => x.BodyguardingCharacterID == Id)
											  .ToList())
			{
				bodyguard.Quit();
			}

			Location.Leave(this);
		}

		if (IsPlayerCharacter && !State.HasFlag(CharacterState.Dead) && !State.HasFlag(CharacterState.Stasis))
		{
			using (new FMDB())
			{
				var dbchar = FMDB.Context.Characters.Find(Id);
				dbchar.LastLogoutTime = DateTime.UtcNow;
				SaveMinutes(dbchar);
				FMDB.Context.SaveChanges();
			}

			LastLogoutDateTime = DateTime.UtcNow;
		}

		InvalidatePositionTargets();
		SoftReleasePositionTarget();
		CacheScheduledEffects();
		Gameworld.EffectScheduler.Destroy(this, true);
		Gameworld.Scheduler.Destroy(this);
		Body.Quit();

		Gameworld.HeartbeatManager.TenSecondHeartbeat -= NeedsHeartbeat;
		Gameworld.Destroy(this);
		OutputHandler?.Register(null);
		if (Controller != null)
		{
			_nextContext = new LoggedInMenu(Account, Gameworld);
		}

		if (State != CharacterState.Dead)
		{
			State |= CharacterState.Stasis;
		}

		return true;
	}

	public void LoginCharacter()
	{
		ScheduleCachedEffects();
		LoginDateTime = DateTime.UtcNow;
		OutputHandler.Handle(new EmoteOutput(new Emote("@ has entered the area.", this),
			flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		HandleEvent(EventType.CharacterEntersGame, this);
		StartNeedsHeartbeat();
		RemoveAllEffects(x => x.IsEffectType<LinkdeadLogout>());
		Body.Login();
		if (PositionTarget?.TargetedBy.Contains(this) == false)
		{
			if (!PositionTarget.CanBePositionedAgainst(PositionState, PositionModifier))
			{
				SetTarget(null);
				SetModifier(PositionModifier.None);
			}
			else
			{
				SetTarget(PositionTarget);
			}
		}

		if (Gameworld.CachedBodyguards.ContainsKey(Id))
		{
			foreach (var bodyguard in Gameworld.CachedBodyguards[Id])
			{
				Gameworld.Add(bodyguard, true);
				Location.Login(bodyguard);
			}

			Gameworld.CachedBodyguards[Id].Clear();
		}

		if (IsPlayerCharacter)
		{
			var elections = ClanMemberships
							.Where(x => x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders))
							.SelectMany(x => x.Clan.Appointments.Where(y => y.IsAppointedByElection)
											  .SelectMany(y => y.Elections))
							.Where(x =>
								(x.ElectionStage == ElectionStage.Nomination && x.Appointment.CanNominate(this).Truth &&
								 !x.Nominees.Any(y => y.MemberId == Id)) ||
								(x.ElectionStage == ElectionStage.Voting && x.Appointment.NumberOfVotes(this) > 0 &&
								 !x.Votes.Any(y => y.Voter.MemberId == Id))
							)
							.ToList();
			if (elections.Any())
			{
				OutputHandler.Send(
					"\nThere are elections for which you can either nominate or vote. See CLAN ELECTIONS for more information.\n"
						.Colour(Telnet.BoldCyan));
			}
		}
	}

	public virtual bool IsPlayerCharacter => true;

	#endregion

	#region In Character

	#region Emote

	public void Emote(string emote)
	{
		Body.Emote(emote);
	}

	#endregion

	#region Description

	public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (type == DescriptionType.Long)
		{
			return Body.HowSeen(voyeur, proper, DescriptionType.Long, colour, flags) +
				   (Controller != null ? Controller.LDescAdditionalTags : _noControllerTags);
		}

		return Body.HowSeen(voyeur, proper, type, colour, flags);
	}

	public override IEnumerable<string> Keywords => Body.Keywords;

	#endregion

	public override Gendering ApparentGender(IPerceiver voyeur)
	{
		return Body.ApparentGender(voyeur);
	}

	public void SetGender(Gender gender)
	{
		_gender = Gendering.Get(gender);
		Body.GenderChanged();
		Changed = true;
	}

	#region GameItem Manipulation and Display

	public void DisplayInventory()
	{
		Body.DisplayInventory(Body, true);
	}

	#endregion

	#endregion

	#endregion

	#region ICharacter Members

	public virtual bool IsGuest => false;

	public void SaveMinutes(MudSharp.Models.Character dbchar)
	{
		if (State.HasFlag(CharacterState.Dead) || State.HasFlag(CharacterState.Stasis))
		{
			return;
		}

		var now = DateTime.UtcNow;
		if (dbchar == null)
		{
			dbchar = FMDB.Context.Characters.Find(Id);
		}

		var oldMinutes = dbchar.TotalMinutesPlayed;
		dbchar.TotalMinutesPlayed += (int)(now - LastMinutesUpdate).TotalMinutes;
		_dbTotalMinutesPlayed = dbchar.TotalMinutesPlayed;
		LastMinutesUpdate = now;
		foreach (var resource in Gameworld.ChargenResources)
		{
			resource.UpdateOnSave(this, oldMinutes, dbchar.TotalMinutesPlayed);
		}
	}

	public string DebugInfo()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Debug Info for Character {Id} - {HowSeen(this, flags: PerceiveIgnoreFlags.IgnoreSelf)}");
		sb.AppendLine();
		sb.AppendLine($"State: {State}");
		sb.AppendLine($"Status: {Status}");
		sb.AppendLine($"Changed: {Changed}");
		sb.AppendLine($"AlliesChanged: {AlliesChanged}");
		sb.AppendLine($"LanguagesChanged: {LanguagesChanged}");
		sb.AppendLine($"KnowledgesChanged: {KnowledgesChanged}");
		sb.AppendLine($"MeritsChanged: {MeritsChanged}");
		sb.AppendLine();
		sb.AppendLine(Body.DebugInfo());
		return sb.ToString();
	}

	public (bool Truth, string Message) IsBlocked(params string[] blocks)
	{
		foreach (var effect in CombinedEffects())
		foreach (var block in blocks)
		{
			if (effect.IsBlockingEffect(block))
			{
				return (true, effect.BlockingDescription(block, this));
			}
		}

		return (false, string.Empty);
	}

	public bool IsHelpless
	{
		get
		{
			// TODO - more reasons for being helpless, like being restrained
			return State.HasFlag(CharacterState.Paralysed) ||
				   State.HasFlag(CharacterState.Unconscious) ||
				   State.HasFlag(CharacterState.Sleeping) ||
				   State.HasFlag(CharacterState.Dead) ||
				   EffectsOfType<Dragging.DragTarget>().Any() ||
				   CombinedEffectsOfType<BeingGrappled>().Any(x => x.UnderControl) ||
				   Body.Limbs.All(x =>
					   EffectsOfType<ILimbIneffectiveEffect>().Any(y => y.Applies(x) && y.AppliesToLimb(x))) ||
				   CombinedEffectsOfType<IHelplessEffect>().Any(x => x.Applies())
				;
		}
	}

	private MudDate _birthday;

	public MudDate Birthday
	{
		get => _birthday;
		set
		{
			_birthday = value;
			Changed = true;
		}
	}

	public void MovePosition(IPositionState whichPosition, PositionModifier whichModifier, IPositionable target,
		string unparsedEmote)
	{
		throw new NotSupportedException();
	}

	private string _longTermPlan;

	public string LongTermPlan
	{
		get => _longTermPlan;
		set
		{
			_longTermPlan = value;
			Changed = true;
		}
	}

	private string _shortTermPlan;

	public string ShortTermPlan
	{
		get => _shortTermPlan;
		set
		{
			_shortTermPlan = value;
			Changed = true;
		}
	}

	public bool WillingToPermitInventoryManipulation(ICharacter manipulator)
	{
		return manipulator.IsAdministrator() ||
			   manipulator == this ||
			   IsHelpless ||
			   State.HasFlag(CharacterState.Sleeping) ||
			   State.HasFlag(CharacterState.Unconscious) ||
			   State.HasFlag(CharacterState.Paralysed) ||
			   State.HasFlag(CharacterState.Dead) ||
			   IsTrustedAlly(manipulator) ||
			   CombinedEffectsOfType<BeDressedEffect>().Any(x => x.Dresser == manipulator)
			;
	}

	public bool WillingToPermitMedicalIntervention(ICharacter medic)
	{
		return
			medic == this ||
			medic.IsAdministrator() ||
			IsHelpless ||
			State.HasFlag(CharacterState.Sleeping) ||
			State.HasFlag(CharacterState.Unconscious) ||
			State.HasFlag(CharacterState.Paralysed) ||
			State.HasFlag(CharacterState.Dead) ||
			IsTrustedAlly(medic)
			;
	}

	public bool UnableToResistInterventions(ICharacter intervenor)
	{
		return intervenor.IsAdministrator() ||
			   IsHelpless ||
			   State.HasFlag(CharacterState.Unconscious) ||
			   State.HasFlag(CharacterState.Paralysed) ||
			   State.HasFlag(CharacterState.Dead);
	}

	public (bool Truth, string Message) CanManipulateItem(IGameItem item)
	{
		if (item.InInventoryOf != null && item.InInventoryOf != Body)
		{
			if (!item.InInventoryOf.Actor.WillingToPermitInventoryManipulation(this))
			{
				return (false,
					new QuickEmote("$0 &0|are|is not willing to permit you to manipulate things in &0's possession.",
						this, item.InInventoryOf.Actor));
			}

			return (true, string.Empty);
		}

		if (item.ContainedIn != null && !Location.CanGetAccess(item.ContainedIn, this))
		{
			return (false, Location.WhyCannotGetAccess(item.ContainedIn, this));
		}

		if (!Location.CanGetAccess(item, this))
		{
			return (false, Location.WhyCannotGetAccess(item, this));
		}

		return (true, string.Empty);
	}

	public ICharacterTemplate GetCharacterTemplate()
	{
		var accents = _accents.Where(x => x.Value <= Difficulty.Trivial).Select(x => x.Key).Distinct().ToList();
		foreach (var language in Languages)
		{
			if (accents.All(x => x.Language != language))
			{
				accents.Add(_accents.Where(x => x.Key.Language == language).FirstMin(x => x.Value).Key ??
							language.DefaultLearnerAccent);
			}
		}

		return new SimpleCharacterTemplate
		{
			Handedness = Handedness,
			MissingBodyparts = Body.SeveredRoots.ToList(),
			SelectedAccents = accents,
			SelectedKnowledges = Knowledges.ToList(),
			SelectedCharacteristics = Body.CharacteristicDefinitions
										  .Select(x => (x, Body.GetCharacteristic(x, this))).ToList(),
			SelectedMerits = Merits.OfType<ICharacterMerit>().ToList(),
			SelectedRoles = Roles.ToList(),
			SelectedWeight = Weight,
			SelectedHeight = Height,
			SelectedName = PersonalName,
			SelectedCulture = Culture,
			SelectedEthnicity = Ethnicity,
			SelectedRace = Race,
			SelectedBirthday = Birthday,
			SelectedEntityDescriptionPatterns = Body.EntityDescriptionPatterns.ToList(),
			SelectedFullDesc = Body.GetRawDescriptions.FullDescription,
			SelectedSdesc = Body.GetRawDescriptions.ShortDescription,
			SelectedGender = Gender.Enum,
			SkillValues = (from skill in Traits.OfType<ISkill>() select (skill.Definition, skill.RawValue))
				.ToList(),
			SelectedAttributes =
				(from attribute in Traits.OfType<IAttribute>()
				 select TraitFactory.LoadAttribute(attribute.AttributeDefinition, Body, attribute.RawValue))
				.ToList<ITrait>(),
			Gameworld = Gameworld
		};
	}

	#endregion

	#region IHaveCurrency

	private ICurrency _currency;

	public ICurrency Currency
	{
		get => _currency;
		set
		{
			_currency = value;
			Changed = true;
		}
	}

	#endregion

	#region IHaveRace

	public IRace Race => Body.Race;

	public IEthnicity Ethnicity => Body.Ethnicity;

	public IEnumerable<INameCulture> NameCultures
	{
		get
		{
			foreach (var gender in Enum.GetValues<Gender>())
			{
				yield return Ethnicity.NameCultureForGender(gender) ?? Culture.NameCultureForGender(gender) ??  Gameworld.NameCultures.First();
			}
		}
	}

	public INameCulture NameCultureForGender(Gender gender)
	{
		return Ethnicity.NameCultureForGender(gender) ?? Culture.NameCultureForGender(gender) ?? Gameworld.NameCultures.First();
	}

	#endregion

	#region IHaveABody Members

	public double Weight
	{
		get => Body.Weight;
		set => Body.Weight = value;
	}

	public double Height
	{
		get => Body.Height;
		set => Body.Height = value;
	}

	#endregion

	#region IControllable Members

	bool IControllable.HasPrompt => true;

	string IControllable.Prompt
	{
		get
		{
			if (Account.PromptType == PromptType.Default)
				//Convert people over from default
			{
				Account.PromptType = PromptType.Full | PromptType.PositionInfo;
			}

			if (Account.PromptType.HasFlag(PromptType.Brief))
			{
				return "\n>\n\n";
			}

			if (Account.PromptType.HasFlag(PromptType.Classic))
			{
				var classicStealthString = "";
				if (EffectsOfType<IHideEffect>().Any())
				{
					classicStealthString += "Hi";
				}

				if (EffectsOfType<ISneakEffect>().Any())
				{
					classicStealthString += (classicStealthString.Length > 0 ? " " : "") + "Sn";
				}

				if (EffectsOfType<IAdminInvisEffect>().Any())
				{
					classicStealthString += (classicStealthString.Length > 0 ? " " : "") + "Wi";
				}

				var classic = HealthStrategy.ReportConditionPrompt(this, PromptType.Classic);
				if (Account.PromptType.HasFlag(PromptType.IncludeMagic) &&
					Capabilities.Any(x => x.ShowMagicResourcesInPrompt))
				{
					foreach (var resource in MagicResourceAmounts)
					{
						var current = resource.Value / resource.Key.ResourceCap(this);
						if (double.IsInfinity(current))
						{
							current = 1.0;
						}

						classic = classic.Append($" / {resource.Key.ShortName}: {resource.Key.ClassicPromptString(current)}");
					}
				}

				if (Account.PromptType.HasFlag(PromptType.SpeakInfo))
				{
					classic = classic.Append($" / Speaking: {CurrentLanguage.Name.Colour(Telnet.Green)}");
				}

				if (Account.PromptType.HasFlag(PromptType.StealthInfo) && classicStealthString.Length > 0)
				{
					classic = classic.Append(" / " + classicStealthString);
				}

				return "\n<" + classic + ">\n\n";
			}

			var stealthString = "";
			if (EffectsOfType<IHideEffect>().Any())
			{
				stealthString = "Hiding".Colour(Telnet.Magenta);
			}

			if (EffectsOfType<ISneakEffect>().Any())
			{
				stealthString += (stealthString.Length > 0 ? " & " : "") + "Sneaking".Colour(Telnet.Magenta);
			}

			if (EffectsOfType<IAdminInvisEffect>().Any())
			{
				stealthString += (stealthString.Length > 0 ? " & " : "") + "WizInvis".Colour(Telnet.BoldMagenta);
			}

			var staminaRatio = CurrentStamina / MaximumStamina;
			ANSIColour staminaColour;
			var boldColour = false;
			if (staminaRatio >= 1.0)
			{
				staminaColour = Telnet.Green;
			}
			else if (staminaRatio > 0.8)
			{
				staminaColour = Telnet.Green;
				boldColour = true;
			}
			else if (staminaRatio > 0.4)
			{
				staminaColour = Telnet.Yellow;
				boldColour = true;
			}
			else if (staminaRatio > 0.2)
			{
				staminaColour = Telnet.Red;
				boldColour = true;
			}
			else
			{
				staminaColour = Telnet.Red;
			}

			var currentstaminaString = CurrentStamina.ToString("N0", this);
			var maxstaminaString = MaximumStamina.ToString("N0", this);

			if (Account.PromptType.HasFlag(PromptType.Full))
			{
				var fpsb = new StringBuilder();
				fpsb.Append("\n");
				fpsb.Append(HealthStrategy.ReportConditionPrompt(this, PromptType.Full));
				fpsb.Append("\n<Stamina: ");
				fpsb.Append(boldColour ? staminaColour.Bold : staminaColour.Colour);
				fpsb.Append(currentstaminaString);
				fpsb.Append("/");
				fpsb.Append(maxstaminaString);
				fpsb.Append(Telnet.RESETALL);
				fpsb.Append(" | Exertion: ");
				fpsb.Append((CurrentExertion > LongtermExertion ? CurrentExertion.Describe() : LongtermExertion.Describe())
					.Colour(Telnet.Green));
				fpsb.Append(" | Status: ");
				fpsb.Append(State.DescribeColour());

				if (NeedsToBreathe && !CanBreathe)
				{
					fpsb.Append(" | Breath: ");
					fpsb.Append(HeldBreathPercentage.ToString("P0", this).ColourValue());
				}

				if (Account.PromptType.HasFlag(PromptType.IncludeMagic) &&
				    Capabilities.Any(x => x.ShowMagicResourcesInPrompt))
				{
					foreach (var amount in MagicResourceAmounts)
					{
						var cap = amount.Key.ResourceCap(this);
						if (cap == 0)
						{
							continue;
						}
						fpsb.Append(" | ");
						fpsb.Append(amount.Key.ShortName);
						fpsb.Append(": ");
						fpsb.Append((amount.Value / cap).ToString("P0", this));
					}
				}

				if (Account.PromptType.HasFlag(PromptType.PositionInfo) ||
				    (Account.PromptType.HasFlag(PromptType.SpeakInfo) && CurrentLanguage is not null && CurrentAccent is not null) ||
				    (Account.PromptType.HasFlag(PromptType.StealthInfo) && !string.IsNullOrEmpty(stealthString))
				   )
				{
					fpsb.Append(">\n<");
					var included = false;
					if (Account.PromptType.HasFlag(PromptType.SpeakInfo) && CurrentLanguage is not null && CurrentAccent is not null)
					{
						fpsb.Append("Speaking: ");
						fpsb.Append(Telnet.Green.Colour);
						fpsb.Append(CurrentLanguage.Name);
						fpsb.Append(" (");
						fpsb.Append(CurrentAccent.Name);
						fpsb.Append(")");
						fpsb.Append(Telnet.RESETALL);
						included = true;
					}

					if (Account.PromptType.HasFlag(PromptType.StealthInfo) && !string.IsNullOrEmpty(stealthString))
					{
						if (included)
						{
							fpsb.Append(" | ");
						}

						fpsb.Append(stealthString);
						included = true;
					}

					if (Account.PromptType.HasFlag(PromptType.PositionInfo))
					{
						if (included)
						{
							fpsb.Append(" | ");
						}

						fpsb.Append(Body.GetPositionDescription(this, true, false, PerceiveIgnoreFlags.IgnoreHiding));
					}
				}

				fpsb.Append(">\n\n");
				return fpsb.ToString();

				//return
				//	string.Format(this, "\n<{2}>\n<Stamina: {4}{0}/{1}{5}{10} | Exertion: {3}{9}{7}{8}{6}>\n\n",
				//		currentstaminaString,
				//		maxstaminaString,
				//		HealthStrategy.ReportConditionPrompt(this, PromptType.Full),
				//		(CurrentExertion > LongtermExertion ? CurrentExertion.Describe() : LongtermExertion.Describe())
				//		.Colour(Telnet.Green),
				//		boldColour ? staminaColour.Bold : staminaColour.Colour,
				//		Telnet.RESETALL,
				//		Account.PromptType.HasFlag(PromptType.PositionInfo)
				//			? $" | {Body.GetPositionDescription(this, true, false, PerceiveIgnoreFlags.IgnoreHiding)}"
				//			: "",
				//		Account.PromptType.HasFlag(PromptType.SpeakInfo)
				//			? $" | Speaking: {CurrentLanguage.Name.Colour(Telnet.Green)}"
				//			: "",
				//		Account.PromptType.HasFlag(PromptType.StealthInfo) && !string.IsNullOrEmpty(stealthString)
				//			? $" | {stealthString}"
				//			: "",
				//		!NeedsToBreathe || CanBreathe
				//			? ""
				//			: $" | Breath: {HeldBreathPercentage.ToString("P0", this).ColourValue()}",
				//		Account.PromptType.HasFlag(PromptType.IncludeMagic) &&
				//		Capabilities.Any(x => x.ShowMagicResourcesInPrompt)
				//			? $" | {MagicResourceAmounts.Select(x => $"{x.Key.ShortName}: {(x.Value / x.Key.ResourceCap(this)).ToString("P0", this)}").ListToString(separator: " ", conjunction: "", twoItemJoiner: " ")}"
				//			: ""
				//	);
			}

			//Otherwise, build a brief prompt
			var healthString = HealthStrategy.ReportConditionPrompt(this, PromptType.FullBrief);

			var sb = new StringBuilder();
			if (!healthString.Equals(string.Empty))
			{
				healthString = "<" + healthString + ">";
			}

			var setStamina = false;
			if (staminaRatio < 0.8)
			{
				sb.Append(
					$"Stamina: {(boldColour ? staminaColour.Bold : staminaColour.Colour)}{currentstaminaString}/{maxstaminaString}{Telnet.RESETALL}");

				setStamina = true;
			}

			var setExertion = false;
			if (CurrentExertion > ExertionLevel.Normal || LongtermExertion > ExertionLevel.Normal)
			{
				if (setStamina)
				{
					sb.Append(" | ");
				}

				sb.Append(
					$"Exertion: {(CurrentExertion > LongtermExertion ? CurrentExertion.Describe() : LongtermExertion.Describe()).Colour(Telnet.Green)}");
				setExertion = true;
			}

			var setSpeaking = false;
			if (Account.PromptType.HasFlag(PromptType.SpeakInfo))
			{
				if (setStamina || setExertion)
				{
					sb.Append(" | ");
				}

				sb.Append($"Speaking: {CurrentLanguage.Name.Colour(Telnet.Green)}");
				setSpeaking = true;
			}

			var setSneaking = false;
			if (Account.PromptType.HasFlag(PromptType.StealthInfo))
			{
				if (setStamina || setExertion || setSpeaking)
				{
					if (stealthString.Equals(string.Empty) != true)
					{
						sb.Append(" | ");
					}
				}

				if (!stealthString.Equals(string.Empty))
				{
					sb.Append(stealthString);
					setSneaking = true;
				}
			}

			if (Account.PromptType.HasFlag(PromptType.PositionInfo))
			{
				if (setStamina || setExertion || setSpeaking || setSneaking)
				{
					sb.Append(" | ");
				}

				sb.Append(Body.GetPositionDescription(this, true, false, PerceiveIgnoreFlags.IgnoreHiding));
			}

			if (sb.Length == 0)
			{
				return healthString.Equals(string.Empty) ? "" : $"\n{healthString}\n\n";
			}
			else
			{
				return $"\n{healthString}<{sb}>\n\n";
			}
		}
	}

	#endregion

	#region IHaveCommunity Members

	private readonly List<IClanMembership> _clanMemberships = new();
	public IEnumerable<IClanMembership> ClanMemberships => _clanMemberships;

	public void AddMembership(IClanMembership membership)
	{
		_clanMemberships.Add(membership);
	}

	public void RemoveMembership(IClanMembership membership)
	{
		_clanMemberships.Remove(membership);
	}

	#endregion

	#region IHaveDubs Members

	private List<IDub> _dubs = new();
	public override IList<IDub> Dubs => _dubs;

	public override bool HasDubFor(IKeyworded target, IEnumerable<string> keywords)
	{
		if (target is not IPerceivable perceivable)
		{
			return false;
		}

		var dub = Dubs.FirstOrDefault(x =>
			x.TargetId == perceivable.Id &&
			x.TargetType == perceivable.FrameworkItemType &&
			x.Keywords.Any(y => keywords.Any(z => y.StartsWith(z, StringComparison.InvariantCultureIgnoreCase)))
		);

		if (dub == null)
		{
			return false;
		}

		dub.LastUsage = DateTime.UtcNow;
		dub.LastDescription = perceivable.HowSeen(this, colour: false, flags: PerceiveIgnoreFlags.IgnoreNamesSetting);
		dub.Changed = true;
		return true;
	}

	public override bool HasDubFor(IKeyworded target, string keyword)
	{
		if (target is not IPerceivable perceivable)
		{
			return false;
		}

		var dub = Dubs.FirstOrDefault(x =>
			x.TargetId == perceivable.Id &&
			x.TargetType == perceivable.FrameworkItemType &&
			x.Keywords.Any(y => y.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
		);

		if (dub == null)
		{
			return false;
		}

		dub.LastUsage = DateTime.UtcNow;
		dub.LastDescription = perceivable.HowSeen(this, colour: false, flags: PerceiveIgnoreFlags.IgnoreNamesSetting);
		dub.Changed = true;
		return true;
	}

	#endregion

	#region IHaveRoles Members

	protected List<IChargenRole> _roles = new();
	public IEnumerable<IChargenRole> Roles => _roles;

	#endregion

	#region IHaveNeeds Members

	public INeedsModel NeedsModel { get; protected set; }

	public virtual NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false)
	{
		return Body.FulfilNeeds(fulfiller, ignoreDelays);
	}

	public void DescribeNeedsResult(NeedsResult result)
	{
		Body.DescribeNeedsResult(result);
	}

	public virtual void NeedsHeartbeat()
	{
		Body.NeedsHeartbeat();
	}

	public virtual void StartNeedsHeartbeat()
	{
		if (NeedsModel.NeedsSave)
		{
			Gameworld.HeartbeatManager.TenSecondHeartbeat -= NeedsHeartbeat;
			Gameworld.HeartbeatManager.TenSecondHeartbeat += NeedsHeartbeat;
		}
	}

	#endregion

	#region IEat Members

	public bool Eat(IEdible edible, IContainer container, ITable table, double bites, IEmote playerEmote)
	{
		return Body.Eat(edible, container, table, bites, playerEmote);
	}

	public bool SilentEat(IEdible edible, double bites)
	{
		return Body.SilentEat(edible, bites);
	}

	public bool CanEat(IEdible edible, IContainer container, ITable table, double bites)
	{
		return Body.CanEat(edible, container, table, bites);
	}

	public (bool Success, string ErrorMessage) CanEat(ICorpse corpse, double bites)
	{
		return Body.CanEat(corpse, bites);
	}

	public (bool Success, string ErrorMessage) CanEat(ISeveredBodypart bodypart, double bites)
	{
		return Body.CanEat(bodypart, bites);
	}

	public (bool Success, string ErrorMessage) CanEat(string foragableYield, double bites)
	{
		return Body.CanEat(foragableYield, bites);
	}

	public (bool Success, string ErrorMessage) Eat(ICorpse corpse, double bites, IEmote playerEmote)
	{
		return Body.Eat(corpse, bites, playerEmote);
	}

	public (bool Success, string ErrorMessage) Eat(ISeveredBodypart bodypart, double bites, IEmote playerEmote)
	{
		return Body.Eat(bodypart, bites, playerEmote);
	}

	public (bool Success, string ErrorMessage) Eat(string foragableYield, double bites, IEmote playerEmote)
	{
		return Body.Eat(foragableYield, bites, playerEmote);
	}

	public bool Drink(ILiquidContainer container, ITable table, double quantity, IEmote playerEmote)
	{
		return Body.Drink(container, table, quantity, playerEmote);
	}

	public bool SilentDrink(ILiquidContainer container, double quantity)
	{
		return Body.SilentDrink(container, quantity);
	}

	public bool CanDrink(ILiquidContainer container, ITable table, double quantity)
	{
		return Body.CanDrink(container, table, quantity);
	}

	public bool Swallow(ISwallowable swallowable, IContainer container, ITable table, IEmote playerEmote)
	{
		return Body.Swallow(swallowable, container, table, playerEmote);
	}

	public bool SilentSwallow(ISwallowable swallowable)
	{
		return Body.SilentSwallow(swallowable);
	}

	public bool CanSwallow(ISwallowable swallowable, IContainer container, ITable table)
	{
		return Body.CanSwallow(swallowable, container, table);
	}

	#endregion

	#region IHaveKnowledges Members

	public IEnumerable<IKnowledge> Knowledges
	{
		get { return CharacterKnowledges.Select(x => x.Knowledge).ToList(); }
	}

	private readonly List<ICharacterKnowledge> _characterKnowledges = new();
	public IEnumerable<ICharacterKnowledge> CharacterKnowledges => _characterKnowledges;

	public string PreferredSurgicalSchool { get; set; }

	private bool _knowledgesChanged;

	public bool KnowledgesChanged
	{
		get => _knowledgesChanged;
		set
		{
			if (value)
			{
				Changed = true;
			}

			_knowledgesChanged = value;
		}
	}

	private readonly List<ICharacterKnowledge> _addedKnowledges = new();

	public void AddKnowledge(ICharacterKnowledge knowledge)
	{
		_characterKnowledges.Add(knowledge);
		_addedKnowledges.Add(knowledge);

		//Knowledge might confer a script, add it too
		foreach (var script in Gameworld.Scripts.Where(x =>
					 x.ScriptKnowledge == knowledge.Knowledge && !_scripts.Contains(x)))
		{
			LearnScript(script);
		}

		KnowledgesChanged = true;
	}

	private readonly List<ICharacterKnowledge> _removedKnowledges = new();

	public void RemoveKnowledge(IKnowledge knowledge)
	{
		var removedKnowledge = _characterKnowledges.First(x => x.Knowledge.Id == knowledge.Id);
		_characterKnowledges.Remove(removedKnowledge);
		_removedKnowledges.Add(removedKnowledge);

		//May need to remove scripts too
		foreach (var script in Gameworld.Scripts.Where(x => x.ScriptKnowledge == knowledge &&
															_scripts.Contains(x)))
			//Make sure we don't have another knowledge that gives us the script
		{
			if (!_characterKnowledges.Any(x => x.Knowledge == script.ScriptKnowledge))
			{
				ForgetScript(script);
			}
		}

		KnowledgesChanged = true;
	}

	#endregion

	#region IStyleCharacterCharacteristics Members

	public IEnumerable<IGrowableCharacteristicValue> PossibleStyles(ICharacteristicDefinition definition)
	{
		var selectables = Gameworld.CharacteristicValues.OfType<IGrowableCharacteristicValue>().Where(x =>
			x.Definition == definition && ((bool?)x.ChargenApplicabilityProg?.Execute(this) ?? true));
		if (!(Body.GetCharacteristic(definition, null) is IGrowableCharacteristicValue current))
		{
			return Enumerable.Empty<IGrowableCharacteristicValue>();
		}

		return selectables.Where(x =>
			x.GrowthStage <= current.GrowthStage || (x.GrowthStage == current.GrowthStage + 1 &&
													 !Body.AffectedBy<IRecentlyStyled>(definition))).ToList();
	}

	public string WhyCannotStyle(ICharacter target, ICharacteristicDefinition definition,
		IGrowableCharacteristicValue value)
	{
		if (!CanSee(target))
		{
			return "You can't see your target, which makes it pretty hard to do any styling.";
		}

		if (!target.ColocatedWith(this))
		{
			return "Your target is not in the same location as you, which makes it pretty hard to do any styling.";
		}

		if (Combat != null)
		{
			return "You can't focus on styling while you're in combat.";
		}

		if (target.Combat != null)
		{
			return "You can't style someone while they're in combat.";
		}

		if (Movement != null)
		{
			return "You can't focus on styling while you're moving about.";
		}

		if (target.Movement != null)
		{
			return "You can't focus on styling someone while they're moving about.";
		}

		if (!target.PossibleStyles(definition).Contains(value))
		{
			return $"That is not a style that {target.HowSeen(this)} can have.";
		}

		if (Gameworld.GetCheck(CheckType.StyleCharacteristicCapabilityCheck).Check(this, value.StyleDifficulty)
					 .IsFail())
		{
			return "That style is too difficult for you to even attempt at your current level of ability.";
		}

		var current = target.Body.GetCharacteristic(definition, null) as IGrowableCharacteristicValue;
		var universal = Gameworld.Tags.Get(Gameworld.GetStaticLong("UniversalStyleToolTagId"));
		if (universal != null && value.StyleToolTag == null && Body.HeldOrWieldedItems.All(x => !x.IsA(universal)))
		{
			return
				$"You need to be holding a tool with the {universal.FullName.Colour(Telnet.Cyan)} tag to style that style.";
		}

		if (value.StyleToolTag != null && Body.HeldOrWieldedItems.All(x => !x.IsA(value.StyleToolTag)))
		{
			return
				$"You need to be holding a tool with the {value.StyleToolTag.FullName.Colour(Telnet.Cyan)} tag to style that style.";
		}

		if (current.GrowthStage != value.GrowthStage)
		{
			var different = Gameworld.Tags.Get(Gameworld.GetStaticLong("DifferentGrowthStyleToolTagId"));
			if (different != null && Body.HeldOrWieldedItems.All(x => !x.IsA(different)))
			{
				return
					$"You need to be holding a tool with the {different.FullName.Colour(Telnet.Cyan)} tag to style that style.";
			}
		}

		throw new ApplicationException("Got to the end of Character.WhyCannotStyle");
	}

	public bool CanStyle(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value)
	{
		if (!CanSee(target))
		{
			return false;
		}

		if (!target.ColocatedWith(this))
		{
			return false;
		}

		if (Combat != null)
		{
			return false;
		}

		if (target.Combat != null)
		{
			return false;
		}

		if (Movement != null)
		{
			return false;
		}

		if (target.Movement != null)
		{
			return false;
		}

		if (!target.PossibleStyles(definition).Contains(value))
		{
			return false;
		}

		if (Gameworld.GetCheck(CheckType.StyleCharacteristicCapabilityCheck).Check(this, value.StyleDifficulty)
					 .IsFail())
		{
			return false;
		}

		var current = target.Body.GetCharacteristic(definition, null) as IGrowableCharacteristicValue;
		var universal = Gameworld.Tags.Get(Gameworld.GetStaticLong("UniversalStyleToolTagId"));
		if (universal != null && value.StyleToolTag == null &&
			Body.HeldOrWieldedItems.All(x => !x.IsA(universal)))
		{
			return false;
		}

		if (value.StyleToolTag != null && Body.HeldOrWieldedItems.All(x => !x.IsA(value.StyleToolTag)))
		{
			return false;
		}

		if (current.GrowthStage != value.GrowthStage)
		{
			var different = Gameworld.Tags.Get(Gameworld.GetStaticLong("DifferentGrowthStyleToolTagId"));
			if (different != null && Body.HeldOrWieldedItems.All(x => !x.IsA(different)))
			{
				return false;
			}
		}

		return true;
	}

	public bool Style(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value,
		bool force = false)
	{
		if (!force && !CanStyle(target, definition, value))
		{
			OutputHandler.Send(WhyCannotStyle(target, definition, value));
			return false;
		}

		Gameworld.GetCheck(CheckType.StyleCharacteristicCheck).Check(this, value.StyleDifficulty, target);

		if (force)
		{
			target.SetCharacteristic(definition, value);
			return true;
		}

		void BeginStyle()
		{
			if (!CanStyle(target, definition, value))
			{
				OutputHandler.Send(WhyCannotStyle(target, definition, value));
				return;
			}

			OutputHandler.Handle(new EmoteOutput(
				new Emote($"@ begin|begins to work on $0's {definition.Name.ToLowerInvariant()}.", this, target)));
			EffectHandler.AddEffect(new StagedCharacterActionWithTarget(this, target,
				$"styling {definition.Name.ToLowerInvariant()}",
				$"@ stop|stops working on $1's {definition.Name.ToLowerInvariant()}.",
				$"@ cannot move because #0 are|is working on $1's {definition.Name}.", new[] { "general", "movement" },
				$"working on $1's {definition.Name.ToLowerInvariant()}.", new Action<IPerceivable>[]
				{
					perc =>
					{
						OutputHandler.Handle(new EmoteOutput(new Emote(
							$"@ continue|continues to work on $0's {definition.Name.ToLowerInvariant()}.", this,
							target)));
						Gameworld.GetCheck(CheckType.StyleCharacteristicCheck)
								 .Check(this, value.StyleDifficulty, target);
					},
					perc =>
					{
						OutputHandler.Handle(new EmoteOutput(new Emote(
							$"@ have|has finished working on $0's {definition.Name.ToLowerInvariant()}, it is now {value.GetValue.A_An().Colour(Telnet.Green)}.",
							this, target)));
						Gameworld.GetCheck(CheckType.StyleCharacteristicCheck)
								 .Check(this, value.StyleDifficulty, target);
						target.SetCharacteristic(definition, value);
						if (!target.Body.EffectsOfType<RecentlyStyled>(x => x.CharacteristicType == definition).Any())
						{
							target.Body.AddEffect(new RecentlyStyled(target.Body, definition),
								TimeSpan.FromSeconds(Gameworld.GetStaticInt("RecentlyStyledDelaySeconds")));
						}
						else
						{
							var effect = target.Body
											   .EffectsOfType<RecentlyStyled>(x => x.CharacteristicType == definition)
											   .First();
							target.Body.Reschedule(effect,
								TimeSpan.FromSeconds(Gameworld.GetStaticInt("RecentlyStyledDelaySeconds")));
						}
					}
				}, 2, TimeSpan.FromSeconds(20)), TimeSpan.FromSeconds(20));
		}

		if (target == this || target.IsTrustedAlly(this))
		{
			BeginStyle();
		}
		else
		{
			OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is proposing to work on changing $0's {definition.Name.ToLowerInvariant()}.", this, target)));
			target.OutputHandler.Send(
				$"You can type {"accept".Colour(Telnet.Yellow)} to proceed, or {"decline".Colour(Telnet.Yellow)} to reject their proposal.");
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = text => BeginStyle(),
				DescriptionString = "style a characteristic",
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ decline|declines $1's offer to work on changing &0's {definition.Name.ToLowerInvariant()}.",
						target, target, this)));
				},
				Keywords = { "style" },
				RejectAction = text =>
				{
					target.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ decline|declines $1's offer to work on changing &0's {definition.Name.ToLowerInvariant()}.",
						target, target, this)));
				}
			}));
		}

		return true;
	}

	#endregion

	#region Outfits

	private bool _outfitsChanged;
	private bool _noMercy;

	public bool OutfitsChanged
	{
		get => _outfitsChanged;
		set
		{
			_outfitsChanged = value;
			if (value)
			{
				Changed = true;
			}
		}
	}

	private readonly List<IOutfit> _outfits = new();
	public IEnumerable<IOutfit> Outfits => _outfits;

	public void AddOutfit(IOutfit outfit)
	{
		_outfits.Add(outfit);
		OutfitsChanged = true;
	}

	public void RemoveOutfit(IOutfit outfit)
	{
		_outfits.Remove(outfit);
		OutfitsChanged = true;
	}

	private void SaveOutfits(MudSharp.Models.Character dbchar)
	{
		dbchar.Outfits = new XElement("Outfits",
			from outfit in _outfits
			select outfit.SaveToXml()
		).ToString();
		_outfitsChanged = false;
	}

	private void LoadOutfits(MudSharp.Models.Character dbchar)
	{
		if (string.IsNullOrWhiteSpace(dbchar.Outfits))
		{
			return;
		}

		var root = XElement.Parse(dbchar.Outfits);
		foreach (var element in root.Elements("Outfit"))
		{
			_outfits.Add(new Outfit(this, element));
		}
	}

	#endregion
}