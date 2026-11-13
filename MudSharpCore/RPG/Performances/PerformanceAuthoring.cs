#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.Framework;

namespace MudSharp.RPG.Performances;

/// <summary>
/// Actor-aware authoring operations for reusable performance works. The service stores
/// canonical work records; this class owns editor visibility, fixture instantiation and
/// structural validation before publication.
/// </summary>
public static class PerformanceAuthoring
{
	private sealed record Fixture(string Key, string Name, Func<PerformanceWork> Build);

	private static readonly IReadOnlyList<Fixture> Fixtures = new[]
	{
		new Fixture("mass", "Parish High Mass", BuildMass),
		new Fixture("concert", "Here Comes the Sun", BuildConcert),
		new Fixture("procession", "Lantern Procession", BuildProcession)
	};

	public static void Execute(ICharacter actor, StringStack args, PerformanceService service)
	{
		if (args.IsFinished)
		{
			actor.OutputHandler.Send(HelpFor(actor));
			return;
		}

		var command = args.PopSpeech().ToLowerInvariant();
		switch (command)
		{
			case "help":
			case "?":
				actor.OutputHandler.Send(HelpFor(actor));
				return;
			case "list":
				List(actor, service);
				return;
			case "fixtures":
				actor.OutputHandler.Send("Available fixture templates: " +
				                          string.Join(", ", Fixtures.Select(x => $"{x.Key} ({x.Name})")) + ".");
				return;
			case "new":
				New(actor, args, service);
				return;
			case "fixture":
				FixtureWork(actor, args, service);
				return;
			case "copy":
				Copy(actor, args, service);
				return;
			case "show":
				Show(actor, args, service);
				return;
			case "edit":
				Edit(actor, args, service);
				return;
			case "validate":
				Validate(actor, args, service, false);
				return;
			case "publish":
				Validate(actor, args, service, true);
				return;
			default:
				actor.OutputHandler.Send($"Unknown work command {command.ColourCommand()}. Use {"performance work help".ColourCommand()} to see the editor commands.");
				return;
		}
	}

	/// <summary>Returns a structural validation error, or null when the saved definition is structurally sound.</summary>
	public static string? ValidateDefinition(PerformanceWork work)
	{
		if (string.IsNullOrWhiteSpace(work.Name))
		{
			return "A work needs a name.";
		}

		if (work.Roles.Count == 0)
		{
			return "A work needs at least one role.";
		}

		var roles = work.Roles.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		if (roles.Count != work.Roles.Count || work.Roles.Any(x => string.IsNullOrWhiteSpace(x.Key) ||
		                                                           !IsCanonicalKey(x.Key) ||
		                                                           string.IsNullOrWhiteSpace(x.Mark) ||
		                                                           !IsCanonicalKey(x.Mark) ||
		                                                           !Enum.IsDefined(x.Mode)))
		{
			return "Role keys and starting marks must be non-empty, role keys must be unique, and modes must be valid.";
		}

		if (work.Places.Any(x => !IsCanonicalKey(x)) || work.Places.Distinct(StringComparer.Ordinal).Count() != work.Places.Count)
		{
			return "Place keys must be non-empty and unique.";
		}

		if (work.Props.Any(x => !IsCanonicalKey(x)) || work.Props.Distinct(StringComparer.Ordinal).Count() != work.Props.Count)
		{
			return "Prop keys must be non-empty and unique.";
		}
		if (work.LeaveProps.Any(key => !work.Props.Contains(key, StringComparer.Ordinal)))
		{
			return "A prop cleanup policy names a prop that is not declared in this work.";
		}
		if (work.Roles.Any(role => role.EligibilityProgId < 0))
		{
			return "A role has an unresolved eligibility rule. Replace it with a compatible prog or clear the rule.";
		}

		var places = work.Places.ToHashSet(StringComparer.Ordinal);
		var misplaced = work.Roles.FirstOrDefault(x => !places.Contains(x.Mark));
		if (misplaced is not null)
		{
			return $"Role '{misplaced.Key}' uses undeclared starting mark '{misplaced.Mark}'. Add that place or change the role's mark.";
		}

		var exits = work.Exits.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		if (exits.Count != work.Exits.Count || work.Exits.Any(x => !IsCanonicalKey(x.Key) || !IsCanonicalKey(x.From) || !IsCanonicalKey(x.To) ||
		                                                           !places.Contains(x.From) || !places.Contains(x.To)))
		{
			return "Exit keys must be unique and both ends must name declared places.";
		}

		foreach (var cue in work.Cues)
		{
			if (!Enum.IsDefined(cue.Verb))
			{
				return "A cue has an unknown action.";
			}

			if (!string.IsNullOrWhiteSpace(cue.Role) && !roles.Contains(cue.Role))
			{
				return $"Cue refers to unknown role '{cue.Role}'.";
			}

			if (!string.IsNullOrWhiteSpace(cue.Place) && !places.Contains(cue.Place))
			{
				return $"Cue refers to unknown place '{cue.Place}'.";
			}
			if (cue.Verb is not (PerformanceVerb.Wait or PerformanceVerb.Stage or PerformanceVerb.Unstage) && string.IsNullOrWhiteSpace(cue.Role))
			{
				return "Only wait and stage operations can omit their acting role.";
			}

			if (cue.Verb is not PerformanceVerb.Wait && string.IsNullOrWhiteSpace(cue.Place))
			{
				return "An action cue needs a named place.";
			}

			if (cue.Seconds < 0)
			{
				return "Cue durations cannot be negative.";
			}

			if ((cue.Verb == PerformanceVerb.Guard || cue.Verb == PerformanceVerb.Travel && cue.Target.Length > 0) && !exits.Contains(cue.Target))
			{
				return $"Cue refers to unknown exit '{cue.Target}'.";
			}

			if ((cue.Verb is PerformanceVerb.Give or PerformanceVerb.Get or PerformanceVerb.Play or PerformanceVerb.Stop) &&
			    !work.Props.Contains(cue.Target, StringComparer.Ordinal))
			{
				return $"Cue refers to unknown prop '{cue.Target}'.";
			}

			if (cue.Verb == PerformanceVerb.Give && !roles.Contains(cue.Text))
			{
				return $"Give cue refers to unknown recipient role '{cue.Text}'.";
			}
			if (cue.Verb == PerformanceVerb.Give && work.Roles.Any(r => r.Key == cue.Text && !r.Required))
				return "A handoff requires a recipient who will be present at every staging.";
			if (cue.Verb is PerformanceVerb.Guard or PerformanceVerb.Travel && cue.Target.Length > 0)
			{
				var leg = work.Exits.First(x => x.Key == cue.Target);
				if (cue.Place != (cue.Verb == PerformanceVerb.Guard ? leg.From : leg.To))
					return $"The {cue.Verb} cue for '{cue.Role}' uses '{cue.Place}'. Exit '{leg.Key}' requires '{(cue.Verb == PerformanceVerb.Guard ? leg.From : leg.To)}' for this action.";
			}

			if (cue.Verb is PerformanceVerb.Speak or PerformanceVerb.Sing or PerformanceVerb.Emote &&
			    string.IsNullOrWhiteSpace(cue.Text))
			{
				return $"A {cue.Verb.ToString().ToLowerInvariant()} cue needs authored text.";
			}

			if (cue.Verb is PerformanceVerb.Stage or PerformanceVerb.Unstage)
			{
				if (!places.Contains(cue.Target))
				{
					return "Each stage or unstage cue must target one declared audience place.";
				}
				if (cue.Place == cue.Target) return "A stage and its audience must be in different places.";
			}
		}

		if (work.Cues.Count == 0)
		{
			return "A work needs at least one cue.";
		}

		var cuedRoles = work.Cues.Where(x => !string.IsNullOrWhiteSpace(x.Role))
			.Select(x => x.Role)
			.ToHashSet(StringComparer.Ordinal);
		var uncued = work.Roles.FirstOrDefault(x => x.Required && !cuedRoles.Contains(x.Key));
		if (uncued is not null)
		{
			return $"Required role '{uncued.Key}' has no cue. Add a cue for this role before publishing.";
		}
		if (!string.IsNullOrWhiteSpace(work.FailureRole) && !roles.Contains(work.FailureRole))
		{
			return $"Failure direction refers to unknown role '{work.FailureRole}'.";
		}

		return null;
	}

