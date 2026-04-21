#nullable enable
using ExpressionEngine;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class MagicResourceDeltaEffect : IMagicSpellEffectTemplate
{
	public const string HelpText = @"You can use the following options with this effect:

	#3resource <which>#0 - sets which magic resource is changed
	#3formula <expr>#0 - sets the amount delta formula";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("magicresourcedelta",
			(root, spell) => new MagicResourceDeltaEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("magicresourcedelta", BuilderFactory,
			"Instantly adds or removes magic resource from a character, item or room",
			HelpText,
			true,
			true,
			StandaloneSpellEffectTemplateHelper.MagicResourceTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new MagicResourceDeltaEffect(new XElement("Effect",
			new XAttribute("type", "magicresourcedelta"),
			new XElement("Resource", 0L),
			new XElement("Formula", new XCData("power*outcome"))
		), spell), string.Empty);
	}

	protected MagicResourceDeltaEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		LoadFromXml(root);
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicResource? Resource { get; private set; }
	public ITraitExpression DeltaExpression { get; private set; } = null!;

	private void LoadFromXml(XElement root)
	{
		var resourceText = root.Element("Resource")?.Value ?? "0";
		Resource = long.TryParse(resourceText, out var value)
			? Gameworld.MagicResources.Get(value)
			: Gameworld.MagicResources.GetByIdOrName(resourceText);
		DeltaExpression = new TraitExpression(root.Element("Formula")?.Value ?? "0", Gameworld);
	}

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "magicresourcedelta"),
			new XElement("Resource", Resource?.Id ?? 0L),
			new XElement("Formula", new XCData(DeltaExpression.OriginalFormulaText))
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return StandaloneSpellEffectTemplateHelper.IsMagicResourceTarget(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IHaveMagicResource targetResource || Resource is null)
		{
			return null;
		}

		var delta = DeltaExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (delta == 0.0)
		{
			return null;
		}

		targetResource.AddResource(Resource, delta);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new MagicResourceDeltaEffect(SaveToXml(), Spell);
	}

	public string Show(ICharacter actor)
	{
		return
			$"Magic Resource Delta - {(Resource?.Name ?? "None".ColourError())} by {DeltaExpression.OriginalFormulaText.ColourCommand()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "resource":
				return BuildingCommandResource(actor, command);
			case "formula":
				return BuildingCommandFormula(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic resource should this effect change?");
			return false;
		}

		var resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such magic resource.");
			return false;
		}

		Resource = resource;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect will now change the {resource.Name.ColourValue()} magic resource.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula should determine the resource change?");
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DeltaExpression = expression;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect will now use the formula {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}
}
