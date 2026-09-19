#nullable enable

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using ExpressionEngine;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Magic.Environment;
using MagicGenerator = MudSharp.Models.MagicGenerator;

namespace MudSharp.Magic.Generators;

/// <summary>A persisted profile. Its cell runtime belongs exclusively to the world coordinator.</summary>
public sealed partial class EnvironmentalMagicGenerator : BaseMagicResourceGenerator, IEnvironmentalMagicProfile
{
	public const int CurrentDefinitionVersion = 1;
	public const int MaximumOutputs = 8;
	public const int MaximumInputs = 32;
	public const int MaximumFormulaLength = 4096;
	private static readonly FrozenSet<string> BuiltInInputs = new[]
	{
		"balance", "maximum", "basecapacity", "baserate", "scardamage", "pressure", "hasdefile",
		"minutessincedefile"
	}.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	public static IReadOnlySet<string> AgricultureSources { get; } = new[]
	{
		"hasfield", "hascrop", "haswoodland", "crophealth", "cropyieldpotential", "woodlandhealth",
		"woodlandyieldpotential", "pasture", "fieldcondition"
	}.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	private readonly List<EnvironmentalMagicOutput> _outputs = new();
	private readonly List<EnvironmentalMagicInput> _inputs = new();
	private readonly Dictionary<long, CompiledOutput> _compiledOutputs = new();
	private readonly Dictionary<(string Text, bool Rate), (IExpression? Expression, string? Error)> _formulaCache = new();
	private readonly List<string> _validationErrors = new();
	private readonly ReadOnlyCollection<EnvironmentalMagicOutput> _outputView;
	private readonly ReadOnlyCollection<EnvironmentalMagicInput> _inputView;
	private readonly ReadOnlyCollection<string> _errorView;
	private IMagicResource[] _generatedResources = Array.Empty<IMagicResource>();
	private int _definitionVersion = CurrentDefinitionVersion;
	private string? _loadError;
	private DateTimeOffset _changeUtc;
	private sealed record CompiledOutput(EnvironmentalMagicOutput Definition, IExpression Maximum,
		IExpression Rate, string[] MaximumParameters, string[] RateParameters);

	public long Revision { get; private set; } = 1;
	/// <summary>Runtime diagnostics only; counts actual maximum/rate expression evaluations.</summary>
	public long FormulaEvaluationCount { get; private set; }
	public double PressureHalfLifeSeconds { get; private set; } = 3600.0;
	public double NaturalRepairPerMinute { get; private set; }
	public double? IdleRecheckSeconds { get; private set; }
	public DateTimeOffset DecayReferenceUtc { get; private set; } = DateTimeOffset.UnixEpoch;
	public double DecayIntegral { get; private set; }
	public IReadOnlyList<EnvironmentalMagicOutput> Outputs => _outputView;
	public IReadOnlyList<EnvironmentalMagicInput> Inputs => _inputView;
	public IReadOnlyList<string> ValidationErrors => _errorView;
	public IReadOnlySet<string> RequiredInputNames { get; private set; } =
		Array.Empty<string>().ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	public override IEnumerable<IMagicResource> GeneratedResources => _generatedResources;
	public override string RegeneratorTypeName => "Environmental";

	public EnvironmentalMagicGenerator(IFuturemud gameworld, string name, IMagicResource resource)
		: base(gameworld, name)
	{
		_outputView = _outputs.AsReadOnly();
		_inputView = _inputs.AsReadOnly();
		_errorView = _validationErrors.AsReadOnly();
		_organicSourceView = _organicSources.AsReadOnly();
		_organicPenaltyView = _organicPenalties.AsReadOnly();
		_organicErrorView = _organicValidationErrors.AsReadOnly();
		_outputs.Add(new EnvironmentalMagicOutput(resource.Id, resource, "basecapacity", "baserate", 100.0, 1.0));
		DecayReferenceUtc = gameworld.EnvironmentalMagic?.UtcNow ?? DateTimeOffset.UtcNow;
		RebuildDefinition();
		Insert();
	}

	public EnvironmentalMagicGenerator(MagicGenerator generator, IFuturemud gameworld) : base(generator, gameworld)
	{
		_outputView = _outputs.AsReadOnly();
		_inputView = _inputs.AsReadOnly();
		_errorView = _validationErrors.AsReadOnly();
		_organicSourceView = _organicSources.AsReadOnly();
		_organicPenaltyView = _organicPenalties.AsReadOnly();
		_organicErrorView = _organicValidationErrors.AsReadOnly();
		LoadDefinition(generator.Definition);
		RebuildDefinition();
	}

