using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudSharp.Framework;

public class GenericProposal : IProposal
{
	public GenericProposal()
	{
	}

	[SetsRequiredMembers]
	public GenericProposal(Action<string> acceptAction, Action<string> rejectAction, Action expireAction,
		string descriptionString, params string[] keywords)
	{
		AcceptAction = acceptAction;
		RejectAction = rejectAction;
		ExpireAction = expireAction;
		DescriptionString = descriptionString;
		Keywords = keywords.ToList();
	}

	public required string DescriptionString { get; init; }

	public required Action<string> AcceptAction { get; init; }

	public required Action<string> RejectAction { get; init; }

	public required Action ExpireAction { get; init; }

	#region Implementation of IKeyworded

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return ((IKeyworded)this).Keywords;
	}

	public List<string> Keywords { get; init; } = new();

	IEnumerable<string> IKeyworded.Keywords => Keywords.Any() ? Keywords : DescriptionString.Split(' ');

	public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
	{
		return
			GetKeywordsFor(voyeur).Any(
				x =>
					abbreviated
						? x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase)
						: x.Equals(targetKeyword, StringComparison.InvariantCultureIgnoreCase));
	}

	public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
	{
		return
			GetKeywordsFor(voyeur).Any(
				x =>
					abbreviated
						? targetKeywords.Any(y => x.StartsWith(y, StringComparison.InvariantCultureIgnoreCase))
						: targetKeywords.Any(y => x.Equals(y, StringComparison.InvariantCultureIgnoreCase)));
	}

	#endregion

	#region Implementation of IProposal

	/// <summary>
	///     Signals to the proposal that it has been accepted by the Supplicant, and it should take action to resolve the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the ACCEPT command</param>
	public void Accept(string message = "")
	{
		AcceptAction(message);
	}

	/// <summary>
	///     Signals to the proposal that it has been rejected by the Supplicant, and it should take action to cancel the
	///     Proposition.
	/// </summary>
	/// <param name="message">Any additional text passed in by the DECLINE command</param>
	public void Reject(string message = "")
	{
		RejectAction(message);
	}

	/// <summary>
	///     Signals to the proposal that it has timed out, and it should take action to cancel the proposition.
	/// </summary>
	public void Expire()
	{
		ExpireAction();
	}

	/// <summary>
	///     Asks for a one line description of the proposal, as seen by the individual supplied
	/// </summary>
	/// <param name="voyeur">The voyeur for the description</param>
	/// <returns>A string describing the proposal</returns>
	public string Describe(IPerceiver voyeur)
	{
		return DescriptionString;
	}

	#endregion
}