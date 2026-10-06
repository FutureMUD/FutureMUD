using MudSharp.Body.Traits;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	private string? _gradeLoadError;
	private XElement? _unreadableGradeProfile;
	public ControlledSpellProfile? GradeProfile { get; private set; }

	public static ControlledSpellProfile FixtureGradeProfile() => new(1,
		Array.AsReadOnly(new[]
		{
			new ControlledSpellGrade(1, SpellPower.ExtremelyWeak, 0, 0),
			new ControlledSpellGrade(2, SpellPower.VeryWeak, 20, 0),
			new ControlledSpellGrade(3, SpellPower.Weak, 40, 1),
			new ControlledSpellGrade(4, SpellPower.Standard, 55, 1),
			new ControlledSpellGrade(5, SpellPower.Strong, 70, 2),
			new ControlledSpellGrade(6, SpellPower.VeryStrong, 85, 2),
			new ControlledSpellGrade(7, SpellPower.ExtremelyStrong, 95, 3)
		}), 1.5, 1, 0.25, TimeSpan.FromSeconds(600), TimeSpan.FromSeconds(60), 10, Array.Empty<SpellScalarBinding>());

	private void LoadGradeProfile(XElement definition)
	{
		if (definition.Element("ControlledPower") is not { } root) return;
		try
		{
			if ((int?)root.Attribute("schema") != 1) throw new FormatException("Unsupported controlled-power schema.");
			GradeProfile = new((int)root.Attribute("version")!, Array.AsReadOnly(root.Elements("Grade").Select(x =>
				new ControlledSpellGrade((int)x.Attribute("number")!, (SpellPower)(int)x.Attribute("power")!,
					(double)x.Attribute("skill")!, (int)x.Attribute("difficulty")!)).ToArray()),
				(double)root.Attribute("overreachCost")!, (int)root.Attribute("overreachDifficulty")!,
				(double)root.Attribute("masteryChance")!, TimeSpan.FromSeconds((double)root.Attribute("masterySeconds")!),
				TimeSpan.FromSeconds((double)root.Attribute("skillSeconds")!), (double)root.Attribute("openingSkill")!,
				Array.AsReadOnly((root.Element("ScalarBindings")?.Elements("Binding") ?? []).Select(x =>
					new SpellScalarBinding((string)x.Attribute("list")!, (int)x.Attribute("index")!, (string)x.Attribute("effect")!,
						(string)x.Attribute("field")!, x.Value)).ToArray()),
				root.Element("Efficiency") is { } efficiency ? LoadEfficiency(efficiency) : null,
				root.Element("Practice") is { } practice ? LoadPractice(practice) : null,
				root.Element("Incantation") is { } incantation ? LoadIncantation(incantation) : null,
				root.Element("Area") is { } area ? LoadArea(area) : null);
		}
		catch (Exception ex) { _gradeLoadError = $"ControlledPower: {ex.Message}"; _unreadableGradeProfile = new(root); }
	}

	private static ControlledSpellEfficiency LoadEfficiency(XElement root) => (string?)root.Attribute("type") == "source"
		? new((double)root.Attribute("minimum")!, (double)root.Attribute("scale")!)
		: throw new FormatException("Unsupported controlled-power efficiency type.");

	private XElement? SaveGradeProfile()
	{
		if (_unreadableGradeProfile is not null) return new(_unreadableGradeProfile);
		if (GradeProfile is not { } p) return null;
		return new("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", p.Version),
			new XAttribute("overreachCost", p.OverreachMultiplier), new XAttribute("overreachDifficulty", p.OverreachDifficultySteps),
			new XAttribute("masteryChance", p.MasteryChance), new XAttribute("masterySeconds", p.MasteryInterval.TotalSeconds),
			new XAttribute("skillSeconds", p.SkillInterval.TotalSeconds), new XAttribute("openingSkill", p.OpeningSkill),
			p.Efficiency is { } efficiency ? new XElement("Efficiency", new XAttribute("type", "source"),
				new XAttribute("minimum", efficiency.MinimumCost), new XAttribute("scale", efficiency.Scale)) : null,
			SavePractice(),
			SaveIncantation(),
			SaveArea(),
			p.Grades.Select(x => new XElement("Grade", new XAttribute("number", x.Grade), new XAttribute("power", (int)x.Power),
				new XAttribute("skill", x.MinimumProficiency), new XAttribute("difficulty", x.DifficultySteps))),
			new XElement("ScalarBindings", p.ScalarBindings.Select(x => new XElement("Binding", new XAttribute("list", x.List),
				new XAttribute("index", x.Index), new XAttribute("effect", x.Effect), new XAttribute("field", x.Field), new XCData(x.Expression)))));
	}

	public IReadOnlyList<string> GradeConfigurationErrors()
	{
		List<string> errors = [];
		if (_gradeLoadError is not null) errors.Add(_gradeLoadError);
		if (GradeProfile is not { } p) return errors.AsReadOnly();
		if (p.Version < 1 || p.Grades.Count is < 1 or > 7 || !p.Grades.Select(x => x.Grade).SequenceEqual(Enumerable.Range(1, p.Grades.Count)))
			errors.Add("ControlledPower: requires a positive version and one to seven consecutively ordered grades.");
		if (p.Efficiency is { IsValid: false }) errors.Add("ControlledPower: invalid source efficiency minimum or scale.");
		errors.AddRange(IncantationConfigurationErrors(p.Incantation));
		errors.AddRange(AreaConfigurationErrors(p.Area));
		if (p.Practice is { } practice && (!Enum.IsDefined(practice.Difficulty) || practice.Difficulty >= Difficulty.Impossible ||
			practice.Duration <= TimeSpan.Zero || practice.Duration > TimeSpan.FromDays(1) ||
			!double.IsFinite(practice.EnergyMultiplier) || practice.EnergyMultiplier <= 0 ||
			practice.MaximumGrade is { } maximum && (maximum < 1 || maximum > p.Grades.Count) || PracticeInventoryPlanTemplate is null))
			errors.Add("ControlledPower: invalid practice difficulty, duration, energy multiplier, maximum grade or explicit material plan.");
		if (!double.IsFinite(p.OverreachMultiplier) || p.OverreachMultiplier < 1 || p.OverreachDifficultySteps < 0 ||
			!double.IsFinite(p.MasteryChance) || p.MasteryChance is < 0 or > 1 || p.MasteryInterval <= TimeSpan.Zero ||
			p.SkillInterval <= TimeSpan.Zero || !double.IsFinite(p.OpeningSkill) || p.OpeningSkill < 0)
			errors.Add("ControlledPower: invalid cost, chance, opportunity interval or opening skill.");
		foreach (var g in p.Grades)
		{
			if (!Enum.IsDefined(g.Power) || !double.IsFinite(g.MinimumProficiency) || g.MinimumProficiency < 0 || g.DifficultySteps < 0)
				errors.Add($"Grade {g.Grade}: invalid power, proficiency or difficulty.");
			if (Trigger is ICastMagicTrigger trigger && (g.Power < trigger.MinimumPower || g.Power > trigger.MaximumPower))
				errors.Add($"Grade {g.Grade}: power is outside the trigger range.");
		}
		errors.AddRange(CastingNumerics.ConfigurationErrors(this));
		return errors.AsReadOnly();
	}

	private void AppendGradeShow(StringBuilder sb, ICharacter actor)
	{
		if (GradeProfile is not { } p) { if (_gradeLoadError is not null) sb.AppendLine(_gradeLoadError.ColourError()); return; }
		sb.AppendLine($"Controlled Power Profile v{p.Version}: mastery {p.MasteryChance.ToString("P0", actor)} / {p.MasteryInterval.Describe(actor)}, skill interval {p.SkillInterval.Describe(actor)}");
		if (p.Efficiency is { } efficiency) sb.AppendLine($"  Source energy curve: minimum {efficiency.MinimumCost.ToString("N2", actor)}, scale {efficiency.Scale.ToString("N2", actor)}; replaces the designated cost expression before overreach.");
		AppendPracticeShow(sb, actor);
		AppendIncantationShow(sb, actor);
		AppendAreaShow(sb, actor);
		foreach (var g in p.Grades) sb.AppendLine($"  Grade {g.Grade}: {g.Power.DescribeEnum().ColourName()}, raw skill {g.MinimumProficiency.ToString("N2", actor)}, difficulty +{g.DifficultySteps}");
		foreach (var b in p.ScalarBindings) sb.AppendLine($"  {b.List}[{b.Index}] {b.Effect}.{b.Field} = {b.Expression.ColourCommand()}");
	}
}
