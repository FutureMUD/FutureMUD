namespace MudSharp.FutureProg.Statements;

internal abstract class Statement : IStatement
{
	public virtual string ErrorMessage { get; protected set; }

	public abstract StatementResult Execute(IVariableSpace variables);

	public virtual StatementResult ExpectedResult => StatementResult.Normal;
}