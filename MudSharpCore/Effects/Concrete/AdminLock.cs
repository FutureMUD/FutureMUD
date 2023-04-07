using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class AdminLock : Effect, IOverrideLockEffect
{
	public AdminLock(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected AdminLock(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"An admin has placed an admin lock on this lockable.";
	}

	protected override string SpecificEffectType => "AdminLock";
	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminLock", (effect, owner) => new AdminLock(effect, owner));
	}

	public override bool Applies()
	{
		return true;
	}

	public override bool Applies(object target)
	{
		return target is ICharacter ch && !ch.IsAdministrator();
	}

	#endregion
}