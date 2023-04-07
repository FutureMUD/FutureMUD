using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Health.Wounds;

public class RobotWound : PerceivedItem, IWound
{
	public RobotWound(IHaveWounds owner, Models.Wound wound, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_parent = owner;
		LoadFromDb(wound);
	}

	public RobotWound(IFuturemud gameworld, IHaveWounds owner, double damage, double stun, DamageType damageType,
		IBodypart bodypart, IGameItem lodged, IGameItem toolOrigin, ICharacter actorOrigin)
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
		_currentStun = Math.Max(0.0, stun * bodypart.StunModifier);
		DamageType = damageType;
		Bodypart = bodypart;
		Lodged = lodged;
		_toolOriginId = toolOrigin?.Id ?? 0;
		_actorOriginId = actorOrigin?.Id ?? 0;
		if (actorOrigin?.Combat?.Friendly == true)
		{
			IsFriendlyWound = true;
		}

		if (Bodypart is IOrganProto organ)
		{
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

		Gameworld.SaveManager.AddInitialisation(this);
	}

	public override void Register(IOutputHandler handler)
	{
		// Do nothing
	}

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	public override object DatabaseInsert()
	{
		if (Parent.Id == 0)
		{
			return null;
		}

		var dbitem = new Models.Wound();
		FMDB.Context.Wounds.Add(dbitem);
		dbitem.WoundType = "Robot";
		dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
		dbitem.GameItemId = (Parent as IGameItem)?.Id;
		dbitem.OriginalDamage = OriginalDamage;
		dbitem.CurrentDamage = CurrentDamage;
		dbitem.CurrentStun = CurrentStun;
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
		_id = ((Models.Wound)dbitem)?.Id ?? 0;
	}

	public override InitialisationPhase InitialisationPhase => InitialisationPhase.AfterFirstDatabaseHit;

	private void LoadFromDb(Models.Wound wound)
	{
		_id = wound.Id;
		IdInitialised = true;
		_name = string.Empty;
		_currentDamage = wound.CurrentDamage;
		_originalDamage = wound.OriginalDamage;
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
		var element = root.Element("BleedStatus");
		if (element != null)
		{
			_bleedStatus = (BleedStatus)int.Parse(element.Value);
		}

		element = root.Element("TreatmentAttempts");
		if (element != null)
		{
			_unsuccessfulTreatmentAttempts = int.Parse(element.Value);
		}

		IsFriendlyWound = bool.Parse(root.Element("IsFriendlyWound")?.Value ?? "false");
	}

	public string SaveExtras()
	{
		return new XElement("Definition",
			new XElement("BleedStatus", (int)BleedStatus),
			new XElement("TreatmentAttempts", _unsuccessfulTreatmentAttempts),
			new XElement("IsFriendlyWound", IsFriendlyWound)
		).ToString();
	}

