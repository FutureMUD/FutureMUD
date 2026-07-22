#nullable enable

using MudSharp.Body.CommunicationStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication;

public static class AlertUtilities
{
	public const uint AlertRoomRange = 2;
	public const AudioVolume AlertVolume = AudioVolume.ExtremelyLoud;
	public const int MaximumStoredAlertEmoteLength = 500;
	private const string SampleDirectionText = "immediately to the north";

	public static bool ValidateAlertEmote(string emoteText, IPerceiver source, out string error)
	{
		var emote = new PlayerEmote(emoteText, source, true, PermitLanguageOptions.IgnoreLanguage);
		if (emote.Valid)
		{
			error = string.Empty;
			return true;
		}

		error = emote.ErrorMessage;
		return false;
	}

	public static bool ValidateDistantAlertEmote(string emoteText, IPerceiver source, out string error)
	{
		if (!emoteText.Contains("{0}", StringComparison.InvariantCulture))
		{
			error = "The distant alert emote must include {0} where the direction and distance text should appear.";
			return false;
		}

		if (!TryFormatDistantAlertEmote(emoteText, SampleDirectionText, out var formattedText, out error))
		{
			return false;
		}

		var emote = new Emote(formattedText, source, PermitLanguageOptions.IgnoreLanguage, source);
		if (emote.Valid)
		{
			error = string.Empty;
			return true;
		}

		error = emote.ErrorMessage;
		return false;
	}

	public static bool ValidateStoredAlertEmote(string emoteText, IPerceiver source, out string error)
	{
		if (!ValidateStoredAlertLength(emoteText, out error))
		{
			return false;
		}

		return ValidateAlertEmote(emoteText, source, out error);
	}

	public static bool ValidateStoredDistantAlertEmote(string emoteText, IPerceiver source, out string error)
	{
		if (!ValidateStoredAlertLength(emoteText, out error))
		{
			return false;
		}

		return ValidateDistantAlertEmote(emoteText, source, out error);
	}

	public static bool DoAlert(ICharacter actor, string? alertEmote = null, string? distantAlertEmote = null, bool echoFailure = true)
	{
		if (!actor.Body.Communications.CanVocalise(actor.Body, AlertVolume))
		{
			if (echoFailure)
			{
				actor.OutputHandler.Send(actor.Body.Communications.WhyCannotVocalise(actor.Body, AlertVolume));
			}

			return false;
		}

		var localText = alertEmote.IfNullOrWhiteSpace(ResolveAlertEmote(actor));
		var distantText = distantAlertEmote.IfNullOrWhiteSpace(ResolveDistantAlertEmote(actor));
		var localEmote = BuildLocalEmote(actor, localText);
		if (!localEmote.Valid)
		{
			if (echoFailure)
			{
				actor.OutputHandler.Send(localEmote.ErrorMessage);
			}

			return false;
		}

		if (!ValidateDistantAlertEmote(distantText, actor, out var distantError))
		{
			if (echoFailure)
			{
				actor.OutputHandler.Send(distantError);
			}

			return false;
		}

		actor.OutputHandler.Handle(new EmoteOutput(localEmote));
		FireAlertEventForSameLayer(actor, localText);
		SendAlertToOtherLayers(actor, distantText);
		SendAlertToNearbyRooms(actor, distantText);
		return true;
	}

	public static string ResolveAlertEmote(ICharacter actor)
	{
		return actor.CustomAlertEmote.IfNullOrWhiteSpace(
			ResolveNpcAlertOverride(actor).IfNullOrWhiteSpace(
				actor.Race.DefaultAlertEmote.IfNullOrWhiteSpace(actor.Gameworld.GetStaticString("DefaultAlertEmote"))
			)
		);
	}

	public static string ResolveDistantAlertEmote(ICharacter actor)
	{
		return actor.CustomDistantAlertEmote.IfNullOrWhiteSpace(
			ResolveNpcDistantAlertOverride(actor).IfNullOrWhiteSpace(
				actor.Race.DefaultDistantAlertEmote.IfNullOrWhiteSpace(actor.Gameworld.GetStaticString("DefaultDistantAlertEmote"))
			)
		);
	}

	private static string? ResolveNpcAlertOverride(ICharacter actor)
	{
		return actor is INPC npc
			? npc.AIs.OfType<IOverrideAlertEmote>()
			     .Select(x => x.AlertEmote)
			     .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
			: null;
	}

	private static string? ResolveNpcDistantAlertOverride(ICharacter actor)
	{
		return actor is INPC npc
			? npc.AIs.OfType<IOverrideAlertEmote>()
			     .Select(x => x.DistantAlertEmote)
			     .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
			: null;
	}

	private static PlayerEmote BuildLocalEmote(ICharacter actor, string emoteText)
	{
		if (actor.Body.Communications is HumanoidCommunicationStrategy humanoid && humanoid.IsGagged(actor.Body))
		{
			return new PlayerEmote("@ make|makes a muffled alert into &0's gag.", actor, true,
				PermitLanguageOptions.IgnoreLanguage);
		}

		return new PlayerEmote(emoteText, actor, true, PermitLanguageOptions.IgnoreLanguage);
	}

	private static string BuildDistantText(ICharacter actor, string emoteText, string directionText)
	{
		if (actor.Body.Communications is HumanoidCommunicationStrategy humanoid && humanoid.IsGagged(actor.Body))
		{
			return $"A muffled alert can be heard {directionText}.";
		}

		return TryFormatDistantAlertEmote(emoteText, directionText, out var text, out _)
			? text
			: actor.Gameworld.GetStaticString("DefaultDistantAlertEmote");
	}

