#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Audio;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>Visibility gated by a detection sense, without changing physical interaction.</summary>
public sealed class DetectionVisibility : Effect
{
	public DetectionVisibility(IPerceivable owner, PerceptionTypes senses, long visiblePlane = 0) : base(owner)
	{ Senses = senses; VisiblePlane = visiblePlane; }
	private DetectionVisibility(XElement xml, IPerceivable owner) : base(xml, owner)
	{ Senses = (PerceptionTypes)(long)xml.Element("Visibility")!.Attribute("senses")!; VisiblePlane = (long?)xml.Element("Visibility")!.Attribute("plane") ?? 0; }
	public PerceptionTypes Senses { get; }
	public long VisiblePlane { get; }
	public bool RevealedTo(IPerceiver voyeur) =>
		(voyeur.GetPerception(voyeur.NaturalPerceptionTypes) & Senses) != 0 ||
		VisiblePlane > 0 && voyeur.GetPlanarPresence().PresencePlaneIds.Contains(VisiblePlane);
	public override PerceptionTypes Obscuring => PerceptionTypes.AllVisual;
	public override bool Applies(object target) => base.Applies(target) && (target is not IPerceiver voyeur || !RevealedTo(voyeur));
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) =>
		base.Applies(target, flags) && (target is not IPerceiver voyeur || !RevealedTo(voyeur));
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "DetectionVisibility";
	protected override XElement SaveDefinition() => new("Visibility", new XAttribute("senses", (long)Senses), new XAttribute("plane", VisiblePlane));
	public override string Describe(IPerceiver voyeur) => $"Visibility requires {Senses.DescribeEnum()} detection or presence on plane #{VisiblePlane}.";
	public static void InitialiseEffectType() => RegisterFactory("DetectionVisibility", (xml, owner) => new DetectionVisibility(xml, owner));
}

/// <summary>A descriptive glow or local audible hum; neither emits light or propagates between rooms.</summary>
public sealed class ItemPresentation : Effect, ISDescAdditionEffect, IDescriptionAdditionEffect
{
	public ItemPresentation(IGameItem owner, bool humming) : base(owner) => Humming = humming;
	private ItemPresentation(XElement xml, IPerceivable owner) : base(xml, owner) => Humming = (bool)xml.Element("Humming")!;
	public bool Humming { get; }
	public static string DescribeCarriedHum(ICharacter source, IPerceiver voyeur)
	{
		if (voyeur is not ICharacter listener || source.Location is null || source.Location != listener.Location) return "";
		var humming = source.Body.DirectItems.Where(x => x.Effects.OfType<ItemPresentation>().Any(y => y.Humming && y.Applies())).ToArray();
		if (humming.Length == 0) return "";
		var exposed = source.Body.ExternalItemsForOtherActors.ToHashSet();
		var score = humming.Sum(x => source.Body.WornItems.Contains(x) && !exposed.Contains(x) ? 1 : 2);
		var level = Math.Clamp(score / 5, 0, 4);
		if (level == 0 || !AudioPerception.CanHear(listener, humming[0], (AudioVolume)level, Proximity.Distant)) return "";
		var volume = new[] { "", "faint", "moderate", "loud", "blaring" }[level];
		return $"A {volume} humming sound comes from {source.HowSeen(voyeur)}.";
	}
	public string AddendumText => Humming ? "(humming)" : "(glowing)";
	public string GetAddendumText(bool colour) => colour ? AddendumText.Colour(Telnet.BoldMagenta) : AddendumText;
	public bool DescriptionAdditionApplies(IPerceiver voyeur) => !Humming ||
		voyeur is ICharacter listener && Owner is IGameItem item && item.Location is not null && item.Location == listener.Location &&
		AudioPerception.CanHear(listener, item, AudioVolume.Faint, Proximity.Distant);
	public string GetAdditionalText(IPerceiver voyeur, bool colour) =>
		Humming ? "It emits a humming sound." : "It glows softly.";
	public bool PlayerSet => false;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "ItemPresentation";
	protected override XElement SaveDefinition() => new("Humming", Humming);
	public override string Describe(IPerceiver voyeur) => Humming ? "Emits a local audible hum." : "A descriptive glow without illumination.";
	public static void InitialiseEffectType() => RegisterFactory("ItemPresentation", (xml, owner) => new ItemPresentation(xml, owner));
}

/// <summary>Spell-independent magic classification for objects with no sustained spell owner.</summary>
public sealed class ItemMagicAura : Effect, IMagicEffect
{
	public ItemMagicAura(IGameItem owner) : base(owner) { }
	private ItemMagicAura(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public IMagicSchool School => null!;
	public IMagicPower PowerOrigin => null!;
	public Difficulty DetectMagicDifficulty => Difficulty.Normal;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "ItemMagicAura";
	public override string Describe(IPerceiver voyeur) => "An intrinsic magical aura.";
	public static void InitialiseEffectType() => RegisterFactory("ItemMagicAura", (xml, owner) => new ItemMagicAura(xml, owner));
}
