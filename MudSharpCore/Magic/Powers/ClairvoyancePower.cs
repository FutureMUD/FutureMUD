#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class ClairvoyancePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Clairvoyance";
	public override string DatabaseType => "clairvoyance";
	protected override string DefaultVerb => "clairvoyance";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("clairvoyance", (power, gameworld) => new ClairvoyancePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("clairvoyance", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new ClairvoyancePower(gameworld, school, name, trait));
	}

	private ClairvoyancePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Briefly view a target's location";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} CLAIRVOYANCE <target> to see that target's present location.";
		PowerDistance = MagicPowerDistance.SeenTargetOnly;
		SkillCheckDifficulty = Difficulty.Normal;
		FailEcho = "You reach out for $1's surroundings, but the image collapses.";
		SuccessEcho = "@ close|closes &0's eyes and peers across distance.";
		DoDatabaseInsert();
	}

	private ClairvoyancePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveTargetedDefinition();
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose location do you want to see?", out var target) || target is null)
		{
			return;
		}

		var outcome = CheckPower(actor, target, CheckType.ClairvoyancePower);
		if (outcome < MinimumSuccessThreshold)
		{
			SendFailure(actor, target);
			return;
		}

		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(SuccessEcho, actor, actor, target)));
		}

		actor.OutputHandler.Send(RemoteLookRenderer.DescribeRemoteCell(actor, target.Location, target.RoomLayer));
		PsionicActivityNotifier.Notify(actor, this, "a remote psychic viewing");
		ConsumePowerCosts(actor, Verb);
	}
}
