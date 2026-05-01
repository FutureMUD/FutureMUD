#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements;
using MudSharp.TimeAndDate;

namespace MudSharp.Computers;

internal sealed class ComputerProgramExecutionOutcome
{
	public ComputerProcessStatus Status { get; init; }
	public ComputerProcessWaitType WaitType { get; init; }
	public DateTime? WakeTimeUtc { get; init; }
	public string? WaitArgument { get; init; }
	public long? WaitingCharacterId { get; init; }
	public long? WaitingTerminalItemId { get; init; }
	public object? Result { get; init; }
	public string? Error { get; init; }
	public string StateJson { get; init; } = string.Empty;
}

internal static class ComputerProgramExecutor
{
	private const string RootPath = "__root__";

	private sealed class PersistedProgValue
	{
		public string Type { get; set; } = string.Empty;
		public bool IsNull { get; set; }
		public string? Scalar { get; set; }
		public List<PersistedProgValue>? Items { get; set; }
		public Dictionary<string, PersistedProgValue>? Map { get; set; }
		public Dictionary<string, List<PersistedProgValue>>? MultiMap { get; set; }
	}

	private sealed class PersistedExecutionFrame
	{
		public string Kind { get; set; } = string.Empty;
		public string OwnerPath { get; set; } = RootPath;
		public string BranchKey { get; set; } = string.Empty;
		public int NextIndex { get; set; }
		public Dictionary<string, PersistedProgValue>? LocalVariables { get; set; }
		public int IterationCount { get; set; }
		public int CurrentIndex { get; set; }
		public int? TotalIterations { get; set; }
		public List<PersistedProgValue>? CollectionItems { get; set; }
	}

	private sealed class PersistedProgramState
	{
		public List<PersistedExecutionFrame> Frames { get; set; } = new();
	}

	private abstract class ExecutionFrame
	{
		public abstract string Kind { get; }
	}

	private sealed class BlockFrame : ExecutionFrame
	{
		public override string Kind => IsSwitchBlock ? "switch" : "block";
		public string OwnerPath { get; init; } = RootPath;
		public string BranchKey { get; init; } = string.Empty;
		public int NextIndex { get; set; }
		public Dictionary<string, IProgVariable> LocalVariables { get; init; } = new(StringComparer.InvariantCultureIgnoreCase);
		public bool IsSwitchBlock { get; init; }
	}

	private sealed class WhileFrame : ExecutionFrame
	{
		public override string Kind => "while";
		public string StatementPath { get; init; } = string.Empty;
		public int IterationCount { get; set; }
	}

	private sealed class ForFrame : ExecutionFrame
	{
		public override string Kind => "for";
		public string StatementPath { get; init; } = string.Empty;
		public int CurrentIndex { get; set; } = 1;
		public int? TotalIterations { get; set; }
	}

	private sealed class ForEachFrame : ExecutionFrame
	{
		public override string Kind => "foreach";
		public string StatementPath { get; init; } = string.Empty;
		public int CurrentIndex { get; set; }
		public List<IProgVariable>? CollectionItems { get; set; }
	}

	private sealed class StatementNode
	{
		public required string Path { get; init; }
		public required IStatement Statement { get; init; }
		public Dictionary<string, IReadOnlyList<StatementNode>> Blocks { get; } =
			new(StringComparer.InvariantCultureIgnoreCase);
	}

	private sealed class ComputerProgramStructure
	{
		private readonly Dictionary<string, StatementNode> _nodes = new(StringComparer.InvariantCultureIgnoreCase);

		public ComputerProgramStructure(MudSharp.FutureProg.FutureProg prog)
		{
			RootStatements = BuildStatements(prog.Statements, string.Empty);
		}

		public IReadOnlyList<StatementNode> RootStatements { get; }

		public StatementNode GetStatement(string path)
		{
			return _nodes[path];
		}

		public IReadOnlyList<StatementNode> GetBlock(string ownerPath, string branchKey)
		{
			if (ownerPath == RootPath)
			{
				return RootStatements;
			}

			return _nodes[ownerPath].Blocks.TryGetValue(branchKey, out var block)
				? block
				: Array.Empty<StatementNode>();
		}

