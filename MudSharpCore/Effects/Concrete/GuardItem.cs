using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class GuardItem : Effect, IGuardItemEffect, ILDescSuffixEffect, IAffectProximity
{
	public GuardItem(ICharacter owner, IGameItem target, bool includeVicinity) : base(owner)
	{
		TargetItem = target;
		IncludeVicinity = includeVicinity;
		CharacterOwner = owner;
		RegisterEvents();
	}

	protected GuardItem(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	public override void Login()
	{
		RegisterEvents();
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetItem == thing || (IncludeVicinity && TargetItem.InVicinity(thing)))
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	public ICharacter CharacterOwner { get; set; }

	protected override string SpecificEffectType => "GuardItem";
	public bool IncludeVicinity { get; set; }

	public IGameItem TargetItem { get; set; }

	public bool ShouldRemove(IAffectedByChangeInGuarding newEffect)
	{
		if (newEffect == this)
		{
			return false;
		}

		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Guarding {TargetItem.HowSeen(voyeur)}{(IncludeVicinity ? " and all in its vicinity" : "")}.";
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}

	public override bool SavingEffect => true;

	public override void RemovalEffect()
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ are|is no longer guarding $0.", CharacterOwner, TargetItem),
			flags: OutputFlags.SuppressObscured));
		ReleaseEvents();
	}

	private void LoadFromXml(XElement root)
	{
		IncludeVicinity = bool.Parse(root.Element("Vicinity").Value);
		var id = long.Parse(root.Element("Id").Value);
		CharacterOwner = Owner as ICharacter;
		TargetItem = Owner.Location.LayerGameItems(CharacterOwner.RoomLayer).FirstOrDefault(x => x.Id == id);
		LoadErrors = TargetItem == null || CharacterOwner == null;
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XElement("Id", TargetItem.Id), new XElement("Vicinity", IncludeVicinity));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("GuardItem", (effect, owner) => new GuardItem(effect, owner));
	}

	private void RegisterEvents()
	{
		TargetItem.OnRemovedFromLocation += InvalidateEffect;
		TargetItem.OnQuit += InvalidateEffect;
		TargetItem.OnDeleted += InvalidateEffect;
		TargetItem.OnDeath += InvalidateEffect;
		CharacterOwner.OnWantsToMove += CharacterOwner_OnWantsToMove;
		CharacterOwner.OnMoved += CharacterOwner_OnMoved;
		CharacterOwner.OnEngagedInMelee += InvalidateEffect;
		CharacterOwner.OnDeath += InvalidateEffect;
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
	}

	private void CharacterOwner_OnStateChanged(IPerceivable owner)
	{
		if (CharacterOwner.State.IsDisabled())
		{
			InvalidateEffect(CharacterOwner);
			return;
		}
	}

	private void CharacterOwner_OnMoved(object sender, MoveEventArgs e)
	{
		InvalidateEffect(CharacterOwner);
	}

	private void CharacterOwner_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		response.Rejected = true;
		response.Reason = $"You cannot move while you are guarding {TargetItem.HowSeen(CharacterOwner)}.";
	}

	private void ReleaseEvents()
	{
		TargetItem.OnRemovedFromLocation -= InvalidateEffect;
		TargetItem.OnQuit -= InvalidateEffect;
		TargetItem.OnDeleted -= InvalidateEffect;
		TargetItem.OnDeath -= InvalidateEffect;
		CharacterOwner.OnWantsToMove -= CharacterOwner_OnWantsToMove;
		CharacterOwner.OnMoved -= CharacterOwner_OnMoved;
		CharacterOwner.OnEngagedInMelee -= InvalidateEffect;
		CharacterOwner.OnDeath -= InvalidateEffect;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
	}

	private void InvalidateEffect(IPerceivable owner)
	{
		ReleaseEvents();
		Owner.RemoveEffect(this);
	}

	public string SuffixFor(IPerceiver voyeur)
	{
		return $"guarding {TargetItem.HowSeen(voyeur)}";
	}

	public bool SuffixApplies()
	{
		return true;
	}
}