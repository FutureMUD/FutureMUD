using Microsoft.JSInterop;
using System.Text.Json;

namespace TerrainPlanner.Client.Services;

public sealed class BrowserStorage
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
	{
		WriteIndented = true
	};
	private readonly IJSRuntime _jsRuntime;

	public BrowserStorage(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
		return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
	}

	public ValueTask SetAsync<T>(string key, T value) =>
		_jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, JsonOptions));

	public ValueTask RemoveAsync(string key) => _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

	public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);

	public static T Deserialize<T>(string json) =>
		JsonSerializer.Deserialize<T>(json, JsonOptions) ?? throw new InvalidDataException("The JSON document was empty.");
}
