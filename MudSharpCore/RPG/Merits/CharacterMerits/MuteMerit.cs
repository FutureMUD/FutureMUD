using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MuteMerit : CharacterMeritBase, IMuteMerit
{
	protected MuteMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		LanguageOptions = (PermitLanguageOptions)int.Parse(root.Element("PermitLanguageOption")?.Value ?? "2");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Mute",
			(merit, gameworld) => new MuteMerit(merit, gameworld));
	}

	public PermitLanguageOptions LanguageOptions { get; }
}