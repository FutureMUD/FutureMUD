#nullable enable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonOneUseReloadTests
{
	[TestMethod]
	public void PermanentTopicalDose_ExactBindingRemainsRecognisableAfterPreHealingSpentLatch()
	{
		var f = MudFixture(activeOrigin: true, confirmed: true, permanent: true);
		Assert.IsTrue(SpellOwnedItemService.IsBoundPermanentTopicalDose(f.Parent.Object, f.Origin));
		f.Component.Apply(f.Body.Object, f.Part.Object, 0.0, null!);
		Assert.IsTrue(f.Component.DoseBinding!.Spent);
		Assert.IsTrue(SpellOwnedItemService.IsBoundPermanentTopicalDose(f.Parent.Object, f.Origin));
		Assert.IsFalse(SpellOwnedItemService.IsBoundPermanentTopicalDose(f.Parent.Object, f.Origin with { Grade = 3 }));
		f.Parent.Setup(x => x.GetItemType<IArmageddonTopicalDoseComponent>()).Returns((IArmageddonTopicalDoseComponent)null!);
		Assert.IsFalse(SpellOwnedItemService.IsBoundPermanentTopicalDose(f.Parent.Object, f.Origin));
	}

	[TestMethod]
	[DataRow("none")] [DataRow("not-topical")] [DataRow("wrong-family")] [DataRow("active")] [DataRow("diagnostic")]
	[DataRow("wrong-spell")] [DataRow("wrong-creator")] [DataRow("wrong-grade")] [DataRow("future")]
	[DataRow("deadline")] [DataRow("death")] [DataRow("extra-claim")] [DataRow("foreign-item")]
	[DataRow("wrong-kind")] [DataRow("wrong-claim-life")] [DataRow("wrong-life")] [DataRow("wrong-mode")]
	public void PermanentTopicalDose_OnlyCompletedExactSingleItemClaimCanConsume(string mutation)
	{
		var now = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
		var origin = new SpellOwnedItemOrigin(Guid.Parse("a4c16f26-f369-4d21-9736-9910e3d5dc5b"), SpellLifecycleMode.Permanent, null, 42, 85, 2);
		var claim = new Db.MagicSpellOwnedEntity { Kind = (int)SpellOwnedEntityKind.GameItem, EntityId = 123, LifecycleId = origin.LifecycleId };
		var row = new Db.MagicSpellLifecycle { Id = origin.LifecycleId, SpellId = 85, Grade = 2, CreatorId = 42,
			Family = "restorative-mud", Mode = (int)SpellLifecycleMode.Permanent, CreatedUtc = now.AddSeconds(-1),
			State = (int)SpellLifecycleState.Completed, Entities = [claim] };
		switch (mutation)
		{
			case "wrong-family": row.Family = "ember-seed"; break;
			case "active": row.State = (int)SpellLifecycleState.Active; break;
			case "diagnostic": row.Diagnostic = SpellOwnedItemService.ActivationPending; break;
			case "wrong-spell": row.SpellId++; break;
			case "wrong-creator": row.CreatorId++; break;
			case "wrong-grade": row.Grade++; break;
			case "future": row.CreatedUtc = now.AddTicks(1); break;
			case "deadline": row.DeadlineUtc = now.AddSeconds(1); break;
			case "death": row.DeathObservedUtc = now; break;
			case "extra-claim": row.Entities.Add(new() { Kind = claim.Kind, EntityId = 456, LifecycleId = row.Id }); break;
			case "foreign-item": claim.EntityId++; break;
			case "wrong-kind": claim.Kind = (int)SpellOwnedEntityKind.Body; break;
			case "wrong-claim-life": claim.LifecycleId = Guid.Empty; break;
			case "wrong-life": row.Id = Guid.Empty; break;
			case "wrong-mode": row.Mode = (int)SpellLifecycleMode.TemporaryCleanup; break;
		}
		Assert.AreEqual(mutation == "none", SpellOwnedItemService.ConsumptionOriginMatches(row, 123, origin, now, mutation != "not-topical"));
	}

	[TestMethod]
	public void TemporaryConsumable_StillRequiresActiveUnexpiredExactClaim()
	{
		var now = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
		var origin = new SpellOwnedItemOrigin(Guid.Parse("a4c16f26-f369-4d21-9736-9910e3d5dc5b"), SpellLifecycleMode.TemporaryCleanup, now.AddSeconds(1), 42, 85, 2);
		var row = new Db.MagicSpellLifecycle { Id = origin.LifecycleId, SpellId = 85, Grade = 2, CreatorId = 42, Family = "ember-seed",
			Mode = (int)origin.Mode, CreatedUtc = now, DeadlineUtc = origin.DeadlineUtc, State = (int)SpellLifecycleState.Active,
			Entities = [new() { Kind = (int)SpellOwnedEntityKind.GameItem, EntityId = 123, LifecycleId = origin.LifecycleId }] };
		Assert.IsTrue(SpellOwnedItemService.ConsumptionOriginMatches(row, 123, origin, now, false));
		Assert.IsFalse(SpellOwnedItemService.ConsumptionOriginMatches(row, 123, origin, origin.DeadlineUtc!.Value, false));
		row.State = (int)SpellLifecycleState.Completed;
		Assert.IsFalse(SpellOwnedItemService.ConsumptionOriginMatches(row, 123, origin, now, false));
	}
}
