#nullable enable

using System.Data;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedItemService
{
	public bool HasActiveOrigin(IGameItem item, SpellOwnedItemOrigin expected)
	{
		if (!ReferenceEquals(item.Gameworld, _world) || item.Id <= 0 || item.Deleted || item.Destroyed ||
			expected.Mode != SpellLifecycleMode.TemporaryCleanup || expected.DeadlineUtc is not { } deadline ||
			deadline <= RuntimeClock.UtcNow) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return ActiveConsumptionOrigin(FMDB.Context, item.Id, expected);
		}
		catch { return false; }
	}

	private static bool ActiveConsumptionOrigin(FuturemudDatabaseContext db, long itemId, SpellOwnedItemOrigin expected)
	{
		var life = db.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).SingleOrDefault(x => x.Id == expected.LifecycleId);
		return life is not null && life.State == (int)SpellLifecycleState.Active && life.Mode == (int)expected.Mode &&
			life.Diagnostic == "" && life.SpellId == expected.SpellId && life.CreatorId == expected.CreatorId && life.Grade == expected.Grade &&
			life.CreatedUtc <= RuntimeClock.UtcNow && life.DeadlineUtc == expected.DeadlineUtc && life.DeadlineUtc > RuntimeClock.UtcNow &&
			life.DeathObservedUtc is null && life.Entities.Count == 1 &&
			life.Entities.Single().Kind == (int)SpellOwnedEntityKind.GameItem && life.Entities.Single().EntityId == itemId &&
			db.GameItems.Any(x => x.Id == itemId);
	}

	public bool TryCommitComponentConsumption(IGameItemComponent component, SpellOwnedItemOrigin expected)
	{
		if (component is not (ArmageddonEmberSeedGameItemComponent or TopicalCreamGameItemComponent) ||
			component.Id <= 0 || !ReferenceEquals(component.Gameworld, _world) ||
			!component.Parent.Components.Any(x => ReferenceEquals(x, component)) || !HasActiveOrigin(component.Parent, expected)) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
			if (!ActiveConsumptionOrigin(FMDB.Context, component.Parent.Id, expected)) return false;
			var row = FMDB.Context.GameItemComponents.SingleOrDefault(x => x.Id == component.Id && x.GameItemId == component.Parent.Id);
			if (row is null || (bool?)XElement.Parse(row.Definition).Attribute("Spent") != false) return false;
			component.Save();
			if ((bool?)XElement.Parse(row.Definition).Attribute("Spent") != true || FMDB.Context.SaveChanges() <= 0) return false;
			transaction.Commit();
			return true;
		}
		catch
		{
			// A caller keeps its attempted one-use state latched; ambiguous commit never produces a consequence.
			return false;
		}
	}
}
