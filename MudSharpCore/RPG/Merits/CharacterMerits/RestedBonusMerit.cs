using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class RestedBonusMerit : CharacterMeritBase, IRestedBonusMerit
{
	protected RestedBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var element = definition.Element("Multiplier");
		Multiplier = double.Parse(element?.Value ?? "1.0");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("RestedBonus",
			(merit, gameworld) => new RestedBonusMerit(merit, gameworld));
	}

	#region Implementation of IRestedBonusMerit

	public double Multiplier { get; protected set; }

	#endregion
}