#nullable enable

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.FutureProg;
using MudSharp.Magic.Environment;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Generators;

public sealed partial class EnvironmentalMagicGenerator
{
	public const int CurrentOrganicDefinitionVersion = 1;
	public const int MaximumOrganicSources = 32;
	private static readonly FrozenSet<string> OrganicBuiltInInputs = new[]
	{
		"scardamage", "pressure", "hasdefile", "minutessincedefile", "nativestock", "nativehealth",
		"nativeyield", "nativecapacity", "fieldcondition", "baselineincrease"
	}.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	private static readonly FrozenSet<string> EmptyOrganicInputNames =
		Array.Empty<string>().ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	private static readonly ProgVariableTypes[] OrganicProtectionParameters =
	[
		ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.MagicCapability,
		ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Location
	];
	private readonly List<NativeOrganicSourceDeclaration> _organicSources = [];
	private readonly List<NativeOrganicPenaltyDefinition> _organicPenalties = [];
	private readonly List<string> _organicValidationErrors = [];
	private readonly ReadOnlyCollection<NativeOrganicSourceDeclaration> _organicSourceView;
	private readonly ReadOnlyCollection<NativeOrganicPenaltyDefinition> _organicPenaltyView;
	private readonly ReadOnlyCollection<string> _organicErrorView;
	private readonly Dictionary<NativeOrganicPenaltyChannel, CompiledOrganicPenalty> _compiledOrganicPenalties = [];
	private readonly Dictionary<NativeOrganicPenaltyChannel, List<string>> _organicPenaltyValidationErrors = [];
	private bool _hasOrganicConfiguration;
	private bool _showLandScarAddendum;
	private int _organicDefinitionVersion = CurrentOrganicDefinitionVersion;
	private long? _organicProtectionProgId;
	private IFutureProg? _organicProtectionProg;
	private string? _organicProtectionRawValue;
	private string? _organicProtectionLoadError;

	private sealed record CompiledOrganicPenalty(IExpression Expression, string[] Parameters,
		IReadOnlySet<string> RequiredInputs);

	public bool HasOrganicConfiguration => _hasOrganicConfiguration;
	public bool ShowLandScarAddendum => _showLandScarAddendum;
	public IReadOnlyList<NativeOrganicSourceDeclaration> OrganicSources => _organicSourceView;
	public IReadOnlyList<NativeOrganicPenaltyDefinition> OrganicPenalties => _organicPenaltyView;
	public long? OrganicProtectionProgId => _organicProtectionProgId;
	public IFutureProg? OrganicProtectionProg => _organicProtectionProg;
	public IReadOnlyList<string> OrganicValidationErrors => _organicErrorView;

