#nullable enable
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Construction.Grids;

public class TelecommunicationsGrid : GridBase, ITelecommunicationsGrid
{
    private const int DefaultMaximumRings = 6;
    private readonly List<LoadedAssignment> _loadedAssignments = [];
    private readonly Dictionary<ITelephoneNumberOwner, string> _ownerNumbers = [];
    private readonly Dictionary<string, HashSet<ITelephoneNumberOwner>> _ownersByNumber =
        new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, TelecommunicationsCall> _activeCallsByNumber =
        new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, List<StoredAudioRecording>> _hostedVoicemailRecordings =
        new(StringComparer.InvariantCultureIgnoreCase);
    private readonly List<long> _linkedGridIds = [];
    private readonly HashSet<ITelecommunicationsGrid> _linkedGrids = [];

    private readonly List<long> _connectedConsumerIds = [];
    private readonly List<IConsumePower> _connectedConsumers = [];
    private readonly List<long> _connectedProducerIds = [];
    private readonly List<IProducePower> _connectedProducers = [];
    private readonly List<IConsumePower> _idleConsumers = [];
    private bool _gridPowered;
    private bool _hostedVoicemailEnabled;
    private string _hostedVoicemailAccessCode = "9999";
    private bool _ringHeartbeatSubscribed;

    public TelecommunicationsGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
    {
        XElement root = XElement.Parse(grid.Definition);
        Prefix = root.Element("Prefix")?.Value ?? "555";
        NumberLength = int.Parse(root.Element("NumberLength")?.Value ?? "4");
        NextNumber = long.Parse(root.Element("NextNumber")?.Value ?? "1");
        MaximumRings = int.Parse(root.Element("MaxRings")?.Value ?? DefaultMaximumRings.ToString());
        _hostedVoicemailEnabled = bool.Parse(root.Element("HostedVoicemailEnabled")?.Value ?? "false");
        _hostedVoicemailAccessCode = root.Element("HostedVoicemailAccessCode")?.Value ?? "9999";

        foreach (XElement? element in root.Elements("Endpoint").Concat(root.Elements("Phone")))
        {
            long componentId = long.TryParse(element.Attribute("component")?.Value, out long parsedComponentId)
                ? parsedComponentId
                : 0L;
            long itemId = long.TryParse(element.Attribute("item")?.Value, out long parsedItemId)
                ? parsedItemId
                : long.Parse(element.Attribute("id")!.Value);
            string number = element.Attribute("number")!.Value;
            _loadedAssignments.Add(new LoadedAssignment(componentId, itemId, number));
        }

        foreach (XElement element in root.Elements("Consumer"))
        {
            _connectedConsumerIds.Add(long.Parse(element.Value));
        }

        foreach (XElement element in root.Elements("Producer"))
        {
            _connectedProducerIds.Add(long.Parse(element.Value));
        }

        foreach (XElement element in root.Elements("LinkedGrid"))
        {
            _linkedGridIds.Add(long.Parse(element.Value));
        }

        XElement? voicemailElement = root.Element("HostedVoicemailMailboxes");
        if (voicemailElement != null)
        {
            foreach (XElement mailbox in voicemailElement.Elements("Mailbox"))
            {
                string? number = mailbox.Attribute("number")?.Value;
                if (string.IsNullOrWhiteSpace(number))
                {
                    continue;
                }

                _hostedVoicemailRecordings[number] = mailbox.Elements("StoredRecording")
                                                            .Select(StoredAudioRecording.LoadFromXml)
                                                            .OrderBy(x => x.RecordedAtUtc)
                                                            .ToList();
            }
        }
    }

    public TelecommunicationsGrid(IFuturemud gameworld, ICell? initialLocation, string prefix, int numberLength,
        bool hostedVoicemailEnabled = false, string? hostedVoicemailAccessCode = null)
        : base(gameworld, initialLocation)
    {
        Prefix = prefix;
        NumberLength = numberLength;
        MaximumRings = DefaultMaximumRings;
        NextNumber = 1;
        _hostedVoicemailEnabled = hostedVoicemailEnabled;
        _hostedVoicemailAccessCode = NormaliseVoicemailAccessCode(hostedVoicemailAccessCode);
    }

    public TelecommunicationsGrid(ITelecommunicationsGrid rhs) : base(rhs)
    {
        Prefix = rhs.Prefix;
        NumberLength = rhs.NumberLength;
        MaximumRings = rhs.MaximumRings;
        _hostedVoicemailEnabled = rhs.HostedVoicemailEnabled;
        _hostedVoicemailAccessCode = NormaliseVoicemailAccessCode(rhs.HostedVoicemailAccessNumber[rhs.Prefix.Length..]);
        NextNumber = rhs is TelecommunicationsGrid grid ? grid.NextNumber : 1;
        if (rhs is TelecommunicationsGrid telecomGrid)
        {
            foreach ((string? number, List<StoredAudioRecording>? recordings) in telecomGrid._hostedVoicemailRecordings)
            {
                _hostedVoicemailRecordings[number] = recordings
                    .OrderBy(x => x.RecordedAtUtc)
                    .ToList();
            }
        }
    }

    public override string GridType => "Telecommunications";
    public string Prefix { get; }
    public int NumberLength { get; }
    public int MaximumRings { get; private set; }
    public bool HostedVoicemailEnabled => _hostedVoicemailEnabled;
    public string HostedVoicemailAccessNumber => $"{Prefix}{_hostedVoicemailAccessCode}";
    public double TotalSupply => _connectedProducers.Sum(x => x.MaximumPowerInWatts);
    public double TotalDrawdown => _connectedConsumers.Except(_idleConsumers).Sum(x => x.PowerConsumptionInWatts);
    public IEnumerable<ITelecommunicationsGrid> LinkedGrids => _linkedGrids.ToList();
    private long NextNumber { get; set; }

