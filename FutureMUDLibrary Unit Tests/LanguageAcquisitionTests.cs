using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Communication.Language;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class LanguageAcquisitionTests
{
	private static Mock<ILanguage> Language(long id)
	{
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(id);
		language.SetupGet(x => x.Accents).Returns([]);
		return language;
	}

	private static IAccent Accent(long id, ILanguage language, AccentRole role, params ILanguage[] sources)
	{
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Id).Returns(id);
		accent.SetupGet(x => x.Language).Returns(language);
		accent.SetupGet(x => x.Role).Returns(role);
		accent.SetupGet(x => x.AssociatedLanguages).Returns(sources);
		return accent.Object;
	}

	[TestMethod]
	public void NativeIdentityUsesKnownPreferencesThenSkillAndStableId()
	{
		var first = Language(1).Object;
		var best = Language(2).Object;
		var unknown = Language(3).Object;
		Assert.IsNull(LanguageAcquisition.ResolveNative([], _ => 100));
		Assert.AreSame(first, LanguageAcquisition.ResolveNative([first], _ => 0));
		Assert.AreSame(best, LanguageAcquisition.ResolveNative([first, best], x => x == best ? 200 : 50));
		Assert.AreSame(first, LanguageAcquisition.ResolveNative([best, first], _ => 200));
		Assert.AreSame(first, LanguageAcquisition.ResolveNative([first, best], _ => 200, unknown, first, best));
		Assert.AreSame(best, LanguageAcquisition.ResolveNative([first, best], _ => 200, best, first));
	}

	[TestMethod]
	public void AccentSelectionHonoursEachFallbackAndStableOrdering()
	{
		var home = Language(1).Object;
		var target = Language(2);
		var foreign = Accent(30, target.Object, AccentRole.Foreign, home);
		var secondForeign = Accent(40, target.Object, AccentRole.Foreign, home);
		var fallback = Accent(20, target.Object, AccentRole.Fallback);
		var native = Accent(10, target.Object, AccentRole.Native);
		var heard = Accent(50, target.Object, AccentRole.Native);
		var unmatched = Accent(1, target.Object, AccentRole.Foreign);
		Assert.AreSame(foreign, LanguageAcquisition.ResolveAccent(target.Object, home, heard,
			[secondForeign, native, heard, fallback, unmatched, foreign]));
		Assert.AreSame(fallback, LanguageAcquisition.ResolveAccent(target.Object, home, heard, [native, fallback, heard]));
		Assert.AreSame(heard, LanguageAcquisition.ResolveAccent(target.Object, home, heard, [native, heard]));
		Assert.AreSame(native, LanguageAcquisition.ResolveAccent(target.Object, home, foreign, [heard, native, unmatched]));
		Assert.AreSame(unmatched, LanguageAcquisition.ResolveAccent(target.Object, home, available: [unmatched]));
		Assert.IsNull(LanguageAcquisition.ResolveAccent(target.Object, home));
		Assert.AreSame(native, LanguageAcquisition.ResolveAccent(target.Object, target.Object, available: [fallback, native]));
	}

	[TestMethod]
	public void HeardAccentContextIsLearnerScopedAndRestoredAfterNestedFailure()
	{
		var learner = Mock.Of<IHaveLanguage>();
		var other = Mock.Of<IHaveLanguage>();
		var accent = Accent(1, Language(1).Object, AccentRole.Native);
		using (new LanguageAcquisitionContext(learner, accent))
		{
			Assert.AreSame(accent, LanguageAcquisitionContext.HeardBy(learner));
			Assert.IsNull(LanguageAcquisitionContext.HeardBy(other));
			try { using var inner = new LanguageAcquisitionContext(other, accent); throw new InvalidOperationException(); }
			catch (InvalidOperationException) { }
			Assert.AreSame(accent, LanguageAcquisitionContext.HeardBy(learner));
		}
		Assert.IsNull(LanguageAcquisitionContext.HeardBy(learner));
	}
}
