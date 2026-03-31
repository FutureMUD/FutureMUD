#nullable enable
using System.Collections.Generic;
using MudSharp.Communication.Language;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Grids;

public interface ITelecommunicationsGrid : IGrid
{
	string Prefix { get; }
	int NumberLength { get; }
	double TotalSupply { get; }
	double TotalDrawdown { get; }
	void JoinGrid(ITelephoneNumberOwner owner);
	void LeaveGrid(ITelephoneNumberOwner owner);
	void JoinGrid(IConsumePower consumer);
	void LeaveGrid(IConsumePower consumer);
	void JoinGrid(IProducePower producer);
	void LeaveGrid(IProducePower producer);
	void RecalculateGrid();
	bool DrawdownSpike(double wattage);
	bool TryStartCall(ITelephone caller, string number, out string error);
	bool TryPickUp(ITelephone phone, out string error);
	bool TryResolvePhone(string number, out ITelephone? phone);
	IEnumerable<ITelephoneNumberOwner> GetOwnersForNumber(string number);
	string? GetPhoneNumber(ITelephoneNumberOwner owner);
	bool RequestNumber(ITelephoneNumberOwner owner, string? preferredNumber, bool allowSharedNumber = false,
		bool fallbackToAutomatic = true);
	void ReleaseNumber(ITelephoneNumberOwner owner);
	void EndCall(ITelephone phone, ITelephoneCall? call);
}
