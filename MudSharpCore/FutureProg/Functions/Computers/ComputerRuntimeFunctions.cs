#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
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

internal static class ComputerRuntimeSignalFunctionHelper
{
	public static bool TryResolveSignalSource(string sourceIdentifier, out ISignalSourceComponent? source,
		out LocalSignalBinding binding, out string error)
	{
		source = null;
		binding = new LocalSignalBinding(0L, string.Empty, 0L, string.Empty,
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);
		error = string.Empty;
		var context = ComputerExecutionContextScope.Current;
		if (context is null)
		{
			error = "The waitsignal function requires an active computer execution context.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(sourceIdentifier))
		{
			error = "You must specify a signal source component name.";
			return false;
		}

		var anchorItemId = context.Host.OwnerHostItemId ?? context.Owner.OwnerHostItemId;
		if (anchorItemId is not > 0L)
		{
			error = "The waitsignal function requires a real in-world computer host item.";
			return false;
		}

		var anchorItem = context.Gameworld.TryGetItem(anchorItemId.Value, true);
		if (anchorItem is null)
		{
			error = "The current execution host item is no longer available.";
			return false;
		}

		var identifier = sourceIdentifier.Trim();
		var endpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		var endpointIndex = identifier.LastIndexOf(':');
		if (endpointIndex > 0 && endpointIndex < identifier.Length - 1)
		{
			endpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(identifier[(endpointIndex + 1)..]);
			identifier = identifier[..endpointIndex].TrimEnd();
		}

		source = long.TryParse(identifier, out var componentId)
			? SignalComponentUtilities.FindSignalSourceOnItem(anchorItem, componentId, string.Empty, endpointKey)
			: SignalComponentUtilities.FindSignalSourceOnItem(anchorItem, 0L, identifier, endpointKey);
		if (source is null)
		{
			var anchorDescription = context.Actor is not null
				? anchorItem.HowSeen(context.Actor, true)
				: anchorItem.Name;
			error =
				$"There is no signal source component named {identifier.ColourCommand()} on {anchorDescription}.";
			return false;
		}

		binding = SignalComponentUtilities.CreateBinding(source, endpointKey);
		return true;
	}
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

internal class UserInputFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"userinput",
			Array.Empty<ProgVariableTypes>(),
			(pars, _) => new UserInputFunction(pars),
			Array.Empty<string>(),
			Array.Empty<string>(),
			"Suspends the current computer program until the connected terminal user types a line of input, then returns that line as text.",
			"Computers",
			ProgVariableTypes.Text,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	protected UserInputFunction(IList<IFunction> parameterFunctions)
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

		var context = CurrentContext;
		if (context?.Session is null)
		{
			ErrorMessage = "The userinput function requires an active computer terminal session.";
			return StatementResult.Error;
		}

		var pendingInput = context.ConsumePendingTerminalInput();
		if (pendingInput is not null)
		{
			Result = new TextVariable(pendingInput);
			return StatementResult.Normal;
		}

		if (context.Process is null)
		{
			ErrorMessage = "The userinput function requires a running computer-program process.";
			return StatementResult.Error;
		}

		throw new ComputerProgramWaitException(
			ComputerProcessWaitType.UserInput,
			ComputerProcessWaitArguments.CreateUserInput(
				context.Session.User.Id,
				context.Session.Terminal.TerminalItemId),
			context.Session.User.Id,
			context.Session.Terminal.TerminalItemId);
	}
}

internal class WaitSignalFunction : ComputerRuntimeBuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"waitsignal",
			[ProgVariableTypes.Text],
			(pars, _) => new WaitSignalFunction(pars),
			["source"],
			["The signal source component name, optionally suffixed with :endpoint"],
			"Suspends the current computer program until the named signal source on the execution host item emits a non-zero signal, then returns that numeric signal value.",
			"Computers",
			ProgVariableTypes.Number,
			allowedContexts: ComputerRuntimeFunctionContexts.ProgramOnly));
	}

	protected WaitSignalFunction(IList<IFunction> parameterFunctions)
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
		if (context?.Process is null)
		{
			ErrorMessage = "The waitsignal function requires a running computer-program process.";
			return StatementResult.Error;
		}

		var pendingSignal = context.ConsumePendingSignalInput();
		if (pendingSignal.HasValue)
		{
			Result = new NumberVariable(Convert.ToDecimal(pendingSignal.Value.Value));
			return StatementResult.Normal;
		}

		if (!ComputerRuntimeSignalFunctionHelper.TryResolveSignalSource(
			    ParameterFunctions[0].Result?.GetObject?.ToString() ?? string.Empty,
			    out _,
			    out var binding,
			    out var error))
		{
			ErrorMessage = error;
			return StatementResult.Error;
		}

		throw new ComputerProgramWaitException(
			ComputerProcessWaitType.Signal,
			ComputerProcessWaitArguments.CreateSignal(binding));
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
