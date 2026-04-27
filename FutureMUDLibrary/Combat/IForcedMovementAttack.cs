namespace MudSharp.Combat
{
    public interface IForcedMovementAttack : ISecondaryDifficultyAttack
    {
        ForcedMovementTypes ForcedMovementTypes { get; }
        ForcedMovementVerbs ForcedMovementVerbs { get; }
        ForcedMovementRange RequiredRange { get; }
    }
}
