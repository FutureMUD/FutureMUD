using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ConvertToCharacterFunction : BuiltInFunction
{
    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tocharacter",
            new[] { ProgVariableTypes.Perceiver },
            (pars, gameworld) => new ConvertToCharacterFunction(pars),
            ["character"],
            ["The perceiver to convert to a character."],
            "Converts a perceiver to a character, if possible. Otherwise returns null.",
            "Conversion",
            ProgVariableTypes.Character
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tocharacter",
            new[] { ProgVariableTypes.Perceivable },
            (pars, gameworld) => new ConvertToCharacterFunction(pars),
            ["character"],
            ["The perceivable to convert to a character."],
            "Converts a perceivable to a character, if possible. Otherwise returns null.",
            "Conversion",
            ProgVariableTypes.Character
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tocharacter",
            new[] { ProgVariableTypes.Toon },
            (pars, gameworld) => new ConvertToCharacterFunction(pars),
            ["character"],
            ["The toon to convert to a character."],
            "Converts a toon to a character, if possible. Otherwise returns null.",
            "Conversion",
            ProgVariableTypes.Character
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tocharacter",
            new[] { ProgVariableTypes.CollectionItem },
            (pars, gameworld) => new ConvertToCharacterFunction(pars),
            ["character"],
            ["The collection item to convert to a character."],
            "Converts a collection item to a character, if possible. Otherwise returns null.",
            "Conversion",
            ProgVariableTypes.Character
        ));
    }
    protected ConvertToCharacterFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Character;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        Result = ParameterFunctions[0].Result as ICharacter;
        return StatementResult.Normal;
    }
}
