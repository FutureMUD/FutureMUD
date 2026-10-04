#nullable enable

using MudSharp.Character;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class CanCommandAnimation(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(
		new FunctionCompilerInformation("cancommandanimation", [ProgVariableTypes.Character, ProgVariableTypes.Character],
			(pars, _) => new CanCommandAnimation(pars), ["animation", "commander"],
			["The exact animated corpse instance", "The character attempting to command it"],
			"True only for the creator's canonical identity while this durable animation has an active, unexpired command grant.",
			"Magic", ProgVariableTypes.Boolean));

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var animation = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var commander = ParameterFunctions[1].Result?.GetObject as ICharacter;
		Result = new BooleanVariable(animation is not null && commander is not null &&
			ReferenceEquals(animation.Gameworld, commander.Gameworld) &&
			animation.Gameworld.SpellOwnedCorpseAnimations?.CanCommand(animation.InstanceId,
				CharacterInstanceIdentityComparer.IdentityId(commander)) == true);
		return StatementResult.Normal;
	}
}
