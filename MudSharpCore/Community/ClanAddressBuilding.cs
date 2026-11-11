#nullable enable

using MudSharp.FutureProg;

namespace MudSharp.Community;

internal static class ClanAddressBuilding
{
	private const string Help = "Address options: <text|none>, set <text|none>, add <text|none> [from lower|peer|higher|outsider|any] [public] [prog <prog>], list, clear, remove <row>, move <row> before <row>, rule <row> text <text|none>, rule <row> from <relations>, rule <row> audience members|public, rule <row> prog <prog|none>. Appointments also support priority <integer>. Rows accept a displayed number or stable ID. A one-character predicate receives the subject; two receive speaker then subject. Null speakers try public outsider rules, then lower rules.";

	public static string Describe(IEnumerable<ClanAddressRule> rules, ICharacter actor)
	{
		var lines = rules.Select((x, i) => $"{(i + 1).ToString("N0", actor)}. {(x.Text ?? "none").ColourValue()} | from {x.Relations.DescribeEnum().ColourName()} | {(x.IsPublic ? "public" : "members")} | prog {x.FutureProgId?.ToString("N0", actor) ?? "none"} | ID {x.Id}").ToList();
		return "Forms of Address:\n" + (lines.Count == 0 ? "None (allows fallback)." : string.Join("\n", lines));
	}

	public static void Edit(ICharacter actor, StringStack command, IRank? rank = null, IAppointment? appointment = null)
	{
		var original = rank?.FormsOfAddress ?? appointment!.FormsOfAddress;
		if (command.IsFinished || command.PeekSpeech().EqualTo("list"))
		{
			actor.OutputHandler.Send(Describe(original, actor) + $"\n{(appointment is null ? "" : $"Address priority: {appointment.AddressPriority.ToString("N0", actor)}\n")}{Help}");
			return;
		}
		var action = command.PopSpeech();
		if (action.EqualTo("priority"))
		{
			if (appointment is null || !int.TryParse(command.SafeRemainingArgument, out var priority))
			{
				actor.OutputHandler.Send("Specify an integer address priority for an appointment.");
				return;
			}
			appointment.AddressPriority = priority;
			appointment.Changed = true;
			actor.OutputHandler.Send($"Address priority is now {priority.ToString("N0", actor).ColourValue()}.");
			return;
		}
		var rules = original.Select(x => { var copy = x.Copy(); copy.Id = x.Id; return copy; }).ToList();
		ClanAddressRule? Find(string text) => int.TryParse(text, out var index) ? rules.ElementAtOrDefault(index - 1) :
			Guid.TryParse(text, out var id) ? rules.FirstOrDefault(x => x.Id == id) : null;
		bool Text(string input, out string? text)
		{
			text = input.EqualTo("none") ? null : input;
			if (text is null || !string.IsNullOrWhiteSpace(text) && text.Length <= 200) return true;
			actor.OutputHandler.Send("Specify an address of 1–200 characters, or none to suppress fallback.");
			return false;
		}
		bool Relation(string input, out ClanAddressRelation relation)
		{
			if (Enum.TryParse(input.Replace('|', ','), true, out relation) && relation != 0 && (relation & ~ClanAddressRelation.Any) == 0) return true;
			actor.OutputHandler.Send("Use lower, peer, higher, outsider or any; combine with |.");
			return false;
		}
		bool Prog(string input, out long? id)
		{
			id = null;
			if (input.EqualTo("none")) return true;
			var prog = new ProgLookupFromBuilderInputMultipleReturnTypes(actor, input, ProgVariableTypes.Boolean,
				new IEnumerable<ProgVariableTypes>[] { [ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Character] }).LookupProg();
			if (prog is null) return false;
			if (ClanTemplateProvisioning.IsGeneratedAddressProg(prog))
			{ actor.OutputHandler.Send("Generated clan-bound address predicates keep their recipe binding. Edit the existing row or select a shared utility predicate instead."); return false; }
			if (prog.AcceptsAnyParameters) { actor.OutputHandler.Send("Use a fixed one- or two-character predicate signature."); return false; }
			id = prog.Id;
			return true;
		}

		switch (action.ToLowerInvariant())
		{
			case "clear": rules.Clear(); break;
			case "remove":
				var remove = Find(command.PopSpeech());
				if (remove is null) { actor.OutputHandler.Send("There is no such address row."); return; }
				rules.Remove(remove);
				break;
			case "move":
				var moving = Find(command.PopSpeech());
				var direction = command.PopSpeech();
				var before = Find(command.PopSpeech());
				if (moving is null || before is null || moving == before || !direction.EqualTo("before"))
				{ actor.OutputHandler.Send("Use move <row> before <other-row>."); return; }
				rules.Remove(moving); rules.Insert(rules.IndexOf(before), moving);
				break;
			case "rule":
				var rule = Find(command.PopSpeech());
				if (rule is null) { actor.OutputHandler.Send("There is no such address row."); return; }
				switch (command.PopSpeech().ToLowerInvariant())
				{
					case "text": if (!Text(command.SafeRemainingArgument, out var text)) return; rule.Text = text; break;
					case "from": if (!Relation(command.SafeRemainingArgument, out var relation)) return; rule.Relations = relation; break;
					case "audience":
						var audience = command.SafeRemainingArgument;
						if (!audience.EqualTo("members") && !audience.EqualTo("public")) { actor.OutputHandler.Send("Choose members or public."); return; }
						rule.IsPublic = audience.EqualTo("public"); break;
					case "prog": if (!Prog(command.SafeRemainingArgument, out var id)) return; rule.FutureProgId = id; rule.TemplateKey = null; break;
					default: actor.OutputHandler.Send(Help); return;
				}
				break;
			case "add":
				if (!Text(command.PopSpeech(), out var addedText)) return;
				var added = new ClanAddressRule { Text = addedText };
				while (!command.IsFinished)
				{
					switch (command.PopSpeech().ToLowerInvariant())
					{
						case "from": if (!Relation(command.PopSpeech(), out var relation)) return; added.Relations = relation; break;
						case "public": added.IsPublic = true; break;
						case "prog": if (!Prog(command.PopSpeech(), out var id)) return; added.FutureProgId = id; break;
						default: actor.OutputHandler.Send(Help); return;
					}
				}
				var position = rules.FindIndex(IsDefault);
				rules.Insert(position >= 0 ? position : rules.Count, added);
				break;
			default:
				var defaultText = action.EqualTo("set") ? command.SafeRemainingArgument : action + (command.IsFinished ? "" : " " + command.SafeRemainingArgument);
				if (!Text(defaultText, out var defaultAddress)) return;
				var fallback = rules.FirstOrDefault(IsDefault);
				if (fallback is null) rules.Add(new ClanAddressRule { Text = defaultAddress });
				else { fallback.Text = defaultAddress; fallback.TemplateKey = null; }
				break;
		}
		if (rules.Count(IsDefault) > 1) { actor.OutputHandler.Send("Only one unconditional any-relation default is allowed. Add a relation or predicate to exceptions."); return; }
		if (rules.FirstOrDefault(IsDefault) is { } last) { rules.Remove(last); rules.Add(last); }
		original.Clear(); original.AddRange(rules);
		if (rank is not null) rank.Changed = true; else appointment!.Changed = true;
		actor.OutputHandler.Send("The forms of address have been updated.\n" + Describe(original, actor));
	}

	private static bool IsDefault(ClanAddressRule rule) => rule.Relations == ClanAddressRelation.Any && rule.FutureProgId is null;
}
