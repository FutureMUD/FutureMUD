#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FutureMUD.Web.Pages.PatchNotes;

public sealed class DetailModel : PageModel
{
	private readonly MarkdownContentService _content;
	public DetailModel(MarkdownContentService content) => _content = content;
	public ContentDocument? Document { get; private set; }

	public IActionResult OnGet(string slug)
	{
		Document = _content.GetPatchNote(slug);
		return Document is null ? NotFound() : Page();
	}
}
