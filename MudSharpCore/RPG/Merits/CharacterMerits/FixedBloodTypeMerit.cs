using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class FixedBloodTypeMerit : CharacterMeritBase, IFixedBloodTypeMerit
{
	protected FixedBloodTypeMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		Bloodtype = gameworld.Bloodtypes.Get(long.Parse(XElement.Parse(merit.Definition).Attribute("bloodtype").Value));
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("FixedBloodtype",
			(merit, gameworld) => new FixedBloodTypeMerit(merit, gameworld));
	}

	public IBloodtype Bloodtype { get; }
}