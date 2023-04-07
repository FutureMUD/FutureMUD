using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.FutureProg;

namespace MudSharp.GameItems
{
    public enum OutfitExclusivity
    {
        NonExclusive,
        ExcludeItemsBelow,
        ExcludeAllItems
    }

    public interface IOutfit : IFutureProgVariable
    {
        ICharacter Owner { get; }
        string Name { get; set; }
        string Description { get; set; }
        OutfitExclusivity Exclusivity { get; set; }
        IEnumerable<IOutfitItem> Items { get; }
        IOutfitItem AddItem(IGameItem item, IGameItem preferredContainer, IWearProfile desiredProfile, int wearOrder = -1);
        void RemoveItem(long id);
        XElement SaveToXml();
        IOutfit CopyOutfit(ICharacter newOwner, string newName);

        string Show(ICharacter voyeur);
        bool BuildingCommand(ICharacter builder, StringStack command);
        void SwapItems(IOutfitItem item1, IOutfitItem item2);
    }

    public interface IOutfitItem : IKeyworded, IFutureProgVariable
    {
        long Id { get; }
        string ItemDescription { get; set; }
        long? PreferredContainerId { get; set; }
        [CanBeNull] string PreferredContainerDescription { get; set; }
        int WearOrder { get; set; }
        IWearProfile DesiredProfile { get; set; }
        XElement SaveToXml();

    }
}
