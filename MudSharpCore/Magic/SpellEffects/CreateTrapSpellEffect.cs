#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.Traps;

namespace MudSharp.Magic.SpellEffects;

/// <summary>
/// Installs a current magical trap template on a spell target. The deployed effect pins the selected template
/// revision, so later builder revisions cannot silently change an already prepared glyph or ward.
/// </summary>
public sealed class CreateTrapSpellEffect : IMagicSpellEffectTemplate
{
	private long _templateId;
	private int _templateRevision;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createtrap", (root, spell) => new CreateTrapSpellEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("placetrap", (root, spell) => new CreateTrapSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory(
			"createtrap",
			BuilderFactory,
			"Installs a configured magical trap template on an item or cell",
			"Use template <traptemplate> to choose a current magical template.",
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleTargetType(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
		SpellEffectFactory.RegisterBuilderFactory(
			"placetrap",
			BuilderFactory,
			"Installs a configured magical trap template on an item or cell",
			"Use template <traptemplate> to choose a current magical template.",
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleTargetType(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CreateTrapSpellEffect(
			new XElement("Effect",
				new XAttribute("type", "createtrap"),
				new XElement("TrapTemplateId", 0L),
				new XElement("TrapTemplateRevision", 0)),
			spell), string.Empty);
	}

	private CreateTrapSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_templateId = long.TryParse(root.Element("TrapTemplateId")?.Value, out var templateId) ? templateId : 0L;
		_templateRevision = int.TryParse(root.Element("TrapTemplateRevision")?.Value, out var templateRevision)
			? templateRevision
			: 0;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	private ITrapTemplate? TrapTemplate => _templateId > 0 ? Gameworld.TrapTemplates.Get(_templateId, _templateRevision) : null;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createtrap"),
			new XElement("TrapTemplateId", _templateId),
			new XElement("TrapTemplateRevision", _templateRevision));
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() is not ("template" or "traptemplate"))
		{
			actor.Send("Use template <traptemplate> to set the magical trap installed by this spell.");
			return false;
		}

		var template = Gameworld.TrapTemplates.GetByIdOrName(command.SafeRemainingArgument);
		if (template is null || template.SourceKind != TrapSourceKind.Magical ||
		    template.Status != RevisionStatus.Current)
		{
			actor.Send("You must choose a current magical trap template.");
			return false;
		}

		if (!template.CanSubmit())
		{
			actor.Send($"That trap template is not ready: {template.WhyCannotSubmit()}");
			return false;
		}

		_templateId = template.Id;
		_templateRevision = template.RevisionNumber;
		Spell.Changed = true;
		actor.Send($"This spell will now install {template.Name.ColourName()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Create Trap",
			("Template", TrapTemplate?.Name.ColourName() ?? "None".ColourError()));
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleTargetType(trigger.TargetTypes);
	}

	private static bool IsCompatibleTargetType(string targetTypes)
	{
		return targetTypes is "item" or "room" or "exit";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		var template = TrapTemplate;
		var boundExit = additionalParameters
			.FirstOrDefault(x => x.ParameterName.EqualTo("exit"))?.Item as ICellExit;
		var anchor = boundExit?.Origin ?? target;
		if (anchor is null || template is null || template.SourceKind != TrapSourceKind.Magical ||
		    template.Status != RevisionStatus.Current || !template.CanSubmit() || !TrapEffect.IsValidAnchor(template, anchor) ||
		    anchor.EffectsOfType<TrapEffect>().Any(x => x.State is not TrapState.Spent and not TrapState.Expired &&
		                                           HasSameBinding(x, boundExit)))
		{
			return null;
		}

		var trap = new TrapEffect(anchor, template, caster, boundExit);
		if (TrapEffect.HasTimedLifetime(template))
		{
			anchor.AddEffect(trap, template.Lifespan!.Value);
		}
		else
		{
			anchor.AddEffect(trap);
		}

		if (!caster.IsAdministrator())
		{
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(
				caster,
				CrimeTypes.BoobyTrapping,
				null,
				anchor as IGameItem,
				template.Name);
		}

		return null;
	}

	private static bool HasSameBinding(TrapEffect trap, ICellExit? exit) => exit is null
		? !trap.BoundExitId.HasValue
		: trap.BoundExitId == exit.Exit.Id && trap.BoundExitOriginId == exit.Origin.Id;

	public IMagicSpellEffectTemplate Clone()
	{
		return new CreateTrapSpellEffect(SaveToXml(), Spell);
	}
}
