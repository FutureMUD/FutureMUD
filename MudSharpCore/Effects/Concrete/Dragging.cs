using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;

public class Dragging : Effect, IDragging
{
	public class DragHelper : Effect, ILDescSuffixEffect, IDragParticipant
	{
		public override bool IsBlockingEffect(string blockingType)
		{
			return string.IsNullOrEmpty(blockingType) || blockingType.EqualToAny("general", "movement", "drag");
		}

		public override IEnumerable<string> Blocks => new[] { "general", "movement", "drag" };

		public override string BlockingDescription(string blockingType, IPerceiver voyeur)
		{
			return $"dragging {Drag.Target.HowSeen(voyeur)}";
		}

		public override bool CanBeStoppedByPlayer => true;

		public IDragging Drag { get; set; }
		public IDragging TheDrag => Drag;

		public DragHelper(ICharacter owner, IDragging drag) : base(owner)
		{
			Drag = drag;
		}

		protected override string SpecificEffectType => "DragHelper";

		public override string Describe(IPerceiver voyeur)
		{
			return $"Helping {Drag.Owner.HowSeen(voyeur)} to drag {Drag.Target.HowSeen(voyeur)}.";
		}

		#region Implementation of ILDescSuffixEffect

		public string SuffixFor(IPerceiver voyeur)
		{
			return $"helping to drag {Drag.Target.HowSeen(voyeur)}.";
		}

		public bool SuffixApplies()
		{
			return true;
		}

		#endregion
	}

	public class DragTarget : Effect, ILDescSuffixEffect, IDragParticipant, IRemoveOnGet
	{
		public override bool IsBlockingEffect(string blockingType)
		{
			return string.IsNullOrEmpty(blockingType) || blockingType.EqualToAny("general", "movement", "drag");
		}

		public override IEnumerable<string> Blocks => new[] { "general", "movement", "drag" };

		public override string BlockingDescription(string blockingType, IPerceiver voyeur)
		{
			return $"being dragged";
		}

		public IDragging Drag { get; set; }
		public IDragging TheDrag => Drag;

		public DragTarget(IPerceivable owner, IDragging drag) : base(owner)
		{
			Drag = drag;
		}

		protected override string SpecificEffectType => "DragTarget";

		public override string Describe(IPerceiver voyeur)
		{
			return $"Being dragged by {Drag.Owner.HowSeen(voyeur)}.";
		}

		#region Implementation of ILDescSuffixEffect

		public string SuffixFor(IPerceiver voyeur)
		{
			return $"being dragged by {Drag.Owner.HowSeen(voyeur)}.";
		}

		public bool SuffixApplies()
		{
			return true;
		}

		#endregion

		#region Overrides of Effect

		/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
		public override void RemovalEffect()
		{
			base.RemovalEffect();
			TheDrag.Owner.RemoveEffect(TheDrag, true);
		}

		public override bool IsEffectType<T>(object target)
		{
			return
				base.IsEffectType<T>(target) &&
				(Drag.CharacterDraggers.Contains(target) ||
				 (target is ILegalAuthority la &&
				  Drag.CharacterDraggers.Any(x => la.GetEnforcementAuthority(x) is not null))
				);
		}

		#endregion
	}

