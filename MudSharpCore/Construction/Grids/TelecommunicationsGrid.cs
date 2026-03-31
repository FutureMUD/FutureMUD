#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

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
	private readonly List<long> _linkedGridIds = [];
	private readonly HashSet<ITelecommunicationsGrid> _linkedGrids = [];

	private readonly List<long> _connectedConsumerIds = [];
	private readonly List<IConsumePower> _connectedConsumers = [];
	private readonly List<long> _connectedProducerIds = [];
	private readonly List<IProducePower> _connectedProducers = [];
	private readonly List<IConsumePower> _idleConsumers = [];
	private bool _gridPowered;
	private bool _ringHeartbeatSubscribed;

	public TelecommunicationsGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
	{
		var root = XElement.Parse(grid.Definition);
		Prefix = root.Element("Prefix")?.Value ?? "555";
		NumberLength = int.Parse(root.Element("NumberLength")?.Value ?? "4");
		NextNumber = long.Parse(root.Element("NextNumber")?.Value ?? "1");
		MaximumRings = int.Parse(root.Element("MaxRings")?.Value ?? DefaultMaximumRings.ToString());

		foreach (var element in root.Elements("Endpoint").Concat(root.Elements("Phone")))
		{
			var componentId = long.TryParse(element.Attribute("component")?.Value, out var parsedComponentId)
				? parsedComponentId
				: 0L;
			var itemId = long.TryParse(element.Attribute("item")?.Value, out var parsedItemId)
				? parsedItemId
				: long.Parse(element.Attribute("id")!.Value);
			var number = element.Attribute("number")!.Value;
			_loadedAssignments.Add(new LoadedAssignment(componentId, itemId, number));
		}

		foreach (var element in root.Elements("Consumer"))
		{
			_connectedConsumerIds.Add(long.Parse(element.Value));
		}

		foreach (var element in root.Elements("Producer"))
		{
			_connectedProducerIds.Add(long.Parse(element.Value));
		}

		foreach (var element in root.Elements("LinkedGrid"))
		{
			_linkedGridIds.Add(long.Parse(element.Value));
		}
	}

	public TelecommunicationsGrid(IFuturemud gameworld, ICell? initialLocation, string prefix, int numberLength)
		: base(gameworld, initialLocation)
	{
		Prefix = prefix;
		NumberLength = numberLength;
		MaximumRings = DefaultMaximumRings;
		NextNumber = 1;
	}

	public TelecommunicationsGrid(ITelecommunicationsGrid rhs) : base(rhs)
	{
		Prefix = rhs.Prefix;
		NumberLength = rhs.NumberLength;
		MaximumRings = rhs.MaximumRings;
		NextNumber = rhs is TelecommunicationsGrid grid ? grid.NextNumber : 1;
	}

	public override string GridType => "Telecommunications";
	public string Prefix { get; }
	public int NumberLength { get; }
	public int MaximumRings { get; private set; }
	public double TotalSupply => _connectedProducers.Sum(x => x.MaximumPowerInWatts);
	public double TotalDrawdown => _connectedConsumers.Except(_idleConsumers).Sum(x => x.PowerConsumptionInWatts);
	public IEnumerable<ITelecommunicationsGrid> LinkedGrids => _linkedGrids.ToList();
	private long NextNumber { get; set; }

	public override void LoadTimeInitialise()
	{
		base.LoadTimeInitialise();

		foreach (var id in _connectedConsumerIds)
		{
			var consumer = FindItemComponent<IConsumePower>(itemId: id);
			if (consumer == null)
			{
				continue;
			}

			_connectedConsumers.Add(consumer);
		}

		foreach (var id in _connectedProducerIds)
		{
			var producer = FindItemComponent<IProducePower>(itemId: id);
			if (producer == null)
			{
				continue;
			}

			_connectedProducers.Add(producer);
		}

		_connectedConsumerIds.Clear();
		_connectedProducerIds.Clear();
		_idleConsumers.AddRange(_connectedConsumers);

		foreach (var linkedGridId in _linkedGridIds)
		{
			if (Gameworld.Grids.Get(linkedGridId) is ITelecommunicationsGrid linkedGrid &&
			    !ReferenceEquals(linkedGrid, this))
			{
				_linkedGrids.Add(linkedGrid);
			}
		}

		_linkedGridIds.Clear();

		foreach (var assignment in _loadedAssignments)
		{
			var owner = FindItemComponent<ITelephoneNumberOwner>(assignment.ComponentId, assignment.ItemId);
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
		var root = base.SaveDefinition();
		root.Add(new XElement("Prefix", Prefix));
		root.Add(new XElement("NumberLength", NumberLength));
		root.Add(new XElement("NextNumber", NextNumber));
		root.Add(new XElement("MaxRings", MaximumRings));

		foreach (var linkedGrid in _linkedGrids.OrderBy(x => x.Id))
		{
			root.Add(new XElement("LinkedGrid", linkedGrid.Id));
		}

		foreach (var (owner, number) in _ownerNumbers.OrderBy(x => x.Value).ThenBy(x => x.Key.Id))
		{
			root.Add(new XElement("Endpoint",
				new XAttribute("component", owner.Id),
				new XAttribute("item", owner.Parent.Id),
				new XAttribute("number", number)
			));
		}

		foreach (var consumer in _connectedConsumers)
		{
			root.Add(new XElement("Consumer", consumer.Parent.Id));
		}

		foreach (var producer in _connectedProducers)
		{
			root.Add(new XElement("Producer", producer.Parent.Id));
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
		foreach (var phone in owner.ConnectedTelephones.ToList())
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

		foreach (var consumer in _connectedConsumers.Except(_idleConsumers)
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
			foreach (var consumer in _connectedConsumers.Except(_idleConsumers)
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
			foreach (var consumer in _idleConsumers.OrderBy(x => x.PowerConsumptionInWatts).ToList())
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
		var normalised = Normalise(number);
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

		var destinationGrid = ResolveDestinationGrid(normalised, out error);
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
		var owners = GetOwnersForNumber(normalised).ToList();
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

		var targetPhones = owners.SelectMany(TelephoneNetworkHelpers.CollectConnectedTelephones)
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

		var ringablePhones = targetPhones.Where(x => x.CanReceiveCalls).ToList();
		if (!ringablePhones.Any())
		{
			error = "That line cannot receive calls right now.";
			return false;
		}

		var call = new TelecommunicationsCall(this, normalised, caller);
		_activeCallsByNumber[normalised] = call;
		EnsureRingHeartbeat();
		caller.BeginOutgoingCall(call, normalised);
		foreach (var phone in ringablePhones)
		{
			call.AddRingingPhone(phone);
		}

		call.NotifyCallerOfRinging();

		error = string.Empty;
		return true;
	}

	public bool TryPickUp(ITelephone phone, out string error)
	{
		if (phone.CurrentCall is TelecommunicationsCall currentCall)
		{
			return currentCall.TryAnswer(phone, out error);
		}

		var number = phone.NumberOwner?.PhoneNumber;
		if (string.IsNullOrWhiteSpace(number))
		{
			error = "That telephone is not connected to a telecommunications line.";
			return false;
		}

		if (!_activeCallsByNumber.TryGetValue(number, out var call))
		{
			error = "There is no live call on that line right now.";
			return false;
		}

		return call.TryAnswer(phone, out error);
	}

	public bool TryResolvePhone(string number, out ITelephone? phone)
	{
		var normalised = Normalise(number);
		var destinationGrid = ResolveDestinationGrid(normalised, out _);
		phone = destinationGrid?.GetOwnersForNumber(normalised)
		                 .SelectMany(TelephoneNetworkHelpers.CollectConnectedTelephones)
		                 .FirstOrDefault();
		return phone != null;
	}

	public IEnumerable<ITelephoneNumberOwner> GetOwnersForNumber(string number)
	{
		return _ownersByNumber.TryGetValue(Normalise(number), out var owners)
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
		var previousNumber = _ownerNumbers.GetValueOrDefault(owner);
		ReleaseNumber(owner);

		if (!string.IsNullOrWhiteSpace(preferredNumber))
		{
			var normalised = Normalise(preferredNumber);
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

		var number = GenerateNumber();
		ConnectOwner(owner, number);
		return true;
	}

	public void ReleaseNumber(ITelephoneNumberOwner owner)
	{
		if (!_ownerNumbers.Remove(owner, out var number))
		{
			owner.AssignPhoneNumber(null);
			return;
		}

		if (_ownersByNumber.TryGetValue(number, out var owners))
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
		if (!_ownersByNumber.TryGetValue(number, out var owners))
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
		if (!_ownersByNumber.TryGetValue(number, out var owners))
		{
			return true;
		}

		return owners.All(x => x == owner) || allowSharedNumber;
	}

	private string GenerateNumber()
	{
		while (true)
		{
			var number = $"{Prefix}{NextNumber.ToString().PadLeft(NumberLength, '0')}";
			NextNumber++;
			if (_ownersByNumber.ContainsKey(number))
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

	private ITelecommunicationsGrid? ResolveDestinationGrid(string number, out string error)
	{
		if (number.StartsWith(Prefix, StringComparison.InvariantCultureIgnoreCase))
		{
			error = string.Empty;
			return this;
		}

		var matches = _linkedGrids.Where(x => number.StartsWith(x.Prefix, StringComparison.InvariantCultureIgnoreCase))
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
		foreach (var call in _activeCallsByNumber.Values.ToList())
		{
			call.HandleRingHeartbeat();
		}
	}

	private TComponent? FindItemComponent<TComponent>(long componentId = 0, long itemId = 0)
		where TComponent : class, MudSharp.GameItems.IGameItemComponent
	{
		if (componentId > 0)
		{
			var component = Gameworld.Items.SelectMany(x => x.Components)
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
		var sb = new StringBuilder();
		sb.AppendLine($"Grid #{Id.ToString("N0", actor)}");
		sb.AppendLine($"Type: {"Telecommunications".Colour(Telnet.BoldBlue)}");
		sb.AppendLine($"Locations: {Locations.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Prefix: {Prefix.ColourValue()}");
		sb.AppendLine($"Subscriber Digits: {NumberLength.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Maximum Rings: {MaximumRings.ToString("N0", actor).ColourValue()}");
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
		public bool IsConnected => _participants.Count > 1;
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
			if (_participants.Contains(phone))
			{
				error = "That telephone is already connected to the call.";
				return false;
			}

			var sameLine = phone.NumberOwner?.PhoneNumber?.EqualTo(Number) == true;
			if (_ringingPhones.Contains(phone))
			{
				var callerWasConnected = Caller.IsConnected;
				_ringingPhones.Remove(phone);
				_participants.Add(phone);
				if (!callerWasConnected)
				{
					Caller.ConnectCall(this);
					Caller.NotifyCallProgress("The call connects.");
				}

				phone.ConnectCall(this);
				phone.NotifyCallProgress("The call connects.");
				foreach (var other in _ringingPhones.ToList())
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
				foreach (var machine in _participants.OfType<IAnsweringMachine>()
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
			if (phone == Caller || _participants.Count <= 1)
			{
				Terminate();
			}
		}

		public void RelayTransmission(ITelephone source, SpokenLanguageInfo spokenLanguage)
		{
			foreach (var phone in _participants.Where(x => x != source).ToList())
			{
				phone.ReceiveTransmission(0.0, spokenLanguage, 0L, source);
			}
		}

		public void RelayDigits(ITelephone source, string digits)
		{
			foreach (var phone in _participants.Where(x => x != source).ToList())
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
				Caller.NotifyCallProgress("The line rings out.");
				Terminate();
				return;
			}

			_ringCount++;
			Caller.NotifyCallProgress("You hear the line ringing.");
			foreach (var machine in _ringingPhones.OfType<IAnsweringMachine>()
			                                     .Where(x => x.AutoAnswerRings <= _ringCount)
			                                     .ToList())
			{
				if (TryAnswer(machine, out _))
				{
					return;
				}
			}
		}

		private void Terminate()
		{
			var phones = _participants.Concat(_ringingPhones).Distinct().ToList();
			_participants.Clear();
			_ringingPhones.Clear();
			_grid.RemoveCall(this);
			foreach (var phone in phones)
			{
				phone.EndCall(this, false);
			}
		}
	}
}
