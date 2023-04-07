using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class InstallDoor : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"InstallDoor".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Exit, FutureProgVariableTypes.Item, FutureProgVariableTypes.Location,
					FutureProgVariableTypes.Location
				},
				(pars, gameworld) => new InstallDoor(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected InstallDoor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
		if (exit == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var item = (IGameItem)ParameterFunctions[1].Result?.GetObject;
		if (item == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var door = item.GetItemType<IDoor>();
		if (door == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (exit.Exit.Door != null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var hingecell = (ICell)ParameterFunctions[2].Result?.GetObject;
		if (hingecell == null)
		{
			hingecell = exit.Origin;
		}

		var opencell = (ICell)ParameterFunctions[3].Result?.GetObject;
		if (opencell == null)
		{
			opencell = exit.Destination;
		}

		if (door.InstalledExit != null)
		{
			var otherExit = door.InstalledExit;
			otherExit.Door = null;
			otherExit.Changed = true;
			door.OpenDirectionCell = null;
			door.HingeCell = null;
			door.State = DoorState.Uninstalled;
			door.InstalledExit = null;
		}

		door.Parent.Get(null);
		door.InstalledExit = exit.Exit;
		door.HingeCell = hingecell;
		door.OpenDirectionCell = opencell;
		door.State = DoorState.Open;
		exit.Exit.Door = door;
		exit.Exit.Changed = true;

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}