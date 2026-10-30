using System;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Effects.Concrete;

/// <summary>A saved, independently scheduled health-capacity consequence; ordinary effect timers own expiry.</summary>
public sealed class TimedMaximumHitPointsEffect : Effect, ICapacityOnlyHitPointsPenalty, IDrugAftereffect
{
	public TimedMaximumHitPointsEffect(IBody owner, double bonus, long sourceDrugId = 0, string? sourceFamily = null, bool capacityOnly = false) : base(owner)
	{
		if (!double.IsFinite(bonus)) throw new ArgumentOutOfRangeException(nameof(bonus));
		MaximumHitPointsBonus = bonus;
		SourceDrugId = sourceDrugId;
		SourceFamily = sourceFamily;
		CapacityOnly = capacityOnly;
	}
	private TimedMaximumHitPointsEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Effect")!;
		MaximumHitPointsBonus = (double?)data.Element("Bonus") ?? 0.0;
		if (!double.IsFinite(MaximumHitPointsBonus)) MaximumHitPointsBonus = 0.0;
		SourceDrugId = (long?)data.Element("Drug") ?? 0;
		SourceFamily = (string?)data.Element("Family");
		CapacityOnly = (bool?)data.Element("CapacityOnly") ?? false;
	}
	public static void InitialiseEffectType() => RegisterFactory("TimedMaximumHitPoints", (root, owner) => new TimedMaximumHitPointsEffect(root, owner));
	protected override string SpecificEffectType => "TimedMaximumHitPoints";
	public override bool SavingEffect => true;
	public double MaximumHitPointsBonus { get; }
	public double MaximumHitPointsMultiplier => 1.0;
	public long SourceDrugId { get; }
	public string? SourceFamily { get; }
	public bool CapacityOnly { get; }
	public override string Describe(IPerceiver voyeur) => $"Temporary maximum HP modifier: {MaximumHitPointsBonus.ToString("N2", voyeur)}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Bonus", MaximumHitPointsBonus),
		new XElement("Drug", SourceDrugId), new XElement("CapacityOnly", CapacityOnly), SourceFamily is null ? null : new XElement("Family", SourceFamily));
}
