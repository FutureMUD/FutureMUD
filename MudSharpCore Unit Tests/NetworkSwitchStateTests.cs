#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Computers;
using MudSharp.Construction.Grids;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NetworkSwitchStateTests
{
	[DataTestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	public void StateQueries_UnanchoredUplinkCycle_ReturnNullAndOfflineRepeatedly(int length)
	{
		var switches = CreateCycle(length);
		foreach (var item in switches)
		{
			for (var attempt = 0; attempt < 3; attempt++)
			{
				Assert.IsNull(item.TelecommunicationsGrid);
				Assert.IsFalse(item.NetworkTransportReady);
			}
		}
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	public void StateQueries_CycleWithDirectGrid_PreservesFallbackButRemainsOffline(int length)
	{
		var switches = CreateCycle(length);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		switches[0].TelecommunicationsGrid = grid;
		foreach (var item in switches)
		{
			for (var attempt = 0; attempt < 3; attempt++)
			{
				Assert.AreSame(grid, item.TelecommunicationsGrid);
				Assert.IsFalse(item.NetworkTransportReady);
			}
		}
	}

	[TestMethod]
	public void StateQueries_CycleWithCompetingGrids_PreservesLocalFallbackWithoutInventingTransport()
	{
		var switches = CreateCycle(2);
		var firstGrid = Mock.Of<ITelecommunicationsGrid>();
		var secondGrid = Mock.Of<ITelecommunicationsGrid>();
		switches[0].TelecommunicationsGrid = firstGrid;
		switches[1].TelecommunicationsGrid = secondGrid;
		for (var attempt = 0; attempt < 3; attempt++)
		{
			Assert.AreSame(secondGrid, switches[0].TelecommunicationsGrid);
			Assert.AreSame(firstGrid, switches[1].TelecommunicationsGrid);
			Assert.IsFalse(switches[0].NetworkTransportReady);
			Assert.IsFalse(switches[1].NetworkTransportReady);
		}
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(5)]
	[DataRow(32)]
	public void StateQueries_AcyclicPoweredChain_ResolvesGridAndTransportRepeatedly(int length)
	{
		var switches = Enumerable.Range(1, length).Select(x => CreateSwitch(x)).ToArray();
		var grid = Mock.Of<ITelecommunicationsGrid>();
		switches[^1].TelecommunicationsGrid = grid;
		for (var i = 0; i < length - 1; i++)
		{
			switches[i].RawConnect(switches[i + 1], ComputerConnectionTypes.NetworkUplinkPlug);
		}
		for (var attempt = 0; attempt < 3; attempt++)
		{
			Assert.AreSame(grid, switches[0].TelecommunicationsGrid);
			Assert.IsTrue(switches[0].NetworkTransportReady);
		}
	}

	[DataTestMethod]
	[DataRow(true, true)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void StateQueries_AcyclicUpstreamWithDirectAttachments_PreservesPrecedenceAndPower(bool powered, bool on)
	{
		var downstream = CreateSwitch(1);
		var upstream = CreateSwitch(2, powered, on);
		var upstreamGrid = Mock.Of<ITelecommunicationsGrid>();
		var downstreamGrid = Mock.Of<ITelecommunicationsGrid>();
		downstream.TelecommunicationsGrid = downstreamGrid;
		upstream.TelecommunicationsGrid = upstreamGrid;
		downstream.RawConnect(upstream, ComputerConnectionTypes.NetworkUplinkPlug);

		Assert.AreSame(upstreamGrid, downstream.TelecommunicationsGrid);
		Assert.AreEqual(powered && on, downstream.NetworkTransportReady);
	}

	[TestMethod]
	public void StateQueries_UpstreamWithoutGrid_PreservesDirectFallback()
	{
		var item = CreateSwitch(1);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		item.TelecommunicationsGrid = grid;
		SetUplink(item, Mock.Of<INetworkInfrastructure>(x => x.NetworkTransportReady));

		Assert.AreSame(grid, item.TelecommunicationsGrid);
		Assert.IsTrue(item.NetworkTransportReady);
	}

	[TestMethod]
	public void StateQueries_UnavailableUpstream_DoesNotUseDirectAttachmentToBypassTransport()
	{
		var item = CreateSwitch(1);
		var localGrid = Mock.Of<ITelecommunicationsGrid>();
		var upstreamGrid = Mock.Of<ITelecommunicationsGrid>();
		item.TelecommunicationsGrid = localGrid;
		SetUplink(item, Mock.Of<INetworkInfrastructure>(x => x.TelecommunicationsGrid == upstreamGrid && !x.NetworkTransportReady));

		Assert.AreSame(upstreamGrid, item.TelecommunicationsGrid);
		Assert.IsFalse(item.NetworkTransportReady);
	}

	[TestMethod]
	public void StateQueries_DisconnectedSwitch_DoesNotReportTransportAndCanRecover()
	{
		var item = CreateSwitch(1);
		Assert.IsNull(item.TelecommunicationsGrid);
		Assert.IsFalse(item.NetworkTransportReady);
		Assert.IsFalse(CreateSwitch(2).NetworkTransportReady);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		item.TelecommunicationsGrid = grid;
		Assert.AreSame(grid, item.TelecommunicationsGrid);
		Assert.IsTrue(item.NetworkTransportReady);
		item.TelecommunicationsGrid = null;
		Assert.IsNull(item.TelecommunicationsGrid);
		Assert.IsFalse(item.NetworkTransportReady);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void StateQueries_FailingUpstreamQuery_ClearsBothGuardsBeforeRetry(bool transportQuery)
	{
		var first = CreateSwitch(1);
		var second = CreateSwitch(2);
		first.RawConnect(second, ComputerConnectionTypes.NetworkUplinkPlug);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		var endpoint = new Mock<INetworkInfrastructure>();
		endpoint.SetupGet(x => x.TelecommunicationsGrid).Returns(grid);
		endpoint.SetupGet(x => x.NetworkTransportReady).Returns(true);
		var interrupted = false;
		if (transportQuery)
		{
			endpoint.SetupGet(x => x.NetworkTransportReady).Returns(() =>
			{
				if (!interrupted)
				{
					interrupted = true;
					throw new InvalidOperationException("interrupted transport query");
				}
				return true;
			});
		}
		else
		{
			endpoint.SetupGet(x => x.TelecommunicationsGrid).Returns(() =>
			{
				if (!interrupted)
				{
					interrupted = true;
					throw new InvalidOperationException("interrupted grid query");
				}
				return grid;
			});
		}
		SetUplink(second, endpoint.Object);

		Assert.ThrowsException<InvalidOperationException>(() => _ = first.NetworkTransportReady);
		Assert.AreSame(grid, first.TelecommunicationsGrid);
		Assert.IsTrue(first.NetworkTransportReady);
		Assert.IsTrue(first.NetworkTransportReady);
	}

	[TestMethod]
	public void StateQueries_BrokenCycleCanBecomeValid_ReevaluatesCurrentTopology()
	{
		var switches = CreateCycle(2);
		Assert.IsNull(switches[0].TelecommunicationsGrid);
		Assert.IsFalse(switches[0].NetworkTransportReady);
		switches[1].RawDisconnect(switches[0], false);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		switches[1].TelecommunicationsGrid = grid;
		Assert.AreSame(grid, switches[0].TelecommunicationsGrid);
		Assert.IsTrue(switches[0].NetworkTransportReady);
	}

	[TestMethod]
	public void TelecommunicationsGrid_ConcurrentIndependentQueries_DoNotTreatOtherThreadAsCycle()
	{
		var item = CreateSwitch(1);
		var grid = Mock.Of<ITelecommunicationsGrid>();
		using var entered = new ManualResetEventSlim();
		using var release = new ManualResetEventSlim();
		var calls = 0;
		var endpoint = new Mock<INetworkInfrastructure>();
		endpoint.SetupGet(x => x.TelecommunicationsGrid).Returns(() =>
		{
			if (Interlocked.Increment(ref calls) == 1)
			{
				entered.Set();
				if (!release.Wait(TimeSpan.FromSeconds(5)))
				{
					throw new TimeoutException("Owned query was not released");
				}
			}
			return grid;
		});
		SetUplink(item, endpoint.Object);
		var query = Task.Run(() => item.TelecommunicationsGrid);
		try
		{
			Assert.IsTrue(entered.Wait(TimeSpan.FromSeconds(5)));
			Assert.AreSame(grid, item.TelecommunicationsGrid);
		}
		finally
		{
			release.Set();
			Assert.IsTrue(query.Wait(TimeSpan.FromSeconds(5)));
		}
		Assert.AreSame(grid, query.Result);
	}

	private static NetworkSwitchGameItemComponent[] CreateCycle(int length)
	{
		var switches = Enumerable.Range(1, length).Select(x => CreateSwitch(x)).ToArray();
		for (var i = 0; i < length; i++)
		{
			switches[i].RawConnect(switches[(i + 1) % length], ComputerConnectionTypes.NetworkUplinkPlug);
		}
		return switches;
	}

	private static NetworkSwitchGameItemComponent CreateSwitch(long id, bool powered = true, bool on = true)
	{
		var prototype = TestObjectFactory.CreateUninitialized<NetworkSwitchGameItemComponentProto>();
		var parent = Mock.Of<IGameItem>(x => x.Id == id && x.Gameworld == Mock.Of<IFuturemud>());
		var item = new NetworkSwitchGameItemComponent(prototype, parent, true);
		typeof(PoweredMachineBaseGameItemComponent).GetField("_onAndPowered", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, powered);
		typeof(PoweredMachineBaseGameItemComponent).GetField("_switchedOn", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, on);
		return item;
	}

	private static void SetUplink(NetworkSwitchGameItemComponent item, INetworkInfrastructure infrastructure)
	{
		typeof(NetworkSwitchGameItemComponent).GetField("_connectedInfrastructure", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, infrastructure);
	}
}
