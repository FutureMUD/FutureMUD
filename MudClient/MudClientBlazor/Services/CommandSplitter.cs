using System.Text;

namespace MudClientBlazor.Services;

public static class CommandSplitter
{
	public static IReadOnlyList<string> Split(string input, ClientSettings settings)
	{
		ArgumentNullException.ThrowIfNull(input);
		ArgumentNullException.ThrowIfNull(settings);

		var commands = new List<string>();
		var currentCommand = new StringBuilder();
		var delimiter = settings.CommandStackingDelimiter.FirstOrDefault();
		var pendingEscape = false;

		for (var i = 0; i < input.Length; i++)
		{
			var character = input[i];

			if (pendingEscape)
			{
				if (character == delimiter || character == '\\')
				{
					currentCommand.Append(character);
					pendingEscape = false;
					continue;
				}

				currentCommand.Append('\\');
				pendingEscape = false;
			}

			if (settings.SemicolonCommandStackingEnabled && character == delimiter)
			{
				AddCommand(commands, currentCommand);
				continue;
			}

			if (settings.NewlineCommandStackingEnabled && character is '\r' or '\n')
			{
				AddCommand(commands, currentCommand);
				if (character == '\r' && i + 1 < input.Length && input[i + 1] == '\n')
				{
					i++;
				}

				continue;
			}

			if (settings.SemicolonCommandStackingEnabled && character == '\\')
			{
				pendingEscape = true;
				continue;
			}

			currentCommand.Append(character);
		}

		if (pendingEscape)
		{
			currentCommand.Append('\\');
		}

		AddCommand(commands, currentCommand);
		return commands;
	}

	private static void AddCommand(ICollection<string> commands, StringBuilder currentCommand)
	{
		var command = currentCommand.ToString().Trim();
		if (!string.IsNullOrWhiteSpace(command))
		{
			commands.Add(command);
		}

		currentCommand.Clear();
	}
}
