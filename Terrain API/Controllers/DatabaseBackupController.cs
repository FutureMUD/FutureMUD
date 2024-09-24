using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MudSharp_API.Controllers;

public class DatabaseBackupController : Controller
{
	[HttpPost("uploaddbbackup")]
	public async Task<IActionResult> UploadBackup([FromForm] IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("File is empty");

		var filePath = Path.Combine("Backups", file.FileName);

		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		return Ok("File uploaded successfully");
	}
}