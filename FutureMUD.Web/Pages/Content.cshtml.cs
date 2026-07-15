#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FutureMUD.Web.Pages;

public sealed class ContentModel : PageModel
{
	private readonly MarkdownContentService _content;
	public ContentModel(MarkdownContentService content) => _content = content;
	public ContentDocument Document { get; private set; } = null!;

	public IActionResult OnGet(string slug)
	{
		try
		{
			Document = _content.GetPage(slug);
			return Page();
		}
		catch (FileNotFoundException)
		{
			return NotFound();
		}
	}
}
