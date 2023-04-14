using MudSharp.Character;
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
using MudSharp.Character.Name;
using MudSharp.Database;
using OpenAI_API;
using OpenAI_API.Chat;

namespace MudSharp.FutureProg.Functions.OpenAI;
#nullable enable
internal class GPTRequest : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"GPTRequest".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Number, FutureProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new GPTRequest(pars, gameworld),
				new List<string>
				{
					"Thread",
					"Message",
					"User",
					"MaximumHistory",
					"Function"
				}, // parameter names
				new List<string>
				{
					"The GPTThread to Call",
					"The message to send to GPT",
					"The user whose history is being invoked",
					"The maximum history entries",
					"A Prog to call with the results of this request"
				}, // parameter help text
				"This function will schedule a call to GPT including a thread history, and call the specified prog with the outcome. Returns true if successfully queued.", // help text for the function,
				"OpenAI", // the category to which this function belongs,
				FutureProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"GPTRequest".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Number, FutureProgVariableTypes.Text, FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Number, FutureProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new GPTRequest(pars, gameworld),
				new List<string>
				{
					"Thread",
					"Message",
					"User",
					"MaximumHistory",
					"Function"
				}, // parameter names
				new List<string>
				{
					"The GPTThread to Call",
					"The message to send to GPT",
					"The user whose history is being invoked",
					"The maximum history entries",
					"A Prog to call with the results of this request"
				}, // parameter help text
				"This function will schedule a call to GPT including a thread history, and call the specified prog with the outcome. Returns true if successfully queued.", // help text for the function,
				"OpenAI", // the category to which this function belongs,
				FutureProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected GPTRequest(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get { return FutureProgVariableTypes.Text; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}
		var messageText = (string)(ParameterFunctions[1].Result?.GetObject ?? "");
		var character = (ICharacter?)ParameterFunctions[2].Result?.GetObject;
		var history = (int)(decimal)(ParameterFunctions[3].Result?.GetObject ?? -1.0M);
		using (new FMDB())
		{
			Models.GPTThread? thread;
			if (ParameterFunctions[0].ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
			{
				thread = FMDB.Context.GPTThreads.Find((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0M));
			}
			else
			{
				var name = (string)(ParameterFunctions[0].Result?.GetObject ?? "");
				thread = FMDB.Context.GPTThreads.FirstOrDefault(x => x.Name == name);
			}

			if (thread is null)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			var prog = new FutureProgLookupFromBuilderInput(Gameworld, null, (string)(ParameterFunctions[4].Result?.GetObject ?? ""),
				FutureProgVariableTypes.Void, new List<IEnumerable<FutureProgVariableTypes>>
				{
					new []
					{
						FutureProgVariableTypes.Text,
						FutureProgVariableTypes.Character
					},
					new []
					{
						FutureProgVariableTypes.Text,
					},
				}).LookupProg();
			if (prog is null)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			MudSharp.OpenAI.OpenAIHandler.MakeGPTRequest(thread, messageText, character, text =>
			{
				prog.Execute(text, character);
			}, history);

		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}