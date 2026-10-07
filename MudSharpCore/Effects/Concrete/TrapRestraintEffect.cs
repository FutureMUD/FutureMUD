#nullable enable

using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// A short-lived immobilisation applied by a trap. It is intentionally distinct from item restraints:
/// no wearable restraint item is fabricated, and escape is performed through the trap command's check.
/// </summary>
public sealed class TrapRestraintEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("TrapRestraint", (effect, owner) => new TrapRestraintEffect(effect, owner));
	}

	public TrapRestraintEffect(ICharacter owner, Guid trapInstanceId, string description, long creatorId = 0, long originRoomId = 0)
		: base(owner)
	{
		TrapInstanceId = trapInstanceId;
		DescriptionText = description;
		CreatorId = creatorId;
		OriginRoomId = originRoomId;
	}

	private TrapRestraintEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		TrapInstanceId = Guid.Parse(effect.Element("TrapInstanceId")!.Value);
		DescriptionText = effect.Element("Description")?.Value ?? "restrained by a trap";
		CreatorId = long.TryParse(effect.Element("CreatorId")?.Value, out var creator) ? creator : 0;
		OriginRoomId = long.TryParse(effect.Element("OriginCellId")?.Value, out var room) ? room : 0;
	}

	public Guid TrapInstanceId { get; }
	public long CreatorId { get; }
	public long OriginRoomId { get; }
	public string DescriptionText { get; }

	public override IEnumerable<string> Blocks => ["movement"];

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return DescriptionText;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return DescriptionText;
	}

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "TrapRestraint";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("TrapInstanceId", TrapInstanceId),
			new XElement("CreatorId", CreatorId),
			new XElement("OriginCellId", OriginRoomId),
			new XElement("Description", new XCData(DescriptionText)));
	}
}

