#nullable enable

using MudSharp.Framework;

namespace MudSharp.Economy.Property;

public enum PrivatePropertyAccessReason
{
	NotPrivate,
	Administrator,
	Owner,
	TrustedAlly,
	Leaseholder,
	Tenant,
	HotelGuest,
	Employee,
	WorkPermit,
	Unauthorised,
	InvalidController
}

public sealed record PrivatePropertyAccessResult(
	bool IsPrivateProperty,
	bool IsAuthorised,
	IFrameworkItem? Controller,
	PrivatePropertyAccessReason Reason,
	string Explanation);
