#nullable enable
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Grids;

public interface ITelecommunicationsGrid : IGrid
{
	string Prefix { get; }
	int NumberLength { get; }
	void JoinGrid(ITelephone phone);
	void LeaveGrid(ITelephone phone);
	bool TryStartCall(ITelephone caller, string number, out string error);
	bool TryResolvePhone(string number, out ITelephone? phone);
	string? GetPhoneNumber(ITelephone phone);
	bool RequestNumber(ITelephone phone, string? preferredNumber);
	void ReleaseNumber(ITelephone phone);
}
