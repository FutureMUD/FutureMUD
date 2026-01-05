using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class OnlineCharacters : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation
    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "onlinecharacters",
				[], // the parameters the function takes
                (pars, gameworld) => new OnlineCharacters(pars, gameworld),
                new List<string> {
                }, // parameter names
                new List<string> {
                }, // parameter help text
                "Returns all of the PC characters that are online.", // help text for the function,
                "Character",// the category to which this function belongs,
                ProgVariableTypes.Character | ProgVariableTypes.Collection // the return type of the function
            )
        );
    }
    #endregion

    #region Constructors
    protected OnlineCharacters(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		Result = new CollectionVariable(Gameworld.Characters.ToList(), ProgVariableTypes.Character);
        return StatementResult.Normal;
    }
}
