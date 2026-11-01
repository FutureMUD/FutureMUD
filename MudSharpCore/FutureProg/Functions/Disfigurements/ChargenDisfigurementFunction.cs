#nullable enable

using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.CharacterCreation;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.FutureProg.Functions.Disfigurements;

internal class ChargenDisfigurementFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly string _operation;

	public ChargenDisfigurementFunction(IList<IFunction> parameters, IFuturemud gameworld, string operation)
		: base(parameters)
	{
		_gameworld = gameworld;
		_operation = operation;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("addchargenmissingbodypart", [ProgVariableTypes.Chargen, ProgVariableTypes.Text],
			["chargen", "bodypart"], ["The chargen application", "Exact bodypart name or ID"],
			"Adds a severable missing bodypart. Returns false for invalid or already missing parts.");
		Register("addchargenprosthesis", [ProgVariableTypes.Chargen, ProgVariableTypes.ItemPrototype],
			["chargen", "prototype"], ["The chargen application", "The prosthetic item prototype"],
			"Adds a compatible prosthetic for a missing bodypart. Returns false for invalid or duplicate selections.");
		Register("addchargenscar", [ProgVariableTypes.Chargen, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text],
			["chargen", "bodypart", "damage", "severity"],
			["The chargen application", "Exact bodypart name or ID", "Damage type name", "Wound severity name"],
			"Generates a wound scar on a present external bodypart using normal scar generation rules. Returns false for invalid or duplicate selections.");
		Register("addchargenscar", [ProgVariableTypes.Chargen, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text],
			["chargen", "bodypart", "damage", "severity", "surgery"],
			["The chargen application", "Exact bodypart name or ID", "Damage type name", "Wound severity name", "Surgical procedure type name"],
			"Generates a surgical scar on a present external bodypart using normal scar generation rules.");
		Register("addchargentattoo", [ProgVariableTypes.Chargen, ProgVariableTypes.Number, ProgVariableTypes.Text],
			["chargen", "template", "bodypart"], ["The chargen application", "Tattoo template ID", "Exact bodypart name or ID"],
			"Adds a tattoo to a present external bodypart using default text slots. Returns false if required custom text is missing or the selection is invalid or duplicated.");
		Register("addchargentattoo", [ProgVariableTypes.Chargen, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Dictionary | ProgVariableTypes.Text],
			["chargen", "template", "bodypart", "text"],
			["The chargen application", "Tattoo template ID", "Exact bodypart name or ID", "Text dictionary keyed by tattoo text slot name; writing settings come from the slot defaults"],
			"Adds a tattoo with custom slot text to a present external bodypart. Returns false for invalid or duplicate selections.");
	}

	private static void Register(string name, ProgVariableTypes[] types, List<string> names, List<string> help, string description)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name, types,
			(pars, world) => new ChargenDisfigurementFunction(pars, world, name), names, help, description,
			"Chargen", ProgVariableTypes.Boolean));
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not IChargen chargen || chargen.SelectedRace is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var success = _operation switch
		{
			"addchargenmissingbodypart" => AddMissingBodypart(chargen, Text(1)),
			"addchargenprosthesis" => AddProsthesis(chargen, ParameterFunctions[1].Result?.GetObject as IGameItemProto),
			"addchargenscar" => AddScar(chargen, Text(1), Text(2), Text(3), ParameterFunctions.Count > 4 ? Text(4) : null),
			"addchargentattoo" => AddTattoo(chargen, Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0M), Text(2),
				ParameterFunctions.Count > 3 ? ParameterFunctions[3].Result?.GetObject as IDictionary<string, IProgVariable> : null),
			_ => false
		};
		Result = new BooleanVariable(success);
		return StatementResult.Normal;
	}

	private string Text(int index) => ParameterFunctions[index].Result?.GetObject?.ToString() ?? "";

	private static IBodypart? FindPart(IChargen chargen, string name) => chargen.SelectedRace.BaseBody
		.BodypartsFor(chargen.SelectedRace, chargen.SelectedGender)
		.FirstOrDefault(x => x.Name.EqualTo(name) || x.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) == name);

	private static bool IsMissing(IChargen chargen, IBodypart part) =>
		chargen.MissingBodyparts.Any(x => x == part || part.DownstreamOfPart(x));

	internal static bool AddMissingBodypart(IChargen chargen, string name)
	{
		var part = FindPart(chargen, name);
		if (part is not IExternalBodypart || part.IsVital || !part.CanSever || IsMissing(chargen, part)) return false;
		chargen.MissingBodyparts.RemoveAll(x => x.DownstreamOfPart(part));
		chargen.MissingBodyparts.Add(part);
		return true;
	}

	internal static bool AddProsthesis(IChargen chargen, IGameItemProto? item)
	{
		var prosthetic = item?.GetItemType<ProstheticGameItemComponentProto>();
		if (prosthetic is null || !chargen.SelectedRace.BaseBody.CountsAs(prosthetic.TargetBody) ||
			!chargen.MissingBodyparts.Any(x => x.CountsAs(prosthetic.TargetBodypart)) ||
			chargen.SelectedProstheses.Contains(item!) ||
			chargen.SelectedProstheses.Any(x => x.GetItemType<ProstheticGameItemComponentProto>() is { } other &&
				(other.TargetBodypart.CountsAs(prosthetic.TargetBodypart) || prosthetic.TargetBodypart.CountsAs(other.TargetBodypart)))) return false;
		chargen.SelectedProstheses.Add(item!);
		return true;
	}

	internal bool AddScar(IChargen chargen, string name, string damage, string severity, string? surgery)
	{
		var part = FindPart(chargen, name);
		if (part is not IExternalBodypart || IsMissing(chargen, part) ||
			!damage.TryParseEnum<DamageType>(out var damageType) ||
			!severity.TryParseEnum<WoundSeverity>(out var woundSeverity) || woundSeverity == WoundSeverity.None ||
			chargen.SelectedCulture?.PrimaryCalendar is null) return false;
		SurgicalProcedureType? procedure = null;
		if (surgery is not null)
		{
			if (!surgery.TryParseEnum<SurgicalProcedureType>(out var parsed)) return false;
			procedure = parsed;
		}

		if (chargen.SelectedScars.Any(x => x.Bodypart == part && x.DamageType == damageType &&
			x.Severity == woundSeverity && x.SurgicalProcedureType == procedure)) return false;
		var context = new ScarWoundContext(procedure.HasValue, damageType, woundSeverity, procedure,
			0, Outcome.None, false, false, false, false, false);
		chargen.SelectedScars.Add(ScarGeneration.GenerateScar(_gameworld, chargen.SelectedRace, part, context,
			chargen.SelectedCulture.PrimaryCalendar.CurrentDateTime));
		return true;
	}

	internal bool AddTattoo(IChargen chargen, long id, string name, IDictionary<string, IProgVariable>? customText)
	{
		var part = FindPart(chargen, name);
		if (part is not IExternalBodypart || IsMissing(chargen, part) ||
			_gameworld.DisfigurementTemplates.Get(id) is not ITattooTemplate tattoo ||
			(tattoo.BodypartShapes.Any() && !tattoo.BodypartShapes.Contains(part.Shape)) ||
			chargen.SelectedRace.ModifiedSize(part) < tattoo.Size ||
			chargen.SelectedTattoos.Any(x => x.Tattoo == tattoo && x.Bodypart == part)) return false;
		var values = new Dictionary<string, ITattooTextValue>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var slot in tattoo.TextSlots)
		{
			var supplied = customText?.FirstOrDefault(x => x.Key.EqualTo(slot.Name)).Value?.GetObject?.ToString();
			var text = supplied ?? slot.DefaultText;
			if (slot.RequiredCustomText && string.IsNullOrWhiteSpace(supplied)) return false;
			var maxLength = Math.Max(1, (int)Math.Floor(slot.MaximumLength * (slot.DefaultScript?.DocumentLengthModifier ?? 1.0)));
			if (text?.Length > maxLength) return false;
			values[slot.Name] = new TattooTextValue(slot.Name, slot.DefaultLanguage, slot.DefaultScript,
				slot.DefaultStyle, slot.DefaultColour, slot.DefaultMinimumSkill, text, slot.DefaultAlternateText);
		}

		chargen.SelectedTattoos.Add(new SelectedTattoo(tattoo, part, values));
		return true;
	}
}
