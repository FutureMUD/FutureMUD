#nullable enable

using System;

namespace MudSharp.Economy.Currency;

/// <summary>Prepares and publishes a second native effect as part of a currency transfer.</summary>
public interface ICurrencyTransferParticipant : IDisposable
{
	/// <summary>Performs fallible preparation before the transfer's final validation.</summary>
	bool Prepare();

	/// <summary>Whether the prepared effect still matches the state captured during preparation.</summary>
	bool IsCurrent { get; }

	/// <summary>Publishes the prepared effect using callback-free writes only.</summary>
	void Commit();

	/// <summary>Notifies observers after both the currency and participant effects are committed.</summary>
	void Notify();
}
