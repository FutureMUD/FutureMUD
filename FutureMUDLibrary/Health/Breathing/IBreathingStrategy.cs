using MudSharp.Body;
using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health.Breathing
{
    public interface IBreathingStrategy
    {
        string Name {get;}
        bool NeedsToBreathe { get; }
        bool IsBreathing(IBody body);
        bool CanBreathe(IBody body);
        void Breathe(IBody body);
        IFluid BreathingFluid(IBody body);
    }
}
