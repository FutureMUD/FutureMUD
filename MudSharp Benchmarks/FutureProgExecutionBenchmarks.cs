#nullable enable

using BenchmarkDotNet.Attributes;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using System.Collections;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class FutureProgExecutionBenchmarks
{
	private IFuturemud _gameworld = null!;
	private FutureProg _zeroParameter = null!;
	private FutureProg _oneParameter = null!;
	private FutureProg _multiParameter = null!;
	private FutureProg _logicAndBranch = null!;
	private FutureProg _dotReferences = null!;
	private FutureProg _foreachLoop = null!;
	private FutureProg _collectionAny = null!;
	private FutureProg _collectionWhereSelect = null!;
	private FutureProg _collectionIndex = null!;
	private FutureProg _collectionReturn = null!;
	private FutureProg _dictionaryReturn = null!;
	private FutureProg _fullyStatic = null!;
	private FutureProgInvokerFunction _nestedInvoker = null!;
	private VariableSpace _nestedVariables = null!;
	private List<decimal> _numbers = null!;
	private Dictionary<string, string> _dictionary = null!;

	[GlobalSetup]
	public void Setup()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld
			.Setup(x => x.GetStaticBool(It.IsAny<string>()))
			.Returns(false);
		_gameworld = gameworld.Object;

		FutureProg.Initialise();
		_zeroParameter = Compile("BenchmarkZero", ProgVariableTypes.Number, [], "return 42");
		_oneParameter = Compile("BenchmarkOne", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "value")], "return @value + 1");
		_multiParameter = Compile("BenchmarkMulti", ProgVariableTypes.Number,
			[
				Tuple.Create(ProgVariableTypes.Number, "a"),
				Tuple.Create(ProgVariableTypes.Number, "b"),
				Tuple.Create(ProgVariableTypes.Number, "c")
			], "return (@a + @b) * @c");
		_logicAndBranch = Compile("BenchmarkLogic", ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.Number, "a"),
				Tuple.Create(ProgVariableTypes.Number, "b"),
				Tuple.Create(ProgVariableTypes.Boolean, "flag")
			],
			"""
			if ((@a > 1 and @b < 10) or @flag)
				return true
			end if
			return false
			""");
		_dotReferences = Compile("BenchmarkDots", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Text, "value")],
			"return @value.upper.length + @value.lower.length");
		_foreachLoop = Compile("BenchmarkForeach", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"""
			var total as number
			foreach (value in @values)
				total += @value
			end foreach
			return @total
			""");
		_collectionAny = Compile("BenchmarkAny", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"return @values.any(value, @value > 50)");
		_collectionWhereSelect = Compile("BenchmarkWhereSelect",
			ProgVariableTypes.Number | ProgVariableTypes.Collection,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"return @values.where(value, @value > 25).select(value, @value * 2)");
		_collectionIndex = Compile("BenchmarkIndex", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"return @values[50]");
		_collectionReturn = Compile("BenchmarkCollectionReturn",
			ProgVariableTypes.Number | ProgVariableTypes.Collection,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"return @values");
		_dictionaryReturn = Compile("BenchmarkDictionaryReturn",
			ProgVariableTypes.Text | ProgVariableTypes.Dictionary,
			[Tuple.Create(ProgVariableTypes.Text | ProgVariableTypes.Dictionary, "values")],
			"return @values");
		_fullyStatic = Compile("BenchmarkStatic", ProgVariableTypes.Number, [], "return 42");
		_fullyStatic.StaticType = FutureProgStaticType.FullyStatic;
		_fullyStatic.ExecuteDecimal();

		var nestedTarget = Compile("BenchmarkNestedTarget", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "value")], "return @value + 1");
		_nestedInvoker = new FutureProgInvokerFunction(nestedTarget,
			[new VariableReferenceFunction("value", ProgVariableTypes.Number)]);
		_nestedVariables = new VariableSpace(new Dictionary<string, IProgVariable>
		{
			["value"] = new NumberVariable(41)
		});

		_numbers = Enumerable.Range(0, 100).Select(x => (decimal)x).ToList();
		_dictionary = Enumerable.Range(0, 25).ToDictionary(x => $"key{x}", x => $"value{x}");
	}

	[Benchmark(Baseline = true)]
	public decimal ZeroParameterInvocation() => _zeroParameter.ExecuteDecimal();

	[Benchmark]
	public decimal OneParameterInvocation() => _oneParameter.ExecuteDecimal((object)41M);

	[Benchmark]
	public decimal MultiParameterInvocation() => _multiParameter.ExecuteDecimal(new object[] { 4M, 6M, 5M });

	[Benchmark]
	public bool LogicAndBranch() => _logicAndBranch.ExecuteBool(4M, 6M, false);

	[Benchmark]
	public decimal DotReferenceChain() => _dotReferences.ExecuteDecimal("FutureMUD");

	[Benchmark]
	public decimal ForeachLoop() => _foreachLoop.ExecuteDecimal(_numbers);

	[Benchmark]
	public bool CollectionAny() => _collectionAny.ExecuteBool(_numbers);

	[Benchmark]
	public int CollectionWhereSelect() => _collectionWhereSelect.ExecuteCollection<decimal>(_numbers).Count();

	[Benchmark]
	public decimal CollectionIndex() => _collectionIndex.ExecuteDecimal(_numbers);

	[Benchmark]
	public int CollectionReturnTranslation() => _collectionReturn.ExecuteCollection<decimal>(_numbers).Count();

	[Benchmark]
	public int DictionaryReturnTranslation() => _dictionaryReturn.ExecuteDictionary<string>(_dictionary).Count;

	[Benchmark]
	public decimal FullyStaticCacheHit() => _fullyStatic.ExecuteDecimal();

	[Benchmark]
	public object NestedProgInvocation()
	{
		_nestedInvoker.Execute(_nestedVariables);
		return _nestedInvoker.Result.GetObject;
	}

	[Benchmark]
	public bool TypeHasFlag() =>
		(ProgVariableTypes.Character | ProgVariableTypes.Collection).HasFlag(ProgVariableTypes.Collection);

	[Benchmark]
	public bool TypeCompatibility() =>
		(ProgVariableTypes.Character | ProgVariableTypes.Collection)
		.CompatibleWith(ProgVariableTypes.Toon | ProgVariableTypes.Collection);

	[Benchmark]
	public ProgVariableTypeCode TypeDispatch() => ProgVariableTypes.Number.LegacyCode;

	[Benchmark]
	public bool CompileSimple()
	{
		var prog = new FutureProg(_gameworld, "CompileSimple", ProgVariableTypes.Number, [], "return 42");
		return prog.Compile();
	}

	[Benchmark]
	public bool CompileRepresentative()
	{
		var prog = new FutureProg(_gameworld, "CompileRepresentative", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"""
			var total as number
			foreach (value in @values.where(candidate, @candidate > 25))
				if (@value < 75)
					total += @value
				end if
			end foreach
			return @total
			""");
		return prog.Compile();
	}

	private FutureProg Compile(string name, ProgVariableTypes returnType,
		IEnumerable<Tuple<ProgVariableTypes, string>> parameters, string text)
	{
		var prog = new FutureProg(_gameworld, name, returnType, parameters, text);
		if (!prog.Compile())
		{
			throw new InvalidOperationException($"Benchmark prog {name} failed to compile: {prog.CompileError}");
		}

		return prog;
	}
}
