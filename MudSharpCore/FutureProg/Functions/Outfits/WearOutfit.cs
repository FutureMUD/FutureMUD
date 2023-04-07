using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Inventory;

namespace MudSharp.FutureProg.Functions.Outfits;

internal class WearOutfit : BuiltInFunction
{
	public bool Force { get; private set; }
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"wearoutfit",
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Outfit },
				(pars, gameworld) => new WearOutfit(pars, gameworld, false)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"wearoutfitforce",
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Outfit },
				(pars, gameworld) => new WearOutfit(pars, gameworld, true)
			)
		);
	}

	#endregion

	#region Constructors

	protected WearOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool force) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		Force = force;
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

		var character = (ICharacter)ParameterFunctions[0].Result;
		if (character == null)
		{
			ErrorMessage = "Null character in WearOutfit";
			return StatementResult.Error;
		}

		var outfit = (IOutfit)ParameterFunctions[1].Result;
		if (outfit == null)
		{
			ErrorMessage = "Null outfit in WearOutfit";
			return StatementResult.Error;
		}

		var i = 1;
		var plan = new InventoryPlanTemplate(Gameworld,
			from item in outfit.Items
			select new InventoryPlanPhaseTemplate(i++,
				new[]
				{
					new InventoryPlanActionWear(Gameworld, 0, 0, x => x.Id == item.Id, null)
						{ DesiredProfile = item.DesiredProfile, OriginalReference = item.ItemDescription }
				})
		);

		var charPlan = plan.CreatePlan(character);
		var feasible = charPlan.PlanIsFeasible();
		var infeasible = charPlan.InfeasibleActions().ToList();
		if (Force && feasible != InventoryPlanFeasibility.Feasible)
		{
			if (infeasible.Count == outfit.Items.Count())
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		if (feasible == InventoryPlanFeasibility.Feasible ||
		    (feasible == InventoryPlanFeasibility.NotFeasibleMissingItems && Force))
		{
			var results = charPlan.ExecuteWholePlan();
			charPlan.FinalisePlanWithExemptions(results.Select(x => x.PrimaryTarget).ToList());
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}