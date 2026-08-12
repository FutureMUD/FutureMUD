#nullable enable

namespace MudSharp.Email;

internal interface IEmailSecretResolver
{
	bool TryResolve(string reference, out string secret, out string error);
}

internal sealed class EnvironmentEmailSecretResolver : IEmailSecretResolver
{
	public bool TryResolve(string reference, out string secret, out string error)
	{
		secret = string.Empty;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(reference) || !reference.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
		{
			error = "Email secrets must use an environment-variable reference in the form env:VARIABLE_NAME.";
			return false;
		}

		var variableName = reference[4..].Trim();
		if (string.IsNullOrWhiteSpace(variableName))
		{
			error = "Email secret environment-variable reference is missing its variable name.";
			return false;
		}

		var value = Environment.GetEnvironmentVariable(variableName);
		if (string.IsNullOrWhiteSpace(value))
		{
			error = $"Email secret environment variable {variableName} is not set.";
			return false;
		}

		secret = value;
		return true;
	}
}
