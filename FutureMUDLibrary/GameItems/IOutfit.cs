using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems
{
    public enum OutfitExclusivity
    {
        NonExclusive,
        ExcludeItemsBelow,
        ExcludeAllItems
    }

    public enum OutfitTemplateItemPlacement
    {
        Worn,
        Inventory,
        Room,
        Container
    }

    public interface IOutfit : IProgVariable
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

    public interface IOutfitItem : IKeyworded, IProgVariable
    {
        long Id { get; }
        [CanBeNull] IGameItem Item { get; }
        string ItemDescription { get; set; }
        long? PreferredContainerId { get; set; }
        [CanBeNull] string PreferredContainerDescription { get; set; }
        int WearOrder { get; set; }
        IWearProfile DesiredProfile { get; set; }
        XElement SaveToXml();

    }

    public interface IOutfitTemplate : IEditableItem
    {
        string Description { get; set; }
        OutfitExclusivity Exclusivity { get; set; }
        IEnumerable<IOutfitTemplateItem> Items { get; }
        IEnumerable<string> ValidationWarnings { get; }
        IOutfitTemplate Clone(string newName);
        IOutfit Materialise(ICharacter target, string outfitNameOverride = null);
    }

    public interface IOutfitTemplateItem : IKeyworded
    {
        string TemplateKey { get; set; }
        IGameItemProto GameItemProto { get; set; }
        IWearProfile DesiredProfile { get; set; }
        OutfitTemplateItemPlacement Placement { get; set; }
        [CanBeNull] string ContainerKey { get; set; }
        string LoadArguments { get; set; }
        int WearOrder { get; set; }
    }
}
