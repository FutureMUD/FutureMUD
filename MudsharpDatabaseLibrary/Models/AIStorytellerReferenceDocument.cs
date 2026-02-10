namespace MudSharp.Models;

public class AIStorytellerReferenceDocument
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string FolderName { get; set; }
	public string DocumentType { get; set; }
	public string Keywords { get; set; }
	public string DocumentContents { get; set; }
	public string RestrictedStorytellerIds { get; set; }
}
