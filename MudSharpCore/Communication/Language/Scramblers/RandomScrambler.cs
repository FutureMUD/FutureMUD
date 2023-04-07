using MudSharp.Framework;

namespace MudSharp.Communication.Language.Scramblers;

public class RandomScrambler : ILanguageScrambler
{
	private RandomScrambler()
	{
	}

	public static RandomScrambler Instance { get; } = new();

	public string Name => "Random";

	public string Description => "All letters will be scrambled at random. Spaces are left intact.";

	public bool Instanced => false;

	public string Scramble(string input, double obscuredratio)
	{
		var array = input.ToCharArray();

		for (var i = 0; i < input.Length; i++)
		{
			if (array[i] == ' ')
			{
				continue;
			}

			array[i] = RandomUtilities.GetRandomCharacter(char.IsUpper(array[i]));
		}

		return array.ToString();
	}

	public string Scramble(ExplodedString input, double obscuredratio)
	{
		return Scramble(input.ToString(), obscuredratio);
	}

	public string Scramble(ExplodedString input, double shortbiasedobscuredratio, double longbiasedobscuredratio)
	{
		return Scramble(input, shortbiasedobscuredratio + longbiasedobscuredratio);
	}
}