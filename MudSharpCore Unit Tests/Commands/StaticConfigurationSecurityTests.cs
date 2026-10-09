using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class StaticConfigurationSecurityTests
{
	[DataTestMethod]
	[DataRow("EmailServer")]
	[DataRow("EngineUpdateBinariesPath")]
	[DataRow("DiscordAuthToken")]
	[DataRow("DiscordBotIpAddress")]
	[DataRow("DiscordBotPort")]
	[DataRow("GPT_Secret_Key")]
	[DataRow("Gemini_Secret_Key")]
	[DataRow("Anthropic_API_Key")]
	public void ShowConfig_RestrictedSetting_RedactsForEveryLowerAdministratorRank(string setting)
	{
		foreach (var permission in new[] { PermissionLevel.JuniorAdmin, PermissionLevel.Admin,
			         PermissionLevel.SeniorAdmin, PermissionLevel.HighAdmin })
		{
			var fixture = new Fixture(permission);
			fixture.Show(setting);

			StringAssert.Contains(fixture.Text, setting);
			StringAssert.Contains(fixture.Text, "Redacted for Security".ColourError());
			Assert.IsFalse(fixture.Text.Contains(fixture.Values[setting]));
			fixture.Gameworld.Verify(x => x.GetStaticConfiguration(setting), Times.Never);
		}
	}

	[DataTestMethod]
	[DataRow("EmailServer")]
	[DataRow("EngineUpdateBinariesPath")]
	[DataRow("DiscordAuthToken")]
	[DataRow("DiscordBotIpAddress")]
	[DataRow("DiscordBotPort")]
	[DataRow("GPT_Secret_Key")]
	[DataRow("Gemini_Secret_Key")]
	[DataRow("Anthropic_API_Key")]
	public void EditStaticConfig_RestrictedSetting_HighAdminCannotReadOrEnterEditor(string setting)
	{
		var fixture = new Fixture(PermissionLevel.HighAdmin);
		fixture.Edit(setting);

		StringAssert.Contains(fixture.Text, setting);
		StringAssert.Contains(fixture.Text, "Redacted for Security".ColourError());
		Assert.IsFalse(fixture.Text.Contains(fixture.Values[setting]));
		Assert.IsNull(fixture.PostAction);
		fixture.Gameworld.Verify(x => x.GetStaticConfiguration(setting), Times.Never);
		fixture.Gameworld.Verify(x => x.UpdateStaticConfiguration(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("EmailServer")]
	[DataRow("EngineUpdateBinariesPath")]
	[DataRow("DiscordAuthToken")]
	[DataRow("DiscordBotIpAddress")]
	[DataRow("DiscordBotPort")]
	[DataRow("GPT_Secret_Key")]
	[DataRow("Gemini_Secret_Key")]
	[DataRow("Anthropic_API_Key")]
	public void RestrictedSetting_Implementor_CanViewAndRecallValueForEditing(string setting)
	{
		var fixture = new Fixture(PermissionLevel.Founder);
		fixture.Show(setting);
		StringAssert.Contains(fixture.Text, fixture.Values[setting]);
		Assert.IsFalse(fixture.Text.Contains("Redacted for Security"));

		fixture.Edit(setting);
		Assert.AreEqual(fixture.Values[setting], fixture.Recall);
		Assert.IsNotNull(fixture.PostAction);
		Assert.AreEqual(setting, fixture.Arguments![0]);
		Assert.AreSame(fixture.Gameworld.Object, fixture.Arguments[1]);
		Assert.AreSame(fixture.Actor.Object, fixture.Arguments[2]);
	}

	[DataTestMethod]
	[DataRow(PermissionLevel.HighAdmin)]
	[DataRow(PermissionLevel.Founder)]
	public void EditStaticConfig_List_RedactsOnlyRestrictedValuesForLowerRanks(PermissionLevel permission)
	{
		var fixture = new Fixture(permission);
		fixture.Edit();

		foreach (var setting in fixture.Values.Keys.Where(x => x != "UseDiscordBot"))
		{
			StringAssert.Contains(fixture.Text, setting);
			Assert.AreEqual(permission == PermissionLevel.Founder, fixture.Text.Contains(fixture.Values[setting]));
			fixture.Gameworld.Verify(x => x.GetStaticConfiguration(setting),
				permission == PermissionLevel.Founder ? Times.AtLeastOnce() : Times.Never());
		}

		StringAssert.Contains(fixture.Text, "UseDiscordBot");
		StringAssert.Contains(fixture.Text, "true");
		Assert.AreEqual(permission != PermissionLevel.Founder, fixture.Text.Contains("Redacted for Security"));
		if (permission != PermissionLevel.Founder)
		{
			StringAssert.Contains(fixture.Text, "Redacted for Security".ColourError());
		}
	}

	[TestMethod]
	public void OrdinarySetting_HighAdmin_CanStillViewAndEnterEditor()
	{
		var fixture = new Fixture(PermissionLevel.HighAdmin);
		fixture.Show("UseDiscordBot");
		StringAssert.Contains(fixture.Text, "true");
		fixture.Edit("UseDiscordBot");

		Assert.AreEqual("true", fixture.Recall);
		Assert.IsNotNull(fixture.PostAction);
		Assert.IsFalse(fixture.Text.Contains("Redacted for Security"));
	}

	[TestMethod]
	public void RestrictedSetting_CaseVariantsInInputAndStoredName_CannotBypassRestriction()
	{
		var fixture = new Fixture(PermissionLevel.HighAdmin);
		fixture.Gameworld.SetupGet(x => x.StaticConfigurationNames).Returns(new[] { "gpt_secret_key" });
		fixture.Show("GpT_SeCrEt_KeY");
		fixture.Edit("GpT_SeCrEt_KeY");

		StringAssert.Contains(fixture.Text, "gpt_secret_key");
		StringAssert.Contains(fixture.Text, "Redacted for Security".ColourError());
		Assert.IsNull(fixture.PostAction);
		fixture.Gameworld.Verify(x => x.GetStaticConfiguration(It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void EditStaticConfig_SubmitAfterImplementorPermissionRevoked_DoesNotSave()
	{
		var fixture = new Fixture(PermissionLevel.Founder);
		fixture.Edit("GPT_Secret_Key");
		Assert.IsNotNull(fixture.PostAction);
		fixture.Actor.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.HighAdmin);

		fixture.PostAction!("replacement-secret", fixture.Output.Object, fixture.Arguments!);

		StringAssert.Contains(fixture.Text, "Your change was not saved.");
		fixture.Gameworld.Verify(x => x.UpdateStaticConfiguration(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	private sealed class Fixture
	{
		public Dictionary<string, string> Values { get; } = new()
		{
			["EmailServer"] = "secret-email-config",
			["EngineUpdateBinariesPath"] = "owner-update-staging-path",
			["DiscordAuthToken"] = "secret-discord-token",
			["DiscordBotIpAddress"] = "secret-discord-host",
			["DiscordBotPort"] = "12345",
			["GPT_Secret_Key"] = "secret-openai-key",
			["Gemini_Secret_Key"] = "secret-gemini-key",
			["Anthropic_API_Key"] = "secret-anthropic-key",
			["UseDiscordBot"] = "true"
		};
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IFuturemud> Gameworld { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public List<string> Messages { get; } = new();
		public string Text => string.Join("\n", Messages);
		public Action<string, IOutputHandler, object[]>? PostAction { get; private set; }
		public object[]? Arguments { get; private set; }
		public string? Recall { get; private set; }

		public Fixture(PermissionLevel permission)
		{
			Actor.SetupGet(x => x.PermissionLevel).Returns(permission);
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
			Actor.SetupGet(x => x.Gameworld).Returns(Gameworld.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.LineFormatLength).Returns(120);
			Actor.SetupGet(x => x.Account.LineFormatLength).Returns(120);
			Gameworld.SetupGet(x => x.StaticConfigurationNames).Returns(Values.Keys);
			Gameworld.Setup(x => x.GetStaticConfiguration(It.IsAny<string>()))
				.Returns((string setting) => Values[setting]);
			Output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Messages.Add(text))
				.Returns(true);
			Actor.Setup(x => x.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(),
					It.IsAny<Action<IOutputHandler, object[]>>(), It.IsAny<double>(), It.IsAny<string?>(),
					It.IsAny<EditorOptions>(), It.IsAny<object[]?>()))
				.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double,
					string?, EditorOptions, object[]?>((post, _, _, recall, _, args) =>
					{
						PostAction = post;
						Recall = recall;
						Arguments = args;
					});
		}

		public void Show(string setting) => typeof(ShowModule)
			.GetMethod("Show", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, new object[] { Actor.Object, $"show config {setting}" });

		public void Edit(string setting = "") => typeof(StaffModule)
			.GetMethod("EditStaticConfiguration", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, new object[] { Actor.Object, $"editstaticconfig {setting}" });
	}
}
