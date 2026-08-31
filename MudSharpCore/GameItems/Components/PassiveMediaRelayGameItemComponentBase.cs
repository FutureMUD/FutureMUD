#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

/// <summary>
/// A zero-power local relay used by cables and splitters. It inherits the connector persistence implementation
/// from the powered media base, but deliberately registers itself during login without requiring an electrical
/// source; physical media cabling is passive in the game model.
/// </summary>
public abstract class PassiveMediaRelayGameItemComponentBase : MediaBoundSinkPoweredComponentBase, IMediaSource
{
	private long _sequence;

	protected PassiveMediaRelayGameItemComponentBase(PoweredMachineBaseGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
	}

	protected PassiveMediaRelayGameItemComponentBase(MudSharp.Models.GameItemComponent component,
		PoweredMachineBaseGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
	}

	protected PassiveMediaRelayGameItemComponentBase(PassiveMediaRelayGameItemComponentBase rhs,
		IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_sequence = rhs._sequence;
	}

	public override bool MediaAvailable => Parent.TrueLocations.Any();

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		ActivateMediaSink();
	}

	public override void Login()
	{
		base.Login();
		ActivateMediaSink();
	}

	public override void Quit()
	{
		DeactivateMediaSink();
		base.Quit();
	}

	protected override void OnPowerCutInAction()
	{
		// Passive media components do not require electrical power.
	}

	protected override void OnPowerCutOutAction()
	{
		// A connected power source must not disable a passive cable or splitter.
	}

	public override void ReceiveMedia(MediaPacket packet)
	{
		if (!MediaAvailable || packet.HasVisited(MediaEndpoint))
		{
			return;
		}

		var capabilities = packet.Capabilities & MediaCapabilities;
		if (capabilities == MediaCapabilities.None)
		{
			return;
		}

		Gameworld.MediaChannelService.Publish(packet with
		{
			Sequence = ++_sequence,
			TimestampUtc = DateTime.UtcNow,
			Capabilities = capabilities,
			Source = MediaEndpoint,
			Provenance = packet.Provenance.Append(MediaEndpoint).ToArray()
		});
	}
}
