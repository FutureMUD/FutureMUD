using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.ArtificialIntelligence;

internal class IsTerritoryFunction : BuiltInFunction
{
    protected IsTerritoryFunction(IList<IFunction> parameters, int flagCount = 0) : base(parameters)
    {
        FlagCount = flagCount;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public int FlagCount { get; protected set; }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICharacter source = (ICharacter)ParameterFunctions[0].Result;
        if (source == null)
        {
            ErrorMessage = "Source Character was null in IsTerritory function.";
            return StatementResult.Error;
        }

        ICell target = (ICell)ParameterFunctions[1].Result;
        if (target == null)
        {
            ErrorMessage = "Target Cell was null in IsTerritory function.";
            return StatementResult.Error;
        }

        Territory effect = source.EffectsOfType<Territory>().FirstOrDefault();
        if (effect == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        for (int i = 0; i < FlagCount; i++)
        {
            if (!effect.HasFlag(target, ParameterFunctions[i + 2].Result?.GetObject?.ToString() ?? string.Empty))
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }
        }

        Result = new BooleanVariable(effect.Cells.Contains(target));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "isterritory",
            new[] { ProgVariableTypes.Character, ProgVariableTypes.Location },
            (pars, gameworld) => new IsTerritoryFunction(pars),
            new List<string> { "character", "location" },
            new List<string> { "The character whose Territory effect should be checked.", "The room to test against the character's territory." },
            "Checks whether a room is in a character's Territory effect and, when flags are supplied, whether all supplied flags are present. Errors if the character or room is null; returns false if the character has no territory effect or any flag is missing.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "isterritory",
            new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text },
            (pars, gameworld) => new IsTerritoryFunction(pars, 1),
            new List<string> { "character", "location", "flag1" },
            new List<string> { "The character whose Territory effect should be checked.", "The room to test against the character's territory.", "A text tag that must be present on this territory room." },
            "Checks whether a room is in a character's Territory effect and, when flags are supplied, whether all supplied flags are present. Errors if the character or room is null; returns false if the character has no territory effect or any flag is missing.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "isterritory",
            new[]
            {
                ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
                ProgVariableTypes.Text
            },
            (pars, gameworld) => new IsTerritoryFunction(pars, 2),
            new List<string> { "character", "location", "flag1", "flag2" },
            new List<string> { "The character whose Territory effect should be checked.", "The room to test against the character's territory.", "A text tag that must be present on this territory room.", "A second text tag that must be present on this territory room." },
            "Checks whether a room is in a character's Territory effect and, when flags are supplied, whether all supplied flags are present. Errors if the character or room is null; returns false if the character has no territory effect or any flag is missing.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "isterritory",
            new[]
            {
                ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
                ProgVariableTypes.Text, ProgVariableTypes.Text
            },
            (pars, gameworld) => new IsTerritoryFunction(pars, 3),
            new List<string> { "character", "location", "flag1", "flag2", "flag3" },
            new List<string> { "The character whose Territory effect should be checked.", "The room to test against the character's territory.", "A text tag that must be present on this territory room.", "A second text tag that must be present on this territory room.", "A third text tag that must be present on this territory room." },
            "Checks whether a room is in a character's Territory effect and, when flags are supplied, whether all supplied flags are present. Errors if the character or room is null; returns false if the character has no territory effect or any flag is missing.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));
    }
}
