using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups.GroupTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups;

public class GroupAI : LateInitialisingItem, IGroupAI
{
    public GroupAI(GroupAi ai, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = ai.Id;
        IdInitialised = true;
        _name = ai.Name;
        XElement root = XElement.Parse(ai.Definition);
        CurrentAction = (GroupAction)int.Parse(root.Element("Action").Value);
        Alertness = (GroupAlertness)int.Parse(root.Element("Alertness").Value);
        foreach (var reference in LoadMemberReferences(root.Element("Members")))
        {
            _groupMemberReferences.Add(reference);
        }

        Template = Gameworld.GroupAITemplates.Get(ai.GroupAiTemplateId);
        Data = Template.GroupAIType.LoadData(XElement.Parse(ai.Data), Gameworld);
        Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondTick;
        Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += MinuteTick;
    }

    public GroupAI(IGroupAITemplate template, string name)
    {
        Gameworld = template.Gameworld;
        Template = template;
        _name = name;
        _currentAction = GroupAction.Graze;
        _alertness = GroupAlertness.NotAlert;
        Data = Template.GroupAIType.GetInitialData(Gameworld);
        Gameworld.Add(this);
        Gameworld.SaveManager.AddInitialisation(this);
        Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondTick;
        Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += MinuteTick;
    }

    private XElement SaveToXml()
    {
        return new XElement("Group",
            new XElement("Action", (int)CurrentAction),
            new XElement("Alertness", (int)Alertness),
            new XElement("Members",
                from ch in GroupMembers
                select SaveMemberReference(ch)
            )
        );
    }

    private static IEnumerable<GroupMemberReference> LoadMemberReferences(XElement membersRoot)
    {
        if (membersRoot is null)
        {
            yield break;
        }

        foreach (var element in membersRoot.Elements())
        {
            if (element.Name.LocalName.EqualTo("Member"))
            {
                var characterText = element.Attribute("character")?.Value ?? element.Attribute("id")?.Value;
                if (!long.TryParse(characterText, out var characterId))
                {
                    continue;
                }

                var instanceId = long.TryParse(element.Attribute("instance")?.Value, out var parsedInstanceId)
                    ? parsedInstanceId
                    : (long?)null;
                yield return new GroupMemberReference(characterId, instanceId);
                continue;
            }

            if (long.TryParse(element.Value, out var legacyId))
            {
                yield return new GroupMemberReference(legacyId, null);
            }
        }
    }

    private static XElement SaveMemberReference(ICharacter character)
    {
        var element = new XElement("Member",
            new XAttribute("character", CharacterInstanceIdentityComparer.IdentityId(character)));
        var instanceId = CharacterInstanceIdentityComparer.InstanceId(character);
        if (instanceId is not null)
        {
            element.Add(new XAttribute("instance", instanceId.Value));
        }

        return element;
    }

    public override void Save()
    {
        GroupAi dbitem = FMDB.Context.GroupAis.Find(Id);
        dbitem.Name = Name;
        dbitem.Definition = SaveToXml().ToString();
        dbitem.Data = Data.SaveToXml().ToString();
        Changed = false;
    }

    public override object DatabaseInsert()
    {
        GroupAi dbitem = new();
        FMDB.Context.GroupAis.Add(dbitem);

        dbitem.Name = Name;
        dbitem.Definition = SaveToXml().ToString();
        dbitem.Data = Data.SaveToXml().ToString();
        dbitem.GroupAiTemplateId = Template.Id;

        return dbitem;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((GroupAi)dbitem).Id;
    }

    public sealed override string FrameworkItemType => "GroupAI";

    public void TenSecondTick()
    {
        GroupAIType.HandleTenSecondTick(this);
    }

    public void MinuteTick()
    {
        GroupAIType.HandleMinuteTick(this);
    }

    public IGroupTypeData Data
    {
        get => _data;
        set
        {
            _data = value;
            Changed = true;
        }
    }

    private readonly record struct GroupMemberReference(long CharacterId, long? InstanceId);

    private readonly List<GroupMemberReference> _groupMemberReferences = new();
    private List<ICharacter> _groupMembers;
    private IGroupTypeData _data;

    private void InitialiseGroupMembers()
    {
        if (_groupMembers == null)
        {
            _groupMembers = _groupMemberReferences
                            .Select(x => CharacterInstanceIdentityComparer.ResolvePhysicalInstance(
                                Gameworld,
                                x.CharacterId,
                                x.InstanceId,
                                fallbackToPrimary: x.InstanceId is null))
                            .Where(x => x is not null)
                            .Select(x => x!)
                            .DistinctPhysicalInstances()
                            .ToList();
            _groupMemberReferences.Clear();
            _groupMemberReferences.AddRange(_groupMembers.Select(x => new GroupMemberReference(
                CharacterInstanceIdentityComparer.IdentityId(x),
                CharacterInstanceIdentityComparer.InstanceId(x))));
        }
    }

