using MudSharp.Effects.Concrete;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Capabilities;

public sealed partial class VancianMagicCapability
{
	public const string VancianHelp = @"Vancian capability settings (names containing spaces may be quoted):
	casterlevel <prog>; canchangeknown <prog>; onchangeknown <prog|none>; cancast <prog|none>
	basepower <power>; upcaststep <integer>; reliableoutcome <MinorPass|Pass|MajorPass>; maxloadouts <1-1000>
	repertoire add <alias> <Selected|Spellbook>; repertoire remove <alias>
	repertoire <alias> name|alias <text>; repertoire <alias> order <integer>
	repertoire <alias> levels <min> <max>; repertoire <alias> candidates <prog>
	repertoire <alias> limit <prog>; repertoire <alias> bookpolicy <EveryRefresh|PatternChangesOnly>
	allowance add <alias> <Memorised|Spontaneous|AtWill> <slotlevel|none>; allowance remove <alias>
	allowance <alias> name|alias <text>; allowance <alias> order <integer>
	allowance <alias> repertoire add|remove <repertoire>; allowance <alias> levels <min> <max>
	allowance <alias> count <prog>; allowance <alias> eligibility <prog|none>
	allowance <alias> mode <Memorised|Spontaneous|AtWill> <slotlevel|none>
	recovery mode <PreparationAction|SleepAutomatic|SleepThenPreparation>
	recovery preparetime|sleeptime|interval <timespan>; recovery canrefresh|onrefresh <prog|none>
	bookuse|transcribe|inscribe|oninscribe|scrolluse|scrolldifficulty <prog|none>
	scrolltrait <trait>; scrollthreshold <MinorPass|Pass|MajorPass>; check

Counts and caster levels are finite non-negative numeric prog results, floored per operation.
Policy progs use the canonical owner for progression/repertoire/recovery and the acting body for physical actions.
Candidate and permission progs must be side-effect free. Notifications run only after a committed change.
All concentration, inherent-power and regenerator settings from the standard capability editor remain available.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var setting = command.PopForSwitch();
		try
		{
			if (setting == "check") { actor.OutputHandler.Send(string.Join("\n", ConfigurationErrors().DefaultIfEmpty("This capability is ready."))); return true; }
			if (setting is "help" or "?") { actor.OutputHandler.Send(VancianHelp + "\n" + HelpText.SubstituteANSIColour()); return false; }
			if (_definitionError is not null) throw new InvalidOperationException($"Definition disabled: {_definitionError}");
			if (setting == "repertoire") return EditRepertoire(actor, command);
			if (setting == "allowance") return EditAllowance(actor, command);
			if (setting == "recovery")
			{
				setting = command.PopForSwitch();
				switch (setting)
				{
					case "mode": RecoveryMode = ParseEnum<VancianRecoveryMode>(command.SafeRemainingArgument); return Edited(actor);
					case "preparetime": PreparationDuration = Duration(command.SafeRemainingArgument, actor, true); return Edited(actor);
					case "sleeptime": RequiredSleepDuration = Duration(command.SafeRemainingArgument, actor, true); return Edited(actor);
					case "interval": MinimumRefreshInterval = Duration(command.SafeRemainingArgument, actor, false); return Edited(actor);
					case "canrefresh": case "onrefresh": break;
					default: throw new InvalidOperationException(VancianHelp);
				}
			}
			if (VancianPolicy.Signatures.ContainsKey(setting) && setting is not ("candidates" or "limit" or "count" or "eligibility"))
			{
				var id = ReadProg(command.SafeRemainingArgument, setting, setting is not ("casterlevel" or "canchangeknown"));
				if (id == 0) _policyProgs.Remove(setting); else _policyProgs[setting] = id;
				return Edited(actor);
			}
			switch (setting)
			{
				case "basepower": BasePower = ParseEnum<SpellPower>(command.SafeRemainingArgument); break;
				case "upcaststep": PowerStepPerSlotLevel = Nonnegative(command.SafeRemainingArgument); break;
				case "reliableoutcome": ReliableOutcome = SuccessfulOutcome(command.SafeRemainingArgument); break;
				case "scrollthreshold": ScrollMinimumOutcome = SuccessfulOutcome(command.SafeRemainingArgument); break;
				case "maxloadouts":
					var count = Nonnegative(command.SafeRemainingArgument);
					if (count is < 1 or > 1000) throw new InvalidOperationException("Use a loadout limit from 1 to 1000.");
					MaximumSavedLoadouts = count; break;
				case "scrolltrait": _scrollTraitId = (Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument) ?? throw new InvalidOperationException("No such trait.")).Id; break;
				default: return base.BuildingCommand(actor, command.GetUndo());
			}
			return Edited(actor);
		}
		catch (Exception ex) when (ex is InvalidOperationException or FormatException or OverflowException)
		{
			actor.OutputHandler.Send(ex.Message); return false;
		}
	}

	private long ReadProg(string text, string kind, bool optional = false)
	{
		if (optional && text.EqualTo("none")) return 0;
		var prog = Gameworld.FutureProgs.GetByIdOrName(text);
		if (!VancianPolicy.ValidSignature(prog, kind))
		{
			var signature = VancianPolicy.Signatures[kind];
			throw new InvalidOperationException($"{kind} requires a compiled {signature.Return.Describe()} prog ({string.Join(", ", signature.Parameters.Select(x => x.Describe()))}), accepting exactly these parameters.");
		}
		return prog!.Id;
	}

	private static T ParseEnum<T>(string text) where T : struct, Enum => text.TryParseEnum<T>(out var value) && Enum.IsDefined(value)
		? value : throw new InvalidOperationException($"Use one of {string.Join(", ", Enum.GetNames<T>())}.");
	private static int Nonnegative(string text) => int.TryParse(text, out var value) && value >= 0 ? value : throw new InvalidOperationException("Enter a non-negative integer.");
	private static Outcome SuccessfulOutcome(string text)
	{
		var outcome = ParseEnum<Outcome>(text);
		return outcome is Outcome.MinorPass or Outcome.Pass or Outcome.MajorPass ? outcome : throw new InvalidOperationException("Use MinorPass, Pass or MajorPass.");
	}
	private static TimeSpan Duration(string text, ICharacter actor, bool positive)
	{
		if (!TimeSpan.TryParse(text, actor, out var value) && !TimeSpanParserUtil.TimeSpanParser.TryParse(text, out value))
			throw new InvalidOperationException("Enter an elapsed timespan, for example 00:10:00 or 10 minutes.");
		if (value < TimeSpan.Zero || (positive && value == TimeSpan.Zero)) throw new InvalidOperationException("That duration must be positive (the refresh interval may be zero).");
		return value;
	}
	private bool Edited(ICharacter actor) { Changed = true; actor.OutputHandler.Send("Vancian configuration updated. Use check to inspect readiness."); return true; }
	private bool Confirm(ICharacter actor, string message, Func<bool> stillCurrent, Action change)
	{
		actor.OutputHandler.Send($"{message} Historical state is retained and cannot refill from this edit. Use ACCEPT VANCIAN to confirm.");
		actor.AddEffect(new Accept(actor, new GenericProposal(_ =>
		{
			if (!stillCurrent()) { actor.OutputHandler.Send("The configuration changed; repeat the edit."); return; }
			change(); Edited(actor);
		}, _ => actor.OutputHandler.Send("Edit cancelled."), () => actor.OutputHandler.Send("Edit expired."), message, "vancian")), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool EditRepertoire(ICharacter actor, StringStack command)
	{
		var alias = command.PopSpeech();
		if (alias.EqualTo("add"))
		{
			alias = command.PopSpeech();
			if (string.IsNullOrWhiteSpace(alias) || alias.Length > 80 || _repertoires.Any(x => x.Alias.EqualTo(alias)) || _repertoires.Count >= 128) throw new InvalidOperationException("Use a unique repertoire alias (maximum 80 characters, 128 rules).");
			_repertoires.Add(new(Guid.NewGuid(), alias, alias, _repertoires.Count, ParseEnum<VancianRepertoireSource>(command.SafeRemainingArgument)));
			return Edited(actor);
		}
		if (alias.EqualTo("remove"))
		{
			var name = command.SafeRemainingArgument;
			var remove = _repertoires.Find(x => x.Alias.EqualTo(name)) ?? throw new InvalidOperationException("No such repertoire.");
			return Confirm(actor, $"Remove repertoire {remove.Alias} ({remove.Key})? Linked allowances: {string.Join(", ", _allowances.Where(x => x.RepertoireKeys.Contains(remove.Key)).Select(x => x.Alias))}.",
				() => _repertoires.Contains(remove), () => _repertoires.Remove(remove));
		}
		var rule = _repertoires.Find(x => x.Alias.EqualTo(alias)) ?? throw new InvalidOperationException("No such repertoire. Use repertoire add <alias> <Selected|Spellbook>.");
		var operation = command.PopForSwitch();
		var updated = rule;
		switch (operation)
		{
			case "name": updated = rule with { Name = ValidName(command.SafeRemainingArgument) }; break;
			case "alias":
				var name = ValidName(command.SafeRemainingArgument);
				if (_repertoires.Any(x => x != rule && x.Alias.EqualTo(name))) throw new InvalidOperationException("Alias already in use.");
				updated = rule with { Alias = name }; break;
			case "order": updated = rule with { SortOrder = Nonnegative(command.SafeRemainingArgument) }; break;
			case "levels":
				var min = Nonnegative(command.PopSpeech()); var max = Nonnegative(command.PopSpeech());
				if (max < min) throw new InvalidOperationException("Maximum must be at least minimum.");
				updated = rule with { MinimumSpellLevel = min, MaximumSpellLevel = max }; break;
			case "candidates": updated = rule with { CandidateProgId = ReadProg(command.SafeRemainingArgument, "candidates") }; break;
			case "limit" when rule.Source == VancianRepertoireSource.Selected: updated = rule with { SelectionLimitProgId = ReadProg(command.SafeRemainingArgument, "limit") }; break;
			case "bookpolicy" when rule.Source == VancianRepertoireSource.Spellbook: updated = rule with { BookPolicy = ParseEnum<VancianBookPolicy>(command.SafeRemainingArgument) }; break;
			default: throw new InvalidOperationException(VancianHelp);
		}
		_repertoires[_repertoires.IndexOf(rule)] = updated;
		return Edited(actor);
	}

	private bool EditAllowance(ICharacter actor, StringStack command)
	{
		var alias = command.PopSpeech();
		if (alias.EqualTo("add"))
		{
			alias = command.PopSpeech();
			if (string.IsNullOrWhiteSpace(alias) || alias.Length > 80 || _allowances.Any(x => x.Alias.EqualTo(alias)) || _allowances.Count >= 256) throw new InvalidOperationException("Use a unique allowance alias (maximum 80 characters, 256 allowances).");
			var mode = ParseEnum<VancianAllowanceMode>(command.PopSpeech());
			var levelText = command.PopSpeech();
			int? level = mode == VancianAllowanceMode.AtWill && levelText.EqualTo("none") ? null : Nonnegative(levelText);
			if (mode == VancianAllowanceMode.AtWill && level.HasValue) throw new InvalidOperationException("AtWill requires none for its level.");
			_allowances.Add(new(Guid.NewGuid(), alias, alias, _allowances.Count, mode, level, Array.AsReadOnly(Array.Empty<Guid>())));
			return Edited(actor);
		}
		if (alias.EqualTo("remove"))
		{
			var name = command.SafeRemainingArgument;
			var remove = _allowances.Find(x => x.Alias.EqualTo(name)) ?? throw new InvalidOperationException("No such allowance.");
			return Confirm(actor, $"Remove allowance {remove.Alias} ({remove.Key}) and suspend its slots?", () => _allowances.Contains(remove), () => _allowances.Remove(remove));
		}
		var allowance = _allowances.Find(x => x.Alias.EqualTo(alias)) ?? throw new InvalidOperationException("No such allowance. Use allowance add <alias> <mode> <slotlevel|none>.");
		var updated = allowance;
		var structural = false;
		switch (command.PopForSwitch())
		{
			case "name": updated = allowance with { Name = ValidName(command.SafeRemainingArgument) }; break;
			case "alias":
				var name = ValidName(command.SafeRemainingArgument);
				if (_allowances.Any(x => x != allowance && x.Alias.EqualTo(name))) throw new InvalidOperationException("Alias already in use.");
				updated = allowance with { Alias = name }; break;
			case "order": updated = allowance with { SortOrder = Nonnegative(command.SafeRemainingArgument) }; break;
			case "levels":
				var min = Nonnegative(command.PopSpeech()); var max = Nonnegative(command.PopSpeech());
				if (max < min) throw new InvalidOperationException("Maximum must be at least minimum.");
				updated = allowance with { MinimumSpellLevel = min, MaximumSpellLevel = max }; break;
			case "count" when allowance.Mode != VancianAllowanceMode.AtWill: updated = allowance with { SlotCountProgId = ReadProg(command.SafeRemainingArgument, "count") }; break;
			case "eligibility": updated = allowance with { SpellEligibilityProgId = ReadProg(command.SafeRemainingArgument, "eligibility", true) }; break;
			case "repertoire":
				var operation = command.PopForSwitch(); var ruleAlias = command.PopSpeech();
				var rule = _repertoires.Find(x => x.Alias.EqualTo(ruleAlias)) ?? throw new InvalidOperationException("No such repertoire.");
				if (allowance.Mode != VancianAllowanceMode.Memorised && rule.Source == VancianRepertoireSource.Spellbook) throw new InvalidOperationException("Only Memorised allowances support Spellbook repertoires.");
				var keys = allowance.RepertoireKeys.ToList();
				if (operation == "add" && !keys.Contains(rule.Key)) keys.Add(rule.Key);
				else if (operation == "remove") keys.Remove(rule.Key);
				else throw new InvalidOperationException("Use repertoire add|remove <repertoire>; duplicate links are not permitted.");
				updated = allowance with { RepertoireKeys = keys.AsReadOnly() }; structural = true; break;
			case "mode":
				var mode = ParseEnum<VancianAllowanceMode>(command.PopSpeech()); var levelText = command.PopSpeech();
				int? level = mode == VancianAllowanceMode.AtWill && levelText.EqualTo("none") ? null : Nonnegative(levelText);
				if (mode == VancianAllowanceMode.AtWill && level.HasValue || mode != VancianAllowanceMode.Memorised && allowance.RepertoireKeys.Any(key => _repertoires.Find(x => x.Key == key)?.Source != VancianRepertoireSource.Selected)) throw new InvalidOperationException("Incompatible allowance mode, level or book links.");
				updated = allowance with { Mode = mode, SlotLevel = level, SlotCountProgId = mode == VancianAllowanceMode.AtWill ? 0 : allowance.SlotCountProgId }; structural = true; break;
			default: throw new InvalidOperationException(VancianHelp);
		}
		if (structural)
		{
			updated = updated with { StructuralVersion = checked(allowance.StructuralVersion + 1) };
			return Confirm(actor, $"Change structure of {allowance.Alias} ({allowance.Key})? Existing slots suspend until refresh.",
				() => _allowances.Contains(allowance), () => _allowances[_allowances.IndexOf(allowance)] = updated);
		}
		_allowances[_allowances.IndexOf(allowance)] = updated;
		return Edited(actor);
	}

	private static string ValidName(string name) => !string.IsNullOrWhiteSpace(name) && name.Length <= 80 && !name.Any(char.IsControl)
		? name : throw new InvalidOperationException("Use a non-empty name of at most 80 printable characters.");

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(_definitionError is not null || School is null || ConcentrationTrait is null
			? $"Vancian capability #{Id}: {Name}\n" : base.Show(actor).Replace("Skill Level Based", "Vancian"));
		sb.AppendLine($"Base power: {BasePower.DescribeEnum()}, step: {PowerStepPerSlotLevel.ToString("N0", actor)}, reliable outcome: {ReliableOutcome.DescribeEnum()}");
		sb.AppendLine($"Recovery: {RecoveryMode.DescribeEnum()}; preparation {PreparationDuration.Describe(actor)}, sleep {RequiredSleepDuration.Describe(actor)}, interval {MinimumRefreshInterval.Describe(actor)}");
		sb.AppendLine($"Saved loadout limit: {MaximumSavedLoadouts.ToString("N0", actor)}; scroll trait: {ScrollCheckTrait?.Name ?? "missing"}; threshold: {ScrollMinimumOutcome.DescribeEnum()}");
		foreach (var (name, id) in _policyProgs) sb.AppendLine($"{name}: {Gameworld.FutureProgs.Get(id)?.MXPClickableFunctionNameWithId() ?? $"missing #{id}"}");
		foreach (var r in _repertoires.OrderBy(x => x.SortOrder)) sb.AppendLine($"Repertoire {r.Alias}: {r.Name} [{r.Key}], {r.Source.DescribeEnum()}, levels {r.MinimumSpellLevel}-{r.MaximumSpellLevel}, candidate #{r.CandidateProgId}, limit #{r.SelectionLimitProgId}, {r.BookPolicy.DescribeEnum()}");
		foreach (var a in _allowances.OrderBy(x => x.SortOrder)) sb.AppendLine($"Allowance {a.Alias}: {a.Name} [{a.Key}/v{a.StructuralVersion}], {a.Mode.DescribeEnum()}, slot {a.SlotLevel?.ToString("N0", actor) ?? "none"}, base levels {a.MinimumSpellLevel}-{a.MaximumSpellLevel}, count #{a.SlotCountProgId}, eligibility #{a.SpellEligibilityProgId}, links {string.Join(", ", a.RepertoireKeys.Select(k => _repertoires.Find(x => x.Key == k)?.Alias ?? k.ToString()))}");
		sb.AppendLine(string.Join("\n", ConfigurationErrors().DefaultIfEmpty("Ready.")));
		return sb.ToString();
	}
}
