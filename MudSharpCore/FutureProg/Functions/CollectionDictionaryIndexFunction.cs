using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions;

internal class CollectionDictionaryIndexFunction : Function
{
	public string WhichVariable { get; }
	public IFunction IndexFunction { get; }

	public CollectionDictionaryIndexFunction(string whichVariable, IFunction indexFunction,
		ProgVariableTypes returnType)
	{
		WhichVariable = whichVariable;
		IndexFunction = indexFunction;
		ReturnType = returnType | ProgVariableTypes.Collection;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var dictionary =
			(CollectionDictionary<string, IProgVariable>)variables.GetVariable(WhichVariable)?.GetObject;
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
		var valueType = dictionary.ValueType;
		;
		Result = index != null && dictionary.ContainsKey(index)
			? new CollectionVariable(dictionary[index], ReturnType & ~ProgVariableTypes.Collection)
			: new CollectionVariable(Utilities.CreateList(valueType), ReturnType & ~ProgVariableTypes.Collection);
		return StatementResult.Normal;
	}
}