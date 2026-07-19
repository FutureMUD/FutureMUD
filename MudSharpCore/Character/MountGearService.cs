using MudSharp.GameItems;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Character;

public static class MountGearService
{
	public static MountGearProfile ProfileFor(ICharacter mount, ICharacter? rider = null)
	{
		var gearItems = RelevantGearItems(mount, rider).Distinct().ToList();
		var ridingGear = gearItems
		                 .SelectMany(x => x.GetItemTypes<IRidingGear>())
		                 .ToList();
		var roles = ridingGear.Aggregate(RidingGearRole.None, (current, gear) => current | gear.Roles);
		var controlBonus = ridingGear.Sum(x => x.ControlBonus);
		var stabilityBonus = ridingGear.Sum(x => x.StabilityBonus);

		return new MountGearProfile(gearItems.Where(x => x.IsItemType<IRidingGear>()), roles, controlBonus, stabilityBonus);
	}

	private static IEnumerable<IGameItem> RelevantGearItems(ICharacter mount, ICharacter? rider)
	{
		if (mount.Body is not null)
		{
			foreach (var item in mount.Body.ExternalItems.Concat(mount.Body.ExternalItemsForOtherActors))
			{
				yield return item;
			}
		}

		if (rider?.Body is not null)
		{
			foreach (var item in rider.Body.ExternalItems.Concat(rider.Body.ExternalItemsForOtherActors))
			{
				yield return item;
			}
		}
	}

	public static void WarnIfIncomplete(ICharacter mount, ICharacter rider)
	{
		var profile = ProfileFor(mount, rider);
		if (!profile.Warnings.Any())
		{
			return;
		}

		rider.OutputHandler.Send(
			$"You mount {mount.HowSeen(rider)} with incomplete tack ({profile.Warnings.ListToString()}), so control and staying mounted will be harder.");
	}

	public static bool CanControlMount(ICharacter mount, ICharacter rider, bool allowBuckOnMajorFailure = true)
	{
		if (!mount.IsPrimaryRider(rider))
		{
			rider.OutputHandler.Send("You are not in control of this mount.");
			return false;
		}

		if (!mount.PermitControl(rider))
		{
			mount.HandleControlDenied(rider);
			return false;
		}

		var profile = ProfileFor(mount, rider);
		var difficulty = mount.ControlMountDifficulty(rider);
		var check = rider.Gameworld?.GetCheck(CheckType.FleeMovementMountedCheck);
		if (check is null || difficulty == Difficulty.Automatic && profile.ControlBonus >= 0.0)
		{
			return true;
		}

		var result = check.Check(rider, difficulty, mount, externalBonus: profile.ControlBonus);
		if (!result.Outcome.IsFail())
		{
			return true;
		}

		rider.OutputHandler.Send(
			$"You fail to control {mount.HowSeen(rider)} ({result.FinalDifficulty.DescribeEnum()} after tack modifiers).");
		if (allowBuckOnMajorFailure && result.Outcome.FailureDegrees() >= 2)
		{
			mount.BuckRider(rider);
		}

		return false;
	}

	public static bool BuckRider(ICharacter mount, ICharacter rider)
	{
		var profile = ProfileFor(mount, rider);
		var difficulty = mount.ResistBuckDifficulty(rider);
		var check = rider.Gameworld?.GetCheck(CheckType.FleeMovementMountedCheck);
		if (check is null || difficulty == Difficulty.Automatic && profile.StabilityBonus >= 0.0)
		{
			rider.OutputHandler.Send($"You keep your seat on {mount.HowSeen(rider)}.");
			return false;
		}

		var result = check.Check(rider, difficulty, mount, externalBonus: profile.StabilityBonus);
		if (!result.Outcome.IsFail())
		{
			rider.OutputHandler.Send($"You keep your seat on {mount.HowSeen(rider)}.");
			return false;
		}

		rider.OutputHandler.Send(new EmoteOutput(new Emote("$0 buck|bucks $1 off!", mount, mount, rider)));
		rider.DoFallOffHorse();
		return true;
	}
}
