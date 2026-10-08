#nullable enable

using MudSharp.Construction;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectEtherealEffect
{
	private sealed record SourceEtherealScope(long Silt, long Shadow, string ComponentReference);
	private SourceEtherealScope? _sourceScope;
	private XElement? _invalidSourceScope;
	private string? _sourceScopeError;

	private void LoadSourceScope(XElement root)
	{
		if (root.Element("SourceScope") is not { } scope) return;
		try
		{
			if ((int?)scope.Attribute("version") != 1) throw new FormatException("Unsupported ethereal source scope version.");
			var value = new SourceEtherealScope((long?)scope.Attribute("silt") ?? 0,
				(long?)scope.Attribute("shadow") ?? 0, (string?)scope.Attribute("component") ?? "");
			ValidateScope(value); _sourceScope = value;
		}
		catch (Exception error) when (error is FormatException or ArgumentException or OverflowException)
		{ _sourceScopeError = error.Message; _invalidSourceScope = new(scope); }
	}

	private void ValidateScope(SourceEtherealScope scope)
	{
		if (scope.Silt <= 0 || scope.Shadow <= 0 || Gameworld.Terrains.Get(scope.Silt) is null || Gameworld.Terrains.Get(scope.Shadow) is null)
			throw new FormatException("Bind existing native Silt and Shadow terrain IDs explicitly.");
		if (!ValidComponentReference(scope.ComponentReference))
			throw new FormatException("Component reference must contain 1-128 ASCII letters, digits, dots, underscores or hyphens.");
	}

	private void SaveSourceScope(XElement root)
	{
		if (_invalidSourceScope is not null) root.Add(new XElement(_invalidSourceScope));
		else if (_sourceScope is { } scope) root.Add(new XElement("SourceScope", new XAttribute("version", 1),
			new XAttribute("silt", scope.Silt), new XAttribute("shadow", scope.Shadow), new XAttribute("component", scope.ComponentReference)));
	}

	private string SourceScopeShow => _sourceScope is { } scope
		? $"\nSource scope: Silt terrain #{scope.Silt}, Shadow terrain #{scope.Shadow}, dedicated component reference {scope.ComponentReference}. Silt takes precedence, including overlapping mappings."
		: _sourceScopeError is { } error ? $"\nInvalid source scope: {error}" : "\nSource scope: off (ordinary native ethereal detection).";

	private bool BuildSourceScope(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		var mode = command.PopSpeech();
		if (mode.EqualTo("component"))
		{
			var numberText = command.PopSpeech(); var componentReference = command.PopSpeech();
			if (Spell is not MagicSpell spell || spell.InventoryPlanTemplate.GetType() != typeof(InventoryPlanTemplate) ||
				spell.InventoryPlanTemplate.Phases.Any(x => x.GetType() != typeof(InventoryPlanPhaseTemplate)))
			{ actor.OutputHandler.Send("Component reference editing requires an ordinary native inventory plan."); return false; }
			var actions = spell.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).ToArray();
			if (!command.IsFinished || !int.TryParse(numberText, out var number) || number < 1 || number > actions.Length ||
				actions[number - 1] is not InventoryPlanActionConsume action || action.GetType() != typeof(InventoryPlanActionConsume) || !ValidComponentReference(componentReference) ||
				actions.Where((_, index) => index != number - 1).Any(x => Equals(x.OriginalReference, componentReference)))
			{ actor.OutputHandler.Send("Use source component <consumed action number> <unique reference>. References contain ASCII letters, digits, dots, underscores or hyphens."); return false; }
			var replacement = new InventoryPlanActionConsume(action.SaveToXml(), Gameworld)
			{
				OriginalReference = componentReference, PrimaryItemSelector = action.PrimaryItemSelector,
				SecondaryItemSelector = action.SecondaryItemSelector, PrimaryItemFitnessScorer = action.PrimaryItemFitnessScorer
			};
			spell.InventoryPlanTemplate = new InventoryPlanTemplate(Gameworld, spell.InventoryPlanTemplate.Phases.Select(phase =>
				new InventoryPlanPhaseTemplate(phase.PhaseNumber, phase.Actions.Select(x => ReferenceEquals(x, action) ? replacement : x))))
			{ Options = spell.InventoryPlanTemplate.Options };
			_sourceSelection = null; _sourceConsumption = null;
			Spell.Changed = true; actor.OutputHandler.Send("Consumed action reference updated; bind the matching source scope explicitly."); return true;
		}
		if (mode.EqualTo("off") && command.IsFinished)
		{
			_sourceScope = null; _sourceScopeError = null; _invalidSourceScope = null; _sourceSelection = null;
			Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
		}
		var silt = Gameworld.Terrains.GetByIdOrName(mode);
		var shadow = Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
		var reference = command.PopSpeech();
		if (silt is null || shadow is null || !command.IsFinished)
		{ actor.OutputHandler.Send("Use source <Silt terrain ID/name> <Shadow terrain ID/name> <unique component reference>, source component <consumed action number> <reference>, or source off."); return false; }
		var scope = new SourceEtherealScope(silt.Id, shadow.Id, reference);
		try { ValidateScope(scope); }
		catch (FormatException error) { actor.OutputHandler.Send(error.Message); return false; }
		_sourceScope = scope; _sourceScopeError = null; _invalidSourceScope = null; _sourceSelection = null;
		Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}

	private static bool ValidComponentReference(string value) => !string.IsNullOrEmpty(value) && value.Length <= 128 &&
		value.All(x => char.IsAsciiLetterOrDigit(x) || x is '.' or '_' or '-');

	private sealed record TerrainState(ITerrain Terrain, long Id, string Name, string Behaviour, RoomLayer[] Layers,
		ITag[] Tags)
	{
		public static TerrainState Capture(ITerrain terrain) => new(terrain, terrain.Id, terrain.Name,
			terrain.TerrainBehaviourString, terrain.TerrainLayers.ToArray(), terrain.Tags.ToArray());
		public bool Current(IFuturemud world) => ReferenceEquals(world.Terrains.Get(Id), Terrain) && Terrain.Id == Id &&
			Terrain.Name == Name && Terrain.TerrainBehaviourString == Behaviour && Terrain.TerrainLayers.SequenceEqual(Layers) &&
			Terrain.Tags.SequenceEqual(Tags, ReferenceEqualityComparer.Instance);
	}

	private sealed record TagState(ITag Tag, long Id, string Name, ITag? Parent)
	{
		public bool Current(IFuturemud world) => ReferenceEquals(world.Tags.Get(Id), Tag) && Tag.Id == Id &&
			Tag.Name == Name && ReferenceEquals(Tag.Parent, Parent);
	}
}
