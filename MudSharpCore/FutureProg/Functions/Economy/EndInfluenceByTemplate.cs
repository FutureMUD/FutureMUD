using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Economy
{
    internal class EndInfluenceByTemplate : BuiltInFunction
    {
        public IFuturemud Gameworld { get; set; }
        #region Static Initialisation
        public static void RegisterFunctionCompiler()
        {
            FutureProg.RegisterBuiltInFunctionCompiler(
                new FunctionCompilerInformation(
                    "EndInfluenceByTemplate".ToLowerInvariant(),
                    new[] { ProgVariableTypes.Number, ProgVariableTypes.Number }, // the parameters the function takes
                    (pars, gameworld) => new EndInfluenceByTemplate(pars, gameworld),
                    new List<string> {
                        "Market",
                        "TemplateId"
                    }, // parameter names
                    new List<string> {
                        "The market on which you wish to end the influences",
                        "The ID of the influence template you want to end"
                    }, // parameter help text
                    "Ends all matching influences on the specified market that originate from the specified template", // help text for the function,

                    "Markets", // the category to which this function belongs,

                    ProgVariableTypes.Boolean // the return type of the function
                )
            );

            FutureProg.RegisterBuiltInFunctionCompiler(
                new FunctionCompilerInformation(
                    "EndInfluenceByTemplate".ToLowerInvariant(),
                    new[] { ProgVariableTypes.Number, ProgVariableTypes.Text }, // the parameters the function takes
                    (pars, gameworld) => new EndInfluenceByTemplate(pars, gameworld),
                    new List<string> {
                        "Market",
                        "Template"
                    }, // parameter names
                    new List<string> {
                        "The market on which you wish to end the influences",
                        "The ID of the influence template you want to end"
                    }, // parameter help text
                    "Ends all matching influences on the specified market that originate from the specified template", // help text for the function,

                    "Markets", // the category to which this function belongs,

                    ProgVariableTypes.Boolean // the return type of the function
                )
            );
        }
        #endregion

        #region Constructors
        protected EndInfluenceByTemplate(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

            IMarket market = Gameworld.Markets.Get((long?)(decimal?)ParameterFunctions[0].Result?.GetObject ?? 0L);
            if (market is null)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            IMarketInfluenceTemplate template = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
                ? Gameworld.MarketInfluenceTemplates.Get((string)ParameterFunctions[1].Result?.GetObject ?? "").FirstOrDefault()
                : Gameworld.MarketInfluenceTemplates.Get((long?)(decimal?)ParameterFunctions[1].Result?.GetObject ?? 0L);
            if (template is null)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }

            foreach (IMarketInfluence influence in market.MarketInfluences.Where(x => x.MarketInfluenceTemplate == template).ToList())
            {
                influence.EndOrCancel();
            }

            Result = new BooleanVariable(true);
            return StatementResult.Normal;
        }
    }
}
