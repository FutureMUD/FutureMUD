#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	[TestMethod]
	[DoNotParallelize]
	public void PropertyDivestment_FinalShareImmediatelyRevokesAuthorityAndRemovesStoredOwner()
	{
		using var db = new InMemoryDatabaseTestScope();
		using var handle = new FMDB();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var start = LendingStart();
		var calendar = Mock.Of<ICalendar>(x => x.CurrentDateTime == start);
		var zone = Mock.Of<IEconomicZone>(x => x.Id == 21L && x.Gameworld == world.Object && x.FinancialPeriodReferenceCalendar == calendar);
		var formerOwner = Mock.Of<ICharacter>(x => x.Id == 17L && x.FrameworkItemType == "Character");
		var recipient = Mock.Of<ICharacter>(x => x.Id == 18L && x.FrameworkItemType == "Character");
		world.Setup(x => x.TryGetCharacter(17, true)).Returns(formerOwner);
		world.Setup(x => x.TryGetCharacter(18, true)).Returns(recipient);
		var property = new Property("Warehouse", zone, Mock.Of<IRoom>(x => x.Id == 22L), 800m, formerOwner, null);
		var share = property.PropertyOwners.Single();

		property.DivestOwnership(share, 1m, formerOwner);
		Assert.AreSame(share, property.PropertyOwners.Single());
		Assert.AreEqual(1m, share.ShareOfOwnership);

		property.DivestOwnership(share, 0.25m, recipient);
		Assert.AreEqual(0.75m, share.ShareOfOwnership);
		Assert.IsTrue(property.IsAuthorisedOwner(formerOwner));
		property.DivestOwnership(share, 1m, recipient);
		Assert.IsFalse(property.IsAuthorisedOwner(formerOwner));
		Assert.IsTrue(property.IsAuthorisedOwner(recipient));
		var remaining = property.PropertyOwners.Single();
		Assert.AreEqual(1m, remaining.ShareOfOwnership);
		((PropertyOwner)remaining).Save();
		db.Context.SaveChanges();
		var stored = db.Context.PropertyOwners.AsNoTracking().Single(x => x.PropertyId == property.Id);
		Assert.AreEqual(recipient.Id, stored.FrameworkItemId);
		Assert.AreEqual(1m, stored.ShareOfOwnership);
	}

	[TestMethod]
	[DoNotParallelize]
	public void PropertyLease_SigningPersistsTwoRentPeriodsSeparatelyFromBond()
	{
		using var db = new InMemoryDatabaseTestScope();
		using var handle = new FMDB();
		var start = LendingStart();
		var tenant = Mock.Of<ICharacter>(x => x.Id == 19L && x.FrameworkItemType == "Character");
		var order = new Mock<IPropertyLeaseOrder> { CallBase = true };
		order.SetupGet(x => x.Id).Returns(11L);
		order.SetupGet(x => x.BondRequired).Returns(10m);
		order.SetupGet(x => x.PricePerInterval).Returns(20m);
		order.SetupGet(x => x.Interval).Returns(new RecurringInterval { IntervalAmount = 14, Type = IntervalType.Daily });
		var property = Mock.Of<IProperty>(x => x.Id == 12L && x.Gameworld == _gameworld && x.LeaseOrder == order.Object);
		var lease = new PropertyLease(property, tenant, start, start + TimeSpan.FromDays(56), order.Object);
		Assert.AreEqual(50m, order.Object.PaymentDueAtSigning);
		var stored = db.Context.PropertyLeases.AsNoTracking().Single(x => x.Id == lease.Id);
		Assert.AreEqual(40m, stored.PaymentBalance);
		Assert.AreEqual(10m, stored.BondPayment);
		Assert.AreEqual(40m, new PropertyLease(stored, property).PaymentBalance);
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(0, 0, false)]
	[DataRow(20, 20, false)]
	[DataRow(-20, 0, false)]
	[DataRow(-20, -20, true)]
	public void PropertyTerminateLease_RequiresArrearsAtRequestAndConfirmation(int initialBalance, int finalBalance, bool shouldTerminate)
	{
		var room = Mock.Of<IRoom>();
		var zone = Mock.Of<IEconomicZone>(x => x.Id == 21L && x.ConveyancingRooms == new[] { room });
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var lease = new Mock<IPropertyLease>();
		lease.SetupProperty(x => x.PaymentBalance, initialBalance);
		var property = Mock.Of<IProperty>(x => x.Id == 11L && x.Name == "Hall" && x.EconomicZone == zone &&
			x.IsAuthorisedOwner(actor.Object) && x.Lease == lease.Object);
		actor.SetupGet(x => x.Location).Returns(room);
		actor.SetupGet(x => x.Gameworld.Properties).Returns(new All<IProperty> { property });
		actor.SetupGet(x => x.Gameworld.EconomicZones).Returns(new All<IEconomicZone> { zone });
		Accept? proposal = null;
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => proposal = (Accept)effect);
		var phrasing = typeof(Accept).GetField("_standardAcceptPhrasing", BindingFlags.Static | BindingFlags.NonPublic)!;
		var original = phrasing.GetValue(null);
		try
		{
			phrasing.SetValue(null, "Type accept to confirm.");
			typeof(PropertyModule).GetMethod("PropertyTerminateLease", BindingFlags.NonPublic | BindingFlags.Static)!
				.Invoke(null, [actor.Object, new StringStack("Hall")]);
			Assert.AreEqual(initialBalance < 0, proposal is not null);
			lease.Object.PaymentBalance = finalBalance;
			proposal?.Proposal.Accept();
			lease.Verify(x => x.TerminateLease(), shouldTerminate ? Times.Once : Times.Never);
		}
		finally
		{
			phrasing.SetValue(null, original);
		}
	}
}
