using System;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Wounds;

/// <summary>
///     A simple wound is a wound with no pain, shock or bleeding. It would be used by, for example, items or undead.
///     Simple wounds also do not heal.
/// </summary>
public class SimpleWound : PerceivedItem, IWound
{
	public SimpleWound(IHaveWounds parent, Wound wound, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_parent = parent;
		LoadFromDb(wound);
		//LoadEffects(wound.Effects);
		_internal = !(_bodypart == null || _bodypart is IExternalBodypart);
	}

	public SimpleWound(IFuturemud gameworld, IHaveWounds owner, double damage, DamageType damageType,
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
		_originalDamage = Math.Max(0.0, damage);
		_currentDamage = Math.Max(0.0, damage);
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

		Gameworld.SaveManager.AddInitialisation(this);
		_internal = !(_bodypart == null || _bodypart is IExternalBodypart);
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
			//FMDB.Context.Entry(new MudSharp.Models.Wound{Id = _ID}).State = EntityState.Deleted;
			//FMDB.Context.SaveChanges();
		}
	}

	public bool Repairable => true;

	public void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus)
	{
		// Do nothing
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

	#region IFutureProgVariable Implementation

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	#endregion

	public static string GetWoundDescription(DamageType type)
	{
		switch (type)
		{
			case DamageType.Slashing:
				return "Gash";
			case DamageType.Chopping:
				return "Gouge";
			case DamageType.Crushing:
			case DamageType.Falling:
			case DamageType.Arcane:
				return "Dent";
			case DamageType.Piercing:
				return "Hole";
			case DamageType.ArmourPiercing:
				return "Bullet Hole";
			case DamageType.Ballistic:
				return "Bullet Hole";
			case DamageType.Burning:
			case DamageType.Electrical:
				return "Scorch";
			case DamageType.Freezing:
				return "Frostburn";
			case DamageType.Chemical:
				return "Chemical Burn";
			case DamageType.Shockwave:
			case DamageType.Sonic:
				return "Crack";
			case DamageType.Bite:
				return "Bite";
			case DamageType.Claw:
				return "Claw-Gash";
			case DamageType.Hypoxia:
				return "Cyanosis";
			case DamageType.Cellular:
				return "Tissue Death";
			case DamageType.Shearing:
				return "Shear";
			case DamageType.Wrenching:
				return "Twist";
			case DamageType.Shrapnel:
				return "Shrapnel Hole";
			case DamageType.Necrotic:
			case DamageType.Eldritch:
				return "Rot";
		}

		return "Unknown";
	}

	public override object DatabaseInsert()
	{
		if (Parent.Id == 0)
		{
			return null;
		}

		var dbitem = new Wound();
		FMDB.Context.Wounds.Add(dbitem);
		dbitem.WoundType = "Simple";
		dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
		dbitem.GameItemId = (Parent as IGameItem)?.Id;
		dbitem.OriginalDamage = OriginalDamage;
		dbitem.CurrentDamage = _currentDamage;
		dbitem.DamageType = (int)DamageType;
		dbitem.BodypartProtoId = Bodypart?.Id;
		dbitem.LodgedItemId = Lodged?.Id;
		dbitem.ActorOriginId = _actorOriginId != 0 ? _actorOriginId : default(long?);
		dbitem.ToolOriginId = _toolOriginId != 0 ? _toolOriginId : default(long?);
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

	public bool IsFriendlyWound { get; protected set; }

	public string Describe(WoundExaminationType type, Outcome outcome)
	{
		switch (type)
		{
			case WoundExaminationType.Glance:
				// Depending on the outcome, glances might not see certain levels of wounds
				var i = outcome.IsPass() ? 3 - outcome.SuccessDegrees() : 3 + outcome.FailureDegrees();
				if (Severity.StageDown(i) == WoundSeverity.None)
				{
					return "";
				}

				break;
		}

		return $"{Severity.Describe()} {GetWoundDescription(DamageType)}".A_An().ToLowerInvariant();
	}

	public Difficulty CanBeTreated(TreatmentType type)
	{
		if (Bodypart != null && (type != TreatmentType.Mend || type != TreatmentType.Repair))
		{
			return Difficulty.Impossible;
		}

		if (Bodypart == null && type != TreatmentType.Repair)
		{
			return Difficulty.Impossible;
		}

		var difficulty = Difficulty.Impossible;
		switch (Severity)
		{
			case WoundSeverity.None:
			case WoundSeverity.Superficial:
				return Difficulty.Automatic;
			case WoundSeverity.Minor:
				difficulty = Difficulty.Trivial;
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
		}

		if (_unsuccessfulTreatmentAttempts > 0)
		{
			difficulty = difficulty.StageUp(_unsuccessfulTreatmentAttempts / 3);
		}

		if (difficulty == Difficulty.Impossible)
		{
			difficulty = Difficulty.Insane;
		}

		return difficulty;
	}

	public string WhyCannotBeTreated(TreatmentType type)
	{
		if (Bodypart != null && type != TreatmentType.Mend)
		{
			return "This kind of wound can only be mended through extraordinary means.";
		}

		if (Bodypart == null && type != TreatmentType.Repair)
		{
			return "This kind of damage must be repaired by a qualified craftsperson.";
		}

		return "That kind of treatment cannot be applied to that wound.";
	}

	private int _unsuccessfulTreatmentAttempts;

	public void Treat(IPerceiver treater, TreatmentType type, ITreatment treatmentItem, Outcome testOutcome,
		bool silent)
	{
		if (testOutcome.IsFail() && testOutcome != Outcome.MinorFail)
		{
			_unsuccessfulTreatmentAttempts++;
			Changed = true;
			if (_unsuccessfulTreatmentAttempts > 3 && RandomUtilities.Random(0, 100) < 2 && Parent is IGameItem item)
			{
				item.Quality = item.Quality.StageUp(-1);
				if (treater != null && !silent)
				{
					treater.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"$0's efforts to repair {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} are so unsuccessful that they have permanently lowered $1's quality.",
						treater, treater, Parent)));
				}
			}
			else if (treater != null && !silent)
			{
				treater.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"$0's efforts to repair {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} are totally unsuccessful.",
					treater, treater)));
			}
		}
		else if (testOutcome == Outcome.MinorFail)
		{
			if (treater != null && !silent)
			{
				treater.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"$0's efforts to repair {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} are unsuccessful.",
					treater, treater)));
			}
		}
		else
		{
			if (treater != null && !silent)
			{
				treater.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"$0's effort to repair {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} has been {(testOutcome == Outcome.MajorPass ? "majorly" : testOutcome == Outcome.Pass ? "" : "marginally")} successful.",
					treater, treater)));
			}

			_unsuccessfulTreatmentAttempts = 0;
			CurrentDamage = Parent.GetSeverityFloor(Severity.StageDown(testOutcome.SuccessDegrees() + 1));
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
		return false;
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