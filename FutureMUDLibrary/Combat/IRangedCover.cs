using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat
{
    public enum CoverType
    {
        Soft,
        Hard
    }

    public enum CoverExtent
    {
        Marginal,
        Partial,
        NearTotal,
        Total
    }

    public interface IRangedCover : IKeywordedItem, IEditableItem
    {
        CoverType CoverType { get; }
        CoverExtent CoverExtent { get; }
        IPositionState HighestPositionState { get; }
        int MaximumSimultaneousCovers { get; }
        bool CoverStaysWhileMoving { get; }
        string DescriptionString { get; }
        string ActionDescriptionString { get; }

        Difficulty MinimumRangedDifficulty { get; }
        string Describe(ICharacter covered, IPerceivable coverProvider, IPerceiver voyeur);
        IEmote DescribeAction(ICharacter covered, IPerceivable coverProvider);
    }
}