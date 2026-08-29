using System.Collections.Generic;

namespace MudSharp.Models;

public class SignedLanguageVariety
{
	public SignedLanguageVariety()
	{
		CharactersSignedLanguageVarieties = new HashSet<CharacterSignedLanguageVariety>();
		CharactersCurrentSignedLanguageVariety = new HashSet<Character>();
	}

	public long Id { get; set; }
	public long SignedLanguageId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Suffix { get; set; }
	public string VagueSuffix { get; set; }
	public int RecognitionDifficulty { get; set; }

	public virtual SignedLanguage SignedLanguage { get; set; }
	public virtual ICollection<CharacterSignedLanguageVariety> CharactersSignedLanguageVarieties { get; set; }
	public virtual ICollection<Character> CharactersCurrentSignedLanguageVariety { get; set; }
}
