#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellReturnTetherEffect : MagicSpellEffectBase, IIndependentlyExpiringSpellEffect
{
	private bool _registered;
	private bool _checking;
	public SpellReturnBinding Binding { get; }
	public DateTime? ExpiryUtc => Binding.DeadlineUtc;
	public static void InitialiseEffectType() => RegisterFactory("SpellReturnTether", (xml, owner) => new SpellReturnTetherEffect(xml, owner));
	public SpellReturnTetherEffect(ICharacter owner, IMagicSpellEffectParent parent, SpellReturnBinding binding) : base(owner, parent, null)
	{ Binding = binding; }
	private SpellReturnTetherEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		Binding = new(Guid.Parse(root.Element("Origin")!.Value), SpellReturnAnchor.Parse(root.Element("Anchor")!.Value),
			DateTime.Parse(root.Element("DeadlineUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime());
	}
	public override void InitialEffect() => Login();
	public override void Login()
	{
		if (_registered || Owner is not ICharacter actor) return;
		_registered = true;
		Gameworld.HeartbeatManager.SecondHeartbeat += CheckReturn;
		actor.OnQuit += Quit; actor.OnDeath += Died; actor.OnDeleted += Died; actor.CurrentBodyChanged += ChangedBody;
	}
	private bool Attached() => Owner is ICharacter actor && ReferenceEquals(actor.Body.Actor, actor) &&
		actor.Body.Id == Binding.Anchor.BodyId && ParentEffect is { } parent && ReferenceEquals(parent.Owner, Owner) &&
		Owner.Effects.Any(x => ReferenceEquals(x, this)) && Owner.Effects.Any(x => ReferenceEquals(x, parent)) &&
		parent.SpellEffects.Any(x => ReferenceEquals(x, this));
	public void CheckReturn()
	{
		if (_checking || Owner is not ICharacter actor || Gameworld.SpellReturns is not { } service) return;
		_checking = true;
		try
		{
			if (RuntimeClock.UtcNow < Binding.DeadlineUtc) return;
			if (service.TryReturn(Binding, actor, Spell, Attached)) Owner.RemoveEffect(this, true);
		}
		finally { _checking = false; }
	}
	private void Quit(IPerceivable owner) => Stop(SpellRetirementReason.Logout);
	private void Died(IPerceivable owner) => Stop(SpellRetirementReason.EarlyDeath);
	private void ChangedBody(ICharacter actor, IBody before, IBody after) => Stop(SpellRetirementReason.AnchorSevered);
	private void Stop(SpellRetirementReason reason)
	{
		if (Owner is ICharacter actor) Gameworld.SpellReturns?.Cancel(Binding, actor, reason);
		Owner.RemoveEffect(this, true);
	}
	public override void RemovalEffect()
	{
		if (_registered && Owner is ICharacter actor)
		{
			_registered = false; Gameworld.HeartbeatManager.SecondHeartbeat -= CheckReturn;
			actor.OnQuit -= Quit; actor.OnDeath -= Died; actor.OnDeleted -= Died; actor.CurrentBodyChanged -= ChangedBody;
			Gameworld.SpellReturns?.Cancel(Binding, actor, SpellRetirementReason.Dispel);
		}
		base.RemovalEffect();
	}
	protected override string SpecificEffectType => "SpellReturnTether";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Origin", Binding.Id),
		new XElement("Anchor", new XCData(Binding.Anchor.Save())), new XElement("DeadlineUtc", Binding.DeadlineUtc.ToString("O", CultureInfo.InvariantCulture)));
	public override string Describe(IPerceiver voyeur) => $"A return tether to room {Binding.Anchor.RoomId.ToString("N0", voyeur).ColourValue()}, due {Binding.DeadlineUtc.ToString("u", voyeur).ColourValue()}.";
}
