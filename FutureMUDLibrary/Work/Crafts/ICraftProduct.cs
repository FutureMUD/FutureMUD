using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts
{
    public interface ICraftProduct : IFrameworkItem, ISaveable {
        ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality);
        DateTime OriginalAdditionTime { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
        void CreateNewRevision(Models.Craft dbcraft, bool failproduct);
        bool IsValid();
        string WhyNotValid();
        string HowSeen(IPerceiver voyeur);
    }
}