	private EnvironmentalMagicGenerator(EnvironmentalMagicGenerator original, string name) : base(original.Gameworld, name)
	{
		_outputView = _outputs.AsReadOnly();
		_inputView = _inputs.AsReadOnly();
		_errorView = _validationErrors.AsReadOnly();
		_organicSourceView = _organicSources.AsReadOnly();
		_organicPenaltyView = _organicPenalties.AsReadOnly();
		_organicErrorView = _organicValidationErrors.AsReadOnly();
		LoadDefinition(original.SaveDefinition().ToString());
		DecayReferenceUtc = Gameworld.EnvironmentalMagic?.UtcNow ?? DateTimeOffset.UtcNow;
		DecayIntegral = 0.0;
		RebuildDefinition();
		Insert();
	}

	private void Insert()
	{
		using (new FMDB())
		{
			var model = new MagicGenerator
			{
				Name = Name,
				Type = "environmental",
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicGenerators.Add(model);
			FMDB.Context.SaveChanges();
			_id = model.Id;
		}
	}

	public override IMagicResourceRegenerator Clone(string name) => new EnvironmentalMagicGenerator(this, name);

	/// <summary>Pure definition export for durable operations that anchor to this pressure timeline.</summary>
	public string ExportDefinition() => SaveDefinition().ToString();

	public double PressureDecayIntegralAt(DateTimeOffset utc) =>
		DecayIntegral + (utc - DecayReferenceUtc).TotalSeconds / PressureHalfLifeSeconds;

	protected override void ValidateMinuteDelegateHolder(IHaveMagicResource thing)
	{
		throw new InvalidOperationException(thing is ICell
			? $"Environmental regenerator #{Id} ({Name}) is centrally coordinated and cannot register a per-cell minute delegate."
			: $"Environmental regenerator #{Id} ({Name}) supports physical cells only; character and item holders are unsupported.");
	}

	protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing) =>
		throw new InvalidOperationException("Environmental generators have no per-holder heartbeat delegate.");

	protected override void BeforeDefinitionChange()
	{
		_changeUtc = Gameworld.EnvironmentalMagic?.UtcNow ?? DateTimeOffset.UtcNow;
		Gameworld.EnvironmentalMagic?.BeforeProfileChange(this);
	}

	protected override void DefinitionChanged()
	{
		Revision++;
		_formulaCache.Clear();
		RefreshReferences();
		base.DefinitionChanged();
		Gameworld.EnvironmentalMagic?.ProfileChanged(this);
	}

	public void RefreshReferences()
	{
		RebuildDefinition();
	}

	private void ApplyDefinitionChange(Action mutation)
	{
		BeforeDefinitionChange();
		mutation();
		DefinitionChanged();
	}

	private void LoadDefinition(string definition)
	{
		XElement root;
		try
		{
			root = XElement.Parse(definition);
		}
		catch (XmlException exception)
		{
			_definitionVersion = 0;
			_loadError = $"The environmental XML could not be read: {exception.Message}";
			return;
		}

		_definitionVersion = int.TryParse(root.Attribute("version")?.Value, out var version) ? version : 0;
		if (root.Name != "Definition")
		{
			_definitionVersion = 0;
			_loadError = "The environmental XML requires a Definition root.";
		}

		PressureHalfLifeSeconds = ReadNumber(root.Element("PressureHalfLifeSeconds")?.Value, 3600.0);
		NaturalRepairPerMinute = ReadNumber(root.Element("NaturalRepairPerMinute")?.Value, 0.0);
		var idle = root.Element("IdleRecheckSeconds")?.Value;
		IdleRecheckSeconds = string.IsNullOrWhiteSpace(idle) ? null : ReadNumber(idle, double.NaN);
		DecayIntegral = ReadNumber(root.Element("DecayIntegral")?.Value, 0.0);
		var decayReference = root.Element("DecayReferenceUtc")?.Value;
		if (decayReference is not null)
		{
			if (DateTimeOffset.TryParse(decayReference, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal, out var reference))
			{
				DecayReferenceUtc = reference.ToUniversalTime();
			}
			else
			{
				_loadError = "The pressure-decay reference timestamp is invalid.";
				DecayIntegral = double.NaN;
			}
		}

		foreach (var element in root.Element("Outputs")?.Elements("Output") ?? Enumerable.Empty<XElement>())
		{
			var resourceId = long.TryParse(element.Attribute("resource")?.Value, out var id) ? id : 0;
			_outputs.Add(new EnvironmentalMagicOutput(resourceId, null,
				element.Element("Maximum")?.Value ?? string.Empty, element.Element("Rate")?.Value ?? string.Empty,
				ReadNumber(element.Attribute("basecapacity")?.Value, 100.0),
				ReadNumber(element.Attribute("baserate")?.Value, 1.0)));
		}

		foreach (var element in root.Element("Inputs")?.Elements("Input") ?? Enumerable.Empty<XElement>())
		{
			var kind = Enum.TryParse<EnvironmentalMagicInputKind>(element.Attribute("kind")?.Value, true,
				out var parsedKind) ? parsedKind : (EnvironmentalMagicInputKind)(-1);
			var source = element.Attribute("source")?.Value ?? string.Empty;
			long? progId = null;
			if (kind == EnvironmentalMagicInputKind.Prog &&
				long.TryParse(element.Attribute("prog")?.Value ?? source, out var parsedProg))
			{
				progId = parsedProg;
			}

			_inputs.Add(new EnvironmentalMagicInput(element.Attribute("name")?.Value ?? string.Empty, kind,
				source, ReadNumber(element.Attribute("scale")?.Value, 1.0), progId));
		}

		LoadOrganicDefinition(root.Element("Organic"));
	}

