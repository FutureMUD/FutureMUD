#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using RuntimeCharacter = MudSharp.Character.Character;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record BodyRetirementReader(string Database, FixtureIds Fixture, long RetiredBody,
		long CorpseItem, long ForeignItem, long OtherReference, long HistoryWound);

	private static object? InvokeCharacterMethod(RuntimeCharacter character, string name, params object?[] arguments)
	{
		try { return typeof(RuntimeCharacter).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(character, arguments); }
		catch (TargetInvocationException ex) when (ex.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); throw;
		}
	}

	private static void LoadRetirementForms(TestDatabase database, NativeRuntime native)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var character = db.Characters.Include(x => x.Body).Include(x => x.CharacterBodies).ThenInclude(x => x.Body)
			.Include(x => x.CharacterBodySources).ThenInclude(x => x.Body).Single(x => x.Id == native.Actor.Id);
		InvokeCharacterMethod(native.Actor, "LoadForms", character);
		foreach (var body in native.Actor.Bodies.Cast<RuntimeBody>())
		{
			body.TotalBloodVolumeLitres = 5;
			SetPrivateField(body, "_currentBloodVolumeLitres", 5.0);
		}
	}

	private static Mock<IGameItem> RetirementItem(NativeRuntime native, long id)
	{
		var item = new Mock<IGameItem>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Gameworld).Returns(native.World);
		item.SetupGet(x => x.Effects).Returns([]);
		item.SetupGet(x => x.Deleted).Returns(false);
		return item;
	}

	private static CorpseGameItemComponent AttachRetirementCorpse(NativeRuntime native, Mock<IGameItem> item,
		Db.GameItemComponent? model = null)
	{
		var prototype = (CorpseGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(CorpseGameItemComponentProto));
		var corpse = model is null ? new CorpseGameItemComponent(prototype, item.Object, true)
			: new CorpseGameItemComponent(model, prototype, item.Object);
		item.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse);
		item.Setup(x => x.GetItemType<IButcherable>()).Returns(corpse);
		item.SetupGet(x => x.Components).Returns([corpse]);
		item.Setup(x => x.Login()).Callback(corpse.Login);
		return corpse;
	}

	private static int RunBodyRetirementAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		using (var schema = NewIndependentContext(database.ConnectionString)) schema.Database.Migrate();
		var fixture = FixtureSeed.Create(database, "arm03_ordinary_backup", true);
		long backupId, corpseId, foreignId, referenceId, historyId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var backup = AddLifecycleBody(db, fixture.BodyId); db.SaveChanges(); backupId = backup.Id;
			foreach (var id in new[] { fixture.BodyId, backupId })
				db.CharacterBodies.Add(new() { CharacterId = fixture.CharacterId, BodyId = id, Alias = $"ordinary{id}", TransformationEcho = "", AllowVoluntarySwitch = true });
			db.CharacterBodySources.Add(new() { CharacterId = fixture.CharacterId, BodyId = fixture.BodyId, SourceType = 0, SourceId = 17, SourceKey = "ordinary-backup-fixture" });
			var corpse = NewLifecycleItem(); var foreign = NewLifecycleItem(); var reference = NewLifecycleItem();
			db.GameItems.AddRange(corpse, foreign, reference);
			var history = (Db.Wound)db.Entry(db.Wounds.Single(x => x.Id == fixture.ExistingWoundId)).CurrentValues.ToObject();
			history.Id = 0; history.BodyId = backupId; history.ActorOriginId = fixture.CharacterId; db.Wounds.Add(history);
			db.SaveChanges(); corpseId = corpse.Id; foreignId = foreign.Id; referenceId = reference.Id; historyId = history.Id;
		}
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true); PrepareLifecycleRuntime(native);
		Mock.Get(native.Body.Race).SetupGet(x => x.BreathingStrategy).Returns(new NonBreather());
		LoadRetirementForms(database, native);
		SetPrivateMember(native.Actor, "QueuedMoveCommands", new Queue<string>());
		SetPrivateMember(native.Actor, "NeedsModel", new Mock<INeedsModel>().Object);
		SetPrivateField(native.Actor, "_riders", new List<ICharacter>());
		var model = new Mock<ICorpseModel>(); model.SetupGet(x => x.Id).Returns(1); model.SetupGet(x => x.CreateCorpse).Returns(true);
		Mock.Get(native.Actor.Body.Race).SetupGet(x => x.CorpseModel).Returns(model.Object);
		var models = new All<ICorpseModel>(); models.Add(model.Object); native.WorldMock.SetupGet(x => x.CorpseModels).Returns(models);
		var parent = RetirementItem(native, corpseId); var corpseComponent = AttachRetirementCorpse(native, parent);
		var items = new All<IGameItem>(); items.Add(parent.Object); native.WorldMock.SetupGet(x => x.Items).Returns(items);
		var itemPrototype = new Mock<IGameItemProto>(); itemPrototype.Setup(x => x.CreateNew(null)).Returns(parent.Object);
		var previousPrototype = CorpseGameItemComponentProto.ItemProto;
		try
		{
			CorpseGameItemComponentProto.ItemProto = itemPrototype.Object;
			native.Actor.AddEffect(new BodyBackupEffect(native.Actor, backupId, native.Actor.Location, native.Actor.RoomLayer,
				1, BodyRemainsContext.SleeveDeath, "ordinary backup fixture", "", "", "", true));
			object?[] transfer = [null];
			Require((bool)InvokeCharacterMethod(native.Actor, "TryTransferToBodyBackupOnDeath", transfer)! &&
				ReferenceEquals(transfer[0], parent.Object) && native.Actor.CurrentBody.Id == backupId,
				"Native ordinary backup-death transfer failed.");
		}
		finally { CorpseGameItemComponentProto.ItemProto = previousPrototype; }
		void SaveOrdinaryRetirement()
		{
			using var scope = FMDB.BeginIsolatedScope();
			using var db = new FMDB();
			var character = FMDB.Context.Characters.Find(fixture.CharacterId)!;
			InvokeCharacterMethod(native.Actor, "SaveForms", character);
			FMDB.Context.GameItemComponents.Add(new() { GameItemId = corpseId,
				Definition = (string)typeof(CorpseGameItemComponent).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(corpseComponent, null)!,
				GameItemComponentProtoId = 0 });
			FMDB.Context.GameItemComponents.Add(new() { GameItemId = referenceId, Definition = $"<Definition><OriginalBody>{fixture.BodyId}</OriginalBody></Definition>", GameItemComponentProtoId = 0 });
			FMDB.Context.SaveChanges();
		}
		using (var db = NewIndependentContext(database.ConnectionString))
			db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03_ordinary_save_refusal BEFORE INSERT ON CharacterBodyRetirements FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'ARM03 ordinary provenance rollback fixture'");
		var refused = false;
		try { SaveOrdinaryRetirement(); }
		catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("ARM03 ordinary provenance rollback fixture") == true) { refused = true; }
		finally
		{
			using var db = NewIndependentContext(database.ConnectionString);
			db.Database.ExecuteSqlRaw("DROP TRIGGER arm03_ordinary_save_refusal");
		}
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(refused && db.Characters.Single(x => x.Id == fixture.CharacterId).BodyId == fixture.BodyId &&
				db.CharacterBodies.Any(x => x.BodyId == fixture.BodyId) && db.CharacterBodySources.Any(x => x.BodyId == fixture.BodyId) &&
				!db.CharacterBodyRetirements.Any() && !db.GameItemComponents.Any(x => x.GameItemId == corpseId),
				"Failed ordinary provenance save changed canonical body, mappings, receipt or remains state.");
		Console.WriteLine("ARM03-ordinary-rollback=passed forced-provider-insert-refusal ordinary-provenance-form-source-canonical-body-and-remains-save-atomic retryable");
		SaveOrdinaryRetirement();
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.Characters.Single(x => x.Id == fixture.CharacterId).BodyId == backupId &&
				!db.CharacterBodies.Any(x => x.BodyId == fixture.BodyId) && !db.CharacterBodySources.Any(x => x.BodyId == fixture.BodyId) &&
				db.CharacterBodyRetirements.Any(x => x.BodyId == fixture.BodyId && x.CharacterId == fixture.CharacterId) &&
				!db.MagicSpellOwnedEntities.Any(), "Native form save did not prune ordinary mappings, or fabricated spell ownership.");
		Console.WriteLine("ARM03-ordinary-save=passed native-backup-death-and-form-save canonical-body-moved ordinary-provenance-persisted retired-form-and-source-mappings-pruned no-spell-ownership");
		var input = new BodyRetirementReader(database.Name, fixture with { BodyId = backupId, ExistingWoundId = historyId },
			fixture.BodyId, corpseId, foreignId, referenceId, historyId);
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--body-retirement-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Ordinary retirement reader exceeded60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
		return 0;
	}

	private static int RunBodyRetirementReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<BodyRetirementReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, false); PrepareLifecycleRuntime(native); LoadRetirementForms(database, native);
		var bodies = new All<IBody>(); bodies.Add(native.Actor.Body); native.WorldMock.SetupGet(x => x.Bodies).Returns(bodies);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IBody>())).Callback<IBody>(body => bodies.Add(body));
		native.WorldMock.Setup(x => x.TryGetCharacter(input.Fixture.CharacterId, It.IsAny<bool>())).Returns(native.Actor);
		var model = new Mock<ICorpseModel>(); model.SetupGet(x => x.Id).Returns(1);
		var models = new All<ICorpseModel>(); models.Add(model.Object); native.WorldMock.SetupGet(x => x.CorpseModels).Returns(models);
		Db.GameItemComponent stored;
		using (var db = NewIndependentContext(database.ConnectionString)) stored = db.GameItemComponents.AsNoTracking().Single(x => x.GameItemId == input.CorpseItem);
		var parent = RetirementItem(native, input.CorpseItem); var corpse = AttachRetirementCorpse(native, parent, stored);
		var items = new All<IGameItem>(); items.Add(parent.Object); native.WorldMock.SetupGet(x => x.Items).Returns(items);
		Require(bodies.Get(input.RetiredBody) is null, "Reader preloaded the retired body instead of exercising ordinary provenance lookup.");
		var retired = corpse.OriginalBody;
		Require(corpse.OriginalBodyId == input.RetiredBody && retired.Id == input.RetiredBody &&
			ReferenceEquals(bodies.Get(input.RetiredBody), retired) && !ReferenceEquals(retired, native.Actor.CurrentBody) &&
			corpse.OriginalCharacter.Id == input.Fixture.CharacterId && !corpse.RepresentsFinalCharacterDeath,
			"Native reload lost the exact ordinary non-final remains references.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// The join remains deliberately unloaded to exercise the database possession guard.
			db.BodiesGameItems.Add(new() { BodyId = input.RetiredBody, GameItemId = input.ForeignItem, EquippedOrder = 0 });
			db.SaveChanges();
		}
		Require(!native.Actor.TryCleanupRetiredBody(retired, parent.Object), "Persisted foreign possession did not block ordinary cleanup.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.GameItems.Any(x => x.Id == input.ForeignItem) && db.BodiesGameItems.Any(x => x.GameItemId == input.ForeignItem && x.BodyId == input.RetiredBody),
				"Refused ordinary cleanup changed the foreign item or its body join.");
			db.BodiesGameItems.Remove(db.BodiesGameItems.Single(x => x.GameItemId == input.ForeignItem));
			db.CellsGameItems.Add(new() { GameItemId = input.ForeignItem, CellId = input.Fixture.CellId }); db.SaveChanges();
		}
		Require(!native.Actor.TryCleanupRetiredBody(retired, parent.Object), "Another persisted physical reference did not block ordinary cleanup.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.Bodies.Any(x => x.Id == input.RetiredBody), "Reference-blocked ordinary body disappeared.");
			db.GameItems.Remove(db.GameItems.Find(input.OtherReference)!); db.SaveChanges();
		}
		Console.WriteLine("ARM03-ordinary-guards=passed separate-process-real-corpse-reload resolves-exact-retired-body-without-preload unloaded-foreign-item-and-other-reference-refuse intact-possession explicitly-rehomed");
		corpse.Delete();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(!db.Bodies.Any(x => x.Id == input.RetiredBody),
				"Ordinary backup body survived final remains release after saved mapping removal and separate-process reload.");
			Require(!db.CharacterBodyRetirements.Any(x => x.BodyId == input.RetiredBody), "Eligible cleanup left ordinary retirement metadata behind.");
			Require(db.Characters.Single(x => x.Id == input.Fixture.CharacterId).BodyId == input.Fixture.BodyId &&
				db.Bodies.Any(x => x.Id == input.Fixture.BodyId) && db.Wounds.Single(x => x.Id == input.HistoryWound).ActorOriginId == input.Fixture.CharacterId &&
				db.CellsGameItems.Any(x => x.GameItemId == input.ForeignItem && x.CellId == input.Fixture.CellId) && !db.MagicSpellOwnedEntities.Any(),
				"Ordinary retirement changed canonical identity, current body, foreign possessions or historical attribution.");
			db.GameItems.Remove(db.GameItems.Find(input.CorpseItem)!); db.SaveChanges();
		}
		Console.WriteLine("ARM03-ordinary-release=passed native-CorpseGameItemComponent.Delete reference-free-ordinary-body-retired canonical-current-body-history-and-foreign-goods-preserved no-spell-ownership");
		return 0;
	}
}
