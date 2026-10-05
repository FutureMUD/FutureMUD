using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits;
using MudSharp.Framework.Scheduling;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record DeviceHostReview(string Database, FixtureIds Fixture, long Item, string Output);
	private sealed record DeviceHostReady(Guid Charge, string Definition);
	private sealed record DeviceHostWinner(string Definition, int Revision);
	private static void PublishReviewFile<T>(string path, T value)
	{
		File.WriteAllText(path + ".writing", JsonSerializer.Serialize(value));
		File.Move(path + ".writing", path);
	}
	private static void AwaitReviewFile(string path, Process? child = null)
	{
		var deadline = DateTime.UtcNow.AddSeconds(50);
		while (!File.Exists(path))
		{
			if (child?.HasExited == true || DateTime.UtcNow >= deadline) throw new InvalidOperationException("Owned second host did not reach its review boundary: " + path);
			Thread.Sleep(100);
		}
	}
	private static int ReadReviewDeviceHost(string argument)
	{
		var input = JsonSerializer.Deserialize<DeviceHostReview>(Encoding.UTF8.GetString(Convert.FromBase64String(argument)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true); DevicePrototypes(host, database); DeviceCounter(host);
		var native = host.Native; var actor = native.Actor;
		using (var db = NewIndependentContext(database.ConnectionString))
			foreach (var model in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => x.FunctionName.StartsWith("armdevReview")))
			{
				var prog = new FutureProg(model, native.World); Require(prog.Compile(), prog.CompileError);
				((All<IFutureProg>)native.World.FutureProgs).Add(prog);
			}
		Mock.Get(native.Body.Race).Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
		var item = native.World.TryGetItem(input.Item, true)!;
		if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
		var bank = (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>();
		Db.GameItemComponent original;
		using (var db = NewIndependentContext(database.ConnectionString)) original = db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id);
		var stalePrototype = new ChargedMagicDeviceGameItemComponent(original, (ChargedMagicDeviceGameItemComponentProto)bank.Prototype, item);
		var token = bank.NextCharge!.Value;
		var service = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow, flush: () => native.World.SaveManager.Flush());
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var refusal = service.ActivateDevice(actor, item, "self");
		Require(refusal.Status == MagicCastingStatus.Refused && bank.Reservation is null && !bank.Changed &&
			Convert.ToDecimal(native.World.VariableRegister.GetValue(actor, "armdev_review_calls").GetObject) == 2, "Second process did not refuse at its final authored callback: " + refusal.Message);
		PublishReviewFile(Path.Combine(input.Output, "ready.json"), new DeviceHostReady(token, bank.PersistedDefinition));
		AwaitReviewFile(Path.Combine(input.Output, "winner.json"));
		var winner = JsonSerializer.Deserialize<DeviceHostWinner>(File.ReadAllText(Path.Combine(input.Output, "winner.json")))!;
		void AssertWinner()
		{
			using var db = NewIndependentContext(database.ConnectionString); var row = db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id);
			Require(row.Definition == winner.Definition && row.GameItemComponentProtoRevision == winner.Revision, "Second host overwrote paid recharge, restored a consumed GUID or changed winner revision.");
		}
		native.World.SaveManager.Flush(); AssertWinner();
		bank.Changed = true; native.World.SaveManager.Flush(); AssertWinner();
		Require(bank.DataError is not null && !bank.Changed, "Ordinary stale save did not quarantine/dequeue the second host.");
		DevicePrototypes(host, database);
		SetPrivateMember(stalePrototype.Prototype, "Status", RevisionStatus.Revised);
		Require(!((IGameItemComponent)stalePrototype).CheckPrototypeForUpdate() && stalePrototype.DataError is not null && !stalePrototype.Changed, "Second host prototype CAS did not quarantine/dequeue stale bank/revision.");
		native.World.SaveManager.Flush(); AssertWinner();
		Console.WriteLine($"ARMDEV-review-P1-two-host=passed second-process:{Environment.ProcessId} final-callback-refusal winner-consumption-paid-recharge loser-native-SaveManager-flush exact-durable-byte-preservation ordinary-save-and-prototype-interface-dispatch no-consumed-GUID-restoration");
		return 0;
	}
	private static VariableRegister DeviceCounter(RetirementHost host)
	{
		var register = new VariableRegister(host.Native.World);
		host.Native.WorldMock.SetupGet(x => x.VariableRegister).Returns(register);
		if (!register.IsRegistered(ProgVariableTypes.Character, "armdev_review_calls"))
			Require(register.RegisterVariable(ProgVariableTypes.Character, ProgVariableTypes.Number, "armdev_review_calls", 0), "Native callback counter registration failed.");
		if (!register.IsRegistered(ProgVariableTypes.Character, "armdev_review_item"))
			Require(register.RegisterVariable(ProgVariableTypes.Character, ProgVariableTypes.Item, "armdev_review_item"), "Native callback item registration failed.");
		register.SetValue(host.Native.Actor, "armdev_review_calls", new NumberVariable(0));
		return register;
	}
	private static FutureProg DeviceReviewProg(RetirementHost host, TestDatabase database, string name, bool itemParameter, string text)
	{
		var parameters = new[] { Tuple.Create(ProgVariableTypes.Character, itemParameter ? "caster" : "target"),
			Tuple.Create(itemParameter ? ProgVariableTypes.Item : ProgVariableTypes.Character, itemParameter ? "item" : "caster") };
		using var db = NewIndependentContext(database.ConnectionString);
		var model = new Db.FutureProg { FunctionName = name, FunctionText = text, FunctionComment = "Owned charged-device review regression.",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Harness", Subcategory = "ChargedDevices", StaticType = (int)FutureProgStaticType.NotStatic };
		for (var n = 0; n < parameters.Length; n++) model.FutureProgsParameters.Add(new() { ParameterIndex = n, ParameterName = parameters[n].Item2, ParameterTypeDefinition = parameters[n].Item1.ToStorageString() });
		db.FutureProgs.Add(model); db.SaveChanges();
		var prog = new FutureProg(host.Native.World, name, ProgVariableTypes.Boolean, parameters, text) { Id = model.Id, StaticType = FutureProgStaticType.NotStatic };
		Require(prog.Compile(), "Native review policy compile failed: " + prog.CompileError);
		((All<IFutureProg>)host.Native.World.FutureProgs).Add(prog); return prog;
	}
	private static string CountedPolicy(int at, string action, bool accepted = true) =>
		$"setregister @caster \"armdev_review_calls\" (getregister(@caster, \"armdev_review_calls\")+1)\nif (getregister(@caster, \"armdev_review_calls\") == {at})\n{action}\nend if\nreturn {(accepted ? "true" : "getregister(@caster, \"armdev_review_calls\") != " + at)}";
	private static void ReviewDeviceNative(TestDatabase database, FixtureIds fixture, RetirementHost host, HarnessClock clock,
		MagicCastingService service, ICharacter staff, MagicSpell source, SkillLevelBasedMagicCapability capability)
	{
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var register = DeviceCounter(host);
		var filter = DeviceReviewProg(host, database, "armdevReviewFilter", false, CountedPolicy(2, "", false));
		var payload = new MagicSpell(source, "ARMDEV Reviewed Blindness"); ((All<IMagicSpell>)world.MagicSpells).Add(payload);
		Require(payload.BuildingCommand(actor, new StringStack("trigger new character")), "Native reviewed trigger failed.");
		Require(payload.BuildingCommand(actor, new StringStack($"trigger set filter {filter.Id}")), "Native reviewed filter failed.");
		Require(capability.BuildingCommand(actor, new StringStack($"casting entry add {payload.Id}")), "Native review admission failed.");
		// Revision tests own a distinct prototype, so the original baseline carrier and its
		// restart evidence cannot acquire a test revision as a side effect.
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var definition = XElement.Parse(host.Prototypes.Values.Single(x => x.Name == "ARMDEV Wand").Components.OfType<ChargedMagicDeviceGameItemComponentProto>().Single().Configuration);
			definition.Add(new XElement("Spell", payload.Id));
			var component = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, Name = "ARMDEV Reviewed Wand", Type = "ChargedMagicDevice", Description = "Owned review carrier",
				Definition = definition.ToString(), EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			var carrier = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMDEV Reviewed Wand", Keywords = "review wand", ShortDescription = "a reviewed charged wand",
				FullDescription = "Owned review fixture", MaterialId = world.Materials.First().Id, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.",
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			carrier.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			carrier.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = db.GameItemComponentProtos.Single(x => x.Type == "Holdable" && x.Name.StartsWith("ARM03B2B")).Id });
			db.GameItemProtos.Add(carrier); db.SaveChanges();
		}
		DevicePrototypes(host, database);
		var proto = host.Prototypes.Values.Single(x => x.Name == "ARMDEV Reviewed Wand").Components.OfType<ChargedMagicDeviceGameItemComponentProto>().Single();
		world.SaveManager.Flush(); Require(service.Grant(staff, actor, capability.Id, payload.Id, "Native review fixture").Allowed, "Native review grant failed.");
		var store = new MagicCastingStateStore(); store.Write(acquired: service.Acquisition(actor, payload.Id)! with { ControlledGrade = 2 });
		var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARMDEV Reviewed Wand").CreateNew(actor);
		world.Add(item); actor.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); native.Body.Get(item, silent: true);
		var bank = (ChargedMagicDeviceGameItemComponent)item.GetItemType<IChargedMagicDevice>();
		register.SetValue(actor, "armdev_review_item", item);
		void Produce(int count)
		{
			var started = service.BeginDeviceProduction(actor, item, capability.Id, payload.Id, 2, count); Require(started.Status == MagicCastingStatus.Started, started.Message);
			clock.Advance(TimeSpan.FromSeconds(count)); Require(service.CompleteDeviceProduction(actor, started.OperationId!.Value).Status == MagicCastingStatus.Succeeded, "Native review production failed.");
		}
		// Empty one hand while preserving the original carrier. This fixture uses its own bank.
		foreach (var held in native.Body.HeldItems.Where(x => !ReferenceEquals(x, item)).ToArray()) { native.Body.Take(held); actor.Location.Insert(held, true); }
		if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
		Produce(2); world.SaveManager.Flush(); FlushCasting(native);
		var output = Path.Combine(Path.GetTempPath(), "armdev-device-twohost_" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(output);
		Console.WriteLine("ARMDEV-two-host-output=" + output);
		var childStart = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		childStart.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); childStart.ArgumentList.Add("--device-review-host");
		childStart.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new DeviceHostReview(database.Name, fixture, item.Id, output)))));
		using var child = Process.Start(childStart)!; var childOutput = child.StandardOutput.ReadToEndAsync(); var childError = child.StandardError.ReadToEndAsync();
		try
		{
			AwaitReviewFile(Path.Combine(output, "ready.json"), child);
			var oldToken = JsonSerializer.Deserialize<DeviceHostReady>(File.ReadAllText(Path.Combine(output, "ready.json")))!.Charge;
			filter.FunctionText = "return true"; Require(filter.Compile(), filter.CompileError);
			var consumed = service.ActivateDevice(actor, item, "self"); Require(consumed.Status == MagicCastingStatus.Succeeded && consumed.OperationId == oldToken, consumed.Message);
			actor.RemoveAllEffects(x => x is MudSharp.Effects.Concrete.SpellEffects.SpellBlindnessEffect, true); Produce(1);
			var winner = bank.PersistedDefinition;
			Require(bank.Charges == 2 && !XElement.Parse(winner).Elements("Charge").Any(x => x.Value == oldToken.ToString()), "Winner did not retain the paid recharge or remove the consumed GUID.");

			// Prototype updates must dispatch through IGameItemComponent, atomically compare old revision
			// and definition, and neither attach a stale EF row nor schedule a generic bank write.
			Db.GameItemComponent row; Db.GameItemComponentProto nextModel;
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				row = db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id);
				nextModel = new() { Id = proto.Id, RevisionNumber = proto.RevisionNumber + 1, Type = "ChargedMagicDevice", Name = proto.Name, Description = "Reviewed revision",
					Definition = proto.Configuration, EditableItem = new() { RevisionNumber = proto.RevisionNumber + 1, BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
				db.GameItemComponentProtos.Add(nextModel);
				db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == proto.Id && x.RevisionNumber == proto.RevisionNumber).EditableItem.RevisionStatus = (int)RevisionStatus.Revised;
				db.SaveChanges();
			}
			var next = new ChargedMagicDeviceGameItemComponentProto(nextModel, world);
			var previousCatalogue = world.ItemComponentProtos;
			SetPrivateMember(proto, "Status", RevisionStatus.Revised);
			var stalePrototype = new ChargedMagicDeviceGameItemComponent(row, proto, item);
			var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
			catalogue.Setup(x => x.GetEnumerator()).Returns(() => new IGameItemComponentProto[] { proto, next }.AsEnumerable().GetEnumerator());
			native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
			Require(((IGameItemComponent)bank).CheckPrototypeForUpdate() && ReferenceEquals(bank.Prototype, next), "Same-host prototype CAS failed.");
			Require(!((IGameItemComponent)stalePrototype).CheckPrototypeForUpdate() && stalePrototype.DataError is not null && !stalePrototype.Changed, "Stale prototype update was not quarantined/dequeued.");
			world.SaveManager.Flush();
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var durable = db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id);
				Require(durable.Definition == winner && durable.GameItemComponentProtoRevision == next.RevisionNumber, "Prototype update or subsequent generic flush overwrote durable bank/revision.");
			}
			Console.WriteLine("ARMDEV-review-P1-prototype=passed actual-interface-virtual-dispatch same-host-atomic-revision-update stale-host-conflict subsequent-native-flush-preserves-bank-and-revision");
			native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(previousCatalogue);
			PublishReviewFile(Path.Combine(output, "winner.json"), new DeviceHostWinner(winner, next.RevisionNumber));
			Require(child.WaitForExit(50000) && child.ExitCode == 0, "Second host regression failed.");
			Console.Write(childOutput.GetAwaiter().GetResult()); Console.Write(childError.GetAwaiter().GetResult());
		}
		finally
		{
			if (!child.HasExited) { child.Kill(true); child.WaitForExit(10000); }
			File.WriteAllText(Path.Combine(output, "second-host.log"), childOutput.GetAwaiter().GetResult() + childError.GetAwaiter().GetResult());
		}
		var activePrototype = (ChargedMagicDeviceGameItemComponentProto)bank.Prototype;

		Db.Merit meritRow;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			meritRow = new() { Name = "ARMDEV Reviewed Capability", Type = "Magic Capability", MeritScope = (int)MeritScope.Character, MeritType = (int)MeritType.Merit,
				Definition = $"<Merit><Capabilities><Capability>{capability.Id}</Capability></Capabilities><DescriptionText>an owned caster merit</DescriptionText></Merit>" };
			db.Merits.Add(meritRow); db.SaveChanges();
		}
		var merit = (IMerit)Activator.CreateInstance(typeof(MudSharp.RPG.Merits.CharacterMerits.MagicCapabilityMerit), BindingFlags.Instance | BindingFlags.NonPublic,
			null, new object[] { meritRow, world }, null)!;
		var merits = new All<IMerit>(); merits.Add(merit); native.WorldMock.SetupGet(x => x.Merits).Returns(merits);
		foreach (var callback in new[] { "usability", "filter" })
		foreach (var mode in new[] { "charged", "focus" })
		foreach (var mutation in new[] { "custody", "entitlement" })
		{
			actor.SetMerits([merit]); if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
			register.SetValue(actor, "armdev_review_calls", new NumberVariable(0));
			var at = mode == "focus" ? 3 : 2;
			var action = mutation == "custody" ? $"silentdrop(@caster, {(callback == "usability" ? "@item" : "getregister(@caster, \"armdev_review_item\")")})" : $"removemerit(@caster, tomerit({merit.Id}))";
			FutureProg policy;
			if (callback == "usability")
			{
				filter.FunctionText = "return true"; Require(filter.Compile(), filter.CompileError);
				policy = DeviceReviewProg(host, database, $"armdevReviewUse_{mode}_{mutation}", true, CountedPolicy(at, action));
				Require(activePrototype.BuildingCommand(actor, new StringStack($"usable {policy.Id}")), "Native usability configuration failed.");
			}
			else
			{
				Require(activePrototype.BuildingCommand(actor, new StringStack("usable none")), "Native usability clear failed.");
				filter.FunctionText = CountedPolicy(at, action); Require(filter.Compile(), filter.CompileError); policy = filter;
			}
			var balance = actor.MagicResourceAmounts[native.Resource]; var beforeBank = bank.PersistedDefinition;
			Dictionary<string, string> journal;
			using (var db = NewIndependentContext(database.ConnectionString)) journal = db.MagicCastingOperations.AsNoTracking().ToDictionary(x => x.Id.ToString(), x => x.Definition + x.Stage);
			var result = mode == "charged" ? service.ActivateDevice(actor, item, "self") : service.CastDeviceFocus(new(actor, capability.Id, payload.Id, 2, false, "self"), item);
			Require(result.Status == MagicCastingStatus.Refused, $"{callback}/{mode}/{mutation} callback was admitted: {result.Message}");
			Require(Convert.ToDecimal(register.GetValue(actor, "armdev_review_calls").GetObject) == at, "The actual authored callback did not run at the final boundary.");
			Require(mutation == "custody" ? !native.Body.HeldOrWieldedItems.Contains(item) : !actor.Capabilities.Any(x => x.Id == capability.Id), "Compiled policy did not actually mutate native custody/entitlement.");
			Require(actor.MagicResourceAmounts[native.Resource] == balance && bank.PersistedDefinition == beforeBank && bank.Reservation is null && !bank.Changed, "Authored mutation caused payment/bank/reservation change.");
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var after = db.MagicCastingOperations.AsNoTracking().ToDictionary(x => x.Id.ToString(), x => x.Definition + x.Stage);
				Require(after.Count == journal.Count && after.All(x => journal.GetValueOrDefault(x.Key) == x.Value) && db.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id).Definition == beforeBank, "Authored callback refusal changed durable bank/journal.");
			}
			Console.WriteLine($"ARMDEV-review-P2=passed compiled-{callback} mode:{mode} mutation:{mutation} final-call:{at} actual-native-mutation refusal no-payment-bank-journal-change");
		}
		actor.SetMerits([merit]); filter.FunctionText = "return true"; Require(filter.Compile(), filter.CompileError);
		Require(activePrototype.BuildingCommand(actor, new StringStack("usable none")), "Native review policy cleanup failed.");
		ReviewDeviceIndirectCallbacks(database, host, clock, service, payload, capability, merit, bank);
		ReviewDeviceTransferredQuarantine(database, fixture, host, clock, service, payload, capability, staff);
		if (native.Body.HeldOrWieldedItems.Contains(item)) { native.Body.Take(item); actor.Location.Insert(item, true); }
		world.SaveManager.Flush(); FlushCasting(native);
	}
}
