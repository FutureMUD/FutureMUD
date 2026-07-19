
namespace MudSharp.CharacterCreation;

public class CharacterIntro : ICharacterIntro
{
    public Queue<string> Echoes { get; set; }
    public Queue<TimeSpan> Delays { get; set; }
}