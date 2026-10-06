#nullable enable

using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record QueuedCommandReader(string Database, long Instance, long Commander, DateTime Now,
		SpellLifecycleOrigin Origin, bool Allowed);

	private static int RunQueuedCommandReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<QueuedCommandReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var service = new SpellOwnedCorpseAnimationService(Moq.Mock.Of<IFuturemud>());
		Require(service.CommandGrant(input.Instance, input.Commander) == (input.Allowed ? input.Origin : null),
			"Fresh grant reader changed the accepted origin or extended an expired grant.");
		using var db = NewIndependentContext(database.ConnectionString);
		var row = db.CharacterInstances.AsNoTracking().Single(x => x.Id == input.Instance);
		Require(!row.EffectData.Contains("SelectedCombatAction", StringComparison.Ordinal),
			"An ephemeral queued command was persisted for replay.");
		Console.WriteLine("ARMQueued-reader=passed fresh-process immutable-origin exact-current-grant no-deadline-extension no-selected-action-persistence no-actor-materialization");
		return 0;
	}

	private static string ScheduleSnapshot(IFuturemud world)
	{
		var output = new StringBuilder(); world.Scheduler.DebugOutputForScheduler(output); return output.ToString();
	}

	private static int RunQueuedCommandAuthorityChecks(TestDatabase database, IFuturemud world, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, MudSharp.GameItems.IGameItem goods,
		Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance, ICharacter, string> order)
	{
		var service = world.SpellOwnedCorpseAnimations!;
		Moq.Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(Moq.It.IsAny<IPerceivableHaveTraits>(), Moq.It.IsAny<Difficulty>(),
			Moq.It.IsAny<IPerceivable>(), Moq.It.IsAny<IUseTrait>(), Moq.It.IsAny<double>(), Moq.It.IsAny<MudSharp.Body.Traits.TraitUseType>(), Moq.It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.SetValue(null, new MudSharp.Body.Traits.TraitExpression("1", world));
		SelectedCombatAction QueueGet()
		{
			animated.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			if (animated.Combat is null) order(animated, caster, "hit opponent");
			Require(animated.Combat is SimpleMeleeCombat, "Real stock hit did not establish native combat.");
			if (goods.InInventoryOf is not null) animated.Body.Drop(goods, silent: true);
			Require(goods.InInventoryOf is null && animated.Location.GameItems.Contains(goods), "The native item is not on the ground.");
			order(animated, caster, "get goods");
			var action = animated.EffectsOfType<SelectedCombatAction>().SingleOrDefault();
			Require(action is not null, "Real stock get command did not queue a native selected action.");
			Require(!action!.SavingEffect, "Accepted authority became a persistent effect.");
			return action;
		}
		void UntilControlEnds(SpellLifecycleOrigin origin)
		{
			var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
			var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, System.Globalization.CultureInfo.InvariantCulture,
				System.Globalization.DateTimeStyles.RoundtripKind);
			clock.Advance(until - RuntimeClock.UtcNow);
			Require(origin.DeadlineUtc > RuntimeClock.UtcNow && animated.IsEmbodied && !service.CanCommand(animated.InstanceId, caster.Id),
				"Control expiry did not leave the animation alive without authority.");
		}
		void Read(SpellLifecycleOrigin origin, bool allowed) =>
			RunItemReaderProcess(new QueuedCommandReader(database.Name, animated.InstanceId, caster.Id, RuntimeClock.UtcNow, origin, allowed),
				"--queued-command-reader");
		void Restore()
		{
			// Fixture cleanup uses a direct native inventory action, never the rejected ordered action.
			if (goods.InInventoryOf is null) animated.Body.Get(goods, silent: true);
			Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
			restored(animated);
		}

		var action = QueueGet();
		var original = service.CommandGrant(animated.InstanceId, caster.Id)!;
		Require(original is not null, "Accepted stock order has no immutable grant origin.");
		var combat = animated.Combat!;
		var move = animated.ChooseMove();
		Require(move is RetrieveItemMove && !animated.Effects.Contains(action), "Native strategy did not consume a valid ordered action.");
		combat.CombatAction(animated, move);
		Require(animated.Body.ExternalItems.Contains(goods) && !animated.Location.GameItems.Contains(goods),
			"Valid ordered native move failed to retrieve the real item.");
		Console.WriteLine("ARMQueued-valid=passed actual-CommandableAI real-get-command native-selected-queue native-strategy-consumption native-CombatAction real-Body-Get");

		action = QueueGet(); world.SaveManager.Flush(); Read(original, true);
		UntilControlEnds(original);
		Require(action.GetMove(animated) is null, "A queued order retained authority after the exact control deadline.");
		_ = animated.ChooseMove();
		Require(!animated.Effects.Contains(action) && goods.InInventoryOf is null && animated.Following == caster,
			"Expired queued order ran, retained its effect, or removed independent following.");
		Read(original, false);
		Console.WriteLine("ARMQueued-expired-queue=passed exact-control-boundary animation-alive original-order-refused queue-consumed goods-unchanged independent-native-following fresh-process-grant-refusal");
		Restore();

		animated = cast(); action = QueueGet();
		original = service.CommandGrant(animated.InstanceId, caster.Id)!; combat = animated.Combat!;
		move = animated.ChooseMove(); Require(move is RetrieveItemMove, "Pre-expiry order was not selected.");
		UntilControlEnds(original); combat.CombatAction(animated, move);
		Require(goods.InInventoryOf is null && animated.Following == caster && animated.IsEmbodied,
			"A move selected before expiry executed after authority expired.");
		Console.WriteLine("ARMQueued-expired-selected=passed actual-native-CombatAction selected-before-deadline resolved-after-deadline no-Body-Get animation-and-following-preserved");
		Restore();

		foreach (var callback in new[] { "retire", "leave-combat" })
		{
			animated = cast();
			var callbackAi = animated.AIs.OfType<CommandableAI>().Single();
			var progField = typeof(CommandableAI).GetField("_canCommandProg", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
			var originalProg = (MudSharp.FutureProg.IFutureProg)progField.GetValue(callbackAi)!;
			var callbackProg = new Moq.Mock<MudSharp.FutureProg.IFutureProg>();
			var armed = false; var checks = 0; var afterCallback = "";
			callbackProg.Setup(x => x.ExecuteBool(Moq.It.IsAny<object[]>())).Returns<object[]>(arguments =>
			{
				var allowed = originalProg.ExecuteBool(arguments);
				if (armed && ++checks == (callback == "retire" ? 1 : 2))
				{
					armed = false;
					if (callback == "retire")
					{
						animated.Body.Get(goods, silent: true);
						Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
					}
					else animated.Combat!.LeaveCombat(animated);
					afterCallback = ScheduleSnapshot(world);
				}
				return allowed;
			});
			progField.SetValue(callbackAi, callbackProg.Object);
			try
			{
				action = QueueGet(); combat = animated.Combat!; move = animated.ChooseMove();
				Require(move is RetrieveItemMove, "Callback fixture failed to select the real native move.");
				armed = true; checks = 0; combat.CombatAction(animated, move);
				Require(!armed && afterCallback == ScheduleSnapshot(world) && animated.Combat is null &&
					!animated.EffectsOfType<IdleCombatant>().Any(), "Authority callback recreated combat work after departure.");
				if (callback == "retire") restored(animated);
				else
				{
					Require(goods.InInventoryOf is null && animated.Following == caster && animated.IsEmbodied,
						"Late policy departure executed retrieval or removed independent following.");
					Restore();
				}
				Console.WriteLine($"ARMQueued-policy-{callback}=passed controlled-authorizing-prog-callback actual-native-{callback} real-selected-RetrieveItemMove no-resolution no-idle-or-schedule-resurrection same-body-corpse-foreign-gear");
			}
			finally { progField.SetValue(callbackAi, originalProg); }
		}

		animated = cast(); action = QueueGet();
		var commandAi = animated.AIs.OfType<CommandableAI>().Single(); animated.RemoveAI(commandAi);
		Require(action.GetMove(animated) is null, "Removed command policy still authorized the queued order.");
		_ = animated.ChooseMove();
		Require(goods.InInventoryOf is null && animated.Following == caster, "Policy revocation affected goods or independent following.");
		Console.WriteLine("ARMQueued-policy-revoked=passed real-native-AI-removal queued-order-refused selected-effect-consumed independent-following-preserved");
		Restore();

		animated = cast(); action = QueueGet(); original = service.CommandGrant(animated.InstanceId, caster.Id)!;
		combat = animated.Combat!; move = animated.ChooseMove();
		animated.Body.Get(goods, silent: true);
		Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out var diagnostic), diagnostic);
		restored(animated);
		var schedules = new StringBuilder(); world.Scheduler.DebugOutputForScheduler(schedules);
		combat.CombatAction(animated, move);
		Require(animated.Combat is null && !animated.EffectsOfType<IdleCombatant>().Any() &&
			ScheduleSnapshot(world) == schedules.ToString() && !animated.EffectsOfType<ISelectedCombatAction>().Any(),
			"A stale selected action recreated work after native retirement.");
		Console.WriteLine("ARMQueued-retired-selected=passed actual-native-retirement stale-CombatAction-refused no-new-idle-effect-or-schedule no-selected-work same-canonical-body-corpse-foreign-gear");
		return 0;
	}
}
