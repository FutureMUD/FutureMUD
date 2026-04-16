#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Components;

internal sealed class LocalSignalSinkSubscription
{
	private readonly IGameItem _parent;
	private readonly IGameItemComponent _owner;
	private readonly SignalChangedEvent _signalHandler;

	private ISignalSourceComponent? _source;

	public LocalSignalSinkSubscription(IGameItem parent, IGameItemComponent owner, SignalChangedEvent signalHandler)
	{
		_parent = parent;
		_owner = owner;
		_signalHandler = signalHandler;
	}

	public ISignalSource? UpstreamSource => _source;

	public void Reconnect(LocalSignalBinding binding)
	{
		Reconnect(binding.SourceItemId, binding.SourceItemName, binding.SourceComponentId, binding.SourceComponentName,
			binding.SourceEndpointKey);
	}

	public void Reconnect(long sourceIdentifier, string sourceComponentName, string? sourceEndpointKey)
	{
		Reconnect(0L, string.Empty, sourceIdentifier, sourceComponentName, sourceEndpointKey);
	}

	public void Reconnect(long sourceItemId, string sourceItemName, long sourceIdentifier, string sourceComponentName,
		string? sourceEndpointKey)
	{
		Detach();
		_source = SignalComponentUtilities.FindSignalSource(_parent, sourceItemId, sourceItemName, sourceIdentifier, sourceComponentName,
			sourceEndpointKey, _owner);
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged += _signalHandler;
		_signalHandler(_source, _source.CurrentSignal);
	}

	public void Detach()
	{
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged -= _signalHandler;
		_source = null;
	}
}
