using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Xml.Linq;
using MudSharp.Character.Heritage;

namespace MudSharp.NPC.AI.Groups;

public class GroupEmote : IGroupEmote
{
	public GroupEmote(string emoteText)
	{
		EmoteText = emoteText;
		MinimumAlertness = GroupAlertness.NotAlert;
		MaximumAlertness = GroupAlertness.Aggressive;
	}

	public GroupEmote(XElement root)
	{
		EmoteText = root.Element("EmoteText").Value;
		var value = int.Parse(root.Element("RequiredGender").Value);
		if (value != -1)
		{
			RequiredGender = (Gender)value;
		}

		value = int.Parse(root.Element("RequiredRole").Value);
		if (value != -1)
		{
			RequiredRole = (GroupRole)value;
		}

		value = int.Parse(root.Element("RequiredTargetRole").Value);
		if (value != -1)
		{
			RequiredTargetRole = (GroupRole)value;
		}

		value = int.Parse(root.Element("RequiredAgeCategory").Value);
		if (value != -1)
		{
			RequiredAgeCategory = (AgeCategory)value;
		}

		value = int.Parse(root.Element("RequiredAction")?.Value ?? "-1");
		if (value != -1)
		{
			RequiredAction = (GroupAction)value;
		}

		MinimumAlertness = (GroupAlertness)int.Parse(root.Element("MinimumAlertness").Value);
		MaximumAlertness = (GroupAlertness)int.Parse(root.Element("MaximumAlertness").Value);
	}

	public XElement SaveToXml()
	{
		return new XElement("Emote",
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("RequiredGender", (int?)RequiredGender ?? -1),
			new XElement("RequiredRole", (int?)RequiredRole ?? -1),
			new XElement("RequiredTargetRole", (int?)RequiredTargetRole ?? -1),
			new XElement("RequiredAgeCategory", (int?)RequiredAgeCategory ?? -1),
			new XElement("MinimumAlertness", (int)MinimumAlertness),
			new XElement("MaximumAlertness", (int)MaximumAlertness),
			new XElement("RequiredAction", (int?)RequiredAction ?? -1)
		);
	}

	public string EmoteText { get; set; }
	public Gender? RequiredGender { get; set; }
	public GroupRole? RequiredRole { get; set; }
	public GroupRole? RequiredTargetRole { get; set; }
	public AgeCategory? RequiredAgeCategory { get; set; }
	public GroupAction? RequiredAction { get; set; }
	public GroupAlertness MinimumAlertness { get; set; }
	public GroupAlertness MaximumAlertness { get; set; }

	public string DescribeForShow()
	{
		var sb = new StringBuilder();
		if (RequiredGender.HasValue)
		{
			sb.Append($" [{Gendering.Get(RequiredGender.Value).GenderClass()}]".Colour(Telnet.BoldBlue));
		}

		if (RequiredRole.HasValue)
		{
			sb.Append($" [{RequiredRole.Value.DescribeEnum()}]".Colour(Telnet.BoldBlue));
		}

		if (RequiredTargetRole.HasValue)
		{
			sb.Append($" [t={RequiredTargetRole.Value.DescribeEnum()}]".Colour(Telnet.BoldBlue));
		}

		if (RequiredAgeCategory.HasValue)
		{
			sb.Append($" [{RequiredAgeCategory.Value.DescribeEnum()}]".Colour(Telnet.BoldBlue));
		}

		if (RequiredAction.HasValue)
		{
			sb.Append($" [{RequiredAction.Value.DescribeEnum()}]".Colour(Telnet.BoldBlue));
		}

		sb.Append($" [{MinimumAlertness.DescribeEnum()} to {MaximumAlertness.DescribeEnum()}]");
		sb.Append($" {EmoteText}");
		return sb.ToString();
	}

	public void DoEmote(IGroupAI herd)
	{
		var primaryCandidates =
			herd.GroupRoles.Where(x => AppliesToIndividual(x.Key, x.Value)).Select(x => x.Key).ToList();
		if (RequiredTargetRole.HasValue)
		{
			var combined = primaryCandidates
			               .Select(x => (Primary: x, Secondary: x.Location
			                                                     .LayerCharacters(x.RoomLayer)
			                                                     .Where(y => herd.GroupRoles.ContainsKey(y) &&
			                                                                 herd.GroupRoles[y] == RequiredRole)
			                                                     .GetRandomElement()))
			               .Where(x => x.Secondary != null)
			               .GetRandomElement();
			combined.Primary.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, combined.Primary,
				combined.Primary, combined.Secondary)));
			return;
		}

		var primary = primaryCandidates.GetRandomElement();
		primary.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, primary, primary)));
	}

	private bool AppliesToIndividual(ICharacter ch, GroupRole role)
	{
		if (RequiredGender.HasValue && ch.Gender.Enum != RequiredGender)
		{
			return false;
		}

		if (RequiredRole.HasValue && role != RequiredRole)
		{
			return false;
		}

		if (RequiredAgeCategory.HasValue && ch.Race.AgeCategory(ch) != RequiredAgeCategory)
		{
			return false;
		}

		return true;
	}

	public bool Applies(IGroupAI herd)
	{
		if (herd.Alertness > MaximumAlertness)
		{
			return false;
		}

		if (herd.Alertness < MinimumAlertness)
		{
			return false;
		}

		if (RequiredAction.HasValue && herd.CurrentAction != RequiredAction.Value)
		{
			return false;
		}

		var primaryCandidates =
			herd.GroupRoles.Where(x => AppliesToIndividual(x.Key, x.Value)).Select(x => x.Key).ToList();
		if (!primaryCandidates.Any())
		{
			return false;
		}

		if (!RequiredTargetRole.HasValue)
		{
			return true;
		}

		switch (RequiredAction)
		{
			case GroupAction.AttackThreats:
			case GroupAction.Posture:
				foreach (var ch in primaryCandidates)
				{
					if (ch.Location.LayerCharacters(ch.RoomLayer).Any(x =>
						    herd.ConsidersThreat(x, herd.Alertness) && !herd.GroupMembers.Contains(x)))
					{
						return true;
					}
				}

				return false;
		}

		foreach (var ch in primaryCandidates)
		{
			if (ch.Location.LayerCharacters(ch.RoomLayer)
			      .Any(x => herd.GroupRoles.ContainsKey(x) && herd.GroupRoles[x] == RequiredRole))
			{
				return true;
			}
		}

		return false;
	}
}