using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;
using DbWriting = MudSharp.Models.Writing;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreloadedBookContentTests
{
	private const long LanguageId = 1L;
	private const long ScriptId = 2L;
	private const long ColourId = 3L;
	private const long PaperProtoId = 4L;

	[TestMethod]
	public void PrintedWriting_LoadsThroughFactoryWithNullableAuthorAndProvenance()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.5);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200).Object;
		var dbWriting = new DbWriting
		{
			Id = 10,
			AuthorId = null,
			TrueAuthorId = null,
			LanguageId = LanguageId,
			ScriptId = ScriptId,
			WritingColour = ColourId,
			WritingType = "printed",
			ImplementType = (int)WritingImplementType.Printed,
			Style = (int)WritingStyleDescriptors.MachinePrinted,
			Definition = new XElement("Definition",
				new XElement("Text", "Follow the marked trail."),
				new XElement("Provenance", "Guild Trail Manual")
			).ToString(),
			LiteracySkill = 42.0,
			LanguageSkill = 43.0,
			HandwritingSkill = 44.0,
			ForgerySkill = 5.0
		};

		var writing = WritingFactory.LoadWriting(dbWriting, gameworld);

		Assert.IsInstanceOfType(writing, typeof(PrintedWriting));
		Assert.IsNull(writing.Author);
		Assert.IsNull(writing.TrueAuthor);
		Assert.AreEqual(WritingImplementType.Printed, writing.ImplementType);
		Assert.AreEqual((int)("Follow the marked trail.".RawTextLength() * 1.5), writing.DocumentLength);
		Assert.AreEqual(42.0, writing.LiteracySkill);
		Assert.AreEqual(43.0, writing.LanguageSkill);
		Assert.AreEqual(44.0, writing.HandwritingSkill);
		Assert.AreEqual(5.0, writing.ForgerySkill);
		Assert.AreEqual(true, writing.GetProperty("printed").GetObject);
		Assert.AreEqual("Guild Trail Manual", writing.GetProperty("provenance").GetObject);
		Assert.IsInstanceOfType(writing.GetProperty("author"), typeof(NullVariable));
	}

	[TestMethod]
	public void BookPrototypeXml_CreatesDefaultTitleAndSharedImmutablePrintedContent()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(
			3,
			"Apprentice Manual",
			WritingElement(1, 1, "Read this first.", "Training Press"),
			WritingElement(2, 1, "Then read this.", "Training Press")
		));
		var savedXml = InvokeSaveToXml(proto);
		var loadedProto = CreateBookProto(gameworld.Object, savedXml);
		var firstBook = (BookGameItemComponent)loadedProto.CreateNew(CreateParent(gameworld.Object, 20));
		var secondBook = (BookGameItemComponent)loadedProto.CreateNew(CreateParent(gameworld.Object, 21));

		Assert.AreEqual("Apprentice Manual", firstBook.Title);
		Assert.AreEqual(2, firstBook.PagesAndReadables.Count);
		Assert.AreEqual(2, secondBook.PagesAndReadables.Count);
		Assert.AreSame(firstBook.PagesAndReadables[0].Writing, secondBook.PagesAndReadables[0].Writing);
		Assert.AreEqual("Read this first.", firstBook.PagesAndReadables[0].Writing.ParseFor(null));
		Assert.AreEqual("Training Press", ((IWriting)firstBook.PagesAndReadables[0].Writing).GetProperty("provenance").GetObject);
	}

	[TestMethod]
	public void BookPrototypeCollectionReferencesExpandIntoEachBookInstance()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var firstWriting = CreateWriting(20, "First", 6, "First.");
		var laterWriting = CreateWriting(21, "Later", 6, "Later.");
		var collection = CreateWritingCollection(12, "Training Packet",
			(5, 1, firstWriting),
			(7, 1, laterWriting)
		);
		var gameworld = CreateGameworld(language, script, colour, 200, collection);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(
			5,
			"Collection Manual",
			CollectionElement(collection.Id, 2)
		));
		var firstBook = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 60));
		var secondBook = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 61));

		Assert.AreEqual(2, firstBook.PagesAndReadables.Count);
		Assert.AreEqual(2, secondBook.PagesAndReadables.Count);
		Assert.AreEqual(2, firstBook.PagesAndReadables[0].Page);
		Assert.AreEqual(4, firstBook.PagesAndReadables[1].Page);
		Assert.AreSame(firstWriting, firstBook.PagesAndReadables[0].Writing);
		Assert.AreSame(laterWriting, firstBook.PagesAndReadables[1].Writing);
		Assert.AreSame(firstWriting, secondBook.PagesAndReadables[0].Writing);

		var appendedWriting = CreateWriting(22, "Appendix", 6, "More.");
		Assert.IsTrue(firstBook.AddWriting(appendedWriting, 2));
		Assert.AreEqual(2, firstBook.PagesAndReadables.Count(x => x.Page == 2));
		Assert.AreEqual(1, secondBook.PagesAndReadables.Count(x => x.Page == 2));
		Assert.AreEqual(2, collection.Entries.Count());
	}

	[TestMethod]
	public void BookPrototypeDirectReadableReferencesResolveLazilyAfterWritingsLoad()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var writing = CreateWriting(25, "Manual Page", 8, "Loaded later.");
		var proto = CreateBookProto(gameworld.Object, BookDefinition(
			2,
			"Lazy Manual",
			DirectWritingElement(1, 1, writing.Id)
		));

		var savedXml = InvokeSaveToXml(proto);
		gameworld.Object.Add(writing);
		var book = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 70));

		Assert.IsTrue(savedXml.Contains("Id=\"25\""));
		Assert.AreEqual(1, book.PagesAndReadables.Count);
		Assert.AreSame(writing, book.PagesAndReadables[0].Writing);
	}

	[TestMethod]
	public void CopyBookSkipsTornSourcePages()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(4, "Copy Test"));
		var source = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 80));
		var destination = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 81));
		var tornWriting = CreateWriting(30, "Torn", 6, "Gone.");
		var presentWriting = CreateWriting(31, "Present", 6, "Here.");
		source.AddWriting(tornWriting, 1);
		source.AddWriting(presentWriting, 2);
		source.TornPages.Add(1);

		Assert.IsTrue(InvokeTryCopyBook(source, destination, mutate: true, out var error), error);

		Assert.AreEqual(1, destination.PagesAndReadables.Count);
		Assert.AreSame(presentWriting, destination.PagesAndReadables[0].Writing);
		Assert.AreEqual(1, destination.PagesAndReadables[0].Page);
	}

	[TestMethod]
	public void WritingCollectionMarkdownImportRejectsPageZeroBeforeCreatingReadables()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.CurrentWritingLanguage).Returns(language);
		actor.SetupGet(x => x.CurrentScript).Returns(script);

		var result = WritingCollectionImport.ImportMarkdown(gameworld.Object, actor.Object, "--- page 0 ---\nThis should not import.");

		Assert.IsFalse(result.Success);
		Assert.AreEqual("Page numbers must be positive.", result.Error);
		Assert.AreEqual(0, gameworld.Object.Writings.Count);
	}
	[TestMethod]
	public void BookPrototypeContent_InvalidPagesAndOversizedTextAreSkippedOnFreshItems()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 12);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(
			1,
			"Small Book",
			WritingElement(1, 1, "Fits.", "Small Press"),
			WritingElement(2, 1, "Wrong page.", "Small Press"),
			WritingElement(1, 2, "This line is much too long for the configured page capacity.", "Small Press")
		));

		var book = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 30));

		Assert.AreEqual(1, book.PagesAndReadables.Count);
		Assert.AreEqual("Fits.", book.PagesAndReadables[0].Writing.ParseFor(null));
	}

	[TestMethod]
	public void BookWithPreloadedContent_AllowsPlayerWritingWhenCapacityRemains()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 50);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(
			2,
			"Mixed Notebook",
			WritingElement(1, 1, "Printed.", "Press")
		));
		var book = (BookGameItemComponent)proto.CreateNew(CreateParent(gameworld.Object, 40));
		var handwritten = new Mock<IWriting>();
		handwritten.SetupGet(x => x.DocumentLength).Returns(10);

		var result = book.AddWriting(handwritten.Object, 1);

		Assert.IsTrue(result);
		Assert.AreEqual(2, book.PagesAndReadables.Count(x => x.Page == 1));
	}

	[TestMethod]
	public void BookAndPaperReadableComponents_InitialiseAfterReadableChildren()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var bookProto = CreateBookProto(gameworld.Object, BookDefinition(1, "Manual"));
		var paperProto = CreatePaperProto(gameworld.Object, 200);
		var parent = CreateParent(gameworld.Object, 50);

		var book = (BookGameItemComponent)bookProto.CreateNew(parent);
		var paper = (PaperSheetGameItemComponent)paperProto.CreateNew(parent);

		Assert.AreEqual(InitialisationPhase.AfterFirstDatabaseHit, book.InitialisationPhase);
		Assert.AreEqual(InitialisationPhase.AfterFirstDatabaseHit, paper.InitialisationPhase);
	}

	[TestMethod]
	public void BookLoadFromXml_SkipsMissingReadableReferences()
	{
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(1, "Manual"));
		var definition = new XElement("Definition",
			new XElement("Open", true),
			new XElement("TornPages"),
			new XElement("Title", new XCData("Manual")),
			new XElement("CurrentPage", 1),
			new XElement("Writings",
				new XElement("Writing",
					new XAttribute("Id", 0),
					new XAttribute("Page", 1),
					new XAttribute("Order", 1)
				)
			)
		).ToString();

		var book = new BookGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 1,
			Definition = definition,
			GameItemComponentProtoId = proto.Id,
			GameItemComponentProtoRevision = proto.RevisionNumber,
			GameItemId = 50
		}, proto, CreateParent(gameworld.Object, 50));

		Assert.AreEqual(0, book.PagesAndReadables.Count);
		Assert.IsFalse(book.Readables.Any());
	}

	[TestMethod]
	public void FutureProgBookHelpers_CompileAndExecuteAgainstBookComponents()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var language = CreateLanguage(LanguageId, "Common");
		var script = CreateScript(ScriptId, "Latin", 1.0);
		var colour = CreateColour(ColourId, "black");
		var gameworld = CreateGameworld(language, script, colour, 200);
		var proto = CreateBookProto(gameworld.Object, BookDefinition(3, string.Empty));
		var item = CreateBookItem(gameworld.Object, proto, out var book);

		AssertFunctionRegistered("addprintedwriting", ProgVariableTypes.Item, ProgVariableTypes.Number,
			ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Script, ProgVariableTypes.Text);
		AssertFunctionRegistered("addprintedwriting", ProgVariableTypes.Item, ProgVariableTypes.Number,
			ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Script, ProgVariableTypes.Text,
			ProgVariableTypes.Text, ProgVariableTypes.Text);
		var setTitle = CompileFunction("setbooktitle", gameworld.Object,
			(ProgVariableTypes.Item, item),
			(ProgVariableTypes.Text, "Field Manual")
		);
		Assert.AreEqual(StatementResult.Normal, setTitle.Execute(null));
		Assert.AreEqual(true, setTitle.Result.GetObject);
		Assert.AreEqual("Field Manual", book.Title);

		var addPrinted = CompileFunction("addprintedwriting", gameworld.Object,
			(ProgVariableTypes.Item, item),
			(ProgVariableTypes.Number, 1.0M),
			(ProgVariableTypes.Text, "Keep dry."),
			(ProgVariableTypes.Language, language),
			(ProgVariableTypes.Script, script),
			(ProgVariableTypes.Text, "Quartermaster")
		);
		Assert.AreEqual(StatementResult.Normal, addPrinted.Execute(null));
		Assert.AreEqual(true, addPrinted.Result.GetObject);
		var printed = (IWriting)book.PagesAndReadables.Single(x => x.Page == 1).Writing;
		Assert.AreEqual("Quartermaster", printed.GetProperty("provenance").GetObject);

		var copyWriting = CompileFunction("copywritingto", gameworld.Object,
			(ProgVariableTypes.Item, item),
			(ProgVariableTypes.Number, 2.0M),
			(ProgVariableTypes.Writing, printed)
		);
		Assert.AreEqual(StatementResult.Normal, copyWriting.Execute(null));
		Assert.AreEqual(true, copyWriting.Result.GetObject);
		var copied = (IWriting)book.PagesAndReadables.Single(x => x.Page == 2).Writing;
		Assert.AreSame(printed, copied);
		Assert.AreEqual("Keep dry.", copied.ParseFor(null));
	}

	private static IFunction CompileFunction(string name, IFuturemud gameworld, params (ProgVariableTypes Type, object Value)[] parameters)
	{
		var compiler = FutureProg.GetFunctionCompilerInformations()
		                         .Single(x => x.FunctionName.EqualTo(name) &&
		                                      x.Parameters.SequenceEqual(parameters.Select(y => y.Type)));
		var functions = parameters
		                .Select(x => new ConstantTestFunction(x.Type, x.Value))
		                .Cast<IFunction>()
		                .ToList();
		return compiler.CompilerFunction(functions, gameworld);
	}

	private static void AssertFunctionRegistered(string name, params ProgVariableTypes[] parameters)
	{
		Assert.IsTrue(
			FutureProg.GetFunctionCompilerInformations()
			          .Any(x => x.FunctionName.EqualTo(name) && x.Parameters.SequenceEqual(parameters)),
			$"Expected FutureProg function {name}({string.Join(", ", parameters.Select(x => x.Describe()))}) to be registered."
		);
	}

	private static BookGameItemComponentProto CreateBookProto(IFuturemud gameworld, string definition)
	{
		return (BookGameItemComponentProto)Activator.CreateInstance(
			typeof(BookGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { ComponentProtoModel(100, "Book", definition), gameworld },
			CultureInfo.InvariantCulture
		)!;
	}

	private static PaperSheetGameItemComponentProto CreatePaperProto(IFuturemud gameworld, int maximumLength)
	{
		return (PaperSheetGameItemComponentProto)Activator.CreateInstance(
			typeof(PaperSheetGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(101, "PaperSheet", new XElement("Definition",
					new XElement("MaximumCharacterLengthOfText", maximumLength)
				).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture
		)!;
	}

	private static DbGameItemComponentProto ComponentProtoModel(long id, string type, string definition)
	{
		return new DbGameItemComponentProto
		{
			Id = id,
			Type = type,
			Name = type,
			Description = $"{type} component",
			Definition = definition,
			RevisionNumber = 1,
			EditableItem = new DbEditableItem
			{
				Id = id,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}

	private static string InvokeSaveToXml(BookGameItemComponentProto proto)
	{
		return (string)typeof(BookGameItemComponentProto)
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(proto, Array.Empty<object>())!;
	}

	private static string BookDefinition(int pageCount, string title, params XElement[] entries)
	{
		return new XElement("Definition",
			new XElement("PaperProto", PaperProtoId),
			new XElement("PageCount", pageCount),
			new XElement("DefaultTitle", new XCData(title)),
			new XElement("InitialReadables", entries.Where(x => !x.Name.LocalName.EqualTo("Collection"))),
			new XElement("InitialCollections", entries.Where(x => x.Name.LocalName.EqualTo("Collection")))
		).ToString();
	}

	private static XElement CollectionElement(long collectionId, int startPage)
	{
		return new XElement("Collection",
			new XAttribute("Id", collectionId),
			new XAttribute("StartPage", startPage)
		);
	}

	private static XElement DirectWritingElement(int page, int order, long writingId)
	{
		return new XElement("Writing",
			new XAttribute("Id", writingId),
			new XAttribute("Page", page),
			new XAttribute("Order", order)
		);
	}

	private static bool InvokeTryCopyBook(BookGameItemComponent source, BookGameItemComponent destination, bool mutate, out string error)
	{
		var args = new object[] { source, destination, string.Empty, mutate };
		var result = (bool)typeof(MudSharp.Commands.Modules.LiteracyModule)
			.GetMethod("TryCopyBook", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, args)!;
		error = (string)args[2];
		return result;
	}

	private static XElement WritingElement(int page, int order, string text, string provenance)
	{
		return new XElement("Writing",
			new XAttribute("Type", "printed"),
			new XAttribute("Page", page),
			new XAttribute("Order", order),
			new XElement("Language", LanguageId),
			new XElement("Script", ScriptId),
			new XElement("Colour", ColourId),
			new XElement("Style", (int)WritingStyleDescriptors.MachinePrinted),
			new XElement("Provenance", new XCData(provenance)),
			new XElement("Text", new XCData(text)),
			new XElement("LiteracySkill", 100.0),
			new XElement("LanguageSkill", 100.0),
			new XElement("HandwritingSkill", 100.0),
			new XElement("ForgerySkill", 0.0)
		);
	}

	private static IWriting CreateWriting(long id, string name, int length, string text)
	{
		var writing = new Mock<IWriting>();
		writing.SetupGet(x => x.Id).Returns(id);
		writing.SetupGet(x => x.Name).Returns(name);
		writing.SetupGet(x => x.FrameworkItemType).Returns("Writing");
		writing.SetupGet(x => x.DocumentLength).Returns(length);
		writing.SetupGet(x => x.ImplementType).Returns(WritingImplementType.Printed);
		writing.Setup(x => x.ParseFor(It.IsAny<ICharacter>())).Returns(text);
		writing.Setup(x => x.DescribeInLook(It.IsAny<ICharacter>())).Returns(text);
		return writing.Object;
	}

	private static IWritingCollection CreateWritingCollection(long id, string name, params (int Page, int Order, ICanBeRead Readable)[] entries)
	{
		var collectionEntries = entries.Select(x =>
		{
			var entry = new Mock<IWritingCollectionEntry>();
			entry.SetupGet(y => y.Page).Returns(x.Page);
			entry.SetupGet(y => y.Order).Returns(x.Order);
			entry.SetupGet(y => y.Readable).Returns(x.Readable);
			return entry.Object;
		}).ToList();
		var collection = new Mock<IWritingCollection>();
		collection.SetupGet(x => x.Id).Returns(id);
		collection.SetupGet(x => x.Name).Returns(name);
		collection.SetupGet(x => x.FrameworkItemType).Returns("WritingCollection");
		collection.SetupGet(x => x.Description).Returns(string.Empty);
		collection.SetupGet(x => x.DefaultTitle).Returns(name);
		collection.SetupGet(x => x.Entries).Returns(collectionEntries);
		collection.SetupGet(x => x.PageCount).Returns(collectionEntries.Select(x => x.Page).DefaultIfEmpty(0).Max());
		collection.Setup(x => x.DocumentLengthForPage(It.IsAny<int>()))
		          .Returns<int>(page => collectionEntries.Where(x => x.Page == page).Sum(x => x.Readable.DocumentLength));
		return collection.Object;
	}

	private static Mock<IFuturemud> CreateGameworld(ILanguage language, IScript script, IColour colour, int pageCapacity, params IWritingCollection[] writingCollections)
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		var writings = new List<IWriting>();
		var drawings = new List<IDrawing>();
		var nextReadableId = 1000L;
		void InitialiseReadable(ILateInitialisingItem item)
		{
			switch (item)
			{
				case PrintedWriting:
					item.SetIDFromDatabase(new DbWriting { Id = nextReadableId++ });
					break;
				case IDrawing:
					item.SetIDFromDatabase(new MudSharp.Models.Drawing { Id = nextReadableId++ });
					break;
			}
		}

		saveManager.Setup(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()))
		           .Callback<ILateInitialisingItem>(InitialiseReadable);
		saveManager.Setup(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()))
		           .Callback<ILateInitialisingItem>(InitialiseReadable);
		var paperProto = CreatePaperProto(gameworld.Object, pageCapacity);
		var paperItemProto = new Mock<IGameItemProto>();
		paperItemProto.SetupGet(x => x.Id).Returns(PaperProtoId);
		paperItemProto.Setup(x => x.GetItemType<PaperSheetGameItemComponentProto>()).Returns(paperProto);

		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.Languages).Returns(Repository(language));
		gameworld.SetupGet(x => x.Scripts).Returns(Repository(script));
		gameworld.SetupGet(x => x.Colours).Returns(Repository(colour));
		gameworld.SetupGet(x => x.Writings).Returns(RepositoryFromList(writings));
		gameworld.SetupGet(x => x.Drawings).Returns(RepositoryFromList(drawings));
		gameworld.SetupGet(x => x.WritingCollections).Returns(Repository(writingCollections));
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository(paperItemProto.Object));
		gameworld.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(ColourId);
		gameworld.Setup(x => x.Add(It.IsAny<IWriting>()))
		         .Callback<IWriting>(writing =>
		         {
			         if (!writings.Contains(writing))
			         {
				         writings.Add(writing);
			         }
		         });
		gameworld.Setup(x => x.Add(It.IsAny<IDrawing>()))
		         .Callback<IDrawing>(drawing =>
		         {
			         if (!drawings.Contains(drawing))
			         {
				         drawings.Add(drawing);
			         }
		         });
		return gameworld;
	}

	private static IGameItem CreateParent(IFuturemud gameworld, long id)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(id);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		return parent.Object;
	}

	private static IGameItem CreateBookItem(IFuturemud gameworld, BookGameItemComponentProto proto, out BookGameItemComponent book)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(500);
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		book = (BookGameItemComponent)proto.CreateNew(item.Object);
		item.Setup(x => x.GetItemType<BookGameItemComponent>()).Returns(book);
		return item.Object;
	}

	private static ILanguage CreateLanguage(long id, string name)
	{
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(id);
		language.SetupGet(x => x.Name).Returns(name);
		return language.Object;
	}

	private static IScript CreateScript(long id, string name, double modifier)
	{
		var script = new Mock<IScript>();
		script.SetupGet(x => x.Id).Returns(id);
		script.SetupGet(x => x.Name).Returns(name);
		script.SetupGet(x => x.DocumentLengthModifier).Returns(modifier);
		return script.Object;
	}

	private static IColour CreateColour(long id, string name)
	{
		var colour = new Mock<IColour>();
		colour.SetupGet(x => x.Id).Returns(id);
		colour.SetupGet(x => x.Name).Returns(name);
		return colour.Object;
	}

	private static IUneditableAll<T> Repository<T>(params T[] items) where T : class, IFrameworkItem
	{
		return RepositoryFromList(items.ToList());
	}

	private static IUneditableAll<T> RepositoryFromList<T>(IList<T> items) where T : class, IFrameworkItem
	{
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.Has(It.IsAny<T>()))
		          .Returns<T>(items.Contains);
		repository.Setup(x => x.Has(It.IsAny<long>()))
		          .Returns<long>(id => items.Any(x => x.Id == id));
		repository.Setup(x => x.Has(It.IsAny<string>()))
		          .Returns<string>(name => items.Any(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>()))
		          .Returns<string>(name => items.FirstOrDefault(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		          .Returns<string, bool>((value, permitAbbreviations) =>
			          long.TryParse(value, out var id)
				          ? items.FirstOrDefault(x => x.Id == id)
				          : items.FirstOrDefault(x => x.Name.EqualTo(value)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => items.GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(() => items.Count);
		return repository.Object;
	}

	private static IUneditableRevisableAll<T> RevisableRepository<T>(params T[] items) where T : class, IRevisableItem
	{
		var repository = new Mock<IUneditableRevisableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<bool>()))
		          .Returns<string, bool>((name, ignoreCase) => items.FirstOrDefault(x =>
			          ignoreCase ? x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) : x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		return repository.Object;
	}

	private sealed class ConstantTestFunction : IFunction
	{
		public ConstantTestFunction(ProgVariableTypes type, object value)
		{
			Result = new ObjectTestVariable(type, value);
			ReturnType = type;
		}

		public IProgVariable Result { get; }
		public ProgVariableTypes ReturnType { get; }
		public StatementResult ExpectedResult => StatementResult.Normal;
		public string ErrorMessage => string.Empty;

		public StatementResult Execute(IVariableSpace variables)
		{
			return StatementResult.Normal;
		}

		public bool IsReturnOrContainsReturnOnAllBranches()
		{
			return false;
		}
	}

	private sealed class ObjectTestVariable : ProgVariable
	{
		private readonly object _value;

		public ObjectTestVariable(ProgVariableTypes type, object value)
		{
			Type = type;
			_value = value;
		}

		public override ProgVariableTypes Type { get; }
		public override object GetObject => _value;

		public override IProgVariable GetProperty(string property)
		{
			throw new NotSupportedException();
		}
	}
}
