#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic.Environment;

namespace MudSharp.Construction;

public partial class Terrain
{
	public long? EnvironmentalMagicProfileId { get; private set; }

	public void SetEnvironmentalMagicProfile(long? profileId)
	{
		if (EnvironmentalMagicProfileId == profileId) return;
		EnvironmentalMagicProfileId = profileId;
		Changed = true;
		Gameworld.EnvironmentalMagic?.TerrainDefaultChanged(this);
	}

	private string DescribeEnvironmentalProfile(ICharacter actor)
	{
		if (!EnvironmentalMagicProfileId.HasValue)
		{
			return "None".ColourValue();
		}

		var id = EnvironmentalMagicProfileId.Value;
		return Gameworld.MagicResourceRegenerators.Get(id) is IEnvironmentalMagicProfile profile
			? $"{profile.Name} (#{id.ToString("N0", actor)})".ColourName()
			: $"Missing environmental regenerator #{id.ToString("N0", actor)}".ColourError();
	}

	private bool BuildingCommandEnvironment(ICharacter actor, StringStack command)
	{
		var reference = command.SafeRemainingArgument;
		if (reference.EqualTo("none"))
		{
			SetEnvironmentalMagicProfile(null);
			actor.OutputHandler.Send("This terrain no longer supplies an environmental magic profile.");
			return true;
		}

		if (Gameworld.MagicResourceRegenerators.GetByIdOrName(reference) is not IEnvironmentalMagicProfile profile)
		{
			actor.OutputHandler.Send("Specify none or the name/ID of an environmental magic regenerator.".ColourError());
			return false;
		}

		SetEnvironmentalMagicProfile(profile.Id);
		actor.OutputHandler.Send($"This terrain now supplies {profile.Name.ColourName()} to cells that inherit their environmental profile.");
		return true;
	}
}
