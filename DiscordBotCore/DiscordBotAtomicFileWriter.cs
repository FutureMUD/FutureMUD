#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Bot;

internal sealed class DiscordBotAtomicFileWriter
{
	private readonly SemaphoreSlim _writeGate = new(1, 1);

	public async Task WriteAsync(string path, string contents)
	{
		await _writeGate.WaitAsync();
		string? temporaryPath = null;
		try
		{
			var fullPath = Path.GetFullPath(path);
			var directory = Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException("The output path has no parent directory.");
			temporaryPath = Path.Combine(directory, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
			await File.WriteAllTextAsync(temporaryPath, contents, new UTF8Encoding(false));
			File.Move(temporaryPath, fullPath, overwrite: true);
			temporaryPath = null;
		}
		finally
		{
			try
			{
				if (temporaryPath is not null && File.Exists(temporaryPath))
				{
					File.Delete(temporaryPath);
				}
			}
			finally
			{
				_writeGate.Release();
			}
		}
	}
}
