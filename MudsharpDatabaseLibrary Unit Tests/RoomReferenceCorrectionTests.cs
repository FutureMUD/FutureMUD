#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomReferenceCorrectionTests
{
	private static RoomReferenceMap Map() => new(new[] { (9000L, (long?)8101L), (8101L, (long?)77L), (44L, (long?)null) }, new[] { 8101L, 77L });

	[TestMethod]
	public void Map_DifferentAndCollidingNamespaces_UsesParentRelationshipExactlyOnce()
	{
		Assert.AreEqual(8101L, Map().Convert("Room", 9000, "Characters #3.PositionTarget"));
		Assert.AreEqual(77L, Map().Convert("Room", 8101, "Characters #3.PositionTarget"));
		Assert.IsNull(Map().Convert("Cell", 8101, "converted"));
		Assert.IsNull(Map().Convert("Room:v2", 8101, "new"));
		Assert.IsNull(Map().Convert("Room", null, "absent"));
		Assert.IsNull(Map().Convert(null, null, "absent"));
	}

	[DataTestMethod]
	[DataRow(44L)]
	[DataRow(51L)]
	[DataRow(0L)]
	[DataRow(-1L)]
	public void Map_InvalidOrEmptyParent_ReportsOwningReference(long id)
	{
		var error = Assert.ThrowsException<InvalidOperationException>(() => Map().Convert("Room", id, "Crimes #19.ThirdPartyId"));
		StringAssert.Contains(error.Message, "Crimes #19.ThirdPartyId");
	}

	[TestMethod]
	public void Map_AmbiguousOrDeletedMapping_Refuses()
	{
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMap(new[] { (9L, (long?)81L), (9L, (long?)82L) }, new[] { 81L, 82L }));
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMap(new[] { (9L, (long?)81L), (10L, (long?)81L) }, new[] { 81L }));
		Assert.ThrowsException<InvalidOperationException>(() => Map().Convert(null, 9000L, "missing type"));
		var map = new RoomReferenceMap(new[] { (9L, (long?)81L), (10L, (long?)82L) }, new[] { 82L });
		Assert.ThrowsException<InvalidOperationException>(() => map.Resolve(9, "deleted"));
		Assert.AreEqual(82L, map.Resolve(10, "live"));
	}

	[DataTestMethod]
	[DataRow("room")]
	[DataRow(" Room ")]
	[DataRow("Room:v3")]
	[DataRow("Room:v2:extra")]
	public void Map_MalformedRoomDiscriminator_RefusesWithoutNormalising(string type)
	{
		Assert.ThrowsException<InvalidOperationException>(() => Map().Convert(type, 9000, "typed column"));
	}

	[TestMethod]
	public void Emote_ActualTargetAndOptionalOther_OnlyChangesTheirValueSpans()
	{
		const string xml = "<?xml version='1.0'?>\r\n<Emote PlayerMode='false'><!--Room--><RawText><![CDATA[Type=Room and $0]]></RawText>\r\n<Token Type='OptionalItself' TargetType='Room' TargetId='9000' OtherType=\"Room\" OtherId=\"8101\" extra='&quot;Room&quot;'/>\r\n<Token Type='Room' TargetType='Cell' TargetId='9000' /></Emote>";
		var expected = xml.Replace("TargetType='Room' TargetId='9000'", "TargetType='Cell' TargetId='8101'")
			.Replace("OtherType=\"Room\" OtherId=\"8101\"", "OtherType=\"Cell\" OtherId=\"77\"");
		var actual = RoomReferenceXml.Rewrite(xml, true, Map(), "Characters #2.PositionEmote");
		Assert.AreEqual(expected, actual);
		Assert.AreEqual(actual, RoomReferenceXml.Rewrite(actual, true, Map(), "rerun"));
	}

	[DataTestMethod]
	[DataRow("ZeroGravityTether", "<AnchorType>Room</AnchorType><AnchorId>9000</AnchorId><PhysicalTetherId>4</PhysicalTetherId><MaximumRooms>8</MaximumRooms>", "<AnchorType>Cell</AnchorType><AnchorId>8101</AnchorId><PhysicalTetherId>4</PhysicalTetherId><MaximumRooms>8</MaximumRooms>")]
	[DataRow("SpellZeroGravityTether", "<AnchorType><![CDATA[Room]]></AnchorType><AnchorId>9000</AnchorId><MaximumRooms>8</MaximumRooms>", "<AnchorType>Cell</AnchorType><AnchorId>8101</AnchorId><MaximumRooms>8</MaximumRooms>")]
	[DataRow("OverrideDescFromProg", "<FixedPerceiver id='9000' type='Room'/><ModifiedDescription><![CDATA[Room untouched]]></ModifiedDescription>", "<FixedPerceiver id='8101' type='Cell'/><ModifiedDescription><![CDATA[Room untouched]]></ModifiedDescription>")]
	[DataRow("OverrideSDescFromProg", "<FixedPerceiver type='Room' id='9000' custom='keep'/><Tag>x</Tag>", "<FixedPerceiver type='Cell' id='8101' custom='keep'/><Tag>x</Tag>")]
	public void Effects_KnownFactoryPaths_PreserveUnrelatedPayload(string kind, string before, string after)
	{
		var xml = $"<Effects>\n<Effect><Type>{kind}</Type><Remaining>120</Remaining><Effect>{before}</Effect></Effect>\n</Effects>";
		var expected = xml.Replace(before, after);
		Assert.AreEqual(expected, RoomReferenceXml.Rewrite(xml, false, Map(), "Bodies #2.EffectData"));
	}

	[TestMethod]
	public void CheckResult_TargetAndToolPaths_MapOnlyProvableNamespaces()
	{
		const string xml = "<Effects><Effect><Type>CheckResult</Type><Effect TargetType='Room' TargetId='9000' ToolType='GameItem' ToolId='8101' Outcome='4'/></Effect><Effect><Type>CheckResult</Type><Effect TargetType='Character' TargetId='9' ToolType='Room' ToolId='9000'/></Effect></Effects>";
		var expected = xml.Replace("TargetType='Room' TargetId='9000'", "TargetType='Cell' TargetId='8101'")
			.Replace("ToolType='Room' ToolId='9000'", "ToolType='Cell' ToolId='8101'");
		Assert.AreEqual(expected, RoomReferenceXml.Rewrite(xml, false, Map(), "GameItems #8.EffectData"));
	}

	[TestMethod]
	public void CheckResult_HistoricalToolTypeBug_RefusesAmbiguousStoredIdentity()
	{
		const string xml = "<Effects><Effect><Type>CheckResult</Type><Effect TargetType='Room' TargetId='9000' ToolType='Room' ToolId='8101'/></Effect></Effects>";
		StringAssert.Contains(Assert.ThrowsException<InvalidOperationException>(() => RoomReferenceXml.Rewrite(xml, false, Map(), "Characters #8.EffectData")).Message, "older writer copied TargetType");
	}

	[DataTestMethod]
	[DataRow("PrivateProperty")]
	[DataRow("PermitWork")]
	public void Controller_HistoricallyUnsupportedParent_IsAnUnchangedInventoryExemption(string kind)
	{
		var xml = $"<Effects><Effect><Type>{kind}</Type><Effect><ControllerType>Room</ControllerType><ControllerId>9000</ControllerId></Effect></Effect></Effects>";
		Assert.AreSame(xml, RoomReferenceXml.Rewrite(xml, false, Map(), "Cells #8.EffectData"));
	}

	[TestMethod]
	public void Effects_ZeroSentinelsUnknownFactoriesAndNarrativeMetadata_AreUnchanged()
	{
		const string xml = "<Effects><Effect><Type>OverrideDescFromProg</Type><Effect><FixedPerceiver type='Room' id='0'/></Effect></Effect><Effect><Type>AdminSight</Type><Effect>opaque Room text</Effect></Effect><Effect><Type>RecentSpeechContextEffect</Type><Effect><Events><Event><TargetFrameworkItemType><![CDATA[Room]]></TargetFrameworkItemType><TargetId>9000</TargetId></Event></Events></Effect></Effect></Effects>";
		Assert.AreSame(xml, RoomReferenceXml.Rewrite(xml, false, Map(), "unchanged"));
	}

	[TestMethod]
	public void MagicSpellParent_ExactRecursiveChildrenPath_RemapsAndLeavesStoredSpellOpaque()
	{
		const string child = "<Effect><Type>ZeroGravityTether</Type><Effect><AnchorType>Room</AnchorType><AnchorId>9000</AnchorId></Effect></Effect>";
		var nested = $"<Effect><Type>MagicSpellParent</Type><Effect><Children>{child}</Children><StoredSpell><Model><![CDATA[Type=Room ParentId=9000]]></Model></StoredSpell></Effect></Effect>";
		var xml = $"<Effects><Effect><Type>MagicSpellParent</Type><Effect><Children>{nested}</Children><StoredSpell><Model><![CDATA[{child}]]></Model></StoredSpell></Effect></Effect></Effects>";
		var expected = xml.Replace("<Children>" + child + "</Children>", "<Children>" + child.Replace("<AnchorType>Room</AnchorType><AnchorId>9000</AnchorId>", "<AnchorType>Cell</AnchorType><AnchorId>8101</AnchorId>") + "</Children>");
		Assert.AreEqual(expected, RoomReferenceXml.Rewrite(xml, false, Map(), "Characters #1.EffectData"));
	}

	[TestMethod]
	public void SourceEmotionalChildren_HaveNoRoomReferencesAndPreserveExactPayloadBytes()
	{
		const string children = "<Effect><Type>SpellSourceFury</Type><Effect version='1' group='Room narrative' unitSeconds='600' capUnits='36' grade='3' nativePower='5' intensity='1' endurancePoints='4' enduranceTrait='9000' unitsPerSourcePoint='0.5'/></Effect><Effect><Type>SpellSourceCalm</Type><Effect version='1' group='calm' unitSeconds='600' capUnits='24' grade='2' nativePower='4' intensity='1' endurancePoints='0' attackbreak='true'/></Effect>";
		var xml = $"<Effects><Effect><Type>MagicSpellParent</Type><Effect><Children>{children}</Children></Effect></Effect></Effects>";
		Assert.AreSame(xml, RoomReferenceXml.Rewrite(xml, false, Map(), "Characters #1.EffectData"));
		var direct = $"<Effects>{children}</Effects>";
		Assert.AreSame(direct, RoomReferenceXml.Rewrite(direct, false, Map(), "Bodies #1.EffectData"));
	}

	[TestMethod]
	public void EffectFactories_CustomSerializer_RefusesAndBuiltInInventoryMatchesSource()
	{
		Assert.ThrowsException<InvalidOperationException>(() => RoomReferenceXml.Rewrite("<Effects><Effect><Type>CustomEffect</Type><Effect/></Effect></Effects>", false, Map(), "Bodies #8.EffectData"));
		var root = new DirectoryInfo(AppContext.BaseDirectory);
		while (root is not null && !Directory.Exists(Path.Combine(root.FullName, "MudSharpCore", "Effects"))) root = root.Parent;
		Assert.IsNotNull(root);
		var names = new HashSet<string>(StringComparer.Ordinal);
		foreach (var directory in new[] { "Effects", "Arenas" })
		foreach (var file in Directory.EnumerateFiles(Path.Combine(root!.FullName, "MudSharpCore", directory), "*.cs", SearchOption.AllDirectories))
		foreach (Match registration in Regex.Matches(File.ReadAllText(file), "RegisterFactory\\s*\\(\\s*\"([^\"]+)\"")) names.Add(registration.Groups[1].Value);
		CollectionAssert.AreEquivalent(names.ToArray(), RoomReferenceEffectTypes.Names.ToArray());
	}

	[DataTestMethod]
	[DataRow("<Emote><Token TargetType='Room'/></Emote>")]
	[DataRow("<Emote><Token TargetId='9000'/></Emote>")]
	[DataRow("<Emote><Token OtherType='Room' OtherId='wat'/></Emote>")]
	[DataRow("<Emote><Token TargetType='Room' TargetId='0'/></Emote>")]
	[DataRow("<Emote><Token TargetType='Room' TargetId='44'/></Emote>")]
	[DataRow("<Emote><Token TargetType='Room' TargetId='9000'></Emote>")]
	[DataRow("<!DOCTYPE Emote [<!ENTITY x 'Room'>]><Emote><Token TargetType='&x;' TargetId='9000'/></Emote>")]
	public void Emote_MalformedUnmappedOrExternalEntity_ReportsRowWithoutPayload(string xml)
	{
		StringAssert.Contains(Assert.ThrowsException<InvalidOperationException>(() => RoomReferenceXml.Rewrite(xml, true, Map(), "CharacterInstances #21.PositionEmote")).Message, "CharacterInstances #21.PositionEmote");
	}

	[DataTestMethod]
	[DataRow("<AnchorType>Room</AnchorType><AnchorType>Cell</AnchorType><AnchorId>9000</AnchorId>")]
	[DataRow("<AnchorType>Room</AnchorType><AnchorId>9000</AnchorId><AnchorId>8101</AnchorId>")]
	[DataRow("<AnchorType>Room<!--ambiguous--></AnchorType><AnchorId>9000</AnchorId>")]
	public void Effects_DuplicateOrNonScalarReference_Refuses(string payload)
	{
		var xml = $"<Effects><Effect><Type>ZeroGravityTether</Type><Effect>{payload}</Effect></Effect></Effects>";
		Assert.ThrowsException<InvalidOperationException>(() => RoomReferenceXml.Rewrite(xml, false, Map(), "Bodies #3.EffectData"));
	}

	[TestMethod]
	public void Xml_AbsentAndExistingChildReferences_AreUnchanged()
	{
		Assert.IsNull(RoomReferenceXml.Rewrite(null, true, Map(), "null"));
		Assert.AreEqual("", RoomReferenceXml.Rewrite("", false, Map(), "empty"));
		const string xml = "<Emote><Token Type='Room' TargetType='Room:v2' TargetId='9000'/><Token TargetType='Cell' TargetId='9000'/></Emote>";
		Assert.AreSame(xml, RoomReferenceXml.Rewrite(xml, true, Map(), "already migrated"));
	}

	[TestMethod]
	public void Migrations_TargetedMarkers_FollowStructuralGuardsAndPrecedeDestructiveDdl()
	{
		var expansion = new CellSpatialExpansion().UpOperations.ToList();
		Assert.IsFalse(expansion.OfType<SqlOperation>().Any(x => x.Sql.Contains("REGEXP_LIKE") || x.Sql.Contains("candidate_columns")));
		Assert.AreEqual(RoomReferenceMigrationInterceptor.Expansion, ((SqlOperation)expansion[3]).Sql);
		Assert.IsTrue(((SqlOperation)expansion[3]).SuppressTransaction);
		var contraction = new CellSpatialContraction().UpOperations.ToList();
		Assert.IsFalse(contraction.OfType<SqlOperation>().Any(x => x.Sql.Contains("candidate_columns")));
		Assert.IsTrue(((SqlOperation)contraction[0]).Sql.Contains("information_schema.VIEWS"));
		Assert.AreEqual(RoomReferenceMigrationInterceptor.Contraction, ((SqlOperation)contraction[2]).Sql);
		var verify = contraction.FindIndex(x => x is SqlOperation s && s.Sql == RoomReferenceMigrationInterceptor.Verify);
		Assert.IsTrue(verify > contraction.FindIndex(x => x is SqlOperation s && s.Sql.Contains("(TRUE)")));
		Assert.IsTrue(verify < contraction.FindIndex(x => x is DropTableOperation or DropForeignKeyOperation or DropColumnOperation));
		var repair = new TargetedLegacyRoomReferences();
		Assert.AreEqual(RoomReferenceMigrationInterceptor.Repair, ((SqlOperation)repair.UpOperations.Single()).Sql);
		Assert.IsTrue(((SqlOperation)repair.UpOperations.Single()).SuppressTransaction);
		Assert.ThrowsException<NotSupportedException>(() => _ = repair.DownOperations);
	}

	[TestMethod]
	public void Provider_GeneratesDedicatedSuppressedMarkersAndRawSqlSignals()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=127.0.0.1;database=unused;uid=unused;password=unused", new MySqlServerVersion(new Version(8, 0, 36))).Options);
		Assert.IsInstanceOfType(context.GetService<IInterceptors>().Aggregate<IDbCommandInterceptor>(), typeof(RoomReferenceMigrationInterceptor));
		foreach (var marker in new[] { RoomReferenceMigrationInterceptor.Expansion, RoomReferenceMigrationInterceptor.Contraction, RoomReferenceMigrationInterceptor.Verify, RoomReferenceMigrationInterceptor.Repair })
		{
			var operation = new SqlOperation { Sql = marker, SuppressTransaction = true };
			var commands = context.GetService<IMigrationsSqlGenerator>().Generate(new[] { operation });
			Assert.AreEqual(1, commands.Count);
			Assert.AreEqual(marker, commands.Single().CommandText.Trim());
			Assert.IsTrue(commands.Single().TransactionSuppressed);
			StringAssert.Contains(commands.Single().CommandText, "SIGNAL SQLSTATE '45000'");
		}
	}
}
