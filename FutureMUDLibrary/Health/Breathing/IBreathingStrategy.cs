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
        string Name { get; }
        bool NeedsToBreathe { get; }
        bool IsBreathing(IBody body);
        bool CanBreathe(IBody body);
        /// <summary>Processes one explicit ten-second respiration step.</summary>
        void Breathe(IBody body);
        /// <summary>Processes respiration for an elapsed interval of real, active time.</summary>
        void Breathe(IBody body, TimeSpan elapsed);
        IFluid BreathingFluid(IBody body);
    }
}
