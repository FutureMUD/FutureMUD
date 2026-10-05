#nullable enable

using System.Collections.ObjectModel;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Emotions;

public sealed record EmotionalTerrainBinding(long TerrainId, EmotionalTerrainRule Rule);

/// <summary>Immutable editable content. This is not casting admission or an emotional runtime hook.</summary>
public sealed class EmotionalStockProfile
{
	public EmotionalStockProfile(EmotionalSpellKind kind, string group, int unitSeconds, int capUnits,
		long eligibilityProgId, double intensity, long traitId, double unitsPerSourcePoint,
		EmotionalTerrainRule fallback, IEnumerable<EmotionalTerrainBinding> terrains,
		IEnumerable<KeyValuePair<int, Difficulty>> saves, bool breakOnAdmittedAttack)
	{
		if (!Enum.IsDefined(kind)) throw new ArgumentException("Select Fury or Calm.");
		if (DetectInvisibleEffect.PolicyError(new(group, unitSeconds, capUnits)) is { } error)
			throw new ArgumentException(error);
		if (eligibilityProgId <= 0 || traitId <= 0 || !double.IsFinite(intensity) || intensity < 0)
			throw new ArgumentException("Select positive native prog/trait IDs and a finite nonnegative intensity.");
		ArgumentNullException.ThrowIfNull(fallback);
		ArgumentNullException.ThrowIfNull(terrains);
		ArgumentNullException.ThrowIfNull(saves);
		var terrainList = terrains.ToArray();
		var saveList = saves.ToArray();
		if (terrainList.Any(x => x is null || x.TerrainId <= 0 || x.Rule is null) ||
			terrainList.Select(x => x.TerrainId).Distinct().Count() != terrainList.Length)
			throw new ArgumentException("Native terrain mappings must be positive and unambiguous.");
		if (saveList.Select(x => x.Key).Distinct().Count() != saveList.Length ||
			saveList.Any(x => x.Key is < 1 or > 7 || !Enum.IsDefined(x.Value)))
			throw new ArgumentException("Save grades/difficulties must be valid and unique.");
		if (kind == EmotionalSpellKind.Fury && (!double.IsFinite(unitsPerSourcePoint) || unitsPerSourcePoint <= 0 ||
			saveList.Length != 0 || breakOnAdmittedAttack))
			throw new ArgumentException("Fury requires positive attribute units, no save and no Calm attack-break policy.");
		if (kind == EmotionalSpellKind.Calm && (unitsPerSourcePoint != 0 || terrainList.Length != 0 || saveList.Length != 7))
			throw new ArgumentException("Calm requires all seven saves and no endurance/terrain bonus mapping.");
		foreach (var rule in terrainList.Select(x => x.Rule).Append(fallback))
		{
			for (var grade = 1; grade <= 7; grade++)
			{
				rule.DurationUnits(grade);
				var maximum = EmotionalSpellPolicy.RollEndurance(grade, rule, (_, upper) => upper);
				if (kind == EmotionalSpellKind.Fury && !double.IsFinite(maximum * unitsPerSourcePoint))
					throw new ArgumentException("The delivered attribute bonus would overflow.");
			}
		}
		if (kind == EmotionalSpellKind.Calm && fallback != new EmotionalTerrainRule(2, 1, 0))
			throw new ArgumentException("Calm uses two units per grade and no endurance bonus.");
		Kind = kind; Group = group; UnitSeconds = unitSeconds; CapUnits = capUnits;
		EligibilityProgId = eligibilityProgId; Intensity = intensity; TraitId = traitId;
		UnitsPerSourcePoint = unitsPerSourcePoint; Fallback = fallback;
		Terrains = Array.AsReadOnly(terrainList);
		Saves = new ReadOnlyDictionary<int, Difficulty>(saveList.ToDictionary(x => x.Key, x => x.Value));
		BreakOnAdmittedAttack = breakOnAdmittedAttack;
	}

	public EmotionalSpellKind Kind { get; }
	public string Group { get; }
	public int UnitSeconds { get; }
	public int CapUnits { get; }
	public long EligibilityProgId { get; }
	public double Intensity { get; }
	public long TraitId { get; }
	public double UnitsPerSourcePoint { get; }
	public EmotionalTerrainRule Fallback { get; }
	public IReadOnlyList<EmotionalTerrainBinding> Terrains { get; }
	public IReadOnlyDictionary<int, Difficulty> Saves { get; }
	public bool BreakOnAdmittedAttack { get; }
	public const string HistoricalExceptionLimit = "Undead, Quickening, Insomnia and Mul/Thodeliv exceptions remain unmapped; no native setting IDs are inferred.";

	public EmotionalTerrainRule TerrainRule(long terrainId)
	{
		if (terrainId <= 0) throw new ArgumentException("A real native caster terrain identity is required.");
		return Terrains.SingleOrDefault(x => x.TerrainId == terrainId)?.Rule ?? Fallback;
	}

	public Difficulty SaveDifficulty(int residualGrade)
	{
		EmotionalSpellPolicy.ValidateGrade(residualGrade);
		if (Kind != EmotionalSpellKind.Calm) throw new InvalidOperationException("Source Fury has no opposed save.");
		return Saves[residualGrade];
	}

