using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Capabilities;

/// <summary>Uses only the non-inserting base loader for shared concentration/power configuration.</summary>
public sealed partial class VancianMagicCapability : SkillLevelBasedMagicCapability, IVancianMagicCapability
{
	private readonly List<VancianRepertoireDefinition> _repertoires = [];
	private readonly List<VancianCastingAllowanceDefinition> _allowances = [];
	private readonly Dictionary<string, long> _policyProgs = new(StringComparer.OrdinalIgnoreCase);
	private string? _definitionError;
	public IReadOnlyList<VancianRepertoireDefinition> Repertoires => _repertoires.AsReadOnly();
	public IReadOnlyList<VancianCastingAllowanceDefinition> Allowances => _allowances.AsReadOnly();
	public IReadOnlyDictionary<string, long> PolicyProgs => _policyProgs.AsReadOnly();
	public SpellPower BasePower { get; private set; } = SpellPower.Standard;
	public int PowerStepPerSlotLevel { get; private set; } = 1;
	public Outcome ReliableOutcome { get; private set; } = Outcome.Pass;
	public int MaximumSavedLoadouts { get; private set; } = 10;
	public VancianRecoveryMode RecoveryMode { get; private set; }
	public TimeSpan PreparationDuration { get; private set; } = TimeSpan.FromMinutes(10);
	public TimeSpan RequiredSleepDuration { get; private set; } = TimeSpan.FromHours(8);
	public TimeSpan MinimumRefreshInterval { get; private set; } = TimeSpan.FromHours(20);
	private long _scrollTraitId;
	public ITraitDefinition ScrollCheckTrait => (_scrollTraitId == 0 ? ConcentrationTrait : Gameworld.Traits.Get(_scrollTraitId))!;
	public Outcome ScrollMinimumOutcome { get; private set; } = Outcome.MinorPass;
	public override IEnumerable<IMagicPower> InherentPowers(ICharacter actor) => ConfigurationErrors().Count == 0 ? base.InherentPowers(actor) : [];
	public override double ConcentrationAbility(ICharacter actor) => ConfigurationErrors().Count == 0 ? base.ConcentrationAbility(actor) : 0.0;

	public VancianMagicCapability(Models.MagicCapability model, IFuturemud gameworld) : base(model, gameworld, true)
	{
		try
		{
			if (ConfigurationLoadError is not null) throw new FormatException($"Shared concentration/power configuration: {ConfigurationLoadError}");
			var root = XElement.Parse(model.Definition).Element("Vancian") ?? throw new FormatException("Missing Vancian definition.");
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported Vancian definition version.");
			BasePower = (SpellPower)(int)root.Attribute("basePower")!;
			PowerStepPerSlotLevel = (int)root.Attribute("step")!;
			ReliableOutcome = (Outcome)(int)root.Attribute("outcome")!;
			MaximumSavedLoadouts = (int)root.Attribute("loadouts")!;
			RecoveryMode = Enum.Parse<VancianRecoveryMode>((string)root.Attribute("recovery")!);
			PreparationDuration = TimeSpan.FromSeconds((double)root.Attribute("prepareSeconds")!);
			RequiredSleepDuration = TimeSpan.FromSeconds((double)root.Attribute("sleepSeconds")!);
			MinimumRefreshInterval = TimeSpan.FromSeconds((double)root.Attribute("intervalSeconds")!);
			_scrollTraitId = (long?)root.Attribute("scrollTrait") ?? 0;
			ScrollMinimumOutcome = (Outcome)((int?)root.Attribute("scrollOutcome") ?? (int)Outcome.MinorPass);
			ShowMagicResourcesInPrompt = (bool?)root.Attribute("showResources") ?? true;
			foreach (var prog in root.Elements("Policy")) _policyProgs.Add((string)prog.Attribute("name")!, (long)prog.Attribute("id")!);
			foreach (var rule in root.Elements("Repertoire"))
				_repertoires.Add(new(Guid.Parse((string)rule.Attribute("key")!), (string)rule.Attribute("alias")!,
					(string)rule.Attribute("name")!, (int)rule.Attribute("order")!, Enum.Parse<VancianRepertoireSource>((string)rule.Attribute("source")!),
					(int)rule.Attribute("min")!, (int)rule.Attribute("max")!, (long)rule.Attribute("candidate")!, (long)rule.Attribute("limit")!,
					Enum.Parse<VancianBookPolicy>((string)rule.Attribute("bookPolicy")!)));
			foreach (var allowance in root.Elements("Allowance"))
				_allowances.Add(new(Guid.Parse((string)allowance.Attribute("key")!), (string)allowance.Attribute("alias")!,
					(string)allowance.Attribute("name")!, (int)allowance.Attribute("order")!, Enum.Parse<VancianAllowanceMode>((string)allowance.Attribute("mode")!),
					(int?)allowance.Attribute("level"), Array.AsReadOnly(allowance.Elements("Rule").Select(x => Guid.Parse(x.Value)).ToArray()),
					(int)allowance.Attribute("version")!, (int)allowance.Attribute("min")!, (int)allowance.Attribute("max")!,
					(long)allowance.Attribute("count")!, (long)allowance.Attribute("eligibility")!));
		}
		catch (Exception ex) { _definitionError = ex.Message; }
	}

