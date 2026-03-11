using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Framework.Save;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using NumberStyles = System.Globalization.NumberStyles;
using SystemCultureInfo = System.Globalization.CultureInfo;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Health.Strategies;

public abstract class BaseHealthStrategy : SaveableItem, IHealthStrategy
{
	private static readonly Dictionary<string, Func<HealthStrategy, IFuturemud, IHealthStrategy>> _databaseLoaders = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, Func<IFuturemud, string, IHealthStrategy>> _builderLoaders = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, string> _typeHelps = new(StringComparer.InvariantCultureIgnoreCase);
	private static readonly Dictionary<string, string> _typeBlurbs = new(StringComparer.InvariantCultureIgnoreCase);

	private static readonly IReadOnlyList<string> CommonBuilderHelpLines =
	[
		"#3name <name>#0 - renames this health strategy",
		"#3lodge <expression>#0 - sets the expression used for the 1d100 lodge check"
	];

	protected Expression LodgeDamageExpression = new("0");
	protected RankedRange<WoundSeverity> SeverityRanges = new();
	protected RankedRange<WoundSeverity> PercentageSeverityRanges = new();

	protected sealed record TraitExpressionBuilderField<TStrategy>(
		string ElementName,
		IReadOnlyList<string> Commands,
		string DisplayName,
		Func<TStrategy, ITraitExpression> Getter,
		Action<TStrategy, ITraitExpression> Setter)
		where TStrategy : BaseHealthStrategy;

	protected sealed record DoubleBuilderField<TStrategy>(
		string ElementName,
		IReadOnlyList<string> Commands,
		string DisplayName,
		Func<TStrategy, double> Getter,
		Action<TStrategy, double> Setter,
		Func<ICharacter, double, string>? Formatter = null,
		Func<TStrategy, double, (bool Success, string Error, double Value)>? Validator = null,
		Func<ICharacter, string, (bool Success, string Error, double Value)>? Parser = null,
		string InputTypeDescription = "value")
		where TStrategy : BaseHealthStrategy;

	protected sealed record IntBuilderField<TStrategy>(
		string ElementName,
		IReadOnlyList<string> Commands,
		string DisplayName,
		Func<TStrategy, int> Getter,
		Action<TStrategy, int> Setter,
		Func<ICharacter, int, string>? Formatter = null,
		Func<TStrategy, int, (bool Success, string Error, int Value)>? Validator = null)
		where TStrategy : BaseHealthStrategy;

	protected sealed record BoolBuilderField<TStrategy>(
		string ElementName,
		IReadOnlyList<string> Commands,
		string DisplayName,
		Func<TStrategy, bool> Getter,
		Action<TStrategy, bool> Setter)
		where TStrategy : BaseHealthStrategy;

	protected sealed record TimeSpanBuilderField<TStrategy>(
		string ElementName,
		IReadOnlyList<string> Commands,
		string DisplayName,
		Func<TStrategy, TimeSpan> Getter,
		Action<TStrategy, TimeSpan> Setter,
		Func<TStrategy, TimeSpan, (bool Success, string Error, TimeSpan Value)>? Validator = null)
		where TStrategy : BaseHealthStrategy;

	protected BaseHealthStrategy()
	{
		SetSimpleDefaultSeverityRanges();
	}

	protected BaseHealthStrategy(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		SetSimpleDefaultSeverityRanges();
	}

	protected BaseHealthStrategy(HealthStrategy strategy, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = strategy.Id;
		_name = strategy.Name;
		SetSimpleDefaultSeverityRanges();
		LoadDefinition(XElement.Parse(strategy.Definition));
	}

	protected BaseHealthStrategy(BaseHealthStrategy rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		LodgeDamageExpression = new Expression(rhs.LodgeDamageExpression.OriginalExpression);
		CopySeverityRangesFrom(rhs);
	}

	public sealed override string FrameworkItemType => "HealthStrategy";

	public abstract string HealthStrategyType { get; }
	public abstract HealthStrategyOwnerType OwnerType { get; }
	public abstract IHealthStrategy Clone(string name);

	public string HelpInfo => BuildBuilderHelp(SubtypeBuilderHelpText);

	protected virtual IEnumerable<string> SubtypeBuilderHelpText => Enumerable.Empty<string>();

