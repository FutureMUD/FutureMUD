#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.Computers;

public class ComputerExecutionService : IComputerExecutionService
{
	private readonly IFuturemud _gameworld;
	private readonly object _sync = new();
	private readonly Dictionary<long, ComputerWorkspaceExecutableBase> _executables = new();
	private readonly Dictionary<long, ComputerWorkspaceProcess> _processes = new();
	private bool _loaded;

	public ComputerExecutionService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public void LoadPersistedState()
	{
		EnsureLoaded();
		lock (_sync)
		{
			foreach (var process in _processes.Values.ToList())
			{
				switch (process.Status)
				{
					case ComputerProcessStatus.Running:
					case ComputerProcessStatus.NotStarted:
						process.Status = ComputerProcessStatus.Failed;
						process.LastError = "The computer program was interrupted before it could suspend or complete.";
						process.LastUpdatedAtUtc = DateTime.UtcNow;
						process.EndedAtUtc = DateTime.UtcNow;
						process.StateJson = string.Empty;
						process.WaitType = ComputerProcessWaitType.None;
						process.WakeTimeUtc = null;
						process.WaitArgument = null;
						PersistProcess_NoLock(process);
						break;
					case ComputerProcessStatus.Sleeping:
						ScheduleResume_NoLock(process);
						break;
				}
			}
		}
	}

	public ICharacterComputerWorkspace GetWorkspace(ICharacter owner)
	{
		EnsureLoaded();
		return new CharacterComputerWorkspace(owner, () => GetExecutables(owner), () => GetProcesses(owner));
	}

