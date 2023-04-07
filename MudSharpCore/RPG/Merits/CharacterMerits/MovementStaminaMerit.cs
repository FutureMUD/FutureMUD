using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MovementStaminaMerit : CharacterMeritBase, IMovementStaminaMerit
{
	protected MovementStaminaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
	}

	public double Multiplier { get; set; }

	public double StaminaMultiplier(IMoveSpeed speed)
	{
		return Multiplier;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Movement Stamina Multiplier",
			(merit, gameworld) => new MovementStaminaMerit(merit, gameworld));
	}
}