#nullable enable
using System.Collections.Generic;
using MudSharp.Construction.Grids;

namespace MudSharp.GameItems.Interfaces;

public interface ITelephoneNumberOwner : IGameItemComponent
{
	string? PhoneNumber { get; }
	string? PreferredNumber { get; set; }
	bool AllowSharedNumber { get; set; }
	ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
	IEnumerable<ITelephone> ConnectedTelephones { get; }
	void AssignPhoneNumber(string? number);
}
