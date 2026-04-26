using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.ArtificialIntelligence;

internal class WildAnimalHerdRoleFunction : BuiltInFunction
{
    protected WildAnimalHerdRoleFunction(IList<IFunction> parameters) : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICharacter character = (ICharacter)ParameterFunctions[0].Result;
        if (character == null)
        {
            ErrorMessage = "Character was null in WildAnimalHerdRole function.";
            return StatementResult.Error;
        }

        Result = new TextVariable(
            character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault()?.Role.DescribeEnum() ?? "None");
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "wildanimalherdrole",
            new[] { ProgVariableTypes.Character },
            (pars, gameworld) => new WildAnimalHerdRoleFunction(pars),
            new List<string> { "character" },
            new List<string> { "The character whose wild animal herd effect should be inspected." },
            "Returns the character's current wild animal herd role as text, or 'None' if they are not in a herd. Errors if the character parameter is null.",
            "Artificial Intelligence",
            ProgVariableTypes.Text
        ));
    }
}
