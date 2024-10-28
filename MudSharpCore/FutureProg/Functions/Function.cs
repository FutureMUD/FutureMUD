using MudSharp.FutureProg.Statements;

namespace MudSharp.FutureProg.Functions;

internal abstract class Function : Statement, IFunction
{
	public virtual IProgVariable Result { get; protected set; }

	public virtual ProgVariableTypes ReturnType { get; protected set; }
}