	/// <summary>Validates builder-visible rule references after validating the serialisable definition.</summary>
	public static string? Validate(ICharacter actor, PerformanceWork work)
	{
		var error = ValidateDefinition(work);
		if (error is not null) return error;

		foreach (var role in work.Roles.Where(x => x.EligibilityProgId != 0))
		{
			var savedProg = actor.Gameworld.FutureProgs.Get(role.EligibilityProgId);
			if (!UsableRule(savedProg, ProgVariableTypes.Character, actor.IsAdministrator()))
			{
				return $"Role '{role.Key}' has an unavailable eligibility rule. Replace it with a visible Boolean prog accepting a Character, or clear the rule.";
			}
		}

		return null;
	}

	internal static PerformanceWork CreateFixtureTemplate(string key) =>
		Fixtures.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Build()
		?? throw new ArgumentException($"Unknown performance fixture '{key}'.", nameof(key));

	private static void List(ICharacter actor, PerformanceService service)
	{
		var visible = service.Works
			.Where(x => CanView(actor, x))
			.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ThenBy(x => x.Revision)
			.ToList();
		if (visible.Count == 0)
		{
			actor.OutputHandler.Send("There are no performance works available to you.");
			return;
		}

		var sb = new StringBuilder("Performance works".ColourName() + ":\n");
		foreach (var work in visible)
		{
			var identifier = actor.IsAdministrator() ? $"#{work.Id.ToString("D", actor).ColourValue()} " : "";
			sb.AppendLine($"  {identifier}{work.Name.ColourName()} (revision {work.Revision.ToString("N0", actor).ColourValue()}{(work.Published ? ", published".ColourValue() : ", draft".ColourCommand())})" +
			              (work.OwnerId == actor.Account.Id ? " [yours]" : string.Empty));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void New(ICharacter actor, StringStack args, PerformanceService service)
	{
		var name = args.SafeRemainingArgument.Trim();
		if (name.Length == 0)
		{
			actor.OutputHandler.Send("What should the new work be called?");
			return;
		}

		if (service.Works.Any(x => x.OwnerId == actor.Account.Id && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && !x.Published))
		{
			actor.OutputHandler.Send("You already have an unpublished work with that name.");
			return;
		}

		service.SaveWork(new PerformanceWork { OwnerId = actor.Account.Id, Name = name });
		actor.OutputHandler.Send($"You create draft work {name.ColourName()}. Add roles, places, props and cues, then validate and publish it.".SubstituteANSIColour());
	}

	private static void FixtureWork(ICharacter actor, StringStack args, PerformanceService service)
	{
		var key = args.PopSpeech().ToLowerInvariant();
		var fixture = Fixtures.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
		if (fixture is null)
		{
			actor.OutputHandler.Send("Choose one of the listed fixture keys: " + string.Join(", ", Fixtures.Select(x => x.Key)) + ".");
			return;
		}

		var work = fixture.Build();
		var overrideName = args.SafeRemainingArgument.Trim();
		work.Name = overrideName.Length == 0 ? fixture.Name : overrideName;
		work.OwnerId = actor.Account.Id;
		work.Revision = 1;
		work.Id = 0;
		work.Published = false;
		work.StaffAuthored = false;
		service.SaveWork(work);
		actor.OutputHandler.Send($"You create {work.Name.ColourName()} as a draft from the {fixture.Name.ColourName()} fixture. Role, place, prop and cue references are symbolic. Bind them to your local production when staging.".SubstituteANSIColour());
	}

	private static void Copy(ICharacter actor, StringStack args, PerformanceService service)
	{
		if (args.IsFinished)
		{
			actor.OutputHandler.Send("Copy which visible work?");
			return;
		}

		var selector = args.PopSpeech();
		var source = Resolve(actor, selector, service);
		var requestedName = args.SafeRemainingArgument.Trim();
		var copy = source is null ? null : CreateCopy(actor, source, requestedName);
		if (source is null || copy is null)
		{
			actor.OutputHandler.Send(WorkLookupError(selector, false));
			return;
		}

		service.SaveWork(copy);
		actor.OutputHandler.Send($"You copy '{source.Name}' as your draft '{copy.Name}'. Validate the copy and replace any unavailable eligibility rules before publishing.");
	}

	internal static PerformanceWork? CreateCopy(ICharacter actor, PerformanceWork source, string requestedName)
	{
		if (!CanView(actor, source) || (!source.Published && !CanEdit(actor, source))) return null;
		var copy = CloneForEdit(source, actor.Account.Id, forkRevision: true);
		copy.Name = string.IsNullOrWhiteSpace(requestedName) ? $"{source.Name} (copy)" : requestedName.Trim();
		copy.Revision = 1;
		copy.Published = false;
		copy.StaffAuthored = false;
		if (!actor.IsAdministrator())
		{
			foreach (var role in copy.Roles.Where(x => x.EligibilityProgId > 0))
			{
				var prog = actor.Gameworld.FutureProgs.Get(role.EligibilityProgId);
				if (prog is null || !prog.Public)
				{
					role.EligibilityProgId = -1;
				}
			}
		}

		return copy;
	}

	private static void Show(ICharacter actor, StringStack args, PerformanceService service)
	{
		var selector = args.SafeRemainingArgument;
		var work = Resolve(actor, selector, service);
		if (work is null)
		{
			actor.OutputHandler.Send(WorkLookupError(selector, false));
			return;
		}

		actor.OutputHandler.Send(Describe(actor, work));
	}

	private static void Edit(ICharacter actor, StringStack args, PerformanceService service)
	{
		if (args.IsFinished)
		{
			actor.OutputHandler.Send("Edit what? Use 'performance work edit <work> name <new name>', or 'role/place/prop/exit/cue/failure' with the syntax in help.");
			return;
		}

		var selector = args.PopSpeech();
		var work = Resolve(actor, selector, service);
		if (work is null || !CanEdit(actor, work))
		{
			actor.OutputHandler.Send(WorkLookupError(selector, true));
			return;
		}

		var candidate = CloneForEdit(work, actor.Account.Id, forkRevision: work.Published);

		if (args.IsFinished)
		{
			actor.OutputHandler.Send("What field do you want to edit?");
			return;
		}

		var field = args.PopSpeech().ToLowerInvariant();
		var error = field switch
		{
			"name" => EditName(candidate, args),
			"role" => EditRole(actor, candidate, args),
			"place" => EditPlace(candidate, args),
			"prop" => EditProp(candidate, args),
			"exit" => EditExit(candidate, args),
			"cue" => EditCue(candidate, args),
			"failure" => EditFailure(candidate, args),
			_ => $"'{field}' is not an editable field. Choose name, role, place, prop, exit, cue or failure."
		};
		if (error is not null)
		{
			actor.OutputHandler.Send($"{error.ColourError()}\nUse {"performance work help".ColourCommand()} to review editor syntax.");
			return;
		}

		candidate.Published = false;
		candidate.StaffAuthored = false;
		service.SaveWork(candidate);
		actor.OutputHandler.Send($"Draft {candidate.Name.ColourName()} updated. Run {$"performance work validate \"{candidate.Name}\"".ColourCommand()} before publishing.");
	}

	private static string? EditName(PerformanceWork work, StringStack args)
	{
		var value = args.SafeRemainingArgument.Trim();
		if (value.Length == 0) return "A work name is required. Add the new name after the 'name' field.";
		work.Name = value;
		return null;
	}

	private static string? EditRole(ICharacter actor, PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Choose role add, remove, cleanup or eligibility.";
		var operation = args.PopSpeech().ToLowerInvariant();
		if (operation == "cleanup")
		{
			if (args.IsFinished) return "Specify the role key and return or release policy. Example: role cleanup usher return.";
			var key = NormalizeKey(args.PopSpeech());
			var role = work.Roles.FirstOrDefault(x => x.Key == key);
			var policy = NormalizeKey(args.SafeRemainingArgument);
			if (role is null) return $"There is no role with key '{key}'.";
			if (role.Mode == PerformanceMode.Pc) return $"Role '{key}' is PC-only. NPC return policy applies only to roles that allow NPC performers.";
			if (policy is not ("return" or "release")) return $"'{policy}' is not an NPC cleanup policy. Choose return or release.";
			role.ReturnAfterPerformance = policy == "return";
			return null;
		}
		if (operation == "eligibility")
		{
			if (args.IsFinished) return "Specify a role key and a compatible prog name, or clear. Example: role eligibility reader CanRead.";
			var key = NormalizeKey(args.PopSpeech());
			var role = work.Roles.FirstOrDefault(x => x.Key == key);
			if (role is null) return $"There is no role with key '{key}'.";
			if (args.IsFinished) return $"Specify the eligibility prog for role '{key}', or clear to remove its rule.";
			var progInput = args.SafeRemainingArgument.Trim();
			if (progInput.Equals("clear", StringComparison.OrdinalIgnoreCase))
			{
				role.EligibilityProgId = 0;
				return null;
			}

			var prog = FindRule(actor, progInput, ProgVariableTypes.Character, actor.IsAdministrator());
			if (prog is null)
			{
				var selector = actor.IsAdministrator() ? "a compatible prog name or ID" : "a public prog name";
				return $"'{progInput}' is unavailable for this role. Enter {selector} that returns Boolean and accepts a Character, or clear the rule.";
			}
			role.EligibilityProgId = prog.Id;
			return null;
		}

		if (operation == "remove")
		{
			if (args.IsFinished) return "Specify the role key to remove.";
			var key = NormalizeKey(args.PopSpeech());
			if (work.Roles.RemoveAll(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) == 0)
				return $"There is no role with key '{key}'.";
			return null;
		}

		if (operation != "add") return $"'{operation}' is not a role operation. Choose add, remove, cleanup or eligibility.";
		if (args.IsFinished) return "Role add syntax: role add <key> <npc|pc|either> <true|false> <starting mark>.";
		var keyValue = NormalizeKey(args.PopSpeech());
		var modeValue = args.PopSpeech();
		var requiredValue = args.PopSpeech();
		var mark = NormalizeKey(args.SafeRemainingArgument);
		if (string.IsNullOrWhiteSpace(keyValue)) return "A role key is required.";
		if (!IsCanonicalKey(keyValue)) return $"'{keyValue}' is not a valid role key. Supply a non-empty symbolic key.";
		if (work.Roles.Any(x => x.Key.Equals(keyValue, StringComparison.OrdinalIgnoreCase))) return $"Role '{keyValue}' already exists.";
		if (!Enum.TryParse(modeValue, true, out PerformanceMode mode) || !Enum.IsDefined(mode))
			return $"'{modeValue}' is not a valid role mode. Choose npc, pc or either.";
		if (!bool.TryParse(requiredValue, out var required))
			return $"'{requiredValue}' is not a valid required flag. Choose true or false.";
		if (string.IsNullOrWhiteSpace(mark)) return "A starting mark is required.";
		if (!IsCanonicalKey(mark)) return $"'{mark}' is not a valid starting mark key. Supply a non-empty symbolic key.";
		work.Roles.Add(new PerformanceRole { Key = keyValue, Mode = mode, Required = required, Mark = mark });
		return null;
	}

	private static string? EditPlace(PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Choose place add or remove, followed by a symbolic key.";
		var operation = args.PopSpeech().ToLowerInvariant();
		var key = NormalizeKey(args.SafeRemainingArgument);
		if (key.Length == 0) return $"A place key is required for the '{operation}' operation.";
		if (!IsCanonicalKey(key)) return $"'{key}' is not a valid place key. Supply a named place or item key.";
		if (operation == "add")
		{
			if (work.Places.Contains(key, StringComparer.OrdinalIgnoreCase)) return $"Place '{key}' already exists.";
			work.Places.Add(key);
			return null;
		}
		if (operation != "remove") return $"'{operation}' is not a place operation. Choose add or remove.";
		if (work.Places.RemoveAll(x => x.Equals(key, StringComparison.OrdinalIgnoreCase)) == 0) return $"There is no place with key '{key}'.";
		return null;
	}

	private static string? EditProp(PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Choose prop add, remove or cleanup.";
		var operation = args.PopSpeech().ToLowerInvariant();
		if (operation == "cleanup")
		{
			if (args.IsFinished) return "Specify the prop key and return or leave policy. Example: prop cleanup lantern return.";
			var prop = NormalizeKey(args.PopSpeech());
			var mode = NormalizeKey(args.SafeRemainingArgument);
			if (!work.Props.Contains(prop, StringComparer.Ordinal)) return $"There is no prop with key '{prop}'.";
			if (mode is not ("return" or "leave")) return $"'{mode}' is not a prop cleanup policy. Choose return or leave.";
			work.LeaveProps.Remove(prop);
			if (mode == "leave") work.LeaveProps.Add(prop);
			return null;
		}
		var key = NormalizeKey(args.SafeRemainingArgument);
		if (key.Length == 0) return $"A prop key is required for the '{operation}' operation.";
		if (!IsCanonicalKey(key)) return $"'{key}' is not a valid prop key. Supply a named place or item key.";
		if (operation == "add")
		{
			if (work.Props.Contains(key, StringComparer.OrdinalIgnoreCase)) return $"Prop '{key}' already exists.";
			work.Props.Add(key);
			return null;
		}
		if (operation != "remove") return $"'{operation}' is not a prop operation. Choose add, remove or cleanup.";
		if (!work.Props.Remove(key)) return $"There is no prop with key '{key}'.";
		work.LeaveProps.Remove(key);
		return null;
	}

	private static string? EditExit(PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Choose exit add or remove.";
		var operation = args.PopSpeech().ToLowerInvariant();
		if (operation == "remove")
		{
			if (args.IsFinished) return "Specify the exit key to remove.";
			var key = NormalizeKey(args.PopSpeech());
			if (work.Exits.RemoveAll(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) == 0) return $"There is no exit with key '{key}'.";
			return null;
		}
		if (operation != "add") return $"'{operation}' is not an exit operation. Choose add or remove.";
		if (args.IsFinished) return "Exit add syntax: exit add <key> <from-place> <to-place>.";
		var keyValue = NormalizeKey(args.PopSpeech());
		var fromValue = NormalizeKey(args.PopSpeech());
		var toValue = NormalizeKey(args.SafeRemainingArgument);
		if (string.IsNullOrWhiteSpace(keyValue)) return "An exit key is required.";
		if (!IsCanonicalKey(keyValue)) return $"'{keyValue}' is not a valid exit key. Supply a non-empty symbolic key.";
		if (work.Exits.Any(x => x.Key.Equals(keyValue, StringComparison.OrdinalIgnoreCase))) return $"Exit '{keyValue}' already exists.";
		if (string.IsNullOrWhiteSpace(fromValue) || string.IsNullOrWhiteSpace(toValue)) return "Both the origin and destination place keys are required.";
		if (!work.Places.Contains(fromValue, StringComparer.Ordinal)) return $"Origin place '{fromValue}' is not declared in this work. Add the place first.";
		if (!work.Places.Contains(toValue, StringComparer.Ordinal)) return $"Destination place '{toValue}' is not declared in this work. Add the place first.";
		if (fromValue == toValue) return "An exit must connect two different places.";
		var exit = new PerformanceExit { Key = keyValue, From = fromValue, To = toValue };
		work.Exits.Add(exit);
		return null;
	}

	internal static string? NonNegativeIntegerError(string value, string field)
	{
		if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
			return $"The text '{value}' is not a valid whole number. {field} must be a whole number of 0 or more.";
		return parsed < 0 ? $"{field} must be 0 or more. You entered {parsed}." : null;
	}

	private static string? EditCue(PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Choose cue add or remove.";
		var operation = args.PopSpeech().ToLowerInvariant();
		if (operation == "remove")
		{
			if (work.Cues.Count == 0) return "There are no cues to remove.";
			if (args.IsFinished) return "Specify the one-based cue number to remove.";
			var value = args.PopSpeech();
			if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
				return $"The text '{value}' is not a valid cue number. Choose a whole number from 1 to {work.Cues.Count.ToString(CultureInfo.InvariantCulture)}.";
			if (index < 1 || index > work.Cues.Count)
				return work.Cues.Count == 1
					? $"Cue number {value} is outside the valid range. This work has one cue. Use cue remove 1."
					: $"Cue number {value} is outside the valid range. Choose a cue from 1 through {work.Cues.Count.ToString(CultureInfo.InvariantCulture)}.";
			work.Cues.RemoveAt(index - 1);
			return null;
		}
		if (operation != "add") return $"'{operation}' is not a cue operation. Choose add or remove.";
		if (args.IsFinished) return "Cue add syntax: cue add <role|-> <place|-> <verb> <target|-> <seconds> <text>.";

		var role = NormalizeKey(args.PopSpeech());
		var place = NormalizeKey(args.PopSpeech());
		if (args.IsFinished) return "Specify a cue verb after its role and place.";
		var verbText = args.PopSpeech();
		if (!Enum.TryParse(verbText, true, out PerformanceVerb verb) || !Enum.IsDefined(verb))
			return $"'{verbText}' is not a supported cue action. Choose speak, sing, emote, travel, give, get, play, stop, guard, stage, unstage or wait.";
		if (args.IsFinished) return $"Specify a target for the {verb.DescribeEnum()} cue, or '-' when this action has no target.";
		var target = NormalizeKey(args.PopSpeech());
		if (args.IsFinished) return "Specify the cue duration in whole seconds before the cue text.";
		var secondsText = args.PopSpeech();
		var secondsError = NonNegativeIntegerError(secondsText, "Cue duration");
		if (secondsError is not null) return secondsError;
		var seconds = int.Parse(secondsText, NumberStyles.Integer, CultureInfo.InvariantCulture);
		var text = args.SafeRemainingArgument.Trim();
		if (verb is PerformanceVerb.Speak or PerformanceVerb.Sing or PerformanceVerb.Emote && text.Length == 0)
			return $"A {verb.DescribeEnum()} cue needs text after the duration.";
		work.Cues.Add(new PerformanceCue { Role = role == "-" ? string.Empty : role, Place = place == "-" ? string.Empty : place,
		                                  Verb = verb, Target = target == "-" ? string.Empty : target, Seconds = seconds, Text = text });
		return null;
	}

	private static string? EditFailure(PerformanceWork work, StringStack args)
	{
		if (args.IsFinished) return "Specify a failure speaker role or '-' followed by the cancellation announcement.";
		var role = args.PopSpeech();
		var text = args.SafeRemainingArgument.Trim();
		if (text.Length == 0) return "Failure text is required after the speaker role.";
		work.FailureRole = role == "-" ? string.Empty : NormalizeKey(role);
		work.FailureText = text;
		return null;
	}

	private static void Validate(ICharacter actor, StringStack args, PerformanceService service, bool publish)
	{
		var selector = args.SafeRemainingArgument;
		var work = Resolve(actor, selector, service);
		if (work is null || !CanEdit(actor, work))
		{
			actor.OutputHandler.Send(WorkLookupError(selector, true));
			return;
		}

		var error = publish ? Publish(actor, work) : Validate(actor, work);
		if (error is not null)
		{
			actor.OutputHandler.Send($"{work.Name.ColourName()} cannot be published. {error.ColourError()}");
			return;
		}

		if (!publish)
		{
			actor.OutputHandler.Send($"'{work.Name}' is structurally valid and ready to publish.");
			return;
		}

		service.SaveWork(work);
		actor.OutputHandler.Send($"You publish '{work.Name}' revision {work.Revision}.");
	}

	internal static string? Publish(ICharacter actor, PerformanceWork work)
	{
		if (!CanEdit(actor, work))
		{
			return "You do not have permission to publish this work.";
		}

		var error = Validate(actor, work);
		if (error is not null) return error;
		work.Published = true;
		work.StaffAuthored = actor.IsAdministrator();
		return null;
	}

	private static string WorkLookupError(string selector, bool editing)
	{
		var access = editing ? "editable" : "visible";
		var problem = selector.Length == 0 ? "Supply a work name." : $"No {access} work matches {selector.ColourCommand()}.";
		return $"{problem} Use {"performance work list".ColourCommand()} to see available works.";
	}

	private static PerformanceWork? Resolve(ICharacter actor, string selector, PerformanceService service)
	{
		selector = selector.Trim();
		if (actor.IsAdministrator() && long.TryParse(selector, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
		{
			return service.Works.FirstOrDefault(x => x.Id == id && CanView(actor, x));
		}

		return service.Works
			.Where(x => x.Name.Equals(selector, StringComparison.OrdinalIgnoreCase) && CanView(actor, x))
			.OrderByDescending(x => x.OwnerId == actor.Account.Id)
			.ThenByDescending(x => x.Revision)
			.FirstOrDefault();
	}

	internal static bool CanView(ICharacter actor, PerformanceWork work) =>
		work.Published || work.OwnerId == actor.Account.Id || actor.IsAdministrator();

	internal static bool CanEdit(ICharacter actor, PerformanceWork work) =>
		work.OwnerId == actor.Account.Id || actor.IsAdministrator();

	private static bool IsCanonicalKey(string? key) =>
		!string.IsNullOrWhiteSpace(key) && key == key.Trim().ToLowerInvariant();

	private static string NormalizeKey(string value) => value.Trim().ToLowerInvariant();

	internal static PerformanceWork CloneForEdit(PerformanceWork source, long ownerId, bool forkRevision) => new()
	{
		Id = forkRevision ? 0 : source.Id,
		OwnerId = forkRevision ? ownerId : source.OwnerId,
		Name = source.Name,
		Revision = source.Revision + (forkRevision ? 1 : 0),
		StaffAuthored = false,
		Roles = source.Roles.Select(x => new PerformanceRole { Key = x.Key, Mark = x.Mark, Mode = x.Mode, Required = x.Required, EligibilityProgId = x.EligibilityProgId, ReturnAfterPerformance = x.ReturnAfterPerformance }).ToList(),
		Places = new List<string>(source.Places),
		Exits = source.Exits.Select(x => new PerformanceExit { Key = x.Key, From = x.From, To = x.To }).ToList(),
		Props = new List<string>(source.Props),
		LeaveProps = new List<string>(source.LeaveProps),
		Cues = source.Cues.Select(x => new PerformanceCue { Role = x.Role, Place = x.Place, Verb = x.Verb, Target = x.Target, Text = x.Text, Seconds = x.Seconds }).ToList(),
		FailureRole = source.FailureRole,
		FailureText = source.FailureText
	};

	internal static bool UsableRule(IFutureProg? prog, ProgVariableTypes parameter, bool allowPrivate) =>
		prog is not null && (allowPrivate || prog.Public) && prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) &&
		prog.MatchesParameters([parameter]);

	internal static IFutureProg? FindRule(ICharacter actor, string input, ProgVariableTypes parameter, bool allowPrivate)
	{
		var progs = actor.Gameworld.FutureProgs;
		if (long.TryParse(input, out var id))
		{
			var prog = actor.IsAdministrator() ? progs.Get(id) : null;
			return UsableRule(prog, parameter, allowPrivate) ? prog : null;
		}
		return progs.FirstOrDefault(x => x.FunctionName.EqualTo(input) && UsableRule(x, parameter, allowPrivate));
	}

	private static string Describe(ICharacter actor, PerformanceWork work)
	{
		var sb = new StringBuilder($"{work.Name.ColourName()}, revision {work.Revision.ToString("N0", actor).ColourValue()} ({(work.Published ? "published".ColourValue() : "draft".ColourCommand())})\n");
		sb.AppendLine("Roles".ColourName() + ":");
		foreach (var role in work.Roles)
		{
			var eligibility = string.Empty;
			if (role.EligibilityProgId > 0)
			{
				var prog = actor.Gameworld.FutureProgs.Get(role.EligibilityProgId);
				eligibility = prog is null || (!actor.IsAdministrator() && !prog.Public)
					? ", eligibility unavailable"
					: actor.IsAdministrator()
						? $", eligible by {prog.FunctionName.ColourName()} (#{prog.Id.ToString("D", actor).ColourValue()}{(prog.Public ? ", public" : ", private")})"
						: $", eligible by {prog.FunctionName.ColourName()}";
			}

			var dismissal = role.Mode == PerformanceMode.Pc ? "" : role.ReturnAfterPerformance ? ", NPC returns after the show" : ", NPC released on the spot";
			sb.AppendLine($"  {role.Key.ColourName()}: {role.Mode.DescribeEnum().ColourName()}, {role.Required.ToColouredString()} required, mark {role.Mark.ColourValue()}{eligibility}{dismissal.ColourCommand()}");
		}
		sb.AppendLine("Places".ColourName() + ": " + string.Join(", ", work.Places.Select(x => x.ColourValue())));
		sb.AppendLine("Exits".ColourName() + ":");
		foreach (var exit in work.Exits) sb.AppendLine($"  {exit.Key.ColourName()}: {exit.From.ColourValue()} to {exit.To.ColourValue()}");
		sb.AppendLine("Props".ColourName() + ": " + string.Join(", ", work.Props.Select(key => $"{key.ColourName()} ({(work.LeaveProps.Contains(key) ? "leave" : "return if borrowed").ColourCommand()})")));
		sb.AppendLine("Cues".ColourName() + ":");
		for (var i = 0; i < work.Cues.Count; i++)
		{
			var cue = work.Cues[i];
			sb.AppendLine($"  {(i + 1).ToString("N0", actor).ColourValue()}. {cue.Role.ColourName()} at {cue.Place.ColourValue()}: {cue.Verb.DescribeEnum().ColourName()} {cue.Target.ColourValue()} ({cue.Seconds.ToString("N0", actor).ColourValue()}s) {cue.Text.ColourCommand()}");
		}
		if (!string.IsNullOrWhiteSpace(work.FailureText)) sb.AppendLine($"{"Failure direction".ColourName()} ({work.FailureRole.ColourValue()}): {work.FailureText.ColourCommand()}");
		return sb.ToString();
	}

	private static PerformanceWork BuildMass() => new()
	{
		Name = "Parish High Mass",
		Roles = new List<PerformanceRole>
		{
			new() { Key = "celebrant", Mode = PerformanceMode.Npc, Required = true, Mark = "sanctuary" },
			new() { Key = "reader", Mode = PerformanceMode.Either, Required = true, Mark = "sanctuary" },
			new() { Key = "server", Mode = PerformanceMode.Npc, Required = true, Mark = "sacristy" }
		},
		Places = new List<string> { "sacristy", "sanctuary" },
		Exits = new List<PerformanceExit> { new() { Key = "sacristy-door", From = "sacristy", To = "sanctuary" } },
		Props = new List<string> { "missal" },
		Cues = new List<PerformanceCue>
		{
			new() { Role = "server", Place = "sacristy", Verb = PerformanceVerb.Get, Target = "missal" },
			new() { Role = "server", Place = "sanctuary", Verb = PerformanceVerb.Travel, Target = "sacristy-door" },
			new() { Role = "celebrant", Place = "sanctuary", Verb = PerformanceVerb.Speak, Text = "We gather for the reading.", Seconds = 4 },
			new() { Role = "server", Place = "sanctuary", Verb = PerformanceVerb.Give, Target = "missal", Text = "reader" },
			new() { Role = "reader", Place = "sanctuary", Verb = PerformanceVerb.Speak, Text = "A reading for this feast.", Seconds = 8 },
			new() { Role = "reader", Place = "sanctuary", Verb = PerformanceVerb.Give, Target = "missal", Text = "server" },
			new() { Role = "celebrant", Place = "sanctuary", Verb = PerformanceVerb.Speak, Text = "Let us pray.", Seconds = 4 }
		},
		FailureRole = "celebrant",
		FailureText = "We will pause the service here."
	};

	private static PerformanceWork BuildConcert() => new()
	{
		Name = "Here Comes the Sun",
		Roles = new List<PerformanceRole>
		{
			new() { Key = "guitarist", Mode = PerformanceMode.Npc, Required = true, Mark = "platform" },
			new() { Key = "singer", Mode = PerformanceMode.Either, Required = true, Mark = "platform" },
			new() { Key = "introducer", Mode = PerformanceMode.Pc, Required = false, Mark = "platform" },
			new() { Key = "usher-stalls", Mode = PerformanceMode.Npc, Required = true, Mark = "foyer" },
			new() { Key = "usher-gallery", Mode = PerformanceMode.Npc, Required = true, Mark = "foyer" }
		},
		Places = new List<string> { "foyer", "stalls", "gallery", "platform" },
		Exits = new List<PerformanceExit>
		{
			new() { Key = "stalls-stairs", From = "stalls", To = "platform" },
			new() { Key = "gallery-stairs", From = "gallery", To = "platform" }
		},
		Props = new List<string> { "guitar" },
		Cues = new List<PerformanceCue>
		{
			new() { Role = "usher-stalls", Place = "stalls", Verb = PerformanceVerb.Travel },
			new() { Role = "usher-stalls", Place = "stalls", Verb = PerformanceVerb.Guard, Target = "stalls-stairs" },
			new() { Role = "usher-gallery", Place = "gallery", Verb = PerformanceVerb.Travel },
			new() { Role = "usher-gallery", Place = "gallery", Verb = PerformanceVerb.Guard, Target = "gallery-stairs" },
			new() { Place = "platform", Verb = PerformanceVerb.Stage, Target = "stalls" },
			new() { Place = "platform", Verb = PerformanceVerb.Stage, Target = "gallery" },
			new() { Role = "introducer", Place = "platform", Verb = PerformanceVerb.Speak, Text = "Welcome to the concert.", Seconds = 4 },
			new() { Role = "guitarist", Place = "platform", Verb = PerformanceVerb.Play, Target = "guitar" },
			new() { Role = "singer", Place = "platform", Verb = PerformanceVerb.Sing, Text = "Opening song.", Seconds = 8 },
			new() { Role = "guitarist", Place = "platform", Verb = PerformanceVerb.Stop, Target = "guitar" },
			new() { Role = "guitarist", Place = "platform", Verb = PerformanceVerb.Emote, Text = "bows to the audience" },
			new() { Place = "platform", Verb = PerformanceVerb.Unstage, Target = "stalls" },
			new() { Place = "platform", Verb = PerformanceVerb.Unstage, Target = "gallery" }
		},
		FailureRole = "introducer",
		FailureText = "The performance is paused while the hall is made safe."
	};

	private static PerformanceWork BuildProcession() => new()
	{
		Name = "Lantern Procession",
		Roles = new List<PerformanceRole>
		{
			new() { Key = "leader", Mode = PerformanceMode.Either, Required = true, Mark = "chapel" },
			new() { Key = "bearer", Mode = PerformanceMode.Npc, Required = true, Mark = "chapel" },
			new() { Key = "petitioner", Mode = PerformanceMode.Pc, Required = true, Mark = "chapel" }
		},
		Places = new List<string> { "chapel", "square", "shrine" },
		Exits = new List<PerformanceExit>
		{
			new() { Key = "chapel-square", From = "chapel", To = "square" },
			new() { Key = "square-shrine", From = "square", To = "shrine" }
		},
		Props = new List<string> { "lantern" },
		Cues = new List<PerformanceCue>
		{
			new() { Role = "bearer", Place = "chapel", Verb = PerformanceVerb.Get, Target = "lantern" },
			new() { Role = "leader", Place = "chapel", Verb = PerformanceVerb.Speak, Text = "Let us begin the procession.", Seconds = 4 },
			new() { Role = "leader", Place = "square", Verb = PerformanceVerb.Travel, Target = "chapel-square" },
			new() { Role = "bearer", Place = "square", Verb = PerformanceVerb.Travel, Target = "chapel-square" },
			new() { Role = "petitioner", Place = "square", Verb = PerformanceVerb.Travel, Target = "chapel-square" },
			new() { Role = "petitioner", Place = "square", Verb = PerformanceVerb.Speak, Text = "The petitioner offers their request.", Seconds = 0 },
			new() { Role = "leader", Place = "shrine", Verb = PerformanceVerb.Travel, Target = "square-shrine" },
			new() { Role = "bearer", Place = "shrine", Verb = PerformanceVerb.Travel, Target = "square-shrine" },
			new() { Role = "petitioner", Place = "shrine", Verb = PerformanceVerb.Travel, Target = "square-shrine" },
			new() { Role = "bearer", Place = "shrine", Verb = PerformanceVerb.Give, Target = "lantern", Text = "leader" },
			new() { Role = "leader", Place = "shrine", Verb = PerformanceVerb.Speak, Text = "We have arrived at the shrine.", Seconds = 4 }
		},
		FailureRole = "leader",
		FailureText = "The procession must pause here."
	};

	private static string HelpFor(ICharacter actor) =>
		(actor.IsAdministrator() ? BuilderHelp : PlayerHelp).SubstituteANSIColour();

	private const string PlayerHelp = @"#2Performance work authoring#0

#3Create and inspect#0
  #3performance work list#0
  #3performance work new <name>#0
  #3performance work fixtures#0
  #3performance work fixture <mass|concert|procession> [name]#0
  #3performance work copy <visible published work> [name]#0
  #3performance work show <work name>#0

#3Edit your draft#0
  #3performance work edit <work> name <new name>#0
  #3performance work edit <work> role add <key> <npc|pc|either> <true|false> <starting mark>#0
  #3performance work edit <work> role remove <key>#0
  #3performance work edit <work> role cleanup <key> <return|release>#0
  #3performance work edit <work> role eligibility <key> <public prog name|clear>#0
  #3performance work edit <work> place add <key>#0
  #3performance work edit <work> place remove <key>#0
  #3performance work edit <work> prop add <key>#0
  #3performance work edit <work> prop remove <key>#0
  #3performance work edit <work> prop cleanup <key> <return|leave>#0
  #3performance work edit <work> exit add <key> <from-place> <to-place>#0
  #3performance work edit <work> exit remove <key>#0
  #3performance work edit <work> cue add <role|-> <place|-> <verb> <target|-> <seconds> [text]#0
  #3performance work edit <work> cue remove <number>#0
  #3performance work edit <work> failure <role|-> <text>#0
  #3performance work validate <work>#0
  #3performance work publish <work>#0

#6Authoring rules#0
Use lowercase, unique symbolic keys for roles, places, exits and props. A role's starting mark must be a declared place. Use mode #3npc#0, #3pc#0 or #3either#0; required is #3true#0 or #3false#0. Every required role needs a cue. A dash (#3-#0) means no acting role, place or target where the selected action permits it.

Speech, song and emote cues need text after the duration. Cue verbs are #3speak#0, #3sing#0, #3emote#0, #3travel#0, #3give#0, #3get#0, #3play#0, #3stop#0, #3guard#0, #3stage#0, #3unstage#0 and #3wait#0. Give uses target=prop key and text=recipient role. Get, Play and Stop use a prop key as target. Guard uses an exit key at its origin. Travel names the destination and can use a directed exit key or a dash. Stage and Unstage use the source place and an audience place as target. Cue duration is a whole number of seconds, zero or greater.

Eligibility rules are public Boolean progs accepting a Character. Use #3clear#0 to remove a rule. An NPC cleanup policy of #3return#0 sends an NPC to the room and layer they occupied before gathering. #3release#0 resumes normal AI duties at the performance's final location. Props obtained from the floor, containers or cast members are returned to their source by default. Use #3leave#0 only when an item should stay with its final recipient.

Fixture keys supply editable starting scripts. They do not bind rooms, props or cast members. Publish only after validation. Published works can be copied into your own draft. Players see their own drafts and published works. Quote multiword names, for example #3performance work show ""Parish High Mass""#0.";

	private const string BuilderHelp = PlayerHelp + @"

#5Staff building tools#0
Use #3performance work list#0 to see all drafts and their work IDs. Use an ID in place of a work name to inspect, copy, edit, validate or publish a particular revision. Editing a published revision creates a new draft.

#3performance work edit <work> role eligibility <role> <prog name or ID>#0 accepts compatible public and private rules. A player copying a work with a private rule must replace or clear that rule before publication. Check #3performance work show <work>#0 to see the prog's name, ID and visibility.";
}