    public override void LoadTimeInitialise()
    {
        base.LoadTimeInitialise();

        foreach (long id in _connectedConsumerIds)
        {
            IConsumePower? consumer = FindItemComponent<IConsumePower>(itemId: id);
            if (consumer == null)
            {
                continue;
            }

            _connectedConsumers.Add(consumer);
        }

        foreach (long id in _connectedProducerIds)
        {
            IProducePower? producer = FindItemComponent<IProducePower>(itemId: id);
            if (producer == null)
            {
                continue;
            }

            _connectedProducers.Add(producer);
        }

        _connectedConsumerIds.Clear();
        _connectedProducerIds.Clear();
        _idleConsumers.AddRange(_connectedConsumers);

        foreach (long linkedGridId in _linkedGridIds)
        {
            if (Gameworld.Grids.Get(linkedGridId) is ITelecommunicationsGrid linkedGrid &&
                !ReferenceEquals(linkedGrid, this))
            {
                _linkedGrids.Add(linkedGrid);
            }
        }

        _linkedGridIds.Clear();

        foreach (LoadedAssignment assignment in _loadedAssignments)
        {
            ITelephoneNumberOwner? owner = FindItemComponent<ITelephoneNumberOwner>(assignment.ComponentId, assignment.ItemId);
            if (owner == null)
            {
                continue;
            }

            ConnectOwner(owner, assignment.Number);
        }

        _loadedAssignments.Clear();
        RecalculateGrid();
    }

    protected override XElement SaveDefinition()
    {
        XElement root = base.SaveDefinition();
        root.Add(new XElement("Prefix", Prefix));
        root.Add(new XElement("NumberLength", NumberLength));
        root.Add(new XElement("NextNumber", NextNumber));
        root.Add(new XElement("MaxRings", MaximumRings));
        root.Add(new XElement("HostedVoicemailEnabled", _hostedVoicemailEnabled));
        root.Add(new XElement("HostedVoicemailAccessCode", _hostedVoicemailAccessCode));

        foreach (ITelecommunicationsGrid? linkedGrid in _linkedGrids.OrderBy(x => x.Id))
        {
            root.Add(new XElement("LinkedGrid", linkedGrid.Id));
        }

        foreach ((ITelephoneNumberOwner? owner, string? number) in _ownerNumbers.OrderBy(x => x.Value).ThenBy(x => x.Key.Id))
        {
            root.Add(new XElement("Endpoint",
                new XAttribute("component", owner.Id),
                new XAttribute("item", owner.Parent.Id),
                new XAttribute("number", number)
            ));
        }

        foreach (IConsumePower consumer in _connectedConsumers)
        {
            root.Add(new XElement("Consumer", consumer.Parent.Id));
        }

        foreach (IProducePower producer in _connectedProducers)
        {
            root.Add(new XElement("Producer", producer.Parent.Id));
        }

        if (_hostedVoicemailRecordings.Any())
        {
            root.Add(new XElement("HostedVoicemailMailboxes",
                from mailbox in _hostedVoicemailRecordings.OrderBy(x => x.Key)
                select new XElement("Mailbox",
                    new XAttribute("number", mailbox.Key),
                    from recording in mailbox.Value.OrderBy(x => x.RecordedAtUtc)
                    select recording.SaveToXml())));
        }

        return root;
    }

    public void LinkGrid(ITelecommunicationsGrid other)
    {
        if (ReferenceEquals(other, this))
        {
            return;
        }

        if (_linkedGrids.Add(other))
        {
            Changed = true;
        }

        if (other is TelecommunicationsGrid telecomGrid && telecomGrid._linkedGrids.Add(this))
        {
            telecomGrid.Changed = true;
        }
    }

    public void UnlinkGrid(ITelecommunicationsGrid other)
    {
        if (_linkedGrids.Remove(other))
        {
            Changed = true;
        }

        if (other is TelecommunicationsGrid telecomGrid && telecomGrid._linkedGrids.Remove(this))
        {
            telecomGrid.Changed = true;
        }
    }

    public void SetMaximumRings(int maximumRings)
    {
        MaximumRings = Math.Max(1, maximumRings);
        Changed = true;
    }

    public void ConfigureHostedVoicemail(bool enabled, string? accessCode = null)
    {
        _hostedVoicemailEnabled = enabled;
        if (accessCode != null)
        {
            _hostedVoicemailAccessCode = NormaliseVoicemailAccessCode(accessCode);
        }

        Changed = true;
    }

    public void JoinGrid(ITelephoneNumberOwner owner)
    {
        if (_ownerNumbers.ContainsKey(owner))
        {
            return;
        }

        if (_loadedAssignments.Any(x => x.ComponentId == owner.Id) ||
            _loadedAssignments.Any(x => x.ComponentId == 0 && x.ItemId == owner.Parent.Id))
        {
            return;
        }

        RequestNumber(owner, owner.PreferredNumber, owner.AllowSharedNumber);
    }

    public void LeaveGrid(ITelephoneNumberOwner owner)
    {
        foreach (ITelephone? phone in owner.ConnectedTelephones.ToList())
        {
            if (phone.IsEngaged)
            {
                phone.EndCall(phone.CurrentCall);
            }
        }

        ReleaseNumber(owner);
    }

