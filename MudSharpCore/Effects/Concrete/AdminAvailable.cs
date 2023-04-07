using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AdminAvailable : Effect, IAdminAvailableEffect, IAdminEffect
{
	public AdminAvailable(IPerceivable owner)
		: base(owner)
	{
	}

	public AdminAvailable(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "AdminAvailable";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Admin Available on the WHO List";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminAvailable", (effect, owner) => new AdminAvailable(effect, owner));
	}

	public override string ToString()
	{
		return "Admin Available Effect";
	}
}