#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ManipulationModuleParsingTests
{
	private static bool IsTestAmount(string text)
	{
		return text.Equals("10ml", StringComparison.OrdinalIgnoreCase) ||
		       text.Equals("30ml", StringComparison.OrdinalIgnoreCase) ||
		       text.Equals("50ml", StringComparison.OrdinalIgnoreCase);
	}

	private static T GetProperty<T>(object target, string property)
	{
		return (T)target.GetType()
		                .GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
		                .GetValue(target)!;
	}

	private static object ParseDrink(string text)
	{
		var method = typeof(ManipulationModule).GetMethod("TryParseDrinkCommand",
			BindingFlags.Static | BindingFlags.NonPublic)!;
		var arguments = new object?[] { text, new Func<string, bool>(IsTestAmount), null };

		Assert.IsTrue((bool)method.Invoke(null, arguments)!);
		return arguments[2]!;
	}

	private static object ParseFill(string text, bool allowOwner = true)
	{
		var method = typeof(ManipulationModule).GetMethod("TryParseFillCommand",
			BindingFlags.Static | BindingFlags.NonPublic)!;
		var arguments = new object?[] { text, new Func<string, bool>(IsTestAmount), allowOwner, null };

		Assert.IsTrue((bool)method.Invoke(null, arguments)!);
		return arguments[3]!;
	}

	private static object ParsePour(string text)
	{
		var method = typeof(ManipulationModule).GetMethod("TryParsePourCommand",
			BindingFlags.Static | BindingFlags.NonPublic)!;
		var arguments = new object?[] { text, null };

		Assert.IsTrue((bool)method.Invoke(null, arguments)!);
		return arguments[1]!;
	}

	[TestMethod]
	public void TryParseDrinkCommand_AmountFirstWithTableAndEmote_Parses()
	{
		var parsed = ParseDrink("30ml bottle on table (carefully)");

		Assert.AreEqual("bottle", GetProperty<string>(parsed, "Target"));
		Assert.AreEqual("30ml", GetProperty<string>(parsed, "Amount"));
		Assert.AreEqual("table", GetProperty<string>(parsed, "Table"));
		Assert.AreEqual("carefully", GetProperty<string>(parsed, "Emote"));
	}

	[TestMethod]
	public void TryParseDrinkCommand_TargetFirstWithAmountAndTable_Parses()
	{
		var parsed = ParseDrink("bottle 30ml on table");

		Assert.AreEqual("bottle", GetProperty<string>(parsed, "Target"));
		Assert.AreEqual("30ml", GetProperty<string>(parsed, "Amount"));
		Assert.AreEqual("table", GetProperty<string>(parsed, "Table"));
		Assert.AreEqual(string.Empty, GetProperty<string>(parsed, "Emote"));
	}

	[TestMethod]
	public void TryParseFillCommand_DirectSourceWithAmount_Parses()
	{
		var parsed = ParseFill("vial flask 10ml");

		Assert.AreEqual("vial", GetProperty<string>(parsed, "Target"));
		Assert.AreEqual(string.Empty, GetProperty<string>(parsed, "Owner"));
		Assert.AreEqual("flask", GetProperty<string>(parsed, "From"));
		Assert.AreEqual("10ml", GetProperty<string>(parsed, "Amount"));
	}

	[TestMethod]
	public void TryParseFillCommand_OwnerSourceWithoutAmount_Parses()
	{
		var parsed = ParseFill("vial bob flask");

		Assert.AreEqual("vial", GetProperty<string>(parsed, "Target"));
		Assert.AreEqual("bob", GetProperty<string>(parsed, "Owner"));
		Assert.AreEqual("flask", GetProperty<string>(parsed, "From"));
		Assert.AreEqual(string.Empty, GetProperty<string>(parsed, "Amount"));
	}

	[TestMethod]
	public void TryParseFillCommand_OwnerSourceWithAmount_Parses()
	{
		var parsed = ParseFill("vial bob flask 10ml");

		Assert.AreEqual("vial", GetProperty<string>(parsed, "Target"));
		Assert.AreEqual("bob", GetProperty<string>(parsed, "Owner"));
		Assert.AreEqual("flask", GetProperty<string>(parsed, "From"));
		Assert.AreEqual("10ml", GetProperty<string>(parsed, "Amount"));
	}

	[TestMethod]
	public void TryParsePourCommand_NoAmount_Parses()
	{
		var parsed = ParsePour("bottle into cup");

		Assert.AreEqual("bottle", GetProperty<string>(parsed, "From"));
		Assert.AreEqual("cup", GetProperty<string>(parsed, "Into"));
		Assert.AreEqual(string.Empty, GetProperty<string>(parsed, "Amount"));
	}

	[TestMethod]
	public void TryParsePourCommand_WithAmount_Parses()
	{
		var parsed = ParsePour("50ml bottle into cup");

		Assert.AreEqual("bottle", GetProperty<string>(parsed, "From"));
		Assert.AreEqual("cup", GetProperty<string>(parsed, "Into"));
		Assert.AreEqual("50ml", GetProperty<string>(parsed, "Amount"));
	}

	[TestMethod]
	public void GetTargetLock_UnnamedLockableTarget_ReturnsFirstLock()
	{
		var actor = new Mock<ICharacter>();
		var target = new Mock<IGameItem>();
		var lockable = new Mock<ILockable>();
		var targetLock = new Mock<ILock>();
		var lockItem = new Mock<IGameItem>();

		target.Setup(x => x.IsItemType<ILockable>()).Returns(true);
		target.Setup(x => x.GetItemType<ILockable>()).Returns(lockable.Object);
		target.Setup(x => x.IsItemType<ILock>()).Returns(false);
		lockable.SetupGet(x => x.Locks).Returns([targetLock.Object]);
		targetLock.SetupGet(x => x.Parent).Returns(lockItem.Object);
		lockItem.Setup(x => x.GetItemType<ILock>()).Returns(targetLock.Object);

		var method = typeof(ManipulationModule).GetMethod("GetTargetLock",
			BindingFlags.Static | BindingFlags.NonPublic)!;
		var result = method.Invoke(null, [actor.Object, target.Object, string.Empty]);

		Assert.AreSame(targetLock.Object, result);
	}
}
