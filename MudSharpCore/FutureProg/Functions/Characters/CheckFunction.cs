using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;

namespace MudSharp.FutureProg.Functions.Characters;

internal class CheckFunction : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    protected CheckFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        Gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Number;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            Result = new NullVariable(ProgVariableTypes.Number);
            return StatementResult.Error;
        }

        if (ParameterFunctions[0].Result is not ICharacter character)
        {
            Result = new NumberVariable(0);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].Result is not ITraitDefinition trait)
        {
            Result = new NumberVariable(0);
            return StatementResult.Normal;
        }

        var difficultyNum = (int)(decimal)ParameterFunctions[2].Result.GetObject;
        if (difficultyNum < (int)Difficulty.Automatic)
        {
            difficultyNum = (int)Difficulty.Automatic;
        }
        else if (difficultyNum > (int)Difficulty.Impossible)
        {
            difficultyNum = (int)Difficulty.Impossible;
        }

        var difficulty = (Difficulty)difficultyNum;
        Result = new NumberVariable(Gameworld.GetCheck(CheckType.ProgSkillUseCheck).Check(character, difficulty, trait)
                                             .CheckDegrees());
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "check",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number
                },
                (pars, gameworld) => new CheckFunction(pars, gameworld),
                new List<string> { "character", "trait", "difficulty" },
                new List<string>
                {
                    "The character whose trait you want to check against", "The trait you want to check",
                    "The difficulty of the check. 0 = Automatic, 10 = Impossible, 5 = Normal"
                },
                "This function allows you to roll a check against a trait at a defined difficulty for a character, and see the result. The return values are -3 = Major Fail, -2 = Fail, -1 = Minor Fail, 1 = Minor Pass, 2 = Pass, 3 = Major Pass",
                "Character",
                ProgVariableTypes.Number
            )
        );
    }
}