	private void LoadOrganicDefinition(XElement? organic)
	{
		if (organic is null)
		{
			return;
		}

		_hasOrganicConfiguration = true;
		_showLandScarAddendum = bool.TryParse(organic.Attribute("scaraddendum")?.Value, out var showScar) && showScar;
		_organicDefinitionVersion = int.TryParse(organic.Attribute("version")?.Value, out var version) ? version : 0;
		var protectionText = organic.Element("ProtectionProg")?.Value;
		if (!string.IsNullOrWhiteSpace(protectionText))
		{
			if (long.TryParse(protectionText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var progId) && progId > 0)
			{
				_organicProtectionProgId = progId;
			}
			else
			{
				_organicProtectionRawValue = protectionText;
				_organicProtectionLoadError = "The organic protection prog ID is malformed.";
			}
		}

		foreach (var element in organic.Element("Sources")?.Elements("Source") ?? [])
		{
			var kind = Enum.TryParse<NativeOrganicSourceKind>(element.Attribute("kind")?.Value, true, out var parsedKind)
				? parsedKind
				: (NativeOrganicSourceKind)(-1);
			var forageKey = element.Attribute("foragekey")?.Value;
			var selector = element.Attribute("selector")?.Value ??
			               (Enum.IsDefined(kind) ? NativeOrganicSourceSelectors.Canonical(kind, forageKey) : string.Empty);
			var uses = (element.Element("Uses")?.Elements("Use") ?? [])
				.Select(x => Enum.TryParse<AgricultureFieldUse>(x.Value, true, out var use)
					? use
					: (AgricultureFieldUse)(-1))
				.ToArray();
			var definitions = (element.Element("Definitions")?.Elements("Definition") ?? [])
				.Select(x => long.TryParse(x.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : 0)
				.ToArray();
			_organicSources.Add(new NativeOrganicSourceDeclaration(selector, kind, forageKey, uses, definitions));
		}

		foreach (var element in organic.Element("Penalties")?.Elements("Penalty") ?? [])
		{
			var channel = Enum.TryParse<NativeOrganicPenaltyChannel>(element.Attribute("channel")?.Value, true,
				out var parsedChannel) ? parsedChannel : (NativeOrganicPenaltyChannel)(-1);
			_organicPenalties.Add(new NativeOrganicPenaltyDefinition(channel, element.Value, Array.Empty<string>()));
		}
	}

	private XElement? SaveOrganicDefinition()
	{
		if (!_hasOrganicConfiguration)
		{
			return null;
		}

		return new XElement("Organic",
			new XAttribute("version", _organicDefinitionVersion),
			new XAttribute("scaraddendum", _showLandScarAddendum),
			_organicProtectionProgId.HasValue
				? new XElement("ProtectionProg", _organicProtectionProgId.Value)
				: _organicProtectionRawValue is not null
					? new XElement("ProtectionProg", _organicProtectionRawValue)
					: null,
			new XElement("Sources", _organicSources.Select(source => new XElement("Source",
				new XAttribute("selector", source.Selector),
				new XAttribute("kind", source.Kind),
				source.ForageKey is not null ? new XAttribute("foragekey", source.ForageKey) : null,
				new XElement("Uses", source.AllowedFieldUses.Select(use => new XElement("Use", use))),
				new XElement("Definitions", source.DefinitionIds.Select(id => new XElement("Definition", id)))))),
			new XElement("Penalties", _organicPenalties.Select(penalty => new XElement("Penalty",
				new XAttribute("channel", penalty.Channel), penalty.Formula))));
	}

	private void RebuildOrganicDefinition(ISet<string> namedInputs)
	{
		_organicValidationErrors.Clear();
		_compiledOrganicPenalties.Clear();
		_organicPenaltyValidationErrors.Clear();
		if (_organicProtectionLoadError is not null)
		{
			_organicValidationErrors.Add(_organicProtectionLoadError);
		}
		if (!_hasOrganicConfiguration)
		{
			_organicProtectionProg = null;
			return;
		}

		if (_organicDefinitionVersion != CurrentOrganicDefinitionVersion)
		{
			_organicValidationErrors.Add(
				$"Unsupported organic definition version {_organicDefinitionVersion}; expected {CurrentOrganicDefinitionVersion}.");
		}
		if (_organicSources.Count > MaximumOrganicSources)
		{
			_organicValidationErrors.Add($"An environmental profile permits at most {MaximumOrganicSources} organic source declarations.");
		}

		var selectors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		for (var i = 0; i < _organicSources.Count; i++)
		{
			var source = _organicSources[i];
			var error = ValidateOrganicSource(source);
			if (error is not null)
			{
				_organicValidationErrors.Add($"Organic source '{source.Selector}': {error}");
			}
			var identity = ExpectedOrganicSelector(source);
			if (!selectors.Add(identity))
			{
				_organicValidationErrors.Add($"Organic source '{identity}' is declared more than once.");
			}
		}

		var duplicateChannels = _organicPenalties
			.Where(penalty => Enum.IsDefined(penalty.Channel))
			.GroupBy(penalty => penalty.Channel)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToHashSet();
		foreach (var channel in duplicateChannels)
		{
			AddOrganicPenaltyValidationError(channel,
				$"Organic penalty {channel.DescribeEnum()} is defined more than once.");
		}

		for (var i = 0; i < _organicPenalties.Count; i++)
		{
			var penalty = _organicPenalties[i];
			if (!Enum.IsDefined(penalty.Channel))
			{
				_organicValidationErrors.Add("An organic penalty names an unknown channel.");
				continue;
			}
			if (duplicateChannels.Contains(penalty.Channel) ||
			    _organicDefinitionVersion != CurrentOrganicDefinitionVersion)
			{
				continue;
			}

			var expression = CompileOrganicFormula(penalty.Formula, namedInputs, out var parameters, out var error);
			_organicPenalties[i] = penalty with { RequiredInputNames = parameters };
			if (expression is not null)
			{
				foreach (var parameter in parameters.Where(x => !OrganicBuiltInInputs.Contains(x)))
				{
					var bindings = _inputs.Where(x => x.Name.EqualTo(parameter)).ToList();
					var bindingError = bindings.Count == 1 ? ValidateInput(bindings[0]) :
						$"Input {parameter} must have exactly one declared binding.";
					if (bindingError is null) continue;
					error = bindingError;
					expression = null;
					break;
				}
			}
			if (expression is null)
			{
				AddOrganicPenaltyValidationError(penalty.Channel,
					$"Organic penalty {penalty.Channel.DescribeEnum()}: {error}");
				continue;
			}
			_compiledOrganicPenalties[penalty.Channel] = new CompiledOrganicPenalty(expression, parameters,
				parameters.ToFrozenSet(StringComparer.OrdinalIgnoreCase));
		}

		_organicProtectionProg = _organicProtectionProgId.HasValue
			? Gameworld.FutureProgs.Get(_organicProtectionProgId.Value)
			: null;
		if (_organicProtectionProgId.HasValue && !ValidOrganicProtectionProg(_organicProtectionProg))
		{
			_organicValidationErrors.Add(
				$"Organic protection prog #{_organicProtectionProgId.Value} must be a compiled boolean prog accepting exactly character, character, magic capability, text, number and location.");
		}
	}

	private void AddOrganicPenaltyValidationError(NativeOrganicPenaltyChannel channel, string error)
	{
		_organicValidationErrors.Add(error);
		if (!_organicPenaltyValidationErrors.TryGetValue(channel, out var errors))
		{
			errors = [];
			_organicPenaltyValidationErrors[channel] = errors;
		}
		errors.Add(error);
	}

	private static string ExpectedOrganicSelector(NativeOrganicSourceDeclaration source)
	{
		if (!Enum.IsDefined(source.Kind))
		{
			return source.Selector;
		}
		return NativeOrganicSourceSelectors.Canonical(source.Kind, source.ForageKey);
	}

	private static bool OrganicSourceRelatesTo(NativeOrganicSourceDeclaration source, string canonicalSelector) =>
		source.Selector.EqualTo(canonicalSelector) || ExpectedOrganicSelector(source).EqualTo(canonicalSelector);

	private void AddOrganicSourceGlobalErrors(List<string> errors)
	{
		if (_organicDefinitionVersion != CurrentOrganicDefinitionVersion)
		{
			errors.Add(
				$"Unsupported organic definition version {_organicDefinitionVersion}; expected {CurrentOrganicDefinitionVersion}.");
		}
		if (_organicSources.Count > MaximumOrganicSources)
		{
			errors.Add($"An environmental profile permits at most {MaximumOrganicSources} organic source declarations.");
		}
	}

	internal string? OrganicSourceValidationError(string canonicalSelector)
	{
		var errors = new List<string>();
		AddOrganicSourceGlobalErrors(errors);
		var related = _organicSources.Where(source => OrganicSourceRelatesTo(source, canonicalSelector)).ToList();
		foreach (var source in related)
		{
			var error = ValidateOrganicSource(source);
			if (error is not null)
			{
				errors.Add($"Organic source '{source.Selector}': {error}");
			}
		}
		if (related.Count > 1)
		{
			errors.Add($"Organic source '{canonicalSelector}' is declared more than once.");
		}
		return errors.Count == 0 ? null : string.Join("; ", errors.Distinct());
	}

	internal string? OrganicSourceValidationError(NativeOrganicSourceDeclaration declaration)
	{
		var errors = new List<string>();
		AddOrganicSourceGlobalErrors(errors);
		var error = ValidateOrganicSource(declaration);
		if (error is not null)
		{
			errors.Add($"Organic source '{declaration.Selector}': {error}");
		}
		var expected = ExpectedOrganicSelector(declaration);
		if (_organicSources.Count(source => OrganicSourceRelatesTo(source, expected)) > 1)
		{
			errors.Add($"Organic source '{expected}' is declared more than once.");
		}
		return errors.Count == 0 ? null : string.Join("; ", errors.Distinct());
	}

	internal string? OrganicPenaltyValidationError(NativeOrganicPenaltyChannel channel)
	{
		if (!_hasOrganicConfiguration)
		{
			return null;
		}
		var errors = new List<string>();
		if (_organicDefinitionVersion != CurrentOrganicDefinitionVersion)
		{
			errors.Add(
				$"Unsupported organic definition version {_organicDefinitionVersion}; expected {CurrentOrganicDefinitionVersion}.");
		}
		if (_organicPenaltyValidationErrors.TryGetValue(channel, out var channelErrors))
		{
			errors.AddRange(channelErrors);
		}
		return errors.Count == 0 ? null : string.Join("; ", errors.Distinct());
	}

	private string? ValidateOrganicSource(NativeOrganicSourceDeclaration source)
	{
		if (!Enum.IsDefined(source.Kind))
		{
			return "the source kind is unknown.";
		}
		var canonical = NativeOrganicSourceSelectors.Canonical(source.Kind, source.ForageKey);
		if (source.Kind == NativeOrganicSourceKind.Forage && !NativeOrganicSourceSelectors.IsValidForageKey(source.ForageKey))
		{
			return "forage declarations require a non-empty normalised yield key of at most 128 characters.";
		}
		if (!source.Selector.EqualTo(canonical))
		{
			return $"the canonical selector is '{canonical}'.";
		}
		if (source.DefinitionIds.Any(id => id <= 0) || source.DefinitionIds.Distinct().Count() != source.DefinitionIds.Count)
		{
			return "definition restrictions must contain unique positive IDs.";
		}
		if (source.AllowedFieldUses.Any(use => !Enum.IsDefined(use)) ||
		    source.AllowedFieldUses.Distinct().Count() != source.AllowedFieldUses.Count)
		{
			return "field-use restrictions must contain unique known uses.";
		}

		switch (source.Kind)
		{
			case NativeOrganicSourceKind.Forage:
				return source.DefinitionIds.Count > 0 ? "forage sources cannot have vegetation definition restrictions." : null;
			case NativeOrganicSourceKind.Crop:
				if (source.AllowedFieldUses.Any(use => use is not AgricultureFieldUse.Crop and not AgricultureFieldUse.Orchard))
				{
					return "crop sources may only permit Crop and Orchard field uses.";
				}
				var missingCrop = source.DefinitionIds.FirstOrDefault(id => Gameworld.AgricultureCropDefinitions?.Get(id) is null);
				return missingCrop > 0 ? $"crop definition #{missingCrop} does not exist." : null;
			case NativeOrganicSourceKind.Woodland:
				if (source.AllowedFieldUses.Any(use => use != AgricultureFieldUse.Woodland))
				{
					return "woodland sources may only permit the Woodland field use.";
				}
				var missingWoodland = source.DefinitionIds.FirstOrDefault(id => Gameworld.AgricultureWoodlandDefinitions?.Get(id) is null);
				return missingWoodland > 0 ? $"woodland definition #{missingWoodland} does not exist." : null;
			case NativeOrganicSourceKind.Pasture:
				if (source.AllowedFieldUses.Any(use => use != AgricultureFieldUse.Pasture))
				{
					return "pasture sources may only permit the Pasture field use.";
				}
				return source.DefinitionIds.Count > 0 ? "pasture sources cannot have vegetation definition restrictions." : null;
			default:
				return "the source kind is unknown.";
		}
	}

	private static IExpression? CompileOrganicFormula(string text, ISet<string> namedInputs,
		out string[] parameters, out string? error)
	{
		parameters = Array.Empty<string>();
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
		parameters = expression.ParameterNames.ToArray();
		foreach (var parameter in parameters)
		{
			if (!OrganicBuiltInInputs.Contains(parameter) && !namedInputs.Contains(parameter))
			{
				error = $"Input {parameter} is not an organic scalar/native input or a declared named input.";
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
				error = $"Random function {function} cannot define a repeatable organic penalty.";
				return null;
			}
		}
		if (parameters.Length == 0)
		{
			if (!expression.TryEvaluateDoubleWith(new Dictionary<string, object>(), out var value, out var evaluationError))
			{
				error = evaluationError;
				return null;
			}
			if (!double.IsFinite(value) || value is < 0.0 or > 1.0)
			{
				error = "A constant organic penalty must be finite and from 0 through 1.";
				return null;
			}
		}
		return expression;
	}

	private static bool ValidOrganicProtectionProg(IFutureProg? prog) => prog is not null &&
		!prog.AcceptsAnyParameters && string.IsNullOrWhiteSpace(prog.CompileError) &&
		prog.ReturnType == ProgVariableTypes.Boolean && prog.Parameters.SequenceEqual(OrganicProtectionParameters);

	public IReadOnlySet<string> RequiredOrganicInputNames(NativeOrganicPenaltyChannel channel) =>
		_compiledOrganicPenalties.TryGetValue(channel, out var compiled)
			? compiled.RequiredInputs
			: EmptyOrganicInputNames;

	public NativeOrganicPenaltyEvaluation EvaluateOrganicPenalty(NativeOrganicPenaltyChannel channel,
		IReadOnlyDictionary<string, double> inputs)
	{
		if (OrganicPenaltyValidationError(channel) is { } validationError)
		{
			return NativeOrganicPenaltyEvaluation.Invalid(validationError);
		}
		var configured = _organicPenalties.FirstOrDefault(x => x.Channel == channel);
		if (configured is null)
		{
			return NativeOrganicPenaltyEvaluation.Neutral;
		}
		if (!_compiledOrganicPenalties.TryGetValue(channel, out var compiled))
		{
			return NativeOrganicPenaltyEvaluation.Invalid(
				$"The {channel.DescribeEnum()} organic penalty definition is invalid.");
		}

		var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		foreach (var parameter in compiled.Parameters)
		{
			if (!inputs.TryGetValue(parameter, out var value))
			{
				return NativeOrganicPenaltyEvaluation.Invalid($"Input {parameter} is missing from the organic snapshot.");
			}
			if (!double.IsFinite(value))
			{
				return NativeOrganicPenaltyEvaluation.Invalid($"Input {parameter} is not finite.");
			}
			values[parameter] = value;
		}
		if (!compiled.Expression.TryEvaluateDoubleWith(values, out var factor, out var error))
		{
			return NativeOrganicPenaltyEvaluation.Invalid(error ?? "The organic penalty could not be evaluated.");
		}
		if (!double.IsFinite(factor) || factor is < 0.0 or > 1.0)
		{
			return NativeOrganicPenaltyEvaluation.Invalid(
				"The organic penalty result must be finite and from 0 through 1.");
		}
		return new NativeOrganicPenaltyEvaluation(true, true, factor, null);
	}

	private bool BuildingCommandOrganic(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "sources":
				actor.OutputHandler.Send(DescribeOrganicSources(actor));
				return true;
			case "source":
				return BuildingCommandOrganicSource(actor, command);
			case "penalty":
				return BuildingCommandOrganicPenalty(actor, command);
			case "protection":
				return BuildingCommandOrganicProtection(actor, command);
			case "scaraddendum":
				return BuildingCommandOrganicScarAddendum(actor, command);
			default:
				actor.OutputHandler.Send("Use organic sources, organic source <...>, organic penalty <channel> <formula|none>, organic protection <prog|none>, or organic scaraddendum <on|off>.");
				return false;
		}
	}

