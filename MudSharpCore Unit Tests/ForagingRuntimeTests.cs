#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.Work.Foraging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DbCell = MudSharp.Models.Cell;
using DbCellsForagableYield = MudSharp.Models.CellsForagableYield;
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
	public void Zone_PendingForagableProfile_ResolvesAfterProfilesLoad()
	{
		const long profileId = 75L;
		var profiles = new RevisableAll<IForagableProfile>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		var zone = TestObjectFactory.CreateUninitialized<Zone>();
		SetLateInitialisingGameworld(zone, gameworld.Object);
		typeof(Zone).GetField("_foragableProfileId", BindingFlags.Instance | BindingFlags.NonPublic)!
		            .SetValue(zone, profileId);

		Assert.IsNull(zone.ForagableProfile);

		var profile = CreateProfileMock(profileId, ("food", 1.0));
		profiles.Add(profile.Object);

		Assert.AreSame(profile.Object, zone.ForagableProfile);
	}

	[TestMethod]
	public void Terrain_PendingForagableProfile_ResolvesAfterProfilesLoad()
	{
		const long profileId = 76L;
		var profiles = new RevisableAll<IForagableProfile>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		var terrain = TestObjectFactory.CreateUninitialized<Terrain>();
		typeof(SaveableItem).GetField("_gameworld", BindingFlags.Instance | BindingFlags.NonPublic)!
		                    .SetValue(terrain, gameworld.Object);
		typeof(Terrain).GetField("_foragableProfileId", BindingFlags.Instance | BindingFlags.NonPublic)!
		               .SetValue(terrain, profileId);

		Assert.IsNull(terrain.ForagableProfile);

		var profile = CreateProfileMock(profileId, ("food", 1.0));
		profiles.Add(profile.Object);

		Assert.AreSame(profile.Object, terrain.ForagableProfile);
	}

	[TestMethod]
	public void Cell_FractionalRecoveryPersistsAndBecomesDiscreteYieldOnTick48()
	{
		const long profileId = 77L;
		var profiles = new RevisableAll<IForagableProfile>();
		var profile = CreateRecoveringProfileMock(profileId, "junk", 3.0, 1.0 / 48.0);
		profiles.Add(profile.Object);
		var heartbeat = new Mock<IHeartbeatManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var cell = CreateLoadedCell(gameworld.Object, profileId, "junk", 0.0);

		RunYieldTicks(cell, 24);
		var persistedYield = cell.GetForagableYield("junk");
		var reloadedCell = CreateLoadedCell(gameworld.Object, profileId, "junk", persistedYield);
		RunYieldTicks(reloadedCell, 23);

		Assert.IsFalse(reloadedCell.CanConsumeYield("junk", 1.0));
		RunYieldTicks(reloadedCell, 1);
		Assert.IsTrue(reloadedCell.CanConsumeYield("junk", 1.0));
		Assert.IsTrue(reloadedCell.TryConsumeYield("junk", 1.0));
		Assert.AreEqual(0.0, reloadedCell.GetForagableYield("junk"), 1.0e-12);
		Assert.IsFalse(reloadedCell.TryConsumeYield("junk", 1.0));
	}

	[DataTestMethod]
	[DataRow("item")]
	[DataRow("commodity")]
	public void Cell_DiscreteItemAndCommodityRequireAWholeYield(string yieldType)
	{
		var profile = CreateProfileMock(78L, (yieldType, 2.0));
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var cell = CreateLoadedCell(gameworld.Object, 78L, yieldType, 0.999);

		Assert.IsFalse(cell.CanConsumeYield(yieldType, 1.0));
		Assert.IsFalse(cell.TryConsumeYield(yieldType, 1.0));
	}

	[TestMethod]
	public void Cell_PersistedYields_AreClampedToTheActiveProfile()
	{
		var profile = CreateProfileMock(80L, ("food", 1.0));
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());

		var overfullCell = CreateLoadedCell(gameworld.Object, 80L, "food", 1.5);
		var negativeCell = CreateLoadedCell(gameworld.Object, 80L, "food", -0.5);

		Assert.AreEqual(1.0, overfullCell.GetForagableYield("food"));
		Assert.AreEqual(0.0, negativeCell.GetForagableYield("food"));
	}

	[TestMethod]
	public void Cell_NonFiniteYields_CannotBeConsumedOrPersisted()
	{
		var profile = CreateProfileMock(81L, ("food", 1.0));
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var cell = CreateLoadedCell(gameworld.Object, 81L, "food", 1.0);
		var persistedNonFiniteCell = CreateLoadedCell(gameworld.Object, 81L, "food", double.NaN);
		var yields = (Dictionary<string, double>)typeof(Cell)
			.GetField("_foragableYields", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(cell)!;
		yields["food"] = double.NaN;

		Assert.IsFalse(cell.CanConsumeYield("food", 1.0));
		Assert.IsFalse(cell.TryConsumeYield("food", 1.0));
		Assert.IsFalse(cell.TryConsumeYield("food", double.NaN));
		cell.ConsumeYield("food", double.NaN);
		Assert.AreEqual(0.0, cell.GetForagableYield("food"));
		Assert.AreEqual(0.0, persistedNonFiniteCell.GetForagableYield("food"));
	}

	[TestMethod]
	public void Cell_NonFiniteHourlyRecovery_DoesNotPoisonYieldPool()
	{
		var profile = CreateRecoveringProfileMock(83L, "food", 1.0, double.NaN);
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var cell = CreateLoadedCell(gameworld.Object, 83L, "food", 0.5);

		RunYieldTicks(cell, 1);

		Assert.AreEqual(0.5, cell.GetForagableYield("food"));
		Assert.IsFalse(cell.TryConsumeYield("food", 1.0));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	[DoNotParallelize]
	public void Forage_DiscreteItemAndCommodityOutput_RejectsFractionalYield(bool commodityOutput)
	{
		using var forageTime = new ForageTimeExpressionScope();
		const long profileId = 82L;
		var profile = CreateProfileMock(profileId, ("food", 2.0));
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var check = new Mock<ICheck>();
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			     It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable?>(), It.IsAny<double>(),
			     It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()))
		     .Returns(Enum.GetValues<Difficulty>()
		                  .ToDictionary(x => x, x => CheckOutcome.SimpleOutcome(CheckType.ForageCheck, Outcome.MajorPass)));
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		gameworld.Setup(x => x.GetCheck(CheckType.ForageCheck)).Returns(check.Object);
		var cell = CreateLoadedCell(gameworld.Object, profileId, "food", 0.999);
		var foragable = new Mock<IForagable>();
		foragable.SetupGet(x => x.ForagableTypes).Returns(["food"]);
		foragable.SetupGet(x => x.ForageDifficulty).Returns(Difficulty.Normal);
		if (commodityOutput)
		{
			foragable.SetupGet(x => x.CommodityMaterial).Returns(Mock.Of<ISolid>());
			foragable.SetupGet(x => x.CommodityWeightExpression).Returns("1");
		}
		else
		{
			foragable.SetupGet(x => x.ItemProto).Returns(Mock.Of<IGameItemProto>());
		}

		profile.SetupGet(x => x.Foragables).Returns([foragable.Object]);
		profile.Setup(x => x.GetForageResult(It.IsAny<MudSharp.Character.ICharacter>(),
			              It.IsAny<IReadOnlyDictionary<Difficulty, CheckOutcome>>(), "food"))
		       .Returns(foragable.Object);
		var character = new Mock<MudSharp.Character.ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		character.SetupGet(x => x.Location).Returns(cell);
		SimpleCharacterAction? action = null;
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		         .Callback<IEffect, TimeSpan>((effect, _) =>
		         {
			         Assert.IsInstanceOfType(effect, typeof(SimpleCharacterAction));
			         action = (SimpleCharacterAction)effect;
		         });

		var forageMethod = typeof(Cell).Assembly
		                              .GetType("MudSharp.Commands.Modules.GameModule")!
		                              .GetMethod("Forage", BindingFlags.Static | BindingFlags.NonPublic)!;
		forageMethod.Invoke(null, [character.Object, "forage food"]);

		Assert.IsNotNull(action);
		action.Action(character.Object);
		Assert.AreEqual(0.999, cell.GetForagableYield("food"));
		gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	public void Cell_ConcurrentDiscreteCompletionsSpendFinalPointOnce()
	{
		var profile = CreateProfileMock(79L, ("junk", 2.0));
		var profiles = new RevisableAll<IForagableProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var cell = CreateLoadedCell(gameworld.Object, 79L, "junk", 1.0);
		var successes = 0;

		Parallel.For(0, 2, _ =>
		{
			if (cell.TryConsumeYield("junk", 1.0))
			{
				Interlocked.Increment(ref successes);
			}
		});

		Assert.AreEqual(1, successes);
		Assert.AreEqual(0.0, cell.GetForagableYield("junk"));
	}

	[TestMethod]
	public void ForagerAI_DirectEdibleYieldRemainsFractional()
	{
		var race = new Mock<IRace>();
		race.SetupGet(x => x.EdibleForagableYields).Returns(new[]
		{
			new EdibleForagableYield { YieldType = "grazing", YieldPerBite = 0.1 }
		});
		race.Setup(x => x.CanEatForagableYield("grazing")).Returns(true);
		var character = new Mock<MudSharp.Character.ICharacter>();
		character.SetupGet(x => x.Race).Returns(race.Object);
		var cell = new Mock<ICell>();
		cell.Setup(x => x.GetForagableYield("grazing")).Returns(0.1);

		Assert.IsTrue(ForagerAIHelpers.HasDirectEdibleYield(character.Object, cell.Object));
		cell.Verify(x => x.CanConsumeYield(It.IsAny<string>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void ForagerAI_DiscreteFoodRequiresAWholeYield()
	{
		var proto = new Mock<IGameItemProto>();
		proto.Setup(x => x.IsItemType<FoodGameItemComponentProto>()).Returns(true);
		var foragable = new Mock<IForagable>();
		foragable.SetupGet(x => x.ItemProto).Returns(proto.Object);
		foragable.SetupGet(x => x.ForagableTypes).Returns(new[] { "food" });
		foragable.Setup(x => x.CanForage(It.IsAny<MudSharp.Character.ICharacter>(), Outcome.MajorPass)).Returns(true);
		var profile = new Mock<IForagableProfile>();
		profile.SetupGet(x => x.Foragables).Returns(new[] { foragable.Object });
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.ForagableProfile).Returns(profile.Object);
		cell.Setup(x => x.CanConsumeYield("food", 1.0)).Returns(false);
		var needs = new Mock<INeedsModel>();
		needs.SetupGet(x => x.Status).Returns(NeedsResult.Hungry);
		var race = new Mock<IRace>();
		race.SetupGet(x => x.EdibleForagableYields).Returns(Array.Empty<EdibleForagableYield>());
		var character = new Mock<MudSharp.Character.ICharacter>();
		character.SetupGet(x => x.State).Returns(CharacterState.Awake);
		character.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		character.SetupGet(x => x.NeedsModel).Returns(needs.Object);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.SetupGet(x => x.Location).Returns(cell.Object);

		Assert.IsFalse(ForagerAIHelpers.HasEligibleForageableFood(character.Object, cell.Object));
		Assert.IsFalse(ForagerAIHelpers.TryForageForFood(character.Object));

		cell.Setup(x => x.CanConsumeYield("food", 1.0)).Returns(true);
		Assert.IsTrue(ForagerAIHelpers.HasEligibleForageableFood(character.Object, cell.Object));
		Assert.IsTrue(ForagerAIHelpers.TryForageForFood(character.Object));
		character.Verify(x => x.ExecuteCommand("forage food"), Times.Once);
	}

	[TestMethod]
	public void Cell_ExplicitProfileLoadedAfterConstruction_ResolvesAndRestoresPersistedYields()
	{
		var profiles = new RevisableAll<IForagableProfile>();
		var heartbeat = new Mock<IHeartbeatManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(profiles);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		var cell = CreateForagingCell(gameworld.Object, null, 42L);

		Assert.IsNull(cell.ForagableProfile);

		var profile = CreateProfileMock(42L, ("food", 10.0), ("wood", 8.0), ("stone", 5.0));
		profiles.Add(profile.Object);
		var dbCell = new DbCell { Id = 100L, ForagableProfileId = 42L };
		dbCell.CellsForagableYields.Add(new DbCellsForagableYield { ForagableType = "food", Yield = 3.0 });
		dbCell.CellsForagableYields.Add(new DbCellsForagableYield { ForagableType = "wood", Yield = 2.0 });
		dbCell.CellsForagableYields.Add(new DbCellsForagableYield { ForagableType = "stone", Yield = 1.0 });

		cell.PostLoadTasks(dbCell);

		Assert.AreSame(profile.Object, cell.ForagableProfile);
		Assert.AreEqual(3.0, cell.GetForagableYield("food"));
		Assert.AreEqual(2.0, cell.GetForagableYield("wood"));
		Assert.AreEqual(1.0, cell.GetForagableYield("stone"));
		CollectionAssert.AreEquivalent(new[] { "food", "wood", "stone" }, cell.ForagableTypes.ToArray());
		Assert.IsFalse(cell.YieldsChanged);
	}

	[TestMethod]
	public void Cell_NullExplicitProfile_StillUsesInheritedProfileAndPersistedYields()
	{
		var inheritedProfile = CreateProfileMock(55L, ("food", 6.0));
		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.ForagableProfile).Returns(inheritedProfile.Object);
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Zone).Returns(zone.Object);
		var heartbeat = new Mock<IHeartbeatManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ForagableProfiles).Returns(new RevisableAll<IForagableProfile>());
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		var cell = CreateForagingCell(gameworld.Object, room.Object, null);
		var dbCell = new DbCell();
		dbCell.CellsForagableYields.Add(new DbCellsForagableYield { ForagableType = "food", Yield = 2.5 });

		cell.PostLoadTasks(dbCell);

		Assert.AreSame(inheritedProfile.Object, cell.ForagableProfile);
		Assert.AreEqual(2.5, cell.GetForagableYield("food"));
		Assert.IsFalse(cell.YieldsChanged);
	}

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
	public void Foragable_LoadFromDb_CommodityOutputCanSubmit()
	{
		var material = new Mock<ISolid>();
		material.SetupGet(x => x.Id).Returns(22);
		material.SetupGet(x => x.Name).Returns("Oak");
		var gameworld = CreateGameworld(material: material.Object);
		var foragable = new Foragable(new DbForagable
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Fallen Branches",
			ForagableTypes = "wood",
			ForageDifficulty = (int)Difficulty.Normal,
			RelativeChance = 100,
			MinimumOutcome = (int)Outcome.MajorFail,
			MaximumOutcome = (int)Outcome.MajorPass,
			QuantityDiceExpression = "1",
			ItemProtoId = 0,
			CommodityMaterialId = 22,
			CommodityWeightExpression = "500",
			EditableItem = CreateEditableItem()
		}, gameworld.Object);

		Assert.IsNull(foragable.ItemProto);
		Assert.AreSame(material.Object, foragable.CommodityMaterial);
		Assert.AreEqual("500", foragable.CommodityWeightExpression);
		Assert.IsTrue(foragable.CanSubmit());
	}

	[TestMethod]
	public void ForagableProfile_GetForageResult_ReturnsCommodityMatch()
	{
		var actor = Mock.Of<MudSharp.Character.ICharacter>();
		var foragable = CreateForagable(1, "Fallen Branches", "wood", Difficulty.Normal, commodity: true);
		foragable.Setup(x => x.CanForage(actor, Outcome.Pass)).Returns(true);
		var gameworld = CreateGameworld(foragable: foragable.Object);
		var profile = CreateProfile(gameworld.Object, "wood", 1);
		var outcomes = new Dictionary<Difficulty, CheckOutcome>
		{
			[Difficulty.Normal] = CheckOutcome.SimpleOutcome(CheckType.ForageCheck, Outcome.Pass)
		};

		Assert.AreSame(foragable.Object, profile.GetForageResult(actor, outcomes, "wood"));
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

	private static Mock<IForagable> CreateForagable(long id, string name, string forageType, Difficulty difficulty,
		bool commodity = false)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id + 100);
		proto.SetupGet(x => x.RevisionNumber).Returns(0);
		proto.SetupGet(x => x.Name).Returns(name);

		var foragable = new Mock<IForagable>();
		foragable.SetupGet(x => x.Id).Returns(id);
		foragable.SetupGet(x => x.Name).Returns(name);
		if (commodity)
		{
			foragable.SetupGet(x => x.ItemProto).Returns((IGameItemProto)null!);
		}
		else
		{
			foragable.SetupGet(x => x.ItemProto).Returns(proto.Object);
		}

		if (commodity)
		{
			var material = new Mock<ISolid>();
			material.SetupGet(x => x.Id).Returns(id + 200);
			material.SetupGet(x => x.Name).Returns(name);
			foragable.SetupGet(x => x.CommodityMaterial).Returns(material.Object);
			foragable.SetupGet(x => x.CommodityWeightExpression).Returns("500");
		}
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

	private static Mock<IFuturemud> CreateGameworld(IGameItemProto? itemProto = null, IForagable? foragable = null,
		ISolid? material = null, ITag? tag = null)
	{
		var progRepo = new Mock<IUneditableAll<IFutureProg>>();
		progRepo.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null!);

		var itemRepo = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		itemRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(itemProto!);

		var materialRepo = new Mock<IUneditableAll<ISolid>>();
		materialRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(material!);

		var tagRepo = new Mock<IUneditableAll<ITag>>();
		tagRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(tag!);

		var foragableRepo = new Mock<IUneditableRevisableAll<IForagable>>();
		foragableRepo.Setup(x => x.Get(It.IsAny<long>())).Returns(foragable!);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemRepo.Object);
		gameworld.SetupGet(x => x.Materials).Returns(materialRepo.Object);
		gameworld.SetupGet(x => x.Tags).Returns(tagRepo.Object);
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

	private static Cell CreateForagingCell(IFuturemud gameworld, IRoom? room, long? explicitProfileId)
	{
		var cell = TestObjectFactory.CreateUninitialized<Cell>();
		SetLateInitialisingGameworld(cell, gameworld);
		typeof(Cell).GetField("<Room>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(cell, room);
		typeof(Cell).GetField("_foragableYields", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(cell, new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase));
		typeof(Cell).GetField("_foragableProfileId", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(cell, explicitProfileId ?? 0);
		return cell;
	}

	private static void SetLateInitialisingGameworld(object item, IFuturemud gameworld)
	{
		typeof(LateKeywordedInitialisingItem)
			.GetProperty(nameof(LateKeywordedInitialisingItem.Gameworld),
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(item, gameworld);
	}

	private sealed class ForageTimeExpressionScope : IDisposable
	{
		private readonly Futuremud _game;
		private readonly List<Futuremud> _games;

		public ForageTimeExpressionScope()
		{
			_game = TestObjectFactory.CreateUninitialized<Futuremud>();
			typeof(Futuremud).GetField("_staticConfigurations", BindingFlags.Instance | BindingFlags.NonPublic)!
			                 .SetValue(_game, new Dictionary<string, string> { ["BaseForageTimeExpression"] = "1" });
			_games = (List<Futuremud>)typeof(Futuremud)
				.GetField("_allgames", BindingFlags.Static | BindingFlags.NonPublic)!
				.GetValue(null)!;
			_games.Insert(0, _game);
		}

		public void Dispose()
		{
			_games.Remove(_game);
		}
	}

	private static Cell CreateLoadedCell(IFuturemud gameworld, long profileId, string yieldType, double yield)
	{
		var cell = CreateForagingCell(gameworld, null, profileId);
		var dbCell = new DbCell { ForagableProfileId = profileId };
		dbCell.CellsForagableYields.Add(new DbCellsForagableYield { ForagableType = yieldType, Yield = yield });
		cell.PostLoadTasks(dbCell);
		return cell;
	}

	private static void RunYieldTicks(Cell cell, int count)
	{
		var yieldTick = typeof(Cell).GetMethod("YieldTick", BindingFlags.Instance | BindingFlags.NonPublic)!;
		for (var i = 0; i < count; i++)
		{
			yieldTick.Invoke(cell, null);
		}
	}

	private static Mock<IForagableProfile> CreateProfileMock(long id,
		params (string Type, double Maximum)[] yields)
	{
		var profile = new Mock<IForagableProfile>();
		profile.SetupGet(x => x.Id).Returns(id);
		profile.SetupGet(x => x.RevisionNumber).Returns(1);
		profile.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		profile.SetupGet(x => x.MaximumYieldPoints)
			.Returns(yields.ToDictionary(x => x.Type, x => x.Maximum));
		profile.SetupGet(x => x.HourlyYieldPoints)
			.Returns(new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase));
		return profile;
	}

	private static Mock<IForagableProfile> CreateRecoveringProfileMock(long id, string type, double maximum,
		double hourly)
	{
		var profile = CreateProfileMock(id, (type, maximum));
		profile.SetupGet(x => x.HourlyYieldPoints)
			.Returns(new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase) { [type] = hourly });
		return profile;
	}
}
