using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Framework;

public class DummyPerceiver : DummyPerceivable, IPerceiver
{
	public DummyPerceiver(string sdesc = "a thing", string fdesc = "it is a thing", ICell location = null,
		bool sentient = false, double illumination = 0) : base(sdesc, fdesc, location, sentient, illumination)
	{
	}

	public int LineFormatLength => 120;
	public int InnerLineFormatLength => 80;
	public ICellOverlayPackage CurrentOverlayPackage { get; set; }
	public PerceptionTypes NaturalPerceptionTypes { get; } = PerceptionTypes.All;
	public bool BriefCombatMode { get; set; }

	public new RoomLayer RoomLayer { get; set; } = RoomLayer.GroundLevel;

	public bool ColocatedWith(IPerceiver otherThing)
	{
		return true;
	}

	public bool CanHear(IPerceivable thing)
	{
		return true;
	}

	public bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
	{
		return true;
	}

	public bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return true;
	}

	public double VisionPercentage => 1.0;

	public bool CanSmell(IPerceivable thing)
	{
		return true;
	}

	public bool IsPersonOfInterest(IPerceivable thing)
	{
		return false;
	}

	public virtual IPerceivable Target(string keyword)
	{
		return
			Location.Characters
			        .Cast<IPerceivable>()
			        .Concat(Location.GameItems)
			        .GetFromItemListByKeyword(keyword, this);
	}

	public virtual IPerceivable TargetLocal(string keyword)
	{
		return
			Location.Characters
			        .Cast<IPerceivable>()
			        .Concat(Location.GameItems)
			        .GetFromItemListByKeyword(keyword, this);
	}

	public virtual ICharacter TargetActor(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		return Location.Characters.GetFromItemListByKeyword(keyword, this);
	}

	public virtual ICharacter TargetAlly(string keyword)
	{
		return TargetActor(keyword);
	}

	public virtual ICharacter TargetNonAlly(string keyword)
	{
		return TargetActor(keyword);
	}

	public virtual IBody TargetBody(string keyword)
	{
		return Location.Characters.Select(x => x.Body).GetFromItemListByKeyword(keyword, this);
	}

	public virtual IGameItem TargetItem(string keyword)
	{
		return TargetLocalItem(keyword);
	}

	public virtual IGameItem TargetLocalItem(string keyword)
	{
		var targetExit = Location.GetExit(keyword, "", this);
		if (targetExit?.Exit.Door != null)
		{
			return targetExit.Exit.Door.Parent;
		}

		return Location.GameItems.GetFromItemListByKeyword(keyword, this);
	}

	public virtual IGameItem TargetLocalOrHeldItem(string keyword)
	{
		return TargetLocalItem(keyword);
	}

	public virtual IGameItem TargetPersonalItem(string keyword)
	{
		throw new NotImplementedException();
	}

	public virtual IGameItem TargetHeldItem(string keyword)
	{
		throw new NotImplementedException();
	}

	public virtual IGameItem TargetWornItem(string keyword)
	{
		throw new NotImplementedException();
	}

	public virtual IGameItem TargetTopLevelWornItem(string keyword)
	{
		throw new NotImplementedException();
	}

	public virtual ICharacter TargetActorOrCorpse(string keyword,
		PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		throw new NotImplementedException();
	}

	public virtual (ICharacter Target, IEnumerable<ICellExit> Path) TargetDistantActor(string keyword,
		ICellExit initialExit, uint maximumRange,
		bool respectDoors, bool respectCorners)
	{
		var permittedDirections = initialExit == null
			? Constants.CardinalDirections
			: Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
		var target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
			                 initialExit?.OutboundDirection ?? CardinalDirection.Unknown)
		                 .SelectMany(x => x.Characters)
		                 .GetFromItemListByKeyword(keyword, this);
		return (target, this.PathBetween(target, maximumRange, false, false, respectDoors));
	}

	public virtual (IGameItem Target, IEnumerable<ICellExit> Path) TargetDistantItem(string keyword,
		ICellExit initialExit, uint maximumRange,
		bool respectDoors, bool respectCorners)
	{
		var permittedDirections = initialExit == null
			? Constants.CardinalDirections
			: Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
		var target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
			                 initialExit?.OutboundDirection ?? CardinalDirection.Unknown)
		                 .SelectMany(x => x.GameItems)
		                 .GetFromItemListByKeyword(keyword, this);
		return (target, this.PathBetween(target, maximumRange, false, false, respectDoors));
	}

	public IList<IDub> Dubs { get; } = new List<IDub>();

	public bool HasDubFor(IKeyworded target, IEnumerable<string> keywords)
	{
		return false;
	}

	public bool HasDubFor(IKeyworded target, string keyword)
	{
		return false;
	}

	public object GetFormat(Type formatType)
	{
		return CultureInfo.InvariantCulture.GetFormat(formatType);
	}

	public IAccount Account { get; set; } = DummyAccount.Instance;

	#region Implementation of ICombatant

	private ICombat _combat;

	public ICombat Combat
	{
		get => _combat;
		set
		{
			if (_combat != null && value == null)
			{
				PerceiverLeaveCombat();
			}

			_combat = value;
			if (_combat != null)
			{
				PerceiverJoinCombat();
			}
		}
	}

	private IPerceiver _combatTarget;

	public virtual IPerceiver CombatTarget
	{
		get => _combatTarget;
		set
		{
			if (_combatTarget != value)
			{
				//Aim?.ReleaseEvents();
				//Aim = null;
				RemoveAllEffects(x => x.IsEffectType<ICombatEffectRemovedOnTargetChange>());
				if (TargettedBodypart != null &&
				    !(value is ICharacter tch && tch.Body.Bodyparts.Contains(TargettedBodypart)))
				{
					TargettedBodypart = null;
				}
			}

			_combatTarget = value;
		}
	}

	public virtual double DefensiveAdvantage { get; set; }
	public virtual double OffensiveAdvantage { get; set; }

	public double GetBonusForDefendersFromTargeting()
	{
		return 0.0;
	}

	public double GetDefensiveAdvantagePenaltyFromTargeting()
	{
		return 0.0;
	}

	private IAimInformation _aim;

	public IAimInformation Aim
	{
		get => _aim;
		set
		{
			if (_aim != null)
			{
				_aim.AimInvalidated -= Aim_AimInvalidated;
				_aim.ReleaseEvents();
			}

			_aim = value;
			if (_aim != null)
			{
				_aim.AimInvalidated -= Aim_AimInvalidated;
				_aim.AimInvalidated += Aim_AimInvalidated;
			}
		}
	}

	private void Aim_AimInvalidated(object sender, EventArgs e)
	{
		var aim = (IAimInformation)sender;
		HandleEvent(EventType.LostAim, this, aim.Target, aim.Weapon.Parent);
		Aim = null;
	}

	public IBodypart TargettedBodypart { get; set; }

	public DefenseType PreferredDefenseType { get; set; }
	public CombatStrategyMode CombatStrategyMode { get; set; }

	public ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant)
	{
		return null;
	}

	public bool CheckCombatStatus()
	{
		return true;
	}

	public void AcquireTarget()
	{
		// Do nothing
	}

	public ICombatMove ChooseMove()
	{
		return null;
	}

	public ItemQuality NaturalWeaponQuality(INaturalAttack attack)
	{
		return ItemQuality.Standard;
	}

	public bool CanTruce()
	{
		return true;
	}

	public string WhyCannotTruce()
	{
		throw new ApplicationException(
			"Perceiver without overriden version of WhyCannotTruce asked to provide a reason why they could not truce.");
	}

	public Facing GetFacingFor(ICombatant opponent, bool reset = false)
	{
		return Facing.Front;
	}

	public bool MeleeRange { get; set; }

	/// <summary>
	/// This property calculates whether a combatant is in melee range with their target or is themselves engaged in melee by another
	/// </summary>
	public bool IsEngagedInMelee => MeleeRange ||
	                                (Combat?.Combatants.Except(this).Any(x => x.CombatTarget == this && x.MeleeRange) ??
	                                 false);

	public bool CanEngage(IPerceiver target)
	{
		return false;
	}

	public string WhyCannotEngage(IPerceiver target)
	{
		return $"You cannot engage {target.HowSeen(this)} in combat.";
	}

	public bool Engage(IPerceiver target, bool ranged)
	{
		return false;
	}

	public event PerceivableEvent OnJoinCombat;

	protected void PerceiverJoinCombat()
	{
		OnJoinCombat?.Invoke(this);
	}

	public event PerceivableEvent OnEngagedInMelee;

	protected void PerceiverEngagedInMelee()
	{
		OnEngagedInMelee?.Invoke(this);
	}

	public event PerceivableEvent OnLeaveCombat;

	protected void PerceiverLeaveCombat()
	{
		OnLeaveCombat?.Invoke(this);
	}

	public ICombatantCover Cover { get; set; }

	public bool TakeOrQueueCombatAction(ISelectedCombatAction action)
	{
		return false;
	}

	public ICharacterCombatSettings CombatSettings
	{
		get => null;
		set { }
	}

	public bool ShouldFall()
	{
		return false;
	}

	public void FallToGround()
	{
		// Do nothing
	}

	public bool FallOneLayer(ref double distance)
	{
		return true;
	}

	public bool CouldTransitionToLayer(RoomLayer layer)
	{
		return false;
	}

	#endregion
}