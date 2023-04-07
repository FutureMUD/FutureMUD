using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups.GroupTypes
{
    public interface IGroupAIType
    {
        string Name { get; }
        bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness);
        void EvaluateGroupRolesAndMemberships(IGroupAI group);
        void HandleTenSecondTick(IGroupAI group);
        void HandleMinuteTick(IGroupAI group);
        IGroupTypeData LoadData(XElement root, IFuturemud gameworld);
        IGroupTypeData GetInitialData(IFuturemud gameworld);
        XElement SaveToXml();
    }
}
