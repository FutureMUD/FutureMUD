using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg;

internal class VariableSpace : IVariableSpace
{
	protected Dictionary<string, IFutureProgVariable> _variables;

	public VariableSpace(Dictionary<string, IFutureProgVariable> variables)
	{
		_variables = variables;
	}

	public virtual IFutureProgVariable GetVariable(string variable)
	{
		if (_variables.ContainsKey(variable))
		{
			return _variables[variable];
		}

		throw new ApplicationException($"Unknown variable {variable} in FutureProg.");
	}

	public virtual bool HasVariable(string variable)
	{
		return _variables.ContainsKey(variable);
	}

	public virtual void SetVariable(string variable, IFutureProgVariable value)
	{
		_variables[variable] = value;
	}
}