using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using ConcreteCharacter = MudSharp.Character.Character;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeAccentMasteryTests
{
	[TestMethod]
	public void AccentlessSpeechRequiresNoAccentCheck()
	{
		Assert.AreEqual(Difficulty.Automatic, new Fixture().Character.AccentDifficulty(null!));
	}

	private sealed class Fixture
	{
		public ConcreteCharacter Character { get; } = TestObjectFactory.CreateUninitialized<ConcreteCharacter>();
		public Mock<ITrait> Skill { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ILanguage> Language { get; } = new();
		public ILanguage Home { get; } = Mock.Of<ILanguage>();
		public Dictionary<IAccent, Difficulty> Familiarity { get; } = [];
		public Dictionary<ILanguage, IAccent> Acquisition { get; } = [];
		public IAccent Native { get; }
		public IAccent Learner { get; }
		public Fixture()
		{
			var definition = new Mock<ITraitDefinition>();
			definition.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			Language.SetupGet(x => x.LinkedTrait).Returns(definition.Object);
			Skill.SetupGet(x => x.Definition).Returns(definition.Object);
			Skill.SetupGet(x => x.MaxValue).Returns(200);
			Skill.SetupGet(x => x.RawValue).Returns(100);
			World.Setup(x => x.GetStaticBool("AllowAccentsToGetToAutomatic")).Returns(true);
			World.Setup(x => x.GetStaticConfiguration("NativeAccentFamiliarityFloor")).Returns("Easy");
			Set("_languagesChanged", true);
			Set("_nativeLanguage", Home);
			Set("_languages", new List<ILanguage> { Home, Language.Object });
			Set("_characterTraits", new List<ITrait> { Skill.Object });
			Set("_accents", Familiarity);
			Set("_acquisitionAccents", Acquisition);
			Set("_preferredAccents", new Dictionary<ILanguage, IAccent>());
			typeof(ConcreteCharacter).GetProperty("Gameworld")!.SetValue(Character, World.Object);
			Native = Accent(1, AccentRole.Native);
			Learner = Accent(2, AccentRole.Fallback);
			Language.SetupGet(x => x.Accents).Returns([Native, Learner]);
			Acquisition[Language.Object] = Learner;
			Familiarity[Learner] = Difficulty.Normal;
		}
		public void Set(string field, object value) => typeof(ConcreteCharacter).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Character, value);
		public IAccent Accent(long id, AccentRole role)
		{
			var accent = new Mock<IAccent>();
			accent.SetupGet(x => x.Id).Returns(id);
			accent.SetupGet(x => x.Language).Returns(Language.Object);
			accent.SetupGet(x => x.Role).Returns(role);
			accent.SetupGet(x => x.Group).Returns("shared");
			accent.SetupGet(x => x.Difficulty).Returns(Difficulty.Normal);
			accent.SetupGet(x => x.AssociatedLanguages).Returns([]);
			return accent.Object;
		}
	}

	[TestMethod]
	public void BothMasteryConditionsAreRequiredAndPreferencesDoNotChangeAnchor()
	{
		var f = new Fixture();
		f.Familiarity[f.Learner] = Difficulty.Automatic;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Easy, f.Familiarity[f.Native]);
		f.Character.SetPreferredAccent(f.Native);
		Assert.AreSame(f.Learner, f.Character.AcquisitionAccent(f.Language.Object));
		f.Skill.SetupGet(x => x.RawValue).Returns(200);
		f.Familiarity[f.Learner] = Difficulty.Normal;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Easy, f.Familiarity[f.Native]);
		f.Familiarity[f.Learner] = Difficulty.Automatic;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Automatic, f.Familiarity[f.Native]);
	}

	[TestMethod]
	public void TrivialMasteryRespectsAutomaticSettingAndNativeAnchorCanUnlock()
	{
		var f = new Fixture();
		f.Skill.SetupGet(x => x.RawValue).Returns(200);
		f.Familiarity[f.Learner] = Difficulty.Trivial;
		f.Character.LearnAccent(f.Native, Difficulty.Trivial);
		Assert.AreEqual(Difficulty.Easy, f.Familiarity[f.Native]);
		f.World.Setup(x => x.GetStaticBool("AllowAccentsToGetToAutomatic")).Returns(false);
		f.Character.LearnAccent(f.Native, Difficulty.Trivial);
		Assert.AreEqual(Difficulty.Trivial, f.Familiarity[f.Native]);
		f.Acquisition[f.Language.Object] = f.Native;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Automatic, f.Familiarity[f.Native]);
	}

	[TestMethod]
	public void LegacyFamiliarityIsPreservedAndRelatedAccentsCannotBypassFloor()
	{
		var f = new Fixture();
		f.Familiarity[f.Learner] = Difficulty.Automatic;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Easy, f.Character.AccentDifficulty(f.Native, false));
		f.Familiarity[f.Native] = Difficulty.Trivial;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Trivial, f.Character.AccentDifficulty(f.Native, false));
		f.Character.NativeLanguage = f.Language.Object;
		f.Character.LearnAccent(f.Native, Difficulty.Automatic);
		Assert.AreEqual(Difficulty.Automatic, f.Familiarity[f.Native]);
	}

	[TestMethod]
	public void NativeIdentityIsStableUntilItsLanguageIsForgotten()
	{
		var f = new Fixture();
		f.Skill.SetupGet(x => x.RawValue).Returns(200);
		Assert.AreSame(f.Home, f.Character.NativeLanguage);
		f.Character.ForgetLanguage(f.Home);
		Assert.AreSame(f.Language.Object, f.Character.NativeLanguage);
		f.Character.ForgetLanguage(f.Language.Object);
		Assert.IsNull(f.Character.NativeLanguage);
		Assert.IsNull(f.Character.AcquisitionAccent(f.Language.Object));
	}

	[TestMethod]
	public void LearningUsesHeardAccentOnceAndLanguageWithoutAccentsIsSafe()
	{
		var f = new Fixture();
		f.Set("_languages", new List<ILanguage> { f.Home });
		f.Acquisition.Clear();
		f.Familiarity.Clear();
		f.Familiarity[f.Native] = Difficulty.Automatic; // Prior accent grants do not override a new language acquisition.
		var heard = f.Accent(3, AccentRole.Native);
		f.Language.SetupGet(x => x.Accents).Returns([f.Native, heard]);
		using (new LanguageAcquisitionContext(f.Character, heard)) f.Character.LearnLanguage(f.Language.Object);
		Assert.AreSame(heard, f.Character.AcquisitionAccent(f.Language.Object));
		f.Character.LearnLanguage(f.Language.Object);
		Assert.AreSame(heard, f.Character.AcquisitionAccent(f.Language.Object));
		var empty = new Mock<ILanguage>();
		empty.SetupGet(x => x.Accents).Returns([]);
		f.Character.LearnLanguage(empty.Object);
		Assert.IsNull(f.Character.AcquisitionAccent(empty.Object));
	}
}
