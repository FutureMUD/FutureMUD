using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Device-only admission to exact native debits, after live policy evaluation.
/// Ordinary capacity queries never use a batch admission. Each debit must enter explicitly.</summary>
public sealed class MagicResourceCapacityAdmission
{
	public sealed record Payment(IHaveMagicResource Holder, IMagicResource Resource, double Amount, double Capacity);
	private static readonly AsyncLocal<Batch?> CurrentBatch = new();
	private static readonly AsyncLocal<Debit?> CurrentDebit = new();
	private readonly Payment[] _payments;
	private bool _opened;
	public MagicResourceCapacityAdmission(IEnumerable<Payment> payments)
	{
		if (CurrentBatch.Value is not null || CurrentDebit.Value is not null) throw new InvalidOperationException("Nested device payment admission.");
		_payments = payments.ToArray();
		if (_payments.Any(x => !double.IsFinite(x.Amount) || x.Amount < 0 || !double.IsFinite(x.Capacity) || x.Capacity < 0) ||
			_payments.GroupBy(x => (x.Holder, x.Resource)).Any(x => x.Count() != 1))
			throw new InvalidOperationException("Invalid or duplicate device payment admission.");
	}
	public IDisposable OpenScope()
	{
		if (_opened || CurrentBatch.Value is not null || CurrentDebit.Value is not null)
			throw new InvalidOperationException("Device payment admission cannot be reused or nested.");
		_opened = true;
		return new Batch(_payments);
	}
	public static IDisposable? BeginDebit(IHaveMagicResource holder, IMagicResource resource, double amount)
	{
		if (CurrentBatch.Value is not { } batch) return null;
		batch.Check();
		if (CurrentDebit.Value is not null) throw new InvalidOperationException("Reentrant admitted debit.");
		var index = Array.FindIndex(batch.Payments, x => ReferenceEquals(x.Holder, holder) && ReferenceEquals(x.Resource, resource) && x.Amount == amount);
		if (index < 0 || batch.Used[index]) throw new InvalidOperationException("Unadmitted or repeated device debit.");
		batch.Used[index] = true;
		return new Debit(batch, batch.Payments[index]);
	}
	internal static bool TryTakeCapacity(IMagicResource resource, IHaveMagicResource holder, out double capacity)
	{
		capacity = double.NaN;
		if (CurrentDebit.Value is not { } debit) return false;
		debit.Batch.Check();
		if (debit.Consumed || !ReferenceEquals(debit.Payment.Resource, resource) || !ReferenceEquals(debit.Payment.Holder, holder))
			throw new InvalidOperationException("Unadmitted or repeated capacity read inside a device debit.");
		debit.Consumed = true; capacity = debit.Payment.Capacity; return true;
	}
	private sealed class Batch : IDisposable
	{
		public readonly Payment[] Payments;
		public readonly bool[] Used;
		private readonly int _thread = System.Environment.CurrentManagedThreadId;
		private bool _disposed;
		public Batch(Payment[] payments) { Payments = payments; Used = new bool[payments.Length]; CurrentBatch.Value = this; }
		public void Check()
		{
			if (_disposed || _thread != System.Environment.CurrentManagedThreadId || !ReferenceEquals(CurrentBatch.Value, this))
				throw new InvalidOperationException("Device payment admission escaped its synchronous invocation.");
		}
		public void Dispose() { _disposed = true; CurrentDebit.Value = null; if (ReferenceEquals(CurrentBatch.Value, this)) CurrentBatch.Value = null; }
	}
	private sealed class Debit : IDisposable
	{
		public readonly Batch Batch;
		public readonly Payment Payment;
		public bool Consumed;
		public Debit(Batch batch, Payment payment) { Batch = batch; Payment = payment; CurrentDebit.Value = this; }
		public void Dispose() { if (ReferenceEquals(CurrentDebit.Value, this)) CurrentDebit.Value = null; }
	}
}
