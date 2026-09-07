using MudSharp.CharacterCreation;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using SkillGroup = MudSharp.CharacterCreation.ChargenSkillSelectionGroup;

#nullable enable
namespace MudSharp.Commands.Modules;

internal partial class ChargenModule
{
	private const string SkillGroupHelp = @"Manage generic chargen skill-selection groups:
	chargenskillgroup list [filter]
	chargenskillgroup show <id|name>
	chargenskillgroup create <name>
	chargenskillgroup clone <id|name> <new-name>
	chargenskillgroup edit <id|name>
	chargenskillgroup set name <name>
	chargenskillgroup set description
	chargenskillgroup set picks <minimum> [maximum]
	chargenskillgroup set eligibility <Boolean(chargen) prog>
	chargenskillgroup set membereligibility <Boolean(chargen,trait) prog|none>
	chargenskillgroup set member add|remove <skill>
	chargenskillgroup set existing newonly|countknown
	chargenskillgroup set order <integer>
	chargenskillgroup set enabled [yes|no]
	chargenskillgroup validate <id|name|all>
	chargenskillgroup retire <id|name>
Quote names containing spaces. New and cloned groups are disabled. Retirement preserves identity.";

	[PlayerCommand("ChargenSkillGroup", "chargenskillgroup")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("chargenskillgroup", SkillGroupHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void ChargenSkillGroup(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var verb = ss.PopSpeech().ToLowerInvariant();
		var groups = actor.Gameworld.ChargenSkillSelectionGroups;
		void Edit(SkillGroup item)
		{
			actor.RemoveAllEffects<BuilderEditingEffect<SkillGroup>>();
			actor.AddEffect(new BuilderEditingEffect<SkillGroup>(actor) { EditingItem = item });
			actor.OutputHandler.Send(item.Show());
		}
		SkillGroup? Resolve(string text)
		{
			var matches = groups.OfType<SkillGroup>().Where(x => x.Id.ToString() == text || x.Name.EqualTo(text)).ToList();
			if (matches.Count == 1) return matches[0];
			actor.OutputHandler.Send("No unique group matches that ID or exact name.");
			return null;
		}
		if (verb == "list")
		{
			var filter = ss.SafeRemainingArgument;
			actor.OutputHandler.Send(StringUtilities.GetTextTable(groups
				.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
				.OrderBy(x => x.DisplayOrder)
				.ThenBy(x => x.StableKey)
				.Select(x => new List<string>
				{
					x.Id.ToString("N0", actor), x.Name, x.Enabled.ToColouredString(),
					$"{x.MinimumPicks.ToString("N0", actor)}-{x.MaximumPicks.ToString("N0", actor)}",
					x.Members.Count.ToString("N0", actor), x.EligibilityProg?.FunctionName ?? "Missing",
					x.Validate().Any() ? "Invalid" : "Valid"
				}), ["Id", "Name", "Enabled", "Picks", "Members", "Eligibility", "Configuration"], actor, Telnet.Green));
			return;
		}
		if (verb == "set")
		{
			var group = actor.EffectsOfType<BuilderEditingEffect<SkillGroup>>().FirstOrDefault()?.EditingItem;
			if (group is null) actor.OutputHandler.Send("First edit a skill-selection group.");
			else group.BuildingCommand(actor, ss);
			return;
		}
		if (verb is "create" or "clone")
		{
			SkillGroup? original = null;
			if (verb == "clone") { original = Resolve(ss.PopSpeech()); if (original is null) return; }
			var name = ss.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(name) || name.Length > 200) { actor.OutputHandler.Send("Supply a name of 1-200 characters."); return; }
			using (new FMDB())
			{
				var model = new Models.ChargenSkillSelectionGroup
				{
					Name = name, StableKey = Guid.NewGuid().ToString("N"), Enabled = false,
					Description = original?.Description ?? "", MinimumPicks = original?.MinimumPicks ?? 0,
					MaximumPicks = original?.MaximumPicks ?? 0, DisplayOrder = original?.DisplayOrder ?? 0,
					EligibilityProgId = original?.EligibilityProgId, MemberEligibilityProgId = original?.MemberEligibilityProgId,
					ExistingSkillPolicy = (int)(original?.ExistingSkillPolicy ?? ExistingSkillPolicy.NewOnly)
				};
				foreach (var (skill, i) in (original?.Members ?? []).Select((x, i) => (x, i)))
					model.Members.Add(new Models.ChargenSkillSelectionGroupMember { TraitDefinitionId = skill.Id, DisplayOrder = i });
				FMDB.Context.ChargenSkillSelectionGroups.Add(model);
				FMDB.Context.SaveChanges();
				var group = new SkillGroup(model, actor.Gameworld);
				groups.Add(group); Edit(group);
			}
			return;
		}
		if (verb == "validate" && ss.SafeRemainingArgument.EqualTo("all"))
		{
			actor.OutputHandler.Send(string.Join("\n", groups.Select(x => $"{x.Name}: {string.Join("; ", x.Validate().DefaultIfEmpty("Valid. Applicant-specific feasibility must still be checked."))}")));
			var enabled = groups.Where(x => x.Enabled && !x.Retired && !x.Validate().Any()).ToList();
			if (!SkillGroupAllocation.IsFeasible(enabled.Select(x => new SkillGroupAllocation(x.StableKey,
				x.MinimumPicks, x.MaximumPicks, x.Members.Select(y => y.Id).ToList(), []))))
			{
				actor.OutputHandler.Send("Potential overlap shortage: if these enabled groups apply together, their required minima cannot all be filled by distinct skills. Eligibility may make the groups mutually exclusive; applicant-specific validation remains necessary.");
			}
			return;
		}
		var target = Resolve(ss.SafeRemainingArgument);
		if (target is null) return;
		switch (verb)
		{
			case "edit": Edit(target); return;
			case "show": actor.OutputHandler.Send(target.Show()); return;
			case "validate":
				actor.OutputHandler.Send(string.Join("\n", target.Validate().DefaultIfEmpty("Valid. Applicant-specific feasibility must still be checked.")));
				var overlapping = groups.Where(x => x.Id != target.Id && x.Enabled && !x.Retired && x.Members.Intersect(target.Members).Any()).ToList();
				if (overlapping.Count > 0) actor.OutputHandler.Send($"Shares choices with {overlapping.Select(x => x.Name).ListToString()}. Each skill can fill only one group slot; run validate all for potential combined shortages.");
				return;
			case "retire":
				actor.OutputHandler.Send($"Retire {target.Name}? Type accept to confirm.");
				actor.AddEffect(new Accept(actor, new GenericProposal
				{
					DescriptionString = $"retiring skill group {target.Name}", Keywords = ["retire", "skillgroup"],
					AcceptAction = _ => { target.Retire(); actor.OutputHandler.Send("Group retired."); },
					RejectAction = _ => actor.OutputHandler.Send("Retirement cancelled."),
					ExpireAction = () => actor.OutputHandler.Send("Retirement expired.")
				}), TimeSpan.FromSeconds(120));
				return;
			default: actor.OutputHandler.Send(SkillGroupHelp); return;
		}
	}
}
