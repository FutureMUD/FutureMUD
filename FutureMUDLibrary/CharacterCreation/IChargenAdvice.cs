using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation
{
    public interface IChargenAdvice : IEditableItem
    {
        string AdviceTitle { get; set; }
        string AdviceText { get; set; }
        ChargenStage TargetStage { get; set; }
        IFutureProg ShouldShowAdviceProg { get; set; }
    }
}