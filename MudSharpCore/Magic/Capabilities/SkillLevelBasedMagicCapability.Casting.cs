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
					LoadPrerequisites(x), (double?)x.Attribute("opening"),
					(double?)x.Attribute("rawCap"), (bool?)x.Attribute("capRelative") ?? false)).ToArray()),
				Array.AsReadOnly(root.Elements("SupportGrant").Select(x => new MagicCastingSupportGrant(
					Guid.Parse((string)x.Attribute("key")!), (long)x.Attribute("trait")!, (double)x.Attribute("opening")!,
					(double?)x.Attribute("rawCap"), (bool?)x.Attribute("starting") ?? false, LoadPrerequisites(x))).ToArray()),
				(bool?)root.Attribute("enrolOnClassSelection") ?? false,
				root.Element("Patron") is { } patron ? System.Text.Json.JsonSerializer.Deserialize<PatronCastingPolicy>(patron.Value) : null);
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
			new XAttribute("enrolOnClassSelection", p.EnrolOnClassSelection),
			p.Patron is null ? null : new XElement("Patron", new XCData(System.Text.Json.JsonSerializer.Serialize(p.Patron))),
			p.Admissions.Select(x => new XElement("Admission", new XAttribute("key", x.Key), new XAttribute("spell", x.SpellId),
				x.TraitId.HasValue ? new XAttribute("trait", x.TraitId.Value) : null, new XAttribute("starting", x.Starting),
				new XAttribute("min", x.MinimumGrade), new XAttribute("max", x.MaximumGrade),
				x.OpeningSkill.HasValue ? new XAttribute("opening", x.OpeningSkill.Value) : null,
				x.RawSkillCap.HasValue ? new XAttribute("rawCap", x.RawSkillCap.Value) : null,
				x.CapRelativeProficiency ? new XAttribute("capRelative", true) : null,
				x.Prerequisites.Select(SavePrerequisite))),
			p.Supports.Select(x => new XElement("SupportGrant", new XAttribute("key", x.Key), new XAttribute("trait", x.TraitId),
				new XAttribute("opening", x.OpeningSkill), new XAttribute("starting", x.Starting),
				x.RawSkillCap.HasValue ? new XAttribute("rawCap", x.RawSkillCap.Value) : null, x.Prerequisites.Select(SavePrerequisite)))));
	}

	private void CloneCastingFrom(SkillLevelBasedMagicCapability source)
	{
		_castingLoadError = source._castingLoadError;
		_unreadableCasting = source._unreadableCasting is null ? null : new XElement(source._unreadableCasting);
		if (source.CastingPolicy is not { } policy) return;
		CastingPolicy = policy with { Identity = Guid.NewGuid(), Admissions = Array.AsReadOnly(policy.Admissions.Select(x => x with
		{
			Key = Guid.NewGuid(), Prerequisites = Array.AsReadOnly(x.Prerequisites.Select(e => e with { Key = Guid.NewGuid() }).ToArray())
		}).ToArray()), SupportGrants = Array.AsReadOnly(policy.Supports.Select(x => x with
		{
			Key = Guid.NewGuid(), Prerequisites = Array.AsReadOnly(x.Prerequisites.Select(e => e with { Key = Guid.NewGuid() }).ToArray())
		}).ToArray()) };
	}

	public IReadOnlyList<string> CastingConfigurationErrors()
	{
		List<string> errors = [];
		if (_castingLoadError is not null) errors.Add(_castingLoadError);
		if (CastingPolicy is not { } p) return errors.AsReadOnly();
		if (p.Patron is { } patron)
		{
			foreach (var (spellId, incantation) in patron.Incantations ?? new Dictionary<long, ControlledSpellIncantation>())
			{
				if (!p.Admissions.Any(x => x.SpellId == spellId) || Gameworld.MagicSpells.Get(spellId) is not MagicSpell spell)
					errors.Add($"Patron incantation #{spellId} must reference an admitted native spell.");
				else errors.AddRange(spell.IncantationConfigurationErrors(incantation).Select(x => $"Patron spell #{spellId}: {x}"));
			}
			if (patron.FixedGrade is < 1 or > 7 || patron.SelectionLimit < 0 || patron.PermanentSpells.Distinct().Count() != patron.PermanentSpells.Length || patron.PermanentSpells.Any(x => !p.Admissions.Any(a => a.SpellId == x))) errors.Add("Invalid patron grade, selection limit or permanent spell list.");
			foreach (var id in new[] { patron.AuthorityProgId, patron.TerritoryProgId })
			{
				var prog = Gameworld.FutureProgs.Get(id);
				if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !prog.MatchesParameters([ProgVariableTypes.Character])) errors.Add($"Patron prog #{id} must be boolean(character).");
			}
		}
		if (this is IVancianMagicCapability && p.Enabled) errors.Add("Casting cannot be enabled on a Vancian capability.");
		if (p.Identity == Guid.Empty || p.Version != 1 || p.StartingGrantVersion < 1) errors.Add("Invalid casting identity/version.");
		CheckTrait(p.DefaultTraitId, "Default trait");
		if (Gameworld.MagicResources.Get(p.SourceResourceId) is null) errors.Add($"Missing source resource {p.SourceResourceId}.");
		if (Gameworld.MagicResources.Get(p.ReserveResourceId) is null) errors.Add($"Missing reserve {p.ReserveResourceId}.");
		if (p.Admissions.Count > 512) errors.Add("A capability supports at most 512 admissions.");
		var keys = p.Admissions.Select(x => x.Key).Concat(p.Supports.Select(x => x.Key))
			.Concat(p.Admissions.SelectMany(x => x.Prerequisites).Concat(p.Supports.SelectMany(x => x.Prerequisites)).Select(x => x.Key)).ToArray();
		if (keys.Any(x => x == Guid.Empty) || keys.Distinct().Count() != keys.Length) errors.Add("Duplicate or empty admission/prerequisite keys.");
		if (p.Admissions.Select(x => x.SpellId).Distinct().Count() != p.Admissions.Count) errors.Add("Duplicate spell admissions.");
		foreach (var a in p.Admissions)
		{
			CheckTrait(a.TraitId ?? p.DefaultTraitId, $"Admission {a.Key}");
			if (a.RawSkillCap.HasValue && Gameworld.Traits.Get(a.TraitId ?? p.DefaultTraitId) is MudSharp.Body.Traits.Subtypes.ITheoreticalSkillDefinition)
				errors.Add($"Admission {a.Key}: capped theoretical skills are unsupported; select a native single-value skill or an uncapped binding.");
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
			if (a.Prerequisites.Count > 512 || a.Prerequisites.Select(x => (x.Kind, x.SourceId)).Distinct().Count() != a.Prerequisites.Count)
				errors.Add($"Admission {a.Key}: duplicate or excessive prerequisites.");
			foreach (var e in a.Prerequisites)
			{
				var prior = p.Admissions.FirstOrDefault(x => x.SpellId == e.SpellId);
				if (e.Kind == MagicCastingPrerequisiteKind.SupportTrait) continue;
				if (prior is null || e.MinimumGrade < 1 || e.MinimumGrade > prior.MaximumGrade || !double.IsFinite(e.MinimumProficiency) || e.MinimumProficiency < 0)
					errors.Add($"Prerequisite {e.Key}: invalid grade/proficiency or spell not admitted by this capability.");
			}
		}
		ValidateSupportGraph(p, errors);
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
		sb.AppendLine("Casting Policy:\n");
		if (CastingPolicy is { } p)
		{
			sb.AppendLine($"\tIdentity: {p.Identity.ToString().ColourName()}");
			sb.AppendLine($"\tEnabled: {p.Enabled.ToColouredString()}");
			var trait = Gameworld.Traits.Get(p.DefaultTraitId);
			sb.AppendLine($"\tTrait: {trait?.NameAndId(actor) ?? "None".ColourError()}");
			var sourceResource = Gameworld.MagicResources.Get(p.SourceResourceId);
			sb.AppendLine($"\tSource Energy: {sourceResource?.NameAndId(actor) ?? "None".ColourError()}");
			var reserveResource = Gameworld.MagicResources.Get(p.ReserveResourceId);
			sb.AppendLine($"\tReserve Energy: {reserveResource?.NameAndId(actor) ?? "None".ColourError()}");
			sb.AppendLine($"\tPassive Entitlement: {p.PassiveEntitlement.ToColouredString()}");
			sb.AppendLine($"\tStarting Grant Version: {p.StartingGrantVersion.ToStringN0Colour(actor, Telnet.Green)}");
			sb.AppendLine($"\tEnrol After Class Selection: {p.EnrolOnClassSelection.ToColouredString()}");
			sb.AppendLine();
			sb.AppendLine("Spells:");
			sb.AppendLine();
			foreach (var a in p.Admissions)
			{
				var spell = Gameworld.MagicSpells.Get(a.SpellId);
				if (spell is null)
				{
					continue;
				}

				sb.AppendLine($"{spell.NameAndId(actor, false)}".GetLineWithTitleInner(actor, School.PowerListColour, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"\tIdentity: {a.Key.ToString().ColourName()}");
				trait = Gameworld.Traits.Get(a.TraitId ?? p.DefaultTraitId);
				sb.AppendLine($"\tTrait: {trait?.NameAndId(actor) ?? "None".ColourError()}");
				sb.AppendLine($"\tGrades: {a.MinimumGrade}-{a.MaximumGrade}");
				sb.AppendLine($"\tStarting Spell: {a.Starting.ToColouredString()}");
				sb.AppendLine($"\tOpens At: {a.OpeningSkill?.ToStringN2Colour(actor, Telnet.Green) ?? "Profile Default".ColourCommand()}");
				sb.AppendLine($"\tCaps At: {a.RawSkillCap?.ToStringN2Colour(actor, Telnet.Green) ?? "Native Skill Value".ColourCommand()}");
				sb.AppendLine($"\tProficiency Gates: {(a.CapRelativeProficiency ? "Percent of Route Cap" : "Absolute Raw Skill").ColourValue()}");
				sb.AppendLine();
				sb.AppendLine("Prerequisites:");
				sb.AppendLine();
				sb.AppendLine(StringUtilities.GetTextTable(
					from e in a.Prerequisites
					let sourcetext = (e.Kind == MagicCastingPrerequisiteKind.Spell ?
						Gameworld.MagicSpells.Get(e.SourceId)?.NameAndId(actor, false) :
						Gameworld.Traits.Get(e.SourceId)?.NameAndId(actor, false)) ?? "None".ColourError()
					select new List<string> {
						e.Kind.DescribeEnum(),
						sourcetext,
						e.MinimumGrade.ToStringN0(actor),
						e.MinimumProficiency.ToStringN2(actor),
						e.Key.ToString()
						},
						[
							"Kind",
							"Source",
							"Min Grade",
							"Min Proficiency",
							"Identity"
						],
					actor,
					Telnet.Red
				));
			}
			sb.AppendLine();
			sb.AppendLine("Supports:");
			sb.AppendLine();
			
			foreach (var s in p.Supports)
			{
				var supportTrait = Gameworld.Traits.Get(s.TraitId);
				if (supportTrait is null)
				{
					continue;
				}
				sb.AppendLine($"{supportTrait.NameAndId(actor, false)}".GetLineWithTitleInner(actor, School.PowerListColour, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"\tIdentity: {s.Key.ToString().ColourName()}");
				sb.AppendLine($"\tOpens At: {s.OpeningSkill.ToStringN2Colour(actor, Telnet.Green)}");
				sb.AppendLine($"\tCaps At: {s.RawSkillCap?.ToStringN2Colour(actor, Telnet.Green) ?? "Native Skill Value".ColourCommand()}");
				sb.AppendLine($"\tStarting Skill: {s.Starting.ToColouredString()}");
				sb.AppendLine();
				sb.AppendLine("Prerequisites:");
				sb.AppendLine();
				sb.AppendLine(StringUtilities.GetTextTable(
					from e in s.Prerequisites
					let sourcetext = (e.Kind == MagicCastingPrerequisiteKind.Spell ?
						Gameworld.MagicSpells.Get(e.SourceId)?.NameAndId(actor, false) :
						Gameworld.Traits.Get(e.SourceId)?.NameAndId(actor, false)) ?? "None".ColourError()
					select new List<string> {
						e.Kind.DescribeEnum(),
						sourcetext,
						e.MinimumGrade.ToStringN0(actor),
						e.MinimumProficiency.ToStringN2(actor),
						e.Key.ToString()
						},
						[
							"Kind",
							"Source",
							"Min Grade",
							"Min Proficiency",
							"Identity"
						],
					actor,
					Telnet.Red
				));
			}
		}
		foreach (var error in CastingConfigurationErrors()) sb.AppendLine(error.ColourError());
	}
}
