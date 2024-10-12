using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.FutureProg;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Construction;
using MudSharp.Work.Butchering;

namespace MudSharp.Effects.Concrete;

public class Butchering : StagedCharacterActionWithTarget, IAffectProximity
{
	internal class BeingButchered : Effect, INoGetEffect, IAffectProximity
	{
		public ICharacter Butcher { get; set; }

		public BeingButchered(IPerceivable owner, ICharacter butcher, IFutureProg applicabilityProg = null) : base(
			owner, applicabilityProg)
		{
			Butcher = butcher;
		}

		public bool CombatRelated => false;

		protected override string SpecificEffectType => "Being Butchered";

		public override string Describe(IPerceiver voyeur)
		{
			return "Being butchered";
		}

		public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
		{
			if (Butcher == thing)
			{
				return (true, Proximity.Immediate);
			}

			return (false, Proximity.Unapproximable);
		}
	}

	public ICharacter Butcher { get; set; }
	public IButcherable Butcherable { get; set; }
	public IRaceButcheryProfile Profile => Butcherable.OriginalCharacter.Race.ButcheryProfile;
	public IGameItem Tool { get; set; }
	public string Subcategory { get; set; }

	public ITraitDefinition Trait { get; set; }

	public Difficulty CheckDifficulty { get; set; }

	public Queue<string> Emotes { get; set; }

	public Butchering(ICharacter butcher, IButcherable butcherable, string subcategory, IGameItem tool) : base(butcher,
		butcherable.Parent)
	{
		Tool = tool;
		Butcher = butcher;
		Butcherable = butcherable;
		Subcategory = subcategory;
		(Trait, CheckDifficulty) = Profile.BreakdownCheck(subcategory);
		var emotesAndDelays = Profile.BreakdownEmotes(subcategory).ToList();
		Emotes = new Queue<string>(emotesAndDelays.Select(x => x.Emote));
		TimesBetweenTicks = new Queue<TimeSpan>(emotesAndDelays.Select(x => TimeSpan.FromMilliseconds(x.Delay)));

		void IntermediateStepAction(IPerceivable perceivable)
		{
			Butcher.OutputHandler.Handle(new EmoteOutput(new Emote(Emotes.Dequeue(), Butcher, Butcher,
				Butcherable.Parent, Tool)));
		}

		void FinalStepAction(IPerceivable perceivable)
		{
			Butcher.OutputHandler.Handle(new EmoteOutput(new Emote(Emotes.Dequeue(), Butcher, Butcher,
				Butcherable.Parent, Tool)));
			ReleaseEventHandlers();
			if (Butcherable.Butcher(Butcher, subcategory))
			{
				Butcherable.Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ has been completely used up.",
					Butcherable.Parent)));
				Butcherable.Parent.Delete();
			}
		}

		ActionQueue = new Queue<Action<IPerceivable>>(Enumerable
		                                              .Repeat<Action<IPerceivable>>(IntermediateStepAction,
			                                              Emotes.Count - 1).Plus(FinalStepAction));
		FireOnCount = Emotes.Count;
		switch (Profile.Verb)
		{
			case ButcheryVerb.Butcher:
				ActionDescription = "breaking down $1";
				CancelEmoteString = "@ stop|stops breaking down $1";
				WhyCannotMoveEmoteString = "@ cannot move because #0 are|is breaking down $1.";
				LDescAddendum = "breaking down $1";
				break;
			case ButcheryVerb.Salvage:
				ActionDescription = "salvaging $1";
				CancelEmoteString = "@ stop|stops salvaging $1";
				WhyCannotMoveEmoteString = "@ cannot move because #0 are|is salvaging $1.";
				LDescAddendum = "salvaging $1";
				break;
			default:
				throw new ArgumentOutOfRangeException("Unsupported butchery verb" + Profile.Verb.ToString());
		}

		_blocks.AddRange(new[] { "general", "movement" });

		SetupEventHandlers();
	}

	protected override void SetupEventHandlers()
	{
		base.SetupEventHandlers();
		if (Tool != null)
		{
			Tool.OnDeleted -= ToolGone;
			Tool.OnDeleted += ToolGone;
			Tool.OnQuit -= ToolGone;
			Tool.OnQuit += ToolGone;
			CharacterOwner.Body.OnInventoryChange -= CheckInventoryChange;
			CharacterOwner.Body.OnInventoryChange += CheckInventoryChange;
		}

		Target.AddEffect(new BeingButchered(Target, Butcher));
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Butcherable.Parent == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	private void CheckInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item != Tool)
		{
			return;
		}

		if (newState != InventoryState.Wielded && newState != InventoryState.Held)
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

		Target.RemoveAllEffects(x => x.IsEffectType<BeingButchered>());
	}

	#region Overrides of Effect

	/// <inheritdoc />
	public override bool Applies(object target)
	{
		if (target is IRace race)
		{
			return Butcherable.OriginalCharacter.Race == race;
		}

		return base.Applies(target);
	}

	#endregion
}