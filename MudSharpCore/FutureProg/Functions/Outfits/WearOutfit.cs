using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Outfit },
                (pars, gameworld) => new WearOutfit(pars, gameworld, false),
                [
                    "character",
                    "outfit"
                ],
                [
                    "The character to wear the outfit",
                    "The outfit to be worn"
                ],
                "Has a character wear an outfit as if they'd type OUTFIT WEAR. Returns true if it succeeds. Can fail if items are missing.",
                "Outfits",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "wearoutfitforce",
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Outfit },
                (pars, gameworld) => new WearOutfit(pars, gameworld, true),
                [
                    "character",
                    "outfit"
                ],
                [
                    "The character to wear the outfit",
                    "The outfit to be worn"
                ],
                "Has a character wear an outfit as if they'd type OUTFIT WEAR. Returns true if it succeeds. Ignores missing items.",
                "Outfits",
                ProgVariableTypes.Boolean
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

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICharacter character = (ICharacter)ParameterFunctions[0].Result;
        if (character == null)
        {
            ErrorMessage = "Null character in WearOutfit";
            return StatementResult.Error;
        }

        IOutfit outfit = (IOutfit)ParameterFunctions[1].Result;
        if (outfit == null)
        {
            ErrorMessage = "Null outfit in WearOutfit";
            return StatementResult.Error;
        }

        int i = 1;
        InventoryPlanTemplate plan = new(Gameworld,
            from item in outfit.Items
            select new InventoryPlanPhaseTemplate(i++,
                new[]
                {
                    new InventoryPlanActionWear(Gameworld, 0, 0, x => x.Id == item.Id, null)
                        { DesiredProfile = item.DesiredProfile, OriginalReference = item.ItemDescription }
                })
        );

        IInventoryPlan charPlan = plan.CreatePlan(character);
        InventoryPlanFeasibility feasible = charPlan.PlanIsFeasible();
        List<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> infeasible = charPlan.InfeasibleActions().ToList();
        if (Force && feasible != InventoryPlanFeasibility.Feasible)
        {
            if (infeasible.Count == outfit.Items.Count())
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }
        }

        if (feasible == InventoryPlanFeasibility.Feasible || (feasible == InventoryPlanFeasibility.NotFeasibleMissingItems && Force))
        {
            IEnumerable<InventoryPlanActionResult> results = charPlan.ExecuteWholePlan();
            charPlan.FinalisePlanWithExemptions(results.Select(x => x.PrimaryTarget).ToList());
            Result = new BooleanVariable(true);
            return StatementResult.Normal;
        }

        Result = new BooleanVariable(false);
        return StatementResult.Normal;
    }
}