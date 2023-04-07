using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class WillToLiveMerit : CharacterMeritBase, IHypoxiaReducingMerit, IOrganFunctionBonusMerit
{
	protected WillToLiveMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		BrainBonus = double.Parse(definition.Attribute("brainbonus")?.Value ?? "0.0");
		HeartBonus = double.Parse(definition.Attribute("heartbonus")?.Value ?? "0.0");
		HypoxiaReductionFactor = double.Parse(definition.Attribute("hypoxia")?.Value ?? "1.0");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Will To Live",
			(merit, gameworld) => new WillToLiveMerit(merit, gameworld));
	}

	public double BrainBonus { get; set; }
	public double HeartBonus { get; set; }

	public IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body)
	{
		foreach (var organ in body.Organs)
		{
			if (organ is BrainProto)
			{
				if (organ.OrganFunctionFactor(body) > 0.0)
				{
					yield return (organ, BrainBonus);
				}

				continue;
			}

			if (organ is HeartProto)
			{
				if (organ.OrganFunctionFactor(body) > 0.0)
				{
					yield return (organ, HeartBonus);
				}

				continue;
			}
		}
	}

	public double HypoxiaReductionFactor { get; }
}