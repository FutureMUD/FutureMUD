using MudSharp.Character.Heritage;
using MudSharp.GameItems;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface IContextualSizeMerit : ICharacterMerit {
        SizeCategory ContextualSize(SizeCategory original, SizeContext context);
    }
}
