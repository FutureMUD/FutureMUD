#nullable enable

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Moq;
using MudSharp.Body;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void SeedLoadOutputFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var gun = db.GameItemComponentProtos.Single(x => x.Name == "ARMRegression InternalMagazineGun");
		var xml = XElement.Parse(gun.Definition); xml.Element("InternalMagazineCapacity")!.Value = "2"; gun.Definition = xml.ToString();
		var round = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARMRegression round");
		var stack = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Stackable");
		round.GameItemProtosGameItemComponentProtos.Add(new MudSharp.Models.GameItemProtosGameItemComponentProtos { GameItemComponentProtoId = stack.Id });
		db.SaveChanges();
	}

	private static int RunLoadOutputCustody(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		ConfigureRegressionP2Fixture(host, database);
		var world = host.Native.World;
		// Match the real world's All.Add late-id registration; querying Id here flushes
		// copied components before GameItem has queued its parent initialisation.
		host.Native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
		{
			if (!host.Items.Any(x => ReferenceEquals(x, item))) host.Items.Add(item);
		});
		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, new MudSharp.Body.Traits.TraitExpression("1000", world));
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var configured = new System.Collections.Generic.HashSet<CommandableAI>();
		foreach (var scenario in new[] { "output-rebind", "take-refill", "take-throw", "split-valid", "ordered-valid", "direct-valid" })
		{
			var actor = scenario == "output-rebind" ? animated : cast(); var body = (Body)actor.Body;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included load")), "Declare extended test whitelist load; stock whitelist excludes it.");
			var unrelated = body.HeldItems.ToArray();
			foreach (var held in unrelated) body.Drop(held, silent: true);
			GameItem New(string name)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				world.Add(item); actor.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var gunItem = New("ARMRegression gun"); var round = New("ARMRegression round");
			var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			round.GetItemType<IStackable>()!.Quantity = scenario == "split-valid" ? 3 : scenario == "ordered-valid" ? 2 : 1;
			Require(ReferenceEquals(body.GetWithoutMerge(gunItem), gunItem) && ReferenceEquals(body.GetWithoutMerge(round), round), "Acquire native exact gun and round in hand.");
			var callbacks = 0; var deletions = 0; var taken = 0; var expectedError = new InvalidOperationException("one-shot native taken callback");
			var previousItems = world.Items.ToHashSet();
			GameItem? observedParticipant = null;
			round.OnDeleted += _ => ++deletions;
			InventoryChangeEvent fault = (_, state, changed) =>
			{
				if (!ReferenceEquals(changed, round) || state != InventoryState.Dropped || ++taken != 1) return;
				Require(round.ContainedIn is null && round.DirectLocation is null && round.GetItemType<IHoldable>()!.HeldBy is null, "Actual Body.Take callback sees exact detached participant.");
				if (scenario == "take-refill") { using var independent = CommandExecutionScope.EnterIndependent(); round.GetItemType<IStackable>()!.Quantity = 3; }
				if (scenario == "take-throw") throw expectedError;
			};
			body.OnInventoryChange += fault;
			var originalOutput = actor.OutputHandler; var output = new Mock<IOutputHandler>(); output.SetupGet(x => x.Perceiver).Returns(actor);
			output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<IOutput, bool, bool>((message, newline, nopage) =>
			{
				if (new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(InternalMagazineGunGameItemComponent) && x.GetMethod()?.Name == "Load"))
				{
					++callbacks;
					if (scenario == "split-valid")
					{
						Require(round.Quantity == 1 && ReferenceEquals(round.GetItemType<IHoldable>()!.HeldBy, body), "Split source retains exactly one held round during Load output.");
						var splits = world.Items.OfType<GameItem>().Where(x => !previousItems.Contains(x) && ReferenceEquals(x.Prototype, round.Prototype)).ToArray();
						Require(splits.Length == 1 && splits[0].Quantity == 2 && splits[0].OwnershipReference == round.OwnershipReference, "Observe the exact new native split with its quantity and title during Load output.");
						observedParticipant = splits[0];
					}
					else observedParticipant = round;
					Require(observedParticipant.ContainedIn is null && observedParticipant.DirectLocation is null && observedParticipant.GetItemType<IHoldable>()!.HeldBy is null, "Observe the actual detached native Load participant during its output.");
					if (scenario == "output-rebind")
					{
						using var independent = CommandExecutionScope.EnterIndependent(); actor.Location.Insert(round, true);
						Require(ReferenceEquals(round.DirectLocation, actor.Location) && actor.Location.GameItems.Contains(round), "Independent output callback establishes actual cell custody.");
					}
				}
				originalOutput.Send(message, newline, nopage);
			}).Returns(true);
			SetPrivateMember(actor, "OutputHandler", output.Object);
			try
			{
				if (scenario == "take-throw")
				{
					try { using var independent = CommandExecutionScope.EnterIndependent(); gun.Load(actor); throw new ApplicationException("Expected the original one-shot native callback exception."); }
					catch (InvalidOperationException error) { Require(ReferenceEquals(error, expectedError), "Preserve the original native callback exception."); }
				}
				else if (scenario == "direct-valid") { using var independent = CommandExecutionScope.EnterIndependent(); gun.Load(actor); }
				else order(actor, caster, "load gun");
			}
			finally { SetPrivateMember(actor, "OutputHandler", originalOutput); body.OnInventoryChange -= fault; }
			var accepted = scenario is "ordered-valid" or "direct-valid" or "split-valid";
			var loaded = gun.MagazineContents.OfType<GameItem>().ToArray();
			var expectedTotal = scenario is "take-refill" or "split-valid" ? 3 : scenario == "ordered-valid" ? 2 : 1;
			var liveItems = new[] { round }.Concat(loaded).Distinct().ToArray();
			Console.WriteLine($"ARMLoadOutput-custody={scenario} output:{callbacks} taken:{taken} source:{round.Quantity} holder:{round.GetItemType<IHoldable>()!.HeldBy?.Id} cell:{round.DirectLocation?.Id} container:{round.ContainedIn?.Id} floor:{actor.Location.GameItems.Contains(round)} magazine:{string.Join(',', loaded.Select(x => x.Id+":"+x.Quantity))}");
			Require(callbacks == (scenario is "take-refill" or "take-throw" ? 0 : 1) && deletions == 0 && liveItems.All(x => !x.Deleted) &&
				liveItems.Sum(x => x.Quantity) == expectedTotal && loaded.Length == (accepted ? 1 : 0) &&
				(!accepted || ReferenceEquals(loaded.Single(), observedParticipant) && loaded.Single().Quantity == (scenario is "split-valid" or "ordered-valid" ? 2 : 1)) &&
				(scenario == "split-valid" ? round.Quantity == 1 && ReferenceEquals(round.GetItemType<IHoldable>()!.HeldBy, body) && loaded.Single() != round :
				ReferenceEquals(round.ContainedIn, accepted ? gunItem : null) && ReferenceEquals(round.DirectLocation, accepted ? null : actor.Location) &&
				actor.Location.GameItems.Contains(round) == !accepted && round.GetItemType<IHoldable>()!.HeldBy is null),
				"Load exact participant admission preserves output relocation, independent refill, thrown callback and split conservation.");

			body.CurrentStamina = 100; Require(actor.SetTraitValue(trait, 40), "Persist native reader trait baseline."); world.SaveManager.Flush();
			var canonical = ((ICharacter)actor.GetTrait(trait).Owner).Id;
			var items = new[] { gunItem }.Concat(liveItems).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference,
				x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id, body.WieldedItems.Contains(x), x.Quantity)).ToArray();
			RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, canonical, body.Id,
				actor.CurrentStamina, trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
				gunItem.Id, gunItem.Condition, null, gun.MagazineContents.Select(x => x.Id).ToArray(), items, "load-output-"+scenario), "--firearm-authority-reader");
			Console.WriteLine($"ARMLoadOutput={scenario} passed actual-native-Load-output callback:{callbacks} taken:{taken} exact-{expectedTotal}-total preserved cold-reader extended-test-whitelist");
			using (CommandExecutionScope.EnterIndependent())
			{
				body.Take(gunItem); gunItem.Delete();
				if (!round.Deleted) { round.GetItemType<IHoldable>()!.HeldBy?.Take(round); round.Delete(); }
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
				foreach (var held in unrelated) body.GetWithoutMerge(held);
				world.SaveManager.Flush(); restored(actor);
			}
		}
		return 0;
	}
}

