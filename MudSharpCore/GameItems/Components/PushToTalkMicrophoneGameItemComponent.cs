#nullable enable

using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class PushToTalkMicrophoneGameItemComponent : MediaEndpointPoweredComponentBase, IMediaSource, ITransmit
{
	private PushToTalkMicrophoneGameItemComponentProto _prototype;
	private Guid _streamId = Guid.NewGuid();
	private long _sequence;

	public PushToTalkMicrophoneGameItemComponent(PushToTalkMicrophoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public PushToTalkMicrophoneGameItemComponent(MudSharp.Models.GameItemComponent component,
		PushToTalkMicrophoneGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadRuntimeState(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PushToTalkMicrophoneGameItemComponent(PushToTalkMicrophoneGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_streamId = Guid.NewGuid();
		_sequence = rhs._sequence;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public MediaEndpointAddress MediaEndpoint => new(Parent.Id, Id, _prototype.EndpointKey, MediaEndpointDirection.Output);
	public MediaCapabilities MediaCapabilities => MediaCapabilities.Audio;
	public bool MediaAvailable => IsPowered && Parent.TrueLocations.Any();
	public bool ManualTransmit => true;
	public string TransmitPremote => _prototype.TransmitPremote;
	protected override int MediaOutputPorts => _prototype.OutputPorts;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PushToTalkMicrophoneGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (PushToTalkMicrophoneGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		return SaveMediaEndpointState(root);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		LoadRuntimeState(root);
	}

	protected override void OnPowerCutInAction()
	{
	}

	protected override void OnPowerCutOutAction()
	{
	}

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!MediaAvailable)
		{
			return;
		}

		var origin = spokenLanguage.Origin;
		var character = origin as ICharacter;
		var payload = new MediaLanguagePayload(false, spokenLanguage.Language.Id, spokenLanguage.Accent?.Id ?? 0L,
			spokenLanguage.RawText, (int)spokenLanguage.Volume, (int)spokenLanguage.OriginOutcome, character?.Id,
			origin.Name, (short)origin.ApparentGender(null).Enum, string.Empty, string.Empty,
			(long)RecordedAudioSegment.EstimateDuration(spokenLanguage.RawText).TotalMilliseconds);
		Gameworld.MediaChannelService.Publish(new MediaPacket(_streamId, ++_sequence, DateTime.UtcNow,
			MediaCapabilities.Audio, MediaEventKind.Audio, MediaEndpoint, [MediaEndpoint], payload));
	}

	private void LoadRuntimeState(XElement root)
	{
		LoadMediaEndpointState(root);
	}
}
