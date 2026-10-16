#nullable enable

using System.Text;

namespace MudSharp.Character;

public partial class Character
{
	public override void AppendEventSubscriptionReport(StringBuilder sb)
	{
		base.AppendEventSubscriptionReport(sb);
		sb.AppendLine($"Death subscriptions: {OnDeath?.GetInvocationList().Length ?? 0}");
		sb.AppendLine($"Start move subscriptions: {OnStartMove?.GetInvocationList().Length ?? 0}");
		sb.AppendLine($"Stop move subscriptions: {OnStopMove?.GetInvocationList().Length ?? 0}");
	}
}
