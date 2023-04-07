using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Discord;

internal class SendDiscordMessageFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	protected SendDiscordMessageFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"senddiscord",
				new[] { FutureProgVariableTypes.Number, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SendDiscordMessageFunction(pars, gameworld)
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var channelResult = ParameterFunctions[0].Result;
		if (channelResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var channelId = (ulong)(double)channelResult.GetObject;

		var titleResult = ParameterFunctions[1].Result;
		if (titleResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var title = titleResult.GetObject.ToString();

		var textResult = ParameterFunctions[2].Result;
		if (textResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var text = textResult.GetObject.ToString();

		Gameworld.DiscordConnection?.SendMessageFromProg(channelId, title, text);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}