#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Computers;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ComputerWorkspaceRuntimeTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void ComputerExecutableCompiler_AllowsSleepOnlyInPrograms()
	{
		var gameworld = new Mock<IFuturemud>();
		var (program, programError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"SleeperProgram",
			ComputerExecutableKind.Program,
			ProgVariableTypes.Number,
			Array.Empty<ComputerExecutableParameter>(),
			@"sleep 1s
return 42");

		Assert.IsNotNull(program, programError);
		Assert.AreEqual(string.Empty, programError);

		var (function, functionError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"SleeperFunction",
			ComputerExecutableKind.Function,
			ProgVariableTypes.Number,
			Array.Empty<ComputerExecutableParameter>(),
			@"sleep 1s
return 42");

		Assert.IsNull(function);
		StringAssert.Contains(functionError, "computer function compilation");
	}

	[TestMethod]
	public void ComputerExecutableCompiler_AllowsUserInputOnlyInPrograms()
	{
		var gameworld = new Mock<IFuturemud>();
		var (program, programError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"InteractiveProgram",
			ComputerExecutableKind.Program,
			ProgVariableTypes.Text,
			Array.Empty<ComputerExecutableParameter>(),
			@"return userinput()");

		Assert.IsNotNull(program, programError);
		Assert.AreEqual(string.Empty, programError);

		var (function, functionError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"InteractiveFunction",
			ComputerExecutableKind.Function,
			ProgVariableTypes.Text,
			Array.Empty<ComputerExecutableParameter>(),
			@"return userinput()");

		Assert.IsNull(function);
		StringAssert.Contains(functionError, "computer function compilation");
	}

	[TestMethod]
	public void ComputerExecutableCompiler_AllowsWaitSignalOnlyInPrograms()
	{
		var gameworld = new Mock<IFuturemud>();
		var (program, programError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"SignalProgram",
			ComputerExecutableKind.Program,
			ProgVariableTypes.Number,
			Array.Empty<ComputerExecutableParameter>(),
			@"return waitsignal(""WakeSensor"")");

		Assert.IsNotNull(program, programError);
		Assert.AreEqual(string.Empty, programError);

		var (function, functionError) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			"SignalFunction",
			ComputerExecutableKind.Function,
			ProgVariableTypes.Number,
			Array.Empty<ComputerExecutableParameter>(),
			@"return waitsignal(""WakeSensor"")");

		Assert.IsNull(function);
		StringAssert.Contains(functionError, "computer function compilation");
	}

	[TestMethod]
	public void ComputerHelpService_FiltersToComputerSafeMetadata()
	{
		var service = new ComputerHelpService();
		var types = service.GetAvailableTypes(FutureProgCompilationContext.ComputerProgram).ToHashSet();
		var functionStatements = service.GetStatementHelp(FutureProgCompilationContext.ComputerFunction).Select(x => x.Key)
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		var programStatements = service.GetStatementHelp(FutureProgCompilationContext.ComputerProgram).Select(x => x.Key)
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		var functions = service.GetFunctionHelp(FutureProgCompilationContext.ComputerProgram).ToList();

		Assert.IsTrue(types.Contains(ProgVariableTypes.Number));
		Assert.IsTrue(types.Contains(ProgVariableTypes.Collection));
		Assert.IsFalse(types.Contains(ProgVariableTypes.Character));
		Assert.IsFalse(functionStatements.Contains("sleep"));
		Assert.IsTrue(programStatements.Contains("sleep"));
		Assert.IsTrue(functions.Any(), "Expected at least one programming-safe built-in function.");
		Assert.IsFalse(functions.Any(x => x.FunctionName.EqualTo("LoadItem")));
		Assert.IsTrue(functions.Any(x => x.FunctionName.EqualTo("UserInput")));
		Assert.IsTrue(functions.Any(x => x.FunctionName.EqualTo("WaitSignal")));
		Assert.IsFalse(service.GetFunctionHelp(FutureProgCompilationContext.ComputerFunction)
			.Any(x => x.FunctionName.EqualTo("UserInput")));
		Assert.IsFalse(service.GetFunctionHelp(FutureProgCompilationContext.ComputerFunction)
			.Any(x => x.FunctionName.EqualTo("WaitSignal")));
		Assert.IsTrue(functions.All(x => x.AllowedContexts.Contains(FutureProgCompilationContext.ComputerProgram)));
		Assert.AreEqual("Both",
			ComputerHelpFormatter.DescribeAvailability(new[]
			{
				FutureProgCompilationContext.ComputerFunction,
				FutureProgCompilationContext.ComputerProgram
			}));
		Assert.AreEqual("Function",
			ComputerHelpFormatter.DescribeAvailability(new[] { FutureProgCompilationContext.ComputerFunction }));
		Assert.AreEqual("Program",
			ComputerHelpFormatter.DescribeAvailability(new[] { FutureProgCompilationContext.ComputerProgram }));
	}

	[TestMethod]
	public void ComputerProgramExecutor_CompletesProgramWithoutSleep()
	{
		var program = CompileProgram(
			"ImmediateProgram",
			@"return @value + 1",
			new ComputerExecutableParameter("value", ProgVariableTypes.Number));

		var outcome = ComputerProgramExecutor.Execute(program, new object?[] { 4.0m });

		Assert.AreEqual(ComputerProcessStatus.Completed, outcome.Status);
		Assert.AreEqual(5.0m, Convert.ToDecimal(outcome.Result));
		Assert.AreEqual(string.Empty, outcome.StateJson);
	}

	[TestMethod]
	public void ComputerProgramExecutor_SuspendsAndResumesProgramWithSleep()
	{
		var program = CompileProgram(
			"SleepingProgram",
			@"sleep 1s
return @value + 1",
			new ComputerExecutableParameter("value", ProgVariableTypes.Number));

		var suspended = ComputerProgramExecutor.Execute(program, new object?[] { 4.0m });

		Assert.AreEqual(ComputerProcessStatus.Sleeping, suspended.Status);
		Assert.AreEqual(ComputerProcessWaitType.Sleep, suspended.WaitType);
		Assert.IsTrue(suspended.WakeTimeUtc.HasValue);
		Assert.IsFalse(string.IsNullOrWhiteSpace(suspended.StateJson));

		var resumed = ComputerProgramExecutor.Execute(program, Enumerable.Empty<object?>(), suspended.StateJson);

		Assert.AreEqual(ComputerProcessStatus.Completed, resumed.Status);
		Assert.AreEqual(5.0m, Convert.ToDecimal(resumed.Result));
	}

	[TestMethod]
	public void ComputerExecutionService_ExecutesFunctionsImmediatelyWithoutPersistingProcesses()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var owner = CreateOwner(gameworld.Object);
			var service = new ComputerExecutionService(gameworld.Object);

			var executable = service.CreateExecutable(owner.Object, ComputerExecutableKind.Function, "Adder");
			var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
			runtimeExecutable.ReturnType = ProgVariableTypes.Number;
			runtimeExecutable.Parameters = new[]
			{
				new ComputerExecutableParameter("value", ProgVariableTypes.Number)
			};
			runtimeExecutable.SourceCode = @"return @value + 1";
			service.SaveExecutable(executable);

			var compile = service.CompileExecutable(executable);
			Assert.IsTrue(compile.Success, compile.ErrorMessage);

			var result = service.Execute(owner.Object, executable, new object?[] { 4.0m });

			Assert.IsTrue(result.Success, result.ErrorMessage);
			Assert.AreEqual(ComputerProcessStatus.Completed, result.Status);
			Assert.AreEqual(5.0m, Convert.ToDecimal(result.Result));
			Assert.IsNull(result.Process);
			Assert.AreEqual(0, context.CharacterComputerProgramProcesses.Count());
			scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Never);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerExecutionService_PersistsSleepingProgramsAndAllowsKill()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var capturedSchedules = new List<ISchedule>();
			var scheduler = new Mock<IScheduler>();
			scheduler.Setup(x => x.AddSchedule(It.IsAny<ISchedule>()))
				.Callback<ISchedule>(capturedSchedules.Add);
			var gameworld = CreateGameworld(scheduler);
			var owner = CreateOwner(gameworld.Object);
			var service = new ComputerExecutionService(gameworld.Object);

			var executable = service.CreateExecutable(owner.Object, ComputerExecutableKind.Program, "Sleeper");
			var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
			runtimeExecutable.ReturnType = ProgVariableTypes.Number;
			runtimeExecutable.SourceCode = @"sleep 1s
