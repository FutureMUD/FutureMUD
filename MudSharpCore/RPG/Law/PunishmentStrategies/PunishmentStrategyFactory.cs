using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;
#nullable enable
public static class PunishmentStrategyFactory
{
	public static IEnumerable<string> ValidTypes => new[]
	{
		"fine",
		"bond",
		"hierarchy",
		"jail",
		"execute",
		"multi"
	};

	public static IPunishmentStrategy LoadStrategy(IFuturemud gameworld, string definition, ILegalAuthority authority)
	{
		var root = XElement.Parse(definition);
		switch (root!.Attribute("type")!.Value)
		{
			case "fine":
				return new PunishmentStrategyFine(gameworld, root, authority);
			case "bond":
				return new PunishmentStrategyGoodBehaviourBond(gameworld, root);
			case "jail":
				return new PunishmentStrategyJail(gameworld, root);
			case "hierarchy":
				return new PunishmentStrategyHierarchy(gameworld, root, authority);
			case "execute":
				return new PunishmentStrategyExecute(gameworld, root);
			case "multi":
				return new PunishmentStrategyMultiple(gameworld, root, authority);
			default:
				throw new NotImplementedException(
					$"Unknown punishment strategy type {root.Attribute("type")!.Value} in PunishmentStrategyFactory.");
		}
	}

	public static IPunishmentStrategy? GetStrategyFromBuilderInput(IFuturemud gameworld, ILegalAuthority authority,
		string input)
	{
		switch (input.ToLowerInvariant())
		{
			case "fine":
				return new PunishmentStrategyFine(gameworld, authority);
			case "bond":
				return new PunishmentStrategyGoodBehaviourBond(gameworld);
			case "hierarchy":
				return new PunishmentStrategyHierarchy(gameworld);
			case "jail":
			case "goal":
			case "prison":
				return new PunishmentStrategyJail(gameworld);
			case "execute":
				return new PunishmentStrategyExecute(gameworld);
			case "multi":
				return new PunishmentStrategyMultiple(gameworld);
		}

		return null;
	}
}