#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Magic;
using MudSharp.NPC.AI;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>A resisted grant for bounded IC orders, tied to two exact physical instances.</summary>
public sealed class SpellCompellingRegardEffect : MagicSpellEffectBase, IIndependentlyExpiringSpellEffect
{
	private readonly long _controllerId, _controllerInstance, _controllerBody;
	private readonly long _targetId, _targetInstance, _targetBody, _undead;
	private ICharacter? _controller;
	private bool _registered;
	public bool AllowPcs { get; }
	public DateTime? ExpiryUtc { get; }
	public SpellCompellingRegardEffect(ICharacter target, IMagicSpellEffectParent parent, ICharacter controller, bool allowPcs, long undead, DateTime expiry)
		: base(target, parent, null)
	{
		_controllerId = CharacterInstanceIdentityComparer.IdentityId(controller); _controllerInstance = controller.InstanceId; _controllerBody = controller.Body.Id;
		_targetId = CharacterInstanceIdentityComparer.IdentityId(target); _targetInstance = target.InstanceId; _targetBody = target.Body.Id;
		AllowPcs = allowPcs; _undead = undead; ExpiryUtc = expiry; _controller = controller;
	}
	private SpellCompellingRegardEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		_controllerId = (long)root.Element("Controller")!; _controllerInstance = (long)root.Element("ControllerInstance")!; _controllerBody = (long)root.Element("ControllerBody")!;
		_targetId = (long)root.Element("Target")!; _targetInstance = (long)root.Element("TargetInstance")!; _targetBody = (long)root.Element("TargetBody")!;
		_undead = (long)root.Element("UndeadMerit")!; AllowPcs = (bool)root.Element("AllowPcs")!;
		ExpiryUtc = DateTime.Parse(root.Element("ExpiryUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();
		if (new[] { _controllerId, _controllerInstance, _controllerBody, _targetId, _targetInstance, _targetBody, _undead }.Any(x => x <= 0))
			throw new FormatException("Invalid exact Compelling Regard binding.");
	}
	public static void InitialiseEffectType() => RegisterFactory("SpellCompellingRegard", (xml, owner) => new SpellCompellingRegardEffect(xml, owner));
	private bool SameController(ICharacter actor) => CharacterInstanceIdentityComparer.IdentityId(actor) == _controllerId && actor.InstanceId == _controllerInstance && actor.Body.Id == _controllerBody;
	public bool IsActive => Owner is ICharacter target && RuntimeClock.UtcNow < ExpiryUtc &&
		target.InstanceId == _targetInstance && CharacterInstanceIdentityComparer.IdentityId(target) == _targetId && target.Body.Id == _targetBody &&
		(AllowPcs || !target.IsPlayerCharacter) && !target.IsAdministrator() && !target.AffectedBy<IIgnoreForceEffect>() && !target.Merits.Any(x => x.Id == _undead) &&
		CommandExecutionAuthority.IsCurrent(target, false) && ParentEffect is { } parent && ReferenceEquals(parent.Owner, target) &&
		target.Effects.Any(x => ReferenceEquals(x, this)) && target.Effects.Any(x => ReferenceEquals(x, parent)) && parent.SpellEffects.Any(x => ReferenceEquals(x, this));
	public bool CanControl(ICharacter controller)
	{
		if (Owner is not ICharacter target) return false;
		var controllerBody = controller.Body; var targetBody = target.Body; var room = target.Location; var layer = target.RoomLayer;
		bool Physical() => SameController(controller) && ReferenceEquals(controller.Gameworld, Gameworld) &&
			ReferenceEquals(controller.Body, controllerBody) && ReferenceEquals(target.Body, targetBody) &&
			ReferenceEquals(controller.Location, room) && ReferenceEquals(target.Location, room) && controller.RoomLayer == layer && target.RoomLayer == layer &&
			CommandExecutionAuthority.IsCurrent(controller, false) && CommandExecutionAuthority.IsCurrent(target, false);
		return Physical() && IsActive && Physical() && controller.CanSee(target) && Physical() &&
			MagicInterdictionHelper.GetInterdiction(controller, target, Spell.School, false) is null &&
			Physical() && IsActive && Physical() && controller.CanSee(target) && Physical();
	}
	public override void InitialEffect() => Login();
	public override void Login()
	{
		if (_registered || Owner is not ICharacter target) return;
		_controller ??= Gameworld.Actors.SingleOrDefault(SameController);
		if (_controller is null) { Owner.RemoveEffect(this, true); return; }
		_registered = true;
		foreach (var actor in new[] { target, _controller })
		{ actor.OnQuit += Lost; actor.OnDeath += Lost; actor.OnDeleted += Lost; actor.CurrentBodyChanged += ChangedBody; }
	}
	private void Lost(IPerceivable actor) => Owner.RemoveEffect(this, true);
	private void ChangedBody(ICharacter actor, IBody before, IBody after) => Lost(actor);
	public override void RemovalEffect()
	{
		if (_registered && Owner is ICharacter target && _controller is { } controller)
		{
			_registered = false;
			foreach (var actor in new[] { target, controller })
			{ actor.OnQuit -= Lost; actor.OnDeath -= Lost; actor.OnDeleted -= Lost; actor.CurrentBodyChanged -= ChangedBody; }
		}
		base.RemovalEffect();
	}
	protected override string SpecificEffectType => "SpellCompellingRegard";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Controller", _controllerId), new XElement("ControllerInstance", _controllerInstance),
		new XElement("ControllerBody", _controllerBody), new XElement("Target", _targetId), new XElement("TargetInstance", _targetInstance), new XElement("TargetBody", _targetBody),
		new XElement("UndeadMerit", _undead), new XElement("AllowPcs", AllowPcs), new XElement("ExpiryUtc", ExpiryUtc!.Value.ToString("O", CultureInfo.InvariantCulture)));
	public override string Describe(IPerceiver voyeur) => "Bound by Compelling Regard to accept permitted in-character orders.";
}
