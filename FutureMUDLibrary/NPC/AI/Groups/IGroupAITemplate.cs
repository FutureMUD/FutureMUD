using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.NPC.AI.Groups.GroupTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.NPC.AI.Groups
{
    public interface IGroupAITemplate : IFrameworkItem, ISaveable, IEditableItem
    {
        IGroupAIType GroupAIType { get; }
        bool AvoidCell(ICell cell, GroupAlertness alertness);
        bool ConsidersThreat(ICharacter ch, GroupAlertness alertness);
        IEnumerable<IGroupEmote> GroupEmotes { get; }
        (bool Truth, string Error) IsValidForCreatingGroups { get; }
    }
}
