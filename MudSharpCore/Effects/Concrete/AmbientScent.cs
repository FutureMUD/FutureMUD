using MudSharp.Construction;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Effects.Concrete;

public class AmbientScent : Effect, IScentTrailEffect
{
	public AmbientScent(IPerceivable owner, long sourceItemId, string sourceDescription, string description,
		RoomLayer roomLayer, int distance, Difficulty scentDifficulty, ANSIColour? colour = null)
		: base(owner)
	{
		SourceItemId = sourceItemId;
		SourceDescription = sourceDescription;
		AdditionalText = description;
		RoomLayer = roomLayer;
		Distance = distance;
		BaseScentDifficulty = scentDifficulty;
		Colour = colour ?? Telnet.Yellow;
	}

	public AmbientScent(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var definition = effect.Element("Effect")!;
		SourceItemId = long.Parse(definition.Element("SourceItemId")?.Value ?? "0");
		SourceDescription = definition.Element("SourceDescription")?.Value ?? "something";
		AdditionalText = definition.Element("AdditionalText")?.Value ?? string.Empty;
		RoomLayer = (RoomLayer)int.Parse(definition.Element("RoomLayer")?.Value ?? ((int)RoomLayer.GroundLevel).ToString());
		Distance = int.Parse(definition.Element("Distance")?.Value ?? "0");
		BaseScentDifficulty = (Difficulty)int.Parse(definition.Element("ScentDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		Colour = Telnet.GetColour(definition.Element("Colour")?.Value) ?? Telnet.Yellow;
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("AmbientScent", (effect, owner) => new AmbientScent(effect, owner));
	}

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "AmbientScent";
	public long SourceItemId { get; private set; }
	public string SourceDescription { get; private set; }
	public string AdditionalText { get; private set; }
	public bool PlayerSet => false;
	public RoomLayer RoomLayer { get; private set; }
	public int Distance { get; private set; }
	public Difficulty BaseScentDifficulty { get; private set; }
	public ANSIColour Colour { get; private set; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("SourceItemId", SourceItemId),
			new XElement("SourceDescription", new XCData(SourceDescription)),
			new XElement("AdditionalText", new XCData(AdditionalText)),
			new XElement("RoomLayer", (int)RoomLayer),
			new XElement("Distance", Distance),
			new XElement("ScentDifficulty", (int)BaseScentDifficulty),
			new XElement("Colour", Colour.Name));
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) &&
		       (target is not IPerceiver perceiver || perceiver.RoomLayer == RoomLayer);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Ambient scent from {SourceDescription} at distance {Distance.ToString("N0", voyeur)}: {AdditionalText}";
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour ? AdditionalText.Colour(Colour) : AdditionalText;
	}

	public bool Matches(string sourceDescription, string additionalText, RoomLayer roomLayer, int distance,
		Difficulty scentDifficulty)
	{
		return SourceDescription == sourceDescription &&
		       AdditionalText == additionalText &&
		       RoomLayer == roomLayer &&
		       Distance == distance &&
		       BaseScentDifficulty == scentDifficulty;
	}

	public Difficulty ScentDifficulty(ICharacter actor)
	{
		return BaseScentDifficulty;
	}

	public string DescribeForTracksCommand(ICharacter actor)
	{
		var distanceText = Distance switch
		{
			0 => "here",
			1 => "nearby",
			_ => $"{Distance.ToString("N0", actor)} rooms away"
		};

		return
			$"{AdditionalText} The scent appears to come from {SourceDescription.ColourName()} {distanceText} and is {ScentDifficulty(actor).DescribeColoured()} to smell.";
	}

}