	private static double ReadNumber(string? value, double missingDefault) => value is null
		? missingDefault
		: double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ? number : double.NaN;

	protected override XElement SaveDefinition() => new("Definition",
		new XAttribute("version", _definitionVersion),
		new XElement("PressureHalfLifeSeconds", PressureHalfLifeSeconds),
		new XElement("NaturalRepairPerMinute", NaturalRepairPerMinute),
		IdleRecheckSeconds.HasValue ? new XElement("IdleRecheckSeconds", IdleRecheckSeconds.Value) : null,
		new XElement("DecayReferenceUtc", DecayReferenceUtc.ToString("O", CultureInfo.InvariantCulture)),
		new XElement("DecayIntegral", DecayIntegral),
		new XElement("Outputs", _outputs.Select(output => new XElement("Output",
			new XAttribute("resource", output.ResourceId), new XAttribute("basecapacity", output.BaseCapacity),
			new XAttribute("baserate", output.BaseRate), new XElement("Maximum", output.MaximumFormula),
			new XElement("Rate", output.RateFormula)))),
		new XElement("Inputs", _inputs.Select(input => new XElement("Input", new XAttribute("name", input.Name),
			new XAttribute("kind", input.Kind), new XAttribute("source", input.Source),
			new XAttribute("scale", input.Scale), input.ProgId.HasValue ? new XAttribute("prog", input.ProgId.Value) : null))),
		SaveOrganicDefinition());

	private void RebuildDefinition()
	{
		_validationErrors.Clear();
		_compiledOutputs.Clear();
		if (_loadError is not null)
		{
			_validationErrors.Add(_loadError);
		}
		if (_definitionVersion != CurrentDefinitionVersion)
		{
			_validationErrors.Add($"Unsupported environmental definition version {_definitionVersion}; expected {CurrentDefinitionVersion}.");
		}
		if (!double.IsFinite(PressureHalfLifeSeconds) || PressureHalfLifeSeconds < 1.0)
		{
			_validationErrors.Add("Pressure half-life must be a finite number of at least one second.");
		}
		if (!double.IsFinite(DecayIntegral))
		{
			_validationErrors.Add("The pressure-decay integral must be finite.");
		}
		if (!double.IsFinite(NaturalRepairPerMinute) || NaturalRepairPerMinute < 0.0)
		{
			_validationErrors.Add("Natural scar repair must be a finite non-negative amount per minute.");
		}
		if (IdleRecheckSeconds.HasValue && (!double.IsFinite(IdleRecheckSeconds.Value) ||
			IdleRecheckSeconds.Value < 1.0 || IdleRecheckSeconds.Value > 3600.0))
		{
			_validationErrors.Add("An optional idle recheck interval must be from 1 to 3600 seconds.");
		}
		if (_outputs.Count is < 1 or > MaximumOutputs)
		{
			_validationErrors.Add($"A profile requires from 1 to {MaximumOutputs} resource outputs.");
		}
		if (_inputs.Count > MaximumInputs)
		{
			_validationErrors.Add($"A profile permits at most {MaximumInputs} named inputs.");
		}

		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		for (var i = 0; i < _inputs.Count; i++)
		{
			var input = _inputs[i];
			if (input.Kind == EnvironmentalMagicInputKind.Prog)
			{
				input = input with { Prog = input.ProgId.HasValue ? Gameworld.FutureProgs.Get(input.ProgId.Value) : null };
				_inputs[i] = input;
			}
			if (!names.Add(input.Name))
			{
				_validationErrors.Add($"Input {input.Name} is defined more than once (names are case-insensitive).");
			}
			var inputError = ValidateInput(input);
			if (inputError is not null)
			{
				_validationErrors.Add(inputError);
			}
		}
		RebuildOrganicDefinition(names);

		var required = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var resources = new HashSet<long>();
		for (var i = 0; i < _outputs.Count; i++)
		{
			var output = _outputs[i] with { Resource = Gameworld.MagicResources.Get(_outputs[i].ResourceId) };
			_outputs[i] = output;
			if (!resources.Add(output.ResourceId))
			{
				_validationErrors.Add($"Resource #{output.ResourceId} has duplicate outputs.");
			}
			if (output.Resource is null)
			{
				_validationErrors.Add($"Output resource #{output.ResourceId} does not exist.");
			}
			else if (!output.Resource.ResourceType.HasFlag(MagicResourceType.LocationResource))
			{
				_validationErrors.Add($"Output resource #{output.ResourceId} ({output.Resource.Name}) does not support location holders.");
			}
			if (!double.IsFinite(output.BaseCapacity) || output.BaseCapacity < 0.0 ||
				!double.IsFinite(output.BaseRate) || output.BaseRate < 0.0)
			{
				_validationErrors.Add($"Output resource #{output.ResourceId} requires finite, non-negative baseline capacity and rate.");
			}

			var maximum = CompileDefinitionFormula(output.MaximumFormula, false, names, out var maximumError);
			var rate = CompileDefinitionFormula(output.RateFormula, true, names, out var rateError);
			if (maximumError is not null)
			{
				_validationErrors.Add($"Resource #{output.ResourceId} maximum: {maximumError}");
			}
			if (rateError is not null)
			{
				_validationErrors.Add($"Resource #{output.ResourceId} rate: {rateError}");
			}
			if (maximum is null || rate is null)
			{
				continue;
			}

			var maximumParameters = maximum.ParameterNames.ToArray();
			var rateParameters = rate.ParameterNames.ToArray();
			required.UnionWith(maximumParameters);
			required.UnionWith(rateParameters);
			_compiledOutputs.TryAdd(output.ResourceId,
				new CompiledOutput(output, maximum, rate, maximumParameters, rateParameters));
		}

		RequiredInputNames = required.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
		_generatedResources = _outputs
			.Where(output => output.Resource is not null)
			.Select(output => output.Resource!)
			.Distinct()
			.ToArray();
	}