	private bool BuildingCommandOrganicScarAddendum(ICharacter actor, StringStack command)
	{
		var setting = command.PopForSwitch();
		if (setting is not ("on" or "off") || !command.IsFinished)
		{
			actor.OutputHandler.Send("Use organic scaraddendum <on|off>.");
			return false;
		}
		ApplyDefinitionChange(() =>
		{
			_hasOrganicConfiguration = true;
			_organicDefinitionVersion = CurrentOrganicDefinitionVersion;
			_showLandScarAddendum = setting == "on";
		});
		actor.OutputHandler.Send($"Land scar room addendum is now {(_showLandScarAddendum ? "on" : "off").ColourValue()}.");
		return true;
	}

	private bool BuildingCommandOrganicSource(ICharacter actor, StringStack command)
	{
		var argument = command.PopSpeech();
		if (argument.EqualTo("add"))
		{
			return BuildingCommandOrganicSourceAdd(actor, command);
		}
		if (argument.EqualToAny("remove", "delete"))
		{
			return BuildingCommandOrganicSourceRemove(actor, command);
		}
		if (!TryOrganicSelector(argument, out var selector, out var kind, out _))
		{
			actor.OutputHandler.Send("Specify crop, woodland, pasture, or forage:<yield-key> as the source selector.");
			return false;
		}
		var index = _organicSources.FindIndex(x => x.Selector.EqualTo(selector));
		if (index < 0)
		{
			actor.OutputHandler.Send($"Organic source {selector.ColourCommand()} is not authorised by this profile.");
			return false;
		}
		var setting = command.PopForSwitch();
		return setting switch
		{
			"uses" => BuildingCommandOrganicSourceUses(actor, command, index, kind),
			"definitions" or "definition" => BuildingCommandOrganicSourceDefinitions(actor, command, index, kind),
			_ => OrganicSourceSettingError(actor)
		};
	}

