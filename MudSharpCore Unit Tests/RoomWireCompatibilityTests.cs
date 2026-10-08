#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Economy.Employment;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Gathering;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomWireCompatibilityTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CraftReservation_FrozenStationAndEnvelope_PreserveLocationAndReservation(bool envelope)
	{
		const string station = """
			{"Version":"craft-station-v1","Selector":"here","CellId":8101,"ItemId":701,
			"Description":"frozen forge","ReservedAt":"2026-10-07T00:00:00+00:00","ExpiresAt":"2099-10-07T00:30:00+00:00"}
			""";
		var json = envelope ? $$"""
			{"Version":"craft-v2","CraftId":601,"Revision":2,"CraftName":"frozen craft","ActiveItemId":702,
			"PreExistingItemIds":[702],"TaskInputItemIds":[703],"OutputItemIds":[704],
			"ReservedAt":"2026-10-07T00:00:00+00:00","ExpiresAt":"2099-10-07T00:30:00+00:00",
			"Reservation":{"CraftId":601,"RevisionNumber":2,"CraftName":"frozen craft","FromPhase":1,"ToPhase":2,
			"Inputs":[],"Tools":[]},"Station":{{station}}}
			""" : station;
		var service = typeof(EmploymentTaskContext).Assembly.GetType("MudSharp.Economy.Employment.EmploymentCraftService", true)!;
		var parse = service.GetMethod(envelope ? "TryParseCraftState" : "TryParseStationState", BindingFlags.Static | BindingFlags.NonPublic)!;
		object?[] args = envelope ? [json, null, null] : [json, null];
		Assert.IsTrue((bool)parse.Invoke(null, args)!);
		var loaded = args[^1]!;
		var loadedStation = envelope ? loaded.GetType().GetProperty("Station")!.GetValue(loaded)! : loaded;
		Assert.AreEqual(8101L, loadedStation.GetType().GetProperty("RoomId")!.GetValue(loadedStation));
		Assert.AreEqual(701L, loadedStation.GetType().GetProperty("ItemId")!.GetValue(loadedStation));
		Assert.AreEqual("frozen forge", loadedStation.GetType().GetProperty("Description")!.GetValue(loadedStation));
		var serialize = service.GetMethod(envelope ? "SerializeCraftState" : "SerializeStationState", BindingFlags.Static | BindingFlags.NonPublic)!;
		using var saved = JsonDocument.Parse((string)serialize.Invoke(null, [loaded])!);
		var writtenStation = envelope ? saved.RootElement.GetProperty("Station") : saved.RootElement;
		Assert.AreEqual(8101L, writtenStation.GetProperty("CellId").GetInt64());
		Assert.IsFalse(writtenStation.TryGetProperty("RoomId", out _));
		Assert.AreEqual(DateTimeOffset.Parse("2099-10-07T00:30:00+00:00"), writtenStation.GetProperty("ExpiresAt").GetDateTimeOffset());
		if (envelope)
		{
			Assert.AreEqual(601L, saved.RootElement.GetProperty("Reservation").GetProperty("CraftId").GetInt64());
			Assert.AreEqual(703L, saved.RootElement.GetProperty("TaskInputItemIds")[0].GetInt64());
			Assert.AreEqual(704L, saved.RootElement.GetProperty("OutputItemIds")[0].GetInt64());
		}
	}

	[TestMethod]
	public void LandReceipt_FrozenVersionOneNativeIdentities_PreserveEverySourceAndHealthIdentity()
	{
		const string frozen = """
			{"Version":1,"Stage":"Completed","CapturedSources":[{"Selector":"crop",
			"Lifecycle":{"CellId":8101,"FieldId":61,"Generation":2,"DefinitionId":3,"ForageProfileId":null,"ForageProfileRevision":0,"ForageDefinitionRevision":0},
			"FundingUnits":2,"CollateralUnits":0.5,"NativeStock":10,"OpeningPrepaidFraction":0.25,"WholeNativeDebit":3,"ClosingPrepaidFraction":0.75}],
			"AppliedSources":[{"Selector":"crop",
			"Lifecycle":{"CellId":8101,"FieldId":61,"Generation":2,"DefinitionId":3,"ForageProfileId":null,"ForageProfileRevision":0,"ForageDefinitionRevision":0},
			"FundingUnits":2,"CollateralUnits":0.5,"NativeStock":10,"OpeningPrepaidFraction":0.25,"WholeNativeDebit":3,"ClosingPrepaidFraction":0.75}],
			"HealthChanges":[{"Selector":"woodland","Lifecycle":{"CellId":8102,"FieldId":62,"Generation":4,"DefinitionId":5,
			"ForageProfileId":null,"ForageProfileRevision":0,"ForageDefinitionRevision":0},"AppliedLoss":2,"DiscardedPrepaidFraction":0.125}],
			"EcologicalRequest":{"OperationId":"30000000-0000-0000-0000-000000000003","ActorId":2,"Attribution":"frozen receipt","Damage":0.5,"Pressure":1},
			"EcologicalResult":null}
			""";
		// Use the reader's actual private DTO, not a separately maintained test schema.
		var type = typeof(MagicGatheringService).GetNestedType("LandReceiptDetail", BindingFlags.NonPublic)!;
		var detail = JsonSerializer.Deserialize(frozen, type)!;
		foreach (var property in new[] { "CapturedSources", "AppliedSources" })
		{
			var allocation = ((IReadOnlyList<MagicLandSourceAllocation>)type.GetProperty(property)!.GetValue(detail)!).Single();
			Assert.AreEqual(new NativeOrganicLifecycleIdentity(8101, 61, 2, 3, null, 0, 0), allocation.Lifecycle);
			Assert.AreEqual(0.25m, allocation.OpeningPrepaidFraction);
			Assert.AreEqual(0.75m, allocation.ClosingPrepaidFraction);
		}
		var health = ((IEnumerable)type.GetProperty("HealthChanges")!.GetValue(detail)!).Cast<object>().Single();
		Assert.AreEqual(new NativeOrganicLifecycleIdentity(8102, 62, 4, 5, null, 0, 0), health.GetType().GetProperty("Lifecycle")!.GetValue(health));
		using var saved = JsonDocument.Parse(JsonSerializer.Serialize(detail, type));
		foreach (var property in new[] { "CapturedSources", "AppliedSources", "HealthChanges" })
		{
			var identity = saved.RootElement.GetProperty(property)[0].GetProperty("Lifecycle");
			Assert.IsTrue(identity.GetProperty("CellId").GetInt64() > 0);
			Assert.IsFalse(identity.TryGetProperty("RoomId", out _));
		}
		var receipt = new MagicGatheringReceipt(Guid.NewGuid(), 1, 2, 3, 4, Guid.NewGuid(), 1, MagicGatheringMethodKind.Land,
			8101, null, null, null, 8, 2.5, 1, 0, 0, 0, 0, true, true, true, true, true, "Completed", new DateTime(2026, 10, 7))
			{ LandDetailJson = frozen, EcologicalApplied = true };
		var reader = typeof(MagicGatheringService).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(x => x.Name == "LandDetails");
		var reported = (IReadOnlyDictionary<string, double>)reader.Invoke(null, [receipt])!;
		Assert.AreEqual(2.5, reported["paid:crop"]);
		Assert.AreEqual(3.0, reported["wholeDebit:crop"]);
		Assert.AreEqual(0.125, reported["discardedPrepaid:woodland"]);
		Assert.AreEqual(frozen, receipt.LandDetailJson);
	}

	[TestMethod]
	public void ReadTreatment_FrozenVersionOneCheckpoint_PreservesIdentityAndPendingWork()
	{
		const string frozen = """
			{"Version":1,"Id":"10000000-0000-0000-0000-000000000001",
			"ParentId":"20000000-0000-0000-0000-000000000002","CellId":8101,
			"SpellId":42,"CasterId":43,"ActingInstanceId":44,"PlaneIds":[5,6],
			"ProfileId":45,"Layer":2,"RequiresPresence":true,"ContinuationProgId":46,
			"Rate":0.5,"InitialBudget":10,"RemainingBudget":6.5,"TotalRepaired":3.5,
			"RemainingSeconds":17,"EarnedWork":1.25,"Revision":7,"Sequence":3,
			"AcknowledgedSequence":2,"Status":1,"CancellationRequested":false,
			"PendingRequest":{"OperationId":"30000000-0000-0000-0000-000000000003",
			"ActorId":43,"Attribution":"frozen pending work","Repair":0.75},
			"LastOperationId":"40000000-0000-0000-0000-000000000004","Diagnostic":"pending"}
			""";
		var row = new MudSharp.Models.LandRejuvenationTreatment
		{
			Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), RoomId = 8101,
			Revision = 7, Status = "Pending", Checkpoint = frozen
		};
		var loaded = DatabaseEnvironmentalMagicOperationStore.ReadTreatment(row);
		Assert.AreEqual(8101L, loaded.RoomId);
		Assert.AreEqual(7L, loaded.Revision);
		Assert.AreEqual(6.5, loaded.RemainingBudget);
		Assert.AreEqual(17.0, loaded.RemainingSeconds);
		Assert.AreEqual(1.25, loaded.EarnedWork);
		Assert.AreEqual(3L, loaded.Sequence);
		Assert.AreEqual(2L, loaded.AcknowledgedSequence);
		Assert.AreEqual(LandRejuvenationStatus.Pending, loaded.Status);
		Assert.AreEqual(Guid.Parse("20000000-0000-0000-0000-000000000002"), loaded.ParentId);
		Assert.AreEqual(Guid.Parse("30000000-0000-0000-0000-000000000003"), loaded.PendingRequest!.OperationId);
		Assert.AreEqual(0.75, loaded.PendingRequest.Repair);
		Assert.AreEqual("frozen pending work", loaded.PendingRequest.Attribution);
		CollectionAssert.AreEqual(new long[] { 5, 6 }, loaded.PlaneIds);

		row.Checkpoint = JsonSerializer.Serialize(loaded);
		using var saved = JsonDocument.Parse(row.Checkpoint);
		Assert.AreEqual(8101L, saved.RootElement.GetProperty("CellId").GetInt64());
		Assert.IsFalse(saved.RootElement.TryGetProperty("RoomId", out _));
		Assert.AreEqual(loaded.PendingRequest, DatabaseEnvironmentalMagicOperationStore.ReadTreatment(row).PendingRequest);
		row.RoomId = 9001;
		Assert.ThrowsException<InvalidOperationException>(() => DatabaseEnvironmentalMagicOperationStore.ReadTreatment(row));
	}
}
