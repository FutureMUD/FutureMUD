using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace MudSharp_API.Controllers;

public class DatabaseBackupController : Controller
{
    [HttpPost("uploaddbbackup")]
    public async Task<IActionResult> UploadBackup([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        string filePath = Path.Combine("Backups", file.FileName);

        using (FileStream stream = new(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok("File uploaded successfully");
    }
}