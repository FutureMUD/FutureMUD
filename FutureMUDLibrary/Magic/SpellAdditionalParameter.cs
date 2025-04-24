namespace MudSharp.Magic;

public record SpellAdditionalParameter{
	public string ParameterName { get; init; }
	public object Item { get; init; }
}