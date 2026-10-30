#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Magic.Casting;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class IsMagicIncantationFunction(IList<IFunction> parameters, IFuturemud world, bool commandInput = false) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		if (actor is null) { Result = new BooleanVariable(false); return StatementResult.Normal; }
		var text = ParameterFunctions[1].Result?.GetObject?.ToString() ?? "";
		Result = new BooleanVariable(commandInput ? IsCastingCommand(actor, text) : world.MagicCasting?.RecognisesFormula(actor, text) == true);
		return StatementResult.Normal;
	}

	private bool IsCastingCommand(ICharacter actor, string text)
	{
		var input = new StringStack(text);
		var verb = input.PopSpeech();
		if (verb.EqualTo("cast") && !world.MagicSchools.Any(x => x.SchoolVerb.EqualTo("cast"))) return true;
		// Resolve using the same command tree and abbreviation order as normal execution. Never execute it.
		var disposable = text;
		var command = actor.CommandTree?.Commands.LocateCommand(actor, ref disposable);
		if (command is null) return false;
		if (world.MagicSchools.Any(x => x.SchoolVerb.EqualTo(command.Name)))
		{
			var argument = input.PopSpeech();
			return argument.EqualToAny("cast", "quiet", "formula", "area", "practice") ||
				Commands.Modules.MagicModule.IsImplicitCastCommand(command.Name, argument);
		}
		if (!command.Name.EqualToAny("Say", "Tell", "Talk", "Whisper", "LoudSay", "Yell", "Shout", "Sing", "SingTo")) return false;
		// Some directed forms have their own command, others branch on the original alias in the native handler.
		if (command.Name.EqualToAny("Tell", "SingTo") || verb.EqualToAny("talkto", "whisperto", "loudtell", "yellat", "shoutat")) input.PopSpeech();
		input.PopParentheses();
		return world.MagicCasting?.RecognisesFormula(actor, input.RemainingArgument) == true;
	}
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("ismagicincantation", [ProgVariableTypes.Character, ProgVariableTypes.Text],
			(parameters, world) => new IsMagicIncantationFunction(parameters, world), ["actor", "formula"],
			["The speaker, whose selected language is used.", "The complete formula text."],
			"Pure recognition of an authored formula or power/alias in the selected language. Does not acquire, charge, roll, progress or cast.", "Magic", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new("ismagiccastingcommand", [ProgVariableTypes.Character, ProgVariableTypes.Text],
			(parameters, world) => new IsMagicIncantationFunction(parameters, world, true), ["actor", "input"],
			["The speaker.", "The complete original command input, retaining quotation boundaries."],
			"Pure recognition of native school casting commands, legacy cast, and spoken incantations. Resolves native abbreviations without executing commands; ordinary dialogue and school help/status return false.", "Magic", ProgVariableTypes.Boolean));
	}
}
