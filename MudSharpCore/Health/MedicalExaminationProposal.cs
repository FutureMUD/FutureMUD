using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Health;

public class MedicalExaminationProposal : Proposal
{
	public ICharacter Examiner { get; set; }
	public ICharacter Patient { get; set; }
	public ISurgicalProcedure Procedure { get; set; }

	#region Overrides of Proposal

	/// <summary>
	///     Signals to the proposal that it has been accepted by the Supplicant, and it should take action to resolve the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the ACCEPT command</param>
	public override void Accept(string message = "")
	{
		if (Examiner.Combat != null)
		{
			Examiner.Send("You are too busy fighting to worry about that!");
			return;
		}

		if (Examiner.Movement != null)
		{
			Examiner.Send("You cannot perform an examination of a patient while you are moving.");
			return;
		}

		if (Patient == null)
		{
			Examiner.Send("You do not see anyone like that upon whom you can perform an examination.");
			return;
		}

		if (Patient.State == CharacterState.Dead)
		{
			Examiner.Send("You cannot perform an examination of your patient on account of them being dead.");
			return;
		}

		if (Patient.Movement != null)
		{
			Examiner.Send("There is no way to perform an examination of a patient while they are moving.");
			return;
		}

		if (Patient.Combat != null)
		{
			Examiner.Send("You cannot perform an examination of a patient while they are still in combat.");
			return;
		}

		var result = Examiner.Gameworld.GetCheck(Procedure.Check)
		                     .Check(Examiner, Difficulty.Normal, Patient, externalBonus: Procedure.BaseCheckBonus);
		var sb = new StringBuilder();
		var age = Patient.Location.Date(Patient.Birthday.Calendar).YearsDifference(Patient.Birthday);
		string ageDescriptor;
		if (age <= 10)
		{
			ageDescriptor = $"{age:N0}";
		}
		else if (age <= 20)
		{
			ageDescriptor = $"{age / 2 * 2:N0}-{age / 2 * 2 + 1:N0}";
		}
		else if (age <= 65)
		{
			ageDescriptor = $"{Math.Round(age / 5.0, 0) * 5}";
		}

		sb.AppendLine(
			$"Your patient is a {Patient.Race.Name.ToLowerInvariant()} {Patient.ApparentGender(Examiner).GenderClass()} of approximately {Patient.Location.Date(Patient.Birthday.Calendar).YearsDifference(Patient.Birthday) / 5 * 5} years of age.");
	}

	/// <summary>
	///     Signals to the proposal that it has been rejected by the Supplicant, and it should take action to cancel the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the DECLINE command</param>
	public override void Reject(string message = "")
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///     Signals to the proposal that it has timed out, and it should take action to cancel the proposition.
	/// </summary>
	public override void Expire()
	{
		throw new NotImplementedException();
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
		return $"{Examiner.HowSeen(voyeur, true)} is proposing to examine {Patient.HowSeen(voyeur)}.";
	}

	public override IEnumerable<string> Keywords { get; } = new[] { "examination" };

	#endregion
}