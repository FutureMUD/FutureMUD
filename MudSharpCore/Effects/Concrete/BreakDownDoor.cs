using MudSharp.Construction.Boundary;

#nullable enable annotations

namespace MudSharp.Effects.Concrete;

public class BreakDownDoor : Effect, IEffectSubtype
{
    public ICharacter CharacterOwner { get; set; }
    public ICellExit Exit { get; set; }

    /// <summary>
    /// The transient pathing episode that created this focus, if any. This is deliberately not persisted.
    /// </summary>
    public FollowingPath? PathingEpisode { get; internal set; }

    public DateTime? NextSmashAttemptUtc { get; set; }

    public BreakDownDoor(ICharacter owner, ICellExit exit) : base(owner)
    {
        CharacterOwner = owner;
        Exit = exit;
    }

    protected override string SpecificEffectType => "BreakDownDoor";

    public override string Describe(IPerceiver voyeur)
    {
        return $"Breaking down the door to {Exit.OutboundDirectionDescription}.";
    }
}
