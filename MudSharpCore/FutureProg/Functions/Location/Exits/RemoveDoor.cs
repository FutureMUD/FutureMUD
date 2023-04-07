using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class RemoveDoor : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"RemoveDoor".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Exit },
				(pars, gameworld) => new RemoveDoor(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected RemoveDoor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
		if (exit == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		if (exit.Exit.Door == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var door = exit.Exit.Door;
		exit.Exit.Door = null;
		exit.Exit.Changed = true;
		door.OpenDirectionCell = null;
		door.HingeCell = null;
		door.State = DoorState.Uninstalled;
		door.InstalledExit = null;

		Result = door.Parent;
		return StatementResult.Normal;
	}
}