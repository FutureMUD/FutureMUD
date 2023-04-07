using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AdminInvis : Effect, IAdminInvisEffect, IAdminEffect
{
	public AdminInvis(IPerceivable owner)
		: base(owner)
	{
	}

	public AdminInvis(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "AdminInvis";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Admin is using WIZINVIS";
	}

	public override PerceptionTypes Obscuring => PerceptionTypes.All;

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminInvis", (effect, owner) => new AdminInvis(effect, owner));
	}

	public override string ToString()
	{
		return "Admin Invisibility Effect";
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new AdminInvis(newItem);
		}

		return null;
	}
}