using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderStringParameter : IAutobuilderParameter
{
	public AutobuilderStringParameter(string parameterName, string missingErrorMessage, bool isOptional)
	{
		ParameterName = parameterName;
		MissingErrorMessage = missingErrorMessage;
		IsOptional = isOptional;
	}

	#region Implementation of IAutobuilderParameter

	public bool IsOptional { get; set; }
	public string TypeName => "Text";
	public string ParameterName { get; set; }
	public string MissingErrorMessage { get; set; }

	public bool IsValidArgument(string argument, object[] previousArguments)
	{
		return true;
	}

	public string WhyIsNotValidArgument(string argument, object[] previousArguments)
	{
		throw new ApplicationException(
			"WhyIsNotValidArgument called on AutobuilderStringParameter, which should never be invalid.");
	}

	public object GetArgument(string argument)
	{
		return argument;
	}

	#endregion
}