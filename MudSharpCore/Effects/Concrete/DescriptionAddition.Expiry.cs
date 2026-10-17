#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Construction;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public partial class DescriptionAddition
{
	public DateTime? ExpiresUtc { get; private set; }
	public string ExpirationMessage { get; private set; } = string.Empty;
	private bool _expirationCompleted;

	/// <summary>Opt-in wall-clock expiry. Existing unscheduled addenda retain their original behaviour.</summary>
	public void SetExpiration(DateTime expiresUtc, string message)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(message);
		ExpiresUtc = expiresUtc.Kind == DateTimeKind.Local
			? expiresUtc.ToUniversalTime()
			: DateTime.SpecifyKind(expiresUtc, DateTimeKind.Utc);
		ExpirationMessage = message;
		Changed = true;
		if (Owner.Effects.Contains(this)) ScheduleExpiration();
	}

	private void LoadExpiration(XElement definition)
	{
		if (definition.Element("ExpiresUtc") is not { } expiry) return;
		if (!DateTime.TryParse(expiry.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date) ||
			date.Kind != DateTimeKind.Utc || string.IsNullOrWhiteSpace((string?)definition.Element("ExpirationMessage")))
		{
			LoadErrors = true;
			return;
		}
		ExpiresUtc = date;
		ExpirationMessage = definition.Element("ExpirationMessage")!.Value;
		// Overdue cold storage is retired without replaying a broadcast to newly arriving observers.
		LoadErrors = date <= RuntimeClock.UtcNow;
	}

	private XElement WithExpiration(XElement definition)
	{
		if (ExpiresUtc is { } expires)
			definition.Add(new XElement("ExpiresUtc", expires.ToString("O", CultureInfo.InvariantCulture)),
				new XElement("ExpirationMessage", new XCData(ExpirationMessage)));
		return definition;
	}

	private DescriptionAddition CopyExpirationTo(DescriptionAddition copy)
	{
		if (ExpiresUtc is { } expires) copy.SetExpiration(expires, ExpirationMessage);
		return copy;
	}

	public override void InitialEffect() => ScheduleExpiration();

	// Native cached duration restoration runs before Login. The persisted UTC deadline remains authoritative.
	public override void Login() => ScheduleExpiration();

	private void ScheduleExpiration()
	{
		if (ExpiresUtc is not { } expires || _expirationCompleted) return;
		Gameworld.EffectScheduler.Reschedule(this, expires > RuntimeClock.UtcNow
			? expires - RuntimeClock.UtcNow : TimeSpan.Zero);
	}

	public override void ExpireEffect()
	{
		if (ExpiresUtc is not { } expires)
		{
			base.ExpireEffect();
			return;
		}
		if (_expirationCompleted || !Owner.Effects.Contains(this)) return;
		if (expires > RuntimeClock.UtcNow)
		{
			ScheduleExpiration();
			return;
		}
		_expirationCompleted = true;
		Owner.RemoveEffect(this);
		if (Owner is ILocation location) location.HandleWholeLocation(ExpirationMessage);
		else if (Owner.Location is { } containingLocation)
		{
			if (containingLocation is IRoom { RouteDefinition: not null } && Owner is IPerceiver source)
				containingLocation.HandleLocal(source, source.RoomLayer, ExpirationMessage);
			else containingLocation.Handle(ExpirationMessage);
		}
		else Owner.OutputHandler?.Send(ExpirationMessage);
	}
}
