using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

public abstract class MagicSpellEffectBase : Effect, IMagicSpellEffect
{
	protected MagicSpellEffectBase(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner,
		prog)
	{
		ParentEffect = parent;
	}

	protected MagicSpellEffectBase(XElement root, IPerceivable owner) : base(root, owner)
	{
		ApplicabilityProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ApplicabilityProg")?.Value ?? "0"));
	}

	#region Implementation of IMagicSpellEffect

	public IMagicSpellEffectParent ParentEffect { get; set; }
	public IMagicSpell Spell => ParentEffect.Spell;

	#endregion

	#region Overrides of Effect

	public sealed override bool SavingEffect => false;

	#endregion
}