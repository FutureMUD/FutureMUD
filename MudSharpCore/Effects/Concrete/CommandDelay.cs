using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class CommandDelay : Effect, ICommandDelay
{
	public CommandDelay(ICharacter owner, string command,
		string message = "You must wait a short time before doing that again",
		Action onExpireAction = null) : base(owner)
	{
		Message = message;
		WhichCommands.Add(command);
		OnExpireAction = onExpireAction;
	}

	public CommandDelay(ICharacter owner, IEnumerable<string> commands,
		string message = "You must wait a short time before doing that again",
		Action onExpireAction = null) : base(owner)
	{
		Message = message;
		WhichCommands.AddRange(commands);
		OnExpireAction = onExpireAction;
	}

	public List<string> WhichCommands = new();

	public Action OnExpireAction { get; set; }

	public string Message { get; set; }

	protected override string SpecificEffectType => "CommandDelay";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Prevented from entering the command{(WhichCommands.Count == 1 ? "" : "s")} {WhichCommands.ListToString()}";
	}

	public bool IsDelayed(string whichCommand)
	{
		return WhichCommands.Any(x => x.EqualTo(whichCommand));
	}

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		OnExpireAction?.Invoke();
		Owner.HandleEvent(EventType.CommandDelayExpired, Owner, WhichCommands);
	}
}