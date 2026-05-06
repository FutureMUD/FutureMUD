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
	public void BookPrototypeXml_CreatesDefaultTitleAndIndependentPrintedContent()
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
		Assert.AreNotSame(firstBook.PagesAndReadables[0].Writing, secondBook.PagesAndReadables[0].Writing);
		Assert.AreEqual("Read this first.", firstBook.PagesAndReadables[0].Writing.ParseFor(null));
		Assert.AreEqual("Training Press", ((IWriting)firstBook.PagesAndReadables[0].Writing).GetProperty("provenance").GetObject);
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
		Assert.AreNotSame(printed, copied);
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

	private static string BookDefinition(int pageCount, string title, params XElement[] writings)
	{
		return new XElement("Definition",
			new XElement("PaperProto", PaperProtoId),
			new XElement("PageCount", pageCount),
			new XElement("DefaultTitle", new XCData(title)),
			new XElement("InitialReadables", writings)
		).ToString();
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

	private static Mock<IFuturemud> CreateGameworld(ILanguage language, IScript script, IColour colour, int pageCapacity)
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		var paperProto = CreatePaperProto(gameworld.Object, pageCapacity);
		var paperItemProto = new Mock<IGameItemProto>();
		paperItemProto.SetupGet(x => x.Id).Returns(PaperProtoId);
		paperItemProto.Setup(x => x.GetItemType<PaperSheetGameItemComponentProto>()).Returns(paperProto);

		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.Languages).Returns(Repository(language));
		gameworld.SetupGet(x => x.Scripts).Returns(Repository(script));
		gameworld.SetupGet(x => x.Colours).Returns(Repository(colour));
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository(paperItemProto.Object));
		gameworld.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(ColourId);
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
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>()))
		          .Returns<string>(name => items.FirstOrDefault(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(items.Length);
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
