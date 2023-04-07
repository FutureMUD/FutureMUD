using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class CommandDelayMagicPower : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("CommandDelayMagicPower", (effect, owner) => new CommandDelayMagicPower(effect, owner));
	}

	public IMagicPower Power { get; protected set; }

	public CommandDelayMagicPower(IPerceivable owner, IMagicPower power) : base(owner)
	{
		Power = power;
	}

	protected CommandDelayMagicPower(XElement root, IPerceivable owner) : base(root, owner)
	{
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Value));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Blocked from using the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}

	public override bool Applies(object target)
	{
		if (target is IMagicPower power)
		{
			return Power == power;
		}

		return base.Applies(target);
	}

	protected override string SpecificEffectType => "CommandDelayMagicPower";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Power", Power.Id);
	}

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		((ICharacter)Owner).OutputHandler.Send(
			$"You can use the {Power.Name.TitleCase().Colour(Power.School.PowerListColour)} power again.");
	}
}