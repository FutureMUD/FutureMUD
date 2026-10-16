using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;

namespace MudSharp.Work.Crafts
{
    public interface ICraftProduct : IFrameworkItem, ISaveable
    {
        ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality);
        DateTime OriginalAdditionTime { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
        void CreateNewRevision(Models.Craft dbcraft, bool failproduct, Dictionary<long, long> inputIdMap, Dictionary<long, long> toolIdMap);
        bool IsValid();
        string WhyNotValid();
        string HowSeen(IPerceiver voyeur);
        bool RefersToItemProto(long id);
        bool RefersToTag(ITag tag);
        bool RefersToLiquid(ILiquid liquid);
        bool IsItem(IGameItem item);
        /// <summary>
        /// Matches a possible successful output without executing production or inferring its history.
        /// Dynamic product types must explicitly implement a defensible match; search hints are insufficient.
        /// </summary>
        bool MatchesForAnalysis(IGameItem item) => false;
        string ProductType { get; }
    }
}
