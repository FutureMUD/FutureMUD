using System;
using System.Collections.Generic;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Effects;

public partial class Effect : IProgVariable
{
	#region IFutureProgVariable Implementation

	public ProgVariableTypes Type => ProgVariableTypes.Effect;

	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "type":
				return new TextVariable(SpecificEffectType);
			case "owner":
				return Owner;
			default:
				throw new NotSupportedException($"Unsupported property type {property} in Effect.GetProperty");
		}
	}

	private static ProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return ProgVariableTypes.Number;
			case "name":
				return ProgVariableTypes.Text;
			case "type":
				return ProgVariableTypes.Text;
			case "owner":
				return ProgVariableTypes.Perceivable;
			default:
				return ProgVariableTypes.Error;
		}
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "owner", ProgVariableTypes.Perceivable }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "type", "" },
			{ "owner", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Effect, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}