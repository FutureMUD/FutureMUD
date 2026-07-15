#nullable enable

using FutureMUD.Web.Configuration;
using Microsoft.Extensions.Options;
using MudSharp.Documentation;
using System.Text.Json;

namespace FutureMUD.Web.Services;

public sealed class DocumentationService
{
	private readonly string _cataloguePath;
	private readonly object _lock = new();
	private DocumentationCatalogue? _catalogue;
	private DateTime _lastWriteUtc;

	public DocumentationService(IOptions<FutureMudWebOptions> options)
	{
		_cataloguePath = Path.Combine(options.Value.DataRoot, "documentation", "live", "catalogue.json");
	}

	public DocumentationCatalogue GetCatalogue()
	{
		lock (_lock)
		{
			if (!File.Exists(_cataloguePath))
			{
				return new DocumentationCatalogue();
			}

			var writeTime = File.GetLastWriteTimeUtc(_cataloguePath);
			if (_catalogue is not null && writeTime == _lastWriteUtc)
			{
				return _catalogue;
			}

			using var stream = File.OpenRead(_cataloguePath);
			var catalogue = JsonSerializer.Deserialize<DocumentationCatalogue>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))
				?? throw new InvalidDataException("The documentation catalogue is empty.");
			if (catalogue.SchemaVersion != DocumentationCatalogue.CurrentSchemaVersion)
			{
				throw new InvalidDataException($"Unsupported documentation schema {catalogue.SchemaVersion}.");
			}
			_catalogue = catalogue;
			_lastWriteUtc = writeTime;
			return catalogue;
		}
	}
}
