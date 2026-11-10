#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>Permanent, detection-only affinity that can retain an item during a configured plane transition.</summary>
public sealed class EtherealItemAffinityEffect : Effect, IPlaneSpecificInventoryRetention, IIntrinsicItemMagicEffect
{
	public static void InitialiseEffectType() => RegisterFactory("EtherealItemAffinity",
		(root, owner) => new EtherealItemAffinityEffect(root, owner));

	public EtherealItemAffinityEffect(IGameItem owner, long planeId, bool readyForTransition, bool visibleToPlane) : base(owner)
	{
		if (planeId <= 0) throw new ArgumentOutOfRangeException(nameof(planeId));
		PlaneId = planeId;
		IsReadyForTransition = readyForTransition;
		IsVisibleToThatPlane = visibleToPlane;
	}

	private EtherealItemAffinityEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Definition") ?? throw new FormatException("Missing ethereal item affinity definition.");
		PlaneId = (long?)definition.Element("PlaneId") ?? 0;
		IsReadyForTransition = (bool?)definition.Element("ReadyForTransition") ?? false;
		IsVisibleToThatPlane = (bool?)definition.Element("VisibleToPlane") ?? false;
		if (PlaneId <= 0) throw new FormatException("Ethereal item affinity requires a plane ID.");
	}

	public long PlaneId { get; }
	public bool IsReadyForTransition { get; private set; }
	public bool IsVisibleToThatPlane { get; private set; }
	public bool ClearEtherealVisibility()
		=> SetEtherealVisibility(false);

	public bool SetEtherealVisibility(bool visible)
	{
		if (IsVisibleToThatPlane == visible) return false;
		var before = PlanarVisibilityEchoHelper.CaptureVisibleObservers(Owner);
		IsVisibleToThatPlane = visible;
		Changed = true;
		PlanarVisibilityEchoHelper.EchoVisibilityChanges(Owner, before);
		return true;
	}
	public int DispelPriority => 30;
	public int MinimumDispelGrade => 1;
	public bool IsIntrinsicMagicActive => IsReadyForTransition || IsVisibleToThatPlane;
	public override PerceptionTypes Obscuring => IsVisibleToThatPlane ? PerceptionTypes.AllVisual : PerceptionTypes.None;
	public override bool Applies(object target) => base.Applies(target) && IsVisibleToThatPlane &&
		(target is not IPerceiver voyeur || !RevealedTo(voyeur));
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) =>
		base.Applies(target, flags) && IsVisibleToThatPlane && (target is not IPerceiver voyeur || !RevealedTo(voyeur));
	private bool RevealedTo(IPerceiver voyeur) =>
		(voyeur.GetPerception(voyeur.NaturalPerceptionTypes) & (PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal)) != PerceptionTypes.None ||
		voyeur.GetPlanarPresence().PresencePlaneIds.Contains(PlaneId);
	public bool TryDispel(int grade)
	{
		if (grade < MinimumDispelGrade || !IsReadyForTransition && !IsVisibleToThatPlane) return false;
		IsReadyForTransition = false;
		Changed = true;
		SetEtherealVisibility(false);
		return true;
	}
	public override bool SavingEffect => true;
	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		if (!ReferenceEquals(effectOwnerItem, Owner)) return false;
		var other = targetItem.EffectsOfType<EtherealItemAffinityEffect>().ToArray();
		return !other.Any(x => x.PlaneId == PlaneId && x.IsReadyForTransition == IsReadyForTransition &&
			x.IsVisibleToThatPlane == IsVisibleToThatPlane) || other.Any(x => x.PlaneId != PlaneId ||
			x.IsReadyForTransition != IsReadyForTransition || x.IsVisibleToThatPlane != IsVisibleToThatPlane);
	}
	protected override string SpecificEffectType => "EtherealItemAffinity";
	protected override XElement SaveDefinition() => new("Definition",
		new XElement("PlaneId", PlaneId),
		new XElement("ReadyForTransition", IsReadyForTransition),
		new XElement("VisibleToPlane", IsVisibleToThatPlane));
	public override string Describe(IPerceiver voyeur) => IsReadyForTransition
		? "This item is ethereally attuned and may follow its bearer into the ethereal plane."
		: "This item bears a faint ethereal affinity.";
	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem) =>
		new EtherealItemAffinityEffect(newItem, PlaneId, IsReadyForTransition, IsVisibleToThatPlane);
}
