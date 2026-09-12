using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Framework;
using MudSharp.Magic.Vancian;
using Prog = MudSharp.FutureProg.FutureProg;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class VancianPersistenceReviewTests
{
	[TestMethod]
	public void RegisterTimestamp_RoundTripPreservesUtcPrecisionAndLegacyValuesRemainUtc()
	{
		var timestamp = new DateTime(2026, 9, 12, 3, 4, 5, DateTimeKind.Utc).AddTicks(1234567);
		var value = new VariableRegister.ValueVariableValue { Type = ProgVariableTypes.DateTime, Value = timestamp };
		var world = new VancianTestFixture().World.Object;
		var restored = new VariableRegister.ValueVariableValue(value.SaveToXml(), ProgVariableTypes.DateTime, world);
		Assert.AreEqual(timestamp, restored.Value); Assert.AreEqual(DateTimeKind.Utc, ((DateTime)restored.Value).Kind);
		var legacy = new VariableRegister.ValueVariableValue(new System.Xml.Linq.XElement("var", "09/12/2026 03:04:05"), ProgVariableTypes.DateTime, world);
		Assert.AreEqual(new DateTime(2026, 9, 12, 3, 4, 5, DateTimeKind.Utc), legacy.Value);
		Assert.AreEqual(DateTimeKind.Utc, ((DateTime)legacy.Value).Kind);
	}
	private sealed class RegisterDatabase : IDisposable
	{
		private static readonly PropertyInfo ContextProperty = typeof(FMDB).GetProperty("Context")!;
		private static readonly PropertyInfo CountProperty = typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!;
		private readonly object? _previousContext = ContextProperty.GetValue(null);
		private readonly object? _previousCount = CountProperty.GetValue(null);
		private readonly DbContextOptions<FuturemudDatabaseContext> _options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
		private FuturemudDatabaseContext? _context;
		public RegisterDatabase() => Reload();
		public void Reload()
		{
			_context?.Dispose(); _context = new FuturemudDatabaseContext(_options);
			ContextProperty.SetValue(null, _context); CountProperty.SetValue(null, 1u);
		}
		public void Dispose()
		{
			ContextProperty.SetValue(null, _previousContext); CountProperty.SetValue(null, _previousCount); _context?.Dispose();
		}
	}

	private static MemoryVancianStore ReloadStore(MemoryVancianStore source)
	{
		var target = new MemoryVancianStore();
		foreach (var (key, state) in source.States) target.States[key] = state.Copy();
		foreach (var (key, operation) in source.Log) target.Log[key] = operation;
		return target;
	}

	[DataTestMethod]
	[DataRow("Known", "success")]
	[DataRow("Known", "save")]
	[DataRow("Known", "completion")]
	[DataRow("Refresh", "success")]
	[DataRow("Refresh", "save")]
	[DataRow("Refresh", "completion")]
	[DataRow("Inscription", "success")]
	[DataRow("Inscription", "save")]
	[DataRow("Inscription", "completion")]
	public void Callback_RealRegisterAndIndependentReloadAgreeWithDurableCompletion(string kind, string failure)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		using var database = new RegisterDatabase();
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1;
		f.Select(2); f.Plan(2); f.Refresh();
		var saver = new SaveManager();
		var saveBoundary = new Mock<ISaveManager>();
		saveBoundary.Setup(x => x.Add(It.IsAny<ISaveable>())).Callback<ISaveable>(saver.Add);
		saveBoundary.Setup(x => x.IsQueued(It.IsAny<ISaveable>())).Returns<ISaveable>(saver.IsQueued);
		f.World.SetupGet(x => x.SaveManager).Returns(saveBoundary.Object);
		var register = new VariableRegister(f.World.Object); f.World.SetupGet(x => x.VariableRegister).Returns(register);
		Assert.IsTrue(register.RegisterVariable(ProgVariableTypes.Character, ProgVariableTypes.DateTime, "cooldown")); saver.Flush();
		var signatureName = kind == "Known" ? "onchangeknown" : kind == "Refresh" ? "onrefresh" : "oninscribe";
		var signature = VancianPolicy.Signatures[signatureName];
		var model = new MudSharp.Models.FutureProg { Id = 1000, FunctionName = "recordcooldown", ReturnTypeDefinition = signature.Return.ToStorageString(),
			FunctionText = "setregister @owner \"cooldown\" now()\nreturn" };
		foreach (var (type, index) in signature.Parameters.Select((type, i) => (type, i)))
			model.FutureProgsParameters.Add(new() { ParameterIndex = index, ParameterName = index == 0 ? "owner" : $"arg{index}", ParameterTypeDefinition = type.ToStorageString() });
		var prog = new Prog(model, f.World.Object);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		f.World.SetupGet(x => x.FutureProgs).Returns(VancianTestFixture.Collection<IFutureProg>(() => f.Progs.Select(x => x.Object).Append(prog)));
		f.Policies[signatureName] = prog.Id;
		DateTime? executedValue = null;
		saveBoundary.Setup(x => x.Flush()).Callback(() =>
		{
			if (register.Changed)
			{
				executedValue = (DateTime)register.GetValue(f.Actor.Object, "cooldown").GetObject;
				Assert.AreEqual("Invoking", f.Store.Log.Values.Single(x => x.Kind == kind && x.Status == "Invoking").Status);
				if (failure == "save") throw new InvalidOperationException("Injected register save failure");
			}
			saver.Flush();
		});
		f.Store.BeforeRecord = operation =>
		{
			if (operation.Kind == kind && operation.Status == "Completed" && failure == "completion")
				throw new InvalidOperationException("Injected failure after saving bookkeeping, before acknowledging completion");
		};
		if (kind == "Known") f.Select(3);
		else if (kind == "Refresh") f.Refresh();
		else
		{
			var scroll = items.Scroll(100);
			var begin = f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, scroll.Item.Object);
			Assert.IsTrue(begin.Success, begin.Message); f.Clock.Advance(TimeSpan.FromSeconds(1));
			Assert.IsTrue(f.Service.CompleteWriting(f.Actor.Object, begin.OperationId!.Value).Success);
		}
		Assert.IsNotNull(executedValue);
		var operation = f.Store.Log.Values.Last(x => x.Kind == kind);
		Assert.AreEqual(failure == "success" ? "Completed" : "NeedsReview", operation.Status);
		var reloadedStore = ReloadStore(f.Store); database.Reload();
		var reloadedRegister = new VariableRegister(f.World.Object);
		var restoredValue = reloadedRegister.GetValue(f.Actor.Object, "cooldown")?.GetObject;
		if (failure != "save") Assert.AreEqual(executedValue.Value, restoredValue);
		else Assert.AreNotEqual(executedValue.Value, restoredValue);
		Assert.AreEqual(failure != "success", reloadedStore.HasUnresolved(10, 20, kind));
		Assert.AreEqual(f.State.Generation, reloadedStore.Read(10, 20).Generation);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Restart_AbandonedPrecommitWritingReleasesExactSlotAndItemOnce(bool transcription)
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1; f.Select(2); f.Plan(2); f.Refresh();
		var scroll = items.Scroll(100); var book = items.Book(101);
		if (transcription) items.Charge(scroll.Component);
		var start = transcription
			? f.Service.BeginTranscription(f.Actor.Object, f.Capability.Object, scroll.Item.Object, items.Spell, book.Item.Object)
			: f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, scroll.Item.Object);
		Assert.IsTrue(start.Success, start.Message);
		var store = ReloadStore(f.Store);
		var reloaded = new SpellScrollGameItemComponent(new() { Id = 100, Definition = VancianItemTests.Serialize(scroll.Component) }, (SpellScrollGameItemComponentProto)scroll.Component.Prototype, scroll.Item.Object);
		scroll.Item.Setup(x => x.GetItemType<ISpellScroll>()).Returns(reloaded);
		f.World.Setup(x => x.TryGetItem(100, true)).Returns(scroll.Item.Object);
		var itemSaves = 0; var service = new VancianMagicService(f.World.Object, store, f.Clock, persistComponent: _ => itemSaves++);
		var state = service.State(f.Actor.Object, f.Capability.Object);
		Assert.AreEqual("Cancelled", store.Operation(start.OperationId!.Value)!.Status);
		Assert.AreEqual(VancianSlotStatus.Prepared, state.Slots.Single().Status); Assert.IsNull(reloaded.Reservation);
		Assert.AreEqual(transcription, reloaded.IsCharged); Assert.AreEqual(0, book.Component.Formulae.Count);
		var writes = store.Writes; service.State(f.Actor.Object, f.Capability.Object);
		Assert.AreEqual(writes, store.Writes); Assert.AreEqual(1, itemSaves);
		Assert.IsFalse(service.CompleteWriting(f.Actor.Object, start.OperationId.Value).Success);
	}

	[DataTestMethod]
	[DataRow("Committing")]
	[DataRow("Consumed")]
	[DataRow("NeedsReview")]
	public void Restart_PostcommitOrIndeterminateWritingNeverRefundsOrCreatesCharge(string status)
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1; f.Select(2); f.Plan(2); f.Refresh(); var scroll = items.Scroll(100);
		var begin = f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, scroll.Item.Object);
		var operation = f.Store.Operation(begin.OperationId!.Value)!;
		f.Store.Record(operation with { Status = status });
		var store = ReloadStore(f.Store); var service = new VancianMagicService(f.World.Object, store, f.Clock, persistComponent: _ => Assert.Fail("Postcommit reservation was changed"));
		Assert.AreEqual(VancianSlotStatus.Reserved, service.State(f.Actor.Object, f.Capability.Object).Slots.Single().Status);
		Assert.AreEqual(status, store.Operation(operation.Id)!.Status); Assert.IsTrue(scroll.Component.IsBlank); Assert.AreEqual(operation.Id, scroll.Component.Reservation);
	}
}
