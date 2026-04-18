using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using System;

namespace MudSharp.Effects.Interfaces
{
    public interface IActiveCraftEffect : IEffect, ILDescSuffixEffect, IRemoveOnStateChange, IRemoveOnMovementEffect, IRemoveOnMeleeCombat
    {
        ICharacter CharacterOwner { get; }
        IActiveCraftGameItemComponent Component { get; set; }
        void SubscribeEvents();
        void ReleaseEvents();
        TimeSpan NextPhaseDuration { get; set; }
    }
}