return 42";
			service.SaveExecutable(executable);

			var compile = service.CompileExecutable(executable);
			Assert.IsTrue(compile.Success, compile.ErrorMessage);

			var result = service.Execute(owner.Object, executable, Array.Empty<object?>());

			Assert.IsTrue(result.Success, result.ErrorMessage);
			Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
			Assert.IsNotNull(result.Process);
			Assert.AreEqual(1, capturedSchedules.Count);
			Assert.AreEqual(ScheduleType.ComputerProgram, capturedSchedules[0].Type);
			Assert.AreEqual(1, context.CharacterComputerProgramProcesses.Count());

			var persisted = context.CharacterComputerProgramProcesses.Single();
			Assert.AreEqual((int)ComputerProcessStatus.Sleeping, persisted.Status);
			Assert.AreEqual((int)ComputerProcessWaitType.Sleep, persisted.WaitType);
			Assert.IsFalse(string.IsNullOrWhiteSpace(persisted.StateJson));

			Assert.IsTrue(service.KillProcess(owner.Object, result.Process!.Id, out var killError), killError);

			Assert.AreEqual((int)ComputerProcessStatus.Killed, persisted.Status);
			Assert.AreEqual("Killed by user request.", persisted.LastError);
			scheduler.Verify(x => x.Destroy(It.Is<IFrameworkItem>(y => y.Id == result.Process.Id), ScheduleType.ComputerProgram),
				Times.AtLeastOnce);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerExecutionService_PersistsUserInputWaitMetadataForWorkspaceProcesses()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var owner = CreateOwner(gameworld.Object, 43L);
			var output = new Mock<IOutputHandler>();
			owner.SetupGet(x => x.OutputHandler).Returns(output.Object);
			var service = new ComputerExecutionService(gameworld.Object);
			var workspace = service.GetWorkspace(owner.Object);
			var host = workspace.ExecutionHost;
			var terminal = new Mock<IComputerTerminal>();
			terminal.SetupGet(x => x.TerminalItemId).Returns(501L);
			var session = new ComputerTerminalSession
			{
				User = owner.Object,
				Terminal = terminal.Object,
				Host = host,
				CurrentOwner = workspace
			};

			var executable = service.CreateExecutable(owner.Object, ComputerExecutableKind.Program, "Interactive");
			var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
			runtimeExecutable.ReturnType = ProgVariableTypes.Text;
			runtimeExecutable.SourceCode = @"return userinput()";
			service.SaveExecutable(executable);
			Assert.IsTrue(service.CompileExecutable(executable).Success);

			var result = service.Execute(owner.Object, workspace, executable, Array.Empty<object?>(), session);
			Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);

			var persisted = context.CharacterComputerProgramProcesses.Single();
			Assert.AreEqual((int)ComputerProcessWaitType.UserInput, persisted.WaitType);
			Assert.IsTrue(ComputerProcessWaitArguments.TryParseUserInput(persisted.WaitArgument, out var waitingCharacterId,
				out var waitingTerminalItemId));
			Assert.AreEqual(owner.Object.Id, waitingCharacterId);
			Assert.AreEqual(501L, waitingTerminalItemId);

			var reloadedService = new ComputerExecutionService(gameworld.Object);
			var reloadedProcess = reloadedService.GetProcesses(owner.Object).Single();
			Assert.AreEqual(ComputerProcessWaitType.UserInput, reloadedProcess.WaitType);
			Assert.AreEqual(owner.Object.Id, reloadedProcess.WaitingCharacterId);
			Assert.AreEqual(501L, reloadedProcess.WaitingTerminalItemId);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerExecutionService_RejectsExecutionOnUnpoweredHostOwner()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var actor = CreateOwner(gameworld.Object);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = false, Name = "Offline Host" };
		var owner = new StubComputerOwner(host, "Offline Host Storage");

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Function, "Adder");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Number;
		runtimeExecutable.Parameters = new[]
		{
			new ComputerExecutableParameter("value", ProgVariableTypes.Number)
		};
		runtimeExecutable.SourceCode = @"return @value + 1";
		service.SaveExecutable(owner, executable);

		var result = service.Execute(actor.Object, owner, executable, new object?[] { 4.0m });

		Assert.IsFalse(result.Success);
		Assert.AreEqual(ComputerProcessStatus.Failed, result.Status);
		StringAssert.Contains(result.ErrorMessage, "not currently powered");
	}

	[TestMethod]
	public void ComputerExecutionService_WriteTerminalFunction_UsesCurrentSession()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var output = new Mock<IOutputHandler>();
		var user = new Mock<ICharacter>();
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var terminal = new Mock<IComputerTerminal>();
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Function, "WriteTerminal");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Boolean;
		runtimeExecutable.SourceCode = @"return writeterminal(""hello from host"")";
		service.SaveExecutable(owner, executable);

		var compile = service.CompileExecutable(executable);
		Assert.IsTrue(compile.Success, compile.ErrorMessage);

		var result = service.Execute(null, owner, executable, Array.Empty<object?>(), session);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Completed, result.Status);
		Assert.AreEqual(true, result.Result);
		output.Verify(x => x.Send("hello from host", true, true), Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_UserInputProgram_SuspendsAndResumesFromTerminalInput()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var output = new Mock<IOutputHandler>();
		var user = CreateOwner(gameworld.Object, 44L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(101L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "Interactive");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Text;
		runtimeExecutable.SourceCode = @"writeterminal(""Enter text:"")
return userinput()";
		service.SaveExecutable(owner, executable);

		var compile = service.CompileExecutable(executable);
		Assert.IsTrue(compile.Success, compile.ErrorMessage);

		var result = service.Execute(user.Object, owner, executable, Array.Empty<object?>(), session);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, result.Process!.WaitType);
		Assert.AreEqual(user.Object.Id, result.Process.WaitingCharacterId);
		Assert.AreEqual(101L, result.Process.WaitingTerminalItemId);
		output.Verify(x => x.Send("Enter text:", true, true), Times.Once);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "hello", out var submitError), submitError);

		var process = owner.Processes.Single();
		Assert.AreEqual(ComputerProcessStatus.Completed, process.Status);
		Assert.AreEqual("hello", process.Result);
		output.Verify(x => x.Send("Enter text:", true, true), Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_UserInputProgram_FailsWithoutSession()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var user = CreateOwner(gameworld.Object, 45L);

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "Interactive");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Text;
		runtimeExecutable.SourceCode = @"return userinput()";
		service.SaveExecutable(owner, executable);

		var compile = service.CompileExecutable(executable);
		Assert.IsTrue(compile.Success, compile.ErrorMessage);

		var result = service.Execute(user.Object, owner, executable, Array.Empty<object?>());

		Assert.IsFalse(result.Success);
		Assert.AreEqual(ComputerProcessStatus.Failed, result.Status);
		StringAssert.Contains(result.ErrorMessage, "active computer terminal session");
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_RejectsWhenNothingIsWaiting()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var user = CreateOwner(gameworld.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(102L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};

		var result = service.TrySubmitTerminalInput(session, "hello", out var error);

		Assert.IsFalse(result);
		StringAssert.Contains(error, "waiting for terminal input");
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_RejectsWrongUserOrTerminal()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var output = new Mock<IOutputHandler>();
		var waitingUser = CreateOwner(gameworld.Object, 46L);
		waitingUser.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(103L);
		var session = new ComputerTerminalSession
		{
			User = waitingUser.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "Interactive");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Text;
		runtimeExecutable.SourceCode = @"return userinput()";
		service.SaveExecutable(owner, executable);
		Assert.IsTrue(service.CompileExecutable(executable).Success);
		var waitingResult = service.Execute(waitingUser.Object, owner, executable, Array.Empty<object?>(), session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, waitingResult.Status);

		var wrongUser = CreateOwner(gameworld.Object, 47L);
		var wrongUserSession = new ComputerTerminalSession
		{
			User = wrongUser.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};
		Assert.IsFalse(service.TrySubmitTerminalInput(wrongUserSession, "wrong", out var wrongUserError));
		StringAssert.Contains(wrongUserError, "waiting for terminal input");

		var wrongTerminal = new Mock<IComputerTerminal>();
		wrongTerminal.SetupGet(x => x.TerminalItemId).Returns(104L);
		var wrongTerminalSession = new ComputerTerminalSession
		{
			User = waitingUser.Object,
			Terminal = wrongTerminal.Object,
			Host = host,
			CurrentOwner = owner
		};
		Assert.IsFalse(service.TrySubmitTerminalInput(wrongTerminalSession, "wrong", out var wrongTerminalError));
		StringAssert.Contains(wrongTerminalError, "waiting for terminal input");
	}

	[TestMethod]
	public void ComputerExecutionService_UserInputProgram_RejectsSecondForegroundWaiterOnSameSession()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host" };
		var owner = new StubComputerOwner(host, "Local Host");
		var output = new Mock<IOutputHandler>();
		var user = CreateOwner(gameworld.Object, 48L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(105L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};

		var firstExecutable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "InteractiveOne");
		var firstRuntimeExecutable = (ComputerRuntimeExecutableBase)firstExecutable;
		firstRuntimeExecutable.ReturnType = ProgVariableTypes.Text;
		firstRuntimeExecutable.SourceCode = @"return userinput()";
		service.SaveExecutable(owner, firstExecutable);
		Assert.IsTrue(service.CompileExecutable(firstExecutable).Success);

		var secondExecutable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "InteractiveTwo");
		var secondRuntimeExecutable = (ComputerRuntimeExecutableBase)secondExecutable;
		secondRuntimeExecutable.ReturnType = ProgVariableTypes.Text;
		secondRuntimeExecutable.SourceCode = @"return userinput()";
		service.SaveExecutable(owner, secondExecutable);
		Assert.IsTrue(service.CompileExecutable(secondExecutable).Success);

		var firstResult = service.Execute(user.Object, owner, firstExecutable, Array.Empty<object?>(), session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, firstResult.Status);

		var secondResult = service.Execute(user.Object, owner, secondExecutable, Array.Empty<object?>(), session);

		Assert.IsFalse(secondResult.Success);
		Assert.AreEqual(ComputerProcessStatus.Failed, secondResult.Status);
		StringAssert.Contains(secondResult.ErrorMessage, "already waiting for input");
	}

	[TestMethod]
	public void ComputerExecutionService_WaitSignalProgram_SuspendsAndResumesFromSignal()
	{
		var scheduler = new Mock<IScheduler>();
		var (gameworld, _, signalSource, setSignal) = CreateGameworldWithSignalSource(scheduler, "WakeSensor");
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host", OwnerHostItemId = 1L };
		var owner = new StubComputerOwner(host, "Local Host");
		var user = CreateOwner(gameworld.Object, 49L);

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "SignalWaiter");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Number;
		runtimeExecutable.SourceCode = @"return waitsignal(""WakeSensor"")";
		service.SaveExecutable(owner, executable);
		Assert.IsTrue(service.CompileExecutable(executable).Success);

		var result = service.Execute(user.Object, owner, executable, Array.Empty<object?>());

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
		Assert.AreEqual(ComputerProcessWaitType.Signal, result.Process!.WaitType);

		setSignal(new ComputerSignal(1.0, null, null));
		signalSource.Raise(x => x.SignalChanged += null, signalSource.Object, new ComputerSignal(1.0, null, null));

		var process = owner.Processes.Single();
		Assert.AreEqual(ComputerProcessStatus.Completed, process.Status);
		Assert.AreEqual(1.0m, Convert.ToDecimal(process.Result));
	}

	[TestMethod]
	public void ComputerExecutionService_WaitSignalProgram_ResumesWhenOwnerReactivatesToActiveSignal()
	{
		var scheduler = new Mock<IScheduler>();
		var (gameworld, _, _, setSignal) = CreateGameworldWithSignalSource(scheduler, "WakeSensor");
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Local Host", OwnerHostItemId = 1L };
		var owner = new StubComputerOwner(host, "Local Host");
		var user = CreateOwner(gameworld.Object, 50L);

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "SignalWaiter");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Number;
		runtimeExecutable.SourceCode = @"return waitsignal(""WakeSensor"")";
		service.SaveExecutable(owner, executable);
		Assert.IsTrue(service.CompileExecutable(executable).Success);

		var result = service.Execute(user.Object, owner, executable, Array.Empty<object?>());
		Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
		var process = (ComputerRuntimeProcess)owner.Processes.Single();
		process.PowerLossBehaviour = ComputerPowerLossBehaviour.PersistSuspended;

		host.Powered = false;
		service.DeactivateOwner(owner);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, process.Status);
		Assert.AreEqual(ComputerProcessWaitType.Signal, process.WaitType);

		setSignal(new ComputerSignal(2.5, null, null));
		host.Powered = true;
		service.ActivateOwner(owner);

		Assert.AreEqual(ComputerProcessStatus.Completed, process.Status);
		Assert.AreEqual(2.5m, Convert.ToDecimal(process.Result));
	}

	[TestMethod]
	public void ComputerExecutionService_WaitSignalProgram_FailsWithoutRealHostItem()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost { Powered = true, Name = "Abstract Host", OwnerHostItemId = null };
		var owner = new StubComputerOwner(host, "Abstract Host");
		var user = CreateOwner(gameworld.Object, 51L);

		var executable = service.CreateExecutable(owner, ComputerExecutableKind.Program, "SignalWaiter");
		var runtimeExecutable = (ComputerRuntimeExecutableBase)executable;
		runtimeExecutable.ReturnType = ProgVariableTypes.Number;
		runtimeExecutable.SourceCode = @"return waitsignal(""WakeSensor"")";
		service.SaveExecutable(owner, executable);
		Assert.IsTrue(service.CompileExecutable(executable).Success);

		var result = service.Execute(user.Object, owner, executable, Array.Empty<object?>());

		Assert.IsFalse(result.Success);
		Assert.AreEqual(ComputerProcessStatus.Failed, result.Status);
		StringAssert.Contains(result.ErrorMessage, "real in-world computer host item");
	}

	[TestMethod]
	public void ComputerExecutionService_BuiltInApplications_AreAvailableOnHostsButNotWorkspaces()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var owner = CreateOwner(gameworld.Object, 60L);
		var workspaceHost = new CharacterWorkspaceHost(
			gameworld.Object,
			owner.Object.Id,
			() => Enumerable.Empty<IComputerExecutableDefinition>(),
			() => Enumerable.Empty<IComputerProcess>());
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var storageOwner = new StubComputerOwner(host, "Archive Drive");

		Assert.IsFalse(service.GetBuiltInApplications(workspaceHost).Any());
		var applications = service.GetBuiltInApplications(storageOwner).ToList();
		Assert.IsTrue(applications.Any(x => x.ApplicationId == "directory"));
		Assert.IsTrue(applications.Any(x => x.ApplicationId == "sysmon"));
		Assert.IsNotNull(service.GetBuiltInApplication(storageOwner, "sys"));
		Assert.IsNotNull(service.GetBuiltInApplication(storageOwner, "dir"));
	}

	[TestMethod]
	public void ComputerExecutionService_ExecuteBuiltInApplication_SysMonWritesToTerminalAndCreatesHostProcess()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var owner = new StubComputerOwner(host, "Archive Drive");
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 61L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1201L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = owner
		};
		var application = service.GetBuiltInApplication(owner, "sysmon");

		Assert.IsNotNull(application);
		var result = service.ExecuteBuiltInApplication(user.Object, owner, application!, session);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Completed, result.Status);
		Assert.IsNotNull(result.Process);
		Assert.IsNotNull(host.GetProcess(result.Process!.Id));
		Assert.IsFalse(owner.Processes.Any(x => x.Id == result.Process.Id));
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("SysMon") && s.Contains("Processes:")),
				true,
				true),
			Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_ExecuteBuiltInApplication_RequiresTerminalSession()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var owner = new StubComputerOwner(host, "Archive Drive");
		var user = CreateOwner(gameworld.Object, 62L);
		var application = service.GetBuiltInApplication(owner, "sysmon");

		Assert.IsNotNull(application);
		var result = service.ExecuteBuiltInApplication(user.Object, owner, application!, null);

		Assert.IsFalse(result.Success);
		Assert.AreEqual(ComputerProcessStatus.Failed, result.Status);
		StringAssert.Contains(result.ErrorMessage, "terminal session");
	}

	[TestMethod]
	public void ComputerExecutionService_ExecuteBuiltInApplication_FileManager_SuspendsForTerminalInput()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		host.FileSystemStorage.WriteFile("readme.txt", "hello");
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 63L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1202L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = host
		};
		var application = service.GetBuiltInApplication(host, "filemanager");

		Assert.IsNotNull(application);
		var result = service.ExecuteBuiltInApplication(user.Object, host, application!, session);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
		Assert.IsNotNull(result.Process);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, result.Process!.WaitType);
		Assert.AreEqual(user.Object.Id, result.Process.WaitingCharacterId);
		Assert.AreEqual(1202L, result.Process.WaitingTerminalItemId);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("FileManager") && s.Contains("readme.txt")),
				true,
				true),
			Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_ResumesBuiltInFileManagerAndCopiesFiles()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var storage = new StubComputerOwner(host, "Archive Drive")
		{
			OwnerStorageItemIdValue = 99L,
			FileSystemStorage = new ComputerMutableFileSystem(2048)
		};
		storage.FileSystemStorage.WriteFile("notes.txt", "hello world");
		host.MountedStorage = new[] { storage };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 64L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1203L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = storage
		};
		var application = service.GetBuiltInApplication(storage, "filemanager");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, storage, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);
		Assert.IsNotNull(start.Process);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, start.Process!.WaitType);
		Assert.AreEqual(user.Object.Id, start.Process.WaitingCharacterId);
		Assert.AreEqual(1203L, start.Process.WaitingTerminalItemId);
		var waitingProcess = host.GetProcess(start.Process.Id);
		Assert.IsNotNull(waitingProcess);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, waitingProcess!.Status);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, waitingProcess.WaitType);
		Assert.AreEqual(user.Object.Id, waitingProcess.WaitingCharacterId);
		Assert.AreEqual(1203L, waitingProcess.WaitingTerminalItemId);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "copy notes.txt host", out var copyError), copyError);
		Assert.AreEqual("hello world", host.FileSystem!.ReadFile("notes.txt"));
		var liveProcess = host.GetProcess(start.Process!.Id);
		Assert.IsNotNull(liveProcess);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, liveProcess!.Status);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, liveProcess.WaitType);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "exit", out var exitError), exitError);
		Assert.AreEqual(ComputerProcessStatus.Completed, host.GetProcess(start.Process.Id)!.Status);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("Copied") && s.Contains("notes.txt")),
				true,
				true),
			Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_FileManagerEditUsesEditorModeAndSavesOnSubmit()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var storage = new StubComputerOwner(host, "Archive Drive")
		{
			OwnerStorageItemIdValue = 99L,
			FileSystemStorage = new ComputerMutableFileSystem(2048)
		};
		storage.FileSystemStorage.WriteFile("notes.txt", "hello world");
		host.MountedStorage = new[] { storage };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 65L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		Action<string, IOutputHandler, object[]>? postAction = null;
		Action<IOutputHandler, object[]>? cancelAction = null;
		string? capturedRecallText = null;
		EditorOptions capturedOptions = EditorOptions.None;
		user.Setup(x => x.EditorMode(
				It.IsAny<Action<string, IOutputHandler, object[]>>(),
				It.IsAny<Action<IOutputHandler, object[]>>(),
				It.IsAny<double>(),
				It.IsAny<string>(),
				It.IsAny<EditorOptions>(),
				It.IsAny<object[]>()))
			.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double, string, EditorOptions, object[]>(
				(post, cancel, _, recall, options, _) =>
				{
					postAction = post;
					cancelAction = cancel;
					capturedRecallText = recall;
					capturedOptions = options;
				});
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1204L);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = storage
		};
		var application = service.GetBuiltInApplication(storage, "filemanager");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, storage, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "edit notes.txt", out var editError), editError);
		Assert.IsNotNull(postAction);
		Assert.IsNotNull(cancelAction);
		Assert.AreEqual("hello world", capturedRecallText);
		Assert.IsTrue(capturedOptions.HasFlag(EditorOptions.PermitEmpty));
		Assert.AreEqual("hello world", storage.FileSystem!.ReadFile("notes.txt"));

		var liveProcess = host.GetProcess(start.Process!.Id);
		Assert.IsNotNull(liveProcess);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, liveProcess!.Status);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, liveProcess.WaitType);

		postAction!("updated text", output.Object, Array.Empty<object>());
		Assert.AreEqual("updated text", storage.FileSystem.ReadFile("notes.txt"));
	}

	[TestMethod]
	public void ComputerExecutionService_ExecuteBuiltInApplication_Directory_SuspendsForTerminalInput()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var storage = new StubComputerOwner(host, "Archive Drive")
		{
			OwnerStorageItemIdValue = 77L,
			FileSystemStorage = new ComputerMutableFileSystem(1024)
		};
		host.MountedStorage = new[] { storage };
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1205L);
		terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
		host.ConnectedTerminals = new[] { terminal.Object };
		var adapter = new Mock<INetworkAdapter>();
		adapter.SetupGet(x => x.Powered).Returns(true);
		adapter.SetupGet(x => x.NetworkReady).Returns(true);
		adapter.SetupGet(x => x.NetworkAddress).Returns("local.host");
		host.NetworkAdapters = new[] { adapter.Object };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 66L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = host
		};
		var application = service.GetBuiltInApplication(host, "directory");

		Assert.IsNotNull(application);
		var result = service.ExecuteBuiltInApplication(user.Object, host, application!, session);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, result.Status);
		Assert.IsNotNull(result.Process);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, result.Process!.WaitType);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("Directory") && s.Contains("Local Services") && s.Contains("Connected Terminals")),
				true,
				true),
			Times.Once);
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_ResumesBuiltInDirectoryAndShowsServices()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 1L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(1L).ToList()
		};
		var storage = new StubComputerOwner(host, "Archive Drive")
		{
			OwnerStorageItemIdValue = 88L,
			FileSystemStorage = new ComputerMutableFileSystem(1024)
		};
		host.MountedStorage = new[] { storage };
		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1206L);
		terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
		host.ConnectedTerminals = new[] { terminal.Object };
		var adapter = new Mock<INetworkAdapter>();
		adapter.SetupGet(x => x.Powered).Returns(true);
		adapter.SetupGet(x => x.NetworkReady).Returns(false);
		adapter.SetupGet(x => x.NetworkAddress).Returns("local.dir");
		host.NetworkAdapters = new[] { adapter.Object };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 67L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = host
		};
		var application = service.GetBuiltInApplication(host, "directory");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, host, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "services", out var servicesError), servicesError);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("Local Services") && s.Contains("Directory") && s.Contains("FileManager") && s.Contains("SysMon")),
				true,
				true),
			Times.AtLeastOnce);

		var liveProcess = host.GetProcess(start.Process!.Id);
		Assert.IsNotNull(liveProcess);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, liveProcess!.Status);
		Assert.AreEqual(ComputerProcessWaitType.UserInput, liveProcess.WaitType);
	}

	[TestMethod]
	public void TelecommunicationsGrid_GetCanonicalNetworkAddress_FallsBackWhenPreferredAddressCollides()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var hostA = new StubComputerHost { Powered = true, Name = "Host A", OwnerHostItemId = 11L };
		var hostB = new StubComputerHost { Powered = true, Name = "Host B", OwnerHostItemId = 12L };
		var adapterA = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 201L,
			ConnectedHost = hostA,
			Powered = true,
			PreferredNetworkAddress = "relay",
			TelecommunicationsGrid = grid
		};
		var adapterB = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 202L,
			ConnectedHost = hostB,
			Powered = true,
			PreferredNetworkAddress = "relay",
			TelecommunicationsGrid = grid
		};

		grid.JoinGrid(adapterA);
		grid.JoinGrid(adapterB);

		Assert.AreEqual("adapter-201", grid.GetCanonicalNetworkAddress(adapterA));
		Assert.AreEqual("adapter-202", grid.GetCanonicalNetworkAddress(adapterB));
	}

	[TestMethod]
	public void TelecommunicationsGrid_GetReachableNetworkEndpoints_TraversesLinkedGridsWithoutDuplicates()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var localGrid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var linkedGrid = new TelecommunicationsGrid(gameworld.Object, null, "556", 4);
		var farGrid = new TelecommunicationsGrid(gameworld.Object, null, "557", 4);
		localGrid.LinkGrid(linkedGrid);
		linkedGrid.LinkGrid(farGrid);
		farGrid.LinkGrid(localGrid);

		var localHost = new StubComputerHost { Powered = true, Name = "Local Host", OwnerHostItemId = 21L };
		var linkedHost = new StubComputerHost { Powered = true, Name = "Linked Host", OwnerHostItemId = 22L };
		var farHost = new StubComputerHost { Powered = true, Name = "Far Host", OwnerHostItemId = 23L };
		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 301L,
			ConnectedHost = localHost,
			Powered = true,
			PreferredNetworkAddress = "local.host",
			TelecommunicationsGrid = localGrid
		};
		var linkedAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 302L,
			ConnectedHost = linkedHost,
			Powered = true,
			PreferredNetworkAddress = "linked.host",
			TelecommunicationsGrid = linkedGrid
		};
		var farAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 303L,
			ConnectedHost = farHost,
			Powered = true,
			PreferredNetworkAddress = "far.host",
			TelecommunicationsGrid = farGrid
		};

		localGrid.JoinGrid(localAdapter);
		linkedGrid.JoinGrid(linkedAdapter);
		farGrid.JoinGrid(farAdapter);

		var endpoints = localGrid.GetReachableNetworkEndpoints().ToList();

		Assert.AreEqual(3, endpoints.Count);
		Assert.AreEqual(1, endpoints.Count(x => x.IsLocalGrid));
		Assert.AreEqual(1, endpoints.Count(x => x.CanonicalAddress == "local.host"));
		Assert.AreEqual(1, endpoints.Count(x => x.CanonicalAddress == "linked.host"));
		Assert.AreEqual(1, endpoints.Count(x => x.CanonicalAddress == "far.host"));
	}

	[TestMethod]
	public void ComputerExecutionService_GetReachableHosts_ExcludesOfflineHostsAndUnimplementedNetworkServices()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
		var localGrid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var linkedGrid = new TelecommunicationsGrid(gameworld.Object, null, "556", 4);
		localGrid.LinkGrid(linkedGrid);

		var localHost = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 31L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(31L).ToList()
		};
		var remoteHost = new StubComputerHost
		{
			Powered = true,
			Name = "Remote Host",
			OwnerHostItemId = 32L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(32L).ToList()
		};
		var offlineHost = new StubComputerHost
		{
			Powered = false,
			Name = "Offline Host",
			OwnerHostItemId = 33L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(33L).ToList()
		};
		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 401L,
			ConnectedHost = localHost,
			Powered = true,
			PreferredNetworkAddress = "local.host",
			TelecommunicationsGrid = localGrid
		};
		var remoteAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 402L,
			ConnectedHost = remoteHost,
			Powered = true,
			PreferredNetworkAddress = "remote.host",
			TelecommunicationsGrid = linkedGrid
		};
		var offlineAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 403L,
			ConnectedHost = offlineHost,
			Powered = true,
			PreferredNetworkAddress = "offline.host",
			TelecommunicationsGrid = linkedGrid
		};

		localGrid.JoinGrid(localAdapter);
		linkedGrid.JoinGrid(remoteAdapter);
		linkedGrid.JoinGrid(offlineAdapter);
		localHost.NetworkAdapters = new[] { localAdapter };
		remoteHost.NetworkAdapters = new[] { remoteAdapter };
		offlineHost.NetworkAdapters = new[] { offlineAdapter };

		var hosts = service.GetReachableHosts(localHost).ToList();

		Assert.IsTrue(hosts.Any(x => ReferenceEquals(x.Host, localHost) && x.IsLocalGrid));
		Assert.IsTrue(hosts.Any(x => ReferenceEquals(x.Host, remoteHost) && !x.IsLocalGrid));
		Assert.IsFalse(hosts.Any(x => ReferenceEquals(x.Host, offlineHost)));
		Assert.AreEqual(0, service.GetAdvertisedServices(localHost, remoteHost).Count());
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_DirectoryHostsAndRemoteServicesUseNetworkDiscovery()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
		var localGrid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var linkedGrid = new TelecommunicationsGrid(gameworld.Object, null, "556", 4);
		localGrid.LinkGrid(linkedGrid);

		var host = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 41L,
			FileSystemStorage = new ComputerMutableFileSystem(4096),
			BuiltInApplications = ComputerBuiltInApplications.ForHost(41L).ToList()
		};
		var remoteHost = new StubComputerHost
		{
			Powered = true,
			Name = "Remote Host",
			OwnerHostItemId = 42L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(42L).ToList()
		};
		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 501L,
			ConnectedHost = host,
			Powered = true,
			PreferredNetworkAddress = "local.host",
			TelecommunicationsGrid = localGrid
		};
		var remoteAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 502L,
			ConnectedHost = remoteHost,
			Powered = true,
			PreferredNetworkAddress = "remote.host",
			TelecommunicationsGrid = linkedGrid
		};

		localGrid.JoinGrid(localAdapter);
		linkedGrid.JoinGrid(remoteAdapter);
		host.NetworkAdapters = new[] { localAdapter };
		remoteHost.NetworkAdapters = new[] { remoteAdapter };

		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1301L);
		terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
		host.ConnectedTerminals = new[] { terminal.Object };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 68L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = host,
			CurrentOwner = host
		};
		var application = service.GetBuiltInApplication(host, "directory");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, host, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "hosts", out var hostsError), hostsError);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("Reachable Hosts") && s.Contains("Remote Host") &&
				                   s.Contains("remote.host") && s.Contains("linked")),
				true,
				true),
			Times.AtLeastOnce);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "services remote.host", out var servicesError), servicesError);
		output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("Advertised Services for") &&
				                   s.Contains("Remote Host") &&
				                   s.Contains("does not currently advertise any implemented network services")),
				true,
				true),
			Times.AtLeastOnce);
	}

	[TestMethod]
	public void ComputerMailService_RegisterDomainCreateAccountAndAuthenticate_Succeeds()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var host = new StubComputerHost
			{
				Powered = true,
				Name = "Mail Host",
				OwnerHostItemId = 801L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(801L).ToList()
			};
			host.SetNetworkServiceEnabled("mail", true, out _);

			var mailService = gameworld.Object.ComputerMailService;
			Assert.IsTrue(mailService.RegisterDomain(host, "alpha.example", out var domainError), domainError);
			Assert.IsTrue(mailService.CreateAccount(host, "alice@alpha.example", "secret", out var accountError), accountError);

			var authentication = mailService.Authenticate(host, "alice@alpha.example", "secret");

			Assert.IsTrue(authentication.Success, authentication.ErrorMessage);
			Assert.IsNotNull(authentication.Account);
			Assert.AreEqual("alice@alpha.example", authentication.Account!.Address);
			Assert.AreEqual(1, context.ComputerMailDomains.Count());
			Assert.AreEqual(1, context.ComputerMailAccounts.Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerMailService_SendMessageToReachableRemoteMailbox_CreatesInboxAndSentCopies()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var service = new ComputerExecutionService(gameworld.Object);
			gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
			var localGrid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
			var remoteGrid = new TelecommunicationsGrid(gameworld.Object, null, "556", 4);
			localGrid.LinkGrid(remoteGrid);

			var localHost = new StubComputerHost
			{
				Powered = true,
				Name = "Local Mail Host",
				OwnerHostItemId = 811L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(811L).ToList()
			};
			var remoteHost = new StubComputerHost
			{
				Powered = true,
				Name = "Remote Mail Host",
				OwnerHostItemId = 812L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(812L).ToList()
			};
			localHost.SetNetworkServiceEnabled("mail", true, out _);
			remoteHost.SetNetworkServiceEnabled("mail", true, out _);

			var localAdapter = new StubNetworkAdapter
			{
				NetworkAdapterItemId = 821L,
				ConnectedHost = localHost,
				Powered = true,
				PreferredNetworkAddress = "local.mail",
				TelecommunicationsGrid = localGrid
			};
			var remoteAdapter = new StubNetworkAdapter
			{
				NetworkAdapterItemId = 822L,
				ConnectedHost = remoteHost,
				Powered = true,
				PreferredNetworkAddress = "remote.mail",
				TelecommunicationsGrid = remoteGrid
			};
			localGrid.JoinGrid(localAdapter);
			remoteGrid.JoinGrid(remoteAdapter);
			localHost.NetworkAdapters = new[] { localAdapter };
			remoteHost.NetworkAdapters = new[] { remoteAdapter };

			var mailService = gameworld.Object.ComputerMailService;
			Assert.IsTrue(mailService.RegisterDomain(localHost, "local.example", out var localDomainError), localDomainError);
			Assert.IsTrue(mailService.RegisterDomain(remoteHost, "remote.example", out var remoteDomainError), remoteDomainError);
			Assert.IsTrue(mailService.CreateAccount(localHost, "alice@local.example", "secret", out var localAccountError), localAccountError);
			Assert.IsTrue(mailService.CreateAccount(remoteHost, "bob@remote.example", "secret", out var remoteAccountError), remoteAccountError);

			var authentication = mailService.Authenticate(localHost, "alice@local.example", "secret");
			Assert.IsTrue(authentication.Success, authentication.ErrorMessage);

			Assert.IsTrue(mailService.SendMessage(
					localHost,
					authentication.Account!,
					"bob@remote.example",
					"Test Subject",
					"Test Body",
					out var sendError),
				sendError);

			Assert.AreEqual(1, context.ComputerMailMessages.Count());
			Assert.AreEqual(2, context.ComputerMailMailboxEntries.Count());
			var inboxEntry = context.ComputerMailMailboxEntries.Single(x => !x.IsSentFolder);
			var sentEntry = context.ComputerMailMailboxEntries.Single(x => x.IsSentFolder);
			Assert.IsFalse(inboxEntry.IsRead);
			Assert.IsTrue(sentEntry.IsRead);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerExecutionService_GetAdvertisedServices_IncludesMailDomainsWhenEnabled()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var service = new ComputerExecutionService(gameworld.Object);
			gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
			var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
			var sourceHost = new StubComputerHost
			{
				Powered = true,
				Name = "Source Host",
				OwnerHostItemId = 831L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(831L).ToList()
			};
			var targetHost = new StubComputerHost
			{
				Powered = true,
				Name = "Target Host",
				OwnerHostItemId = 832L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(832L).ToList()
			};
			targetHost.SetNetworkServiceEnabled("mail", true, out _);

			var sourceAdapter = new StubNetworkAdapter
			{
				NetworkAdapterItemId = 841L,
				ConnectedHost = sourceHost,
				Powered = true,
				PreferredNetworkAddress = "source.host",
				TelecommunicationsGrid = grid
			};
			var targetAdapter = new StubNetworkAdapter
			{
				NetworkAdapterItemId = 842L,
				ConnectedHost = targetHost,
				Powered = true,
				PreferredNetworkAddress = "target.host",
				TelecommunicationsGrid = grid
			};
			grid.JoinGrid(sourceAdapter);
			grid.JoinGrid(targetAdapter);
			sourceHost.NetworkAdapters = new[] { sourceAdapter };
			targetHost.NetworkAdapters = new[] { targetAdapter };

			Assert.IsTrue(gameworld.Object.ComputerMailService.RegisterDomain(targetHost, "mail.example", out var domainError), domainError);

			var services = service.GetAdvertisedServices(sourceHost, targetHost).ToList();

			Assert.AreEqual(1, services.Count);
			Assert.AreEqual("mail", services[0].ApplicationId);
			CollectionAssert.AreEqual(new[] { "mail.example" }, services[0].ServiceDetails.ToArray());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_MailAppLogsInAndReadsMailbox()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var scheduler = new Mock<IScheduler>();
			var gameworld = CreateGameworld(scheduler);
			var service = new ComputerExecutionService(gameworld.Object);
			gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
			var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
			var host = new StubComputerHost
			{
				Powered = true,
				Name = "Mail Host",
				OwnerHostItemId = 851L,
				BuiltInApplications = ComputerBuiltInApplications.ForHost(851L).ToList()
			};
			host.SetNetworkServiceEnabled("mail", true, out _);
			var adapter = new StubNetworkAdapter
			{
				NetworkAdapterItemId = 861L,
				ConnectedHost = host,
				Powered = true,
				PreferredNetworkAddress = "mail.host",
				TelecommunicationsGrid = grid
			};
			grid.JoinGrid(adapter);
			host.NetworkAdapters = new[] { adapter };

			var mailService = gameworld.Object.ComputerMailService;
			Assert.IsTrue(mailService.RegisterDomain(host, "mail.example", out var domainError), domainError);
			Assert.IsTrue(mailService.CreateAccount(host, "alice@mail.example", "secret", out var accountError), accountError);
			var authentication = mailService.Authenticate(host, "alice@mail.example", "secret");
			Assert.IsTrue(authentication.Success, authentication.ErrorMessage);
			Assert.IsTrue(mailService.SendMessage(
					host,
					authentication.Account!,
					"alice@mail.example",
					"Welcome",
					"Hello from the mail system",
					out var sendError),
				sendError);
			var inboxEntryId = context.ComputerMailMailboxEntries
				.AsNoTracking()
				.Single(x => !x.IsSentFolder)
				.Id;

			var terminal = new Mock<IComputerTerminal>();
			terminal.SetupGet(x => x.TerminalItemId).Returns(1701L);
			terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
			host.ConnectedTerminals = new[] { terminal.Object };
			var output = new Mock<IOutputHandler>();
			var account = new Mock<IAccount>();
			account.SetupGet(x => x.UseUnicode).Returns(false);
			var user = CreateOwner(gameworld.Object, 88L);
			user.SetupGet(x => x.OutputHandler).Returns(output.Object);
			user.SetupGet(x => x.LineFormatLength).Returns(120);
			user.SetupGet(x => x.Account).Returns(account.Object);
			var session = new ComputerTerminalSession
			{
				User = user.Object,
				Terminal = terminal.Object,
				Host = host,
				CurrentOwner = host
			};
			var application = service.GetBuiltInApplication(host, "mail");

			Assert.IsNotNull(application);
			var start = service.ExecuteBuiltInApplication(user.Object, host, application!, session);
			Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

			Assert.IsTrue(service.TrySubmitTerminalInput(session, "login alice@mail.example secret", out var loginError), loginError);
			output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("log in to") && s.Contains("alice@mail.example")),
				true,
				true), Times.AtLeastOnce);

			Assert.IsTrue(service.TrySubmitTerminalInput(session, "inbox", out var inboxError), inboxError);
			output.Verify(x => x.Send(
				It.Is<string>(s => s.Contains("mailbox:") && s.Contains("Welcome")),
				true,
				true), Times.AtLeastOnce);

			Assert.IsTrue(service.TrySubmitTerminalInput(session, $"read {inboxEntryId}", out var readError), readError);
			context.ChangeTracker.Clear();
			Assert.IsTrue(context.ComputerMailMailboxEntries
				.AsNoTracking()
				.Single(x => x.Id == inboxEntryId)
				.IsRead);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void ComputerFileTransferService_GetAdvertisedServiceDetails_ReportsPublicFileCount()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var host = new StubComputerHost
		{
			Powered = true,
			Name = "FTP Host",
			OwnerHostItemId = 901L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(901L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(1000L)
		};
		host.SetNetworkServiceEnabled("ftp", true, out _);
		host.FileSystemStorage!.WriteFile("public.txt", "hello");
		host.FileSystemStorage.SetFilePubliclyAccessible("public.txt", true);
		host.FileSystemStorage.WriteFile("private.txt", "secret");

		var details = gameworld.Object.ComputerFileTransferService.GetAdvertisedServiceDetails(host, "ftp").ToList();

		Assert.AreEqual(1, details.Count);
		Assert.AreEqual("1 public file", details[0]);
	}

	[TestMethod]
	public void ComputerFileTransferService_PublicFiles_AreReachableAcrossNetwork()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var localHost = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 911L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(911L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(1000L)
		};
		var remoteHost = new StubComputerHost
		{
			Powered = true,
			Name = "Remote Host",
			OwnerHostItemId = 912L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(912L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(1000L)
		};
		remoteHost.SetNetworkServiceEnabled("ftp", true, out _);
		remoteHost.FileSystemStorage!.WriteFile("public.txt", "public body");
		remoteHost.FileSystemStorage.SetFilePubliclyAccessible("public.txt", true);
		remoteHost.FileSystemStorage.WriteFile("private.txt", "private body");

		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 921L,
			ConnectedHost = localHost,
			Powered = true,
			PreferredNetworkAddress = "local.ftp",
			TelecommunicationsGrid = grid
		};
		var remoteAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 922L,
			ConnectedHost = remoteHost,
			Powered = true,
			PreferredNetworkAddress = "remote.ftp",
			TelecommunicationsGrid = grid
		};
		grid.JoinGrid(localAdapter);
		grid.JoinGrid(remoteAdapter);
		localHost.NetworkAdapters = new[] { localAdapter };
		remoteHost.NetworkAdapters = new[] { remoteAdapter };

		var files = gameworld.Object.ComputerFileTransferService.GetFiles(localHost, remoteHost, null, null, out var listError)
			.ToList();
		Assert.AreEqual(string.Empty, listError);
		Assert.AreEqual(1, files.Count);
		Assert.AreEqual("public.txt", files[0].FileName);

		var publicFile = gameworld.Object.ComputerFileTransferService.ReadFile(localHost, remoteHost, null, null,
			"public.txt", out var publicError);
		Assert.IsNotNull(publicFile, publicError);
		Assert.AreEqual("public body", publicFile.TextContents);

		var privateFile = gameworld.Object.ComputerFileTransferService.ReadFile(localHost, remoteHost, null, null,
			"private.txt", out var privateError);
		Assert.IsNull(privateFile);
		StringAssert.Contains(privateError, "does not expose a readable file");
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_FileManager_CanReadAndCopyPublicRemoteFiles()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var localHost = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 931L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(931L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(5000L)
		};
		var remoteHost = new StubComputerHost
		{
			Powered = true,
			Name = "Remote Host",
			OwnerHostItemId = 932L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(932L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(5000L)
		};
		remoteHost.SetNetworkServiceEnabled("ftp", true, out _);
		remoteHost.FileSystemStorage!.WriteFile("public.txt", "Hello from the network");
		remoteHost.FileSystemStorage.SetFilePubliclyAccessible("public.txt", true);

		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 941L,
			ConnectedHost = localHost,
			Powered = true,
			PreferredNetworkAddress = "local.file",
			TelecommunicationsGrid = grid
		};
		var remoteAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 942L,
			ConnectedHost = remoteHost,
			Powered = true,
			PreferredNetworkAddress = "remote.file",
			TelecommunicationsGrid = grid
		};
		grid.JoinGrid(localAdapter);
		grid.JoinGrid(remoteAdapter);
		localHost.NetworkAdapters = new[] { localAdapter };
		remoteHost.NetworkAdapters = new[] { remoteAdapter };

		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1901L);
		terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
		localHost.ConnectedTerminals = new[] { terminal.Object };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 98L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = localHost,
			CurrentOwner = localHost
		};
		var application = service.GetBuiltInApplication(localHost, "filemanager");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, localHost, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "show public remote.file public.txt", out var showError), showError);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("Hello from the network")), true, true), Times.AtLeastOnce);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "copy public remote.file public.txt", out var copyError), copyError);
		var copied = localHost.FileSystemStorage!.GetFile("public.txt");
		Assert.IsNotNull(copied);
		Assert.AreEqual("Hello from the network", copied.TextContents);
		Assert.IsFalse(copied.PubliclyAccessible);
	}

	[TestMethod]
	public void ComputerExecutionService_TrySubmitTerminalInput_FtpApp_CanAuthenticateAndUploadFiles()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = CreateGameworld(scheduler);
		var service = new ComputerExecutionService(gameworld.Object);
		gameworld.SetupGet(x => x.ComputerExecutionService).Returns(service);
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var localHost = new StubComputerHost
		{
			Powered = true,
			Name = "Local Host",
			OwnerHostItemId = 951L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(951L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(5000L)
		};
		localHost.FileSystemStorage!.WriteFile("local.txt", "Hello remote");
		var remoteHost = new StubComputerHost
		{
			Powered = true,
			Name = "Remote Host",
			OwnerHostItemId = 952L,
			BuiltInApplications = ComputerBuiltInApplications.ForHost(952L).ToList(),
			FileSystemStorage = new ComputerMutableFileSystem(5000L)
		};
		remoteHost.SetNetworkServiceEnabled("ftp", true, out _);
		Assert.IsTrue(gameworld.Object.ComputerFileTransferService.CreateAccount(remoteHost, "alice", "secret", out var createError),
			createError);

		var localAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 961L,
			ConnectedHost = localHost,
			Powered = true,
			PreferredNetworkAddress = "local.ftp",
			TelecommunicationsGrid = grid
		};
		var remoteAdapter = new StubNetworkAdapter
		{
			NetworkAdapterItemId = 962L,
			ConnectedHost = remoteHost,
			Powered = true,
			PreferredNetworkAddress = "remote.ftp",
			TelecommunicationsGrid = grid
		};
		grid.JoinGrid(localAdapter);
		grid.JoinGrid(remoteAdapter);
		localHost.NetworkAdapters = new[] { localAdapter };
		remoteHost.NetworkAdapters = new[] { remoteAdapter };

		var terminal = new Mock<IComputerTerminal>();
		terminal.SetupGet(x => x.TerminalItemId).Returns(1951L);
		terminal.SetupGet(x => x.Sessions).Returns(Enumerable.Empty<IComputerTerminalSession>());
		localHost.ConnectedTerminals = new[] { terminal.Object };
		var output = new Mock<IOutputHandler>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.UseUnicode).Returns(false);
		var user = CreateOwner(gameworld.Object, 99L);
		user.SetupGet(x => x.OutputHandler).Returns(output.Object);
		user.SetupGet(x => x.LineFormatLength).Returns(120);
		user.SetupGet(x => x.Account).Returns(account.Object);
		var session = new ComputerTerminalSession
		{
			User = user.Object,
			Terminal = terminal.Object,
			Host = localHost,
			CurrentOwner = localHost
		};
		var application = service.GetBuiltInApplication(localHost, "ftp");

		Assert.IsNotNull(application);
		var start = service.ExecuteBuiltInApplication(user.Object, localHost, application!, session);
		Assert.AreEqual(ComputerProcessStatus.Sleeping, start.Status);

		Assert.IsTrue(service.TrySubmitTerminalInput(session, "open remote.ftp", out var openError), openError);
		Assert.IsTrue(service.TrySubmitTerminalInput(session, "login alice secret", out var loginError), loginError);
		Assert.IsTrue(service.TrySubmitTerminalInput(session, "put local.txt uploaded.txt", out var putError), putError);

		var uploaded = remoteHost.FileSystemStorage!.GetFile("uploaded.txt");
		Assert.IsNotNull(uploaded);
		Assert.AreEqual("Hello remote", uploaded.TextContents);
		Assert.IsFalse(uploaded.PubliclyAccessible);
	}

	private static ComputerWorkspaceProgram CompileProgram(string name, string source,
		params ComputerExecutableParameter[] parameters)
	{
		var gameworld = new Mock<IFuturemud>();
		var (prog, error) = ComputerExecutableCompiler.Compile(
			gameworld.Object,
			name,
			ComputerExecutableKind.Program,
			ProgVariableTypes.Number,
			parameters,
			source);

		Assert.IsNotNull(prog, error);
		var program = new ComputerWorkspaceProgram(1, gameworld.Object)
		{
			Name = name,
			ReturnType = ProgVariableTypes.Number,
			Parameters = parameters,
			SourceCode = source,
			CompilationStatus = ComputerCompilationStatus.Compiled,
			CompileError = string.Empty
		};
		program.CompiledProg = prog;
		return program;
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static Mock<IFuturemud> CreateGameworld(Mock<IScheduler> scheduler)
	{
		var gameworld = new Mock<IFuturemud>();
		var saveManager = new Mock<ISaveManager>();
		var mailService = new ComputerMailService(gameworld.Object);
		var fileTransferService = new ComputerFileTransferService(gameworld.Object);
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.ComputerMailService).Returns(mailService);
		gameworld.SetupGet(x => x.ComputerFileTransferService).Returns(fileTransferService);
		return gameworld;
	}

	private static (Mock<IFuturemud> Gameworld, Mock<IGameItem> HostItem, Mock<ISignalSourceComponent> SignalSource,
		Action<ComputerSignal> SetSignal)
		CreateGameworldWithSignalSource(Mock<IScheduler> scheduler, string sourceName)
	{
		var gameworld = CreateGameworld(scheduler);
		var hostItem = new Mock<IGameItem>();
		var signalSource = new Mock<ISignalSourceComponent>();
		var currentSignal = default(ComputerSignal);

		hostItem.SetupGet(x => x.Id).Returns(1L);
		hostItem.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		hostItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(() => new[] { signalSource.Object });

		signalSource.SetupGet(x => x.Parent).Returns(hostItem.Object);
		signalSource.SetupGet(x => x.Id).Returns(11L);
		signalSource.As<IGameItemComponent>()
			.SetupGet(x => x.Name)
			.Returns(sourceName);
		signalSource.As<ISignalSource>()
			.SetupGet(x => x.Name)
			.Returns(sourceName);
		signalSource.SetupGet(x => x.LocalSignalSourceIdentifier).Returns(11L);
		signalSource.SetupGet(x => x.EndpointKey).Returns(SignalComponentUtilities.DefaultLocalSignalEndpointKey);
		signalSource.SetupGet(x => x.CurrentSignal).Returns(() => currentSignal);
		signalSource.SetupGet(x => x.CurrentValue).Returns(() => currentSignal.Value);
		signalSource.SetupGet(x => x.Duration).Returns(() => currentSignal.Duration);
		signalSource.SetupGet(x => x.PulseInterval).Returns(() => currentSignal.PulseInterval);

		gameworld.Setup(x => x.TryGetItem(1L, true)).Returns(hostItem.Object);
		return (gameworld, hostItem, signalSource, signal => currentSignal = signal);
	}

	private static Mock<ICharacter> CreateOwner(IFuturemud gameworld, long id = 42L)
	{
		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Id).Returns(id);
		owner.SetupGet(x => x.Gameworld).Returns(gameworld);
		return owner;
	}

	private static FMDBState CaptureFMDBState()
	{
		return new FMDBState(
			(FuturemudDatabaseContext?)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
				.GetValue(null),
			typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
			(uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
				.GetValue(null)!);
	}

	private static void PrimeFMDB(FuturemudDatabaseContext context)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
	}

	private static void RestoreFMDBState(FMDBState state)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
			.SetValue(null, state.InstanceCount);
	}

	private sealed class StubNetworkAdapter : INetworkAdapter
	{
		public IComputerHost? ConnectedHost { get; set; }
		public bool Powered { get; set; }
		public bool NetworkReady => Powered && ConnectedHost?.Powered == true && TelecommunicationsGrid is not null;
		public string? PreferredNetworkAddress { get; set; }
		public string? NetworkAddress => TelecommunicationsGrid?.GetCanonicalNetworkAddress(this) ??
		                                 $"adapter-{NetworkAdapterItemId}";
		public long NetworkAdapterItemId { get; init; }
		public ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
	}

	private sealed class StubComputerHost : IComputerHost, IComputerMutableOwner, IComputerFtpAccountStore
	{
		private readonly Dictionary<long, ComputerRuntimeProcess> _processes = new();
		private readonly HashSet<string> _enabledNetworkServices = new(StringComparer.InvariantCultureIgnoreCase);
		private readonly Dictionary<string, ComputerMutableFtpAccount> _ftpAccounts = new(StringComparer.InvariantCultureIgnoreCase);
		private long _nextProcessId = 1L;

		public bool Powered { get; set; }
		public string Name { get; set; } = string.Empty;
		public long FileOwnerId => OwnerHostItemId ?? 0L;
		public long? OwnerCharacterId => null;
		public long? OwnerHostItemId { get; set; } = 1L;
		public long? OwnerStorageItemId { get; set; }
		public IComputerHost ExecutionHost => this;
		public ComputerMutableFileSystem? FileSystemStorage { get; set; }
		public IComputerFileSystem? FileSystem => FileSystemStorage;
		public IEnumerable<IComputerExecutableDefinition> Executables { get; set; } = Enumerable.Empty<IComputerExecutableDefinition>();
		public IEnumerable<IComputerProcess> Processes => _processes.Values;
		public IEnumerable<IComputerBuiltInApplication> BuiltInApplications { get; set; } = Enumerable.Empty<IComputerBuiltInApplication>();
		public IEnumerable<IComputerStorage> MountedStorage { get; set; } = Enumerable.Empty<IComputerStorage>();
		public IEnumerable<IComputerTerminal> ConnectedTerminals { get; set; } = Enumerable.Empty<IComputerTerminal>();
		public IEnumerable<INetworkAdapter> NetworkAdapters { get; set; } = Enumerable.Empty<INetworkAdapter>();
		public IEnumerable<string> EnabledNetworkServices => _enabledNetworkServices.OrderBy(x => x).ToList();
		public IEnumerable<IComputerFtpAccount> FtpAccounts => _ftpAccounts.Values.OrderBy(x => x.UserName).ToList();

		public IComputerProcess? GetProcess(long processId)
		{
			return _processes.TryGetValue(processId, out var process) ? process : null;
		}

		public bool IsNetworkServiceEnabled(string applicationId)
		{
			return _enabledNetworkServices.Contains(applicationId);
		}

		public bool SetNetworkServiceEnabled(string applicationId, bool enabled, out string error)
		{
			error = string.Empty;
			if (enabled)
			{
				_enabledNetworkServices.Add(applicationId);
			}
			else
			{
				_enabledNetworkServices.Remove(applicationId);
			}

			return true;
		}

		public bool CreateFtpAccount(string userName, string passwordHash, long passwordSalt, out string error)
		{
			error = string.Empty;
			if (_ftpAccounts.ContainsKey(userName))
			{
				error = "Duplicate account.";
				return false;
			}

			_ftpAccounts[userName] = new ComputerMutableFtpAccount
			{
				UserName = userName,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				Enabled = true
			};
			return true;
		}

		public bool SetFtpAccountEnabled(string userName, bool enabled, out string error)
		{
			if (!_ftpAccounts.TryGetValue(userName, out var account))
			{
				error = "Unknown account.";
				return false;
			}

			account.Enabled = enabled;
			error = string.Empty;
			return true;
		}

		public bool SetFtpAccountPassword(string userName, string passwordHash, long passwordSalt, out string error)
		{
			if (!_ftpAccounts.TryGetValue(userName, out var account))
			{
				error = "Unknown account.";
				return false;
			}

			account.PasswordHash = passwordHash;
			account.PasswordSalt = passwordSalt;
			error = string.Empty;
			return true;
		}

		public IComputerExecutableDefinition CreateExecutableDefinition(ComputerExecutableKind kind, string name)
		{
			throw new NotSupportedException();
		}

		public void SaveExecutableDefinition(IComputerExecutableDefinition executable)
		{
		}

		public bool DeleteExecutableDefinition(IComputerExecutableDefinition executable, out string error)
		{
			error = "Unsupported";
			return false;
		}

		public ComputerRuntimeProcess CreateProcessDefinition(ICharacter? actor, IComputerProgramDefinition program)
		{
			var process = new ComputerRuntimeProcess
			{
				Id = _nextProcessId++,
				ProcessName = program.Name,
				OwnerCharacterId = actor?.Id ?? 0L,
				Program = program,
				Host = this,
				Status = ComputerProcessStatus.Running,
				WaitType = ComputerProcessWaitType.None,
				StartedAtUtc = DateTime.UtcNow,
				LastUpdatedAtUtc = DateTime.UtcNow
			};
			_processes[process.Id] = process;
			return process;
		}

		public void SaveProcessDefinition(ComputerRuntimeProcess process)
		{
			_processes[process.Id] = process;
		}

		public void DeleteProcessDefinition(IComputerProcess process)
		{
			_processes.Remove(process.Id);
		}
	}

	private sealed class StubComputerOwner : IComputerMutableOwner, IComputerStorage
	{
		private readonly Dictionary<long, ComputerRuntimeExecutableBase> _executables = new();
		private readonly Dictionary<long, ComputerRuntimeProcess> _processes = new();
		private long _nextExecutableId = 1L;
		private long _nextProcessId = 1L;

		public StubComputerOwner(IComputerHost host, string name)
		{
			ExecutionHost = host;
			Name = name;
		}

		public string Name { get; }
		public long FileOwnerId => OwnerStorageItemId ?? 0L;
		public long? OwnerCharacterId => null;
		public long? OwnerHostItemId => null;
		public long? OwnerStorageItemId => OwnerStorageItemIdValue;
		public long? OwnerStorageItemIdValue { get; set; } = 99L;
		public IComputerHost ExecutionHost { get; }
		public ComputerMutableFileSystem? FileSystemStorage { get; set; }
		public IComputerFileSystem? FileSystem => FileSystemStorage;
		public long CapacityInBytes => FileSystemStorage?.CapacityInBytes ?? 0L;
		public bool Mounted => MountedHost is not null;
		public IComputerHost? MountedHost => ExecutionHost;
		public IEnumerable<IComputerExecutableDefinition> Executables => _executables.Values;
		public IEnumerable<IComputerProcess> Processes => _processes.Values;

		public IComputerExecutableDefinition CreateExecutableDefinition(ComputerExecutableKind kind, string name)
		{
			ComputerRuntimeExecutableBase executable = kind == ComputerExecutableKind.Function
				? new ComputerMutableFunction(_nextExecutableId++, FutureProgTestBootstrap.Gameworld)
				: new ComputerMutableProgram(_nextExecutableId++, FutureProgTestBootstrap.Gameworld);
			executable.Name = name;
			executable.OwnerStorageItemId = OwnerStorageItemId;
			executable.CompilationStatus = ComputerCompilationStatus.NotCompiled;
			executable.CreatedAtUtc = DateTime.UtcNow;
			executable.LastModifiedAtUtc = DateTime.UtcNow;
			_executables[executable.Id] = executable;
			return executable;
		}

		public void SaveExecutableDefinition(IComputerExecutableDefinition executable)
		{
			_executables[executable.Id] = (ComputerRuntimeExecutableBase)executable;
		}

		public bool DeleteExecutableDefinition(IComputerExecutableDefinition executable, out string error)
		{
			error = string.Empty;
			return _executables.Remove(executable.Id);
		}

		public ComputerRuntimeProcess CreateProcessDefinition(ICharacter? actor, IComputerProgramDefinition program)
		{
			var process = new ComputerRuntimeProcess
			{
				Id = _nextProcessId++,
				ProcessName = program.Name,
				OwnerCharacterId = actor?.Id ?? 0L,
				Program = program,
				Host = ExecutionHost,
				Status = ComputerProcessStatus.Running,
				WaitType = ComputerProcessWaitType.None,
				StartedAtUtc = DateTime.UtcNow,
				LastUpdatedAtUtc = DateTime.UtcNow
			};
			_processes[process.Id] = process;
			return process;
		}

		public void SaveProcessDefinition(ComputerRuntimeProcess process)
		{
			_processes[process.Id] = process;
		}

		public void DeleteProcessDefinition(IComputerProcess process)
		{
			_processes.Remove(process.Id);
		}
	}
}
