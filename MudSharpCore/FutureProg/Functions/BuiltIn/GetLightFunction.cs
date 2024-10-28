using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetLightFunction : BuiltInFunction
{
	public GetLightFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceiver = (IPerceiver)ParameterFunctions.First().Result;
		switch (perceiver)
		{
			case IGameItem gi when gi.Location == null:
				Result = new NumberVariable(gi.TrueLocations.Select(x => x.CurrentIllumination(gi)).DefaultIfEmpty(0.0)
				                              .Max());
				break;
			default:
				Result = new NumberVariable(perceiver.Location.CurrentIllumination(perceiver));
				break;
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getlight",
			new[] { ProgVariableTypes.Perceiver },
			(pars, gameworld) => new GetLightFunction(pars),
			new List<string> { "perceiver" },
			new List<string> { "The perceiver for whom you would like to know the current light levels" },
			"This function returns the current light level that a specified perceiver is exposed to. For characters, this would be the light levels in their room. For items, it is the maximum light levels in any room they are part of.",
			"Perception",
			ProgVariableTypes.Number
		));
	}
}