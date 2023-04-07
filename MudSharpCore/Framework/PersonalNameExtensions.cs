using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MudSharp.Character.Name;

namespace MudSharp.Framework;

public static class PersonalNameExtensions
{
	private static readonly Regex _GetByNameRegex = new(@"^(?:(?<count>\d+)\.){0,1}(?<text>.+)$",
		RegexOptions.IgnoreCase);

	public static IEnumerable<T> GetAllByName<T>(this IEnumerable<T> characters, string nameText)
		where T : IHavePersonalName
	{
		return characters.Where(x =>
			x.PersonalName.GetName(NameStyle.FullName).Equals(nameText, StringComparison.InvariantCultureIgnoreCase) ||
			x.PersonalName.GetName(NameStyle.SimpleFull)
			 .Equals(nameText, StringComparison.InvariantCultureIgnoreCase) ||
			x.PersonalName.GetName(NameStyle.GivenOnly)
			 .Equals(nameText, StringComparison.InvariantCultureIgnoreCase) ||
			x.PersonalName.GetName(NameStyle.Affectionate)
			 .Equals(nameText, StringComparison.InvariantCultureIgnoreCase));
	}

	public static T GetByPersonalName<T>(this IEnumerable<T> characters, string nameText)
		where T : IHavePersonalName
	{
		var match = _GetByNameRegex.Match(nameText);
		var count = match.Groups["count"].Success ? int.Parse(match.Groups["count"].Value) : 1;
		return characters.Where(x =>
			                 x.PersonalName.GetName(NameStyle.FullName)
			                  .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			                 x.PersonalName.GetName(NameStyle.SimpleFull)
			                  .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			                 x.PersonalName.GetName(NameStyle.GivenOnly)
			                  .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			                 x.PersonalName.GetName(NameStyle.Affectionate)
			                  .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase))
		                 .ElementAtOrDefault(count - 1);
	}

	public static IPersonalName GetName(this IEnumerable<IPersonalName> names, string nameText)
	{
		var match = _GetByNameRegex.Match(nameText);
		var count = match.Groups["count"].Success ? int.Parse(match.Groups["count"].Value) : 1;
		return names.Where(x =>
			            x.GetName(NameStyle.FullName)
			             .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			            x.GetName(NameStyle.SimpleFull)
			             .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			            x.GetName(NameStyle.GivenOnly)
			             .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase) ||
			            x.GetName(NameStyle.Affectionate)
			             .Equals(match.Groups["text"].Value, StringComparison.InvariantCultureIgnoreCase))
		            .ElementAtOrDefault(count - 1);
	}
}