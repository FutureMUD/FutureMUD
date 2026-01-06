using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderDirectionParameter : IAutobuilderParameter
{
    public AutobuilderDirectionParameter(string parameterName, string missingErrorMessage, bool isOptional)
    {
        ParameterName = parameterName;
        MissingErrorMessage = missingErrorMessage;
        IsOptional = isOptional;
    }

    #region Implementation of IAutobuilderParameter

    public bool IsOptional { get; set; }
    public string TypeName => "Direction";
    public string ParameterName { get; set; }
    public string MissingErrorMessage { get; set; }

    public bool IsValidArgument(string argument, object[] previousArguments)
    {
		return argument.TryParseEnum<CardinalDirection>(out var value) && value != CardinalDirection.Unknown && value != CardinalDirection.Up && value != CardinalDirection.Down;
    }

    public string WhyIsNotValidArgument(string argument, object[] previousArguments)
    {
        if (!argument.TryParseEnum<CardinalDirection>(out var value))
        {
            return
                $"You must enter a valid cardinal direction for the {ParameterName.Colour(Telnet.Cyan)} argument, and {argument.Colour(Telnet.Red)} does not convert to a valid cardinal direction.";
        }

		return $"The cardinal directions #6up#0, #6down#0 and #6unknown#0 are not valid for this parameter.";
    }

    public object GetArgument(string argument)
    {
		return argument.ParseEnumWithDefault(CardinalDirection.North);
    }

    #endregion
}