	/// <summary>Delegates source arithmetic only; callers still need sealed admission and native lifecycle.</summary>
	public EmotionalLifetime ResolveLifetime(int residualGrade, long terrainId, SpellPower power,
		Func<int, int, int> inclusiveRandom, TimeSpan? remaining = null, EmotionalRetainedState? previous = null)
	{
		ArgumentNullException.ThrowIfNull(inclusiveRandom);
		var terrain = TerrainRule(terrainId);
		var endurance = Kind == EmotionalSpellKind.Fury
			? EmotionalSpellPolicy.RollEndurance(residualGrade, terrain, inclusiveRandom) : 0;
		return EmotionalSpellPolicy.ResolveLifetime(Kind, residualGrade, terrain, UnitSeconds, CapUnits,
			new(residualGrade, power, Intensity, endurance), remaining, previous);
	}

	/// <summary>Installer supplies the inferred attribute. Runtime never repeats seeder inference.</summary>
	public void ValidateBindings(IFuturemud world)
	{
		var trait = world.Traits.Get(TraitId);
		if (trait is null || (Kind == EmotionalSpellKind.Fury && trait.TraitType != TraitType.Attribute))
			throw new InvalidOperationException("Map an existing native trait; Fury requires an ordinary attribute, not a derived attribute.");
		var prog = world.FutureProgs.Get(EligibilityProgId);
		if (prog is null || prog.ReturnType != ProgVariableTypes.Boolean ||
			!prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]) || !prog.Compile())
			throw new InvalidOperationException("Map a compiling boolean eligibility prog with (target, caster) parameters.");
		if (Terrains.Any(x => world.Terrains.Get(x.TerrainId) is null))
			throw new InvalidOperationException("A mapped native terrain no longer exists.");
	}

	public XElement SaveToXml() => new("SourceProfile", new XAttribute("version", 1), new XAttribute("kind", Kind),
		new XAttribute("eligibility", EligibilityProgId), new XAttribute("intensity", Intensity),
		new XAttribute("trait", TraitId), new XAttribute("units", UnitsPerSourcePoint),
		new XAttribute("attackbreak", BreakOnAdmittedAttack),
		new XElement("Lifetime", new XAttribute("group", Group), new XAttribute("seconds", UnitSeconds), new XAttribute("cap", CapUnits)),
		WriteRule("Fallback", Fallback),
		new XElement("Terrains", Terrains.Select(x => WriteRule("Terrain", x.Rule, x.TerrainId))),
		new XElement("Saves", Saves.OrderBy(x => x.Key).Select(x => new XElement("Grade", new XAttribute("number", x.Key),
			new XAttribute("difficulty", (int)x.Value)))),
		new XElement("HistoricalExceptions", new XAttribute("status", "unmapped")));

	public static EmotionalStockProfile Read(XElement root)
	{
		var names = new[] { "Lifetime", "Fallback", "Terrains", "Saves", "HistoricalExceptions" };
		if (names.Any(name => root.Elements(name).Count() != 1) || root.Elements().Any(x => !names.Contains(x.Name.LocalName)))
			throw new ArgumentException("The profile must have exactly one of each supported configuration section.");
		if (root.Name != "SourceProfile" || (int?)root.Attribute("version") != 1 ||
			!Enum.TryParse<EmotionalSpellKind>((string?)root.Attribute("kind"), false, out var kind) ||
			root.Element("HistoricalExceptions")?.Attribute("status")?.Value != "unmapped")
			throw new ArgumentException("Unsupported source profile kind/version or historical exception mapping.");
		var lifetime = root.Element("Lifetime") ?? throw new ArgumentException("A lifetime profile is required.");
		return new(kind, (string?)lifetime.Attribute("group") ?? "", (int)lifetime.Attribute("seconds")!,
			(int)lifetime.Attribute("cap")!, (long)root.Attribute("eligibility")!, (double)root.Attribute("intensity")!,
			(long)root.Attribute("trait")!, (double)root.Attribute("units")!,
			ReadRule(root.Element("Fallback") ?? throw new ArgumentException("A fallback rule is required.")),
			(root.Element("Terrains") ?? throw new ArgumentException("Terrain mappings are required.")).Elements("Terrain")
				.Select(x => new EmotionalTerrainBinding((long)x.Attribute("id")!, ReadRule(x))),
			(root.Element("Saves") ?? throw new ArgumentException("Save mappings are required.")).Elements("Grade")
				.Select(x => new KeyValuePair<int, Difficulty>((int)x.Attribute("number")!, (Difficulty)(int)x.Attribute("difficulty")!)),
			(bool)root.Attribute("attackbreak")!);
	}

	private static XElement WriteRule(string name, EmotionalTerrainRule rule, long? id = null) => new(name,
		id.HasValue ? new XAttribute("id", id.Value) : null, new XAttribute("numerator", rule.DurationNumerator),
		new XAttribute("denominator", rule.DurationDenominator), new XAttribute("endurance", rule.EnduranceBonusPerGrade));
	private static EmotionalTerrainRule ReadRule(XElement root) => new((int)root.Attribute("numerator")!,
		(int)root.Attribute("denominator")!, (int)root.Attribute("endurance")!);
}
