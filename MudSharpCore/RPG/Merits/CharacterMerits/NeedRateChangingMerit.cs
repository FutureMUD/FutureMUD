using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class NeedRateChangingMerit : CharacterMeritBase, INeedRateChangingMerit
{
	protected NeedRateChangingMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		HungerMultiplier = double.Parse(definition.Attribute("hunger")?.Value ?? "1.0");
		ThirstMultiplier = double.Parse(definition.Attribute("thirst")?.Value ?? "1.0");
		DrunkennessMultiplier = double.Parse(definition.Attribute("alcohol")?.Value ?? "1.0");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Needs Rate Change",
			(merit, gameworld) => new NeedRateChangingMerit(merit, gameworld));
	}

	#region Implementation of INeedRateChangingMerit

	public double HungerMultiplier { get; set; }
	public double ThirstMultiplier { get; set; }
	public double DrunkennessMultiplier { get; set; }

	#endregion
}