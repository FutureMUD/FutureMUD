using MudSharp.Magic;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	private static bool CastingPlayer(ICharacter actor, IMagicSchool school, string command, StringStack input)
	{
		if (actor.Gameworld.MagicCasting is not { } service) return false;
		var capabilities = actor.Gameworld.MagicCapabilities.OfType<IMagicCastingCapability>()
			.Where(x => x.School.Id == school.Id && x.HasCastingPolicy).ToArray();
		if (capabilities.Length == 0) return false;
		if (command.EqualTo("area"))
		{
			actor.OutputHandler.Send("Area casting is unavailable until a separate explicit area policy is authored."); return true;
		}
		var method = "Say";
		if (command.EqualTo("quiet"))
		{
			method = "Whisper";
			if (input.IsFinished) { actor.OutputHandler.Send("Quiet casting is unavailable without an authored incantation policy and a complete named cast or formula."); return true; }
			if (input.PeekSpeech().EqualToAny("practice", "area")) { actor.OutputHandler.Send("That quiet mode combination is not explicitly enabled."); return true; }
			command = input.PeekSpeech().EqualTo("formula") ? input.PopSpeech() : "cast";
		}
		if (command.EqualTo("formula"))
		{
			actor.OutputHandler.Send(service.CastFormula(actor, input.RemainingArgument, method, school.Id)?.Message ?? "No complete authored formula matches."); return true;
		}
		var admitted = capabilities.SelectMany(x => x.CastingPolicy?.Admissions ?? []).Select(x => x.SpellId).ToHashSet();
		if (command.EqualTo("spells"))
		{
			var sb = new StringBuilder("Acquisition is permanent; current routes and energy are shown separately.\n");
			foreach (var spell in actor.Gameworld.MagicSpells.Where(x => admitted.Contains(x.Id) || x.School.Id == school.Id).OrderBy(x => x.Name))
			{
				var acquired = service.Acquisition(actor, spell.Id);
				var legacy = spell is MagicSpell native && native.HasLegacyRoute(actor);
				var vancian = MudSharp.Magic.Vancian.VancianMagicService.For(actor.Gameworld).KnowsThroughVancian(actor, spell);
				if (acquired is null && !legacy && !vancian && !admitted.Contains(spell.Id)) continue;
				sb.AppendLine($"{spell.Name.ColourName()}: {(acquired is null ? "not acquired" : $"acquired, controlled grade {acquired.ControlledGrade}")}{(legacy ? "; independent legacy grant" : "")}{(vancian ? "; independent Vancian knowledge" : "")}");
				AppendRoutes(sb, spell.Id);
			}
			actor.OutputHandler.Send(sb.ToString()); return true;
		}
		if (!command.EqualToAny("cast", "practice", "spell", "spellhelp")) return false;
		var practice = command.EqualTo("practice");
		var args = new StringStack(input.RemainingArgument);
		var spellText = args.PopSpeech();
		var selected = actor.Gameworld.MagicSpells.Where(x => x.Name.EqualTo(spellText) || x.Id.ToString() == spellText).ToArray();
		if (selected.Length != 1 || !admitted.Contains(selected[0].Id))
		{
			if (!practice && method == "Say") return false;
			actor.OutputHandler.Send("No unique explicitly admitted spell matches that practice request."); return true;
		}
		var chosen = selected[0];
		if (!command.EqualToAny("cast", "practice"))
		{
			var known = service.Acquisition(actor, chosen.Id);
			var sb = new StringBuilder($"{chosen.Name}\n{chosen.Description}\nAcquisition: {(known is null ? "not acquired" : $"controlled grade {known.ControlledGrade}; {known.Provenance}")}\nNative school: {chosen.School.Name}\n");
			AppendRoutes(sb, chosen.Id);
			sb.AppendLine($"{school.SchoolVerb} cast \"{chosen.Name}\" grade <1..7> [overreach] on <target> [via <capability>]");
			sb.AppendLine($"{school.SchoolVerb} practice \"{chosen.Name}\" grade <1..7> [overreach] [via <capability>] (target-free, explicitly authored practice only)");
			if (chosen is MagicSpell { GradeProfile.Incantation: { } incantation })
			{
				sb.AppendLine($"{school.SchoolVerb} quiet \"{chosen.Name}\" grade <1..7> [overreach] on <target> [via <capability>]");
				sb.AppendLine($"{school.SchoolVerb} formula <POWER> {string.Join(" ", incantation.CategoryWords)} [overreach] on <target> [via <capability>] (five words in any order)");
				sb.AppendLine($"Native speech or POWER + alias: {string.Join(", ", incantation.Aliases)}; select {actor.Gameworld.Languages.Get(incantation.LanguageId)?.Name}. Category vocabulary: {incantation.VocabularyProvenance}.");
			}
			actor.OutputHandler.Send(sb.ToString()); return true;
		}
		if (!args.PopSpeech().EqualTo("grade"))
		{
			if (!practice && method == "Say" && chosen is MagicSpell native && native.HasLegacyRoute(actor)) return false;
			actor.OutputHandler.Send(practice ? "Practice requires grade <1..7> and optional overreach; it is target-free." :
				"Configured casting requires grade <1..7>, optional overreach, and on <target>."); return true;
		}
		if (!int.TryParse(args.PopSpeech(), out var grade) || grade is < 1 or > 7)
		{ actor.OutputHandler.Send("Specify an integer grade from 1 to 7."); return true; }
		var overreach = args.PeekSpeech().EqualTo("overreach"); if (overreach) args.PopSpeech();
		if (!practice && !args.PopSpeech().EqualTo("on")) { actor.OutputHandler.Send("Specify on <target> after the grade and optional overreach."); return true; }
		List<string> targets = []; string? via = null;
		while (!args.IsFinished)
		{
			var token = args.PopSpeech();
			if (token.EqualTo("via"))
			{
				via = args.PopSpeech();
				if (!args.IsFinished || string.IsNullOrWhiteSpace(via)) { actor.OutputHandler.Send("The final via argument must name one capability."); return true; }
				break;
			}
			if (token.EqualToAny("practice", "formula", "quiet", "area"))
			{ actor.OutputHandler.Send("That later casting mode is unavailable."); return true; }
			if (practice) { actor.OutputHandler.Send("Practice is target-free; only a final via <capability> argument is permitted."); return true; }
			targets.Add(token.Contains(' ') ? token.DoubleQuotes() : token);
		}
		if (!practice && targets.Count == 0) { actor.OutputHandler.Send("Specify a target; use self for a self-targeted spell."); return true; }
		var routes = capabilities.Where(x => actor.Capabilities.Any(c => c.Id == x.Id) && x.CastingPolicy?.Admissions.Any(a => a.SpellId == chosen.Id) == true &&
			(via is null || x.Name.EqualTo(via) || x.Id.ToString() == via)).ToArray();
		if (routes.Length != 1)
		{ actor.OutputHandler.Send(routes.Length == 0 ? "No current explicitly admitted route matches that capability." : "Multiple routes admit that spell. Select via <capability>; energy does not select a route."); return true; }
		actor.OutputHandler.Send(service.Cast(new(actor, routes[0].Id, chosen.Id, grade, overreach, string.Join(" ", targets),
			practice ? MagicCastingMode.Practice : MagicCastingMode.Manifest, Method: method)).Message);
		return true;

		void AppendRoutes(StringBuilder sb, long spellId)
		{
			foreach (var route in service.Routes(actor, spellId).Where(x => capabilities.Any(c => c.Id == x.CapabilityId)))
			{
				var resource = actor.Gameworld.MagicResources.Get(route.ReserveId);
				var holder = MagicCastingService.ReserveHolder(actor, route.ReserveId);
				var balance = resource is null ? 0 : holder.MagicResourceAmounts.GetValueOrDefault(resource);
				sb.AppendLine($"  via {actor.Gameworld.MagicCapabilities.Get(route.CapabilityId)!.Name.ColourName()}: {route.Reason}; energy {balance.ToString("N2", actor).ColourValue()} {resource?.Name}");
			}
		}
	}

	private static void CastingAdmin(ICharacter actor, StringStack args)
	{
		if (!actor.IsAdministrator() || actor.Gameworld.MagicCasting is not MagicCastingService service) return;
		var action = args.PopSpeech().ToLowerInvariant();
		if (action is not ("enrol" or "grant" or "resolve"))
		{
			actor.OutputHandler.Send("magic casting enrol <character> <capability> <reason>\nmagic casting grant <character> <capability> <spell> <reason>\nmagic casting resolve <character> <operation-guid> <reconciliation reason>"); return;
		}
		var targetText = args.PopSpeech();
		var target = long.TryParse(targetText, out var id) ? actor.Gameworld.TryGetCharacter(id, true) : actor.TargetActor(targetText);
		if (target is null) { actor.OutputHandler.Send("No such character."); return; }
		if (action == "resolve")
		{
			if (!Guid.TryParse(args.PopSpeech(), out var operation)) { actor.OutputHandler.Send("Specify an operation GUID."); return; }
			actor.OutputHandler.Send(service.ReconcileOperation(actor, target, operation, args.SafeRemainingArgument).Message); return;
		}
		var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(args.PopSpeech());
		if (capability is null) { actor.OutputHandler.Send("No such capability."); return; }
		if (action == "enrol") { actor.OutputHandler.Send(service.Enrol(actor, target, capability.Id, args.SafeRemainingArgument).Message); return; }
		var spell = actor.Gameworld.MagicSpells.GetByIdOrName(args.PopSpeech());
		if (spell is null) { actor.OutputHandler.Send("No such spell."); return; }
		actor.OutputHandler.Send(service.Grant(actor, target, capability.Id, spell.Id, args.SafeRemainingArgument).Message);
	}
}
