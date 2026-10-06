#nullable enable

using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
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
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunOrderedNpcCallbacks(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe,
		Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		var world = host.Native.World;
		var service = world.SpellOwnedCorpseAnimations!;
		foe.Race.CombatSettings.CanDefend = false;
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, new TraitExpression("1", world));
		GameItem NewGear()
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B C2 gear").CreateNew(caster);
			world.Add(item); caster.Location.Insert(item, true); item.Login(); world.SaveManager.Flush();
			return item;
		}
		void ExpireControl(ScriptedAiCharacterInstance actor)
		{
			var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
			var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
			var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			clock.Advance(until - RuntimeClock.UtcNow);
			Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied && origin.DeadlineUtc > RuntimeClock.UtcNow,
				"Wear callback must expire only command control.");
		}

		foreach (var change in new[] { "valid", "expire", "retire", "foreign" })
		{
			if (change != "valid") animated = cast();
			var actor = animated;
			var body = actor.Body;
			void BodySaveState(string stage) => Console.WriteLine($"ARMOrdered-save-diagnostic-{change}-{stage}=body:{body.Id} noSave:{body.GetNoSave()} inventoryChanged:{body.InventoryChanged} changed:{body.Changed} queued:{world.SaveManager.IsQueued(body)}");
			BodySaveState("initial");
			var gear = NewGear();
			var wearable = gear.GetItemType<IWearable>()!;
			var eligible = body.CanGet(gear, 0);
			var refusal = eligible ? string.Empty : body.WhyCannotGet(gear, 0);
			body.Get(gear, silent: true);
			BodySaveState("get");
			Require(ReferenceEquals(gear.InInventoryOf, body), $"Wear setup did not get the exact gear into the real body. eligible={eligible} reason={refusal} held={string.Join(',', body.HeldItems.Select(x => x.Id))} direct-cell={gear.Location?.Id} holder={gear.InInventoryOf?.Id}");
			order(actor, caster, "hit opponent");
			actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			order(actor, caster, "wear gear");
			var combat = actor.Combat!;
			var move = actor.ChooseMove();
			Require(move is WearItemMove && service.CanCommand(actor.InstanceId, caster.Id), "Actual stock wear did not select a native wear move.");
			var callback = new Mock<IFutureProg>();
			var proto = (WearableGameItemComponentProto)wearable.Prototype;
			var previous = proto.WearableProg;
			GameItem? foreignGear = change == "foreign" ? NewGear() : null;
			if (foreignGear is not null) caster.Body.Get(foreignGear, silent: true);
			var called = 0;
			callback.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback<object[]>(args =>
			{
				if (!ReferenceEquals(args[0], actor)) return;
				called++;
				if (called != 1) return;
				if (change == "foreign")
				{
					caster.Body.Wear(foreignGear!, silent: true);
					Require(ReferenceEquals(foreignGear!.GetItemType<IWearable>()!.WornBy, caster.Body), "Foreign wear callback did not perform native Body.Wear.");
				}
				if (change is "expire" or "foreign") ExpireControl(actor);
				if (change == "retire")
				{
					Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
					Require(!ReferenceEquals(body.Actor, actor), "Native retirement did not rebind the canonical corpse body.");
				}
			}).Returns(true);
			typeof(WearableGameItemComponentProto).GetProperty(nameof(WearableGameItemComponentProto.WearableProg))!.SetValue(proto, callback.Object);
			try { combat.CombatAction(actor, move); }
			finally { typeof(WearableGameItemComponentProto).GetProperty(nameof(WearableGameItemComponentProto.WearableProg))!.SetValue(proto, previous); }
			Require(called > 0, "Native wearable eligibility callback was not exercised.");
			Require(ReferenceEquals(wearable.WornBy, body) == (change == "valid"), "Refused native wear changed the original body's worn state.");
			Require(ReferenceEquals(gear.InInventoryOf, body), "Native wear callback lost or transferred the exact original gear.");
			BodySaveState("wear");
			world.SaveManager.Flush();
			BodySaveState("flush");
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var persisted = db.BodiesGameItems.AsNoTracking().Single(x => x.GameItemId == gear.Id && x.BodyId == body.Id);
				Require((persisted.WearProfile is not null) == (change == "valid"), "Native Save did not preserve the verified worn/held state.");
			}
			if (change != "valid") Require(!move.UsesStaminaWithResult(CombatMoveResult.Irrelevant), "Refused native wear retained stamina accounting.");
			if (change != "retire")
			{
				Require(actor.IsEmbodied && actor.Following == caster, "Wear refusal removed independent animation/following.");
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
			}
			restored(actor);
			if (foreignGear is not null)
			{
				Require(ReferenceEquals(foreignGear.GetItemType<IWearable>()!.WornBy, caster.Body), "Original order refusal undid the foreign native wear.");
				caster.Body.Take(foreignGear); foreignGear.Delete();
			}
			body.Take(gear); gear.Delete(); world.SaveManager.Flush();
			Console.WriteLine($"ARMOrdered-wear-{change}=passed paid-stock actual-command native-ChooseMove CombatAction real-body eligibility-callback canonical-owner custody database-worn-state");
		}
		RunOrderedNpcManualAttackCallbacks(database, host, clock, caster, foe, cast, restored, order);
		return RunOrderedNpcCurrency(database, host, caster, foe, clock, cast, restored, order, fixture);
	}
}
