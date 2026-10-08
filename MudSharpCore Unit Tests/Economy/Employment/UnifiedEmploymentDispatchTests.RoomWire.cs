#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void EmploymentPersistence_FrozenLocationPayloads_PreservePlanOrderAndNullableTargets(bool hasOptionalTargets)
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(2100, "Manager").Object;
			var destination = Room(8101, "delivery room").Object;
			var optionalDestination = Room(8102, "stable destination").Object;
			var hotelLocation = Room(8103, "guest room").Object;
			var sourceShop = new Mock<IPermanentShop>();
			var targetShop = new Mock<IPermanentShop>();
			var merchandise = new Mock<IMerchandise>();
			sourceShop.SetupGet(x => x.Id).Returns(2201);
			targetShop.SetupGet(x => x.Id).Returns(2202);
			merchandise.SetupGet(x => x.Id).Returns(2203);
			targetShop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(2301);
			var hotel = new Mock<IHotel>();
			hotel.SetupGet(x => x.Id).Returns(2302);
			hotel.SetupGet(x => x.Property).Returns(property.Object);
			property.SetupGet(x => x.Hotel).Returns(hotel.Object);
			var hotelRoom = new Mock<IHotelRoom>();
			hotelRoom.SetupGet(x => x.Room).Returns(hotelLocation);
			hotel.SetupGet(x => x.Rooms).Returns([hotelRoom.Object]);
			var world = Gameworld(currency.Object, new Dictionary<long, ICharacter> { [manager.Id] = manager },
				[destination, optionalDestination, hotelLocation], shopsToAdd: [sourceShop.Object, targetShop.Object],
				propertiesToAdd: [property.Object]);
			IEmploymentHost host = new PersistedEmploymentHost(2401, "Frozen plan host", world.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthoritySet.All.Authorities), null);
			host.TaskBoard.CreateActiveTask("historical targets", new EmploymentActionPlan([
				new DeliverItemsActionStep(destination),
				new ShopStockTransferActionStep(sourceShop.Object, targetShop.Object, merchandise.Object, destination),
				new ReturnAssetActionStep(null, null, destination),
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lead,
					destination: hasOptionalTargets ? optionalDestination : null),
				new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.RoomReady,
					room: hasOptionalTargets ? hotelRoom.Object : null)
			]), manager);

			var rows = context.EmploymentActionSteps.OrderBy(x => x.SortOrder).ToArray();
			Assert.AreEqual(5, rows.Length);
			for (var i = 0; i < rows.Length; i++)
			{
				using var written = JsonDocument.Parse(rows[i].BoardText!);
				var historicalKey = i == 4 ? "RoomCellId" : "DestinationCellId";
				Assert.IsTrue(written.RootElement.TryGetProperty(historicalKey, out _));
				Assert.IsFalse(written.RootElement.TryGetProperty(i == 4 ? "RoomRoomId" : "DestinationRoomId", out _));
				rows[i].DestinationRoomId = null;
			}
			// Literal historical JSON, not produced by the new payload serializer.
			rows[0].BoardText = """{"DestinationCellId":8101,"ContainerId":null,"ContainerTag":null}""";
			rows[1].BoardText = """{"SourceShopId":2201,"TargetShopId":2202,"TargetMerchandiseId":2203,"DestinationCellId":8101,"ContainerId":null,"ContainerTag":null}""";
			rows[2].BoardText = """{"ContainerId":null,"ContainerTag":null,"DestinationCellId":8101,"DestinationContainerId":null,"DestinationContainerTag":null}""";
			rows[3].BoardText = hasOptionalTargets
				? """{"Operation":"Lead","DestinationCellId":8102,"WaiveFees":true}"""
				: """{"Operation":"Lead","DestinationCellId":null,"WaiveFees":true}""";
			rows[4].BoardText = hasOptionalTargets
				? """{"Operation":"RoomReady","PropertyId":2301,"RoomCellId":8103,"Note":"historical note"}"""
				: """{"Operation":"RoomReady","PropertyId":2301,"RoomCellId":null,"Note":"historical note"}""";
			context.SaveChanges();

			IEmploymentHost reloaded = new PersistedEmploymentHost(2401, "Frozen plan host", world.Object, currency.Object);
			var steps = reloaded.Employment.TaskBoard.ActiveTasks.Single().ActionPlan.Steps;
			Assert.AreEqual(5, steps.Count, "No historical step may disappear during loading.");
			CollectionAssert.AreEqual(new[] { EmploymentActionStepType.DeliverItems, EmploymentActionStepType.ShopStockTransfer,
				EmploymentActionStepType.ReturnAsset, EmploymentActionStepType.StableAnimalOperation,
				EmploymentActionStepType.HotelAdministration }, steps.Select(x => x.StepType).ToArray());
			Assert.AreSame(destination, ((DeliverItemsActionStep)steps[0]).Destination);
			var transfer = (ShopStockTransferActionStep)steps[1];
			Assert.AreSame(destination, transfer.Destination);
			Assert.AreEqual(2201L, transfer.SourceShop.Id);
			Assert.AreEqual(2202L, transfer.TargetShop.Id);
			Assert.AreEqual(2203L, transfer.TargetMerchandise.Id);
			Assert.AreSame(destination, ((ReturnAssetActionStep)steps[2]).Destination);
			Assert.AreSame(hasOptionalTargets ? optionalDestination : null, ((StableAnimalOperationActionStep)steps[3]).Destination);
			Assert.IsTrue(((StableAnimalOperationActionStep)steps[3]).WaiveFees);
			Assert.AreSame(hasOptionalTargets ? hotelRoom.Object : null, ((HotelAdministrationActionStep)steps[4]).Room);
			Assert.AreEqual("historical note", ((HotelAdministrationActionStep)steps[4]).Note);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}
}
