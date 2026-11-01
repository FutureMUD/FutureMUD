#nullable enable

namespace MudSharp.CharacterCreation.Screens;

internal static class SkipperProgHelper
{
	public static IFutureProg? Load(IFuturemud gameworld, XElement definition, string name)
	{
		var value = definition.Element(name)?.Value;
		return string.IsNullOrWhiteSpace(value) || value == "0"
			? null
			: gameworld.FutureProgs.GetByIdOrName(value);
	}

	public static bool TryBuildingCommand(IFuturemud gameworld, ICharacter actor, StringStack command,
		ProgVariableTypes returnType, out IFutureProg? prog)
	{
		prog = null;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Specify a prog returning {returnType.Describe().ColourName()} with one chargen parameter, or {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			return true;
		}

		prog = new ProgLookupFromBuilderInput(gameworld, actor, command.SafeRemainingArgument,
			returnType, new[] { ProgVariableTypes.Chargen }).LookupProg();
		return prog is not null;
	}
}
