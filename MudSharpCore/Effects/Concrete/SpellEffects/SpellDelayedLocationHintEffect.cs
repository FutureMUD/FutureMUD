#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellDelayedLocationHintEffect : MagicSpellEffectBase, IIndependentlyExpiringSpellEffect
{
	private bool _registered;
	private bool _completed;
	public long AnchorRoomId { get; }
	public long BodyId { get; }
	public int MaximumRange { get; }
	public DateTime? ExpiryUtc { get; }
	public SpellDelayedLocationHintEffect(ICharacter owner, IMagicSpellEffectParent parent, long anchorRoomId, long bodyId,
		TimeSpan delay, int maximumRange) : base(owner, parent, null!)
	{
		AnchorRoomId = anchorRoomId; BodyId = bodyId; MaximumRange = maximumRange; ExpiryUtc = RuntimeClock.UtcNow + delay;
	}
	private SpellDelayedLocationHintEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		AnchorRoomId = (long)root.Element("Room")!; BodyId = (long)root.Element("Body")!;
		MaximumRange = Math.Clamp((int)root.Element("Range")!, 1, 140);
		ExpiryUtc = DateTime.Parse(root.Element("Deadline")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();
	}
	public static void InitialiseEffectType() => RegisterFactory("SpellDelayedLocationHint", (xml, owner) => new SpellDelayedLocationHintEffect(xml, owner));
	public override void InitialEffect() => Login();
	public override void Login()
	{
		if (_registered || Owner is not ICharacter actor) return;
		_registered = true; Gameworld.HeartbeatManager.SecondHeartbeat += CheckHint;
		actor.OnQuit += Cancel; actor.OnDeath += Cancel; actor.OnDeleted += Cancel; actor.CurrentBodyChanged += ChangedBody;
	}
	private void Cancel(IPerceivable owner) => Owner.RemoveEffect(this, true);
	private void ChangedBody(ICharacter actor, IBody before, IBody after) => Cancel(actor);
	public void CheckHint()
	{
		if (_completed || RuntimeClock.UtcNow < ExpiryUtc) return;
		_completed = true;
		if (Owner is ICharacter actor && actor.Body.Id == BodyId && ReferenceEquals(actor.Body.Actor, actor) &&
			!actor.State.HasFlag(CharacterState.Dead) && Owner.Effects.Contains(this) && Owner.Effects.Contains(ParentEffect))
		{
			var anchor = Gameworld.Rooms.Get(AnchorRoomId);
			IReadOnlyList<MudSharp.Construction.Boundary.IRoomExit>? path = null;
			try { if (anchor is not null) path = SpellPermittedPath.Find(actor, actor.Location, anchor, MaximumRange); }
			catch (InvalidOperationException) { /* An over-budget route is unavailable, not a heartbeat failure. */ }
			var hint = path is null ? "only a distant blankness" : path.Count == 0 ? anchor!.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee) :
				$"{string.Join(", then ", path.Select(x => x.OutboundDirectionDescription))}, toward {anchor!.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}";
			actor.OutputHandler.Send($"A pulse of remembered magic reveals {hint}.");
		}
		Owner.RemoveEffect(this, true);
	}
	public override void RemovalEffect()
	{
		_completed = true;
		if (_registered && Owner is ICharacter actor)
		{
			_registered = false; Gameworld.HeartbeatManager.SecondHeartbeat -= CheckHint;
			actor.OnQuit -= Cancel; actor.OnDeath -= Cancel; actor.OnDeleted -= Cancel; actor.CurrentBodyChanged -= ChangedBody;
		}
		base.RemovalEffect();
	}
	protected override string SpecificEffectType => "SpellDelayedLocationHint";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Room", AnchorRoomId), new XElement("Body", BodyId),
		new XElement("Range", MaximumRange), new XElement("Deadline", ExpiryUtc!.Value.ToString("O", CultureInfo.InvariantCulture)));
	public override string Describe(IPerceiver voyeur) => $"A delayed route memory of room {AnchorRoomId.ToString("N0", voyeur).ColourValue()}.";
}
