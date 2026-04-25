namespace MudSharp.Magic;

#nullable enable

public record SpellAdditionalParameter
{
    public string ParameterName { get; init; } = null!;
    public object? Item { get; init; }
}
