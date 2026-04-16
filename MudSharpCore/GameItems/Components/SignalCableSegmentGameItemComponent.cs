#nullable enable

using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class SignalCableSegmentGameItemComponent : GameItemComponent, ISignalCableSegment
{
	private SignalCableSegmentGameItemComponentProto _prototype;
	private ISignalSourceComponent? _source;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;
	private LocalSignalBinding? _binding;

	public SignalCableSegmentGameItemComponent(SignalCableSegmentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SignalCableSegmentGameItemComponent(MudSharp.Models.GameItemComponent component,
		SignalCableSegmentGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public SignalCableSegmentGameItemComponent(SignalCableSegmentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = rhs._binding;
		SourceCellId = rhs.SourceCellId;
		DestinationCellId = rhs.DestinationCellId;
		RouteDescription = rhs.RouteDescription;
		_currentSignal = rhs._currentSignal;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;
	public LocalSignalBinding CurrentBinding => _binding ?? new LocalSignalBinding(0L, string.Empty, 0L, string.Empty, EndpointKey);
	public long RoutedExitId { get; private set; }
	public bool IsRouted => _binding is not null && SourceCellId > 0 && DestinationCellId > 0;
	public long SourceCellId { get; private set; }
	public long DestinationCellId { get; private set; }
	public string RouteDescription { get; private set; } = string.Empty;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SignalCableSegmentGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		return !IsRouted
			? $"{description}\n\nIt is not currently routed to any signal source."
			: $"{description}\n\nIt is currently mirroring {SignalComponentUtilities.DescribeSignalComponent(CurrentBinding).ColourName()} across {RouteDescription.ColourCommand()} and outputting {CurrentValue.ToString("N2", voyeur).ColourValue()}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SignalCableSegmentGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			_binding is null
				? null
				: new XElement("Binding",
					new XAttribute("sourceitemid", _binding.SourceItemId),
					new XAttribute("sourceitem", _binding.SourceItemName),
					new XAttribute("sourceid", _binding.SourceComponentId),
					new XAttribute("source", _binding.SourceComponentName),
					new XAttribute("endpoint", _binding.SourceEndpointKey)),
			new XElement("SourceCellId", SourceCellId),
			new XElement("DestinationCellId", DestinationCellId),
			new XElement("RoutedExitId", RoutedExitId),
			new XElement("RouteDescription", new XCData(RouteDescription))
		).ToString();
	}

	public override void FinaliseLoad()
	{
		RefreshRoute();
	}

	public override void Delete()
	{
		DetachSource();
		base.Delete();
	}

	public override void Quit()
	{
		DetachSource();
		base.Quit();
	}

	public bool ConfigureRoute(ISignalSourceComponent source, long exitId, out string error)
	{
		if (Parent.TrueLocations.FirstOrDefault() is not ICell destinationCell)
		{
			error = "That cable must be located in a room before it can be routed.";
			return false;
		}

		var sourceCell = source.Parent.TrueLocations.FirstOrDefault();
		if (sourceCell is null)
		{
			error = "That source is not currently in a valid location.";
			return false;
		}

		var routeExit = sourceCell.ExitsFor(source.Parent)
			.FirstOrDefault(x => x.Exit.Id == exitId && x.Destination == destinationCell);
		if (routeExit is null)
		{
			error = "That source is not connected to this cable by the specified one-room exit hop.";
			return false;
		}

		_binding = SignalComponentUtilities.CreateBinding(source);
		SourceCellId = sourceCell.Id;
		DestinationCellId = destinationCell.Id;
		RoutedExitId = exitId;
		RouteDescription = routeExit.OutboundDirectionDescription;
		Changed = true;
		RefreshRoute();
		error = string.Empty;
		return true;
	}

	public void ClearRoute()
	{
		_binding = null;
		SourceCellId = 0L;
		DestinationCellId = 0L;
		RoutedExitId = 0L;
		RouteDescription = string.Empty;
		Changed = true;
		DetachSource();
		SetSignal(default);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		SetSignal(signal);
	}

	private void LoadFromXml(XElement root)
	{
		var binding = root.Element("Binding");
		if (binding is not null)
		{
			_binding = new LocalSignalBinding(
				long.TryParse(binding.Attribute("sourceitemid")?.Value, out var sourceItemId) ? sourceItemId : 0L,
				binding.Attribute("sourceitem")?.Value ?? string.Empty,
				long.TryParse(binding.Attribute("sourceid")?.Value, out var sourceId) ? sourceId : 0L,
				binding.Attribute("source")?.Value ?? string.Empty,
				SignalComponentUtilities.NormaliseSignalEndpointKey(binding.Attribute("endpoint")?.Value));
		}

		SourceCellId = long.TryParse(root.Element("SourceCellId")?.Value, out var sourceCellId) ? sourceCellId : 0L;
		DestinationCellId = long.TryParse(root.Element("DestinationCellId")?.Value, out var destinationCellId)
			? destinationCellId
			: 0L;
		RoutedExitId = long.TryParse(root.Element("RoutedExitId")?.Value, out var routedExitId) ? routedExitId : 0L;
		RouteDescription = root.Element("RouteDescription")?.Value ?? string.Empty;
	}

	private void RefreshRoute()
	{
		if (_binding is null || SourceCellId <= 0 || DestinationCellId <= 0)
		{
			DetachSource();
			SetSignal(default);
			return;
		}

		var sourceItem = Gameworld.TryGetItem(_binding.SourceItemId, true);
		if (sourceItem is null || !sourceItem.TrueLocations.Any(x => x.Id == SourceCellId) ||
		    !Parent.TrueLocations.Any(x => x.Id == DestinationCellId))
		{
			DetachSource();
			SetSignal(default);
			return;
		}

		var sourceCell = sourceItem.TrueLocations.First(x => x.Id == SourceCellId);
		var destinationCell = Parent.TrueLocations.First(x => x.Id == DestinationCellId);
		var routeExit = sourceCell.ExitsFor(sourceItem)
			.FirstOrDefault(x => x.Exit.Id == RoutedExitId && x.Destination == destinationCell);
		if (routeExit is null)
		{
			DetachSource();
			SetSignal(default);
			return;
		}

		var endpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(_binding.SourceEndpointKey);
		var resolvedSource = SignalComponentUtilities.FindSignalSourceOnItem(sourceItem, _binding.SourceComponentId,
			_binding.SourceComponentName, endpointKey);
		if (!ReferenceEquals(_source, resolvedSource))
		{
			DetachSource();
			_source = resolvedSource;
			if (_source is not null)
			{
				_source.SignalChanged += HandleSourceSignalChanged;
			}
		}

		RouteDescription = routeExit.OutboundDirectionDescription;
		SetSignal(_source?.CurrentSignal ?? default);
		if (!_heartbeatSubscribed)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatTick;
			_heartbeatSubscribed = true;
		}
	}

	private void HeartbeatTick()
	{
		RefreshRoute();
	}

	private void HandleSourceSignalChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		SetSignal(signal);
	}

	private void DetachSource()
	{
		if (_source is not null)
		{
			_source.SignalChanged -= HandleSourceSignalChanged;
			_source = null;
		}

		if (_heartbeatSubscribed)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatTick;
			_heartbeatSubscribed = false;
		}
	}

	private void SetSignal(ComputerSignal signal)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}

		_currentSignal = signal;
		SignalChanged?.Invoke(this, _currentSignal);
	}
}
