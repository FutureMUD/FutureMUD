#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.RPG.Law;
using System;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class LegalAuthorityRepeatSuppressionTests
{
	[TestMethod]
	public void ShouldSuppressAutomaticRepeatCrime_ViolentSameVictimDifferentWeaponAndLocation_Suppresses()
	{
		DateTime now = DateTime.UtcNow;
		Mock<ILaw> law = CreateLaw(1L, CrimeTypes.GreviousBodilyHarm, doNotRepeat: false);
		Mock<ICharacter> victim = CreateCharacter(10L);
		Mock<IGameItem> newWeapon = CreateItem(22L, "weapon");
		Mock<ICell> newLocation = CreateCell(32L);
		Mock<ICrime> existingCrime = CreateCrime(law.Object, 10L, now.AddMinutes(-2), 21L, "weapon",
			CreateCell(31L).Object);

		bool result = LegalAuthority.ShouldSuppressAutomaticRepeatCrime(law.Object, [existingCrime.Object],
			victim.Object, newWeapon.Object, newLocation.Object, now);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void ShouldSuppressAutomaticRepeatCrime_NonViolentRepeatDisabled_DoesNotSuppress()
	{
		DateTime now = DateTime.UtcNow;
		Mock<ILaw> law = CreateLaw(1L, CrimeTypes.Theft, doNotRepeat: false);
		Mock<ICharacter> victim = CreateCharacter(10L);
		Mock<IGameItem> item = CreateItem(20L, "GameItem");
		Mock<ICell> location = CreateCell(30L);
		Mock<ICrime> existingCrime = CreateCrime(law.Object, 10L, now.AddMinutes(-2), 20L, "GameItem",
			location.Object);

		bool result = LegalAuthority.ShouldSuppressAutomaticRepeatCrime(law.Object, [existingCrime.Object],
			victim.Object, item.Object, location.Object, now);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void ShouldSuppressAutomaticRepeatCrime_NonViolentRepeatEnabledSameItemAndLocation_Suppresses()
	{
		DateTime now = DateTime.UtcNow;
		Mock<ILaw> law = CreateLaw(1L, CrimeTypes.Theft, doNotRepeat: true);
		Mock<ICharacter> victim = CreateCharacter(10L);
		Mock<IGameItem> item = CreateItem(20L, "GameItem");
		Mock<ICell> existingLocation = CreateCell(30L);
		Mock<ICell> newLocation = CreateCell(30L);
		Mock<ICrime> existingCrime = CreateCrime(law.Object, 10L, now.AddMinutes(-2), 20L, "GameItem",
			existingLocation.Object);

		bool result = LegalAuthority.ShouldSuppressAutomaticRepeatCrime(law.Object, [existingCrime.Object],
			victim.Object, item.Object, newLocation.Object, now);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void ShouldSuppressAutomaticRepeatCrime_OlderViolentCrime_DoesNotSuppress()
	{
		DateTime now = DateTime.UtcNow;
		Mock<ILaw> law = CreateLaw(1L, CrimeTypes.Assault, doNotRepeat: false);
		Mock<ICharacter> victim = CreateCharacter(10L);
		Mock<ICrime> existingCrime = CreateCrime(law.Object, 10L, now.AddMinutes(-11), null, null,
			CreateCell(30L).Object);

		bool result = LegalAuthority.ShouldSuppressAutomaticRepeatCrime(law.Object, [existingCrime.Object],
			victim.Object, null!, CreateCell(31L).Object, now);

		Assert.IsFalse(result);
	}

	private static Mock<ILaw> CreateLaw(long id, CrimeTypes crimeType, bool doNotRepeat)
	{
		Mock<ILaw> law = new();
		law.SetupGet(x => x.Id).Returns(id);
		law.SetupGet(x => x.CrimeType).Returns(crimeType);
		law.SetupGet(x => x.DoNotAutomaticallyApplyRepeats).Returns(doNotRepeat);
		return law;
	}

	private static Mock<ICrime> CreateCrime(ILaw law, long? victimId, DateTime realTimeOfCrime, long? itemId,
		string? itemType, ICell location)
	{
		Mock<ICrime> crime = new();
		crime.SetupGet(x => x.Law).Returns(law);
		crime.SetupGet(x => x.VictimId).Returns(victimId);
		crime.SetupGet(x => x.RealTimeOfCrime).Returns(realTimeOfCrime);
		crime.SetupGet(x => x.ThirdPartyId).Returns(itemId);
		crime.SetupGet(x => x.ThirdPartyFrameworkItemType).Returns(itemType);
		crime.SetupGet(x => x.CrimeLocation).Returns(location);
		return crime;
	}

	private static Mock<ICharacter> CreateCharacter(long id)
	{
		Mock<ICharacter> character = new();
		character.SetupGet(x => x.Id).Returns(id);
		return character;
	}

	private static Mock<IGameItem> CreateItem(long id, string frameworkItemType)
	{
		Mock<IGameItem> item = new();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.FrameworkItemType).Returns(frameworkItemType);
		return item;
	}

	private static Mock<ICell> CreateCell(long id)
	{
		Mock<ICell> cell = new();
		cell.SetupGet(x => x.Id).Returns(id);
		return cell;
	}
}
