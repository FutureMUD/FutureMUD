using MudSharp.Body;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface IOrganHitReductionMerit : ICharacterMerit
    {
        bool MissesOrgan(KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo, IDamage damage, WoundSeverity severity);
    }
}
