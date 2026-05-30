#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicPortalTopologyTests
{
	[TestMethod]
	public void RebuildNetwork_ValidLink_MaterializesTopologyPortal()
	{
		var (gameworld, manager, source, destination, network) = BuildTopology();
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);

		new MagicPortalTopologyService().RebuildNetwork(network);

		var portal = manager.TransientExits.OfType<IMagicPortalTopologyExit>().Single();
		Assert.AreSame(network, portal.Network);
		Assert.AreSame(source.Object, portal.Source);
		Assert.AreSame(destination.Object, portal.Destination);
		Assert.AreEqual("enter", portal.Verb);
		Assert.AreEqual("north", portal.SourceEndpoint.Key);
		Assert.AreEqual("south", portal.DestinationEndpoint.Key);
	}

	[TestMethod]
	public void RebuildNetwork_RepeatedRebuild_DoesNotDuplicateAndPreservesLegacyPortal()
	{
		var (gameworld, manager, source, destination, network) = BuildTopology();
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var legacy = new TransientExit(gameworld.Object, source.Object, destination.Object, "enter", "gate", "gate",
			"a gate", "a gate", "through", "through", 1.0);
		manager.RegisterTransientExit(legacy);

		new MagicPortalTopologyService().RebuildNetwork(network);
		new MagicPortalTopologyService().RebuildNetwork(network);

		Assert.AreSame(legacy, manager.TransientExits.First(x => ReferenceEquals(x, legacy)));
		Assert.AreEqual(1, manager.TransientExits.OfType<IMagicPortalTopologyExit>().Count());
		Assert.AreEqual(2, manager.TransientExits.Count());
	}

	[TestMethod]
	public void RebuildNetwork_ItemEndpointWithoutDirectLocation_DoesNotMaterialize()
	{
		var zone = new Mock<IZone>().Object;
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var source = CreateCell(1, "Source", gameworld.Object, zone);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(99);
		item.SetupGet(x => x.Name).Returns("Rune Stone");
		item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		item.SetupGet(x => x.Location).Returns((ICell)null!);
		gameworld.Setup(x => x.TryGetItem(99, true)).Returns(item.Object);
		var cells = new All<ICell>();
		cells.Add(source.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);

		var network = BuildNetwork(gameworld.Object, source.Object, itemId: 99);
		var networks = new All<IMagicPortalNetwork>();
		networks.Add(network);
		gameworld.SetupGet(x => x.MagicPortalNetworks).Returns(networks);

		new MagicPortalTopologyService().RebuildNetwork(network);

		Assert.IsFalse(manager.TransientExits.Any());
		StringAssert.Contains(network.Links.Single().WhyInvalid, "not directly located");
	}

	[TestMethod]
	public void RebuildNetwork_ItemEndpointWithEffectiveButNotDirectLocation_DoesNotMaterialize()
	{
		var zone = new Mock<IZone>().Object;
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var source = CreateCell(1, "Source", gameworld.Object, zone);
		var destination = CreateCell(2, "Destination", gameworld.Object, zone);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(99);
		item.SetupGet(x => x.Name).Returns("Rune Stone");
		item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		item.SetupGet(x => x.Location).Returns(destination.Object);
		item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		item.SetupGet(x => x.InInventoryOf).Returns(new Mock<IBody>().Object);
		gameworld.Setup(x => x.TryGetItem(99, true)).Returns(item.Object);
		var cells = new All<ICell>();
		cells.Add(source.Object);
		cells.Add(destination.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);

		var network = BuildNetwork(gameworld.Object, source.Object, itemId: 99);
		var networks = new All<IMagicPortalNetwork>();
		networks.Add(network);
		gameworld.SetupGet(x => x.MagicPortalNetworks).Returns(networks);

		new MagicPortalTopologyService().RebuildNetwork(network);

		Assert.IsFalse(manager.TransientExits.Any());
		StringAssert.Contains(network.Links.Single().WhyInvalid, "not directly located");
	}

	[TestMethod]
	public void RebuildNetworksForItem_ItemEndpointMovedOut_RemovesMaterializedTopologyPortal()
	{
		var zone = new Mock<IZone>().Object;
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var source = CreateCell(1, "Source", gameworld.Object, zone);
		var destination = CreateCell(2, "Destination", gameworld.Object, zone);
		var destinationItems = new List<IGameItem>();
		destination.SetupGet(x => x.GameItems).Returns(destinationItems);
		ICell? itemLocation = destination.Object;
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(99);
		item.SetupGet(x => x.Name).Returns("Rune Stone");
		item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		item.SetupGet(x => x.Location).Returns(() => itemLocation);
		item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		destinationItems.Add(item.Object);
		gameworld.Setup(x => x.TryGetItem(99, true)).Returns(item.Object);
		var cells = new All<ICell>();
		cells.Add(source.Object);
		cells.Add(destination.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var network = BuildNetwork(gameworld.Object, source.Object, itemId: 99);
		var networks = new All<IMagicPortalNetwork>();
		networks.Add(network);
		gameworld.SetupGet(x => x.MagicPortalNetworks).Returns(networks);
		var service = new MagicPortalTopologyService();
		service.RebuildNetwork(network);
		Assert.AreEqual(1, manager.TransientExits.OfType<IMagicPortalTopologyExit>().Count());

		destinationItems.Clear();
		itemLocation = null;
		service.RebuildNetworksForItem(gameworld.Object, item.Object);

		Assert.IsFalse(manager.TransientExits.OfType<IMagicPortalTopologyExit>().Any());
	}

	[TestMethod]
	public void PortalTopologySpellEffect_PermanentMissingLink_DoesNotMoveExistingEndpoint()
	{
		var state = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var zone = new Mock<IZone>().Object;
			var gameworld = CreateGameworld();
			var manager = new ExitManager(gameworld.Object);
			gameworld.SetupGet(x => x.ExitManager).Returns(manager);
			var source = CreateCell(1, "Source", gameworld.Object, zone);
			var destination = CreateCell(2, "Destination", gameworld.Object, zone);
			var cells = new All<ICell>();
			cells.Add(source.Object);
			cells.Add(destination.Object);
			gameworld.SetupGet(x => x.Cells).Returns(cells);
			var persistedEndpoint = new DB.MagicPortalEndpoint
			{
				Id = 11,
				MagicPortalNetworkId = 10,
				Key = "north",
				Name = "North",
				AnchorType = (int)MagicPortalEndpointType.Cell,
				CellId = source.Object.Id,
				IsActive = true,
				CreatedDateTime = DateTime.UtcNow
			};
			context.MagicPortalEndpoints.Add(persistedEndpoint);
			context.SaveChanges();
			var network = new MagicPortalNetwork(new DB.MagicPortalNetwork
			{
				Id = 10,
				Name = "Test Network",
				IsActive = true,
				AllowCrossZone = false,
				Verb = "enter",
				OutboundKeyword = "portal",
				InboundKeyword = "portal",
				OutboundTarget = "a standing portal",
				InboundTarget = "a standing portal",
				OutboundDescription = "through",
				InboundDescription = "through",
				TimeMultiplier = 1.0,
				CreatedDateTime = DateTime.UtcNow,
				MagicPortalEndpoints =
				[
					new DB.MagicPortalEndpoint
					{
						Id = persistedEndpoint.Id,
						MagicPortalNetworkId = persistedEndpoint.MagicPortalNetworkId,
						Key = persistedEndpoint.Key,
						Name = persistedEndpoint.Name,
						AnchorType = persistedEndpoint.AnchorType,
						CellId = persistedEndpoint.CellId,
						IsActive = persistedEndpoint.IsActive,
						CreatedDateTime = persistedEndpoint.CreatedDateTime
					}
				],
				MagicPortalLinks = []
			}, gameworld.Object);
			var networks = new All<IMagicPortalNetwork>();
			networks.Add(network);
			gameworld.SetupGet(x => x.MagicPortalNetworks).Returns(networks);
			var spell = new Mock<IMagicSpell>();
			spell.SetupGet(x => x.Id).Returns(50);
			spell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "portalnetwork"),
				new XElement("NetworkId", network.Id),
				new XElement("EndpointKey", new XCData("north")),
				new XElement("AnchorMode", (int)MagicPortalTopologyAnchorMode.TargetRoom),
				new XElement("LinkEndpointKey", new XCData("missing")),
				new XElement("ReplaceExisting", true),
				new XElement("Permanent", true)), spell.Object);
			var caster = new Mock<ICharacter>();
			caster.SetupGet(x => x.Id).Returns(100);
			caster.SetupGet(x => x.Location).Returns(source.Object);
			caster.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

			var result = effect.GetOrApplyEffect(caster.Object, destination.Object, OpposedOutcomeDegree.None,
				SpellPower.Insignificant, new Mock<IMagicSpellEffectParent>().Object, []);

			Assert.IsNull(result);
			Assert.AreEqual(source.Object.Id, context.MagicPortalEndpoints.Single().CellId);
			Assert.AreEqual(source.Object.Id, network.Endpoints.Single().CellId);
		}
		finally
		{
			RestoreFMDBState(state);
		}
	}

	private static (Mock<IFuturemud> Gameworld, ExitManager Manager, Mock<ICell> Source, Mock<ICell> Destination,
		MagicPortalNetwork Network) BuildTopology()
	{
		var zone = new Mock<IZone>().Object;
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		var source = CreateCell(1, "Source", gameworld.Object, zone);
		var destination = CreateCell(2, "Destination", gameworld.Object, zone);
		var cells = new All<ICell>();
		cells.Add(source.Object);
		cells.Add(destination.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var network = BuildNetwork(gameworld.Object, source.Object, destination.Object);
		var networks = new All<IMagicPortalNetwork>();
		networks.Add(network);
		gameworld.SetupGet(x => x.MagicPortalNetworks).Returns(networks);
		return (gameworld, manager, source, destination, network);
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var plane = new Mock<IPlane>();
		plane.SetupGet(x => x.Id).Returns(1);
		plane.SetupGet(x => x.Name).Returns("Prime");
		plane.SetupGet(x => x.DisplayOrder).Returns(0);
		var planes = new All<IPlane>();
		planes.Add(plane.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.DefaultPlane).Returns(plane.Object);
		gameworld.SetupGet(x => x.Planes).Returns(planes);
		gameworld.SetupGet(x => x.MagicSchools).Returns(new All<IMagicSchool>());
		gameworld.SetupGet(x => x.Items).Returns(new All<IGameItem>());
		return gameworld;
	}

	private static Mock<ICell> CreateCell(long id, string name, IFuturemud gameworld, IZone zone)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		cell.SetupGet(x => x.Zone).Returns(zone);
		var gameItems = new List<IGameItem>();
		cell.SetupGet(x => x.GameItems).Returns(gameItems);
		cell.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>()))
		    .Returns([]);
		return cell;
	}

	private static MagicPortalNetwork BuildNetwork(IFuturemud gameworld, ICell source, ICell? destination = null,
		long? itemId = null)
	{
		var dbNetwork = new DB.MagicPortalNetwork
		{
			Id = 10,
			Name = "Test Network",
			IsActive = true,
			AllowCrossZone = false,
			Verb = "enter",
			OutboundKeyword = "portal",
			InboundKeyword = "portal",
			OutboundTarget = "a standing portal",
			InboundTarget = "a standing portal",
			OutboundDescription = "through",
			InboundDescription = "through",
			TimeMultiplier = 1.0,
			CreatedDateTime = DateTime.UtcNow,
			MagicPortalEndpoints =
			[
				new DB.MagicPortalEndpoint
				{
					Id = 11,
					MagicPortalNetworkId = 10,
					Key = "north",
					Name = "North",
					AnchorType = (int)MagicPortalEndpointType.Cell,
					CellId = source.Id,
					IsActive = true,
					CreatedDateTime = DateTime.UtcNow
				},
				new DB.MagicPortalEndpoint
				{
					Id = 12,
					MagicPortalNetworkId = 10,
					Key = "south",
					Name = "South",
					AnchorType = itemId.HasValue ? (int)MagicPortalEndpointType.Item : (int)MagicPortalEndpointType.Cell,
					CellId = destination?.Id,
					GameItemId = itemId,
					IsActive = true,
					CreatedDateTime = DateTime.UtcNow
				}
			],
			MagicPortalLinks =
			[
				new DB.MagicPortalLink
				{
					Id = 20,
					MagicPortalNetworkId = 10,
					SourceEndpointId = 11,
					DestinationEndpointId = 12,
					IsActive = true,
					CreatedDateTime = DateTime.UtcNow
				}
			]
		};

		return new MagicPortalNetwork(dbNetwork, gameworld);
	}

	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;

		return new FuturemudDatabaseContext(options);
	}

	private static FMDBState CaptureFMDBState()
	{
		return new FMDBState(
			(FuturemudDatabaseContext?)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
				.GetValue(null),
			typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
			(uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
				.GetValue(null)!);
	}

	private static void PrimeFMDB(FuturemudDatabaseContext context)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
	}

	private static void RestoreFMDBState(FMDBState state)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
			.SetValue(null, state.InstanceCount);
	}
}
