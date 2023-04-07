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
using MudSharp.Body.Needs;

namespace MudSharp.NPC.AI.Groups;

public class GroupAI : LateInitialisingItem, IGroupAI
{
	public GroupAI(GroupAi ai, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = ai.Id;
		IdInitialised = true;
		_name = ai.Name;
		var root = XElement.Parse(ai.Definition);
		CurrentAction = (GroupAction)int.Parse(root.Element("Action").Value);
		Alertness = (GroupAlertness)int.Parse(root.Element("Alertness").Value);
		foreach (var id in root.Element("Members").Elements().Select(x => long.Parse(x.Value)))
		{
			_groupMemberIds.Add(id);
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
				select new XElement("Id", ch.Id)
			)
		);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.GroupAis.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveToXml().ToString();
		dbitem.Data = Data.SaveToXml().ToString();
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new GroupAi();
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

	private readonly List<long> _groupMemberIds = new();
	private List<ICharacter> _groupMembers;
	private IGroupTypeData _data;

	private void InitialiseGroupMembers()
	{
		if (_groupMembers == null)
		{
			_groupMembers = new List<ICharacter>(_groupMemberIds.Select(x => Gameworld.TryGetCharacter(x, true)));
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

	public ICharacter GroupLeader => GroupMembers.FirstOrDefault(x => GroupRoles[x] == GroupRole.Leader);

	public void AddToGroup(ICharacter character)
	{
		InitialiseGroupMembers();
		_groupMembers.Add(character);
		_groupMemberIds.Add(character.Id);
	}

	public void RemoveFromGroup(ICharacter character)
	{
		InitialiseGroupMembers();
		_groupMembers.Remove(character);
		_groupMemberIds.Remove(character.Id);
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Group AI #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Template: {Template.Name.Colour(Telnet.Cyan)} (#{Template.Id.ToString("N0", actor)})");
		sb.AppendLine($"Type: {GroupAIType.Name.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Alertness: {Alertness.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Priority: {CurrentAction.DescribeEnum().ColourValue()}");
		sb.AppendLine("Members: ");
		foreach (var member in GroupMembers.OrderByDescending(x => x.Race.AgeCategory(x))
		                                   .ThenByDescending(x => GroupRoles[x] == GroupRole.Leader))
		{
			var extra = new StringBuilder();
			if (member.NeedsModel.Status.IsHungry())
			{
				extra.Append(" hungry".Colour(Telnet.BoldYellow));
			}

			if (member.NeedsModel.Status.IsThirsty())
			{
				extra.Append(" thirsty".Colour(Telnet.BoldYellow));
			}

			sb.AppendLine(
				$"\t[#{member.Id.ToString("N0", actor)}] {member.HowSeen(actor)} ({member.Gender.GenderClass(true)} {member.Race.AgeCategory(member).DescribeEnum(true)} - {GroupRoles[member].DescribeEnum()}){extra.ToString()}");
		}

		var dataText = Data.ShowText(actor);
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
				var dbitem = FMDB.Context.GroupAis.Find(Id);
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

	public Dictionary<ICharacter, GroupRole> GroupRoles { get; } = new();

	public IEnumerable<IGroupEmote> GroupEmotes => Template.GroupEmotes;
}