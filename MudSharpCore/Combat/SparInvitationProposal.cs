using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat;

public class SparInvitationProposal : Proposal
{
	public SparInvitationProposal(IPerceiver applicant, IPerceiver supplicant, ICombat combat)
	{
		Applicant = applicant;
		Supplicant = supplicant;
		Combat = combat;
		if (Combat != null)
		{
			combat.CombatEnds += CombatOnCombatEnds;
			combat.CombatMerged += CombatOnCombatMerged;
		}
	}

	public IPerceiver Applicant { get; set; }
	public IPerceiver Supplicant { get; set; }
	public ICombat Combat { get; set; }

	public bool CombatExpired { get; set; }

	private void CombatOnCombatMerged(ICombat obsoleteCombat, ICombat newCombat)
	{
		CombatExpired = true;
	}

	private void CombatOnCombatEnds(object sender, EventArgs e)
	{
		CombatExpired = true;
	}

	#region Overrides of Proposal

	/// <summary>
	///     Signals to the proposal that it has been accepted by the Supplicant, and it should take action to resolve the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the ACCEPT command</param>
	public override void Accept(string message = "")
	{
		if (CombatExpired)
		{
			Supplicant.Send("Unfortunately, that invitation to spar is no longer valid.");
			return;
		}

		if (Combat != null && Applicant.Combat != Combat)
		{
			Supplicant.Send("Unfortunately, that invitation to spar is no longer valid.");
			return;
		}

		if (Supplicant.Combat != null)
		{
			Supplicant.Send("You cannot join a spar any longer as you are already engaged in combat.");
			return;
		}

		if ((Applicant as ICharacter)?.State.HasFlag(CharacterState.Dead) ?? false)
		{
			Supplicant.Send("You cannot join that spar any longer as the proponent is dead.");
			return;
		}

		if (Supplicant.Location != Applicant.Location)
		{
			Supplicant.Send(
				"You cannot join that spar any longer as you are not in the same location as the proponent.");
			return;
		}

		if (Combat != null)
		{
			Combat.JoinCombat(Supplicant);
			Supplicant.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ accept|accepts $1's invitation to spar, and joins the ongoing bout.",
					Supplicant, Supplicant, Applicant)));
		}
		else if (Applicant.Combat != null)
		{
			//In this case, the applicant started a new fight since sending the invite, so join that if we can
			if (!Applicant.Combat.Friendly)
			{
				Supplicant.Send(
					$"You cannot spar {Applicant.HowSeen(Supplicant)} as {Applicant.ApparentGender(Supplicant).Subjective()} is in a lethal fight now!");
				return;
			}
			else
			{
				//Join their spar
				Applicant.Combat.JoinCombat(Supplicant);
				Supplicant.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ accept|accepts $1's invitation to spar, and joins the ongoing bout.",
						Supplicant, Supplicant, Applicant)));
			}
		}
		else
		{
			//Start new spar instead
			var combat = new SparCombat { Gameworld = Applicant.Gameworld };
			combat.JoinCombat(Applicant);
			combat.JoinCombat(Supplicant);
			Supplicant.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ accept|accepts $1's invitation to spar, commencing a new bout.",
					Supplicant, Supplicant, Applicant)));
		}

		Applicant.CombatTarget = Supplicant;
		Supplicant.CombatTarget = Applicant;
		Supplicant.Engage(Applicant);
	}

	/// <summary>
	///     Signals to the proposal that it has been rejected by the Supplicant, and it should take action to cancel the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the DECLINE command</param>
	public override void Reject(string message = "")
	{
		Supplicant.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ decline|declines $1's invitation to spar.", Supplicant, Supplicant,
				Applicant)));
	}

	/// <summary>
	///     Signals to the proposal that it has timed out, and it should take action to cancel the proposition.
	/// </summary>
	public override void Expire()
	{
		Supplicant.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ decline|declines $1's invitation to spar.", Supplicant, Supplicant,
				Applicant)));
	}

	/// <summary>
	///     Asks for a one line description of the proposal, as seen by the individual supplied
	/// </summary>
	/// <param name="voyeur">The voyeur for the description</param>
	/// <returns>
	///     A string describing the proposal
	/// </returns>
	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Applicant.HowSeen(voyeur, true)} {(voyeur == Applicant ? "are" : "is")} proposing to spar with {Supplicant.HowSeen(voyeur)}.";
	}

	public override IEnumerable<string> Keywords { get; } = new[] { "spar", "bout" };

	#endregion
}