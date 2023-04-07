using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;

namespace MudSharp.Framework.Revision;

public static class RevisionExtensions
{
	/// <summary>
	///     Supplied with an ICharacter and a StringStack, works out the best match for the user input
	/// </summary>
	/// <typeparam name="T">Any IEditableItem</typeparam>
	/// <param name="source">Any IEnumerable of IEditableItems</param>
	/// <param name="actor">The character supplying the input</param>
	/// <param name="text">The remaining input in StringStack form. Pops arguments if used.</param>
	/// <returns>The best match or null</returns>
	public static T BestRevisableMatch<T>(this IEnumerable<T> source, ICharacter actor, StringStack text)
		where T : class, IEditableRevisableItem
	{
		if (text.IsFinished)
		{
			return null;
		}

		var revision = 0;
		string name = null;
		;
		if (long.TryParse(text.PopSpeech(), out var id))
		{
			if (!text.IsFinished && int.TryParse(text.Peek(), out revision))
			{
				text.Pop();
			}
		}
		else
		{
			name = text.Last;
			if (!text.IsFinished && int.TryParse(text.Peek(), out revision))
			{
				text.Pop();
			}
		}

		return source.BestRevisableMatch(actor, id, revision, name);
	}

	public static T BestRevisableMatch<T>(this IEnumerable<T> source, ICharacter actor, long id, int revision,
		string name) where T : class, IEditableRevisableItem
	{
		source = id > 0
			? source.Where(x => x.Id == id).OrderByDescending(x => x.RevisionNumber)
			: source.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
			        .OrderByDescending(x => x.RevisionNumber);

		if (revision > 0)
		{
			return source.FirstOrDefault(x => x.RevisionNumber == revision);
		}

		return
			source.FirstOrDefault(
				x => x.Status == RevisionStatus.UnderDesign && x.BuilderAccountID == actor.Account.Id) ??
			source.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision) ??
			source.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
			source.FirstOrDefault();
	}
}