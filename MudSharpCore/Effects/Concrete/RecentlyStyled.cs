using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class RecentlyStyled : Effect, IRecentlyStyled
{
	public RecentlyStyled(IBody owner, ICharacteristicDefinition type) : base(owner)
	{
		CharacteristicType = type;
	}

	protected RecentlyStyled(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacteristicType = Gameworld.Characteristics.Get(long.Parse(effect.Element("CharacteristicType").Value));
	}

	public ICharacteristicDefinition CharacteristicType { get; set; }

	public override bool Applies(object target)
	{
		return base.Applies(target) && CharacteristicType == target;
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("CharacteristicType", CharacteristicType.Id);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("RecentlyStyled", (effect, owner) => new RecentlyStyled(effect, owner));
	}

	protected override string SpecificEffectType => "RecentlyStyled";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Has recently had their {CharacteristicType.Name} styled.";
	}
}