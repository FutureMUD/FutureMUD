using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Effects.Concrete;

public class ItemSalvaging : StagedCharacterActionWithTarget, IAffectProximity
{
	internal sealed class BeingSalvaged : Effect, INoGetEffect, IAffectProximity
	{
		public ICharacter Salvager { get; }

		public BeingSalvaged(IPerceivable owner, ICharacter salvager) : base(owner)
		{
			Salvager = salvager;
		}

		protected override string SpecificEffectType => "Being Salvaged";
		public bool CombatRelated => false;
		public override string Describe(IPerceiver voyeur) => "Being salvaged";

		public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
			=> thing == Salvager ? (true, Proximity.Immediate) : (false, Proximity.Unapproximable);
	}

	private readonly ISalvageable _salvageable;
	private readonly IGameItem? _tool;
	private BeingSalvaged? _lock;
	private readonly Queue<string> _emotes;

	public ItemSalvaging(ICharacter salvager, ISalvageable salvageable, IGameItem? tool)
		: base(salvager, salvageable.Parent)
	{
		_salvageable = salvageable;
		_tool = tool;
		var stages = salvageable.Stages.ToList();
		_emotes = new Queue<string>(stages.Select(x => x.Emote));
		TimesBetweenTicks = new Queue<TimeSpan>(stages.Skip(1).Select(x => TimeSpan.FromSeconds(x.Delay)));

		void Intermediate(IPerceivable perceivable)
		{
			SendStageEmote();
		}

		void Final(IPerceivable perceivable)
		{
			SendStageEmote();
			if (_lock is null || !Target.Effects.Contains(_lock))
			{
				return;
			}

			if (!_salvageable.CanSalvage(out var reason))
			{
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ stop|stops salvaging $1 because {reason}.", CharacterOwner, CharacterOwner, Target)));
				return;
			}

			var success = CharacterOwner.Gameworld.GetCheck(CheckType.ButcheryCheck)
			                            .Check(CharacterOwner, _salvageable.Difficulty, _salvageable.Trait, Target)
			                            .IsPass();
			ReleaseEventHandlers();
			var products = _salvageable.CreateProducts(CharacterOwner, success).ToList();
			var result = products.Count == 0
				? "no usable products"
				: products.Select(x => x.HowSeen(CharacterOwner)).ListToString();
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				CompletionEmote(success, result),
				CharacterOwner, CharacterOwner, Target)));
			_salvageable.Parent.Delete();
		}

		ActionQueue = new Queue<Action<IPerceivable>>(
			Enumerable.Repeat<Action<IPerceivable>>(Intermediate, Math.Max(0, stages.Count - 1)).Plus(Final));
		FireOnCount = stages.Count;
		ActionDescription = "salvaging $1";
		CancelEmoteString = "@ stop|stops salvaging $1";
		WhyCannotMoveEmoteString = "@ cannot move because #0 are|is salvaging $1.";
		LDescAddendum = "salvaging $1";
		_blocks.AddRange(["general", "movement"]);
		SetupEventHandlers();
	}

	internal static string CompletionEmote(bool success, string products)
	{
		return success
			? $"@ finish|finishes salvaging $1 and recover|recovers {products}."
			: $"@ finish|finishes salvaging $1, but rough work leaves much of it unusable. #0 recover|recovers {products}.";
	}

	private void SendStageEmote()
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			_emotes.Dequeue(), CharacterOwner, CharacterOwner, Target, _tool)));
	}

	protected override void SetupEventHandlers()
	{
		base.SetupEventHandlers();
		_lock ??= new BeingSalvaged(Target, CharacterOwner);
		if (_tool is not null)
		{
			_tool.OnDeleted -= ToolGone;
			_tool.OnDeleted += ToolGone;
			_tool.OnQuit -= ToolGone;
			_tool.OnQuit += ToolGone;
			CharacterOwner.Body.OnInventoryChange -= CheckInventoryChange;
			CharacterOwner.Body.OnInventoryChange += CheckInventoryChange;
		}

		if (!Target.Effects.Contains(_lock))
		{
			Target.AddEffect(_lock);
		}
	}

	private void CheckInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == _tool && newState is not InventoryState.Wielded and not InventoryState.Held)
		{
			ToolGone(item);
		}
	}

	private void ToolGone(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ stop|stops salvaging $1 because #0 are|is no longer holding $2.",
			CharacterOwner, CharacterOwner, Target, _tool)));
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		if (_tool is not null)
		{
			_tool.OnDeleted -= ToolGone;
			_tool.OnQuit -= ToolGone;
			CharacterOwner.Body.OnInventoryChange -= CheckInventoryChange;
		}

		if (_lock is not null)
		{
			Target.RemoveEffect(_lock);
		}
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
		=> thing == Target ? (true, Proximity.Immediate) : (false, Proximity.Unapproximable);
}