	private static bool OrganicSourceSettingError(ICharacter actor)
	{
		actor.OutputHandler.Send("Choose uses <compatible uses|any> or definitions <positive IDs|any> for that source.");
		return false;
	}

	private bool BuildingCommandOrganicSourceAdd(ICharacter actor, StringStack command)
	{
		if (_organicSources.Count >= MaximumOrganicSources)
		{
			actor.OutputHandler.Send($"This profile already has the maximum of {MaximumOrganicSources} organic sources.");
			return false;
		}
		var kindText = command.PopForSwitch();
		if (!Enum.TryParse<NativeOrganicSourceKind>(kindText, true, out var kind) || !Enum.IsDefined(kind))
		{
			actor.OutputHandler.Send("Use organic source add forage <yield-key>, crop, woodland, or pasture.");
			return false;
		}
		string? forageKey = null;
		if (kind == NativeOrganicSourceKind.Forage)
		{
			forageKey = NativeOrganicSourceSelectors.NormaliseForageKey(command.SafeRemainingArgument);
			if (!NativeOrganicSourceSelectors.IsValidForageKey(forageKey))
			{
				actor.OutputHandler.Send("A forage source requires a non-empty yield key of at most 128 characters.");
				return false;
			}
		}
		else if (!command.IsFinished)
		{
			actor.OutputHandler.Send("Field source additions take no further arguments.");
			return false;
		}
		var selector = NativeOrganicSourceSelectors.Canonical(kind, forageKey);
		if (_organicSources.Any(x => x.Selector.EqualTo(selector)))
		{
			actor.OutputHandler.Send($"Organic source {selector.ColourCommand()} is already authorised.");
			return false;
		}
		var source = new NativeOrganicSourceDeclaration(selector, kind, forageKey,
			Array.Empty<AgricultureFieldUse>(), Array.Empty<long>());
		ApplyDefinitionChange(() =>
		{
			_hasOrganicConfiguration = true;
			_organicDefinitionVersion = CurrentOrganicDefinitionVersion;
			_organicSources.Add(source);
		});
		actor.OutputHandler.Send($"Authorised native organic source {selector.ColourCommand()} for this environmental profile.");
		return true;
	}

