using System.Globalization;
using MudSharp.Form.Audio;
using MudSharp.FutureProg;
using MudSharp.Magic.SpellTriggers;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private static ControlledSpellArea LoadArea(XElement root)
	{
		if ((int?)root.Attribute("schema") != 1) throw new FormatException("Unsupported area schema.");
		return new((SpellAreaScope)(int)root.Attribute("scope")!, (SpellAreaSelection)(int)root.Attribute("selection")!,
			(SpellAreaIdentity)(int)root.Attribute("identity")!, (SpellAreaPlane)(int)root.Attribute("plane")!,
			(int)root.Attribute("targets")!, (int)root.Attribute("applications")!, (bool)root.Attribute("caster")!,
			(bool)root.Attribute("allies")!, (bool)root.Attribute("others")!, (bool)root.Attribute("layer")!,
			(bool)root.Attribute("grounded")!, (bool)root.Attribute("staff")!, (double)root.Attribute("casterDamage")!,
			(double)root.Attribute("otherDamage")!, (long)root.Attribute("filter")!,
			Array.AsReadOnly(root.Elements("Method").Select(x => new ControlledSpellDelivery(
				(string?)x.Attribute("name") ?? throw new FormatException("Area method requires a name."),
				(AudioVolume)(int)x.Attribute("volume")!, (double)x.Attribute("energy")!, (int)x.Attribute("difficulty")!)).ToArray()),
			(string?)root.Attribute("provenance") ?? throw new FormatException("Area requires provenance."));
	}
	private XElement? SaveArea() => GradeProfile?.Area is not { } p ? null :
		new("Area", new XAttribute("schema", 1), new XAttribute("scope", (int)p.Scope),
			new XAttribute("selection", (int)p.Selection), new XAttribute("identity", (int)p.Identity),
			new XAttribute("plane", (int)p.Plane), new XAttribute("targets", p.MaximumTargets),
			new XAttribute("applications", p.MaximumApplications), new XAttribute("caster", p.IncludeCaster),
			new XAttribute("allies", p.IncludeAllies), new XAttribute("others", p.IncludeOthers),
			new XAttribute("layer", p.SameLayer), new XAttribute("grounded", p.GroundedOnly),
			new XAttribute("staff", p.ExcludeStaff), new XAttribute("casterDamage", p.CasterDamageMultiplier),
			new XAttribute("otherDamage", p.OtherDamageMultiplier), new XAttribute("filter", p.FilterProgId),
			new XAttribute("provenance", p.Provenance), p.Deliveries.Select(x => new XElement("Method",
				new XAttribute("name", x.Method), new XAttribute("volume", (int)x.Volume),
				new XAttribute("energy", x.EnergyMultiplier), new XAttribute("difficulty", x.DifficultySteps))));

	private IReadOnlyList<string> AreaConfigurationErrors(ControlledSpellArea? p)
	{
		if (p is null) return [];
		List<string> errors = [];
		if (Trigger is not CastingTriggerCharacter) errors.Add("Area: author a native character trigger; room/exit effects keep their native target and are not character lists.");
		if (Trigger is CastingTriggerCharacter { CanTargetSelf: false } && p.IncludeCaster)
			errors.Add("Area: native trigger forbids self; enable its self eligibility before including the caster.");
		if (!Enum.IsDefined(p.Scope) || !Enum.IsDefined(p.Selection) || !Enum.IsDefined(p.Identity) || !Enum.IsDefined(p.Plane) ||
			p.MaximumTargets is < 1 or > 256 || p.MaximumApplications is < 1 or > 256 ||
			!double.IsFinite(p.CasterDamageMultiplier) || p.CasterDamageMultiplier < 0 ||
			!double.IsFinite(p.OtherDamageMultiplier) || p.OtherDamageMultiplier < 0 ||
			string.IsNullOrWhiteSpace(p.Provenance) || p.Provenance.Length > 512)
			errors.Add("Area: invalid scope, selection, identity, plane, positive bounds (1..256), non-negative damage multipliers or provenance.");
		if (p.FilterProgId != 0 && (Gameworld.FutureProgs.Get(p.FilterProgId) is not { } filter ||
			!filter.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!filter.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character])))
			errors.Add("Area: filter must be a boolean Prog accepting target character and caster character.");
		if (p.Deliveries.Count == 0 || p.Deliveries.Select(x => x.Method).Distinct(StringComparer.OrdinalIgnoreCase).Count() != p.Deliveries.Count ||
			p.Deliveries.Any(x => !MagicCastingMethods.TryVolume(x.Method, out var volume) || volume != x.Volume ||
				!double.IsFinite(x.EnergyMultiplier) || x.EnergyMultiplier <= 0 || x.DifficultySteps is < -10 or > 10))
			errors.Add("Area: explicitly author unique native methods with positive energy and difficulty steps -10..10.");
		return errors;
	}
	private void AppendAreaShow(StringBuilder sb, ICharacter actor)
	{
		if (GradeProfile?.Area is not { } p) { sb.AppendLine("  Area: not authored."); return; }
		sb.AppendLine($"  Area {p.Scope}/{p.Selection}/{p.Identity}/{p.Plane}; targets {p.MaximumTargets}, applications {p.MaximumApplications}; stable identity/body ordering");
		sb.AppendLine($"  Include caster {p.IncludeCaster}, allies {p.IncludeAllies}, others {p.IncludeOthers}; same layer {p.SameLayer}, grounded {p.GroundedOnly}, exclude staff {p.ExcludeStaff}");
		sb.AppendLine($"  Damage caster x{p.CasterDamageMultiplier.ToString("N4", actor)}, others x{p.OtherDamageMultiplier.ToString("N4", actor)}; filter #{p.FilterProgId}; {p.Provenance}");
		foreach (var d in p.Deliveries) sb.AppendLine($"  Area native {d.Method}: total energy x{d.EnergyMultiplier.ToString("N2", actor)}, difficulty {d.DifficultySteps:+0;-0;0}");
	}
	private bool BuildingCommandArea(ICharacter actor, StringStack command, ControlledSpellProfile profile)
	{
		var action = command.PopSpeech().ToLowerInvariant(); var p = profile.Area;
		if (action == "off") p = null;
		else if (action == "fixture")
		{
			var example = command.PopSpeech().ToLowerInvariant();
			if (example is not ("earthquake" or "chainlightning" or "roomfireball"))
				throw new FormatException("Use fixture earthquake, chainlightning or roomfireball; these are partial source examples with provisional bounds/native mapping, not installed spells.");
			p = new(SpellAreaScope.RoomCharacters, example == "chainlightning" ? SpellAreaSelection.RandomWithReplacement : SpellAreaSelection.Ordered,
				SpellAreaIdentity.PhysicalBody, example == "chainlightning" ? SpellAreaPlane.MagicReach : SpellAreaPlane.PhysicalAndMagicReach,
				256, example == "chainlightning" ? 3 : 256, example != "roomfireball", true, true, true,
				example == "earthquake", true, example == "earthquake" ? 1.0 / 3 : example == "chainlightning" ? 0.25 : 1,
				1, 0, Array.AsReadOnly(new[] { new ControlledSpellDelivery("Say", AudioVolume.Decent, 1, 0) }),
				$"Brief C source {example}; provisional limits/layer/ground/staff mapping; author remaining source protection predicates separately");
		}
		else
		{
			if (p is null) throw new FormatException("Author grades area fixture <earthquake|chainlightning|roomfireball> first.");
			switch (action)
			{
				case "include":
					var who = command.PopSpeech().ToLowerInvariant(); var include = bool.Parse(command.PopSpeech());
					p = who switch { "caster" => p with { IncludeCaster = include }, "allies" => p with { IncludeAllies = include },
						"others" => p with { IncludeOthers = include }, _ => throw new FormatException("Use include caster/allies/others <true|false>.") }; break;
				case "damage":
					var whom = command.PopSpeech().ToLowerInvariant(); var multiplier = Number();
					p = whom switch { "caster" => p with { CasterDamageMultiplier = multiplier }, "others" => p with { OtherDamageMultiplier = multiplier },
						_ => throw new FormatException("Use damage caster/others <non-negative multiplier>.") }; break;
				case "selection": p = p with { Selection = Enum.Parse<SpellAreaSelection>(command.PopSpeech(), true) }; break;
				case "scope": p = p with { Scope = Enum.Parse<SpellAreaScope>(command.PopSpeech(), true) }; break;
				case "identity": p = p with { Identity = Enum.Parse<SpellAreaIdentity>(command.PopSpeech(), true) }; break;
				case "plane": p = p with { Plane = Enum.Parse<SpellAreaPlane>(command.PopSpeech(), true) }; break;
				case "targets": p = p with { MaximumTargets = Integer() }; break;
				case "applications": p = p with { MaximumApplications = Integer() }; break;
				case "layer": p = p with { SameLayer = bool.Parse(command.PopSpeech()) }; break;
				case "grounded": p = p with { GroundedOnly = bool.Parse(command.PopSpeech()) }; break;
				case "staff": p = p with { ExcludeStaff = bool.Parse(command.PopSpeech()) }; break;
				case "filter":
					var token = command.PopSpeech(); p = p with { FilterProgId = token.EqualTo("none") ? 0 :
						(Gameworld.FutureProgs.GetByIdOrName(token) ?? throw new FormatException("No such area filter Prog.")).Id }; break;
				case "method":
					var method = MagicCastingMethods.CanonicalName(command.PopSpeech()) ?? throw new FormatException("Specify a native speech method.");
					var methods = p.Deliveries.Where(x => !x.Method.EqualTo(method)).ToList();
					if (command.PeekSpeech().EqualTo("off")) command.PopSpeech();
					else { MagicCastingMethods.TryVolume(method, out var volume); methods.Add(new(method, volume, Number(), Integer())); }
					p = p with { Deliveries = methods.AsReadOnly() }; break;
				case "provenance": p = p with { Provenance = command.SafeRemainingArgument }; command = new StringStack(""); break;
				default: throw new FormatException("Area options: fixture/off, include, damage, scope, selection, identity, plane, targets, applications, layer, grounded, staff, filter, method, provenance.");
			}
		}
		if (!command.IsFinished) throw new FormatException("Unexpected trailing area input.");
		if (AreaConfigurationErrors(p).FirstOrDefault() is { } error) throw new FormatException(error);
		GradeProfile = profile with { Area = p }; _unreadableGradeProfile = null; _gradeLoadError = null; Changed = true;
		actor.OutputHandler.Send("Area policy updated. Inclusion and total method modifiers are explicit; fixture limits/native source mappings are provisional.");
		return true;
		double Number() => double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
		int Integer() => int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
	}
}
