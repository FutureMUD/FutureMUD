#nullable enable

using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

public sealed class PsychometricHistoryEffect : Effect
{
	public PsychometricHistory History { get; } = new();
	public static void InitialiseEffectType() => RegisterFactory("PsychometricHistory", (xml, owner) => new PsychometricHistoryEffect(xml, owner));
	public PsychometricHistoryEffect(IPerceivable owner) : base(owner) => PsychometricRecorder.TrackPayload(this);
	private PsychometricHistoryEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		PsychometricRecorder.TrackPayload(this);
		var root = xml.Element("Effect")!;
		History.MixedProvenance = (bool?)root.Attribute("mixed") ?? false;
		CustodyPeriod ReadCustody(XElement x) => new((long)x.Attribute("carrier")!, (DateTime)x.Attribute("since")!,
			(DateTime?)x.Attribute("until"), (bool?)x.Attribute("unknown") ?? false);
		History.RestoreCustody((string?)root.Attribute("epoch") ?? "", root.Element("Current") is { } current ? ReadCustody(current) : null,
			root.Elements("Previous").Select(ReadCustody));
		foreach (var x in root.Elements("Impression"))
		{
			History.Record(new PsychometricImpression(Enum.Parse<ImpressionKind>((string)x.Attribute("kind")!),
				(long)x.Attribute("source")!, (long?)x.Attribute("target"), (DateTime)x.Attribute("created")!,
				(DateTime?)x.Attribute("expires"), x.Value, (int?)x.Attribute("layer") ?? 0,
				(string?)x.Attribute("position") ?? "", (long?)x.Attribute("school") ?? 0), owner is MudSharp.GameItems.IGameItem);
		}
	}
	protected override string SpecificEffectType => "PsychometricHistory";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => "Contains bounded psychometric history (read through psychometry).";
	protected override XElement SaveDefinition()
	{
		History.Prune(RuntimeClock.UtcNow);
		XElement WriteCustody(string name, CustodyPeriod p) => new(name, new XAttribute("carrier", p.CarrierId),
			new XAttribute("since", p.SinceUtc), p.UntilUtc is { } until ? new XAttribute("until", until) : null,
			new XAttribute("unknown", p.UnknownBeginning));
		return new XElement("Effect", new XAttribute("epoch", History.Epoch), new XAttribute("mixed", History.MixedProvenance),
			History.CurrentCarrier is { } current ? WriteCustody("Current", current) : null,
			History.PreviousCarriers.Select(x => WriteCustody("Previous", x)),
			History.Impressions.Select(x => new XElement("Impression", new XAttribute("kind", x.Kind),
				new XAttribute("source", x.SourceId), x.TargetId is { } target ? new XAttribute("target", target) : null,
				new XAttribute("created", x.CreatedUtc), x.ExpiresUtc is { } expires ? new XAttribute("expires", expires) : null,
				new XAttribute("layer", x.Layer), new XAttribute("position", x.Position), new XAttribute("school", x.SchoolId), new XCData(x.Text))));
	}
}
