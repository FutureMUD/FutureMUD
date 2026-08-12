#nullable enable
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProjectQueueSchedulingModelTests
{
	[TestMethod]
	public void ProjectQueueModel_PersistsSchedulingAndPhysicalClaimState()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var queue = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(ProjectLabourQueue));
		Assert.IsNotNull(queue);
		Assert.IsTrue(queue.FindProperty(nameof(ProjectLabourQueue.ActiveProjectId))!.IsNullable);
		Assert.IsTrue(queue.FindProperty(nameof(ProjectLabourQueue.ProjectLabourRequirementId))!.IsNullable);
		Assert.IsTrue(queue.FindProperty(nameof(ProjectLabourQueue.ClaimingCharacterInstanceId))!.IsNullable);
		Assert.IsNotNull(queue.FindProperty(nameof(ProjectLabourQueue.EntryType)));
		Assert.IsNotNull(queue.FindProperty(nameof(ProjectLabourQueue.CompletionMode)));
		Assert.IsNotNull(queue.FindProperty(nameof(ProjectLabourQueue.ElapsedHours)));

		var projectForeignKey = queue.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(ActiveProject));
		Assert.AreEqual(DeleteBehavior.SetNull, projectForeignKey.DeleteBehavior);
		var claimantForeignKey = queue.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(CharacterInstance));
		Assert.AreEqual(DeleteBehavior.SetNull, claimantForeignKey.DeleteBehavior);
	}

	[TestMethod]
	public void Migration_AddsSchedulingColumnsAndPreservesExistingQueueRowsAsJoinOnce()
	{
		var operations = GetUpOperations(new ProjectQueueSchedulingAndLaunchEntries());
		Assert.IsTrue(operations.OfType<AddColumnOperation>().Any(x => x.Table == "ProjectLabourQueues" && x.Name == "EntryType" && Equals(x.DefaultValue, 0)));
		Assert.IsTrue(operations.OfType<AddColumnOperation>().Any(x => x.Table == "ProjectLabourQueues" && x.Name == "CompletionMode" && Equals(x.DefaultValue, 0)));
		Assert.IsTrue(operations.OfType<AddColumnOperation>().Any(x => x.Table == "ProjectLabourQueues" && x.Name == "ClaimingCharacterInstanceId"));
		Assert.IsTrue(operations.OfType<AddColumnOperation>().Any(x => x.Table == "Characters" && x.Name == "ProjectLabourQueueLooping" && Equals(x.DefaultValue, false)));
		Assert.IsTrue(operations.OfType<AddForeignKeyOperation>().Any(x => x.Name == "FK_ProjectLabourQueues_ActiveProjects" && x.OnDelete == ReferentialAction.SetNull));
		Assert.IsTrue(operations.OfType<AddForeignKeyOperation>().Any(x => x.Name == "FK_ProjectLabourQueues_CharacterInstances" && x.OnDelete == ReferentialAction.SetNull));
	}

	private static MigrationOperation[] GetUpOperations(Migration migration)
	{
		var builder = new MigrationBuilder("MySql");
		var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(up);
		up.Invoke(migration, [builder]);
		return builder.Operations.ToArray();
	}
}
