using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate.Time;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class GameSecondsPerRealSecondsFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public GameSecondsPerRealSecondsFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
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

		var textResult = ParameterFunctions[0].Result;
		if (textResult?.GetObject == null)
		{
			ErrorMessage = "gamesecondsperrealseconds - Unable to parse clock name parameter.";
			return StatementResult.Error;
		}

		var text = textResult.GetObject.ToString();

		var clock = _gameworld.Clocks.GetByName(text);
		if (clock == null)
		{
			ErrorMessage = "gamesecondsperrealseconds - Invalid clock name received.";
			return StatementResult.Error;
		}

		Result = new NumberVariable(clock.InGameSecondsPerRealSecond);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"gamesecondsperrealseconds",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new GameSecondsPerRealSecondsFunction(pars, gameworld)
		));
	}
}