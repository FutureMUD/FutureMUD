﻿using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Plans {
    public enum DesiredItemState {
        InRoom,
        Held,
        Wielded,
        Worn,
        Consumed,
        InContainer,
        Sheathed,
        Attached,
        ConsumeLiquid,
        WieldedOneHandedOnly,
        WieldedTwoHandedOnly,
        Unknown,
        ConsumeCommodity
    }

    public interface IInventoryPlanAction : IXmlSavable {
        DesiredItemState DesiredState { get; }
        ITag DesiredTag { get; }
        ITag DesiredSecondaryTag { get; }
        IGameItem ScoutTarget(ICharacter executor);
        IGameItem ScoutSecondary(ICharacter executor, IGameItem item);
        object OriginalReference { get; }
        string Describe(ICharacter voyeur);
        bool RequiresFreeHandsToExecute(ICharacter who, IGameItem item);
    }
}