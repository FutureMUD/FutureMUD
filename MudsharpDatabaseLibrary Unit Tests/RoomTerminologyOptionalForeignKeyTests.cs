#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomTerminologyOptionalForeignKeyTests
{
	private const string DatabaseName = "fixture";
	private const string OldName = "FK_CharacterInstances_Cells";
	private const string NewName = "FK_CharacterInstances_Rooms";
	private static readonly Lazy<string> GeneratedSql = new(GenerateSql);

	[TestMethod]
	public void Naming_ActualPredecessorOmission_DoesNotProduceUnconditionalForeignKeyChanges()
	{
		var original = new CharacterInstances().UpOperations;
		var table = original.OfType<CreateTableOperation>().Single(x=>x.Name=="CharacterInstances");
		Assert.AreEqual(0,table.ForeignKeys.Count,"The compatibility migration deliberately omits all physical FKs.");
		CollectionAssert.AreEqual(new[] {"LocationId"},original.OfType<CreateIndexOperation>()
			.Single(x=>x.Name==OldName+"_idx").Columns);
		var operations = new RoomTerminology().UpOperations;
		Assert.IsFalse(operations.OfType<DropForeignKeyOperation>().Any(x=>x.Name==OldName));
		Assert.IsFalse(operations.OfType<AddForeignKeyOperation>().Any(x=>x.Name==NewName));
		var index = operations.OfType<RenameIndexOperation>().Single(x=>x.Name==OldName+"_idx");
		Assert.AreEqual(NewName+"_idx",index.NewName);
		Assert.AreEqual("CharacterInstances",index.Table);
		var changes = OptionalChanges();
		Assert.AreEqual(2,changes.Length);
		foreach (var change in changes)
		{
			Assert.IsTrue(change.SuppressTransaction);
			Assert.AreEqual("DO 0",SelectStatement(change.Sql,0,null,null));
			Assert.IsTrue(change.Sql.Contains("DEALLOCATE PREPARE"));
			Assert.IsTrue(change.Sql.EndsWith("= NULL;",StringComparison.Ordinal));
		}
	}

	[DataTestMethod]
	[DataRow("absent",false)]
	[DataRow("valid",false)]
	[DataRow("restrict-update",false)]
	[DataRow("wrong-table",true)]
	[DataRow("wrong-column",true)]
	[DataRow("wrong-principal-table",true)]
	[DataRow("wrong-principal-schema",true)]
	[DataRow("wrong-principal-column",true)]
	[DataRow("wrong-ordinal",true)]
	[DataRow("extra-column",true)]
	[DataRow("wrong-delete",true)]
	[DataRow("cascade-update",true)]
	[DataRow("set-null-update",true)]
	public void Naming_GeneratedPreflight_AcceptsOnlyAbsenceOrSupportedPresence(string variant,bool refuses)
	{
		var guard = Regex.Match(GeneratedSql.Value,
			@"(?m)^  IF (.+) THEN\r?\n   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CharacterInstances_Cells';");
		Assert.IsTrue(guard.Success);
		var fixture = Variant(variant);
		var expression = FoldMetadataQueries(guard.Groups[1].Value,fixture);
		Assert.AreEqual(refuses,Convert.ToBoolean(new DataTable().Compute(expression,"")),variant);
	}

	[DataTestMethod]
	[DataRow("NO ACTION")]
	[DataRow("RESTRICT")]
	public void Naming_ValidPresence_RecreatesTheExactCapturedActions(string updateRule)
	{
		var changes = OptionalChanges();
		Assert.AreEqual($"ALTER TABLE `CharacterInstances` DROP FOREIGN KEY `{OldName}`",
			SelectStatement(changes[0].Sql,1,updateRule,"SET NULL"));
		Assert.AreEqual($"ALTER TABLE `CharacterInstances` ADD CONSTRAINT `{NewName}` FOREIGN KEY (`LocationId`) REFERENCES `Rooms` (`Id`) ON DELETE SET NULL ON UPDATE {updateRule}",
			SelectStatement(changes[1].Sql,1,updateRule,"SET NULL"));
	}

	[DataTestMethod]
	[DataRow(0,"NO ACTION",null)]
	[DataRow(0,null,"SET NULL")]
	[DataRow(1,null,"SET NULL")]
	[DataRow(1,"NO ACTION",null)]
	[DataRow(1,"CASCADE","SET NULL")]
	[DataRow(1,"SET NULL","SET NULL")]
	[DataRow(1,"NO ACTION","CASCADE")]
	[DataRow(2,null,null)]
	public void Naming_InvalidCapture_CannotSelectForeignKeyDdlOrAnAbsenceNoOp(int present,string? updateRule,string? deleteRule)
	{
		foreach (var change in OptionalChanges())
		{
			Assert.IsNull(SelectStatement(change.Sql,present,updateRule,deleteRule));
			Assert.IsNull(SelectStatement(change.Sql,0,null,null,"unexpected-name"));
			Assert.IsNull(SelectStatement(change.Sql,0,null,null,missing: true));
		}
	}

	[DataTestMethod]
	[DataRow("absent",0,null,null,false)]
	[DataRow("valid",1,"NO ACTION","SET NULL",false)]
	[DataRow("restrict-update",1,"RESTRICT","SET NULL",false)]
	[DataRow("valid",0,null,null,true)]
	[DataRow("absent",1,"NO ACTION","SET NULL",true)]
	[DataRow("restrict-update",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-delete",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-table",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-column",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-principal-table",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-principal-schema",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-principal-column",1,"NO ACTION","SET NULL",true)]
	[DataRow("wrong-ordinal",1,"NO ACTION","SET NULL",true)]
	[DataRow("extra-column",1,"NO ACTION","SET NULL",true)]
	[DataRow("absent",0,"NO ACTION",null,true)]
	[DataRow("absent",2,null,null,true)]
	public void Naming_GeneratedPostflight_RefusesChangedPresenceShapeOrExactActions(string variant,int present,
		string? updateRule,string? deleteRule,bool refuses)
	{
		var match = Regex.Match(GeneratedSql.Value,
			@"WHERE (original.ConstraintName<>.*?)\) THEN\s+SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: optional foreign-key presence, shape or actions changed';",
			RegexOptions.Singleline);
		Assert.IsTrue(match.Success);
		var fixture = Variant(variant);
		if (fixture is not null)
		{
			fixture = fixture with {Name=NewName, PrincipalTable=fixture.PrincipalTable=="Cells" ? "Rooms" : fixture.PrincipalTable};
		}
		var expression = FoldMetadataQueries(match.Groups[1].Value,fixture)
			.Replace("original.ConstraintName","OriginalName")
			.Replace("original.IsPresent","IsPresent")
			.Replace("original.UpdateRule","OriginalUpdate")
			.Replace("original.DeleteRule","OriginalDelete")
			.Replace("current_rule.CONSTRAINT_NAME","CurrentName")
			.Replace("current_rule.UPDATE_RULE","CurrentUpdate")
			.Replace("current_rule.DELETE_RULE","CurrentDelete");
		using var rows = Table("OriginalName","IsPresent","OriginalUpdate","OriginalDelete","CurrentName","CurrentUpdate","CurrentDelete");
		var current = fixture is {Table: "CharacterInstances"} ? fixture : null;
		rows.Rows.Add(NewName,present,Value(updateRule),Value(deleteRule),Value(current?.Name),Value(current?.Update),Value(current?.Delete));
		Assert.AreEqual(refuses,rows.Select(expression).Length!=0,variant);
	}

	[TestMethod]
	public void Naming_GeneratedSql_CapturesBeforeDdlAndChecksThenClearsOptionalState()
	{
		var sql = GeneratedSql.Value;
		Assert.IsTrue(sql.Contains("FROM (SELECT 1 AS singleton) singleton\n  LEFT JOIN information_schema.REFERENTIAL_CONSTRAINTS original"));
		Assert.IsTrue(sql.IndexOf("CLOSE definitions;",StringComparison.Ordinal)<sql.IndexOf("CREATE TEMPORARY TABLE `fm_room_naming_optional_fk",StringComparison.Ordinal));
		Assert.IsTrue(sql.IndexOf("CALL `fm_room_terminology_20261007043900`(FALSE)",StringComparison.Ordinal)<sql.IndexOf("DROP FOREIGN KEY",sql.IndexOf("CALL `fm_room_terminology_20261007043900`(FALSE)",StringComparison.Ordinal),StringComparison.Ordinal));
		Assert.IsFalse(Regex.IsMatch(sql,@"(?m)^ALTER TABLE `CharacterInstances` (?:DROP FOREIGN KEY `FK_CharacterInstances_Cells`|ADD CONSTRAINT `FK_CharacterInstances_Rooms`)"));
		Assert.IsTrue(sql.Contains("optional foreign-key capture is incomplete"));
		Assert.IsTrue(sql.Contains("optional foreign-key capture changed after rename"));
		Assert.IsTrue(sql.Contains("old optional foreign-key name remains"));
		Assert.IsTrue(sql.Contains("DROP TEMPORARY TABLE `fm_room_naming_optional_fk_20261007043900`"));
		Assert.IsFalse(sql.Contains("CREATE TEMPORARY TABLE IF NOT EXISTS"));
		Assert.IsFalse(sql.Contains("DROP TEMPORARY TABLE IF EXISTS"));
		Assert.IsFalse(sql.Contains("`)<>1 OR EXISTS"),"Keep temporary-table count and comparison in separate queries.");
		var oldNameGuard = Regex.Match(sql,@"IF (EXISTS\(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS[^\n]+) THEN\r?\n   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: old optional foreign-key name remains';");
		Assert.IsTrue(oldNameGuard.Success);
		Assert.IsTrue(Convert.ToBoolean(new DataTable().Compute(FoldMetadataQueries(oldNameGuard.Groups[1].Value,new ForeignKeyFixture()),"")));
	}

	private static SqlOperation[] OptionalChanges() => new RoomTerminology().UpOperations.OfType<SqlOperation>()
		.Where(x=>x.Sql.Contains("PREPARE fm_room_naming_optional_statement_20261007043900")).ToArray();

	private static string GenerateSql()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused;uid=unused;password=unused",ServerVersion.Parse("8.0.45-mysql")).Options);
		var migration = new RoomTerminology();
		var model = context.GetService<IModelRuntimeInitializer>().Initialize(migration.TargetModel,designTime: true);
		return string.Join("\n",context.GetService<IMigrationsSqlGenerator>().Generate(migration.UpOperations,model).Select(x=>x.CommandText)).Replace("\r\n","\n");
	}

	// Evaluate the emitted metadata predicates against managed fixture tables.
	// This deliberately supports only the information_schema SELECTs used here;
	// it does not execute MySQL, DDL, procedures or arbitrary SQL.
	private static string FoldMetadataQueries(string expression,ForeignKeyFixture? fixture)
	{
		var prefix = new Regex(@"\(SELECT COUNT\(\*\)|EXISTS\(SELECT 1");
		while (prefix.Match(expression) is {Success: true} match)
		{
			var opening = expression.IndexOf('(',match.Index);
			var depth = 1;
			var quoted = false;
			var end = opening+1;
			for (; end<expression.Length; end++)
			{
				if (expression[end]=='\'') quoted=!quoted;
				if (quoted) continue;
				if (expression[end]=='(') depth++;
				if (expression[end]==')' && --depth==0) break;
			}
			Assert.IsTrue(end<expression.Length,"Unbalanced metadata subquery.");
			var query = expression[(opening+1)..end];
			var select = Regex.Match(query,@"^SELECT (?:COUNT\(\*\)|1) FROM information_schema\.(\w+) WHERE (.+)$",RegexOptions.Singleline);
			Assert.IsTrue(select.Success,"Unexpected metadata query: "+query);
			using var table = MetadataTable(select.Groups[1].Value,fixture);
			var count = table.Select(NormalizePredicate(select.Groups[2].Value)).Length;
			var replacement = expression[match.Index..opening].StartsWith("EXISTS",StringComparison.Ordinal)
				? (count>0 ? "True" : "False") : count.ToString(CultureInfo.InvariantCulture);
			expression = expression[..match.Index]+replacement+expression[(end+1)..];
		}
		return expression;
	}

	private static string NormalizePredicate(string expression) => Regex.Replace(expression,
		@"LOWER\((\w+)\)","$1").Replace("DATABASE()","'"+DatabaseName+"'");

	private static DataTable MetadataTable(string kind,ForeignKeyFixture? fixture)
	{
		var table = kind switch
		{
			"TABLE_CONSTRAINTS" => Table("CONSTRAINT_SCHEMA","TABLE_NAME","CONSTRAINT_NAME","CONSTRAINT_TYPE"),
			"REFERENTIAL_CONSTRAINTS" => Table("CONSTRAINT_SCHEMA","TABLE_NAME","CONSTRAINT_NAME","DELETE_RULE","UPDATE_RULE"),
			"KEY_COLUMN_USAGE" => Table("TABLE_SCHEMA","TABLE_NAME","CONSTRAINT_NAME","COLUMN_NAME","ORDINAL_POSITION","REFERENCED_TABLE_SCHEMA","REFERENCED_TABLE_NAME","REFERENCED_COLUMN_NAME"),
			_ => throw new AssertFailedException("Unsupported metadata table: "+kind)
		};
		if (fixture is null) return table;
		if (kind=="TABLE_CONSTRAINTS") table.Rows.Add(DatabaseName,fixture.Table,fixture.Name,"FOREIGN KEY");
		else if (kind=="REFERENTIAL_CONSTRAINTS") table.Rows.Add(DatabaseName,fixture.Table,fixture.Name,fixture.Delete,fixture.Update);
		else for (var i=0; i<fixture.Columns.Length; i++)
		{
			table.Rows.Add(DatabaseName,fixture.Table,fixture.Name,fixture.Columns[i],fixture.Ordinal+i,
				fixture.PrincipalSchema,fixture.PrincipalTable,fixture.PrincipalColumn);
		}
		return table;
	}

	private static DataTable Table(params string[] columns)
	{
		var table = new DataTable {CaseSensitive=false,Locale=CultureInfo.InvariantCulture};
		foreach (var column in columns) table.Columns.Add(column,column is "ORDINAL_POSITION" or "IsPresent" ? typeof(int) : typeof(string));
		return table;
	}

	private static object Value(string? value) => (object?)value ?? DBNull.Value;

	private static string? SelectStatement(string sql,int present,string? update,string? delete,string name=NewName,bool missing=false)
	{
		var filter = Regex.Match(sql,@" WHERE (ConstraintName=.*?)\);\s*PREPARE",RegexOptions.Singleline);
		Assert.IsTrue(filter.Success);
		using var captures = Table("ConstraintName","IsPresent","UpdateRule","DeleteRule");
		if (!missing) captures.Rows.Add(name,present,Value(update),Value(delete));
		if (captures.Select(filter.Groups[1].Value).Length==0) return null;
		var choices = Regex.Match(sql,@"SELECT CASE IsPresent WHEN 0 THEN '([^']+)' WHEN 1 THEN (.+) END");
		Assert.IsTrue(choices.Success);
		if (present==0) return choices.Groups[1].Value;
		var branch = choices.Groups[2].Value;
		if (branch.StartsWith("'",StringComparison.Ordinal)) return branch[1..^1];
		Assert.IsTrue(branch.StartsWith("CONCAT(",StringComparison.Ordinal));
		var operands = branch[7..^1];
		var tokens = Regex.Matches(operands,@"'([^']*)'|DeleteRule|UpdateRule");
		Assert.IsTrue(Regex.IsMatch(Regex.Replace(operands,@"'[^']*'|DeleteRule|UpdateRule",""),@"^[,\s]*$"));
		return string.Concat(tokens.Select(x=>x.Value=="DeleteRule" ? delete : x.Value=="UpdateRule" ? update : x.Groups[1].Value));
	}

	private static ForeignKeyFixture? Variant(string variant)
	{
		var fixture = new ForeignKeyFixture();
		return variant switch
		{
			"absent" => null,
			"valid" => fixture,
			"restrict-update" => fixture with {Update="RESTRICT"},
			"wrong-table" => fixture with {Table="UnexpectedTable"},
			"wrong-column" => fixture with {Columns=["BodyId"]},
			"wrong-principal-table" => fixture with {PrincipalTable="UnexpectedTable"},
			"wrong-principal-schema" => fixture with {PrincipalSchema="another_database"},
			"wrong-principal-column" => fixture with {PrincipalColumn="UnexpectedId"},
			"wrong-ordinal" => fixture with {Ordinal=2},
			"extra-column" => fixture with {Columns=["LocationId","BodyId"]},
			"wrong-delete" => fixture with {Delete="CASCADE"},
			"cascade-update" => fixture with {Update="CASCADE"},
			"set-null-update" => fixture with {Update="SET NULL"},
			_ => throw new AssertFailedException("Unknown fixture variant: "+variant)
		};
	}

	private sealed record ForeignKeyFixture
	{
		public string Name { get; init; } = OldName;
		public string Table { get; init; } = "CharacterInstances";
		public string[] Columns { get; init; } = ["LocationId"];
		public int Ordinal { get; init; } = 1;
		public string PrincipalSchema { get; init; } = DatabaseName;
		public string PrincipalTable { get; init; } = "Cells";
		public string PrincipalColumn { get; init; } = "Id";
		public string Delete { get; init; } = "SET NULL";
		public string Update { get; init; } = "NO ACTION";
	}
}
