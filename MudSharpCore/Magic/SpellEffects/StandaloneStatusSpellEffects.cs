#nullable enable
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

internal static class StandaloneSpellEffectTemplateHelper
{
	public static readonly string[] CharacterTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsCharacterTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static readonly string[] RoomTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsRoomTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static readonly string[] MagicResourceTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsMagicResourceTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static bool IsCharacterTarget(string types)
	{
		return types switch
		{
			"character" => true,
			"characters" => true,
			_ => false
		};
	}

	public static bool IsRoomTarget(string types)
	{
		return types switch
		{
			"room" => true,
			"rooms" => true,
			_ => false
		};
	}

	public static bool IsMagicResourceTarget(string types)
	{
		return types switch
		{
			"character" => true,
			"characters" => true,
			"item" => true,
			"items" => true,
			"room" => true,
			"rooms" => true,
			"perceivable" => true,
			"perceivables" => true,
			"character&room" => true,
			_ => false
		};
	}
}

public abstract class CharacterSpellEffectTemplateBase : IMagicSpellEffectTemplate
{
	protected CharacterSpellEffectTemplateBase(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		LoadFromXml(root);
	}

	protected virtual void LoadFromXml(XElement root)
	{
	}

	protected abstract string BuilderEffectType { get; }
	protected abstract string ShowText { get; }

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;

	public virtual bool IsInstantaneous => false;
	public virtual bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		XElement root = new("Effect", new XAttribute("type", BuilderEffectType));
		SaveToXml(root);
		return root;
	}

	protected virtual void SaveToXml(XElement root)
	{
	}

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("No options for this effect.");
		return false;
	}

	public virtual string Show(ICharacter actor)
	{
		return ShowText;
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return StandaloneSpellEffectTemplateHelper.IsCharacterTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter character)
		{
			return null;
		}

		return CreateEffect(caster, character, outcome, power, parent, additionalParameters);
	}

	protected abstract IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters);

	public abstract IMagicSpellEffectTemplate Clone();
}

public abstract class CharacterSpellEffectRemovalTemplateBase : CharacterSpellEffectTemplateBase
{
	protected CharacterSpellEffectRemovalTemplateBase(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	public override bool IsInstantaneous => true;

	protected sealed override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		RemoveEffects(target);
		return null;
	}

	protected abstract void RemoveEffects(ICharacter target);
}

public class SilenceEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("silence", (root, spell) => new SilenceEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("silence", BuilderFactory,
			"Silences the target",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new SilenceEffect(new XElement("Effect", new XAttribute("type", "silence")), spell), string.Empty);

	public static bool IsCompatibleWithTrigger(string types) => StandaloneSpellEffectTemplateHelper.IsCharacterTarget(types);

	protected override string BuilderEffectType => "silence";
	protected override string ShowText => "Silence";

	protected SilenceEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellSilenceEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new SilenceEffect(SaveToXml(), Spell);
}

public class RemoveSilenceEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removesilence", (root, spell) => new RemoveSilenceEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removesilence", BuilderFactory,
			"Removes magical silence from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveSilenceEffect(new XElement("Effect", new XAttribute("type", "removesilence")), spell), string.Empty);

	protected override string BuilderEffectType => "removesilence";
	protected override string ShowText => "Remove Silence";

	protected RemoveSilenceEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellSilenceEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveSilenceEffect(SaveToXml(), Spell);
}

public class SleepEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("sleep", (root, spell) => new SleepEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("sleep", BuilderFactory,
			"Forces the target into magical slumber",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new SleepEffect(new XElement("Effect", new XAttribute("type", "sleep")), spell), string.Empty);

	protected override string BuilderEffectType => "sleep";
	protected override string ShowText => "Sleep";

	protected SleepEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellSleepEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new SleepEffect(SaveToXml(), Spell);
}

public class RemoveSleepEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removesleep", (root, spell) => new RemoveSleepEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removesleep", BuilderFactory,
			"Removes magical sleep from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveSleepEffect(new XElement("Effect", new XAttribute("type", "removesleep")), spell), string.Empty);

	protected override string BuilderEffectType => "removesleep";
	protected override string ShowText => "Remove Sleep";

	protected RemoveSleepEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellSleepEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveSleepEffect(SaveToXml(), Spell);
}

public class FearEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("fear", (root, spell) => new FearEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("fear", BuilderFactory,
			"Fills the target with magical fear",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new FearEffect(new XElement("Effect", new XAttribute("type", "fear")), spell), string.Empty);

	protected override string BuilderEffectType => "fear";
	protected override string ShowText => "Fear";

	protected FearEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellFearEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new FearEffect(SaveToXml(), Spell);
}

public class RemoveFearEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removefear", (root, spell) => new RemoveFearEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removefear", BuilderFactory,
			"Removes magical fear from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveFearEffect(new XElement("Effect", new XAttribute("type", "removefear")), spell), string.Empty);

	protected override string BuilderEffectType => "removefear";
	protected override string ShowText => "Remove Fear";

	protected RemoveFearEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellFearEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveFearEffect(SaveToXml(), Spell);
}

