#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Magic;

public readonly record struct PsionicActivity(
	ICharacter Source,
	IMagicPower Power,
	PsionicActivityKind Kind,
	string Description);

public static class PsionicActivityNotifier
{
	public static void Notify(ICharacter source, IMagicPower power, string description, ICharacter? target = null)
	{
		Notify(source, power, description, target is null ? [] : [target]);
	}

	public static void Notify(ICharacter source, IMagicPower power, string description, IEnumerable<ICharacter> targets)
	{
		var activity = new PsionicActivity(source, power, power.IsPsionic ? PsionicActivityKind.Psychic : PsionicActivityKind.Magical,
			description);

		foreach (var effect in source.Gameworld.Characters.SelectMany(x => x.EffectsOfType<PsionicSensitivityEffect>()).ToList())
		{
			effect.NotifyActivity(activity);
		}

		if (power is not Powers.MagicPowerBase powerBase ||
		    !powerBase.CreatesPsionicTrace ||
		    powerBase.PsionicTraceDuration <= TimeSpan.Zero)
		{
			return;
		}

		var traceId = Guid.NewGuid();
		var targetList = targets.Where(x => x is not null).Distinct().ToList();
		var owners = new List<IPerceivable> { source };
		owners.AddRange(targetList);
		if (source.Location is not null)
		{
			owners.Add(source.Location);
		}

		foreach (var owner in owners.Distinct())
		{
			var traceTarget = owner is ICharacter characterOwner && targetList.Contains(characterOwner)
				? characterOwner
				: targetList.Count == 1
					? targetList[0]
					: null;
			var concealment = traceTarget is null
				? null
				: source.EffectsOfType<IMindContactConcealmentEffect>()
				        .FirstOrDefault(x => x.ConcealsIdentityFrom(source, traceTarget, power.School));
			var unknownDescription = concealment?.UnknownIdentityDescription ?? "an unknown mind";
			var concealmentStages = concealment?.AuditDifficultyStages ?? 0;

			if (owner.EffectsOfType<IPsionicTraceEffect>().Any(x => x.TraceId == traceId))
			{
				continue;
			}

			owner.AddEffect(new PsionicTraceEffect(owner, source, traceTarget, source.Location, powerBase, activity.Kind,
				string.IsNullOrWhiteSpace(powerBase.PsionicTraceDescription)
					? description
					: powerBase.PsionicTraceDescription,
				unknownDescription, powerBase.PsionicTraceReadDifficulty, concealmentStages, traceId,
				DateTime.UtcNow, powerBase.PsionicTraceDuration), powerBase.PsionicTraceDuration);
		}
	}
}
