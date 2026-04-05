#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSettingResolverTests
{
    private sealed class TestAll<T>(IEnumerable<T> values) : IUneditableAll<T> where T : class, IFrameworkItem
    {
        private readonly List<T> _values = values.ToList();

        public bool Has(T value)
        {
            return _values.Contains(value);
        }

        public bool Has(long id)
        {
            return _values.Any(x => x.Id == id);
        }

        public bool Has(string name)
        {
            return _values.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public T? Get(long id)
        {
            return _values.FirstOrDefault(x => x.Id == id);
        }

        public bool TryGet(long id, out T? result)
        {
            result = Get(id);
            return result is not null;
        }

        public List<T> Get(string name)
        {
            return _values.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public T? GetByName(string name)
        {
            return _values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public T? GetByIdOrName(string value, bool permitAbbreviations = true)
        {
            return long.TryParse(value, out long id) ? Get(id) : GetByName(value);
        }

        public void ForEach(Action<T> action)
        {
            foreach (T item in _values)
            {
                action(item);
            }
        }

        public int Count => _values.Count;
        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private static Mock<ICharacterCombatSettings> CreateSetting(long id, string name, double priority,
        bool canUse = true, bool globalTemplate = true)
    {
        Mock<ICharacterCombatSettings> mock = new();
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
        Mock<IFuturemud> world = new();
        world.SetupGet(x => x.CharacterCombatSettings).Returns(new TestAll<ICharacterCombatSettings>(settings));

        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Gameworld).Returns(world.Object);

        if (raceDefault is not null)
        {
            Mock<IRace> race = new();
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
            LifecycleTestCharacter character = (LifecycleTestCharacter)RuntimeHelpers.GetUninitializedObject(typeof(LifecycleTestCharacter));
            GameworldBackingField.SetValue(character, gameworld);
            Mock<IBody> body = new();
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
        Mock<IFuturemud> world = new();
        world.SetupGet(x => x.CharacterCombatSettings).Returns(new TestAll<ICharacterCombatSettings>(settings));
        world.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        return world;
    }

    private static IRace CreateRace(ICharacterCombatSettings? raceDefault)
    {
        Mock<IRace> race = new();
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
        ICharacterCombatSettings global = CreateSetting(10, "Global", 100).Object;
        ICharacterCombatSettings raceSetting = CreateSetting(20, "Race", 200).Object;
        ICharacterCombatSettings npcSetting = CreateSetting(30, "Npc", 300).Object;
        ICharacter character = CreateCharacter([global, raceSetting, npcSetting], raceSetting);
        Mock<INPCTemplate> template = new();
        template.SetupGet(x => x.DefaultCombatSetting).Returns(npcSetting);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character, template.Object);

        Assert.AreSame(npcSetting, result);
    }

    [TestMethod]
    public void ResolveFallback_ValidRaceOverride_BeatsGlobalPriority()
    {
        ICharacterCombatSettings global = CreateSetting(10, "Global", 100).Object;
        ICharacterCombatSettings raceSetting = CreateSetting(20, "Race", 0).Object;
        ICharacter character = CreateCharacter([global, raceSetting], raceSetting);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character);

        Assert.AreSame(raceSetting, result);
    }

    [TestMethod]
    public void ResolveFallback_InvalidNpcTemplateOverride_FallsThroughToRace()
    {
        ICharacterCombatSettings global = CreateSetting(10, "Global", 100).Object;
        ICharacterCombatSettings raceSetting = CreateSetting(20, "Race", 0).Object;
        ICharacterCombatSettings invalidNpc = CreateSetting(30, "Npc", 1000, canUse: false).Object;
        ICharacter character = CreateCharacter([global, raceSetting, invalidNpc], raceSetting);
        Mock<INPCTemplate> template = new();
        template.SetupGet(x => x.DefaultCombatSetting).Returns(invalidNpc);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character, template.Object);

        Assert.AreSame(raceSetting, result);
    }

    [TestMethod]
    public void ResolveFallback_InvalidRaceOverride_FallsThroughToHighestPriorityGlobal()
    {
        ICharacterCombatSettings globalLow = CreateSetting(10, "Low", 10).Object;
        ICharacterCombatSettings globalHigh = CreateSetting(20, "High", 20).Object;
        ICharacterCombatSettings invalidRace = CreateSetting(30, "Race", 1000, canUse: false).Object;
        ICharacter character = CreateCharacter([globalLow, globalHigh, invalidRace], invalidRace);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character);

        Assert.AreSame(globalHigh, result);
    }

    [TestMethod]
    public void ResolveFallback_HighestPriorityGlobal_Wins()
    {
        ICharacterCombatSettings low = CreateSetting(10, "Low", 10).Object;
        ICharacterCombatSettings high = CreateSetting(20, "High", 20).Object;
        ICharacter character = CreateCharacter([low, high]);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character);

        Assert.AreSame(high, result);
    }

    [TestMethod]
    public void ResolveFallback_EqualPriorityGlobal_BreaksTieByLowestId()
    {
        ICharacterCombatSettings lowerId = CreateSetting(10, "LowerId", 10).Object;
        ICharacterCombatSettings higherId = CreateSetting(20, "HigherId", 10).Object;
        ICharacter character = CreateCharacter([higherId, lowerId]);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveFallback(character);

        Assert.AreSame(lowerId, result);
    }

    [TestMethod]
    public void ResolveProvisional_NpcThenRaceThenLowestIdGlobal_UsesNonValidatingOrder()
    {
        ICharacterCombatSettings globalHigherId = CreateSetting(20, "Global", 200).Object;
        ICharacterCombatSettings globalLowerId = CreateSetting(10, "LowerIdGlobal", 100).Object;
        ICharacterCombatSettings raceSetting = CreateSetting(30, "Race", 0).Object;
        ICharacterCombatSettings npcSetting = CreateSetting(40, "Npc", 0).Object;
        ICharacter character = CreateCharacter([globalHigherId, globalLowerId, raceSetting, npcSetting], raceSetting);
        Mock<INPCTemplate> template = new();
        template.SetupGet(x => x.DefaultCombatSetting).Returns(npcSetting);

        Assert.AreSame(npcSetting, CharacterCombatSettingsResolver.ResolveProvisional(character, template.Object));
        Assert.AreSame(raceSetting, CharacterCombatSettingsResolver.ResolveProvisional(CreateCharacter([globalHigherId, globalLowerId, raceSetting], raceSetting)));
        Assert.AreSame(globalLowerId, CharacterCombatSettingsResolver.ResolveProvisional(CreateCharacter([globalHigherId, globalLowerId])));
    }

    [TestMethod]
    public void ResolveProvisional_DoesNotInvokeAvailabilityOrPriorityValidation()
    {
        Mock<ICharacterCombatSettings> setting = new();
        setting.SetupGet(x => x.Id).Returns(10);
        setting.SetupGet(x => x.Name).Returns("Global");
        setting.SetupGet(x => x.FrameworkItemType).Returns("CharacterCombatSetting");
        setting.SetupGet(x => x.GlobalTemplate).Returns(true);
        ICharacter character = CreateCharacter([setting.Object]);

        ICharacterCombatSettings result = CharacterCombatSettingsResolver.ResolveProvisional(character);

        Assert.AreSame(setting.Object, result);
        setting.Verify(x => x.CanUse(It.IsAny<ICharacter>()), Times.Never);
        setting.Verify(x => x.PriorityFor(It.IsAny<ICharacter>()), Times.Never);
    }

    [TestMethod]
    public void RevalidateCombatSettingsAfterInitialisation_DifferentFinalSetting_QueuesSave()
    {
        ICharacterCombatSettings provisional = CreateSetting(10, "Provisional", 0).Object;
        ICharacterCombatSettings final = CreateSetting(20, "Final", 10).Object;
        Mock<ISaveManager> saveManager = new();
        Mock<IFuturemud> world = CreateWorldWithSaveManager([provisional, final], saveManager);
        LifecycleTestCharacter character = LifecycleTestCharacter.Create(world.Object, CreateRace(null));

        character.ApplyProvisional(provisional);
        character.CompleteInitialisation(123);

        Assert.AreSame(final, character.CombatSettings);
        saveManager.Verify(x => x.Add(character), Times.Once);
    }

    [TestMethod]
    public void RevalidateCombatSettingsAfterInitialisation_UnchangedFinalSetting_DoesNotQueueSave()
    {
        ICharacterCombatSettings provisional = CreateSetting(10, "Provisional", 0).Object;
        Mock<ISaveManager> saveManager = new();
        Mock<IFuturemud> world = CreateWorldWithSaveManager([provisional], saveManager);
        LifecycleTestCharacter character = LifecycleTestCharacter.Create(world.Object, CreateRace(provisional));

        character.ApplyProvisional(provisional);
        character.CompleteInitialisation(123);

        Assert.AreSame(provisional, character.CombatSettings);
        saveManager.Verify(x => x.Add(It.IsAny<ISaveable>()), Times.Never);
    }

    [TestMethod]
    public void RevalidateCombatSettingsAfterInitialisation_NoValidatedSetting_ClearsProvisionalAndQueuesSave()
    {
        ICharacterCombatSettings provisional = CreateSetting(10, "Provisional", 0, canUse: false).Object;
        ICharacterCombatSettings rejected = CreateSetting(20, "Rejected", 100, canUse: false).Object;
        Mock<ISaveManager> saveManager = new();
        Mock<IFuturemud> world = CreateWorldWithSaveManager([provisional, rejected], saveManager);
        LifecycleTestCharacter character = LifecycleTestCharacter.Create(world.Object, CreateRace(rejected));

        character.ApplyProvisional(provisional);
        character.CompleteInitialisation(123);

        Assert.IsNull(character.CombatSettings);
        saveManager.Verify(x => x.Add(character), Times.Once);
    }
}
