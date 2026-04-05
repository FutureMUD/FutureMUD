#nullable enable
using MudSharp.Construction.Grids;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface ITelephoneNumberOwner : IGameItemComponent
{
    string? PhoneNumber { get; }
    string? PreferredNumber { get; set; }
    bool AllowSharedNumber { get; set; }
    bool HostedVoicemailEnabled { get; set; }
    ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
    IEnumerable<ITelephone> ConnectedTelephones { get; }
    void AssignPhoneNumber(string? number);
}
