#nullable enable

using ExpressionEngine;
using MudSharp.Accounts;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Commands;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.NPC.Templates;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class MagicTagEffect : IMagicSpellEffectTemplate, IMagicInterdictionTagProvider
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("magictag", (root, spell) => new MagicTagEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("magictag", BuilderFactory,
			"Applies informational magic metadata tags",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new MagicTagEffect(new XElement("Effect",
			new XAttribute("type", "magictag"),
			new XElement("Tag", new XCData("magic")),
			new XElement("Value", new XCData("")),
			new XElement("ReplaceExisting", true)
		), spell), string.Empty);
	}

	protected MagicTagEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Tag = root.Element("Tag")?.Value ?? "magic";
		Value = root.Element("Value")?.Value ?? string.Empty;
		ReplaceExisting = bool.Parse(root.Element("ReplaceExisting")?.Value ?? "true");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Tag { get; private set; }
	public string Value { get; private set; }
	public bool ReplaceExisting { get; private set; }
	public IEnumerable<MagicInterdictionTag> MagicInterdictionTags
	{
		get
		{
			yield return new MagicInterdictionTag(Tag, Value);
		}
	}

	public virtual XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "magictag"),
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value)),
			new XElement("ReplaceExisting", ReplaceExisting)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "character" or "characters" or "item" or "items" or "room" or "rooms" or "perceivable" or
			"perceivables";
	}

	public virtual bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return IsCompatibleWithTrigger(types.TargetTypes);
	}

	public virtual IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null)
		{
			return null;
		}

		if (ReplaceExisting)
		{
			target.RemoveAllEffects<IMagicTagEffect>(x => x.Tag.EqualTo(Tag), true);
		}

		return new SpellMagicTagEffect(target, parent, Tag, Value);
	}

	public virtual IMagicSpellEffectTemplate Clone()
	{
		return new MagicTagEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3tag <key>#0 - sets the tag key
	#3value <value>#0 - sets the tag value
	#3replace#0 - toggles replacing existing tags with the same key";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "key":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which magic tag key should this effect apply?");
					return false;
				}

				Tag = command.SafeRemainingArgument;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will apply the magic tag key {Tag.ColourName()}.");
				return true;
			case "value":
				Value = command.SafeRemainingArgument;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will apply the magic tag value {Value.ColourValue()}.");
				return true;
			case "replace":
				ReplaceExisting = !ReplaceExisting;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {ReplaceExisting.NowNoLonger()} replace existing matching tags.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Magic Tag",
			("Tag", Tag.ColourName()),
			("Value", Value.ColourValue()),
			("Replace Existing", ReplaceExisting.ToColouredString())
		);
	}
}

public class RemoveMagicTagEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removemagictag",
			(root, spell) => new RemoveMagicTagEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removemagictag", BuilderFactory,
			"Removes informational magic metadata tags",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => MagicTagEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RemoveMagicTagEffect(new XElement("Effect",
			new XAttribute("type", "removemagictag"),
			new XElement("Tag", new XCData("magic")),
			new XElement("Value", new XCData("")),
			new XElement("MatchValue", false)
		), spell), string.Empty);
	}

	protected RemoveMagicTagEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Tag = root.Element("Tag")?.Value ?? "magic";
		Value = root.Element("Value")?.Value ?? string.Empty;
		MatchValue = bool.Parse(root.Element("MatchValue")?.Value ?? "false");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Tag { get; private set; }
	public string Value { get; private set; }
	public bool MatchValue { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "removemagictag"),
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value)),
			new XElement("MatchValue", MatchValue)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return MagicTagEffect.IsCompatibleWithTrigger(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null)
		{
			return null;
		}

		target.RemoveAllEffects<IMagicTagEffect>(x =>
			x.Tag.EqualTo(Tag) && (!MatchValue || x.Value.EqualTo(Value)), true);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RemoveMagicTagEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3tag <key>#0 - sets the tag key
	#3value <value>#0 - sets the optional value match
	#3matchvalue#0 - toggles requiring the value to match too";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "key":
				Tag = command.SafeRemainingArgument;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will remove magic tags with key {Tag.ColourName()}.");
				return true;
			case "value":
				Value = command.SafeRemainingArgument;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will match the magic tag value {Value.ColourValue()}.");
				return true;
			case "matchvalue":
				MatchValue = !MatchValue;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {MatchValue.NowNoLonger()} require the value to match.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Remove Magic Tag",
			("Tag", Tag.ColourName()),
			("Value", MatchValue ? Value.ColourValue() : "any".ColourValue())
		);
	}
}

public class ItemDamageEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("itemdamage", (root, spell) => new ItemDamageEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("itemdamage", BuilderFactory,
			"Inflicts normal item damage on target items",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new ItemDamageEffect(new XElement("Effect",
			new XAttribute("type", "itemdamage"),
			new XElement("DamageFormula", new XCData("power * 5")),
			new XElement("PainFormula", new XCData("0")),
			new XElement("StunFormula", new XCData("0")),
			new XElement("DamageType", (int)DamageType.Crushing)
		), spell), string.Empty);
	}

	protected ItemDamageEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		DamageFormula = new Expression(root.Element("DamageFormula")?.Value ?? "power * 5");
		PainFormula = new Expression(root.Element("PainFormula")?.Value ?? "0");
		StunFormula = new Expression(root.Element("StunFormula")?.Value ?? "0");
		DamageType = (DamageType)int.Parse(root.Element("DamageType")?.Value ?? ((int)DamageType.Crushing).ToString());
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public Expression DamageFormula { get; private set; }
	public Expression PainFormula { get; private set; }
	public Expression StunFormula { get; private set; }
	public DamageType DamageType { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "itemdamage"),
			new XElement("DamageFormula", new XCData(DamageFormula.OriginalExpression)),
			new XElement("PainFormula", new XCData(PainFormula.OriginalExpression)),
			new XElement("StunFormula", new XCData(StunFormula.OriginalExpression)),
			new XElement("DamageType", (int)DamageType)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "item" or "items" or "perceivable" or "perceivables";
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return IsCompatibleWithTrigger(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem item || item.Deleted)
		{
			return null;
		}

		foreach (var expression in new[] { DamageFormula, PainFormula, StunFormula })
		{
			expression.Parameters["power"] = (int)power;
			expression.Parameters["outcome"] = (int)outcome;
		}

		item.SufferDamage(new Damage
		{
			ActorOrigin = caster,
			ToolOrigin = null,
			DamageType = DamageType,
			DamageAmount = DamageFormula.EvaluateDouble(),
			PainAmount = PainFormula.EvaluateDouble(),
			ShockAmount = 0,
			StunAmount = StunFormula.EvaluateDouble(),
			PenetrationOutcome = Outcome.NotTested
		});
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ItemDamageEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3damage <formula>#0 - sets the item damage formula
	#3pain <formula>#0 - sets the pain formula
	#3stun <formula>#0 - sets the stun formula
	#3type <which>#0 - sets the damage type";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var which = command.PopSpeech().ToLowerInvariant();
		if (which is "damage" or "pain" or "stun")
		{
			var expression = new Expression(command.SafeRemainingArgument);
			if (expression.HasErrors())
			{
				actor.OutputHandler.Send(expression.Error);
				return false;
			}

			if (which == "damage")
			{
				DamageFormula = expression;
			}
			else if (which == "pain")
			{
				PainFormula = expression;
			}
			else
			{
				StunFormula = expression;
			}

			Spell.Changed = true;
			actor.OutputHandler.Send($"The {which} formula is now {expression.OriginalExpression.ColourCommand()}.");
			return true;
		}

		if (which is "type")
		{
			if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out var type))
			{
				actor.OutputHandler.Send($"That is not a valid damage type. Valid types are {Enum.GetValues<DamageType>().ListToColouredString()}.");
				return false;
			}

			DamageType = type;
			Spell.Changed = true;
			actor.OutputHandler.Send($"The damage type is now {DamageType.DescribeEnum().ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Item Damage",
			("Damage Type", DamageType.DescribeEnum().ColourValue()),
			("Damage Formula", DamageFormula.OriginalExpression.ColourCommand()),
			("Pain Formula", PainFormula.OriginalExpression.ColourCommand()),
			("Stun Formula", StunFormula.OriginalExpression.ColourCommand())
		);
	}
}

