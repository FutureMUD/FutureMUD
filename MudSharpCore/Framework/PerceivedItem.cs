using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using JetBrains.Annotations;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Shape;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using GameItem = MudSharp.Models.GameItem;

namespace MudSharp.Framework;

// TODO - this shouldn't be here?
public class Plane : KeywordedItem
{
	public override string FrameworkItemType => "Plane";
}

public abstract class PerceivedItem : LateKeywordedInitialisingItem, IPerceivable
{
	//private bool _effectsChanged;

	protected PerceivedItem()
	{
		_hookedFunctions = _installedHooks.ToLookup(x => x.Type, x => x.Function);
		EffectHandler = new EffectHandler(this);
	}

	protected PerceivedItem(long id)
		: this()
	{
		_id = id;
		IdInitialised = true;
	}

	public bool IsSingleEntity => true;

	protected string _shortDescription;
	protected string _fullDescription;
	protected IEntityDescriptionPattern _shortDescriptionPattern;
	protected IEntityDescriptionPattern _fullDescriptionPattern;

	public virtual IEffectHandler EffectHandler { get; protected init; }

	public virtual bool IsSelf(IPerceivable other)
	{
		return other == this;
	}

	public virtual ICell Location { get; protected set; }

	public virtual event LocatableEvent OnLocationChanged;
	public virtual event LocatableEvent OnLocationChangedIntentionally;
	public virtual bool Sentient => false;