	private static bool ValidInputName(string name) => name.Length is > 0 and <= 64 &&
		char.IsAsciiLetter(name[0]) && name.All(character => char.IsAsciiLetterOrDigit(character) || character == '_');

	private static string? ValidateInput(EnvironmentalMagicInput input)
	{
		if (!ValidInputName(input.Name) || BuiltInInputs.Contains(input.Name))
		{
			return $"Input {input.Name} must be a unique non-built-in name of up to 64 ASCII letters, numbers or underscores, starting with a letter.";
		}
		if (!double.IsFinite(input.Scale))
		{
			return $"Input {input.Name} requires a finite explicit scale.";
		}
		if (string.IsNullOrWhiteSpace(input.Source))
		{
			return $"Input {input.Name} requires a source.";
		}

		switch (input.Kind)
		{
			case EnvironmentalMagicInputKind.Forage:
				return null;
			case EnvironmentalMagicInputKind.Agriculture:
				return AgricultureSources.Contains(input.Source)
					? null
					: $"Input {input.Name} specifies an unknown agriculture source: {input.Source}.";
			case EnvironmentalMagicInputKind.Prog:
				if (input.Prog is null)
				{
					return $"Input {input.Name} refers to missing prog #{input.ProgId?.ToString() ?? input.Source}.";
				}
				if (!input.Prog.ReturnType.CompatibleWith(ProgVariableTypes.Number) ||
					input.Prog.AcceptsAnyParameters || !input.Prog.MatchesParameters(new[] { ProgVariableTypes.Location }))
				{
					return $"Input {input.Name} requires a numeric prog accepting one location argument.";
				}
				if (input.Prog.StaticType != FutureProgStaticType.NotStatic)
				{
					return $"Input {input.Name} requires a NotStatic prog; parameter-only and fully static caches cannot track changing room state.";
				}
				return string.IsNullOrWhiteSpace(input.Prog.CompileError)
					? null
					: $"Input {input.Name} prog #{input.Prog.Id} has a compile error: {input.Prog.CompileError}";
			default:
				return $"Input {input.Name} has an unsupported input kind.";
		}
	}

	private IExpression? CompileDefinitionFormula(string text, bool rate, ISet<string> names, out string? error)
	{
		if (!_formulaCache.TryGetValue((text, rate), out var compiled))
		{
			var expression = CompileFormula(text, rate, names, out var compilationError);
			compiled = (expression, compilationError);
			_formulaCache[(text, rate)] = compiled;
		}
		error = compiled.Error;
		return compiled.Expression;
	}

