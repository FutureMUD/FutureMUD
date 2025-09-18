using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class MountFunction : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "Mount".ToLowerInvariant(),
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Character },
                (pars, gameworld) => new MountFunction(pars, gameworld),
                new List<string> { "Character", "Mount" },
                new List<string>
                {
                    "The character who you want to force to mount",
                    "The mount you want them to mount"
                },
                "Makes a character mount an available mount. Returns true if successful. Does handle echoes.",
                "Characters",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected MountFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

        if (character.RidingMount is not null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].Result?.GetObject is not ICharacter mount)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (!mount.CanBeMountedBy(character))
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        mount.Mount(character);
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
