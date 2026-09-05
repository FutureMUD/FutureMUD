#nullable enable

using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.Planes;

namespace MudSharp.Magic.Powers;

public sealed class TelekinesisPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "telekinesis";
	public static void RegisterLoader() => Register("telekinesis", (m,w) => new TelekinesisPower(m,w), (w,s,n,t) => new TelekinesisPower(w,s,n,t));
	private TelekinesisPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private TelekinesisPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) => Initialise(TelekineticManipulation.Syntax);
	public override string ShowHelp(ICharacter voyeur) => base.ShowHelp(voyeur) + $"\nManipulation syntax: {School.SchoolVerb} {Verb} {TelekineticManipulation.Syntax}\nThe builder amount setting is the maximum mass in kilograms. Liquid amounts use the world's volume units. Sources and destinations must be visible, unattended and within the configured mass limit. Closed, locked, anchored and inaccessible objects retain their ordinary restrictions.";
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!HandleGeneralUseRestrictions(actor) || !CanAffordToInvokePower(actor, Verb).Truth) return;
		var item = actor.TargetLocalItem(command.PopSpeech());
		var operation = command.PopForSwitch();
		var affected = new HashSet<IGameItem>();
		bool Eligible(IGameItem value)
		{
			if (value.InInventoryOf is not null || value.ContainedIn is not null || value.RoomLayer != actor.RoomLayer ||
			    !TelekineticManipulation.IsWithinMassLimit(value.Weight, actor.Gameworld.UnitManager.BaseWeightToKilograms, Amount) || !actor.CanSee(value) ||
			    !actor.CanInteractPlanar(value, PlanarInteractionKind.Physical) ||
			    !MagicPowerSpatialTargeting.AcquireTargets(actor, MagicPowerDistance.SameLocationOnly).Contains(value) ||
			    actor.Location.CanGetAccess(value, actor) == false ||
			    MagicInterdictionHelper.GetInterdiction(actor, value, School, false) is not null || CanInvokePowerProg.ExecuteBool(actor, value) == false) return false;
			affected.Add(value);
			return true;
		}
		if (item is null) { actor.Send("You cannot reach a visible unattended object by that description."); return; }
		if (!TelekineticManipulation.TryPrepare(actor, item, operation, command, Eligible, out var execute, out var error))
		{ actor.Send(error); return; }
		if (!Resolve(actor, actor, MentalActionKind.Influence, false, out _)) return;
		if (execute())
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("An unseen force manipulates $1.", actor, actor, item)));
			foreach (var target in affected) PsychometricRecorder.Record(actor, ImpressionKind.Magic, $"telekinetic manipulation ({operation})", target, School.Id, directItemOnly: true);
		}
		else actor.Send("The mechanism does not respond to your manipulation.");
		Complete(actor, null, "telekinesis");
	}
}
