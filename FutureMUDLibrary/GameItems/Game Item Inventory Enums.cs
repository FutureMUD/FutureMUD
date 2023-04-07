using System;

namespace MudSharp.GameItems {
    /// <summary>
    ///     This enum contains reasons, from the IGameItem's perspective only, why or why not an item can be "gotten"
    /// </summary>
    public enum ItemGetResponse {
        /// <summary>
        ///     This item can be gotten
        /// </summary>
        CanGet,

        /// <summary>
        ///     This item can be gotten, but will leave a stack behind (so don't try to clean it up)
        /// </summary>
        CanGetStack,

        /// <summary>
        ///     The item cannot be gotten because it cannot be positioned
        /// </summary>
        Unpositionable,

        /// <summary>
        ///     This item has no IHoldable component and cannot be gotten
        /// </summary>
        NotIHoldable,

        NoGetEffect,

        NoGetEffectCombat,

        NoGetEffectPlan
    }

    [Flags]
    public enum ItemCanGetIgnore {
        None = 0,
        IgnoreCombat = 1,
        IgnoreInventoryPlans = 2,
        IgnoreFreeHands = 4,
        IgnoreLiftUseDrag = 8,
        IgnoreWeight = 16,
        IgnoreInContainer = 32,
    }

    [Flags]
    public enum ItemCanWieldFlags {
        None = 0,
        IgnoreFreeHands = 1,
        RequireTwoHands = 2,
        RequireOneHand = 3
    }

    public enum ItemCanWieldResponse {
        CanWield,
        CanWieldOneHandedButNotTwoHanded,
        CannotWield
    }

    public static class EnumExtensions {
        public static bool AsBool(this ItemGetResponse response) {
            return (response == ItemGetResponse.CanGet) || (response == ItemGetResponse.CanGetStack);
        }
    }
}