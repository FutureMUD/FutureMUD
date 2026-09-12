using System.Security.Cryptography;
using System.Text.Json;
using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.Magic.Vancian;

/// <summary>Self-contained spell configuration and numerical source; never resolves a live creator.</summary>
public sealed class StoredSpellSnapshot
{
	private readonly string _model;
	private readonly string? _duration;
	private readonly string _numbers;
	public Guid ChargeId { get; }
	public long SpellId { get; }
	public long SchoolId { get; }
	public long CreatorId { get; }
	public DateTime CreatedUtc { get; }
	public string Fingerprint { get; }
	public SpellNumericalContext Numbers => SpellNumericalContext.Load(XElement.Parse(_numbers));
	public int CastingLevel => Numbers.CastingLevel;
	public int SpellLevel => Numbers.SpellLevel;
	public SpellPower Power => Numbers.Power;
	private StoredSpellSnapshot(Guid chargeId, long spellId, long schoolId, long creatorId, DateTime created,
		string model, string? duration, string numbers, string fingerprint)
	{ ChargeId = chargeId; SpellId = spellId; SchoolId = schoolId; CreatorId = creatorId; CreatedUtc = created; _model = model; _duration = duration; _numbers = numbers; Fingerprint = fingerprint; }
	public static StoredSpellSnapshot Capture(MagicSpell spell, ICharacter creator, IVancianMagicCapability capability,
		int level, SpellPower power, int casterLevel, DateTime now)
	{
		var errors = ScrollSpellCompatibility.Errors(spell);
		if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
		var model = spell.SnapshotModel();
		// Stored release has already paid all plan/resource costs, which need not retain live cost-expression references.
		var definition = XElement.Parse(model.Definition);
		ScrollSpellCompatibility.ValidateReferences(definition, spell.Gameworld);
		definition.Element("Costs")!.RemoveNodes();
		definition.Element("Plan")?.ReplaceNodes(new XElement("Phase"));
		model.Definition = definition.ToString(SaveOptions.DisableFormatting);
		var json = JsonSerializer.Serialize(model);
		var duration = spell.EffectDurationExpression?.OriginalFormulaText;
		var context = new SpellNumericalContext(spell.SpellLevel, level, casterLevel, power, capability.ReliableOutcome, true);
		var copy = new MagicSpell(model, spell.Gameworld) { EffectDurationExpression = spell.EffectDurationExpression! };
		copy.SetNoSave(true);
		ScrollSpellCompatibility.Bind(copy, context, creator);
		return new(Guid.NewGuid(), spell.Id, spell.School.Id, VancianPolicy.Owner(creator).Id, now, json, duration,
			context.Save().ToString(SaveOptions.DisableFormatting), Hash(json + "\n" + duration));
	}
	private static string Hash(string input) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
	public XElement Save() => new("StoredSpell", new XAttribute("version", 1), new XAttribute("charge", ChargeId),
		new XAttribute("spell", SpellId), new XAttribute("school", SchoolId), new XAttribute("creator", CreatorId),
		new XAttribute("created", CreatedUtc.ToString("O")), new XAttribute("fingerprint", Fingerprint),
		new XElement("Model", _model), _duration is not null ? new XElement("Duration", _duration) : null, XElement.Parse(_numbers));
	public static StoredSpellSnapshot Load(XElement root)
	{
		if ((int?)root.Attribute("version") != 1 || root.ToString().Length > 8_000_000) throw new FormatException("Unsupported or oversized stored spell snapshot.");
		var model = (string)root.Element("Model")!; var duration = (string?)root.Element("Duration");
		var fingerprint = (string)root.Attribute("fingerprint")!;
		if (Hash(model + "\n" + duration) != fingerprint) throw new FormatException("Stored spell configuration checksum mismatch.");
		var numbers = SpellNumericalContext.Load(root.Element("Numbers")!);
		if (!numbers.IsStored) throw new FormatException("A scroll must contain frozen numerical bindings.");
		if (!Guid.TryParse((string?)root.Attribute("charge"), out var charge) || charge == Guid.Empty ||
			(long?)root.Attribute("spell") is not > 0 || (long?)root.Attribute("school") is not > 0 || (long?)root.Attribute("creator") is not > 0)
			throw new FormatException("Invalid stored charge or provenance identity.");
		return new(Guid.Parse((string)root.Attribute("charge")!), (long)root.Attribute("spell")!, (long)root.Attribute("school")!,
			(long)root.Attribute("creator")!, DateTime.Parse((string)root.Attribute("created")!, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(),
			model, duration, numbers.Save().ToString(SaveOptions.DisableFormatting), fingerprint);
	}
	public MagicSpell CreateSpell(IFuturemud gameworld, bool requireCurrentOptIn = true)
	{
		var live = gameworld.MagicSpells.Get(SpellId);
		if (requireCurrentOptIn && (live is null || !live.ScrollInscriptionAllowed || live.School.Id != SchoolId)) throw new InvalidOperationException("The source spell was deleted, revoked or changed school.");
		var model = JsonSerializer.Deserialize<Models.MagicSpell>(_model) ?? throw new FormatException("Missing stored spell definition.");
		if (model.Id != SpellId || model.MagicSchoolId != SchoolId || model.SpellLevel != SpellLevel || gameworld.MagicSchools.Get(SchoolId) is null) throw new FormatException("Snapshot identity/school mismatch.");
		if (model.ResistingTraitDefinitionId is { } opposed && gameworld.Traits.Get(opposed) is null) throw new InvalidOperationException("Missing resistance trait reference.");
		ScrollSpellCompatibility.ValidateReferences(XElement.Parse(model.Definition), gameworld);
		var spell = new MagicSpell(model, gameworld) { EffectDurationExpression = _duration is null ? null! : new TraitExpression(_duration, gameworld), StoredSnapshot = this };
		spell.SetNoSave(true);
		var errors = ScrollSpellCompatibility.Errors(spell, false);
		if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
		ScrollSpellCompatibility.Bind(spell, Numbers, null);
		return spell;
	}
}
