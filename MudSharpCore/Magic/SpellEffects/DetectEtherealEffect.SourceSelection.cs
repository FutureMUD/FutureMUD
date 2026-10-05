#nullable enable

using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectEtherealEffect : IMagicSpellEffectPreparedSelection, IMagicSpellEffectTemplate
{
	private sealed record ActionState(InventoryPlanAction Action, string Xml,
		Func<IGameItem, bool>? Primary, Func<IGameItem, bool>? Secondary)
	{
		public bool Current() => Action.SaveToXml().ToString(SaveOptions.DisableFormatting) == Xml &&
			ReferenceEquals(Action.PrimaryItemSelector, Primary) && ReferenceEquals(Action.SecondaryItemSelector, Secondary) &&
			Action.PrimaryItemFitnessScorer is null;
	}

	private sealed record ComponentState(IGameItem Item, long Id, IGameItemProto Prototype, IStackable? Stack,
		int Quantity, IBody? Custodian, IGameItem? Container, ICell? Location, RoomLayer Layer, ITag[] Tags,
		ITag[] PrototypeTags, TagState[] Hierarchy)
	{
		public static ComponentState Capture(IGameItem item)
		{
			var tags = item.Tags.ToArray(); var prototypeTags = item.Prototype.Tags.ToArray();
			var hierarchy = new Dictionary<ITag, TagState>(ReferenceEqualityComparer.Instance);
			foreach (var tag in tags.Concat(prototypeTags))
			{
				var path = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
				for (var current = tag; current is not null; current = current.Parent)
				{
					if (!path.Add(current)) throw new InvalidOperationException("Component tag hierarchy contains a cycle.");
					hierarchy.TryAdd(current, new(current, current.Id, current.Name, current.Parent));
				}
			}
			return new(item, item.Id, item.Prototype, item.GetItemType<IStackable>(), item.GetItemType<IStackable>()?.Quantity ?? 1,
				item.InInventoryOf, item.ContainedIn, item.Location, item.RoomLayer, tags, prototypeTags, hierarchy.Values.ToArray());
		}
		public bool RawCurrent(IFuturemud world, int? expectedQuantity = null) =>
			ReferenceEquals(world.Items.Get(Id), Item) && Item.Id == Id && !Item.Deleted && !Item.Destroyed &&
			ReferenceEquals(Item.Prototype, Prototype) && ReferenceEquals(Item.GetItemType<IStackable>(), Stack) &&
			(Stack?.Quantity ?? 1) == (expectedQuantity ?? Quantity) && ReferenceEquals(Item.InInventoryOf, Custodian) &&
			ReferenceEquals(Item.ContainedIn, Container) && ReferenceEquals(Item.Location, Location) && Item.RoomLayer == Layer &&
			Item.Tags.SequenceEqual(Tags, ReferenceEqualityComparer.Instance) &&
			Prototype.Tags.SequenceEqual(PrototypeTags, ReferenceEqualityComparer.Instance) && Hierarchy.All(x => x.Current(world));
	}

	private sealed record SourceEtherealSelection(MagicSpell Source, IInventoryPlanTemplate AuthoredPlan,
		string AuthoredXml, InventoryPlanOptions Options, ActionState[] Actions, string SourceDefinition,
		string EffectXml, SourceEtherealScope Scope, int Grade, ICharacter Caster, IBody Body, ICell Cell,
		ICellOverlay Overlay, RoomLayer Layer, TerrainState Silt, TerrainState Shadow, TerrainState Terrain,
		bool Exempt, TagState[] Tags, ITag Family, ITag MinimumRank, ComponentState? Component)
		: IMagicSpellEffectPreparedSelectionToken;

	private SourceEtherealSelection? _sourceSelection;
	private SourceConsumptionObservation? _sourceConsumption;

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_sourceScopeError is { } error) throw new InvalidOperationException(error);
		if (_sourceScope is null) return null;
		if (_sourceSelection is { } existing) { ConfirmSource(existing, caster, recipient); return existing; }
		if (Spell is not MagicSpell invocation || invocation.InvocationGrade is not { } grade || grade is < 1 or > 7 ||
			Gameworld.MagicSpells.Get(Spell.Id) is not MagicSpell source || !ReferenceEquals(caster, recipient) ||
			LifetimePolicy is null || caster.Body is null || caster.Location?.CurrentOverlay?.Terrain is null)
			throw new InvalidOperationException("Source ethereal detection needs self, explicit grade, lifetime and current native terrain.");
		ValidateScope(_sourceScope);
		var cell = caster.Location; var terrain = cell.CurrentOverlay.Terrain;
		// Overlapping mappings are intentional and fail Silt first, never exempt it.
		if (terrain.Id == _sourceScope.Silt) throw new InvalidOperationException("Source ethereal detection refuses Silt.");
		var authored = source.InventoryPlanTemplate;
		if (authored.GetType() != typeof(InventoryPlanTemplate))
			throw new InvalidOperationException("Source component selection requires a native XML inventory plan; custom templates are unsupported.");
		var actions = authored.Phases.SelectMany(x => x.Actions).ToArray();
		if (actions.Any(x => x is not InventoryPlanAction || ((InventoryPlanAction)x).PrimaryItemFitnessScorer is not null))
			throw new InvalidOperationException("Source component selection cannot preserve custom actions or runtime fitness scorers; remove them explicitly.");
		var states = actions.Cast<InventoryPlanAction>().Select(x => new ActionState(x,
			x.SaveToXml().ToString(SaveOptions.DisableFormatting), x.PrimaryItemSelector, x.SecondaryItemSelector)).ToArray();
		foreach (var state in states)
			if (InventoryPlanAction.LoadAction(XElement.Parse(state.Xml), Gameworld).GetType() != state.Action.GetType())
				throw new InvalidOperationException("An inventory action has unsupported runtime behavior beyond its XML type.");
		var dedicated = states.Where(x => Equals(x.Action.OriginalReference, _sourceScope.ComponentReference)).ToArray();
		if (dedicated.Length != 1 || dedicated[0].Action is not InventoryPlanActionConsume consume ||
			consume.Quantity != 1 || !consume.CarriedOnly || consume.RequiredGrade is not null)
			throw new InvalidOperationException("Source scope needs exactly one referenced, quantity-one, carried consume action for all grades.");
		var rank = consume.SaveToXml().Element("GradeRank");
		if (rank is null || (int?)rank.Attribute("offset") != -3 || rank.Elements("Rank").Count() != 5)
			throw new InvalidOperationException("Source Divination needs five rank tags0-4 with grade offset -3.");
		consume = new InventoryPlanActionConsume(consume.SaveToXml(), Gameworld);
		var rankTags = rank.Elements("Rank").OrderBy(x => (int)x.Attribute("minimum")!).Select(x => Gameworld.Tags.Get((long)x.Attribute("tag")!)!).ToArray();
		var allTags = new List<ITag>();
		foreach (var tag in rankTags.Append(consume.DesiredTag))
		{
			var visited = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
			for (var current = tag; current is not null; current = current.Parent)
			{
				if (!visited.Add(current)) throw new InvalidOperationException("Component rank hierarchy contains a cycle.");
				if (!allTags.Contains(current, ReferenceEqualityComparer.Instance)) allTags.Add(current);
			}
		}
		var token = new SourceEtherealSelection(source, authored, authored.SaveToXml().ToString(SaveOptions.DisableFormatting),
			authored.Options, states, source.SnapshotModel().Definition, SaveToXml().ToString(SaveOptions.DisableFormatting),
			_sourceScope, grade, caster, caster.Body, cell, cell.CurrentOverlay, caster.RoomLayer,
			TerrainState.Capture(Gameworld.Terrains.Get(_sourceScope.Silt)!), TerrainState.Capture(Gameworld.Terrains.Get(_sourceScope.Shadow)!),
			TerrainState.Capture(terrain), terrain.Id == _sourceScope.Shadow,
			allTags.Select(x => new TagState(x, x.Id, x.Name, x.Parent)).ToArray(), consume.DesiredTag, rankTags[Math.Max(grade - 3, 0)], null);
		ConfirmSource(token, caster, recipient);
		// Native rank binding can invoke tag predicates too. Freeze first and reject
		// their drift, just as with later planning/selector callbacks.
		consume.BindSelectedGrade(grade);
		ConfirmSource(token, caster, recipient);
		var candidates = new Dictionary<IGameItem, ComponentState>(ReferenceEqualityComparer.Instance);
		var scouting = BuildSourcePlan(token, null, candidates);
		var plan = (InventoryPlan)scouting.CreatePlan(caster);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			throw new InvalidOperationException("Source component inventory plan is infeasible.");
		var results = plan.PeekPlanResults().ToArray();
		var scouts = plan.Phases.Values.SelectMany(x => x.ScoutedItems).ToArray();
		// Native scouts, selectors and feasibility callbacks cannot refresh the chosen branch.
		ConfirmSource(token, caster, recipient);
		if (scouts.Any(x => !results.Any(result => result.ActionState == x.Action.DesiredState &&
			Equals(result.OriginalReference, x.Action.OriginalReference) && ReferenceEquals(result.PrimaryTarget, x.Primary) &&
			(x.Secondary is null || ReferenceEquals(result.SecondaryTarget, x.Secondary)))))
			throw new InvalidOperationException("This native plan has inputs omitted from casting receipts; source scope does not support that plan.");
		if (!token.Exempt)
		{
			var chosen = results.Where(x => Equals(x.OriginalReference, token.Scope.ComponentReference)).ToArray();
			if (chosen.Length != 1 || chosen[0].PrimaryTarget is not { } item || !candidates.TryGetValue(item, out var before) ||
				!before.RawCurrent(Gameworld) || !ReferenceEquals(before.Custodian, caster.Body) || before.Container is not null || before.Quantity < 1)
				throw new InvalidOperationException("Source component changed during native scouting or has ambiguous identity.");
			if (scouts.Any(x => !Equals(x.Action.OriginalReference, token.Scope.ComponentReference) &&
				(ReferenceEquals(x.Primary, item) || ReferenceEquals(x.Secondary, item))) ||
				results.Any(x => !Equals(x.OriginalReference, token.Scope.ComponentReference) &&
					(ReferenceEquals(x.PrimaryTarget, item) || ReferenceEquals(x.SecondaryTarget, item))))
				throw new InvalidOperationException("Another authored action shares the selected source component; its consumption is ambiguous.");
			token = token with { Component = before };
		}
		ConfirmSource(token, caster, recipient);
		_sourceSelection = token; InstallSourcePlan(invocation, token); return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		try
		{
			if (_sourceScope is null || selection is not SourceEtherealSelection token || Spell is not MagicSpell invocation)
				throw new InvalidOperationException("Source selection token is missing or incompatible.");
			ConfirmSource(token, caster, recipient); _sourceSelection = token; InstallSourcePlan(invocation, token);
			error = null; return true;
		}
		catch (Exception exception) when (exception is InvalidOperationException or FormatException)
		{ error = exception.Message; return false; }
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		try
		{
			if (_sourceScope is not null) ConfirmSource(_sourceSelection ?? throw new InvalidOperationException("Missing source selection."), caster, recipient);
			else if (_sourceScopeError is not null) throw new InvalidOperationException(_sourceScopeError);
			error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}

	private void ConfirmSource(SourceEtherealSelection token, ICharacter caster, IPerceivable recipient, bool component = true)
	{
		if (_sourceScopeError is not null || _sourceScope != token.Scope || Spell is not MagicSpell invocation ||
			invocation.InvocationGrade != token.Grade || !ReferenceEquals(caster, recipient) || !ReferenceEquals(caster, token.Caster) ||
			!ReferenceEquals(caster.Body, token.Body) || !ReferenceEquals(caster.Location, token.Cell) || caster.RoomLayer != token.Layer ||
			!ReferenceEquals(token.Cell.CurrentOverlay, token.Overlay) || !ReferenceEquals(token.Overlay.Terrain, token.Terrain.Terrain) ||
			token.Terrain.Id == token.Scope.Silt || token.Exempt != (token.Terrain.Id == token.Scope.Shadow) ||
			!token.Silt.Current(Gameworld) || !token.Shadow.Current(Gameworld) || !token.Terrain.Current(Gameworld) ||
			token.Tags.Any(x => !x.Current(Gameworld)) || !ReferenceEquals(Gameworld.MagicSpells.Get(token.Source.Id), token.Source) ||
			!ReferenceEquals(token.Source.InventoryPlanTemplate, token.AuthoredPlan) || token.AuthoredPlan.Options != token.Options ||
			token.AuthoredPlan.SaveToXml().ToString(SaveOptions.DisableFormatting) != token.AuthoredXml || token.Actions.Any(x => !x.Current()) ||
			token.Source.SnapshotModel().Definition != token.SourceDefinition || SaveToXml().ToString(SaveOptions.DisableFormatting) != token.EffectXml ||
			component && token.Component is { } selected && !selected.RawCurrent(Gameworld))
			throw new InvalidOperationException("Frozen source environment, configuration or component changed; no reselection is permitted.");
	}

	private void InstallSourcePlan(MagicSpell invocation, SourceEtherealSelection token)
	{
		_sourceConsumption = new SourceConsumptionObservation();
		invocation.InventoryPlanTemplate = new SourceEtherealPlanTemplate(this, token,
			BuildSourcePlan(token, token.Component, null), _sourceConsumption);
	}

	private InventoryPlanTemplate BuildSourcePlan(SourceEtherealSelection token, ComponentState? pinned,
		Dictionary<IGameItem, ComponentState>? candidates)
	{
		var xml = XElement.Parse(token.AuthoredXml);
		foreach (var action in xml.Descendants("Action").Where(x => x.Attribute("grade") is { } grade && (int)grade != token.Grade).ToArray()) action.Remove();
		if (token.Exempt)
			foreach (var action in xml.Descendants("Action").Where(x => (string?)x.Attribute("originalreference") == token.Scope.ComponentReference).ToArray()) action.Remove();
		var plan = new InventoryPlanTemplate(xml, Gameworld) { Options = token.Options };
		var original = token.Actions.Where(x => x.Action is not InventoryPlanActionConsume { RequiredGrade: { } grade } || grade == token.Grade)
			.Where(x => !token.Exempt || !Equals(x.Action.OriginalReference, token.Scope.ComponentReference)).ToArray();
		var copied = plan.Phases.SelectMany(x => x.Actions).Cast<InventoryPlanAction>().ToArray();
		if (copied.Length != original.Length) throw new InvalidOperationException("Native source plan topology changed during cloning.");
		for (var index = 0; index < copied.Length; ++index)
		{
			var before = original[index]; var current = copied[index];
			current.PrimaryItemSelector = before.Primary; current.SecondaryItemSelector = before.Secondary;
			if (current is InventoryPlanActionConsume consume) consume.BindSelectedGrade(token.Grade);
			if (!Equals(current.OriginalReference, token.Scope.ComponentReference)) continue;
			current.PrimaryItemSelector = item =>
			{
				if (pinned is not null && !ReferenceEquals(item, pinned.Item)) return false;
				var raw = pinned ?? ComponentState.Capture(item);
				if (candidates is not null && !candidates.ContainsKey(item)) candidates.Add(item, raw);
				var allowed = before.Primary?.Invoke(item) ?? true;
				if (allowed) allowed = item.IsA(token.Family) && item.IsA(token.MinimumRank);
				// These checks deliberately run AFTER preserved selectors and rank callbacks.
				ConfirmSource(token, token.Caster, token.Caster, false);
				return allowed && raw.RawCurrent(Gameworld) && ReferenceEquals(item.InInventoryOf, token.Body) && item.ContainedIn is null;
			};
		}
		return plan;
	}

	public new IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (_sourceScope is not null || _sourceScopeError is not null)
			ConfirmSourceApplication(caster, target ?? throw new InvalidOperationException("Source ethereal detection needs self."));
		return base.GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
	}

	private void ConfirmSourceApplication(ICharacter caster, IPerceivable recipient)
	{
		if (_sourceScopeError is { } error) throw new InvalidOperationException(error);
		if (_sourceScope is null) return;
		var token = _sourceSelection ?? throw new InvalidOperationException("Source ethereal application needs prepared selection.");
		ConfirmSource(token, caster, recipient, false);
		if (_sourceConsumption is not { Finished: true }) throw new InvalidOperationException("Source material execution was not observed.");
		if (!token.Exempt) ConfirmObservedConsumption(token);
	}

	private void ConfirmObservedConsumption(SourceEtherealSelection token)
	{
		var before = token.Component ?? throw new InvalidOperationException("Missing observed source component.");
		var item = before.Item;
		var observed = before.Quantity > 1 && before.Stack is not null
			? before.RawCurrent(Gameworld, before.Quantity - 1)
			: item.Deleted && Gameworld.Items.Get(before.Id) is null && item.InInventoryOf is null && item.ContainedIn is null &&
				!token.Caster.Inventory.Any(x => ReferenceEquals(x, item)) && !token.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, item));
		if (!observed) throw new InvalidOperationException("Source consumption is not proven by the observed item state; no retry or substitute is permitted.");
	}
}
