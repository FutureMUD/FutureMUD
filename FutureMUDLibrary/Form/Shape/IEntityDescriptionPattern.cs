using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.Form.Shape {
	public enum DescriptionType
	{
		/// <summary>
		///     The nominal description of the thing, for example, "a short, blue-eyed man"
		/// </summary>
		Short,

		/// <summary>
		///     The possessive description of the thing, for example "your" or "a short, blue-eyed man's"
		/// </summary>
		Possessive,

		/// <summary>
		///     The description of the thing as seen in its scenic context, e.g. "A short, blue-eyed man is here."
		/// </summary>
		Long,

		/// <summary>
		///     The full, detailed description of a thing, such as when it is LOOKed at
		/// </summary>
		Full,

		/// <summary>
		///     The description displayed, if any, when something requests to look at the contents of this item, such as when it is
		///     "look in"'d.
		/// </summary>
		Contents,

		Evaluate,
	}

	public enum EntityDescriptionType {
		ShortDescription = 0,
		FullDescription
	}

	public interface IEntityDescriptionPattern : IEditableItem, ISaveable {
		EntityDescriptionType Type { get; }
		IFutureProg ApplicabilityProg { get; }
		string Pattern { get; }
		int RelativeWeight { get; }
		bool IsValidSelection(ICharacterTemplate template);
		bool IsValidSelection(ICharacter character);
		IEntityDescriptionPattern Clone();
	}

	public static class EntityDescriptionPatternExtensions {
		public static string GetDescriptionHelpNoTemplate()
		{
			var sb = new StringBuilder();
			sb.AppendLine("You can use the following variables in the markup:");
			sb.AppendLine();
			sb.AppendLine("\t#6&a_an[#3your content#6]#0 - puts 'a' or 'an' and a space in front of your content as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&?a_an[#3your content#6]#0 - puts 'a' or 'an' and a space or nothing in front of your content based on pluralisation".SubstituteANSIColour());
			sb.AppendLine("\t#6&pronoun|#3plural text#6|#3singular text#6&#0 - alternates text based on the grammatical number of the pronoun (e.g. he/she/it vs you/they/them)".SubstituteANSIColour());
			sb.AppendLine("\t#6&he#0 - he/she/it/they/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&him#0 - him/her/it/them/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&his#0 - his/her/its/theirs/your as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&himself#0 - himself/herself/itself/themself/yourself as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&male#0 - male/female/neuter/non-binary as appropriate".SubstituteANSIColour());
			sb.AppendLine($"\t#6&race#0 - the name of the character's race".SubstituteANSIColour());
			sb.AppendLine($"\t#6&culture#0 - the name of the character's culture".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicity#0 - the name of the character's ethnicity".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicgroup#0 - the character's ethnic group".SubstituteANSIColour());
			sb.AppendLine($"\t#6&personword#0 - the character's culture's person word".SubstituteANSIColour());
			sb.AppendLine($"\t#6&age#0 - the character's age category".SubstituteANSIColour());
			sb.AppendLine($"\t#6&height#0 - the character's height".SubstituteANSIColour());
			sb.AppendLine($"\t#6&tattoos#0 - a description of the character's tattoos, or blank if none".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withtattoos#0 - an alternate description of the character's tattoos, or blank if none".SubstituteANSIColour());
			sb.AppendLine($"\t#6&scars#0 - a description of the character's scars, or blank if none".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withscars#0 - an alternate description of the character's scars, or blank if none".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine("You can also use the following forms with each characteristic:");
			sb.AppendLine();
			sb.AppendLine($@"	#6$varname#0 - Displays the ordinary form of the variable. See below for specifics.
	#6$varnamefancy#0 - Displays the fancy form of the variable. See below for specifics.
	#6$varnamebasic#0 - Displays the basic form of the variable. See below for specifics.
	#6$varname[#3display if not obscured#6][#3display if obscured#6]#0 - See below for specifics.
	#6$?varname[#3display if not null/default#6][#3display if null/default#6]#0 - See below for specifics.

#1Note: Inside the above, @ substitutes the variable description, $ substitutes for the name of the obscuring item (e.g. sunglasses), and * substitutes for the sdesc of the obscuring item (e.g. a pair of gold-rimmed sunglasses).#0".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine($"Any characteristics that target a bodypart type can be used with an additional form. Inside this form, you can use @ for the variable description, % for the number of parts possessed, * for the same but in wordy form:");
			sb.AppendLine();
			sb.AppendLine($"\t#6%varname[text if normal count][n-n2:text if between n and n2 count][x:text if x count][y:text if y count]#0".SubstituteANSIColour());
			sb.AppendLine($"\tExample: #3%$eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]#0".SubstituteANSIColour());

			return sb.ToString();
		}

		public static string GetDescriptionHelpFor(ICharacterTemplate target, IAccount account)
		{
			var sb = new StringBuilder();
			sb.AppendLine("You can use the following variables in the markup:");
			sb.AppendLine();
			sb.AppendLine("\t#6&a_an[#3your content#6]#0 - puts 'a' or 'an' and a space in front of your content as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&?a_an[#3your content#6]#0 - puts 'a' or 'an' and a space or nothing in front of your content based on pluralisation".SubstituteANSIColour());
			sb.AppendLine("\t#6&pronoun|#3plural text#6|#3singular text#6&#0 - alternates text based on the grammatical number of the pronoun (e.g. he/she/it vs you/they/them)".SubstituteANSIColour());
			sb.AppendLine("\t#6&he#0 - he/she/it/they/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&him#0 - him/her/it/them/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&his#0 - his/her/its/theirs/your as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&himself#0 - himself/herself/itself/themself/yourself as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&male#0 - male/female/neuter/non-binary as appropriate".SubstituteANSIColour());
			sb.AppendLine($"\t#6&race#0 - the name of the character's race ({target.SelectedRace?.Name.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&culture#0 - the name of the character's culture ({target.SelectedCulture?.Name.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicity#0 - the name of the character's ethnicity ({target.SelectedEthnicity?.Name.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicgroup#0 - the character's ethnic group ({target.SelectedEthnicity?.EthnicGroup?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&personword#0 - the character's culture's person word ({target.SelectedCulture?.PersonWord(target.SelectedGender)?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&age#0 - the character's age category ({target.SelectedRace?.AgeCategory(target).DescribeEnum(true).ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&height#0 - the character's height ({account.Gameworld.UnitManager.Describe(target.SelectedHeight, UnitType.Length, account).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&tattoos#0 - a description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&tattoos", null, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withtattoos#0 - an alternate description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&withtattoos", null, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&scars#0 - a description of the character's scars, or blank if none ({target.ParseCharacteristics("&scars", null, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withscars#0 - an alternate description of the character's scars, or blank if none ({target.ParseCharacteristics("&withscars", null, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine("You can also use the following forms with each characteristic:");
			sb.AppendLine();
			sb.AppendLine($@"	#6$varname#0 - Displays the ordinary form of the variable. See below for specifics.
	#6$varnamefancy#0 - Displays the fancy form of the variable. See below for specifics.
	#6$varnamebasic#0 - Displays the basic form of the variable. See below for specifics.
	#6$varname[#3display if not obscured#6][#3display if obscured#6]#0 - See below for specifics.
	#6$?varname[#3display if not null/default#6][#3display if null/default#6]#0 - See below for specifics.

#1Note: Inside the above, @ substitutes the variable description, $ substitutes for the name of the obscuring item (e.g. sunglasses), and * substitutes for the sdesc of the obscuring item (e.g. a pair of gold-rimmed sunglasses).#0".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine($"The following variables can be used with the above syntax: {target.SelectedCharacteristics.Select(x => x.Item1.Pattern.TransformRegexIntoPattern().ColourName()).ListToCommaSeparatedValues(", ")}");
			var partCharacteristics = target.SelectedCharacteristics.Select(x => x.Item1).OfType<IBodypartSpecificCharacteristicDefinition>().ToList();
			if (partCharacteristics.Any())
			{
				sb.AppendLine();
				sb.AppendLine($"The {"characteristic".Pluralise(partCharacteristics.Count != 1)} {partCharacteristics.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToString()} can be used with an additional form. Inside this form, you can use @ for the variable description, % for the number of parts possessed, * for the same but in wordy form:");
				sb.AppendLine();
				sb.AppendLine($"\t#6%varname[text if normal count][n-n2:text if between n and n2 count][x:text if x count][y:text if y count]#0".SubstituteANSIColour());
				sb.AppendLine($"\tExample: #3%$eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]#0".SubstituteANSIColour());
			}

			return sb.ToString();
		}

		public static string GetDescriptionHelpFor(ICharacter target, ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine("You can use the following variables in the markup:");
			sb.AppendLine();
			sb.AppendLine("\t#6&a_an[#3your content#6]#0 - puts 'a' or 'an' and a space in front of your content as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&?a_an[#3your content#6]#0 - puts 'a' or 'an' and a space or nothing in front of your content based on pluralisation".SubstituteANSIColour());
			sb.AppendLine("\t#6&pronoun|#3plural text#6|#3singular text#6&#0 - alternates text based on the grammatical number of the pronoun (e.g. he/she/it vs you/they/them)".SubstituteANSIColour());
			sb.AppendLine("\t#6&he#0 - he/she/it/they/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&him#0 - him/her/it/them/you as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&his#0 - his/her/its/theirs/your as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&himself#0 - himself/herself/itself/themself/yourself as appropriate".SubstituteANSIColour());
			sb.AppendLine("\t#6&male#0 - male/female/neuter/non-binary as appropriate".SubstituteANSIColour());
			sb.AppendLine($"\t#6&race#0 - the name of the character's race ({target.Race.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&culture#0 - the name of the character's culture ({target.Culture.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicity#0 - the name of the character's ethnicity ({target.Ethnicity.Name.ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&ethnicgroup#0 - the character's ethnic group ({target.Ethnicity.EthnicGroup?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&personword#0 - the character's culture's person word ({target.Culture.PersonWord(target.Gender.Enum)?.ToLowerInvariant().ColourValue() ?? ""})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&age#0 - the character's age category ({target.Race.AgeCategory(target).DescribeEnum(true).ToLowerInvariant().ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&height#0 - the character's height ({actor.Gameworld.UnitManager.Describe(target.Height, UnitType.Length, actor).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&tattoos#0 - a description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&tattoos", actor, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withtattoos#0 - an alternate description of the character's tattoos, or blank if none ({target.ParseCharacteristics("&withtattoos", actor, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&scars#0 - a description of the character's scars, or blank if none ({target.ParseCharacteristics("&scars", actor, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine($"\t#6&withscars#0 - an alternate description of the character's scars, or blank if none ({target.ParseCharacteristics("&withscars", actor, true).ColourValue()})".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine("You can also use the following forms with each characteristic:");
			sb.AppendLine();
			sb.AppendLine($@"	#6$varname#0 - Displays the ordinary form of the variable. See below for specifics.
	#6$varnamefancy#0 - Displays the fancy form of the variable. See below for specifics.
	#6$varnamebasic#0 - Displays the basic form of the variable. See below for specifics.
	#6$varname[#3display if not obscured#6][#3display if obscured#6]#0 - See below for specifics.
	#6$?varname[#3display if not null/default#6][#3display if null/default#6]#0 - See below for specifics.

#1Note: Inside the above, @ substitutes the variable description, $ substitutes for the name of the obscuring item (e.g. sunglasses), and * substitutes for the sdesc of the obscuring item (e.g. a pair of gold-rimmed sunglasses).#0".SubstituteANSIColour());
			sb.AppendLine();
			sb.AppendLine($"The following variables can be used with the above syntax: {target.CharacteristicDefinitions.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToCommaSeparatedValues(", ")}");
			var partCharacteristics = target.CharacteristicDefinitions.OfType<IBodypartSpecificCharacteristicDefinition>().ToList();
			if (partCharacteristics.Any())
			{
				sb.AppendLine();
				sb.AppendLine($"The {"characteristic".Pluralise(partCharacteristics.Count != 1)} {partCharacteristics.Select(x => x.Pattern.TransformRegexIntoPattern().ColourName()).ListToString()} can be used with an additional form. Inside this form, you can use @ for the variable description, % for the number of parts possessed, * for the same but in wordy form:");
				sb.AppendLine();
				sb.AppendLine($"\t#6%varname[text if normal count][n-n2:text if between n and n2 count][x:text if x count][y:text if y count]#0".SubstituteANSIColour());
				sb.AppendLine($"\tExample: #3%$eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]#0".SubstituteANSIColour());
			}

			return sb.ToString();
		}

		public static string Describe(this EntityDescriptionType type) {
			switch (type) {
				case EntityDescriptionType.ShortDescription:
					return "Short Description";
				case EntityDescriptionType.FullDescription:
					return "Full Description";
			}

			return "Unknown";
		}
	}
}