#nullable enable

using MudSharp.Construction;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Performances;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>A persistent directed stage-view link, shared by independently owned handles.</summary>
public sealed class StageObservationEffect : Effect, IRemoteObservationEffect
{
	public sealed record Handle(long Id, string Label, string? RunKey, RoomLayer? SourceLayer = null, RoomLayer? AudienceLayer = null);

	private readonly List<Handle> _handles = new();

	public StageObservationEffect(IRoom source, IRoom audience, long id, string label, string? runKey,
		RoomLayer? sourceLayer = null, RoomLayer? audienceLayer = null) : base(source)
	{
		AudienceRoomId = audience.Id;
		_handles.Add(new Handle(id, label, runKey, sourceLayer, audienceLayer));
	}

	public StageObservationEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		AudienceRoomId = (long)definition.Element("AudienceRoomId")!;
		foreach (var element in definition.Elements("Handle"))
		{
			_handles.Add(new Handle((long)element.Attribute("Id")!, (string?)element.Attribute("Label") ?? "Stage",
				(string?)element.Attribute("RunKey"),(RoomLayer?)(int?)element.Attribute("SourceLayer"),
				(RoomLayer?)(int?)element.Attribute("AudienceLayer")));
		}
	}

	public long AudienceRoomId { get; }
	public IReadOnlyList<Handle> Handles => _handles;
	protected override string SpecificEffectType => "StageObservation";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) =>
		$"Stage view to room #{AudienceRoomId.ToString("N0", voyeur).ColourValue()} ({_handles.Count.ToString("N0", voyeur)} handle(s)).";
	protected override XElement SaveDefinition() => new("Effect", new XElement("AudienceRoomId", AudienceRoomId),
		_handles.Select(x => new XElement("Handle", new XAttribute("Id", x.Id), new XAttribute("Label", x.Label),
			x.RunKey is null ? null : new XAttribute("RunKey", x.RunKey),
			x.SourceLayer is null ? null : new XAttribute("SourceLayer",(int)x.SourceLayer),
			x.AudienceLayer is null ? null : new XAttribute("AudienceLayer",(int)x.AudienceLayer))));
	public static void InitialiseEffectType() => RegisterFactory("StageObservation", (effect, owner) => new StageObservationEffect(effect, owner));

	public bool AddHandle(Handle handle)
	{
		if (_handles.Any(x => x.Id == handle.Id)) return false;
		_handles.Add(handle);
		Changed = true;
		return true;
	}

	public bool RemoveHandle(long id)
	{
		if (_handles.RemoveAll(x => x.Id == id) == 0) return false;
		Changed = true;
		return true;
	}

	public void RemoveRunHandles(IReadOnlySet<string> activeRunKeys)
	{
		if (_handles.RemoveAll(x => x.RunKey is not null && !activeRunKeys.Contains(x.RunKey)) > 0) Changed = true;
	}

	public void HandleOutput(string text, ILocation location) => HandleOutput(text, location, null);
	public void HandleOutput(string text, ILocation location, RoomLayer? layer)
	{
		if (!Applies() || string.IsNullOrWhiteSpace(text) || !ReferenceEquals(location, Owner)) return;
		var audience = GetAudience();
		if (audience is null) return;
		foreach (var observer in audience.Characters.ToArray())
		{
			if (observer.State.IsConscious() && observer.OutputHandler is not NonPlayerOutputHandler &&
			    For(observer, layer) is { } handle && CanSeeStage(observer, (IRoom)Owner) && observer.CanSee(Owner))
				observer.OutputHandler.Send($"[{Label(handle, observer)}] {text}");
		}
	}

	public void HandleOutput(IOutput output, ILocation location) => HandleOutput(output, location, null);
	public void HandleOutput(IOutput output, ILocation location, RoomLayer? layer)
	{
		if (!Applies() || !ReferenceEquals(location, Owner) || output.Flags.HasFlag(OutputFlags.IgnoreWatchers) ||
		    output.Flags.HasFlag(OutputFlags.ElectronicOnly) || output.Flags.HasFlag(OutputFlags.WizOnly) ||
		    output.Visibility != OutputVisibility.Normal ||
		    output is IRecordableLanguageOutput { LanguageInfo: SpokenLanguageInfo { Volume: <= AudioVolume.Quiet } }) return;
		var audience = GetAudience();
		if (audience is null) return;
		foreach (var observer in audience.Characters.ToArray())
		{
			if (observer.State.IsConscious() && observer.OutputHandler is not NonPlayerOutputHandler && For(observer,layer) is { } handle &&
			    RenderFor(output, observer, Owner) is { } text &&
			    observer.OutputHandler.Send($"[{Label(handle, observer)}] {text}"))
				StageEmissionContext.StageDelivered(observer);
		}
	}

	private string Label(Handle handle, ICharacter observer)
	{
		if (handle.RunKey is null) return handle.Label;
		var room = (IRoom)Owner;
		return CanSeeStage(observer, room) && observer.CanSee(room) ? room.HowSeen(observer, colour: false) : "Stage";
	}

	private static bool CanSeeStage(ICharacter viewer, IRoom room) =>
		viewer.IsAdministrator() || viewer.IlluminationSightDifficulty(room) != Difficulty.Impossible;

	internal static string? RenderFor(IOutput output, ICharacter viewer, IPerceivable room)
	{
		if (!output.ShouldSee(viewer)) return null;
		var source = output switch
		{
			IRecordableLanguageOutput language => language.DefaultSource,
			IEmoteOutput emote => emote.DefaultSource,
			IMediaPacketOutput media => media.PresentationSource,
			_ => room
		};
		var sees = viewer.CanSee(source) && (room is not IRoom stage || CanSeeStage(viewer, stage));
		var hears = viewer.CanHear(source);
		if ((!sees && !hears) || (!sees && output.Flags.HasFlag(OutputFlags.PurelyVisual)) ||
		    (!hears && (output is AudioOutput || output.Flags.HasFlag(OutputFlags.PurelyAudible)))) return null;

		if (output is IRecordableLanguageOutput speech)
		{
			if (speech.LanguageInfo.Form != LanguageForm.Spoken)
				return sees && speech.LanguageInfo.Form != LanguageForm.Psychic ? output.ParseFor(viewer) : null;
			if (!sees) return $"{(output as LanguageOutput)?.AudiblePreamble ?? "A voice speaks"}{speech.LanguageInfo.ParseFor(viewer)}";
			if (!hears) return $"{speech.PreLanguageEmote.ParseFor(viewer)} something that you cannot hear.".Proper();
			return output.ParseFor(viewer);
		}

		if (sees || output is AudioOutput or IMediaPacketOutput || output.Flags.HasFlag(OutputFlags.PurelyAudible))
			return output.ParseFor(viewer);

		if (output is not EmoteOutput action) return null;
		var audible = new List<string>();
		if (action is InstrumentPerformanceOutput instrument &&
		    AudioPerception.CanHear(viewer, instrument.Sound.Source, instrument.Volume, Proximity.Distant))
			audible.Add(instrument.Sound.ParseFor(viewer));
		foreach (var emote in new[] { action.GetEmoteFor(viewer), (action as AppendableEmoteOutput)?.AppendedEmote }.OfType<Emote>())
		{
			foreach (var token in emote.LanguageTokens.Where(x => x.PermitLanguageOptions != PermitLanguageOptions.IgnoreLanguage))
				audible.Add($"A voice says {token.DisplayThirdPerson(viewer, PerceiveIgnoreFlags.None)}");
		}
		return audible.Count > 0 ? string.Join(" ", audible) : null;
	}

	private Handle? For(ICharacter observer,RoomLayer? layer) => _handles.FirstOrDefault(x =>
		(x.SourceLayer is null || layer is null || x.SourceLayer == layer) &&
		(x.AudienceLayer is null || x.AudienceLayer == observer.RoomLayer));

	private IRoom? GetAudience()
	{
		var audience = Gameworld.Rooms.Get(AudienceRoomId);
		if (audience is not null && audience.RouteDefinition is null &&
		    !audience.EffectsOfType<RoomTemplateInstance>().Any(x => x.IsTemplate)) return audience;
		Owner.RemoveEffect(this);
		return null;
	}
}
