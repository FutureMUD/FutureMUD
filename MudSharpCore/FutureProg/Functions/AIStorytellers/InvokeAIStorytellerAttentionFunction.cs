using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp.FutureProg.Functions.AIStorytellers;

internal class InvokeAIStorytellerAttentionFunction : BuiltInFunction
{
	private IFuturemud Gameworld { get; }
	private bool ResolveById { get; }

	public static void RegisterFunctionCompiler()
	{
		RegisterFunction("invokestorytellerattention");
		RegisterFunction("aistorytellerattention");
	}

	private static void RegisterFunction(string functionName)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			functionName,
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Text },
			(pars, gameworld) => new InvokeAIStorytellerAttentionFunction(pars, gameworld, true),
			new List<string>
			{
				"storyteller",
				"attention"
			},
			new List<string>
			{
				"The id of the AI storyteller to notify.",
				"The attention text to send to the storyteller."
			},
			"Invokes direct storyteller attention by id and returns true if the invocation was accepted.",
			"AI Storyteller",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			functionName,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new InvokeAIStorytellerAttentionFunction(pars, gameworld, false),
			new List<string>
			{
				"storyteller",
				"attention"
			},
			new List<string>
			{
				"The name of the AI storyteller to notify.",
				"The attention text to send to the storyteller."
			},
			"Invokes direct storyteller attention by name and returns true if the invocation was accepted.",
			"AI Storyteller",
			ProgVariableTypes.Boolean
		));
	}

	private InvokeAIStorytellerAttentionFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld,
		bool resolveById)
		: base(parameterFunctions)
	{
		Gameworld = gameworld;
		ResolveById = resolveById;
	}

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

		var attention = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (string.IsNullOrWhiteSpace(attention))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		IAIStoryteller storyteller;
		if (ResolveById)
		{
			if (!TryGetId(ParameterFunctions[0].Result?.GetObject, out var id))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			storyteller = Gameworld.AIStorytellers.Get(id);
		}
		else
		{
			var text = ParameterFunctions[0].Result?.GetObject?.ToString();
			if (string.IsNullOrWhiteSpace(text))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			storyteller = Gameworld.AIStorytellers.GetByIdOrName(text, permitAbbreviations: false);
		}

		Result = new BooleanVariable(storyteller?.InvokeDirectAttention(attention) ?? false);
		return StatementResult.Normal;
	}

	private static bool TryGetId(object value, out long id)
	{
		id = 0L;
		if (value is null)
		{
			return false;
		}

		if (value is int intValue)
		{
			id = intValue;
			return true;
		}

		if (value is long longValue)
		{
			id = longValue;
			return true;
		}

		if (value is decimal decimalValue)
		{
			id = (long)decimalValue;
			return true;
		}

		if (value is double doubleValue)
		{
			id = (long)doubleValue;
			return true;
		}

		if (value is float floatValue)
		{
			id = (long)floatValue;
			return true;
		}

		return long.TryParse(value.ToString(), out id);
	}
}
