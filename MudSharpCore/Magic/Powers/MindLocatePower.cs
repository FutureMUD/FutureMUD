#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Construction;

namespace MudSharp.Magic.Powers;

/// <summary>Resisted remote observation of a target's current room, adapted from source Locate.</summary>
public sealed class MindLocatePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Mind Locate";
	public override string DatabaseType => "psioniclocate";
	protected override string DefaultVerb => "locate";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psioniclocate", (power, world) => new MindLocatePower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psioniclocate", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new MindLocatePower(world, school, name, trait)));
	}

	private MindLocatePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait)
		: base(world, school, name, trait)
	{
		Blurb = "Resist an attempt to observe your current location";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} LOCATE <target> to attempt a remote view of the room where that person is now.";
		PowerDistance = MagicPowerDistance.SamePlaneOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		FailEcho = PsionicPowerEmotes.Get("locate", "FailEcho", "You cannot determine that mind's location.");
		SuccessEcho = PsionicPowerEmotes.Get("locate", "SuccessEcho", "You focus on the distant mind and its surroundings resolve before you.");
		DoDatabaseInsert();
	}

	private MindLocatePower(MagicPower model, IFuturemud world) : base(model, world) { }
	protected override XElement SaveDefinition() => SaveTargetedDefinition();

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose location do you want to find?", out var target) || target is null) return;
		if (!PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} refuses involuntary mental traffic.");
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.MindLookPower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			ConsumePowerCosts(actor, Verb);
			return;
		}

		SendSuccess(actor, target);
		if (target.Location is null)
		{
			actor.OutputHandler.Send("The distant view slips away before you can make out a place.");
		}
		else
		{
			actor.OutputHandler.Send(RemoteLookRenderer.DescribeRemoteRoom(actor, target.Location, target.RoomLayer), true, true);
		}
		PsionicActivityNotifier.Notify(actor, this, "a remote room observation", target);
		ConsumePowerCosts(actor, Verb);
	}
}
