#nullable enable

using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SetRegisterCommandTests
{
	private Mock<ICharacter> _actor = null!;
	private Mock<IVariableRegister> _register = null!;
	private Mock<ICell> _cell = null!;

	[TestInitialize]
	public void Initialise()
	{
		_actor = new Mock<ICharacter>();
		_register = new Mock<IVariableRegister>();
		_cell = new Mock<ICell> { DefaultValue = DefaultValue.Mock };
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.VariableRegister).Returns(_register.Object);
		world.SetupGet(x => x.Cells).Returns(new Mock<IUneditableAll<ICell>>().Object);
		_actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		_actor.SetupGet(x => x.Location).Returns(_cell.Object);
		_actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		_cell.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		_cell.SetupGet(x => x.GetObject).Returns(_cell.Object);
		_cell.SetupGet(x => x.CurrentOverlay.CellName).Returns("Test Room");
		_cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns("Test Room (#1)");
		_register.Setup(x => x.SetValue(_cell.Object, It.IsAny<string>(), It.IsAny<IProgVariable>())).Returns(true);
	}

	[DataTestMethod]
	[DataRow("25", "number", "25")]
	[DataRow("The gate is closed.", "text", "The gate is closed.")]
	[DataRow("\"The gate is closed.\"", "text", "The gate is closed.")]
	[DataRow("\"\"", "text", "")]
	[DataRow("true", "boolean", "True")]
	public void SetRegister_ScalarValue_UsesDeclaredType(string input, string typeName, string expected)
	{
		Declare(FutureProg.GetTypeByName(typeName));
		Run($"setregister location here \"Test Variable\" {input}");
		_register.Verify(x => x.SetValue(_cell.Object, "test variable",
			It.Is<IProgVariable>(v => v.GetObject.ToString() == expected)), Times.Once);
	}

	[DataTestMethod]
	[DataRow("(\"first value\" \"second value\")")]
	[DataRow("\"first value\" \"second value\"")]
	public void SetRegister_TextCollection_PreservesElementQuotes(string input)
	{
		Declare(ProgVariableTypes.Text | ProgVariableTypes.Collection);
		IProgVariable? stored = null;
		_register.Setup(x => x.SetValue(_cell.Object, "test variable", It.IsAny<IProgVariable>()))
			.Callback<IProgVariable, string, IProgVariable>((_, _, value) => stored = value).Returns(true);
		Run($"setregister location here \"test variable\" {input}");
		Assert.IsNotNull(stored);
		var elements = ((IList)stored.GetObject).Cast<IProgVariable>().Select(x => (string)x.GetObject).ToArray();
		CollectionAssert.AreEqual(new[] { "first value", "second value" }, elements);
	}

	[TestMethod]
	public void SetRegister_DictionaryValue_ParsesTypedEntries()
	{
		Declare(ProgVariableTypes.Text | ProgVariableTypes.Dictionary);
		Run("setregister location here \"test variable\" (key \"value with spaces\")");
		_register.Verify(x => x.SetValue(_cell.Object, "test variable",
			It.Is<IProgVariable>(v => ((IProgVariable)((IDictionary)v.GetObject)["key"]!).GetObject.Equals("value with spaces"))), Times.Once);
	}

	[DataTestMethod]
	[DataRow("RegisterDefault")]
	[DataRow("CellSetRegister")]
	public void RegisterCommands_Collections_PreserveQuotedElements(string method)
	{
		Declare(ProgVariableTypes.Text | ProgVariableTypes.Collection);
		var declaringType = method == "RegisterDefault" ? typeof(ProgModule) : typeof(RoomBuilderModule);
		var input = new StringStack($"{(method == "RegisterDefault" ? "location " : "")}\"test variable\" (\"one value\" two)");
		declaringType.GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, new object[] { _actor.Object, input });
		if (method == "RegisterDefault")
		{
			_register.Verify(x => x.SetDefaultValue(ProgVariableTypes.Location, "test variable",
				It.Is<IProgVariable>(v => ((IProgVariable)((IList)v.GetObject)[0]!).GetObject.Equals("one value"))), Times.Once);
		}
		else
		{
			_register.Verify(x => x.SetValue(_cell.Object, "test variable",
				It.Is<IProgVariable>(v => ((IProgVariable)((IList)v.GetObject)[0]!).GetObject.Equals("one value"))), Times.Once);
		}
	}

	[TestMethod]
	public void SetRegister_ReferenceValue_ResolvesExistingObject()
	{
		Declare(ProgVariableTypes.Location);
		Run("setregister location here \"test variable\" here");
		_register.Verify(x => x.SetValue(_cell.Object, "test variable", _cell.Object), Times.Once);
	}

	[TestMethod]
	public void SetRegister_MissingReferenceValue_DoesNotStoreImplicitNull()
	{
		Declare(ProgVariableTypes.Location);
		Run("setregister location here \"test variable\" 999999");
		_register.Verify(x => x.SetValue(It.IsAny<IProgVariable>(), It.IsAny<string>(), It.IsAny<IProgVariable>()), Times.Never);
	}

	[TestMethod]
	public void SetRegister_NullReferenceValue_PassesTypedNullToPersistence()
	{
		Declare(ProgVariableTypes.Location);
		Run("setregister location here \"test variable\" null");
		_register.Verify(x => x.SetValue(_cell.Object, "test variable",
			It.Is<IProgVariable>(v => v.Type == ProgVariableTypes.Location && v.GetObject == null)), Times.Once);
	}

	[DataTestMethod]
	[DataRow("setregister location null \"test variable\" 25")]
	[DataRow("setregister location 999999 \"test variable\" 25")]
	[DataRow("setregister location here unknown 25")]
	[DataRow("setregister location here \"test variable\" invalid")]
	[DataRow("setregister location here \"test variable\"")]
	[DataRow("setregister number 1 \"test variable\" 25")]
	[DataRow("setregister nonsense here \"test variable\" 25")]
	public void SetRegister_InvalidInput_DoesNotWrite(string input)
	{
		Declare(ProgVariableTypes.Number);
		Run(input);
		_register.Verify(x => x.SetValue(It.IsAny<IProgVariable>(), It.IsAny<string>(), It.IsAny<IProgVariable>()), Times.Never);
	}

	private void Declare(ProgVariableTypes type)
	{
		_register.Setup(x => x.IsRegistered(ProgVariableTypes.Location, "test variable")).Returns(true);
		_register.Setup(x => x.GetType(ProgVariableTypes.Location, "test variable")).Returns(type);
	}

	private void Run(string input)
	{
		typeof(ProgModule).GetMethod("SetRegister", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, new object[] { _actor.Object, input });
	}
}
