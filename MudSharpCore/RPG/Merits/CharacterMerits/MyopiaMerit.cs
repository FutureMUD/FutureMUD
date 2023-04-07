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

public class MyopiaMerit : CharacterMeritBase, IMyopiaMerit
{
	public bool CorrectedByGlasses { get; protected set; }

	public MyopiaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		LoadFromXml(XElement.Parse(merit.Definition));
	}

	private void LoadFromXml(XElement root)
	{
		CorrectedByGlasses = bool.Parse(root.Attribute("glasses")?.Value ?? "false");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Myopia",
			(merit, gameworld) => new MyopiaMerit(merit, gameworld));
	}
}