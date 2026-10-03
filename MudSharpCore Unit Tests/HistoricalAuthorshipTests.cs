#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using Db = MudSharp.Models;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HistoricalAuthorshipTests
{
	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new(MockBehavior.Strict);
		internal readonly Mock<ICharacterArchiveService> Archives = new(MockBehavior.Strict);
		internal readonly Mock<ILanguage> Language = new();
		internal readonly Mock<IScript> Script = new();
		internal bool Archived;
		internal string? ArchivedFullName;
		internal Fixture()
		{
			Language.SetupGet(x => x.Id).Returns(1); Language.SetupGet(x => x.Name).Returns("Common");
			Language.Setup(x => x.LinkedTrait.Decorator.Decorate(It.IsAny<double>())).Returns("clear");
			Script.SetupGet(x => x.Id).Returns(2); Script.SetupGet(x => x.Name).Returns("Letters");
			Script.SetupGet(x => x.DocumentLengthModifier).Returns(1);
			var languages = new All<ILanguage>(); languages.Add(Language.Object);
			var scripts = new All<IScript>(); scripts.Add(Script.Object);
			World.SetupGet(x => x.Languages).Returns(languages); World.SetupGet(x => x.Scripts).Returns(scripts);
			World.SetupGet(x => x.Colours).Returns(new All<IColour>());
			World.SetupGet(x => x.SaveManager).Returns(new SaveManager());
			World.SetupGet(x => x.CharacterArchives).Returns(Archives.Object);
			Archives.Setup(x => x.Find(It.IsAny<long>())).Returns<long>(id => Archived ?
				new ArchivedCharacterIdentity(id, id + 100, Guid.Empty, DateTime.UnixEpoch,
					$"Archived author {id}", "a guardian", "A guardian.") { FullName = ArchivedFullName } : null);
		}
		internal Db.Writing Model(bool composite) => new()
		{
			Id = 901, AuthorId = 107, TrueAuthorId = 109, LanguageId = 1, ScriptId = 2,
			WritingType = composite ? "composite" : "simple", Definition = composite ?
				"<Definition><Text>Keep this message.</Text><DrawingSize>0</DrawingSize><DrawingSkill>30</DrawingSkill><ShortDescription>A message.</ShortDescription></Definition>" : "Keep this message."
		};
		internal IWriting Writing(bool composite) => composite ? new CompositeWriting(Model(true), World.Object) :
			new SimpleWriting(Model(false), World.Object);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Copy_ArchivedAuthors_PreservesIdsAndNarrativeWithoutLoadingActors(bool composite)
	{
		var fixture = new Fixture { Archived = true }; var source = fixture.Writing(composite);
		Assert.IsNull(source.Author); Assert.IsNull(source.TrueAuthor);
		Assert.AreEqual("Archived author 107", source.AuthorName());
		var copy = source.Copy();
		Assert.AreEqual(107L, copy.AuthorId); Assert.AreEqual(109L, copy.TrueAuthorId);
		Assert.AreEqual(source.ParseFor(null!), copy.ParseFor(null!));
		Assert.AreEqual(107L, copy.ArchivedAuthor.CharacterId); Assert.AreEqual(109L, copy.ArchivedTrueAuthor.CharacterId);
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Author_AnotherProcessArchivesCachedIdentity_DurableAttributionWins(bool composite)
	{
		var fixture = new Fixture(); var cached = new Mock<ICharacter>(); cached.SetupGet(x => x.Id).Returns(107);
		IWriting writing = composite ? new CompositeWriting(fixture.Model(true), fixture.World.Object) { Author = cached.Object, TrueAuthor = cached.Object } :
			new SimpleWriting(fixture.Model(false), fixture.World.Object) { Author = cached.Object, TrueAuthor = cached.Object };
		Assert.AreSame(cached.Object, writing.Author); Assert.AreSame(cached.Object, writing.TrueAuthor);
		fixture.Archived = true;
		Assert.IsNull(writing.Author); Assert.IsNull(writing.TrueAuthor);
		Assert.AreEqual(107L, writing.ArchivedAuthor.CharacterId); Assert.AreEqual(107L, writing.ArchivedTrueAuthor.CharacterId);
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void Drawing_AnotherProcessArchivesCachedIdentity_CopyKeepsHistoricalAuthor()
	{
		var fixture = new Fixture(); var cached = new Mock<ICharacter>();
		cached.SetupGet(x => x.Id).Returns(107); cached.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		var drawing = new Drawing(cached.Object, 30, "A sketch.", "A lasting sketch.", WritingImplementType.Stylus, DrawingSize.Sketch);
		Assert.AreSame(cached.Object, drawing.Author); fixture.Archived = true;
		Assert.IsNull(drawing.Author); var copy = drawing.Copy();
		Assert.AreEqual(107L, copy.AuthorId); Assert.AreEqual("Archived author 107", copy.AuthorName());
		Assert.AreEqual("A lasting sketch.", copy.ParseFor(null!));
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static (IWriting[] Writings, WeakReference<ICharacter> Author) CreateOnlyAuthorshipRoots(Fixture fixture)
	{
		var author = new Mock<ICharacter>(); author.SetupGet(x => x.Id).Returns(107);
		var source = new SimpleWriting(fixture.Model(false), fixture.World.Object) { Author = author.Object, TrueAuthor = author.Object };
		return ([source, source.Copy()], new WeakReference<ICharacter>(author.Object));
	}

	[TestMethod]
	public void WritingAndCopy_OnlyAuthorshipRemains_DoesNotRetainPhysicalActor()
	{
		var fixture = new Fixture(); var result = CreateOnlyAuthorshipRoots(fixture);
		GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
		Assert.IsFalse(result.Author.TryGetTarget(out _), "Historical artifacts retained a live actor graph.");
		Assert.AreEqual(107L, result.Writings[0].AuthorId); Assert.AreEqual(107L, result.Writings[1].TrueAuthorId);
		GC.KeepAlive(result.Writings);
	}

	[TestMethod]
	public void WritingHeader_HandwritingBeforeAndAfterArchive_PrintedProvenanceRemainsDistinct()
	{
		var fixture = new Fixture(); var writing = fixture.Writing(false);
		var reader = (RuntimeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeCharacter));
		Assert.IsFalse(reader.GetWritingHeader(writing).Contains("Source:"));
		fixture.Archived = true; Assert.IsFalse(reader.GetWritingHeader(writing).Contains("Source:"));
		IWriting printed = new PrintedWriting(fixture.World.Object, "Printed.", fixture.Language.Object,
			fixture.Script.Object, "A publisher", WritingStyleDescriptors.MachinePrinted, null!);
		StringAssert.Contains(reader.GetWritingHeader(printed), "Source:");
		StringAssert.Contains(reader.GetWritingHeader(printed), "A publisher");
	}

	[TestMethod]
	public void FullNameFilter_ArchivedNicknameDisplay_PreservesWritingAndDrawingMatches()
	{
		var fixture = new Fixture { Archived = true, ArchivedFullName = "John Smith" };
		var simple = fixture.Writing(false); var compositeModel = fixture.Model(true); compositeModel.Id = 903;
		var composite = new CompositeWriting(compositeModel, fixture.World.Object);
		var drawing = new Drawing(new Db.Drawing { Id = 902, AuthorId = 107, ShortDescription = "A sketch.", FullDescription = "A lasting sketch." }, fixture.World.Object);
		fixture.Archives.Setup(x => x.Find(It.IsAny<long>())).Returns<long>(id => new ArchivedCharacterIdentity(id, 207, Guid.Empty,
			DateTime.UnixEpoch, "John \"Wolf\" Smith", "a guardian", "A guardian.")
		{
			NameInfo = "<Names><PersonalName><Name culture='1'><Element usage='BirthName'>John</Element><Element usage='Surname'>Smith</Element><Element usage='Nickname'>Wolf</Element></Name></PersonalName></Names>"
		});
		var culture = new Mock<INameCulture>(); culture.SetupGet(x => x.Id).Returns(1);
		culture.Setup(x => x.NamePattern(NameStyle.FullName)).Returns(Tuple.Create("{0} {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname }));
		var cultures = new All<INameCulture>(); cultures.Add(culture.Object); fixture.World.SetupGet(x => x.NameCultures).Returns(cultures);
		var writings = new All<IWriting>(); writings.Add(simple); writings.Add(composite);
		var drawings = new All<IDrawing>(); drawings.Add(drawing);
		fixture.World.SetupGet(x => x.Writings).Returns(writings); fixture.World.SetupGet(x => x.Drawings).Returns(drawings);
		var output = new Mock<IOutputHandler>(); var text = new List<string>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((value, _, _) => text.Add(value));
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		staff.SetupGet(x => x.OutputHandler).Returns(output.Object); staff.SetupGet(x => x.LineFormatLength).Returns(120);
		staff.Setup(x => x.Account.UseUnicode).Returns(false);
		foreach (var command in new[] { "Writings", "Drawings" })
		{
			typeof(StorytellerModule).GetMethod(command, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null,
				[staff.Object, $"{command.ToLowerInvariant()} by \"John Smith\""]);
			StringAssert.Contains(text[^1], "John Smith");
		}
		Assert.AreEqual("John \"Wolf\" Smith", simple.AuthorName(NameStyle.FullWithNickname));
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ArchivedAuthor_MissingNameUsage_FallsBackWithoutPhysicalActorLoad()
	{
		var fixture = new Fixture { Archived = true };
		fixture.Archives.Setup(x => x.Find(It.IsAny<long>())).Returns<long>(id => new ArchivedCharacterIdentity(id, 207, Guid.Empty,
			DateTime.UnixEpoch, "Historical guardian", "a guardian", "A guardian.")
			{ NameInfo = "<Names><PersonalName><Name culture='1'><Element>John</Element></Name></PersonalName></Names>" });
		var culture = new Mock<INameCulture>(); culture.SetupGet(x => x.Id).Returns(1);
		var cultures = new All<INameCulture>(); cultures.Add(culture.Object); fixture.World.SetupGet(x => x.NameCultures).Returns(cultures);
		foreach (var writing in new[] { fixture.Writing(false), fixture.Writing(true) })
		{
			Assert.IsNull(writing.Author); Assert.IsNull(writing.TrueAuthor);
			Assert.AreEqual("Historical guardian", writing.AuthorName());
			Assert.AreEqual("Historical guardian", writing.ArchivedTrueAuthor.DisplayName);
		}
		var drawing = new Drawing(new Db.Drawing { Id = 902, AuthorId = 107 }, fixture.World.Object);
		Assert.IsNull(drawing.Author); Assert.AreEqual("Historical guardian", drawing.AuthorName());
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void WritingShow_LiveAccountlessNpcAndArchivedAuthor_DisplaysAttribution()
	{
		var fixture = new Fixture(); var author = new Mock<ICharacter>();
		author.SetupGet(x => x.Id).Returns(107); author.SetupGet(x => x.Name).Returns("A live guardian");
		var writing = new SimpleWriting(fixture.Model(false), fixture.World.Object) { Author = author.Object };
		var writings = new All<IWriting>(); writings.Add(writing); fixture.World.SetupGet(x => x.Writings).Returns(writings);
		var output = new Mock<IOutputHandler>(); var text = new List<string>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((value, _, _) => text.Add(value));
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		staff.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var command = typeof(StorytellerModule).GetMethod("Writing", BindingFlags.Static | BindingFlags.NonPublic)!;
		command.Invoke(null, [staff.Object, "writing show 901"]);
		StringAssert.Contains(text[^1], "A live guardian"); StringAssert.Contains(text[^1], "none");
		fixture.Archived = true; command.Invoke(null, [staff.Object, "writing show 901"]);
		StringAssert.Contains(text[^1], "Archived author 107"); StringAssert.Contains(text[^1], "archived identity");
		fixture.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}
}
