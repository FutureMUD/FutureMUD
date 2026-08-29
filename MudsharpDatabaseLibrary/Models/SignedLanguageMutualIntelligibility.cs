namespace MudSharp.Models;

public class SignedLanguageMutualIntelligibility
{
	public long ListenerLanguageId { get; set; }
	public long TargetLanguageId { get; set; }
	public int IntelligibilityDifficulty { get; set; }

	public virtual SignedLanguage ListenerLanguage { get; set; }
	public virtual SignedLanguage TargetLanguage { get; set; }
}
