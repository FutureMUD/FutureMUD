#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Publishing;

public static partial class PublishingEndpoints
{
	public static IEndpointRouteBuilder MapPublishingApi(this IEndpointRouteBuilder endpoints)
	{
		var group = endpoints.MapGroup("/api/publishing/v1")
			.RequireRateLimiting("publishing");

		group.MapPost("/releases", async (CreateReleaseRequest request, ReleaseStore store, CancellationToken token) =>
			await ExecuteAsync(() => store.CreateOrResumeAsync(request, token)))
			.WithMetadata(new RequestSizeLimitAttribute(64 * 1024));

		group.MapGet("/releases/{uploadId}", async (string uploadId, ReleaseStore store, CancellationToken token) =>
			await ExecuteAsync(async () => await store.GetAsync(uploadId, token)
				?? throw new ReleaseStoreException("Release draft not found.", StatusCodes.Status404NotFound)));

		group.MapPut("/releases/{uploadId}/artifacts/{artifactId}/chunks/{index:int}", async (
			HttpRequest request,
			string uploadId,
			string artifactId,
			int index,
			ReleaseStore store,
			CancellationToken token) =>
		{
			var range = ParseContentRange(request.Headers.ContentRange.ToString());
			var digest = request.Headers["Digest"].ToString();
			if (range is null || string.IsNullOrWhiteSpace(digest))
			{
				return Results.Problem("Content-Range and Digest headers are required.", statusCode: StatusCodes.Status400BadRequest);
			}
			return await ExecuteAsync(() => store.PutChunkAsync(
				uploadId,
				artifactId,
				index,
				range.Value.Start,
				range.Value.End,
				range.Value.Total,
				digest,
				request.Body,
				token));
		}).DisableAntiforgery();

		group.MapPost("/releases/{uploadId}/complete", async (string uploadId, ReleaseStore store, CancellationToken token) =>
			await ExecuteAsync(() => store.CompleteAsync(uploadId, token)));

		group.MapPost("/releases/{uploadId}/promote", async (string uploadId, ReleaseStore store, CancellationToken token) =>
			await ExecuteAsync(() => store.PromoteAsync(uploadId, token)));

		group.MapDelete("/releases/{uploadId}", async (string uploadId, ReleaseStore store, CancellationToken token) =>
		{
			try
			{
				await store.AbandonAsync(uploadId, token);
				return Results.NoContent();
			}
			catch (ReleaseStoreException exception)
			{
				return Results.Problem(exception.Message, statusCode: exception.StatusCode);
			}
		});

		return endpoints;
	}

	public static IEndpointRouteBuilder MapDownloadEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/downloads/{product}/{version}/{fileName}", async Task<IResult> (
			HttpContext context,
			string product,
			string version,
			string fileName,
			ReleaseStore store,
			CancellationToken token) =>
		{
			if (fileName.Contains("..", StringComparison.Ordinal) ||
				fileName.Contains('/') ||
				fileName.Contains('\\'))
			{
				return Results.NotFound();
			}
			var release = await store.GetLiveReleaseAsync(product, token);
			if (release is null ||
				!release.Product.Equals(product, StringComparison.Ordinal) ||
				!release.Version.Equals(version, StringComparison.Ordinal))
			{
				return Results.NotFound();
			}
			var artifact = release.Artifacts.FirstOrDefault(item =>
				item.FileName.Equals(fileName, StringComparison.Ordinal));
			if (artifact is null)
			{
				return Results.NotFound();
			}
			var path = store.GetLiveArtifactPath(release.Product, artifact.FileName);
			if (!File.Exists(path))
			{
				return Results.NotFound();
			}
			context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
			context.Response.Headers["X-Checksum-SHA256"] = artifact.Sha256;
			return Results.File(
				path,
				"application/zip",
				fileDownloadName: artifact.FileName,
				lastModified: release.PublishedAtUtc,
				entityTag: new EntityTagHeaderValue($"\"{artifact.Sha256}\""),
				enableRangeProcessing: true);
		});

		endpoints.MapGet("/downloads/{product}/{version}/{fileName}.sha256", async Task<IResult> (
			string product,
			string version,
			string fileName,
			ReleaseStore store,
			CancellationToken token) =>
		{
			var release = await store.GetLiveReleaseAsync(product, token);
			var artifact = release?.Artifacts.FirstOrDefault(item => item.FileName == fileName && release.Version == version);
			return artifact is null
				? Results.NotFound()
				: Results.Text($"{artifact.Sha256}  {artifact.FileName}\n", "text/plain", Encoding.UTF8);
		});

		endpoints.MapGet("/downloads/{product}/latest/{runtime}", async Task<IResult> (
			HttpContext context,
			string product,
			string runtime,
			ReleaseStore store,
			CancellationToken token) =>
		{
			var release = await store.GetLiveReleaseAsync(product, token);
			var artifact = release?.Artifacts.FirstOrDefault(item => item.Runtime == runtime);
			if (release is null || artifact is null)
			{
				return Results.NotFound();
			}
			context.Response.Headers.CacheControl = "no-cache,no-store";
			return Results.Redirect($"/downloads/{Uri.EscapeDataString(product)}/{Uri.EscapeDataString(release.Version)}/{Uri.EscapeDataString(artifact.FileName)}", false, false);
		});

		return endpoints;
	}

	public static IEndpointRouteBuilder MapLegacyRedirects(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/ProgFunctionsAlphabetically.html", () => Results.Redirect("/docs/futureprog/functions", true));
		endpoints.MapGet("/ProgFunctionsByCategory.html", () => Results.Redirect("/docs/futureprog/functions", true));
		endpoints.MapGet("/ProgCollectionHelps.html", () => Results.Redirect("/docs/futureprog/collections", true));
		endpoints.MapGet("/ProgTypeHelps.html", () => Results.Redirect("/docs/futureprog/types", true));
		endpoints.MapGet("/index.php/about-futuremud", () => Results.Redirect("/about", true));
		endpoints.MapGet("/index.php/about-futuremud/", () => Results.Redirect("/about", true));
		endpoints.MapGet("/index.php/futuremud-public-releases", () => Results.Redirect("/downloads", true));
		endpoints.MapGet("/index.php/futuremud-public-releases/", () => Results.Redirect("/downloads", true));
		return endpoints;
	}

	private static async Task<IResult> ExecuteAsync<T>(Func<Task<T>> action)
	{
		try
		{
			return Results.Ok(await action());
		}
		catch (ReleaseStoreException exception)
		{
			return Results.Problem(exception.Message, statusCode: exception.StatusCode);
		}
		catch (InvalidDataException exception)
		{
			return Results.Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
		}
	}

	internal static (long Start, long End, long Total)? ParseContentRange(string value)
	{
		var match = ContentRangeRegex().Match(value);
		return match.Success &&
			long.TryParse(match.Groups[1].Value, out var start) &&
			long.TryParse(match.Groups[2].Value, out var end) &&
			long.TryParse(match.Groups[3].Value, out var total)
			? (start, end, total)
			: null;
	}

	[GeneratedRegex("\\Abytes (\\d+)-(\\d+)/(\\d+)\\z", RegexOptions.CultureInvariant)]
	private static partial Regex ContentRangeRegex();
}
