using MudSharp.Form.Material;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class LoadCommodityFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public LoadCommodityFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		ISolid material;
		if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Solid))
		{
			material = (ISolid)ParameterFunctions[0].Result?.GetObject;
		}
		else if (ParameterFunctions[0].Result?.GetObject is decimal dvalue)
		{
			material = _gameworld.Materials.Get((long)dvalue);
		}
		else
		{
			material = _gameworld.Materials.GetByName(ParameterFunctions[0].Result?.GetObject?.ToString() ?? "none");
		}

		double weight;
		if (ParameterFunctions[1].Result?.GetObject is decimal wvalue)
		{
			weight = (double)wvalue;
		}
		else
		{
			weight = _gameworld.UnitManager.GetBaseUnits(
				ParameterFunctions[1].Result?.GetObject?.ToString() ?? "nothing", Framework.Units.UnitType.Mass,
				out var success);
			if (!success)
			{
				ErrorMessage = "Invalid Weight";
				return StatementResult.Error;
			}
		}

		if (material == null || weight <= 0.0)
		{
			ErrorMessage = "Material was null or weight was invalid";
			return StatementResult.Error;
		}

		ITag tag = null;
		if (ParameterFunctions.Count == 3)
		{
			if (ParameterFunctions[2].Result?.GetObject is decimal dvalue)
			{
				tag = _gameworld.Tags.Get((long)dvalue);
			}
			else
			{
				tag = _gameworld.Tags.GetByName(ParameterFunctions[2].Result?.GetObject?.ToString() ?? "none");
			}
		}

		var newItem = GameItems.Prototypes.CommodityGameItemComponentProto.CreateNewCommodity(material, weight, tag);
		_gameworld.Add(newItem);
		Result = newItem;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info."
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info."
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info."
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Number, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The ID of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The ID of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The name of the material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Number, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load in base units for this MUD. See MUD owner for configuration info.",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcommodity",
			new[] { ProgVariableTypes.Solid, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadCommodityFunction(pars, gameworld),
			new List<string> { "material", "weight", "tag" },
			new List<string>
			{
				"The material that you want to load",
				"The weight of the material to load, e.g. 15.3kg, 12lb etc",
				"The name of the tag that you want to load"
			},
			"This function loads a new commodity item into the game. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
			"Items",
			ProgVariableTypes.Item
		));
	}
}