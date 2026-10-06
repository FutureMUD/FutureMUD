using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Resources;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void VerifyCompoundCapacityChanges(NativeRuntime native, MagicSpell spell, ITraitDefinition attribute,
		TraitExpression expression, SimpleMagicResource reserve, SkillLevelBasedMagicCapability capability, Action advanceClock)
	{
		var actor = native.Actor;
		var targets = (List<IMagicSpellEffectTemplate>)spell.SpellEffects;
		var casters = (List<IMagicSpellEffectTemplate>)spell.CasterSpellEffects;
		var originalTargets = targets.ToArray(); var originalCasters = casters.ToArray();
		var originalFormula = expression.OriginalFormulaText;
		var originalRaw = actor.TraitRawValue(attribute);
		var originalBasis = reserve.AttributeCapacity!.UseRawAttribute;
		var originalBalance = actor.MagicResourceAmounts[reserve];

		void ClearEffects()
		{
			actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
			actor.RemoveAllEffects<MagicSpellLockout>(fireRemovalAction: true);
		}
		void SetBalance(double amount)
		{
			Require(actor.UseResource(reserve, actor.MagicResourceAmounts[reserve]), "Compound fixture balance reset refused.");
			actor.AddResource(reserve, amount);
			Require(actor.MagicResourceAmounts[reserve] == amount, "Compound fixture legitimate credit did not match.");
		}
		void SetBonuses(double[] bonuses, bool split)
		{
			targets.Clear(); casters.Clear();
			for (var i = 0; i < bonuses.Length; i++)
			{
				var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "boost"),
					new XAttribute("trait", attribute.Id), new XAttribute("bonus", bonuses[i]), new XAttribute("context", 0)), spell);
				(split && i == 1 ? casters : targets).Add(effect);
			}
			spell.Changed = true;
		}

		try
		{
			Require(expression.BuildingCommand(actor, new StringStack("formula variable*10")) && actor.SetTraitValue(attribute, 10) &&
				reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} effective")), "Compound native capacity setup refused.");
			Require(!spell.GradeProfile!.ScalarBindings.Any(), "Compound fixture requires no effect-specific scalar bindings.");
			foreach (var mode in new[] { "command", "prepared", "combined" })
			foreach (var positiveFirst in new[] { true, false })
			foreach (var negative in new[] { -5.0, -8.0 })
			{
				ClearEffects(); SetBalance(90);
				SetBonuses(positiveFirst ? [5, negative] : [negative, 5], mode == "combined");
				if (mode == "command")
				{
					advanceClock();
					MagicModule.MagicGeneric(actor, $"{capability.School.SchoolVerb} cast \"{spell.Name}\" grade 7 on self via {capability.Id}");
				}
				else spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
				var finalCapacity = (10 + 5 + negative) * 10;
				var expectedBalance = Math.Min(mode == "command" ? 65 : 90, finalCapacity);
				Require(actor.EffectsOfType<SpellTraitBoostEffect>().Count() == 2 && reserve.ResourceCap(actor) == finalCapacity &&
					actor.MagicResourceAmounts[reserve] == expectedBalance,
					$"Compound {mode} application lost energy at a transient maximum; positive-first:{positiveFirst}, negative:{negative}.");
				if (mode == "combined") actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
				else
				{
					var parent = actor.EffectsOfType<MagicSpellParent>().Single();
					if (positiveFirst) parent.ExpireEffect(); else actor.RemoveEffect(parent, true);
				}
				Require(reserve.ResourceCap(actor) == 100 && actor.MagicResourceAmounts[reserve] == expectedBalance &&
					!actor.EffectsOfType<SpellTraitBoostEffect>().Any(), "Compound removal lost energy or refilled a completed increase.");
			}
			foreach (var positiveFirst in new[] { true, false })
			{
				ClearEffects(); SetBalance(90); SetBonuses(positiveFirst ? [8, -3] : [-3, 8], false);
				spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
				Require(reserve.ResourceCap(actor) == 150 && actor.MagicResourceAmounts[reserve] == 90, "Compound capacity increase refilled energy.");
				actor.AddResource(reserve, 50);
				actor.EffectsOfType<MagicSpellParent>().Single().ExpireEffect();
				Require(reserve.ResourceCap(actor) == 100 && actor.MagicResourceAmounts[reserve] == 100,
					"Genuine completed capacity decrease did not clamp once against the final100 maximum.");
			}
			foreach (var mode in new[] { "command", "prepared", "combined" })
			foreach (var positiveFirst in new[] { true, false })
			foreach (var shape in new (double Positive, double Negative, double Delta)[] { (5.0, -5.0, 7.0), (5.0, -8.0, 7.0),
				(10.0, -5.0, 50.0), (5.0, -5.0, -7.0), (5.0, -5.0, -60.0) })
			{
				ClearEffects(); SetBalance(90);
				SetBonuses(positiveFirst ? [shape.Positive, shape.Negative] : [shape.Negative, shape.Positive], mode == "combined");
				targets.Insert(1, SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "magicresourcedelta"),
					new XElement("Resource", reserve.Id), new XElement("Formula", shape.Delta.ToString(System.Globalization.CultureInfo.InvariantCulture))), spell));
				if (mode == "command")
				{
					advanceClock();
					MagicModule.MagicGeneric(actor, $"{capability.School.SchoolVerb} cast \"{spell.Name}\" grade 7 on self via {capability.Id}");
				}
				else spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
				var finalCapacity = (10 + shape.Positive + shape.Negative) * 10;
				var expectedBalance = Math.Min((mode == "command" ? 65 : 90) + shape.Delta, finalCapacity);
				Require(actor.EffectsOfType<SpellTraitBoostEffect>().Count() == 2 && reserve.ResourceCap(actor) == finalCapacity &&
					actor.MagicResourceAmounts[reserve] == expectedBalance,
					$"Mixed compound {mode} delta used a transient maximum; positive-first:{positiveFirst}, delta:{shape.Delta}, final:{finalCapacity}.");
				actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
				Require(reserve.ResourceCap(actor) == 100 && actor.MagicResourceAmounts[reserve] == Math.Min(expectedBalance, 100),
					"Mixed compound cleanup lost energy or refilled the reserve.");
			}
			ClearEffects(); SetBalance(17); targets.Clear(); casters.Clear();
			targets.Add(SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "magicresourcedelta"),
				new XElement("Resource", reserve.Id), new XElement("Formula", "7")), spell));
			spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
			Require(actor.MagicResourceAmounts[reserve] == 24, "Live compound batching blocked an ordinary native resource effect.");
			Console.WriteLine("ARM-CAPACITY-compound=passed both-child-orders normal-command-casts:4 prepared-targets:4 combined-target-caster:4 complete-removals:14 neutral-90-preserved final-application-decrease:70 final-removal-decrease:100 no-refill native-resource-delta:17-to-24 mixed-native-deltas:30 neutral-credit:97 final-decrease:70 final-increase-credit:140 negative-delta:83 large-debit:30 command-cost:25");
		}
		finally
		{
			ClearEffects(); targets.Clear(); targets.AddRange(originalTargets); casters.Clear(); casters.AddRange(originalCasters); spell.Changed = true;
			Require(expression.BuildingCommand(actor, new StringStack("formula " + originalFormula)) && actor.SetTraitValue(attribute, originalRaw) &&
				reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} {(originalBasis ? "raw" : "effective")}")), "Compound fixture configuration restoration refused.");
			SetBalance(originalBalance);
		}
	}
}