    public void JoinGrid(IConsumePower consumer)
    {
        if (_connectedConsumers.Contains(consumer))
        {
            return;
        }

        _idleConsumers.Add(consumer);
        _connectedConsumers.Add(consumer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IConsumePower consumer)
    {
        _idleConsumers.Remove(consumer);
        _connectedConsumers.Remove(consumer);
        Changed = true;
        RecalculateGrid();
    }

    public void JoinGrid(IProducePower producer)
    {
        if (_connectedProducers.Contains(producer))
        {
            return;
        }

        _connectedProducers.Add(producer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IProducePower producer)
    {
        _connectedProducers.Remove(producer);
        Changed = true;
        RecalculateGrid();
    }

    public bool DrawdownSpike(double wattage)
    {
        if (!_gridPowered)
        {
            return false;
        }

        if (TotalSupply >= TotalDrawdown + wattage)
        {
            return true;
        }

        foreach (IConsumePower? consumer in _connectedConsumers.Except(_idleConsumers)
                                                    .OrderByDescending(x => x.PowerConsumptionInWatts)
                                                    .ToList())
        {
            consumer.OnPowerCutOut();
            _idleConsumers.Add(consumer);
            if (TotalSupply >= TotalDrawdown + wattage)
            {
                break;
            }
        }

        if (TotalSupply <= 0.0)
        {
            _gridPowered = false;
        }

        return true;
    }

    public void RecalculateGrid()
    {
        if (!_gridPowered)
        {
            if (TotalSupply > 0.0)
            {
                _gridPowered = true;
            }
            else
            {
                return;
            }
        }

        if (TotalDrawdown > TotalSupply)
        {
            foreach (IConsumePower? consumer in _connectedConsumers.Except(_idleConsumers)
                                                        .OrderByDescending(x => x.PowerConsumptionInWatts)
                                                        .ToList())
            {
                consumer.OnPowerCutOut();
                _idleConsumers.Add(consumer);
                if (TotalDrawdown <= TotalSupply)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (IConsumePower? consumer in _idleConsumers.OrderBy(x => x.PowerConsumptionInWatts).ToList())
            {
                if (TotalDrawdown + consumer.PowerConsumptionInWatts > TotalSupply)
                {
                    break;
                }

                _idleConsumers.Remove(consumer);
                consumer.OnPowerCutIn();
            }
        }

        if (TotalSupply <= 0.0)
        {
            _gridPowered = false;
        }
    }

    public bool TryStartCall(ITelephone caller, string number, out string error)
    {
        string normalised = Normalise(number);
        if (string.IsNullOrWhiteSpace(normalised))
        {
            error = "That is not a valid number.";
            return false;
        }

        if (caller.NumberOwner?.PhoneNumber?.EqualTo(normalised) == true)
        {
            error = "You cannot call the same line you are using.";
            return false;
        }

        ITelecommunicationsGrid? destinationGrid = ResolveDestinationGrid(normalised, out error);
        if (destinationGrid == null)
        {
            return false;
        }

        if (!ReferenceEquals(destinationGrid, this))
        {
            if (destinationGrid is TelecommunicationsGrid telecomGrid)
            {
                return telecomGrid.TryStartCallOnThisGrid(caller, normalised, out error);
            }

            error = "That telecommunications exchange cannot route calls right now.";
            return false;
        }

        return TryStartCallOnThisGrid(caller, normalised, out error);
    }

    private bool TryStartCallOnThisGrid(ITelephone caller, string normalised, out string error)
    {
        if (_hostedVoicemailEnabled && normalised.EqualTo(HostedVoicemailAccessNumber))
        {
            return TryStartHostedVoicemailAccess(caller, out error);
        }

        List<ITelephoneNumberOwner> owners = GetOwnersForNumber(normalised).ToList();
        if (!owners.Any())
        {
            error = "That number is not connected.";
            return false;
        }

        if (_activeCallsByNumber.ContainsKey(normalised))
        {
            error = "That line is currently busy.";
            return false;
        }

        List<ITelephone> targetPhones = owners.SelectMany(TelephoneNetworkHelpers.CollectConnectedTelephones)
                                 .Where(x => x != caller)
                                 .Distinct()
                                 .ToList();
        if (!targetPhones.Any())
        {
            error = "That line cannot receive calls right now.";
            return false;
        }

        if (targetPhones.Any(x => x.IsOffHook || x.IsConnected || x.IsRinging))
        {
            error = "That line is currently busy.";
            return false;
        }

        List<ITelephone> ringablePhones = targetPhones.Where(x => x.SupportsVoiceCalls && x.CanReceiveCalls).ToList();
        if (!ringablePhones.Any())
        {
            List<IFaxMachine> faxMachines = targetPhones.OfType<IFaxMachine>()
                                          .Where(x => x.CanReceiveFaxes)
                                          .ToList();
            if (faxMachines.Any())
            {
                const string mismatchMessage =
                    "The line answers with a burst of unintelligible modem-like noises before disconnecting.";
                caller.NotifyCallProgress(mismatchMessage);
                foreach (IFaxMachine faxMachine in faxMachines)
                {
                    faxMachine.NotifyCallProgress(
                        "An incoming voice call hits the fax line and collapses into modem-like screeching before it disconnects.");
                }

                caller.EndCall(null, false);
                error = string.Empty;
                return true;
            }

            error = "That line cannot receive calls right now.";
            return false;
        }

        TelecommunicationsCall call = new(this, normalised, caller);
        _activeCallsByNumber[normalised] = call;
        EnsureRingHeartbeat();
        caller.BeginOutgoingCall(call, normalised);
        foreach (ITelephone phone in ringablePhones)
        {
            call.AddRingingPhone(phone);
        }

        call.NotifyCallerOfRinging();

        error = string.Empty;
        return true;
    }

    public bool TrySendFax(IFaxMachine sender, string number, IReadOnlyCollection<ICanBeRead> document, out string error)
    {
        string normalised = Normalise(number);
        if (string.IsNullOrWhiteSpace(normalised))
        {
            error = "That is not a valid number.";
            return false;
        }

        if (sender.NumberOwner?.PhoneNumber?.EqualTo(normalised) == true)
        {
            error = "You cannot fax the same line you are using.";
            return false;
        }

        ITelecommunicationsGrid? destinationGrid = ResolveDestinationGrid(normalised, out error);
        if (destinationGrid == null)
        {
            return false;
        }

        if (!ReferenceEquals(destinationGrid, this))
        {
            if (destinationGrid is TelecommunicationsGrid telecomGrid)
            {
                return telecomGrid.TrySendFaxOnThisGrid(sender, normalised, document, out error);
            }

            error = "That telecommunications exchange cannot route faxes right now.";
            return false;
        }

        return TrySendFaxOnThisGrid(sender, normalised, document, out error);
    }

    private bool TrySendFaxOnThisGrid(IFaxMachine sender, string normalised, IReadOnlyCollection<ICanBeRead> document,
        out string error)
    {
        List<ITelephoneNumberOwner> owners = GetOwnersForNumber(normalised).ToList();
        if (!owners.Any())
        {
            error = "That number is not connected.";
            return false;
        }

        if (_activeCallsByNumber.ContainsKey(normalised))
        {
            error = "That line is currently busy.";
            return false;
        }

        List<ITelephone> targetPhones = owners.SelectMany(x => x.ConnectedTelephones)
                                 .Where(x => x != sender)
                                 .Distinct()
                                 .ToList();
        if (!targetPhones.Any())
        {
            error = "That line cannot receive faxes right now.";
            return false;
        }

        if (targetPhones.Any(x => x.IsOffHook || x.IsConnected || x.IsRinging))
        {
            error = "That line is currently busy.";
            return false;
        }

        List<IFaxMachine> faxRecipients = targetPhones.OfType<IFaxMachine>()
                                        .Where(x => x.CanReceiveFaxes)
                                        .ToList();
        if (faxRecipients.Any())
        {
            foreach (IFaxMachine faxRecipient in faxRecipients)
            {
                faxRecipient.ReceiveFax(sender.PhoneNumber ?? "Unknown", document);
            }

            error = string.Empty;
            return true;
        }

        List<ITelephone> voiceRecipients = targetPhones.Where(x => x.SupportsVoiceCalls && x.CanReceiveCalls).ToList();
        if (voiceRecipients.Any())
        {
            foreach (ITelephone voiceRecipient in voiceRecipients)
            {
                voiceRecipient.NotifyCallProgress(
                    "The line erupts with a burst of unintelligible modem-like noises before disconnecting.");
            }

            error = "The far end answers with unintelligible modem-like noises and the fax transmission fails.";
            return false;
        }

        error = "That line cannot receive faxes right now.";
        return false;
    }

    public bool TryPickUp(ITelephone phone, out string error)
    {
        if (phone.CurrentCall is TelecommunicationsCall currentCall)
        {
            return currentCall.TryAnswer(phone, out error);
        }

        string? number = phone.NumberOwner?.PhoneNumber;
        if (string.IsNullOrWhiteSpace(number))
        {
            error = "That telephone is not connected to a telecommunications line.";
            return false;
        }

        if (!_activeCallsByNumber.TryGetValue(number, out TelecommunicationsCall? call))
        {
            error = "There is no live call on that line right now.";
            return false;
        }

        return call.TryAnswer(phone, out error);
    }

    public bool TryResolvePhone(string number, out ITelephone? phone)
    {
        string normalised = Normalise(number);
        ITelecommunicationsGrid? destinationGrid = ResolveDestinationGrid(normalised, out _);
        phone = destinationGrid?.GetOwnersForNumber(normalised)
                         .SelectMany(TelephoneNetworkHelpers.CollectConnectedTelephones)
                         .FirstOrDefault();
        return phone != null;
    }

    public IEnumerable<ITelephoneNumberOwner> GetOwnersForNumber(string number)
    {
        return _ownersByNumber.TryGetValue(Normalise(number), out HashSet<ITelephoneNumberOwner>? owners)
            ? owners.ToList()
            : Enumerable.Empty<ITelephoneNumberOwner>();
    }

    public string? GetPhoneNumber(ITelephoneNumberOwner owner)
    {
        return _ownerNumbers.GetValueOrDefault(owner);
    }

    public bool RequestNumber(ITelephoneNumberOwner owner, string? preferredNumber, bool allowSharedNumber = false,
        bool fallbackToAutomatic = true)
    {
        string? previousNumber = _ownerNumbers.GetValueOrDefault(owner);
        ReleaseNumber(owner);

        if (!string.IsNullOrWhiteSpace(preferredNumber))
        {
            string normalised = Normalise(preferredNumber);
            if (CanAssignNumber(owner, normalised, allowSharedNumber))
            {
                ConnectOwner(owner, normalised);
                return true;
            }

            if (!fallbackToAutomatic)
            {
                if (!string.IsNullOrWhiteSpace(previousNumber))
                {
                    ConnectOwner(owner, previousNumber);
                }

                return false;
            }
        }

        string number = GenerateNumber();
        ConnectOwner(owner, number);
        return true;
    }

    public void ReleaseNumber(ITelephoneNumberOwner owner)
    {
        if (!_ownerNumbers.Remove(owner, out string? number))
        {
            owner.AssignPhoneNumber(null);
            return;
        }

        if (_ownersByNumber.TryGetValue(number, out HashSet<ITelephoneNumberOwner>? owners))
        {
            owners.Remove(owner);
            if (!owners.Any())
            {
                _ownersByNumber.Remove(number);
            }
        }

        owner.AssignPhoneNumber(null);
        Changed = true;
    }

    public void EndCall(ITelephone phone, ITelephoneCall? call)
    {
        if (call is not TelecommunicationsCall telecomCall)
        {
            return;
        }

        telecomCall.HangUp(phone);
    }

    private void RemoveCall(TelecommunicationsCall call)
    {
        _activeCallsByNumber.Remove(call.Number);
        ReleaseRingHeartbeatIfIdle();
    }

    private void ConnectOwner(ITelephoneNumberOwner owner, string number)
    {
        number = Normalise(number);
        _ownerNumbers[owner] = number;
        if (!_ownersByNumber.TryGetValue(number, out HashSet<ITelephoneNumberOwner>? owners))
        {
            owners = [];
            _ownersByNumber[number] = owners;
        }

        owners.Add(owner);
        owner.AssignPhoneNumber(number);
        Changed = true;
    }

    private bool CanAssignNumber(ITelephoneNumberOwner owner, string number, bool allowSharedNumber)
    {
        if (_hostedVoicemailEnabled && number.EqualTo(HostedVoicemailAccessNumber))
        {
            return false;
        }

        if (!_ownersByNumber.TryGetValue(number, out HashSet<ITelephoneNumberOwner>? owners))
        {
            return true;
        }

        return owners.All(x => x == owner) || allowSharedNumber;
    }

    private string GenerateNumber()
    {
        while (true)
        {
            string number = $"{Prefix}{NextNumber.ToString().PadLeft(NumberLength, '0')}";
            NextNumber++;
            if (_ownersByNumber.ContainsKey(number) ||
                (_hostedVoicemailEnabled && number.EqualTo(HostedVoicemailAccessNumber)))
            {
                continue;
            }

            return number;
        }
    }

    private static string Normalise(string number)
    {
        return new string(number.Where(char.IsDigit).ToArray());
    }

    private static string NormaliseVoicemailAccessCode(string? accessCode)
    {
        string digits = Normalise(accessCode ?? string.Empty);
        return string.IsNullOrEmpty(digits) ? "9999" : digits;
    }

    private bool IsHostedVoicemailEnabledForNumber(string number)
    {
        return _hostedVoicemailEnabled &&
               GetOwnersForNumber(number).Any(x => x.HostedVoicemailEnabled);
    }

    private List<StoredAudioRecording> GetHostedVoicemailMessages(string number)
    {
        return _hostedVoicemailRecordings.TryGetValue(number, out List<StoredAudioRecording>? recordings)
            ? recordings
            : [];
    }

    private void StoreHostedVoicemailMessage(string number, StoredAudioRecording recording)
    {
        if (!_hostedVoicemailRecordings.TryGetValue(number, out List<StoredAudioRecording>? recordings))
        {
            recordings = [];
            _hostedVoicemailRecordings[number] = recordings;
        }

        recordings.Add(recording);
        recordings.Sort((a, b) => a.RecordedAtUtc.CompareTo(b.RecordedAtUtc));
        Changed = true;
    }

    private bool DeleteHostedVoicemailMessage(string number, string messageName)
    {
        if (!_hostedVoicemailRecordings.TryGetValue(number, out List<StoredAudioRecording>? recordings))
        {
            return false;
        }

        if (recordings.RemoveAll(x => x.Name.EqualTo(messageName)) == 0)
        {
            return false;
        }

        if (!recordings.Any())
        {
            _hostedVoicemailRecordings.Remove(number);
        }

        Changed = true;
        return true;
    }

    private void DeleteAllHostedVoicemailMessages(string number)
    {
        if (_hostedVoicemailRecordings.Remove(number))
        {
            Changed = true;
        }
    }

    private bool TryStartHostedVoicemailAccess(ITelephone caller, out string error)
    {
        if (!_hostedVoicemailEnabled)
        {
            error = "Hosted voicemail is not enabled on this telecommunications exchange.";
            return false;
        }

        string? callerNumber = caller.NumberOwner?.PhoneNumber;
        if (string.IsNullOrWhiteSpace(callerNumber) || caller.NumberOwner?.TelecommunicationsGrid != this)
        {
            error = "You must call your own exchange voicemail service from a line on this exchange.";
            return false;
        }

        if (!IsHostedVoicemailEnabledForNumber(callerNumber))
        {
            error = "Your line does not have hosted voicemail enabled.";
            return false;
        }

        if (_activeCallsByNumber.ContainsKey(HostedVoicemailAccessNumber))
        {
            error = "The voicemail service is currently busy.";
            return false;
        }

        TelecommunicationsCall call = new(this, HostedVoicemailAccessNumber, caller);
        _activeCallsByNumber[HostedVoicemailAccessNumber] = call;
        caller.BeginOutgoingCall(call, HostedVoicemailAccessNumber);
        call.ConnectHostedVoicemailAccess(callerNumber);
        error = string.Empty;
        return true;
    }

    private ITelecommunicationsGrid? ResolveDestinationGrid(string number, out string error)
    {
        if (number.StartsWith(Prefix, StringComparison.InvariantCultureIgnoreCase))
        {
            error = string.Empty;
            return this;
        }

        List<ITelecommunicationsGrid> matches = _linkedGrids.Where(x => number.StartsWith(x.Prefix, StringComparison.InvariantCultureIgnoreCase))
                                  .Distinct()
                                  .ToList();
        if (matches.Count <= 1)
        {
            error = string.Empty;
            return matches.SingleOrDefault() ?? this;
        }

        error = "That number matches more than one linked telecommunications exchange.";
        return null;
    }

    private void EnsureRingHeartbeat()
    {
        if (_ringHeartbeatSubscribed)
        {
            return;
        }

        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RingHeartbeat;
        _ringHeartbeatSubscribed = true;
    }

    private void ReleaseRingHeartbeatIfIdle()
    {
        if (!_ringHeartbeatSubscribed || _activeCallsByNumber.Any())
        {
            return;
        }

        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RingHeartbeat;
        _ringHeartbeatSubscribed = false;
    }

    private void RingHeartbeat()
    {
        foreach (TelecommunicationsCall? call in _activeCallsByNumber.Values.ToList())
        {
            call.HandleRingHeartbeat();
        }
    }

    private TComponent? FindItemComponent<TComponent>(long componentId = 0, long itemId = 0)
        where TComponent : class, MudSharp.GameItems.IGameItemComponent
    {
        if (componentId > 0)
        {
            TComponent? component = Gameworld.Items.SelectMany(x => x.Components)
                                     .OfType<TComponent>()
                                     .FirstOrDefault(x => x.Id == componentId);
            if (component != null)
            {
                return component;
            }
        }

        if (itemId <= 0)
        {
            return null;
        }

        return Gameworld.Items.FirstOrDefault(x => x.Id == itemId)?.GetItemType<TComponent>();
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Grid #{Id.ToString("N0", actor)}");
        sb.AppendLine($"Type: {"Telecommunications".Colour(Telnet.BoldBlue)}");
        sb.AppendLine($"Locations: {Locations.Count().ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Prefix: {Prefix.ColourValue()}");
        sb.AppendLine($"Subscriber Digits: {NumberLength.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Maximum Rings: {MaximumRings.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Hosted Voicemail: {_hostedVoicemailEnabled.ToColouredString()}");
        sb.AppendLine($"Voicemail Access Number: {HostedVoicemailAccessNumber.ColourValue()}");
        sb.AppendLine($"Hosted Mailboxes With Messages: {_hostedVoicemailRecordings.Count.ToString("N0", actor).ColourValue()}");
        sb.AppendLine(
            $"Linked Exchanges: {(_linkedGrids.Any() ? _linkedGrids.OrderBy(x => x.Prefix).Select(x => $"#{x.Id.ToString("N0", actor)} ({x.Prefix})").ListToString() : "none".ColourError())}");
        sb.AppendLine($"Powered: {_gridPowered.ToColouredString()}");
        sb.AppendLine($"Total Supply: {TotalSupply.ToString("N2", actor).ColourValue()}");
        sb.AppendLine($"Total Drawdown: {TotalDrawdown.ToString("N2", actor).ColourValue()}");
        sb.AppendLine($"Connected Numbers: {_ownerNumbers.Count.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Active Calls: {_activeCallsByNumber.Count.ToString("N0", actor).ColourValue()}");
        return sb.ToString();
    }

    private readonly record struct LoadedAssignment(long ComponentId, long ItemId, string Number);

    private sealed class TelecommunicationsCall : ITelephoneCall
    {
        private readonly TelecommunicationsGrid _grid;
        private readonly HashSet<ITelephone> _participants = [];
        private readonly HashSet<ITelephone> _ringingPhones = [];
        private HostedVoicemailSession? _hostedVoicemailSession;
        private int _ringCount;

        public TelecommunicationsCall(TelecommunicationsGrid grid, string number, ITelephone caller)
        {
            _grid = grid;
            Number = number;
            Caller = caller;
            _participants.Add(caller);
        }

        public string Number { get; }
        public ITelephone Caller { get; }
        public IReadOnlyCollection<ITelephone> Participants => _participants;
        public IReadOnlyCollection<ITelephone> RingingPhones => _ringingPhones;
        public bool IsConnected => _participants.Count > 1 || _hostedVoicemailSession != null;
        public bool IsRinging => _ringingPhones.Any();

        public void AddRingingPhone(ITelephone phone)
        {
            _ringingPhones.Add(phone);
            phone.ReceiveIncomingCall(this);
        }

        public void NotifyCallerOfRinging()
        {
            if (!_ringingPhones.Any())
            {
                return;
            }

            if (_ringCount == 0)
            {
                _ringCount = 1;
            }

            Caller.NotifyCallProgress("You hear the line ringing.");
        }

        public bool TryAnswer(ITelephone phone, out string error)
        {
            if (_hostedVoicemailSession != null)
            {
                error = "That call is already connected to the voicemail service.";
                return false;
            }

            if (_participants.Contains(phone))
            {
                error = "That telephone is already connected to the call.";
                return false;
            }

            bool sameLine = phone.NumberOwner?.PhoneNumber?.EqualTo(Number) == true;
            if (_ringingPhones.Contains(phone))
            {
                bool callerWasConnected = Caller.IsConnected;
                _ringingPhones.Remove(phone);
                _participants.Add(phone);
                if (!callerWasConnected)
                {
                    Caller.ConnectCall(this);
                    Caller.NotifyCallProgress("The call connects.");
                }

                phone.ConnectCall(this);
                phone.NotifyCallProgress("The call connects.");
                foreach (ITelephone? other in _ringingPhones.ToList())
                {
                    other.EndCall(this, false);
                }

                _ringingPhones.Clear();
                error = string.Empty;
                return true;
            }

            if (sameLine && IsConnected && phone.CanReceiveCalls)
            {
                _participants.Add(phone);
                phone.ConnectCall(this);
                foreach (IAnsweringMachine? machine in _participants.OfType<IAnsweringMachine>()
                                                     .Where(x => !ReferenceEquals(x, phone))
                                                     .Where(x => x.PhoneNumber.EqualTo(Number))
                                                     .ToList())
                {
                    machine.EndCall(this);
                }

                error = string.Empty;
                return true;
            }

            error = "That telephone cannot join a call right now.";
            return false;
        }

        public void HangUp(ITelephone phone)
        {
            if (_ringingPhones.Remove(phone))
            {
                phone.EndCall(this, false);
                if (!_ringingPhones.Any() && _participants.Count <= 1)
                {
                    Caller.NotifyCallProgress("The line rings out.");
                    Terminate();
                }

                return;
            }

            if (!_participants.Remove(phone))
            {
                return;
            }

            phone.EndCall(this, false);
            if (phone == Caller || _participants.Count <= 1 || _hostedVoicemailSession != null)
            {
                Terminate();
            }
        }

        public void RelayTransmission(ITelephone source, SpokenLanguageInfo spokenLanguage)
        {
            if (_hostedVoicemailSession?.HandleTransmission(source, spokenLanguage) == true)
            {
                return;
            }

            foreach (ITelephone? phone in _participants.Where(x => x != source).ToList())
            {
                phone.ReceiveTransmission(0.0, spokenLanguage, 0L, source);
            }
        }

        public void RelayDigits(ITelephone source, string digits)
        {
            if (_hostedVoicemailSession?.HandleDigits(source, digits) == true)
            {
                return;
            }

            foreach (ITelephone? phone in _participants.Where(x => x != source).ToList())
            {
                phone.ReceiveDigits(source, digits);
            }
        }

        public void HandleRingHeartbeat()
        {
            if (!_ringingPhones.Any())
            {
                return;
            }

            if (_ringCount >= _grid.MaximumRings)
            {
                if (!_grid.TryStartHostedVoicemailForCall(this))
                {
                    Caller.NotifyCallProgress("The line rings out.");
                    Terminate();
                }

                return;
            }

            _ringCount++;
            Caller.NotifyCallProgress("You hear the line ringing.");
            foreach (IAnsweringMachine? machine in _ringingPhones.OfType<IAnsweringMachine>()
                                                 .Where(x => x.AutoAnswerRings <= _ringCount)
                                                 .ToList())
            {
                if (TryAnswer(machine, out _))
                {
                    return;
                }
            }
        }

        public void ConnectHostedVoicemailRecording(string mailboxNumber)
        {
            _ringingPhones.ToList().ForEach(x => x.EndCall(this, false));
            _ringingPhones.Clear();
            Caller.ConnectCall(this);
            Caller.NotifyCallProgress("The exchange voicemail service answers.");
            _hostedVoicemailSession = HostedVoicemailSession.CreateRecording(_grid, this, mailboxNumber);
        }

        public void ConnectHostedVoicemailAccess(string mailboxNumber)
        {
            Caller.ConnectCall(this);
            Caller.NotifyCallProgress("The exchange voicemail service answers.");
            _hostedVoicemailSession = HostedVoicemailSession.CreateAccess(_grid, this, mailboxNumber);
        }

        private void Terminate()
        {
            List<ITelephone> phones = _participants.Concat(_ringingPhones).Distinct().ToList();
            _hostedVoicemailSession?.Finish();
            _hostedVoicemailSession = null;
            _participants.Clear();
            _ringingPhones.Clear();
            _grid.RemoveCall(this);
            foreach (ITelephone phone in phones)
            {
                phone.EndCall(this, false);
            }
        }
    }

    private bool TryStartHostedVoicemailForCall(TelecommunicationsCall call)
    {
        if (!IsHostedVoicemailEnabledForNumber(call.Number))
        {
            return false;
        }

        call.ConnectHostedVoicemailRecording(call.Number);
        return true;
    }

    private sealed class HostedVoicemailSession
    {
        private readonly TelecommunicationsGrid _grid;
        private readonly TelecommunicationsCall _call;
        private readonly string _mailboxNumber;
        private readonly HostedVoicemailMode _mode;
        private readonly List<RecordedAudioSegment> _workingSegments = [];
        private DateTime? _lastSegmentUtc;
        private DateTime? _recordedAtUtc;
        private int _currentPlaybackIndex = -1;
        private string? _currentMessageName;

        private HostedVoicemailSession(TelecommunicationsGrid grid, TelecommunicationsCall call, string mailboxNumber,
            HostedVoicemailMode mode)
        {
            _grid = grid;
            _call = call;
            _mailboxNumber = mailboxNumber;
            _mode = mode;
        }

        public static HostedVoicemailSession CreateRecording(TelecommunicationsGrid grid, TelecommunicationsCall call,
            string mailboxNumber)
        {
            HostedVoicemailSession session = new(grid, call, mailboxNumber, HostedVoicemailMode.RecordIncomingMessage)
            {
                _recordedAtUtc = DateTime.UtcNow
            };
            call.Caller.NotifyCallProgress(
                "The exchange voicemail service says: The person you are calling is unavailable. Please leave a message after the tone.");
            call.Caller.NotifyCallProgress("A sharp beep sounds over the line.");
            return session;
        }

        public static HostedVoicemailSession CreateAccess(TelecommunicationsGrid grid, TelecommunicationsCall call,
            string mailboxNumber)
        {
            HostedVoicemailSession session = new(grid, call, mailboxNumber, HostedVoicemailMode.AccessMailbox);
            session.AnnounceMailboxSummary();
            session.AnnounceMenu();
            return session;
        }

        public bool HandleTransmission(ITelephone source, SpokenLanguageInfo spokenLanguage)
        {
            if (_mode != HostedVoicemailMode.RecordIncomingMessage || !ReferenceEquals(source, _call.Caller))
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;
            TimeSpan delay = _lastSegmentUtc.HasValue ? now - _lastSegmentUtc.Value : TimeSpan.Zero;
            _workingSegments.Add(RecordedAudioSegment.FromSpokenLanguage(spokenLanguage, delay));
            _lastSegmentUtc = now;
            return true;
        }

        public bool HandleDigits(ITelephone source, string digits)
        {
            if (!ReferenceEquals(source, _call.Caller))
            {
                return false;
            }

            foreach (char digit in digits)
            {
                HandleDigit(digit);
            }

            return true;
        }

        public void Finish()
        {
            if (_mode != HostedVoicemailMode.RecordIncomingMessage || !_workingSegments.Any() || !_recordedAtUtc.HasValue)
            {
                return;
            }

            string messageName = $"voicemail-{_recordedAtUtc.Value:yyyyMMddHHmmss}";
            _grid.StoreHostedVoicemailMessage(_mailboxNumber,
                new StoredAudioRecording(messageName, new RecordedAudio(_workingSegments), _recordedAtUtc.Value));
        }

        private void HandleDigit(char digit)
        {
            if (_mode == HostedVoicemailMode.RecordIncomingMessage)
            {
                if (digit == '#')
                {
                    _call.Caller.NotifyCallProgress("The exchange voicemail service ends the recording.");
                    _call.HangUp(_call.Caller);
                    return;
                }

                _call.Caller.NotifyCallProgress("The exchange voicemail service continues recording your message.");
                return;
            }

            switch (digit)
            {
                case '1':
                    PlayNextMessage();
                    return;
                case '2':
                    ReplayCurrentMessage();
                    return;
                case '3':
                    DeleteCurrentMessage();
                    return;
                case '7':
                    DeleteAllMessages();
                    return;
                case '9':
                    AnnounceMenu();
                    return;
                case '#':
                    _call.Caller.NotifyCallProgress("The exchange voicemail service ends the call.");
                    _call.HangUp(_call.Caller);
                    return;
                default:
                    _call.Caller.NotifyCallProgress("That is not a valid voicemail keypad option.");
                    return;
            }
        }

        private List<StoredAudioRecording> Messages =>
            _grid.GetHostedVoicemailMessages(_mailboxNumber)
                 .OrderBy(x => x.RecordedAtUtc)
                 .ToList();

        private void AnnounceMailboxSummary()
        {
            int count = Messages.Count;
            _call.Caller.NotifyCallProgress(
                count == 0
                    ? "You have no saved voicemail messages."
                    : $"You have {count} saved voicemail message{(count == 1 ? string.Empty : "s")}.");
        }

        private void AnnounceMenu()
        {
            _call.Caller.NotifyCallProgress(
                "Press 1 to play the next message, 2 to replay the current message, 3 to delete the current message, 7 to delete all messages, 9 to hear these options again, or # to hang up.");
        }

        private void PlayNextMessage()
        {
            List<StoredAudioRecording> messages = Messages;
            if (!messages.Any())
            {
                _currentPlaybackIndex = -1;
                _currentMessageName = null;
                _call.Caller.NotifyCallProgress("You have no saved voicemail messages.");
                return;
            }

            int nextIndex = Math.Min(_currentPlaybackIndex + 1, messages.Count - 1);
            PlayMessage(messages, nextIndex);
        }

        private void ReplayCurrentMessage()
        {
            List<StoredAudioRecording> messages = Messages;
            if (!messages.Any())
            {
                _currentPlaybackIndex = -1;
                _currentMessageName = null;
                _call.Caller.NotifyCallProgress("You have no saved voicemail messages.");
                return;
            }

            if (_currentPlaybackIndex < 0 || _currentPlaybackIndex >= messages.Count)
            {
                PlayMessage(messages, 0);
                return;
            }

            PlayMessage(messages, _currentPlaybackIndex);
        }

        private void PlayMessage(IReadOnlyList<StoredAudioRecording> messages, int index)
        {
            StoredAudioRecording recording = messages[index];
            _currentPlaybackIndex = index;
            _currentMessageName = recording.Name;
            _call.Caller.NotifyCallProgress(
                $"Playing message #{index + 1} recorded {recording.RecordedAtUtc:u}.");
            foreach (RecordedAudioSegment segment in recording.Recording.Segments)
            {
                _call.Caller.NotifyCallProgress($"{segment.Speaker.Name}: {segment.RawText}");
            }
        }

        private void DeleteCurrentMessage()
        {
            if (string.IsNullOrWhiteSpace(_currentMessageName))
            {
                _call.Caller.NotifyCallProgress("There is no current message selected to delete.");
                return;
            }

            if (!_grid.DeleteHostedVoicemailMessage(_mailboxNumber, _currentMessageName))
            {
                _call.Caller.NotifyCallProgress("That voicemail message could not be deleted.");
                return;
            }

            _call.Caller.NotifyCallProgress("The current voicemail message is deleted.");
            _currentPlaybackIndex = Math.Max(-1, _currentPlaybackIndex - 1);
            _currentMessageName = null;
            AnnounceMailboxSummary();
        }

        private void DeleteAllMessages()
        {
            _grid.DeleteAllHostedVoicemailMessages(_mailboxNumber);
            _currentPlaybackIndex = -1;
            _currentMessageName = null;
            _call.Caller.NotifyCallProgress("All saved voicemail messages are deleted.");
            AnnounceMailboxSummary();
        }
    }

    private enum HostedVoicemailMode
    {
        RecordIncomingMessage = 0,
        AccessMailbox = 1
    }
}
