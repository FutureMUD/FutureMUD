using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetMaterial : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetMaterial".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Solid },
				(pars, gameworld) => new SetMaterial(pars, gameworld),
				new List<string> { "item", "material" },
				new List<string>
				{
					"The item whose primary material you want to set",
					"The material you want to set"
				},
				"This function sets the primary material of an item to the specified material. It returns true if this succeeds.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetMaterial".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
				(pars, gameworld) => new SetMaterial(pars, gameworld),
				new List<string> { "item", "material" },
				new List<string>
				{
					"The item whose primary material you want to set",
					"The ID of the material that you want to set"
				},
				"This function sets the primary material of an item to the specified material. It returns true if this succeeds.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetMaterial".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SetMaterial(pars, gameworld),
				new List<string> { "item", "material" },
				new List<string>
				{
					"The item whose primary material you want to set",
					"The name of the material that you want to set"
				},
				"This function sets the primary material of an item to the specified material. It returns true if this succeeds.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetMaterial(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var item = (IGameItem)ParameterFunctions[0].Result?.GetObject;
		if (item == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		ISolid material;
		if (ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Solid))
		{
			material = (ISolid)ParameterFunctions[1].Result?.GetObject;
		}
		else if (ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			material = Gameworld.Materials.Get(Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0));
		}
		else
		{
			material = Gameworld.Materials.GetByName(ParameterFunctions[1].Result?.GetObject?.ToString() ??
			                                         string.Empty);
		}

		if (material == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		item.Material = material;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}