public class ParalysisEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("paralysis", (root, spell) => new ParalysisEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("paralysis", BuilderFactory,
			"Paralyses the target",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new ParalysisEffect(new XElement("Effect", new XAttribute("type", "paralysis")), spell), string.Empty);

	protected override string BuilderEffectType => "paralysis";
	protected override string ShowText => "Paralysis";

	protected ParalysisEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellParalysisEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new ParalysisEffect(SaveToXml(), Spell);
}

public class RemoveParalysisEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeparalysis", (root, spell) => new RemoveParalysisEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removeparalysis", BuilderFactory,
			"Removes magical paralysis from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveParalysisEffect(new XElement("Effect", new XAttribute("type", "removeparalysis")), spell), string.Empty);

	protected override string BuilderEffectType => "removeparalysis";
	protected override string ShowText => "Remove Paralysis";

	protected RemoveParalysisEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellParalysisEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveParalysisEffect(SaveToXml(), Spell);
}

public class FlyingEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("flying", (root, spell) => new FlyingEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("flying", BuilderFactory,
			"Grants magical flight to the target",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new FlyingEffect(new XElement("Effect", new XAttribute("type", "flying")), spell), string.Empty);

	protected override string BuilderEffectType => "flying";
	protected override string ShowText => "Flying";

	protected FlyingEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellFlightEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new FlyingEffect(SaveToXml(), Spell);
}

public class RemoveFlyingEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeflying", (root, spell) => new RemoveFlyingEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removeflying", BuilderFactory,
			"Removes magical flight from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveFlyingEffect(new XElement("Effect", new XAttribute("type", "removeflying")), spell), string.Empty);

	protected override string BuilderEffectType => "removeflying";
	protected override string ShowText => "Remove Flying";

	protected RemoveFlyingEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellFlightEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveFlyingEffect(SaveToXml(), Spell);
}

public class WaterBreathingEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("waterbreathing", (root, spell) => new WaterBreathingEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("waterbreathing", BuilderFactory,
			"Lets the target breathe underwater",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new WaterBreathingEffect(new XElement("Effect", new XAttribute("type", "waterbreathing")), spell), string.Empty);

	protected override string BuilderEffectType => "waterbreathing";
	protected override string ShowText => "Water Breathing";

	protected WaterBreathingEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellWaterBreathingEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new WaterBreathingEffect(SaveToXml(), Spell);
}

public class RemoveWaterBreathingEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removewaterbreathing", (root, spell) => new RemoveWaterBreathingEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removewaterbreathing", BuilderFactory,
			"Removes magical water breathing from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveWaterBreathingEffect(new XElement("Effect", new XAttribute("type", "removewaterbreathing")), spell), string.Empty);

	protected override string BuilderEffectType => "removewaterbreathing";
	protected override string ShowText => "Remove Water Breathing";

	protected RemoveWaterBreathingEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellWaterBreathingEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveWaterBreathingEffect(SaveToXml(), Spell);
}

public class CurseEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("curse", (root, spell) => new CurseEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("curse", BuilderFactory,
			"Curses the target",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new CurseEffect(new XElement("Effect", new XAttribute("type", "curse")), spell), string.Empty);

	protected override string BuilderEffectType => "curse";
	protected override string ShowText => "Curse";

	protected CurseEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellCurseEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new CurseEffect(SaveToXml(), Spell);
}

public class RemoveCurseEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removecurse", (root, spell) => new RemoveCurseEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removecurse", BuilderFactory,
			"Removes magical curses from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveCurseEffect(new XElement("Effect", new XAttribute("type", "removecurse")), spell), string.Empty);

	protected override string BuilderEffectType => "removecurse";
	protected override string ShowText => "Remove Curse";

	protected RemoveCurseEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellCurseEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveCurseEffect(SaveToXml(), Spell);
}

public class DetectInvisibleEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("detectinvisible", (root, spell) => new DetectInvisibleEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("detectinvisible", BuilderFactory,
			"Lets the target perceive invisibility",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new DetectInvisibleEffect(new XElement("Effect", new XAttribute("type", "detectinvisible")), spell), string.Empty);

	protected override string BuilderEffectType => "detectinvisible";
	protected override string ShowText => "Detect Invisible";

	protected DetectInvisibleEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellDetectInvisibleEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new DetectInvisibleEffect(SaveToXml(), Spell);
}

public class RemoveDetectInvisibleEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removedetectinvisible", (root, spell) => new RemoveDetectInvisibleEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removedetectinvisible", BuilderFactory,
			"Removes magical detect invisible from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveDetectInvisibleEffect(new XElement("Effect", new XAttribute("type", "removedetectinvisible")), spell), string.Empty);

	protected override string BuilderEffectType => "removedetectinvisible";
	protected override string ShowText => "Remove Detect Invisible";

	protected RemoveDetectInvisibleEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellDetectInvisibleEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveDetectInvisibleEffect(SaveToXml(), Spell);
}

