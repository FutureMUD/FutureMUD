using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework;

public abstract class Proposal : IProposal
{
	#region IProposal Members

	public abstract void Accept(string message = "");

	public abstract void Reject(string message = "");

	public abstract void Expire();

	public abstract string Describe(IPerceiver voyeur);

	#endregion

	#region IKeyworded Members

	public abstract IEnumerable<string> Keywords { get; }

	#endregion
}