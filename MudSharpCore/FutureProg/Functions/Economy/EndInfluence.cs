using MudSharp.Economy;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.Economy
{
    internal class EndInfluence : BuiltInFunction
    {
        public IFuturemud Gameworld { get; set; }
        #region Static Initialisation
        public static void RegisterFunctionCompiler()
        {
            FutureProg.RegisterBuiltInFunctionCompiler(
                new FunctionCompilerInformation(
                    "EndInfluence".ToLowerInvariant(),
                    new[] { ProgVariableTypes.Number }, // the parameters the function takes
                    (pars, gameworld) => new EndInfluence(pars, gameworld),
                    new List<string>
                    {
                        "Influence"
                    }, // parameter names
                    new List<string>
                    {
                        "The ID of the influence to end"
                    }, // parameter help text
                    "Ends the specified market influence, if it exists", // help text for the function,

                    "Markets", // the category to which this function belongs,

                    ProgVariableTypes.Boolean // the return type of the function
                )
            );
        }
        #endregion

        #region Constructors
        protected EndInfluence(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

            IMarketInfluence influence = Gameworld.MarketInfluences.Get((long?)(decimal?)ParameterFunctions[0].Result?.GetObject ?? 0L);
            if (influence is null)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            influence.EndOrCancel();
            Result = new BooleanVariable(true);
            return StatementResult.Normal;
        }
    }
}
