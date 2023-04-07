using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation;

public class ChargenApprovalProposal : Proposal, IProposal
{
	private readonly ICharacter Proponent;
	private readonly Chargen Target;

	public ChargenApprovalProposal(ICharacter proponent, Chargen target)
	{
		Proponent = proponent;
		Target = target;
	}

	#region IProposal Members

	private void AcceptEditorPost(string message, IOutputHandler output, object[] arguments)
	{
		using (new FMDB())
		{
			var chargen = FMDB.Context.Chargens.FirstOrDefault(x => x.Id == Target.Id);

			if (chargen == null)
			{
				Proponent.OutputHandler.Send("There is no such application for you to approve.");
				return;
			}

			if (chargen.Status == (int)CharacterStatus.Active)
			{
				Proponent.OutputHandler.Send("That application has already been approved.");
				return;
			}
		}

		Target.ApproveApplication(Proponent, Proponent.Account, message, output);
	}

	private void AcceptEditorCancel(IOutputHandler output, object[] arguments)
	{
		output.Send("You decide not to review the application.");
		Target.ReleaseApplication();
	}

	public override void Accept(string message = "")
	{
		Proponent.OutputHandler.Send("Please enter any comments you have to the applicant.");
		Proponent.EditorMode(AcceptEditorPost, AcceptEditorCancel, 1.0, null, EditorOptions.PermitEmpty);
	}

	private void DeclineEditorPost(string message, IOutputHandler output, object[] arguments)
	{
		Target.RejectApplication(Proponent, Proponent.Account, message, output);
	}

	private void DeclineEditorCancel(IOutputHandler output, object[] arguments)
	{
		output.Send("You decide not to review the application.");
		Target.ReleaseApplication();
	}

	public override void Reject(string message = "")
	{
		Proponent.OutputHandler.Send("Please enter any comments you have to the applicant.");
		Proponent.EditorMode(DeclineEditorPost, DeclineEditorCancel, 1.0);
	}

	public override void Expire()
	{
		Proponent.OutputHandler.Send("You decline to review the application.");
		Target.ReleaseApplication();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return Proponent.HowSeen(voyeur, true) + " is reviewing the character application " +
		       Target.SelectedName.GetName(NameStyle.FullName) + ".";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "chargen" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}