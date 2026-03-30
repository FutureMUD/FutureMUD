#nullable enable
using MudSharp.Character;
using MudSharp.Construction.Grids;

namespace MudSharp.GameItems.Interfaces;

public interface ITelephone : IGameItemComponent, ITransmit, IReceive, IConsumePower, ISwitchable, IOnOff
{
	string? PhoneNumber { get; }
	string? PreferredNumber { get; set; }
	bool IsPowered { get; }
	bool CanReceiveCalls { get; }
	bool IsRinging { get; }
	bool IsConnected { get; }
	bool IsEngaged { get; }
	ITelephone? ConnectedPhone { get; }
	ITelecommunicationsGrid? TelecommunicationsGrid { get; set; }
	void AssignPhoneNumber(string? number);
	bool CanDial(ICharacter actor, string number, out string error);
	bool Dial(ICharacter actor, string number, out string error);
	bool CanAnswer(ICharacter actor, out string error);
	bool Answer(ICharacter actor, out string error);
	bool CanHangUp(ICharacter actor, out string error);
	bool HangUp(ICharacter actor, out string error);
	void BeginOutgoingCall(ITelephone otherPhone, string number);
	void ReceiveIncomingCall(ITelephone caller);
	void ConnectCall(ITelephone otherPhone);
	void EndCall(ITelephone? otherPhone, bool notifyOtherPhone = true);
}
