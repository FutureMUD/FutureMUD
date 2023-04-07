using System;
using System.Collections.Generic;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Effects;

public partial class Effect : IFutureProgVariable
{
	#region IFutureProgVariable Implementation

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Effect;

	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
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

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return FutureProgVariableTypes.Number;
			case "name":
				return FutureProgVariableTypes.Text;
			case "type":
				return FutureProgVariableTypes.Text;
			case "owner":
				return FutureProgVariableTypes.Perceivable;
			default:
				return FutureProgVariableTypes.Error;
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "type", FutureProgVariableTypes.Text },
			{ "owner", FutureProgVariableTypes.Perceivable }
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
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Effect, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}