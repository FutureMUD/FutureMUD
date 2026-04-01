#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Body;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSettingResolverTests
{
	private sealed class TestAll<T>(IEnumerable<T> values) : IUneditableAll<T> where T : class, IFrameworkItem
	{
		private readonly List<T> _values = values.ToList();

		public bool Has(T value) => _values.Contains(value);
		public bool Has(long id) => _values.Any(x => x.Id == id);
		public bool Has(string name) => _values.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		public T? Get(long id) => _values.FirstOrDefault(x => x.Id == id);
		public bool TryGet(long id, out T? result)
		{
			result = Get(id);
			return result is not null;
		}

		public List<T> Get(string name) => _values.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
		public T? GetByName(string name) => _values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		public T? GetByIdOrName(string value, bool permitAbbreviations = true)
		{
			return long.TryParse(value, out var id) ? Get(id) : GetByName(value);
		}

		public void ForEach(Action<T> action)
		{
			foreach (var item in _values)
			{
				action(item);
			}
		}

		public int Count => _values.Count;
		public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private static Mock<ICharacterCombatSettings> CreateSetting(long id, string name, double priority,
		bool canUse = true, bool globalTemplate = true)
	{
		var mock = new Mock<ICharacterCombatSettings>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns("CharacterCombatSetting");
		mock.SetupGet(x => x.GlobalTemplate).Returns(globalTemplate);
		mock.Setup(x => x.CanUse(It.IsAny<ICharacter>())).Returns(canUse);
		mock.Setup(x => x.PriorityFor(It.IsAny<ICharacter>())).Returns(priority);
		return mock;
	}

	private static ICharacter CreateCharacter(IEnumerable<ICharacterCombatSettings> settings,
		ICharacterCombatSettings? raceDefault = null)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.CharacterCombatSettings).Returns(new TestAll<ICharacterCombatSettings>(settings));

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(world.Object);

		if (raceDefault is not null)
		{
			var race = new Mock<IRace>();
			race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
			{
				CanAttack = true,
				CanDefend = true,
				CanUseWeapons = true,
				DefaultCombatSetting = raceDefault
			});
			character.SetupGet(x => x.Race).Returns(race.Object);
		}
		else
		{
			character.SetupGet(x => x.Race).Returns(() => null!);
		}

		return character.Object;
	}

	private sealed class LifecycleTestCharacter : MudSharp.Character.Character
	{
		private static readonly FieldInfo GameworldBackingField =
			typeof(LateKeywordedInitialisingItem).GetField("<Gameworld>k__BackingField",
				BindingFlags.Instance | BindingFlags.NonPublic)!;

		private LifecycleTestCharacter() : base(null!, null!, true)
		{
		}

		public static LifecycleTestCharacter Create(IFuturemud gameworld, IRace? race)
		{
			var character = (LifecycleTestCharacter)RuntimeHelpers.GetUninitializedObject(typeof(LifecycleTestCharacter));
			GameworldBackingField.SetValue(character, gameworld);
			var body = new Mock<IBody>();
			body.SetupGet(x => x.Race).Returns(() => race!);
			character.Body = body.Object;
			return character;
		}

		public void ApplyProvisional(ICharacterCombatSettings setting)
		{
			SetCombatSettingsProvisional(setting);
		}

		public void CompleteInitialisation(long id)
		{
			_id = id;
			RevalidateCombatSettingsAfterInitialisation();
		}

		protected override ICharacterCombatSettings ResolveCombatSettingsAfterInitialisation()
		{
			return CharacterCombatSettingsResolver.ResolveFallback(this);
		}
	}

	private static Mock<IFuturemud> CreateWorldWithSaveManager(IEnumerable<ICharacterCombatSettings> settings,
		Mock<ISaveManager> saveManager)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.CharacterCombatSettings).Returns(new TestAll<ICharacterCombatSettings>(settings));
		world.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		return world;
	}

	private static IRace CreateRace(ICharacterCombatSettings? raceDefault)
	{
		var race = new Mock<IRace>();
		race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
		{
			CanAttack = true,
			CanDefend = true,
			CanUseWeapons = true,
			DefaultCombatSetting = raceDefault
		});
		return race.Object;
	}

	[TestMethod]
	public void ResolveFallback_ValidNpcTemplateOverride_BeatsRaceAndGlobalPriority()
	{
		var global = CreateSetting(10, "Global", 100).Object;
		var raceSetting = CreateSetting(20, "Race", 200).Object;
		var npcSetting = CreateSetting(30, "Npc", 300).Object;
		var character = CreateCharacter([global, raceSetting, npcSetting], raceSetting);
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.DefaultCombatSetting).Returns(npcSetting);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character, template.Object);

		Assert.AreSame(npcSetting, result);
	}

	[TestMethod]
	public void ResolveFallback_ValidRaceOverride_BeatsGlobalPriority()
	{
		var global = CreateSetting(10, "Global", 100).Object;
		var raceSetting = CreateSetting(20, "Race", 0).Object;
		var character = CreateCharacter([global, raceSetting], raceSetting);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character);

		Assert.AreSame(raceSetting, result);
	}

	[TestMethod]
	public void ResolveFallback_InvalidNpcTemplateOverride_FallsThroughToRace()
	{
		var global = CreateSetting(10, "Global", 100).Object;
		var raceSetting = CreateSetting(20, "Race", 0).Object;
		var invalidNpc = CreateSetting(30, "Npc", 1000, canUse: false).Object;
		var character = CreateCharacter([global, raceSetting, invalidNpc], raceSetting);
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.DefaultCombatSetting).Returns(invalidNpc);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character, template.Object);

		Assert.AreSame(raceSetting, result);
	}

	[TestMethod]
	public void ResolveFallback_InvalidRaceOverride_FallsThroughToHighestPriorityGlobal()
	{
		var globalLow = CreateSetting(10, "Low", 10).Object;
		var globalHigh = CreateSetting(20, "High", 20).Object;
		var invalidRace = CreateSetting(30, "Race", 1000, canUse: false).Object;
		var character = CreateCharacter([globalLow, globalHigh, invalidRace], invalidRace);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character);

		Assert.AreSame(globalHigh, result);
	}

	[TestMethod]
	public void ResolveFallback_HighestPriorityGlobal_Wins()
	{
		var low = CreateSetting(10, "Low", 10).Object;
		var high = CreateSetting(20, "High", 20).Object;
		var character = CreateCharacter([low, high]);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character);

		Assert.AreSame(high, result);
	}

	[TestMethod]
	public void ResolveFallback_EqualPriorityGlobal_BreaksTieByLowestId()
	{
		var lowerId = CreateSetting(10, "LowerId", 10).Object;
		var higherId = CreateSetting(20, "HigherId", 10).Object;
		var character = CreateCharacter([higherId, lowerId]);

		var result = CharacterCombatSettingsResolver.ResolveFallback(character);

		Assert.AreSame(lowerId, result);
	}

	[TestMethod]
	public void ResolveProvisional_NpcThenRaceThenLowestIdGlobal_UsesNonValidatingOrder()
	{
		var globalHigherId = CreateSetting(20, "Global", 200).Object;
		var globalLowerId = CreateSetting(10, "LowerIdGlobal", 100).Object;
		var raceSetting = CreateSetting(30, "Race", 0).Object;
		var npcSetting = CreateSetting(40, "Npc", 0).Object;
		var character = CreateCharacter([globalHigherId, globalLowerId, raceSetting, npcSetting], raceSetting);
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.DefaultCombatSetting).Returns(npcSetting);

		Assert.AreSame(npcSetting, CharacterCombatSettingsResolver.ResolveProvisional(character, template.Object));
		Assert.AreSame(raceSetting, CharacterCombatSettingsResolver.ResolveProvisional(CreateCharacter([globalHigherId, globalLowerId, raceSetting], raceSetting)));
		Assert.AreSame(globalLowerId, CharacterCombatSettingsResolver.ResolveProvisional(CreateCharacter([globalHigherId, globalLowerId])));
	}

	[TestMethod]
	public void ResolveProvisional_DoesNotInvokeAvailabilityOrPriorityValidation()
	{
		var setting = new Mock<ICharacterCombatSettings>();
		setting.SetupGet(x => x.Id).Returns(10);
		setting.SetupGet(x => x.Name).Returns("Global");
		setting.SetupGet(x => x.FrameworkItemType).Returns("CharacterCombatSetting");
		setting.SetupGet(x => x.GlobalTemplate).Returns(true);
		var character = CreateCharacter([setting.Object]);

		var result = CharacterCombatSettingsResolver.ResolveProvisional(character);

		Assert.AreSame(setting.Object, result);
		setting.Verify(x => x.CanUse(It.IsAny<ICharacter>()), Times.Never);
		setting.Verify(x => x.PriorityFor(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void RevalidateCombatSettingsAfterInitialisation_DifferentFinalSetting_QueuesSave()
	{
		var provisional = CreateSetting(10, "Provisional", 0).Object;
		var final = CreateSetting(20, "Final", 10).Object;
		var saveManager = new Mock<ISaveManager>();
		var world = CreateWorldWithSaveManager([provisional, final], saveManager);
		var character = LifecycleTestCharacter.Create(world.Object, CreateRace(null));

		character.ApplyProvisional(provisional);
		character.CompleteInitialisation(123);

		Assert.AreSame(final, character.CombatSettings);
		saveManager.Verify(x => x.Add(character), Times.Once);
	}

	[TestMethod]
	public void RevalidateCombatSettingsAfterInitialisation_UnchangedFinalSetting_DoesNotQueueSave()
	{
		var provisional = CreateSetting(10, "Provisional", 0).Object;
		var saveManager = new Mock<ISaveManager>();
		var world = CreateWorldWithSaveManager([provisional], saveManager);
		var character = LifecycleTestCharacter.Create(world.Object, CreateRace(provisional));

		character.ApplyProvisional(provisional);
		character.CompleteInitialisation(123);

		Assert.AreSame(provisional, character.CombatSettings);
		saveManager.Verify(x => x.Add(It.IsAny<ISaveable>()), Times.Never);
	}

	[TestMethod]
	public void RevalidateCombatSettingsAfterInitialisation_NoValidatedSetting_ClearsProvisionalAndQueuesSave()
	{
		var provisional = CreateSetting(10, "Provisional", 0, canUse: false).Object;
		var rejected = CreateSetting(20, "Rejected", 100, canUse: false).Object;
		var saveManager = new Mock<ISaveManager>();
		var world = CreateWorldWithSaveManager([provisional, rejected], saveManager);
		var character = LifecycleTestCharacter.Create(world.Object, CreateRace(rejected));

		character.ApplyProvisional(provisional);
		character.CompleteInitialisation(123);

		Assert.IsNull(character.CombatSettings);
		saveManager.Verify(x => x.Add(character), Times.Once);
	}
}
