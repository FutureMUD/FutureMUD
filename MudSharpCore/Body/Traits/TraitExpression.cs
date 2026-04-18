using ExpressionEngine;
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        new(@"(?<name>\w+):(?<reference>[0-9]+)(?:{(?<flags>[a-z,]+)})*",
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
            bool canImprove = true, canBranch = true;
            if (match.Groups["flags"].Length > 0)
            {
                foreach (string flag in match.Groups["flags"].Value.Split(','))
                {
                    switch (flag)
                    {
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
        }

        string expressionText = TraitFormulaRegex.Replace(expression.Expression, m => m.Groups["name"].Value);

        foreach (Match match in ExtendedOptionsRegex.Matches(expressionText))
        {
            switch (match.Groups["option"].Value.ToLowerInvariant())
            {
                case "race":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRaceOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "culture":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionCultureOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "role":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes =
                                new List<ChargenRoleType>(Enum.GetValues(typeof(ChargenRoleType))
                                                              .OfType<ChargenRoleType>())
                        };
                    break;
                case "class":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
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
                        new TraitExpressionMeritOption(match.Groups["value"].Value)
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
            bool canImprove = true, canBranch = true;
            if (match.Groups["flags"].Length > 0)
            {
                foreach (string flag in match.Groups["flags"].Value.Split(','))
                {
                    switch (flag)
                    {
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
        }

        expression = TraitFormulaRegex.Replace(expression, m => m.Groups["name"].Value);
        foreach (Match match in ExtendedOptionsRegex.Matches(expression))
        {
            switch (match.Groups["option"].Value.ToLowerInvariant())
            {
                case "race":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRaceOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "culture":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionCultureOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "role":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes =
                                new List<ChargenRoleType>(Enum.GetValues(typeof(ChargenRoleType))
                                                              .OfType<ChargenRoleType>())
                        };
                    break;
                case "class":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
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
                        new TraitExpressionMeritOption(match.Groups["value"].Value)
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
    private readonly Dictionary<string, (bool CanImprove, bool CanBranch)> _parameterProperties = new();

    private readonly Dictionary<string, TraitExpressionExtendedOption> _options = new();

    public Expression Formula { get; protected set; }

    private Dictionary<string, TraitExpressionParameter> _parameters = new();

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
                    Trait = Gameworld.Traits.Get(param.Value)
                };
                (newParam.CanImprove, newParam.CanBranch) = _parameterProperties[param.Key];
                _parameters[param.Key] = newParam;
            }

            _parameterIndexes.Clear();
            _parameterProperties.Clear();
        }
    }

    public double Evaluate(IHaveTraits owner, ITraitDefinition variable = null,
        TraitBonusContext context = TraitBonusContext.None)
    {
        ProcessLazyLoading();

        foreach (KeyValuePair<string, TraitExpressionParameter> param in Parameters)
        {
            Formula.Parameters[param.Key] = owner.TraitValue(param.Value.Trait, context);
        }

        if (variable != null)
        {
            Formula.Parameters["variable"] = owner.TraitValue(variable, context);
        }
        else
        {
            Formula.Parameters["variable"] = 0;
        }

        foreach (KeyValuePair<string, TraitExpressionExtendedOption> option in _options)
        {
            Formula.Parameters[option.Key] = option.Value.Evaluate(owner);
        }

#if DEBUG
#else
	try {
#endif
        return Convert.ToDouble(Formula.Evaluate());
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

    public double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null,
        TraitBonusContext context = TraitBonusContext.None, params (string Name, object Value)[] values)
    {
        ProcessLazyLoading();
        foreach ((string Name, object Value) value in values)
        {
            Formula.Parameters[value.Name] = value.Value;
        }

        foreach (KeyValuePair<string, TraitExpressionParameter> param in Parameters)
        {
            Formula.Parameters[param.Key] = owner.TraitValue(param.Value.Trait, context);
        }

        if (variable != null)
        {
            Formula.Parameters["variable"] = owner.TraitValue(variable, context);
        }
        else
        {
            Formula.Parameters["variable"] = 0;
        }

        foreach (KeyValuePair<string, TraitExpressionExtendedOption> option in _options)
        {
            Formula.Parameters[option.Key] = option.Value.Evaluate(owner);
        }

#if DEBUG
#else
	try {
#endif
        return Formula.EvaluateDouble();
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

    public double EvaluateMax(IHaveTraits owner)
    {
        ProcessLazyLoading();

        foreach (KeyValuePair<string, TraitExpressionParameter> param in Parameters)
        {
            Formula.Parameters[param.Key] = owner.TraitMaxValue(param.Value.Trait);
        }

        foreach (KeyValuePair<string, TraitExpressionExtendedOption> option in _options)
        {
            Formula.Parameters[option.Key] = option.Value.Evaluate(owner);
        }

#if DEBUG
#else
	try {
#endif

        return Convert.ToDouble(Formula.Evaluate());
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
	#3formula <formula>#0 - the formula for the expression
	#3parameter <which> <trait>#0 - adds a new parameter named referring to the specified trait
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

        ITraitDefinition trait = long.TryParse(command.PopSpeech(), out long value)
            ? Gameworld.Traits.Get(value)
            : Gameworld.Traits.GetByName(command.Last);
        if (trait == null)
        {
            actor.OutputHandler.Send("There is no such trait.");
            return false;
        }

        bool canimprove = true, canbranch = true;
        if (Parameters.ContainsKey(text))
        {
            TraitExpressionParameter old = Parameters[text];
            canimprove = old.CanImprove;
            canbranch = old.CanBranch;
        }

        Parameters[text] = new TraitExpressionParameter
        {
            Trait = trait,
            CanImprove = canimprove,
            CanBranch = canbranch
        };
        Changed = true;
        actor.OutputHandler.Send(
            $"The parameter {text} now refers to trait {trait.Name.Colour(Telnet.Cyan)}, {(canbranch ? "can" : "cannot")} branch and {(canimprove ? "can" : "cannot")} improve.");
        return true;
    }

    private bool BuildingCommandFormula(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What formula do you want to give to this trait expression?");
            return false;
        }

        string expression = command.SafeRemainingArgument;
        foreach (Match match in TraitFormulaRegex.Matches(expression))
        {
            _parameterIndexes[match.Groups["name"].Value] = long.Parse(match.Groups["reference"].Value);
            bool canImprove = true, canBranch = true;
            if (match.Groups["flags"].Length > 0)
            {
                foreach (string flag in match.Groups["flags"].Value.Split(','))
                {
                    switch (flag)
                    {
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
        }

        expression = TraitFormulaRegex.Replace(expression, m => m.Groups["name"].Value);

        foreach (Match match in ExtendedOptionsRegex.Matches(expression))
        {
            switch (match.Groups["option"].Value.ToLowerInvariant())
            {
                case "race":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRaceOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "culture":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionCultureOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                case "role":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
                        {
                            Name = match.Groups["name"].Value.ToLowerInvariant(),
                            _roleTypes =
                                new List<ChargenRoleType>(Enum.GetValues(typeof(ChargenRoleType))
                                                              .OfType<ChargenRoleType>())
                        };
                    break;
                case "class":
                    _options[match.Groups["name"].Value.ToLowerInvariant()] =
                        new TraitExpressionRoleOption(match.Groups["value"].Value)
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
                        new TraitExpressionMeritOption(match.Groups["value"].Value)
                        { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
                default:
                    _options[match.Groups["name"].Value.ToLowerInvariant()] = new TraitExpressionAlwaysZeroOption
                    { Name = match.Groups["name"].Value.ToLowerInvariant() };
                    break;
            }
        }

        expression = ExtendedOptionsRegex.Replace(expression, m => m.Groups["name"].Value.ToLowerInvariant());

        Expression newFormula = new(expression);
        if (newFormula.HasErrors())
        {
            actor.OutputHandler.Send($"Your formula has errors: {newFormula.Error.ColourCommand()}");
            return false;
        }

        Formula = newFormula;
        OriginalFormulaText = command.SafeRemainingArgument;
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

        Parameters[which].CanBranch = !Parameters[which].CanBranch;
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

        Parameters[which].CanImprove = !Parameters[which].CanImprove;
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

        ITraitDefinition trait = long.TryParse(command.PopSpeech(), out long value)
            ? Gameworld.Traits.Get(value)
            : Gameworld.Traits.GetByName(command.Last);
        if (trait == null)
        {
            actor.OutputHandler.Send("There is no such trait.");
            return false;
        }

        Parameters[which].Trait = trait;
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
                    $"\t{item.Key}: {item.Value.Trait.Name.Colour(Telnet.Green)}{(item.Value.CanBranch ? "" : " [no_branch]".Colour(Telnet.Red))}{(item.Value.CanImprove ? "" : " [no_improve]".Colour(Telnet.Red))}");
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