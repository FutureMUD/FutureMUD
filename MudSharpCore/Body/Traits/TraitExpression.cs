using ExpressionEngine;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System.Text.RegularExpressions;

namespace MudSharp.Body.Traits;

public partial class TraitExpression : SaveableItem, ITraitExpression
{
    #region Overrides of Item

    public override string FrameworkItemType => "TraitExpression";

    #endregion

    public new string Name
    {
        get => _name;
        set
        {
            _name = value;
            Changed = true;
        }
    }

    public static Regex TraitFormulaRegex =
        new(@"(?<name>\w+):(?<target>!)?(?<reference>[0-9]+)(?:{(?<flags>[a-z,]+)})*",
            RegexOptions.IgnoreCase);

    public static Regex ExtendedOptionsRegex =
        new(@"{(?<name>\w+)\s+(?<option>[a-z]+)=(?<value>[^}]+?)}", RegexOptions.IgnoreCase);

    private void LoadFromDatabase(MudSharp.Models.TraitExpression expression)
    {
        _id = expression.Id;
        _name = expression.Name;
        OriginalFormulaText = expression.Expression;
        foreach (TraitExpressionParameters param in expression.TraitExpressionParameters)
        {
            _parameterIndexes[param.Parameter] = param.TraitDefinitionId;
            _parameterProperties[param.Parameter] = (param.CanImprove, param.CanBranch);
        }

        foreach (Match match in TraitFormulaRegex.Matches(expression.Expression))
        {
            _parameterIndexes[match.Groups["name"].Value] = long.Parse(match.Groups["reference"].Value);
            var isTarget = match.Groups["target"].Success;
            _parameterTargets[match.Groups["name"].Value] = isTarget;
            bool canImprove = !isTarget, canBranch = !isTarget, useRawValue = false;
            if (match.Groups["flags"].Length > 0)
            {
                foreach (string flag in match.Groups["flags"].Value.Split(','))
                {
                    switch (flag.ToLowerInvariant())
                    {
                        case "raw":
                            useRawValue = true;
                            break;
                        case "noimprove":
                            canImprove = false;
                            break;
                        case "nobranch":
                            canBranch = false;
                            break;
                    }
                }
            }

            _parameterProperties[match.Groups["name"].Value] = (canImprove, canBranch);
            _parameterRawValues[match.Groups["name"].Value] = useRawValue;
        }

        string expressionText = TraitFormulaRegex.Replace(expression.Expression, m => m.Groups["name"].Value);

        foreach (Match match in ExtendedOptionsRegex.Matches(expressionText))
        {
            switch (match.Groups["option"].Value.ToLowerInvariant())
            {
                case "race":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRaceOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "culture":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionCultureOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "role":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value, Gameworld)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes =
                                new List<ChargenRoleType>(Enum.GetValues(typeof(ChargenRoleType))
                                                              .OfType<ChargenRoleType>())
                        };
                    break;
                case "class":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value, Gameworld)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes = new List<ChargenRoleType>
                            {
                                ChargenRoleType.Class
                            }
                        };
                    break;
                case "merit":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionMeritOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                default:
                    _options[match.Groups["name"].Value.ToLowerInvariant()] = new TraitExpressionAlwaysZeroOption
                    { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
            }
        }

        expressionText = ExtendedOptionsRegex.Replace(expressionText, m => m.Groups["name"].Value.ToLowerInvariant());

        Formula = new Expression(expressionText);
    }

    public TraitExpression(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
        Formula = new Expression("0");
        OriginalFormulaText = "0";
        using (new FMDB())
        {
            Models.TraitExpression dbitem = new();
            FMDB.Context.TraitExpressions.Add(dbitem);

            dbitem.Name = Name;
            dbitem.Expression = "0";
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public TraitExpression(Expression expression, Dictionary<string, ITraitDefinition> parameters)
    {
        _name = "Unnamed Trait Expression";
        Formula = expression;
        OriginalFormulaText = expression.OriginalExpression;
        _parameters = parameters.ToDictionary(x => x.Key, x => new TraitExpressionParameter
        {
            Trait = x.Value,
            CanImprove = true,
            CanBranch = true
        });
    }

    public TraitExpression(Models.TraitExpression expression, IFuturemud game)
    {
        Gameworld = game;
        LoadFromDatabase(expression);
    }

    public TraitExpression(string expression, IFuturemud game)
    {
        Gameworld = game;
        _name = "Unnamed Trait Expression";
        OriginalFormulaText = expression;
        foreach (Match match in TraitFormulaRegex.Matches(expression))
        {
            _parameterIndexes[match.Groups["name"].Value] = long.Parse(match.Groups["reference"].Value);
            var isTarget = match.Groups["target"].Success;
            _parameterTargets[match.Groups["name"].Value] = isTarget;
            bool canImprove = !isTarget, canBranch = !isTarget, useRawValue = false;
            if (match.Groups["flags"].Length > 0)
            {
                foreach (string flag in match.Groups["flags"].Value.Split(','))
                {
                    switch (flag.ToLowerInvariant())
                    {
                        case "raw":
                            useRawValue = true;
                            break;
                        case "noimprove":
                            canImprove = false;
                            break;
                        case "nobranch":
                            canBranch = false;
                            break;
                    }
                }
            }

            _parameterProperties[match.Groups["name"].Value] = (canImprove, canBranch);
            _parameterRawValues[match.Groups["name"].Value] = useRawValue;
        }

        expression = TraitFormulaRegex.Replace(expression, m => m.Groups["name"].Value);
        foreach (Match match in ExtendedOptionsRegex.Matches(expression))
        {
            switch (match.Groups["option"].Value.ToLowerInvariant())
            {
                case "race":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRaceOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "culture":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionCultureOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "role":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value, Gameworld)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes =
                                new List<ChargenRoleType>(Enum.GetValues(typeof(ChargenRoleType))
                                                              .OfType<ChargenRoleType>())
                        };
                    break;
                case "class":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value, Gameworld)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes = new List<ChargenRoleType>
                            {
                                ChargenRoleType.Class
                            }
                        };
                    break;
                case "merit":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionMeritOption(match.Groups["value"].Value, Gameworld)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                default:
                    _options[match.Groups["name"].Value.ToLowerInvariant()] = new TraitExpressionAlwaysZeroOption
                    { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
            }
        }

        expression = ExtendedOptionsRegex.Replace(expression, m => m.Groups["name"].Value.ToLowerInvariant());
        Formula = new Expression(expression);
    }

    public TraitExpression(TraitExpression rhs)
    {
        Gameworld = rhs.Gameworld;
        using (new FMDB())
        {
            Models.TraitExpression dbitem = new()
            {
                Name = rhs.Name,
                Expression = rhs.OriginalFormulaText
            };
            foreach (KeyValuePair<string, TraitExpressionParameter> parameter in rhs.Parameters)
            {
                TraitExpressionParameters dbparam = new();
                dbitem.TraitExpressionParameters.Add(dbparam);
                dbparam.Parameter = parameter.Key;
                dbparam.CanBranch = parameter.Value.CanBranch;
                dbparam.CanImprove = parameter.Value.CanImprove;
                dbparam.TraitDefinitionId = parameter.Value.Trait.Id;
            }

            FMDB.Context.TraitExpressions.Add(dbitem);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dbitem);
        }
    }

    private readonly Dictionary<string, long> _parameterIndexes = new();
    private readonly Dictionary<string, bool> _parameterTargets = new();
    private readonly Dictionary<string, bool> _parameterRawValues = new();
    private readonly Dictionary<string, (bool CanImprove, bool CanBranch)> _parameterProperties = new();

    private readonly Dictionary<string, TraitExpressionExtendedOption> _options = new();

    public Expression Formula { get; protected set; }

    private Dictionary<string, TraitExpressionParameter> _parameters = new();

	public IReadOnlyDictionary<string, long> TraitReferenceIds
	{
		get
		{
			var names = Formula.ParameterNames.ToHashSet(StringComparer.Ordinal);
			var references = _parameters.Where(x => names.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value.Trait.Id);
			// A builder formula edit may replace an already loaded parameter. The pending
			// native parse takes precedence, just as it does in ProcessLazyLoading.
			foreach (var reference in _parameterIndexes)
				if (names.Contains(reference.Key)) references[reference.Key] = reference.Value;
			return references;
		}
	}

    public Dictionary<string, TraitExpressionParameter> Parameters
    {
        get
        {
            ProcessLazyLoading();
            return _parameters;
        }
    }

    public IEnumerable<string> NonTraitParameters => Formula.ParameterNames.Except(Parameters.Keys);

    private void ProcessLazyLoading()
    {
        if (_parameterIndexes.Any())
        {
            foreach (KeyValuePair<string, long> param in _parameterIndexes)
            {
                TraitExpressionParameter newParam = new()
                {
                    Trait = Gameworld.Traits.Get(param.Value),
                    IsTarget = _parameterTargets.GetValueOrDefault(param.Key),
                    UseRawValue = _parameterRawValues.GetValueOrDefault(param.Key)
                };
                (newParam.CanImprove, newParam.CanBranch) = _parameterProperties[param.Key];
                _parameters[param.Key] = newParam;
            }

            _parameterIndexes.Clear();
            _parameterProperties.Clear();
            _parameterTargets.Clear();
            _parameterRawValues.Clear();
        }
    }

    public virtual double Evaluate(IHaveTraits owner, ITraitDefinition variable = null,
        TraitBonusContext context = TraitBonusContext.None)
    {
        return EvaluateInternal(owner, null, variable, context, false, Array.Empty<(string Name, object Value)>());
    }

    public virtual double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null,
        TraitBonusContext context = TraitBonusContext.None, params (string Name, object Value)[] values)
    {
        return EvaluateInternal(owner, null, variable, context, false, values);
    }

    public double EvaluateMax(IHaveTraits owner)
    {
        return EvaluateInternal(owner, null, null, TraitBonusContext.None, true, Array.Empty<(string Name, object Value)>());
    }

    /// <summary>Target traits are read-only inputs; no target means zero, including numerical capture.</summary>
    public virtual double EvaluateWithTarget(IHaveTraits owner, IHaveTraits target, ITraitDefinition variable = null,
        TraitBonusContext context = TraitBonusContext.None, params (string Name, object Value)[] values) =>
        EvaluateInternal(owner, target, variable, context, false, values);

    private double EvaluateInternal(IHaveTraits owner, IHaveTraits target, ITraitDefinition variable, TraitBonusContext context,
        bool maximum, IEnumerable<(string Name, object Value)> values)
    {
        ProcessLazyLoading();
        Dictionary<string, object> evaluationValues = new(StringComparer.OrdinalIgnoreCase);
        foreach ((string Name, object Value) value in values)
        {
            evaluationValues[value.Name] = value.Value;
        }

        foreach (KeyValuePair<string, TraitExpressionParameter> param in Parameters)
        {
            var subject = param.Value.IsTarget ? target : owner;
            evaluationValues[param.Key] = subject is null ? 0.0 : maximum
                ? subject.TraitMaxValue(param.Value.Trait)
                : param.Value.UseRawValue ? subject.TraitRawValue(param.Value.Trait) : subject.TraitValue(param.Value.Trait, context);
        }

        evaluationValues["variable"] = variable is null || maximum
            ? 0.0
            : owner.TraitValue(variable, context);

        foreach (KeyValuePair<string, TraitExpressionExtendedOption> option in _options)
        {
            evaluationValues[option.Key] = option.Value.Evaluate(owner);
        }

#if DEBUG
#else
	try {
#endif
        return Formula.EvaluateDoubleWith(evaluationValues);
#if DEBUG
#else
	}
	catch (Exception e)
	{
		Console.WriteLine($"Exception in TraitExpression #{Id.ToString("N0")} ({Formula.OriginalExpression}):\n\n{e.Message}");
		return 0.0;
	}
#endif
    }

	/// <summary>Captures only owner bindings, never evaluates the formula, randomness or gameplay effects.</summary>
	public IEnumerable<string> NumericalBindingNames => Parameters.Keys.Concat(_options.Keys).Append("variable").Distinct(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyDictionary<string, double> CaptureNumericalBindings(IHaveTraits owner, ITraitDefinition variable,
		TraitBonusContext context)
	{
		ProcessLazyLoading();
		if (Parameters.Values.Any(x => x.Trait is null)) throw new InvalidOperationException("A numerical expression references a missing trait.");
		var bindings = Parameters.ToDictionary(x => x.Key, x => x.Value.IsTarget ? 0.0 : x.Value.UseRawValue ? owner.TraitRawValue(x.Value.Trait) : owner.TraitValue(x.Value.Trait, context), StringComparer.OrdinalIgnoreCase);
		bindings["variable"] = variable is null ? 0.0 : owner.TraitValue(variable, context);
		foreach (var (name, option) in _options) bindings[name] = option.Evaluate(owner);
		if (bindings.Values.Any(x => !double.IsFinite(x))) throw new InvalidOperationException("A creator numerical binding is not finite.");
		return bindings.AsReadOnly();
	}

    public override void Save()
    {
        ProcessLazyLoading();
        Models.TraitExpression dbitem = FMDB.Context.TraitExpressions.Find(Id);
        if (dbitem is null)
        {
            Changed = false;
            return;
        }

        dbitem.Name = Name;
        dbitem.Expression = OriginalFormulaText;
        FMDB.Context.TraitExpressionParameters.RemoveRange(dbitem.TraitExpressionParameters);
        foreach (KeyValuePair<string, TraitExpressionParameter> parameter in Parameters)
        {
            TraitExpressionParameters dbparameter = new();
            dbitem.TraitExpressionParameters.Add(dbparameter);
            dbparameter.CanBranch = parameter.Value.CanBranch;
            dbparameter.CanImprove = parameter.Value.CanImprove;
            dbparameter.Parameter = parameter.Key;
            dbparameter.TraitDefinitionId = parameter.Value.Trait.Id;
        }

        Changed = false;
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
		var result = BuildingCommandCore(actor, command);
		if (result && Gameworld.MagicResources.OfType<MudSharp.Magic.Resources.SimpleMagicResource>()
			.Any(x => x.AttributeCapacity?.ExpressionId == Id))
		{
			foreach (var character in Gameworld.Characters.DistinctBy(x => MudSharp.Magic.Casting.MagicCastingService.Owner(x).Id))
				Gameworld.MagicCasting?.NotifyCapacityChange(character);
		}
		return result;
    }

	private bool BuildingCommandCore(ICharacter actor, StringStack command)
    {
        ProcessLazyLoading();
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "formula":
                return BuildingCommandFormula(actor, command);
            case "parameter":
            case "param":
                return BuildingCommandParameter(actor, command);
            case "branch":
                return BuildingCommandBranch(actor, command);
            case "improve":
                return BuildingCommandImprove(actor, command);
            case "trait":
                return BuildingCommandTrait(actor, command);
            case "delete":
            case "del":
            case "rem":
            case "remove":
                return BuildingCommandDelete(actor, command);
        }

        actor.OutputHandler.Send(
            @"Valid options are as follows:

	#3name <name>#0 - the name of this trait expression
	#3formula <formula>#0 - the formula; name:id reads self, name:!id reads the target
	#3parameter <which> <trait|!trait>#0 - binds a self or read-only target trait (target defaults to zero)
	#3branch <which>#0 - toggles branching of an existing parameter
	#3improve <which>#0 - toggles improvement of a particular parameter
	#3trait <which> <trait>#0 - sets the trait for a particular parameter
	#3delete <which>#0 - deletes a particular parameter".SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandDelete(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which parameter do you want to remove?");
            return false;
        }

        string which = command.PopSpeech();
        if (!Parameters.Any(x => x.Key.EqualTo(which)))
        {
            actor.OutputHandler.Send("There is no such parameter.");
            return false;
        }

        RemoveInlineParameter(which.ToLowerInvariant());
        Parameters.Remove(which.ToLowerInvariant());
        Changed = true;
        actor.OutputHandler.Send($"You remove the {which.ToLowerInvariant().Colour(Telnet.Cyan)} parameter.");
        return true;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this parameter expression?");
            return false;
        }

        string name = command.SafeRemainingArgument;
        if (Gameworld.TraitExpressions.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                "There is already a trait expression with that name. Non-default names must be unique.");
            return false;
        }

        _name = name;
        Changed = true;
        actor.OutputHandler.Send($"This trait expression is now named {_name.Colour(Telnet.Cyan)}.");
        return true;
    }

    private bool BuildingCommandParameter(ICharacter actor, StringStack command)
    {
        string syntaxText =
            $"The syntax is {"traitexpression set parameter <whichparameter> <trait>".ColourCommand()} or {"traitexpression set parameter delete <whichparameter>".ColourCommand()}.";
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(syntaxText);
            return false;
        }

        if (command.Peek().EqualTo("delete"))
        {
            command.PopSpeech();
            if (command.IsFinished)
            {
                actor.OutputHandler.Send(syntaxText);
                return false;
            }

            string which = command.PopSpeech().ToLowerInvariant();
            if (!Parameters.ContainsKey(which))
            {
                actor.OutputHandler.Send("There is no such parameter to delete.");
                return false;
            }

            RemoveInlineParameter(which);
            Parameters.Remove(which);
            Changed = true;
            actor.OutputHandler.Send($"You delete the parameter {which}.");
            return true;
        }

        string text = command.PopSpeech().ToLowerInvariant();

        if (text.EqualTo("variable"))
        {
            actor.OutputHandler.Send(
                "You cannot give a parameter a name of variable, as that serves a special function in the trait expression.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which trait should that parameter refer to?");
            return false;
        }

        var traitText = command.PopSpeech();
        var isTarget = traitText.StartsWith('!');
        if (isTarget) traitText = traitText[1..];
        ITraitDefinition trait = long.TryParse(traitText, out long value)
            ? Gameworld.Traits.Get(value)
            : Gameworld.Traits.GetByName(traitText);
        if (trait == null)
        {
            actor.OutputHandler.Send("There is no such trait.");
            return false;
        }

        if (isTarget && !Formula.ParameterNames.Any(x => x.EqualTo(text)))
        {
            actor.OutputHandler.Send("Add this parameter to the formula before binding a target trait, or use name:!id directly in the formula.");
            return false;
        }

        bool canimprove = true, canbranch = true, useRawValue = false;
        if (Parameters.ContainsKey(text))
        {
            TraitExpressionParameter old = Parameters[text];
            canimprove = old.CanImprove;
            canbranch = old.CanBranch;
            useRawValue = old.UseRawValue;
        }

        Parameters[text] = new TraitExpressionParameter
        {
            Trait = trait,
            IsTarget = isTarget,
            UseRawValue = useRawValue,
            CanImprove = !isTarget && canimprove,
            CanBranch = !isTarget && canbranch
        };
        SynchronizeInlineParameter(text);
        Changed = true;
        actor.OutputHandler.Send(
            $"The parameter {text} now refers to {(isTarget ? "the target's " : "")}trait {trait.Name.Colour(Telnet.Cyan)}, {(Parameters[text].CanBranch ? "can" : "cannot")} branch and {(Parameters[text].CanImprove ? "can" : "cannot")} improve.");
        return true;
    }

    // Target scope is encoded in the existing formula text, including parameters added via the editor.
    private void SynchronizeInlineParameter(string name)
    {
        var parameter = Parameters[name];
        var flags = new List<string>();
        if (parameter.UseRawValue) flags.Add("raw");
        if (!parameter.CanImprove) flags.Add("noimprove");
        if (!parameter.CanBranch) flags.Add("nobranch");
        var binding = $"{name}:{(parameter.IsTarget ? "!" : "")}{parameter.Trait.Id}" +
                      (flags.Count > 0 ? $"{{{string.Join(",", flags)}}}" : "");
        var found = false;
        OriginalFormulaText = TraitFormulaRegex.Replace(OriginalFormulaText, match =>
        {
            if (!match.Groups["name"].Value.Equals(name, StringComparison.OrdinalIgnoreCase)) return match.Value;
            found = true;
            return binding;
        });
        if ((parameter.IsTarget || parameter.UseRawValue) && !found)
            OriginalFormulaText = Regex.Replace(OriginalFormulaText, $@"\b{Regex.Escape(name)}\b", _ => binding,
                RegexOptions.IgnoreCase);
    }

    private void RemoveInlineParameter(string name) =>
        OriginalFormulaText = TraitFormulaRegex.Replace(OriginalFormulaText, match =>
            match.Groups["name"].Value.Equals(name, StringComparison.OrdinalIgnoreCase)
                ? match.Groups["name"].Value : match.Value);

    private bool BuildingCommandFormula(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What formula do you want to give to this trait expression?");
            return false;
        }

        var candidate = new TraitExpression(command.SafeRemainingArgument, Gameworld);
        if (candidate.HasErrors())
        {
            actor.OutputHandler.Send($"Your formula has errors: {candidate.Error.ColourCommand()}");
            return false;
        }

        foreach (var (name, id) in candidate._parameterIndexes)
        {
            _parameterIndexes[name] = id;
            _parameterProperties[name] = candidate._parameterProperties[name];
            _parameterTargets[name] = candidate._parameterTargets[name];
            _parameterRawValues[name] = candidate._parameterRawValues[name];
        }
        foreach (var (name, option) in candidate._options) _options[name] = option;
        var newFormula = candidate.Formula;
        Formula = newFormula;
        OriginalFormulaText = command.SafeRemainingArgument;
        ProcessLazyLoading();
        foreach (var name in Parameters.Where(x => x.Value.IsTarget || x.Value.UseRawValue).Select(x => x.Key).ToArray())
        {
            if (!Formula.ParameterNames.Any(x => x.EqualTo(name))) Parameters.Remove(name);
            else SynchronizeInlineParameter(name);
        }
        Changed = true;
        actor.OutputHandler.Send($"You set the formula for this trait expression to: {OriginalFormulaText.ColourCommand()}");
        return true;
    }

    private bool BuildingCommandBranch(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which parameter do you want to toggle branching for?");
            return false;
        }

        string which = command.PopSpeech().ToLowerInvariant();
        if (!Parameters.ContainsKey(which))
        {
            actor.OutputHandler.Send("There is no such parameter.");
            return false;
        }

        if (Parameters[which].IsTarget)
        {
            actor.OutputHandler.Send("Target trait parameters are read-only and cannot improve or branch.");
            return false;
        }

        Parameters[which].CanBranch = !Parameters[which].CanBranch;
        SynchronizeInlineParameter(which);
        actor.OutputHandler.Send(
            $"The parameter {which.Colour(Telnet.Cyan)} will {(Parameters[which].CanBranch ? "now" : "no longer")} permit branching of the {Parameters[which].Trait.Name.Colour(Telnet.Green)} trait.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandImprove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which parameter do you want to toggle improvement for?");
            return false;
        }

        string which = command.PopSpeech().ToLowerInvariant();
        if (!Parameters.ContainsKey(which))
        {
            actor.OutputHandler.Send("There is no such parameter.");
            return false;
        }

        if (Parameters[which].IsTarget)
        {
            actor.OutputHandler.Send("Target trait parameters are read-only and cannot improve or branch.");
            return false;
        }

        Parameters[which].CanImprove = !Parameters[which].CanImprove;
        SynchronizeInlineParameter(which);
        actor.OutputHandler.Send(
            $"The parameter {which.Colour(Telnet.Cyan)} will {(Parameters[which].CanImprove ? "now" : "no longer")} permit improvement of the {Parameters[which].Trait.Name.Colour(Telnet.Green)} trait.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandTrait(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which parameter do you want to set the trait for?");
            return false;
        }

        string which = command.PopSpeech().ToLowerInvariant();
        if (!Parameters.ContainsKey(which))
        {
            actor.OutputHandler.Send("There is no such parameter.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which trait do you want to set for that parameter?");
            return false;
        }

        var traitText = command.PopSpeech();
        var isTarget = traitText.StartsWith('!');
        if (isTarget) traitText = traitText[1..];
        ITraitDefinition trait = long.TryParse(traitText, out long value)
            ? Gameworld.Traits.Get(value)
            : Gameworld.Traits.GetByName(traitText);
        if (trait == null)
        {
            actor.OutputHandler.Send("There is no such trait.");
            return false;
        }

        if ((isTarget || Parameters[which].IsTarget || Parameters[which].UseRawValue) && !Formula.ParameterNames.Any(x => x.EqualTo(which)))
        {
            actor.OutputHandler.Send("Add this parameter to the formula before binding a target trait, or use name:!id directly in the formula.");
            return false;
        }

        Parameters[which].Trait = trait;
        if (isTarget)
        {
            Parameters[which].IsTarget = true;
            Parameters[which].CanImprove = Parameters[which].CanBranch = false;
        }
        SynchronizeInlineParameter(which);
        Changed = true;
        actor.OutputHandler.Send(
            $"The parameter {which.Colour(Telnet.Cyan)} now refers to the {trait.Name.Colour(Telnet.Green)} trait.");
        return true;
    }

    public string ShowBuilder(ICharacter actor)
    {
        ProcessLazyLoading();
        StringBuilder sb = new();
        sb.AppendLine($"Trait Expression #{Id.ToString("N0", actor)}: {Name.Colour(Telnet.Cyan)}");
        sb.AppendLine($"Formula: {OriginalFormulaText.ColourCommand()}");
        if (Parameters.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Parameters:");
            foreach (KeyValuePair<string, TraitExpressionParameter> item in Parameters)
            {
                sb.AppendLine(
                    $"\t{item.Key}: {item.Value.Trait.Name.Colour(Telnet.Green)}{(item.Value.IsTarget ? " [target]".ColourName() : "")}{(item.Value.UseRawValue ? " [raw]".ColourName() : "")}{(item.Value.CanBranch ? "" : " [no_branch]".Colour(Telnet.Red))}{(item.Value.CanImprove ? "" : " [no_improve]".Colour(Telnet.Red))}");
            }
        }

        return sb.ToString();
    }

    public bool HasErrors()
    {
        return Formula.HasErrors();
    }

    public string Error => Formula.Error;

    public string OriginalFormulaText { get; private set; }
}
