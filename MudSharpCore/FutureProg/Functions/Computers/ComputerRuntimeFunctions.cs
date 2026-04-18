#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Computers;

internal static class ComputerRuntimeFunctionContexts
{
	public static readonly FutureProgCompilationContext[] Both =
	[
		FutureProgCompilationContext.ComputerFunction,
		FutureProgCompilationContext.ComputerProgram
	];

	public static readonly FutureProgCompilationContext[] ProgramOnly =
	[
		FutureProgCompilationContext.ComputerProgram
	];
}

internal abstract class ComputerRuntimeBuiltInFunction : BuiltInFunction
{
	protected ComputerRuntimeBuiltInFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	protected static ComputerExecutionContext? CurrentContext => ComputerExecutionContextScope.Current;
}

internal class ReadFileFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"readfile",
			[ProgVariableTypes.Text],
			(pars, _) => new ReadFileFunction(pars),
			["filename"],
			["The file name to read from the current computer owner"],
			"Reads the full text contents of a file from the current computer owner.",
			"Computers",
			ProgVariableTypes.Text,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected ReadFileFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var fileName = ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty;
		var text = CurrentContext?.Owner.FileSystem?.ReadFile(fileName) ?? string.Empty;
		Result = new TextVariable(text);
		return StatementResult.Normal;
	}
}

internal class WriteFileFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"writefile",
			[ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, _) => new WriteFileFunction(pars),
			["filename", "text"],
			["The file name to write", "The replacement text contents"],
			"Overwrites or creates a file on the current computer owner and returns true if a writable file system was available.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected WriteFileFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var fileSystem = CurrentContext?.Owner.FileSystem;
		if (fileSystem is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		fileSystem.WriteFile(
			ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty,
			ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class AppendFileFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"appendfile",
			[ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, _) => new AppendFileFunction(pars),
			["filename", "text"],
			["The file name to append to", "The text to append"],
			"Appends text to a file on the current computer owner and returns true if a writable file system was available.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected AppendFileFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var fileSystem = CurrentContext?.Owner.FileSystem;
		if (fileSystem is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		fileSystem.AppendFile(
			ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty,
			ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class FileExistsFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"fileexists",
			[ProgVariableTypes.Text],
			(pars, _) => new FileExistsFunction(pars),
			["filename"],
			["The file name to test on the current computer owner"],
			"Returns true if the named file exists on the current computer owner.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected FileExistsFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var exists = CurrentContext?.Owner.FileSystem?.FileExists(
			ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty) ?? false;
		Result = new BooleanVariable(exists);
		return StatementResult.Normal;
	}
}

internal class GetFilesFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getfiles",
			Array.Empty<ProgVariableTypes>(),
			(pars, _) => new GetFilesFunction(pars),
			Array.Empty<string>(),
			Array.Empty<string>(),
			"Returns the file names available on the current computer owner.",
			"Computers",
			ProgVariableTypes.Text | ProgVariableTypes.Collection,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected GetFilesFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new CollectionVariable(
			(CurrentContext?.Owner.FileSystem?.Files ?? Enumerable.Empty<IComputerFile>())
			.Select(x => new TextVariable(x.FileName))
			.ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal class WriteTerminalFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"writeterminal",
			[ProgVariableTypes.Text],
			(pars, _) => new WriteTerminalFunction(pars),
			["text"],
			["The text to write to the current terminal session"],
			"Writes text to the currently active computer terminal session and returns true if a session was available.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected WriteTerminalFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var session = CurrentContext?.Session;
		if (session is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		session.User.OutputHandler.Send(ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty, nopage: true);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class ClearTerminalFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"clearterminal",
			Array.Empty<ProgVariableTypes>(),
			(pars, _) => new ClearTerminalFunction(pars),
			Array.Empty<string>(),
			Array.Empty<string>(),
			"Clears the currently active terminal session by writing a fresh blank screen and returns true if a session was available.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.Both));
	}

	protected ClearTerminalFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var session = CurrentContext?.Session;
		if (session is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		session.User.OutputHandler.Send(string.Concat(Enumerable.Repeat("\n", 40)), nopage: true);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class LaunchProgramFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"launchprogram",
			[ProgVariableTypes.Text],
			(pars, _) => new LaunchProgramFunction(pars),
			["program"],
			["The program name or id to launch on the current owner"],
			"Launches a computer program on the current owner and returns its process id, or 0 if launch failed.",
			"Computers",
			ProgVariableTypes.Number,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	protected LaunchProgramFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var context = CurrentContext;
		if (context?.Owner is null || context.Actor?.Gameworld?.ComputerExecutionService is null)
		{
			Result = new NumberVariable(0.0M);
			return StatementResult.Normal;
		}

		var executable = context.Actor.Gameworld.ComputerExecutionService.GetExecutable(
			context.Owner,
			ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty);
		if (executable is not IComputerProgramDefinition)
		{
			Result = new NumberVariable(0.0M);
			return StatementResult.Normal;
		}

		var result = context.Actor.Gameworld.ComputerExecutionService.Execute(
			context.Actor,
			context.Owner,
			executable,
			Array.Empty<object?>(),
			context.Session);
		Result = new NumberVariable(result.Process?.Id ?? 0L);
		return StatementResult.Normal;
	}
}

internal class KillProgramFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"killprogram",
			[ProgVariableTypes.Number],
			(pars, _) => new KillProgramFunction(pars),
			["processid"],
			["The process id to kill on the current owner"],
			"Kills a process on the current owner and returns true if that process was ended.",
			"Computers",
			ProgVariableTypes.Boolean,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	protected KillProgramFunction(IList<IFunction> parameterFunctions)
		: base(parameterFunctions)
	{
	}

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

		var context = CurrentContext;
		if (context?.Owner is null || context.Actor?.Gameworld?.ComputerExecutionService is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var processId = Convert.ToInt64((decimal?)(ParameterFunctions[0].Result?.GetObject) ?? 0.0M);
		Result = new BooleanVariable(
			context.Actor.Gameworld.ComputerExecutionService.KillProcess(context.Owner, processId, out _));
		return StatementResult.Normal;
	}
}
