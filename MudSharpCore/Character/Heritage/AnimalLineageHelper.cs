#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character.Heritage;

internal static class AnimalLineageHelper
{
	private static readonly IReadOnlySet<string> StockAnimalBodyFamilies = new HashSet<string>(
	[
		"Quadruped Base",
		"Ungulate",
		"Toed Quadruped",
		"Avian",
		"Vermiform",
		"Serpentine",
		"Piscine",
		"Decapod",
		"Malacostracan",
		"Cephalopod",
		"Jellyfish",
		"Pinniped",
		"Cetacean",
		"Insectoid",
		"Winged Insectoid",
		"Arachnid"
	], System.StringComparer.OrdinalIgnoreCase);

	internal static bool IsAnimal(ICharacter? character)
	{
		return character is not null && IsAnimal(character.Race, character.Gameworld);
	}

	internal static bool IsAnimal(IRace? race, IFuturemud gameworld)
	{
		if (race?.BaseBody is not { } baseBody)
		{
			return false;
		}

		List<IBodyPrototype> animalBodies = gameworld.BodyPrototypes
			.Where(x => StockAnimalBodyFamilies.Contains(x.Name))
			.ToList();

		if (animalBodies.Count == 0)
		{
			return StockAnimalBodyFamilies.Contains(baseBody.Name);
		}

		return animalBodies.Any(baseBody.CountsAs);
	}
}
