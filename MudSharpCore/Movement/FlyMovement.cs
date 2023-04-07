using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Movement;

public class FlyMovement : SingleMovement
{
	public override bool IgnoreTerrainStamina => true;

	public FlyMovement(GroupMovement movement, ICharacter mover) : base(movement, mover)
	{
	}

	public FlyMovement(ICellExit exit, ICharacter mover, TimeSpan duration, IEmote playerEmote = null) : base(exit,
		mover, duration, playerEmote)
	{
	}

	protected override void HandleMovementEcho()
	{
		Mover.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote(
					$"@ fly|flies {Exit.InboundMovementSuffix}",
					Mover),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote));
	}

	protected override MixedEmoteOutput GetMovementOutput => new(
		new Emote("@ begin|begins flying " + Exit.OutboundMovementSuffix, Mover), flags: OutputFlags.SuppressObscured);
}