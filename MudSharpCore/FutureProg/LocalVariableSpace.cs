using System.Collections.Generic;

namespace MudSharp.FutureProg;

internal class LocalVariableSpace : VariableSpace
{
	protected IVariableSpace Parent;

	public LocalVariableSpace(IVariableSpace parent)
		: base(new Dictionary<string, IFutureProgVariable>())
	{
		Parent = parent;
	}

	public override bool HasVariable(string variable)
	{
		return Parent.HasVariable(variable) || base.HasVariable(variable);
	}

	public override IFutureProgVariable GetVariable(string variable)
	{
		return Parent.HasVariable(variable) ? Parent.GetVariable(variable) : base.GetVariable(variable);
	}

	public override void SetVariable(string variable, IFutureProgVariable value)
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
}