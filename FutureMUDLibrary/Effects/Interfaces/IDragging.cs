using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Effects.Interfaces
{
    public class Dragger {
        public ICharacter Character { get; set; }
        public IDragAid Aid { get; set; }
    }

    public interface IDragging : ILDescSuffixEffect, IDragParticipant, IEffect
    {
        IPerceivable Target { get; set; }
        ICharacter CharacterOwner { get; set; }
        IEnumerable<ICharacter> Helpers { get; }
        IEnumerable<Dragger> Draggers { get; }
        IEnumerable<ICharacter> CharacterDraggers { get; }
        void AddHelper(ICharacter actor, IDragAid aid);
        void RemoveHelper(ICharacter actor);
        void RegisterDraggerEvents();
        void ReleaseDraggerEvents();
        void RegisterTargetEvents();
        void ReleaseTargetEvents();
    }
}