public class DestroyItemEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("destroyitem", (root, spell) => new DestroyItemEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("destroyitem", BuilderFactory,
			"Safely deletes target items",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => ItemDamageEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new DestroyItemEffect(new XElement("Effect",
			new XAttribute("type", "destroyitem"),
			new XElement("RespectPurgeWarnings", true)
		), spell), string.Empty);
	}

	protected DestroyItemEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		RespectPurgeWarnings = bool.Parse(root.Element("RespectPurgeWarnings")?.Value ?? "true");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool RespectPurgeWarnings { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "destroyitem"),
			new XElement("RespectPurgeWarnings", RespectPurgeWarnings)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return ItemDamageEffect.IsCompatibleWithTrigger(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem item || item.Deleted || (RespectPurgeWarnings && item.WarnBeforePurge))
		{
			return null;
		}

		item.Delete();
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new DestroyItemEffect(SaveToXml(), Spell);
	}

	public const string HelpText = "#3warnings#0 - toggles respecting purge-warning safeguards";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("warnings"))
		{
			actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
			return false;
		}

		RespectPurgeWarnings = !RespectPurgeWarnings;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will {RespectPurgeWarnings.NowNoLonger()} skip items with purge warnings.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Destroy Item",
			("Respect Purge Warnings", RespectPurgeWarnings.ToColouredString())
		);
	}
}

