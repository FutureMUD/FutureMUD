using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Logging;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Work.Projects.Impacts;

namespace MudSharp.Health.Wounds;

public class SimpleOrganicWound : PerceivedItem, IWound
{
	private BleedStatus _bleedStatus;
	private bool _cleanAttempted;
	private bool _cleaned;

	private string _damageDescription;

	private IInfection _infection;

	public SimpleOrganicWound(IHaveWounds parent, Wound wound, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_parent = parent;
		LoadFromDb(wound);
		//LoadEffects(wound.Effects);
	}

	private static double _baseFluidOrganBloodlossPerWoundSeverity;

	public static double BaseFluidOrganBloodlossPerWoundSeverity
	{
		get
		{
			if (_baseFluidOrganBloodlossPerWoundSeverity == 0)
			{
				_baseFluidOrganBloodlossPerWoundSeverity = Futuremud.Games.First()
				                                                    .GetStaticDouble(
					                                                    "BaseFluidOrganBloodlossPerWoundSeverity");
			}

			return _baseFluidOrganBloodlossPerWoundSeverity;
		}
	}

	private static double _percentageExternalBloodlossPerWoundSeverity;

	public static double PercentageExternalBloodlossPerWoundSeverity
	{
		get
		{
			if (_percentageExternalBloodlossPerWoundSeverity == 0)
			{
				_percentageExternalBloodlossPerWoundSeverity = Futuremud.Games.First()
				                                                        .GetStaticDouble(
					                                                        "PercentageExternalBloodlossPerWoundSeverity");
			}

			return _percentageExternalBloodlossPerWoundSeverity;
		}
	}

	protected void CheckForOrganBleeding()
	{
		if (!DamageType.CanCauseOrganBleeding())
		{
			return;
		}

		var organ = Bodypart as IOrganProto;
		var body = CharacterParent.Body;
		var effect = body.EffectsOfType<InternalBleeding>().FirstOrDefault(x => x.Organ == organ);
		if (effect == null)
		{
			effect = new InternalBleeding(body, organ, 0.0);
			body.AddEffect(effect);
		}

		effect.BloodlossPerTick = BaseFluidOrganBloodlossPerWoundSeverity * (int)Severity * organ.BleedModifier;
	}

	public SimpleOrganicWound(IFuturemud gameworld, IHaveWounds owner, double damage, double pain, double stun,
		DamageType damageType, IBodypart bodypart, IGameItem lodged, IGameItem toolOrigin,
		ICharacter actorOrigin)
	{
		if (bodypart == null)
		{
			throw new ArgumentNullException(nameof(bodypart));
		}

		Gameworld = gameworld;
		_parent = owner ?? throw new ArgumentNullException(nameof(owner));
		_currentDamage = Math.Max(0.0,
			Math.Min(damage * bodypart.DamageModifier, CharacterParent.Body.HitpointsForBodypart(bodypart)));
		_originalDamage = Math.Max(0.0,
			Math.Min(damage * bodypart.DamageModifier, CharacterParent.Body.HitpointsForBodypart(bodypart)));
		_currentPain = Math.Max(0.0, pain * bodypart.PainModifier);
		_currentStun = Math.Max(0.0, stun * bodypart.StunModifier);
		DamageType = damageType;
		Bodypart = bodypart;
		_lodged = lodged;
		_actorOriginId = actorOrigin?.Id ?? 0;
		_toolOriginId = toolOrigin?.Id ?? 0;
		if (actorOrigin?.Combat?.Friendly == true)
		{
			IsFriendlyWound = true;
		}

		if (Bodypart is IOrganProto organ)
		{
			CheckForOrganBleeding();
		}
		else
		{
			switch (damageType)
			{
				case DamageType.Slashing:
				case DamageType.Claw:
				case DamageType.Chopping:
				case DamageType.Ballistic:
				case DamageType.ArmourPiercing:
				case DamageType.Shearing:
				case DamageType.Arcane:
					_bleedStatus = Severity >= WoundSeverity.Moderate
						? BleedStatus.Bleeding
						: BleedStatus.NeverBled;
					break;
				case DamageType.Piercing:
				case DamageType.Bite:
				case DamageType.Shrapnel:
					_bleedStatus = Severity >= WoundSeverity.Severe ? BleedStatus.Bleeding : BleedStatus.NeverBled;
					break;
				case DamageType.Wrenching:
					_bleedStatus = Severity >= WoundSeverity.Horrifying ? BleedStatus.Bleeding : BleedStatus.NeverBled;
					break;
				case DamageType.Falling:
					_bleedStatus = Severity >= WoundSeverity.Grievous ? BleedStatus.Bleeding : BleedStatus.NeverBled;
					break;
				default:
					_bleedStatus = BleedStatus.NeverBled;
					break;
			}
		}

		_damageDescription = GetWoundDescription(damageType, Severity);
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public override void Register(IOutputHandler handler)
	{
		// Do nothing
	}

	public override string FrameworkItemType => "Wound";

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Wounds.Find(Id);
			if (dbitem != null)
			{
				dbitem.CurrentDamage = _currentDamage;
				dbitem.OriginalDamage = OriginalDamage;
				dbitem.CurrentPain = _currentPain;
				dbitem.CurrentStun = _currentStun;
				dbitem.LodgedItem = FMDB.Context.GameItems.Find(Lodged?.Id);
				dbitem.ExtraInformation = SaveExtras();
			}
		}

