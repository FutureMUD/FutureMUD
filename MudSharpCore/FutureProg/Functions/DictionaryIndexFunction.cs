using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions;

internal class DictionaryIndexFunction : Function
{
	public string WhichVariable { get; }
	public IFunction IndexFunction { get; }

	public DictionaryIndexFunction(string whichVariable, IFunction indexFunction, FutureProgVariableTypes returnType)
	{
		WhichVariable = whichVariable;
		IndexFunction = indexFunction;
		ReturnType = returnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var dictionary = (Dictionary<string, IFutureProgVariable>)variables.GetVariable(WhichVariable)?.GetObject;
		if (dictionary == null)
		{
			ErrorMessage = "Dictionary was null";
			return StatementResult.Error;
		}

		var result = IndexFunction.Execute(variables);
		if (result == StatementResult.Error)
		{
			ErrorMessage = IndexFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var index = IndexFunction.Result?.ToString();
		Result = index != null && dictionary.ContainsKey(index) ? dictionary[index] : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}
}