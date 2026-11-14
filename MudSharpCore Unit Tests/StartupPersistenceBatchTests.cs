#nullable enable

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Save;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StartupPersistenceBatchTests
{
	// Isolate batch preparation/recovery from gameplay construction. The full native
	// Character/NPC save path is also exercised by the disposable large-world boot.
	private sealed class SaveProbeCharacter : MudSharp.Character.Character
	{
		private SaveProbeCharacter() : base(null!, null!) { }
		public override long Id { get; set; }
		public override bool IdHasBeenRegistered => true;
		public override bool IsPrimaryInstance => true;
		public int SaveCalls;
		public override void Save()
		{
			SaveCalls++;
			var row = FMDB.Context.Characters.Find(Id)!;
			row.LongTermPlan = "saved by startup batch";
			row.CharactersLanguages.Add(new MudSharp.Models.CharactersLanguages { CharacterId = Id, LanguageId = 2 });
			Changed = false;
		}
	}

	private static SaveProbeCharacter Actor(long id)
	{
		var actor = (SaveProbeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(SaveProbeCharacter));
		actor.Id = id;
		return actor;
	}

	[TestMethod]
	public void StartupBatch_ThirtyTwoTrackedCharacterAndChildUpdatesSurviveReload()
	{
		using var database = new InMemoryDatabaseTestScope(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		for (var id = 1; id <= 32; id++)
		{
			var body = new MudSharp.Models.Body { Id = 1000 + id };
			database.Context.Characters.Add(new MudSharp.Models.Character { Id = id, BodyId = body.Id, Body = body });
		}
		database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var actors = Enumerable.Range(1, 32).Select(x => Actor(x)).ToArray();
		var manager = new SaveManager(); foreach (var actor in actors) manager.Add(actor);
		using (manager.DeferUnrelatedIdentitySaves()) manager.Flush();
		Assert.IsTrue(actors.All(x => x.SaveCalls == 1));
		database.Context.ChangeTracker.Clear();
		Assert.AreEqual(32, database.Context.Characters.Count(x => x.LongTermPlan == "saved by startup batch"));
		Assert.AreEqual(32, database.Context.Characters.Include(x => x.CharactersLanguages)
			.Count(x => x.CharactersLanguages.Any(y => y.LanguageId == 2)));
	}

	[TestMethod]
	public void StartupBatch_PreparationFailureRequeuesEveryCharacterBeforeAnySaveAttempt()
	{
		using var database = new InMemoryDatabaseTestScope();
		var actors = Enumerable.Range(1, 32).Select(x => Actor(x)).ToArray();
		var manager = new SaveManager(); foreach (var actor in actors) manager.Add(actor);
		database.Context.Dispose();
		using (manager.DeferUnrelatedIdentitySaves())
			Assert.ThrowsExactly<ObjectDisposedException>(() => manager.Flush());
		Assert.IsTrue(actors.All(x => x.SaveCalls == 0));
		Assert.IsTrue(actors.All(manager.IsQueued));
		Assert.IsFalse(manager.Flushing);
	}
}
