using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class OnlineActors : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation
    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "onlineactors",
                [], // the parameters the function takes
                (pars, gameworld) => new OnlineActors(pars, gameworld),
                new List<string>
                {
                }, // parameter names
                new List<string>
                {
                }, // parameter help text
                "Returns all of the Actors (PC and NPC characters) that are loaded into the game.", // help text for the function,
                "Character",// the category to which this function belongs,
                ProgVariableTypes.Character | ProgVariableTypes.Collection // the return type of the function
            )
        );
    }
    #endregion

    #region Constructors
    protected OnlineActors(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }
    #endregion

    public override ProgVariableTypes ReturnType
    {
        get { return ProgVariableTypes.Character | ProgVariableTypes.Collection; }
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        Result = new CollectionVariable(Gameworld.Actors.ToList(), ProgVariableTypes.Character);
        return StatementResult.Normal;
    }
}