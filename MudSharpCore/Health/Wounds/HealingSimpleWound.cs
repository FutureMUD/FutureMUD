using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Logging;
using System.Xml.Linq;
using MudSharp.Body.Needs;

namespace MudSharp.Health.Wounds;

public class HealingSimpleWound : PerceivedItem, IWound
{
	public HealingSimpleWound(IHaveWounds parent, Wound wound, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_parent = parent;
		LoadFromDb(wound);
		//LoadEffects(wound.Effects);
		_internal = !(_bodypart == null || _bodypart is IExternalBodypart);
	}

	public HealingSimpleWound(IFuturemud gameworld, IHaveWounds owner, double damage, DamageType damageType,
		IBodypart bodypart, IGameItem lodged, IGameItem toolOrigin, ICharacter actorOrigin)
	{
		if (owner == null)
		{
			throw new ArgumentNullException(nameof(owner));
		}
#if DEBUG
		if (bodypart == null && owner is ICharacter)
		{
			throw new ApplicationException("Character wound with no bodypart.");
		}
#endif
		Gameworld = gameworld;
		_parent = owner;
		_currentDamage = Math.Max(0.0,
			Math.Min(damage * bodypart.DamageModifier, CharacterParent.Body.HitpointsForBodypart(bodypart)));
		_originalDamage = _currentDamage;
		DamageType = damageType;
		_bodypart = bodypart;
		_lodged = lodged;
		_actorOriginId = actorOrigin?.Id ?? 0;
		_toolOriginId = toolOrigin?.Id ?? 0;
		BleedStatus = BleedStatus.NeverBled;
		if (actorOrigin?.Combat?.Friendly == true)
		{
			IsFriendlyWound = true;
		}

		_damageDescription = SimpleOrganicWound.GetWoundDescription(damageType, Severity);
		Gameworld.SaveManager.AddInitialisation(this);
		_internal = !(_bodypart == null || _bodypart is IExternalBodypart);
	}

	public ICharacter CharacterParent
	{
		get
		{
			if (_parent is ICharacter ch)
			{
				return ch;
			}

			return ((IGameItem)_parent).GetItemType<ISeveredBodypart>().OriginalCharacter;
		}
	}

	public override void Register(IOutputHandler handler)
	{
		// Do nothing
	}

	public override string FrameworkItemType => "Wound";

	public override void Save()
	{
		var dbitem = FMDB.Context.Wounds.Find(Id);
		dbitem.CurrentDamage = CurrentDamage;
		dbitem.OriginalDamage = OriginalDamage;
		dbitem.LodgedItem = FMDB.Context.GameItems.Find(Lodged?.Id);
		dbitem.ExtraInformation = SaveExtras();
		base.Save();
	}

	public string SaveExtras()
	{
		return new XElement("Definition",
			new XElement("DamageDescription", _damageDescription),
			new XElement("BleedStatus", (int)BleedStatus),
			new XElement("Tended", (int)_tended),
			new XElement("TreatmentAttempts", _unsuccessfulTreatmentAttempts),
			new XElement("IsFriendlyWound", IsFriendlyWound)
		).ToString();
	}

