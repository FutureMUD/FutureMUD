#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Work.Foraging;
using System;
using System.Collections.Generic;
using System.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbForagable = MudSharp.Models.Foragable;
using DbForagableProfile = MudSharp.Models.ForagableProfile;
using DbForagableProfilesForagables = MudSharp.Models.ForagableProfilesForagables;
using DbForagableProfilesMaximumYields = MudSharp.Models.ForagableProfilesMaximumYields;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ForagingRuntimeTests
{
	[TestMethod]
	public void Foragable_LoadFromDb_IgnoresBlankAndDuplicateTypes()
	{
		var proto = new Mock<IGameItemProto>();
		var gameworld = CreateGameworld(itemProto: proto.Object);
		var foragable = new Foragable(new DbForagable
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Wild Berries",
			ForagableTypes = "food,, Food, wood ",
			ForageDifficulty = (int)Difficulty.Normal,
			RelativeChance = 100,
			MinimumOutcome = (int)Outcome.MajorFail,
			MaximumOutcome = (int)Outcome.MajorPass,
			QuantityDiceExpression = "1",
			ItemProtoId = 1,
			EditableItem = CreateEditableItem()
		}, gameworld.Object);

		CollectionAssert.AreEqual(new[] { "food", "wood" }, foragable.ForagableTypes.ToArray());
	}

	[TestMethod]
	public void Foragable_LoadFromDb_BlankTypesCannotSubmit()
	{
		var proto = new Mock<IGameItemProto>();
		var gameworld = CreateGameworld(itemProto: proto.Object);
		var foragable = new Foragable(new DbForagable
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Wild Berries",
			ForagableTypes = "",
			ForageDifficulty = (int)Difficulty.Normal,
			RelativeChance = 100,
			MinimumOutcome = (int)Outcome.MajorFail,
			MaximumOutcome = (int)Outcome.MajorPass,
			QuantityDiceExpression = "1",
			ItemProtoId = 1,
			EditableItem = CreateEditableItem()
		}, gameworld.Object);

		Assert.IsFalse(foragable.ForagableTypes.Any());
		Assert.IsFalse(foragable.CanSubmit());
	}

	[TestMethod]
	public void ForagableProfile_GetForageResult_RequiresMatchingProfileYield()
	{
		var foragable = CreateForagable(1, "Loose Stones", "stone", Difficulty.Normal);
		var gameworld = CreateGameworld(foragable: foragable.Object);
		var profile = CreateProfile(gameworld.Object, "wood", 1);
		var outcomes = new Dictionary<Difficulty, CheckOutcome>
		{
			[Difficulty.Normal] = CheckOutcome.SimpleOutcome(CheckType.ForageCheck, Outcome.Pass)
		};

		Assert.IsNull(profile.GetForageResult(Mock.Of<MudSharp.Character.ICharacter>(), outcomes, "stone"));
	}

	[TestMethod]
	public void ForagableProfile_GetForageResult_SkipsForagablesWithoutDifficultyOutcome()
	{
		var foragable = CreateForagable(1, "Wild Berries", "food", Difficulty.Impossible);
		var gameworld = CreateGameworld(foragable: foragable.Object);
		var profile = CreateProfile(gameworld.Object, "food", 1);
		var outcomes = new Dictionary<Difficulty, CheckOutcome>
		{
			[Difficulty.Normal] = CheckOutcome.SimpleOutcome(CheckType.ForageCheck, Outcome.Pass)
		};

		Assert.IsNull(profile.GetForageResult(Mock.Of<MudSharp.Character.ICharacter>(), outcomes, "food"));
		foragable.Verify(x => x.CanForage(It.IsAny<MudSharp.Character.ICharacter>(), It.IsAny<Outcome>()), Times.Never);
	}

	[TestMethod]
	public void ForagableProfile_GetForageResult_ReturnsWeightedMatch()
	{
		var actor = Mock.Of<MudSharp.Character.ICharacter>();
		var foragable = CreateForagable(1, "Wild Berries", "food", Difficulty.Normal);
		foragable.Setup(x => x.CanForage(actor, Outcome.Pass)).Returns(true);
		var gameworld = CreateGameworld(foragable: foragable.Object);
		var profile = CreateProfile(gameworld.Object, "food", 1);
		var outcomes = new Dictionary<Difficulty, CheckOutcome>
		{
			[Difficulty.Normal] = CheckOutcome.SimpleOutcome(CheckType.ForageCheck, Outcome.Pass)
		};

		Assert.AreSame(foragable.Object, profile.GetForageResult(actor, outcomes, "food"));
	}

	private static Mock<IForagable> CreateForagable(long id, string name, string forageType, Difficulty difficulty)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id + 100);
		proto.SetupGet(x => x.RevisionNumber).Returns(0);
		proto.SetupGet(x => x.Name).Returns(name);

		var foragable = new Mock<IForagable>();
		foragable.SetupGet(x => x.Id).Returns(id);
		foragable.SetupGet(x => x.Name).Returns(name);
		foragable.SetupGet(x => x.ItemProto).Returns(proto.Object);
		foragable.SetupGet(x => x.ForagableTypes).Returns(new[] { forageType });
		foragable.SetupGet(x => x.ForageDifficulty).Returns(difficulty);
		foragable.SetupGet(x => x.RelativeChance).Returns(100);
		return foragable;
	}

	private static ForagableProfile CreateProfile(IFuturemud gameworld, string yieldType, long foragableId)
	{
		var profile = new DbForagableProfile
		{
			Id = 10,
			RevisionNumber = 0,
			Name = "Test Profile",
			EditableItem = CreateEditableItem()
		};
		profile.ForagableProfilesForagables.Add(new DbForagableProfilesForagables
		{
			ForagableId = foragableId
		});
		profile.ForagableProfilesMaximumYields.Add(new DbForagableProfilesMaximumYields
		{
			ForageType = yieldType,
			Yield = 10.0
		});
		return new ForagableProfile(profile, gameworld);
	}

	private static Mock<IFuturemud> CreateGameworld(IGameItemProto? itemProto = null, IForagable? foragable = null)
	{
		var progRepo = new Mock<IUneditableAll<IFutureProg>>();
		progRepo.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null!);

		var itemRepo = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		itemRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(itemProto!);

		var foragableRepo = new Mock<IUneditableRevisableAll<IForagable>>();
		foragableRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(foragable!);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemRepo.Object);
		gameworld.SetupGet(x => x.Foragables).Returns(foragableRepo.Object);
		return gameworld;
	}

	private static DbEditableItem CreateEditableItem()
	{
		return new DbEditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = (int)RevisionStatus.Current,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow
		};
	}
}
