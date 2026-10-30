#nullable enable

using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.RPG.Law;

namespace MudSharp.FutureProg.Functions.Crime;

internal enum CustodyFunctionKind { Report, Enforceable, Subdued, Handover }

internal sealed class CustodyAndReportingFunction(IList<IFunction> parameters, CustodyFunctionKind kind) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType
	{
		get => kind is CustodyFunctionKind.Report or CustodyFunctionKind.Enforceable ? ProgVariableTypes.Crime : ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var values = ParameterFunctions.Select(x => x.Result?.GetObject).ToArray();
		switch (kind)
		{
			case CustodyFunctionKind.Report:
				Result = values[0] is ILaw law && values[1] is ICharacter criminal && values[2] is ICharacter witness
					? law.Authority.ReportWitnessedCrime(law, criminal, witness, values[3] as ICharacter,
						values[4] as IGameItem, values[5]?.ToString() ?? "") ?? (IProgVariable)new NullVariable(ReturnType)
					: new NullVariable(ReturnType);
				break;
			case CustodyFunctionKind.Enforceable:
				Result = values[0] is ICharacter wanted && values[1] is ILegalAuthority authority
					? EnforcementCustodyHelper.SelectArrestableCrime(authority, wanted) ?? (IProgVariable)new NullVariable(ReturnType)
					: new NullVariable(ReturnType);
				break;
			case CustodyFunctionKind.Subdued:
				Result = new BooleanVariable(values[0] is ICharacter target && values[1] is ICharacter holder && CustodyHandover.IsSubduedBy(target, holder));
				break;
			case CustodyFunctionKind.Handover:
				Result = new BooleanVariable(values[0] is ICharacter escort && values[1] is ICharacter prisoner &&
					values[2] is ICharacter receiver && values[3] is ILegalAuthority legal &&
					CustodyHandover.TryHandover(escort, prisoner, receiver, legal));
				break;
		}
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		void Register(string name, CustodyFunctionKind mode, ProgVariableTypes[] types, string[] names, string help) =>
			FutureProg.RegisterBuiltInFunctionCompiler(new(name, types, (parameters, _) => new CustodyAndReportingFunction(parameters, mode),
				names, names.Select(x => $"The {x}.").ToArray(), help, "Crime",
				mode is CustodyFunctionKind.Report or CustodyFunctionKind.Enforceable ? ProgVariableTypes.Crime : ProgVariableTypes.Boolean));
		Register("reportcrime", CustodyFunctionKind.Report,
			[ProgVariableTypes.Law, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
			["law", "criminal", "witness", "victim", "item", "context"],
			"Records and reports a witnessed violation of a specific applicable law, including non-automatic laws. Witness must see the colocated criminal within jurisdiction. Victim/item may be null. Uses native witnesses, repeat suppression and history; does not grant knowledge of concealed identities. Returns the crime or null on refusal.");
		Register("enforceablecrime", CustodyFunctionKind.Enforceable, [ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority],
			["criminal", "authority"], "Returns the highest-priority arrestable crime matching native patrol and visible-identity rules, or null.");
		Register("issubduedby", CustodyFunctionKind.Subdued, [ProgVariableTypes.Character, ProgVariableTypes.Character],
			["target", "holder"], "Whether the holder's actual native grapple has the target under control. Dragging, unconsciousness and custody alone are insufficient.");
		Register("handovercriminal", CustodyFunctionKind.Handover,
			[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority],
			["holder", "criminal", "receiver", "authority"],
			"Hands an actually subdued wanted character to a colocated authorised receiver. Releases associated physical controls and invokes native remand/belongings handling. Returns false on invalid scenes; callbacks may interrupt cleanup before incarceration. Emits one native handover echo on acceptance.");
	}
}
