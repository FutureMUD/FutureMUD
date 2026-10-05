#nullable enable

using System.Reflection;
using System.Xml.Linq;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunQueuedCommandInternalCallbacks(TestDatabase database, IFuturemud world, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, IGameItem goods,
		Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance, ICharacter, string> order)
	{
		var service = world.SpellOwnedCorpseAnimations!;
		// The archive race defaults to no defence. Enable defence for this controlled callback fixture.
		foe.Race.CombatSettings.CanDefend = true;
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, new TraitExpression("1", world));
		var stock = PsionicStockContent.CombatPowers.Single(x => x.Verb == "kineticparry");
		var definition = PsionicStockContent.CombatDefinition(stock, world.Traits.GetByName("ARM02 Earth Proficiency")!.Id, 0, 0, 0);
		definition.Element("InvocationCosts")!.Remove(); definition.Element("SustainResourceCosts")!.Remove();
		definition.Element("RequiresVision")!.Value = "false"; definition.Element("ReactionStamina")!.Value = "0";
		var model = new MudSharp.Models.MagicPower { Name = "Internal response acceptance fixture", Blurb = "Controlled callback",
			ShowHelp = "Isolated acceptance only", PowerModel = "magicdefense", Definition = definition.ToString(), MagicSchoolId = world.MagicSchools.First().Id };
		using (var db = NewIndependentContext(database.ConnectionString)) { db.MagicPowers.Add(model); db.SaveChanges(); }
		var power = new MagicDefensePower(model, world); foe.LearnPower(power);
		((All<IMagicPower>)world.MagicPowers).Add(power);
		var effect = new MagicDefense(foe, power); foe.AddEffect(effect);
		var eligibility = new Mock<IFutureProg>();
		typeof(MagicDefensePower).GetProperty("EligibilityProg")!.SetValue(power, eligibility.Object);
		try
		{
			foreach (var change in new[] { "valid", "expire", "leave" })
			{
				if (change != "valid") animated = cast();
				order(animated, caster, "hit opponent");
				if (goods.InInventoryOf is not null) animated.Body.Drop(goods, silent: true);
				order(animated, caster, "get goods");
				var combat = animated.Combat!; var move = animated.ChooseMove();
				Require(move is RetrieveItemMove && service.CanCommand(animated.InstanceId, caster.Id), "The actual accepted get was not valid before execution.");
				var origin = service.CommandGrant(animated.InstanceId, caster.Id)!;
				var responses = 0; var schedulesAfterDeparture = "";
				eligibility.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() =>
				{
					if (++responses != 2) return; // Actual Character.ResponseToMove outer pass stays valid.
					if (change == "expire")
					{
						var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
						var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
						clock.Advance(until - RuntimeClock.UtcNow);
						Require(!service.CanCommand(animated.InstanceId, caster.Id) && animated.IsEmbodied && origin.DeadlineUtc > RuntimeClock.UtcNow,
							"The internal eligibility callback did not expire only control.");
					}
					else if (change == "leave") { combat.LeaveCombat(animated); schedulesAfterDeparture = ScheduleSnapshot(world); }
				}).Returns(true);
				combat.CombatAction(animated, move);
				Require(responses == 2, $"Native magic-defense eligibility responses={responses}; powers={foe.Powers.Contains(power)} able={foe.State.IsAble()} blocked={foe.IsBlocked("general").Truth} race-defend={foe.Race.CombatSettings.CanDefend} stamina={foe.CanSpendStamina(0)} afford={power.CanAffordToInvokePower(foe, power.ReactionVerb).Truth} effects={foe.Effects.Contains(effect)} target={ReferenceEquals(foe.CombatTarget, animated)}.");
				Require((goods.InInventoryOf is not null) == (change == "valid"), "Internal response authority loss failed to prevent native Body.Get.");
				Require(animated.IsEmbodied && animated.Following == caster, "Internal response refusal removed independent animation/following.");
				if (change != "valid") Require(!move.UsesStaminaWithResult(CombatMoveResult.Irrelevant), "Rejected retrieval retained action stamina cost.");
				if (change == "leave") Require(animated.Combat is null && !animated.EffectsOfType<IdleCombatant>().Any() && ScheduleSnapshot(world) == schedulesAfterDeparture,
					"Internal response departure recreated combat work.");
				Console.WriteLine($"ARMCallback-{change}=passed actual-paid-stock-get native-ChooseMove CombatAction outer-response-valid internal-Character-ResponseToMove native-MagicDefense-CanDefend controlled-eligibility-prog actual-Body-Get-only-valid following-preserved");
				if (goods.InInventoryOf is null) animated.Body.Get(goods, silent: true); // Explicit fixture conservation, not the rejected move.
				Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out var why), why); restored(animated);
			}
		}
		finally { foe.RemoveEffect(effect, true); foe.ForgetPower(power); }
		return 0;
	}
}