		private IReadOnlyList<StatementNode> BuildStatements(IReadOnlyList<IStatement> statements, string prefix)
		{
			List<StatementNode> nodes = new();
			for (var i = 0; i < statements.Count; i++)
			{
				var path = string.IsNullOrEmpty(prefix) ? i.ToString(CultureInfo.InvariantCulture) : $"{prefix}/{i}";
				var statement = statements[i];
				var node = new StatementNode
				{
					Path = path,
					Statement = statement
				};
				_nodes[path] = node;
				nodes.Add(node);

				switch (statement)
				{
					case IfBlock ifBlock:
						node.Blocks["if:true"] = BuildStatements(ifBlock.TrueStatements, $"{path}/if:true");
						for (var index = 0; index < ifBlock.ElseIfStatementBlocks.Count; index++)
						{
							node.Blocks[$"if:elseif:{index}"] =
								BuildStatements(ifBlock.ElseIfStatementBlocks[index].Statements, $"{path}/if:elseif:{index}");
						}
						node.Blocks["if:false"] = BuildStatements(ifBlock.FalseStatements, $"{path}/if:false");
						break;
					case Switch @switch:
						for (var index = 0; index < @switch.CaseBlocks.Count; index++)
						{
							node.Blocks[$"switch:case:{index}"] =
								BuildStatements(@switch.CaseBlocks[index].Statements, $"{path}/switch:case:{index}");
						}
						node.Blocks["switch:default"] =
							BuildStatements(@switch.DefaultStatements, $"{path}/switch:default");
						break;
					case WhileLoop whileLoop:
						node.Blocks["while:body"] = BuildStatements(whileLoop.BodyStatements, $"{path}/while:body");
						break;
					case ForLoop forLoop:
						node.Blocks["for:body"] = BuildStatements(forLoop.BodyStatements, $"{path}/for:body");
						break;
					case ForEachLoop forEachLoop:
						node.Blocks["foreach:body"] =
							BuildStatements(forEachLoop.BodyStatements, $"{path}/foreach:body");
						break;
				}
			}

			return nodes;
		}
	}

	private sealed class StackVariableSpace : IVariableSpace
	{
		private readonly IReadOnlyList<Dictionary<string, IProgVariable>> _scopes;

		public StackVariableSpace(IReadOnlyList<Dictionary<string, IProgVariable>> scopes)
		{
			_scopes = scopes;
		}

		public IProgVariable GetVariable(string variable)
		{
			foreach (var scope in _scopes)
			{
				if (scope.TryGetValue(variable, out var value))
				{
					return value;
				}
			}

			throw new ApplicationException($"Unknown variable {variable} in computer program.");
		}

		public bool HasVariable(string variable)
		{
			return _scopes.Any(x => x.ContainsKey(variable));
		}

		public void SetVariable(string variable, IProgVariable value)
		{
			foreach (var scope in _scopes)
			{
				if (scope.ContainsKey(variable))
				{
					scope[variable] = value;
					return;
				}
			}

			_scopes[0][variable] = value;
		}
	}

	public static ComputerProgramExecutionOutcome Execute(
		ComputerRuntimeProgramBase program,
		IEnumerable<object?> parameters,
		string? existingStateJson = null)
	{
		if (program.CompiledProg is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = "The computer program is not compiled."
			};
		}

		var structure = new ComputerProgramStructure(program.CompiledProg);
		var frames = string.IsNullOrWhiteSpace(existingStateJson)
			? CreateInitialFrames(program, parameters)
			: RestoreFrames(existingStateJson!, program.CompiledProg.InternalGameworld);

