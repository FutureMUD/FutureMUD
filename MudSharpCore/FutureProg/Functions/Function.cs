using MudSharp.FutureProg.Statements;

namespace MudSharp.FutureProg.Functions;

internal abstract class Function : Statement, IFunction
{
	public virtual IFutureProgVariable Result { get; protected set; }

	public virtual FutureProgVariableTypes ReturnType { get; protected set; }
}