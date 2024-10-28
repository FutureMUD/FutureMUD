using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class ClearMorphTimer : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ClearMorphTimer".ToLowerInvariant(),
				new[] { ProgVariableTypes.Item },
				(pars, gameworld) => new ClearMorphTimer(pars, gameworld),
				new List<string> { "item" },
				new List<string> { "The item who's morph timer you want to clear" },
				"This function stops an item from morphing. Its timer will not resume unless specifically made to do so via SetMorphTimer. This function returns true unless there was a problem, e.g. item was null.",
				"Items",
				ProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected ClearMorphTimer(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		item.EndMorphTimer();
		item.MorphTime = System.DateTime.MinValue;
		item.Changed = true;

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}