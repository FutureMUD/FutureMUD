#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

/// <summary>A persistent world rune revealed and admitted by a configured native sense.</summary>
public sealed class PublicTravelAnchor : Effect, ILiveTravelAnchorEffect
{
	public PublicTravelAnchor(IGameItem owner, PerceptionTypes requiredPerception = PerceptionTypes.SenseMagical)
		: base(owner)
	{
		RequiredPerception = requiredPerception;
	}

	private PublicTravelAnchor(XElement root, IPerceivable owner) : base(root, owner)
	{
		RequiredPerception = (PerceptionTypes)((long?)root.Element("Effect")?.Element("RequiredPerception") ??
			(long)PerceptionTypes.SenseMagical);
	}

	public PerceptionTypes RequiredPerception { get; }
	public long ItemId => Owner.Id;
	public bool IsActive => Owner is IGameItem { Id: > 0, Deleted: false, Destroyed: false } item &&
		item.Effects.Any(x => ReferenceEquals(x, this)) && base.Applies();

	public bool IsRevealedTo(IPerceiver voyeur) =>
		(voyeur.GetPerception(voyeur.NaturalPerceptionTypes) & RequiredPerception) == RequiredPerception;

	public bool AllowsTravel(ICharacter actor) => IsActive && IsRevealedTo(actor) &&
		Owner is IGameItem { InInventoryOf: null, ContainedIn: null, Location: not null };

	// Detection reveals the rune to the ordinary native visibility pipeline. It does not grant
	// sight through closed containers, darkness or planar boundaries.
	public override PerceptionTypes Obscuring => PerceptionTypes.All;
	public override bool Applies(object target) => base.Applies(target) &&
		(target is not IPerceiver voyeur || !IsRevealedTo(voyeur));
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) =>
		base.Applies(target, flags) && (target is not IPerceiver voyeur || !IsRevealedTo(voyeur));

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PublicTravelAnchor";
	protected override XElement SaveDefinition() => new("Effect", new XElement("RequiredPerception", (long)RequiredPerception));
	public override string Describe(IPerceiver voyeur) =>
		$"A public travel rune requiring {RequiredPerception.DescribeEnum().ColourName()} to perceive and use.";
	public static void InitialiseEffectType() => RegisterFactory("PublicTravelAnchor", (root, owner) => new PublicTravelAnchor(root, owner));
}
