using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;

namespace MudSharp.RPG.Law;

public partial class LegalClass
{
	public ProgVariableTypes Type => ProgVariableTypes.LegalClass;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"legalauthority" => Authority,
			"priority" => new NumberVariable(LegalClassPriority),
			"canbedetaineduntilfinespaid" => new BooleanVariable(CanBeDetainedUntilFinesPaid),
			_ => throw new ApplicationException($"Invalid property {property} requested in LegalClass.GetProperty")
		};
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "legalauthority", ProgVariableTypes.LegalAuthority },
			{ "priority", ProgVariableTypes.Number },
			{ "canbedetaineduntilfinespaid", ProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the legal class" },
			{ "name", "The name of the legal class" },
			{ "legalauthority", "The legal authority that owns this legal class" },
			{ "priority", "The evaluation priority of this legal class" },
			{ "canbedetaineduntilfinespaid", "Whether members of this legal class can be detained until fines are paid" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.LegalClass,
			DotReferenceHandler(), DotReferenceHelp());
	}
}
