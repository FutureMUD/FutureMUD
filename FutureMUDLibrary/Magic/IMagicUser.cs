using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Health;

namespace MudSharp.Magic
{
    public interface IMagicUser : IMortalPerceiver
    {
        IEnumerable<IMagicCapability> Capabilities { get; }
        IEnumerable<IMagicPower> Powers { get; }
        void LearnPower(IMagicPower power);
        void ForgetPower(IMagicPower power);
        void CheckResources();
    }
}
