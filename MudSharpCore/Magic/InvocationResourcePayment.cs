#nullable enable

using System.Threading;

namespace MudSharp.Magic;

/// <summary>
/// A new invocation cannot spend provisional excess retained by another spell's capacity batch.
/// Ordinary resource effects keep the batch's deferred arithmetic outside these payment calls.
/// </summary>
internal static class InvocationResourcePayment
{
	private static readonly AsyncLocal<int> PaymentDepth = new();
	internal static bool RequiresCappedAvailability => PaymentDepth.Value > 0;

	internal static bool CanAfford(IHaveMagicResource holder, IMagicResource resource, double amount)
	{
		PaymentDepth.Value++;
		try { return holder.CanUseResource(resource, amount); }
		finally { PaymentDepth.Value--; }
	}

	internal static bool TryPay(IHaveMagicResource holder, IMagicResource resource, double amount)
	{
		PaymentDepth.Value++;
		try { return holder.UseResource(resource, amount); }
		finally { PaymentDepth.Value--; }
	}
}