	private static IExpression? CompileFormula(string text, bool rate, ISet<string> names, out string? error)
	{
		error = null;
		if (string.IsNullOrWhiteSpace(text) || text.Length > MaximumFormulaLength)
		{
			error = $"Specify a formula of 1 to {MaximumFormulaLength} characters.";
			return null;
		}
		var expression = new Expression(text);
		if (expression.HasErrors())
		{
			error = expression.Error;
			return null;
		}
		foreach (var parameter in expression.ParameterNames)
		{
			if (!rate && parameter.EqualToAny("balance", "maximum"))
			{
				error = $"{parameter} is rate-only; a maximum cannot depend on its own balance or maximum.";
				return null;
			}
			if (!BuiltInInputs.Contains(parameter) && !names.Contains(parameter))
			{
				error = $"Input {parameter} has not been declared.";
				return null;
			}
		}
		foreach (var function in expression.FunctionNames)
		{
			if (!Expression.IsSupportedFunction(function))
			{
				error = $"Unknown function {function}.";
				return null;
			}
			if (function.EqualToAny("rand", "drand", "dice"))
			{
				error = $"Random function {function} cannot define a repeatable environmental cap or rate.";
				return null;
			}
		}
		if (!expression.ParameterNames.Any())
		{
			if (!expression.TryEvaluateDoubleWith(new Dictionary<string, object>(), out var constant, out var constantError))
			{
				error = constantError;
				return null;
			}
			if (constant < 0.0)
			{
				error = "A constant maximum or rate cannot be negative.";
				return null;
			}
		}
		return expression;
	}

	public EnvironmentalMagicOutputEvaluation EvaluateOutput(EnvironmentalMagicOutput output,
		IReadOnlyDictionary<string, double> sharedInputs, double balance)
	{
		if (_validationErrors.Count > 0)
		{
			return EnvironmentalMagicOutputEvaluation.Invalid(_validationErrors[0]);
		}
		if (!_compiledOutputs.TryGetValue(output.ResourceId, out var compiled) || compiled.Definition != output)
		{
			return EnvironmentalMagicOutputEvaluation.Invalid("The output does not belong to the current profile revision.");
		}
		if (!double.IsFinite(balance) || balance < 0.0)
		{
			return EnvironmentalMagicOutputEvaluation.Invalid("The recorded balance must be a finite non-negative number.");
		}

		var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		if (!BindValues(compiled.MaximumParameters, output, sharedInputs, balance, 0.0, values, out var error))
		{
			return EnvironmentalMagicOutputEvaluation.Invalid(error!);
		}
		FormulaEvaluationCount++;
		if (!compiled.Maximum.TryEvaluateDoubleWith(values, out var maximum, out var maximumError) || maximum < 0.0)
		{
			return EnvironmentalMagicOutputEvaluation.Invalid(maximum < 0.0 ? "The maximum is negative." : maximumError);
		}
		values.Clear();
		if (!BindValues(compiled.RateParameters, output, sharedInputs, balance, maximum, values, out error))
		{
			return EnvironmentalMagicOutputEvaluation.Invalid(error!);
		}
		FormulaEvaluationCount++;
		if (!compiled.Rate.TryEvaluateDoubleWith(values, out var rate, out var rateError) || rate < 0.0)
		{
			return EnvironmentalMagicOutputEvaluation.Invalid(rate < 0.0 ? "The regeneration rate is negative." : rateError);
		}
		return new EnvironmentalMagicOutputEvaluation(true, maximum, rate, null);
	}

	private static bool BindValues(IEnumerable<string> parameters, EnvironmentalMagicOutput output,
		IReadOnlyDictionary<string, double> sharedInputs, double balance, double maximum,
		Dictionary<string, object> values, out string? error)
	{
		error = null;
		foreach (var parameter in parameters)
		{
			double value;
			switch (parameter.ToLowerInvariant())
			{
				case "basecapacity": value = output.BaseCapacity; break;
				case "baserate": value = output.BaseRate; break;
				case "balance": value = balance; break;
				case "maximum": value = maximum; break;
				default:
					if (!sharedInputs.TryGetValue(parameter, out value))
					{
						error = $"Input {parameter} is missing from the evaluation snapshot.";
						return false;
					}
					break;
			}
			if (!double.IsFinite(value))
			{
				error = $"Input {parameter} is not finite.";
				return false;
			}
			values[parameter] = value;
		}
		return true;
	}

