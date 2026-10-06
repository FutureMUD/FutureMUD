#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void ConfigureAuthorshipCatalogues(TestDatabase database, NativeRuntime native, long writingId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Writings.Find(writingId)!;
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(model.LanguageId);
		language.SetupGet(x => x.Name).Returns("Historical fixture language");
		language.SetupGet(x => x.LinkedTrait).Returns(native.World.Traits.First());
		foreach (var key in new[] { "LiteracySkillId", "HandwritingSkillId", "ForgerySkillId" })
			native.WorldMock.Setup(x => x.GetStaticLong(key)).Returns(native.World.Traits.First().Id);
		var script = new Mock<IScript>();
		script.SetupGet(x => x.Id).Returns(model.ScriptId);
		script.SetupGet(x => x.Name).Returns("Historical fixture script");
		script.SetupGet(x => x.DocumentLengthModifier).Returns(1.0);
		var languages = new All<ILanguage>(); languages.Add(language.Object);
		var scripts = new All<IScript>(); scripts.Add(script.Object);
		native.WorldMock.SetupGet(x => x.Languages).Returns(languages);
		native.WorldMock.SetupGet(x => x.Scripts).Returns(scripts);
		native.WorldMock.SetupGet(x => x.Writings).Returns(new All<IWriting>());
		native.WorldMock.SetupGet(x => x.Drawings).Returns(new All<IDrawing>());
	}

	private static void RunHistoricalAuthorshipChecks(TestDatabase database, FixtureIds fixture, RetirementHost host,
		RuntimeNpc npc, Action<RuntimeNpc> completed)
	{
		var native = host.Native;
		var compositeId = CreateArchiveWriting(database, fixture.CharacterId);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Writings.Find(compositeId)!.TrueAuthorId = npc.Id; db.SaveChanges();
		}
		ConfigureAuthorshipCatalogues(database, native, compositeId);
		var culture = Mock.Get(native.World.NameCultures.First());
		culture.Setup(x => x.NamePattern(NameStyle.FullName)).Returns(Tuple.Create("{0} {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname }));
		culture.Setup(x => x.NamePattern(NameStyle.FullWithNickname)).Returns(Tuple.Create("{0} \"{2}\" {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname, NameUsage.Nickname }));
		npc.PersonalName = new PersonalName(new XElement("Name", new XAttribute("culture", 1),
			new XElement("Element", new XAttribute("usage", "BirthName"), "John"), new XElement("Element", new XAttribute("usage", "Surname"), "Smith"),
			new XElement("Element", new XAttribute("usage", "Nickname"), "Wolf")), native.World);
		npc.CurrentName = npc.PersonalName;
		npc.CurrentWritingLanguage = native.World.Languages.First(); npc.CurrentScript = native.World.Scripts.First();
		var simple = new SimpleWriting(native.World, npc, "The summoned guardian wrote this.", native.Actor);
		CompositeWriting composite;
		using (var db = NewIndependentContext(database.ConnectionString)) composite = new CompositeWriting(db.Writings.Find(compositeId)!, native.World);
		var drawing = new Drawing(npc, 42, "a guardian's sketch", "The guardian's retained sketch.", WritingImplementType.Pencil, DrawingSize.Sketch);
		((All<IWriting>)native.World.Writings).Add(simple); ((All<IWriting>)native.World.Writings).Add(composite);
		((All<IDrawing>)native.World.Drawings).Add(drawing);
		Require(ReferenceEquals(simple.Author, npc) && ReferenceEquals(simple.TrueAuthor, native.Actor) &&
			ReferenceEquals(composite.Author, native.Actor) && ReferenceEquals(composite.TrueAuthor, npc) && ReferenceEquals(drawing.Author, npc),
			"Historical native fixture did not warm distinct author/true-author caches.");
		native.World.SaveManager.Flush();
		var originalIds = new[] { simple.Id, composite.Id, drawing.Id };
		var life = host.Store.Find(ReadNativeNpcLifecycle(database, npc.Id))!;
		Require(npc.Die() is null, "Historical temporary native author produced unconfigured remains.");
		npc.Quit(silent: true); native.World.SaveManager.Flush();
		Require(!npc.IsArchived && host.Roots.CachedActors.Has(npc.Id), "The old host must retain its weakly cached dead author for the process-race check.");
		RunOwnedRetirementReader(new(database.Name, fixture, life.Origin.Id, RuntimeClock.UtcNow, 0, originalIds, "authorship-retire"));
		var loadsBefore = native.WorldMock.Invocations.Count(x => x.Method.Name == nameof(IFuturemud.TryGetCharacter) && (long)x.Arguments[0] == npc.Id);
		Require(!npc.IsArchived && simple.Author is null && composite.TrueAuthor is null && drawing.Author is null &&
			simple.ArchivedAuthor?.CharacterId == npc.Id && composite.ArchivedTrueAuthor?.CharacterId == npc.Id && drawing.ArchivedAuthor?.CharacterId == npc.Id &&
			simple.AuthorName() == "John Smith" && simple.ArchivedAuthor?.DisplayName == "John \"Wolf\" Smith" &&
			ReferenceEquals(simple.TrueAuthor, native.Actor) && ReferenceEquals(composite.Author, native.Actor),
			"A durable archive committed in another process did not override the old host's still-live cached identity.");
		Require(native.WorldMock.Invocations.Count(x => x.Method.Name == nameof(IFuturemud.TryGetCharacter) && (long)x.Arguments[0] == npc.Id) == loadsBefore,
			"Historical author access attempted to materialize the archived NPC in the old host.");
		Console.WriteLine("ARM03B3-cache=passed separate-process-archive-overrides-old-host-cached-author-before-IsArchived-runtime-release live-distinct-trueauthor-preserved no-archived-actor-load");
		life = host.Store.Find(life.Origin.Id)!;
		Require(native.World.CharacterArchives!.TryArchiveNpc(life.Origin.Id, life.Version, npc, RuntimeClock.UtcNow, out var reason), "Old native host archive-release retry failed: " + reason);
		completed(npc);
		var simpleCopy = simple.Copy(); var compositeCopy = composite.Copy(); var drawingCopy = drawing.Copy();
		((All<IWriting>)native.World.Writings).Add(simpleCopy); ((All<IWriting>)native.World.Writings).Add(compositeCopy);
		((All<IDrawing>)native.World.Drawings).Add(drawingCopy);
		native.World.SaveManager.Flush();
		var allIds = originalIds.Concat(new[] { simpleCopy.Id, compositeCopy.Id, drawingCopy.Id }).ToArray();
		AssertHistoricalAuthorship(database, native, fixture.CharacterId, npc.Id, allIds);
		RunOwnedRetirementReader(new(database.Name, fixture, life.Origin.Id, RuntimeClock.UtcNow, 0, allIds, "authorship-read"));
		Console.WriteLine("ARM03B3-authorship=passed actual-SimpleNPCTemplate-created-NPC actual-SimpleWriting-and-Drawing-construction persisted-CompositeWriting-load native-Die-Quit-archive historical-author-and-distinct-trueauthor-retained ordinary-postarchive-copy-and-SaveManager-Flush exact-six-artifact-IDs-content-and-metadata independent-restart-read");
	}

	private static int RunHistoricalAuthorshipReader(TestDatabase database, RetirementHost host, RetirementReader input, SpellOwnedLifecycle life)
	{
		var actorId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter).Id;
		ConfigureAuthorshipCatalogues(database, host.Native, input.Foreign[1]);
		Mock.Get(host.Native.World.NameCultures.First()).Setup(x => x.NamePattern(NameStyle.FullName))
			.Returns(Tuple.Create("{0} {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname }));
		Mock.Get(host.Native.World.NameCultures.First()).Setup(x => x.NamePattern(NameStyle.FullWithNickname))
			.Returns(Tuple.Create("{0} \"{2}\" {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname, NameUsage.Nickname }));
		if (input.Action == "authorship-retire")
		{
			var npc = (RuntimeNpc)host.Native.World.TryGetCharacter(actorId, true);
			Require(npc.State.HasFlag(CharacterState.Dead), "The independent historical reader did not load actual persisted native death.");
			host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
			Require(host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Completed, "Historical author still blocked native retirement.");
			AssertHistoricalAuthorship(database, host.Native, input.Fixture.CharacterId, actorId, input.Foreign);
			Console.WriteLine("ARM03B3-reader-retire=passed separate-owned-process persisted-native-NPC-death-load actual-Quit-and-compaction writing-drawing-history-preserved no-historical-actor-materialization");
			return 0;
		}
		Require(input.Action == "authorship-read" && life.State == SpellLifecycleState.Completed && input.Foreign.Length == 6,
			"The independent historical reader received an unexpected descriptor.");
		AssertHistoricalAuthorship(database, host.Native, input.Fixture.CharacterId, actorId, input.Foreign);
		Require(!host.Roots.Actors.Has(actorId) && !host.Roots.CachedActors.Has(actorId), "Fresh historical-only reads populated a physical author root.");
		Console.WriteLine("ARM03B3-reader-read=passed independent-owned-process exact-original-and-copied-writing-drawing-rows archived-display-distinct-live-trueauthor text-graffiti-drawing-metadata-preserved no-heavy-author-cache");
		return 0;
	}

	private static void AssertHistoricalAuthorship(TestDatabase database, NativeRuntime native, long liveId, long archivedId, long[] ids)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		Require(db.Characters.Find(archivedId) is { IsArchived: true, BodyId: null } && !db.Npcs.Any(x => x.CharacterId == archivedId),
			"Historical attribution restored the archived physical NPC.");
		Require(db.Characters.Find(archivedId)!.Name == "John" && native.World.CharacterArchives!.Find(archivedId)?.DisplayName == "John \"Wolf\" Smith",
			"Historical native name fixture must distinguish persisted given name, full name and nickname-inclusive display.");
		var loadsBefore = native.WorldMock.Invocations.Count(x => x.Method.Name == nameof(IFuturemud.TryGetCharacter) && (long)x.Arguments[0] == archivedId);
		for (var offset = 0; offset < ids.Length; offset += 3)
		{
			var simple = new SimpleWriting(db.Writings.Find(ids[offset])!, native.World);
			var composite = new CompositeWriting(db.Writings.Find(ids[offset + 1])!, native.World);
			var drawing = new Drawing(db.Drawings.Find(ids[offset + 2])!, native.World);
			Require(simple.AuthorId == archivedId && simple.TrueAuthorId == liveId && simple.Author is null && simple.ArchivedAuthor?.CharacterId == archivedId &&
				simple.TrueAuthor?.Id == liveId && simple.AuthorName() == "John Smith" && simple.Text == "The summoned guardian wrote this." &&
				composite.AuthorId == liveId && composite.TrueAuthorId == archivedId && composite.Author?.Id == liveId && composite.TrueAuthor is null &&
				composite.ArchivedTrueAuthor?.CharacterId == archivedId && composite.Text == "Retained historical text." && composite.ShortDescription == "historical graffiti" && composite.DrawingSkill == 35 &&
				drawing.AuthorId == archivedId && drawing.Author is null && drawing.ArchivedAuthor?.CharacterId == archivedId &&
				drawing.FullDescription == "The guardian's retained sketch." && drawing.DrawingSkill == 42 && drawing.DrawingSize == DrawingSize.Sketch && drawing.ImplementType == WritingImplementType.Pencil,
				"Historical copy/reload lost canonical attribution, text, drawing metadata, or distinct live authorship.");
		}
		Require(native.WorldMock.Invocations.Count(x => x.Method.Name == nameof(IFuturemud.TryGetCharacter) && (long)x.Arguments[0] == archivedId) == loadsBefore,
			"A fresh historical read called the physical archived-author loader.");
	}
}
