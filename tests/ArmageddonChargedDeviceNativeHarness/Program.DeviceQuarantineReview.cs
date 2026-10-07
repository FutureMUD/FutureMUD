using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record DeviceQuarantineReader(string Database, FixtureIds Original, FixtureIds Recipient, long Item,
		long Spell, Guid Failure, string Boundary, string Bank, Guid Next, double MakerBalance, double RecipientBalance,
		Dictionary<string, string> Journal);
	private static NativeHarnessCharacter DeviceRecipient(TestDatabase database, RetirementHost host, FixtureIds recipient)
	{
		var native = host.Native; using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Bodies.Include(x => x.Wounds).ThenInclude(x => x.Infections).Single(x => x.Id == recipient.BodyId);
		var actor = NativeHarnessCharacter.Create(native.World, recipient.CharacterId, native.Actor.Location, native.Actor.Culture);
		var body = new RuntimeBody(model, native.World, actor); actor.AttachBody(body);
		SetPrivateField(body, "_healthTickActive", true); SetPrivateField(body, "_currentBloodVolumeLitres", 5.0);
		body.TotalBloodVolumeLitres = 5.0; body.CalculateOrganFunctions(true);
		var merit = native.World.Merits.GetByName("ARMDEV Reviewed Capability");
		actor.SetMerits([merit ?? NativeRuntime.NewCapabilityMerit(native.World.MagicCapabilities.Get(recipient.CapabilityId)
			?? throw new InvalidOperationException("Owned receiver capability is missing."))]);
		actor.LoadMagic(db.Characters.Include(x => x.CharactersMagicResources).Single(x => x.Id == recipient.CharacterId));
		return actor;
	}
	private static Dictionary<string, string> DeviceJournalBytes(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return db.MagicCastingOperations.AsNoTracking().ToDictionary(x => x.Id.ToString(), x => x.Definition + x.Stage + x.Diagnostic);
	}
	private static void AssertDeviceRecipientRefusal(TestDatabase database, RetirementHost host, NativeHarnessCharacter recipient,
		MagicCastingService service, ChargedMagicDeviceGameItemComponent bank, DeviceQuarantineReader expected)
	{
		var native = host.Native;
		Require(recipient.Id != native.Actor.Id && recipient.Body.Id != native.Body.Id && ReferenceEquals(MagicCastingService.Owner(recipient), recipient) &&
			recipient.Capabilities.Any(x => ReferenceEquals(x, native.World.MagicCapabilities.Get(expected.Recipient.CapabilityId))), "Receiver is not a distinct eligible native identity/body.");
		Require(recipient.Body.HeldOrWieldedItems.Contains(bank.Parent), "Transfer did not establish receiver custody.");
		Require(service.QuarantineReason(recipient, spellId: expected.Spell) is null && service.QuarantineReason(recipient, itemIds: [bank.Parent.Id]) is not null,
			"Fixture did not separate original-owner quarantine from global physical-item quarantine.");
		var result = service.ActivateDevice(recipient, bank.Parent, "self");
		Require(result.Status == MagicCastingStatus.Refused && result.Message.Contains("quarantin", StringComparison.OrdinalIgnoreCase), "Transferred quarantined charge was admitted: " + result.Message);
		Require(bank.PersistedDefinition == expected.Bank && bank.Export().ToString() == expected.Bank && bank.NextCharge == expected.Next && bank.Reservation is null && !bank.Changed &&
			native.Actor.MagicResourceAmounts[native.Resource] == expected.MakerBalance && recipient.MagicResourceAmounts[native.Resource] == expected.RecipientBalance &&
			!native.Actor.EffectsOfType<SpellBlindnessEffect>().Any() && !recipient.EffectsOfType<SpellBlindnessEffect>().Any(), "Refusal mutated bank/GUID/reservation/balance/effects.");
		var journal = DeviceJournalBytes(database); Require(journal.Count == expected.Journal.Count && journal.All(x => expected.Journal.GetValueOrDefault(x.Key) == x.Value), "Transfer refusal changed journal bytes.");
		using var db = NewIndependentContext(database.ConnectionString);
		Require(db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id).Definition == expected.Bank, "Transfer refusal changed durable bank bytes.");
	}
	private static int ReadDeviceQuarantine(string argument)
	{
		var input = JsonSerializer.Deserialize<DeviceQuarantineReader>(File.ReadAllText(argument))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Original, clock, wielding: true, consumablesAnatomy: true); DevicePrototypes(host, database);
		var recipient = DeviceRecipient(database, host, input.Recipient);
		var item = host.Native.World.TryGetItem(input.Item, true)!;
		if (!recipient.Body.HeldOrWieldedItems.Contains(item)) recipient.Body.Get(item, silent: true);
		var service = new MagicCastingService(host.Native.World, clock: () => RuntimeClock.UtcNow, flush: () => host.Native.World.SaveManager.Flush());
		AssertDeviceRecipientRefusal(database, host, recipient, service, (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>(), input);
		Console.WriteLine($"ARMDEV-quarantine-transfer-reload=passed {input.Boundary} process:{Environment.ProcessId} distinct-recipient:{recipient.Id} global-item-refusal exact-bank-GUID-balances-effects-journal");
		return 0;
	}
	private static void ReviewDeviceTransferredQuarantine(TestDatabase database, FixtureIds original, RetirementHost host, HarnessClock clock,
		MagicCastingService normal, MagicSpell payload, SkillLevelBasedMagicCapability capability, ICharacter staff)
	{
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var recipientFixture = FixtureSeed.Create(database, "armdev_quarantine_recipient", false) with
			{ ResourceId = native.Resource.Id, CapabilityId = capability.Id, RoomId = original.RoomId, HealthStrategyId = original.HealthStrategyId, BodypartId = original.BodypartId };
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var body = db.Bodies.Find(recipientFixture.BodyId)!; body.BodyPrototypeId = native.Body.Prototype.Id; body.RaceId = native.Body.Race.Id;
			body.EthnicityId = native.Body.Ethnicity.Id; body.HealthStrategyId = original.HealthStrategyId;
			var character = db.Characters.Find(recipientFixture.CharacterId)!; character.Location = original.RoomId; character.CultureId = actor.Culture.Id;
			db.CharactersMagicResources.RemoveRange(db.CharactersMagicResources.Where(x => x.CharacterId == character.Id));
			db.CharactersMagicResources.Add(new() { CharacterId = character.Id, MagicResourceId = native.Resource.Id, Amount = 75 }); db.SaveChanges();
		}
		var recipient = DeviceRecipient(database, host, recipientFixture);
		foreach (var held in native.Body.HeldItems.ToArray()) { native.Body.Take(held); actor.Location.Insert(held, true); }
		actor.AddResource(native.Resource, 100);
		foreach (var boundary in new[] { "DeviceConsumed", "DeviceFilled" })
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARMDEV Reviewed Wand").CreateNew(actor);
			world.Add(item); actor.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); native.Body.Get(item, silent: true);
			var bank = (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>();
			var fault = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => world.SaveManager.Flush(), checkpoint: stage =>
				{ if (stage == boundary) throw new InvalidOperationException("Owned transferred-quarantine " + boundary); });
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(boundary == "DeviceFilled" ? fault : normal);
			var producer = boundary == "DeviceFilled" ? fault : normal;
			var started = producer.BeginDeviceProduction(actor, item, capability.Id, payload.Id, 2, 2); Require(started.Status == MagicCastingStatus.Started, started.Message);
			clock.Advance(TimeSpan.FromSeconds(2)); var complete = producer.CompleteDeviceProduction(actor, started.OperationId!.Value);
			MagicCastingResult failed;
			if (boundary == "DeviceFilled") failed = complete;
			else { Require(complete.Status == MagicCastingStatus.Succeeded, complete.Message); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(fault); failed = fault.ActivateDevice(actor, item, "self"); }
			Require(failed.Status == MagicCastingStatus.NeedsReview && bank.Charges == (boundary == "DeviceConsumed" ? 1 : 2) && bank.Reservation is null, "Fault did not leave the expected durable usable bank.");
			native.Body.Take(item); actor.Location.Insert(item, true); recipient.Body.Get(item, silent: true); world.SaveManager.Flush();
			var expected = new DeviceQuarantineReader(database.Name, original, recipientFixture, item.Id, payload.Id, failed.OperationId!.Value,
				boundary, bank.PersistedDefinition, bank.NextCharge!.Value, actor.MagicResourceAmounts[native.Resource], recipient.MagicResourceAmounts[native.Resource], DeviceJournalBytes(database));
			AssertDeviceRecipientRefusal(database, host, recipient, fault, bank, expected);
			var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
			var descriptor = Path.Combine(Path.GetTempPath(), "armdev-quarantine-reader-" + Guid.NewGuid().ToString("N") + ".json");
			File.WriteAllText(descriptor, JsonSerializer.Serialize(expected));
			try
			{
				start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--device-quarantine-reader"); start.ArgumentList.Add(descriptor);
				using var child = Process.Start(start)!; var output = child.StandardOutput.ReadToEndAsync(); var error = child.StandardError.ReadToEndAsync();
				if (!child.WaitForExit(60000)) { child.Kill(true); throw new TimeoutException("Owned quarantine receiver reload timed out."); }
				Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
			}
			finally { File.Delete(descriptor); }
			Require(fault.ReconcileOperation(staff, actor, expected.Failure, "Explicit owned transfer-quarantine reconciliation").Allowed, "Explicit reconciliation refused.");
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(normal);
			var released = normal.ActivateDevice(recipient, item, "self"); Require(released.Status == MagicCastingStatus.Succeeded && bank.Charges == (boundary == "DeviceConsumed" ? 0 : 1), "Legitimate receiver remained refused after explicit reconciliation: " + released.Message);
			recipient.RemoveAllEffects<SpellBlindnessEffect>(fireRemovalAction: true); recipient.Body.Take(item); actor.Location.Insert(item, true); world.SaveManager.Flush();
			Console.WriteLine($"ARMDEV-quarantine-transfer=passed {boundary} distinct-native-identity/body immediate-and-reload-refusal exact-state-until-explicit-reconciliation then-legitimate-receiver-release");
		}
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(normal);
	}
}
