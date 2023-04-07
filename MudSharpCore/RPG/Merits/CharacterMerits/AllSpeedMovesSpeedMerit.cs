using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class AllSpeedMovesSpeedMerit : CharacterMeritBase, IMovementSpeedMerit
{
	protected AllSpeedMovesSpeedMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
	}

	public double Multiplier { get; set; }

	public double SpeedMultiplier(IMoveSpeed speed)
	{
		return Multiplier;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Speed Multiplier",
			(merit, gameworld) => new AllSpeedMovesSpeedMerit(merit, gameworld));
	}
}