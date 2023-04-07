using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;
using MoveSpeed = MudSharp.Movement.MoveSpeed;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SpecificSpeedMovementMultiplierMerit : CharacterMeritBase, IMovementSpeedMerit
{
	protected SpecificSpeedMovementMultiplierMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
		MoveSpeedIds = (from element in definition.Element("Speeds")?.Elements() ?? Enumerable.Empty<XElement>()
		                select long.Parse(element.Attribute("id")?.Value ?? "0")).ToList();
	}

	public IEnumerable<long> MoveSpeedIds { get; set; }
	public double Multiplier { get; set; }

	public double SpeedMultiplier(IMoveSpeed speed)
	{
		return MoveSpeedIds.Contains(speed.Id) ? Multiplier : 1.0;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Speed Multiplier",
			(merit, gameworld) => new SpecificSpeedMovementMultiplierMerit(merit, gameworld));
	}
}