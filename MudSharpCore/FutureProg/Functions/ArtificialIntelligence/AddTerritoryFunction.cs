using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.ArtificialIntelligence;

internal class AddTerritoryFunction : BuiltInFunction
{
    protected AddTerritoryFunction(IList<IFunction> parameters, int flagCount = 0) : base(parameters)
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
            ErrorMessage = "Source Character was null in AddTerritory function.";
            return StatementResult.Error;
        }

        ICell target = (ICell)ParameterFunctions[1].Result;
        if (target == null)
        {
            ErrorMessage = "Target Cell was null in AddTerritory function.";
            return StatementResult.Error;
        }

        Territory effect = source.EffectsOfType<Territory>().FirstOrDefault();
        if (effect == null)
        {
            effect = new Territory(source);
            source.AddEffect(effect);
        }

        effect.AddCell(target);
        for (int i = 0; i < FlagCount; i++)
        {
            effect.TagCell(target, ParameterFunctions[i + 2].Result?.GetObject?.ToString() ?? string.Empty);
        }

        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "addterritory",
            new[] { ProgVariableTypes.Character, ProgVariableTypes.Location },
            (pars, gameworld) => new AddTerritoryFunction(pars),
            new List<string> { "character", "location" },
            new List<string> { "The character whose Territory effect should gain the room.", "The room to add to the character's territory." },
            "Adds a room to a character's Territory effect, creating the effect if necessary. Optional text flags tag the room for later AI checks. Errors if the character or room is null; otherwise returns true.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "addterritory",
            new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text },
            (pars, gameworld) => new AddTerritoryFunction(pars, 1),
            new List<string> { "character", "location", "flag1" },
            new List<string> { "The character whose Territory effect should gain the room.", "The room to add to the character's territory.", "A text tag to attach to this territory room for later AI checks." },
            "Adds a room to a character's Territory effect, creating the effect if necessary. Optional text flags tag the room for later AI checks. Errors if the character or room is null; otherwise returns true.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "addterritory",
            new[]
            {
                ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
                ProgVariableTypes.Text
            },
            (pars, gameworld) => new AddTerritoryFunction(pars, 2),
            new List<string> { "character", "location", "flag1", "flag2" },
            new List<string> { "The character whose Territory effect should gain the room.", "The room to add to the character's territory.", "A text tag to attach to this territory room for later AI checks.", "A second text tag to attach to this territory room." },
            "Adds a room to a character's Territory effect, creating the effect if necessary. Optional text flags tag the room for later AI checks. Errors if the character or room is null; otherwise returns true.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "addterritory",
            new[]
            {
                ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Text,
                ProgVariableTypes.Text, ProgVariableTypes.Text
            },
            (pars, gameworld) => new AddTerritoryFunction(pars, 3),
            new List<string> { "character", "location", "flag1", "flag2", "flag3" },
            new List<string> { "The character whose Territory effect should gain the room.", "The room to add to the character's territory.", "A text tag to attach to this territory room for later AI checks.", "A second text tag to attach to this territory room.", "A third text tag to attach to this territory room." },
            "Adds a room to a character's Territory effect, creating the effect if necessary. Optional text flags tag the room for later AI checks. Errors if the character or room is null; otherwise returns true.",
            "Artificial Intelligence",
            ProgVariableTypes.Boolean
        ));
    }
}
