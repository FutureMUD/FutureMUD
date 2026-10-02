using MudSharp.Body.Traits;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private XElement? _unreadableCasting;
	private string? _castingLoadError;
	public bool HasCastingPolicy => CastingPolicy is not null || _unreadableCasting is not null;
	public MagicCastingPolicy? CastingPolicy { get; private set; }

	private void LoadCastingDefinition(XElement definition)
	{
		if (definition.Element("Casting") is not { } root) return;
		try
		{
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported Casting version.");
			CastingPolicy = new(1, Guid.Parse((string)root.Attribute("identity")!), (bool)root.Attribute("enabled")!,
				(long)root.Attribute("trait")!, (long)root.Attribute("source")!, (long)root.Attribute("reserve")!,
				(bool)root.Attribute("passive")!, (int)root.Attribute("startingVersion")!,
				Array.AsReadOnly(root.Elements("Admission").Select(x => new MagicCastingAdmission(
					Guid.Parse((string)x.Attribute("key")!), (long)x.Attribute("spell")!, (long?)x.Attribute("trait"),
					(bool)x.Attribute("starting")!, (int)x.Attribute("min")!, (int)x.Attribute("max")!,
					Array.AsReadOnly(x.Elements("Prerequisite").Select(e => new MagicCastingPrerequisite(
						Guid.Parse((string)e.Attribute("key")!), (long)e.Attribute("spell")!, (int)e.Attribute("grade")!,
						(double)e.Attribute("proficiency")!)).ToArray()), (double?)x.Attribute("opening"),
					(double?)x.Attribute("rawCap"), (bool?)x.Attribute("capRelative") ?? false)).ToArray()));
		}
		catch (Exception ex)
		{
			_castingLoadError = $"Casting: {ex.Message}";
			_unreadableCasting = new XElement(root);
		}
	}

	private void SaveCastingDefinition(XElement root)
	{
		if (_unreadableCasting is not null) { root.Add(new XElement(_unreadableCasting)); return; }
		if (CastingPolicy is not { } p) return;
		root.Add(new XElement("Casting", new XAttribute("version", p.Version), new XAttribute("identity", p.Identity),
			new XAttribute("enabled", p.Enabled), new XAttribute("trait", p.DefaultTraitId),
			new XAttribute("source", p.SourceResourceId), new XAttribute("reserve", p.ReserveResourceId),
			new XAttribute("passive", p.PassiveEntitlement), new XAttribute("startingVersion", p.StartingGrantVersion),
			p.Admissions.Select(x => new XElement("Admission", new XAttribute("key", x.Key), new XAttribute("spell", x.SpellId),
				x.TraitId.HasValue ? new XAttribute("trait", x.TraitId.Value) : null, new XAttribute("starting", x.Starting),
				new XAttribute("min", x.MinimumGrade), new XAttribute("max", x.MaximumGrade),
				x.OpeningSkill.HasValue ? new XAttribute("opening", x.OpeningSkill.Value) : null,
				x.RawSkillCap.HasValue ? new XAttribute("rawCap", x.RawSkillCap.Value) : null,
				x.CapRelativeProficiency ? new XAttribute("capRelative", true) : null,
				x.Prerequisites.Select(e => new XElement("Prerequisite", new XAttribute("key", e.Key),
					new XAttribute("spell", e.SpellId), new XAttribute("grade", e.MinimumGrade), new XAttribute("proficiency", e.MinimumProficiency)))))));
	}

	private void CloneCastingFrom(SkillLevelBasedMagicCapability source)
	{
		_castingLoadError = source._castingLoadError;
		_unreadableCasting = source._unreadableCasting is null ? null : new XElement(source._unreadableCasting);
		if (source.CastingPolicy is not { } policy) return;
		CastingPolicy = policy with { Identity = Guid.NewGuid(), Admissions = Array.AsReadOnly(policy.Admissions.Select(x => x with
		{
			Key = Guid.NewGuid(), Prerequisites = Array.AsReadOnly(x.Prerequisites.Select(e => e with { Key = Guid.NewGuid() }).ToArray())
		}).ToArray()) };
	}

	public IReadOnlyList<string> CastingConfigurationErrors()
	{
		List<string> errors = [];
		if (_castingLoadError is not null) errors.Add(_castingLoadError);
		if (CastingPolicy is not { } p) return errors.AsReadOnly();
		if (this is IVancianMagicCapability && p.Enabled) errors.Add("Casting cannot be enabled on a Vancian capability.");
		if (p.Identity == Guid.Empty || p.Version != 1 || p.StartingGrantVersion < 1) errors.Add("Invalid casting identity/version.");
		CheckTrait(p.DefaultTraitId, "Default trait");
		if (Gameworld.MagicResources.Get(p.SourceResourceId) is null) errors.Add($"Missing source resource {p.SourceResourceId}.");
		if (Gameworld.MagicResources.Get(p.ReserveResourceId) is null) errors.Add($"Missing reserve {p.ReserveResourceId}.");
		if (p.Admissions.Count > 512) errors.Add("A capability supports at most 512 admissions.");
		var keys = p.Admissions.Select(x => x.Key).Concat(p.Admissions.SelectMany(x => x.Prerequisites).Select(x => x.Key)).ToArray();
		if (keys.Any(x => x == Guid.Empty) || keys.Distinct().Count() != keys.Length) errors.Add("Duplicate or empty admission/prerequisite keys.");
		if (p.Admissions.Select(x => x.SpellId).Distinct().Count() != p.Admissions.Count) errors.Add("Duplicate spell admissions.");
		foreach (var a in p.Admissions)
		{
			CheckTrait(a.TraitId ?? p.DefaultTraitId, $"Admission {a.Key}");
			if (a.OpeningSkill is { } opening && (!double.IsFinite(opening) || opening < 0) ||
				a.RawSkillCap is { } cap && (!double.IsFinite(cap) || cap <= 0) ||
				a.OpeningSkill is { } opened && a.RawSkillCap is { } ceiling && opened > ceiling ||
				a.CapRelativeProficiency && a.RawSkillCap is null)
				errors.Add($"Admission {a.Key}: invalid opening, raw cap or cap-relative proficiency policy.");
			if (Gameworld.MagicSpells.Get(a.SpellId) is not IControlledMagicSpell spell || spell.GradeProfile is null)
				errors.Add($"Admission {a.Key}: missing spell/grade profile {a.SpellId}.");
			else
			{
				errors.AddRange(spell.GradeConfigurationErrors().Select(x => $"Spell {a.SpellId}: {x}"));
				if (!spell.CastingCosts.Any(x => x.Key.Id == p.SourceResourceId)) errors.Add($"Spell {a.SpellId}: missing designated cost resource {p.SourceResourceId}.");
				if (a.MinimumGrade < 1 || a.MaximumGrade < a.MinimumGrade || a.MaximumGrade > spell.GradeProfile.Grades.Count)
					errors.Add($"Admission {a.Key}: invalid grade range.");
				if (a.CapRelativeProficiency && spell.GradeProfile.Grades.Any(x => x.MinimumProficiency > 100))
					errors.Add($"Admission {a.Key}: cap-relative thresholds must be percentages from 0 to 100.");
				if (a.RawSkillCap is { } rawCap && (a.OpeningSkill ?? spell.GradeProfile.OpeningSkill) > rawCap)
					errors.Add($"Admission {a.Key}: opening exceeds its raw cap.");
			}
			if (a.Prerequisites.Count > 512 || a.Prerequisites.Select(x => x.SpellId).Distinct().Count() != a.Prerequisites.Count)
				errors.Add($"Admission {a.Key}: duplicate or excessive prerequisites.");
			foreach (var e in a.Prerequisites)
			{
				var prior = p.Admissions.FirstOrDefault(x => x.SpellId == e.SpellId);
				if (prior is null || e.MinimumGrade < 1 || e.MinimumGrade > prior.MaximumGrade || !double.IsFinite(e.MinimumProficiency) || e.MinimumProficiency < 0)
					errors.Add($"Prerequisite {e.Key}: invalid grade/proficiency or spell not admitted by this capability.");
			}
		}
		if (p.Admissions.Select(x => x.SpellId).Distinct().Count() == p.Admissions.Count && p.Admissions.Count <= 512)
		{
			var nodes = p.Admissions.ToDictionary(x => x.SpellId);
			HashSet<long> done = [], active = [];
			bool Visit(long id)
			{
				if (done.Contains(id) || !nodes.TryGetValue(id, out var node)) return false;
				if (!active.Add(id)) return true;
				if (node.Prerequisites.Any(e => Visit(e.SpellId))) return true;
				active.Remove(id); done.Add(id); return false;
			}
			if (nodes.Keys.Any(Visit)) errors.Add("Casting prerequisite cycle.");
		}
		return errors.AsReadOnly();

		void CheckTrait(long id, string label)
		{
			if (Gameworld.Traits.Get(id) is not { TraitType: TraitType.Skill, OwnerScope: TraitOwnerScope.Character })
				errors.Add($"{label}: {id} must be a character-owned native skill.");
		}
	}

	private void AppendCastingShow(StringBuilder sb, ICharacter actor)
	{
		if (!HasCastingPolicy) return;
		sb.AppendLine("Casting Policy:");
		if (CastingPolicy is { } p)
		{
			sb.AppendLine($"  Enabled: {p.Enabled.ToColouredString()} Identity: {p.Identity} Trait: {p.DefaultTraitId.ToString("N0", actor).ColourValue()}");
			sb.AppendLine($"  Energy: {p.SourceResourceId} -> {p.ReserveResourceId}; passive: {p.PassiveEntitlement.ToColouredString()}; starting version: {p.StartingGrantVersion}");
			foreach (var a in p.Admissions)
			{
				sb.AppendLine($"  {a.Key}: spell {a.SpellId}, trait {a.TraitId ?? p.DefaultTraitId}, grades {a.MinimumGrade}-{a.MaximumGrade}, starting {a.Starting.ToColouredString()}");
				sb.AppendLine($"    Opening: {a.OpeningSkill?.ToString("N2", actor) ?? "profile default"}; raw improvement cap: {a.RawSkillCap?.ToString("N2", actor) ?? "native"}; proficiency gates: {(a.CapRelativeProficiency ? "percent of route cap" : "absolute raw skill")}");
				foreach (var e in a.Prerequisites) sb.AppendLine($"    {e.Key}: spell {e.SpellId}, grade {e.MinimumGrade}, raw skill {e.MinimumProficiency.ToString("N2", actor)}");
			}
		}
		foreach (var error in CastingConfigurationErrors()) sb.AppendLine(error.ColourError());
	}
}
