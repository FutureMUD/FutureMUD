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
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;

namespace MudSharp.Computers;

public class ComputerExecutionService : IComputerExecutionService
{
	private readonly IFuturemud _gameworld;
	private readonly object _sync = new();
	private readonly Dictionary<long, ComputerRuntimeExecutableBase> _executables = new();
	private readonly Dictionary<long, ComputerRuntimeProcess> _processes = new();
	private readonly Dictionary<long, ICharacterComputerWorkspace> _workspaceOwners = new();
	private readonly Dictionary<long, IComputerExecutableOwner> _mutableExecutableOwners = new();
	private readonly List<IComputerExecutableOwner> _registeredItemOwners = [];
	private readonly Dictionary<long, ComputerSignalWaitSubscription> _signalWaitSubscriptions = new();
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
						process.WaitingCharacterId = null;
						process.WaitingTerminalItemId = null;
						PersistWorkspaceProcess_NoLock(process);
						break;
					case ComputerProcessStatus.Sleeping:
						if (process.WaitType == ComputerProcessWaitType.Sleep)
						{
							ScheduleResume_NoLock(process);
						}
						break;
				}
			}
		}
	}

	public ICharacterComputerWorkspace GetWorkspace(ICharacter owner)
	{
		EnsureLoaded();
		lock (_sync)
		{
			return GetOrCreateWorkspace_NoLock(owner);
		}
	}

	public IEnumerable<IComputerExecutableDefinition> GetExecutables(IComputerExecutableOwner owner)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			return ResolveExecutables_NoLock(owner)
				.OrderBy(x => x.Name)
				.ThenBy(x => x.Id)
				.ToList();
		}
	}

	public IComputerExecutableDefinition? GetExecutable(IComputerExecutableOwner owner, string identifier)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			var executables = ResolveExecutables_NoLock(owner).ToList();
			if (long.TryParse(identifier, out var id))
			{
				return executables.FirstOrDefault(x => x.Id == id);
			}

			var exact = executables
				.Where(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x.Name)
				.ThenBy(x => x.Id)
				.FirstOrDefault();
			if (exact is not null)
			{
				return exact;
			}

			return executables
				.Where(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x.Name.Length)
				.ThenBy(x => x.Name)
				.ThenBy(x => x.Id)
				.FirstOrDefault();
		}
	}

	public IEnumerable<IComputerBuiltInApplication> GetBuiltInApplications(IComputerExecutableOwner owner)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			return owner.ExecutionHost.BuiltInApplications
				.OrderBy(x => x.Name)
				.ThenBy(x => x.Id)
				.ToList();
		}
	}

	public IComputerBuiltInApplication? GetBuiltInApplication(IComputerExecutableOwner owner, string identifier)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			return ResolveBuiltInApplication_NoLock(owner, identifier);
		}
	}

	public IComputerExecutableDefinition CreateExecutable(IComputerExecutableOwner owner, ComputerExecutableKind kind,
		string name)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			if (owner is ICharacterComputerWorkspace workspace)
			{
				return CreateWorkspaceExecutable_NoLock(workspace.Owner, kind, name);
			}

			if (owner is IComputerMutableOwner mutableOwner)
			{
				var executable = mutableOwner.CreateExecutableDefinition(kind, name);
				_mutableExecutableOwners[executable.Id] = owner;
				return executable;
			}

			throw new NotSupportedException("That computer executable owner does not support creating executables.");
		}
	}

	public void SaveExecutable(IComputerExecutableOwner owner, IComputerExecutableDefinition executable)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			if (owner is ICharacterComputerWorkspace)
			{
				SaveWorkspaceExecutable_NoLock(executable);
				return;
			}

			if (owner is IComputerMutableOwner mutableOwner)
			{
				if (executable is ComputerRuntimeExecutableBase runtime)
				{
					runtime.CompiledProg = null;
					runtime.CompilationStatus = ComputerCompilationStatus.NotCompiled;
					runtime.CompileError = string.Empty;
					runtime.LastModifiedAtUtc = DateTime.UtcNow;
				}

				_mutableExecutableOwners[executable.Id] = owner;
				mutableOwner.SaveExecutableDefinition(executable);
				return;
			}

			throw new NotSupportedException("That computer executable owner does not support saving executables.");
		}
	}

	public bool DeleteExecutable(IComputerExecutableOwner owner, IComputerExecutableDefinition executable, out string error)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			if (!ExecutableBelongsToOwner(executable, owner))
			{
				error = "That computer executable does not belong to the current owner.";
				return false;
			}

			if (ResolveProcesses_NoLock(owner).Any(x =>
				    x.Program.Id == executable.Id && x.Status is ComputerProcessStatus.Running or ComputerProcessStatus.Sleeping))
			{
				error = "You cannot delete a computer executable while one of its processes is still active.";
				return false;
			}

			if (owner is ICharacterComputerWorkspace workspace)
			{
				return DeleteWorkspaceExecutable_NoLock(workspace.Owner, executable, out error);
			}

			if (owner is IComputerMutableOwner mutableOwner)
			{
				foreach (var process in ResolveProcesses_NoLock(owner).Where(x => x.Program.Id == executable.Id).ToList())
				{
					if (process is not ComputerRuntimeProcess runtimeProcess)
					{
						continue;
					}

					_gameworld.Scheduler.Destroy(runtimeProcess, ScheduleType.ComputerProgram);
					DetachSignalWait_NoLock(runtimeProcess.Id);
					mutableOwner.DeleteProcessDefinition(runtimeProcess);
				}

				_mutableExecutableOwners.Remove(executable.Id);
				return mutableOwner.DeleteExecutableDefinition(executable, out error);
			}

			error = "That computer executable owner does not support deleting executables.";
			return false;
		}
	}

	public ComputerExecutionResult Execute(ICharacter? actor, IComputerExecutableOwner owner,
		IComputerExecutableDefinition executable, IEnumerable<object?> parameters, IComputerTerminalSession? session = null)
	{
		if (executable is IComputerBuiltInApplication builtInApplication)
		{
			return ExecuteBuiltInApplication(actor, owner, builtInApplication, session);
		}

		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			if (!ExecutableBelongsToOwner(executable, owner))
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That computer executable does not belong to the current owner.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (owner is not ICharacterComputerWorkspace && !owner.ExecutionHost.Powered)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = $"{owner.ExecutionHost.Name} is not currently powered.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (executable is not ComputerRuntimeExecutableBase runtimeExecutable)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That executable is not backed by a mutable computer runtime.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (!EnsureCompiled_NoLock(owner, runtimeExecutable, out var compileError))
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = compileError,
					Status = ComputerProcessStatus.Failed
				};
			}

			if (runtimeExecutable is ComputerRuntimeFunctionBase function)
			{
				using var functionScope = new ComputerExecutionContextScope(new ComputerExecutionContext
				{
					Owner = owner,
					Host = owner.ExecutionHost,
					Gameworld = _gameworld,
					Actor = actor,
					Session = session
				});

				var result = function.CompiledProg!.Execute(parameters.ToArray());
				return new ComputerExecutionResult
				{
					Success = true,
					Status = ComputerProcessStatus.Completed,
					Result = result
				};
			}

			if (runtimeExecutable is not ComputerRuntimeProgramBase program)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That executable type is not supported.",
					Status = ComputerProcessStatus.Failed
				};
			}

			var process = CreateProcess_NoLock(actor, owner, program);
			using var programScope = new ComputerExecutionContextScope(new ComputerExecutionContext
			{
				Owner = owner,
				Host = owner.ExecutionHost,
				Gameworld = _gameworld,
				Actor = actor,
				Session = session,
				Process = process
			});

			var outcome = ComputerProgramExecutor.Execute(program, parameters);
			outcome = ApplyExecutionOutcome_NoLock(owner, process, outcome);

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

	public ComputerExecutionResult ExecuteBuiltInApplication(ICharacter? actor, IComputerExecutableOwner owner,
		IComputerBuiltInApplication application, IComputerTerminalSession? session = null)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			RegisterOwner_NoLock(owner.ExecutionHost);

			var resolvedApplication = ResolveBuiltInApplication_NoLock(owner, application.ApplicationId) ??
			                          ResolveBuiltInApplication_NoLock(owner, application.Name);
			if (resolvedApplication is null || resolvedApplication.Id != application.Id)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = "That built-in computer application is not available on the current execution host.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (!owner.ExecutionHost.Powered)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = $"{owner.ExecutionHost.Name} is not currently powered.",
					Status = ComputerProcessStatus.Failed
				};
			}

			if (owner.ExecutionHost is not IComputerMutableOwner processOwner)
			{
				return new ComputerExecutionResult
				{
					Success = false,
					ErrorMessage = $"{owner.ExecutionHost.Name} does not currently support running built-in applications.",
					Status = ComputerProcessStatus.Failed
				};
			}

			var process = processOwner.CreateProcessDefinition(actor, resolvedApplication);
			using var scope = new ComputerExecutionContextScope(new ComputerExecutionContext
			{
				Owner = owner,
				Host = owner.ExecutionHost,
				Gameworld = _gameworld,
				Actor = actor,
				Session = session,
				Process = process
			});

			var outcome = ComputerBuiltInApplicationExecutors.Execute(
				_gameworld,
				actor,
				owner,
				session,
				process,
				resolvedApplication);
			outcome = ApplyExecutionOutcome_NoLock(owner.ExecutionHost, process, outcome);

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

	public IEnumerable<IComputerProcess> GetProcesses(IComputerExecutableOwner owner)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			return ResolveProcesses_NoLock(owner)
				.OrderByDescending(x => x.LastUpdatedAtUtc)
				.ThenByDescending(x => x.Id)
				.ToList();
		}
	}

	public IComputerProcess? GetProcess(IComputerExecutableOwner owner, long processId)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			return ResolveProcesses_NoLock(owner).FirstOrDefault(x => x.Id == processId);
		}
	}

	public bool KillProcess(IComputerExecutableOwner owner, long processId, out string error)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			var process = ResolveProcesses_NoLock(owner).FirstOrDefault(x => x.Id == processId);
			if (process is not ComputerRuntimeProcess runtimeProcess)
			{
				error = "There is no such computer process for you to kill.";
				return false;
			}

			if (runtimeProcess.Status is ComputerProcessStatus.Completed or ComputerProcessStatus.Failed or ComputerProcessStatus.Killed)
			{
				error = "That computer process has already ended.";
				return false;
			}

			_gameworld.Scheduler.Destroy(runtimeProcess, ScheduleType.ComputerProgram);
			DetachSignalWait_NoLock(runtimeProcess.Id);
			runtimeProcess.Status = ComputerProcessStatus.Killed;
			runtimeProcess.WaitType = ComputerProcessWaitType.None;
			runtimeProcess.WakeTimeUtc = null;
			runtimeProcess.WaitArgument = null;
			runtimeProcess.WaitingCharacterId = null;
			runtimeProcess.WaitingTerminalItemId = null;
			runtimeProcess.LastError = "Killed by user request.";
			runtimeProcess.LastUpdatedAtUtc = DateTime.UtcNow;
			runtimeProcess.EndedAtUtc = DateTime.UtcNow;
			runtimeProcess.StateJson = string.Empty;
			PersistProcess_NoLock(owner, runtimeProcess);
			error = string.Empty;
			return true;
		}
	}

	public void ActivateOwner(IComputerExecutableOwner owner)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			foreach (var process in ResolveProcesses_NoLock(owner).OfType<ComputerRuntimeProcess>().ToList())
			{
				DetachSignalWait_NoLock(process.Id);
				switch (process.Status)
				{
					case ComputerProcessStatus.Running:
					case ComputerProcessStatus.NotStarted:
						process.Status = ComputerProcessStatus.Failed;
						process.LastError = "The computer program lost power before it could complete.";
						process.LastUpdatedAtUtc = DateTime.UtcNow;
						process.EndedAtUtc = DateTime.UtcNow;
						process.StateJson = string.Empty;
						process.WaitType = ComputerProcessWaitType.None;
						process.WakeTimeUtc = null;
						process.WaitArgument = null;
						process.WaitingCharacterId = null;
						process.WaitingTerminalItemId = null;
						PersistProcess_NoLock(owner, process);
						break;
					case ComputerProcessStatus.Sleeping when owner.ExecutionHost.Powered &&
					                                     process.WaitType == ComputerProcessWaitType.Sleep:
						ScheduleResume_NoLock(process);
						break;
					case ComputerProcessStatus.Sleeping when owner.ExecutionHost.Powered &&
					                                     process.WaitType == ComputerProcessWaitType.Signal:
						AttachSignalWait_NoLock(owner, process, true);
						break;
				}
			}
		}
	}

	public void DeactivateOwner(IComputerExecutableOwner owner)
	{
		EnsureLoadedForOwner(owner);
		lock (_sync)
		{
			RegisterOwner_NoLock(owner);
			foreach (var process in ResolveProcesses_NoLock(owner).OfType<ComputerRuntimeProcess>().ToList())
			{
				_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
				DetachSignalWait_NoLock(process.Id);

				if (!process.IsRunning)
				{
					continue;
				}

				switch (process.PowerLossBehaviour)
				{
					case ComputerPowerLossBehaviour.PersistSuspended when process.Status == ComputerProcessStatus.Sleeping:
						break;
					default:
						process.Status = ComputerProcessStatus.Failed;
						process.WaitType = ComputerProcessWaitType.None;
						process.WakeTimeUtc = null;
						process.WaitArgument = null;
						process.WaitingCharacterId = null;
						process.WaitingTerminalItemId = null;
						process.LastError = "The computer program was interrupted by power loss.";
						process.LastUpdatedAtUtc = DateTime.UtcNow;
						process.EndedAtUtc = DateTime.UtcNow;
						process.StateJson = string.Empty;
						PersistProcess_NoLock(owner, process);
						break;
				}
			}
		}
	}

	public bool TrySubmitTerminalInput(IComputerTerminalSession session, string text, out string error)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			error = "What do you want to type?";
			return false;
		}

		if (session.CurrentOwner is ICharacterComputerWorkspace)
		{
			EnsureLoaded();
		}
		lock (_sync)
		{
			if (!session.Host.Powered)
			{
				error = $"{session.Host.Name} is not currently powered.";
				return false;
			}

			var waitingProcesses = FindWaitingUserInputProcesses_NoLock(session)
				.OrderByDescending(x => x.LastUpdatedAtUtc)
				.ThenByDescending(x => x.Id)
				.ToList();
			if (!waitingProcesses.Any())
			{
				error = $"Nothing on {session.Host.Name} is currently waiting for terminal input.";
				return false;
			}

			if (waitingProcesses.Count > 1)
			{
				error =
					$"More than one program on {session.Host.Name} is waiting for terminal input on that session. Kill one of them before typing again.";
				return false;
			}

			return ResumeWaitingProcessFromTerminalInput_NoLock(waitingProcesses.Single(), session, text, out error);
		}
	}

	public IEnumerable<ComputerNetworkHostSummary> GetReachableHosts(IComputerHost sourceHost)
	{
		EnsureLoadedForOwner(sourceHost);
		lock (_sync)
		{
			RegisterOwner_NoLock(sourceHost);
			return GetReachableHosts_NoLock(sourceHost);
		}
	}

	public ComputerNetworkHostSummary? ResolveReachableHost(IComputerHost sourceHost, string identifier)
	{
		if (string.IsNullOrWhiteSpace(identifier))
		{
			return null;
		}

		EnsureLoadedForOwner(sourceHost);
		lock (_sync)
		{
			RegisterOwner_NoLock(sourceHost);
			var summaries = GetReachableHosts_NoLock(sourceHost);
			var exactAddress = summaries
				.Where(x => x.CanonicalAddress.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
				.ToList();
			if (exactAddress.Count == 1)
			{
				return exactAddress.Single();
			}

			if (exactAddress.Count > 1)
			{
				return null;
			}

			var exactHost = summaries
				.Where(x => x.Host.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
				.ToList();
			if (exactHost.Count == 1)
			{
				return exactHost.Single();
			}

			if (exactHost.Count > 1)
			{
				return null;
			}

			var partial = summaries
				.Where(x => x.CanonicalAddress.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) ||
				            x.Host.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
				.ToList();
			return partial.Count == 1 ? partial.Single() : null;
		}
	}

	public IEnumerable<ComputerNetworkServiceSummary> GetAdvertisedServices(IComputerHost sourceHost, IComputerHost targetHost)
	{
		EnsureLoadedForOwner(sourceHost);
		lock (_sync)
		{
			RegisterOwner_NoLock(sourceHost);
			return GetReachableHosts_NoLock(sourceHost)
				.Any(x => ReferenceEquals(x.Host, targetHost) || x.Host.OwnerHostItemId == targetHost.OwnerHostItemId)
				? GetAdvertisedServicesForHost_NoLock(targetHost)
				: Enumerable.Empty<ComputerNetworkServiceSummary>();
		}
	}

	public IEnumerable<IComputerExecutableDefinition> GetExecutables(ICharacter owner)
	{
		return GetExecutables(GetWorkspace(owner));
	}

	public IComputerExecutableDefinition? GetExecutable(ICharacter owner, string identifier)
	{
		return GetExecutable(GetWorkspace(owner), identifier);
	}

	public IComputerExecutableDefinition? GetExecutable(long id)
	{
		EnsureLoaded();
		lock (_sync)
		{
			if (_executables.TryGetValue(id, out var executable))
			{
				return executable;
			}

			if (_mutableExecutableOwners.TryGetValue(id, out var owner))
			{
				return owner.Executables.FirstOrDefault(x => x.Id == id);
			}
		}

		return EnumerateItemOwners()
			.SelectMany(x => x.Executables)
			.FirstOrDefault(x => x.Id == id);
	}

	public IComputerExecutableDefinition CreateExecutable(ICharacter owner, ComputerExecutableKind kind, string name)
	{
		return CreateExecutable(GetWorkspace(owner), kind, name);
	}

	public void SaveExecutable(IComputerExecutableDefinition executable)
	{
		EnsureLoadedForExecutable(executable);
		lock (_sync)
		{
			var owner = ResolveOwnerForExecutable_NoLock(executable);
			if (owner is null)
			{
				return;
			}

			SaveExecutable(owner, executable);
		}
	}

	public bool DeleteExecutable(ICharacter owner, IComputerExecutableDefinition executable, out string error)
	{
		return DeleteExecutable(GetWorkspace(owner), executable, out error);
	}

	public ComputerCompilationResult CompileExecutable(IComputerExecutableDefinition executable)
	{
		if (executable is IComputerBuiltInApplication)
		{
			return new ComputerCompilationResult
			{
				Success = true,
				ErrorMessage = string.Empty,
				Executable = executable
			};
		}

		EnsureLoadedForExecutable(executable);
		lock (_sync)
		{
			var owner = ResolveOwnerForExecutable_NoLock(executable);
			if (owner is null)
			{
				return new ComputerCompilationResult
				{
					Success = false,
					ErrorMessage = "That executable does not currently belong to a loaded computer owner.",
					Executable = executable
				};
			}

			return CompileExecutable_NoLock(owner, executable);
		}
	}

	public ComputerExecutionResult Execute(ICharacter owner, IComputerExecutableDefinition executable,
		IEnumerable<object?> parameters)
	{
		return Execute(owner, GetWorkspace(owner), executable, parameters);
	}

	public IEnumerable<IComputerProcess> GetProcesses(ICharacter owner)
	{
		return GetProcesses(GetWorkspace(owner));
	}

	public IComputerProcess? GetProcess(ICharacter owner, long processId)
	{
		return GetProcess(GetWorkspace(owner), processId);
	}

	public bool KillProcess(ICharacter owner, long processId, out string error)
	{
		return KillProcess(GetWorkspace(owner), processId, out error);
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

			LoadWorkspaceFromDatabase_NoLock();
			_loaded = true;
		}
	}

	private void EnsureLoadedForOwner(IComputerExecutableOwner owner)
	{
		if (owner is ICharacterComputerWorkspace)
		{
			EnsureLoaded();
		}
	}

	private void EnsureLoadedForExecutable(IComputerExecutableDefinition executable)
	{
		if (executable.OwnerCharacterId is > 0)
		{
			EnsureLoaded();
		}
	}

	private ICharacterComputerWorkspace GetOrCreateWorkspace_NoLock(ICharacter owner)
	{
		if (_workspaceOwners.TryGetValue(owner.Id, out var existing))
		{
			return existing;
		}

		var workspace = new CharacterComputerWorkspace(owner, () => GetExecutables(owner), () => GetProcesses(owner));
		_workspaceOwners[owner.Id] = workspace;
		return workspace;
	}

	private void RegisterOwner_NoLock(IComputerExecutableOwner owner)
	{
		if (owner is ICharacterComputerWorkspace workspace)
		{
			_workspaceOwners[workspace.Owner.Id] = workspace;
			return;
		}

		if (_registeredItemOwners.All(x => !ReferenceEquals(x, owner)))
		{
			_registeredItemOwners.Add(owner);
		}

		foreach (var executable in owner.Executables)
		{
			_mutableExecutableOwners[executable.Id] = owner;
		}
	}

	private IEnumerable<IComputerExecutableDefinition> ResolveExecutables_NoLock(IComputerExecutableOwner owner)
	{
		if (owner is ICharacterComputerWorkspace workspace)
		{
			return _executables.Values.Where(x => x.OwnerCharacterId == workspace.Owner.Id).ToList();
		}

		return owner.Executables.ToList();
	}

	private IComputerBuiltInApplication? ResolveBuiltInApplication_NoLock(IComputerExecutableOwner owner, string identifier)
	{
		return string.IsNullOrWhiteSpace(identifier)
			? null
			: ComputerBuiltInApplications.Get(owner.ExecutionHost, identifier.Trim());
	}

	private IEnumerable<IComputerProcess> ResolveProcesses_NoLock(IComputerExecutableOwner owner)
	{
		if (owner is ICharacterComputerWorkspace workspace)
		{
			return _processes.Values.Where(x => x.OwnerCharacterId == workspace.Owner.Id).ToList();
		}

		return owner.Processes.ToList();
	}

	private bool ExecutableBelongsToOwner(IComputerExecutableDefinition executable, IComputerExecutableOwner owner)
	{
		if (owner is ICharacterComputerWorkspace workspace)
		{
			return executable.OwnerCharacterId == workspace.Owner.Id;
		}

		return owner.OwnerStorageItemId.HasValue && executable.OwnerStorageItemId == owner.OwnerStorageItemId ||
		       owner.OwnerHostItemId.HasValue && executable.OwnerHostItemId == owner.OwnerHostItemId;
	}

	private void LoadWorkspaceFromDatabase_NoLock()
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
				var runtimeExecutable = CreateWorkspaceRuntimeExecutable(executable);
				_executables[runtimeExecutable.Id] = runtimeExecutable;
			}

			var processes = FMDB.Context.CharacterComputerProgramProcesses
				.AsNoTracking()
				.ToList();

			foreach (var process in processes)
			{
				if (!_executables.TryGetValue(process.CharacterComputerExecutableId, out var executable) ||
				    executable is not ComputerRuntimeProgramBase runtimeProgram)
				{
					continue;
				}

				var runtimeProcess = new ComputerRuntimeProcess
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
				if (ComputerProcessWaitArguments.TryParseUserInput(runtimeProcess.WaitArgument, out var waitingCharacterId,
					    out var waitingTerminalItemId))
				{
					runtimeProcess.WaitingCharacterId = waitingCharacterId;
					runtimeProcess.WaitingTerminalItemId = waitingTerminalItemId;
				}
				_processes[runtimeProcess.Id] = runtimeProcess;
			}
		}
	}

	private ComputerRuntimeExecutableBase CreateWorkspaceRuntimeExecutable(CharacterComputerExecutable dbitem)
	{
		ComputerRuntimeExecutableBase executable = dbitem.ExecutableKind == (int)ComputerExecutableKind.Function
			? new ComputerWorkspaceFunction(dbitem.Id, _gameworld)
			: new ComputerWorkspaceProgram(dbitem.Id, _gameworld)
			{
				AutorunOnBoot = dbitem.AutorunOnBoot
			};

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

	private IComputerExecutableDefinition CreateWorkspaceExecutable_NoLock(ICharacter owner, ComputerExecutableKind kind,
		string name)
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

			var executable = CreateWorkspaceRuntimeExecutable(dbitem);
			_executables[executable.Id] = executable;
			return executable;
		}
	}

	private void SaveWorkspaceExecutable_NoLock(IComputerExecutableDefinition executable)
	{
		if (executable is not ComputerRuntimeExecutableBase runtime)
		{
			return;
		}

		runtime.CompiledProg = null;
		runtime.CompilationStatus = ComputerCompilationStatus.NotCompiled;
		runtime.CompileError = string.Empty;
		runtime.LastModifiedAtUtc = DateTime.UtcNow;
		PersistWorkspaceExecutable_NoLock(runtime);
	}

	private bool DeleteWorkspaceExecutable_NoLock(ICharacter owner, IComputerExecutableDefinition executable, out string error)
	{
		if (executable.OwnerCharacterId != owner.Id)
		{
			error = "You do not own that computer executable.";
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

	private bool EnsureCompiled_NoLock(IComputerExecutableOwner owner, ComputerRuntimeExecutableBase executable,
		out string error)
	{
		if (executable.CompiledProg is not null && executable.CompilationStatus == ComputerCompilationStatus.Compiled)
		{
			error = string.Empty;
			return true;
		}

		var result = CompileExecutable_NoLock(owner, executable);
		error = result.ErrorMessage;
		return result.Success;
	}

	private ComputerCompilationResult CompileExecutable_NoLock(IComputerExecutableOwner owner,
		IComputerExecutableDefinition executable)
	{
		if (executable is not ComputerRuntimeExecutableBase runtime)
		{
			return new ComputerCompilationResult
			{
				Success = false,
				ErrorMessage = "That executable cannot be compiled by the computer compiler.",
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
		PersistExecutable_NoLock(owner, runtime);

		return new ComputerCompilationResult
		{
			Success = prog is not null,
			ErrorMessage = compileError,
			Executable = runtime
		};
	}

	private ComputerRuntimeProcess CreateProcess_NoLock(ICharacter? actor, IComputerExecutableOwner owner,
		ComputerRuntimeProgramBase program)
	{
		if (owner is ICharacterComputerWorkspace workspace)
		{
			var now = DateTime.UtcNow;
			using (new FMDB())
			{
				var dbprocess = new CharacterComputerProgramProcess
				{
					CharacterComputerExecutableId = program.Id,
					OwnerCharacterId = workspace.Owner.Id,
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

				var runtimeProcess = new ComputerRuntimeProcess
				{
					Id = dbprocess.Id,
					ProcessName = dbprocess.ProcessName,
					OwnerCharacterId = workspace.Owner.Id,
					Program = program,
					Host = CreateWorkspaceHost_NoLock(workspace.Owner.Id),
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

		if (owner is IComputerMutableOwner mutableOwner)
		{
			return mutableOwner.CreateProcessDefinition(actor, program);
		}

		throw new NotSupportedException("That computer executable owner does not support creating processes.");
	}

	private ComputerProgramExecutionOutcome ApplyExecutionOutcome_NoLock(IComputerExecutableOwner owner, ComputerRuntimeProcess process,
		ComputerProgramExecutionOutcome outcome)
	{
		if (outcome.Status == ComputerProcessStatus.Sleeping &&
		    outcome.WaitType == ComputerProcessWaitType.UserInput &&
		    outcome.WaitingCharacterId.HasValue &&
		    outcome.WaitingTerminalItemId.HasValue &&
		    FindWaitingUserInputProcesses_NoLock(
				    outcome.WaitingCharacterId.Value,
				    outcome.WaitingTerminalItemId.Value,
				    process.Id).Any())
		{
			outcome = new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = "Another program on that terminal session is already waiting for input."
			};
		}

		_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
		DetachSignalWait_NoLock(process.Id);
		process.Status = outcome.Status;
		process.WaitType = outcome.WaitType;
		process.WakeTimeUtc = outcome.WakeTimeUtc;
		process.WaitArgument = outcome.WaitArgument;
		process.WaitingCharacterId = outcome.WaitingCharacterId;
		process.WaitingTerminalItemId = outcome.WaitingTerminalItemId;
		process.Result = outcome.Result;
		process.LastError = outcome.Error;
		process.LastUpdatedAtUtc = DateTime.UtcNow;
		process.StateJson = outcome.Status == ComputerProcessStatus.Sleeping ? outcome.StateJson : string.Empty;
		process.EndedAtUtc = outcome.Status is ComputerProcessStatus.Completed or ComputerProcessStatus.Failed or ComputerProcessStatus.Killed
			? DateTime.UtcNow
			: null;
		PersistProcess_NoLock(owner, process);

		if (outcome.Status == ComputerProcessStatus.Sleeping && owner.ExecutionHost.Powered)
		{
			switch (outcome.WaitType)
			{
				case ComputerProcessWaitType.Sleep:
					ScheduleResume_NoLock(process);
					break;
				case ComputerProcessWaitType.Signal:
					AttachSignalWait_NoLock(owner, process, false);
					break;
			}
		}

		return outcome;
	}

	private IGameItem? ResolveSignalAnchorItem_NoLock(IComputerExecutableOwner owner)
	{
		if (owner.ExecutionHost.OwnerHostItemId is > 0L)
		{
			return _gameworld.TryGetItem(owner.ExecutionHost.OwnerHostItemId.Value, true);
		}

		if (owner.OwnerHostItemId is > 0L)
		{
			return _gameworld.TryGetItem(owner.OwnerHostItemId.Value, true);
		}

		if (owner.ExecutionHost.OwnerStorageItemId is > 0L)
		{
			return _gameworld.TryGetItem(owner.ExecutionHost.OwnerStorageItemId.Value, true);
		}

		if (owner.OwnerStorageItemId is > 0L)
		{
			return _gameworld.TryGetItem(owner.OwnerStorageItemId.Value, true);
		}

		return null;
	}

	private ISignalSourceComponent? ResolveWaitingSignalSource_NoLock(IComputerExecutableOwner owner,
		ComputerRuntimeProcess process)
	{
		if (!ComputerProcessWaitArguments.TryParseSignal(process.WaitArgument, out var binding))
		{
			return null;
		}

		var anchorItem = ResolveSignalAnchorItem_NoLock(owner);
		return anchorItem is null
			? null
			: SignalComponentUtilities.FindSignalSource(anchorItem, binding);
	}

	private void AttachSignalWait_NoLock(IComputerExecutableOwner owner, ComputerRuntimeProcess process,
		bool triggerImmediately)
	{
		if (process.Status != ComputerProcessStatus.Sleeping || process.WaitType != ComputerProcessWaitType.Signal)
		{
			return;
		}

		if (_signalWaitSubscriptions.ContainsKey(process.Id))
		{
			return;
		}

		var source = ResolveWaitingSignalSource_NoLock(owner, process);
		if (source is null)
		{
			ApplyExecutionOutcome_NoLock(owner, process, new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = "The awaited signal source is no longer available."
			});
			return;
		}

		SignalChangedEvent handler = (signalSource, signal) => HandleSignalWaitTriggered(process.Id, signalSource, signal);
		source.SignalChanged += handler;
		_signalWaitSubscriptions[process.Id] = new ComputerSignalWaitSubscription
		{
			ProcessId = process.Id,
			Source = source,
			Handler = handler
		};

		if (triggerImmediately && Math.Abs(source.CurrentSignal.Value) > 0.0000001)
		{
			HandleSignalWaitTriggered_NoLock(owner, process, source.CurrentSignal);
		}
	}

	private void DetachSignalWait_NoLock(long processId)
	{
		if (!_signalWaitSubscriptions.Remove(processId, out var subscription))
		{
			return;
		}

		subscription.Detach();
	}

	private void HandleSignalWaitTriggered(long processId, ISignalSourceComponent source, ComputerSignal signal)
	{
		lock (_sync)
		{
			if (!_signalWaitSubscriptions.TryGetValue(processId, out var subscription) ||
			    !ReferenceEquals(subscription.Source, source))
			{
				return;
			}

			var process = EnumerateAllProcesses_NoLock()
				.OfType<ComputerRuntimeProcess>()
				.FirstOrDefault(x => x.Id == processId);
			if (process is null)
			{
				DetachSignalWait_NoLock(processId);
				return;
			}

			var owner = ResolveOwnerForExecutable_NoLock(process.Program);
			if (owner is null)
			{
				DetachSignalWait_NoLock(processId);
				return;
			}

			HandleSignalWaitTriggered_NoLock(owner, process, signal);
		}
	}

	private void HandleSignalWaitTriggered_NoLock(IComputerExecutableOwner owner, ComputerRuntimeProcess process,
		ComputerSignal signal)
	{
		if (Math.Abs(signal.Value) < 0.0000001)
		{
			return;
		}

		var liveProcess = ResolveProcesses_NoLock(owner)
			.OfType<ComputerRuntimeProcess>()
			.FirstOrDefault(x => x.Id == process.Id);
		if (liveProcess is null || liveProcess.Status != ComputerProcessStatus.Sleeping ||
		    liveProcess.WaitType != ComputerProcessWaitType.Signal ||
		    liveProcess.Program is not ComputerRuntimeProgramBase program ||
		    !owner.ExecutionHost.Powered)
		{
			return;
		}

		DetachSignalWait_NoLock(liveProcess.Id);
		if (!EnsureCompiled_NoLock(owner, program, out var compileError))
		{
			ApplyExecutionOutcome_NoLock(owner, liveProcess, new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = compileError
			});
			return;
		}

		using var scope = new ComputerExecutionContextScope(new ComputerExecutionContext
		{
			Owner = owner,
			Host = owner.ExecutionHost,
			Gameworld = _gameworld,
			Process = liveProcess,
			PendingSignalInput = signal
		});

		liveProcess.Status = ComputerProcessStatus.Running;
		liveProcess.WaitType = ComputerProcessWaitType.None;
		liveProcess.WakeTimeUtc = null;
		liveProcess.WaitArgument = null;
		liveProcess.WaitingCharacterId = null;
		liveProcess.WaitingTerminalItemId = null;
		liveProcess.LastUpdatedAtUtc = DateTime.UtcNow;
		PersistProcess_NoLock(owner, liveProcess);

		var outcome = ComputerProgramExecutor.Execute(program, Enumerable.Empty<object?>(), liveProcess.StateJson);
		ApplyExecutionOutcome_NoLock(owner, liveProcess, outcome);
	}

	private IEnumerable<ComputerRuntimeProcess> EnumerateAllProcesses_NoLock()
	{
		return _processes.Values
			.Concat(EnumerateItemOwners()
				.SelectMany(x => x.Processes)
				.OfType<ComputerRuntimeProcess>());
	}

	private IEnumerable<ComputerRuntimeProcess> FindWaitingUserInputProcesses_NoLock(IComputerTerminalSession session)
	{
		return FindWaitingUserInputProcesses_NoLock(session.User.Id, session.Terminal.TerminalItemId, null);
	}

	private IEnumerable<ComputerRuntimeProcess> FindWaitingUserInputProcesses_NoLock(long waitingCharacterId,
		long waitingTerminalItemId,
		long? excludingProcessId)
	{
		return EnumerateAllProcesses_NoLock()
			.Where(x => x.Status == ComputerProcessStatus.Sleeping)
			.Where(x => x.WaitType == ComputerProcessWaitType.UserInput)
			.Where(x => x.WaitingCharacterId == waitingCharacterId)
			.Where(x => x.WaitingTerminalItemId == waitingTerminalItemId)
			.Where(x => !excludingProcessId.HasValue || x.Id != excludingProcessId.Value)
			.ToList();
	}

	private bool ResumeWaitingProcessFromTerminalInput_NoLock(ComputerRuntimeProcess process, IComputerTerminalSession session,
		string text,
		out string error)
	{
		var owner = ResolveOwnerForProcess_NoLock(process);
		if (owner is null)
		{
			error = "That computer program's owner is no longer available.";
			return false;
		}

		var liveProcess = ResolveProcesses_NoLock(owner)
			.OfType<ComputerRuntimeProcess>()
			.FirstOrDefault(x => x.Id == process.Id);
		if (liveProcess is null || liveProcess.Status != ComputerProcessStatus.Sleeping ||
		    liveProcess.WaitType != ComputerProcessWaitType.UserInput)
		{
			error = "That computer program is no longer waiting for terminal input.";
			return false;
		}

		if (liveProcess.Program is ComputerRuntimeProgramBase program)
		{
			if (!EnsureCompiled_NoLock(owner, program, out var compileError))
			{
				var failureOutcome = ApplyExecutionOutcome_NoLock(owner, liveProcess, new ComputerProgramExecutionOutcome
				{
					Status = ComputerProcessStatus.Failed,
					Error = compileError
				});
				error = failureOutcome.Error ?? compileError;
				return false;
			}

			using var scope = new ComputerExecutionContextScope(new ComputerExecutionContext
			{
				Owner = owner,
				Host = owner.ExecutionHost,
				Gameworld = _gameworld,
				Actor = session.User,
				Session = session,
				Process = liveProcess,
			PendingTerminalInput = text
		});

		liveProcess.Status = ComputerProcessStatus.Running;
		liveProcess.WaitType = ComputerProcessWaitType.None;
		liveProcess.WakeTimeUtc = null;
		liveProcess.WaitArgument = null;
		liveProcess.WaitingCharacterId = null;
		liveProcess.WaitingTerminalItemId = null;
		liveProcess.LastUpdatedAtUtc = DateTime.UtcNow;
		PersistProcess_NoLock(owner, liveProcess);

			var outcome = ComputerProgramExecutor.Execute(program, Enumerable.Empty<object?>(), liveProcess.StateJson);
			outcome = ApplyExecutionOutcome_NoLock(owner, liveProcess, outcome);
			error = outcome.Status == ComputerProcessStatus.Failed ? outcome.Error ?? string.Empty : string.Empty;
			return outcome.Status != ComputerProcessStatus.Failed;
		}

		if (liveProcess.Program is IComputerBuiltInApplication builtInApplication)
		{
			using var scope = new ComputerExecutionContextScope(new ComputerExecutionContext
			{
				Owner = owner,
				Host = owner.ExecutionHost,
				Gameworld = _gameworld,
				Actor = session.User,
				Session = session,
				Process = liveProcess,
				PendingTerminalInput = text
			});

			liveProcess.Status = ComputerProcessStatus.Running;
			liveProcess.WaitType = ComputerProcessWaitType.None;
			liveProcess.WakeTimeUtc = null;
			liveProcess.WaitArgument = null;
			liveProcess.WaitingCharacterId = null;
			liveProcess.WaitingTerminalItemId = null;
			liveProcess.LastUpdatedAtUtc = DateTime.UtcNow;
			PersistProcess_NoLock(owner, liveProcess);

			var outcome = ComputerBuiltInApplicationExecutors.Execute(
				_gameworld,
				session.User,
				owner,
				session,
				liveProcess,
				builtInApplication);
			outcome = ApplyExecutionOutcome_NoLock(owner, liveProcess, outcome);
			error = outcome.Status == ComputerProcessStatus.Failed ? outcome.Error ?? string.Empty : string.Empty;
			return outcome.Status != ComputerProcessStatus.Failed;
		}

		error = "That computer program is no longer waiting for terminal input.";
		return false;
	}

	private List<ComputerNetworkHostSummary> GetReachableHosts_NoLock(IComputerHost sourceHost)
	{
		return sourceHost.NetworkAdapters
			.Where(x => x.NetworkReady)
			.Where(x => x.TelecommunicationsGrid is not null)
			.SelectMany(x => x.TelecommunicationsGrid!.GetReachableNetworkEndpoints())
			.Where(x => x.Adapter.ConnectedHost is not null)
			.GroupBy(x => x.Adapter.NetworkAdapterItemId)
			.Select(x => x.First())
			.Select(endpoint =>
			{
				var host = endpoint.Adapter.ConnectedHost!;
				var services = GetAdvertisedServicesForHost_NoLock(host);
				return new ComputerNetworkHostSummary
				{
					Host = host,
					Adapter = endpoint.Adapter,
					Grid = endpoint.Grid,
					CanonicalAddress = endpoint.CanonicalAddress,
					IsLocalGrid = endpoint.IsLocalGrid,
					Available = endpoint.Adapter.NetworkReady && host.Powered,
					AdvertisedServiceCount = services.Count
				};
			})
			.Where(x => x.Available)
			.OrderBy(x => x.IsLocalGrid ? 0 : 1)
			.ThenBy(x => x.Host.Name)
			.ThenBy(x => x.CanonicalAddress)
			.ToList();
	}

	private List<ComputerNetworkServiceSummary> GetAdvertisedServicesForHost_NoLock(IComputerHost targetHost)
	{
		return targetHost.BuiltInApplications
			.Where(x => x.IsNetworkService)
			.Where(ComputerBuiltInApplicationExecutors.IsImplemented)
			.Where(x => targetHost.IsNetworkServiceEnabled(x.ApplicationId))
			.OrderBy(x => x.Name)
			.ThenBy(x => x.Id)
			.Select(x =>
			{
				var details = _gameworld.ComputerMailService.GetAdvertisedServiceDetails(targetHost, x.ApplicationId)
					.ToList();
				return new ComputerNetworkServiceSummary
				{
					ApplicationId = x.ApplicationId,
					Name = x.Name,
					Summary = x.Summary,
					ServiceDetails = details
				};
			})
			.Where(x => x.ApplicationId != "mail" || x.ServiceDetails.Any())
			.ToList();
	}

	private void PersistExecutable_NoLock(IComputerExecutableOwner owner, ComputerRuntimeExecutableBase executable)
	{
		if (owner is ICharacterComputerWorkspace)
		{
			PersistWorkspaceExecutable_NoLock(executable);
			return;
		}

		if (owner is IComputerMutableOwner mutableOwner)
		{
			mutableOwner.SaveExecutableDefinition(executable);
		}
	}

	private void PersistWorkspaceExecutable_NoLock(ComputerRuntimeExecutableBase executable)
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

	private void PersistProcess_NoLock(IComputerExecutableOwner owner, ComputerRuntimeProcess process)
	{
		if (owner is ICharacterComputerWorkspace)
		{
			PersistWorkspaceProcess_NoLock(process);
			return;
		}

		if (owner is IComputerMutableOwner mutableOwner)
		{
			mutableOwner.SaveProcessDefinition(process);
		}
	}

	private void PersistWorkspaceProcess_NoLock(ComputerRuntimeProcess process)
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
						.Cast<IComputerExecutableDefinition>()
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

	private void ScheduleResume_NoLock(ComputerRuntimeProcess process)
	{
		if (process.Status != ComputerProcessStatus.Sleeping)
		{
			return;
		}

		if (process.WaitType != ComputerProcessWaitType.Sleep)
		{
			return;
		}

		var delay = (process.WakeTimeUtc ?? DateTime.UtcNow) - DateTime.UtcNow;
		if (delay < TimeSpan.Zero)
		{
			delay = TimeSpan.Zero;
		}

		_gameworld.Scheduler.Destroy(process, ScheduleType.ComputerProgram);
		_gameworld.Scheduler.AddSchedule(new Schedule<ComputerRuntimeProcess>(
			process,
			ResumeScheduledProcess,
			ScheduleType.ComputerProgram,
			delay,
			$"Resume computer program process #{process.Id}"));
	}

	private void ResumeScheduledProcess(ComputerRuntimeProcess process)
	{
		EnsureLoaded();
		lock (_sync)
		{
			var owner = ResolveOwnerForProcess_NoLock(process);
			if (owner is null)
			{
				return;
			}

			var liveProcess = ResolveProcesses_NoLock(owner)
				.OfType<ComputerRuntimeProcess>()
				.FirstOrDefault(x => x.Id == process.Id);
			if (liveProcess is null || liveProcess.Status != ComputerProcessStatus.Sleeping ||
			    liveProcess.Program is not ComputerRuntimeProgramBase program)
			{
				return;
			}

			if (!EnsureCompiled_NoLock(owner, program, out var compileError))
			{
				ApplyExecutionOutcome_NoLock(owner, liveProcess, new ComputerProgramExecutionOutcome
				{
					Status = ComputerProcessStatus.Failed,
					Error = compileError
				});
				return;
			}

			using var scope = new ComputerExecutionContextScope(new ComputerExecutionContext
			{
				Owner = owner,
				Host = owner.ExecutionHost,
				Gameworld = _gameworld,
				Process = liveProcess
			});

			liveProcess.Status = ComputerProcessStatus.Running;
			liveProcess.WaitType = ComputerProcessWaitType.None;
			liveProcess.WakeTimeUtc = null;
			liveProcess.WaitArgument = null;
			liveProcess.WaitingCharacterId = null;
			liveProcess.WaitingTerminalItemId = null;
			liveProcess.LastUpdatedAtUtc = DateTime.UtcNow;
			PersistProcess_NoLock(owner, liveProcess);

			var outcome = ComputerProgramExecutor.Execute(program, Enumerable.Empty<object?>(), liveProcess.StateJson);
			ApplyExecutionOutcome_NoLock(owner, liveProcess, outcome);
		}
	}

	private IComputerExecutableOwner? ResolveOwnerForProcess_NoLock(ComputerRuntimeProcess process)
	{
		if (process.Program.OwnerCharacterId is > 0 &&
		    _workspaceOwners.TryGetValue(process.Program.OwnerCharacterId.Value, out var cachedWorkspace))
		{
			return cachedWorkspace;
		}

		var owner = EnumerateItemOwners()
			.FirstOrDefault(candidate => ResolveProcesses_NoLock(candidate)
				.OfType<ComputerRuntimeProcess>()
				.Any(runtimeProcess => ReferenceEquals(runtimeProcess, process) || runtimeProcess.Id == process.Id));
		if (owner is not null)
		{
			return owner;
		}

		if (process.Host is IComputerExecutableOwner hostOwner)
		{
			return hostOwner;
		}

		return ResolveOwnerForExecutable_NoLock(process.Program);
	}

	private IComputerExecutableOwner? ResolveOwnerForExecutable_NoLock(IComputerExecutableDefinition executable)
	{
		if (_mutableExecutableOwners.TryGetValue(executable.Id, out var mutableOwner))
		{
			return mutableOwner;
		}

		if (executable.OwnerCharacterId is > 0 &&
		    _workspaceOwners.TryGetValue(executable.OwnerCharacterId.Value, out var cachedWorkspace))
		{
			return cachedWorkspace;
		}

		if (executable.OwnerStorageItemId is > 0)
		{
			return _gameworld.TryGetItem(executable.OwnerStorageItemId.Value, true)?.Components
				.OfType<IComputerStorage>()
				.FirstOrDefault();
		}

		if (executable.OwnerHostItemId is > 0)
		{
			return _gameworld.TryGetItem(executable.OwnerHostItemId.Value, true)?.Components
				.OfType<IComputerHost>()
				.FirstOrDefault();
		}

		if (executable.OwnerCharacterId is > 0)
		{
			var owner = _gameworld.TryGetCharacter(executable.OwnerCharacterId.Value, true);
			if (owner is null)
			{
				return null;
			}

			return GetOrCreateWorkspace_NoLock(owner);
		}

		return null;
	}

	private IEnumerable<IComputerExecutableOwner> EnumerateItemOwners()
	{
		var worldOwners = (_gameworld.Items ?? Enumerable.Empty<IGameItem>())
			.SelectMany(item => item.Components
				.OfType<IComputerHost>()
				.Cast<IComputerExecutableOwner>()
				.Concat(item.Components.OfType<IComputerStorage>()));
		return worldOwners
			.Concat(_registeredItemOwners)
			.Concat(_mutableExecutableOwners.Values.Distinct())
			.Distinct();
	}
}
