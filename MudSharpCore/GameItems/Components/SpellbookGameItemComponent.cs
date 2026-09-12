using MudSharp.GameItems.Prototypes;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class SpellbookGameItemComponent : GameItemComponent, ISpellbook
{
	private SpellbookGameItemComponentProto _prototype;
	private readonly List<SpellbookFormula> _formulae = [];
	private string? _invalidXml;
	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyList<SpellbookFormula> Formulae => _formulae.AsReadOnly();
	public int FormulaCapacity => _prototype.Capacity;
	public string? DataError { get; private set; }
	public SpellbookGameItemComponent(SpellbookGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) { _prototype = proto; }
	public SpellbookGameItemComponent(Models.GameItemComponent model, SpellbookGameItemComponentProto proto, IGameItem parent) : base(model, parent)
	{
		_prototype = proto;
		try
		{
			if (model.Definition.Length > 8_000_000) throw new FormatException("Oversized spellbook payload.");
			var root = XElement.Parse(model.Definition);
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported spellbook schema.");
			foreach (var entry in root.Elements("Formula")) _formulae.Add(new(Guid.Parse((string)entry.Attribute("id")!), (long)entry.Attribute("spell")!,
				DateTime.Parse((string)entry.Attribute("time")!, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), (long?)entry.Attribute("source")));
			if (_formulae.Any(x => x.SpellId <= 0 || x.EntryId == Guid.Empty) || _formulae.GroupBy(x => x.SpellId).Any(x => x.Count() > 1) || _formulae.GroupBy(x => x.EntryId).Any(x => x.Count() > 1)) throw new FormatException("Invalid or duplicate formula entries.");
		}
		catch (Exception ex) { DataError = ex.Message; _invalidXml = model.Definition; }
	}
	private SpellbookGameItemComponent(SpellbookGameItemComponent source, IGameItem parent, bool temporary) : base(source, parent, temporary)
	{ _prototype = source._prototype; _formulae.AddRange(source._formulae); DataError = source.DataError; _invalidXml = source._invalidXml; }
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new SpellbookGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (SpellbookGameItemComponentProto)newProto;
	protected override string SaveToXml() => _invalidXml ?? new XElement("Spellbook", new XAttribute("version", 1), _formulae.Select(x =>
		new XElement("Formula", new XAttribute("id", x.EntryId), new XAttribute("spell", x.SpellId), new XAttribute("time", x.CopiedUtc.ToString("O")), x.SourceItemId is { } source ? new XAttribute("source", source) : null))).ToString();
	internal bool AddFormula(long spell, DateTime now, long? source)
	{
		if (DataError is not null || _formulae.Count >= FormulaCapacity || _formulae.Any(x => x.SpellId == spell)) return false;
		_formulae.Add(new(Guid.NewGuid(), spell, now, source)); Changed = true; return true;
	}
	internal bool RemoveFormula(long spell) { var removed = _formulae.RemoveAll(x => x.SpellId == spell) > 0; Changed |= removed; return removed; }
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags) =>
		$"{description}\n\nThis is a structured spellbook. Use spellbook show <book> to inspect its formulae.";
}
