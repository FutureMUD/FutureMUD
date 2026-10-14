#nullable enable

using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class SpellArmourEffect
{
	private sealed record SourceArmourScope(long WaterPlane, string ComponentReference);
	private sealed record SourceArmourTag(ITag Tag, long Id, string Name, ITag? Parent)
	{
		public bool Current(IFuturemud world) => ReferenceEquals(world.Tags.Get(Id), Tag) && Tag.Id == Id &&
			Tag.Name == Name && ReferenceEquals(Tag.Parent, Parent);
	}
	private sealed record SourceArmourItem(IGameItem Item, long Id, IGameItemProto Prototype, IStackable? Stack,
		int Quantity, IBody Body, IGameItem? Container, IRoom? Location, ITag[] Tags, ITag[] PrototypeTags,
		SourceArmourTag[] Hierarchy)
	{
		public bool Current(IFuturemud world, int? quantity = null) =>
			ReferenceEquals(world.Items.Get(Id), Item) && Item.Id == Id && !Item.Deleted && !Item.Destroyed &&
			ReferenceEquals(Item.Prototype, Prototype) && ReferenceEquals(Item.GetItemType<IStackable>(), Stack) &&
			(Stack?.Quantity ?? 1) == (quantity ?? Quantity) && ReferenceEquals(Item.InInventoryOf, Body) &&
			ReferenceEquals(Item.ContainedIn, Container) && ReferenceEquals(Item.Location, Location) &&
			Item.Tags.SequenceEqual(Tags, ReferenceEqualityComparer.Instance) &&
			Prototype.Tags.SequenceEqual(PrototypeTags, ReferenceEqualityComparer.Instance) &&
			Hierarchy.All(x => x.Current(world));
	}
	private sealed record SourceArmourSelection(MagicSpell Source, IInventoryPlanTemplate AuthoredPlan,
		string AuthoredXml, string SourceDefinition, string EffectXml, SourceArmourScope Scope,
		ICharacter Caster, IPerceivable Recipient, IBody Body, int Grade, IPlane Plane, long[] PresencePlanes,
		bool Exempt, ITag[] RankTags, SourceArmourTag[] RankStates, SourceArmourItem? Item) : IMagicSpellEffectPreparedSelectionToken;

	private SourceArmourScope? _sourceArmourScope;
	private XElement? _invalidSourceArmourScope;
	private string? _sourceArmourError;
	private SourceArmourSelection? _sourceArmourSelection;
	private bool _sourceArmourConsumed;
	private bool _sourceArmourFinished;

	private void LoadSourceArmourScope(XElement root)
	{
		if (root.Element("SourceArmourScope") is not { } scope) return;
		try
		{
			if ((int?)scope.Attribute("version") != 1) throw new FormatException("Unsupported source armour scope version.");
			var candidate = new SourceArmourScope((long?)scope.Attribute("waterPlane") ?? 0,
				(string?)scope.Attribute("component") ?? "");
			ValidateSourceArmourScope(candidate); _sourceArmourScope = candidate;
		}
		catch (Exception error) when (error is FormatException or ArgumentException or OverflowException)
		{ _sourceArmourError = error.Message; _invalidSourceArmourScope = new(scope); }
	}

	private void ValidateSourceArmourScope(SourceArmourScope scope)
	{
		if (scope.WaterPlane <= 0 || Gameworld.Planes.Get(scope.WaterPlane) is null)
			throw new FormatException("Bind an existing native Water plane explicitly.");
		if (string.IsNullOrEmpty(scope.ComponentReference) || scope.ComponentReference.Length > 128 ||
			scope.ComponentReference.Any(x => !char.IsAsciiLetterOrDigit(x) && x is not ('.' or '_' or '-')))
			throw new FormatException("Component reference must contain 1-128 ASCII letters, digits, dots, underscores or hyphens.");
	}

	private void SaveSourceArmourScope(XElement root)
	{
		if (_invalidSourceArmourScope is not null) root.Add(new XElement(_invalidSourceArmourScope));
		else if (_sourceArmourScope is { } scope) root.Add(new XElement("SourceArmourScope",
			new XAttribute("version", 1), new XAttribute("waterPlane", scope.WaterPlane),
			new XAttribute("component", scope.ComponentReference)));
	}

	private string SourceArmourScopeShow => _sourceArmourScope is { } scope
		? $"\nConditional material: one directly carried ranked component ({scope.ComponentReference}) outside Water plane #{scope.WaterPlane}."
		: _sourceArmourError is { } error ? $"\nInvalid conditional material: {error}" : "";

	private bool BuildSourceArmourScope(ICharacter actor, StringStack command)
	{
		var mode = command.PopSpeech();
		if (mode.EqualTo("off") && command.IsFinished)
		{
			_sourceArmourScope = null; _sourceArmourError = null; _invalidSourceArmourScope = null; _sourceArmourSelection = null;
			Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
		}
		var plane = mode.EqualTo("plane") ? Gameworld.Planes.GetByIdOrName(command.PopSpeech()) : null;
		var reference = command.PopSpeech();
		if (plane is null || !command.IsFinished)
		{ actor.OutputHandler.Send("Use source plane <Water plane ID/name> <unique consumed component reference>, or source off."); return false; }
		var candidate = new SourceArmourScope(plane.Id, reference);
		try { ValidateSourceArmourScope(candidate); }
		catch (FormatException error) { actor.OutputHandler.Send(error.Message); return false; }
		_sourceArmourScope = candidate; _sourceArmourError = null; _invalidSourceArmourScope = null; _sourceArmourSelection = null;
		Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_sourceArmourError is { } error) throw new InvalidOperationException(error);
		if (_sourceArmourScope is not { } scope) return null;
		if (_sourceArmourSelection is { } existing) { ConfirmSourceArmour(existing, caster, recipient); return existing; }
		if (Spell is not MagicSpell invocation || invocation.InvocationGrade is not { } grade || grade is < 1 or > 7 ||
			Gameworld.MagicSpells.Get(Spell.Id) is not MagicSpell source || caster.Body is null ||
			!ReferenceEquals(recipient, caster) && recipient is not ICharacter)
			throw new InvalidOperationException("Source armour requires an ordinary character target, a body and an explicit grade.");
		ValidateSourceArmourScope(scope);
		var authored = source.InventoryPlanTemplate;
		if (authored.GetType() != typeof(InventoryPlanTemplate))
			throw new InvalidOperationException("Source armour requires a native XML inventory plan.");
		var actions = authored.Phases.SelectMany(x => x.Actions).ToArray();
		if (authored.Phases.Count() != 1 || actions.Length != 1 || actions[0] is not InventoryPlanActionConsume consume ||
			consume.GetType() != typeof(InventoryPlanActionConsume) || consume.Quantity != 1 || !consume.CarriedOnly ||
			consume.RequiredGrade is not null || consume.PrimaryItemSelector is not null || consume.SecondaryItemSelector is not null ||
			consume.PrimaryItemFitnessScorer is not null || !Equals(consume.OriginalReference, scope.ComponentReference))
			throw new InvalidOperationException("Source armour needs one native, directly carried, quantity-one consumed component action without runtime selectors.");
		var rank = consume.SaveToXml().Element("GradeRank");
		if (rank is null || (int?)rank.Attribute("offset") != -3 || rank.Elements("Rank").Count() != 5)
			throw new InvalidOperationException("Source Clerical material needs five rank tags 0-4 with grade offset -3.");
		var rankTags = rank.Elements("Rank").OrderBy(x => (int)x.Attribute("minimum")!)
			.Select(x => Gameworld.Tags.Get((long)x.Attribute("tag")!) ?? throw new InvalidOperationException("Source rank tag is missing.")).ToArray();
		var rankStates = rankTags.SelectMany(Ancestors).DistinctBy(tag => tag.Id)
			.Select(tag => new SourceArmourTag(tag, tag.Id, tag.Name, tag.Parent)).ToArray();
		var presence = caster.GetPlanarPresence().PresencePlaneIds.OrderBy(x => x).ToArray();
		var token = new SourceArmourSelection(source, authored, authored.SaveToXml().ToString(SaveOptions.DisableFormatting),
			source.SnapshotModel().Definition, SaveToXml().ToString(SaveOptions.DisableFormatting), scope,
			caster, recipient, caster.Body, grade, Gameworld.Planes.Get(scope.WaterPlane)!, presence,
			presence.Contains(scope.WaterPlane), rankTags, rankStates, null);
		ConfirmSourceArmour(token, caster, recipient);
		var plan = BuildSourceArmourPlan(token, null);
		var native = (InventoryPlan)plan.CreatePlan(caster);
		if (native.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			throw new InvalidOperationException("Source Clerical component inventory plan is infeasible.");
		var results = native.PeekPlanResults().ToArray();
		ConfirmSourceArmour(token, caster, recipient);
		if (!token.Exempt)
		{
			if (results.Length != 1 || !Equals(results[0].OriginalReference, scope.ComponentReference) ||
				results[0].PrimaryTarget is not { } item || !ReferenceEquals(item.InInventoryOf, caster.Body) || item.ContainedIn is not null ||
				!item.IsA(rankTags[0]) || !item.IsA(rankTags[Math.Max(grade - 3, 0)]))
				throw new InvalidOperationException("Source Clerical component has ambiguous identity, custody or rank.");
			var tags = item.Tags.ToArray(); var protoTags = item.Prototype.Tags.ToArray();
			var hierarchy = tags.Concat(protoTags).SelectMany(tag => Ancestors(tag)).DistinctBy(tag => tag.Id)
				.Select(tag => new SourceArmourTag(tag, tag.Id, tag.Name, tag.Parent)).ToArray();
			token = token with { Item = new(item, item.Id, item.Prototype, item.GetItemType<IStackable>(),
				item.GetItemType<IStackable>()?.Quantity ?? 1, caster.Body, item.ContainedIn, item.Location, tags, protoTags, hierarchy) };
		}
		ConfirmSourceArmour(token, caster, recipient);
		_sourceArmourSelection = token; InstallSourceArmourPlan(invocation, token); return token;
	}

	private static IEnumerable<ITag> Ancestors(ITag tag)
	{
		var seen = new HashSet<ITag>(ReferenceEqualityComparer.Instance);
		for (var current = tag; current is not null; current = current.Parent)
		{
			if (!seen.Add(current)) throw new InvalidOperationException("Component rank hierarchy contains a cycle.");
			yield return current;
		}
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		try
		{
			if (selection is not SourceArmourSelection token || Spell is not MagicSpell invocation || recipient is not ICharacter)
				throw new InvalidOperationException("Source armour selection token is incompatible.");
			ConfirmSourceArmour(token, caster, recipient); _sourceArmourSelection = token; InstallSourceArmourPlan(invocation, token);
			error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		try
		{
			if (_sourceArmourError is { } invalid) throw new InvalidOperationException(invalid);
			if (_sourceArmourScope is not null) ConfirmSourceArmour(_sourceArmourSelection ?? throw new InvalidOperationException("Missing source armour selection."), caster, recipient);
			error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}

	private void ConfirmSourceArmour(SourceArmourSelection token, ICharacter caster, IPerceivable recipient, bool component = true)
	{
		var presence = caster.GetPlanarPresence().PresencePlaneIds.OrderBy(x => x).ToArray();
		if (_sourceArmourError is not null || _sourceArmourScope != token.Scope || Spell is not MagicSpell invocation ||
			invocation.InvocationGrade != token.Grade || !ReferenceEquals(caster, token.Caster) || !ReferenceEquals(recipient, token.Recipient) ||
			!ReferenceEquals(caster.Body, token.Body) || !ReferenceEquals(Gameworld.Planes.Get(token.Scope.WaterPlane), token.Plane) ||
			!presence.SequenceEqual(token.PresencePlanes) || token.Exempt != presence.Contains(token.Scope.WaterPlane) ||
			!ReferenceEquals(Gameworld.MagicSpells.Get(token.Source.Id), token.Source) ||
			!ReferenceEquals(token.Source.InventoryPlanTemplate, token.AuthoredPlan) ||
			token.AuthoredPlan.SaveToXml().ToString(SaveOptions.DisableFormatting) != token.AuthoredXml ||
			token.Source.SnapshotModel().Definition != token.SourceDefinition ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.EffectXml ||
			token.RankStates.Any(tag => !tag.Current(Gameworld)) ||
			component && token.Item is { } item && !item.Current(Gameworld))
			throw new InvalidOperationException("Frozen source armour environment, configuration or component changed.");
	}

	private InventoryPlanTemplate BuildSourceArmourPlan(SourceArmourSelection token, SourceArmourItem? pinned)
	{
		var xml = XElement.Parse(token.AuthoredXml);
		if (token.Exempt) xml.Descendants("Action").Single().Remove();
		var plan = new InventoryPlanTemplate(xml, Gameworld) { Options = token.AuthoredPlan.Options };
		if (!token.Exempt)
		{
			var action = (InventoryPlanActionConsume)plan.Phases.SelectMany(x => x.Actions).Single();
			action.BindSelectedGrade(token.Grade);
			action.PrimaryItemSelector = item =>
			{
				if (pinned is not null && !ReferenceEquals(item, pinned.Item)) return false;
				var allowed = item.IsA(token.RankTags[0]) && item.IsA(token.RankTags[Math.Max(token.Grade - 3, 0)]);
				ConfirmSourceArmour(token, token.Caster, token.Recipient, false);
				return allowed && ReferenceEquals(item.InInventoryOf, token.Body) && item.ContainedIn is null &&
					(pinned is null || pinned.Current(Gameworld));
			};
		}
		return plan;
	}

	private void InstallSourceArmourPlan(MagicSpell invocation, SourceArmourSelection token)
	{
		_sourceArmourConsumed = false; _sourceArmourFinished = false;
		invocation.InventoryPlanTemplate = new SourceArmourPlan(this, token, BuildSourceArmourPlan(token, token.Item));
	}

	private void ConfirmSourceArmourApplication(ICharacter caster, IPerceivable? target)
	{
		if (_sourceArmourError is { } error) throw new InvalidOperationException(error);
		if (_sourceArmourScope is null) return;
		if (target is not ICharacter) throw new InvalidOperationException("Source armour needs a character target.");
		var token = _sourceArmourSelection ?? throw new InvalidOperationException("Source armour requires prepared selection.");
		ConfirmSourceArmour(token, caster, target, false);
		if (!_sourceArmourFinished || !token.Exempt && !_sourceArmourConsumed)
			throw new InvalidOperationException("Source armour material execution was not observed.");
		if (!token.Exempt) ConfirmSourceArmourConsumption(token);
	}

	private void ConfirmSourceArmourConsumption(SourceArmourSelection token)
	{
		var before = token.Item ?? throw new InvalidOperationException("Source component was not selected.");
		var observed = before.Quantity > 1 && before.Stack is not null
			? before.Current(Gameworld, before.Quantity - 1)
			: before.Item.Deleted && Gameworld.Items.Get(before.Id) is null && before.Item.InInventoryOf is null &&
				before.Item.ContainedIn is null && !token.Caster.Inventory.Any(x => ReferenceEquals(x, before.Item)) &&
				!token.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, before.Item));
		if (!observed) throw new InvalidOperationException("Source armour component consumption was not proven.");
	}

	public new IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		ConfirmSourceArmourApplication(caster, target);
		return base.GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
	}

	private sealed class SourceArmourPlan(SpellArmourEffect effect, SourceArmourSelection token,
		InventoryPlanTemplate native) : IInventoryPlanTemplate
	{
		public IFuturemud Gameworld => native.Gameworld;
		public IEnumerable<IInventoryPlanPhaseTemplate> Phases => native.Phases;
		public IInventoryPlanPhaseTemplate FirstPhase => native.FirstPhase;
		public InventoryPlanOptions Options { get => native.Options; set => native.Options = value; }
		public XElement SaveToXml() => native.SaveToXml();
		public IInventoryPlan CreatePlan(ICharacter executor) => new InventoryPlan(executor, this);
		public IEnumerable<InventoryPlanActionResult> PeekPlanResults(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan) =>
			native.PeekPlanResults(executor, phase, plan);
		public InventoryPlanFeasibility PlanIsFeasible(ICharacter executor, IInventoryPlanPhase phase) => native.PlanIsFeasible(executor, phase);
		public IEnumerable<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> InfeasibleActions(ICharacter actor, IInventoryPlanPhase phase) =>
			native.InfeasibleActions(actor, phase);
		public void FinalisePlan(ICharacter executor, bool restore, IInventoryPlan plan, IList<IGameItem> exemptItems) =>
			native.FinalisePlan(executor, restore, plan, exemptItems);
		public IEnumerable<InventoryPlanActionResult> ExecutePhase(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan)
		{
			if (!ReferenceEquals(executor, token.Caster) || effect._sourceArmourFinished)
				throw new InvalidOperationException("Source armour phase cannot change caster or replay.");
			effect.ConfirmSourceArmour(token, executor, token.Recipient);
			var results = native.ExecutePhase(executor, phase, plan).ToArray();
			effect.ConfirmSourceArmour(token, executor, token.Recipient, false);
			if (!token.Exempt)
			{
				if (results.Length != 1 || results[0].ActionState != DesiredItemState.Consumed ||
					!Equals(results[0].OriginalReference, token.Scope.ComponentReference) ||
					!ReferenceEquals(results[0].PrimaryTarget, token.Item!.Item))
					throw new InvalidOperationException("Native source armour consumption returned an ambiguous result.");
				effect.ConfirmSourceArmourConsumption(token); effect._sourceArmourConsumed = true;
			}
			effect._sourceArmourFinished = true;
			return results;
		}
	}
}
