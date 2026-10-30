#nullable enable

using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public sealed class RoomPolicyEffect : Effect, IRoomPolicyEffect
{
	public RoomPolicyEffect(IRoom owner, RoomRestrictions restrictions, int maximumOccupants = 0, long occupancyCostProgId = 0) : base(owner)
	{
		Restrictions = restrictions;
		MaximumOccupants = Math.Max(0, maximumOccupants);
		OccupancyCostProgId = occupancyCostProgId;
	}

	public RoomPolicyEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect");
		Restrictions = (RoomRestrictions)((int?)definition?.Element("Restrictions") ?? 0);
		MaximumOccupants = Math.Max(0, (int?)definition?.Element("MaximumOccupants") ?? 0);
		OccupancyCostProgId = (long?)definition?.Element("OccupancyCostProg") ?? 0;
	}

	public RoomRestrictions Restrictions { get; }
	public int MaximumOccupants { get; }
	public long OccupancyCostProgId { get; }
	protected override string SpecificEffectType => "RoomPolicy";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) =>
		$"Room restrictions: {Restrictions.DescribeEnum().ColourName()}; maximum occupants: {(MaximumOccupants == 0 ? "unlimited" : MaximumOccupants.ToString("N0", voyeur)).ColourValue()}; occupancy cost prog: {OccupancyCostProgId.ToString("N0", voyeur).ColourValue()}.";
	protected override XElement SaveDefinition() => new("Effect",
		new XElement("Restrictions", (int)Restrictions), new XElement("MaximumOccupants", MaximumOccupants),
		new XElement("OccupancyCostProg", OccupancyCostProgId));
	public static void InitialiseEffectType() =>
		RegisterFactory("RoomPolicy", (effect, owner) => new RoomPolicyEffect(effect, owner));
}
