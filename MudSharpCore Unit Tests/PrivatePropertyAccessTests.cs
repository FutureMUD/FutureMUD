#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PrivatePropertyAccessTests
{
	[TestMethod]
	public void Evaluate_ActiveHostEmployee_IsAuthorised()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = CreateCell(gameworld.Object);
		var actor = CreateCharacter(10L);
		var contract = new Mock<IEmploymentContract>();
		contract.SetupGet(x => x.Employee).Returns(actor.Object);
		contract.SetupGet(x => x.Status).Returns(EmploymentStatus.Active);
		var host = new Mock<IEmploymentHost>();
		host.SetupGet(x => x.Id).Returns(20L);
		host.SetupGet(x => x.Name).Returns("Test Shop");
		host.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		host.SetupGet(x => x.EmploymentContracts).Returns([contract.Object]);
		ApplyEffect(cell, host.Object);

		var result = PrivatePropertyAccessService.Evaluate(cell.Object, actor.Object);

		Assert.IsTrue(result.IsPrivateProperty);
		Assert.IsTrue(result.IsAuthorised);
		Assert.AreEqual(PrivatePropertyAccessReason.Employee, result.Reason);
	}

	[TestMethod]
	public void Evaluate_TrustedAllyOfPropertyOwner_IsAuthorised()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = CreateCell(gameworld.Object);
		var actor = CreateCharacter(10L);
		var owner = CreateCharacter(11L);
		owner.Setup(x => x.IsTrustedAlly(actor.Object)).Returns(true);
		var propertyOwner = new Mock<IPropertyOwner>();
		propertyOwner.SetupGet(x => x.Owner).Returns(owner.Object);
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(30L);
		property.SetupGet(x => x.Name).Returns("Test Property");
		property.SetupGet(x => x.FrameworkItemType).Returns("Property");
		property.SetupGet(x => x.PropertyOwners).Returns([propertyOwner.Object]);
		property.Setup(x => x.IsAuthorisedOwner(actor.Object)).Returns(false);
		property.Setup(x => x.IsAuthorisedLeaseHolder(actor.Object)).Returns(false);
		property.Setup(x => x.HotelRoomForCell(cell.Object)).Returns((IHotelRoom)null!);
		ApplyEffect(cell, property.Object);

		var result = PrivatePropertyAccessService.Evaluate(cell.Object, actor.Object);

		Assert.IsTrue(result.IsAuthorised);
		Assert.AreEqual(PrivatePropertyAccessReason.TrustedAlly, result.Reason);
	}

	[TestMethod]
	public void Evaluate_NoRelationship_WouldTrespass()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = CreateCell(gameworld.Object);
		var actor = CreateCharacter(10L);
		var host = new Mock<IEmploymentHost>();
		host.SetupGet(x => x.Id).Returns(20L);
		host.SetupGet(x => x.Name).Returns("Test Shop");
		host.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		host.SetupGet(x => x.EmploymentContracts).Returns(Array.Empty<IEmploymentContract>());
		ApplyEffect(cell, host.Object);

		var result = PrivatePropertyAccessService.Evaluate(cell.Object, actor.Object);

		Assert.IsTrue(result.IsPrivateProperty);
		Assert.IsFalse(result.IsAuthorised);
		Assert.AreEqual(PrivatePropertyAccessReason.Unauthorised, result.Reason);
	}

	private static Mock<ICell> CreateCell(IFuturemud gameworld)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(1L);
		cell.SetupGet(x => x.Name).Returns("Private Cell");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		return cell;
	}

	private static Mock<ICharacter> CreateCharacter(long id)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(id);
		actor.SetupGet(x => x.Name).Returns($"Character {id}");
		actor.SetupGet(x => x.FrameworkItemType).Returns("Character");
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		actor.Setup(x => x.AffectedBy(It.IsAny<Predicate<PermitWork>>())).Returns(false);
		return actor;
	}

	private static void ApplyEffect(Mock<ICell> cell, IFrameworkItem controller)
	{
		var effect = new PrivatePropertyEffect(cell.Object, controller);
		cell.Setup(x => x.EffectsOfType<PrivatePropertyEffect>(It.IsAny<Predicate<PrivatePropertyEffect>>()))
		    .Returns([effect]);
	}
}