    public IEnumerable<ICharacter> GroupMembers
    {
        get
        {
            InitialiseGroupMembers();
            return _groupMembers;
        }
    }

    public ICharacter GroupLeader => GroupMembers.FirstOrDefault(x => RoleFor(x) == GroupRole.Leader);

    public void AddToGroup(ICharacter character)
    {
        InitialiseGroupMembers();
        if (_groupMembers.ContainsPhysicalInstance(character))
        {
            return;
        }

        _groupMembers.Add(character);
        _groupMemberReferences.Add(new GroupMemberReference(
            CharacterInstanceIdentityComparer.IdentityId(character),
            CharacterInstanceIdentityComparer.InstanceId(character)));
        Changed = true;
    }

    public void RemoveFromGroup(ICharacter character)
    {
        InitialiseGroupMembers();
        _groupMembers.RemovePhysicalInstance(character);
        _groupMemberReferences.RemoveAll(x =>
            x.CharacterId == CharacterInstanceIdentityComparer.IdentityId(character) &&
            (x.InstanceId is null || x.InstanceId == CharacterInstanceIdentityComparer.InstanceId(character)));
        Changed = true;
    }

    private GroupRole RoleFor(ICharacter character)
    {
        return GroupRoles.TryGetValue(character, out var role) ? role : GroupRole.Adult;
    }

    public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Group AI #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
        sb.AppendLine($"Template: {Template.Name.Colour(Telnet.Cyan)} (#{Template.Id.ToString("N0", actor)})");
        sb.AppendLine($"Type: {GroupAIType.Name.Colour(Telnet.Yellow)}");
        sb.AppendLine($"Alertness: {Alertness.DescribeEnum().ColourValue()}");
        sb.AppendLine($"Priority: {CurrentAction.DescribeEnum().ColourValue()}");
        sb.AppendLine("Members: ");
        foreach (ICharacter member in GroupMembers.OrderByDescending(x => x.Race.AgeCategory(x))
                                           .ThenByDescending(x => RoleFor(x) == GroupRole.Leader))
        {
            StringBuilder extra = new();
            if (member.NeedsModel.Status.IsHungry())
            {
                extra.Append(" hungry".Colour(Telnet.BoldYellow));
            }

            if (member.NeedsModel.Status.IsThirsty())
            {
                extra.Append(" thirsty".Colour(Telnet.BoldYellow));
            }

            sb.AppendLine(
                $"\t[#{member.Id.ToString("N0", actor)}:{member.InstanceId.ToString("N0", actor)}] {member.HowSeen(actor)} ({member.Gender.GenderClass(true)} {member.Race.AgeCategory(member).DescribeEnum(true)} - {RoleFor(member).DescribeEnum()}){extra.ToString()}");
        }

        string dataText = Data.ShowText(actor);
        if (!string.IsNullOrWhiteSpace(dataText))
        {
            sb.AppendLine();
            sb.AppendLine(dataText);
        }

        return sb.ToString();
    }

    public void Delete()
    {
        Gameworld.Destroy(this);
        Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondTick;
        Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteTick;
        using (new FMDB())
        {
            if (_id != 0)
            {
                Gameworld.SaveManager.Flush();
                Changed = false;
                Gameworld.SaveManager.Abort(this);
                GroupAi dbitem = FMDB.Context.GroupAis.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.GroupAis.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    public GroupAlertness Alertness
    {
        get => _alertness;
        set
        {
            _alertness = value;
            Changed = true;
        }
    }

    private GroupAction _currentAction;

    public GroupAction CurrentAction
    {
        get => _currentAction;
        set
        {
            _currentAction = value;
            Changed = true;
        }
    }

    public IGroupAITemplate Template { get; protected set; }

    public IGroupAIType GroupAIType => Template.GroupAIType;

    private GroupAlertness _alertness;

    public bool AvoidCell(ICell cell, GroupAlertness alertness)
    {
        return Template.AvoidCell(cell, alertness);
    }

    public virtual bool ConsidersThreat(ICharacter ch, GroupAlertness alertness)
    {
        return Template.ConsidersThreat(ch, alertness) || GroupAIType.ConsidersThreat(ch, this, alertness);
    }

    public Dictionary<ICharacter, GroupRole> GroupRoles { get; } = new(CharacterPhysicalInstanceEqualityComparer.Instance);

    public IEnumerable<IGroupEmote> GroupEmotes => Template.GroupEmotes;
}
