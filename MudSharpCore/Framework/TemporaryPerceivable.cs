using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Framework;

/// <summary>
/// An item implementing IPerceivable but lasting for a very short time, with no real implementation of the game heavy stuff
/// </summary>
public abstract class TemporaryPerceivable : FrameworkItem, IPerceivable
{
	public virtual bool IsSingleEntity => true;

	public abstract bool Sentient { get; }

	public abstract double IlluminationProvided { get; }

	/// <inheritdoc />
	public SizeCategory Size => SizeCategory.Normal;

	public RoomLayer RoomLayer { get; set; } = RoomLayer.GroundLevel;

	public XElement SaveEffects()
	{
		return new XElement("Effects");
	}

	public bool EffectsChanged { get; set; }

	public abstract string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

	public virtual Gendering ApparentGender(IPerceiver voyeur)
	{
		return Gendering.Get(Form.Shape.Gender.Indeterminate);
	}

	public virtual bool IdentityIsObscured => false;

	public virtual bool IdentityIsObscuredTo(ICharacter observer)
	{
		return false;
	}

	public virtual bool IsSelf(IPerceivable other)
	{
		return false;
	}

	public void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false)
	{
		// Do nothing
	}

	public event EventHandler InvalidPositionTargets;

	public virtual Proximity GetProximity(IPerceivable thing)
	{
		return Proximity.Unapproximable;
	}

	public event PerceivableEvent OnQuit;
	public event PerceivableEvent OnDeleted;

	public Gendering Gender => Gendering.Get(Form.Shape.Gender.Indeterminate);

	public abstract ICell Location { get; }

	public event LocatableEvent OnLocationChanged;
	public event LocatableEvent OnLocationChangedIntentionally;
	public virtual IEnumerable<string> Keywords => Enumerable.Empty<string>();

	public virtual IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Enumerable.Empty<string>();
	}

	public virtual bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
	{
		return false;
	}

	public virtual bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
	{
		return false;
	}

	public IEnumerable<IEffect> Effects => Enumerable.Empty<IEffect>();

	IEnumerable<T> IHaveEffects.EffectsOfType<T>(Predicate<T> predicate)
	{
		return Enumerable.Empty<T>();
	}

	bool IHaveEffects.AffectedBy<T>()
	{
		return false;
	}

	bool IHaveEffects.AffectedBy<T>(object target)
	{
		return false;
	}

	bool IHaveEffects.AffectedBy<T>(object target, object thirdparty)
	{
		return false;
	}

	public void AddEffect(IEffect effect)
	{
		// Do nothing
	}

	public void AddEffect(IEffect effect, TimeSpan duration)
	{
		// Do nothing
	}

	public void RemoveEffect(IEffect effect, bool fireRemovalEffect = false)
	{
		// Do nothing
	}

	public void RemoveAllEffects()
	{
		// Do nothing
	}

	public void RemoveAllEffects(Predicate<IEffect> predicate, bool fireRemovalAction = false)
	{
		// Do nothing
	}

	public bool RemoveAllEffects<T>(Predicate<T> predicate = null, bool fireRemovalAction = false) where T : IEffect
	{
		return false;
	}

	public void Reschedule(IEffect effect, TimeSpan newDuration)
	{
		// Do nothing
	}

	public void RescheduleIfLonger(IEffect effect, TimeSpan newDuration)
	{
		// Do nothing
	}

	public TimeSpan ScheduledDuration(IEffect effect)
	{
		return TimeSpan.Zero;
	}

	public void AddDuration(IEffect effect, TimeSpan addedDuration)
	{
		// Do nothing
	}

	public void RemoveDuration(IEffect effect, TimeSpan removedDuration, bool fireRemovalActionIfRemoved = false)
	{
		// Do nothing
	}

	public virtual PerceptionTypes GetPerception(PerceptionTypes type)
	{
		return PerceptionTypes.All;
	}

	public virtual bool HiddenFromPerception(PerceptionTypes type, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return false;
	}

	public virtual bool HiddenFromPerception(IPerceiver voyeur, PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return false;
	}

	public IPositionState PositionState
	{
		get => PositionUndefined.Instance;
		set { }
	}

	public IPerceivable PositionTarget
	{
		get => null;
		set { }
	}

	public PositionModifier PositionModifier
	{
		get => PositionModifier.None;
		set { }
	}

	public IEmote PositionEmote => null;

	public IEnumerable<IPerceivable> TargetedBy => Enumerable.Empty<IPerceivable>();

	public IEnumerable<IPositionState> ValidPositions => Enumerable.Empty<IPositionState>();

	public string DescribePosition(IPerceiver voyeur)
	{
		return HowSeen(voyeur);
	}

	public virtual bool InVicinity(IPerceivable target)
	{
		return false;
	}

	public virtual IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities()
	{
		return Enumerable.Empty<(IPerceivable Thing, Proximity Proximity)>();
	}

	public void SetPosition(IPositionState state, PositionModifier modifier, IPerceivable target, IEmote emote)
	{
		// Do nothing
	}

	public void SetState(IPositionState state)
	{
		// Do nothing
	}

	public void SetTarget(IPerceivable target)
	{
		// Do nothing
	}

	public void SetModifier(PositionModifier modifier)
	{
		// Do nothing
	}

	public void SetEmote(IEmote emote)
	{
		// Do nothing
	}

	public void SetTargetedBy(IPerceivable targeter)
	{
		// Do nothing
	}

	public void RemoveTargetedBy(IPerceivable targeter)
	{
		// Do nothing
	}

	public bool CanBePositionedAgainst(IPositionState whichPosition, PositionModifier modifier)
	{
		return false;
	}

	public event PerceivableEvent OnPositionChanged;

	public IOutputHandler OutputHandler { get; } = new NonPlayerOutputHandler();

	public void Register(IOutputHandler handler)
	{
		// Do nothing
	}

	public virtual bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		return false;
	}

	public virtual bool HandlesEvent(params EventType[] types)
	{
		return false;
	}

	public bool InstallHook(IHook hook)
	{
		return false;
	}

	public bool RemoveHook(IHook hook)
	{
		return false;
	}

	public bool HooksChanged
	{
		get => false;
		set { }
	}

	public IEnumerable<IHook> Hooks => Enumerable.Empty<IHook>();

	public bool Changed
	{
		get => false;
		set { }
	}

	public void Save()
	{
		// Do nothing
	}

	public IFuturemud Gameworld => Futuremud.Games.First();

	public bool Equals(IPerceivable other)
	{
		return false;
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Perceivable;

	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
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
				return new CollectionVariable(Enumerable.Empty<IEffect>().ToList(),
					FutureProgVariableTypes.Effect);
			default:
				throw new NotSupportedException(
					$"Unsupported property type {property} in {FrameworkItemType}.GetProperty");
		}
	}

	public IEnumerable<IWound> ExplosionEmantingFromPerceivable(IExplosiveDamage damage)
	{
		return Enumerable.Empty<IWound>();
	}
}