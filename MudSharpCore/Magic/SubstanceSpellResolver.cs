using System.Globalization;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Explicit compatibility boundary: an unclassified effect is never invoked with a missing caster.</summary>
public static class SubstanceSpellResolver
{
	private static readonly HashSet<string> Binary = new(StringComparer.OrdinalIgnoreCase)
	{
		"invisibility", "blindness", "deafness", "silence", "insomnia", "paralysis", "flying", "waterbreathing",
		"detectinvisible", "sleep", "removesilence", "removesleep", "removeinsomnia", "removefear", "removeparalysis",
		"removeflying", "removewaterbreathing", "removecurse", "removedetectinvisible", "fear", "curse", "detectethereal", "removedetectethereal", "detectmagick", "removedetectmagick",
		"infravision", "removeinfravision", "comprehendlanguage", "removecomprehendlanguage", "removeblindness", "removedeafness",
		"planarstate", "planeshift", "transformform", "magictag", "removemagictag", "destroyitem", "createitem", "removepoison", "removedisease"
	};
	private static readonly Dictionary<string, string[]> Scalars = new(StringComparer.OrdinalIgnoreCase)
	{
		["needdelta"] = ["Hunger", "Thirst", "Drunk"], ["glow"] = ["GlowLuxPerPower"],
		["weight"] = ["Weight"], ["pacifism"] = ["Intensity"], ["rage"] = ["Intensity"],
		["staminaregenrate"] = ["Multiplier"], ["staminaexpendrate"] = ["Multiplier"],
		["needrate"] = ["HungerMult", "ThirstMult", "DrunkMult"],
		["itemenchant"] = ["GlowLux", "AttackCheckBonus", "QualityBonus", "DamageBonus", "PainBonus", "StunBonus", "ArmourDamageReduction", "ProjectileQualityBonus", "ProjectileDamageBonus", "ProjectilePainBonus", "ProjectileStunBonus", "ToolFitnessBonus", "ToolSpeedMultiplier", "ToolUsageMultiplier", "PowerProductionMultiplier", "PowerConsumptionMultiplier", "FuelUseMultiplier"]
	};
	private static bool IsMultiplier(string field) => field.EndsWith("Multiplier", StringComparison.Ordinal) || field.EndsWith("Mult", StringComparison.Ordinal);
	private static double ScaleMultiplier(double value, double scale) => Math.Max(0, 1 + (value - 1) * scale);
	private static string Type(IMagicSpellEffectTemplate effect) => effect.SaveToXml().Attribute("type")!.Value;
	public static IEnumerable<ITraitExpression> Expressions(IMagicSpellEffectTemplate effect) => effect switch
	{
		HealEffect x => [x.HealingAmount], MendEffect x => [x.HealingAmount], DamageEffect x => [x.DamageExpression],
		StaminaDeltaSpellEffect x => [x.AmountExpression], MagicResourceDeltaEffect x => [x.DeltaExpression], SpellArmourEffect x => [x.ArmourConfiguration.MaximumDamageAbsorbed], _ => []
	};
	public static bool Scalable(IMagicSpellEffectTemplate effect) => effect is HealEffect or MendEffect or DamageEffect or
		StaminaDeltaSpellEffect or MagicResourceDeltaEffect or NeedDeltaEffect or TraitBoostEffect or GlowEffect or HealingRateSpellEffect or ItemDamageEffect or SpellArmourEffect || Scalars.ContainsKey(Type(effect));
	public static bool Supported(IMagicSpellEffectTemplate effect) => Scalable(effect) || Binary.Contains(Type(effect));
	public static IEnumerable<string> Errors(IMagicSpell? spell, SubstanceEffectEntry entry)
	{
		if (spell is null) { yield return "Missing spell."; yield break; }
		if (spell.Trigger is not SpellTriggers.SubstanceTrigger) yield return "Use a substancecharacter or substanceitem spell trigger.";
		if (spell.CasterSpellEffects.Any()) yield return "Caster effects are not supported by substances.";
		if (!spell.SpellEffects.Any()) yield return "The spell has no target effects.";
		if (!Enum.IsDefined(entry.Lifecycle) || !Enum.IsDefined(entry.PulseMode) || !Enum.IsDefined(entry.Stacking) || !Enum.IsDefined(entry.Scaling)) yield return "Invalid lifecycle settings.";
		if (!double.IsFinite(entry.MinimumDose) || entry.MinimumDose < 0 || !SubstanceDose.IsPositive(entry.MaximumDose) || !SubstanceDose.IsPositive(entry.DurationSeconds) || !SubstanceDose.IsPositive(entry.MaximumDurationSeconds) || !double.IsFinite(entry.IntervalSeconds) || entry.IntervalSeconds < 1) yield return "Dose, duration and interval settings must be finite and within their valid ranges.";
		if (entry.MinimumDose > entry.MaximumDose) yield return "Minimum dose exceeds maximum dose.";
		if (entry.MaximumDurationSeconds > TimeSpan.MaxValue.TotalSeconds / 2 || entry.IntervalSeconds > TimeSpan.MaxValue.TotalSeconds / 2) yield return "Timing settings exceed the supported timespan.";
		if (entry.DurationSeconds > entry.MaximumDurationSeconds) yield return "Duration exceeds its cap.";
		foreach (var effect in spell.SpellEffects)
		{
			var type = Type(effect);
			if (!Supported(effect)) { yield return $"{type}: no source-independent substance adapter."; continue; }
			if (spell.Trigger is not null && !effect.IsCompatibleWithTrigger(spell.Trigger)) yield return $"{type}: incompatible target.";
			if (effect is TransformFormEffect && spell.Trigger?.TargetTypes != "character") yield return "transformform: requires a character target.";
			if (entry.Lifecycle == SubstanceLifecycle.Maintained && effect.IsInstantaneous) yield return $"{type}: an instantaneous effect cannot be maintained.";
			if (entry.Lifecycle == SubstanceLifecycle.Periodic && !effect.IsInstantaneous) yield return $"{type}: pulse payloads must be instantaneous.";
			if (!Scalable(effect) && (effect.IsInstantaneous || entry.Lifecycle == SubstanceLifecycle.Maintained || entry.Scaling == SubstanceScaling.Magnitude) && entry.MinimumDose <= 0)
				yield return $"{type}: an indivisible effect needs a positive minimum dose.";
			if (effect is TraitBoostEffect boost && boost.Trait is null) yield return "Choose the trait to boost.";
			foreach (var expression in Expressions(effect))
				if (expression.HasErrors() || expression.Parameters.Any() || expression.NonTraitParameters.Except(new[] { "power", "outcome", "degrees", "success", "variable" }, StringComparer.OrdinalIgnoreCase).Any())
					yield return $"{type}: substance formulas may use power/outcome/degrees/success/variable, not caster traits or options.";
		}
	}
	public static bool CanApply(MagicSpell spell, SubstanceResolutionContext context)
	{
		if (MagicInterdictionHelper.GetInterdiction(context.ResponsibleActor!, context.Target, spell.School, false,
			spell.SpellEffects.OfType<IMagicInterdictionTagProvider>().SelectMany(x => x.MagicInterdictionTags)) is not null) return false;
		if (spell.OpposedTrait is not null && context.Target is ICharacter character)
		{
			var result = character.Gameworld.GetCheck(CheckType.ResistMagicSpellCheck).Check(character,
				spell.OpposedDifficulty ?? Difficulty.Normal, spell.OpposedTrait, context.ResponsibleActor);
			if (new OpposedOutcome(Outcome.Pass, result.Outcome).Outcome == OpposedOutcomeDirection.Opponent)
			{
				Emit(spell, context, true);
				return false;
			}
		}
		return true;
	}
	public static void Emit(MagicSpell spell, SubstanceResolutionContext context, bool resisted = false)
	{
		var text = resisted ? spell.TargetResistedEmote : spell.TargetEmote;
		if (string.IsNullOrWhiteSpace(text) || context.Target is not IPerceiver perceiver) return;
		// The target occupies both slots in substance emotes; no character is invented as a caster.
		context.Target.OutputHandler.Handle(new MudSharp.PerceptionEngine.Outputs.EmoteOutput(
			new MudSharp.PerceptionEngine.Parsers.Emote(text, perceiver, context.Target, context.Target), flags: spell.TargetEmoteFlags));
	}
	public static IMagicSpellEffect? Apply(IMagicSpellEffectTemplate effect, IMagicSpell spell,
		SubstanceResolutionContext context, IMagicSpellEffectParent parent, double magnitude)
	{
		if (effect is TransformFormEffect && context.Target is not ICharacter) return null;
		var template = effect;
		if (Scalable(effect))
		{
			var xml = effect.SaveToXml();
			foreach (var expression in Expressions(effect))
			{
				var value = expression.EvaluateWith(null!, values: [("power", (int)context.Substance.Power), ("outcome", (int)context.Outcome), ("degrees", 1), ("success", 1)]);
				if (!double.IsFinite(value * magnitude))
				{
					context.Substance.Gameworld.SystemMessage($"Magical substance #{context.Substance.Id}: spell #{spell.Id} returned a non-finite scaled amount; payload skipped.", true);
					return null;
				}
				var field = effect switch
				{
					HealEffect or MendEffect => "HealingAmount", DamageEffect => "DamageExpression",
					SpellArmourEffect => "MaximumDamageAbsorbed", _ => "Formula"
				};
				xml.SetElementValue(field, (value * magnitude).ToString("R", CultureInfo.InvariantCulture));
			}
			if (Scalars.TryGetValue(Type(effect), out var fields))
				foreach (var key in fields)
				{
					var node = xml.Element(key);
					if (node is null) continue;
					node.Value = (IsMultiplier(key) ? ScaleMultiplier((double)node, magnitude) : (double)node * magnitude).ToString("R", CultureInfo.InvariantCulture);
				}
			if (effect is ItemDamageEffect)
				foreach (var key in new[] { "DamageFormula", "PainFormula", "StunFormula" })
					if (xml.Element(key) is { } node) node.Value = $"({node.Value})*{magnitude.ToString("R", CultureInfo.InvariantCulture)}";
			if (effect is TraitBoostEffect) xml.SetAttributeValue("bonus", (double)xml.Attribute("bonus")! * magnitude);
			if (effect is HealingRateSpellEffect)
			{
				xml.SetElementValue("Multiplier", Math.Max(0, 1.0 + ((double)xml.Element("Multiplier")! - 1.0) * magnitude));
				xml.SetElementValue("Stages", (int)((int)xml.Element("Stages")! * magnitude));
			}
			template = SpellEffectFactory.LoadEffect(xml, spell);
		}
		return template.GetOrApplyEffect(context.ResponsibleActor!, context.Target, context.Outcome, context.Substance.Power, parent, []);
	}
	
