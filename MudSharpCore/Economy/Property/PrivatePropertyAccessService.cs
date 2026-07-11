#nullable enable

using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Economy.Property;

public static class PrivatePropertyAccessService
{
	public static PrivatePropertyEffect? EffectFor(ICell cell)
	{
		return cell.EffectsOfType<PrivatePropertyEffect>().FirstOrDefault();
	}

	public static PrivatePropertyAccessResult Evaluate(ICell cell, ICharacter actor)
	{
		var effect = EffectFor(cell);
		if (effect is null)
		{
			return new PrivatePropertyAccessResult(false, true, null,
				PrivatePropertyAccessReason.NotPrivate, "The location is not marked as private property.");
		}

		var controller = effect.Controller;
		if (controller is null || controller is IClan { IsTemplate: true })
		{
			return new PrivatePropertyAccessResult(true, false, controller,
				PrivatePropertyAccessReason.InvalidController, "The private-property controller is unavailable.");
		}

		if (actor.IsAdministrator())
		{
			return Allowed(controller, PrivatePropertyAccessReason.Administrator, "Administrators are authorised.");
		}

		if (actor.AffectedBy<PermitWork>(x =>
			    x.Cell == cell || x.Property == controller ||
			    x.Controller?.FrameworkItemType == controller.FrameworkItemType &&
			    x.Controller.Id == controller.Id))
		{
			return Allowed(controller, PrivatePropertyAccessReason.WorkPermit, "A current work permit authorises access.");
		}

		return controller switch
		{
			IProperty property => EvaluateProperty(property, cell, actor),
			IEmploymentHost host => EvaluateHost(host, actor),
			_ => new PrivatePropertyAccessResult(true, false, controller,
				PrivatePropertyAccessReason.InvalidController,
				$"{controller.Name} is not a supported private-property controller.")
		};
	}

	private static PrivatePropertyAccessResult EvaluateProperty(IProperty property, ICell cell, ICharacter actor)
	{
		if (property.IsAuthorisedOwner(actor))
		{
			return Allowed(property, PrivatePropertyAccessReason.Owner, "The character is an authorised property owner.");
		}

		if (property.PropertyOwners.Select(x => x.Owner).OfType<ICharacter>().Any(x => x.IsTrustedAlly(actor)))
		{
			return Allowed(property, PrivatePropertyAccessReason.TrustedAlly,
				"The character is a trusted ally of a property owner.");
		}

		if (property.IsAuthorisedLeaseHolder(actor))
		{
			return Allowed(property, PrivatePropertyAccessReason.Leaseholder,
				"The character is an authorised leaseholder.");
		}

		if (property.Lease?.Leaseholder is ICharacter leaseholder && leaseholder.IsTrustedAlly(actor))
		{
			return Allowed(property, PrivatePropertyAccessReason.TrustedAlly,
				"The character is a trusted ally of the leaseholder.");
		}

		if (property.Lease?.IsTenant(actor, false) == true)
		{
			return Allowed(property, PrivatePropertyAccessReason.Tenant, "The character is a declared tenant.");
		}

		if (property.HotelRoomForCell(cell)?.ActiveRental?.Guest == actor)
		{
			return Allowed(property, PrivatePropertyAccessReason.HotelGuest,
				"The character is the active guest for this room.");
		}

		return Denied(property);
	}

	private static PrivatePropertyAccessResult EvaluateHost(IEmploymentHost host, ICharacter actor)
	{
		if (host.HasActiveEmploymentContract(actor))
		{
			return Allowed(host, PrivatePropertyAccessReason.Employee,
				"The character has an active employment contract with the controller.");
		}

		return Denied(host);
	}

	private static PrivatePropertyAccessResult Allowed(IFrameworkItem controller,
		PrivatePropertyAccessReason reason, string explanation)
	{
		return new PrivatePropertyAccessResult(true, true, controller, reason, explanation);
	}

	private static PrivatePropertyAccessResult Denied(IFrameworkItem controller)
	{
		return new PrivatePropertyAccessResult(true, false, controller,
			PrivatePropertyAccessReason.Unauthorised,
			$"The character has no recognised access relationship with {controller.Name}.");
	}
}
