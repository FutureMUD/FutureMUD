namespace MudClientBlazor.Services;

public sealed record LoginAliasCommandStep(
	string Command,
	bool EchoCommand,
	bool LogCommandText,
	bool AddToHistory,
	bool DelayAfterCommand);

public static class LoginAliasCommandSequence
{
	public static IReadOnlyList<LoginAliasCommandStep> Build(LoginAliasSettings login)
	{
		ArgumentNullException.ThrowIfNull(login);

		var steps = new List<LoginAliasCommandStep>(3);
		if (!string.IsNullOrWhiteSpace(login.InitialCommand))
		{
			steps.Add(new LoginAliasCommandStep(
				login.InitialCommand,
				EchoCommand: true,
				LogCommandText: true,
				AddToHistory: true,
				DelayAfterCommand: false));
		}

		if (!string.IsNullOrWhiteSpace(login.Username))
		{
			steps.Add(new LoginAliasCommandStep(
				login.Username,
				EchoCommand: true,
				LogCommandText: true,
				AddToHistory: true,
				DelayAfterCommand: false));
		}

		if (!string.IsNullOrEmpty(login.Password))
		{
			steps.Add(new LoginAliasCommandStep(
				login.Password,
				EchoCommand: false,
				LogCommandText: false,
				AddToHistory: false,
				DelayAfterCommand: false));
		}

		for (var i = 0; i < steps.Count - 1; i++)
		{
			steps[i] = steps[i] with { DelayAfterCommand = true };
		}

		return steps;
	}
}