	public static void Update(IMagicSpellEffect effect, IMagicSpellEffectTemplate template, double dose, SpellPower power)
	{
		switch (effect)
		{
			case SpellWeightEffect weight when template is WeightSpellEffect source: weight.AddedWeight = source.AddedWeight * dose; break;
			case SpellStaminaRegenerationEffect stamina when template is StaminaRegenRateSpellEffect source: stamina.Multiplier = ScaleMultiplier(source.Multiplier, dose); break;
			case SpellStaminaExpenditureEffect stamina when template is StaminaExpenditureSpellEffect source: stamina.Multiplier = ScaleMultiplier(source.Multiplier, dose); break;
			case SpellPacifismEffect pacifism when template is PacifismSpellEffect source: pacifism.IntensityPerGramMass = source.Intensity * dose; break;
			case SpellRageEffect rage when template is RageSpellEffect source: rage.IntensityPerGramMass = source.Intensity * dose; break;
			case SpellNeedRateEffect needs when template is NeedRateSpellEffect source:
				needs.HungerMultiplier = ScaleMultiplier(source.HungerMultiplier, dose);
				needs.ThirstMultiplier = ScaleMultiplier(source.ThirstMultiplier, dose);
				needs.DrunkennessMultiplier = ScaleMultiplier(source.DrunkennessMultiplier, dose); break;
			case SpellItemEnchantmentEffect enchantment when template is ItemEnchantEffect source: enchantment.UpdateSubstanceMagnitude(source, dose); break;
			case SpellArmourProtectionEffect armour when template is SpellArmourEffect source:
				var capacity = source.ArmourConfiguration.MaximumDamageAbsorbed.EvaluateWith(null!, values: [("power", (int)power), ("outcome", 1), ("degrees", 1), ("success", 1)]);
				armour.ArmourConfiguration.MaximumDamageAbsorbed = new TraitExpression((capacity * dose).ToString("R", CultureInfo.InvariantCulture), source.Gameworld);
				break;
			case SpellTraitBoostEffect boost when template is TraitBoostEffect source: boost.Bonus = source.Bonus * dose; break;
			case SpellGlowEffect glow when template is GlowEffect source: glow.GlowLux = source.GlowLuxPerPower * (int)power * dose; break;
			case SpellHealingRateEffect healing when template is HealingRateSpellEffect source:
				healing.HealingRateMultiplier = Math.Max(0, 1 + (source.Multiplier - 1) * dose); healing.HealingDifficultyStages = (int)(source.Stages * dose); break;
		}
	}
}