	public void Delete(bool ignoreDatabaseDeletion = false)
	{
		_noSave = true;
		Changed = false;
		Gameworld.SaveManager.Abort(this);
		Lodged?.Delete();
		if (!IdInitialised || ignoreDatabaseDeletion)
		{
			return;
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.Wounds.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.Wounds.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public bool Repairable => true;

	public void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus)
	{
		if (Lodged != null || !(_parent is ICharacter ch))
		{
			return;
		}

		var tendAmount = 1.0;
		switch (_tended)
		{
			case Outcome.Fail:
				tendAmount = 1.025;
				break;
			case Outcome.MinorFail:
				tendAmount = 1.05;
				break;
			case Outcome.MinorPass:
				tendAmount = 1.1;
				break;
			case Outcome.Pass:
				tendAmount = 1.2;
				break;
			case Outcome.MajorPass:
				tendAmount = 1.3;
				break;
		}

		var amountModifier = tendAmount * externalRateMultiplier;

		if (CurrentStun > 0)
		{
			var stunCheck = Gameworld.GetCheck(CheckType.StunRecoveryCheck);
			var amount =
				Math.Max(0, Parent.HealthStrategy.GetHealingTickAmount(this, Outcome.MinorPass, HealthDamageType.Stun) *
				            amountModifier * Math.Max(0.01,
					            stunCheck.TargetNumber(ch, Difficulty.Normal, null, externalBonus: externalCheckBonus) *
					            0.01) *
				            timePassed.TotalMinutes);
			CurrentStun = Math.Max(_currentDamage - amount, 0);
		}

		var difficulty = CanBeTreated(TreatmentType.Mend).StageUp(_tended != Outcome.None ? -1 : 0);
		if (_currentDamage > 0)
		{
			var healthCheck = Gameworld.GetCheck(CheckType.HealingCheck);
			var amount =
				Math.Max(0,
					Parent.HealthStrategy.GetHealingTickAmount(this, Outcome.MinorPass, HealthDamageType.Damage) *
					amountModifier * Math.Max(0.01,
						healthCheck.TargetNumber(ch, difficulty, null, externalBonus: externalCheckBonus) * 0.01) *
					0.4 *
					timePassed.TotalMinutes);
			CurrentDamage = Math.Max(_currentDamage - amount, 0);
			// TODO - hunger and thirst?
		}
	}

	public bool EligableForInfection()
	{
		return false;
	}

	public DamageType DamageType { get; set; }

	public IInfection Infection
	{
		get => null;
		set { }
	}

	private string _damageDescription;

	public string TextForAdminWoundsCommand => "";

	#region IFutureProgVariable Implementation

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	#endregion

	public override object DatabaseInsert()
	{
		if (Parent.Id == 0)
		{
			return null;
		}

		var dbitem = new Wound();
		FMDB.Context.Wounds.Add(dbitem);
		dbitem.WoundType = "HealingSimple";
		dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
		dbitem.GameItemId = (Parent as IGameItem)?.Id;
		dbitem.OriginalDamage = OriginalDamage;
		dbitem.CurrentDamage = _currentDamage;
		dbitem.DamageType = (int)DamageType;
		dbitem.BodypartProtoId = Bodypart?.Id;
		dbitem.LodgedItemId = Lodged?.Id;
		dbitem.ActorOriginId = _actorOriginId != 0 ? _actorOriginId : default(long?);
		dbitem.ToolOriginId = _toolOriginId != 0 ? _toolOriginId : default(long?);
		dbitem.ExtraInformation = SaveExtras();
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Wound)dbitem)?.Id ?? 0;
	}

	public override InitialisationPhase InitialisationPhase => InitialisationPhase.AfterFirstDatabaseHit;

	private void LoadFromDb(Wound wound)
	{
		_id = wound.Id;
		IdInitialised = true;
		_name = string.Empty;
		_currentDamage = wound.CurrentDamage;
		_originalDamage = wound.OriginalDamage;
		DamageType = (DamageType)wound.DamageType;
		_bodypart = Gameworld.BodypartPrototypes.Get(wound.BodypartProtoId ?? 0);
		if (wound.LodgedItemId.HasValue)
		{
			_lodged = Gameworld.TryGetItem(wound.LodgedItemId ?? 0, true);
		}

		_actorOriginId = wound.ActorOriginId ?? 0;
		_toolOriginId = wound.ToolOriginId ?? 0;
		var root = XElement.Parse(wound.ExtraInformation ?? "<Empty/>");
		IsFriendlyWound = bool.Parse(root.Element("IsFriendlyWound")?.Value ?? "false");
	}

	#region IWound Members

	public bool IsFriendlyWound { get; protected set; }

	public void SetNewOwner(IHaveWounds newOwner)
	{
		using (new FMDB())
		{
			var dbwound = FMDB.Context.Wounds.Find(Id);
			if (dbwound == null)
			{
				return;
			}

			dbwound.BodyId = (newOwner as ICharacter)?.Body.Id;
			dbwound.GameItemId = (newOwner as IGameItem)?.Id;
			FMDB.Context.SaveChanges();
		}

		_parent = newOwner;
	}

	public bool UseDamagePercentageSeverities => false;

	public void SufferAdditionalDamage(IDamage damage)
	{
		OriginalDamage += damage.DamageAmount;
		CurrentDamage += damage.DamageAmount;
		_tended = Outcome.None;
		_unsuccessfulTreatmentAttempts = 0;
		Changed = true;
	}

	public void OnWoundSuffered()
	{
		// Do nothing
	}

	public bool ShouldWoundBeRemoved()
	{
		return _currentDamage <= 0.0 && _lodged == null;
	}

	public BleedStatus BleedStatus
	{
		get => BleedStatus.NeverBled;
		set { }
	}

	private long _toolOriginId;

	public IGameItem ToolOrigin
	{
		get => Gameworld.TryGetItem(_toolOriginId);
		set
		{
			_toolOriginId = value?.Id ?? 0;
			Changed = true;
		}
	}

	private long _actorOriginId;

	public ICharacter ActorOrigin
	{
		get => Gameworld.TryGetCharacter(_actorOriginId, true);
		set
		{
			_actorOriginId = value?.Id ?? 0;
			Changed = true;
		}
	}

	private IGameItem _lodged;

	public IGameItem Lodged
	{
		get => _lodged;
		set
		{
			_lodged = value;
			Changed = true;
		}
	}

	public double CurrentPain
	{
		get => 0;
		set { }
	}

	public double CurrentStun
	{
		get => 0;
		set { }
	}

	public double CurrentShock
	{
		get => 0;
		set { }
	}

	private double _originalDamage;

	public double OriginalDamage
	{
		get => _originalDamage;
		set
		{
			_originalDamage = value;
			Changed = true;
		}
	}

	private double _currentDamage;

	public double CurrentDamage
	{
		get => _currentDamage;
		set
		{
			_currentDamage = Math.Max(0.0, value);
			Changed = true;
		}
	}

	public IBodypart SeveredBodypart { get; set; }

	public WoundSeverity Severity => Parent.GetSeverityFor(this);

	private bool _internal;
	public bool Internal => _internal;

	private Outcome _tended = Outcome.None;

	private IHaveWounds _parent;

	public IHaveWounds Parent
	{
		get => _parent;
		set => _parent = value;
	}

	private IBodypart _bodypart;

	public IBodypart Bodypart
	{
		get => _bodypart;
		set
		{
			_bodypart = value;
			Changed = true;
		}
	}

	public string Describe(WoundExaminationType type, Outcome outcome)
	{
		if (type == WoundExaminationType.Glance)
		{
			// Depending on the outcome, glances might not see certain levels of wounds
			var i = outcome.IsPass() ? 3 - outcome.SuccessDegrees() : 3 + outcome.FailureDegrees();
			if (Severity.StageDown(i) == WoundSeverity.None)
			{
				return "";
			}
		}

		if (Severity == WoundSeverity.None && type != WoundExaminationType.Self)
		{
			return "";
		}

		return $"{Severity.Describe()} {_damageDescription}".A_An().ToLowerInvariant();
	}

	public string WoundTypeDescription => _damageDescription;

	public Difficulty CanBeTreated(TreatmentType type)
	{
		if (!(_parent is ICharacter ch))
		{
			return Difficulty.Impossible;
		}

		if (Severity == WoundSeverity.None)
		{
			return Difficulty.Impossible;
		}

		switch (type)
		{
			case TreatmentType.Repair:
			case TreatmentType.Relocation:
			case TreatmentType.Set:
			case TreatmentType.Clean:
			case TreatmentType.Antiseptic:
			case TreatmentType.Close:
			case TreatmentType.Trauma:
				return Difficulty.Impossible;
			case TreatmentType.AntiInflammatory:
				return Difficulty.Impossible;
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Tend && _tended == Outcome.MajorPass)
		{
			return Difficulty.Impossible;
		}

		var difficulty = Difficulty.Impossible;
		switch (Severity)
		{
			case WoundSeverity.Superficial:
				difficulty = Difficulty.Automatic;
				break;
			case WoundSeverity.Minor:
				difficulty = Difficulty.Trivial;
				break;
			case WoundSeverity.Small:
				difficulty = Difficulty.ExtremelyEasy;
				break;
			case WoundSeverity.Moderate:
				difficulty = Difficulty.VeryEasy;
				break;
			case WoundSeverity.Severe:
				difficulty = Difficulty.Easy;
				break;
			case WoundSeverity.VerySevere:
				difficulty = Difficulty.Normal;
				break;
			case WoundSeverity.Grievous:
				difficulty = Difficulty.Hard;
				break;
			case WoundSeverity.Horrifying:
				difficulty = Difficulty.VeryHard;
				break;
		}

		if (type == TreatmentType.Tend)
		{
			switch (DamageType)
			{
				case DamageType.Claw:
				case DamageType.Chopping:
					difficulty = difficulty.StageUp(1);
					break;
				case DamageType.Ballistic:
				case DamageType.Bite:
					difficulty = difficulty.StageUp(2);
					break;
				case DamageType.Shearing:
				case DamageType.Slashing:
					difficulty = difficulty.StageUp(3);
					break;
			}
		}

		if (difficulty == Difficulty.Impossible)
		{
			difficulty = Difficulty.Insane;
		}

		if (_unsuccessfulTreatmentAttempts > 0)
		{
			difficulty = difficulty.StageUp(_unsuccessfulTreatmentAttempts / 3);
		}

		return difficulty;
	}

	public string WhyCannotBeTreated(TreatmentType type)
	{
		if (!(_parent is ICharacter ch))
		{
			return "Wounds on severed bodyparts cannot be treated.";
		}

		switch (type)
		{
			case TreatmentType.Repair:
			case TreatmentType.Relocation:
			case TreatmentType.Set:
			case TreatmentType.Clean:
			case TreatmentType.Antiseptic:
			case TreatmentType.Close:
			case TreatmentType.Trauma:
				return "That kind of treatment cannot be applied to that wound.";
			case TreatmentType.AntiInflammatory:
				return "That kind of treatment is not yet implemented.";
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return "There is nothing in that wound that requires removal.";
		}

		if (type == TreatmentType.Tend && _tended == Outcome.MajorPass)
		{
			return "That wound has already been tended as skillfully and successfully as is possible.";
		}

		return "That kind of treatment cannot be applied to that wound.";
	}

	private int _unsuccessfulTreatmentAttempts;

	public void Treat(IPerceiver treater, TreatmentType type, ITreatment treatmentItem, Outcome testOutcome,
		bool silent)
	{
		if (!(_parent is ICharacter ch))
		{
			return;
		}

		// Mending would generally be magical healing
		switch (type)
		{
			case TreatmentType.Mend:
				if (testOutcome.IsFail() && testOutcome != Outcome.MinorFail)
				{
					_unsuccessfulTreatmentAttempts++;
					Changed = true;
					if (treater != null && !silent)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts to treat {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} have only succeeded in making things worse!",
							treater, treater)));
					}
				}
				else if (testOutcome == Outcome.MinorFail)
				{
					if (treater != null && !silent)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts to treat {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} has not succeeded.",
							treater, treater)));
					}
				}
				else
				{
					if (treater != null && !silent)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's effort to mend {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} has been {(testOutcome == Outcome.MajorPass ? "majorly" : testOutcome == Outcome.Pass ? "" : "marginally")} successful.",
							treater, treater)));
					}

					_unsuccessfulTreatmentAttempts = 0;
					CurrentDamage = Parent.GetSeverityFloor(Severity.StageDown(testOutcome.SuccessDegrees()));
					CurrentPain = Math.Min(CurrentPain, CurrentDamage);
					CurrentStun = Math.Min(CurrentStun, CurrentDamage);
				}

				return;
			case TreatmentType.Tend:
				if (treater != null && !silent)
				{
					if (treatmentItem == null)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0 have|has finished tending to {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}
					else
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0 have|has finished tending to {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} with $1.",
							treater, treater, treatmentItem.Parent)));
					}
				}

				treatmentItem?.UseTreatment();
				if (!silent && treater != null)
				{
					if (testOutcome > _tended)
					{
						switch (testOutcome)
						{
							case Outcome.MajorFail:
								treater.Send(
									"You're pretty sure you've done a terrible job, but any treatment is better than none right?."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.Fail:
							case Outcome.MinorFail:
								treater.Send(
									"You're pretty sure you could have done a better job, but the wound is better tended than it was."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.MinorPass:
							case Outcome.Pass:
								treater.Send(
									"You're pretty sure you've done a good job, and the wound is better tended than it was."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.MajorPass:
								treater.Send(
									"You're pretty sure you've done an excellent job, and the wound won't need any further treatment."
										.Colour(Telnet.Yellow));
								break;
						}
					}
					else if (testOutcome == _tended)
					{
						switch (testOutcome)
						{
							case Outcome.MajorFail:
								treater.Send(
									"You're pretty sure you've done a terrible job, but at least you haven't made things any worse."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.Fail:
							case Outcome.MinorFail:
								treater.Send(
									"You're pretty sure you could have done a better job, but the wound is no better off than it was before your efforts."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.MinorPass:
							case Outcome.Pass:
								treater.Send(
									"You're pretty sure you've done a good job, but the wound is no better off than it was before your efforts."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.MajorPass:
								treater.Send(
									"You're pretty sure you've done an excellent job, but the wound is no better off than it was before your efforts."
										.Colour(Telnet.Yellow));
								break;
						}
					}
					else
					{
						switch (testOutcome)
						{
							case Outcome.MajorFail:
								treater.Send(
									"You're pretty sure you've done a terrible job, but at least you haven't made things any worse."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.Fail:
							case Outcome.MinorFail:
								treater.Send(
									"You're pretty sure you could have done a better job, but fortunately someone else has already done a better job than you."
										.Colour(Telnet.Yellow));
								break;
							case Outcome.MinorPass:
							case Outcome.Pass:
								treater.Send(
									"You're pretty sure you've done a good job, but fortunately someone else has already done a better job than you."
										.Colour(Telnet.Yellow));
								break;
						}
					}
				}

				_tended = _tended.Best(testOutcome);
				Changed = true;
				return;
		}
	}

	public BleedResult Bleed(double currentBloodLitres, ExertionLevel activityExertionLevel,
		double totalBloodLitres)
	{
		return BleedResult.NoBleed;
	}

	public double PeekBleed(double bloodTotal, ExertionLevel activityExertionLevel)
	{
		return 0;
	}

	public double Exert(ExertionType exertion)
	{
		return 0;
	}

	public bool HealingTick(double externalRateMultiplier, double externalCheckBonus)
	{
		if (!(_parent is ICharacter ch))
		{
			return false;
		}

		if (ch?.Combat != null)
		{
			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealInCombat,
				Outcome.NotTested);
			return false;
		}

		if (Lodged != null)
		{
			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealLodged,
				Outcome.NotTested);
			return false;
		}

		var tendAmount = 1.0;
		switch (_tended)
		{
			case Outcome.Fail:
				tendAmount = 1.1;
				break;
			case Outcome.MinorFail:
				tendAmount = 1.2;
				break;
			case Outcome.MinorPass:
				tendAmount = 1.4;
				break;
			case Outcome.Pass:
				tendAmount = 1.7;
				break;
			case Outcome.MajorPass:
				tendAmount = 2.0;
				break;
		}

		var amountModifer = (ch.State.HasFlag(CharacterState.Sleeping) ? 1.3 : 1.0) * tendAmount *
		                    externalRateMultiplier;

		Outcome result;
		if (CurrentStun > 0)
		{
			var stunCheck = Gameworld.GetCheck(CheckType.StunRecoveryCheck);
			result = stunCheck.Check(ch, Difficulty.Normal, externalBonus: externalCheckBonus);
			if (result.IsPass())
			{
				CurrentStun =
					Math.Max(
						CurrentStun -
						Parent.HealthStrategy.GetHealingTickAmount(this, result, HealthDamageType.Stun) *
						amountModifer, 0);
			}
		}

		// You must be breathing for healing to take place
		if (ch.NeedsToBreathe && !ch.IsBreathing)
		{
			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealCantBreathe,
				Outcome.NotTested);
			return false;
		}

		var needStatus = ch.Body.NeedsModel.Status;
		var multiplier = 1.0;
		if (needStatus.HasFlag(NeedsResult.Parched))
		{
			multiplier -= 0.45;
		}
		else if (needStatus.HasFlag(NeedsResult.Thirsty))
		{
			multiplier -= 0.1;
		}

		if (needStatus.HasFlag(NeedsResult.Starving))
		{
			multiplier -= 0.45;
		}
		else if (needStatus.HasFlag(NeedsResult.Hungry))
		{
			multiplier -= 0.1;
		}

		Outcome healthResult;
		var difficulty = CanBeTreated(TreatmentType.Mend).StageUp(_tended != Outcome.None ? -1 : 0);
		if (_currentDamage > 0 && ch.Body.CurrentExertion <= ExertionLevel.Rest)
		{
			var healthCheck = Gameworld.GetCheck(CheckType.HealingCheck);
			healthResult = healthCheck.Check(ch, difficulty, externalBonus: externalCheckBonus);
			if (healthResult.IsPass())
			{
				var healing = Parent.HealthStrategy.GetHealingTickAmount(this, healthResult, HealthDamageType.Damage) *
				              amountModifer * multiplier;
				CurrentDamage = Math.Max(_currentDamage - healing, 0);
				ch.Body.NeedsModel.FulfilNeeds(new NeedFulfiller
				{
					ThirstPoints = -0.0075 * CurrentDamage / 100,
					WaterLitres = -0.001 * CurrentDamage / 100,
					Calories = -2.0 * CurrentDamage / 100,
					SatiationPoints = -0.01 * CurrentDamage / 100
				}, true);
				Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
					CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.Healed, healthResult,
					healing);
			}
		}

		return true;
	}

	public Difficulty ConcentrationDifficulty
	{
		get
		{
			Difficulty difficulty;
			switch (Severity)
			{
				case WoundSeverity.None:
					difficulty = Difficulty.Automatic;
					break;
				case WoundSeverity.Superficial:
					difficulty = Difficulty.Trivial;
					break;
				case WoundSeverity.Minor:
					difficulty = Difficulty.ExtremelyEasy;
					break;
				case WoundSeverity.Small:
					difficulty = Difficulty.VeryEasy;
					break;
				case WoundSeverity.Moderate:
					difficulty = Difficulty.Easy;
					break;
				case WoundSeverity.Severe:
					difficulty = Difficulty.Normal;
					break;
				case WoundSeverity.VerySevere:
					difficulty = Difficulty.Hard;
					break;
				case WoundSeverity.Grievous:
					difficulty = Difficulty.VeryHard;
					break;
				case WoundSeverity.Horrifying:
					difficulty = Difficulty.ExtremelyHard;
					break;
				default:
					difficulty = Difficulty.Automatic;
					break;
			}

			return difficulty;
		}
	}

	#endregion
}