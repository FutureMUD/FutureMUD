using MudSharp.Character;
using MudSharp.FutureProg;

namespace MudSharp.Framework
{
    public interface ITag : IFrameworkItem {
        ITag Parent { get; set; }
        IFutureProg ShouldSeeProg { get; set; }
        IEditableTag GetEditable { get; }
        bool IsA(ITag otherTag);
        bool ShouldSee(ICharacter actor);
        string FullName { get; }
        void SetName(string name);
    }
}