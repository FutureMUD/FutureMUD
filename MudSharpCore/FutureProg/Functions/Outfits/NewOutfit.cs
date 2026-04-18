using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Inventory;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Outfits;

internal class NewOutfit : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "newoutfit",
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Text },
                (pars, gameworld) => new NewOutfit(pars, gameworld),
                [
                    "character",
                    "name"
                ],
                [
                    "The character who owns the new outfit",
                    "The name of the outfit"
                ],
                "Creates a new blank outfit for a character. Returns the outfit it creates. Can return null if the name is not valid.",
                "Outfits",
                ProgVariableTypes.Outfit
            )
        );
    }

    #endregion

    #region Constructors

    protected NewOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Outfit;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICharacter owner = (ICharacter)ParameterFunctions[0].Result;
        if (owner == null)
        {
            ErrorMessage = "Null owner in NewOutfit";
            return StatementResult.Error;
        }

        string newName = ParameterFunctions[1].Result.GetObject?.ToString();
        if (string.IsNullOrEmpty(newName) || owner.Outfits.Any(x => x.Name.EqualTo(newName)))
        {
            // This doesn't need to be an error, because it might be a valid case
            Result = null;
            return StatementResult.Normal;
        }

        Outfit newOutfit = new(owner, newName);
        owner.AddOutfit(newOutfit);
        Result = newOutfit;
        return StatementResult.Normal;
    }
}