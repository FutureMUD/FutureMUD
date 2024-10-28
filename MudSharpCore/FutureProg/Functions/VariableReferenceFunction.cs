namespace MudSharp.FutureProg.Functions;

internal class VariableReferenceFunction : Function
{
	protected string VariableName;

	public VariableReferenceFunction(string variableName, ProgVariableTypes type)
	{
		VariableName = variableName;
		ReturnType = type;
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		Result = variables.GetVariable(VariableName);
		return StatementResult.Normal;
	}
}