
namespace MudSharp.FutureProg;

internal class LocalVariableSpace : VariableSpace
{
    protected IVariableSpace Parent;

    public LocalVariableSpace(IVariableSpace parent)
        : base(new Dictionary<string, IProgVariable>())
    {
        Parent = parent;
    }

    public override bool HasVariable(string variable)
    {
        return Parent.HasVariable(variable) || base.HasVariable(variable);
    }

    public override IProgVariable GetVariable(string variable)
    {
		if (Parent is VariableSpace parentSpace && parentSpace.TryGetVariable(variable, out var value))
		{
			return value;
		}

		return Parent.HasVariable(variable) ? Parent.GetVariable(variable) : base.GetVariable(variable);
    }

	internal override bool TryGetVariable(string variable, out IProgVariable value)
	{
		if (Parent is VariableSpace parentSpace && parentSpace.TryGetVariable(variable, out value))
		{
			return true;
		}

		if (Parent.HasVariable(variable))
		{
			value = Parent.GetVariable(variable);
			return true;
		}

		return base.TryGetVariable(variable, out value);
	}

    public override void SetVariable(string variable, IProgVariable value)
    {
        if (Parent.HasVariable(variable))
        {
            Parent.SetVariable(variable, value);
        }
        else
        {
            base.SetVariable(variable, value);
        }
    }

	internal void ClearLocalVariables()
	{
		_variables.Clear();
	}
}