	protected static void RegisterHealthStrategy(string type,
		Func<HealthStrategy, IFuturemud, IHealthStrategy> databaseLoader,
		Func<IFuturemud, string, IHealthStrategy> builderLoader,
		string typeHelp,
		string typeBlurb)
	{
		_databaseLoaders[type] = databaseLoader;
		_builderLoaders[type] = builderLoader;
		_typeHelps[type] = typeHelp;
		_typeBlurbs[type] = typeBlurb;
	}

	public static void SetupHealthStrategies()
	{
		foreach (var type in Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(BaseHealthStrategy))))
		{
			var method = type.GetMethod("RegisterHealthStrategyLoader", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, null);
		}
	}

	public static IEnumerable<string> Types => _typeBlurbs.Keys.OrderBy(x => x).ToList();

	public static (string Blurb, string Help) GetTypeInfoFor(string type)
	{
		return _typeHelps.TryGetValue(type, out var help)
			? (_typeBlurbs[type], help)
			: ("That is not a valid health strategy type.", string.Empty);
	}

	public static IHealthStrategy LoadStrategy(HealthStrategy strategy, IFuturemud gameworld)
	{
		if (!_databaseLoaders.TryGetValue(strategy.Type, out var loader))
		{
			throw new ApplicationException(
				$"BaseHealthStrategy.LoadStrategy was asked to load HealthStrategy ID {strategy.Id} type {strategy.Type}, which is not a valid type.");
		}

		return loader(strategy, gameworld);
	}

	public static IHealthStrategy LoadStrategy(IFuturemud gameworld, string type, string name)
	{
		if (!_builderLoaders.TryGetValue(type, out var loader))
		{
			throw new ApplicationException(
				$"BaseHealthStrategy.LoadStrategy was asked to build a HealthStrategy of type {type}, which is not a valid type.");
		}

		return loader(gameworld, name);
	}

	protected static string BuildTypeHelp(string blurb, IEnumerable<string> subtypeLines)
	{
		var lines = subtypeLines.ToList();
		var sb = new StringBuilder();
		sb.AppendLine(blurb);
		sb.AppendLine();
		sb.Append(BuildBuilderHelp(lines));
		return sb.ToString().TrimEnd();
	}

	private static string BuildBuilderHelp(IEnumerable<string> subtypeLines)
	{
		var lines = subtypeLines.ToList();
		var sb = new StringBuilder();
		sb.AppendLine("You can use the following options with this command:");
		sb.AppendLine();
		foreach (var line in CommonBuilderHelpLines)
		{
			sb.AppendLine($"\t{line}");
		}

		foreach (var line in lines)
		{
			sb.AppendLine($"\t{line}");
		}

		return sb.ToString().TrimEnd();
	}

	protected static IEnumerable<string> GetBuilderFieldHelpText<TStrategy>(
		IEnumerable<TraitExpressionBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		return fields.Select(x => $"#3{x.Commands[0]} <id|name>#0 - sets the {x.DisplayName.ToLowerInvariant()}");
	}

	protected static IEnumerable<string> GetBuilderFieldHelpText<TStrategy>(
		IEnumerable<DoubleBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		return fields.Select(x =>
			$"#3{x.Commands[0]} <{x.InputTypeDescription}>#0 - sets the {x.DisplayName.ToLowerInvariant()}");
	}

	protected static DoubleBuilderField<TStrategy> PercentageField<TStrategy>(
		string elementName,
		IReadOnlyList<string> commands,
		string displayName,
		Func<TStrategy, double> getter,
		Action<TStrategy, double> setter,
		Func<TStrategy, double, (bool Success, string Error, double Value)>? validator = null)
		where TStrategy : BaseHealthStrategy
	{
		return new DoubleBuilderField<TStrategy>(
			elementName,
			commands,
			displayName,
			getter,
			setter,
			(actor, value) => value.ToString("P2", actor),
			validator,
			TryParsePercentage,
			"percentage");
	}

	protected static DoubleBuilderField<TStrategy> UnitField<TStrategy>(
		string elementName,
		IReadOnlyList<string> commands,
		string displayName,
		UnitType unitType,
		Func<TStrategy, double> getter,
		Action<TStrategy, double> setter,
		Func<TStrategy, double, (bool Success, string Error, double Value)>? validator = null,
		string inputTypeDescription = "amount")
		where TStrategy : BaseHealthStrategy
	{
		return new DoubleBuilderField<TStrategy>(
			elementName,
			commands,
			displayName,
			getter,
			setter,
			(actor, value) => actor.Gameworld.UnitManager.DescribeMostSignificantExact(value, unitType, actor),
			validator,
			(actor, text) => TryParseUnits(actor, text, unitType),
			inputTypeDescription);
	}

	protected static IEnumerable<string> GetBuilderFieldHelpText<TStrategy>(
		IEnumerable<IntBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		return fields.Select(x => $"#3{x.Commands[0]} <number>#0 - sets the {x.DisplayName.ToLowerInvariant()}");
	}

	protected static IEnumerable<string> GetBuilderFieldHelpText<TStrategy>(
		IEnumerable<BoolBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		return fields.Select(x => $"#3{x.Commands[0]} [true|false]#0 - toggles or sets the {x.DisplayName.ToLowerInvariant()}");
	}

	protected static IEnumerable<string> GetBuilderFieldHelpText<TStrategy>(
		IEnumerable<TimeSpanBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		return fields.Select(x => $"#3{x.Commands[0]} <seconds>#0 - sets the {x.DisplayName.ToLowerInvariant()}");
	}

	protected HealthStrategy DoDatabaseInsert(string type)
	{
		using (new FMDB())
		{
			var dbitem = new HealthStrategy
			{
				Name = Name,
				Type = type,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.HealthStrategies.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			return dbitem;
		}
	}

	private XElement SaveDefinition()
	{
		var root = new XElement("Definition",
			new XElement("LodgeDamageExpression", LodgeDamageExpression.OriginalExpression),
			new XElement("SeverityRanges",
				from range in SeverityRanges.Ranges
				let percentage = PercentageSeverityRanges.Ranges.FirstOrDefault(x => x.Value == range.Value)
				select new XElement("Severity",
					new XAttribute("value", (int)range.Value),
					new XAttribute("lower", range.LowerBound.ToString(SystemCultureInfo.InvariantCulture)),
					new XAttribute("upper", range.UpperBound.ToString(SystemCultureInfo.InvariantCulture)),
					new XAttribute("lowerperc", (percentage?.LowerBound ?? range.LowerBound / 100.0)
						.ToString(SystemCultureInfo.InvariantCulture)),
					new XAttribute("upperperc", (percentage?.UpperBound ?? range.UpperBound / 100.0)
						.ToString(SystemCultureInfo.InvariantCulture)))));
		SaveSubtypeDefinition(root);
		return root;
	}

	protected abstract void SaveSubtypeDefinition(XElement root);

	public override void Save()
	{
		var dbitem = FMDB.Context.HealthStrategies.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Name = Name;
		dbitem.Type = HealthStrategyType;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	private void LoadDefinition(XElement root)
	{
		var element = root.Element("SeverityRanges");
		if (element != null)
		{
			SeverityRanges = new RankedRange<WoundSeverity>();
			PercentageSeverityRanges = new RankedRange<WoundSeverity>();
			foreach (var item in element.Elements("Severity"))
			{
				var lower = double.Parse(item.Attribute("lower")!.Value, SystemCultureInfo.InvariantCulture);
				var upper = double.Parse(item.Attribute("upper")!.Value, SystemCultureInfo.InvariantCulture);
				var severity = (WoundSeverity)int.Parse(item.Attribute("value")!.Value, SystemCultureInfo.InvariantCulture);
				SeverityRanges.Add(severity, lower, upper);
				var lowerPercentage = item.Attribute("lowerperc")?.Value ?? item.Attribute("lowerpec")?.Value;
				var upperPercentage = item.Attribute("upperperc")?.Value;
				if (!string.IsNullOrWhiteSpace(lowerPercentage) && !string.IsNullOrWhiteSpace(upperPercentage))
				{
					PercentageSeverityRanges.Add(severity,
						double.Parse(lowerPercentage, SystemCultureInfo.InvariantCulture),
						double.Parse(upperPercentage, SystemCultureInfo.InvariantCulture));
				}
				else
				{
					PercentageSeverityRanges.Add(severity, lower / 100.0, upper / 100.0);
				}
			}
		}

		element = root.Element("LodgeDamageExpression");
		LodgeDamageExpression = element != null ? new Expression(element.Value) : new Expression("0");
	}

	protected static double LoadDouble(XElement root, string elementName, double defaultValue)
	{
		var value = root.Element(elementName)?.Value;
		if (string.IsNullOrWhiteSpace(value))
		{
			return defaultValue;
		}

		if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands,
			    SystemCultureInfo.InvariantCulture,
			    out var result))
		{
			return result;
		}

		if (double.TryParse(value, out result))
		{
			return result;
		}

		throw new ApplicationException(
			$"BaseHealthStrategy could not parse element {elementName} value '{value}' as a double.");
	}

	protected static int LoadInt(XElement root, string elementName, int defaultValue)
	{
		var value = root.Element(elementName)?.Value;
		if (string.IsNullOrWhiteSpace(value))
		{
			return defaultValue;
		}

		if (int.TryParse(value, NumberStyles.Integer, SystemCultureInfo.InvariantCulture, out var result))
		{
			return result;
		}

		if (int.TryParse(value, out result))
		{
			return result;
		}

		throw new ApplicationException(
			$"BaseHealthStrategy could not parse element {elementName} value '{value}' as an integer.");
	}

	protected static bool LoadBool(XElement root, string elementName, bool defaultValue)
	{
		var value = root.Element(elementName)?.Value;
		if (string.IsNullOrWhiteSpace(value))
		{
			return defaultValue;
		}

		if (bool.TryParse(value, out var result))
		{
			return result;
		}

		throw new ApplicationException(
			$"BaseHealthStrategy could not parse element {elementName} value '{value}' as a boolean.");
	}

	protected static TimeSpan LoadTimeSpanFromSeconds(XElement root, string elementName, double defaultSeconds)
	{
		return TimeSpan.FromSeconds(LoadDouble(root, elementName, defaultSeconds));
	}

	protected static ITraitExpression CreateDefaultExpression(IFuturemud gameworld, string name, string formula)
	{
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.TraitExpression
			{
				Name = name,
				Expression = formula
			};
			FMDB.Context.TraitExpressions.Add(dbitem);
			FMDB.Context.SaveChanges();
			var expression = new TraitExpression(dbitem, gameworld);
			gameworld.Add(expression);
			return expression;
		}
	}

	protected static ITraitExpression CloneExpression(ITraitExpression expression, IFuturemud gameworld)
	{
		var clone = new TraitExpression((TraitExpression)expression);
		gameworld.Add(clone);
		return clone;
	}

	protected void SetSimpleDefaultSeverityRanges()
	{
		SetSeverityRanges(
		[
			(WoundSeverity.None, -1.0, 0.0, -0.01, 0.0),
			(WoundSeverity.Superficial, 0.0, 2.0, 0.0, 0.02),
			(WoundSeverity.Minor, 2.0, 4.0, 0.02, 0.04),
			(WoundSeverity.Small, 4.0, 7.0, 0.04, 0.07),
			(WoundSeverity.Moderate, 7.0, 12.0, 0.07, 0.12),
			(WoundSeverity.Severe, 12.0, 18.0, 0.12, 0.18),
			(WoundSeverity.VerySevere, 18.0, 27.0, 0.18, 0.27),
			(WoundSeverity.Grievous, 27.0, 40.0, 0.27, 0.40),
			(WoundSeverity.Horrifying, 40.0, 100.0, 0.40, 1.0)
		]);
	}

	protected void SetComplexDefaultSeverityRanges()
	{
		SetSeverityRanges(
		[
			(WoundSeverity.None, -2.0, -1.0, -1.0, 0.0),
			(WoundSeverity.Superficial, -1.0, 2.0, 0.0, 0.4),
			(WoundSeverity.Minor, 2.0, 4.0, 0.4, 0.55),
			(WoundSeverity.Small, 4.0, 7.0, 0.55, 0.65),
			(WoundSeverity.Moderate, 7.0, 12.0, 0.65, 0.75),
			(WoundSeverity.Severe, 12.0, 18.0, 0.75, 0.85),
			(WoundSeverity.VerySevere, 18.0, 27.0, 0.85, 0.9),
			(WoundSeverity.Grievous, 27.0, 40.0, 0.9, 0.95),
			(WoundSeverity.Horrifying, 40.0, 100.0, 0.95, 1.0)
		]);
	}

	private void SetSeverityRanges(IEnumerable<(WoundSeverity Severity, double Lower, double Upper, double LowerPercentage, double UpperPercentage)> ranges)
	{
		SeverityRanges = new RankedRange<WoundSeverity>();
		PercentageSeverityRanges = new RankedRange<WoundSeverity>();
		foreach (var range in ranges)
		{
			SeverityRanges.Add(range.Severity, range.Lower, range.Upper);
			PercentageSeverityRanges.Add(range.Severity, range.LowerPercentage, range.UpperPercentage);
		}
	}

	private void CopySeverityRangesFrom(BaseHealthStrategy rhs)
	{
		SeverityRanges = new RankedRange<WoundSeverity>();
		PercentageSeverityRanges = new RankedRange<WoundSeverity>();
		foreach (var range in rhs.SeverityRanges.Ranges)
		{
			SeverityRanges.Add(range.Value, range.LowerBound, range.UpperBound);
		}

		foreach (var range in rhs.PercentageSeverityRanges.Ranges)
		{
			PercentageSeverityRanges.Add(range.Value, range.LowerBound, range.UpperBound);
		}
	}

	protected bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this health strategy?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.HealthStrategies.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a health strategy called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this health strategy from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	protected bool BuildingCommandLodge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What expression should determine the lodge check threshold for a 1d100 roll?");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send($"That is not a valid expression: {expression.Error.ColourError()}");
			return false;
		}

		LodgeDamageExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This health strategy now uses the lodge check {DescribeLodgeDamageCheck().ColourCommand()}.");
		return true;
	}

	protected bool CheckDamageLodges(IDamage damage)
	{
		if (!damage.DamageType.CanLodge())
		{
			return false;
		}

		LodgeDamageExpression.Parameters["damage"] = damage.DamageAmount;
		LodgeDamageExpression.Parameters["type"] = (int)damage.DamageType;
		return Dice.Roll(1, 100) < Convert.ToDouble(LodgeDamageExpression.Evaluate());
	}

	protected string DescribeLodgeDamageCheck()
	{
		return $"1d100 vs {LodgeDamageExpression.OriginalExpression}";
	}

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "lodge":
			case "lodgeexpression":
				return BuildingCommandLodge(actor, command);
		}

		actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
		return false;
	}

	protected bool TryBuildingCommand<TStrategy>(ICharacter actor, StringStack command,
		IEnumerable<TraitExpressionBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		var keyword = command.PopForSwitch();
		var field = fields.FirstOrDefault(x => x.Commands.Any(y => y.EqualTo(keyword)));
		if (field is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which trait expression should be used for the {field.DisplayName.ToLowerInvariant()}?");
			return false;
		}

		var expression = Gameworld.TraitExpressions.GetByIdOrName(command.SafeRemainingArgument);
		if (expression is null)
		{
			actor.OutputHandler.Send($"There is no trait expression identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		field.Setter((TStrategy)this, expression);
		Changed = true;
		actor.OutputHandler.Send(
			$"This health strategy now uses trait expression #{expression.Id.ToString("N0", actor).ColourValue()} ({expression.Name.ColourName()}) for the {field.DisplayName.ToLowerInvariant()}.");
		return true;
	}

	protected bool TryBuildingCommand<TStrategy>(ICharacter actor, StringStack command,
		IEnumerable<DoubleBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		var keyword = command.PopForSwitch();
		var field = fields.FirstOrDefault(x => x.Commands.Any(y => y.EqualTo(keyword)));
		if (field is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What {field.InputTypeDescription} should be used for the {field.DisplayName.ToLowerInvariant()}?");
			return false;
		}

		double value;
		if (field.Parser is not null)
		{
			var parse = field.Parser(actor, command.SafeRemainingArgument);
			if (!parse.Success)
			{
				actor.OutputHandler.Send(parse.Error);
				return false;
			}

			value = parse.Value;
		}
		else if (!TryParseDouble(actor, command.SafeRemainingArgument, out value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (field.Validator is not null)
		{
			var validation = field.Validator((TStrategy)this, value);
			if (!validation.Success)
			{
				actor.OutputHandler.Send(validation.Error);
				return false;
			}

			value = validation.Value;
		}

		field.Setter((TStrategy)this, value);
		Changed = true;
		actor.OutputHandler.Send(
			$"This health strategy now uses {FormatValue(actor, field, value).ColourValue()} for the {field.DisplayName.ToLowerInvariant()}.");
		return true;
	}

	protected bool TryBuildingCommand<TStrategy>(ICharacter actor, StringStack command,
		IEnumerable<IntBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		var keyword = command.PopForSwitch();
		var field = fields.FirstOrDefault(x => x.Commands.Any(y => y.EqualTo(keyword)));
		if (field is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What number should be used for the {field.DisplayName.ToLowerInvariant()}?");
			return false;
		}

		if (!TryParseInt(actor, command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid whole number.");
			return false;
		}

		if (field.Validator is not null)
		{
			var validation = field.Validator((TStrategy)this, value);
			if (!validation.Success)
			{
				actor.OutputHandler.Send(validation.Error);
				return false;
			}

			value = validation.Value;
		}

		field.Setter((TStrategy)this, value);
		Changed = true;
		actor.OutputHandler.Send(
			$"This health strategy now uses {FormatValue(actor, field, value).ColourValue()} for the {field.DisplayName.ToLowerInvariant()}.");
		return true;
	}

	protected bool TryBuildingCommand<TStrategy>(ICharacter actor, StringStack command,
		IEnumerable<BoolBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		var keyword = command.PopForSwitch();
		var field = fields.FirstOrDefault(x => x.Commands.Any(y => y.EqualTo(keyword)));
		if (field is null)
		{
			return false;
		}

		var value = field.Getter((TStrategy)this);
		if (command.IsFinished)
		{
			value = !value;
		}
		else if (!TryParseBoolean(command.SafeRemainingArgument, out value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid boolean value.");
			return false;
		}

		field.Setter((TStrategy)this, value);
		Changed = true;
		actor.OutputHandler.Send($"The {field.DisplayName.ToLowerInvariant()} is now {value.ToColouredString()}.");
		return true;
	}

	protected bool TryBuildingCommand<TStrategy>(ICharacter actor, StringStack command,
		IEnumerable<TimeSpanBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		var keyword = command.PopForSwitch();
		var field = fields.FirstOrDefault(x => x.Commands.Any(y => y.EqualTo(keyword)));
		if (field is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How many seconds should be used for the {field.DisplayName.ToLowerInvariant()}?");
			return false;
		}

		if (!TryParseDouble(actor, command.SafeRemainingArgument, out var seconds))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number of seconds.");
			return false;
		}

		var value = TimeSpan.FromSeconds(seconds);
		if (field.Validator is not null)
		{
			var validation = field.Validator((TStrategy)this, value);
			if (!validation.Success)
			{
				actor.OutputHandler.Send(validation.Error);
				return false;
			}

			value = validation.Value;
		}

		field.Setter((TStrategy)this, value);
		Changed = true;
		actor.OutputHandler.Send($"The {field.DisplayName.ToLowerInvariant()} is now {value.Describe(actor).ColourValue()}.");
		return true;
	}

	protected static void SaveBuilderFields<TStrategy>(XElement root, TStrategy strategy,
		IEnumerable<TraitExpressionBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			root.Add(new XElement(field.ElementName, field.Getter(strategy).Id));
		}
	}

	protected static void SaveBuilderFields<TStrategy>(XElement root, TStrategy strategy,
		IEnumerable<DoubleBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			root.Add(new XElement(field.ElementName,
				field.Getter(strategy).ToString(SystemCultureInfo.InvariantCulture)));
		}
	}

	protected static void SaveBuilderFields<TStrategy>(XElement root, TStrategy strategy,
		IEnumerable<IntBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			root.Add(new XElement(field.ElementName, field.Getter(strategy)));
		}
	}

	protected static void SaveBuilderFields<TStrategy>(XElement root, TStrategy strategy,
		IEnumerable<BoolBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			root.Add(new XElement(field.ElementName, field.Getter(strategy)));
		}
	}

	protected static void SaveBuilderFields<TStrategy>(XElement root, TStrategy strategy,
		IEnumerable<TimeSpanBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			root.Add(new XElement(field.ElementName,
				field.Getter(strategy).TotalSeconds.ToString(SystemCultureInfo.InvariantCulture)));
		}
	}

	protected static void AppendBuilderFieldShow<TStrategy>(StringBuilder sb, ICharacter actor, TStrategy strategy,
		IEnumerable<TraitExpressionBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			var expression = field.Getter(strategy);
			sb.AppendLine(
				$"{field.DisplayName}: #{expression.Id.ToString("N0", actor).ColourValue()} ({expression.Name.ColourName()}) [{expression.OriginalFormulaText.ColourCommand()}]");
		}
	}

	protected static void AppendBuilderFieldShow<TStrategy>(StringBuilder sb, ICharacter actor, TStrategy strategy,
		IEnumerable<DoubleBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			sb.AppendLine($"{field.DisplayName}: {FormatValue(actor, field, field.Getter(strategy)).ColourValue()}");
		}
	}

	protected static void AppendBuilderFieldShow<TStrategy>(StringBuilder sb, ICharacter actor, TStrategy strategy,
		IEnumerable<IntBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			sb.AppendLine($"{field.DisplayName}: {FormatValue(actor, field, field.Getter(strategy)).ColourValue()}");
		}
	}

	protected static void AppendBuilderFieldShow<TStrategy>(StringBuilder sb, ICharacter actor, TStrategy strategy,
		IEnumerable<BoolBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			sb.AppendLine($"{field.DisplayName}: {field.Getter(strategy).ToColouredString()}");
		}
	}

	protected static void AppendBuilderFieldShow<TStrategy>(StringBuilder sb, ICharacter actor, TStrategy strategy,
		IEnumerable<TimeSpanBuilderField<TStrategy>> fields)
		where TStrategy : BaseHealthStrategy
	{
		foreach (var field in fields)
		{
			sb.AppendLine($"{field.DisplayName}: {field.Getter(strategy).Describe(actor).ColourValue()}");
		}
	}

	private static string FormatValue<TStrategy>(ICharacter actor, DoubleBuilderField<TStrategy> field, double value)
		where TStrategy : BaseHealthStrategy
	{
		return field.Formatter?.Invoke(actor, value) ?? value.ToString("N4", actor);
	}

	private static (bool Success, string Error, double Value) TryParsePercentage(ICharacter actor, string text)
	{
		var sanitised = text.Trim();
		foreach (var symbol in new[]
		         {
			         "%",
			         actor.Account.Culture.NumberFormat.PercentSymbol,
			         SystemCultureInfo.InvariantCulture.NumberFormat.PercentSymbol
		         }.Where(x => !string.IsNullOrEmpty(x)).Distinct())
		{
			sanitised = sanitised.Replace(symbol, string.Empty);
		}

		if (!TryParseDouble(actor, sanitised, out var value))
		{
			return (false, $"The text {text.ColourCommand()} is not a valid percentage.", 0.0);
		}

		return (true, string.Empty, value / 100.0);
	}

	private static (bool Success, string Error, double Value) TryParseUnits(ICharacter actor, string text, UnitType unitType)
	{
		if (actor.Gameworld.UnitManager.TryGetBaseUnits(text, unitType, actor, out var value))
		{
			return (true, string.Empty, value);
		}

		return (false,
			$"The text {text.ColourCommand()} is not a valid {unitType.DescribeEnum().ToLowerInvariant()}.",
			0.0);
	}

	private static string FormatValue<TStrategy>(ICharacter actor, IntBuilderField<TStrategy> field, int value)
		where TStrategy : BaseHealthStrategy
	{
		return field.Formatter?.Invoke(actor, value) ?? value.ToString("N0", actor);
	}

	private static bool TryParseBoolean(string text, out bool value)
	{
		switch (text.ToLowerInvariant())
		{
			case "true":
			case "yes":
			case "on":
			case "1":
				value = true;
				return true;
			case "false":
			case "no":
			case "off":
			case "0":
				value = false;
				return true;
			default:
				return bool.TryParse(text, out value);
		}
	}

	private static bool TryParseDouble(ICharacter actor, string text, out double value)
	{
		return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, SystemCultureInfo.InvariantCulture,
			       out value) ||
		       double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, actor.Account.Culture,
			       out value) ||
		       double.TryParse(text, out value);
	}

	private static bool TryParseInt(ICharacter actor, string text, out int value)
	{
		return int.TryParse(text, NumberStyles.Integer, SystemCultureInfo.InvariantCulture, out value) ||
		       int.TryParse(text, NumberStyles.Integer, actor.Account.Culture, out value) ||
		       int.TryParse(text, out value);
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Health Strategy #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan,
				Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {HealthStrategyType.ColourValue()}");
		sb.AppendLine($"Owner Type: {OwnerType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Requires Spinal Cord: {RequiresSpinalCord.ToColouredString()}");
		sb.AppendLine($"Kidney Function Active: {KidneyFunctionActive.ToColouredString()}");
		sb.AppendLine($"Lodge Damage Check: {DescribeLodgeDamageCheck().ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Severity Ranges:");
		sb.AppendLine(
			StringUtilities.GetTextTable(
				from range in SeverityRanges.Ranges
				let percentage = PercentageSeverityRanges.Ranges.FirstOrDefault(x => x.Value == range.Value)
				select new List<string>
				{
					range.Value.DescribeEnum(),
					range.LowerBound.ToString("N3", actor),
					range.UpperBound.ToString("N3", actor),
					(percentage?.LowerBound ?? range.LowerBound / 100.0).ToString("P2", actor),
					(percentage?.UpperBound ?? range.UpperBound / 100.0).ToString("P2", actor)
				},
				new List<string>
				{
					"Severity",
					"Min Damage",
					"Max Damage",
					"Min Percent",
					"Max Percent"
				},
				actor,
				Telnet.FunctionYellow));
		AppendSubtypeShow(sb, actor);
		return sb.ToString().TrimEnd();
	}

	protected virtual void AppendSubtypeShow(StringBuilder sb, ICharacter actor)
	{
	}

	#region IHealthStrategy Members

	public virtual bool RequiresSpinalCord => true;

	public virtual BodyTemperatureStatus CurrentTemperatureStatus(IHaveWounds owner)
	{
		return BodyTemperatureStatus.NormalTemperature;
	}

	public abstract IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart);

	public virtual void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture)
	{
	}

	public virtual void PerformBloodGain(IHaveWounds owner)
	{
	}

	public virtual WoundSeverity GetSeverity(double damage)
	{
		return SeverityRanges.Find(damage);
	}

	public virtual WoundSeverity GetSeverityFor(IWound wound, IHaveWounds owner)
	{
		var life = owner is ICharacter ch
			? ch.Body.HitpointsForBodypart(wound.Bodypart)
			: wound.Bodypart?.MaxLife ?? 1.0;
		if (life == 0.0)
		{
			life = 1.0;
		}

		if (wound is BoneFracture && wound.Bodypart is IBone bone)
		{
			life *= bone.BoneEffectiveHealthModifier;
		}

		if (wound.UseDamagePercentageSeverities)
		{
			return PercentageSeverityRanges.Find(wound.CurrentDamage / life);
		}

		return SeverityRanges.Find(wound.CurrentDamage);
	}

	public virtual double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel)
	{
		if (usePercentageModel)
		{
			return Math.Max(0, PercentageSeverityRanges.Ranges.First(x => x.Value == severity).LowerBound);
		}

		return Math.Max(0, SeverityRanges.Ranges.First(x => x.Value == severity).LowerBound);
	}

	public virtual double GetSeverityCeiling(WoundSeverity severity, bool usePercentageModel)
	{
		if (usePercentageModel)
		{
			return Math.Max(0, PercentageSeverityRanges.Ranges.First(x => x.Value == severity).UpperBound);
		}

		return Math.Max(1, SeverityRanges.Ranges.First(x => x.Value == severity).UpperBound);
	}

	public abstract HealthTickResult PerformHealthTick(IHaveWounds thing);
	public abstract HealthTickResult EvaluateStatus(IHaveWounds thing);
	public abstract string ReportConditionPrompt(IHaveWounds owner, PromptType type);

	public virtual double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type)
	{
		return 0;
	}

	public virtual double WoundPenaltyFor(IHaveWounds owner)
	{
		return 0;
	}

	public virtual bool KidneyFunctionActive => false;

	public virtual double CurrentHealthPercentage(IHaveWounds owner)
	{
		return 1.0 - Math.Min(1.0, owner.Wounds.Sum(x => x.CurrentDamage) / MaxHP(owner).IfZero(1.0));
	}

	public abstract double MaxHP(IHaveWounds owner);

	public virtual double MaxPain(IHaveWounds owner)
	{
		return 0;
	}

	public virtual double MaxStun(IHaveWounds owner)
	{
		return 0;
	}

	public virtual bool IsCriticallyInjured(IHaveWounds owner)
	{
		return false;
	}

	public virtual void PerformKidneyFunction(IBody owner)
	{
	}

	public virtual void PerformLiverFunction(IBody owner)
	{
	}

	public virtual void PerformSpleenFunction(IBody owner)
	{
	}

	#endregion
}
