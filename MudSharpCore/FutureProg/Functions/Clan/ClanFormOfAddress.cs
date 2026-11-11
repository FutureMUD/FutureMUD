#nullable enable

using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal sealed class ClanFormOfAddress : BuiltInFunction
{
	private readonly ProgVariableTypes _sourceType;
	private ClanFormOfAddress(IList<IFunction> parameters, ProgVariableTypes sourceType) : base(parameters) => _sourceType = sourceType;
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var speaker = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var subject = ParameterFunctions[1].Result?.GetObject as ICharacter;
		var source = ParameterFunctions[2].Result?.GetObject;
		var clan = source switch { IClan c => c, IRank r => r.Clan, IAppointment a => a.Clan, _ => null };
		var result = subject is null || clan is null ? null : ClanAddressResolver.Resolve(speaker, subject, clan,
			_sourceType == ProgVariableTypes.ClanRank ? source as IRank : null,
			_sourceType == ProgVariableTypes.ClanAppointment ? source as IAppointment : null);
		Result = new TextVariable(result?.Status == ClanAddressStatus.Text ? result.Text : "");
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var (name, type) in new[]
		         {
			         ("clanformofaddress", ProgVariableTypes.Clan), ("rankformofaddress", ProgVariableTypes.ClanRank),
			         ("appointmentformofaddress", ProgVariableTypes.ClanAppointment)
		         })
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
				[ProgVariableTypes.Character, ProgVariableTypes.Character, type],
				(pars, _) => new ClanFormOfAddress(pars, type), ["speaker", "subject", "source"],
				["Speaker/viewer; null tries public outsider then lower-rank rules.", "Person being addressed.", "Clan, held rank or held appointment."],
				"Returns the permitted contextual direct address, or empty text for none, hidden, invalid, ambiguous or failed rules. The subject must actively hold the source. Appointments override rank by address priority.",
				"Clans", ProgVariableTypes.Text));
	}
}
