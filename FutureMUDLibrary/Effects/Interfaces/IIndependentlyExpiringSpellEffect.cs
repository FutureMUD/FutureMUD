#nullable enable
using System;

namespace MudSharp.Effects.Interfaces;

/// <summary>
/// A spell child that schedules its own absolute expiry. The parent retains it for
/// saving and explicit removal, while ordinary siblings expire with the parent.
/// A null deadline preserves the ordinary parent-controlled lifetime.
/// </summary>
public interface IIndependentlyExpiringSpellEffect : IMagicSpellEffect
{
	DateTime? ExpiryUtc { get; }
}
