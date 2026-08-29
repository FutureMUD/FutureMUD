namespace MudSharp.Models;

public class CharacterSignedLanguageVariety
{
	public long CharacterId { get; set; }
	public long SignedLanguageVarietyId { get; set; }
	public int Familiarity { get; set; }

	public virtual Character Character { get; set; }
	public virtual SignedLanguageVariety SignedLanguageVariety { get; set; }
}
