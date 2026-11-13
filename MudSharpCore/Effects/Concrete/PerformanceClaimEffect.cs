#nullable enable

using System;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;

/// <summary>Temporarily assigns an NPC to a staged performance without replacing its AI.</summary>
public sealed class PerformanceClaimEffect : Effect
{
	public PerformanceClaimEffect(ICharacter owner, Guid runId) : base(owner)
	{
		RunId = runId;
	}

	private PerformanceClaimEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		RunId = Guid.Parse(effect.Element("Element")!.Element("RunId")!.Value);
	}

	public Guid RunId { get; }
	internal IMovement? StartedMovement { get; set; }

	public override void RemovalEffect()
	{
		var npc = (ICharacter)Owner;
		var move = StartedMovement;
		StartedMovement = null;
		// Mounted and party movement belongs to the whole travelling group.
		if (move is { Phase: MovementPhase.OriginalRoom } && ReferenceEquals(npc.Movement, move) &&
		    move.CharacterMovers.All(mover => ReferenceEquals(mover, npc)))
			move.Cancel();
		base.RemovalEffect();
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("PerformanceClaim", (effect, owner) => new PerformanceClaimEffect(effect, owner));
	}

	protected override string SpecificEffectType => "PerformanceClaim";
	public override bool SavingEffect => true;
	protected override XElement SaveDefinition() => new("Element", new XElement("RunId", RunId));
	public override string Describe(IPerceiver voyeur) => "Committed to a staged performance";
}
