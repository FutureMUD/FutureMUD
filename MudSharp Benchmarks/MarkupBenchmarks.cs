using BenchmarkDotNet.Attributes;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class CharacteristicMarkupBenchmarks
{
	private MarkupBenchmarkOwner _owner = null!;
	private string _text = string.Empty;

	[Params("Simple", "Forms", "Articles", "Full")]
	public string Scenario { get; set; } = "Simple";

	[GlobalSetup]
	public void Setup()
	{
		_owner = new MarkupBenchmarkOwner();
		_text = Scenario switch
		{
			"Simple" => "$haircolour hair and $eyecolour eyes",
			"Forms" => "$haircolourbasic hair, $eyecolourfancy eyes, $personbasic",
			"Articles" => "&?a_an[$haircolourbasic-haired $person] with &?a_an[$eyecolour eyes]",
			"Full" => "&a_an[$haircolourbasic-haired $person] stands here. &he has $eyecolourfancy eyes. " +
			          "$?haircolour[&his hair is $haircolourfancy][&he has no hair]. " +
			          "&pronoun|They seem|He seems& watchful.",
			_ => throw new InvalidOperationException(Scenario)
		};
		// Warm regexes and assert identical live/preview content before timing.
		if (Live() != Preview()) throw new InvalidOperationException("Live/preview fixture output differs.");
	}

	[Benchmark]
	public string Live() => _owner.ParseCharacteristics(_text, _owner);

	[Benchmark]
	public string Preview() => IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(
		_text, _owner.Characteristics, Gendering.Get(Gender.Male), null!);
}

[MemoryDiagnoser]
public class EmoteMarkupBenchmarks
{
	private MarkupBenchmarkOwner _source = null!;
	private DummyPerceiver _observer = null!;
	private DummyPerceiver[] _observers = null!;
	private DummyPerceivable _target = null!;
	private Emote _ordinary = null!;
	private Emote _optional = null!;
	private EmoteOutput _assigned = null!;
	private const string OrdinaryText = "@ place|places $0 beside $1 and nod|nods.";
	private const string OptionalText = "@ place|places $0 $?1|beside $1|on the floor|$.";

	[GlobalSetup]
	public void Setup()
	{
		_source = new MarkupBenchmarkOwner();
		_observers = Enumerable.Range(0, 20).Select(_ => new DummyPerceiver("an observer")).ToArray();
		_observer = _observers[0];
		_target = new DummyPerceivable("a brass lantern");
		_ordinary = new Emote(OrdinaryText, _source, _target, _target);
		_optional = new Emote(OptionalText, _source, _target, _target);
		_assigned = new EmoteOutput(_ordinary);
		_assigned.AssignEmote("@ watches the brass lantern.", false, _observer);
		if (!_ordinary.Valid || !_optional.Valid || !_assigned.AllValid)
			throw new InvalidOperationException("Invalid benchmark emote.");
		_ = OrdinaryTwentyRecipients();
		_ = OptionalTwentyRecipients();
	}

	[Benchmark(OperationsPerInvoke = 20)]
	public int OrdinaryTwentyRecipients()
	{
		var length = 0;
		for (var i = 0; i < 20; i++) length += _ordinary.ParseFor(_observers[i]).Length;
		return length;
	}

	[Benchmark(OperationsPerInvoke = 20)]
	public int OptionalTwentyRecipients()
	{
		var length = 0;
		for (var i = 0; i < 20; i++) length += _optional.ParseFor(_observers[i]).Length;
		return length;
	}

	[Benchmark]
	public string ConstructOptionalAndRenderOnce() =>
		new Emote(OptionalText, _source, _target, _target).ParseFor(_observer);

	[Benchmark]
	public string AssignedOutput() => _assigned.ParseFor(_observer);
}

// Real parser inputs without Moq's per-call invocation recording in the measured paths.
internal sealed class MarkupBenchmarkOwner : DummyPerceiver, IPerceivableHaveCharacteristics
{
	public (ICharacteristicDefinition, ICharacteristicValue)[] Characteristics { get; }

	public MarkupBenchmarkOwner() : base("a watchful man")
	{
		Characteristics = new[]
		{
			Create(1, "haircolour", "black", "black", "raven-black"),
			Create(2, "eyecolour", "blue", "blue", "deep blue"),
			Create(3, "person", "man", "man", "adult man")
		};
	}

	private static (ICharacteristicDefinition, ICharacteristicValue) Create(long id, string variable,
		string normal, string basic, string fancy)
	{
		var definition = new CharacteristicDefinition(new MudSharp.Models.CharacteristicDefinition
		{
			Id = id, Name = variable, Pattern = $"^{variable}$", Description = variable,
			Type = (int)CharacteristicType.Standard
		}, null!);
		return (definition, new BenchmarkValue(definition, normal, basic, fancy));
	}

	public override Gendering ApparentGender(IPerceiver voyeur) => Gendering.Get(MudSharp.Form.Shape.Gender.Male);
	public override bool IsSelf(IPerceivable other) => ReferenceEquals(this, other);
	public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions => Characteristics.Select(x => x.Item1);
	public IEnumerable<ICharacteristicValue> RawCharacteristicValues => Characteristics.Select(x => x.Item2);
	public IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> RawCharacteristics => Characteristics;
	public ICharacteristicValue GetCharacteristic(string type, IPerceiver voyeur) => GetCharacteristic(GetCharacteristicDefinition(type).Item1, voyeur);
	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur) => Characteristics.First(x => x.Item1 == type).Item2;
	public void SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value) => throw new NotSupportedException();
	public string DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
		CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal)
	{
		var value = GetCharacteristic(definition, voyeur);
		return type switch
		{
			CharacteristicDescriptionType.Basic => value.GetBasicValue,
			CharacteristicDescriptionType.Fancy => value.GetFancyValue,
			_ => value.GetValue
		};
	}
	public string DescribeCharacteristic(string type, IPerceiver voyeur)
	{
		var definition = GetCharacteristicDefinition(type);
		return DescribeCharacteristic(definition.Item1, voyeur, definition.Item2);
	}
	public IObscureCharacteristics GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur) => null!;
	public Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetCharacteristicDefinition(string pattern)
	{
		var type = CharacteristicDescriptionType.Normal;
		if (pattern.EndsWith("basic", StringComparison.OrdinalIgnoreCase))
		{
			pattern = pattern[..^5];
			type = CharacteristicDescriptionType.Basic;
		}
		else if (pattern.EndsWith("fancy", StringComparison.OrdinalIgnoreCase))
		{
			pattern = pattern[..^5];
			type = CharacteristicDescriptionType.Fancy;
		}
		return Tuple.Create(CharacteristicDefinitions.FirstOrDefault(x => x.MatchesPattern(pattern))!, type);
	}
	public void ExpireDefinition(ICharacteristicDefinition definition) => throw new NotSupportedException();
	public void RecalculateCharacteristicsDueToExternalChange() => throw new NotSupportedException();

	private sealed class BenchmarkValue(ICharacteristicDefinition definition, string normal, string basic, string fancy)
		: CharacteristicValue(normal, definition, basic, fancy, nonsaving: true)
	{
		public override string GetBasicValue => basic;
		public override string GetFancyValue => fancy;
	}
}
