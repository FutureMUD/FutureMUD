using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Work.Butchering;

namespace MudSharp.Effects.Concrete;

public class Skinning : StagedCharacterActionWithTarget, IAffectProximity
{
	internal class BeingSkinned : Effect, INoGetEffect, IAffectProximity
	{
		public ICharacter Skinner { get; set; }

		public BeingSkinned(IPerceivable owner, ICharacter skinner) : base(owner, null)
		{
			Skinner = skinner;
		}

		public bool CombatRelated => false;

		protected override string SpecificEffectType => "Being Skinned";

		public override string Describe(IPerceiver voyeur)
		{
			return "Being skinned";
		}

		public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
		{
			if (Skinner == thing)
			{
				return (true, Proximity.Immediate);
			}

			return (false, Proximity.Unapproximable);
		}
	}

	public ICharacter Skinner { get; set; }
	public IButcherable Skinnable { get; set; }
	public IRaceButcheryProfile Profile => Skinnable.OriginalCharacter.Race.ButcheryProfile;
	public IGameItem Tool { get; set; }

	public ITraitDefinition Trait { get; set; }

	public Difficulty CheckDifficulty { get; set; }

	public Queue<string> Emotes { get; set; }

	public Skinning(ICharacter skinner, IButcherable skinnable, IGameItem tool) : base(skinner, skinnable.Parent)
	{
		Tool = tool;
		Skinner = skinner;
		Skinnable = skinnable;
		(Trait, CheckDifficulty) = (Gameworld.Traits.Get(Gameworld.GetStaticLong("SkinningTraitId")),
			Profile.DifficultySkin);
		var emotesAndDelays = Profile.SkinEmotes.ToList();
		Emotes = new Queue<string>(emotesAndDelays.Select(x => x.Emote));
		TimesBetweenTicks = new Queue<TimeSpan>(emotesAndDelays.Select(x => TimeSpan.FromMilliseconds(x.Delay)));

		void IntermediateStepAction(IPerceivable perceivable)
		{
			Skinner.OutputHandler.Handle(new EmoteOutput(new Emote(Emotes.Dequeue(), Skinner, Skinner, Skinnable.Parent,
				Tool)));
		}

		void FinalStepAction(IPerceivable perceivable)
		{
			Skinner.OutputHandler.Handle(new EmoteOutput(new Emote(Emotes.Dequeue(), Skinner, Skinner, Skinnable.Parent,
				Tool)));
			ReleaseEventHandlers();
			Skinnable.Skin(Skinner);
		}

		ActionQueue = new Queue<Action<IPerceivable>>(Enumerable
		                                              .Repeat<Action<IPerceivable>>(IntermediateStepAction,
			                                              Emotes.Count - 1).Plus(FinalStepAction));
		FireOnCount = Emotes.Count;
		ActionDescription = "skinning $1";
		CancelEmoteString = "@ stop|stops skinning $1";
		WhyCannotMoveEmoteString = "@ cannot move because #0 are|is skinning $1.";
		LDescAddendum = "skinning $1";
		_blocks.AddRange(new[] { "general", "movement" });

		SetupEventHandlers();
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Skinnable.Parent == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	protected override void SetupEventHandlers()
	{
		base.SetupEventHandlers();
		if (Tool != null)
		{
			Tool.OnDeleted += ToolGone;
			Tool.OnQuit += ToolGone;
			CharacterOwner.Body.OnInventoryChange += CheckInventoryChange;
		}

		Target.AddEffect(new BeingSkinned(Target, Skinner));
	}

	private void CheckInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item != Tool)
		{
			return;
		}

		if (newState != InventoryState.Wielded || newState != InventoryState.Held)
		{
			ToolGone(item);
		}
	}

	private void ToolGone(IPerceivable owner)
	{
		Owner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"{CancelEmoteString} because #0 are|is no longer holding $2.", CharacterOwner, CharacterOwner, Target,
			Tool)));
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		if (Tool != null)
		{
			Tool.OnDeleted -= ToolGone;
			Tool.OnQuit -= ToolGone;
			CharacterOwner.Body.OnInventoryChange -= CheckInventoryChange;
		}

		Target.RemoveAllEffects(x => x.IsEffectType<BeingSkinned>());
	}
}