namespace MudSharp.Construction.Autobuilder
{
    public interface IAutobuilderParameter
    {
        bool IsOptional { get; }
        string TypeName { get; }
        string ParameterName { get; }
        string MissingErrorMessage { get; }
        bool IsValidArgument(string argument, object[] previousArguments);
        string WhyIsNotValidArgument(string argument, object[] previousArguments);
        object GetArgument(string argument);
    }
}