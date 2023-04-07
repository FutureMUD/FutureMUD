using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Heritage;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ExitSizeMerit : CharacterMeritBase, IContextualSizeMerit
{
	public int SizeOffset { get; set; }

	protected ExitSizeMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SizeOffset = int.Parse(definition.Element("SizeOffset")?.Value ?? "0");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("ExitSize",
			(merit, gameworld) => new ExitSizeMerit(merit, gameworld));
	}

	public SizeCategory ContextualSize(SizeCategory original, SizeContext context)
	{
		if (context != SizeContext.CellExit)
		{
			return original;
		}

		return (SizeCategory)Math.Min((int)SizeCategory.Titanic, Math.Max(0, (int)original + SizeOffset));
	}
}