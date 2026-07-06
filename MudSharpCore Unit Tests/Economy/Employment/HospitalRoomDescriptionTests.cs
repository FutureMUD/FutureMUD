using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
public class HospitalRoomDescriptionTests
{
	[TestMethod]
	public void HospitalLobbyRoomDescriptionAddenda_AdvertisesServicesOnlyInWaitingRooms()
	{
		var waitingRoom = new Mock<ICell>();
		var theatre = new Mock<ICell>();
		var hospital = new Mock<IHospital>();
		hospital.Setup(x => x.HasLocationRole(waitingRoom.Object, HospitalLocationRole.WaitingRoom))
		        .Returns(true);
		hospital.Setup(x => x.HasLocationRole(theatre.Object, HospitalLocationRole.WaitingRoom))
		        .Returns(false);

		var waitingAddenda = Cell.HospitalLobbyRoomDescriptionAddenda([hospital.Object], waitingRoom.Object)
		                         .Select(x => x.StripANSIColour())
		                         .ToList();
		var theatreAddenda = Cell.HospitalLobbyRoomDescriptionAddenda([hospital.Object], theatre.Object)
		                         .Select(x => x.StripANSIColour())
		                         .ToList();

		Assert.AreEqual(1, waitingAddenda.Count);
		StringAssert.Contains(waitingAddenda[0], "You are in a hospital");
		StringAssert.Contains(waitingAddenda[0], "HOSPITAL SERVICES");
		StringAssert.Contains(waitingAddenda[0], "procure services");
		Assert.AreEqual(0, theatreAddenda.Count);
	}
}