		base.Save();
	}

	public void Delete(bool ignoreDatabaseDeletion = false)
	{
		_noSave = true;
		Changed = false;
		Gameworld.SaveManager.Abort(this);
		Lodged?.Delete();
		Infection?.Delete();
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

	public bool Repairable => false;

	public BleedStatus BleedStatus
	{
		get => _bleedStatus;
		set
		{
			_bleedStatus = value;
			Changed = true;
		}
	}

	public DamageType DamageType { get; set; }

	public IInfection Infection
	{
		get => _infection;
		set
		{
			_infection = value;
			Changed = true;
		}
	}

	#region IFutureProgVariable Implementation

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	#endregion

	public static string GetWoundDescription(DamageType type, WoundSeverity severity)
	{
		switch (type)
		{
			case DamageType.Slashing:
			case DamageType.Chopping:
				if (severity <= WoundSeverity.Small)
				{
					switch (Dice.Roll(1, 6))
					{
						case 1:
							return "Nick";
						case 2:
						case 3:
							return "Cut";
						case 4:
							return "Gash";
						case 5:
							return "Laceration";
						case 6:
							return "Slash";
					}
				}

				switch (Dice.Roll(1, 6))
				{
					case 1:
					case 2:
						return "Cut";
					case 3:
					case 4:
						return "Slash";
					case 5:
						return "Laceration";
					case 6:
						return "Gash";

					default:
						return "Cut";
				}
			case DamageType.Shearing:
				switch (Dice.Roll(1, 6))
				{
					case 1:
					case 2:
						return "Tear";
					case 3:
					case 4:
						return "Cut";
					case 5:
						return "Shear";
					default:
						return "Gash";
				}
			case DamageType.Crushing:
				if (severity <= WoundSeverity.Small)
				{
					return "Bruise";
				}

				return severity <= WoundSeverity.Severe ? "Contusion" : "Crush";
			case DamageType.Falling:
				if (severity <= WoundSeverity.Small)
				{
					switch (Dice.Roll(1, 4))
					{
						case 1:
							return "Bruise";
						case 2:
							return "Sprain";
						case 3:
							return "Twist";
						case 4:
							return "Scrape";
					}
				}

				return severity <= WoundSeverity.Severe ? "Contusion" : "Crush";
			case DamageType.Piercing:
				switch (Dice.Roll(1, 5))
				{
					case 1:
						return "Perforation";
					case 2:
						return "Piercing";
					case 3:
						return "Puncture";
					case 4:
						return "Stab";
					case 5:
						return "Hole";
					default:
						return "Hole";
				}
			case DamageType.Ballistic:
			case DamageType.ArmourPiercing:
				if (severity <= WoundSeverity.Small)
				{
					return "Graze";
				}

				switch (Dice.Roll(1, 5))
				{
					case 1:
					case 2:
					case 3:
					case 4:
						return "Gunshot Wound";
					case 5:
						return "Hole";
					default:
						return "Hole";
				}
			case DamageType.Shrapnel:
				if (severity <= WoundSeverity.Small)
				{
					return "Graze";
				}

				switch (Dice.Roll(1, 5))
				{
					case 1:
					case 2:
					case 3:
					case 4:
						return "Shrapnel Wound";
					case 5:
						return "Shrapnel Hole";
					default:
						return "Shrapnel Hole";
				}
			case DamageType.Necrotic:
				return "Necrosis";
			case DamageType.Burning:
				if (severity <= WoundSeverity.Small)
				{
					return Dice.Roll(1, 2) == 1 ? "Blistering" : "Burn";
				}

				switch (Dice.Roll(1, 3))
				{
					case 1:
						return "Burn";
					case 2:
						return "Scorch";
					case 3:
						return "Sear";
					default:
						return "Burn";
				}
			case DamageType.Eldritch:
				if (severity <= WoundSeverity.Small)
				{
					return "Blistering";
				}

				switch (Dice.Roll(1, 4))
				{
					case 1:
						return "Necrotic Burn";
					case 2:
						return "Rotting Scorch";
					case 3:
						return "Disintegration Burn";
					default:
						return "Eldritch Burn";
				}
			case DamageType.Arcane:
				if (severity <= WoundSeverity.Small)
				{
					return Dice.Roll(1, 2) == 1 ? "Blistering" : "Burn";
				}

				switch (Dice.Roll(1, 3))
				{
					case 1:
						return "Burn";
					case 2:
						return "Scorch";
					case 3:
						return "Sear";
					default:
						return "Burn";
				}
			case DamageType.Freezing:
				return "Frostburn";
			case DamageType.Chemical:
				return "Chemical Burn";
			case DamageType.Shockwave:
			case DamageType.Sonic:
				return "Bruise";
			case DamageType.Bite:
				switch (Dice.Roll(1, 3))
				{
					case 1:
						return "Bite";
					case 2:
						return "Puncture";
					case 3:
						return "Tooth-Puncture";
					default:
						return "Bite";
				}
			case DamageType.Claw:
				switch (Dice.Roll(1, 3))
				{
					case 1:
						return "Gash";
					case 2:
						return "Rake";
					case 3:
						return "Claw-Gash";
					default:
						return "Gash";
				}
			case DamageType.Electrical:
				return "Burn";
			case DamageType.Hypoxia:
				return "Cyanosis";
			case DamageType.Cellular:
				return "Tissue Death";
			case DamageType.Wrenching:
				if (severity <= WoundSeverity.Small)
				{
					return Dice.Roll(1, 2) == 1 ? "Sprain" : "Twist";
				}

				return "Break";
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
		dbitem.WoundType = "SimpleOrganic";
		dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
		dbitem.GameItemId = (Parent as IGameItem)?.Id;
		dbitem.OriginalDamage = OriginalDamage;
		dbitem.CurrentDamage = _currentDamage;
		dbitem.CurrentPain = _currentPain;
		dbitem.CurrentStun = _currentStun;
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

	private void LoadFromDb(Wound wound)
	{
		_id = wound.Id;
		IdInitialised = true;
		_name = string.Empty;
		_currentDamage = wound.CurrentDamage;
		_originalDamage = wound.OriginalDamage;
		_currentPain = wound.CurrentPain;
		_currentStun = wound.CurrentStun;
		DamageType = (DamageType)wound.DamageType;
		_bodypart = Gameworld.BodypartPrototypes.Get(wound.BodypartProtoId ?? 0);
		if (wound.LodgedItemId.HasValue)
		{
			_lodged = Gameworld.TryGetItem(wound.LodgedItemId ?? 0, true);
		}

		_actorOriginId = wound.ActorOriginId ?? 0;
		_toolOriginId = wound.ToolOriginId ?? 0;

		var root = XElement.Parse(wound.ExtraInformation);
		var element = root.Element("DamageDescription");
		if (element != null)
		{
			_damageDescription = element.Value;
		}

		element = root.Element("Cleaned");
		if (element != null)
		{
			_cleaned = bool.Parse(element.Value);
		}

		element = root.Element("CleanAttempted");
		if (element != null)
		{
			_cleanAttempted = bool.Parse(element.Value);
		}

		element = root.Element("BleedStatus");
		if (element != null)
		{
			_bleedStatus = (BleedStatus)int.Parse(element.Value);
		}

		element = root.Element("Tended");
		if (element != null)
		{
			_tended = (Outcome)int.Parse(element.Value);
		}

		element = root.Element("TreatmentAttempts");
		if (element != null)
		{
			_unsuccessfulTreatmentAttempts = int.Parse(element.Value);
		}

		IsFriendlyWound = bool.Parse(root.Element("IsFriendlyWound")?.Value ?? "false");
	}

	#region Overrides of LateKeywordedInitialisingItem

	public override InitialisationPhase InitialisationPhase => InitialisationPhase.AfterFirstDatabaseHit;

	#endregion

	public string SaveExtras()
	{
		return new XElement("Definition",
			new XElement("DamageDescription", _damageDescription),
			new XElement("Cleaned", _cleaned),
			new XElement("CleanAttempted", _cleanAttempted),
			new XElement("BleedStatus", (int)BleedStatus),
			new XElement("Tended", (int)_tended),
			new XElement("TreatmentAttempts", _unsuccessfulTreatmentAttempts),
			new XElement("IsFriendlyWound", IsFriendlyWound)
		).ToString();
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

	public void SufferAdditionalDamage(IDamage damage)
	{
		OriginalDamage += damage.DamageAmount;
		CurrentDamage += damage.DamageAmount;
		_currentPain += damage.PainAmount;
		CurrentStun += damage.StunAmount;
		_cleanAttempted = false;
		_cleaned = false;
		_tended = Outcome.None;
		_unsuccessfulTreatmentAttempts = 0;
	}

	public bool UseDamagePercentageSeverities => false;

	public void OnWoundSuffered()
	{
		// Do nothing
	}

	public bool ShouldWoundBeRemoved()
	{
		return
			_currentDamage <= 0.0 &&
			_currentPain <= 0.0 &&
			_currentStun <= 0.0 &&
			_infection == null &&
			_bleedStatus != BleedStatus.Bleeding &&
			_lodged == null;
	}

	private Outcome _tended = Outcome.None;

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

	private double _currentPain;

	public double CurrentPain
	{
		//TODO - I think this approach to combining wound and infection pain might be dangerous. I believe calls to the 
		//+= operator will unintentionally add the Infection Pain into the Wound Pain each time they are called. 
		//Need to refactor.
		get => _currentPain + (_infection?.Pain ?? 0.0);
		set
		{
			_currentPain = Math.Max(0.0, value);
			Changed = true;
		}
	}

	private double _currentStun;

	public double CurrentStun
	{
		get => _currentStun;
		set
		{
			_currentStun = Math.Max(0.0, value);
			Changed = true;
		}
	}

	public double CurrentShock { get; set; } = 0;

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
			var oldDamage = _currentDamage;
			_currentDamage = Math.Max(0.0, value);
			Changed = true;
			if (oldDamage < _currentDamage && Bodypart is IOrganProto)
			{
				CheckForOrganBleeding();
			}
		}
	}

	public IBodypart SeveredBodypart { get; set; }

	public WoundSeverity Severity => Parent.GetSeverityFor(this);

	public bool Internal => Bodypart is IOrganProto;

	private IHaveWounds _parent;

	public IHaveWounds Parent
	{
		get => _parent;
		set
		{
			_parent = value;
			Changed = true;
		}
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

		switch (type)
		{
			case WoundExaminationType.Glance:
				return $"{Severity.Describe()} {_damageDescription}".A_An().ToLowerInvariant();
			case WoundExaminationType.Look:
				return
					$"{Severity.Describe()} {_damageDescription}{(BleedStatus == BleedStatus.Bleeding && CharacterParent.LongtermExertion > ExertionLevel.Stasis ? " (Bleeding)".Colour(Telnet.Red) : BleedStatus == BleedStatus.TraumaControlled ? " (Bound)".Colour(Telnet.Blue) : BleedStatus == BleedStatus.Closed ? " (Sutured)".Colour(Telnet.Green) : "")}{Infection?.WoundTag(type, outcome)}"
						.A_An().ToLowerInvariant();
			case WoundExaminationType.Self:
				return
					$"{Severity.Describe()} {_damageDescription}{(CurrentPain > CurrentDamage * 2 ? " (Painful)" : "")}{(BleedStatus == BleedStatus.Bleeding && CharacterParent.LongtermExertion > ExertionLevel.Stasis ? " (Bleeding)".Colour(Telnet.Red) : BleedStatus == BleedStatus.TraumaControlled ? " (Bound)".Colour(Telnet.Blue) : BleedStatus == BleedStatus.Closed ? " (Sutured)".Colour(Telnet.Green) : "")}{Infection?.WoundTag(type, outcome)}"
						.A_An().ToLowerInvariant();
			case WoundExaminationType.Examination:
			case WoundExaminationType.Triage:
			case WoundExaminationType.SurgicalExamination:
			case WoundExaminationType.Omniscient:
				return
					$"{Severity.Describe()} {_damageDescription}{(BleedStatus == BleedStatus.Bleeding && CharacterParent.LongtermExertion > ExertionLevel.Stasis ? " (Bleeding)".Colour(Telnet.Red) : BleedStatus == BleedStatus.TraumaControlled ? " (Bound)".Colour(Telnet.Blue) : BleedStatus == BleedStatus.Closed ? " (Sutured)".Colour(Telnet.Green) : "")}{Infection?.WoundTag(type, outcome).LeadingSpaceIfNotEmpty() ?? ""}"
						.A_An().ToLowerInvariant();
		}

		return "";
	}

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
				return Difficulty.Impossible;
			case TreatmentType.AntiInflammatory:
				return Difficulty.Impossible;
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return Difficulty.Impossible;
		}

		if ((type == TreatmentType.Clean || type == TreatmentType.Antiseptic || type == TreatmentType.Tend) &&
		    _bleedStatus == BleedStatus.Bleeding)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Close && _bleedStatus != BleedStatus.TraumaControlled)
		{
			return Difficulty.Impossible;
		}

		if ((type == TreatmentType.Clean || type == TreatmentType.Antiseptic || type == TreatmentType.Close ||
		     type == TreatmentType.Tend) &&
		    Lodged != null)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Trauma && _bleedStatus != BleedStatus.Bleeding)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Clean && _cleaned)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Antiseptic && _cleaned &&
		    ch.Body.AffectedBy<IAntisepticTreatmentEffect>(Bodypart))
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Clean || type == TreatmentType.Antiseptic ||
		    type == TreatmentType.AntiInflammatory)
		{
			return Difficulty.VeryEasy;
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

		if (type == TreatmentType.Trauma)
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
				return "That kind of treatment cannot be applied to that wound.";
			case TreatmentType.AntiInflammatory:
				return "That kind of treatment is not yet implemented.";
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return "There is nothing in that wound that requires removal.";
		}

		if ((type == TreatmentType.Clean || type == TreatmentType.Antiseptic || type == TreatmentType.Tend) &&
		    _bleedStatus == BleedStatus.Bleeding)
		{
			return "Before a wound can be cleaned or tended to, the bleeding must have stopped";
		}

		if (type == TreatmentType.Close && _bleedStatus != BleedStatus.TraumaControlled)
		{
			return "Only wounds that have been stabilised can be sutured closed.";
		}

		if ((type == TreatmentType.Clean || type == TreatmentType.Antiseptic || type == TreatmentType.Close ||
		     type == TreatmentType.Tend) &&
		    Lodged != null)
		{
			return
				"Foreign objects must be removed from the wound before it can be successfully cleaned, tended or closed.";
		}

		if (type == TreatmentType.Trauma && _bleedStatus == BleedStatus.NeverBled)
		{
			return "There is no bleeding trauma on that wound to address.";
		}

		if (type == TreatmentType.Clean && _cleaned)
		{
			return "That wound is already as clean as it's going to get with conventional cleaning.";
		}

		if (type == TreatmentType.Antiseptic && _cleaned &&
		    ch.Body.AffectedBy<IAntisepticTreatmentEffect>(Bodypart) == true)
		{
			return "That wound is both clean and antiseptically treated, and cannot benefit from another treatment.";
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

				if (_bleedStatus == BleedStatus.Bleeding)
				{
					_bleedStatus = BleedStatus.TraumaControlled;
					if (!silent)
					{
						Parent.OutputHandler.Handle(
							$"{Describe(WoundExaminationType.Glance, Outcome.MajorPass)} has stopped bleeding.",
							OutputRange.Local);
					}
				}

				return;
			case TreatmentType.Trauma:
				if (testOutcome == Outcome.MajorFail || (treatmentItem == null && testOutcome.IsFail()))
				{
					if (treater == null || silent)
					{
						return;
					}

					treater.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"Despite $0's efforts, #0 are|is unable to stop {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} from bleeding.",
						treater, treater)));
					return;
				}

				if (treater != null && !silent)
				{
					if (treatmentItem == null)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts have stopped the bleeding from {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}
					else
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts with $1 have stopped the bleeding from {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater, treatmentItem.Parent)));
					}
				}

				treatmentItem?.UseTreatment();
				_bleedStatus = BleedStatus.TraumaControlled;
				Changed = true;
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
			case TreatmentType.Clean:
			case TreatmentType.Antiseptic:
				_cleaned = _cleaned || testOutcome.IsPass() ||
				           (treatmentItem != null && testOutcome != Outcome.MajorFail);
				_cleanAttempted = true;
				if (_cleaned && type == TreatmentType.Antiseptic)
				{
					if (ch.Body.AffectedBy<IAntisepticTreatmentEffect>(Bodypart))
					{
						ch.Body.Reschedule(
							ch.Body.EffectsOfType<IAntisepticTreatmentEffect>().First(x => x.Bodypart == Bodypart),
							TimeSpan.FromSeconds((testOutcome.SuccessDegrees() + 2) * 1200));
					}
					else
					{
						ch.Body.AddEffect(new AntisepticProtection(ch.Body, Bodypart, null),
							TimeSpan.FromSeconds((testOutcome.SuccessDegrees() + 2) * 1200));
					}
				}

				Changed = true;
				if (treater != null && !silent)
				{
					if (treatmentItem == null)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0 have|has finished cleaning {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}
					else
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0 have|has finished cleaning {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} with $1.",
							treater, treater, treatmentItem.Parent)));
					}
				}

				treatmentItem?.UseTreatment();
				return;
			case TreatmentType.Close:
				if (testOutcome == Outcome.MajorFail || (treatmentItem == null && testOutcome.IsFail()))
				{
					if (treater != null && !silent)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"Despite $0's efforts, #0 are|is unable to close up {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}

					return;
				}

				if (treater != null && !silent)
				{
					if (treatmentItem == null)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts have closed {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}
					else
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts with $1 have closed {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater, treatmentItem.Parent)));
					}
				}

				treatmentItem?.UseTreatment();
				_bleedStatus = BleedStatus.Closed;
				Changed = true;
				break;
		}
	}

	public BleedResult Bleed(double currentBloodLitres, ExertionLevel activityExertionLevel,
		double totalBloodLitres)
	{
		if (_bleedStatus == BleedStatus.NeverBled || !(_parent is ICharacter ch))
		{
			return BleedResult.NoBleed;
		}

		if (currentBloodLitres / totalBloodLitres < 0.25)
		{
			currentBloodLitres = 0.25 * totalBloodLitres;
		}

		switch (_bleedStatus)
		{
			case BleedStatus.Bleeding:
				if (ch.Body.BloodLiquid == null)
				{
					_bleedStatus = BleedStatus.NeverBled;
					Changed = true;
					return BleedResult.NoBleed;
				}

				var beingBounds = Parent.EffectsOfType<BeingBound>().Where(x => x.Bodypart == Bodypart).ToList();
				var bleedPercentage = Math.Max(0, (int)Severity + (int)activityExertionLevel - 4) *
				                      PercentageExternalBloodlossPerWoundSeverity *
				                      Bodypart.BleedModifier *
				                      (beingBounds.Any() ? 0.5 : 1);
				var bleeding = bleedPercentage * currentBloodLitres;
				if (bleeding > 0)
				{
					var items = ch.Body.WornItemsProfilesFor(Bodypart);
					var item = ch.Body.WornItemsFor(Bodypart).FirstOrDefault();
					var mixture = new LiquidMixture(new BloodLiquidInstance(ch, bleeding), Gameworld);
					item?.ExposeToLiquid(mixture, Bodypart, LiquidExposureDirection.FromUnderneath);
					if (!mixture.IsEmpty)
					{
						var binders = beingBounds
						                        .Select(x => (x.Binder, x.Binder.Body.HoldLocs))
						                        .ToList();
						var bindingPartCount = binders.Sum(x => x.HoldLocs.Count());
						if (bindingPartCount > 0)
						{
							var mixtures = new Queue<LiquidMixture>(mixture.Split(bindingPartCount));
							foreach (var (binder, locs) in binders)
							{
								foreach (var loc in locs)
								{
									binder.Body.ExposeToLiquid(mixtures.Dequeue(), loc, LiquidExposureDirection.Irrelevant);
								}
							}
						}
						else
						{
							PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, ch.Location, ch.RoomLayer, ch);
						}
					}

					if (items.All(x => x.Item2.Transparent))
					{
						return new BleedResult
						{
							BloodAmount = bleeding,
							CoverItem = null,
							Visible = true,
							Bodypart = Bodypart
						};
					}

					if (items.All(x => x.Item1.SaturationLevel > ItemSaturationLevel.Wet))
					{
						return new BleedResult
						{
							BloodAmount = bleeding,
							CoverItem = items.Last().Item1,
							Visible = true,
							Bodypart = Bodypart
						};
					}

					return new BleedResult { BloodAmount = bleeding, Visible = false, Bodypart = Bodypart };
				}

				return BleedResult.NoBleed;
			case BleedStatus.TraumaControlled:
				if (activityExertionLevel > ExertionLevel.Normal &&
				    Dice.Roll(1, 100) < ((int)activityExertionLevel + (int)Severity - 2) * 2)
				{
					_bleedStatus = BleedStatus.Bleeding;
					_cleaned = false;
					Parent.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								string.Format(
									"$0's exertion has caused {0} on &0's {1} to start bleeding!".Colour(Telnet.Red),
									Describe(WoundExaminationType.Glance, Outcome.MajorPass),
									Bodypart.FullDescription()), (IPerceiver)Parent, Parent)));
					Changed = true; //Moved this into the scope to prevent unnecessary saves
				}

				return BleedResult.NoBleed;
			case BleedStatus.Closed:
				if (activityExertionLevel > ExertionLevel.Heavy &&
				    Dice.Roll(1, 100) < (int)activityExertionLevel + (int)Severity - 5)
				{
					_bleedStatus = BleedStatus.TraumaControlled;
					Parent.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								string.Format("$0's exertion has reopened {0} on &0's {1}!".Colour(Telnet.Red),
									Describe(WoundExaminationType.Glance, Outcome.MajorPass),
									Bodypart.FullDescription()), (IPerceiver)Parent, Parent)));
				}

				Changed = true;
				return BleedResult.NoBleed;
		}

		return BleedResult.NoBleed;
	}

	public double PeekBleed(double bloodTotal, ExertionLevel activityExertionLevel)
	{
		if (_bleedStatus == BleedStatus.Bleeding)
		{
			return Math.Max(0, (int)Severity + (int)activityExertionLevel - 4) *
			       PercentageExternalBloodlossPerWoundSeverity * Bodypart.BleedModifier *
			       bloodTotal;
		}

		return 0;
	}

	public double Exert(ExertionType exertion)
	{
		return 0;
	}

	public bool EligableForInfection()
	{
		if (Infection != null || !(_parent is ICharacter ch))
		{
			return false;
		}

		switch (DamageType)
		{
			case DamageType.Crushing:
			case DamageType.Electrical:
			case DamageType.Shockwave:
				return false;
		}

		//If the wound has healed up 50% from its original level, let's never give it an infection
		if (CurrentDamage <= OriginalDamage * 0.5)
		{
			return false;
		}

		if (Severity == WoundSeverity.Superficial)
		{
			return false;
		}

		return ch.Body.EffectsOfType<IAntisepticTreatmentEffect>().All(x => x.Bodypart != Bodypart);
	}

	/// <summary>
	///     Called in the health heartbeat to determine whether an uninfected wound becomes infected, and also to trigger
	///     subsequent processing if there is an existing infection
	/// </summary>
	private void CheckInfection()
	{
		if (!(_parent is ICharacter ch))
		{
			return;
		}

		Infection?.InfectionTick();
		if (Infection?.InfectionHealed() == true)
		{
			Infection.Delete();
			Infection = null;
			return;
		}

		if (!EligableForInfection())
		{
			return;
		}

		var chance = Gameworld.GetStaticDouble("BaseInfectionChance");
		switch (DamageType)
		{
			case DamageType.Ballistic:
				chance *= 0.8;
				break;
			case DamageType.Bite:
				chance *= 20;
				break;
			case DamageType.Burning:
				chance *= 10;
				break;
			case DamageType.Chemical:
			case DamageType.Chopping:
				chance *= 1.1;
				break;
			case DamageType.Claw:
				chance *= 8;
				break;
			case DamageType.Freezing:
			case DamageType.Piercing:
			case DamageType.Slashing:
				chance *= 1.2;
				break;
			case DamageType.Hypoxia:
			case DamageType.Cellular:
				chance *= 7.0;
				break;
		}

		chance *= ch.CurrentProject.Labour?.LabourImpacts.OfType<ILabourImpactHealing>()
		            .Aggregate(1.0, (sum, x) => sum * x.InfectionChanceMultiplier) ?? 1.0;

		var terrain = ch.Location.Terrain(ch);
		chance *= terrain.InfectionMultiplier;

		switch (Severity)
		{
			case WoundSeverity.Horrifying:
				chance *= 10;
				break;
			case WoundSeverity.Grievous:
				chance *= 7;
				break;
			case WoundSeverity.VerySevere:
				chance *= 4;
				break;
			case WoundSeverity.Severe:
				chance *= 2;
				break;
			case WoundSeverity.Moderate:
				chance *= 1.4;
				break;
		}

		if (BleedStatus == BleedStatus.TraumaControlled ||
		    BleedStatus == BleedStatus.Bleeding)
		{
			chance *= 2; // 2x the infection chance for not suturing wound
		}

		if (RandomUtilities.DoubleRandom(0.0, 1.0) > chance)
		{
			return;
		}

		if (_cleaned && Dice.Roll(1, 100) > 5)
		{
#if DEBUG
			Console.WriteLine($"Infection for {ch.HowSeen(ch, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} " +
			                  $"on {Describe(WoundExaminationType.Look, Outcome.MajorPass)} stopped by clean wound.");
#endif
			ch.Send(
				$"You feel as if {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} on your {Bodypart.FullDescription()} could benefit from a clean.");
			_cleaned = false;
			Changed = true;
			return;
		}

		if (_cleanAttempted && Dice.Roll(1, 100) > 75)
		{
#if DEBUG
			Console.WriteLine(
				$"Infection for {ch.HowSeen(ch, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} on {Describe(WoundExaminationType.Look, Outcome.MajorPass)} stopped by clean attempt.");
#endif
			ch.Send(
				$"You feel as if {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} on your {Bodypart.FullDescription()} could benefit from a clean.");
			_cleaned = false;
			Changed = true;
			return;
		}

		var virulence =
			ch.Merits.OfType<IInfectionResistanceMerit>()
			  .Where(x => x.Applies(ch))
			  .Select(x => x.GetNewInfectionDifficulty(terrain.InfectionVirulence, terrain.PrimaryInfection))
			  .DefaultIfEmpty(terrain.InfectionVirulence)
			  .Min();
#if DEBUG
		if (virulence != terrain.InfectionVirulence)
		{
			Console.WriteLine(
				$"Infection Virulance Changed by Merits - Original {terrain.InfectionVirulence.Describe()} New {virulence.Describe()}.");
		}
#endif
		Infection = Infections.Infection.LoadNewInfection(terrain.PrimaryInfection, virulence, 0.0001, ch.Body, this,
			Bodypart, terrain.InfectionMultiplier);
		Changed = true;
#if DEBUG
		Console.WriteLine(
			$"{ch.HowSeen(ch, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} on {Describe(WoundExaminationType.Look, Outcome.MajorPass)} has become infected with {terrain.PrimaryInfection} {terrain.InfectionVirulence}.");
#endif
	}

	public void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus)
	{
		if (_bleedStatus == BleedStatus.Bleeding || Lodged != null || !(_parent is ICharacter ch))
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

		if (_infection != null)
		{
			return;
		}

		if (_bleedStatus == BleedStatus.TraumaControlled)
		{
			var traumaCheck = Gameworld.GetCheck(CheckType.WoundCloseCheck);
			if (traumaCheck.TargetNumber(ch, CanBeTreated(TreatmentType.Close), null,
				    externalBonus: externalCheckBonus) * timePassed.TotalMinutes >= 50.0)
			{
				_bleedStatus = BleedStatus.Closed;
				Changed = true;
				timePassed = TimeSpan.FromTicks(timePassed.Ticks / 2);
			}
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

		if (_currentPain > 0)
		{
			var painCheck = Gameworld.GetCheck(CheckType.PainRecoveryCheck);
			var amount =
				Math.Max(0, Parent.HealthStrategy.GetHealingTickAmount(this, Outcome.MinorPass, HealthDamageType.Pain) *
				            amountModifier * Math.Max(0.01,
					            painCheck.TargetNumber(ch, difficulty, null, externalBonus: externalCheckBonus) *
					            0.01) *
				            timePassed.TotalMinutes);
			// Pain is never less than half current damage
			_currentPain = Math.Max(CurrentDamage * Bodypart.PainModifier / 2.0, _currentPain - amount);
			Changed = true;
		}
	}

	public bool HealingTick(double externalRateMultiplier, double externalCheckBonus)
	{
		if (!(_parent is ICharacter ch))
		{
			return false;
		}

		if (_bleedStatus == BleedStatus.Bleeding)
		{
			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealBleeding,
				Outcome.NotTested);
			CheckInfection();
			return false;
		}

		CheckInfection();

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

		// Healing types other than stun and blood recovery don't occur with infected wounds
		if (_infection != null)
		{
			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealInfected,
				Outcome.NotTested);
			return false;
		}

		// Trauma wounds don't heal, but they may close on their own
		if (_bleedStatus == BleedStatus.TraumaControlled)
		{
			var traumaCheck = Gameworld.GetCheck(CheckType.WoundCloseCheck);
			if (traumaCheck.Check(ch, CanBeTreated(TreatmentType.Close)).IsPass())
			{
				// TODO - highly likely to get infected if healed this way
				_bleedStatus = BleedStatus.Closed;
				Changed = true;
				Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
					CurrentDamage, CurrentPain, CurrentStun, DamageType,
					WoundHealingTickResult.NoHealNotSuturedAutoClosed, Outcome.NotTested);
				return true;
			}

			Gameworld.LogManager.CustomLogEntry(LogEntryType.HealingTick, Parent, Severity, OriginalDamage,
				CurrentDamage, CurrentPain, CurrentStun, DamageType, WoundHealingTickResult.NoHealNotSutured,
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

		if (_currentPain > 0)
		{
			var painCheck = Gameworld.GetCheck(CheckType.PainRecoveryCheck);
			result = painCheck.Check(ch, Difficulty.Normal, externalBonus: externalCheckBonus);
			double painhealing = 0;
			if (result.IsPass())
			{
				painhealing = Parent.HealthStrategy.GetHealingTickAmount(this, result, HealthDamageType.Pain) *
				              amountModifer;
			}

			// Pain is never less than half current damage
			_currentPain = Math.Max(CurrentDamage * Bodypart.PainModifier / 2.0, _currentPain - painhealing);
			Changed = true;
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

			if (CurrentPain > CurrentDamage * 2)
			{
				difficulty = difficulty.StageUp(1);
			}

			return difficulty;
		}
	}

	public bool IsFriendlyWound { get; protected set; }

	#endregion
}