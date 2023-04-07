using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class Watch : Effect, IRemoteObservationEffect, IScoreAddendumEffect
{
	public ICharacter CharacterOwner { get; set; }
	public string ExitDescription { get; set; }
	public ICell CellOwner { get; set; }
	public IDoor Door { get; set; }

	public Watch(ICell owner, ICharacter characterOwner, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		CellOwner = owner;
		ExitDescription = characterOwner.Location.GetExitTo(owner, characterOwner).OutboundDirectionDescription;
		CharacterOwner = characterOwner;
		Door = characterOwner.Location.GetExitTo(owner, characterOwner)?.Exit?.Door;
		RegisterEvents();
	}

	public override void Login()
	{
		RegisterEvents();
	}

	protected void RegisterEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_NoLongerValid;
		CharacterOwner.OnQuit += CharacterOwner_NoLongerValid;
		CharacterOwner.OnDeath -= CharacterOwner_NoLongerValid;
		CharacterOwner.OnDeath += CharacterOwner_NoLongerValid;
		CharacterOwner.OnMoved -= CharacterOwner_OnMoved;
		CharacterOwner.OnMoved += CharacterOwner_OnMoved;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnEngagedInMelee -= CharacterOwner_NoLongerValid;
		CharacterOwner.OnEngagedInMelee += CharacterOwner_NoLongerValid;
		if (Door != null)
		{
			Door.OnRemovedFromExit -= Door_OnRemovedFromExit;
			Door.OnRemovedFromExit += Door_OnRemovedFromExit;
		}
	}

	protected void ReleaseEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_NoLongerValid;
		CharacterOwner.OnDeath -= CharacterOwner_NoLongerValid;
		CharacterOwner.OnMoved -= CharacterOwner_OnMoved;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnEngagedInMelee -= CharacterOwner_NoLongerValid;
		if (Door != null)
		{
			Door.OnRemovedFromExit -= Door_OnRemovedFromExit;
		}
	}

	private void Door_OnRemovedFromExit(IDoor door)
	{
		Door.OnRemovedFromExit -= Door_OnRemovedFromExit;
		Door = null;
	}

	private void CharacterOwner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterState.Conscious.HasFlag(CharacterOwner.State))
		{
			Owner.EffectsOfType<WatchMaster>().FirstOrDefault()?.RemoveSpiedCell(CellOwner);
			Owner.RemoveEffect(this, true);
		}
	}

	private void CharacterOwner_OnMoved(object sender, Movement.MoveEventArgs e)
	{
		Owner.EffectsOfType<WatchMaster>().FirstOrDefault()?.RemoveSpiedCell(CellOwner);
		Owner.RemoveEffect(this, true);
	}

	protected override string SpecificEffectType => "Watch";

	public override string Describe(IPerceiver voyeur)
	{
		return "Watching a location from afar.";
	}

	private void CharacterOwner_NoLongerValid(IPerceivable owner)
	{
		Owner.EffectsOfType<WatchMaster>().FirstOrDefault()?.RemoveSpiedCell(CellOwner);
		Owner.RemoveEffect(this, true);
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	public void HandleOutput(IOutput output, ILocation location)
	{
		if (CharacterOwner.EffectsOfType<WatchMaster>().All(x => x.WatchEffects.Contains(this)))
		{
			Owner.RemoveEffect(this);
			return;
		}

		if (Door?.IsOpen == false && !Door.CanSeeThrough(CharacterOwner.Body))
		{
			return;
		}

		if (output is EmoteOutput eo)
		{
			CharacterOwner.SeeTarget(eo.DefaultSource as IMortalPerceiver);
			if (output.Flags.HasFlag(OutputFlags.NoticeCheckRequired))
			{
				var newEO = new EmoteOutput(eo);
				newEO.NoticeCheckDifficulty = newEO.NoticeCheckDifficulty.StageUp(3);
				output = newEO;
			}
		}

		if (!output.ShouldSee(CharacterOwner))
		{
			return;
		}

		var check = Gameworld.GetCheck(CheckType.WatchLocation);
		if (!output.Flags.HasFlag(OutputFlags.PurelyAudible))
		{
			var difficulty = CharacterOwner.Location.Terrain(CharacterOwner).SpotDifficulty
			                               .Highest(CellOwner.Terrain(CharacterOwner).SpotDifficulty)
			                               .StageUp(CharacterOwner.EffectsOfType<WatchMaster>().First().WatchEffects
			                                                      .Count);
			if (check.Check(CharacterOwner, difficulty).IsFail())
			{
				return;
			}
		}

		CharacterOwner.OutputHandler.Send(new PrependOutputWrapper(output,
			$"[{location.HowSeen(CharacterOwner)} ({$"to {ExitDescription}".Colour(Telnet.Green)})]\r\n"));
	}

	public bool ShowInScore => true;
	public bool ShowInHealth => false;
	public string ScoreAddendum => $"You are watching {CellOwner.HowSeen(CharacterOwner)}.";
}