	protected override string SubtypeHelpText => @"	#3output add <resource>#0 - add one output, initially capacity 100 and rate 1 per minute
	#3output remove <resource>#0 - remove an output without deleting room balances
	#3output <resource> maximum <formula>#0 - set its environmental maximum
	#3output <resource> rate <formula>#0 - set its regeneration per real minute
	#3output <resource> basecapacity <number>#0 - set its baseline capacity
	#3output <resource> baserate <number>#0 - set its baseline rate per real minute
	#3input <name> forage <yield key> <scale>#0 - bind a read-only forage yield
	#3input <name> agriculture <source> <scale>#0 - bind a read-only field value
	#3input <name> prog <prog> <scale>#0 - bind an uncached numeric location prog
	#3input remove <name>#0 - remove an unused input
	#3halflife <seconds>#0 - set recent-pressure half-life (at least 1 second)
	#3repair <amount>#0 - set natural scar repair per real minute (zero disables)
	#3idle <seconds|default>#0 - set an optional 1-3600 second idle recheck
	#3organic sources#0 - list explicitly authorised native organic sources
	#3organic source add forage <yield-key>#0 - authorise one exact forage yield key
	#3organic source add <crop|woodland|pasture>#0 - authorise one field source
	#3organic source <selector> uses <uses|any>#0 - restrict compatible field uses
	#3organic source <selector> definitions <ids|any>#0 - restrict vegetation definitions
	#3organic source remove <selector>#0 - remove a native source authorisation
	#3organic penalty <channel> <formula|none>#0 - set a dimensionless suppression factor from 0 to 1
	#3organic protection <prog|none>#0 - validate a later-use protection prog without invoking it

Built-ins: #6basecapacity#0, #6baserate#0, #6scardamage#0, #6pressure#0, #6hasdefile#0 and
#6minutessincedefile#0. Rate formulae may also use #6balance#0 and #6maximum#0.
Agriculture sources: #6hasfield#0, #6hascrop#0, #6haswoodland#0, #6crophealth#0,
#6cropyieldpotential#0, #6woodlandhealth#0, #6woodlandyieldpotential#0, #6pasture#0 and #6fieldcondition#0.
Absent field/crop/woodland inputs are zero; presence inputs distinguish absence.
Progs must be #6NotStatic#0, return a number, accept one location, and perform only pure reads.
Organic penalty channels: #6forage#0, #6crophealth#0, #6cropyield#0, #6woodlandhealth#0,
#6woodlandyield#0, #6pasture#0, #6cropinitial#0, #6woodlandinitial#0 and #6pastureinitial#0.
Penalty inputs: #6scardamage#0 and decayed #6pressure#0 are ecological amounts; #6hasdefile#0 is 0/1;
#6minutessincedefile#0 is real minutes; #6nativestock#0, #6nativehealth#0, #6nativeyield#0,
#6nativecapacity#0, #6fieldcondition#0 and #6baselineincrease#0 are raw owner values in that channel's
native units. Declared named inputs are also available. The result is a dimensionless factor from 0 to 1.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "output": return BuildingCommandOutput(actor, command);
			case "input": return BuildingCommandInput(actor, command);
			case "halflife": return BuildingCommandHalfLife(actor, command);
			case "repair": return BuildingCommandRepair(actor, command);
			case "idle": return BuildingCommandIdle(actor, command);
			case "organic": return BuildingCommandOrganic(actor, command);
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandHalfLife(ICharacter actor, StringStack command)
	{
		if (!TryBuilderNumber(actor, command.SafeRemainingArgument, 1.0, double.MaxValue, out var value))
		{
			actor.OutputHandler.Send("Use halflife <seconds>, with a finite value of at least 1 second.");
			return false;
		}
		ApplyDefinitionChange(() =>
		{
			var integral = PressureDecayIntegralAt(_changeUtc);
			DecayIntegral = double.IsFinite(integral) ? integral : 0.0;
			DecayReferenceUtc = _changeUtc;
			PressureHalfLifeSeconds = value;
		});
		actor.OutputHandler.Send($"Recent pressure now has a half-life of {value.ToString("N3", actor).ColourValue()} real seconds. Existing pressure retains its prior decay history.");
		return true;
	}

	private bool BuildingCommandRepair(ICharacter actor, StringStack command)
	{
		if (!TryBuilderNumber(actor, command.SafeRemainingArgument, 0.0, double.MaxValue, out var value))
		{
			actor.OutputHandler.Send("Use repair <amount per real minute>, with a finite non-negative amount.");
			return false;
		}
		ApplyDefinitionChange(() => NaturalRepairPerMinute = value);
		actor.OutputHandler.Send($"Natural scar repair is now {value.ToString("N3", actor).ColourValue()} per real minute, once per cell.");
		return true;
	}

	private bool BuildingCommandIdle(ICharacter actor, StringStack command)
	{
		var text = command.SafeRemainingArgument;
		double? value = null;
		if (!text.EqualTo("default"))
		{
			if (!TryBuilderNumber(actor, text, 1.0, 3600.0, out var parsed))
			{
				actor.OutputHandler.Send("Use idle default or idle <seconds>, from 1 to 3600 seconds.");
				return false;
			}
			value = parsed;
		}
		ApplyDefinitionChange(() => IdleRecheckSeconds = value);
		actor.OutputHandler.Send(value.HasValue
			? $"Dormant cells will request a central recheck every {value.Value.ToString("N0", actor).ColourValue()} real seconds."
			: "Dormant cells will use the configured world reconciliation cadence.");
		return true;
	}

	private static bool TryBuilderNumber(ICharacter actor, string text, double minimum, double maximum,
		out double value) => double.TryParse(text, NumberStyles.Float, actor, out value) &&
		double.IsFinite(value) && value >= minimum && value <= maximum;

