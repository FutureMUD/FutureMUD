using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Framework.Scheduling;

#nullable enable
namespace MudSharp.GameItems.Components;

public abstract class ConditionalMagicItemGameItemComponent : GameItemComponent
{
	private ConditionalMagicItemGameItemComponentProto _prototype;
	protected ConditionalMagicItemPolicy Policy => _prototype.Policy;
	public override IGameItemComponentProto Prototype => _prototype;
	public int Charges { get; private set; }
	private DateTime _nextPulse = DateTime.MinValue;
	private bool _subscribed;
	private bool _wearHandled;
	public long CapabilityId => Policy.CapabilityId;
	public string BindingGroup => Policy.BindingGroup;

	protected ConditionalMagicItemGameItemComponent(ConditionalMagicItemGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		// New restorers are uncharged. A builder or authorised recharge must establish their fuel.
	}
	protected ConditionalMagicItemGameItemComponent(MudSharp.Models.GameItemComponent model, ConditionalMagicItemGameItemComponentProto proto, IGameItem parent) : base(model, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(model.Definition);
		Charges = (int?)root.Attribute("charges") ?? 0;
		if (Charges < 0) throw new FormatException("Invalid conditional item charge bank.");
		ClampChargesToCapacity();
		_nextPulse = (DateTime?)root.Attribute("nextPulse") ?? DateTime.MinValue;
	}
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto proto)
	{
		_prototype = (ConditionalMagicItemGameItemComponentProto)proto;
		ClampChargesToCapacity();
	}
	private void ClampChargesToCapacity()
	{
		// A builder may reduce capacity between revisions, including before a revised item is saved.
		var capacity = Math.Max(0, Policy.Capacity);
		if (Charges <= capacity) return;
		Charges = capacity;
		Changed = true;
	}
	protected override string SaveToXml() => new XElement("Definition", new XAttribute("version", 1), new XAttribute("charges", Charges), new XAttribute("nextPulse", _nextPulse.ToUniversalTime().ToString("O"))).ToString();
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override void Login()
	{
		base.Login();
		if (!_subscribed && Policy.Kind == ConditionalMagicItemKind.ConditionalBinding)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += Pulse;
			Parent.OnInventoryChange += InventoryChanged;
			_subscribed = true;
		}
	}
	private void Stop()
	{
		if (!_subscribed) return;
		Gameworld.HeartbeatManager.SecondHeartbeat -= Pulse;
		Parent.OnInventoryChange -= InventoryChanged;
		_subscribed = false;
	}
	public override void Quit() { Stop(); base.Quit(); }
	public override void Delete() { Stop(); base.Delete(); }
	private bool Allowed(long prog, ICharacter actor) => PatronMagicService.Allowed(Gameworld, prog, actor);
	private bool Valid => Policy.Errors().Count == 0 && _prototype.CanSubmit();
	public bool Accessible(ICharacter actor) => Valid && !Parent.Deleted && !Parent.Destroyed &&
		actor.Location is not null && Parent.TrueLocations.Contains(actor.Location) && Parent.RoomLayer == actor.RoomLayer &&
		actor.CanSee(Parent) && ItemManipulationGuard.CanManipulate(actor, out _, Parent) &&
		(!Policy.RequireWorn || actor.Body.DirectWornItems.Contains(Parent));
	public bool IsBinding(ICharacter wearer) => Policy.Kind == ConditionalMagicItemKind.ConditionalBinding && Valid &&
		!wearer.State.IsDead() && Allowed(Policy.EligibilityProgId, wearer) && wearer.Body.DirectWornItems.Contains(Parent) &&
		(Policy.WearLocations.Length == 0 || wearer.Body.WornItemsFullInfo.Any(x => x.Item == Parent && Policy.WearLocations.Any(y => y.EqualTo(x.Wearloc.Name))));
	public bool PreventRemoval(ICharacter wearer, bool applyConsequence)
	{
		if (wearer.IsAdministrator() || !Policy.PreventRemoval || !IsBinding(wearer)) return false;
		if (applyConsequence) Apply(wearer, Policy.Removal);
		return true;
	}
	private bool Apply(ICharacter target, ItemConsequence consequence)
	{
		if (!ConfiguredItemConsequence.Apply(target, consequence)) return false;
		if (consequence.DestroyItem) Parent.Delete();
		return true;
	}
	public bool InterceptSpell(ICharacter wearer, IMagicSpell spell, IReadOnlyList<IPerceivable> targets)
	{
		if (!Policy.InterceptHostileSpells || !IsBinding(wearer)) return false;
		var mood = (spell as IControlledMagicSpell)?.GradeProfile?.Incantation?.Mood;
		if (!Policy.InterceptSpellIds.Contains(spell.Id) && (mood is null || !Policy.InterceptMoods.Any(x => x.EqualTo(mood)))) return false;
		if (!targets.OfType<ICharacter>().Any(x => Allowed(Policy.ProtectedTargetProgId, x))) return false;
		Apply(wearer, Policy.Removal);
		return true;
	}
	private bool CanControl(ICharacter controller) => Allowed(Policy.ControllerProgId, controller) &&
		(Policy.RequiredRegaliaPrototypeId == 0 || controller.Body.DirectWornItems.Any(x => x.Prototype.Id == Policy.RequiredRegaliaPrototypeId));
	public bool Activate(ICharacter controller, ICharacter wearer, out string reason)
	{
		reason = "You cannot exercise authority over that binding.";
		var controllerBody = controller.Body;
		var wearerBody = wearer.Body;
		var controllerLocation = controller.Location;
		var wearerLocation = wearer.Location;
		if (!CanControl(controller) || !IsBinding(wearer) || !Gameworld.Actors.Contains(wearer) || controller.State.IsDead() || controller.State.IsInStatis()) return false;
		// Progs may execute arbitrary game actions. Check physical custody again after all callbacks.
		if (!ReferenceEquals(controller.Body, controllerBody) || !ReferenceEquals(wearer.Body, wearerBody) ||
			controller.Location != controllerLocation || wearer.Location != wearerLocation || Parent.Deleted || Parent.Destroyed ||
			!wearerBody.DirectWornItems.Contains(Parent) ||
			Policy.RequiredRegaliaPrototypeId != 0 && !controllerBody.DirectWornItems.Any(x => x.Prototype.Id == Policy.RequiredRegaliaPrototypeId)) return false;
		var range = Policy.Range switch
		{
			BindingActivationRange.SameRoom => controller.ColocatedWith(wearer),
			BindingActivationRange.SameZone => controller.Location?.Zone == wearer.Location?.Zone,
			BindingActivationRange.SameShard => controller.Location?.Shard == wearer.Location?.Shard,
			BindingActivationRange.World => true,
			_ => false
		};
		if (!range || !Apply(wearer, Policy.Activation)) return false;
		if (Policy.ActionDelaySeconds > 0) controller.AddEffect(new CommandDelay(controller, "itempower"), TimeSpan.FromSeconds(Policy.ActionDelaySeconds));
		reason = Policy.ActivationMessage;
		return true;
	}
	private void InventoryChanged(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		_wearHandled = false;
		if (newState == InventoryState.Worn) HandleWear();
	}
	private ICharacter? Wearer => Parent.InInventoryOf?.Actor;
	private void HandleWear()
	{
		if (_wearHandled || Wearer is not { } wearer || !wearer.Body.DirectWornItems.Contains(Parent)) return;
		_wearHandled = true;
		if (Allowed(Policy.EligibilityProgId, wearer) && Allowed(Policy.IncompatibleWearerProgId, wearer)) Apply(wearer, Policy.IncompatibleWear);
	}
	private void Pulse()
	{
		if (!Valid || Parent.Deleted || Parent.Destroyed) return;
		HandleWear();
		var now = RuntimeClock.UtcNow;
		if (now < _nextPulse) return;
		_nextPulse = now.AddSeconds(Policy.IntervalSeconds + (Random.Shared.NextDouble() * 2 - 1) * Policy.IntervalJitterSeconds);
		Changed = true;
		if (Wearer is not { } wearer || !IsBinding(wearer) || wearer.State.IsInStatis() || Policy.TerritoryProgId == 0) return;
		if (wearer.Following is ICharacter escort && Allowed(Policy.EscortProgId, escort)) return;
		var inside = Allowed(Policy.TerritoryProgId, wearer);
		if (inside == Policy.PunishOutsideTerritory || Random.Shared.NextDouble() >= Policy.ActivationChance) return;
		Apply(wearer, Policy.Periodic);
	}
	public bool SelectSpells(ICharacter actor, IReadOnlyList<long> spells, out string reason)
	{
		reason = "You cannot use that shrine here.";
		return Policy.Kind == ConditionalMagicItemKind.SpellSelectionShrine && Accessible(actor) &&
			PatronMagicService.Select(actor, Policy, spells, out reason, () => Accessible(actor));
	}
	public string ShowChoices(ICharacter actor) => string.Join("\n", Policy.Choices.Select(x => $"{x.Category}: {Gameworld.MagicSpells.Get(x.SpellId)?.Name ?? "missing spell"} (#{x.SpellId})"));
	public bool Use(ICharacter actor, string action, StringStack arguments, out string reason)
	{
		reason = "You cannot use this item here, or lack its configured authority.";
		var body = actor.Body;
		var location = actor.Location;
		var custody = Parent.InInventoryOf;
		bool StillAccessible() => ReferenceEquals(actor.Body, body) && actor.Location == location &&
			ReferenceEquals(Parent.InInventoryOf, custody) && Accessible(actor);
		if (!Accessible(actor) || !Allowed(Policy.EligibilityProgId, actor) || !StillAccessible()) return false;
		if (Policy.Kind == ConditionalMagicItemKind.SpellSelectionShrine)
		{
			if (action.EqualTo("list")) { reason = ShowChoices(actor); return true; }
			if (!action.EqualTo("select")) return false;
			var spells = new List<long>();
			while (!arguments.IsFinished)
			{
				var spell = Gameworld.MagicSpells.GetByIdOrName(arguments.PopSpeech());
				if (spell is null) { reason = "Choose spell IDs or quoted spell names from this shrine's list."; return false; }
				spells.Add(spell.Id);
			}
			return SelectSpells(actor, spells, out reason);
		}
		if (Policy.Kind == ConditionalMagicItemKind.ResourceRestorer)
		{
			if (action.EqualTo("status")) { reason = $"This item has {Charges.ToString("N0", actor)} of {Policy.Capacity.ToString("N0", actor)} charges."; return true; }
			if (action.EqualTo("recharge") && Allowed(Policy.TerritoryProgId, actor) && StillAccessible())
			{
				Charges = Policy.Capacity; Changed = true; reason = Policy.RechargeMessage; return true;
			}
			var owner = MagicCastingService.Owner(actor);
			if (!action.EqualTo("use") || Charges <= 0 || Gameworld.MagicResources.Get(Policy.ResourceId) is not { } resource ||
				!MagicResourceCapacity.TryGetCap(resource, owner, out var cap, out reason)) return false;
			var balance = owner.MagicResourceAmounts.GetValueOrDefault(resource);
			if (!double.IsFinite(balance) || balance >= cap) { reason = "Your reserve is already full or unavailable."; return false; }
			if (!StillAccessible() || !ReferenceEquals(owner, MagicCastingService.Owner(actor))) return false;
			Charges--; Changed = true;
			owner.AddResource(resource, cap - balance);
			reason = Policy.ActivationMessage;
			return true;
		}
		if (Policy.Kind == ConditionalMagicItemKind.ItemIssuer && action.EqualTo("issue") &&
			Gameworld.ItemProtos.GetByIdOrName(arguments.SafeRemainingArgument) is { } proto && Policy.IssuedPrototypeIds.Contains(proto.Id))
		{
			if (!StillAccessible()) return false;
			var item = proto.CreateNew(actor);
			Gameworld.Add(item);
			item.Login(); item.HandleEvent(MudSharp.Events.EventType.ItemFinishedLoading, item);
			if (actor.Body.CanGet(item, 0)) actor.Body.Get(item, silent: true);
			else actor.Location.Insert(item);
			reason = Policy.ActivationMessage;
			return true;
		}
		return false;
	}
}
