using System;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SecondWindMerit : CharacterMeritBase, ISecondWindMerit
{
	public SecondWindMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		Emote = root.Element("Emote").Value;
		RecoveryMessage = root.Element("RecoveryMessage").Value;
		RecoveryDuration = TimeSpan.FromSeconds(double.Parse(root.Element("RecoveryDuration").Value));
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("SecondWind",
			(merit, gameworld) => new SecondWindMerit(merit, gameworld));
	}

	public string Emote { get; set; }

	public TimeSpan RecoveryDuration { get; set; }

	public string RecoveryMessage { get; set; }
}