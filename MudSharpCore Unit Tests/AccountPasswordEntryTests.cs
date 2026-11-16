#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Menus;
using MudSharp.Network;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AccountPasswordEntryTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RegistrationEmail_NoneIsAllowedOnlyWhenEmailRegistrationIsDisabled(bool disabled)
	{
		var output = new Mock<IOutputHandler>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Name).Returns("Test MUD");
		world.Setup(x => x.GetStaticString(It.IsAny<string>())).Returns("Test menu");
		world.Setup(x => x.GetStaticBool("DisableAccountRegistrationByEmail")).Returns(disabled);
		var menu = new MainMenu(world.Object, new Mock<IPlayerController>().Object);
		var controller = new Mock<IController>();
		controller.SetupGet(x => x.OutputHandler).Returns(output.Object);
		menu.SilentAssumeControl(controller.Object);
		var state = typeof(MainMenu).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!;
		state.SetValue(menu, Enum.Parse(state.FieldType, "CreateAccountEmail"));
		menu.ExecuteCommand("none");
		Assert.AreEqual(disabled ? "CreateAccountLineWidth" : "CreateAccountEmail", state.GetValue(menu)!.ToString());
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("1234567")]
	[DataRow("a sufficiently long password")]
	public void RegistrationPasswordEntry_RequiresTheSharedMinimum(string password)
	{
		var output = new Mock<IOutputHandler>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Name).Returns("Test MUD");
		world.Setup(x => x.GetStaticString(It.IsAny<string>())).Returns("Test menu");
		var menu = new MainMenu(world.Object, new Mock<IPlayerController>().Object);
		var controller = new Mock<IController>();
		controller.SetupGet(x => x.OutputHandler).Returns(output.Object);
		menu.SilentAssumeControl(controller.Object);
		var state = typeof(MainMenu).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!;
		state.SetValue(menu, Enum.Parse(state.FieldType, "CreateAccountPassword"));

		menu.ExecuteCommand(password);

		Assert.AreEqual(SecurityUtilities.IsValidNewPassword(password) ? "CreateAccountPasswordConfirm" : "CreateAccountPassword",
			state.GetValue(menu)!.ToString());
		var hash = typeof(MainMenu).GetField("_accountPassword", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(menu) as string;
		Assert.AreEqual(SecurityUtilities.IsValidNewPassword(password), hash is not null);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("1234567")]
	[DataRow("a sufficiently long password")]
	public void ChangeAndRecoveryPasswordEntry_RejectShortPasswordsBeforeConfirmation(string password)
	{
		// The ordinary constructor queries chargen counts. Exercise just the real password-entry
		// handler shared by password changes and recovery, without a database/session substitute.
		var menu = (LoggedInMenu)RuntimeHelpers.GetUninitializedObject(typeof(LoggedInMenu));
		menu.State = LoggedInMenu.LoggedInMenuState.CheckedOldPassword;
		var messages = new List<string>();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => messages.Add(text));
		var controller = new Mock<IController>();
		controller.SetupGet(x => x.OutputHandler).Returns(output.Object);
		menu.SilentAssumeControl(controller.Object);

		typeof(LoggedInMenu).GetMethod("EnterNewPassword", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(menu, [password]);

		Assert.AreEqual(SecurityUtilities.IsValidNewPassword(password)
			? LoggedInMenu.LoggedInMenuState.EnteredNewPassword
			: LoggedInMenu.LoggedInMenuState.CheckedOldPassword, menu.State);
		var hash = typeof(LoggedInMenu).GetField("_newPasswordHash", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(menu) as string;
		Assert.AreEqual(SecurityUtilities.IsValidNewPassword(password), hash is not null);
		if (!SecurityUtilities.IsValidNewPassword(password))
		{
			StringAssert.Contains(string.Join("\n", messages), "at least 15 characters");
		}
	}
}
