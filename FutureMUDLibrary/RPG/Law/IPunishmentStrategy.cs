using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law
{
    public interface IPunishmentStrategy
    {
        string SaveResult();
        XElement SaveResultXElement();
        string Describe(IPerceiver voyeur);
        bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command);
        string Show(ICharacter actor);
        PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0.0);
        PunishmentOptions GetOptions(ICharacter actor, ICrime crime);

    }
}
