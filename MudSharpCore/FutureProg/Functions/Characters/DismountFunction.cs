using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class DismountFunction : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "Dismount".ToLowerInvariant(),
                new[] { ProgVariableTypes.Character },
                (pars, gameworld) => new DismountFunction(pars, gameworld),
                new List<string> { "Character" },
                new List<string>
                {
                    "The character who you want to force to dismount",
                },
                "Forceably removes a character from their mount. Does not do any echoes.",
                "Characters",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected DismountFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (character.RidingMount is null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        character.RidingMount.RemoveRider(character);
        character.RidingMount = null;
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
