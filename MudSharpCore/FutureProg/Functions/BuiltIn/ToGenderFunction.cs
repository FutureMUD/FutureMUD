using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Shape;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToGenderFunction : BuiltInFunction
{
	protected IFunction InnerFunction;

	public ToGenderFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		InnerFunction = parameters.First();
	}

	public override IProgVariable Result
	{
		get
		{
			switch (((string)InnerFunction.Result?.GetObject ?? "").ToLowerInvariant())
			{
				case "male":
					return new GenderVariable(Gender.Male);
				case "female":
					return new GenderVariable(Gender.Female);
				case "neuter":
					return new GenderVariable(Gender.Neuter);
				case "non-binary":
					return new GenderVariable(Gender.NonBinary);
				case "indeterminate":
					return new GenderVariable(Gender.Indeterminate);
				default:
					return new GenderVariable(Gender.Indeterminate);
			}
		}
		protected set { }
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Gender;
		protected set { }
	}

	public override string ErrorMessage
	{
		get => InnerFunction.ErrorMessage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		return InnerFunction.Execute(variables);
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"togender",
				new[] { ProgVariableTypes.Text },
				(pars, gameworld) => new ToGenderFunction(pars),
				new List<string> { "gender" },
				new List<string> { "The textual representation of the gender you want to load." },
				"This function takes some text and returns a gender based on the text. The valid genders are male, female, neuter, non-binary and indeterminate.",
				"Lookup",
				ProgVariableTypes.Gender
			)
		);
	}
}