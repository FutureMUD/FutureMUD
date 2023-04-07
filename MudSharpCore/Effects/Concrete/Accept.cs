using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Accept : Effect, IProposalEffect
{
	private static string _standardAcceptPhrasing;

	public static string StandardAcceptPhrasing
	{
		get
		{
			if (string.IsNullOrEmpty(_standardAcceptPhrasing))
			{
				_standardAcceptPhrasing = Futuremud.Games.First().GetStaticString("StandardAcceptPhrasing")
				                                   .SubstituteANSIColour();
			}

			return _standardAcceptPhrasing;
		}
	}

	public Accept(IPerceivable owner, IProposal proposal)
		: base(owner)
	{
		Proposal = proposal;
	}

	protected override string SpecificEffectType => "Accept";
	public IProposal Proposal { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Accept Effect - {Proposal.Describe(voyeur)}";
	}

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		Proposal.Expire();
	}

	public override string ToString()
	{
		return "Accept Effect";
	}

	#region IKeyworded Members

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Proposal.GetKeywordsFor(voyeur);
	}

	public IEnumerable<string> Keywords => Proposal.Keywords;

	public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
	{
		return Proposal.HasKeyword(targetKeyword, voyeur, abbreviated);
	}

	public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
	{
		return Proposal.HasKeywords(targetKeywords, voyeur, abbreviated);
	}

	#endregion
}