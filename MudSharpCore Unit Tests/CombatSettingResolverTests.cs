#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Framework;
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
			character.SetupGet(x => x.Race).Returns((IRace?)null);
		}

		return character.Object;
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
}
