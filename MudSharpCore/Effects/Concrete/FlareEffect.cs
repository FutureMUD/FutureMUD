using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class FlareEffect : Effect, IEffect, IAreaLightEffect, IDescriptionAdditionEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("FlareEffect", (effect, owner) => new FlareEffect(effect, owner));
	}

	#endregion

	#region Constructors

	public FlareEffect(ILocation owner, double addedLight, string flareDescription, ANSIColour descriptionColour,
		string flareEndEmote, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		AddedLight = addedLight;
		FlareDescription = flareDescription;
		FlareColour = descriptionColour;
		FlareEndEmote = flareEndEmote;
	}

	protected FlareEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXML(effect);
	}

	#endregion

	public double AddedLight { get; protected set; }
	public string FlareDescription { get; protected set; }
	public ANSIColour FlareColour { get; protected set; }
	public string FlareEndEmote { get; protected set; }

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Flare",
			new XElement("AddedLight", AddedLight),
			new XElement("FlareColour", FlareColour.Name),
			new XElement("FlareDescription", new XCData(FlareDescription)),
			new XElement("FlareEndEmote", new XCData(FlareEndEmote))
		);
	}

	protected void LoadFromXML(XElement root)
	{
		AddedLight = double.Parse(root.Element("Flare").Element("AddedLight").Value);
		FlareDescription = root.Element("Flare").Element("FlareDescription").Value;
		FlareColour = Telnet.GetColour(root.Element("Flare").Element("FlareColour").Value);
		FlareEndEmote = root.Element("Flare").Element("FlareEndEmote").Value;
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "FlareEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Adding {AddedLight.ToString("N3", voyeur).ColourValue()} lumens of illumination.";
	}

	public override bool Applies(object target)
	{
		if (target is not IPerceiver voyeur)
		{
			return true;
		}

		return voyeur.Location.GetOverlayFor(voyeur).OutdoorsType
		             .In(CellOutdoorsType.Outdoors, CellOutdoorsType.IndoorsWithWindows);
	}

	public override bool SavingEffect => true;

	public override void RemovalEffect()
	{
		if (Owner is IZone zone)
		{
			var emote = new EmoteOutput(new Emote(FlareEndEmote, new DummyPerceiver()));
			foreach (var cell in zone.Cells.Where(x => x.CurrentOverlay.OutdoorsType == CellOutdoorsType.Outdoors))
			{
				cell.Handle(emote);
			}

			zone.RecalculateLightLevel();
		}
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour ? FlareDescription.Colour(FlareColour) : FlareDescription;
	}

	public bool PlayerSet => false;

	#endregion
}