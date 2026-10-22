#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

/// <summary>A temporary loss of function in one limb, independent of wounds or magic.</summary>
public sealed class LimbNumbnessEffect : Effect, ILimbIneffectiveEffect
{
	public LimbNumbnessEffect(IBody owner, ILimb limb) : base(owner)
	{
		LimbId = limb.Id;
	}

	private LimbNumbnessEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		LimbId = long.Parse(root.Element("Effect")!.Element("Limb")!.Value, CultureInfo.InvariantCulture);
		_restored = true;
	}

	private readonly bool _restored;
	public long LimbId { get; }
	private IBody Body => (IBody)Owner;
	private ILimb? Limb => Body.Limbs.FirstOrDefault(x => x.Id == LimbId);
	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Numbness;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "LimbNumbness";
	public static void InitialiseEffectType() => RegisterFactory("LimbNumbness",
		(root, owner) => new LimbNumbnessEffect(root, owner));
	protected override XElement SaveDefinition() => new("Effect", new XElement("Limb", LimbId));
	public bool AppliesToLimb(ILimb limb) => limb.Id == LimbId && Body.Limbs.Contains(limb);
	public override bool Applies() => Limb is not null && base.Applies();
	public override string Describe(IPerceiver voyeur) =>
		$"Temporarily unable to feel or move the {Limb?.Name ?? "missing limb"}.";

	public override void InitialEffect()
	{
		// Loading runs before inventory and the character are fully restored.
		if (!_restored) ApplyConsequences();
	}

	public override void Login()
	{
		if (Limb is null)
		{
			Owner.RemoveEffect(this);
			return;
		}
		ApplyConsequences();
	}

	private void ApplyConsequences()
	{
		var limb = Limb;
		if (limb is null) return;
		Body.Actor.OutputHandler.Send($"Your {limb.Name.ToLowerInvariant()} goes numb and will not respond.");
		foreach (var item in limb.Parts.SelectMany(Body.HeldOrWieldedItemsFor)
			.Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToList())
		{
			Body.Actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ lose|loses &0's grip on $1 as &0's {limb.Name.ToLowerInvariant().Sanitise()} goes numb.",
				Body.Actor, Body.Actor, item), style: OutputStyle.NoNewLine));
			Body.Drop(item, silent: true);
		}
		if (limb.LimbType == LimbType.Leg) Body.CheckPositionStillValid();
	}

	public override void RemovalEffect()
	{
		if (Limb is { } limb)
			Body.Actor.OutputHandler.Send($"Feeling returns to your {limb.Name.ToLowerInvariant()}.");
	}
}
