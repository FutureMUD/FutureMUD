using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;
public class LandmarkEffect : Effect
{
	public LandmarkEffect(ICell owner, bool isMeetingPlace, string sphere, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Sphere = sphere;
		IsMeetingPlace = IsMeetingPlace;
	}

	private LandmarkEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Sphere = root.Element("Sphere").Value;
		IsMeetingPlace = bool.Parse(root.Element("IsMeetingPlace").Value);
		foreach (var item in root.Element("ExtraTexts")?.Elements("ExtraText") ?? Enumerable.Empty<XElement>())
		{
			LandmarkDescriptionTexts.Add((Gameworld.FutureProgs.Get(long.Parse(item.Attribute("prog").Value)), item.Value));
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"This location is considered a {(IsMeetingPlace ? "meeting place " : "")}landmark{(string.IsNullOrEmpty(Sphere) ? "" : $" for the {Sphere.TitleCase().ColourName()} sphere")}.";
	}

	protected override string SpecificEffectType { get; } = "Landmark";

	public override bool SavingEffect { get; } = true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("Landmark", (effect, owner) => new LandmarkEffect(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Sphere", new XCData(Sphere)),
			new XElement("IsMeetingPlace", IsMeetingPlace),
			new XElement("ExtraTexts",
				from item in LandmarkDescriptionTexts
				select new XElement("ExtraText",
					new XAttribute("prog", item.Prog.Id),
					new XCData(item.Text)
				)
			)
		);
	}

	public string Sphere { get; set; }

	public bool IsMeetingPlace { get; set; }

	public List<(IFutureProg Prog, string Text)> LandmarkDescriptionTexts { get; } = new();
}
