using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class SetExitSize : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "SetExitSize".ToLowerInvariant(),
                new[]
                {
                    ProgVariableTypes.Exit, ProgVariableTypes.OverlayPackage,
                    ProgVariableTypes.Number, ProgVariableTypes.Number
                },
                (pars, gameworld) => new SetExitSize(pars, gameworld),
                new List<string> { "exit", "overlay", "maximumSize", "uprightSize" },
                new List<string> { "The exit to edit or copy into the supplied overlay package.", "The editable overlay package where the change should be made.", "The maximum size enum value that can enter the exit at all.", "The maximum size enum value that can enter upright." },
                "Sets the maximum size and maximum upright size values on an exit overlay. Returns the edited exit or null if the exit/package is null or the package is not editable.",
                "Rooms",
                ProgVariableTypes.Exit
            )
        );
    }

    #endregion

    #region Constructors

    protected SetExitSize(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Exit;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        ICellExit exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
        if (exit == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        ICellOverlayPackage package = (ICellOverlayPackage)ParameterFunctions[1].Result?.GetObject;
        if (package == null)
        {
            Result = null;
            return StatementResult.Normal;
        }

        if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
        {
            Result = null;
            return StatementResult.Normal;
        }

        exit = GetOrCopyExit.GetOrCopy(exit, package);
        Result = exit;

        SizeCategory size = (SizeCategory)Convert.ToInt32(ParameterFunctions[2].Result?.GetObject ?? (int)SizeCategory.Titanic);
        if (!Enum.IsDefined(typeof(SizeCategory), size))
        {
            size = SizeCategory.Titanic;
        }

        SizeCategory upright =
            (SizeCategory)Convert.ToInt32(ParameterFunctions[3].Result?.GetObject ?? (int)SizeCategory.Titanic);
        if (!Enum.IsDefined(typeof(SizeCategory), upright))
        {
            upright = SizeCategory.Titanic;
        }

        exit.Exit.MaximumSizeToEnter = size;
        exit.Exit.MaximumSizeToEnterUpright = upright;
        exit.Exit.Changed = true;

        return StatementResult.Normal;
    }
}
