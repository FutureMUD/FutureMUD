using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Work.Crafts;

namespace MudSharp.GameItems
{
    #nullable enable
    public interface IGameItemSkin : IEditableRevisableItem
    {
        IGameItemSkin Clone(ICharacter author, string newName);
        IGameItemProto ItemProto { get; }
        string? ItemName { get; }
        string? ShortDescription { get; }
        string? FullDescription { get; }
        string? LongDescription { get; }
        ItemQuality? Quality { get; }
        bool IsPublic { get; }
        IFutureProg CanUseSkinProg { get; }
        (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype);
        (bool Truth, string Error) CanUseSkin(ICharacter crafter, IGameItemProto? prototype, ICraft craft);
    }
}
