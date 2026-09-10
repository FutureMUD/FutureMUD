using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Migrations;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeLanguageMigrationTests
{
	[TestMethod]
	public void UpgradeClassifiesExistingLearnerAccentsBeforeDroppingTheirPointer()
	{
		var operations = new NativeLanguagesAndAccentRoles().UpOperations.ToList();
		var role = operations.OfType<AddColumnOperation>().Single(x => x.Table == "Accents" && x.Name == "Role");
		var conversion = operations.OfType<SqlOperation>().Single();
		var oldPointer = operations.OfType<DropColumnOperation>().Single(x => x.Name == "DefaultLearnerAccentId");
		Assert.IsTrue(operations.IndexOf(role) < operations.IndexOf(conversion));
		Assert.IsTrue(operations.IndexOf(conversion) < operations.IndexOf(oldPointer));
		StringAssert.Contains(conversion.Sql, "l.DefaultLearnerAccentId = a.Id");
		StringAssert.Contains(conversion.Sql, "a.Role = 2");
		Assert.IsTrue(operations.OfType<AddColumnOperation>().Where(x => x.Name is "NativeLanguageId" or "AcquisitionAccentId")
			.All(x => x.IsNullable));
		Assert.IsTrue(operations.OfType<CreateTableOperation>().Any(x => x.Name == "AccentsAssociatedLanguages"));
	}
}
