#nullable enable
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class PsionicRejuvenatePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Psionic Rejuvenation";
	public override string DatabaseType => "psionicrejuvenate";
	protected override string DefaultVerb => "rejuvenate";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicrejuvenate", (row, world) => new PsionicRejuvenatePower(row, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicrejuvenate", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicRejuvenatePower(world, school, name, trait)));
	}
	private PsionicRejuvenatePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		Blurb = "Spend mental resilience to restore your body";
		_showHelpText = $"Use {school.SchoolVerb} rejuvenate <health|hunger|thirst|sobriety|stamina>. Failure worsens the selected condition and still costs stun.";
		DoDatabaseInsert();
	}
	private PsionicRejuvenatePower(MagicPower row, IFuturemud world) : base(row, world) { }
	protected override XElement SaveDefinition() => SaveTargetedDefinition();
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var mode = command.PopForSwitch();
		if (mode is not ("health" or "hunger" or "thirst" or "sobriety" or "stamina"))
		{
			actor.Send("Use rejuvenate health, hunger, thirst, sobriety or stamina."); return;
		}
		if (!CanAffordToInvokePower(actor, verb).Truth || !HandleGeneralUseRestrictions(actor) || CanInvokePower(actor, actor) == false)
		{
			actor.Send("You cannot focus enough to rejuvenate yourself."); return;
		}
		var wounds = actor.Wounds.Where(x => x.CurrentDamage > 0 && x.CanBeTreated(TreatmentType.Mend) != Difficulty.Impossible).ToArray();
		var needs = actor.NeedsModel;
		var foodLimit = ChangingNeedsModelBase.GetEffectiveFoodSatiationLimit(actor.Race.MaximumFoodSatiatedHours);
		var drinkLimit = ChangingNeedsModelBase.GetEffectiveDrinkSatiationLimit(actor.Race.MaximumDrinkSatiatedHours);
		// Native needs use hours and litres rather than the source's condition points.
		// Sobriety uses centilitres, keeping the conversion bounded in the native model.
		var (current, minimum, maximum) = mode switch
		{
			"health" => (actor.HealthStrategy.MaxHP(actor) - actor.Wounds.Sum(x => x.CurrentDamage), 1.0,
				actor.HealthStrategy.MaxHP(actor) - actor.Wounds.Sum(x => x.CurrentDamage) + wounds.Sum(x => x.CurrentDamage)),
			"stamina" => (actor.CurrentStamina, 0.0, actor.MaximumStamina),
			"hunger" => (needs.FoodSatiatedHours, -0.25 * foodLimit, foodLimit),
			"thirst" => (needs.DrinkSatiatedHours, -0.5 * drinkLimit, drinkLimit),
			_ => (-needs.AlcoholLitres * 100, -needs.AlcoholLitres * 100 - actor.HealthStrategy.MaxStun(actor), 0.0)
		};
		if (current >= maximum) { actor.Send("That condition cannot be improved any further."); return; }
		ConsumePowerCosts(actor, Verb);
		var outcome = CheckPower(actor, actor, CheckType.MindSayPower);
		var plan = PsionicRejuvenationPolicy.Plan(actor.TraitValue(SkillCheckTrait),
			actor.HealthStrategy.MaxStun(actor) - actor.Wounds.Sum(x => x.CurrentStun), actor.HealthStrategy.MaxStun(actor),
			current, minimum, maximum, outcome >= MinimumSuccessThreshold);
		if (plan.StunCost <= 0) { actor.Send("You have too little mental resilience left to change that condition."); return; }
		var consequence = new ItemConsequence(Stun: plan.StunCost, Health: mode == "health" ? Math.Max(0, -plan.Change) : 0);
		if (!ConfiguredItemConsequence.CanApply(actor, consequence) || !ConfiguredItemConsequence.Apply(actor, consequence))
		{
			actor.Send("Your body's condition prevents that exchange."); return;
		}
		switch (mode)
		{
			case "health" when plan.Change > 0:
				var remaining = plan.Change;
				foreach (var wound in wounds.OrderByDescending(x => x.CurrentDamage))
				{
					var amount = Math.Min(remaining, wound.CurrentDamage); wound.CurrentDamage -= amount; remaining -= amount;
					if (remaining <= 0) break;
				}
				actor.EvaluateWounds();
				break;
			case "stamina":
				if (plan.Change > 0) actor.GainStamina(plan.Change); else actor.SpendStamina(-plan.Change);
				break;
			case "hunger": actor.FulfilNeeds(new NeedFulfiller { SatiationPoints = plan.Change }, true); break;
			case "thirst": actor.FulfilNeeds(new NeedFulfiller { ThirstPoints = plan.Change }, true); break;
			case "sobriety": actor.FulfilNeeds(new NeedFulfiller { AlcoholLitres = -plan.Change / 100 }, true); break;
		}
		actor.Send(plan.Change > 0 ? $"Your {mode} improves, at the expense of your mental resilience." : $"Your effort falters, worsening your {mode} and exhausting your mind.");
		PsionicActivityNotifier.Notify(actor, this, "rejuvenation", actor);
	}
}