public class ItemEnchantEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("itemenchant", (root, spell) => new ItemEnchantEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("itemenchant", BuilderFactory,
			"Adds magical item descriptions, glow, and combat hooks",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => ItemDamageEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new ItemEnchantEffect(new XElement("Effect",
			new XAttribute("type", "itemenchant"),
			new XElement("SDescAddendum", new XCData("(enchanted)")),
			new XElement("DescAddendum", new XCData("It hums with a palpable magical aura.")),
			new XElement("Colour", "bold magenta"),
			new XElement("GlowLux", 0.0),
			new XElement("AttackCheckBonus", 0.0),
			new XElement("QualityBonus", 0.0),
			new XElement("DamageBonus", 0.0),
			new XElement("PainBonus", 0.0),
			new XElement("StunBonus", 0.0),
			new XElement("ArmourDamageReduction", 0.0),
			new XElement("ProjectileQualityBonus", 0.0),
			new XElement("ProjectileDamageBonus", 0.0),
			new XElement("ProjectilePainBonus", 0.0),
			new XElement("ProjectileStunBonus", 0.0),
			new XElement("ToolFitnessBonus", 0.0),
			new XElement("ToolSpeedMultiplier", 1.0),
			new XElement("ToolUsageMultiplier", 1.0),
			new XElement("PowerProductionMultiplier", 1.0),
			new XElement("PowerConsumptionMultiplier", 1.0),
			new XElement("FuelUseMultiplier", 1.0),
			new XElement("ItemEventType", -1),
			new XElement("ItemEventProg", 0L),
			new XElement("ApplicabilityProg", 0)
		), spell), string.Empty);
	}

	protected ItemEnchantEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		SDescAddendum = root.Element("SDescAddendum")?.Value ?? "(enchanted)";
		DescAddendum = root.Element("DescAddendum")?.Value ?? "It hums with a palpable magical aura.";
		Colour = Telnet.GetColour(root.Element("Colour")?.Value ?? "bold magenta") ?? Telnet.BoldMagenta;
		GlowLux = double.Parse(root.Element("GlowLux")?.Value ?? "0");
		AttackCheckBonus = double.Parse(root.Element("AttackCheckBonus")?.Value ?? "0");
		QualityBonus = double.Parse(root.Element("QualityBonus")?.Value ?? "0");
		DamageBonus = double.Parse(root.Element("DamageBonus")?.Value ?? "0");
		PainBonus = double.Parse(root.Element("PainBonus")?.Value ?? "0");
		StunBonus = double.Parse(root.Element("StunBonus")?.Value ?? "0");
		ArmourDamageReduction = double.Parse(root.Element("ArmourDamageReduction")?.Value ?? "0");
		ProjectileQualityBonus = double.Parse(root.Element("ProjectileQualityBonus")?.Value ?? "0");
		ProjectileDamageBonus = double.Parse(root.Element("ProjectileDamageBonus")?.Value ?? "0");
		ProjectilePainBonus = double.Parse(root.Element("ProjectilePainBonus")?.Value ?? "0");
		ProjectileStunBonus = double.Parse(root.Element("ProjectileStunBonus")?.Value ?? "0");
		ToolFitnessBonus = double.Parse(root.Element("ToolFitnessBonus")?.Value ?? "0");
		ToolSpeedMultiplier = double.Parse(root.Element("ToolSpeedMultiplier")?.Value ?? "1");
		ToolUsageMultiplier = double.Parse(root.Element("ToolUsageMultiplier")?.Value ?? "1");
		PowerProductionMultiplier = double.Parse(root.Element("PowerProductionMultiplier")?.Value ?? "1");
		PowerConsumptionMultiplier = double.Parse(root.Element("PowerConsumptionMultiplier")?.Value ?? "1");
		FuelUseMultiplier = double.Parse(root.Element("FuelUseMultiplier")?.Value ?? "1");
		var eventValue = int.Parse(root.Element("ItemEventType")?.Value ?? "-1");
		ItemEventType = eventValue < 0 ? null : (EventType)eventValue;
		ItemEventProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ItemEventProg")?.Value ?? "0"));
		ApplicabilityProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ApplicabilityProg")?.Value ?? "0"));
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string SDescAddendum { get; private set; }
	public string DescAddendum { get; private set; }
	public ANSIColour Colour { get; private set; }
	public double GlowLux { get; private set; }
	public double AttackCheckBonus { get; private set; }
	public double QualityBonus { get; private set; }
	public double DamageBonus { get; private set; }
	public double PainBonus { get; private set; }
	public double StunBonus { get; private set; }
	public double ArmourDamageReduction { get; private set; }
	public double ProjectileQualityBonus { get; private set; }
	public double ProjectileDamageBonus { get; private set; }
	public double ProjectilePainBonus { get; private set; }
	public double ProjectileStunBonus { get; private set; }
	public double ToolFitnessBonus { get; private set; }
	public double ToolSpeedMultiplier { get; private set; }
	public double ToolUsageMultiplier { get; private set; }
	public double PowerProductionMultiplier { get; private set; }
	public double PowerConsumptionMultiplier { get; private set; }
	public double FuelUseMultiplier { get; private set; }
	public EventType? ItemEventType { get; private set; }
	public IFutureProg? ItemEventProg { get; private set; }
	public IFutureProg? ApplicabilityProg { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "itemenchant"),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("Colour", Colour.Name),
			new XElement("GlowLux", GlowLux),
			new XElement("AttackCheckBonus", AttackCheckBonus),
			new XElement("QualityBonus", QualityBonus),
			new XElement("DamageBonus", DamageBonus),
			new XElement("PainBonus", PainBonus),
			new XElement("StunBonus", StunBonus),
			new XElement("ArmourDamageReduction", ArmourDamageReduction),
			new XElement("ProjectileQualityBonus", ProjectileQualityBonus),
			new XElement("ProjectileDamageBonus", ProjectileDamageBonus),
			new XElement("ProjectilePainBonus", ProjectilePainBonus),
			new XElement("ProjectileStunBonus", ProjectileStunBonus),
			new XElement("ToolFitnessBonus", ToolFitnessBonus),
			new XElement("ToolSpeedMultiplier", ToolSpeedMultiplier),
			new XElement("ToolUsageMultiplier", ToolUsageMultiplier),
			new XElement("PowerProductionMultiplier", PowerProductionMultiplier),
			new XElement("PowerConsumptionMultiplier", PowerConsumptionMultiplier),
			new XElement("FuelUseMultiplier", FuelUseMultiplier),
			new XElement("ItemEventType", ItemEventType.HasValue ? (int)ItemEventType.Value : -1),
			new XElement("ItemEventProg", ItemEventProg?.Id ?? 0L),
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return ItemDamageEffect.IsCompatibleWithTrigger(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is IGameItem
			? new SpellItemEnchantmentEffect(target, parent, SDescAddendum, DescAddendum, Colour, GlowLux,
				AttackCheckBonus, QualityBonus, DamageBonus, PainBonus, StunBonus, ArmourDamageReduction,
				ProjectileQualityBonus, ProjectileDamageBonus, ProjectilePainBonus, ProjectileStunBonus,
				ToolFitnessBonus, ToolSpeedMultiplier, ToolUsageMultiplier, PowerProductionMultiplier,
				PowerConsumptionMultiplier, FuelUseMultiplier, ItemEventType, ItemEventProg, ApplicabilityProg)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ItemEnchantEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3sdesc <text>#0 - sets the short-description addendum
	#3desc <text>#0 - sets the full-description addendum
	#3colour <colour>#0 - sets the addendum colour
	#3glow <lux>#0 - sets glow lux
	#3attack <bonus>#0 - sets weapon attack check bonus
	#3quality <bonus>#0 - sets virtual weapon quality bonus
	#3damage <bonus>#0 - sets weapon damage bonus
	#3pain <bonus>#0 - sets weapon pain bonus
	#3stun <bonus>#0 - sets weapon stun bonus
	#3armour <amount>#0 - sets armour damage reduction
	#3projectilequality <bonus>#0 - sets projectile quality bonus
	#3projectiledamage <bonus>#0 - sets projectile damage bonus
	#3projectilepain <bonus>#0 - sets projectile pain bonus
	#3projectilestun <bonus>#0 - sets projectile stun bonus
	#3toolfitness <bonus>#0 - sets craft tool fitness bonus
	#3toolspeed <multiplier>#0 - sets craft phase speed multiplier
	#3toolusage <multiplier>#0 - sets tool durability usage multiplier
	#3powerproduction <multiplier>#0 - sets powered-item production multiplier
	#3powerconsumption <multiplier>#0 - sets powered-item consumption multiplier
	#3fuelusage <multiplier>#0 - sets powered-item fuel usage multiplier
	#3event <event|none>#0 - sets the item event this enchantment listens for
	#3eventprog <prog|none>#0 - sets the item event callback prog
	#3prog <prog|none>#0 - gates whether the enchantment applies";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var which = command.PopSpeech().ToLowerInvariant();
		switch (which)
		{
			case "sdesc":
				SDescAddendum = command.SafeRemainingArgument;
				break;
			case "desc":
				DescAddendum = command.SafeRemainingArgument;
				break;
			case "colour":
			case "color":
				var colour = Telnet.GetColour(command.SafeRemainingArgument);
				if (colour is null)
				{
					actor.OutputHandler.Send($"That is not a valid colour. The options are:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
					return false;
				}

				Colour = colour;
				break;
			case "glow":
			case "attack":
			case "quality":
			case "damage":
			case "pain":
			case "stun":
			case "armour":
			case "projectilequality":
			case "projectiledamage":
			case "projectilepain":
			case "projectilestun":
			case "toolfitness":
			case "toolspeed":
			case "toolusage":
			case "powerproduction":
			case "powerconsumption":
			case "fuelusage":
				if (!double.TryParse(command.SafeRemainingArgument, out var value))
				{
					actor.OutputHandler.Send("You must enter a valid number.");
					return false;
				}

				SetNumeric(which, value);
				break;
			case "event":
				return BuildingCommandEvent(actor, command);
			case "eventprog":
				return BuildingCommandEventProg(actor, command);
			case "prog":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					ApplicabilityProg = null;
					break;
				}

				ApplicabilityProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
					ProgVariableTypes.Boolean,
					[
						[ProgVariableTypes.Item],
						[ProgVariableTypes.Item, ProgVariableTypes.Character],
						[ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Perceivable]
					]).LookupProg();
				if (ApplicabilityProg is null)
				{
					return false;
				}

				break;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The item enchantment has been updated.");
		return true;
	}

	private void SetNumeric(string which, double value)
	{
		switch (which)
		{
			case "glow":
				GlowLux = value;
				return;
			case "attack":
				AttackCheckBonus = value;
				return;
			case "quality":
				QualityBonus = value;
				return;
			case "damage":
				DamageBonus = value;
				return;
			case "pain":
				PainBonus = value;
				return;
			case "stun":
				StunBonus = value;
				return;
			case "armour":
				ArmourDamageReduction = value;
				return;
			case "projectilequality":
				ProjectileQualityBonus = value;
				return;
			case "projectiledamage":
				ProjectileDamageBonus = value;
				return;
			case "projectilepain":
				ProjectilePainBonus = value;
				return;
			case "projectilestun":
				ProjectileStunBonus = value;
				return;
			case "toolfitness":
				ToolFitnessBonus = value;
				return;
			case "toolspeed":
				ToolSpeedMultiplier = Math.Max(0.01, value);
				return;
			case "toolusage":
				ToolUsageMultiplier = Math.Max(0.0, value);
				return;
			case "powerproduction":
				PowerProductionMultiplier = Math.Max(0.0, value);
				return;
			case "powerconsumption":
				PowerConsumptionMultiplier = Math.Max(0.0, value);
				return;
			case "fuelusage":
				FuelUseMultiplier = Math.Max(0.0, value);
				return;
		}
	}

	private bool BuildingCommandEvent(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear"))
		{
			ItemEventType = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This enchantment no longer listens for item events.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out EventType value))
		{
			actor.OutputHandler.Send("That is not a valid event type.");
			return false;
		}

		ItemEventType = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This enchantment now listens for {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEventProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear"))
		{
			ItemEventProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This enchantment no longer invokes an item event prog.");
			return true;
		}

		ItemEventProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void,
			[
				[ProgVariableTypes.Item],
				[ProgVariableTypes.Item, ProgVariableTypes.Text]
			]).LookupProg();
		if (ItemEventProg is null)
		{
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This enchantment now invokes {ItemEventProg.MXPClickableFunctionName()} for matching item events.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Item Enchant",
			("SDesc Addendum", SDescAddendum.Colour(Colour)),
			("Desc Addendum", DescAddendum.Colour(Colour)),
			("Glow Lux", GlowLux.ToString("N2", actor).ColourValue()),
			("Attack Check", AttackCheckBonus.ToBonusString(actor)),
			("Item Damage", $"Quality {QualityBonus.ToBonusString(actor)}, Damage {DamageBonus.ToBonusString(actor)}, Pain {PainBonus.ToBonusString(actor)}, Stun {StunBonus.ToBonusString(actor)}"),
			("Armour Reduction", ArmourDamageReduction.ToString("N2", actor).ColourValue()),
			("Projectile Damage", $"Quality {ProjectileQualityBonus.ToBonusString(actor)}, Damage {ProjectileDamageBonus.ToBonusString(actor)}, Pain {ProjectilePainBonus.ToBonusString(actor)}, Stun {ProjectileStunBonus.ToBonusString(actor)}"),
			("Tool Use", $"Fitness {ToolFitnessBonus.ToBonusString(actor)}, Speed x{ToolSpeedMultiplier.ToString("N2", actor).ColourValue()}, Usage x{ToolUsageMultiplier.ToString("N2", actor).ColourValue()}"),
			("Power", $"Production x{PowerProductionMultiplier.ToString("N2", actor).ColourValue()}, Consumption x{PowerConsumptionMultiplier.ToString("N2", actor).ColourValue()}, Fuel x{FuelUseMultiplier.ToString("N2", actor).ColourValue()}"),
			("Item Event", ItemEventType?.DescribeEnum().ColourValue() ?? "None".ColourError()),
			("Item Event Prog", ItemEventProg?.MXPClickableFunctionName() ?? "None".ColourError()),
			("Applicability Prog", ApplicabilityProg?.MXPClickableFunctionName() ?? "None".ColourError())
		);
	}
}

