#nullable enable

namespace MudSharp.Vehicles;

public static class VehicleMotiveAuthority
{
	public static bool CanControl(ICharacter actor, ICharacter source)
	{
		return source == actor ||
		       source.IsTrustedAlly(actor) ||
		       source.IsHelpless ||
		       source.IsPrimaryRider(actor) && source.PermitControl(actor) ||
		       source.CanBeMountedBy(actor) && source.PermitControl(actor);
	}
}
