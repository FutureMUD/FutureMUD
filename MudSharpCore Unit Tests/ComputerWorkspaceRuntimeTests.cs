#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Computers;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
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
		Assert.IsFalse(service.GetFunctionHelp(FutureProgCompilationContext.ComputerFunction)
			.Any(x => x.FunctionName.EqualTo("UserInput")));
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
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		return gameworld;
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

	private sealed class StubComputerHost : IComputerHost
	{
		public bool Powered { get; set; }
		public string Name { get; set; } = string.Empty;
		public long? OwnerCharacterId => null;
		public long? OwnerHostItemId => 1L;
		public long? OwnerStorageItemId => null;
		public IComputerHost ExecutionHost => this;
		public IComputerFileSystem? FileSystem => null;
		public IEnumerable<IComputerExecutableDefinition> Executables { get; set; } = Enumerable.Empty<IComputerExecutableDefinition>();
		public IEnumerable<IComputerProcess> Processes { get; set; } = Enumerable.Empty<IComputerProcess>();
		public IEnumerable<IComputerBuiltInApplication> BuiltInApplications => Enumerable.Empty<IComputerBuiltInApplication>();
		public IEnumerable<IComputerStorage> MountedStorage { get; set; } = Enumerable.Empty<IComputerStorage>();
		public IEnumerable<IComputerTerminal> ConnectedTerminals => Enumerable.Empty<IComputerTerminal>();
		public IEnumerable<INetworkAdapter> NetworkAdapters => Enumerable.Empty<INetworkAdapter>();

		public IComputerProcess? GetProcess(long processId)
		{
			return Processes.FirstOrDefault(x => x.Id == processId);
		}
	}

	private sealed class StubComputerOwner : IComputerMutableOwner
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
		public long? OwnerCharacterId => null;
		public long? OwnerHostItemId => null;
		public long? OwnerStorageItemId => 99L;
		public IComputerHost ExecutionHost { get; }
		public IComputerFileSystem? FileSystem => null;
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

		public ComputerRuntimeProcess CreateProcessDefinition(ICharacter? actor, ComputerRuntimeProgramBase program)
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
