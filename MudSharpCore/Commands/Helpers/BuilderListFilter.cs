#nullable enable

using MudSharp.Framework;

namespace MudSharp.Commands.Helpers;

/// <summary>Shared filtering for builder catalogues that do not use an editable-item helper.</summary>
internal static class BuilderListFilter
{
	public static bool TryApply<T>(IEnumerable<T> source, StringStack input, out List<T> matches,
		out string error, Func<T, IEnumerable<string?>>? searchText = null,
		IReadOnlyDictionary<string, Func<T, string, bool>>? fields = null) where T : IFrameworkItem
	{
		matches = [];
		error = string.Empty;
		var query = source;
		while (!input.IsFinished)
		{
			var token = input.PopSpeech();
			if (long.TryParse(token, out var id) && token[0] is not ('+' or '-'))
			{
				query = query.Where(item => item.Id == id);
				continue;
			}
			if (token.Length > 1 && token[0] is '+' or '-')
			{
				var include = token[0] == '+';
				var keyword = token[1..];
				query = query.Where(item => (searchText?.Invoke(item) ?? [item.Name])
					.Any(text => text?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) == true) == include);
				continue;
			}
			if (fields is not null && fields.TryGetValue(token.ToLowerInvariant(), out var predicate))
			{
				var value = input.PopSpeech();
				if (!string.IsNullOrWhiteSpace(value))
				{
					query = query.Where(item => predicate(item, value));
					continue;
				}
			}
			error = $"Invalid or incomplete filter {token.ColourCommand()}. Use an ID, {"+keyword".ColourCommand()} or {"-keyword".ColourCommand()}" +
				(fields?.Count > 0 ? $", or a field followed by a value: {fields.Keys.ListToString()}." : ".");
			return false;
		}
		matches = query.ToList();
		return true;
	}

	public static bool IsIdOrName(IFrameworkItem? item, string value) => item is not null &&
		(long.TryParse(value, out var id) ? item.Id == id : item.Name.EqualTo(value));
}
