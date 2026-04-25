using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Magic.SpellEffects;

public class ForcedExitMovementEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("forcedexitmovement", (root, spell) => new ForcedExitMovementEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("forcedexitmovement", BuilderFactory,
			"Forces a character through the targeted exit when movement is legal",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new ForcedExitMovementEffect(new XElement("Effect", new XAttribute("type", "forcedexitmovement")), spell), string.Empty);
	}

	protected ForcedExitMovementEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "forcedexitmovement"));
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types == "character&exit";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter characterTarget)
		{
			return null;
		}

		ICellExit? exit = additionalParameters
		                  .FirstOrDefault(x => x.ParameterName.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
		                  ?.Item as ICellExit;
		if (exit is null)
		{
			return null;
		}

		CanMoveResponse move = characterTarget.CanMove(exit,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement);
		if (!move)
		{
			characterTarget.OutputHandler.Send(move.ErrorMessage);
			return null;
		}

		(bool success, IEmoteOutput? failureOutput) = characterTarget.CanCross(exit);
		if (!success)
		{
			if (failureOutput is not null)
			{
				characterTarget.OutputHandler.Handle(failureOutput);
			}

			return null;
		}

		characterTarget.Move(exit, ignoreSafeMovement: true);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ForcedExitMovementEffect(SaveToXml(), Spell);
	}

	public string Show(ICharacter actor)
	{
		return "Forced Exit Movement".ColourName();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public const string HelpText = "This effect has no additional builder options.";
}
