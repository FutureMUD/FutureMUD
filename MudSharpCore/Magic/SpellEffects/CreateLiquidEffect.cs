using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class CreateLiquidEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission
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
        _liquidId = long.Parse((root.Element("LiquidId") ?? root.Element("Liquid"))!.Value);
        AmountFormula = new Expression(root.Element("AmountFormula").Value);
		LoadContainerFill(root.Element("ContainerFill"));
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
            new XElement("LiquidId", _liquidId),
            new XElement("AmountFormula", new XCData(AmountFormula.OriginalExpression)),
			SaveContainerFill()
        );
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types)
    {
        return IsCompatibleWithTrigger(types.TargetTypes);
    }

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
		if (ContainerOnly || _fillLoadError is not null)
		{
			if (!TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out var error))
				throw new InvalidOperationException(error);
			return application!.Create(parent);
		}
        ILiquid liquid = Liquid;
        if (liquid is null)
        {
            return null;
        }

        double amount = AmountFormula.EvaluateDoubleWith(
            ("power", (int)power),
            ("outcome", (int)outcome));
        LiquidMixture mixture = new(Liquid, amount, Gameworld);

        if (target is ICharacter tch)
        {
            // Liquid contamination
            tch.Body.ExposeToLiquid(mixture, tch.Body.Limbs.GetRandomElement().Parts.OfType<IExternalBodypart>(), LiquidExposureDirection.Irrelevant);
            return null;
        }

        if (target is IRoom room)
        {
            // Puddle
            PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, room, caster.RoomLayer, null);
            return null;
        }

        if (target is IGameItem gitem)
        {
            ILiquidContainer container = gitem.GetItemType<ILiquidContainer>();
            if (container is null)
            {
                gitem.ExposeToLiquid(mixture, null, LiquidExposureDirection.Irrelevant);
                return null;
            }

            amount = container.LiquidCapacity - (container.LiquidMixture?.TotalVolume ?? 0.0);
            if (mixture.TotalVolume > amount)
            {
                mixture.SetLiquidVolume(amount);
            }

            if (amount <= 0.0 || mixture.TotalVolume <= 0.0)
            {
                return null;
            }

            if (container.LiquidMixture is null || container.LiquidMixture.CanMerge(mixture))
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
	#3amount <formula>#0 - sets the legacy volume in native fluid units
	#3containerfill on|off#0 - uses accessible owned containers with capacity and compatibility admission
	#3litres <formula>#0 - sets a prepayment route-bound grade/power/mastery/trait formula in litres
	#3compatible <liquid>#0 - toggles an additional allowed existing liquid (source liquid is always allowed)
	#3bonusplane <plane> <multiplier>|none#0 - scales the prepared amount on one configured plane

	#3recipe <order 1-32> <boolean(character) prog|always> <liquid>#0 - first matching recipe; explicit always fallback last
	#3recipe <order> remove#0 - removes an ordered recipe

Parameters for amount formula:

	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)";

    public string Show(ICharacter actor)
    {
        return SpellEffectPresentation.Describe(actor, "Create Liquid",
            ("Liquid", Liquid?.Name.Colour(Liquid.DisplayColour) ?? "nothing".ColourError()),
            ("Amount", ContainerOnly ? $"{LitresExpression?.OriginalFormulaText} litres".ColourCommand() : $"{AmountFormula.OriginalExpression} native fluid units".ColourCommand()),
			("Container Fill", ContainerOnly.ToColouredString()), ("Recipes", SaveRecipes()?.ToString(SaveOptions.DisableFormatting) ?? "Fixed liquid"),
			("Compatible", string.Join(", ", _compatibleLiquids.Prepend(_liquidId).Distinct().Select(x => Gameworld.Liquids.Get(x)?.Name ?? $"#{x}"))),
			("Bonus Plane", _bonusPlaneId == 0 ? "None" : $"{Gameworld.Planes.Get(_bonusPlaneId)?.Name}: {_planeMultiplier}"),
			("Validation", DefinitionError ?? "Valid"));
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
		if (command.PeekSpeech().EqualTo("recipe")) return BuildingCommandRecipe(actor, command);
		if (command.PeekSpeech().ToLowerInvariant() is "containerfill" or "litres" or "compatible" or "bonusplane")
			return BuildingCommandContainerFill(actor, command);
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

        ILiquid proto = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
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

        Expression formula = new(command.SafeRemainingArgument);
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
