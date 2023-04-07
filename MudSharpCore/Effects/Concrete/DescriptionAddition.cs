using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class DescriptionAddition : Effect, IDescriptionAdditionEffect
{
	public DescriptionAddition(IPerceivable owner, string description, bool playerSet, ANSIColour colour,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		AdditionalText = description;
		PlayerSet = playerSet;
		Colour = colour;
	}

	public DescriptionAddition(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var definition = effect.Element("Effect");
		AdditionalText = definition.Element("AdditionalText")?.Value ?? "";
		PlayerSet = bool.Parse(definition.Element("PlayerSet")?.Value ?? "true");
		Colour = Telnet.GetColour(definition.Element("Colour")?.Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("DescriptionAddition", (effect, owner) => new DescriptionAddition(effect, owner));
	}

	#region Overrides of Effect

	public override bool SavingEffect { get; } = true;

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("AdditionalText", new XCData(AdditionalText)),
			new XElement("PlayerSet", PlayerSet),
			new XElement("Colour", Colour?.Name ?? "None"));
	}

	#endregion

	#endregion

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"DescriptionAddition {(PlayerSet ? "PlayerSet" : "NonPlayer")} - {AdditionalText}";
	}

	protected override string SpecificEffectType { get; } = "DescriptionAddition";

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new DescriptionAddition(newItem, AdditionalText, PlayerSet, Colour, ApplicabilityProg);
		}

		return null;
	}

	#endregion

	#region Implementation of IDescriptionAdditionEffect

	public string AdditionalText { get; set; }
	public bool PlayerSet { get; set; }
	public ANSIColour Colour { get; set; }

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour && Colour != null ? AdditionalText.Colour(Colour) : AdditionalText;
	}

	#endregion
}