using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FutureMUD_Upgrade_Service.Controllers;

public record FileMetadata(string Version, string Checksum);

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
	private readonly string _storagePath = "downloads"; // Replace with the path where files will be stored on the server
	private readonly string _metadataFilePath = "metadata"; // Replace with the path to the metadata JSON file

	[HttpGet("testauth")]
	[Authorize]
	public IActionResult TestAuth()
	{
		return Ok("Authentication successful.");
	}

	// GET: api/File/metadata/{fileType}
	[HttpGet("metadata/{fileType}")]
	public IActionResult GetLatestFileVersion(string fileType)
	{
		var metadata = ReadMetadata();
		if (!metadata.ContainsKey(fileType))
		{
			return NotFound();
		}

		return Ok(metadata[fileType]);
	}

	// GET: api/File/download/{fileType}
	[HttpGet("download/{fileType}")]
	public IActionResult DownloadFile(string fileType)
	{
		var metadata = ReadMetadata();
		if (!metadata.ContainsKey(fileType))
		{
			return NotFound();
		}

		string fileName = $"{fileType}_{metadata[fileType].Version}.zip";
		string filePath = Path.Combine(_storagePath, fileName);

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound();
		}

		byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
		return File(fileBytes, "application/octet-stream", fileName);
	}

	// POST: api/File/upload/{fileType}/{version}
	[HttpPost("upload/{fileType}/{version}")]
	[Authorize]
	public IActionResult UploadFile(string fileType, string version, IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("File is not present or empty.");
		}

		string fileName = $"{fileType}_{version}.zip";
		string filePath = Path.Combine(_storagePath, fileName);
		using (var fileStream = new FileStream(filePath, FileMode.Create))
		{
			file.CopyTo(fileStream);
		}

		// Replace this with the logic to calculate the checksum
		string checksum = "your_checksum_here";

		// Update the metadata
		var metadata = ReadMetadata();
		metadata[fileType] = new FileMetadata(version, checksum);
		WriteMetadata(metadata);

		return Ok(new { Message = "File uploaded successfully." });
	}

	private Dictionary<string, FileMetadata> ReadMetadata()
	{
		if (!System.IO.File.Exists(_metadataFilePath))
		{
			var initialMetadata = new Dictionary<string, FileMetadata>
			{
				{ "Engine_Windows", new FileMetadata("0.0.0", "") },
				{ "Engine_Linux", new FileMetadata("0.0.0", "") },
				{ "DiscordBot_Windows", new FileMetadata("0.0.0", "") },
				{ "DiscordBot_Linux", new FileMetadata("0.0.0", "") }
			};
			WriteMetadata(initialMetadata);
		}

		string json = System.IO.File.ReadAllText(_metadataFilePath);
		return JsonSerializer.Deserialize<Dictionary<string, FileMetadata>>(json);
	}

	private void WriteMetadata(Dictionary<string, FileMetadata> metadata)
	{
		string json = JsonSerializer.Serialize(metadata);
		System.IO.File.WriteAllText(_metadataFilePath, json);
	}
}