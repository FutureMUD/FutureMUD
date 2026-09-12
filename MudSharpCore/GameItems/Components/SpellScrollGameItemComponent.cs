using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.GameItems.Components;

public sealed class SpellScrollGameItemComponent : GameItemComponent, ISpellScroll
{
	private SpellScrollGameItemComponentProto _prototype;
	private string? _invalidXml;
	public override IGameItemComponentProto Prototype => _prototype;
	public StoredSpellSnapshot? Snapshot { get; private set; }
	public Guid? Reservation { get; private set; }
	public bool Spent { get; private set; }
	public string? DataError { get; private set; }
	public bool IsCharged => DataError is null && !Spent && Snapshot is not null;
	public bool IsBlank => DataError is null && !Spent && Snapshot is null;
	public long? SpellId => Snapshot?.SpellId;
	public int? CastingLevel => Snapshot?.CastingLevel;
	public SpellPower? StoredPower => Snapshot?.Power;
	public Guid? ChargeId => Snapshot?.ChargeId;
	public SpellScrollGameItemComponent(SpellScrollGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) { _prototype = proto; }
	public SpellScrollGameItemComponent(Models.GameItemComponent model, SpellScrollGameItemComponentProto proto, IGameItem parent) : base(model, parent)
	{
		_prototype = proto;
		try
		{
			if (model.Definition.Length > 8_000_000) throw new FormatException("Oversized spell scroll payload.");
			var root = XElement.Parse(model.Definition);
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported spell scroll schema.");
			Spent = (bool?)root.Attribute("spent") ?? false;
			Reservation = (string?)root.Attribute("reservation") is { } reservation ? Guid.Parse(reservation) : null;
			Snapshot = root.Element("StoredSpell") is { } spell ? StoredSpellSnapshot.Load(spell) : null;
		}
		catch (Exception ex) { DataError = ex.Message; _invalidXml = model.Definition; }
	}
	// Normal item copying cannot mint a paid charge or duplicate a reservation.
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new SpellScrollGameItemComponent(_prototype, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (SpellScrollGameItemComponentProto)newProto;
	protected override string SaveToXml() => _invalidXml ?? new XElement("SpellScroll", new XAttribute("version", 1), new XAttribute("spent", Spent),
		Reservation is { } token ? new XAttribute("reservation", token) : null, Snapshot?.Save()).ToString();
	internal bool Reserve(Guid token)
	{
		if (Reservation is not null || DataError is not null || Spent) return false;
		Reservation = token; Changed = true; return true;
	}
	internal void CancelReservation(Guid token) { if (Reservation != token) return; Reservation = null; Changed = true; }
	internal void Charge(Guid token, StoredSpellSnapshot snapshot)
	{
		if (!IsBlank || Reservation != token) throw new InvalidOperationException("This blank scroll is no longer reserved by this inscription.");
		Snapshot = snapshot; Reservation = null; Changed = true;
	}
	internal void Consume(Guid? token = null)
	{
		if (!IsCharged || (Reservation is not null && Reservation != token)) throw new InvalidOperationException("This scroll cannot be consumed by this operation.");
		Spent = true; Reservation = null; Changed = true;
	}
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags) =>
		$"{description}\n\nThis is a {(IsBlank ? "blank" : Spent ? "spent" : "charged")} spell scroll. Use spellscroll show <scroll> for details.";
}
