using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AdminSight : Effect, IAdminSightEffect, IAdminEffect
{
	public AdminSight(IPerceivable owner)
		: base(owner)
	{
	}

	public AdminSight(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "AdminSight";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Admin Sight";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminSight", (effect, owner) => new AdminSight(effect, owner));
	}

	public override string ToString()
	{
		return "Admin Sight Effect";
	}
}