	private bool BuildingCommandOrganicSourceRemove(ICharacter actor, StringStack command)
	{
		if (!TryOrganicSelector(command.SafeRemainingArgument, out var selector, out _, out _))
		{
			actor.OutputHandler.Send("Use organic source remove <crop|woodland|pasture|forage:key>.");
			return false;
		}
		var index = _organicSources.FindIndex(x => x.Selector.EqualTo(selector));
		if (index < 0)
		{
			actor.OutputHandler.Send($"Organic source {selector.ColourCommand()} is not authorised.");
			return false;
		}
		ApplyDefinitionChange(() => _organicSources.RemoveAt(index));
		actor.OutputHandler.Send($"Removed organic source authorisation {selector.ColourCommand()}. Native stock and accounting are unchanged.");
		return true;
	}

	private bool BuildingCommandOrganicSourceUses(ICharacter actor, StringStack command, int index,
		NativeOrganicSourceKind kind)
	{
		if (kind == NativeOrganicSourceKind.Forage)
		{
			actor.OutputHandler.Send("Forage sources use their exact yield key and ignore field-use restrictions.");
			return false;
		}
		var text = command.SafeRemainingArgument;
		AgricultureFieldUse[] uses;
		if (text.EqualTo("any"))
		{
			uses = Array.Empty<AgricultureFieldUse>();
		}
		else
		{
			var tokens = text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			var parsed = new List<AgricultureFieldUse>();
			foreach (var token in tokens)
			{
				if (!Enum.TryParse<AgricultureFieldUse>(token, true, out var use) || !Enum.IsDefined(use))
				{
					actor.OutputHandler.Send($"Unknown agriculture field use {token.ColourError()}.");
					return false;
				}
				parsed.Add(use);
			}
			uses = parsed.Distinct().ToArray();
			if (uses.Length == 0)
			{
				actor.OutputHandler.Send("Specify compatible field uses or any.");
				return false;
			}
		}
		var candidate = _organicSources[index] with { AllowedFieldUses = uses };
		var error = ValidateOrganicSource(candidate);
		if (error is not null)
		{
			actor.OutputHandler.Send(error.ColourError());
			return false;
		}
		ApplyDefinitionChange(() => _organicSources[index] = candidate);
		actor.OutputHandler.Send($"Organic source {candidate.Selector.ColourCommand()} now permits {(uses.Length == 0 ? "any compatible field use" : uses.Select(x => x.DescribeEnum()).ListToString()).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandOrganicSourceDefinitions(ICharacter actor, StringStack command, int index,
		NativeOrganicSourceKind kind)
	{
		if (kind is not NativeOrganicSourceKind.Crop and not NativeOrganicSourceKind.Woodland)
		{
			actor.OutputHandler.Send("Only crop and woodland sources can restrict vegetation definition IDs.");
			return false;
		}
		var text = command.SafeRemainingArgument;
		long[] ids;
		if (text.EqualTo("any"))
		{
			ids = Array.Empty<long>();
		}
		else
		{
			var tokens = text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0 || tokens.Any(token => !long.TryParse(token, NumberStyles.Integer,
				    CultureInfo.InvariantCulture, out var id) || id <= 0))
			{
				actor.OutputHandler.Send("Specify one or more positive definition IDs separated by spaces/commas, or any.");
				return false;
			}
			ids = tokens.Select(token => long.Parse(token, CultureInfo.InvariantCulture)).Distinct().ToArray();
		}
		var candidate = _organicSources[index] with { DefinitionIds = ids };
		var error = ValidateOrganicSource(candidate);
		if (error is not null)
		{
			actor.OutputHandler.Send(error.ColourError());
			return false;
		}
		ApplyDefinitionChange(() => _organicSources[index] = candidate);
		actor.OutputHandler.Send($"Organic source {candidate.Selector.ColourCommand()} now permits {(ids.Length == 0 ? "any compatible definition" : ids.Select(x => $"#{x}").ListToString()).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandOrganicPenalty(ICharacter actor, StringStack command)
	{
		var token = command.PopSpeech();
		if (!TryOrganicPenaltyChannel(token, out var channel))
		{
			actor.OutputHandler.Send($"Choose one of: {OrganicPenaltyTokens().Select(x => x.ColourCommand()).ListToString()}.");
			return false;
		}
		var formula = command.SafeRemainingArgument;
		var index = _organicPenalties.FindIndex(x => x.Channel == channel);
		if (formula.EqualToAny("none", "clear", "default"))
		{
			if (index < 0)
			{
				actor.OutputHandler.Send("That channel already uses the neutral default factor of 1.0.");
				return false;
			}
			ApplyDefinitionChange(() => _organicPenalties.RemoveAt(index));
			actor.OutputHandler.Send($"{channel.DescribeEnum().ColourName()} now uses the neutral default factor of 1.0.");
			return true;
		}
		if (CompileOrganicFormula(formula, _inputs.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase),
			out var parameters, out var error) is null)
		{
			actor.OutputHandler.Send($"That organic penalty is invalid: {error?.ColourError()}");
			return false;
		}
		var definition = new NativeOrganicPenaltyDefinition(channel, formula, parameters);
		ApplyDefinitionChange(() =>
		{
			_hasOrganicConfiguration = true;
			_organicDefinitionVersion = CurrentOrganicDefinitionVersion;
			if (index < 0) _organicPenalties.Add(definition);
			else _organicPenalties[index] = definition;
		});
		actor.OutputHandler.Send($"{channel.DescribeEnum().ColourName()} now uses dimensionless factor {formula.ColourCommand()} (strictly 0 through 1 at runtime).");
		return true;
	}

	private bool BuildingCommandOrganicProtection(ICharacter actor, StringStack command)
	{
		var text = command.SafeRemainingArgument;
		if (text.EqualToAny("none", "clear"))
		{
			ApplyDefinitionChange(() =>
			{
				_hasOrganicConfiguration = true;
				_organicDefinitionVersion = CurrentOrganicDefinitionVersion;
				_organicProtectionProgId = null;
				_organicProtectionProg = null;
				_organicProtectionRawValue = null;
				_organicProtectionLoadError = null;
			});
			actor.OutputHandler.Send("This profile has no later-use organic protection prog.");
			return true;
		}
		var prog = Gameworld.FutureProgs.GetByIdOrName(text);
		if (!ValidOrganicProtectionProg(prog))
		{
			actor.OutputHandler.Send("Specify a compiled boolean prog accepting exactly character, character, magic capability, text, number and location.".ColourError());
			return false;
		}
		ApplyDefinitionChange(() =>
		{
			_hasOrganicConfiguration = true;
			_organicDefinitionVersion = CurrentOrganicDefinitionVersion;
			_organicProtectionProgId = prog!.Id;
			_organicProtectionProg = prog;
			_organicProtectionRawValue = null;
			_organicProtectionLoadError = null;
		});
		actor.OutputHandler.Send($"Organic protection prog is now {prog!.MXPClickableFunctionName()} for later destructive-use integration; this profile does not invoke it itself.");
		return true;
	}

	private string DescribeOrganicSources(ICharacter actor)
	{
		if (_organicSources.Count == 0)
		{
			return "This profile authorises no native organic sources.";
		}
		var rows = _organicSources.Select(source => new[]
		{
			source.Selector,
			source.Kind == NativeOrganicSourceKind.Forage ? "N/A" :
				source.AllowedFieldUses.Count == 0 ? "Any compatible" : source.AllowedFieldUses.Select(x => x.DescribeEnum()).ListToString(),
			source.Kind is not (NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland) ? "N/A" :
				source.DefinitionIds.Count == 0 ? "Any compatible" : source.DefinitionIds.Select(x => $"#{x}").ListToString()
		});
		return StringUtilities.GetTextTable(rows, new[] { "Selector", "Field Uses", "Definitions" }, actor, Telnet.Green);
	}

	private void AppendOrganicShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine();
		sb.AppendLine("Native Organic Sources".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine(DescribeOrganicSources(actor));
		sb.AppendLine();
		sb.AppendLine("Ecological Penalties".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		foreach (var channel in Enum.GetValues<NativeOrganicPenaltyChannel>())
		{
			var definition = _organicPenalties.FirstOrDefault(x => x.Channel == channel);
			sb.AppendLine($"{OrganicPenaltyToken(channel).ColourName()}: {(definition?.Formula ?? "1.0 (neutral default)").ColourCommand()} [dimensionless 0..1]");
		}
		sb.AppendLine($"Protection Prog: {(_organicProtectionProg is null ? "None".ColourValue() : _organicProtectionProg.MXPClickableFunctionName())}");
		sb.AppendLine($"Land Scar Room Addendum: {_showLandScarAddendum.ToColouredString()}");
		if (_organicValidationErrors.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Organic Validation".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			foreach (var error in _organicValidationErrors) sb.AppendLine(error.ColourError());
			sb.AppendLine("Only affected organic conversion/recovery is unavailable; legacy environmental mana remains independently valid.".ColourError());
		}
	}

	private static bool TryOrganicSelector(string text, out string selector, out NativeOrganicSourceKind kind,
		out string? forageKey)
	{
		text = text.Trim();
		forageKey = null;
		if (text.StartsWith("forage:", StringComparison.OrdinalIgnoreCase))
		{
			kind = NativeOrganicSourceKind.Forage;
			forageKey = NativeOrganicSourceSelectors.NormaliseForageKey(text[7..]);
			selector = NativeOrganicSourceSelectors.Canonical(kind, forageKey);
			return NativeOrganicSourceSelectors.IsValidForageKey(forageKey);
		}
		if (text.EqualTo("orchard")) text = "crop";
		if (Enum.TryParse<NativeOrganicSourceKind>(text, true, out kind) && Enum.IsDefined(kind) &&
		    kind != NativeOrganicSourceKind.Forage)
		{
			selector = NativeOrganicSourceSelectors.Canonical(kind);
			return true;
		}
		selector = string.Empty;
		kind = default;
		return false;
	}

	private static bool TryOrganicPenaltyChannel(string token, out NativeOrganicPenaltyChannel channel)
	{
		var normalised = new string(token.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
		foreach (var value in Enum.GetValues<NativeOrganicPenaltyChannel>())
		{
			if (normalised == OrganicPenaltyToken(value))
			{
				channel = value;
				return true;
			}
		}
		channel = default;
		return false;
	}

	private static IEnumerable<string> OrganicPenaltyTokens() =>
		Enum.GetValues<NativeOrganicPenaltyChannel>().Select(OrganicPenaltyToken);

	private static string OrganicPenaltyToken(NativeOrganicPenaltyChannel channel) => channel switch
	{
		NativeOrganicPenaltyChannel.ForageReplenishment => "forage",
		NativeOrganicPenaltyChannel.CropHealthRecovery => "crophealth",
		NativeOrganicPenaltyChannel.CropYieldRecovery => "cropyield",
		NativeOrganicPenaltyChannel.WoodlandHealthRecovery => "woodlandhealth",
		NativeOrganicPenaltyChannel.WoodlandYieldRecovery => "woodlandyield",
		NativeOrganicPenaltyChannel.PastureRecovery => "pasture",
		NativeOrganicPenaltyChannel.CropInitialisation => "cropinitial",
		NativeOrganicPenaltyChannel.WoodlandInitialisation => "woodlandinitial",
		NativeOrganicPenaltyChannel.PastureInitialisation => "pastureinitial",
		_ => channel.ToString().ToLowerInvariant()
	};
}