	public virtual void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false)
	{
		Location = location;
		OnLocationChanged?.Invoke(this, exit);
	}

	public virtual bool IdentityIsObscured => false;

	public virtual bool IdentityIsObscuredTo(ICharacter observer)
	{
		return false;
	}

	public virtual string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		var cansee = voyeur?.CanSee(this, flags) ?? true;

		switch (type)
		{
			case DescriptionType.Short:
				var output = cansee ? (_shortDescriptionPattern?.Pattern ?? _shortDescription).FluentProper(proper) :
					proper ? "Something" : "something";
				return colour ? output.ColourObject() : output;
			case DescriptionType.Possessive:
				return HowSeen(voyeur, proper, DescriptionType.Short, colour) + "'s";
			case DescriptionType.Long:
				return HowSeen(voyeur, proper, DescriptionType.Short, colour) + " is " +
					   DescribePosition(voyeur).Fullstop();
			case DescriptionType.Full:
				return cansee
					? (_fullDescriptionPattern?.Pattern ?? _fullDescription).FluentProper(proper)
					: "You cannot discern anything more about this.";
			default:
				throw new ArgumentException("Not a valid description type");
		}
	}

	public virtual Gendering ApparentGender(IPerceiver voyeur)
	{
		return Neuter.Instance;
	}

	public virtual IOutputHandler OutputHandler { get; protected set; }

	public virtual double IlluminationProvided => 0;

		public virtual SizeCategory Size => SizeCategory.Normal;

		public abstract void Register(IOutputHandler handler);

	public virtual Proximity GetProximity(IPerceivable target)
	{
		if (target == null)
		{
			return Proximity.Unapproximable;
		}

		if (IsSelf(target))
		{
			return Proximity.Intimate;
		}

		if (target is IGameItem gi)
		{
			var giinv = gi.InInventoryOf;
			if (giinv == this)
			{
				return Proximity.Intimate;
			}

			if (giinv != null)
			{
				return GetProximity(giinv.Actor);
			}

			var giic = gi.ContainedIn;
			if (giic == this)
			{
				return Proximity.Intimate;
			}

			if (giic != null)
			{
				return GetProximity(giic);
			}
		}

		if (target.PositionTarget == this || PositionTarget == target)
		{
			return Proximity.Immediate;
		}

		return Location == target.Location ? Proximity.Distant : Proximity.Unapproximable;
	}

	public virtual IEnumerable<IWound> ExplosionEmantingFromPerceivable(IExplosiveDamage damage)
	{
		if (Location == null)
		{
			return Enumerable.Empty<IWound>();
		}

		var wounds = new List<IWound>();
		var things = LocalThingsAndProximities().GroupBy(x => x.Proximity, x => x.Thing);
		foreach (var proximity in things)
		{
			if (proximity.Key > damage.MaximumProximity)
			{
				continue;
			}

			foreach (var thing in proximity)
			{
				if (!(thing is IHaveWounds ihw))
				{
					continue;
				}

				wounds.AddRange(ihw.PassiveSufferDamage(damage, proximity.Key,
					(Body.Facing)RandomUtilities.Random(0, 3))); // TODO - non random facing?
			}
		}

		return wounds;
	}

	#region ISaveable Members

	public override void Save()
	{
		Changed = false;
	}

	#endregion

	public XElement SaveEffects()
	{
		var scheduledEffects = new Dictionary<IEffect, TimeSpan>();
		if (_cachedEffects.Any())
		{
			scheduledEffects = _cachedEffects.ToDictionary(x => x.Effect, x => x.Time);
		}
		else
		{
			scheduledEffects = Effects.Where(x => Gameworld.EffectScheduler.IsScheduled(x))
									  .ToDictionary(x => x, x => Gameworld.EffectScheduler.RemainingDuration(x));
		}

		return new XElement("Effects",
			from effect in Effects.Where(x => x.SavingEffect).ToList()
			select effect.SaveToXml(scheduledEffects)
		);
	}

	#region IEquatable<IPerceivable> Members

	public bool Equals(IPerceivable other)
	{
		if (other == null)
		{
			return false;
		}

		return other.Id == Id && FrameworkItemType.Equals(other.FrameworkItemType);
	}

	#endregion

	protected void LoadHooks(IEnumerable<HooksPerceivable> hooks, string perceiverType)
	{
		foreach (var hook in hooks)
		{
			var ihook = Gameworld.Hooks.Get(hook.HookId);
			if (ihook != null)
			{
				InstallHook(ihook);
			}
		}

		var defaultHooks = Gameworld.DefaultHooks.Where(x => x.Applies(this, perceiverType)).ToList();
		foreach (var hook in defaultHooks)
		{
			InstallHook(hook.Hook);
			HooksChanged = true;
		}
	}

	private readonly List<(IEffect Effect, TimeSpan Time)> _cachedEffects = new();

	protected void ScheduleCachedEffects()
	{
		foreach (var (effect, timespan) in _cachedEffects)
		{
			EffectHandler.Reschedule(effect, timespan);
			effect.Login();
		}

		_cachedEffects.Clear();
	}

	protected void CacheScheduledEffects()
	{
		_cachedEffects.Clear();
		foreach (var effect in Effects.Distinct().ToList())
		{
			if (!Gameworld.EffectScheduler.IsScheduled(effect))
			{
				continue;
			}

			_cachedEffects.Add((effect, EffectHandler.ScheduledDuration(effect)));
			Gameworld.EffectScheduler.Unschedule(effect);
		}
	}

	protected void LoadEffects(XElement effects)
	{
		var removedAnyEffects = false;
		foreach (var effect in effects.Elements("Effect"))
		{
			var newEffect = Effect.LoadEffect(effect, this);
			if (newEffect.LoadErrors)
			{
				removedAnyEffects = true;
				continue;
			}

			var remaining = double.Parse(effect.Element("Remaining").Value);
			if (remaining > 0)
			{
				_cachedEffects.Add((newEffect, TimeSpan.FromMilliseconds(remaining)));
			}

			AddEffect(newEffect);
		}

		if (removedAnyEffects)
		{
			EffectsChanged = true;
		}
	}

	private bool _effectsChanged;

	public bool EffectsChanged
	{
		get => _effectsChanged;
		set
		{
			_effectsChanged = value;
			if (value)
			{
				Changed = true;
			}
		}
	}

	#region IPositionable

	private IPositionState _positionState;

	public virtual IPositionState PositionState
	{
		get => _positionState;
		set
		{
			if (value != _positionState)
			{
				PositionChanged = true;
			}

			_positionState = value;
		}
	}

	private IPerceivable _positionTarget;

	public virtual IPerceivable PositionTarget
	{
		get => _positionTarget;
		set => SetTarget(value);
	}

	private PositionModifier _positionModifier = PositionModifier.None;

	public virtual PositionModifier PositionModifier
	{
		get => _positionModifier;
		set
		{
			if (value != _positionModifier)
			{
				PositionChanged = true;
			}

			_positionModifier = value;
		}
	}

	private IEmote _positionEmote;

	public virtual IEmote PositionEmote
	{
		get => _positionEmote;
		protected set
		{
			if (value != _positionEmote)
			{
				PositionChanged = true;
			}

			_positionEmote = value;
		}
	}

	protected readonly List<IPerceivable> _targetedBy = new();
	public virtual IEnumerable<IPerceivable> TargetedBy => _targetedBy;

	public virtual void SetTargetedBy(IPerceivable targeter)
	{
		if (!_targetedBy.Contains(targeter))
		{
			_targetedBy.Add(targeter);
		}
	}

	public virtual void RemoveTargetedBy(IPerceivable targeter)
	{
		_targetedBy.Remove(targeter);
	}

	public string DescribePosition(IPerceiver voyeur, bool useHere = true)
	{
		return PositionState?.Describe(voyeur, PositionTarget, PositionModifier, PositionEmote, useHere) ?? (useHere ? "here" : "");
	}

	public event PerceivableEvent OnPositionChanged;

	protected void PositionHasChanged()
	{
		OnPositionChanged?.Invoke(this);
	}

	public virtual IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities()
	{
		return Enumerable.Empty<(IPerceivable, Proximity)>();
	}

	public bool InVicinity(IPerceivable target)
	{
		if (IsSelf(target))
		{
			return false;
		}

		var outwardsList = new List<IPerceivable>();
		var inwardsList = new List<IPerceivable>();
		return InternalInVicinity(this, target, outwardsList, inwardsList) ||
			   InternalInVicinity(target, this, inwardsList, outwardsList);
	}

	protected virtual bool InternalInVicinity(IPerceivable origin, IPerceivable target,
		List<IPerceivable> currentPath, List<IPerceivable> previous)
	{
		if (currentPath.Contains(origin))
		{
			return false;
		}

		if (origin.IsSelf(target) || previous.Any(x => x.IsSelf(origin)))
		{
			return true;
		}

		currentPath.Add(origin);
		var table = (origin as IGameItem)?.GetItemType<ITable>();
		if (table != null)
		{
			return table.Chairs.Any(x => InternalInVicinity(x.Parent, target, currentPath, previous)) ||
				   (origin.PositionTarget != null &&
					InternalInVicinity(origin.PositionTarget, target, currentPath, previous));
		}

		var chair = (origin as IGameItem)?.GetItemType<IChair>();
		if (chair != null)
		{
			return (chair.Table != null && InternalInVicinity(chair.Table.Parent, target, currentPath, previous)) ||
				   (origin.PositionTarget != null &&
					InternalInVicinity(origin.PositionTarget, target, currentPath, previous));
		}

		return origin.PositionTarget != null &&
			   InternalInVicinity(origin.PositionTarget, target, currentPath, previous);
	}

	public void SetPosition(IPositionState state, PositionModifier modifier, IPerceivable target, IEmote emote)
	{
		SetState(state);
		SetModifier(modifier);
		SetTarget(target);
		SetEmote(emote);
		PositionHasChanged();
	}

	public virtual void SetEmote(IEmote emote)
	{
		if (PositionEmote != null)
		{
			foreach (var target in PositionEmote.Targets)
			{
				target.InvalidPositionTargets -= Emote_InvalidPositionTargets;
			}
		}

		if (emote != null)
		{
			foreach (var target in emote.Targets)
			{
				target.InvalidPositionTargets += Emote_InvalidPositionTargets;
			}
		}

		PositionEmote = emote;
		PositionChanged = true;
	}

	private void Emote_InvalidPositionTargets(object sender, EventArgs e)
	{
		SetEmote(null);
	}

	public void SetState(IPositionState state)
	{
		PositionState = state;
		PositionChanged = true;
	}

	private void Target_InvalidPositionTargets(object sender, EventArgs e)
	{
		((IPerceivable)sender).InvalidPositionTargets -= Target_InvalidPositionTargets;
		PositionModifier = PositionModifier.None;
		PositionTarget = null;
	}


	public void SetTarget(IPerceivable target)
	{
		if (PositionTarget != null)
		{
			PositionTarget.InvalidPositionTargets -= Target_InvalidPositionTargets;
			PositionTarget.RemoveTargetedBy(this);
		}

		_positionTarget = target;
		if (target != null)
		{
			_positionTarget.SetTargetedBy(this);
			_positionTarget.InvalidPositionTargets += Target_InvalidPositionTargets;
		}

		PositionChanged = true;
	}

	/// <summary>
	/// This function is called to release events on the targeted, for example when something goes into stasis and logs out
	/// </summary>
	protected void SoftReleasePositionTarget()
	{
		if (PositionTarget != null)
		{
			PositionTarget.InvalidPositionTargets -= Target_InvalidPositionTargets;
			PositionTarget.RemoveTargetedBy(this);
		}
	}

	public void SetModifier(PositionModifier modifier)
	{
		PositionModifier = modifier;
		PositionChanged = true;
	}

	public virtual IEnumerable<IPositionState> ValidPositions { get; private set; }

	public virtual bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
	{
		return true;
	}

	public event EventHandler InvalidPositionTargets;

	protected void InvalidatePositionTargets()
	{
		InvalidPositionTargets?.Invoke(this, new EventArgs());
	}

	protected virtual void ReleaseEvents()
	{
		OnQuit = null;
		OnDeleted = null;
		OnPositionChanged = null;
	}

	public event PerceivableEvent OnQuit;

	protected void PerceivableQuit()
	{
		OnQuit?.Invoke(this);
		ReleaseEvents();
	}

	public event PerceivableEvent OnDeleted;

	protected void PerceivableDeleted()
	{
		OnDeleted?.Invoke(this);
		ReleaseEvents();
	}

	private bool _positionChanged;

	public bool PositionChanged
	{
		get => _positionChanged;
		protected set
		{
			if (value && !_positionChanged && !Changed && !_noSave)
			{
				Changed = true;
			}

			_positionChanged = value;
		}
	}

	protected void SavePosition(MudSharp.Models.Character character)
	{
		character.PositionId = (int)PositionState.Id;
		character.PositionModifier = (int)PositionModifier;
		character.PositionEmote = PositionEmote?.SaveToXml().ToString() ?? "";
		character.PositionTargetId = PositionTarget?.Id;
		character.PositionTargetType = PositionTarget?.FrameworkItemType;
		_positionChanged = false;
	}

	protected void SavePosition(GameItem item)
	{
		item.PositionId = (int)PositionState.Id;
		item.PositionModifier = (int)PositionModifier;
		item.PositionEmote = PositionEmote?.SaveToXml().ToString() ?? "";
		item.PositionTargetId = PositionTarget?.Id;
		item.PositionTargetType = PositionTarget?.FrameworkItemType;
		_positionChanged = false;
	}

	protected virtual IPositionState DefaultPosition => PositionUndefined.Instance;

	public void LoadPosition(int positionState, int modifier, string stremote, long? targetid, string targettype)
	{
		var removeNoSave = false;
		if (!_noSave)
		{
			_noSave = true;
			removeNoSave = true;
		}

		_noSave = true;
		PositionState = Body.Position.PositionState.GetState(positionState);
		PositionModifier = (PositionModifier)modifier;
		if (targetid.HasValue)
		{
			var target = Gameworld.GetPerceivable(targettype, targetid.Value);
			if (target?.CanBePositionedAgainst(PositionState, PositionModifier) == false)
			{
				SetTarget(null);
				SetModifier(PositionModifier.None);
			}
			else
			{
				SetTarget(target);
			}
		}

		if (!string.IsNullOrEmpty(stremote))
		{
			var emote = Emote.LoadEmote(XElement.Parse(stremote), Gameworld, this as IPerceiver);
			if (emote.Valid)
			{
				SetEmote(emote);
			}
		}

		if (removeNoSave)
		{
			_noSave = false;
		}
	}

	#endregion

	#region IHandleEvents Members

	private bool _hooksChanged;

	public bool HooksChanged
	{
		get => _hooksChanged;
		set
		{
			_hooksChanged = value;
			if (value)
			{
				Changed = true;
			}
		}
	}

	protected readonly List<IHook> _installedHooks = new();
	protected ILookup<EventType, Func<EventType, object[], bool>> _hookedFunctions;

	public IEnumerable<IHook> Hooks => _installedHooks;

	public virtual bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		// Hooks cannot have exclusive "right to fire". Even if one hook handles the event, we want others to keep handling it.
		return _hookedFunctions[type].Select(x => x(type, arguments)).Any(x => x);
	}

	public virtual bool HandlesEvent(params EventType[] types)
	{
		return types.Any(x => _installedHooks.Any(y => y.Type == x));
	}

	private static readonly EventType[] SubscribingEvents =
	{
		EventType.FiveSecondTick,
		EventType.TenSecondTick,
		EventType.MinuteTick,
		EventType.HourTick
	};

	protected void FiveSecondHeartbeat()
	{
		HandleEvent(EventType.FiveSecondTick, this);
	}

	protected void TenSecondHeartbeat()
	{
		HandleEvent(EventType.TenSecondTick, this);
	}

	protected void MinuteHeartbeat()
	{
		HandleEvent(EventType.MinuteTick, this);
	}

	protected void HourHeartbeat()
	{
		HandleEvent(EventType.HourTick, this);
	}

	public bool InstallHook(IHook hook)
	{
		if (_installedHooks.Contains(hook))
		{
			return false;
		}

		_installedHooks.Add(hook);
		_hookedFunctions = _installedHooks.ToLookup(x => x.Type, x => x.Function);
		foreach (var type in SubscribingEvents)
		{
			if (hook.Type == type)
			{
				switch (type)
				{
					case EventType.FiveSecondTick:
						Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
						Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += FiveSecondHeartbeat;
						break;
					case EventType.TenSecondTick:
						Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
						Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondHeartbeat;
						break;
					case EventType.MinuteTick:
						Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
						Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += MinuteHeartbeat;
						break;
					case EventType.HourTick:
						Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
						Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HourHeartbeat;
						break;
				}
			}
		}

		return true;
	}

	public bool RemoveHook(IHook hook)
	{
		if (!_installedHooks.Contains(hook))
		{
			return false;
		}

		_installedHooks.Remove(hook);
		_hookedFunctions = _installedHooks.ToLookup(x => x.Type, x => x.Function);
		foreach (var type in SubscribingEvents)
		{
			if (type != hook.Type)
			{
				continue;
			}

			if (_installedHooks.Any(x => x.Type == type) ||
				((this as INPC)?.AIs.Any(x => x.HandlesEvent(type)) ?? false))
			{
				continue;
			}

			switch (type)
			{
				case EventType.FiveSecondTick:
					Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
					break;
				case EventType.TenSecondTick:
					Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
					break;
				case EventType.MinuteTick:
					Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
					break;
				case EventType.HourTick:
					Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
					break;
			}
		}

		return true;
	}

	#endregion

	#region IHaveEffects Members

	public virtual IEnumerable<IEffect> Effects => EffectHandler.Effects;

	public virtual IEnumerable<T> EffectsOfType<T>(Predicate<T> predicate = null) where T : class, IEffect
	{
		return EffectHandler.EffectsOfType<T>(predicate);
	}

	public virtual bool AffectedBy<T>() where T : class, IEffect
	{
		return EffectHandler.AffectedBy<T>();
	}

	public virtual bool AffectedBy<T>(object target) where T : class, IEffect
	{
		return EffectHandler.AffectedBy<T>(target);
	}

	public virtual bool AffectedBy<T>(object target, object thirdparty) where T : class, IEffect
	{
		return EffectHandler.AffectedBy<T>(target, thirdparty);
	}

	public void AddEffect(IEffect effect)
	{
		EffectHandler.AddEffect(effect);
	}

	public void AddEffect(IEffect effect, TimeSpan duration)
	{
		if (duration.Ticks < 0)
		{
			EffectHandler.AddEffect(effect);
			return;
		}

		EffectHandler.AddEffect(effect, duration);
	}

	public void RemoveEffect(IEffect effect, bool fireRemovalEffect = false)
	{
		if (effect == null)
		{
			return;
		}

		EffectHandler.RemoveEffect(effect, fireRemovalEffect);
	}

	public virtual void RemoveAllEffects()
	{
		EffectHandler.RemoveAllEffects();
	}

	public virtual void RemoveAllEffects(Predicate<IEffect> predicate, bool fireRemovalAction = false)
	{
		EffectHandler.RemoveAllEffects(predicate, fireRemovalAction);
	}

	public TimeSpan ScheduledDuration(IEffect effect)
	{
		return EffectHandler.ScheduledDuration(effect);
	}

	public virtual bool RemoveAllEffects<T>([CanBeNull] Predicate<T> predicate = null, bool fireRemovalAction = false)
		where T : IEffect
	{
		return EffectHandler.RemoveAllEffects(predicate, fireRemovalAction);
	}

	public void Reschedule(IEffect effect, TimeSpan newDuration)
	{
		EffectHandler.Reschedule(effect, newDuration);
	}

	public void RescheduleIfLonger(IEffect effect, TimeSpan newDuration)
	{
		EffectHandler.RescheduleIfLonger(effect, newDuration);
	}

	public void AddDuration(IEffect effect, TimeSpan addedDuration)
	{
		EffectHandler.AddDuration(effect, addedDuration);
	}

	public void RemoveDuration(IEffect effect, TimeSpan removedDuration, bool fireRemovalActionIfRemoved = false)
	{
		EffectHandler.RemoveDuration(effect, removedDuration, fireRemovalActionIfRemoved);
	}

	public PerceptionTypes GetPerception(PerceptionTypes type)
	{
		return EffectHandler.GetPerception(type);
	}

	public bool HiddenFromPerception(PerceptionTypes type, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return EffectHandler.HiddenFromPerception(type, flags);
	}

	public bool HiddenFromPerception(IPerceiver voyeur, PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return EffectHandler.HiddenFromPerception(voyeur, type, flags);
	}

	#endregion

	#region IFutureProgVariable Implementation

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "effects", ProgVariableTypes.Effect | ProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "type", "Returns the name of the framework item type, for example, character or gameitem or clan" },
			{ "effects", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Perceivable, DotReferenceHandler(),
			DotReferenceHelp());
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Perceiver, DotReferenceHandler(),
			DotReferenceHelp());
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.MagicResourceHaver,
			DotReferenceHandler(), DotReferenceHelp());
	}

	public virtual IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "type":
				return new TextVariable(FrameworkItemType);
			case "effects":
				return new CollectionVariable(EffectHandler.Effects.Where(x => x.Applies()).ToList(),
					ProgVariableTypes.Effect);
			default:
				throw new NotSupportedException(
					$"Unsupported property type {property} in {FrameworkItemType}.GetProperty");
		}
	}

	public abstract ProgVariableTypes Type { get; }

	public object GetObject => this;

	#endregion
}