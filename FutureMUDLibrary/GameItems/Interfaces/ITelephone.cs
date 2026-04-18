#nullable enable
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Form.Audio;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface ITelephone : IGameItemComponent, ITransmit, IReceive, IConsumePower, ISwitchable, IOnOff
{
    ITelephoneNumberOwner? NumberOwner { get; }
    string? PhoneNumber { get; }
    string? PreferredNumber { get; set; }
    bool AllowSharedNumber { get; set; }
    bool SupportsVoiceCalls => true;
    bool IsPowered { get; }
    bool IsOffHook { get; }
    bool CanReceiveCalls { get; }
    bool IsRinging { get; }
    bool IsConnected { get; }
    bool IsEngaged { get; }
    AudioVolume RingVolume { get; }
    ITelephoneCall? CurrentCall { get; }
    IEnumerable<ITelephone> ConnectedPhones { get; }
    ITelephone? ConnectedPhone { get; }
    ITelecommunicationsGrid? TelecommunicationsGrid { get; }
    void AssignPhoneNumber(string? number);
    bool CanPickUp(ICharacter actor, out string error);
    bool PickUp(ICharacter actor, out string error);
    bool CanDial(ICharacter actor, string number, out string error);
    bool Dial(ICharacter actor, string number, out string error);
    bool CanSendDigits(ICharacter actor, string digits, out string error);
    bool SendDigits(ICharacter actor, string digits, out string error);
    bool CanAnswer(ICharacter actor, out string error);
    bool Answer(ICharacter actor, out string error);
    bool CanHangUp(ICharacter actor, out string error);
    bool HangUp(ICharacter actor, out string error);
    void BeginOutgoingCall(ITelephoneCall call, string number);
    void ReceiveIncomingCall(ITelephoneCall call);
    void ConnectCall(ITelephoneCall call);
    void EndCall(ITelephoneCall? call, bool notifyGrid = true);
    void ReceiveDigits(ITelephone source, string digits);
    void NotifyCallProgress(string message);
}
