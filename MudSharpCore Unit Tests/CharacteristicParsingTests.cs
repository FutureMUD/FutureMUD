using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacteristicParsingTests
{
    private class TestCharacteristicDefinition : ICharacteristicDefinition
    {
        private static long _nextId = 1;
        public TestCharacteristicDefinition(string name, string pattern, IFuturemud gameworld, CharacteristicType type = CharacteristicType.Standard)
        {
            Name = name;
            Id = _nextId++;
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
            Description = name;
            Type = type;
            ChargenDisplayType = CharacterGenerationDisplayType.DisplayAll;
            Gameworld = gameworld;
        }

        public string Name { get; }
        public long Id { get; }
        public string FrameworkItemType => "CharacteristicDefinition";
        public Regex Pattern { get; }
        public string Description { get; }
        public CharacteristicType Type { get; }
        public CharacterGenerationDisplayType ChargenDisplayType { get; }
        public ICharacteristicValue DefaultValue { get; private set; }
        public ICharacteristicDefinition Parent => null;
        public bool Changed { get; set; }
        public IFuturemud Gameworld { get; }
        public bool IsValue(ICharacteristicValue value)
        {
            return value?.Definition == this;
        }

        public bool IsDefaultValue(ICharacteristicValue value)
        {
            return value == DefaultValue;
        }

        public ICharacteristicValue GetRandomValue()
        {
            return DefaultValue;
        }

        public void SetDefaultValue(ICharacteristicValue theDefault)
        {
            DefaultValue = theDefault;
        }

        public void Save() { }
        public void BuildingCommand(MudSharp.Character.ICharacter actor, MudSharp.Framework.StringStack command) { }
        public string Show(MudSharp.Character.ICharacter actor)
        {
            return string.Empty;
        }
    }

    private class TestCharacteristicValue : ICharacteristicValue
    {
        private static long _nextId = 1;
        public TestCharacteristicValue(string text, ICharacteristicDefinition def, PluralisationType plural = PluralisationType.Singular)
        {
            Name = text;
            Id = _nextId++;
            Definition = def;
            GetValue = text;
            Pluralisation = plural;
        }

        public string Name { get; }
        public long Id { get; }
        public string FrameworkItemType => "CharacteristicValue";
        public IFutureProg ChargenApplicabilityProg => null;
        public IFutureProg OngoingValidityProg => null;
        public ICharacteristicDefinition Definition { get; }
        public string GetValue { get; }
        public string GetBasicValue => GetValue;
        public string GetFancyValue => GetValue;
        public PluralisationType Pluralisation { get; }
        public void BuildingCommand(MudSharp.Character.ICharacter actor, MudSharp.Framework.StringStack command) { }
        public string Show(MudSharp.Character.ICharacter actor)
        {
            return string.Empty;
        }

        public ICharacteristicValue Clone(string newName)
        {
            return new TestCharacteristicValue(newName, Definition, Pluralisation);
        }
    }

    private static IFuturemud Gameworld => new GameworldStub().ToMock();

    [TestMethod]
    public void ParseCharacteristicsAbsolute_BasicSubstitution()
    {
        IFuturemud gameworld = Gameworld;
        TestCharacteristicDefinition def = new("colour", "colour", gameworld);
        TestCharacteristicValue val = new("red", def);
        def.SetDefaultValue(val);
        List<(ICharacteristicDefinition, ICharacteristicValue)> list = new()
        { (def, val) };

        string result = IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute("Hair is $colour.", list, Gendering.Get(Gender.Male), gameworld);
        Assert.AreEqual("Hair is red.", result);
    }

    [TestMethod]
    public void ParseCharacteristicsAbsolute_AAnSubstitution()
    {
        IFuturemud gameworld = Gameworld;
        TestCharacteristicDefinition def = new("fruit", "fruit", gameworld);
        TestCharacteristicValue val = new("apple", def);
        List<(ICharacteristicDefinition, ICharacteristicValue)> list = new()
        { (def, val) };

        string result = IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute("&a_an[$fruit]", list, Gendering.Get(Gender.Female), gameworld);
        Assert.AreEqual("an apple", result);
    }

    [TestMethod]
    public void ParseCharacteristicsAbsolute_AAnPlural_NoArticle()
    {
        IFuturemud gameworld = Gameworld;
        TestCharacteristicDefinition def = new("item", "item", gameworld);
        TestCharacteristicValue val = new("oranges", def, PluralisationType.Plural);
        List<(ICharacteristicDefinition, ICharacteristicValue)> list = new()
        { (def, val) };

        string result = IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute("&?a_an[$item]", list, Gendering.Get(Gender.NonBinary), gameworld);
        Assert.AreEqual("oranges", result);
    }
}
