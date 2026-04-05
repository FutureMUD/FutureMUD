using MudSharp.Body.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IPreventPositionChange : IEffectSubtype
    {
        bool PreventsChange(IPositionState oldPosition, IPositionState newPosition);
        string WhyPreventsChange(IPositionState oldPosition, IPositionState newPosition);
    }
}
