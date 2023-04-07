using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC.AI.Groups.GroupTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.NPC.AI.Groups
{
    public enum GroupAlertness
    {
        NotAlert,
        Wary,
        Agitated,
        VeryAgitated,
        Aggressive,
        Broken
    }

    public enum GroupRole
    {
        Leader,
        Pretender,
        Child,
        Elder,
        Outsider,
        Adult
    }

    public interface IGroupAI : IFrameworkItem, ISaveable
    {
        IGroupAITemplate Template { get; }
        IEnumerable<ICharacter> GroupMembers { get; }
        ICharacter GroupLeader { get;}
        void AddToGroup(ICharacter character);
        void RemoveFromGroup(ICharacter character);

        IGroupAIType GroupAIType { get; }
        GroupAlertness Alertness { get; set; }
        GroupAction CurrentAction { get; set; }
        IGroupTypeData Data { get; set; }
        Dictionary<ICharacter, GroupRole> GroupRoles { get; }
        bool AvoidCell(ICell cell, GroupAlertness alertness);
        bool ConsidersThreat(ICharacter ch, GroupAlertness alertness);
        IEnumerable<IGroupEmote> GroupEmotes { get; }
        string Show(ICharacter actor);
        void Delete();
    }
}
