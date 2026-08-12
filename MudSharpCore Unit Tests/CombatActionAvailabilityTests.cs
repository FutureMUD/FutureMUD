#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatActionAvailabilityTests
{
	[TestMethod]
	public void GetCouchedLance_MountedWithoutCouchableWeapon_ReturnsNull()
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.WieldedItems).Returns([]);
		var assailant = new Mock<ICharacter>();
		assailant.SetupGet(x => x.RidingMount).Returns(Mock.Of<ICharacter>());
		assailant.SetupGet(x => x.Body).Returns(body.Object);
		var move = new ChargeToMeleeMove { Assailant = assailant.Object };
		var method = typeof(ChargeToMeleeMove).GetMethod("GetCouchedLance",
			BindingFlags.Instance | BindingFlags.NonPublic)!;

		var result = method.Invoke(move, [Mock.Of<ICharacter>()]);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void CanFire_MatchlockWithoutCurrentWeather_ReturnsTrue()
	{
		var prototype = (MusketGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(
			typeof(MusketGameItemComponentProto));
		typeof(MusketGameItemComponentProto)
			.GetProperty(nameof(MusketGameItemComponentProto.IgnitionFamily))!
			.SetValue(prototype, MusketIgnitionFamily.Matchlock);
		var parent = new Mock<IGameItem>();
		var musket = (MusketGameItemComponent)prototype.CreateNew(parent.Object, temporary: true);
		musket.LoadStage = 4;
		musket.IsReadied = true;
		typeof(MusketGameItemComponent)
			.GetProperty(nameof(MusketGameItemComponent.MatchLit))!
			.SetValue(musket, true);
		var location = new Mock<ICell>();
		location.Setup(x => x.CurrentWeather(It.IsAny<ICharacter>())).Returns((IWeatherEvent)null!);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(location.Object);

		Assert.AreEqual(MusketIgnitionFamily.Matchlock, prototype.IgnitionFamily);
		Assert.IsTrue(musket.ReadyToFire);
		Assert.IsTrue(musket.MatchLit);
		Assert.IsTrue(musket.CanFire(actor.Object, Mock.Of<IPerceivable>()));
	}
}
