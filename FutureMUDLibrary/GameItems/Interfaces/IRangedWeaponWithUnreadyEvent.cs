using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces;

public interface IRangedWeaponWithUnreadyEvent : IRangedWeapon
{
	event PerceivableEvent OnUnready;
}