public class DetectEtherealEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("detectethereal", (root, spell) => new DetectEtherealEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("detectethereal", BuilderFactory,
			"Lets the target perceive ethereal things",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new DetectEtherealEffect(new XElement("Effect", new XAttribute("type", "detectethereal")), spell), string.Empty);

	protected override string BuilderEffectType => "detectethereal";
	protected override string ShowText => "Detect Ethereal";

	protected DetectEtherealEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellDetectEtherealEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new DetectEtherealEffect(SaveToXml(), Spell);
}

public class RemoveDetectEtherealEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removedetectethereal", (root, spell) => new RemoveDetectEtherealEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removedetectethereal", BuilderFactory,
			"Removes magical detect ethereal from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveDetectEtherealEffect(new XElement("Effect", new XAttribute("type", "removedetectethereal")), spell), string.Empty);

	protected override string BuilderEffectType => "removedetectethereal";
	protected override string ShowText => "Remove Detect Ethereal";

	protected RemoveDetectEtherealEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellDetectEtherealEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveDetectEtherealEffect(SaveToXml(), Spell);
}

public class DetectMagickEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("detectmagick", (root, spell) => new DetectMagickEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("detectmagick", BuilderFactory,
			"Lets the target sense nearby magic",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new DetectMagickEffect(new XElement("Effect", new XAttribute("type", "detectmagick")), spell), string.Empty);

	protected override string BuilderEffectType => "detectmagick";
	protected override string ShowText => "Detect Magick";

	protected DetectMagickEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellDetectMagickEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new DetectMagickEffect(SaveToXml(), Spell);
}

public class RemoveDetectMagickEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removedetectmagick", (root, spell) => new RemoveDetectMagickEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removedetectmagick", BuilderFactory,
			"Removes magical detect magick from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveDetectMagickEffect(new XElement("Effect", new XAttribute("type", "removedetectmagick")), spell), string.Empty);

	protected override string BuilderEffectType => "removedetectmagick";
	protected override string ShowText => "Remove Detect Magick";

	protected RemoveDetectMagickEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellDetectMagickEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveDetectMagickEffect(SaveToXml(), Spell);
}

public class InfravisionEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("infravision", (root, spell) => new InfravisionEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("infravision", BuilderFactory,
			"Grants magical infravision to the target",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new InfravisionEffect(new XElement("Effect", new XAttribute("type", "infravision")), spell), string.Empty);

	protected override string BuilderEffectType => "infravision";
	protected override string ShowText => "Infravision";

	protected InfravisionEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellInfravisionEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new InfravisionEffect(SaveToXml(), Spell);
}

public class RemoveInfravisionEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeinfravision", (root, spell) => new RemoveInfravisionEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removeinfravision", BuilderFactory,
			"Removes magical infravision from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveInfravisionEffect(new XElement("Effect", new XAttribute("type", "removeinfravision")), spell), string.Empty);

	protected override string BuilderEffectType => "removeinfravision";
	protected override string ShowText => "Remove Infravision";

	protected RemoveInfravisionEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellInfravisionEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveInfravisionEffect(SaveToXml(), Spell);
}

public class ComprehendLanguageEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("comprehendlanguage", (root, spell) => new ComprehendLanguageEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("comprehendlanguage", BuilderFactory,
			"Lets the target comprehend languages",
			"",
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new ComprehendLanguageEffect(new XElement("Effect", new XAttribute("type", "comprehendlanguage")), spell), string.Empty);

	protected override string BuilderEffectType => "comprehendlanguage";
	protected override string ShowText => "Comprehend Language";

	protected ComprehendLanguageEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
		=> new SpellComprehendLanguageEffect(target, parent);

	public override IMagicSpellEffectTemplate Clone() => new ComprehendLanguageEffect(SaveToXml(), Spell);
}

public class RemoveComprehendLanguageEffect : CharacterSpellEffectRemovalTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removecomprehendlanguage", (root, spell) => new RemoveComprehendLanguageEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removecomprehendlanguage", BuilderFactory,
			"Removes magical language comprehension from the target",
			"",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveComprehendLanguageEffect(new XElement("Effect", new XAttribute("type", "removecomprehendlanguage")), spell), string.Empty);

	protected override string BuilderEffectType => "removecomprehendlanguage";
	protected override string ShowText => "Remove Comprehend Language";

	protected RemoveComprehendLanguageEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override void RemoveEffects(ICharacter target) => target.RemoveAllEffects<SpellComprehendLanguageEffect>(null, true);

	public override IMagicSpellEffectTemplate Clone() => new RemoveComprehendLanguageEffect(SaveToXml(), Spell);
}
