#nullable enable

using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

/// <summary>A directed visual subscription to structured communication, without spoken words.</summary>
public sealed class RoomObservationEffect : Effect, IRemoteObservationEffect
{
	public RoomObservationEffect(IRoom owner, long observerRoomId, string prefix) : base(owner)
	{
		ObserverRoomId = observerRoomId;
		Prefix = prefix;
	}

	public RoomObservationEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		ObserverRoomId = (long)definition.Element("ObserverRoomId")!;
		Prefix = (string?)definition.Element("Prefix") ?? string.Empty;
	}

	public long ObserverRoomId { get; private set; }
	public string Prefix { get; }
	protected override string SpecificEffectType => "RoomObservation";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) =>
		$"Visual communication is observed from room #{ObserverRoomId.ToString("N0", voyeur).ColourValue()}: {Prefix.ColourCommand()}";
	protected override XElement SaveDefinition() => new("Effect",
		// This serializer was introduced after the parent-Room removal; IDs always name native playable rooms.
		new XElement("ObserverRoomId", ObserverRoomId), new XElement("Prefix", Prefix));
	public static void InitialiseEffectType() =>
		RegisterFactory("RoomObservation", (effect, owner) => new RoomObservationEffect(effect, owner));

	public void RemapObserver(IReadOnlyDictionary<IRoom, IRoom> rooms)
	{
		var match = rooms.FirstOrDefault(x => x.Key.Id == ObserverRoomId);
		if (match.Value is not null) ObserverRoomId = match.Value.Id;
	}

	public void HandleOutput(string text, ILocation location)
	{
		// Raw text has no authenticated source or sensory classification.
	}

	public void HandleOutput(IOutput output, ILocation location)
	{
		if (!Applies() || output.Flags.HasFlag(OutputFlags.IgnoreWatchers) ||
		    output.Flags.HasFlag(OutputFlags.ElectronicOnly) ||
		    output.Visibility != OutputVisibility.Normal ||
		    output is not IRecordableLanguageOutput language || language.LanguageInfo.Form != LanguageForm.Spoken ||
		    language.DefaultSource is not ICharacter source || !ReferenceEquals(source.Location, Owner)) return;
		var observationRoom = Gameworld.Rooms.Get(ObserverRoomId);
		if (observationRoom is null || observationRoom.RouteDefinition is not null) return;
		if (observationRoom.EffectsOfType<RoomTemplateInstance>().Any(x => x.IsTemplate)) return;
		foreach (var observer in observationRoom.Characters.ToArray())
		{
			if (observer == source || !observer.State.IsConscious() || !observer.CanSee(source) ||
			    output.Flags.HasFlag(OutputFlags.WizOnly) && !observer.IsAdministrator()) continue;
			// Do not parse the language payload or optional player emote: neither is audible through a view link.
			var visibleAction = language.PreLanguageEmote.ParseFor(observer).TrimEnd().TrimEnd('.', '!', '?');
			observer.OutputHandler.Send($"{Prefix} {visibleAction} something.".NormaliseSpacing());
		}
	}
}
