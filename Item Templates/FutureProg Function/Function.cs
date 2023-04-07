using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace $rootnamespace$
{
    internal class $safeitemrootname$ : BuiltInFunction {
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
        {
            FutureProg.RegisterBuiltInFunctionCompiler(
                new FunctionCompilerInformation(
                    "$safeitemrootname$".ToLowerInvariant(),
                    new[] { FutureProgVariableTypes.Number }, // the parameters the function takes
                    (pars, gameworld) => new $safeitemrootname$(pars, gameworld),
					new List<string>{ }, // parameter names
                    new List<string>{ }, // parameter help text
                    "", // help text for the function,
                    "", // the category to which this function belongs,
                    FutureProgVariableTypes.Boolean // the return type of the function
                )
            );
        }
		#endregion
		
		#region Constructors
		protected $safeitemrootname$(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
        {
            Gameworld = gameworld;
        }
		#endregion
		
		public override FutureProgVariableTypes ReturnType {
            get { return FutureProgVariableTypes.Boolean; }
            protected set { }
        }
		
		public override StatementResult Execute(IVariableSpace variables)
        {
            if (base.Execute(variables) == StatementResult.Error)
            {
                return StatementResult.Error;
            }
			
			// Your logic here

            Result = new BooleanVariable(true);
            return StatementResult.Normal;
        }
    }
}
