using System.Runtime.CompilerServices;
using MudSharp.Accounts;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	public const string VancianHelp = @"Use <schoolverb> vancian <capability name or ID> followed by:
	status | spells [repertoire] [level] | spell <spell>
	known show|begin|draft|commit|cancel
	known add|remove <repertoire> <spell>
	loadouts | loadout new|show|validate|select|delete <name>
	loadout copy|rename <name> <newname>
	loadout assign <name> <allowance> <ordinal> <repertoire> <spell>
	loadout clear <name> <allowance> <ordinal>
	prepared | refresh [last] | cancel
	cast <repertoire> <allowance> <spell> <ordinal|next|atwill> [target arguments]

Quote names containing spaces. Known changes require the capability's permission policy.
Saved plans describe future preparation; editing or selecting them never restores spent slots.
Use next only in a finite allowance and atwill only in an at-will allowance. Power is fixed by the chosen route.
Use spellbook and spellscroll help for copying, inscription and stored spell activation.";
	private sealed record KnownDraft(long Version, Dictionary<Guid, List<long>> Selections);
	private static readonly ConditionalWeakTable<ICharacter, Dictionary<long, KnownDraft>> KnownDrafts = new();
	private static IVancianMagicCapability VancianCapability(ICharacter actor, string text, IMagicSchool? school = null, bool administrative = false)
	{
		var choices = (administrative ? actor.Gameworld.MagicCapabilities : actor.Capabilities).OfType<IVancianMagicCapability>()
			.Where(x => school is null || x.School.Id == school.Id).DistinctBy(x => x.Id).ToArray();
		var matches = choices.Where(x => x.Name.EqualTo(text) || long.TryParse(text, out var id) && x.Id == id).ToArray();
		return matches.Length == 1 ? matches[0] : throw new InvalidOperationException("Specify one applicable Vancian capability by its full name or ID.");
	}
	private static VancianRepertoireDefinition Repertoire(IVancianMagicCapability capability, string alias) => capability.Repertoires
		.FirstOrDefault(x => x.Alias.EqualTo(alias) || x.Key.ToString().EqualTo(alias)) ?? throw new InvalidOperationException("No repertoire has that alias or stable key.");
	private static VancianCastingAllowanceDefinition Allowance(IVancianMagicCapability capability, string alias) => capability.Allowances
		.FirstOrDefault(x => x.Alias.EqualTo(alias) || x.Key.ToString().EqualTo(alias)) ?? throw new InvalidOperationException("No allowance has that alias or stable key.");
	private static IMagicSpell VancianSpell(ICharacter actor, IVancianMagicCapability capability, string text) => actor.Gameworld.MagicSpells
		.FirstOrDefault(x => x.School.Id == capability.School.Id && x.Trigger is ICastMagicTrigger && (x.Name.EqualTo(text) || long.TryParse(text, out var id) && x.Id == id))
		?? throw new InvalidOperationException("No ordinary-cast spell in that school has that name or ID.");
	private static int? CastingOrdinal(VancianCastingAllowanceDefinition allowance, string text)
	{
		if (allowance.Mode == VancianAllowanceMode.AtWill)
			return text.EqualTo("atwill") ? null : throw new InvalidOperationException("An at-will allowance requires the atwill marker.");
		if (text.EqualTo("next")) return null;
		return int.TryParse(text, out var ordinal) && ordinal > 0 ? ordinal : throw new InvalidOperationException("A finite allowance requires next or a positive ordinal.");
	}
	private static IGameItem WritingItem(ICharacter actor, string text) => VancianItemAccess.AccessibleItems(actor).GetByIdOrName(text)
		?? actor.TargetItem(text) ?? throw new InvalidOperationException("You cannot find that item.");
	private static void VancianPlayer(ICharacter actor, IMagicSchool school, StringStack command)
	{
		try
		{
			if (command.IsFinished || command.PeekSpeech().EqualToAny("help", "?"))
			{
				actor.OutputHandler.Send(VancianHelp + "\nApplicable capabilities: " + actor.Capabilities
					.OfType<IVancianMagicCapability>().Where(x => x.School.Id == school.Id).DistinctBy(x => x.Id)
					.Select(x => $"{x.Name} (#{x.Id.ToString("N0", actor)})").ListToString());
				return;
			}
			var capability = VancianCapability(actor, command.PopSpeech(), school);
			var service = VancianMagicService.For(actor.Gameworld);
			var state = service.State(actor, capability);
			var verb = command.PopForSwitch();
			switch (verb)
			{
				case "status": case "": actor.OutputHandler.Send(VancianStatus(actor, actor, capability)); return;
				case "help": case "?": actor.OutputHandler.Send(VancianHelp); return;
				case "spells":
					var ruleText = command.PopSpeech(); int? level = null; VancianRepertoireDefinition? filter = null;
					if (int.TryParse(ruleText, out var onlyLevel)) level = onlyLevel;
					else if (!string.IsNullOrEmpty(ruleText)) filter = Repertoire(capability, ruleText);
					if (!command.IsFinished) level = int.Parse(command.PopSpeech());
					var rows = capability.Repertoires.Where(x => filter is null || x.Key == filter.Key).SelectMany(rule => service.Candidates(actor, capability, rule.Key)
						.Where(spell => !level.HasValue || spell.SpellLevel == level).Select(spell => new[] { rule.Alias, spell.Name, spell.SpellLevel.ToString("N0", actor),
							(state.Selections.GetValueOrDefault(rule.Key)?.Contains(spell.Id) == true ? "Selected " : "") +
							(service.HasFormula(actor, capability, spell) ? "Book " : "") +
							(state.Slots.Any(x => x.Preparation?.SpellId == spell.Id && x.Preparation.RepertoireKey == rule.Key) ? "Prepared " : "") +
							(capability.Allowances.Any(a => service.CanCast(actor, capability, rule.Key, a.Key, spell).Available) ? "Castable" : "Candidate") }));
					actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, ["Repertoire", "Spell", "Level", "Availability"], actor.LineFormatLength, unicodeTable: actor.Account.UseUnicode)); return;
				case "spell":
					var spell = VancianSpell(actor, capability, command.PopSpeech());
					if (!capability.Repertoires.Any(r => service.Candidates(actor, capability, r.Key).Contains(spell)) && !spell.CharacterKnowsSpell(actor)) throw new InvalidOperationException("You have no permitted information route for that spell.");
					actor.OutputHandler.Send(spell.ShowPlayerHelp(actor) + $"\nBase level: {spell.SpellLevel.ToString("N0", actor)}\n" + string.Join("\n", capability.Allowances.SelectMany(a => a.RepertoireKeys.Select(r =>
						{ var route = service.CanCast(actor, capability, r, a.Key, spell); return $"{Repertoire(capability, r.ToString()).Alias}/{a.Alias}: {(route.Available ? $"level {route.CastingLevel}, {route.Power.DescribeEnum()}" : route.Reason)}"; })))); return;
				case "known": VancianKnown(actor, capability, command); return;
				case "loadouts":
					actor.OutputHandler.Send(string.Join("\n", state.Loadouts.Select(x => $"{x.Name.ColourName()}: {x.Assignments.Count.ToString("N0", actor)} planned assignments{(state.SelectedLoadout == x.Id ? " [selected next]" : "")}")) +
						$"\nLast committed pattern: {(state.LastPattern is null ? "none" : $"{state.LastPattern.Count.ToString("N0", actor)} assignments, independent of saved names")}"); return;
				case "loadout": VancianLoadoutCommand(actor, capability, command); return;
				case "prepared": actor.OutputHandler.Send(VancianPrepared(actor, actor, capability)); return;
				case "refresh":
					var option = command.PopSpeech(); if (!string.IsNullOrEmpty(option) && !option.EqualTo("last")) throw new InvalidOperationException("Use refresh or refresh last.");
					actor.OutputHandler.Send(service.RequestRefresh(actor, capability, option.EqualTo("last")).Message); return;
				case "cancel":
					var actions = actor.EffectsOfType<VancianTimedAction>().ToArray(); foreach (var action in actions) actor.RemoveEffect(action, true);
					actor.OutputHandler.Send(actions.Length == 0 ? "You have no magical writing or preparation to cancel." : "Your precommit magical work has stopped."); return;
				case "cast":
					var repertoire = Repertoire(capability, command.PopSpeech()); var allowance = Allowance(capability, command.PopSpeech());
					var castSpell = VancianSpell(actor, capability, command.PopSpeech()); var ordinal = CastingOrdinal(allowance, command.PopSpeech());
					actor.OutputHandler.Send(service.Cast(actor, capability, repertoire.Key, allowance.Key, castSpell, ordinal, command).Message); return;
				default: actor.OutputHandler.Send(VancianHelp); return;
			}
		}
		catch (Exception ex) { actor.OutputHandler.Send(ex.Message); }
	}
	private static Dictionary<long, KnownDraft> Drafts(ICharacter actor) => KnownDrafts.GetValue(actor, character =>
	{
		character.OnQuit += _ => KnownDrafts.Remove(character);
		return [];
	});
	private static void VancianKnown(ICharacter actor, IVancianMagicCapability capability, StringStack command)
	{
		var service = VancianMagicService.For(actor.Gameworld); var state = service.State(actor, capability); var drafts = Drafts(actor);
		var verb = command.PopForSwitch();
		if (verb == "show") { actor.OutputHandler.Send(SelectedText(actor, capability, state.Selections)); return; }
		if (verb == "cancel") { drafts.Remove(capability.Id); actor.OutputHandler.Send("Known-spell draft discarded."); return; }
		if (verb == "begin")
		{
			if (state.DataError is not null) throw new InvalidOperationException(state.DataError);
			drafts[capability.Id] = new(state.Version, capability.Repertoires.Where(x => x.Source == VancianRepertoireSource.Selected).ToDictionary(x => x.Key, x => (state.Selections.GetValueOrDefault(x.Key) ?? []).ToList()));
			actor.OutputHandler.Send("Draft begun for every Selected repertoire in this capability. Add or remove spells, then use known draft and known commit."); return;
		}
		if (!drafts.TryGetValue(capability.Id, out var draft)) throw new InvalidOperationException("Use known begin first.");
		var selections = () => draft.Selections.ToDictionary(x => x.Key, x => (IReadOnlyList<long>)x.Value.AsReadOnly());
		switch (verb)
		{
			case "add": case "remove":
				var rule = Repertoire(capability, command.PopSpeech()); var spell = VancianSpell(actor, capability, command.PopSpeech());
				if (rule.Source != VancianRepertoireSource.Selected) throw new InvalidOperationException("A Spellbook repertoire has no selected-spell draft.");
				if (verb == "add") { if (!draft.Selections[rule.Key].Contains(spell.Id)) draft.Selections[rule.Key].Add(spell.Id); }
				else draft.Selections[rule.Key].Remove(spell.Id);
				actor.OutputHandler.Send("Draft updated; committed selections are unchanged."); return;
			case "draft":
				actor.OutputHandler.Send($"Committed:\n{SelectedText(actor, capability, state.Selections)}\nProposed:\n{SelectedText(actor, capability, draft.Selections)}\n" + string.Join("\n", service.ValidateSelections(actor, capability, selections())) + (state.Version != draft.Version ? "\nStale draft: begin again before committing." : "")); return;
			case "commit":
				var result = service.CommitKnown(actor, capability, draft.Version, selections()); actor.OutputHandler.Send(result.Message); if (result.Success) drafts.Remove(capability.Id); return;
			default: throw new InvalidOperationException("Use known show|begin|add|remove|draft|commit|cancel.");
		}
	}
	private static string SelectedText(ICharacter viewer, IVancianMagicCapability capability, Dictionary<Guid, List<long>> selections) => string.Join("\n", capability.Repertoires.Where(x => x.Source == VancianRepertoireSource.Selected).OrderBy(x => x.SortOrder)
		.Select(rule => $"{rule.Alias.ColourName()}: {string.Join(", ", (selections.GetValueOrDefault(rule.Key) ?? []).Select(id => viewer.Gameworld.MagicSpells.Get(id) is { } spell ? $"{spell.Name} (level {spell.SpellLevel.ToString("N0", viewer)})" : $"missing spell #{id.ToString("N0", viewer)}"))}"));
	private static void VancianLoadoutCommand(ICharacter actor, IVancianMagicCapability capability, StringStack command)
	{
		var service = VancianMagicService.For(actor.Gameworld); var state = service.State(actor, capability);
		var verb = command.PopForSwitch(); var name = command.PopSpeech();
		if (verb is "show" or "validate")
		{
			var plan = state.Loadouts.Find(x => x.Name.EqualTo(name)) ?? throw new InvalidOperationException("No saved loadout has that name.");
			var errors = service.ValidatePattern(actor, capability, state, plan.Assignments);
			actor.OutputHandler.Send(string.Join("\n", plan.Assignments.Select(x => AssignmentText(actor, capability, x))) + "\n" + (errors.Count == 0 ? "This saved plan currently validates." : string.Join("\n", errors))); return;
		}
		if (verb == "delete") { ConfirmVancian(actor, $"delete saved loadout {name}", () => service.EditLoadout(actor, capability, verb, name)); return; }
		if (verb == "assign")
		{
			var allowance = Allowance(capability, command.PopSpeech()); var ordinal = int.Parse(command.PopSpeech());
			var rule = Repertoire(capability, command.PopSpeech()); var spell = VancianSpell(actor, capability, command.PopSpeech());
			if (allowance.SlotLevel is not { } level || spell.SpellLevel > level || !allowance.RepertoireKeys.Contains(rule.Key)) throw new InvalidOperationException("That spell/repertoire cannot be assigned to this allowance.");
			actor.OutputHandler.Send(service.EditLoadout(actor, capability, verb, name, assignment: new(allowance.Key, allowance.StructuralVersion, ordinal, rule.Key, spell.Id, level, spell.SpellLevel, VancianPolicy.Power(capability, spell.SpellLevel, level))).Message); return;
		}
		if (verb == "clear") { var allowance = Allowance(capability, command.PopSpeech()); var ordinal = int.Parse(command.PopSpeech()); actor.OutputHandler.Send(service.EditLoadout(actor, capability, verb, name, allowance: allowance.Key, ordinal: ordinal).Message); return; }
		actor.OutputHandler.Send(service.EditLoadout(actor, capability, verb, name, command.PopSpeech()).Message);
	}
	private static string AssignmentText(ICharacter viewer, IVancianMagicCapability capability, VancianAssignment x) =>
		$"{capability.Allowances.FirstOrDefault(a => a.Key == x.AllowanceKey)?.Alias ?? x.AllowanceKey.ToString()} #{x.Ordinal.ToString("N0", viewer)}: {viewer.Gameworld.MagicSpells.Get(x.SpellId)?.Name ?? $"missing #{x.SpellId}"}, base {x.SpellLevel.ToString("N0", viewer)}, casting level {x.SlotLevel.ToString("N0", viewer)}, {x.Power.DescribeEnum()}, repertoire {capability.Repertoires.FirstOrDefault(r => r.Key == x.RepertoireKey)?.Alias ?? x.RepertoireKey.ToString()}";
	private static string VancianPrepared(ICharacter viewer, ICharacter target, IVancianMagicCapability capability)
	{
		var service = VancianMagicService.For(viewer.Gameworld); var state = service.State(target, capability);
		return string.Join("\n", service.Slots(target, capability).Select(x => $"{capability.Allowances.FirstOrDefault(a => a.Key == x.AllowanceKey)?.Alias ?? x.AllowanceKey.ToString()} #{x.Ordinal.ToString("N0", viewer)}: {x.Status.DescribeEnum()} {(x.Preparation is { } assignment ? AssignmentText(viewer, capability, assignment) : "")}{(x.SuspensionReason is { } reason ? $" - {reason}" : "")}")) +
			"\nLast committed pattern:\n" + (state.LastPattern is null ? "None" : state.LastPattern.Count == 0 ? "Explicit empty pattern" : string.Join("\n", state.LastPattern.Select(x => AssignmentText(viewer, capability, x))));
	}
	private static string VancianStatus(ICharacter viewer, ICharacter target, IVancianMagicCapability capability)
	{
		var service = VancianMagicService.For(viewer.Gameworld); var state = service.State(target, capability);
		var sb = new StringBuilder($"{capability.Name.ColourName()} - identity #{VancianPolicy.Owner(target).Id.ToString("N0", viewer)}, state version {state.Version.ToString("N0", viewer)}\n");
		foreach (var error in capability.ConfigurationErrors()) sb.AppendLine(error.ColourError());
		if (state.DataError is { } disabled) return sb.AppendLine($"State disabled: {disabled}").ToString();
		try { sb.AppendLine($"Caster level: {service.CasterLevel(target, capability).ToString("N0", viewer)}"); } catch (Exception ex) { sb.AppendLine(ex.Message); }
		foreach (var rule in capability.Repertoires.OrderBy(x => x.SortOrder))
		{
			sb.AppendLine($"{rule.Alias}: {rule.Source.DescribeEnum()}, levels {rule.MinimumSpellLevel.ToString("N0", viewer)}-{rule.MaximumSpellLevel.ToString("N0", viewer)}, book policy {rule.BookPolicy.DescribeEnum()}");
			if (rule.Source != VancianRepertoireSource.Selected) continue;
			try
			{
				var active = service.ActiveSelections(target, capability, rule, state);
				var suspended = (state.Selections.GetValueOrDefault(rule.Key) ?? []).Where(id => !active.Contains(id)).ToArray();
				if (suspended.Length > 0) sb.AppendLine("  Suspended selections (current eligibility or limit): " +
					suspended.Select(id => viewer.Gameworld.MagicSpells.Get(id)?.Name ?? $"missing #{id.ToString("N0", viewer)}").ListToString());
			}
			catch (Exception ex) { sb.AppendLine($"  Selection access disabled: {ex.Message}"); }
			foreach (var level in service.Candidates(target, capability, rule.Key).Select(x => x.SpellLevel).Distinct())
				try { sb.AppendLine($"  Level {level.ToString("N0", viewer)} selected limit: {service.SelectionLimit(target, capability, rule, level).ToString("N0", viewer)}"); } catch (Exception ex) { sb.AppendLine(ex.Message); }
		}
		foreach (var allowance in capability.Allowances.OrderBy(x => x.SortOrder))
			try { sb.AppendLine($"{allowance.Alias}: {allowance.Mode.DescribeEnum()}, {(allowance.Mode == VancianAllowanceMode.AtWill ? "unlimited eligible at-will uses" : $"level {allowance.SlotLevel}, current capacity {service.Capacity(target, capability, allowance).ToString("N0", viewer)} (created only on refresh)")}"); } catch (Exception ex) { sb.AppendLine(ex.Message); }
		sb.AppendLine($"Recovery: {capability.RecoveryMode.DescribeEnum()}, preparation {capability.PreparationDuration.Describe(viewer)}, required sleep {capability.RequiredSleepDuration.Describe(viewer)}, refresh interval {capability.MinimumRefreshInterval.Describe(viewer)}; earned qualification: {state.SleepQualified.ToColouredString()}; last refresh UTC: {state.LastRefreshUtc?.ToString("g", viewer) ?? "never"}");
		sb.AppendLine(SelectedText(viewer, capability, state.Selections)); sb.AppendLine(VancianPrepared(viewer, target, capability));
		foreach (var operation in service.Store.Operations(state.OwnerId, capability.Id).Where(x => x.Status is "NeedsReview" or "Invoking" or "Pending" or "Committing" or "Reserved" ||
			x.Status == "Consumed" && x.Kind is "Cast" or "ScrollActivation")) sb.AppendLine($"Operation {operation.Id}: {operation.Kind} {operation.Status}; {operation.Diagnostic}");
		return sb.ToString();
	}
	private static void ConfirmVancian(ICharacter actor, string description, Func<VancianResult> action)
	{
		actor.AddEffect(new Accept(actor, new GenericProposal(_ => actor.OutputHandler.Send(action().Message), _ => actor.OutputHandler.Send("Cancelled."), () => actor.OutputHandler.Send("Confirmation expired."), description, "vancian")), TimeSpan.FromSeconds(120));
		actor.OutputHandler.Send($"Use accept vancian to {description}, or decline vancian.");
	}
	public const string SpellbookHelp = "spellbook show <book>\nspellbook copy <capability> <source-book-or-scroll> <spell> <destination-book>\nCopying takes configured time and materials. It spends no personal spell slot. Scroll sources are destroyed only on successful commitment.";
	public const string SpellScrollHelp = "spellscroll show <scroll>\nspellscroll inscribe <capability> <repertoire> <allowance> <spell> <ordinal|next|atwill> <blank-scroll>\nspellscroll cast <scroll> <capability> [target arguments]\nInscription prepays the selected casting and spell costs. Release destroys the scroll, including on a committed failed control check or resisted spell.";
	[PlayerCommand("Spellbook", "spellbook")]
	[HelpInfo("spellbook", SpellbookHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void SpellbookCommand(ICharacter actor, string input)
	{
		try
		{
			var command = new StringStack(input.RemoveFirstWord()); var verb = command.PopForSwitch();
			if (verb == "show")
			{
				var item = WritingItem(actor, command.PopSpeech()); var book = item.GetItemType<ISpellbook>() ?? throw new InvalidOperationException("That item is not a spellbook.");
				if (book.Prototype is not SpellbookGameItemComponentProto proto || !VancianItemAccess.Usable(actor, item, proto)) throw new InvalidOperationException("You cannot access or read that spellbook.");
				actor.OutputHandler.Send(book.DataError ?? $"Formulae: {book.Formulae.Count.ToString("N0", actor)}/{book.FormulaCapacity.ToString("N0", actor)}\n" + string.Join("\n", book.Formulae.Select(x => actor.Gameworld.MagicSpells.Get(x.SpellId) is { } spell ? $"{spell.Name} - {spell.School.Name}, level {spell.SpellLevel.ToString("N0", actor)}" : $"Missing formula #{x.SpellId.ToString("N0", actor)}"))); return;
			}
			if (verb != "copy") { actor.OutputHandler.Send(SpellbookHelp); return; }
			var capability = VancianCapability(actor, command.PopSpeech()); var source = WritingItem(actor, command.PopSpeech());
			var formula = VancianSpell(actor, capability, command.PopSpeech()); var destination = WritingItem(actor, command.PopSpeech());
			actor.OutputHandler.Send(VancianMagicService.For(actor.Gameworld).BeginTranscription(actor, capability, source, formula, destination).Message);
		}
		catch (Exception ex) { actor.OutputHandler.Send(ex.Message); }
	}
	[PlayerCommand("SpellScroll", "spellscroll")]
	[HelpInfo("spellscroll", SpellScrollHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void SpellScrollCommand(ICharacter actor, string input)
	{
		try
		{
			var command = new StringStack(input.RemoveFirstWord()); var verb = command.PopForSwitch(); var service = VancianMagicService.For(actor.Gameworld);
			if (verb == "show") { ShowSpellScroll(actor, WritingItem(actor, command.PopSpeech())); return; }
			if (verb == "cast") { var item = WritingItem(actor, command.PopSpeech()); var cap = VancianCapability(actor, command.PopSpeech()); actor.OutputHandler.Send(service.ActivateScroll(actor, cap, item, command).Message); return; }
			if (verb != "inscribe") { actor.OutputHandler.Send(SpellScrollHelp); return; }
			var capability = VancianCapability(actor, command.PopSpeech()); var rule = Repertoire(capability, command.PopSpeech()); var allowance = Allowance(capability, command.PopSpeech());
			var spell = VancianSpell(actor, capability, command.PopSpeech()); var ordinal = CastingOrdinal(allowance, command.PopSpeech()); var blank = WritingItem(actor, command.PopSpeech());
			actor.OutputHandler.Send(service.BeginInscription(actor, capability, rule.Key, allowance.Key, spell, ordinal, blank).Message);
		}
		catch (Exception ex) { actor.OutputHandler.Send(ex.Message); }
	}
	private static void ShowSpellScroll(ICharacter actor, IGameItem item, bool detailed = false)
	{
		if (item.GetItemType<ISpellScroll>() is not SpellScrollGameItemComponent scroll) throw new InvalidOperationException("That item is not a spell scroll.");
		if (!detailed && !VancianItemAccess.Usable(actor, item, (SpellScrollGameItemComponentProto)scroll.Prototype)) throw new InvalidOperationException("You cannot access or read that scroll.");
		if (scroll.DataError is { } error) { actor.OutputHandler.Send($"Scroll disabled: {error}"); return; }
		if (scroll.Snapshot is not { } snapshot) { actor.OutputHandler.Send($"{(scroll.IsBlank ? "Blank" : "Spent")} scroll. {(scroll.Reservation is null ? "" : "Reserved for writing.")}"); return; }
		var service = VancianMagicService.For(actor.Gameworld);
		var capabilities = actor.Capabilities.OfType<IVancianMagicCapability>().Where(x => x.School.Id == snapshot.SchoolId).DistinctBy(x => x.Id).ToArray();
		if (!detailed && capabilities.Length == 0) { actor.OutputHandler.Send("The scroll contains stored magic from a school you cannot use."); return; }
		try
		{
			var spell = snapshot.CreateSpell(actor.Gameworld);
			actor.OutputHandler.Send($"{spell.Name.ColourName()}: base {snapshot.SpellLevel.ToString("N0", actor)}, stored casting level {snapshot.CastingLevel.ToString("N0", actor)}, power {snapshot.Power.DescribeEnum()}. {(scroll.Spent || service.Store.ItemConsumed(item.Id, snapshot.ChargeId) ? "Spent" : "Charged")}.");
			foreach (var capability in capabilities) { var ceiling = service.NormalScrollCeiling(actor, capability, spell); var difficulty = service.ScrollControlDifficulty(actor, capability, spell, snapshot.CastingLevel, ceiling); actor.OutputHandler.Send($"{capability.Name}: normal maximum {(ceiling < 0 ? "no finite capacity" : ceiling.ToString("N0", actor))}; {(difficulty.HasValue ? $"control check {difficulty.Value.DescribeEnum()}" : "no extra control check")}."); }
		}
		catch (Exception ex) { actor.OutputHandler.Send($"Stored spell is currently inert: {ex.Message}"); }
		if (detailed && actor.IsAdministrator()) actor.OutputHandler.Send(snapshot.Save().ToString());
	}
	public const string VancianAdminHelp = "magic vancian show|refresh|reset <character> <capability>\nmagic vancian known grant|revoke <character> <capability> <repertoire> <spell>\nmagic vancian operations <character> [capability]\nmagic vancian resolve <operation-id> acknowledge|cancel\nmagic vancian book add|remove <book> <spell>\nmagic vancian scroll show <scroll>";
	private static void VancianAdmin(ICharacter actor, StringStack command)
	{
		try
		{
			if (!actor.IsAdministrator()) throw new InvalidOperationException("Administrative permission is required.");
			var service = VancianMagicService.For(actor.Gameworld); var verb = command.PopForSwitch();
			if (verb is "" or "help" or "?") { actor.OutputHandler.Send(VancianAdminHelp); return; }
			if (verb == "scroll") { if (command.PopForSwitch() != "show") throw new InvalidOperationException("Use magic vancian scroll show <scroll>."); ShowSpellScroll(actor, WritingItem(actor, command.PopSpeech()), true); return; }
			if (verb == "book")
			{
				var op = command.PopForSwitch(); var item = WritingItem(actor, command.PopSpeech()); var spell = actor.Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new InvalidOperationException("No such spell.");
				if (op == "remove") ConfirmVancian(actor, $"remove {spell.Name} from that spellbook", () => service.AuthorBook(actor, item, spell, false));
				else if (op == "add") actor.OutputHandler.Send(service.AuthorBook(actor, item, spell, true).Message);
				else throw new InvalidOperationException("Use magic vancian book add|remove <book> <spell>."); return;
			}
			if (verb == "resolve") { var id = Guid.Parse(command.PopSpeech()); var action = command.PopForSwitch(); ConfirmVancian(actor, $"{action} operation {id}; indeterminate prog side effects need separate builder repair", () => service.ResolveOperation(actor, id, action)); return; }
			var knownAction = verb == "known" ? command.PopForSwitch() : "";
			var person = command.PopSpeech(); var target = long.TryParse(person, out var charId) ? actor.Gameworld.TryGetCharacter(charId, true) : actor.TargetActor(person);
			if (target is null) throw new InvalidOperationException("Specify an accessible character or a character ID.");
			if (verb == "operations") { var text = command.PopSpeech(); var filter = string.IsNullOrEmpty(text) ? null : VancianCapability(actor, text, administrative: true); actor.OutputHandler.Send(string.Join("\n", service.Store.Operations(VancianPolicy.Owner(target).Id, filter?.Id).Select(x => $"{x.Id} {x.Kind} {x.Status}, capability #{x.CapabilityId}, source #{x.SourceItem}, destination #{x.DestinationItem}, created {x.CreatedUtc.ToString("g", actor)} UTC: {x.Diagnostic}"))); return; }
			var capability = VancianCapability(actor, command.PopSpeech(), administrative: true);
			switch (verb)
			{
				case "show": actor.OutputHandler.Send(VancianStatus(actor, target, capability)); return;
				case "known":
					var rule = Repertoire(capability, command.PopSpeech()); var spell = VancianSpell(actor, capability, command.PopSpeech());
					if (knownAction is not ("grant" or "revoke")) throw new InvalidOperationException("Use known grant or known revoke.");
					ConfirmVancian(actor, $"{knownAction} {spell.Name} in {target.Name}'s {capability.Name} without player change hooks", () => service.AdministerKnown(actor, target, capability, rule.Key, spell, knownAction == "grant")); return;
				case "refresh": case "reset": ConfirmVancian(actor, $"force {verb} of {target.Name}'s {capability.Name}", () => service.AdministerState(actor, target, capability, verb)); return;
				default: actor.OutputHandler.Send(VancianAdminHelp); return;
			}
		}
		catch (Exception ex) { actor.OutputHandler.Send(ex.Message); }
	}
}
