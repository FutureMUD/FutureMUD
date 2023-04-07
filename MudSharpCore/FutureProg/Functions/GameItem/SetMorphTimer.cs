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

internal class SetMorphTimer : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetMorphTimer".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.TimeSpan },
				(pars, gameworld) => new SetMorphTimer(pars, gameworld, true),
				new List<string> { "item", "timer" },
				new List<string>
				{
					"The item who's morph timer you want to set",
					"The amount of time from now you want the morph timer to expire in"
				},
				"This function allows you to set the morph timer on an item, overwriting whatever it currently is and setting it if it doesn't have one. What happens depends on the prototype. If an item is not set to morph normally, this can still make it morph but most likely morph to nothing. Returns false if anything went wrong like passed a null item.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetMorphTimer".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
				(pars, gameworld) => new SetMorphTimer(pars, gameworld, false),
				new List<string> { "item", "timer" },
				new List<string>
				{
					"The item who's morph timer you want to set",
					"A number of seconds from now you want the morph timer to expire in"
				},
				"This function allows you to set the morph timer on an item, overwriting whatever it currently is and setting it if it doesn't have one. What happens depends on the prototype. If an item is not set to morph normally, this can still make it morph but most likely morph to nothing. Returns false if anything went wrong like passed a null item.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	public bool UseTimeSpan { get; protected set; }

	#region Constructors

	protected SetMorphTimer(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool useTimeSpan) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		UseTimeSpan = useTimeSpan;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
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

		System.DateTime newTimer;
		if (UseTimeSpan)
		{
			newTimer = System.DateTime.UtcNow +
			           (ParameterFunctions[1].Result?.GetObject as TimeSpan? ?? TimeSpan.FromSeconds(0));
		}
		else
		{
			newTimer = System.DateTime.UtcNow +
			           TimeSpan.FromSeconds(
				           Convert.ToDouble(ParameterFunctions[1].Result?.GetObject as decimal? ?? 0.0M));
		}

		item.EndMorphTimer();
		item.MorphTime = newTimer;
		item.Changed = true;
		item.StartMorphTimer();

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}