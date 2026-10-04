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
		long Instance, long Corpse, long Owner, long Body, long Foreign, DateTime Deadline, long Spell, string Action, string EffectHash);
	private static string SavedEffectHash(string xml) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(xml)));

	private static int RunSavedCorpseAnimationParentChecks(TestDatabase database, FixtureIds fixture, RetirementHost host,
		HarnessClock clock, MagicSpell spell, ICharacter owner, IGameItem corpse, IGameItem foreign,
		ScriptedAiCharacterInstance animated, SpellOwnedLifecycle life)
	{
		// This packet isolates saved spell-child recovery. The shared retirement fixture has
		// a morph timer whose Login description separately resolves the corpse's owner.
		// Freeze that unrelated timer before saving; retain the actual paid effect XML.
		corpse.EndMorphTimer();
		((GameItem)corpse).CachedMorphTime = null;
		corpse.Changed = true;
		host.Native.World.SaveManager.Flush();
		string paidXml;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var row = db.GameItems.Single(x => x.Id == corpse.Id);
			Require(row.MorphTimeRemaining is null, "Saved-parent fixture must exclude the separate corpse morph Login path.");
			paidXml = row.EffectData;
		}
		var xml = XElement.Parse(paidXml);
		Require(xml.Elements("Effect").Single().Element("Type")!.Value == "MagicSpellParent" &&
			xml.Descendants("OwnedLifecycleId").Single().Value == life.Origin.Id.ToString() &&
			(DateTime)xml.Descendants("ExpiryUtc").Single() == life.Origin.DeadlineUtc, "Actual paid parent/child XML was not persisted.");
		SavedCorpseParentReader Reader(string action) => new(database.Name, fixture, RuntimeClock.UtcNow, life.Origin.Id,
			animated.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id, life.Origin.DeadlineUtc!.Value, spell.Id, action, SavedEffectHash(paidXml));
		Console.WriteLine("ARM03D1P1-paid-parent-saved=passed actual-paid-grade3-parent-child native-SaveManager-Flush persisted-origin-and-absolute-deadline separate-morph-timer-disabled");
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
		foreach (var action in new[] { "pending-runtime-load", "pending-boot-recovery", "completed-boot-recovery", "completed-runtime-recovery" })
			RunItemReaderProcess(Reader(action), "--corpse-animation-saved-parent-reader");
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
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.EffectHash, "Reader did not receive original paid parent XML.");
			Require(world.MagicSpells.Get(input.Spell) is MagicSpell, "Native casting catalogue did not load the saved source spell.");
		}
		var initialState = host.Store.Find(input.Origin)!.State;
		Require(initialState == (input.Action == "active-boot-load" ? SpellLifecycleState.Active :
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
		Require(corpseConstructions == 1 && ReferenceEquals(host.Items.Get(input.Corpse), corpse) &&
			corpse.EffectsOfType<MagicSpellParent>().Single().Spell.Id == input.Spell &&
			corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Single().ExpiryUtc == input.Deadline &&
			host.Store.Find(input.Origin)!.State == initialState, "Saved parent construction restored inline, lost its deadline or duplicated the corpse.");
		corpse.Login(); // follows native LoadWorldItems: registered items log in before characters are allowed
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
		world.SaveManager.MudBootingMode = false;
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		var component = corpse.GetItemType<ICorpse>();
		Require(corpseConstructions == 1 && host.Items.Count(x => x.Id == input.Corpse) == 1 && ReferenceEquals(world.TryGetItem(input.Corpse, true), corpse) &&
			component.OriginalCharacter.Id == input.Owner && component.OriginalBody.Id == input.Body &&
			component.OriginalCharacter.Identity.Instances.All(x => x.InstanceId != input.Instance) && component.OriginalBody.AllItems.Any(x => x.Id == input.Foreign) &&
			!corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Any() && !corpse.EffectsOfType<MagicSpellParent>().Any() &&
			host.Store.Find(input.Origin)!.Origin.DeadlineUtc == input.Deadline, "Deferred saved-parent recovery lost exact state or left stale effects.");
		AssertCorpseAnimationRestored(database, host, input.Origin, input.Instance, input.Corpse, input.Owner, input.Body, input.Foreign);
		Require(ReferenceEquals(corpse.Location, loadedLocation) && placementChanges == 0 &&
			corpse.Location!.GameItems.Count(x => ReferenceEquals(x, corpse)) == 1, "Saved-child cleanup moved or duplicated the already restored corpse.");
		if (input.Action == "completed-runtime-recovery")
		{
			world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			Require(!XElement.Parse(db.GameItems.Single(x => x.Id == input.Corpse).EffectData).Descendants("OwnedLifecycleId").Any(), "Normal save retained stale animation XML.");
		}
		else
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.EffectHash, "Intermediate reader overwrote the deliberate stale-XML checkpoint.");
		}
		Console.WriteLine($"ARM03D1P1-{input.Action}=passed fresh-process actual-paid-saved-parent native-EffectScheduler after-registration-and-boot one-corpse-instance same-owner-body-foreign-gear unchanged-deadline no-secondary-rematerialization stale-child-and-empty-parent-removed");
		return 0;
	}
}
