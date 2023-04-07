using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class BoneHealthMerit : CharacterMeritBase, IBodypartHealthMerit
{
	public double Modifier { get; protected set; }

	protected BoneHealthMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		Modifier = double.Parse(root.Element("Modifier")?.Value ??
		                        throw new ApplicationException($"BoneHealthMerit {Id} was missing a Modifier element"));
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("BoneHealth",
			(merit, gameworld) => new BoneHealthMerit(merit, gameworld));
	}

	public bool AppliesToBodypart(IBodypart bodypart)
	{
		return bodypart is IBone;
	}

	public double MultiplierForBodypart(IBodypart bodypart)
	{
		return Modifier;
	}
}