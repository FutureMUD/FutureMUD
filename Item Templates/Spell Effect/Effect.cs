using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;
	public class $safefilerootname$ : MagicSpellEffectBase
	{
	public static void InitialiseEffectType() {
		RegisterFactory("$safeitemrootname$SpellEffect", (effect, owner) => new $safefilerootname$(effect, owner));
	}

#region Constructors and Saving
	public $safefilerootname$(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner, parent, prog)
	{
	}

	protected $safefilerootname$(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			// Other XElements
		);
	}
#endregion

	public override string Describe(IPerceiver voyeur)
	{
		return $"$safeitemrootname$SpellEffect";
	}

	protected override string SpecificEffectType => "$safeitemrootname$SpellEffect";
}