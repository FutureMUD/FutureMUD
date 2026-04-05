using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.RPG.Merits.GameItemMerits;

public abstract class GameItemMeritBase : MeritBase, IGameItemMerit
{
    protected GameItemMeritBase(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
    {
    }

    public override bool Applies(IHaveMerits owner)
    {
        IGameItem ownerAsItem = owner as IGameItem;
        return ownerAsItem != null && Applies(ownerAsItem);
    }

    protected abstract bool Applies(IGameItem owner);
}