public class CorpseMarkEffect : MagicTagEffect
{
	public static new void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("corpsemark", (root, spell) => new CorpseMarkEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("corpsemark", BuilderFactory,
			"Applies a magic metadata tag to a corpse",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CorpseMarkEffect(new XElement("Effect",
			new XAttribute("type", "corpsemark"),
			new XElement("Tag", new XCData("corpsemark")),
			new XElement("Value", new XCData("")),
			new XElement("ReplaceExisting", true)
		), spell), string.Empty);
	}

	protected CorpseMarkEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
	}

	public override XElement SaveToXml()
	{
		var xml = base.SaveToXml();
		xml.SetAttributeValue("type", "corpsemark");
		return xml;
	}

	public override bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return types.TargetTypes == "item";
	}

	public override IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is IGameItem item && item.GetItemType<ICorpse>() is not null
			? base.GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters)
			: null;
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new CorpseMarkEffect(SaveToXml(), Spell);
	}
}

public class CorpsePreserveEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("corpsepreserve",
			(root, spell) => new CorpsePreserveEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("corpsepreserve", BuilderFactory,
			"Halts corpse decay while the spell lasts",
			string.Empty,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CorpsePreserveEffect(new XElement("Effect", new XAttribute("type", "corpsepreserve")), spell),
			string.Empty);
	}

	protected CorpsePreserveEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "corpsepreserve"));
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return types.TargetTypes == "item";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is IGameItem item && item.GetItemType<ICorpse>() is not null
			? new SpellCorpsePreservationEffect(target, parent)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CorpsePreserveEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command) => false;
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Corpse Preserve");
}

public class CorpseConsumeEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("corpseconsume", (root, spell) => new CorpseConsumeEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("corpseconsume", BuilderFactory,
			"Consumes and deletes a corpse",
			string.Empty,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CorpseConsumeEffect(new XElement("Effect", new XAttribute("type", "corpseconsume")), spell),
			string.Empty);
	}

	protected CorpseConsumeEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "corpseconsume"));
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger types) => types.TargetTypes == "item";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is IGameItem item && item.GetItemType<ICorpse>() is not null && !item.Deleted)
		{
			item.Delete();
		}

		return null;
	}

	public IMagicSpellEffectTemplate Clone() => new CorpseConsumeEffect(SaveToXml(), Spell);
	public bool BuildingCommand(ICharacter actor, StringStack command) => false;
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Corpse Consume");
}

