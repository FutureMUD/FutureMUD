using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Shape;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// Runtime-only movement watcher for an accepted table participant. Session membership is
/// persisted separately; this effect is intentionally transient so a stale movement subscription
/// can never be restored after a reboot.
/// </summary>
public sealed class RestaurantTableParticipantEffect : Effect
{
	private readonly Restaurant _restaurant;

	public RestaurantTableParticipantEffect(ICharacter owner, Restaurant restaurant, long sessionId) : base(owner)
	{
		_restaurant = restaurant;
		RestaurantId = restaurant.Id;
		SessionId = sessionId;
		Subscribe();
	}

	public long RestaurantId { get; }
	public long SessionId { get; }
	protected override string SpecificEffectType => "RestaurantTableParticipant";

	public override string Describe(IPerceiver voyeur)
	{
		return "Participating in restaurant table service.";
	}

	public override void RemovalEffect()
	{
		Unsubscribe();
		base.RemovalEffect();
	}

	private void Subscribe()
	{
		if (Owner is not ICharacter character)
		{
			return;
		}

		character.OnLocationChanged -= CharacterOnLocationChanged;
		character.OnLocationChanged += CharacterOnLocationChanged;
	}

	private void Unsubscribe()
	{
		if (Owner is ICharacter character)
		{
			character.OnLocationChanged -= CharacterOnLocationChanged;
		}
	}

	private void CharacterOnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		if (Owner is ICharacter character)
		{
			_restaurant.NotifyParticipantLocationChanged(SessionId, character);
		}
	}
}
