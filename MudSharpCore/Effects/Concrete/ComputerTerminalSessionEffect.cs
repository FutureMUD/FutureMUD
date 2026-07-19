#nullable enable

using MudSharp.Computers;
using MudSharp.Effects;

namespace MudSharp.Effects.Concrete;

public class ComputerTerminalSessionEffect : Effect, IEffectSubtype
{
	public ComputerTerminalSessionEffect(ICharacter owner)
		: base(owner)
	{
	}

	public required IComputerTerminalSession Session { get; init; }

	protected override string SpecificEffectType => "ComputerTerminalSessionEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Using terminal session on {Session.Terminal.ConnectedHost?.Name ?? "unknown host"} via {Session.Terminal.GetType().Name}";
	}

	public override void RemovalEffect()
	{
		if (Session.Terminal is GameItems.Components.ComputerTerminalGameItemComponent terminal)
		{
			terminal.DisconnectSession(Session.User, false);
		}
	}
}
