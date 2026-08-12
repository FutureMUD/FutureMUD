
namespace MudSharp.FutureProg;

internal class VariableSpace : IVariableSpace
{
	protected Dictionary<string, IProgVariable> _variables;
	private IProgVariable _returnVariable;
	private readonly bool _hasReturnVariable;

    public VariableSpace(Dictionary<string, IProgVariable> variables)
    {
        _variables = variables;
    }

	internal VariableSpace()
	{
	}

	internal VariableSpace(IProgVariable returnVariable)
	{
		_returnVariable = returnVariable;
		_hasReturnVariable = true;
	}

    public virtual IProgVariable GetVariable(string variable)
    {
		if (_hasReturnVariable && variable == "return")
		{
			return _returnVariable;
		}

		if (_variables?.TryGetValue(variable, out var value) == true)
        {
			return value;
        }

        throw new ApplicationException($"Unknown variable {variable} in FutureProg.");
    }

    public virtual bool HasVariable(string variable)
    {
		return _hasReturnVariable && variable == "return" || _variables?.ContainsKey(variable) == true;
    }

	internal virtual bool TryGetVariable(string variable, out IProgVariable value)
	{
		if (_hasReturnVariable && variable == "return")
		{
			value = _returnVariable;
			return true;
		}

		if (_variables != null)
		{
			return _variables.TryGetValue(variable, out value);
		}

		value = null;
		return false;
	}

    public virtual void SetVariable(string variable, IProgVariable value)
    {
		if (_hasReturnVariable && variable == "return")
		{
			_returnVariable = value;
			return;
		}

		_variables ??= new Dictionary<string, IProgVariable>();
		_variables[variable] = value;
    }
}