	private static XElement InitialDefinition(ITraitDefinition trait) => new("Definition",
		new XElement("ConcentrationTrait", trait.Id), new XElement("ConcentrationCapabilityExpression", "1"),
		new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
		new XElement("Vancian", new XAttribute("version", 1), new XAttribute("basePower", (int)SpellPower.Standard),
			new XAttribute("step", 1), new XAttribute("outcome", (int)Outcome.Pass), new XAttribute("loadouts", 10),
			new XAttribute("recovery", VancianRecoveryMode.PreparationAction), new XAttribute("prepareSeconds", 600),
			new XAttribute("sleepSeconds", 28800), new XAttribute("intervalSeconds", 72000)));

	public new static void RegisterLoader()
	{
		MagicCapabilityFactory.RegisterLoader("vancian", (model, game) => new VancianMagicCapability(model, game));
		MagicCapabilityFactory.RegisterBuilderLoader("vancian", (game, actor, args, name) =>
		{
			var school = game.MagicSchools.GetByIdOrName(args.PopSpeech());
			var trait = game.Traits.GetByIdOrName(args.PopSpeech());
			if (school is null || trait is null)
			{
				actor.OutputHandler.Send("Use magic capability edit new vancian <name> <school> <concentration-trait>.");
				return null!;
			}
			var model = new Models.MagicCapability { Name = name, MagicSchoolId = school.Id, PowerLevel = 1,
				CapabilityModel = "vancian", Definition = InitialDefinition(trait).ToString() };
			using (new FMDB()) { FMDB.Context.MagicCapabilities.Add(model); FMDB.Context.SaveChanges(); }
			return new VancianMagicCapability(model, game);
		});
	}

