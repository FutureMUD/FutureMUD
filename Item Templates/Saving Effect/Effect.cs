#nullable enable
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace $rootnamespace$.Concrete;

public class $safeitemrootname$ : Effect, IEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("$safeitemrootname$", (effect, owner) => new $safeitemrootname$(effect, owner));
	}

	public $safeitemrootname$(IPerceivable owner, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
	}

	protected $safeitemrootname$(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Effect");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0)
		);
	}

	protected override string SpecificEffectType => "$safeitemrootname$";

	public override string Describe(IPerceiver voyeur)
	{
		return "An undescribed saving effect.";
	}

	public override bool SavingEffect => true;
}