public class CorpseSpawnEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("corpsespawn", (root, spell) => new CorpseSpawnEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("corpsespawn", BuilderFactory,
			"Spawns configured NPCs or items from a corpse",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CorpseSpawnEffect(new XElement("Effect",
			new XAttribute("type", "corpsespawn"),
			new XElement("NPCPrototypeId", 0),
			new XElement("ItemPrototypeId", 0),
			new XElement("Quantity", 1),
			new XElement("ConsumeCorpse", false)
		), spell), string.Empty);
	}

	protected CorpseSpawnEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		NPCPrototypeId = long.Parse(root.Element("NPCPrototypeId")?.Value ?? "0");
		ItemPrototypeId = long.Parse(root.Element("ItemPrototypeId")?.Value ?? "0");
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		ConsumeCorpse = bool.Parse(root.Element("ConsumeCorpse")?.Value ?? "false");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public long NPCPrototypeId { get; private set; }
	public long ItemPrototypeId { get; private set; }
	public int Quantity { get; private set; }
	public bool ConsumeCorpse { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "corpsespawn"),
			new XElement("NPCPrototypeId", NPCPrototypeId),
			new XElement("ItemPrototypeId", ItemPrototypeId),
			new XElement("Quantity", Quantity),
			new XElement("ConsumeCorpse", ConsumeCorpse)
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger types) => types.TargetTypes == "item";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem item || item.GetItemType<ICorpse>() is null)
		{
			return null;
		}

		var cell = item.TrueLocations.FirstOrDefault() ?? caster.Location;
		if (NPCPrototypeId > 0 && Gameworld.NpcTemplates.Get(NPCPrototypeId) is INPCTemplate template)
		{
            var ch = template.CreateNewCharacter(cell);
            Gameworld.Add(ch, true);
            ch.RoomLayer = item.RoomLayer;
            template.ApplyTemplateLoadAdditions(ch);
            template.OnLoadProg?.Execute(ch);
            if (ch.Location.IsSwimmingLayer(ch.RoomLayer) && ch.Race.CanSwim)
			{
				ch.PositionState = PositionSwimming.Instance;
			}
			else if (ch.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && ch.CanFly().Truth)
			{
				ch.PositionState = PositionFlying.Instance;
			}

			ch.Location.Login(ch);
			ch.HandleEvent(EventType.NPCOnGameLoadFinished, ch);
		}

		if (ItemPrototypeId > 0 && Gameworld.ItemProtos.Get(ItemPrototypeId) is IGameItemProto proto)
		{
			foreach (var newItem in proto.CreateNew(caster, null!, Quantity, string.Empty))
			{
				newItem.SetOwner(caster);
				cell.Insert(newItem, true);
				newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
				newItem.Login();
			}
		}

		if (ConsumeCorpse && !item.Deleted)
		{
			item.Delete();
		}

		return null;
	}

	public IMagicSpellEffectTemplate Clone() => new CorpseSpawnEffect(SaveToXml(), Spell);

	public const string HelpText = @"You can use the following options with this effect:

	#3npc <proto|none>#0 - sets the NPC prototype to spawn
	#3item <proto|none>#0 - sets the item prototype to spawn
	#3quantity <##>#0 - sets item quantity
	#3consume#0 - toggles consuming the corpse afterwards";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "npc":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					NPCPrototypeId = 0;
					break;
				}

				var npc = Gameworld.NpcTemplates.GetByIdOrName(command.SafeRemainingArgument);
				if (npc is null)
				{
					actor.OutputHandler.Send($"There is no NPC prototype identified by {command.SafeRemainingArgument.ColourCommand()}.");
					return false;
				}

				NPCPrototypeId = npc.Id;
				break;
			case "item":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					ItemPrototypeId = 0;
					break;
				}

				var item = Gameworld.ItemProtos.GetByIdOrName(command.SafeRemainingArgument);
				if (item is null)
				{
					actor.OutputHandler.Send($"There is no item prototype identified by {command.SafeRemainingArgument.ColourCommand()}.");
					return false;
				}

				ItemPrototypeId = item.Id;
				break;
			case "quantity":
				if (!int.TryParse(command.SafeRemainingArgument, out var quantity) || quantity < 1)
				{
					actor.OutputHandler.Send("You must enter a positive integer quantity.");
					return false;
				}

				Quantity = quantity;
				break;
			case "consume":
				ConsumeCorpse = !ConsumeCorpse;
				break;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The corpse spawn effect has been updated.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Corpse Spawn",
			("NPC Prototype", $"#{NPCPrototypeId.ToString("N0", actor).ColourValue()}"),
			("Item Prototype", $"#{ItemPrototypeId.ToString("N0", actor).ColourValue()}"),
			("Quantity", Quantity.ToString("N0", actor).ColourValue()),
			("Consume Corpse", ConsumeCorpse.ToColouredString())
		);
	}
}

public class PortalSpellEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("portal", (root, spell) => new PortalSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("portal", BuilderFactory,
			"Creates a transient paired magical portal exit",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new PortalSpellEffect(new XElement("Effect",
			new XAttribute("type", "portal"),
			new XElement("Verb", new XCData("enter")),
			new XElement("OutboundKeyword", new XCData("portal")),
			new XElement("InboundKeyword", new XCData("portal")),
			new XElement("OutboundTarget", new XCData("a shimmering portal")),
			new XElement("InboundTarget", new XCData("a shimmering portal")),
			new XElement("OutboundDescription", new XCData("through")),
			new XElement("InboundDescription", new XCData("through")),
			new XElement("TimeMultiplier", 1.0),
			new XElement("AllowCrossZone", false),
			new XElement("AnchorTag", new XCData("portal-anchor")),
			new XElement("AnchorValue", new XCData("")),
			new XElement("DestinationProg", 0)
		), spell), string.Empty);
	}

	protected PortalSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Verb = root.Element("Verb")?.Value ?? "enter";
		OutboundKeyword = root.Element("OutboundKeyword")?.Value ?? "portal";
		InboundKeyword = root.Element("InboundKeyword")?.Value ?? "portal";
		OutboundTarget = root.Element("OutboundTarget")?.Value ?? "a shimmering portal";
		InboundTarget = root.Element("InboundTarget")?.Value ?? "a shimmering portal";
		OutboundDescription = root.Element("OutboundDescription")?.Value ?? "through";
		InboundDescription = root.Element("InboundDescription")?.Value ?? "through";
		TimeMultiplier = double.Parse(root.Element("TimeMultiplier")?.Value ?? "1.0");
		AllowCrossZone = bool.Parse(root.Element("AllowCrossZone")?.Value ?? "false");
		AnchorTag = root.Element("AnchorTag")?.Value ?? "portal-anchor";
		AnchorValue = root.Element("AnchorValue")?.Value ?? string.Empty;
		DestinationProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("DestinationProg")?.Value ?? "0"));
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Verb { get; private set; }
	public string OutboundKeyword { get; private set; }
	public string InboundKeyword { get; private set; }
	public string OutboundTarget { get; private set; }
	public string InboundTarget { get; private set; }
	public string OutboundDescription { get; private set; }
	public string InboundDescription { get; private set; }
	public double TimeMultiplier { get; private set; }
	public bool AllowCrossZone { get; private set; }
	public string AnchorTag { get; private set; }
	public string AnchorValue { get; private set; }
	public IFutureProg? DestinationProg { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "portal"),
			new XElement("Verb", new XCData(Verb)),
			new XElement("OutboundKeyword", new XCData(OutboundKeyword)),
			new XElement("InboundKeyword", new XCData(InboundKeyword)),
			new XElement("OutboundTarget", new XCData(OutboundTarget)),
			new XElement("InboundTarget", new XCData(InboundTarget)),
			new XElement("OutboundDescription", new XCData(OutboundDescription)),
			new XElement("InboundDescription", new XCData(InboundDescription)),
			new XElement("TimeMultiplier", TimeMultiplier),
			new XElement("AllowCrossZone", AllowCrossZone),
			new XElement("AnchorTag", new XCData(AnchorTag)),
			new XElement("AnchorValue", new XCData(AnchorValue)),
			new XElement("DestinationProg", DestinationProg?.Id ?? 0)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "room" or "rooms" or "character" or "perceivable" or "character&room";
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var source = caster.Location;
		var destination = target as ICell ??
		                  additionalParameters.FirstOrDefault(x => x.ParameterName.EqualTo("room"))?.Item as ICell ??
		                  AnchorDestination(caster);
		if (source is null || destination is null || ReferenceEquals(source, destination))
		{
			return null;
		}

		if (!AllowCrossZone && !ReferenceEquals(source.Zone, destination.Zone))
		{
			return null;
		}

		if (!source.CanInteractPlanar(destination, PlanarInteractionKind.Magic))
		{
			return null;
		}

		if (DestinationProg is not null && !DestinationProg.ExecuteBool(caster, source, destination))
		{
			return null;
		}

		return new SpellPortalEffect(target ?? destination, parent, source, destination, Verb, OutboundKeyword, InboundKeyword,
			OutboundTarget, InboundTarget, OutboundDescription, InboundDescription, TimeMultiplier);
	}

	private ICell? AnchorDestination(ICharacter caster)
	{
		var roomAnchor = Gameworld.Cells
			.Where(x => x != caster.Location)
			.Where(x => x.EffectsOfType<IMagicTagEffect>(tag =>
				tag.Caster?.Id == caster.Id &&
				tag.Tag.EqualTo(AnchorTag) &&
				(string.IsNullOrEmpty(AnchorValue) || tag.Value.EqualTo(AnchorValue))).Any())
			.OrderBy(x => x.Id)
			.FirstOrDefault();
		if (roomAnchor is not null)
		{
			return roomAnchor;
		}

		return Gameworld.Items
			.Where(x => x.Location is not null && x.Location != caster.Location)
			.Where(x => x.EffectsOfType<IMagicTagEffect>(tag =>
				tag.Caster?.Id == caster.Id &&
				tag.Tag.EqualTo(AnchorTag) &&
				(string.IsNullOrEmpty(AnchorValue) || tag.Value.EqualTo(AnchorValue))).Any())
			.OrderBy(x => x.Id)
			.FirstOrDefault()
			?.Location;
	}

	public IMagicSpellEffectTemplate Clone() => new PortalSpellEffect(SaveToXml(), Spell);

	public const string HelpText = @"You can use the following options with this effect:

	#3verb <verb>#0 - sets the movement verb, e.g. enter
	#3outkey <keyword>#0 - sets the outbound keyword
	#3inkey <keyword>#0 - sets the inbound keyword
	#3outtarget <text>#0 - sets the outbound target text
	#3intarget <text>#0 - sets the inbound target text
	#3outdesc <text>#0 - sets the outbound movement preposition
	#3indesc <text>#0 - sets the inbound movement preposition
	#3speed <multiplier>#0 - sets the movement time multiplier
	#3crosszone#0 - toggles cross-zone portals
	#3anchor <tag> [value]#0 - sets the caster-owned tag anchor lookup
	#3prog <prog|none>#0 - gates valid destinations";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var which = command.PopSpeech().ToLowerInvariant();
		switch (which)
		{
			case "verb":
				Verb = command.SafeRemainingArgument;
				break;
			case "outkey":
				OutboundKeyword = command.SafeRemainingArgument;
				break;
			case "inkey":
				InboundKeyword = command.SafeRemainingArgument;
				break;
			case "outtarget":
				OutboundTarget = command.SafeRemainingArgument;
				break;
			case "intarget":
				InboundTarget = command.SafeRemainingArgument;
				break;
			case "outdesc":
				OutboundDescription = command.SafeRemainingArgument;
				break;
			case "indesc":
				InboundDescription = command.SafeRemainingArgument;
				break;
			case "speed":
				if (!double.TryParse(command.SafeRemainingArgument, out var speed) || speed <= 0.0)
				{
					actor.OutputHandler.Send("You must enter a positive speed multiplier.");
					return false;
				}

				TimeMultiplier = speed;
				break;
			case "crosszone":
				AllowCrossZone = !AllowCrossZone;
				break;
			case "anchor":
				AnchorTag = command.PopSpeech();
				AnchorValue = command.SafeRemainingArgument;
				break;
			case "prog":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					DestinationProg = null;
					break;
				}

				DestinationProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
					ProgVariableTypes.Boolean,
					[
						[ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location]
					]).LookupProg();
				if (DestinationProg is null)
				{
					return false;
				}

				break;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The portal effect has been updated.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Portal",
			("Verb", Verb.ColourCommand()),
			("Outbound Keyword", OutboundKeyword.ColourCommand()),
			("Inbound Keyword", InboundKeyword.ColourCommand()),
			("Outbound Target", OutboundTarget.ColourCommand()),
			("Inbound Target", InboundTarget.ColourCommand()),
			("Outbound Description", OutboundDescription.ColourCommand()),
			("Inbound Description", InboundDescription.ColourCommand()),
			("Time Multiplier", TimeMultiplier.ToString("N2", actor).ColourValue()),
			("Allow Cross Zone", AllowCrossZone.ToColouredString()),
			("Anchor", $"{AnchorTag.ColourName()}={AnchorValue.ColourValue()}"),
			("Destination Prog", DestinationProg?.MXPClickableFunctionName() ?? "None".ColourError())
		);
	}
}

