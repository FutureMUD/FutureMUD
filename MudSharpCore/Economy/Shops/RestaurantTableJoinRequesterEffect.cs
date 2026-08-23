using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using System;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// A transient movement watcher for a diner asking to join an occupied table. A pending request
/// ends immediately when its requester leaves the customer-facing restaurant boundary.
/// </summary>
public sealed class RestaurantTableJoinRequesterEffect : Effect
{
	private readonly Restaurant _restaurant;

	public RestaurantTableJoinRequesterEffect(ICharacter owner, Restaurant restaurant, Guid requestId) : base(owner)
	{
		_restaurant = restaurant;
		RequestId = requestId;
		Subscribe();
	}

	public Guid RequestId { get; }
	protected override string SpecificEffectType => "RestaurantTableJoinRequester";

	public override string Describe(IPerceiver voyeur)
	{
		return "Awaiting acceptance to join a restaurant table.";
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
			_restaurant.NotifyJoinRequesterLocationChanged(RequestId, character);
		}
	}
}
