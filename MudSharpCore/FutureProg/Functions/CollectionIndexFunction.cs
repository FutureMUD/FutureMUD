using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions;

internal class CollectionIndexFunction : Function
{
	public string WhichVariable { get; }
	public IFunction IndexFunction { get; }

	public CollectionIndexFunction(string whichVariable, IFunction indexFunction, FutureProgVariableTypes returnType)
	{
		WhichVariable = whichVariable;
		IndexFunction = indexFunction;
		ReturnType = returnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var collection = (IList)variables.GetVariable(WhichVariable)?.GetObject;
		if (collection == null)
		{
			ErrorMessage = "Collection was null";
			return StatementResult.Error;
		}

		var result = IndexFunction.Execute(variables);
		if (result == StatementResult.Error)
		{
			ErrorMessage = IndexFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var index = Convert.ToInt32(IndexFunction.Result);
		Result = collection.OfType<IFutureProgVariable>().ElementAtOrDefault(index);
		return StatementResult.Normal;
	}
}