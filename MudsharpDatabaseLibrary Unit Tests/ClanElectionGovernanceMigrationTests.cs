using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Migrations;

#nullable enable

namespace MudsharpDatabaseLibrary_Unit_Tests;

[TestClass]
public class ClanElectionGovernanceMigrationTests
{
	[TestMethod]
	public void Upgrade_ExistingOfficesAllowResignationWithoutGrantingNewAuthority()
	{
		var operations = new ClanElectionGovernance().UpOperations;
		var columns = operations.OfType<AddColumnOperation>().ToDictionary(x => x.Name);
		Assert.AreEqual(1UL, Convert.ToUInt64(columns["CanResign"].DefaultValue));
		Assert.AreEqual(0UL, Convert.ToUInt64(columns["CanDirectlyAppoint"].DefaultValue));
		Assert.AreEqual(0UL, Convert.ToUInt64(columns["IsConfirmed"].DefaultValue));
		Assert.IsTrue(columns["ElectionConfirmationAppointmentId"].IsNullable);
		Assert.IsNull(columns["ElectionConfirmationAppointmentId"].DefaultValue);
		var authority = operations.OfType<AddForeignKeyOperation>().Single();
		Assert.AreEqual("Appointments", authority.PrincipalTable);
		Assert.AreEqual(ReferentialAction.Restrict, authority.OnDelete);
		Assert.IsFalse(operations.OfType<DropTableOperation>().Any());
	}
}
