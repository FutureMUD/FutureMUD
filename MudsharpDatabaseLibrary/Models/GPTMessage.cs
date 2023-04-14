using JetBrains.Annotations;

namespace MudSharp.Models;

public class GPTMessage
{
	public long Id { get; set; }
	public long GPTThreadId { get; set; }
	public long? CharacterId { get; set; }
	public string Message { get; set; }
	[CanBeNull] public string Response { get; set; }
	public virtual GPTThread GPTThread { get; set; }
	public virtual Character Character { get; set; }
}