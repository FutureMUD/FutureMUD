#nullable enable

using System.Data;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedItemService
{
	public bool HasActiveOrigin(IGameItem item, SpellOwnedItemOrigin expected)
	{
		if (!ReferenceEquals(item.Gameworld, _world) || item.Id <= 0 || item.Deleted || item.Destroyed ||
			!ConsumableTiming(expected, RuntimeClock.UtcNow)) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return ActiveConsumptionOrigin(FMDB.Context, item.Id, expected, IsBoundPermanentTopicalDose(item, expected));
		}
		catch { return false; }
	}

	private static bool ConsumableTiming(SpellOwnedItemOrigin expected, DateTime now) => expected.Mode switch
	{
		SpellLifecycleMode.TemporaryCleanup => expected.DeadlineUtc > now,
		SpellLifecycleMode.Permanent => expected.DeadlineUtc is null,
		_ => false
	};

	internal static bool IsBoundPermanentTopicalDose(IGameItem item, SpellOwnedItemOrigin expected) =>
		expected.Mode == SpellLifecycleMode.Permanent && expected.DeadlineUtc is null && item.SpellCreationOrigin == expected &&
		item.GetItemType<IArmageddonTopicalDoseComponent>() is TopicalCreamGameItemComponent topical &&
		ReferenceEquals(topical.Parent, item) && item.Components.Any(x => ReferenceEquals(x, topical)) &&
		topical.Prototype is TopicalCreamGameItemComponentProto { NativeDoseProfile: { IsValid: true } profile } &&
		topical.DoseBinding is { IsValid: true } binding && binding.LifecycleId == expected.LifecycleId &&
		binding.SpellId == expected.SpellId && binding.Grade == expected.Grade && binding.CreatorId == expected.CreatorId &&
		binding.ReferenceDoseGrams == profile.ReferenceDoseGrams && binding.BudgetPerGrade == profile.BudgetPerGrade;

	internal static bool ConsumptionOriginMatches(Models.MagicSpellLifecycle? life, long itemId,
		SpellOwnedItemOrigin expected, DateTime now, bool permanentTopicalDose)
	{
		if (life is null || life.Id != expected.LifecycleId || !ConsumableTiming(expected, now)) return false;
		var eligibleState = expected.Mode == SpellLifecycleMode.TemporaryCleanup
			? life.State == (int)SpellLifecycleState.Active
			: permanentTopicalDose && life.Family == "restorative-mud" && life.State == (int)SpellLifecycleState.Completed;
		return eligibleState && life.Mode == (int)expected.Mode && life.Diagnostic == "" &&
			life.SpellId == expected.SpellId && life.CreatorId == expected.CreatorId && life.Grade == expected.Grade &&
			life.CreatedUtc <= now && life.DeadlineUtc == expected.DeadlineUtc && life.DeathObservedUtc is null &&
			life.Entities.Count == 1 && life.Entities.Single().Kind == (int)SpellOwnedEntityKind.GameItem &&
			life.Entities.Single().EntityId == itemId && life.Entities.Single().LifecycleId == life.Id;
	}

	private static bool ActiveConsumptionOrigin(FuturemudDatabaseContext db, long itemId, SpellOwnedItemOrigin expected,
		bool permanentTopicalDose)
	{
		var life = db.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).SingleOrDefault(x => x.Id == expected.LifecycleId);
		return ConsumptionOriginMatches(life, itemId, expected, RuntimeClock.UtcNow, permanentTopicalDose) &&
			db.GameItems.Any(x => x.Id == itemId);
	}

	public bool TryCommitComponentConsumption(IGameItemComponent component, SpellOwnedItemOrigin expected)
	{
		if (component is not (ArmageddonEmberSeedGameItemComponent or TopicalCreamGameItemComponent) ||
			component.Id <= 0 || expected.Mode == SpellLifecycleMode.Permanent &&
			(component is not TopicalCreamGameItemComponent || !ReferenceEquals(component.Parent.GetItemType<IArmageddonTopicalDoseComponent>(), component)) ||
			!ReferenceEquals(component.Gameworld, _world) ||
			!component.Parent.Components.Any(x => ReferenceEquals(x, component)) || !HasActiveOrigin(component.Parent, expected)) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
			if (!ActiveConsumptionOrigin(FMDB.Context, component.Parent.Id, expected, IsBoundPermanentTopicalDose(component.Parent, expected))) return false;
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
