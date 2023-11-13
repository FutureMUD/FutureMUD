using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MagicAttackPower : MagicPowerBase, IMagicAttackPower
{
	public override string PowerType => "Magic Attack";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("magicattack", (power, gameworld) => new MagicAttackPower(power, gameworld));
	}

	protected MagicAttackPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a Verb element.");
		}

		_verb = element.Value;

		element = root.Element("PowerIntentions");
		if (element == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) did not contain a PowerIntentions element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			var flag = CombatMoveIntentions.None;
			var split = element.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in split)
			{
				if (Utilities.TryParseEnum<CombatMoveIntentions>(item, out var sub))
				{
					flag |= sub;
					continue;
				}

				throw new ApplicationException(
					$"MagicAttackPower {Id} ({Name}) had an invalid CombatMoveIntention - {item}");
			}

			PowerIntentions = flag;
		}
		else
		{
			PowerIntentions = (CombatMoveIntentions)value;
		}

		element = root.Element("ValidDefenseTypes");
		if (element == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) did not contain a ValidDefenseTypes element.");
		}

		foreach (var subelement in element.Elements())
		{
			if (!Utilities.TryParseEnum<DefenseType>(subelement.Value, out var defense))
			{
				throw new ApplicationException(
					$"MagicAttackPower {Id} ({Name}) had an invalid DefenseType value - {subelement.Value}");
			}

			_validDefenseTypes.Add(defense);
		}

		element = root.Element("WeaponAttack");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a WeaponAttack element.");
		}

		WeaponAttack = long.TryParse(element.Value, out value)
			? Gameworld.WeaponAttacks.Get(value)
			: Gameworld.WeaponAttacks.GetByName(element.Value);
		if (WeaponAttack == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a valid WeaponAttack.");
		}

		element = root.Element("AttackerTrait");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain an AttackerTrait element.");
		}

		AttackerTrait = long.TryParse(element.Value, out value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);
		if (AttackerTrait == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) had an AttackerTrait that did not refer to a valid TraitDefinition.");
		}

		element = root.Element("Reach");
		Reach = int.TryParse(element?.Value ?? "0", out var reach) ? reach : 0;

		element = root.Element("MoveType");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a MoveType element.");
		}

		if (!Utilities.TryParseEnum<BuiltInCombatMoveType>(element.Value, out var movetype))
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) had a MoveType that did not refer to a valid BuiltInCombatMoveType - {element.Value}");
		}

		MoveType = movetype;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		actor.OutputHandler.Send("Directly invoking the power is coming soon.");
	}

	public bool CanInvokePower(ICharacter invoker, ICharacter target)
	{
		if (CanInvokePowerProg?.Execute<bool?>(invoker, target) == false)
		{
			return false;
		}

		if (!CanAffordToInvokePower(invoker, _verb).Truth)
		{
			return false;
		}

		if (WeaponAttack.UsabilityProg?.Execute<bool?>(invoker, null, target) == false)
		{
			return false;
		}

		return true;
	}

	public void UseAttackPower(IMagicPowerAttackMove move)
	{
		ConsumePowerCosts(move.Assailant, _verb);
	}

	private string _verb;
	public override IEnumerable<string> Verbs => new[] { _verb };
	public CombatMoveIntentions PowerIntentions { get; }

	private readonly List<DefenseType> _validDefenseTypes = new();
	public IEnumerable<DefenseType> ValidDefenseTypes => _validDefenseTypes;
	public ITraitDefinition AttackerTrait { get; }
	public IWeaponAttack WeaponAttack { get; }
	public int Reach { get; }
	public BuiltInCombatMoveType MoveType { get; }

	public double BaseDelay => WeaponAttack.BaseDelay;
	public ExertionLevel ExertionLevel => WeaponAttack.ExertionLevel;
	public double StaminaCost => WeaponAttack.StaminaCost;
	public double Weighting => WeaponAttack.Weighting;
	public Orientation Orientation => WeaponAttack.Orientation;
	public Alignment Alignment => WeaponAttack.Alignment;
	public Difficulty BaseBlockDifficulty => WeaponAttack.Profile.BaseBlockDifficulty;
	public Difficulty BaseParryDifficulty => WeaponAttack.Profile.BaseParryDifficulty;
	public Difficulty BaseDodgeDifficulty => WeaponAttack.Profile.BaseDodgeDifficulty;
}