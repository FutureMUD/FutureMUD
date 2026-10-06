using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private static IReadOnlyList<MagicCastingPrerequisite> LoadPrerequisites(XElement parent) => Array.AsReadOnly(parent.Elements("Prerequisite").Select(e =>
	{
		var kind = (string?)e.Attribute("kind") switch
		{
			null or "spell" => MagicCastingPrerequisiteKind.Spell,
			"trait" => MagicCastingPrerequisiteKind.SupportTrait,
			_ => throw new FormatException("Unknown casting prerequisite kind.")
		};
		if (kind == MagicCastingPrerequisiteKind.SupportTrait && e.Attribute("trait") is null)
			throw new FormatException("A support-trait prerequisite needs an explicit trait reference.");
		return new MagicCastingPrerequisite(Guid.Parse((string)e.Attribute("key")!), (long?)e.Attribute("spell") ?? 0,
			(int?)e.Attribute("grade") ?? 0, (double)e.Attribute("proficiency")!, kind, (long?)e.Attribute("trait"));
	}).ToArray());

	private static XElement SavePrerequisite(MagicCastingPrerequisite e) => new("Prerequisite", new XAttribute("key", e.Key),
		e.Kind == MagicCastingPrerequisiteKind.SupportTrait ? new XAttribute("kind", "trait") : null,
		e.Kind == MagicCastingPrerequisiteKind.SupportTrait ? new XAttribute("trait", e.TraitId!.Value) : new XAttribute("spell", e.SpellId),
		e.Kind == MagicCastingPrerequisiteKind.Spell ? new XAttribute("grade", e.MinimumGrade) : null, new XAttribute("proficiency", e.MinimumProficiency));

	private void ValidateSupportGraph(MagicCastingPolicy p, List<string> errors)
	{
		if (p.Supports.Count > 128 || p.Supports.Select(x => x.TraitId).Distinct().Count() != p.Supports.Count)
		{
			errors.Add("Casting supports require unique traits and at most 128 grants."); return;
		}
		foreach (var s in p.Supports)
		{
			if (s.RawSkillCap.HasValue && Gameworld.Traits.Get(s.TraitId) is MudSharp.Body.Traits.Subtypes.ITheoreticalSkillDefinition)
				errors.Add($"Support {s.Key}: capped theoretical skills are unsupported; select a native single-value skill or an uncapped binding.");
			if (Gameworld.Traits.Get(s.TraitId) is not { TraitType: TraitType.Skill, OwnerScope: TraitOwnerScope.Character } ||
				!double.IsFinite(s.OpeningSkill) || s.OpeningSkill < 0 || s.RawSkillCap is { } cap &&
				(!double.IsFinite(cap) || cap <= 0 || s.OpeningSkill > cap)) errors.Add($"Support {s.Key}: invalid native skill/opening/cap.");
		}
		var nodes = p.Admissions.Select(x => (Kind: MagicCastingPrerequisiteKind.Spell, Id: x.SpellId, Edges: x.Prerequisites))
			.Concat(p.Supports.Select(x => (Kind: MagicCastingPrerequisiteKind.SupportTrait, Id: x.TraitId, Edges: x.Prerequisites))).ToArray();
		foreach (var n in nodes)
		{
			if (n.Edges.Count > 512 || n.Edges.Select(x => (x.Kind, x.SourceId)).Distinct().Count() != n.Edges.Count)
				errors.Add("Duplicate or excessive typed prerequisites.");
			foreach (var e in n.Edges)
			{
				if (!Enum.IsDefined(e.Kind) || !double.IsFinite(e.MinimumProficiency) || e.MinimumProficiency < 0 ||
					e.Kind == MagicCastingPrerequisiteKind.SupportTrait && (e.SpellId != 0 || e.MinimumGrade != 0 ||
						!p.Supports.Any(x => x.TraitId == e.TraitId)) ||
					e.Kind == MagicCastingPrerequisiteKind.Spell && (e.TraitId.HasValue || e.MinimumGrade < 1 ||
						!p.Admissions.Any(x => x.SpellId == e.SpellId && e.MinimumGrade <= x.MaximumGrade)))
					errors.Add($"Prerequisite {e.Key}: invalid or unscoped typed source.");
			}
		}
		if (nodes.Select(x => (x.Kind, x.Id)).Distinct().Count() != nodes.Length) return;
		var byId = nodes.ToDictionary(x => (x.Kind, x.Id), x => x.Edges);
		HashSet<(MagicCastingPrerequisiteKind, long)> done = [], active = [];
		bool Visit((MagicCastingPrerequisiteKind, long) id)
		{
			if (done.Contains(id) || !byId.TryGetValue(id, out var edges)) return false;
			if (!active.Add(id)) return true;
			if (edges.Any(e => Visit((e.Kind, e.SourceId)))) return true;
			active.Remove(id); done.Add(id); return false;
		}
		if (byId.Keys.Any(Visit)) errors.Add("Casting prerequisite cycle.");
	}
}