	public IDragging TheDrag => this;

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || blockingType.EqualToAny("general", "drag");
	}

	public override IEnumerable<string> Blocks => new[] { "general", "drag" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return $"dragging {Target.HowSeen(voyeur)}";
	}

	public override bool CanBeStoppedByPlayer => true;

	public IPerceivable Target { get; set; }

	public ICharacter CharacterOwner { get; set; }

	private readonly List<ICharacter> _helpers = new();

	public IEnumerable<ICharacter> Helpers => _helpers;

	private readonly List<Dragger> _draggers = new();

	public IEnumerable<Dragger> Draggers => _draggers;

	public IEnumerable<ICharacter> CharacterDraggers => _draggers.Select(x => x.Character).ToList();

	#region Helpers

	public void AddHelper(ICharacter actor, IDragAid aid)
	{
		if (!_helpers.Contains(actor))
		{
			_helpers.Add(actor);
			_draggers.Add(new Dragger { Character = actor, Aid = aid });
			actor.OnDeath += Helper_Ineligable;
			actor.OnQuit += Helper_Ineligable;
			actor.OnStateChanged += Helper_StateChanged;
			actor.OnDeleted += Helper_Ineligable;
			actor.OnEngagedInMelee += Helper_Ineligable;
			actor.OnPositionChanged += Helper_PositionChanged;
			actor.OnMoved += Helper_Moved;
			actor.AddEffect(new DragHelper(actor, this));
		}
	}

	private void Helper_Moved(object sender, MoveEventArgs e)
	{
		if (e.Movement is DragMovement)
		{
			return;
		}

		RemoveHelper(e.Mover as ICharacter);
		((ICharacter)e.Mover).OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ are|is no longer helping drag $1 because #0 is not in the same location.", e.Mover, e.Mover, Target)));
	}

	private void ReleaseHelperEvents(ICharacter helper)
	{
		helper.OnDeath -= Helper_Ineligable;
		helper.OnQuit -= Helper_Ineligable;
		helper.OnStateChanged -= Helper_StateChanged;
		helper.OnDeleted -= Helper_Ineligable;
		helper.OnEngagedInMelee -= Helper_Ineligable;
		helper.OnPositionChanged -= Helper_PositionChanged;
		helper.OnMoved -= Helper_Moved;
	}

	private void Helper_PositionChanged(IPerceivable owner)
	{
		var actor = (ICharacter)owner;
		if (!actor.CanMove(true))
		{
			RemoveHelper(actor);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is no longer helping drag $1 because #0 are|is {actor.PositionState.Describe(actor, null, PositionModifier.None, null, false)}.",
				actor, actor, Target)));
		}
	}

	private void Helper_StateChanged(IPerceivable owner)
	{
		var actor = (ICharacter)owner;
		if (!CharacterState.Able.HasFlag(actor.State))
		{
			RemoveHelper(actor);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is no longer helping drag $1 because #0 are|is {actor.State.Describe()}.", actor, actor,
				Target)));
		}
	}

	private void Helper_Ineligable(IPerceivable owner)
	{
		RemoveHelper((ICharacter)owner);
	}

	public void RemoveHelper(ICharacter actor)
	{
		_helpers.Remove(actor);
		_draggers.RemoveAll(x => x.Character == actor);
		ReleaseHelperEvents(actor);
		actor.RemoveAllEffects(x => x.IsEffectType<DragHelper>());
	}

	#endregion

	#region Dragger

	public void RegisterDraggerEvents()
	{
		CharacterOwner.OnDeath += Dragger_Ineligable;
		CharacterOwner.OnQuit += Dragger_Ineligable;
		CharacterOwner.OnDeleted += Dragger_Ineligable;
		CharacterOwner.OnEngagedInMelee += Dragger_Ineligable;
		CharacterOwner.OnPositionChanged += Dragger_PositionChanged;
		CharacterOwner.OnStateChanged += Dragger_StateChanged;
	}

	private void Dragger_StateChanged(IPerceivable owner)
	{
		if (!CharacterState.Able.HasFlag(CharacterOwner.State))
		{
			CharacterOwner.Send(
				$"You are no longer dragging {Target.HowSeen(CharacterOwner)} because you are {CharacterOwner.State.Describe()}.");
			foreach (var helper in Helpers)
			{
				helper.Send($"You are no longer helping {Owner.HowSeen(helper)} to drag {Target.HowSeen(helper)}.");
			}

			Target.Send($"You are no longer being dragged by {Owner.HowSeen(Target as IPerceiver ?? CharacterOwner)}.");
			RemovalEffect();
		}
	}

	private void Dragger_PositionChanged(IPerceivable owner)
	{
		if (owner.PositionState.MoveRestrictions == MovementAbility.Restricted)
		{
			CharacterOwner.Send(
				$"You are no longer dragging {Target.HowSeen(CharacterOwner)} because you are {CharacterOwner.PositionState.Describe(CharacterOwner, null, PositionModifier.None, null, false)}.");
			foreach (var helper in Helpers)
			{
				helper.Send($"You are no longer helping {Owner.HowSeen(helper)} to drag {Target.HowSeen(helper)}.");
			}

			Target.Send($"You are no longer being dragged by {Owner.HowSeen(Target as IPerceiver ?? CharacterOwner)}.");
			RemovalEffect();
		}
	}

	private void Dragger_Ineligable(IPerceivable owner)
	{
		foreach (var helper in Helpers)
		{
			helper.Send($"You are no longer helping {Owner.HowSeen(helper)} to drag {Target.HowSeen(helper)}.");
		}

		Target.Send($"You are no longer being dragged by {Owner.HowSeen(Target as IPerceiver ?? CharacterOwner)}.");
		RemovalEffect();
	}

	public void ReleaseDraggerEvents()
	{
		CharacterOwner.OnDeath -= Dragger_Ineligable;
		CharacterOwner.OnQuit -= Dragger_Ineligable;
		CharacterOwner.OnDeleted -= Dragger_Ineligable;
		CharacterOwner.OnEngagedInMelee -= Dragger_Ineligable;
		CharacterOwner.OnPositionChanged -= Dragger_PositionChanged;
		CharacterOwner.OnStateChanged -= Dragger_StateChanged;
	}

	#endregion

	#region Target

	public void RegisterTargetEvents()
	{
		Target.OnQuit += Target_Ineligable;
		Target.OnDeleted += Target_Ineligable;
		if (Target is ICharacter tChar)
		{
			tChar.OnDeath += Target_Died;
			tChar.OnEngagedInMelee += Target_Engaged;
			tChar.OnWantsToMove += Target_OnWantsToMove;
		}

		if (Target is IGameItem tObj)
		{
			tObj.OnDeath += Target_Destroyed;
			tObj.OnEngagedInMelee += Target_Engaged;
		}
	}

	private void Target_Destroyed(IPerceivable owner)
	{
		foreach (var dragger in Draggers.Select(x => x.Character))
		{
			dragger.Send(
				$"You can no longer continue to drag {Target.HowSeen(dragger)} because it has been destroyed!");
		}

		RemovalEffect();
	}

	private void Target_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		response.Rejected = true;
		response.Reason = "You cannot move while you are being dragged. You must struggle free first.";
	}

	private void Target_Engaged(IPerceivable owner)
	{
		foreach (var dragger in Draggers.Select(x => x.Character))
		{
			dragger.Send(
				$"You can no longer continue to drag {Target.HowSeen(dragger)} because {Target.ApparentGender(dragger).Subjective()} has been engaged in melee!");
		}

		RemovalEffect();
	}

	private void Target_Died(IPerceivable owner)
	{
		var tChar = (ICharacter)Target;
		if (tChar.Corpse != null)
		{
			ReleaseTargetEvents();
			Target.RemoveAllEffects(x => x.IsEffectType<DragTarget>());
			Target = tChar.Corpse.Parent;
			Target.AddEffect(new DragTarget(Target, this));
			RegisterTargetEvents();
		}
		else
		{
			foreach (var dragger in Draggers.Select(x => x.Character))
			{
				dragger.Send(
					$"You can no longer continue to drag {Target.HowSeen(dragger)} because {Target.ApparentGender(dragger).Subjective()} has died!");
			}

			RemovalEffect();
		}
	}

	private void Target_Ineligable(IPerceivable owner)
	{
		foreach (var dragger in Draggers.Select(x => x.Character))
		{
			dragger.Send(
				$"You can no longer continue to drag {Target.HowSeen(dragger)} because {Target.ApparentGender(dragger).Subjective()} is no longer there.");
		}

		RemovalEffect();
	}

	public void ReleaseTargetEvents()
	{
		Target.OnQuit -= Target_Ineligable;
		Target.OnDeleted -= Target_Ineligable;
		if (Target is ICharacter tChar)
		{
			tChar.OnDeath -= Target_Died;
			tChar.OnEngagedInMelee -= Target_Engaged;
			tChar.OnWantsToMove -= Target_OnWantsToMove;
		}

		if (Target is IGameItem tObj)
		{
			tObj.OnDeath -= Target_Destroyed;
			tObj.OnEngagedInMelee -= Target_Engaged;
		}
	}

	#endregion

	public Dragging(ICharacter owner, IDragAid aid, IPerceivable target) : base(owner)
	{
		Target = target;
		CharacterOwner = owner;
		_draggers.Add(new Dragger { Character = owner, Aid = aid });
		Target.AddEffect(new DragTarget(Target, this));
		RegisterTargetEvents();
		RegisterDraggerEvents();
	}

	public override void Login()
	{
		RegisterTargetEvents();
		RegisterDraggerEvents();
	}

	public override void RemovalEffect()
	{
		foreach (var helper in _helpers.ToList())
		{
			RemoveHelper(helper);
			helper.Send($"You are no longer helping {Owner.HowSeen(helper)} drag {Target.HowSeen(helper)}.");
		}

		ReleaseDraggerEvents();
		ReleaseTargetEvents();
		Target.RemoveAllEffects(x => x.IsEffectType<DragTarget>());
		Owner.RemoveEffect(this);
	}

	protected override string SpecificEffectType => "Dragging";

	public override string Describe(IPerceiver voyeur)
	{
		return $"dragging {Target.HowSeen(voyeur)}";
	}

	#region Implementation of ILDescSuffixEffect

	public string SuffixFor(IPerceiver voyeur)
	{
		return $"dragging {Target.HowSeen(voyeur)}";
	}

	public bool SuffixApplies()
	{
		return true;
	}

	#endregion
}