#nullable enable

using MudSharp.Character.Name;
using MudSharp.NPC;
using MudSharp.NPC.AI.Groups;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private const int NpcReportPageSize = 8;

	private string NpcOverview(int page)
	{
		// The global registry holds identities. Secondary NPC bodies run their own AI subscriptions.
		var npcs = _world.NPCs.SelectMany(npc => npc.Identity.Instances)
			.OfType<INPC>()
			.Where(npc => npc.IsEmbodied)
			.Distinct<INPC>(CharacterPhysicalInstanceEqualityComparer.Instance)
			.ToList();
		var withoutAi = npcs.Count(npc => !npc.AIs.Any());
		var types = npcs.SelectMany(npc => npc.AIs
				.GroupBy(ai => ai.AIType, StringComparer.OrdinalIgnoreCase)
				.Select(type => (Type: type.Key, Assignments: type.Count())))
			.GroupBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
			.Select(type => (Type: type.Key, Npcs: type.Count(), Assignments: type.Sum(x => x.Assignments)))
			.OrderByDescending(x => x.Npcs)
			.ThenBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var groups = _world.GroupAIs.OrderBy(group => group.Id).ToList();
		var size = Math.Max(types.Count, groups.Count);
		var offset = ReportPageStart(page, size, NpcReportPageSize);
		var report = new StringBuilder();
		report.AppendLine($"Active NPCs: {npcs.Count} | No individual AI assigned: {withoutAi} | Individual AI assignments: {types.Sum(x => x.Assignments)}");
		report.AppendLine($"Individual AI types ({types.Count}) — distinct NPCs / assignments");
		foreach (var type in types.Skip(offset).Take(NpcReportPageSize))
		{
			report.AppendLine($"{ReportLabel(type.Type)} — {type.Npcs} / {type.Assignments}");
		}
		report.AppendLine($"Active group controllers ({groups.Count})");
		foreach (var group in groups.Skip(offset).Take(NpcReportPageSize))
		{
			report.AppendLine($"#{group.Id} {ReportLabel(group.Name)} — {ReportLabel(group.GroupAIType.Name)}, members: {group.GroupMembers.Count()}");
		}
		if (groups.Count > offset) { report.AppendLine("View members with /npc group id:<group ID>"); }
		if (size - offset > NpcReportPageSize) { report.AppendLine($"Continue with /npc overview page:{page + 1}"); }
		return report.ToString().TrimEnd();
	}

	private string NpcGroupRoster(long id, int page)
	{
		var group = _world.GroupAIs.Get(id) ?? throw new ArgumentException("Unknown group controller. Use /npc overview to find its ID.");
		var members = group.GroupMembers
			.OrderBy(CharacterInstanceIdentityComparer.IdentityId)
			.ThenBy(member => member.InstanceId)
			.ToList();
		var offset = ReportPageStart(page, members.Count, NpcReportPageSize);
		var report = new StringBuilder($"Group #{group.Id} {ReportLabel(group.Name)} — {ReportLabel(group.GroupAIType.Name)}\n");
		report.AppendLine($"Members ({members.Count})");
		foreach (var member in members.Skip(offset).Take(NpcReportPageSize))
		{
			var identity = CharacterInstanceIdentityComparer.IdentityId(member);
			var instance = CharacterInstanceIdentityComparer.InstanceId(member);
			var reference = instance.HasValue ? $"#{identity}:{instance}" : $"#{identity}";
			var name = member.PersonalName?.GetName(NameStyle.FullName) ?? member.Name;
			var role = group.GroupRoles.GetValueOrDefault(member, GroupRole.Adult);
			report.AppendLine($"{reference} {ReportLabel(name)} — {role.DescribeEnum()}");
		}
		if (members.Count - offset > NpcReportPageSize) { report.AppendLine($"Continue with /npc group id:{group.Id} page:{page + 1}"); }
		return report.ToString().TrimEnd();
	}
}
