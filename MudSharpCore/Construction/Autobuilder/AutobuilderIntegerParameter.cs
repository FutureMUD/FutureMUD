using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderIntegerParameter : IAutobuilderParameter
{
	public AutobuilderIntegerParameter(string parameterName, string missingErrorMessage, bool isOptional)
	{
		ParameterName = parameterName;
		MissingErrorMessage = missingErrorMessage;
		IsOptional = isOptional;
	}

	#region Implementation of IAutobuilderParameter

	public bool IsOptional { get; set; }
	public string TypeName => "Integer";
	public string ParameterName { get; set; }
	public string MissingErrorMessage { get; set; }

	public int MinimumValue { get; init; } = int.MinValue;

	public int MaximumValue { get; init; } = int.MaxValue;

	public bool IsValidArgument(string argument, object[] previousArguments)
	{
		return int.TryParse(argument, out var value) && value >= MinimumValue && value <= MaximumValue;
	}

	public string WhyIsNotValidArgument(string argument, object[] previousArguments)
	{
		if (!int.TryParse(argument, out var value))
		{
			return
				$"You must enter a valid integer for the {ParameterName.Colour(Telnet.Cyan)} argument, and {argument.Colour(Telnet.Red)} does not convert to a valid integer.";
		}

		if (value < MinimumValue)
		{
			return
				$"You must enter an integer greater than {MinimumValue:N0} for the {ParameterName.Colour(Telnet.Cyan)} argument.";
		}

		return
			$"You must enter an integer less than {MaximumValue:N0} for the {ParameterName.Colour(Telnet.Cyan)} argument.";
	}

	public object GetArgument(string argument)
	{
		return int.Parse(argument);
	}

	#endregion
}