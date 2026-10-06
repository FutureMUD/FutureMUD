#nullable enable

using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;

namespace MudSharp.Magic.SpellEffects;

public partial class CreateItemEffect
{
	private readonly Dictionary<int, long[]> _gradeOutputs = new();
	private IGameItemProto? _preparedPrototype;
	private long _eligibilityProgId;
	private long _lifetimeMultiplierProgId;
	public IFutureProg? EligibilityProg => Gameworld.FutureProgs.Get(_eligibilityProgId);
	public IFutureProg? LifetimeMultiplierProg => Gameworld.FutureProgs.Get(_lifetimeMultiplierProgId);
	public IReadOnlyDictionary<int, long[]> GradeOutputs => _gradeOutputs.ToDictionary(x => x.Key, x => x.Value.ToArray());

	private void LoadOutputPolicies(XElement root)
	{
		var eligibility = long.Parse(root.Element("EligibilityProg")?.Value ?? "0");
		var multiplier = long.Parse(root.Element("LifetimeMultiplierProg")?.Value ?? "0");
		if (eligibility < 0 || multiplier < 0) throw new FormatException("Output policy prog IDs cannot be negative.");
		var outputs = new Dictionary<int, long[]>();
		foreach (var row in root.Element("GradeOutputs")?.Elements("Output") ?? [])
		{
			var grade = int.Parse(row.Attribute("grade")?.Value ?? throw new FormatException("Missing output grade."));
			var ids = row.Elements("Prototype").Select(x => long.Parse(x.Value)).ToArray();
			if (grade is < 1 or > 7 || ids.Length is < 1 or > 32 || ids.Any(x => x <= 0) || ids.Distinct().Count() != ids.Length || !outputs.TryAdd(grade, ids))
				throw new FormatException("Output pools require unique grades 1-7 and one to thirty-two distinct positive prototype IDs.");
		}
		_eligibilityProgId = eligibility; _lifetimeMultiplierProgId = multiplier;
		foreach (var output in outputs) _gradeOutputs.Add(output.Key, output.Value);
	}

	private IEnumerable<XElement> SaveOutputPolicies()
	{
		if (_eligibilityProgId != 0) yield return new("EligibilityProg", _eligibilityProgId);
		if (_lifetimeMultiplierProgId != 0) yield return new("LifetimeMultiplierProg", _lifetimeMultiplierProgId);
		if (_gradeOutputs.Count > 0) yield return new("GradeOutputs", _gradeOutputs.OrderBy(x => x.Key).Select(x =>
			new XElement("Output", new XAttribute("grade", x.Key), x.Value.Select(id => new XElement("Prototype", id)))));
	}

	private string? OutputPolicyError()
	{
		if (_eligibilityProgId != 0 && (EligibilityProg is not { } eligibility || !eligibility.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !eligibility.MatchesParameters([ProgVariableTypes.Character])))
			return "Item eligibility requires an existing boolean(character) prog.";
		if (_lifetimeMultiplierProgId != 0 && (LifetimeMultiplierProg is not { } multiplier || !multiplier.ReturnType.CompatibleWith(ProgVariableTypes.Number) || !multiplier.MatchesParameters([ProgVariableTypes.Character])))
			return "Item lifetime multiplier requires an existing number(character) prog.";
		if (_gradeOutputs.Count > 0 && (CountByGrade || WornLight)) return "Grade output pools require single plain-item creation without worn-light placement.";
		foreach (var id in _gradeOutputs.Values.SelectMany(x => x))
		{
			var prototype = Gameworld.ItemProtos.Get(id);
			if (NativeItemCreationEligibility.Error(prototype, Gameworld) is { } error) return error;
			if (prototype.IsItemType<ProgLightGameItemComponentProto>() || PrimaryHand && !prototype.IsItemType<MeleeWeaponGameItemComponentProto>())
				return "Output pool prototypes must support the configured placement.";
		}
		return null;
	}

	private IGameItemProto PreparePrototype(int grade) => _preparedPrototype ??= _gradeOutputs.TryGetValue(grade, out var ids)
		? Gameworld.ItemProtos.Get(ids[Random.Shared.Next(ids.Length)])
		: PermanentGrade == grade ? PermanentPrototype! : ItemPrototype;

	private bool BuildingCommandOutputPolicy(ICharacter actor, StringStack command)
	{
		if (LifecycleMode is null) { actor.OutputHandler.Send("Set an explicit item lifecycle before configuring output policies."); return false; }
		var option = command.PopSpeech().ToLowerInvariant();
		if (option == "output")
		{
			if (!int.TryParse(command.PopSpeech(), out var grade) || grade is < 1 or > 7)
			{ actor.OutputHandler.Send("Use output <grade 1-7> <prototype> ... or none."); return false; }
			if (command.PeekSpeech().EqualTo("none"))
			{
				command.PopSpeech(); if (!command.IsFinished) return false; _gradeOutputs.Remove(grade);
			}
			else
			{
				var ids = new List<long>();
				while (!command.IsFinished)
				{
					var prototype = Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
					if (NativeItemCreationEligibility.Error(prototype, Gameworld) is { } error)
					{ actor.OutputHandler.Send(error); return false; }
					ids.Add(prototype!.Id);
				}
				if (ids.Count is < 1 or > 32 || ids.Distinct().Count() != ids.Count) return false;
				var old = _gradeOutputs.GetValueOrDefault(grade); _gradeOutputs[grade] = ids.ToArray();
				if (OutputPolicyError() is { } validation)
				{
					if (old is null) _gradeOutputs.Remove(grade); else _gradeOutputs[grade] = old;
					actor.OutputHandler.Send(validation); return false;
				}
			}
		}
		else
		{
			var text = command.SafeRemainingArgument;
			var prog = text.EqualTo("none") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
			var type = option == "eligibility" ? ProgVariableTypes.Boolean : ProgVariableTypes.Number;
			if (!text.EqualTo("none") && (prog is null || !prog.ReturnType.CompatibleWith(type) || !prog.MatchesParameters([ProgVariableTypes.Character])))
			{ actor.OutputHandler.Send($"Select a {type}(character) prog or none."); return false; }
			if (option == "eligibility") _eligibilityProgId = prog?.Id ?? 0; else _lifetimeMultiplierProgId = prog?.Id ?? 0;
		}
		_preparedPrototype = null; _preparedLifetimeSeconds = null;
		Spell.Changed = true; actor.OutputHandler.Send("Item output policy updated."); return true;
	}
}
