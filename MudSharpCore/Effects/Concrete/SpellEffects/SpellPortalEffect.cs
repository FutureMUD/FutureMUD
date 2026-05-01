#nullable enable

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPortalEffect : MagicSpellEffectBase
{
	private IExit? _registeredExit;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPortal", (effect, owner) => new SpellPortalEffect(effect, owner));
	}

	public SpellPortalEffect(IPerceivable owner, IMagicSpellEffectParent parent, ICell source, ICell destination,
		string verb, string outboundKeyword, string inboundKeyword, string outboundTarget, string inboundTarget,
		string outboundDescription, string inboundDescription, double timeMultiplier, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		SourceCellId = source.Id;
		DestinationCellId = destination.Id;
		Verb = verb;
		OutboundKeyword = outboundKeyword;
		InboundKeyword = inboundKeyword;
		OutboundTarget = outboundTarget;
		InboundTarget = inboundTarget;
		OutboundDescription = outboundDescription;
		InboundDescription = inboundDescription;
		TimeMultiplier = timeMultiplier;
	}

	private SpellPortalEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		SourceCellId = long.Parse(trueRoot?.Element("SourceCell")?.Value ?? "0");
		DestinationCellId = long.Parse(trueRoot?.Element("DestinationCell")?.Value ?? "0");
		Verb = trueRoot?.Element("Verb")?.Value ?? "enter";
		OutboundKeyword = trueRoot?.Element("OutboundKeyword")?.Value ?? "portal";
		InboundKeyword = trueRoot?.Element("InboundKeyword")?.Value ?? "portal";
		OutboundTarget = trueRoot?.Element("OutboundTarget")?.Value ?? "a shimmering portal";
		InboundTarget = trueRoot?.Element("InboundTarget")?.Value ?? "a shimmering portal";
		OutboundDescription = trueRoot?.Element("OutboundDescription")?.Value ?? "through";
		InboundDescription = trueRoot?.Element("InboundDescription")?.Value ?? "through";
		TimeMultiplier = double.Parse(trueRoot?.Element("TimeMultiplier")?.Value ?? "1.0");
	}

	public long SourceCellId { get; }
	public long DestinationCellId { get; }
	public string Verb { get; }
	public string OutboundKeyword { get; }
	public string InboundKeyword { get; }
	public string OutboundTarget { get; }
	public string InboundTarget { get; }
	public string OutboundDescription { get; }
	public string InboundDescription { get; }
	public double TimeMultiplier { get; }

	private ICell? SourceCell => Gameworld.Cells.Get(SourceCellId);
	private ICell? DestinationCell => Gameworld.Cells.Get(DestinationCellId);

	public override void InitialEffect()
	{
		base.InitialEffect();
		RegisterPortal();
	}

	public override void Login()
	{
		base.Login();
		RegisterPortal();
	}

	private void RegisterPortal()
	{
		if (_registeredExit is not null)
		{
			return;
		}

		var source = SourceCell;
		var destination = DestinationCell;
		if (source is null || destination is null)
		{
			return;
		}

		_registeredExit = new TransientExit(Gameworld, source, destination, Verb, OutboundKeyword, InboundKeyword,
			OutboundTarget, InboundTarget, OutboundDescription, InboundDescription, TimeMultiplier);
		Gameworld.ExitManager.RegisterTransientExit(_registeredExit);
	}

	public override void RemovalEffect()
	{
		if (_registeredExit is not null)
		{
			Gameworld.ExitManager.UnregisterTransientExit(_registeredExit);
			_registeredExit = null;
		}

		base.RemovalEffect();
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("SourceCell", SourceCellId),
			new XElement("DestinationCell", DestinationCellId),
			new XElement("Verb", new XCData(Verb)),
			new XElement("OutboundKeyword", new XCData(OutboundKeyword)),
			new XElement("InboundKeyword", new XCData(InboundKeyword)),
			new XElement("OutboundTarget", new XCData(OutboundTarget)),
			new XElement("InboundTarget", new XCData(InboundTarget)),
			new XElement("OutboundDescription", new XCData(OutboundDescription)),
			new XElement("InboundDescription", new XCData(InboundDescription)),
			new XElement("TimeMultiplier", TimeMultiplier)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Maintaining a magical portal from room #{SourceCellId:N0} to room #{DestinationCellId:N0}.";
	}

	protected override string SpecificEffectType => "SpellPortal";
}
