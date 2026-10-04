#nullable enable
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SavedCorpseParentReader(string Database, FixtureIds Fixture, DateTime Now, Guid Origin,
		long Instance, long Corpse, long Owner, long Body, long Foreign, DateTime Deadline, long Spell, string Action, string EffectHash, int MorphSeconds);
	private static string SavedEffectHash(string xml) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(xml)));

	private static int RunSavedCorpseAnimationParentChecks(TestDatabase database, FixtureIds fixture, RetirementHost host,
		HarnessClock clock, MagicSpell spell, ICharacter owner, IGameItem corpse, IGameItem foreign,
		ScriptedAiCharacterInstance animated, SpellOwnedLifecycle life, string action)
	{
		host.Native.World.SaveManager.Flush();
		string paidXml;
		int morphSeconds;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var row = db.GameItems.Single(x => x.Id == corpse.Id);
			Require(row.MorphTimeRemaining > 0, "Paid corpse must retain its real persisted morph duration.");
			morphSeconds = row.MorphTimeRemaining!.Value;
			paidXml = row.EffectData;
		}
		var xml = XElement.Parse(paidXml);
		Require(xml.Elements("Effect").Single().Element("Type")!.Value == "MagicSpellParent" &&
			xml.Descendants("OwnedLifecycleId").Single().Value == life.Origin.Id.ToString() &&
			(DateTime)xml.Descendants("ExpiryUtc").Single() == life.Origin.DeadlineUtc, "Actual paid parent/child XML was not persisted.");
		SavedCorpseParentReader Reader(string next) => new(database.Name, fixture,
			next == "active-expired-boot-recovery" ? life.Origin.DeadlineUtc!.Value.AddSeconds(1) : RuntimeClock.UtcNow, life.Origin.Id,
			animated.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id, life.Origin.DeadlineUtc!.Value, spell.Id, next, SavedEffectHash(paidXml), morphSeconds);
		Console.WriteLine("ARM03D1P1-paid-parent-saved=passed actual-paid-grade3-parent-child native-SaveManager-Flush persisted-origin-and-absolute-deadline positive-morph-duration-retained");
		if (action.StartsWith("active-"))
		{
			// The producer stays quiescent while the reader retires this still-active paid lifecycle.
			RunItemReaderProcess(Reader(action), "--corpse-animation-saved-parent-reader");
			using var db = NewIndependentContext(database.ConnectionString);
			Require(host.Store.Find(life.Origin.Id)!.State == SpellLifecycleState.Completed &&
				!db.CharacterInstances.Any(x => x.Id == animated.InstanceId) &&
				!XElement.Parse(db.GameItems.Single(x => x.Id == corpse.Id).EffectData).Descendants("OwnedLifecycleId").Any(),
				"Active paid reader did not persist exact retirement and stale XML removal.");
			Console.WriteLine($"ARM03D1P1M-{action}-durable=passed actual-paid-active-journal fresh-process retirement-and-cleanup-saved producer-quiescent");
			return 0;
		}
		RunItemReaderProcess(Reader("active-boot-load"), "--corpse-animation-saved-parent-reader");
		using (var db = NewIndependentContext(database.ConnectionString))
			db.Database.ExecuteSqlRaw($"CREATE TRIGGER arm03d1p1_complete_fault BEFORE UPDATE ON MagicSpellLifecycles FOR EACH ROW BEGIN IF OLD.Id='{life.Origin.Id:D}' AND NEW.State=3 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03D1P1 completion checkpoint refusal'; END IF; END");
		try { Require(!host.Native.World.SpellOwnedCorpseAnimations!.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out _), "Completion fault did not hold the committed restoration."); }
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03d1p1_complete_fault"); }
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!db.CharacterInstances.Any(x => x.Id == animated.InstanceId) && db.CellsGameItems.Count(x => x.GameItemId == corpse.Id) == 1 &&
				db.GameItems.Single(x => x.Id == corpse.Id).EffectData == paidXml && host.Store.Find(life.Origin.Id)!.State != SpellLifecycleState.Completed &&
				host.Store.Find(life.Origin.Id)!.Diagnostic.StartsWith("<CorpseRestore"), "Restoration did not commit while retaining the genuine stale paid XML.");
		Console.WriteLine("ARM03D1P1-restoration-committed=passed actual-restoration-transaction secondary-row-removed corpse-cell-linked completion-trigger-refusal paid-XML-still-present no-post-retirement-producer-flush");
		// The producer is now quiescent: its pending item saves must not overwrite a reader.
		foreach (var next in new[] { "pending-runtime-load", "pending-boot-recovery", "completed-boot-recovery", "completed-runtime-recovery" })
			RunItemReaderProcess(Reader(next), "--corpse-animation-saved-parent-reader");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(host.Store.Find(life.Origin.Id)!.State == SpellLifecycleState.Completed &&
				!XElement.Parse(db.GameItems.Single(x => x.Id == corpse.Id).EffectData).Descendants("OwnedLifecycleId").Any(),
				"Final fresh reader did not persist removal of the stale saved child.");
		Console.WriteLine("ARM03D1P1-saved-parent-restart=passed five-fresh-process-loads exact-paid-parent active-boot-and-committed-pending-and-completed-journal one-corpse-instance same-body-identity-foreign-gear final-normal-save producer-quiescent");
		return 0;
	}

	private static int RunSavedCorpseAnimationParentReader(string[] args)
	{
		Require(args.Length == 1, "Saved-parent reader requires one receipt.");
		var input = JsonSerializer.Deserialize<SavedCorpseParentReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, corpseAnimationAnatomy: true);
		ConfigureCorpseAnimationWorld(host, database.ConnectionString);
		var native = host.Native; var world = native.World; var scheduler = new EffectScheduler(world, clock);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		MagicSpellParent.InitialiseEffectType();
		long[] identityIds, bodyIds, itemIds, instanceIds;
		Dictionary<long, double> ownerResources;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.EffectHash, "Reader did not receive original paid parent XML.");
			Require(db.GameItems.Single(x => x.Id == input.Corpse).MorphTimeRemaining == input.MorphSeconds && input.MorphSeconds > 0,
				"Reader lost the paid corpse's persisted morph duration.");
			Require(world.MagicSpells.Get(input.Spell) is MagicSpell, "Native casting catalogue did not load the saved source spell.");
			identityIds = db.Characters.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
			bodyIds = db.Bodies.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
			itemIds = db.GameItems.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
			instanceIds = db.CharacterInstances.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
			ownerResources = db.CharactersMagicResources.Where(x => x.CharacterId == input.Owner).ToDictionary(x => x.MagicResourceId, x => x.Amount);
		}
		var initialState = host.Store.Find(input.Origin)!.State;
		var activeRecovery = input.Action.StartsWith("active-") && input.Action.EndsWith("-recovery");
		Require(initialState == (input.Action.StartsWith("active-") ? SpellLifecycleState.Active :
			input.Action.StartsWith("pending") ? SpellLifecycleState.Retiring : SpellLifecycleState.Completed), "Unexpected saved-parent checkpoint state.");
		var boot = input.Action.Contains("boot"); var blockedCharacterCalls = 0; var characterCalls = 0;
		world.SaveManager.MudBootingMode = boot;
		var phaseMethod = typeof(Futuremud).GetMethod("SetCharacterMaterialisationBootPhase", BindingFlags.Instance | BindingFlags.NonPublic)!;
		void SetPhase(bool blocked) => phaseMethod.Invoke(host.Roots, [Enum.Parse(phaseMethod.GetParameters()[0].ParameterType, blocked ? "Disallowed" : "Allowed")]);
		SetPhase(boot);
		var finalisers = new List<IPostCharacterLoadFinalisable>();
		native.WorldMock.Setup(x => x.RegisterPostCharacterLoadFinalisable(It.IsAny<IPostCharacterLoadFinalisable>()))
			.Callback<IPostCharacterLoadFinalisable>(x => { if (!finalisers.Contains(x)) finalisers.Add(x); });
		native.WorldMock.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, cached) =>
		{
			characterCalls++;
			if (boot) { blockedCharacterCalls++; return host.Roots.TryGetCharacter(id, cached); } // actual native boot guard
			if (id == input.Fixture.CharacterId) return native.Actor;
			var loaded = host.Roots.Actors.Concat(host.Roots.CachedActors).FirstOrDefault(x => x.Id == id);
			if (loaded is not null) return loaded;
			using var scope = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var identity = FMDB.Context.Characters.Include(x => x.Body).Single(x => x.Id == id && !x.IsArchived);
			var npc = new RuntimeNpc(FMDB.Context.Npcs.Single(x => x.CharacterId == id), identity, world);
			host.Roots.Add(npc, true); host.Roots.Add(npc.Body); npc.SetupEventSubscriptions(); return npc;
		});
		var constructing = new HashSet<long>(); var corpseConstructions = 0;
		native.WorldMock.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, add) =>
		{
			if (host.Items.Get(id) is { } loaded) return loaded;
			Require(constructing.Add(id), "Recursive native item construction detected for " + id);
			try
			{
				using var scope = FMDB.BeginIndependentScope(); using var db = new FMDB();
				var row = FMDB.Context.GameItems.SingleOrDefault(x => x.Id == id); if (row is null) return null!;
				if (id == input.Corpse) corpseConstructions++;
				var item = new GameItem(row, world); if (add) host.Items.Add(item); return item;
			}
			finally { constructing.Remove(id); }
		});
		var corpse = world.TryGetItem(input.Corpse, true)!;
		long? placedCell;
		using (var db = NewIndependentContext(database.ConnectionString))
			placedCell = db.CellsGameItems.Where(x => x.GameItemId == input.Corpse).Select(x => (long?)x.CellId).SingleOrDefault();
		// Controlled room catalogue, with the same native item Drop used by Cell.LoadItems.
		if (placedCell.HasValue) world.Cells.Get(placedCell.Value)!.Insert(corpse, true);
		var loadedLocation = corpse.Location; var placementChanges = 0;
		corpse.OnLocationChanged += (_, _) => placementChanges++;
		// Boot placement above follows Cell.LoadItems (Drop). Subsequent restoration
		// follows Cell.Insert (MoveTo), including its real location-change callback.
		var recoveryCell = world.Cells.Get(input.Fixture.CellId)!;
		var recoveryItems = (ICollection<IGameItem>)recoveryCell.GameItems;
		var restorationInsertions = 0;
		Mock.Get(recoveryCell).Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
			.Callback<IGameItem, bool>((item, _) =>
			{
				ForeignCustodyTransferContext.EnsureCell(recoveryCell, item);
				Require(!recoveryItems.Contains(item), "Recovery attempted duplicate cell insertion.");
				item.MoveTo(recoveryCell, item.RoomLayer);
				recoveryItems.Add(item);
				if (ReferenceEquals(item, corpse)) restorationInsertions++;
			});
		Require(corpseConstructions == 1 && ReferenceEquals(host.Items.Get(input.Corpse), corpse) &&
			corpse.EffectsOfType<MagicSpellParent>().Single().Spell.Id == input.Spell &&
			corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Single().ExpiryUtc == input.Deadline &&
			host.Store.Find(input.Origin)!.State == initialState, "Saved parent construction restored inline, lost its deadline or duplicated the corpse.");
		corpse.Login(); // follows native LoadWorldItems: registered items log in before characters are allowed
		native.WorldMock.Verify(x => x.RetrieveAppropriateCommandTree(It.Is<ICharacter>(c => c is ScriptedAiCharacterInstance)), Times.Never);
		var morphDeadline = ((GameItem)corpse).MorphTime;
		Require(morphDeadline == input.Now.AddSeconds(input.MorphSeconds) && corpse.CachedMorphTime is null &&
			host.Scheduler.RemainingDuration(corpse, ScheduleType.Morph) == TimeSpan.FromSeconds(input.MorphSeconds) &&
			host.Scheduler.RemainingDuration(corpse, ScheduleType.MorphSaving) == TimeSpan.FromSeconds(30),
			"Cold Login changed or disabled the corpse's native morph schedules.");
		if (boot)
		{
			scheduler.CheckSchedules(); clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
			Require(blockedCharacterCalls == 0 && characterCalls == 0 && corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Any() &&
				host.Store.Find(input.Origin)!.State == initialState, "Saved durable child performed character resolution or recovery during boot.");
		}
		if (input.Action is "active-boot-load" or "pending-runtime-load")
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.EffectHash, "Load-only reader mutated paid XML.");
			Console.WriteLine($"ARM03D1P1-{input.Action}=passed fresh-process actual-paid-parent-constructor-and-Login one-corpse-construction unchanged-journal-and-XML no-inline-restoration no-producer-interference");
			return 0;
		}
		boot = false; SetPhase(false);
		// Post-character health finalisation remains native, on the already registered corpse.
		foreach (var finaliser in finalisers.ToArray()) finaliser.FinaliseLoading();
		var component = corpse.GetItemType<ICorpse>();
		native.WorldMock.Verify(x => x.RetrieveAppropriateCommandTree(It.Is<ICharacter>(c => c is ScriptedAiCharacterInstance)), Times.Never);
		Require(component.OriginalCharacter.Identity.Instances.All(x => x.InstanceId != input.Instance),
			"Cold loading rematerialized the owned animation before recovery.");
		if (activeRecovery)
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(db.CharacterInstances.Any(x => x.Id == input.Instance) && !db.CellsGameItems.Any(x => x.GameItemId == input.Corpse) &&
				host.Store.Find(input.Origin)!.State == SpellLifecycleState.Active && corpse.Location is null &&
				(input.Action.Contains("expired") ? RuntimeClock.UtcNow >= input.Deadline : RuntimeClock.UtcNow < input.Deadline),
				"Active checkpoint was retired early, materialized, exposed, or crossed the wrong deadline.");
		}
		world.SaveManager.MudBootingMode = false;
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		Require(corpseConstructions == 1 && host.Items.Count(x => x.Id == input.Corpse) == 1 && ReferenceEquals(world.TryGetItem(input.Corpse, true), corpse) &&
			component.OriginalCharacter.Id == input.Owner && component.OriginalBody.Id == input.Body &&
			component.OriginalCharacter.Identity.Instances.All(x => x.InstanceId != input.Instance) && component.OriginalBody.AllItems.Any(x => x.Id == input.Foreign) &&
			!corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Any() && !corpse.EffectsOfType<MagicSpellParent>().Any() &&
			host.Store.Find(input.Origin)!.Origin.DeadlineUtc == input.Deadline, "Deferred saved-parent recovery lost exact state or left stale effects.");
		AssertCorpseAnimationRestored(database, host, input.Origin, input.Instance, input.Corpse, input.Owner, input.Body, input.Foreign);
		Require((activeRecovery ? corpse.Location?.Id == input.Fixture.CellId && placementChanges == 1 : ReferenceEquals(corpse.Location, loadedLocation) && placementChanges == 0) &&
			restorationInsertions == (activeRecovery ? 1 : 0) && corpse.Location!.GameItems.Count(x => ReferenceEquals(x, corpse)) == 1,
			$"Saved-child cleanup moved or duplicated the corpse: active={activeRecovery}, changes={placementChanges}, insertions={restorationInsertions}, location={corpse.Location?.Id}.");
		Require(((GameItem)corpse).MorphTime == morphDeadline &&
			host.Scheduler.RemainingDuration(corpse, ScheduleType.Morph) == morphDeadline - RuntimeClock.UtcNow,
			"Saved-parent recovery reset or removed the real corpse morph timer.");
		if (activeRecovery)
			Require(host.Store.Find(input.Origin)!.Reason == (input.Action.Contains("expired") ? SpellRetirementReason.Expiry : SpellRetirementReason.Logout),
				"Saved active parent recovered with the wrong retirement reason.");
		var completedVersion = host.Store.Find(input.Origin)!.Version;
		world.SpellOwnedCorpseAnimations!.ReconcileRetirements(RuntimeClock.UtcNow);
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		Require(host.Store.Find(input.Origin)!.Version == completedVersion && corpseConstructions == 1 &&
			placementChanges == (activeRecovery ? 1 : 0) && restorationInsertions == (activeRecovery ? 1 : 0),
			"Completed recovery replayed or constructed/moved the corpse again.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(identityIds.SequenceEqual(db.Characters.OrderBy(x => x.Id).Select(x => x.Id)) &&
				bodyIds.SequenceEqual(db.Bodies.OrderBy(x => x.Id).Select(x => x.Id)) && itemIds.SequenceEqual(db.GameItems.OrderBy(x => x.Id).Select(x => x.Id)) &&
				instanceIds.Where(x => !activeRecovery || x != input.Instance).SequenceEqual(db.CharacterInstances.OrderBy(x => x.Id).Select(x => x.Id)),
				"Cold recovery changed identities, bodies, items or secondary rows beyond its exact owned instance.");
		if (input.Action == "completed-runtime-recovery" || activeRecovery)
		{
			world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			Require(!XElement.Parse(db.GameItems.Single(x => x.Id == input.Corpse).EffectData).Descendants("OwnedLifecycleId").Any(), "Normal save retained stale animation XML.");
			Require(db.GameItems.Single(x => x.Id == input.Corpse).MorphTimeRemaining == (int)(morphDeadline - RuntimeClock.UtcNow).TotalSeconds,
				"Normal cleanup save changed the remaining native morph duration.");
		}
		else
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.EffectHash, "Intermediate reader overwrote the deliberate stale-XML checkpoint.");
		}
		native.WorldMock.Verify(x => x.RetrieveAppropriateCommandTree(It.Is<ICharacter>(c => c is ScriptedAiCharacterInstance)), Times.Never);
		Require(component.OriginalCharacter.MagicResourceAmounts[native.Resource] == ownerResources.GetValueOrDefault(native.Resource.Id),
			"Cold recovery replayed the selected AI's resource program.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(ownerResources.OrderBy(x => x.Key).SequenceEqual(db.CharactersMagicResources.Where(x => x.CharacterId == input.Owner)
				.ToDictionary(x => x.MagicResourceId, x => x.Amount).OrderBy(x => x.Key)), "Cold recovery changed persisted canonical magic resources.");
		Console.WriteLine($"ARM03D1P1-{input.Action}=passed fresh-process actual-paid-saved-parent native-EffectScheduler after-registration-and-boot one-corpse-instance same-owner-body-foreign-gear unchanged-deadline positive-morph-timing-preserved exact-secondary-retirement no-replay stale-child-and-empty-parent-removed");
		return 0;
	}
}