	private bool BuildingCommandOutput(ICharacter actor, StringStack command)
	{
		var argument = command.PopSpeech();
		if (argument.EqualTo("add"))
		{
			return BuildingCommandOutputAdd(actor, command);
		}
		if (argument.EqualToAny("remove", "delete"))
		{
			return BuildingCommandOutputRemove(actor, command);
		}
		var index = FindOutput(argument);
		if (index < 0)
		{
			actor.OutputHandler.Send("Use output add <resource>, output remove <resource>, or output <existing resource> <maximum|rate|basecapacity|baserate> <value>.");
			return false;
		}
		var setting = command.PopForSwitch();
		var output = _outputs[index];
		switch (setting)
		{
			case "maximum":
			case "rate":
				var formula = command.SafeRemainingArgument;
				if (CompileFormula(formula, setting == "rate", _inputs.Select(input => input.Name)
					.ToHashSet(StringComparer.OrdinalIgnoreCase), out var error) is null)
				{
					actor.OutputHandler.Send($"That formula is invalid: {error.ColourError()}");
					return false;
				}
				ApplyDefinitionChange(() => _outputs[index] = setting == "maximum"
					? output with { MaximumFormula = formula }
					: output with { RateFormula = formula });
				actor.OutputHandler.Send($"Output #{output.ResourceId.ToString("N0", actor)} {setting} formula is now {formula.ColourCommand()}.");
				return true;
			case "basecapacity":
			case "baserate":
				if (!TryBuilderNumber(actor, command.SafeRemainingArgument, 0.0, double.MaxValue, out var value))
				{
					actor.OutputHandler.Send($"Use output <resource> {setting} <finite non-negative number>.");
					return false;
				}
				ApplyDefinitionChange(() => _outputs[index] = setting == "basecapacity"
					? output with { BaseCapacity = value }
					: output with { BaseRate = value });
				actor.OutputHandler.Send($"Output #{output.ResourceId.ToString("N0", actor)} {setting} is now {value.ToString("N3", actor).ColourValue()}.");
				return true;
			default:
				actor.OutputHandler.Send("Choose maximum <formula>, rate <formula>, basecapacity <number>, or baserate <number> for that output.");
				return false;
		}
	}

	private int FindOutput(string argument) => long.TryParse(argument, out var id)
		? _outputs.FindIndex(output => output.ResourceId == id)
		: _outputs.FindIndex(output => output.Resource?.Name.EqualTo(argument) == true);