public class ForceCommandEffect : IMagicSpellEffectTemplate
{
	private static readonly HashSet<string> BlockedRoots = new(StringComparer.InvariantCultureIgnoreCase)
	{
		"account", "admin", "authorize", "builder", "chargen", "delete", "email", "force", "password", "prog",
		"purge", "quit", "reboot", "reload", "save", "shutdown", "snoop", "switch", "user", "wiz"
	};

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("forcecommand", (root, spell) => new ForceCommandEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("forcecommand", BuilderFactory,
			"Forces target characters to execute a non-staff command",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "character" or "characters")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new ForceCommandEffect(new XElement("Effect",
			new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData("look"))
		), spell), string.Empty);
	}

	protected ForceCommandEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Command = root.Element("Command")?.Value ?? "look";
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Command { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData(Command))
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger types) => types.TargetTypes is "character" or "characters";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter character || character.AffectedBy<IIgnoreForceEffect>() ||
		    !IsCommandAllowed(character, Command))
		{
			return null;
		}

		Gameworld.SystemMessage(new EmoteOutput(new Emote(
			$"@ magically compel|compels $0 to execute '{Command}'", caster, character),
			flags: OutputFlags.WizOnly), true);
		CommandExecutionGuards.ExecuteForcedCommand(character, Command);
		return null;
	}

	private static bool IsCommandAllowed(ICharacter character, string commandText)
	{
		var stack = new StringStack(commandText);
		if (stack.IsFinished)
		{
			return false;
		}

		var root = stack.PopSpeech();
		if (BlockedRoots.Contains(root))
		{
			return false;
		}

		var lookup = commandText;
		var command = character.CommandTree.Commands.LocateCommand(character, ref lookup);
		return command is null || command.PermissionRequired < PermissionLevel.JuniorAdmin;
	}

	public IMagicSpellEffectTemplate Clone() => new ForceCommandEffect(SaveToXml(), Spell);

	public const string HelpText = "#3command <command>#0 - sets the command the target will execute. Staff PCs execute in temporary mortal mode.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("command"))
		{
			actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
			return false;
		}

		Command = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will force the target to execute {Command.ColourCommand()}.");
		return true;
	}

	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Force Command",
		("Command", Command.ColourCommand())
	);
}

