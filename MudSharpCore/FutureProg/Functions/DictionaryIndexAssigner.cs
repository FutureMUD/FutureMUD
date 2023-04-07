using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions;

internal class DictionaryIndexAssigner : Function
{
	public string WhichVariable { get; }
	public IFunction IndexFunction { get; }
	public IFunction AssignmentFunction { get; }

	public DictionaryIndexAssigner(string whichVariable, IFunction indexFunction, IFunction assignmentFunction)
	{
		WhichVariable = whichVariable;
		IndexFunction = indexFunction;
		AssignmentFunction = assignmentFunction;
		ReturnType = AssignmentFunction.ReturnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var dictionary = (Dictionary<string, IFutureProgVariable>)variables.GetVariable(WhichVariable)?.GetObject;
		if (dictionary == null)
		{
			ErrorMessage = "Dictionary was null";
			return StatementResult.Error;
		}

		var indexResult = IndexFunction.Execute(variables);
		if (indexResult == StatementResult.Error)
		{
			ErrorMessage = IndexFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var index = IndexFunction.Result?.ToString();
		if (index == null)
		{
			ErrorMessage = "The indexer was null";
			return StatementResult.Error;
		}

		var assignmentResult = AssignmentFunction.Execute(variables);
		if (assignmentResult == StatementResult.Error)
		{
			ErrorMessage = AssignmentFunction.ErrorMessage;
			return StatementResult.Error;
		}

		dictionary[index] = AssignmentFunction.Result;
		Result = AssignmentFunction.Result;
		return StatementResult.Normal;
	}
}