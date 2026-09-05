#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class PsychicQueryFunction : BuiltInFunction
{
	private readonly string _mode;
	private readonly ProgVariableTypes _returnType;
	private PsychicQueryFunction(IList<IFunction> parameters, string mode, ProgVariableTypes returnType) : base(parameters)
	{ _mode = mode; _returnType = returnType; }
	public override ProgVariableTypes ReturnType { get => _returnType; protected set { } }
	public static void RegisterFunctionCompiler()
	{
		Register("hasmagicpower", [ProgVariableTypes.Character, ProgVariableTypes.Number], ProgVariableTypes.Boolean,
			["character", "powerId"], "Tests effective capability access to a power, including skill unlocks; never grants access.");
		Register("psychicdisposition", [ProgVariableTypes.Character, ProgVariableTypes.Character], ProgVariableTypes.Number,
			["character", "subject"], "Returns the current psychic affinity or aversion towards a subject. This grants no authority or privileges.");
		Register("psychometricimpressions", [ProgVariableTypes.Perceivable], ProgVariableTypes.Text | ProgVariableTypes.Collection,
			["owner"], "Returns recorded item/cell impression text, including authored clues, or an empty collection when the world feature is disabled.");
	}
	private static void Register(string name, ProgVariableTypes[] parameters, ProgVariableTypes result, string[] names, string help) =>
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name, parameters,
			(pars, _) => new PsychicQueryFunction(pars, name, result), names, names.Select(x => $"The {x} to inspect").ToArray(), help, "Magic", result));
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		switch (_mode)
		{
			case "hasmagicpower":
				var id = Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0);
				Result = new BooleanVariable(actor?.Capabilities.SelectMany(x => x.InherentPowers(actor)).Any(x => x.Id == id) == true);
				break;
			case "psychicdisposition":
				var subject = ParameterFunctions[1].Result?.GetObject as ICharacter;
				Result = new NumberVariable(actor is not null && subject is not null ? PsychicEmotionEffect.Disposition(actor, subject) : 0);
				break;
			default:
				var owner = ParameterFunctions[0].Result?.GetObject as IPerceivable;
				var texts = owner is MudSharp.GameItems.IGameItem or MudSharp.Construction.ICell
					? PsychometricRecorder.Read(owner)?.Impressions.Select(x => new TextVariable(x.Text)).ToList() ?? [] : [];
				Result = new CollectionVariable(texts, ProgVariableTypes.Text);
				break;
		}
		return StatementResult.Normal;
	}
}
