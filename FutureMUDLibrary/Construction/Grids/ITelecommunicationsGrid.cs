#nullable enable
using System;
using MudSharp.Computers;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.Construction.Grids;

public sealed class TelecommunicationsNetworkEndpointInfo
{
	public required INetworkAdapter Adapter { get; init; }
	public required ITelecommunicationsGrid Grid { get; init; }
	public string CanonicalAddress { get; init; } = string.Empty;
	public bool IsLocalGrid { get; init; }
	public IReadOnlyCollection<string> SharedRouteKeys { get; init; } = Array.Empty<string>();
}

public interface ITelecommunicationsGrid : IGrid
{
    string Prefix { get; }
    int NumberLength { get; }
    int MaximumRings { get; }
    bool HostedVoicemailEnabled { get; }
    string HostedVoicemailAccessNumber { get; }
    double TotalSupply { get; }
    double TotalDrawdown { get; }
    IEnumerable<ITelecommunicationsGrid> LinkedGrids { get; }
    IEnumerable<INetworkAdapter> NetworkAdapters { get; }
    void JoinGrid(ITelephoneNumberOwner owner);
    void LeaveGrid(ITelephoneNumberOwner owner);
    void JoinGrid(IConsumePower consumer);
    void LeaveGrid(IConsumePower consumer);
    void JoinGrid(IProducePower producer);
    void LeaveGrid(IProducePower producer);
    void JoinGrid(INetworkAdapter adapter);
    void LeaveGrid(INetworkAdapter adapter);
    void LinkGrid(ITelecommunicationsGrid other);
    void UnlinkGrid(ITelecommunicationsGrid other);
    void RecalculateGrid();
    string GetCanonicalNetworkAddress(INetworkAdapter adapter);
    IEnumerable<TelecommunicationsNetworkEndpointInfo> GetReachableNetworkEndpoints();
    IEnumerable<TelecommunicationsNetworkEndpointInfo> GetReachableNetworkEndpoints(INetworkAdapter source);
    TelecommunicationsNetworkEndpointInfo? ResolveReachableNetworkEndpoint(string address);
    TelecommunicationsNetworkEndpointInfo? ResolveReachableNetworkEndpoint(INetworkAdapter source, string address);
    bool DrawdownSpike(double wattage);
    bool TryStartCall(ITelephone caller, string number, out string error);
    bool TrySendFax(IFaxMachine sender, string number, IReadOnlyCollection<ICanBeRead> document, out string error);
    bool TryPickUp(ITelephone phone, out string error);
    bool TryResolvePhone(string number, out ITelephone? phone);
    IEnumerable<ITelephoneNumberOwner> GetOwnersForNumber(string number);
    string? GetPhoneNumber(ITelephoneNumberOwner owner);
    bool RequestNumber(ITelephoneNumberOwner owner, string? preferredNumber, bool allowSharedNumber = false,
        bool fallbackToAutomatic = true);
    void ReleaseNumber(ITelephoneNumberOwner owner);
    void EndCall(ITelephone phone, ITelephoneCall? call);
}