public class SubjectiveDescriptionEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("subjectivedesc",
			(root, spell) => new SubjectiveDescriptionEffect(root, spell, DescriptionType.Full));
		SpellEffectFactory.RegisterLoadTimeFactory("subjectivesdesc",
			(root, spell) => new SubjectiveDescriptionEffect(root, spell, DescriptionType.Short));
		SpellEffectFactory.RegisterBuilderFactory("subjectivedesc",
			(commands, spell) => BuilderFactory(commands, spell, DescriptionType.Full, "subjectivedesc"),
			"Overrides the full description for scoped viewers",
			HelpText,
			false,
			true,
			CompatibleTriggers());
		SpellEffectFactory.RegisterBuilderFactory("subjectivesdesc",
			(commands, spell) => BuilderFactory(commands, spell, DescriptionType.Short, "subjectivesdesc"),
			"Overrides the short description for scoped viewers",
			HelpText,
			false,
			true,
			CompatibleTriggers());
	}

	private static string[] CompatibleTriggers()
	{
		return SpellTriggerFactory.MagicTriggerTypes
			.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "character" or "item" or "perceivable")
			.ToArray();
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell, DescriptionType type, string effectType)
	{
		return (new SubjectiveDescriptionEffect(new XElement("Effect",
			new XAttribute("type", effectType),
			new XElement("Description", new XCData(type == DescriptionType.Short ? "someone altered by magic" : "They appear different through magic.")),
			new XElement("FixedViewer", true),
			new XElement("AudienceScope", IllusionAudienceScope.Caster.ToString()),
			new XElement("ClanId", 0L),
			new XElement("ViewerProg", 0L),
			new XElement("ApplicabilityProg", 0),
			new XElement("Priority", 0),
			new XElement("OverrideKey", new XCData(string.Empty))
		), spell, type), string.Empty);
	}

	protected SubjectiveDescriptionEffect(XElement root, IMagicSpell spell, DescriptionType type)
	{
		Spell = spell;
		DescriptionType = type;
		EffectType = root.Attribute("type")?.Value ?? (type == DescriptionType.Short ? "subjectivesdesc" : "subjectivedesc");
		Description = root.Element("Description")?.Value ?? string.Empty;
		FixedViewer = bool.Parse(root.Element("FixedViewer")?.Value ?? "true");
		AudienceScope = ParseAudienceScope(root.Element("AudienceScope")?.Value, FixedViewer);
		FixedViewer = AudienceScope == IllusionAudienceScope.Caster;
		ClanId = long.TryParse(root.Element("ClanId")?.Value, out var clanId) && clanId > 0L ? clanId : null;
		ViewerProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ViewerProg")?.Value ?? "0"));
		ApplicabilityProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ApplicabilityProg")?.Value ?? "0"));
		Priority = int.Parse(root.Element("Priority")?.Value ?? "0");
		OverrideKey = root.Element("OverrideKey")?.Value ?? string.Empty;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DescriptionType DescriptionType { get; }
	public string EffectType { get; }
	public string Description { get; private set; }
	public bool FixedViewer { get; private set; }
	public IllusionAudienceScope AudienceScope { get; private set; }
	public long? ClanId { get; private set; }
	public IFutureProg? ViewerProg { get; private set; }
	public IFutureProg? ApplicabilityProg { get; private set; }
	public int Priority { get; private set; }
	public string OverrideKey { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", EffectType),
			new XElement("Description", new XCData(Description)),
			new XElement("FixedViewer", FixedViewer),
			new XElement("AudienceScope", AudienceScope.ToString()),
			new XElement("ClanId", ClanId ?? 0L),
			new XElement("ViewerProg", ViewerProg?.Id ?? 0L),
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("Priority", Priority),
			new XElement("OverrideKey", new XCData(OverrideKey))
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger types) => types.TargetTypes is "character" or "item" or "perceivable";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is not null
			? new SpellSubjectiveDescriptionEffect(target, parent, DescriptionType, Description,
				AudienceScope, caster.Id, target.Id, ClanId, ApplicabilityProg, ViewerProg, Priority, OverrideKey)
			: null;
	}

	public IMagicSpellEffectTemplate Clone() => new SubjectiveDescriptionEffect(SaveToXml(), Spell, DescriptionType);

	public const string HelpText = @"You can use the following options with this effect:

	#3description <text>#0 - sets the replacement description
	#3fixedviewer#0 - toggles whether only the caster sees it
	#3scope <caster|target|everyone|samecell|samezone|party|clan>#0 - sets who can see the illusion
	#3clan <which|none>#0 - sets the clan for clan-scoped illusions
	#3viewerprog <prog|none>#0 - gates individual viewers
	#3prog <prog|none>#0 - gates whether the override applies
	#3priority <number>#0 - sets override priority; higher priorities win
	#3key <text|none>#0 - sets an optional illusion/dispel key";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "description":
			case "desc":
			case "sdesc":
				Description = command.SafeRemainingArgument;
				break;
			case "fixedviewer":
				FixedViewer = !FixedViewer;
				AudienceScope = FixedViewer ? IllusionAudienceScope.Caster : IllusionAudienceScope.Everyone;
				break;
			case "scope":
				return BuildingCommandScope(actor, command);
			case "clan":
				return BuildingCommandClan(actor, command);
			case "viewerprog":
				return BuildingCommandViewerProg(actor, command);
			case "priority":
				if (!int.TryParse(command.SafeRemainingArgument, out var value))
				{
					actor.OutputHandler.Send("You must enter a valid whole number priority.");
					return false;
				}

				Priority = value;
				break;
			case "key":
			case "illusionkey":
				OverrideKey = command.SafeRemainingArgument.EqualTo("none")
					? string.Empty
					: command.SafeRemainingArgument;
				break;
			case "prog":
				if (command.SafeRemainingArgument.EqualTo("none"))
				{
					ApplicabilityProg = null;
					break;
				}

				ApplicabilityProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
					ProgVariableTypes.Boolean,
					[
						[ProgVariableTypes.Perceivable],
						[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver]
					]).LookupProg();
				if (ApplicabilityProg is null)
				{
					return false;
				}

				break;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The subjective description effect has been updated.");
		return true;
	}

	private bool BuildingCommandScope(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out IllusionAudienceScope value))
		{
			actor.OutputHandler.Send($"Valid illusion scopes are {Enum.GetValues<IllusionAudienceScope>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		AudienceScope = value;
		FixedViewer = AudienceScope == IllusionAudienceScope.Caster;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This subjective description illusion will now use the {AudienceScope.DescribeEnum().ColourValue()} audience scope.");
		return true;
	}

	private bool BuildingCommandClan(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "0"))
		{
			ClanId = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This subjective description illusion no longer has a clan audience restriction.");
			return true;
		}

		var clan = Gameworld.Clans.GetClan(command.SafeRemainingArgument);
		if (clan is null)
		{
			actor.OutputHandler.Send("There is no such clan.");
			return false;
		}

		ClanId = clan.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This subjective description illusion will now use {clan.FullName.ColourName()} for clan-scoped audiences.");
		return true;
	}

	private bool BuildingCommandViewerProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			ViewerProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This subjective description illusion no longer has a viewer FutureProg gate.");
			return true;
		}

		ViewerProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Perceivable],
				[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver],
				[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver, ProgVariableTypes.Character]
			]).LookupProg();
		if (ViewerProg is null)
		{
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This subjective description illusion now uses {ViewerProg.MXPClickableFunctionName()} as its viewer gate.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor,
			DescriptionType == DescriptionType.Short ? "Subjective SDesc" : "Subjective Description",
			("Fixed Viewer", FixedViewer.ToColouredString()),
			("Audience Scope", AudienceScope.DescribeEnum().ColourValue()),
			("Clan", ClanId.HasValue ? $"#{ClanId.Value.ToString("N0", actor)}".ColourValue() : "none".ColourError()),
			("Viewer Prog", ViewerProg?.MXPClickableFunctionName() ?? "None".ColourError()),
			("Priority", Priority.ToString("N0", actor).ColourValue()),
			("Override Key", string.IsNullOrWhiteSpace(OverrideKey) ? "none".ColourError() : OverrideKey.ColourValue()),
			("Applicability Prog", ApplicabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()),
			("Description", Description.ColourValue())
		);
	}

	private static IllusionAudienceScope ParseAudienceScope(string? text, bool fixedViewer)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return fixedViewer ? IllusionAudienceScope.Caster : IllusionAudienceScope.Everyone;
		}

		return text.TryParseEnum(out IllusionAudienceScope value)
			? value
			: fixedViewer
				? IllusionAudienceScope.Caster
				: IllusionAudienceScope.Everyone;
	}
}

