#nullable enable
#pragma warning disable OPENAI001
using System.ClientModel.Primitives;
using System.ClientModel;
using System.Threading;
using OpenAI.Responses;

namespace MudSharp.RPG.AIStorytellers;

internal interface IStorytellerRequestAdapter
{
	Task<BinaryData> SendAsync(string apiKey, BinaryData request, CancellationToken cancellationToken);
}

internal sealed class StorytellerRequestAdapter : IStorytellerRequestAdapter
{
	public async Task<BinaryData> SendAsync(string apiKey, BinaryData request, CancellationToken cancellationToken)
	{
		var client = new ResponsesClient(apiKey);
		using var content = BinaryContent.Create(request);
		var response = await client.CreateResponseAsync(content, new RequestOptions { CancellationToken = cancellationToken })
			.ConfigureAwait(false);
		return response.GetRawResponse().Content;
	}
}