		while (frames.Any())
		{
			try
			{
				switch (frames[^1])
				{
					case BlockFrame blockFrame:
					{
						var statements = structure.GetBlock(blockFrame.OwnerPath, blockFrame.BranchKey);
						if (blockFrame.NextIndex >= statements.Count)
						{
							if (blockFrame.OwnerPath == RootPath)
							{
								return Complete(program.ReturnType, frames);
							}

							frames.RemoveAt(frames.Count - 1);
							continue;
						}

						var node = statements[blockFrame.NextIndex];
						switch (node.Statement)
						{
							case SleepStatement sleep:
							{
								var variableSpace = BuildVariableSpace(frames);
								if (sleep.DurationFunction.Execute(variableSpace) == StatementResult.Error)
								{
									return Failure(sleep.ErrorMessage);
								}

								var duration = (TimeSpan?)(sleep.DurationFunction.Result?.GetObject) ?? TimeSpan.Zero;
								if (duration < TimeSpan.Zero)
								{
									duration = TimeSpan.Zero;
								}

								blockFrame.NextIndex++;
								return new ComputerProgramExecutionOutcome
								{
									Status = ComputerProcessStatus.Sleeping,
									WaitType = ComputerProcessWaitType.Sleep,
									WakeTimeUtc = DateTime.UtcNow + duration,
									WaitArgument = duration.ToString(),
									StateJson = PersistFrames(frames)
								};
							}
							case IfBlock ifBlock:
							{
								var ifError = ExecuteIf(frames, structure, node.Path, ifBlock);
								if (!string.IsNullOrEmpty(ifError))
								{
									return Failure(ifError);
								}

								blockFrame.NextIndex++;
								continue;
							}
							case Switch switchStatement:
							{
								var switchError = ExecuteSwitch(frames, structure, node.Path, switchStatement);
								if (!string.IsNullOrEmpty(switchError))
								{
									return Failure(switchError);
								}

								blockFrame.NextIndex++;
								continue;
							}
							case WhileLoop:
								blockFrame.NextIndex++;
								frames.Add(new WhileFrame { StatementPath = node.Path });
								continue;
							case ForLoop:
								blockFrame.NextIndex++;
								frames.Add(new ForFrame { StatementPath = node.Path });
								continue;
							case ForEachLoop:
								blockFrame.NextIndex++;
								frames.Add(new ForEachFrame { StatementPath = node.Path });
								continue;
							default:
							{
								var variableSpace = BuildVariableSpace(frames);
								var result = node.Statement.Execute(variableSpace);
								switch (result)
								{
									case StatementResult.Normal:
										blockFrame.NextIndex++;
										continue;
									case StatementResult.Return:
										return Complete(program.ReturnType, frames);
									case StatementResult.Error:
										return Failure(node.Statement.ErrorMessage);
									case StatementResult.Break:
									case StatementResult.Continue:
										if (ConsumeFlowControl(frames, blockFrame, result))
										{
											continue;
										}

										return Failure($"{result.DescribeEnum()} was encountered outside a valid block.");
									default:
										return Failure("Unsupported computer-program execution result.");
								}
							}
						}
					}
					case WhileFrame whileFrame:
					{
						var whileLoop = (WhileLoop)structure.GetStatement(whileFrame.StatementPath).Statement;
						var variableSpace = BuildVariableSpace(frames);
						if (whileLoop.ConditionFunction.Execute(variableSpace) == StatementResult.Error)
						{
							return Failure(whileLoop.ConditionFunction.ErrorMessage);
						}

						if (!((bool?)whileLoop.ConditionFunction.Result?.GetObject ?? false))
						{
							frames.RemoveAt(frames.Count - 1);
							continue;
						}

						if (whileFrame.IterationCount++ > 10000)
						{
							return Failure("While loop of greater than 10,000 iterations detected, aborting...");
						}

						frames.Add(new BlockFrame
						{
							OwnerPath = whileFrame.StatementPath,
							BranchKey = "while:body"
						});
						continue;
					}
					case ForFrame forFrame:
					{
						var forLoop = (ForLoop)structure.GetStatement(forFrame.StatementPath).Statement;
						var variableSpace = BuildVariableSpace(frames);
						if (forFrame.TotalIterations is null)
						{
							var repetitionsResult = forLoop.RepetitionsExpression.Execute(variableSpace);
							if (repetitionsResult == StatementResult.Error)
							{
								return Failure(forLoop.RepetitionsExpression.ErrorMessage);
							}

							forFrame.TotalIterations =
								Convert.ToInt32((decimal?)forLoop.RepetitionsExpression.Result?.GetObject ?? 0.0M);
						}

						if (forFrame.TotalIterations <= 0 || forFrame.CurrentIndex > forFrame.TotalIterations)
						{
							frames.RemoveAt(frames.Count - 1);
							continue;
						}

						var locals = new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase)
						{
							[forLoop.LoopVariableName] = new MudSharp.FutureProg.Variables.NumberVariable(forFrame.CurrentIndex)
						};
						forFrame.CurrentIndex++;
						frames.Add(new BlockFrame
						{
							OwnerPath = forFrame.StatementPath,
							BranchKey = "for:body",
							LocalVariables = locals
						});
						continue;
					}
					case ForEachFrame forEachFrame:
					{
						var forEachLoop = (ForEachLoop)structure.GetStatement(forEachFrame.StatementPath).Statement;
						var variableSpace = BuildVariableSpace(frames);
						if (forEachFrame.CollectionItems is null)
						{
							var collectionResult = forEachLoop.CollectionExpression.Execute(variableSpace);
							if (collectionResult == StatementResult.Error)
							{
								return Failure(forEachLoop.CollectionExpression.ErrorMessage);
							}

							forEachFrame.CollectionItems = ((IList?)forEachLoop.CollectionExpression.Result?.GetObject)
								?.OfType<IProgVariable>()
								.ToList() ?? new List<IProgVariable>();
						}

						if (forEachFrame.CurrentIndex >= forEachFrame.CollectionItems.Count)
						{
							frames.RemoveAt(frames.Count - 1);
							continue;
						}

						var locals = new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase)
						{
							[forEachLoop.LoopVariableName] = forEachFrame.CollectionItems[forEachFrame.CurrentIndex]
						};
						forEachFrame.CurrentIndex++;
						frames.Add(new BlockFrame
						{
							OwnerPath = forEachFrame.StatementPath,
							BranchKey = "foreach:body",
							LocalVariables = locals
						});
						continue;
					}
				}
			}
			catch (ComputerProgramWaitException wait)
			{
				return new ComputerProgramExecutionOutcome
				{
					Status = ComputerProcessStatus.Sleeping,
					WaitType = wait.WaitType,
					WaitArgument = wait.WaitArgument,
					WaitingCharacterId = wait.WaitingCharacterId,
					WaitingTerminalItemId = wait.WaitingTerminalItemId,
					StateJson = PersistFrames(frames)
				};
			}
		}

		return Complete(program.ReturnType, frames);
	}

	private static string ExecuteIf(List<ExecutionFrame> frames, ComputerProgramStructure structure, string path, IfBlock ifBlock)
	{
		var variableSpace = BuildVariableSpace(frames);
		if (ifBlock.ConditionFunction.Execute(variableSpace) == StatementResult.Error)
		{
			return ifBlock.ConditionFunction.ErrorMessage;
		}

		if ((bool?)ifBlock.ConditionFunction.Result?.GetObject ?? false)
		{
			PushBlockIfNotEmpty(frames, structure, path, "if:true", false);
			return string.Empty;
		}

		for (var index = 0; index < ifBlock.ElseIfStatementBlocks.Count; index++)
		{
			var elseIf = ifBlock.ElseIfStatementBlocks[index];
			if (elseIf.ElseLogic.Execute(variableSpace) == StatementResult.Error)
			{
				return elseIf.ElseLogic.ErrorMessage;
			}

			if (!((bool?)elseIf.ElseLogic.Result?.GetObject ?? false))
			{
				continue;
			}

			PushBlockIfNotEmpty(frames, structure, path, $"if:elseif:{index}", false);
			return string.Empty;
		}

		PushBlockIfNotEmpty(frames, structure, path, "if:false", false);
		return string.Empty;
	}

	private static string ExecuteSwitch(List<ExecutionFrame> frames, ComputerProgramStructure structure, string path, Switch switchStatement)
	{
		var variableSpace = BuildVariableSpace(frames);
		if (switchStatement.SwitchExpression.Execute(variableSpace) == StatementResult.Error)
		{
			return switchStatement.SwitchExpression.ErrorMessage;
		}

		var switchValue = switchStatement.SwitchExpression.Result?.GetObject;
		for (var index = 0; index < switchStatement.CaseBlocks.Count; index++)
		{
			var caseBlock = switchStatement.CaseBlocks[index];
			if (caseBlock.CaseExpression.Execute(variableSpace) == StatementResult.Error)
			{
				return caseBlock.CaseExpression.ErrorMessage;
			}

			if (!Equals(caseBlock.CaseExpression.Result?.GetObject, switchValue))
			{
				continue;
			}

			PushBlockIfNotEmpty(frames, structure, path, $"switch:case:{index}", true);
			return string.Empty;
		}

		PushBlockIfNotEmpty(frames, structure, path, "switch:default", true);
		return string.Empty;
	}

	private static void PushBlockIfNotEmpty(List<ExecutionFrame> frames, ComputerProgramStructure structure, string ownerPath,
		string branchKey, bool isSwitchBlock)
	{
		var statements = structure.GetBlock(ownerPath, branchKey);
		if (!statements.Any())
		{
			return;
		}

		frames.Add(new BlockFrame
		{
			OwnerPath = ownerPath,
			BranchKey = branchKey,
			IsSwitchBlock = isSwitchBlock
		});
	}

	private static bool ConsumeFlowControl(List<ExecutionFrame> frames, BlockFrame currentBlock, StatementResult flowControl)
	{
		if (flowControl == StatementResult.Break && currentBlock.IsSwitchBlock)
		{
			frames.RemoveAt(frames.Count - 1);
			return true;
		}

		frames.RemoveAt(frames.Count - 1);
		while (frames.Any())
		{
			switch (frames[^1])
			{
				case BlockFrame parentBlock when !parentBlock.IsSwitchBlock:
					frames.RemoveAt(frames.Count - 1);
					continue;
				case BlockFrame switchBlock:
					frames.RemoveAt(frames.Count - 1);
					if (flowControl == StatementResult.Break)
					{
						return true;
					}

					continue;
				case WhileFrame:
				case ForFrame:
				case ForEachFrame:
					if (flowControl == StatementResult.Break)
					{
						frames.RemoveAt(frames.Count - 1);
					}

					return true;
			}
		}

		return false;
	}

	private static ComputerProgramExecutionOutcome Complete(ProgVariableTypes returnType, IReadOnlyList<ExecutionFrame> frames)
	{
		var rootFrame = frames.OfType<BlockFrame>().LastOrDefault(x => x.OwnerPath == RootPath);
		if (rootFrame is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Completed
			};
		}

		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Completed,
			Result = returnType == ProgVariableTypes.Void
				? null
				: rootFrame.LocalVariables.TryGetValue("return", out var result)
					? result.GetObject
					: null
		};
	}

	private static ComputerProgramExecutionOutcome Failure(string? error)
	{
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Failed,
			Error = error ?? "Unknown computer program execution error."
		};
	}

	private static List<ExecutionFrame> CreateInitialFrames(ComputerRuntimeProgramBase program,
		IEnumerable<object?> parameters)
	{
		var frames = new List<ExecutionFrame>();
		var rootLocals = new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase);
		var parameterValues = parameters.ToList();
		var parameterDefinitions = program.Parameters.ToList();
		for (var index = 0; index < parameterDefinitions.Count; index++)
		{
			rootLocals[parameterDefinitions[index].Name.ToLowerInvariant()] =
				MudSharp.FutureProg.FutureProg.GetVariable(parameterDefinitions[index].Type,
					parameterValues.ElementAtOrDefault(index));
		}

		if (program.ReturnType != ProgVariableTypes.Void)
		{
			rootLocals["return"] = new MudSharp.FutureProg.Variables.NullVariable(program.ReturnType);
		}

		frames.Add(new BlockFrame
		{
			OwnerPath = RootPath,
			LocalVariables = rootLocals
		});
		return frames;
	}

	private static StackVariableSpace BuildVariableSpace(IReadOnlyList<ExecutionFrame> frames)
	{
		var scopes = frames
			.OfType<BlockFrame>()
			.Reverse()
			.Select(x => x.LocalVariables)
			.ToList();
		return new StackVariableSpace(scopes);
	}

	private static string PersistFrames(IEnumerable<ExecutionFrame> frames)
	{
		var state = new PersistedProgramState
		{
			Frames = frames.Select(frame => frame switch
			{
				BlockFrame blockFrame => new PersistedExecutionFrame
				{
					Kind = blockFrame.Kind,
					OwnerPath = blockFrame.OwnerPath,
					BranchKey = blockFrame.BranchKey,
					NextIndex = blockFrame.NextIndex,
					LocalVariables = blockFrame.LocalVariables.ToDictionary(
						x => x.Key,
						x => SerializeVariable(x.Value),
						StringComparer.InvariantCultureIgnoreCase)
				},
				WhileFrame whileFrame => new PersistedExecutionFrame
				{
					Kind = whileFrame.Kind,
					OwnerPath = whileFrame.StatementPath,
					IterationCount = whileFrame.IterationCount
				},
				ForFrame forFrame => new PersistedExecutionFrame
				{
					Kind = forFrame.Kind,
					OwnerPath = forFrame.StatementPath,
					CurrentIndex = forFrame.CurrentIndex,
					TotalIterations = forFrame.TotalIterations
				},
				ForEachFrame forEachFrame => new PersistedExecutionFrame
				{
					Kind = forEachFrame.Kind,
					OwnerPath = forEachFrame.StatementPath,
					CurrentIndex = forEachFrame.CurrentIndex,
					CollectionItems = forEachFrame.CollectionItems?.Select(SerializeVariable).ToList()
				},
				_ => throw new InvalidOperationException("Unknown computer program frame type.")
			}).ToList()
		};

		return JsonSerializer.Serialize(state);
	}

	private static List<ExecutionFrame> RestoreFrames(string stateJson, IFuturemud gameworld)
	{
		var state = JsonSerializer.Deserialize<PersistedProgramState>(stateJson) ?? new PersistedProgramState();
		return state.Frames
			.Select(frame => (ExecutionFrame)(frame.Kind switch
			{
				"block" => new BlockFrame
				{
					OwnerPath = frame.OwnerPath,
					BranchKey = frame.BranchKey,
					NextIndex = frame.NextIndex,
					LocalVariables = DeserializeVariables(frame.LocalVariables, gameworld)
				},
				"switch" => new BlockFrame
				{
					OwnerPath = frame.OwnerPath,
					BranchKey = frame.BranchKey,
					NextIndex = frame.NextIndex,
					LocalVariables = DeserializeVariables(frame.LocalVariables, gameworld),
					IsSwitchBlock = true
				},
				"while" => new WhileFrame
				{
					StatementPath = frame.OwnerPath,
					IterationCount = frame.IterationCount
				},
				"for" => new ForFrame
				{
					StatementPath = frame.OwnerPath,
					CurrentIndex = frame.CurrentIndex,
					TotalIterations = frame.TotalIterations
				},
				"foreach" => new ForEachFrame
				{
					StatementPath = frame.OwnerPath,
					CurrentIndex = frame.CurrentIndex,
					CollectionItems = frame.CollectionItems?.Select(x => DeserializeVariable(x, gameworld)).ToList()
				},
				_ => throw new InvalidOperationException($"Unknown persisted computer frame kind {frame.Kind}.")
			}))
			.ToList();
	}

	internal static string? SerializeValue(ProgVariableTypes type, object? value)
	{
		if (type == ProgVariableTypes.Void)
		{
			return null;
		}

		return JsonSerializer.Serialize(SerializeVariable(MudSharp.FutureProg.FutureProg.GetVariable(type, value)));
	}

	internal static object? DeserializeValue(ProgVariableTypes type, string? valueJson, IFuturemud gameworld)
	{
		if (type == ProgVariableTypes.Void || string.IsNullOrWhiteSpace(valueJson))
		{
			return null;
		}

		var persistedValue = JsonSerializer.Deserialize<PersistedProgValue>(valueJson);
		return persistedValue is null ? null : DeserializeVariable(persistedValue, gameworld).GetObject;
	}

	private static Dictionary<string, IProgVariable> DeserializeVariables(
		Dictionary<string, PersistedProgValue>? values,
		IFuturemud gameworld)
	{
		return values?.ToDictionary(x => x.Key, x => DeserializeVariable(x.Value, gameworld),
			StringComparer.InvariantCultureIgnoreCase)
		       ?? new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase);
	}

	private static PersistedProgValue SerializeVariable(IProgVariable variable)
	{
		var type = variable.Type.WithoutLiteral();
		if (variable.GetObject is null)
		{
			return new PersistedProgValue
			{
				Type = type.ToStorageString(),
				IsNull = true
			};
		}

		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			return new PersistedProgValue
			{
				Type = type.ToStorageString(),
				Items = ((IList)variable.GetObject).OfType<IProgVariable>().Select(SerializeVariable).ToList()
			};
		}

		if (type.HasFlag(ProgVariableTypes.Dictionary))
		{
			return new PersistedProgValue
			{
				Type = type.ToStorageString(),
				Map = ((Dictionary<string, IProgVariable>)variable.GetObject)
					.ToDictionary(x => x.Key, x => SerializeVariable(x.Value), StringComparer.InvariantCultureIgnoreCase)
			};
		}

		if (type.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			return new PersistedProgValue
			{
				Type = type.ToStorageString(),
				MultiMap = ((CollectionDictionary<string, IProgVariable>)variable.GetObject)
					.ToDictionary(
						x => x.Key,
						x => x.Value.Select(SerializeVariable).ToList(),
						StringComparer.InvariantCultureIgnoreCase)
			};
		}

		return new PersistedProgValue
		{
			Type = type.ToStorageString(),
			Scalar = type.LegacyCode switch
			{
				ProgVariableTypeCode.Boolean => ((bool)variable.GetObject).ToString(CultureInfo.InvariantCulture),
				ProgVariableTypeCode.Number => ((decimal)variable.GetObject).ToString(CultureInfo.InvariantCulture),
				ProgVariableTypeCode.TimeSpan => ((TimeSpan)variable.GetObject).Ticks.ToString(CultureInfo.InvariantCulture),
				ProgVariableTypeCode.MudDateTime => ((MudDateTime)variable.GetObject).GetDateTimeString(),
				_ => variable.GetObject.ToString()
			}
		};
	}

	private static IProgVariable DeserializeVariable(PersistedProgValue value, IFuturemud gameworld)
	{
		var type = ProgVariableTypes.FromStorageString(value.Type);
		if (value.IsNull)
		{
			return new MudSharp.FutureProg.Variables.NullVariable(type);
		}

		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			var innerType = type ^ ProgVariableTypes.Collection;
			return new MudSharp.FutureProg.Variables.CollectionVariable(
				value.Items?.Select(x => DeserializeVariable(x, gameworld)).ToList() ?? new List<IProgVariable>(),
				innerType);
		}

		if (type.HasFlag(ProgVariableTypes.Dictionary))
		{
			var innerType = type ^ ProgVariableTypes.Dictionary;
			return new MudSharp.FutureProg.Variables.DictionaryVariable(
				value.Map?.ToDictionary(x => x.Key, x => DeserializeVariable(x.Value, gameworld),
					StringComparer.InvariantCultureIgnoreCase) ??
				new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase),
				innerType);
		}

		if (type.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			var innerType = type ^ ProgVariableTypes.CollectionDictionary;
			var dictionary = new CollectionDictionary<string, IProgVariable>();
			if (value.MultiMap is not null)
			{
				foreach (var item in value.MultiMap)
				{
					dictionary.AddRange(item.Value.Select(x => (Key: item.Key, Value: DeserializeVariable(x, gameworld))));
				}
			}

			return new MudSharp.FutureProg.Variables.CollectionDictionaryVariable(dictionary, innerType);
		}

		object? runtimeValue = type.LegacyCode switch
		{
			ProgVariableTypeCode.Boolean => bool.Parse(value.Scalar ?? "false"),
			ProgVariableTypeCode.Number => decimal.Parse(value.Scalar ?? "0", CultureInfo.InvariantCulture),
			ProgVariableTypeCode.TimeSpan => TimeSpan.FromTicks(long.Parse(value.Scalar ?? "0", CultureInfo.InvariantCulture)),
			ProgVariableTypeCode.MudDateTime => MudDateTime.FromStoredStringOrFallback(value.Scalar ?? "Never", gameworld,
				StoredMudDateTimeFallback.Never, "ComputerProgramVariable", null, null, "MudDateTime"),
			_ => value.Scalar ?? string.Empty
		};

		return MudSharp.FutureProg.FutureProg.GetVariable(type, runtimeValue);
	}
}