public class PhantomIllusionEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("phantomillusion",
			(root, spell) => new PhantomIllusionEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("phantomillusion",
			(commands, spell) => BuilderFactory(commands, spell),
			"Adds non-interactive phantom text to room descriptions",
			HelpText,
			false,
			true,
			CompatibleTriggers());
	}

	private static string[] CompatibleTriggers()
	{
		return SpellTriggerFactory.MagicTriggerTypes
		                          .Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "room" or "rooms")
		                          .ToArray();
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new PhantomIllusionEffect(new XElement("Effect",
			new XAttribute("type", "phantomillusion"),
			new XElement("Text", new XCData("Something unreal shimmers here.")),
			new XElement("AudienceScope", IllusionAudienceScope.Everyone.ToString()),
			new XElement("ClanId", 0L),
			new XElement("ViewerProg", 0L),
			new XElement("Priority", 0),
			new XElement("IllusionKey", new XCData(string.Empty)),
			new XElement("Colour", "none")
		), spell), string.Empty);
	}

	private PhantomIllusionEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Text = root.Element("Text")?.Value ?? string.Empty;
		var scopeText = root.Element("AudienceScope")?.Value;
		AudienceScope = !string.IsNullOrWhiteSpace(scopeText) && scopeText.TryParseEnum(out IllusionAudienceScope scope)
			? scope
			: IllusionAudienceScope.Everyone;
		ClanId = long.TryParse(root.Element("ClanId")?.Value, out var clanId) && clanId > 0L ? clanId : null;
		ViewerProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ViewerProg")?.Value ?? "0"));
		Priority = int.Parse(root.Element("Priority")?.Value ?? "0");
		IllusionKey = root.Element("IllusionKey")?.Value ?? string.Empty;
		var colourText = root.Element("Colour")?.Value ?? "none";
		Colour = colourText.EqualTo("none") ? null : Telnet.GetColour(colourText);
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Text { get; private set; }
	public IllusionAudienceScope AudienceScope { get; private set; }
	public long? ClanId { get; private set; }
	public IFutureProg? ViewerProg { get; private set; }
	public int Priority { get; private set; }
	public string IllusionKey { get; private set; }
	public ANSIColour? Colour { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "phantomillusion"),
			new XElement("Text", new XCData(Text)),
			new XElement("AudienceScope", AudienceScope.ToString()),
			new XElement("ClanId", ClanId ?? 0L),
			new XElement("ViewerProg", ViewerProg?.Id ?? 0L),
			new XElement("Priority", Priority),
			new XElement("IllusionKey", new XCData(IllusionKey)),
			new XElement("Colour", Colour?.Name ?? "none")
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => types.TargetTypes is "room" or "rooms";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is ICell cell
			? new SpellPhantomIllusionEffect(cell, parent, Text, AudienceScope, caster.Id, cell.Id, ClanId, null,
				ViewerProg, Priority, IllusionKey, Colour)
			: null;
	}

	public IMagicSpellEffectTemplate Clone() => new PhantomIllusionEffect(SaveToXml(), Spell);

	public const string HelpText = @"You can use the following options with this effect:

	#3text <description>#0 - sets the phantom room text
	#3scope <caster|target|everyone|samecell|samezone|party|clan>#0 - sets who can see the illusion
	#3clan <which|none>#0 - sets the clan for clan-scoped illusions
	#3viewerprog <prog|none>#0 - gates individual viewers
	#3priority <number>#0 - sets stacking priority; higher priorities are shown first
	#3key <text|none>#0 - sets an optional illusion/dispel key
	#3colour <ansi|none>#0 - sets an optional ANSI colour for the phantom text";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "text":
			case "description":
			case "desc":
				Text = command.SafeRemainingArgument;
				break;
			case "scope":
				return BuildingCommandScope(actor, command);
			case "clan":
				return BuildingCommandClan(actor, command);
			case "viewerprog":
				return BuildingCommandViewerProg(actor, command);
			case "priority":
				if (!int.TryParse(command.SafeRemainingArgument, out var value))
				{
					actor.OutputHandler.Send("You must enter a valid whole number priority.");
					return false;
				}

				Priority = value;
				break;
			case "key":
			case "illusionkey":
				IllusionKey = command.SafeRemainingArgument.EqualTo("none")
					? string.Empty
					: command.SafeRemainingArgument;
				break;
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The phantom illusion effect has been updated.");
		return true;
	}

	private bool BuildingCommandScope(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out IllusionAudienceScope value))
		{
			actor.OutputHandler.Send($"Valid illusion scopes are {Enum.GetValues<IllusionAudienceScope>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		AudienceScope = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This phantom illusion will now use the {AudienceScope.DescribeEnum().ColourValue()} audience scope.");
		return true;
	}

	private bool BuildingCommandClan(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "0"))
		{
			ClanId = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This phantom illusion no longer has a clan audience restriction.");
			return true;
		}

		var clan = Gameworld.Clans.GetClan(command.SafeRemainingArgument);
		if (clan is null)
		{
			actor.OutputHandler.Send("There is no such clan.");
			return false;
		}

		ClanId = clan.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This phantom illusion will now use {clan.FullName.ColourName()} for clan-scoped audiences.");
		return true;
	}

	private bool BuildingCommandViewerProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			ViewerProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This phantom illusion no longer has a viewer FutureProg gate.");
			return true;
		}

		ViewerProg = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Perceivable],
				[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver],
				[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver, ProgVariableTypes.Character]
			]).LookupProg();
		if (ViewerProg is null)
		{
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This phantom illusion now uses {ViewerProg.MXPClickableFunctionName()} as its viewer gate.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "0"))
		{
			Colour = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This phantom illusion no longer applies an extra colour.");
			return true;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"Invalid colour. Options: {Telnet.GetColourOptions.ListToLines(true)}");
			return false;
		}

		Colour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This phantom illusion will now use {colour.Name.Colour(colour)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Phantom Illusion",
			("Audience Scope", AudienceScope.DescribeEnum().ColourValue()),
			("Clan", ClanId.HasValue ? $"#{ClanId.Value.ToString("N0", actor)}".ColourValue() : "none".ColourError()),
			("Viewer Prog", ViewerProg?.MXPClickableFunctionName() ?? "None".ColourError()),
			("Priority", Priority.ToString("N0", actor).ColourValue()),
			("Illusion Key", string.IsNullOrWhiteSpace(IllusionKey) ? "none".ColourError() : IllusionKey.ColourValue()),
			("Colour", Colour?.Name.Colour(Colour) ?? "none".ColourError()),
			("Text", Text.ColourValue())
		);
	}
}
