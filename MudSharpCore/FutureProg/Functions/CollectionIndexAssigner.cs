using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions;

internal class CollectionIndexAssigner : Function
{
	public string WhichVariable { get; }
	public IFunction IndexFunction { get; }
	public IFunction AssignmentFunction { get; }

	public CollectionIndexAssigner(string whichVariable, IFunction indexFunction, IFunction assignmentFunction)
	{
		WhichVariable = whichVariable;
		IndexFunction = indexFunction;
		AssignmentFunction = assignmentFunction;
		ReturnType = AssignmentFunction.ReturnType;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var collection = (IList)variables.GetVariable(WhichVariable)?.GetObject;
		if (collection == null)
		{
			ErrorMessage = "Collection was null";
			return StatementResult.Error;
		}

		var indexResult = IndexFunction.Execute(variables);
		if (indexResult == StatementResult.Error)
		{
			ErrorMessage = IndexFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var index = Convert.ToInt32(IndexFunction.Result);

		var assignmentResult = AssignmentFunction.Execute(variables);
		if (assignmentResult == StatementResult.Error)
		{
			ErrorMessage = AssignmentFunction.ErrorMessage;
			return StatementResult.Error;
		}

		try
		{
			((IList<IFutureProgVariable>)collection).Insert(index, AssignmentFunction.Result);
		}
		catch (IndexOutOfRangeException)
		{
			ErrorMessage = $"Index of {index} was out of range, maximum index was {collection.Count}.";
			return StatementResult.Error;
		}

		Result = AssignmentFunction.Result;
		return StatementResult.Normal;
	}
}