	private bool BuildingCommandOutputAdd(ICharacter actor, StringStack command)
	{
		var resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null || !resource.ResourceType.HasFlag(MagicResourceType.LocationResource))
		{
			actor.OutputHandler.Send("Use output add <resource>, naming an existing resource that supports locations.");
			return false;
		}
		if (_outputs.Count >= MaximumOutputs || _outputs.Any(output => output.ResourceId == resource.Id))
		{
			actor.OutputHandler.Send($"Profiles permit at most {MaximumOutputs} outputs and only one output for each resource.");
			return false;
		}
		ApplyDefinitionChange(() => _outputs.Add(new EnvironmentalMagicOutput(resource.Id, resource,
			"basecapacity", "baserate", 100.0, 1.0)));
		actor.OutputHandler.Send($"Added an output for {resource.Name.ColourName()}, with baseline capacity {100.ToString("N0", actor).ColourValue()} and rate {1.ToString("N0", actor).ColourValue()} per real minute. New room balances start at zero.");
		return true;
	}

	private bool BuildingCommandOutputRemove(ICharacter actor, StringStack command)
	{
		var index = FindOutput(command.SafeRemainingArgument);
		if (index < 0)
		{
			actor.OutputHandler.Send("Use output remove <existing resource name or ID>.");
			return false;
		}
		var resourceId = _outputs[index].ResourceId;
		ApplyDefinitionChange(() => _outputs.RemoveAt(index));
		actor.OutputHandler.Send($"Removed output resource #{resourceId.ToString("N0", actor)}. Recorded room balances and ecological damage are retained.");
		if (_outputs.Count == 0)
		{
			actor.OutputHandler.Send("This profile is now inactive until at least one output is added.".ColourError());
		}
		return true;
	}

	private bool BuildingCommandInput(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech().ToLowerInvariant();
		if (name.EqualToAny("remove", "delete"))
		{
			return BuildingCommandInputRemove(actor, command);
		}
		if (OrganicBuiltInInputs.Contains(name))
		{
			actor.OutputHandler.Send(
				$"Input {name} is a reserved organic scalar/native input name and cannot be rebound.".ColourError());
			return false;
		}
		var kindText = command.PopSpeech();
		if (!Enum.TryParse<EnvironmentalMagicInputKind>(kindText, true, out var kind) || !Enum.IsDefined(kind))
		{
			actor.OutputHandler.Send("Use input <name> <forage|agriculture|prog> <source> <scale>, or input remove <name>.");
			return false;
		}
		var source = command.PopSpeech();
		if (!TryBuilderNumber(actor, command.SafeRemainingArgument, double.MinValue, double.MaxValue, out var scale))
		{
			actor.OutputHandler.Send("Every input requires an explicit finite numeric scale: input <name> <kind> <source> <scale>.");
			return false;
		}
		var prog = kind == EnvironmentalMagicInputKind.Prog ? Gameworld.FutureProgs.GetByIdOrName(source) : null;
		var input = new EnvironmentalMagicInput(name, kind,
			kind == EnvironmentalMagicInputKind.Prog ? prog?.Id.ToString(CultureInfo.InvariantCulture) ?? source : source.ToLowerInvariant(),
			scale, prog?.Id, prog);
		var error = ValidateInput(input);
		if (error is not null)
		{
			actor.OutputHandler.Send(error.ColourError());
			return false;
		}
		var index = _inputs.FindIndex(existing => existing.Name.EqualTo(name));
		if (index < 0 && _inputs.Count >= MaximumInputs)
		{
			actor.OutputHandler.Send($"This profile already has the maximum of {MaximumInputs} named inputs.");
			return false;
		}
		ApplyDefinitionChange(() =>
		{
			if (index < 0)
			{
				_inputs.Add(input);
			}
			else
			{
				_inputs[index] = input;
			}
		});
		actor.OutputHandler.Send($"Input {name.ColourName()} reads {kind.DescribeEnum().ColourName()} source {input.Source.ColourValue()} multiplied by {scale.ToString("N3", actor).ColourValue()}. It is read only when a formula uses it.");
		return true;
	}

	private bool BuildingCommandInputRemove(ICharacter actor, StringStack command)
	{
		var name = command.SafeRemainingArgument;
		if (!_inputs.Any(input => input.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("Use input remove <existing input name>.");
			return false;
		}
		if (RequiredInputNames.Contains(name) || _organicPenalties.Any(penalty =>
			RequiredOrganicInputNames(penalty.Channel).Contains(name)))
		{
			actor.OutputHandler.Send("That input is used by an output or organic penalty formula. Change the formula before removing its input.");
			return false;
		}
		ApplyDefinitionChange(() => _inputs.RemoveAll(input => input.Name.EqualTo(name)));
		actor.OutputHandler.Send($"Removed input {name.ColourName()}.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Environmental Regenerator #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {RegeneratorTypeName.ColourName()} (physical cells; central coordinator)");
		sb.AppendLine($"Definition Version: {_definitionVersion.ToString("N0", actor).ColourValue()}    Runtime Revision: {Revision.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Pressure Half-Life: {PressureHalfLifeSeconds.ToString("N3", actor).ColourValue()} real seconds");
		sb.AppendLine($"Natural Scar Repair: {NaturalRepairPerMinute.ToString("N3", actor).ColourValue()} per real minute (once per cell)");
		sb.AppendLine($"Idle Recheck: {(IdleRecheckSeconds.HasValue ? $"{IdleRecheckSeconds.Value.ToString("N0", actor)} real seconds" : "world default").ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Resource Outputs".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		foreach (var output in _outputs)
		{
			sb.AppendLine();
			sb.AppendLine($"#{output.ResourceId.ToString("N0", actor)} - {(output.Resource?.Name ?? "Missing Resource").ColourName()}");
			sb.AppendLine($"  Maximum: {output.MaximumFormula.ColourCommand()}");
			sb.AppendLine($"  Rate / Minute: {output.RateFormula.ColourCommand()}");
			sb.AppendLine($"  Base Capacity: {output.BaseCapacity.ToString("N3", actor).ColourValue()}    Base Rate: {output.BaseRate.ToString("N3", actor).ColourValue()}");
		}
		sb.AppendLine();
		sb.AppendLine("Named Input Bindings".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		foreach (var input in _inputs)
		{
			sb.AppendLine($"{input.Name.ColourName()}: {input.Kind.DescribeEnum().ColourName()} {input.Source.ColourValue()} x {input.Scale.ToString("N3", actor).ColourValue()} ({(RequiredInputNames.Contains(input.Name) ? "used" : "unused")})");
		}
		if (_inputs.Count == 0)
		{
			sb.AppendLine("No named inputs.");
		}
		AppendOrganicShow(actor, sb);
		sb.AppendLine();
		sb.AppendLine("Validation".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		if (_validationErrors.Count == 0)
		{
			sb.AppendLine("Legacy mana definition is valid. Cell-specific inputs and formula results are checked at use.".ColourValue());
		}
		else
		{
			foreach (var error in _validationErrors)
			{
				sb.AppendLine(error.ColourError());
			}
			sb.AppendLine("This profile is inactive until its configuration errors are repaired.".ColourError());
		}
		return sb.ToString();
	}
}
