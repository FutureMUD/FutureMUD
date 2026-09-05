#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.RPG.Law;

public enum CrimeWitnessSourceKind { Character, Virtual }

/// <summary>Recall and delivery are independent: losing recall never retracts delivered evidence.</summary>
public sealed class CrimeWitnessMemory
{
	public CrimeWitnessSourceKind Kind { get; init; }
	public long SourceId { get; init; }
	public long? LocationId { get; init; }
	public DateTime? SuppressedUntilUtc { get; private set; }
	public bool PermanentlyForgotten { get; private set; }
	public DateTime? ReportDueUtc { get; set; }
	public bool ReportDelivered { get; set; }
	public bool IdentityKnown { get; set; }
	public double Reliability { get; set; }
	public List<string> Audit { get; } = [];
	public bool CanRecall(DateTime now) => !PermanentlyForgotten && (SuppressedUntilUtc is null || SuppressedUntilUtc <= now);

	public void Forget(DateTime now, TimeSpan duration, bool permanent, long actorId)
	{
		if (!permanent && duration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(duration));
		PermanentlyForgotten |= permanent;
		if (!permanent)
		{
			var until = now.Add(duration);
			if (SuppressedUntilUtc is null || SuppressedUntilUtc < until) SuppressedUntilUtc = until;
		}
		Audit.Add($"{now:O} forgotten by {actorId}; permanent={permanent}; until={SuppressedUntilUtc:O}");
	}

	public void Restore(DateTime now, long actorId)
	{
		PermanentlyForgotten = false;
		SuppressedUntilUtc = null;
		Audit.Add($"{now:O} recall restored by {actorId}");
	}

	public XElement Save() => new("Witness", new XAttribute("kind", Kind), new XAttribute("source", SourceId),
		LocationId is { } location ? new XAttribute("location", location) : null,
		SuppressedUntilUtc is { } until ? new XAttribute("until", until) : null,
		new XAttribute("permanent", PermanentlyForgotten), new XAttribute("delivered", ReportDelivered),
		ReportDueUtc is { } due ? new XAttribute("due", due) : null,
		new XAttribute("identity", IdentityKnown), new XAttribute("reliability", Reliability),
		Audit.ConvertAll(x => new XElement("Audit", x)));

	public static CrimeWitnessMemory Load(XElement xml)
	{
		var memory = new CrimeWitnessMemory
		{
			Kind = Enum.Parse<CrimeWitnessSourceKind>((string)xml.Attribute("kind")!),
			SourceId = (long)xml.Attribute("source")!, LocationId = (long?)xml.Attribute("location"),
			SuppressedUntilUtc = (DateTime?)xml.Attribute("until"),
			PermanentlyForgotten = (bool?)xml.Attribute("permanent") ?? false,
			ReportDelivered = (bool?)xml.Attribute("delivered") ?? false,
			ReportDueUtc = (DateTime?)xml.Attribute("due"), IdentityKnown = (bool?)xml.Attribute("identity") ?? false,
			Reliability = (double?)xml.Attribute("reliability") ?? 0
		};
		foreach (var entry in xml.Elements("Audit")) memory.Audit.Add(entry.Value);
		return memory;
	}
}
