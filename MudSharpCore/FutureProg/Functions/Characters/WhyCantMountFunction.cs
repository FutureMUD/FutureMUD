using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class WhyCantMountFunction : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "WhyCantMount".ToLowerInvariant(),
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Character },
                (pars, gameworld) => new WhyCantMountFunction(pars, gameworld),
                new List<string> { "Character", "Mount" },
                new List<string>
                {
                    "The character who you want to be the rider",
                    "The mount you want them to mount"
                },
                "Returns an error message about why a character cannot mount the mount.",
                "Characters",
                ProgVariableTypes.Boolean
            )
        );
    }

    #endregion

    #region Constructors

    protected WhyCantMountFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
            Result = new TextVariable($"The rider is not a character or is null.");
            return StatementResult.Normal;
        }

        if (character.RidingMount is not null)
        {
            Result = new TextVariable($"The rider is already riding a mount.");
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].Result?.GetObject is not ICharacter mount)
        {
            Result = new TextVariable($"The target is not a character or is null.");
            return StatementResult.Normal;
        }

        if (mount.CanBeMountedBy(character))
        {
            Result = new TextVariable("The character can ride the mount.");
            return StatementResult.Normal;
        }

        Result = new TextVariable(mount.WhyCannotBeMountedBy(character));
        return StatementResult.Normal;
    }
}