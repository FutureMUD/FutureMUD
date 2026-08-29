namespace MudSharp.Models;

public class CharactersSignedLanguage
{
	public long CharacterId { get; set; }
	public long SignedLanguageId { get; set; }

	public virtual Character Character { get; set; }
	public virtual SignedLanguage SignedLanguage { get; set; }
}