	public IEnumerable<IComputerExecutableDefinition> GetExecutables(ICharacter owner)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return _executables.Values
				.Where(x => x.OwnerCharacterId == owner.Id)
				.OrderBy(x => x.Name)
				.ThenBy(x => x.Id)
				.Cast<IComputerExecutableDefinition>()
				.ToList();
		}
	}

	public IComputerExecutableDefinition? GetExecutable(ICharacter owner, string identifier)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (long.TryParse(identifier, out var id))
			{
				return _executables.GetValueOrDefault(id) is { OwnerCharacterId: var ownerId } executable &&
				       ownerId == owner.Id
					? executable
					: null;
			}

			var matches = _executables.Values
				.Where(x => x.OwnerCharacterId == owner.Id)
				.Where(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
				.Cast<IComputerExecutableDefinition>()
				.ToList();
			if (matches.Any())
			{
				return matches.First();
			}

			return _executables.Values
				.Where(x => x.OwnerCharacterId == owner.Id)
				.Where(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x.Name.Length)
				.ThenBy(x => x.Name)
				.Cast<IComputerExecutableDefinition>()
				.FirstOrDefault();
		}
	}

	public IComputerExecutableDefinition? GetExecutable(long id)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return _executables.GetValueOrDefault(id);
		}
	}

	public IComputerExecutableDefinition CreateExecutable(ICharacter owner, ComputerExecutableKind kind, string name)
	{
		EnsureLoaded();
		lock (_sync)
		{
			var now = DateTime.UtcNow;
			using (new FMDB())
			{
				var dbitem = new CharacterComputerExecutable
				{
					OwnerCharacterId = owner.Id,
					Name = string.IsNullOrWhiteSpace(name) ? "Unnamed" : name.Trim(),
					ExecutableKind = (int)kind,
					CompilationContext = (int)ComputerExecutableCompiler.GetCompilationContext(kind),
					ReturnTypeDefinition = ProgVariableTypes.Void.ToStorageString(),
					SourceCode = string.Empty,
					CompilationStatus = (int)ComputerCompilationStatus.NotCompiled,
					CompileError = string.Empty,
					AutorunOnBoot = false,
					CreatedAtUtc = now,
					LastModifiedAtUtc = now
				};
				FMDB.Context.CharacterComputerExecutables.Add(dbitem);
				FMDB.Context.SaveChanges();

				var executable = CreateRuntimeExecutable(dbitem);
				_executables[executable.Id] = executable;
				return executable;
			}
		}
	}

	public void SaveExecutable(IComputerExecutableDefinition executable)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (executable is not ComputerWorkspaceExecutableBase runtime)
			{
				return;
			}

			runtime.CompiledProg = null;
			runtime.CompilationStatus = ComputerCompilationStatus.NotCompiled;
			runtime.CompileError = string.Empty;
			runtime.LastModifiedAtUtc = DateTime.UtcNow;
			PersistExecutable_NoLock(runtime);
		}
	}

	public bool DeleteExecutable(ICharacter owner, IComputerExecutableDefinition executable, out string error)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (executable.OwnerCharacterId != owner.Id)
			{
				error = "You do not own that computer executable.";
				return false;
			}

			if (_processes.Values.Any(x => x.Program.Id == executable.Id && x.Status is ComputerProcessStatus.Running or ComputerProcessStatus.Sleeping))
			{
				error = "You cannot delete a computer executable while one of its processes is still active.";
				return false;
			}

			foreach (var process in _processes.Values.Where(x => x.Program.Id == executable.Id).ToList())
			{
				_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
				_processes.Remove(process.Id);
			}

			_executables.Remove(executable.Id);
			using (new FMDB())
			{
				var dbitem = FMDB.Context.CharacterComputerExecutables
					.Include(x => x.Parameters)
					.Include(x => x.Processes)
					.FirstOrDefault(x => x.Id == executable.Id);
				if (dbitem is not null)
				{
					FMDB.Context.CharacterComputerExecutables.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}

			error = string.Empty;
			return true;
		}
	}

	public ComputerCompilationResult CompileExecutable(IComputerExecutableDefinition executable)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return CompileExecutable_NoLock(executable);
		}
	}

	public ComputerExecutionResult Execute(ICharacter owner, IComputerExecutableDefinition executable,
		IEnumerable<object?> parameters)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (executable.OwnerCharacterId != owner.Id)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "You do not own that computer executable.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (executable is not ComputerWorkspaceExecutableBase runtimeExecutable)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That executable is not backed by the character workspace runtime.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (!EnsureCompiled_NoLock(runtimeExecutable, out var compileError))
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = compileError,
					Status = ComputerProcessStatus.Failed
				};
			}

			if (runtimeExecutable is ComputerWorkspaceFunction function)
			{
				var result = function.CompiledProg!.Execute(parameters.ToArray());
				return new ComputerExecutionResult
				{
					Success = true,
					Status = ComputerProcessStatus.Completed,
					Result = result
				};
			}

			if (runtimeExecutable is not ComputerWorkspaceProgram program)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That executable type is not supported.",
					Status = ComputerProcessStatus.Failed
				};
			}

			var process = CreateProcess_NoLock(owner, program);
			var outcome = ComputerProgramExecutor.Execute(program, parameters);
			ApplyExecutionOutcome_NoLock(process, outcome);

			return new ComputerExecutionResult
			{
				Success = outcome.Status != ComputerProcessStatus.Failed,
				ErrorMessage = outcome.Error ?? string.Empty,
				Status = outcome.Status,
				Result = outcome.Result,
				Process = process
			};
		}
	}

	public IEnumerable<IComputerProcess> GetProcesses(ICharacter owner)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return _processes.Values
				.Where(x => x.OwnerCharacterId == owner.Id)
				.OrderByDescending(x => x.LastUpdatedAtUtc)
				.ThenByDescending(x => x.Id)
				.Cast<IComputerProcess>()
				.ToList();
		}
	}

	public IComputerProcess? GetProcess(ICharacter owner, long processId)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return _processes.GetValueOrDefault(processId) is { OwnerCharacterId: var ownerId } process &&
			       ownerId == owner.Id
				? process
				: null;
		}
	}

	public bool KillProcess(ICharacter owner, long processId, out string error)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (!_processes.TryGetValue(processId, out var process) || process.OwnerCharacterId != owner.Id)
			{
				error = "There is no such computer process for you to kill.";
				return false;
			}

			if (process.Status is ComputerProcessStatus.Completed or ComputerProcessStatus.Failed or ComputerProcessStatus.Killed)
			{
				error = "That computer process has already ended.";
				return false;
			}

			_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
			process.Status = ComputerProcessStatus.Killed;
			process.WaitType = ComputerProcessWaitType.None;
			process.WakeTimeUtc = null;
			process.WaitArgument = null;
			process.LastError = "Killed by user request.";
			process.LastUpdatedAtUtc = DateTime.UtcNow;
			process.EndedAtUtc = DateTime.UtcNow;
			process.StateJson = string.Empty;
			PersistProcess_NoLock(process);
			error = string.Empty;
			return true;
		}
	}

	private void EnsureLoaded()
	{
		if (_loaded)
		{
			return;
		}

		lock (_sync)
		{
			if (_loaded)
			{
				return;
			}

			LoadFromDatabase_NoLock();
			_loaded = true;
		}
	}

	private void LoadFromDatabase_NoLock()
	{
		_executables.Clear();
		_processes.Clear();

		using (new FMDB())
		{
			var executables = FMDB.Context.CharacterComputerExecutables
				.Include(x => x.Parameters)
				.AsNoTracking()
				.ToList();

			foreach (var executable in executables)
			{
				var runtimeExecutable = CreateRuntimeExecutable(executable);
				_executables[runtimeExecutable.Id] = runtimeExecutable;
			}

			var processes = FMDB.Context.CharacterComputerProgramProcesses
				.AsNoTracking()
				.ToList();

			foreach (var process in processes)
			{
				if (!_executables.TryGetValue(process.CharacterComputerExecutableId, out var executable) ||
				    executable is not ComputerWorkspaceProgram runtimeProgram)
				{
					continue;
				}

				var runtimeProcess = new ComputerWorkspaceProcess
				{
					Id = process.Id,
					ProcessName = process.ProcessName,
					OwnerCharacterId = process.OwnerCharacterId,
					Program = runtimeProgram,
					Host = CreateWorkspaceHost_NoLock(process.OwnerCharacterId),
					Status = (ComputerProcessStatus)process.Status,
					WaitType = (ComputerProcessWaitType)process.WaitType,
					WakeTimeUtc = process.WakeTimeUtc,
					WaitArgument = process.WaitArgument,
					PowerLossBehaviour = (ComputerPowerLossBehaviour)process.PowerLossBehaviour,
					Result = ComputerProgramExecutor.DeserializeValue(runtimeProgram.ReturnType, process.ResultJson, _gameworld),
					LastError = process.LastError,
					StartedAtUtc = process.StartedAtUtc,
					LastUpdatedAtUtc = process.LastUpdatedAtUtc,
					EndedAtUtc = process.EndedAtUtc
				};
				runtimeProcess.StateJson = process.StateJson ?? string.Empty;
				_processes[runtimeProcess.Id] = runtimeProcess;
			}
		}
	}

	private ComputerWorkspaceExecutableBase CreateRuntimeExecutable(CharacterComputerExecutable dbitem)
	{
		ComputerWorkspaceExecutableBase executable;
		if (dbitem.ExecutableKind == (int)ComputerExecutableKind.Function)
		{
			executable = new ComputerWorkspaceFunction(dbitem.Id, _gameworld);
		}
		else
		{
			executable = new ComputerWorkspaceProgram(dbitem.Id, _gameworld)
			{
				AutorunOnBoot = dbitem.AutorunOnBoot
			};
		}

		executable.Name = dbitem.Name;
		executable.SourceCode = dbitem.SourceCode;
		executable.ReturnType = ProgVariableTypes.FromStorageString(dbitem.ReturnTypeDefinition);
		executable.Parameters = dbitem.Parameters
			.OrderBy(x => x.ParameterIndex)
			.Select(x => new ComputerExecutableParameter(x.ParameterName.ToLowerInvariant(),
				ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)))
			.ToList();
		executable.CompilationStatus = (ComputerCompilationStatus)dbitem.CompilationStatus;
		executable.CompileError = dbitem.CompileError ?? string.Empty;
		executable.OwnerCharacterId = dbitem.OwnerCharacterId;
		executable.OwnerHostItemId = dbitem.OwnerHostItemId;
		executable.OwnerStorageItemId = dbitem.OwnerStorageItemId;
		executable.CreatedAtUtc = dbitem.CreatedAtUtc;
		executable.LastModifiedAtUtc = dbitem.LastModifiedAtUtc;
		return executable;
	}

	private bool EnsureCompiled_NoLock(ComputerWorkspaceExecutableBase executable, out string error)
	{
		if (executable.CompiledProg is not null && executable.CompilationStatus == ComputerCompilationStatus.Compiled)
		{
			error = string.Empty;
			return true;
		}

		var result = CompileExecutable_NoLock(executable);
		error = result.ErrorMessage;
		return result.Success;
	}

	private ComputerCompilationResult CompileExecutable_NoLock(IComputerExecutableDefinition executable)
	{
		if (executable is not ComputerWorkspaceExecutableBase runtime)
		{
			return new ComputerCompilationResult
			{
				Success = false,
				ErrorMessage = "That executable cannot be compiled by the workspace compiler.",
				Executable = executable
			};
		}

		var (prog, compileError) = ComputerExecutableCompiler.Compile(
			_gameworld,
			runtime.Name,
			runtime.ExecutableKind,
			runtime.ReturnType,
			runtime.Parameters,
			runtime.SourceCode);

		runtime.CompiledProg = prog;
		runtime.CompilationStatus = prog is null ? ComputerCompilationStatus.Failed : ComputerCompilationStatus.Compiled;
		runtime.CompileError = compileError;
		runtime.LastModifiedAtUtc = DateTime.UtcNow;
		PersistExecutable_NoLock(runtime);

		return new ComputerCompilationResult
		{
			Success = prog is not null,
			ErrorMessage = compileError,
			Executable = runtime
		};
	}

	private ComputerWorkspaceProcess CreateProcess_NoLock(ICharacter owner, ComputerWorkspaceProgram program)
	{
		var now = DateTime.UtcNow;
		using (new FMDB())
		{
			var dbprocess = new CharacterComputerProgramProcess
			{
				CharacterComputerExecutableId = program.Id,
				OwnerCharacterId = owner.Id,
				ProcessName = program.Name,
				Status = (int)ComputerProcessStatus.Running,
				WaitType = (int)ComputerProcessWaitType.None,
				PowerLossBehaviour = (int)ComputerPowerLossBehaviour.PersistSuspended,
				StateJson = string.Empty,
				StartedAtUtc = now,
				LastUpdatedAtUtc = now
			};
			FMDB.Context.CharacterComputerProgramProcesses.Add(dbprocess);
			FMDB.Context.SaveChanges();

			var runtimeProcess = new ComputerWorkspaceProcess
			{
				Id = dbprocess.Id,
				ProcessName = dbprocess.ProcessName,
				OwnerCharacterId = owner.Id,
				Program = program,
				Host = CreateWorkspaceHost_NoLock(owner.Id),
				Status = ComputerProcessStatus.Running,
				WaitType = ComputerProcessWaitType.None,
				PowerLossBehaviour = ComputerPowerLossBehaviour.PersistSuspended,
				StartedAtUtc = now,
				LastUpdatedAtUtc = now
			};
			_processes[runtimeProcess.Id] = runtimeProcess;
			return runtimeProcess;
		}
	}

	private void ApplyExecutionOutcome_NoLock(ComputerWorkspaceProcess process, ComputerProgramExecutionOutcome outcome)
	{
		_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
		process.Status = outcome.Status;
		process.WaitType = outcome.WaitType;
		process.WakeTimeUtc = outcome.WakeTimeUtc;
		process.WaitArgument = outcome.WaitArgument;
		process.Result = outcome.Result;
		process.LastError = outcome.Error;
		process.LastUpdatedAtUtc = DateTime.UtcNow;
		process.StateJson = outcome.Status == ComputerProcessStatus.Sleeping ? outcome.StateJson : string.Empty;
		process.EndedAtUtc = outcome.Status is ComputerProcessStatus.Completed or ComputerProcessStatus.Failed or ComputerProcessStatus.Killed
			? DateTime.UtcNow
			: null;
		PersistProcess_NoLock(process);

		if (outcome.Status == ComputerProcessStatus.Sleeping)
		{
			ScheduleResume_NoLock(process);
		}
	}

	private void PersistExecutable_NoLock(ComputerWorkspaceExecutableBase executable)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.CharacterComputerExecutables
				.Include(x => x.Parameters)
				.First(x => x.Id == executable.Id);

			dbitem.OwnerCharacterId = executable.OwnerCharacterId ?? 0;
			dbitem.OwnerHostItemId = executable.OwnerHostItemId;
			dbitem.OwnerStorageItemId = executable.OwnerStorageItemId;
			dbitem.Name = executable.Name;
			dbitem.ExecutableKind = (int)executable.ExecutableKind;
			dbitem.CompilationContext = (int)executable.CompilationContext;
			dbitem.ReturnTypeDefinition = executable.ReturnType.ToStorageString();
			dbitem.SourceCode = executable.SourceCode;
			dbitem.CompilationStatus = (int)executable.CompilationStatus;
			dbitem.CompileError = executable.CompileError ?? string.Empty;
			dbitem.AutorunOnBoot = executable is IComputerProgramDefinition { AutorunOnBoot: true };
			dbitem.CreatedAtUtc = executable.CreatedAtUtc;
			dbitem.LastModifiedAtUtc = executable.LastModifiedAtUtc;

			FMDB.Context.CharacterComputerExecutableParameters.RemoveRange(dbitem.Parameters);
			dbitem.Parameters.Clear();
			foreach (var parameter in executable.Parameters.Select((value, index) => (value, index)))
			{
				dbitem.Parameters.Add(new CharacterComputerExecutableParameter
				{
					CharacterComputerExecutableId = dbitem.Id,
					ParameterIndex = parameter.index,
					ParameterName = parameter.value.Name,
					ParameterTypeDefinition = parameter.value.Type.ToStorageString()
				});
			}

			FMDB.Context.SaveChanges();
		}
	}

	private void PersistProcess_NoLock(ComputerWorkspaceProcess process)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.CharacterComputerProgramProcesses.First(x => x.Id == process.Id);
			dbitem.ProcessName = process.ProcessName;
			dbitem.Status = (int)process.Status;
			dbitem.WaitType = (int)process.WaitType;
			dbitem.WakeTimeUtc = process.WakeTimeUtc;
			dbitem.WaitArgument = process.WaitArgument;
			dbitem.PowerLossBehaviour = (int)process.PowerLossBehaviour;
			dbitem.StateJson = process.StateJson ?? string.Empty;
			dbitem.ResultJson = ComputerProgramExecutor.SerializeValue(process.Program.ReturnType, process.Result);
			dbitem.LastError = process.LastError;
			dbitem.StartedAtUtc = process.StartedAtUtc;
			dbitem.LastUpdatedAtUtc = process.LastUpdatedAtUtc;
			dbitem.EndedAtUtc = process.EndedAtUtc;
			FMDB.Context.SaveChanges();
		}
	}

	private CharacterWorkspaceHost CreateWorkspaceHost_NoLock(long ownerCharacterId)
	{
		return new CharacterWorkspaceHost(
			_gameworld,
			ownerCharacterId,
			() =>
			{
				lock (_sync)
				{
					return _executables.Values
						.Where(x => x.OwnerCharacterId == ownerCharacterId)
						.Cast<IComputerExecutable>()
						.ToList();
				}
			},
			() =>
			{
				lock (_sync)
				{
					return _processes.Values
						.Where(x => x.OwnerCharacterId == ownerCharacterId)
						.Cast<IComputerProcess>()
						.ToList();
				}
			});
	}

	private void ScheduleResume_NoLock(ComputerWorkspaceProcess process)
	{
		if (process.Status != ComputerProcessStatus.Sleeping)
		{
			return;
		}

		var delay = (process.WakeTimeUtc ?? DateTime.UtcNow) - DateTime.UtcNow;
		if (delay < TimeSpan.Zero)
		{
			delay = TimeSpan.Zero;
		}

		_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
		_gameworld.Scheduler.AddSchedule(new Schedule<ComputerWorkspaceProcess>(
			process,
			ResumeScheduledProcess,
			ScheduleType.ComputerProgram,
			delay,
			$"Resume computer program process #{process.Id}"));
	}

	private void ResumeScheduledProcess(ComputerWorkspaceProcess process)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (!_processes.TryGetValue(process.Id, out var liveProcess))
			{
				return;
			}

			if (liveProcess.Status != ComputerProcessStatus.Sleeping || liveProcess.Program is not ComputerWorkspaceProgram program)
			{
				return;
			}

			if (!EnsureCompiled_NoLock(program, out var compileError))
			{
				ApplyExecutionOutcome_NoLock(liveProcess, new ComputerProgramExecutionOutcome
				{
					Status = ComputerProcessStatus.Failed,
					Error = compileError
				});
				return;
			}

			liveProcess.Status = ComputerProcessStatus.Running;
			liveProcess.WaitType = ComputerProcessWaitType.None;
			liveProcess.WakeTimeUtc = null;
			liveProcess.WaitArgument = null;
			liveProcess.LastUpdatedAtUtc = DateTime.UtcNow;
			PersistProcess_NoLock(liveProcess);

			var outcome = ComputerProgramExecutor.Execute(program, Enumerable.Empty<object?>(), liveProcess.StateJson);
			ApplyExecutionOutcome_NoLock(liveProcess, outcome);
		}
	}
}
