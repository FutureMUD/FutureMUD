using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SurgeryFinalisationMerit : CharacterMeritBase, ISurgeryFinalisationMerit
{
	protected SurgeryFinalisationMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		BonusDegrees = int.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
	}

	public int BonusDegrees { get; set; }

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Surgery Finalisation",
			(merit, gameworld) => new SurgeryFinalisationMerit(merit, gameworld));
	}
}