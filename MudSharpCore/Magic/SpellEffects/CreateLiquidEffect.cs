using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.SpellEffects;

public class CreateLiquidEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createliquid", (root, spell) => new CreateLiquidEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createliquid", BuilderFactory,
			"Creates a puddle, splashes a character or fills a liquid container",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CreateLiquidEffect(new XElement("Effect",
			new XAttribute("type", "createliquid"),
			new XElement("LiquidId", 0),
			new XElement("AmountFormula", "0")
		), spell), string.Empty);
	}

	protected CreateLiquidEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_liquidId = long.Parse(root.Element("LiquidId").Value);
		AmountFormula = new Expression(root.Element("AmountFormula").Value);
	}
	public IFuturemud Gameworld => Spell.Gameworld;

	public IMagicSpell Spell { get; }

	private long _liquidId;

	public ILiquid Liquid => Gameworld.Liquids.Get(_liquidId);

	public Expression AmountFormula { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createliquid"),
			new XElement("Liquid", _liquidId),
			new XElement("AmountFormula", new XCData(AmountFormula.OriginalExpression))
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
public static bool IsCompatibleWithTrigger(string types)
	{
		switch (types)
		{
			case "item":
			case "items":
			case "character":
			case "characters":
			case "perceivable":
			case "perceivables":
			case "room":
			case "rooms":
				return true;
			default:
				return false;
		}
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var liquid = Liquid;
		if (liquid is null)
		{
			return null;
		}

		var amount = AmountFormula.EvaluateDoubleWith(
			("power", (int)power),
			("outcome", (int)outcome));
		var mixture = new LiquidMixture(Liquid, amount, Gameworld);

		if (target is ICharacter tch)
		{
			// Liquid contamination
			tch.Body.ExposeToLiquid(mixture, tch.Body.Limbs.GetRandomElement().Parts.OfType<IExternalBodypart>(), LiquidExposureDirection.Irrelevant);
			return null;
		}

		if (target is ICell cell)
		{
			// Puddle
			PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, cell, caster.RoomLayer, null);
			return null;
		}

		if (target is IGameItem gitem)
		{
			var container = gitem.GetItemType<ILiquidContainer>();
			if (container is null)
			{
				gitem.ExposeToLiquid(mixture, null, LiquidExposureDirection.Irrelevant);
				return null;
			}

			amount = container.LiquidCapacity - container.LiquidMixture?.TotalVolume ?? 0.0;
			if (mixture.TotalVolume > amount)
			{
				mixture.SetLiquidVolume(amount);
			}

			if (container.LiquidMixture?.CanMerge(mixture) == true)
			{
				container.MergeLiquid(mixture, null, "spell");
			}

			return null;

		}
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CreateLiquidEffect(SaveToXml(), Spell);
	}

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this effect:

	#3liquid <which>#0 - sets the liquid to be loaded
	#3amount <formula>#0 - sets the amount of liquid to be loaded (in ml)

Parameters for amount formula:

	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)";

	public string Show(ICharacter actor)
	{
		return
			$"CreateLiquid - {AmountFormula.OriginalExpression.ColourCommand()}ml of {Liquid?.Name.Colour(Liquid.DisplayColour) ?? "nothing".ColourError()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "liquid":
				return BuildingCommandLiquid(actor, command);
			case "quantity":
			case "amount":
			case "volume":
				return BuildingCommandAmount(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid should this spell effect load?");
			return false;
		}

		var proto = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid liquid.");
			return false;
		}

		_liquidId = proto.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now load the liquid {proto.Name.Colour(proto.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the formula for how much liquid should be loaded?");
			return false;
		}

		var formula = new Expression(command.SafeRemainingArgument);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		AmountFormula = formula;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The formula for liquid volume is now {formula.OriginalExpression.ColourCommand()}.");
		return true;
	}

	#endregion
}