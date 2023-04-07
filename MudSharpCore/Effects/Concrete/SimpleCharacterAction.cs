using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SimpleCharacterAction : CharacterAction
{
	public SimpleCharacterAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		string cancelEmote, string cannotMoveEmote, string block, string ldescAddendum,
		Action<IPerceivable> onStopAction = null) : base(owner, action, actionDescription, cancelEmote, cannotMoveEmote,
		block, ldescAddendum, onStopAction)
	{
		SetupEventHandlers();
	}

	public SimpleCharacterAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		string cancelEmote, string cannotMoveEmote, IEnumerable<string> blocks, string ldescAddendum,
		Action<IPerceivable> onStopAction = null) : base(owner, action, actionDescription, cancelEmote, cannotMoveEmote,
		blocks, ldescAddendum, onStopAction)
	{
		SetupEventHandlers();
	}

	public SimpleCharacterAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		string block, string ldescAddendum, Action<IPerceivable> onStopAction = null) : base(owner)
	{
		CharacterOwner = owner;
		Action = action;
		ActionDescription = actionDescription;
		_blocks.Add(block);
		LDescAddendum = ldescAddendum;
		OnStopAction = onStopAction;
		CancelEmoteString = $"@ stop|stops {actionDescription}";
		WhyCannotMoveEmoteString = $"@ cannot move because #0 are|is {actionDescription}";
		SetupEventHandlers();
	}

	public SimpleCharacterAction(ICharacter owner, Action<IPerceivable> action, string actionDescription,
		IEnumerable<string> blocks, string ldescAddendum, Action<IPerceivable> onStopAction = null) : base(owner)
	{
		CharacterOwner = owner;
		Action = action;
		ActionDescription = actionDescription;
		_blocks.AddRange(blocks);
		LDescAddendum = ldescAddendum;
		OnStopAction = onStopAction;
		CancelEmoteString = $"@ stop|stops {actionDescription}";
		WhyCannotMoveEmoteString = $"@ cannot move because #0 are|is {actionDescription}";
		SetupEventHandlers();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Character Action - {ActionDescription}";
	}

	protected override string SpecificEffectType => "SimpleCharacterAction";
}