	private static void FireAlertEventForSameLayer(ICharacter actor, string localText)
	{
		foreach (var (witness, proximity) in actor.LocalThingsAndProximities()
			         .Where(x => x.Proximity <= Proximity.VeryDistant)
			         .Where(x => x.Thing.RoomLayer == actor.RoomLayer)
			         .Select(x => (x.Thing as ICharacter, x.Proximity))
			         .Where(x => x.Item1 is not null)
			         .Select(x => (x.Item1!, x.Proximity))
			         .Where(x => !x.Item1.IsSelf(actor))
			         .ToList())
		{
			if (!CanHearAlert(actor, witness, AlertVolume, proximity))
			{
				continue;
			}

			witness.HandleEvent(EventType.CharacterAlertHeard, actor, witness, actor.Location, "here", 0.0, localText);
		}
	}

	private static void SendAlertToOtherLayers(ICharacter actor, string distantText)
	{
		foreach (var layer in actor.Location.Terrain(null).TerrainLayers.Except(actor.RoomLayer))
		{
			var layerText = layer.IsHigherThan(actor.RoomLayer) ? "from below" : "from above";
			foreach (var (witness, proximity) in actor.LocalThingsAndProximities()
			         .Where(x => x.Proximity <= Proximity.VeryDistant)
			         .Where(x => x.Thing.RoomLayer == layer)
			         .Select(x => (x.Thing as ICharacter, x.Proximity))
			         .Where(x => x.Item1 is not null)
			         .Select(x => (x.Item1!, x.Proximity))
			         .ToList())
			{
				var volume = AudioVolume.Decent;
				if (!CanHearAlert(actor, witness, volume, proximity))
				{
					continue;
				}

				witness.OutputHandler.Send(new EmoteOutput(
					new Emote(BuildDistantText(actor, distantText, layerText), actor, PermitLanguageOptions.IgnoreLanguage, actor),
					flags: OutputFlags.PurelyAudible));
				witness.HandleEvent(EventType.CharacterAlertHeard, actor, witness, actor.Location, layerText, 1.0,
					distantText);
			}
		}
	}

	private static void SendAlertToNearbyRooms(ICharacter actor, string distantText)
	{
		var allCells = actor.Location.CellsInVicinity(AlertRoomRange, exit => true, cell => true).ToList();
		var surrounds = actor.Location.Surrounds.ToList();
		foreach (var cell in allCells)
		{
			if (cell == actor.Location)
			{
				continue;
			}

			foreach (var witness in cell.Characters.ToList())
			{
				var path = DirectionPathFrom(cell, actor);
				var distance = path.Count;
				var directionText = DirectionText(actor, cell, witness, path, surrounds.Contains(cell));
				var volume = distance <= 1 ? AudioVolume.Decent : AudioVolume.Faint;
				if (!CanHearAlert(actor, witness, volume, Proximity.VeryDistant))
				{
					continue;
				}

				witness.OutputHandler.Send(new EmoteOutput(
					new Emote(BuildDistantText(actor, distantText, directionText), actor,
						PermitLanguageOptions.IgnoreLanguage, actor),
					flags: OutputFlags.PurelyAudible));
				witness.HandleEvent(EventType.CharacterAlertHeard, actor, witness, actor.Location, directionText,
					(double)Math.Max(1, distance), distantText);
			}
		}
	}

	private static List<ICellExit> DirectionPathFrom(ICell cell, ICharacter actor)
	{
		return cell.PathBetween(actor, AlertRoomRange, PathSearch.IgnorePresenceOfDoors).ToList();
	}

	private static string DirectionText(ICharacter actor, ICell cell, ICharacter witness, List<ICellExit> path,
		bool neighbouringCell)
	{
		if (neighbouringCell)
		{
			return actor.Location.GetExitTo(cell, witness)?.InboundDirectionSuffix ?? "from somewhere unknown";
		}

		return path.Any()
			? path.DescribeDirectionsToFrom()
			: "from somewhere unknown";
	}

	private static bool CanHearAlert(ICharacter actor, ICharacter witness, AudioVolume volume, Proximity proximity)
	{
		if (witness.IsSelf(actor))
		{
			return true;
		}

		if (!witness.CanHear(actor))
		{
			return false;
		}

		var difficulty = witness.Location.LocalAudioDifficulty(witness, volume, proximity);
		return witness.Gameworld.GetCheck(CheckType.GenericListenCheck)
		              .Check(witness, difficulty, actor)
		              .IsPass();
	}

	private static bool TryFormatDistantAlertEmote(string emoteText, string directionText, out string formattedText,
		out string error)
	{
		try
		{
			formattedText = string.Format(emoteText, directionText);
			error = string.Empty;
			return true;
		}
		catch (FormatException)
		{
			formattedText = string.Empty;
			error = "That distant alert emote has invalid format placeholders. Use only {0} for the direction and distance.";
			return false;
		}
	}

	private static bool ValidateStoredAlertLength(string emoteText, out string error)
	{
		if (emoteText.Length <= MaximumStoredAlertEmoteLength)
		{
			error = string.Empty;
			return true;
		}

		error = $"Stored ALERT emotes must be {MaximumStoredAlertEmoteLength.ToString("N0")} characters or fewer.";
		return false;
	}
}
