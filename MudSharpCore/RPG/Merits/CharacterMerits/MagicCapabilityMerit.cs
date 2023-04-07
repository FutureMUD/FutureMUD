using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MagicCapabilityMerit : CharacterMeritBase, IMagicCapabilityMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Magic Capability",
			(merit, gameworld) => new MagicCapabilityMerit(merit, gameworld));
	}

	protected MagicCapabilityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		foreach (var item in root.Element("Capabilities").Elements())
		{
			var capability = gameworld.MagicCapabilities.Get(long.Parse(item.Value));
			if (capability != null)
			{
				_capabilities.Add(capability);
			}
		}
	}

	private readonly List<IMagicCapability> _capabilities = new();
	public IEnumerable<IMagicCapability> Capabilities => _capabilities;
}