	public override string FrameworkItemType => "Wound";

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
		return
			_currentDamage <= 0.0 &&
			_currentStun <= 0.0 &&
			_bleedStatus != BleedStatus.Bleeding &&
			_lodged == null;
	}

	private BleedStatus _bleedStatus;

	public BleedStatus BleedStatus
	{
		get => _bleedStatus;
		set
		{
			_bleedStatus = value;
			Changed = true;
		}
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

	public bool Internal
	{
		get => _internal;
		set
		{
			_internal = value;
			Changed = true;
		}
	}

	private IHaveWounds _parent;

	public IHaveWounds Parent
	{
		get => _parent;
		set => _parent = value;
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

	public DamageType DamageType { get; set; }

	private int _unsuccessfulTreatmentAttempts;

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
		if (!type.In(TreatmentType.Mend, TreatmentType.Repair, TreatmentType.Trauma, TreatmentType.Close,
			    TreatmentType.Remove))
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Close && _bleedStatus != BleedStatus.TraumaControlled)
		{
			return Difficulty.Impossible;
		}

		if (type == TreatmentType.Trauma && _bleedStatus != BleedStatus.Bleeding)
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
		if (!type.In(TreatmentType.Mend, TreatmentType.Repair, TreatmentType.Trauma, TreatmentType.Close,
			    TreatmentType.Remove))
		{
			return "That kind of treatment cannot be applied to that wound.";
		}

		if (type == TreatmentType.Remove && Lodged == null)
		{
			return "There is nothing in that wound that requires removal.";
		}

		if (type == TreatmentType.Close && _bleedStatus != BleedStatus.TraumaControlled)
		{
			return "The fluid loss must be arrested before the leak can be permanently sealed.";
		}

		if (type == TreatmentType.Trauma && _bleedStatus != BleedStatus.Bleeding)
		{
			return "There is no fluid leakage from that wound to control.";
		}

		throw new ApplicationException("Got to the end of RobotWound.WhyCannotTreat");
	}

	public void Treat(IPerceiver treater, TreatmentType type, ITreatment treatmentItem, Outcome testOutcome,
		bool silent)
	{
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
					CurrentStun = Math.Min(CurrentStun, CurrentDamage);
				}

				if (_bleedStatus == BleedStatus.Bleeding)
				{
					_bleedStatus = BleedStatus.TraumaControlled;
					if (!silent)
					{
						Parent.OutputHandler.Handle(
							$"{Describe(WoundExaminationType.Glance, Outcome.MajorPass)} has stopped leaking {CharacterParent.Race.BloodLiquid.Name}.",
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
						$"Despite $0's efforts, #0 are|is unable to stop {Describe(WoundExaminationType.Glance, Outcome.MajorPass)} from leaking {CharacterParent.Race.BloodLiquid.Name}.",
						treater, treater)));
					return;
				}

				if (treater != null && !silent)
				{
					if (treatmentItem == null)
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts have stopped the loss of {CharacterParent.Race.BloodLiquid.Name} from {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater)));
					}
					else
					{
						treater.OutputHandler.Handle(new EmoteOutput(new Emote(
							$"$0's efforts with $1 have stopped the loss of {CharacterParent.Race.BloodLiquid.Name} from {Describe(WoundExaminationType.Glance, Outcome.MajorPass)}.",
							treater, treater, treatmentItem.Parent)));
					}
				}

				treatmentItem?.UseTreatment();
				_bleedStatus = BleedStatus.TraumaControlled;
				Changed = true;
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
				_bleedStatus = BleedStatus.NeverBled;
				Changed = true;
				break;
			case TreatmentType.Repair:
				if (testOutcome.IsFail() && testOutcome != Outcome.MinorFail)
				{
					_unsuccessfulTreatmentAttempts++;
					Changed = true;
					if (treater != null && !silent)
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
					CurrentStun = Math.Min(CurrentStun, CurrentDamage);
				}

				break;
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
					                                                        "PercentageExternalFluidLossPerWoundSeverityRobot");
			}

			return _percentageExternalBloodlossPerWoundSeverity;
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
				var bleedPercentage = Math.Max(0, (int)Severity + (int)activityExertionLevel - 4) *
				                      PercentageExternalBloodlossPerWoundSeverity *
				                      Bodypart.BleedModifier *
				                      (Parent.EffectsOfType<BeingBound>().Any(x => x.Bodypart == Bodypart) ? 0.5 : 1);
				var bleeding = bleedPercentage * currentBloodLitres;
				if (bleeding > 0)
				{
					var items = ch.Body.WornItemsProfilesFor(Bodypart);
					var item = ch.Body.WornItemsFor(Bodypart).FirstOrDefault();
					var mixture = new LiquidMixture(new BloodLiquidInstance(ch, bleeding), Gameworld);
					item?.ExposeToLiquid(mixture, Bodypart, LiquidExposureDirection.FromUnderneath);
					if (!mixture.IsEmpty)
					{
						var bindingItems = Parent.EffectsOfType<BeingBound>().Where(x => x.Bodypart == Bodypart)
						                         .SelectMany(x => x.Binder.Body.HoldLocs.OfType<IWear>()
						                                           .SelectNotNull(y =>
							                                           x.Binder.Body.WornItemsFor(y).LastOrDefault()))
						                         .ToList();
						foreach (var bi in bindingItems)
						{
							bi.ExposeToLiquid(mixture, null, LiquidExposureDirection.FromOnTop);
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
					Parent.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								string.Format(
									$"$0's exertion has caused {0} on &0's {1} to start leaking {CharacterParent.Race.BloodLiquid.Name}!"
										.Colour(Telnet.Red),
									Describe(WoundExaminationType.Glance, Outcome.MajorPass),
									Bodypart.FullDescription()), (IPerceiver)Parent, Parent)));
					Changed = true;
				}

				return BleedResult.NoBleed;
			case BleedStatus.Closed:
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
		return 0.0;
	}

	public bool HealingTick(double externalRateMultiplier, double externalCheckBonus)
	{
		return false;
	}

	public bool EligableForInfection()
	{
		return false;
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

	public void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus)
	{
		// Do nothing
	}

	public bool Repairable => true;

	public IInfection Infection
	{
		get => null;
		set { }
	}

	public Difficulty ConcentrationDifficulty => Difficulty.Automatic;
	public bool IsFriendlyWound { get; protected set; }
}