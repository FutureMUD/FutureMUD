using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable
namespace MudSharp.Communication.Language;

/// <summary>Shared deterministic identity and accent selection rules.</summary>
public static class LanguageAcquisition
{
	public static ILanguage? ResolveNative(IEnumerable<ILanguage> known, Func<ILanguage, double> skill,
		params ILanguage?[] preferences)
	{
		var languages = known.Distinct().ToList();
		return preferences.FirstOrDefault(x => x is not null && languages.Contains(x)) ??
			languages.OrderByDescending(skill).ThenBy(x => x.Id).FirstOrDefault();
	}

	public static IAccent? ResolveAccent(ILanguage language, ILanguage? nativeLanguage,
		IAccent? heard = null, IEnumerable<IAccent>? available = null)
	{
		var accents = (available ?? language.Accents).Where(x => x.Language == language).OrderBy(x => x.Id).ToList();
		if (language == nativeLanguage)
		{
			return accents.FirstOrDefault(x => x.Role == AccentRole.Native) ?? accents.FirstOrDefault();
		}

		return accents.FirstOrDefault(x => x.Role == AccentRole.Foreign && nativeLanguage is not null &&
			x.AssociatedLanguages.Contains(nativeLanguage)) ??
			accents.FirstOrDefault(x => x.Role == AccentRole.Fallback) ??
			accents.FirstOrDefault(x => x == heard && x.Role == AccentRole.Native) ??
			accents.FirstOrDefault(x => x.Role == AccentRole.Native) ?? accents.FirstOrDefault();
	}
}

/// <summary>Scopes the heard accent to synchronous skill branching, including nested checks.</summary>
public sealed class LanguageAcquisitionContext : IDisposable
{
	private static readonly AsyncLocal<LanguageAcquisitionContext?> Current = new();
	private readonly LanguageAcquisitionContext? _previous;
	private readonly IHaveLanguage _learner;
	private readonly IAccent? _accent;

	public LanguageAcquisitionContext(IHaveLanguage learner, IAccent? accent)
	{
		_previous = Current.Value;
		_learner = learner;
		_accent = accent;
		Current.Value = this;
	}

	public static IAccent? HeardBy(IHaveLanguage? learner) => Current.Value is { } context && learner is not null && context._learner == learner ? context._accent : null;
	public void Dispose() => Current.Value = _previous;
}
