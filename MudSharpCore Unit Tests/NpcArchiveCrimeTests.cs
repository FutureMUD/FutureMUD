#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveCrimeTests
{
	[TestMethod]
	public void CrimeHistory_ArchivedVictim_UsesWorldAttributionWithoutMaterializingActor()
	{
		var world = new Mock<IFuturemud>(); var cell = new Mock<ICell>(); var overlay = new Mock<ICellOverlay>();
		cell.SetupGet(x => x.Id).Returns(1); overlay.SetupGet(x => x.CellName).Returns("Fixture Court");
		cell.SetupGet(x => x.CurrentOverlay).Returns(overlay.Object);
		var cells = new All<ICell>(); cells.Add(cell.Object); world.SetupGet(x => x.Cells).Returns(cells);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var archives = new Mock<ICharacterArchiveService>();
		archives.Setup(x => x.Find(40)).Returns(new ArchivedCharacterIdentity(40, 209, Guid.Empty,
			new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc), "Historical Guardian", "an archived guardian", "Historical description."));
		world.SetupGet(x => x.CharacterArchives).Returns(archives.Object);
		var law = new Mock<ILaw>(); law.SetupGet(x => x.Gameworld).Returns(world.Object);
		law.SetupGet(x => x.CrimeType).Returns(CrimeTypes.GreviousBodilyHarm);
		var crime = new Crime(new Db.Crime
		{
			Id = 20, CriminalId = 30, VictimId = 40, LocationId = 1, TimeOfCrime = MudDateTime.Never.GetDateTimeString(),
			RealTimeOfCrime = DateTime.UtcNow, CriminalShortDescription = "a defendant", CriminalFullDescription = "A defendant.",
			CriminalCharacteristics = "", WitnessIds = ""
		}, law.Object, world.Object);
		var viewer = Mock.Of<IPerceiver>();
		StringAssert.Contains(crime.DescribeCrimeAtTrial(viewer), "Historical Guardian");
		StringAssert.Contains(crime.DescribeCrime(viewer), "an archived guardian");
		archives.Verify(x => x.Find(40), Times.Exactly(2));
		world.Verify(x => x.TryGetCharacter(40, true), Times.Exactly(2));
	}
}
