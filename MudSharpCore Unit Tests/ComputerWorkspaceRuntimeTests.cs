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
			var runtimeExecutable = (ComputerWorkspaceExecutableBase)executable;
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
			var runtimeExecutable = (ComputerWorkspaceExecutableBase)executable;
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
}
