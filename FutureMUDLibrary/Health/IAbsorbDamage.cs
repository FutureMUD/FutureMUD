using System.Collections.Generic;

namespace MudSharp.Health {
    public interface IAbsorbDamage {
        IDamage SufferDamage(IDamage damage, ref List<IWound> wounds);
        IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds);
        void ProcessPassiveWound(IWound wound);
    }
}