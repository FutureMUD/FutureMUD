#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
public class ClanAddressPersistenceTests
{
	[TestMethod]
	public void SaveReload_AddressRules_PreserveStableIdsOrderingNoneAndMissingProgReferences()
	{
		using var db = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Gameworld).Returns(world.Object);
		clan.SetupGet(x => x.Paygrades).Returns([]);
		clan.SetupGet(x => x.Ranks).Returns([]);
		var rankRow = new MudSharp.Models.Rank { Id = 1, Name = "Officer" };
		var officeRow = new MudSharp.Models.Appointment { Id = 2, Name = "Chair" };
		db.Context.AddRange(rankRow, officeRow);
		db.Context.SaveChanges();
		var rank = new Rank(rankRow, clan.Object);
		var office = new Appointment(officeRow, clan.Object) { AddressPriority = 25 };
		var first = new ClanAddressRule { Text = "Commander", FutureProgId = 999, Relations = ClanAddressRelation.Lower, IsPublic = true };
		var suppressed = new ClanAddressRule { Text = null };
		rank.FormsOfAddress.AddRange([first, suppressed]);
		office.FormsOfAddress.Add(first.Copy());
		rank.Save(); office.Save(); rank.Save();
		db.Context.ChangeTracker.Clear();
		rankRow = db.Context.Ranks.Include(x => x.FormsOfAddress).Single();
		officeRow = db.Context.Appointments.Include(x => x.FormsOfAddress).Single();
		var loaded = new Rank(rankRow, clan.Object);
		var loadedOffice = new Appointment(officeRow, clan.Object);
		Assert.AreEqual(first.Id, loaded.FormsOfAddress[0].Id);
		Assert.AreEqual(suppressed.Id, loaded.FormsOfAddress[1].Id);
		Assert.IsNull(loaded.FormsOfAddress[1].Text);
		Assert.AreEqual(999L, loaded.FormsOfAddress[0].FutureProgId);
		Assert.IsTrue(loaded.FormsOfAddress[0].IsPublic);
		Assert.AreEqual(ClanAddressRelation.Lower, loaded.FormsOfAddress[0].Relations);
		Assert.AreEqual(25, loadedOffice.AddressPriority);
		Assert.AreNotEqual(first.Id, loadedOffice.FormsOfAddress[0].Id);
		loaded.FormsOfAddress.Reverse(); loaded.Save();
		Assert.AreEqual(suppressed.Id, db.Context.RankFormsOfAddress.OrderBy(x => x.Order).First().Id);
		loaded.FormsOfAddress.RemoveAll(x => x.Id == first.Id); loaded.Save();
		Assert.HasCount(1, db.Context.RankFormsOfAddress.ToList());
	}
}
