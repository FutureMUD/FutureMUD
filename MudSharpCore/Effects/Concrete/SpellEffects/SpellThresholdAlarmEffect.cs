#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>One generic owner notice for each native room-arrival witness event.</summary>
public sealed class SpellThresholdAlarmEffect : SimpleSpellStatusEffectBase, IHandleEventsEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellThresholdAlarm",
		(root, owner) => new SpellThresholdAlarmEffect(root, owner));

	public SpellThresholdAlarmEffect(IRoom room, IMagicSpellEffectParent parent, ICharacter caster)
		: base(room, parent)
	{
		OwnerId = caster.Id;
		OwnerInstanceId = caster.InstanceId;
		OwnerIdentityId = CharacterInstanceIdentityComparer.IdentityId(caster);
	}

	private SpellThresholdAlarmEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect") ?? throw new FormatException("Missing threshold alarm data.");
		OwnerId = (long?)xml.Element("OwnerId") ?? 0;
		OwnerInstanceId = (long?)xml.Element("OwnerInstanceId") ?? 0;
		OwnerIdentityId = (long?)xml.Element("OwnerIdentityId") ?? 0;
	}

	public long OwnerId { get; }
	public long OwnerInstanceId { get; }
	public long OwnerIdentityId { get; }
	public bool IsActive => Owner is IRoom room && OwnerId > 0 && OwnerInstanceId > 0 &&
		OwnerIdentityId > 0 && ParentEffect is not null && ReferenceEquals(ParentEffect.Owner, room) &&
		room.Effects.Any(x => ReferenceEquals(x, this)) && room.Effects.Any(x => ReferenceEquals(x, ParentEffect)) &&
		ParentEffect.SpellEffects.Any(x => ReferenceEquals(x, this));

	public bool HandlesEvent(params EventType[] types) =>
		types.Contains(EventType.CharacterEnterRoomWitness);

	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.CharacterEnterRoomWitness || !IsActive || Owner is not IRoom room ||
			arguments.Length < 2 || arguments[0] is not ICharacter mover ||
			arguments[1] is not IRoom entered || !ReferenceEquals(entered, room) ||
			!ReferenceEquals(mover.Location, room) ||
			mover.Id == OwnerId && mover.InstanceId == OwnerInstanceId)
			return false;

		// Only a loaded owner receives a notice. Do not materialise an offline character or reveal the mover.
		var recipient = Gameworld.Characters.Get(OwnerId) ?? Gameworld.Actors.Get(OwnerId);
		if (recipient is null || recipient.InstanceId != OwnerInstanceId ||
			CharacterInstanceIdentityComparer.IdentityId(recipient) != OwnerIdentityId)
			return false;
		recipient.OutputHandler.Send("Your threshold alarm senses someone entering its room.");
		return false;
	}

	protected override XElement SaveDefinition() => SimpleSaveDefinition(
		new XElement("OwnerId", OwnerId), new XElement("OwnerInstanceId", OwnerInstanceId),
		new XElement("OwnerIdentityId", OwnerIdentityId));
	public override string Describe(IPerceiver voyeur) => "A threshold alarm watches for arrivals.";
	protected override string SpecificEffectType => "SpellThresholdAlarm";
}
