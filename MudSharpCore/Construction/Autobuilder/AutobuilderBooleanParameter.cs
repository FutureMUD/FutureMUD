using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderBooleanParameter : IAutobuilderParameter
{
    public AutobuilderBooleanParameter(string parameterName, string missingErrorMessage, bool isOptional)
    {
        ParameterName = parameterName;
        MissingErrorMessage = missingErrorMessage;
        IsOptional = isOptional;
    }

    #region Implementation of IAutobuilderParameter

    public bool IsOptional { get; set; }
    public string TypeName => "Boolean";
    public string ParameterName { get; set; }
    public string MissingErrorMessage { get; set; }

    public bool IsValidArgument(string argument, object[] previousArguments)
    {
		return bool.TryParse(argument, out _);
    }

    public string WhyIsNotValidArgument(string argument, object[] previousArguments)
    {
        return $"The text {argument.ColourCommand()} is not a valid boolean value for the {ParameterName.Colour(Telnet.Cyan)} argument.";
    }

    public object GetArgument(string argument)
    {
        return bool.Parse(argument);
    }

    #endregion
}