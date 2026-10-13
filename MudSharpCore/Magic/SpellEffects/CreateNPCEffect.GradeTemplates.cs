#nullable enable

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateNPCEffect
{
	private readonly Dictionary<int, long> _gradeTemplates = new();
	private string? _gradeTemplateLoadError;
	private XElement[]? _unreadableGradeTemplates;
	public IReadOnlyDictionary<int, long> GradeTemplates => new Dictionary<int, long>(_gradeTemplates);
	private bool HasGradeTemplatePolicy => _gradeTemplates.Count > 0 || _gradeTemplateLoadError is not null;
	private long? SelectedTemplateId => _gradeTemplates.Count == 0 ? _npcPrototypeId :
		Spell is MagicSpell { InvocationGrade: { } grade } && _gradeTemplates.TryGetValue(grade, out var id) ? id : null;

	private void LoadGradeTemplates(XElement root)
	{
		var schemas = root.Elements().Where(x => x.Name.LocalName == "GradeTemplates").ToArray();
		if (schemas.Length == 0) return;
		var schema = schemas[0];
		var mappings = new Dictionary<int, long>();
		if (schemas.Length != 1 || schema.Name != "GradeTemplates" || (string?)schema.Attribute("version") != "1" ||
			schema.Attributes().Any(x => x.Name != "version") || !schema.HasElements ||
			schema.Elements().Any(x => x.Name != "Template") ||
			schema.Nodes().OfType<XText>().Any(x => !string.IsNullOrWhiteSpace(x.Value)))
		{
			Reject("Invalid grade-specific NPC template schema; use version 1 with Template entries.");
			return;
		}
		foreach (var row in schema.Elements("Template"))
		{
			if (row.HasElements || row.Attributes().Any(x => x.Name != "grade") ||
				!int.TryParse((string?)row.Attribute("grade"), out var grade) || grade is < 1 or > 7 ||
				!long.TryParse(row.Value, out var id) || id <= 0 || !mappings.TryAdd(grade, id))
			{
				Reject("NPC template mappings require unique requested grades 1-7 and positive template IDs.");
				return;
			}
		}
		foreach (var mapping in mappings) _gradeTemplates.Add(mapping.Key, mapping.Value);
		return;

		void Reject(string error)
		{
			_gradeTemplateLoadError = error;
			_unreadableGradeTemplates = schemas.Select(x => new XElement(x)).ToArray();
		}
	}

	private IEnumerable<XElement> SaveGradeTemplates()
	{
		if (_unreadableGradeTemplates is not null)
		{
			foreach (var schema in _unreadableGradeTemplates) yield return new XElement(schema);
			yield break;
		}
		if (_gradeTemplates.Count > 0)
			yield return new XElement("GradeTemplates", new XAttribute("version", 1),
				_gradeTemplates.OrderBy(x => x.Key).Select(x =>
					new XElement("Template", new XAttribute("grade", x.Key), x.Value)));
	}

	private IEnumerable<INPCTemplate?> DefinitionTemplates => _gradeTemplates.Count > 0
		? _gradeTemplates.Values.Select(id => Gameworld.NpcTemplates.Get(id))
		: [Gameworld.NpcTemplates.Get(_npcPrototypeId)];

	private string? NativeTemplateDefinitionError()
	{
		if (_gradeTemplateLoadError is not null) return _gradeTemplateLoadError;
		if (_gradeTemplates.Count > 0 && LifecycleMode is null)
			return "Grade-specific NPC templates require an explicit native lifecycle.";
		if (_gradeTemplates.Count > 0 && Spell is MagicSpell { InvocationGrade: { } grade } &&
			!_gradeTemplates.ContainsKey(grade))
			return $"No NPC template is configured for requested grade {grade}; a grade mapping cannot fall back to the legacy template.";
		foreach (var template in DefinitionTemplates)
			if (NativeNpcCreationEligibility.TemplateError(template, Gameworld) is { } error) return error;
		return null;
	}

	private bool BuildingCommandGradeTemplate(ICharacter actor, StringStack command)
	{
		if (LifecycleMode is null)
		{
			actor.OutputHandler.Send("Select an explicit lifecycle before configuring grade-specific NPC templates.");
			return false;
		}
		var gradeText = command.PopSpeech();
		if (gradeText.EqualTo("clear") && command.IsFinished)
		{
			_gradeTemplates.Clear(); _gradeTemplateLoadError = null; _unreadableGradeTemplates = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("Grade-specific NPC templates cleared; the single NPC template will be used.");
			return true;
		}
		if (_gradeTemplateLoadError is not null)
		{
			actor.OutputHandler.Send("Clear the unreadable grade-template policy before replacing its entries.");
			return false;
		}
		if (!int.TryParse(gradeText, out var grade) || grade is < 1 or > 7 || command.IsFinished)
		{
			actor.OutputHandler.Send("Use npcgrade <grade 1-7> <approved NPC template>|none, or npcgrade clear.".ColourCommand());
			return false;
		}
		var templateText = command.SafeRemainingArgument;
		if (templateText.EqualTo("none")) _gradeTemplates.Remove(grade);
		else
		{
			var template = Gameworld.NpcTemplates.GetByIdOrName(templateText);
			if (NativeNpcCreationEligibility.TemplateError(template, Gameworld) is { } error)
			{
				actor.OutputHandler.Send(error.ColourError());
				return false;
			}
			if (LifecycleMode == SpellLifecycleMode.Permanent && template!.ArtificialIntelligences.OfType<SpellCompanionAI>().Any())
			{
				actor.OutputHandler.Send("Spell companion control requires an active temporary lifecycle.".ColourError());
				return false;
			}
			_gradeTemplates[grade] = template!.Id;
		}
		// Retain stale tokens so an already prepared application must observe the changed policy.
		Spell.Changed = true;
		actor.OutputHandler.Send($"The NPC template for requested grade {grade.ToString("N0", actor).ColourValue()} has been updated.");
		return true;
	}
}
