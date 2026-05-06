using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal sealed class Disarm : OpposedAuxiliaryEffectBase
{
	private enum DisarmSelection
	{
		Best,
		Primary,
		Random
	}

	private DisarmSelection Selection { get; set; }

	public Disarm(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		Selection = root.Attribute("selection")?.Value.TryParseEnum(out DisarmSelection selection) == true
			? selection
			: DisarmSelection.Best;
	}

	private Disarm(IFuturemud gameworld, MudSharp.Body.Traits.ITraitDefinition defenseTrait,
		Difficulty defenseDifficulty) : base(gameworld, defenseTrait, defenseDifficulty, 90.0, 0.0, 90.0)
	{
		Selection = DisarmSelection.Best;
		SuccessEcho = "$0 knock|knocks $2 from $1's grip!";
	}

	protected override string TypeName => "disarm";
	protected override string EffectName => "Disarm";
	protected override string AmountName => "No-Get Duration";
	protected override string AmountUnit => "s";
	protected override double DefaultFlatAmount => 90.0;
	protected override double DefaultPerDegreeAmount => 0.0;
	protected override double DefaultMaximumAmount => 90.0;

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("disarm", (action, actor, command) =>
		{
			return !TryParseDefenseArguments(action, actor, command, out var trait, out var difficulty)
				? null
				: new Disarm(actor.Gameworld, trait, difficulty);
		},
			@"The Disarm effect knocks a wielded weapon out of the target's hands on a successful opposed auxiliary move.

The syntax to create this type is as follows:

	#3auxiliary set add disarm [defense trait] [difficulty]#0

If omitted, the defense trait defaults to the auxiliary action's check trait and difficulty defaults to Normal. The default targets the best wielded weapon and marks it with the normal combat no-get/get-item effects for 90 seconds.",
			true);
	}

	public override XElement Save()
	{
		var root = new XElement("Effect",
			new XAttribute("selection", Selection));
		SaveCommonAttributes(root);
		return root;
	}

	public override string DescribeForShow(ICharacter actor)
	{
		return $"{EffectName} | {Selection.DescribeEnum()} | {DescribeCommon(actor)}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "selection":
			case "select":
			case "target":
				return BuildingCommandSelection(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSelection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out DisarmSelection selection))
		{
			actor.OutputHandler.Send($"How should this effect select a weapon? The valid options are {Enum.GetValues<DisarmSelection>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Selection = selection;
		actor.OutputHandler.Send($"This effect will now select the {Selection.DescribeEnum().ColourValue()} wielded item.");
		return true;
	}

	protected override void AppendSpecificShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Selection: {Selection.DescribeEnum().ColourValue()}");
	}

	private IGameItem? SelectItem(ICharacter attacker, ICharacter target)
	{
		var items = target.Body.WieldedItems
		                  .Where(x => target.Body.CanBeDisarmed(x, attacker))
		                  .ToList();
		return Selection switch
		{
			DisarmSelection.Random => items.GetRandomElement(),
			DisarmSelection.Primary => items.FirstOrDefault(),
			_ => items
				.OrderByDescending(x => x.IsItemType<IMeleeWeapon>() || x.IsItemType<IRangedWeapon>())
				.ThenByDescending(x => x.Weight)
				.FirstOrDefault()
		};
	}

	public override void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		if (!TryGetOpposedSuccess(attacker, target, outcome, out var opposed))
		{
			SendEcho(FailureEcho, attacker, tch);
			return;
		}

		var item = SelectItem(attacker, tch);
		if (item is null)
		{
			return;
		}

		tch.Body.Take(item);
		item.RoomLayer = tch.RoomLayer;
		tch.Location.Insert(item);
		var duration = CalculateAmount(opposed);
		if (duration > 0.0)
		{
			item.AddEffect(new CombatNoGetEffect(item, tch.Combat), TimeSpan.FromSeconds(duration));
		}

		tch.AddEffect(new CombatGetItemEffect(tch, item));
		SendEcho(SuccessEcho, attacker, tch, item);
	}
}
