#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FutureMUD.Web.Pages.PatchNotes;

public sealed class IndexModel : PageModel
{
	private readonly MarkdownContentService _content;
	public IndexModel(MarkdownContentService content) => _content = content;
	public IReadOnlyList<ContentDocument> Items { get; private set; } = [];
	public void OnGet() => Items = _content.GetPatchNotes();
}
