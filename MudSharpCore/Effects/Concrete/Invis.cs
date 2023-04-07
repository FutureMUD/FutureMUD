using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class Invis : Effect, IEffectSubtype
{
	public Invis(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	public Invis(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Item is invisible with prog {ApplicabilityProg?.Id ?? 0}.";
	}

	public override PerceptionTypes Obscuring => PerceptionTypes.All;

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new Invis(newItem, ApplicabilityProg);
		}

		return null;
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Invis", (effect, owner) => new Invis(effect, owner));
	}

	protected override string SpecificEffectType => "Invis";

	#endregion
}