	public override string SaveToXml()
	{
		if (_definitionError is not null) throw new InvalidOperationException($"Repair the invalid Vancian definition before saving: {_definitionError}");
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("Vancian", new XAttribute("version", 1), new XAttribute("basePower", (int)BasePower),
			new XAttribute("step", PowerStepPerSlotLevel), new XAttribute("outcome", (int)ReliableOutcome),
			new XAttribute("loadouts", MaximumSavedLoadouts), new XAttribute("recovery", RecoveryMode),
			new XAttribute("prepareSeconds", PreparationDuration.TotalSeconds), new XAttribute("sleepSeconds", RequiredSleepDuration.TotalSeconds),
			new XAttribute("intervalSeconds", MinimumRefreshInterval.TotalSeconds), new XAttribute("scrollTrait", _scrollTraitId),
			new XAttribute("scrollOutcome", (int)ScrollMinimumOutcome),
			new XAttribute("showResources", ShowMagicResourcesInPrompt),
			_policyProgs.OrderBy(x => x.Key).Select(x => new XElement("Policy", new XAttribute("name", x.Key), new XAttribute("id", x.Value))),
			_repertoires.Select(x => new XElement("Repertoire", new XAttribute("key", x.Key), new XAttribute("alias", x.Alias),
				new XAttribute("name", x.Name), new XAttribute("order", x.SortOrder), new XAttribute("source", x.Source),
				new XAttribute("min", x.MinimumSpellLevel), new XAttribute("max", x.MaximumSpellLevel), new XAttribute("candidate", x.CandidateProgId),
				new XAttribute("limit", x.SelectionLimitProgId), new XAttribute("bookPolicy", x.BookPolicy))),
			_allowances.Select(x => new XElement("Allowance", new XAttribute("key", x.Key), new XAttribute("alias", x.Alias),
				new XAttribute("name", x.Name), new XAttribute("order", x.SortOrder), new XAttribute("mode", x.Mode),
				x.SlotLevel.HasValue ? new XAttribute("level", x.SlotLevel.Value) : null, new XAttribute("version", x.StructuralVersion),
				new XAttribute("min", x.MinimumSpellLevel), new XAttribute("max", x.MaximumSpellLevel),
				new XAttribute("count", x.SlotCountProgId), new XAttribute("eligibility", x.SpellEligibilityProgId),
				x.RepertoireKeys.Select(key => new XElement("Rule", key))))));
		return root.ToString();
	}

	public override IMagicCapability Clone(string newName)
	{
		var model = CloneModel(newName);
		using (new FMDB()) { FMDB.Context.MagicCapabilities.Add(model); FMDB.Context.SaveChanges(); }
		return new VancianMagicCapability(model, Gameworld);
	}
	internal Models.MagicCapability CloneModel(string newName)
	{
		var root = XElement.Parse(SaveToXml());
		var keys = _repertoires.Select(x => x.Key).Concat(_allowances.Select(x => x.Key)).ToDictionary(x => x, _ => Guid.NewGuid());
		foreach (var attribute in root.Descendants().Attributes("key")) attribute.Value = keys[Guid.Parse(attribute.Value)].ToString();
		foreach (var rule in root.Descendants("Rule")) rule.Value = keys[Guid.Parse(rule.Value)].ToString();
		return new Models.MagicCapability { Name = newName, MagicSchoolId = School.Id, PowerLevel = PowerLevel,
			CapabilityModel = "vancian", Definition = root.ToString() };
	}

	public IReadOnlyList<string> ConfigurationErrors()
	{
		var errors = new List<string>();
		if (_definitionError is not null) errors.Add(_definitionError);
		if (School is null || ConcentrationTrait is null) errors.Add("The school or concentration trait reference is missing.");
		if (ConcentrationCapabilityExpression.HasErrors() || ConcentrationDifficultyExpression.HasErrors()) errors.Add("A shared concentration expression is invalid.");
		void CheckProg(long id, string type, string label, bool required = true)
		{
			if (id == 0 && !required) return;
			if (!VancianPolicy.ValidSignature(Gameworld.FutureProgs.Get(id), type)) errors.Add($"{label}: missing, uncompiled or incorrect {type} prog #{id}.");
		}
		foreach (var (name, id) in _policyProgs) CheckProg(id, name, name);
		foreach (var required in new[] { "casterlevel", "canchangeknown" })
			if (!_policyProgs.ContainsKey(required)) errors.Add($"{required}: a required prog has not been configured.");
		if (!Enum.IsDefined(BasePower) || PowerStepPerSlotLevel < 0) errors.Add("Invalid base power or upcast step.");
		if (ReliableOutcome is not (Outcome.MinorPass or Outcome.Pass or Outcome.MajorPass) ||
			ScrollMinimumOutcome is not (Outcome.MinorPass or Outcome.Pass or Outcome.MajorPass)) errors.Add("Outcomes must be MinorPass, Pass or MajorPass.");
		if (MaximumSavedLoadouts is < 1 or > 1000) errors.Add("Maximum loadouts must be 1-1000.");
		if (!Enum.IsDefined(RecoveryMode) || MinimumRefreshInterval < TimeSpan.Zero ||
			(RecoveryMode != VancianRecoveryMode.SleepAutomatic && PreparationDuration <= TimeSpan.Zero) ||
			(RecoveryMode != VancianRecoveryMode.PreparationAction && RequiredSleepDuration <= TimeSpan.Zero)) errors.Add("Invalid recovery mode or durations.");
		if (ScrollCheckTrait is null) errors.Add("The scroll-control trait is missing.");
		if (_repertoires.Count > 128 || _allowances.Count > 256) errors.Add("At most 128 repertoires and 256 allowances are supported per capability.");
		if (_repertoires.Select(x => x.Key).Concat(_allowances.Select(x => x.Key)).GroupBy(x => x).Any(x => x.Key == Guid.Empty || x.Count() > 1)) errors.Add("Duplicate or empty subordinate keys.");
		if (_repertoires.GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1) ||
			_allowances.GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1)) errors.Add("Duplicate aliases.");
		foreach (var rule in _repertoires)
		{
			var label = $"Repertoire {rule.Alias} ({rule.Key})";
			if (rule.MinimumSpellLevel < 0 || rule.MaximumSpellLevel < rule.MinimumSpellLevel || !Enum.IsDefined(rule.Source) || !Enum.IsDefined(rule.BookPolicy)) errors.Add($"{label}: invalid source, levels or book policy.");
			CheckProg(rule.CandidateProgId, "candidates", label);
			if (rule.Source == VancianRepertoireSource.Selected) CheckProg(rule.SelectionLimitProgId, "limit", label);
		}
		foreach (var allowance in _allowances)
		{
			var label = $"Allowance {allowance.Alias} ({allowance.Key})";
			if (allowance.StructuralVersion < 1 || !Enum.IsDefined(allowance.Mode) || allowance.MinimumSpellLevel < 0 || allowance.MaximumSpellLevel < allowance.MinimumSpellLevel) errors.Add($"{label}: invalid mode, version or levels.");
			if (allowance.RepertoireKeys.Count == 0 || allowance.RepertoireKeys.Distinct().Count() != allowance.RepertoireKeys.Count) errors.Add($"{label}: requires unique explicit repertoire links.");
			foreach (var key in allowance.RepertoireKeys)
			{
				var rule = _repertoires.Find(x => x.Key == key);
				if (rule is null) errors.Add($"{label}: missing repertoire {key}.");
				else if (allowance.Mode != VancianAllowanceMode.Memorised && rule.Source != VancianRepertoireSource.Selected) errors.Add($"{label}: spontaneous/at-will requires Selected repertoires.");
			}
			if (allowance.Mode == VancianAllowanceMode.AtWill)
			{
				if (allowance.SlotLevel.HasValue || allowance.SlotCountProgId != 0) errors.Add($"{label}: at-will has no slot level or count.");
			}
			else
			{
				if (allowance.SlotLevel is null or < 0) errors.Add($"{label}: finite allowance needs a non-negative slot level.");
				CheckProg(allowance.SlotCountProgId, "count", label);
			}
			CheckProg(allowance.SpellEligibilityProgId, "eligibility", label, false);
		}
		return errors.AsReadOnly();
	}
}
