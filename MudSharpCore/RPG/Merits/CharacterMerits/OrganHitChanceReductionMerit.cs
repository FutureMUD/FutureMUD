using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;
using ExpressionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class OrganHitChanceReductionMerit : CharacterMeritBase, IOrganHitReductionMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Organ Hit Reduction",
			(merit, gameworld) => new OrganHitChanceReductionMerit(merit, gameworld));
	}

	protected OrganHitChanceReductionMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		MinimumWoundSeverity =
			(WoundSeverity)int.Parse(root.Attribute("minseverity")?.Value ?? ((int)WoundSeverity.None).ToString());
		MaximumWoundSeverity =
			(WoundSeverity)int.Parse(root.Attribute("maxseverity")?.Value ??
			                         ((int)WoundSeverity.Horrifying).ToString());
		HitChanceExpression = new Expression(root.Element("Chance")?.Value ?? "0.0");
	}

	public WoundSeverity MinimumWoundSeverity { get; set; }
	public WoundSeverity MaximumWoundSeverity { get; set; }
	public Expression HitChanceExpression { get; set; }

	public bool MissesOrgan(KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo, IDamage damage,
		WoundSeverity severity)
	{
		if (severity < MinimumWoundSeverity)
		{
			return false;
		}

		if (severity > MaximumWoundSeverity)
		{
			return false;
		}

		HitChanceExpression.Parameters["chance"] = organInfo.Value.HitChance;
		if (RandomUtilities.Random(0, 100) <= (double)HitChanceExpression.Evaluate())
		{
			return true;
		}

		return false;
	}
}