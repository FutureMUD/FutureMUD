#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class MilitaryStandardQueryFunction : BuiltInFunction
{
	private readonly MilitaryStandardQueryMode _mode;

	private enum MilitaryStandardQueryMode
	{
		IsStandard,
		Identity,
		Association,
		Custody,
		CaptureCount,
		IsPlanted
	}

	private MilitaryStandardQueryFunction(IList<IFunction> parameters, MilitaryStandardQueryMode mode) : base(parameters)
	{
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _mode switch
		{
			MilitaryStandardQueryMode.IsStandard or MilitaryStandardQueryMode.IsPlanted => ProgVariableTypes.Boolean,
			MilitaryStandardQueryMode.CaptureCount => ProgVariableTypes.Number,
			_ => ProgVariableTypes.Text
		};
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var standard = (ParameterFunctions[0].Result?.GetObject as IGameItem)?.GetItemType<IMilitaryStandard>();
		Result = _mode switch
		{
			MilitaryStandardQueryMode.IsStandard => new BooleanVariable(standard is not null),
			MilitaryStandardQueryMode.Identity => new TextVariable(standard?.IdentityKey ?? string.Empty),
			MilitaryStandardQueryMode.Association => new TextVariable(standard is null
				? string.Empty
				: standard.AssociationType == MilitaryStandardAssociationType.None
					? string.Empty
					: standard.AssociationKey),
			MilitaryStandardQueryMode.Custody => new TextVariable(standard?.CustodyState.DescribeEnum() ?? string.Empty),
			MilitaryStandardQueryMode.CaptureCount => new NumberVariable(standard?.CaptureCount ?? 0),
			MilitaryStandardQueryMode.IsPlanted => new BooleanVariable(standard?.IsPlanted == true),
			_ => new BooleanVariable(false)
		};
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("isstandard", MilitaryStandardQueryMode.IsStandard, ProgVariableTypes.Boolean,
			"Returns true when the item has a military-standard component.");
		Register("standardidentity", MilitaryStandardQueryMode.Identity, ProgVariableTypes.Text,
			"Returns the effective identity key of a military standard, or blank for another item.");
		Register("standardassociation", MilitaryStandardQueryMode.Association, ProgVariableTypes.Text,
			"Returns the effective unit or ship association key of a military standard, or blank.");
		Register("standardcustody", MilitaryStandardQueryMode.Custody, ProgVariableTypes.Text,
			"Returns the current Unclaimed, Friendly or Captured custody state.");
		Register("standardcapturecount", MilitaryStandardQueryMode.CaptureCount, ProgVariableTypes.Number,
			"Returns the number of distinct hostile captures of a military standard.");
		Register("standardisplanted", MilitaryStandardQueryMode.IsPlanted, ProgVariableTypes.Boolean,
			"Returns true when a military standard is currently planted.");
	}

	private static void Register(string name, MilitaryStandardQueryMode mode, ProgVariableTypes returnType,
		string description)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Item],
			(pars, gameworld) => new MilitaryStandardQueryFunction(pars, mode),
			["item"],
			["The item to query"],
			description,
			"Items",
			returnType));
	}
}
