using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;

public class EnforcerEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("Enforcer", (effect, owner) => new EnforcerEffect(effect, owner));
	}

	public ILegalAuthority LegalAuthority { get; }
	public IEnforcementAuthority EnforcementAuthority { get; }
	public ICharacter CharacterOwner { get; }

	protected EnforcerEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		CharacterOwner = (ICharacter)owner;
		LegalAuthority =
			Gameworld.LegalAuthorities.Get(long.Parse(root.Element("Effect").Element("Authority").Value));
		EnforcementAuthority =
			Gameworld.EnforcementAuthorities.Get(long.Parse(root.Element("Effect").Element("Enforcement").Value));
	}

	public EnforcerEffect(ICharacter owner, ILegalAuthority authority) : base(owner, null)
	{
		CharacterOwner = owner;
		LegalAuthority = authority;
		EnforcementAuthority = authority.GetEnforcementAuthority(owner);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Enforcer AI Effect";
	}

	#region Overrides of Effect

	public override bool Applies(object target)
	{
		if (target is ILegalAuthority authority)
		{
			return LegalAuthority == authority;
		}

		return base.Applies(target);
	}

	#endregion

	protected override string SpecificEffectType => "Enforcer";
	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Authority", LegalAuthority.Id),
			new XElement("Enforcement", EnforcementAuthority.Id)
		);
	}
}