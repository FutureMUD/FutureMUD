using System.Text.Json;
using MudClientBlazor.Services;

namespace MudClientTests;

public class QuickAliasSettingsTests
{
	[Fact]
	public void CreateDefault_AddsLoginAndTenAliases()
	{
		var settings = QuickAliasSettings.CreateDefault();

		Assert.Equal("Login", settings.Login.Label);
		Assert.Equal(QuickAliasSettings.AliasCount, settings.Aliases.Count);
		Assert.Equal("Alias 1", settings.Aliases[0].Label);
		Assert.Equal("Alias 10", settings.Aliases[^1].Label);
	}

	[Fact]
	public void Normalize_PreservesSavedValuesAndAddsMissingAliases()
	{
		var saved = new QuickAliasSettings
		{
			Login = new LoginAliasSettings
			{
				InitialCommand = "connect",
				Username = "tester",
				Password = "secret"
			},
			Aliases =
			[
				new QuickAliasBinding
				{
					Id = "alias-3",
					Label = "Heal",
					Command = "cast heal self",
					IsEnabled = true
				}
			]
		};

		var normalized = QuickAliasSettings.Normalize(saved);

		Assert.Equal("connect", normalized.Login.InitialCommand);
		Assert.Equal("tester", normalized.Login.Username);
		Assert.Equal("secret", normalized.Login.Password);
		Assert.Equal(QuickAliasSettings.AliasCount, normalized.Aliases.Count);

		var alias = normalized.Aliases.Single(item => item.Id == "alias-3");
		Assert.Equal("Heal", alias.Label);
		Assert.Equal("cast heal self", alias.Command);
		Assert.Contains(normalized.Aliases, item => item.Id == "alias-10");
	}

	[Fact]
	public void CreatePersistentCopy_RemovesLoginPasswordFromLocalStoragePayload()
	{
		var settings = QuickAliasSettings.CreateDefault();
		settings.Login.Password = "secret";

		var json = JsonSerializer.Serialize(
			QuickAliasSettings.CreatePersistentCopy(settings),
			new JsonSerializerOptions(JsonSerializerDefaults.Web));

		Assert.DoesNotContain("secret", json);
		Assert.Contains("password", json, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void LoginCommandSequence_KeepsPasswordAfterInitialCommand()
	{
		var login = new LoginAliasSettings
		{
			InitialCommand = "connect",
			Username = "tester",
			Password = "secret"
		};

		var steps = LoginAliasCommandSequence.Build(login);

		Assert.Collection(
			steps,
			step =>
			{
				Assert.Equal("connect", step.Command);
				Assert.True(step.EchoCommand);
				Assert.True(step.LogCommandText);
				Assert.True(step.AddToHistory);
				Assert.True(step.DelayAfterCommand);
			},
			step =>
			{
				Assert.Equal("tester", step.Command);
				Assert.True(step.EchoCommand);
				Assert.True(step.LogCommandText);
				Assert.True(step.AddToHistory);
				Assert.True(step.DelayAfterCommand);
			},
			step =>
			{
				Assert.Equal("secret", step.Command);
				Assert.False(step.EchoCommand);
				Assert.False(step.LogCommandText);
				Assert.False(step.AddToHistory);
				Assert.False(step.DelayAfterCommand);
			});
	}

	[Fact]
	public void LoginCommandSequence_SkipsBlankPromptStepsButPreservesPassword()
	{
		var login = new LoginAliasSettings
		{
			InitialCommand = " ",
			Username = "\t",
			Password = " secret "
		};

		var step = Assert.Single(LoginAliasCommandSequence.Build(login));
		Assert.Equal(" secret ", step.Command);
		Assert.False(step.EchoCommand);
		Assert.False(step.LogCommandText);
		Assert.False(step.AddToHistory);
		Assert.False(step.DelayAfterCommand);
	}
}
