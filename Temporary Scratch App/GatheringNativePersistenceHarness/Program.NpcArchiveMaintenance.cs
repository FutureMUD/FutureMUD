#nullable enable

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunNpcArchiveMaintenanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03B-maintenance-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03b_maintenance", true);
		using (var db = NewIndependentContext(database.ConnectionString))
			foreach (var sql in new[] { "ALTER TABLE Characters AUTO_INCREMENT=1000000", "ALTER TABLE Bodies AUTO_INCREMENT=2000000", "ALTER TABLE CharacterInstances AUTO_INCREMENT=3000000" })
				db.Database.ExecuteSqlRaw(sql);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true); PrepareLifecycleRuntime(native);
		var roots = ArchiveRoots(); ((All<ICharacter>)roots.Characters).Add(native.Actor);
		ConfigureArchiveWorld(native, roots, fixture, database.ConnectionString);
		// Controlled catalogues do not qualify deferred initialization saves; the command's
		// persistence boundary remains real EF/MySQL while its preflight flush host is explicit.
		native.WorldMock.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var archives = new CharacterArchiveService(); native.WorldMock.SetupGet(x => x.CharacterArchives).Returns(archives);
		var templateId = CreateArchiveTemplate(database);
		var template = new Mock<INPCTemplate>(); template.SetupGet(x => x.Id).Returns(templateId);
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>(); templates.Setup(x => x.Get(templateId, 0)).Returns(template.Object);
		native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		var spell = new MagicSpell("ARM03B maintenance archive", native.Capability.School);
		var store = new SpellOwnedLifecycleStore(); var now = DateTime.UtcNow.AddMinutes(-5);
		SpellOwnedLifecycle NewDeadSpellNpc(out MudSharp.NPC.NPC npc)
		{
			var origin = new SpellLifecycleOrigin(Guid.NewGuid(), spell.Id, 3, fixture.CharacterId, "archive-maintenance-fixture",
				SpellLifecycleMode.TemporaryCleanup, now, now.AddMinutes(1), "Controlled maintenance boundary; native template callback adapter deferred");
			var life = CreateArchiveNpc(store, origin, fixture, templateId); npc = LoadArchiveNpc(native, roots, life);
			life = store.BeginRetirement(life.Origin.Id, life.Version, SpellRetirementReason.Expiry, now.AddMinutes(1));
			Require(npc.Die() is null, "Maintenance fixture race unexpectedly produced a corpse.");
			return store.ObserveDeath(life.Origin.Id, life.Version, null, now.AddMinutes(2));
		}
		var held = NewDeadSpellNpc(out var heldNpc); var eligible = NewDeadSpellNpc(out var eligibleNpc);
		long ordinaryId, ordinaryBody, offlineId, historyId, heldItem;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Db.Character CopyCharacter(string name)
			{
				var source = db.Characters.AsNoTracking().Single(x => x.Id == fixture.CharacterId);
				var identity = (Db.Character)db.Entry(source).CurrentValues.ToObject(); identity.Id = 0;
				identity.Body = AddLifecycleBody(db, fixture.BodyId); identity.BodyId = 0; identity.Name = name;
				identity.NeedsModel = "NoNeeds"; identity.IsArchived = false; db.Characters.Add(identity); return identity;
			}
			var ordinary = CopyCharacter("Unqualified ordinary dead NPC"); ordinary.State = (int)CharacterState.Dead;
			ordinary.Status = (int)CharacterStatus.Deceased; ordinary.DeathTime = now;
			db.Npcs.Add(new() { Character = ordinary, TemplateId = templateId });
			var offline = CopyCharacter("Maintenance temporary offline PC");
			offline.Status = (int)CharacterStatus.Active; offline.State = (int)CharacterState.Awake; offline.DeathTime = null;
			db.SaveChanges(); ordinaryId = ordinary.Id; ordinaryBody = ordinary.BodyId!.Value; offlineId = offline.Id;
			var history = new Db.CharacterLog { CharacterId = ordinaryId, RoomId = fixture.RoomId, Command = "maintenance history sentinel", Time = now };
			db.Set<Db.CharacterLog>().Add(history); db.Wounds.Find(fixture.ExistingWoundId)!.ActorOriginId = ordinaryId;
			var item = NewLifecycleItem(); db.GameItems.Add(item); db.SaveChanges(); heldItem = item.Id; historyId = history.Id;
			db.GameItemComponents.Add(new() { GameItemId = heldItem, Definition = $"<Definition><OriginalBody>{heldNpc.Body.Id}</OriginalBody></Definition>" }); db.SaveChanges();
		}
		var statistics = new Mock<IGameStatistics>(); statistics.SetupProperty(x => x.RecordPlayersPaused, false);
		native.WorldMock.SetupGet(x => x.GameStatistics).Returns(statistics.Object);
		var offlinePc = new Mock<ICharacter>(); offlinePc.SetupGet(x => x.Id).Returns(offlineId);
		// ICharacter is equatable; a loose mock's false default would leave it in All's iteration list after Remove.
		offlinePc.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(other => ReferenceEquals(other, offlinePc.Object));
		var quitCount = 0; offlinePc.Setup(x => x.Quit(true)).Callback(() =>
		{
			quitCount++; ((All<ICharacter>)roots.Characters).Remove(offlinePc.Object);
		}).Returns(true);
		var actors = new Dictionary<long, ICharacter> { [fixture.CharacterId] = native.Actor, [offlineId] = offlinePc.Object,
			[heldNpc.Id] = heldNpc, [eligibleNpc.Id] = eligibleNpc };
		native.WorldMock.Setup(x => x.TryGetCharacter(It.IsAny<long>(), true)).Returns<long, bool>((id, _) => actors.GetValueOrDefault(id)!);
		native.WorldMock.Setup(x => x.Add(It.IsAny<ICharacter>(), false)).Callback<ICharacter, bool>((pc, _) => ((All<ICharacter>)roots.Characters).Add(pc));
		var items = new All<IGameItem>(); var parent = new Mock<IGameItem>(); var corpse = new Mock<ICorpse>(); var corpseDeleted = false;
		parent.SetupGet(x => x.Id).Returns(9000000);
		parent.SetupGet(x => x.Deleted).Returns(() => corpseDeleted); parent.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse.Object);
		parent.Setup(x => x.Delete()).Callback(() => corpseDeleted = true); corpse.SetupGet(x => x.Parent).Returns(parent.Object);
		corpse.SetupGet(x => x.OriginalCharacter).Returns(native.Actor); corpse.SetupGet(x => x.Body).Returns(native.Actor.Body);
		corpse.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(true); items.Add(parent.Object);
		native.WorldMock.SetupGet(x => x.Items).Returns(items);
		var reports = new List<string>(); native.WorldMock.Setup(x => x.SystemMessage(It.IsAny<string>(), It.IsAny<bool>()))
			.Callback<string, bool>((message, _) => reports.Add(message));
		var founder = new Mock<ICharacter>(); founder.SetupGet(x => x.Gameworld).Returns(native.World);
		founder.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Accept? proposal = null; founder.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => proposal = (Accept)effect);
		ISchedule? schedule = null; Mock.Get(native.World.Scheduler).Setup(x => x.AddSchedule(It.IsAny<ISchedule>())).Callback<ISchedule>(value => schedule = value);
		typeof(ImplementorModule).GetMethod("DebugCleanupCorpses", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [founder.Object]);
		proposal!.Proposal.Accept(); Require(schedule is not null, "The real maintenance proposal did not schedule its callback.");
		Exception? failure = null; try { schedule!.Fire(); } catch (Exception ex) { failure = ex; }
		Console.WriteLine($"ARM03B-maintenance-observation=exception:{failure?.GetType().Name ?? "none"} prior-corpse-delete:{corpseDeleted} offline-quits:{quitCount} paused:{statistics.Object.RecordPlayersPaused}");
		Require(failure is null, "Maintenance threw before temporary PC cleanup: " + failure);
		Require(corpseDeleted && quitCount == 1 && !statistics.Object.RecordPlayersPaused && !roots.Characters.Has(offlineId) &&
			!roots.Characters.Any(x => x.Id == offlineId),
			"Successful maintenance did not release its temporary offline PC and restore statistics.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.Characters.Find(ordinaryId) is { IsArchived: false, BodyId: var retainedBody } && retainedBody == ordinaryBody &&
				db.Npcs.Any(x => x.CharacterId == ordinaryId) && db.Bodies.Any(x => x.Id == ordinaryBody) &&
				db.Set<Db.CharacterLog>().Find(historyId)!.CharacterId == ordinaryId && db.Wounds.Find(fixture.ExistingWoundId)!.ActorOriginId == ordinaryId,
				"Ordinary unqualified identity, body or history changed during maintenance.");
			Require(db.Characters.Find(heldNpc.Id) is { IsArchived: false } && db.Npcs.Any(x => x.CharacterId == heldNpc.Id) &&
				db.Bodies.Any(x => x.Id == heldNpc.Body.Id) && db.GameItems.Any(x => x.Id == heldItem) &&
				db.GameItemComponents.Any(x => x.GameItemId == heldItem) && !string.IsNullOrEmpty(store.Find(held.Origin.Id)!.Diagnostic),
				"Held NPC or foreign serialized reference changed or lost its durable hold.");
			Require(db.Characters.Find(eligibleNpc.Id) is { IsArchived: true, BodyId: null } && db.CharacterArchives.Any(x => x.CharacterId == eligibleNpc.Id) &&
				!db.Npcs.Any(x => x.CharacterId == eligibleNpc.Id) && !db.Bodies.Any(x => x.Id == eligibleNpc.Body.Id), "Eligible native NPC was not archived through the proven boundary.");
		}
		Require(reports.Any(x => x.Contains("Retained dead NPC #" + ordinaryId)) && reports.Any(x => x.Contains("Retained dead NPC #" + heldNpc.Id)) &&
			reports.Any(x => x.Contains("Archived 1 eligible spell-owned dead NPCs; retained 2 dead NPCs; failed 0 archival attempts")), "Maintenance output did not truthfully distinguish retained and archived NPCs.");
		Console.WriteLine("ARM03B-maintenance-retention=passed real-proposal-and-scheduled-maintenance ordinary-dead-NPC-body-and-history-retained held-spell-NPC-and-serialized-foreign-reference-retained durable-diagnostic no-FK-exception");
		Console.WriteLine("ARM03B-maintenance-eligible=passed exactly-one-proven-native-NPC-archived protective-schema-unchanged no-new-lifecycle-or-death-proof");
		Console.WriteLine("ARM03B-maintenance-cleanup=passed prior-corpse-delete-completed truthful-retention-and-archive-report temporary-offline-PC-quit statistics-restored");
		Exception RunMaintenance(string method)
		{
			try
			{
				typeof(ImplementorModule).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [founder.Object]);
				if (method == "DebugCleanupCorpses") { proposal!.Proposal.Accept(); schedule!.Fire(); }
			}
			catch (TargetInvocationException ex) { return ex.InnerException!; }
			catch (Exception ex) { return ex; }
			throw new InvalidOperationException("Injected maintenance failure did not reach the real command.");
		}
		void CleanupAfterFault(string fault, int expectedQuits)
		{
			Require(quitCount == expectedQuits && !statistics.Object.RecordPlayersPaused && !roots.Characters.Has(offlineId) &&
				!roots.Characters.Any(x => x.Id == offlineId), "Temporary PC cleanup failed after " + fault);
		}
		offlinePc.Setup(x => x.Register(It.IsAny<IOutputHandler>())).Throws(new InvalidOperationException("owned registration fault"));
		Require(RunMaintenance("DebugCleanupCorpses").Message == "owned registration fault", "Wrong registration failure.");
		CleanupAfterFault("registration", 2);
		Console.WriteLine("ARM03B-maintenance-registration-finally=passed partial-preload-actor-tracked-before-register temporary-PC-quit statistics-restored original-error-preserved");
		offlinePc.Setup(x => x.Register(It.IsAny<IOutputHandler>()));
		Mock.Get(native.World.ExitManager).Setup(x => x.PreloadCriticalExits()).Throws(new InvalidOperationException("owned exit preload fault"));
		Require(RunMaintenance("DebugCleanupCorpses").Message == "owned exit preload fault", "Wrong exit preload failure.");
		CleanupAfterFault("exit preload", 3);
		Console.WriteLine("ARM03B-maintenance-preload-finally=passed real-preload-failure after-offline-PC-added temporary-PC-quit statistics-restored original-error-preserved");
		Mock.Get(native.World.ExitManager).Setup(x => x.PreloadCriticalExits());
		native.WorldMock.Setup(x => x.TryGetItem(It.IsAny<long>(), true)).Throws(new InvalidOperationException("owned orphan report fault"));
		Require(RunMaintenance("DebugOrphans").Message == "owned orphan report fault", "Wrong orphan reporting failure.");
		CleanupAfterFault("orphan report", 4);
		Console.WriteLine("ARM03B-maintenance-orphans-finally=passed related-active-orphan-report-failure temporary-PC-quit statistics-restored disabled-destructive-item-cleanup-unchanged");
		offlinePc.Setup(x => x.Register(It.IsAny<IOutputHandler>())).Throws(new InvalidOperationException("owned registration fault"));
		foreach (var quitThrows in new[] { false, true })
		{
			offlinePc.Setup(x => x.Quit(true)).Returns(() =>
			{
				quitCount++; ((All<ICharacter>)roots.Characters).Remove(offlinePc.Object);
				if (quitThrows) throw new InvalidOperationException("owned temporary PC quit fault");
				return false;
			});
			var combinedFailure = RunMaintenance("DebugCleanupCorpses") as AggregateException;
			var causes = combinedFailure?.Flatten().InnerExceptions;
			Require(causes is { Count: 2 } && causes.Any(x => x.Message == "owned registration fault") &&
				causes.Any(x => x.Message == (quitThrows ? "owned temporary PC quit fault" : $"Temporary PC #{offlineId} did not quit.")),
				"Combined preload and temporary PC cleanup failure lost one of its causes.");
			CleanupAfterFault("combined preload and quit failure", quitThrows ? 6 : 5);
			Console.WriteLine($"ARM03B-maintenance-combined-{(quitThrows ? "throw" : "false")}=passed real-partial-preload original-and-cleanup-causes-preserved temporary-PC-quit-attempt statistics-restored");
		}
		Console.WriteLine("ARM03B-maintenance-qualifier=controlled-world-save-flush-host offline-PC-and-corpse-item-hosts real-EF-production-NPC-Die-and-scheduled-command no-installed-server creation-remains-evacuation-adapters-and-full-N14-N15-N16-NOT-RUN");
		return 0;
	}
}
