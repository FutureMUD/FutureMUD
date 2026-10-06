#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using NativeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp.GameItems;

public partial class GameItem
{
	/// <summary>Only native detection effects with their exact save wrapper have no custody callbacks.</summary>
	internal static bool SpellRemovalEffectsArePassive(IPerceivable custodian)
	{
		var effects = custodian.Effects.ToArray();
		bool PassiveDetection(IEffect effect) =>
			(effect.GetType() == typeof(SpellDetectMagickEffect) ||
			 effect.GetType() == typeof(SpellDetectInvisibleEffect) ||
			 effect.GetType() == typeof(SpellDetectEtherealEffect) ||
			 effect.GetType() == typeof(SpellInfravisionEffect)) &&
			effect.ApplicabilityProg is null && ReferenceEquals(effect.Owner, custodian);
		foreach (var effect in effects)
		{
			if (effect.GetType() == typeof(MagicSpellParent) && effect is MagicSpellParent parent)
			{
				var children = parent.SpellEffects.ToArray();
				if (parent.ApplicabilityProg is not null || !ReferenceEquals(parent.Owner, custodian) ||
					children.Length == 0 || children.Any(child => !PassiveDetection(child) ||
						!ReferenceEquals(child.ParentEffect, parent) || !effects.Any(x => ReferenceEquals(x, child)))) return false;
				continue;
			}
			if (!PassiveDetection(effect) || effect is not IMagicSpellEffect childEffect ||
				!effects.Any(x => ReferenceEquals(x, childEffect.ParentEffect))) return false;
		}
		return true;
	}

	/// <summary>Commit exact leaf removal and custodian persistence together before releasing runtime roots.</summary>
	private void DeleteSpellOwnedItem()
	{
		if (Components.Any(x => x is not (HoldableGameItemComponent or MeleeWeaponGameItemComponent or SalvageableGameItemComponent or
			FoodGameItemComponent or WearableGameItemComponent or ProgLightGameItemComponent)))
			throw new InvalidOperationException("Created item removal needs an adapter for this component graph.");
		var parent = ContainedIn as GameItem;
		if (ContainedIn is not null && parent is null || parent is not null &&
			(parent.Effects.Any() || parent.Hooks.Any() || !parent.Components.Any(x => x is ContainerGameItemComponent or SheathGameItemComponent)))
			throw new InvalidOperationException("Created item removal needs a native callback-free container or sheath custodian.");
		var body = InInventoryOf as NativeBody;
		if (InInventoryOf is not null && body is null || body is not null &&
			(!SpellRemovalEffectsArePassive(body) || !SpellRemovalEffectsArePassive(body.Actor) ||
			 body.Actor.PositionTarget is not null || body.PositionTarget is not null))
			throw new InvalidOperationException("Created item removal needs a native callback-free body custodian.");
		var location = Location;
		if (location is not null && location is not ICustodyRollbackLocation)
			throw new InvalidOperationException("Created item removal needs a native location rollback adapter.");
		var graph = parent is null ? new[] { this } : new[] { this, parent };
		var restoreItems = graph.Select(x => x.CaptureCustodyRollback()).ToArray();
		var restoreSaves = graph.Select(x => x.CaptureCustodySaveRollback()).ToArray();
		var components = graph.SelectMany(x => x.Components).Cast<GameItemComponent>().ToArray();
		var restoreComponents = components.Select(x => x.CaptureCustodyRollback()).ToArray();
		if (restoreComponents.Any(x => x is null))
			throw new InvalidOperationException("Created item custodian has no verified structural rollback adapter.");
		var restoreInventory = body?.CaptureCustodyInventoryRollback();
		var restoreBodySave = body?.CaptureCustodySaveRollback();
		var restoreLocation = (location as ICustodyRollbackLocation)?.CaptureCustodyMembershipRollback([this]);
		using (var isolated = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		using (var transfer = ForeignCustodyTransferContext.EnterRemoval(body, graph, location))
		{
			try
			{
				var context = FMDB.Context;
				var authority = context.MagicSpellLifecycles.Include(x => x.Entities).Single(x => x.Id == SpellCreationOrigin!.LifecycleId);
				if (authority.State != (int)SpellLifecycleState.Retiring || authority.Mode != (int)SpellLifecycleMode.TemporaryCleanup ||
					authority.Entities.Count != 1 || authority.Entities.Single().EntityId != Id ||
					authority.Entities.Single().Kind != (int)SpellOwnedEntityKind.GameItem)
					throw new InvalidOperationException("Exact item retirement authority changed before custody removal.");
				var row = context.GameItems.Find(Id);
				if (row is not null)
				{
					parent?.Take(this); ContainedIn = null!; body?.Take(this); location?.Extract(this); Get(null!);
					if (DeepItems.Skip(1).Any() || AttachedAndConnectedItems.Any() || LodgedItems.Any() ||
						Wounds.Any(x => x.Lodged is not null) || Effects.Any() || Hooks.Any() ||
						PositionTarget is not null || TargetedBy.Any() || ContainedIn is not null || InInventoryOf is not null || Location is not null)
						throw new InvalidOperationException("A native custody callback introduced an item dependency; restore custody and retain its retirement hold.");
					if (body is not null && (body.AllItems.Any(x => ReferenceEquals(x, this)) || body.HeldItems.Contains(this) ||
						body.WieldedItems.Contains(this) || body.WornItems.Contains(this)) ||
						parent?.DeepItems.Any(x => ReferenceEquals(x, this)) == true || location?.GameItems.Contains(this) == true)
						throw new InvalidOperationException("A native custody callback reacquired the item; restore its exact original membership before retrying.");
					parent?.Save();
					foreach (var component in parent?.Components.Where(x => x.Changed) ?? []) component.Save();
					body?.Save();
					context.GameItems.Remove(row);
					context.SaveChanges();
				}
				transaction.Commit();
			}
			catch (Exception original)
			{
				var failures = new List<Exception> { original };
				void Recover(Action? action) { if (action is null) return; try { action(); } catch (Exception ex) { failures.Add(ex); } }
				Recover(transaction.Rollback);
				foreach (var restore in restoreComponents) Recover(restore);
				foreach (var restore in restoreItems) Recover(restore);
				Recover(restoreInventory); Recover(restoreLocation);
				foreach (var restore in restoreSaves) Recover(restore);
				Recover(restoreBodySave);
				transfer.RestorePendingSaves(Recover);
				foreach (var component in components) Recover(() => component.Changed = true);
				if (body is not null) Recover(body.RecalculateItemHelpers);
				if (failures.Count > 1) throw new AggregateException("Created item removal and compensation failed; retain the exact retirement hold.", failures);
				throw;
			}
		}
		// Committed deletion must never enter compensation. A failed runtime release is retried from its exact durable intent.
		FinishCommittedSpellItemRemoval();
	}

	internal void FinishCommittedSpellItemRemoval()
	{
		EndHealthTick();
		if (!Deleted) DeleteNative(persistedAlready: true);
	}
}
