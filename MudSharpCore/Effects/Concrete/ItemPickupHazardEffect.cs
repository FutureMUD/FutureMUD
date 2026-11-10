#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

/// <summary>A permanent, saved one-shot pickup hazard with optional scripted immunity.</summary>
public sealed class ItemPickupHazardEffect : Effect, IItemPickupAttemptEffect, IIntrinsicItemMagicEffect
{
	private bool _spent;
	public ItemPickupHazardEffect(IGameItem owner, string damageDice, string stunDice, long immunityProgId = 0,
		int minimumDispelGrade = 1, string echo = "An enchantment flares as you try to take it.") : base(owner)
	{
		DamageDice = damageDice; StunDice = stunDice; ImmunityProgId = immunityProgId; MinimumDispelGrade = minimumDispelGrade;
		Echo = echo;
		Validate();
	}
	private ItemPickupHazardEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Hazard")!;
		DamageDice = (string)root.Element("DamageDice")!;
		StunDice = (string)root.Element("StunDice")!;
		ImmunityProgId = (long?)root.Element("ImmunityProg") ?? 0;
		MinimumDispelGrade = (int?)root.Element("MinimumDispelGrade") ?? 1;
		Echo = (string?)root.Element("Echo") ?? "An enchantment flares as you try to take it.";
		_spent = (bool?)root.Element("Spent") ?? false;
		Validate();
	}
	private void Validate()
	{
		if (!Dice.TryValidateDiceExpression(DamageDice, out _) || !Dice.TryValidateDiceExpression(StunDice, out _) ||
			ImmunityProgId < 0 || MinimumDispelGrade < 1)
			throw new FormatException("Invalid pickup-hazard configuration.");
	}
	public string DamageDice { get; }
	public string StunDice { get; }
	public long ImmunityProgId { get; }
	public string Echo { get; }
	public int MinimumDispelGrade { get; }
	public int DispelPriority => 20;
	public bool IsIntrinsicMagicActive => !_spent;
	public bool StopsDispelOnInsufficientGrade => true;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "ItemPickupHazard";
	protected override XElement SaveDefinition() => new("Hazard", new XElement("DamageDice", DamageDice),
		new XElement("StunDice", StunDice), new XElement("ImmunityProg", ImmunityProgId),
		new XElement("MinimumDispelGrade", MinimumDispelGrade), new XElement("Spent", _spent), new XElement("Echo", new XCData(Echo)));
	public override bool PreventsItemFromMerging(IGameItem item, IGameItem otherItem) =>
		otherItem.Effects.OfType<ItemPickupHazardEffect>().All(x => x.DamageDice != DamageDice || x.StunDice != StunDice ||
			x.ImmunityProgId != ImmunityProgId || x.MinimumDispelGrade != MinimumDispelGrade || x._spent != _spent || x.Echo != Echo);
	public bool HandlePickupAttempt(ICharacter actor)
	{
		if (_spent) return true;
		if (Owner is not IGameItem item || item.Deleted || item.Destroyed) return false;
		var wasAttached = item.Effects.Contains(this);
		var holder = item.InInventoryOf; var container = item.ContainedIn; var location = item.Location;
		var actorBody = actor.Body; var actorLocation = actor.Location; var actorLayer = actor.RoomLayer;
		bool Current() => !item.Deleted && !item.Destroyed && ReferenceEquals(item.InInventoryOf, holder) &&
			ReferenceEquals(item.ContainedIn, container) && ReferenceEquals(item.Location, location) &&
			ReferenceEquals(actor.Body, actorBody) && ReferenceEquals(actor.Location, actorLocation) && actor.RoomLayer == actorLayer;
		if (ImmunityProgId > 0)
		{
			var prog = Owner.Gameworld.FutureProgs.Get(ImmunityProgId);
			if (prog is null || prog.ReturnType != MudSharp.FutureProg.ProgVariableTypes.Boolean ||
				!prog.MatchesParameters([MudSharp.FutureProg.ProgVariableTypes.Character, MudSharp.FutureProg.ProgVariableTypes.Item])) return false;
			try
			{
				if (!prog.ExecuteWithStatus(out var immune, actor, Owner) || immune is not bool value) return false;
				if (!Current()) return false;
				if (_spent || wasAttached && !item.Effects.Contains(this)) return true;
				if (value) return true;
			}
			catch { return false; }
		}
		_spent = true;
		Changed = true;
		foreach (var aura in Owner.Effects.OfType<ItemMagicAura>().ToArray()) Owner.RemoveEffect(aura);
		if (!string.IsNullOrEmpty(Echo)) actor.OutputHandler.Send(Echo);
		var damage = Math.Max(0, Dice.Roll(DamageDice));
		var stun = Math.Max(0, Dice.Roll(StunDice));
		if (damage > 0 || stun > 0) HitPointChanges.Apply(actor, -damage, stun);
		return false;
	}
	public bool TryDispel(int grade)
	{
		if (_spent || grade < MinimumDispelGrade) return false;
		_spent = true; Changed = true;
		return true;
	}
	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem) =>
		new ItemPickupHazardEffect(newItem, DamageDice, StunDice, ImmunityProgId, MinimumDispelGrade, Echo) { _spent = this._spent };
	public override string Describe(IPerceiver voyeur) => _spent ? "An expended pickup hazard." : $"Armed pickup hazard: {DamageDice} HP and {StunDice} stun.";
	public static void InitialiseEffectType() => RegisterFactory("ItemPickupHazard", (xml, owner) => new ItemPickupHazardEffect(xml, owner));
}
