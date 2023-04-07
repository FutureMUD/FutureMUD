using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Form.Characteristics {
    public enum CharacteristicDescriptionType {
        Normal,
        Basic,
        Fancy
    }

    public interface IPerceivableHaveCharacteristics : IHaveCharacteristics, IPerceivable
    {

    }

    public interface IHaveCharacteristics {
        IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions { get; }
        IEnumerable<ICharacteristicValue> RawCharacteristicValues { get; }
        IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> RawCharacteristics { get; }
        ICharacteristicValue GetCharacteristic(string type, IPerceiver voyeur);
        ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur);
        void SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value);

        string DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
            CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal);

        string DescribeCharacteristic(string type, IPerceiver voyeur);
        IObscureCharacteristics GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur);
        Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetCharacteristicDefinition(string pattern);

        /// <summary>
        ///     Signals that the MUD Engine has invalidated this definition and this IHaveCharacteristics should remove it if it
        ///     has it
        /// </summary>
        /// <param name="definition"></param>
        void ExpireDefinition(ICharacteristicDefinition definition);

        void RecalculateCharacteristicsDueToExternalChange();
    }

    public static class IHaveCharacteristicsExtensions {
        private static readonly Regex BasicCharacteristicRegex = new(@"(.+)(basic|fancy)", RegexOptions.IgnoreCase);

        private static readonly Regex CharacteristicRegex =
            new(
                @"\$(\!{0,1}|\?{0,1})([\w]+)(?:\[((?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\]){0,1}(?:\[((?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\]){0,1}",
                RegexOptions.IgnoreCase);

        private static readonly Regex ExtraVariableRegex =
            new(@"&(\!{0,1})(height|himself|he|him|his|male|race|culture|ethnicity|ethnicgroup|age|personword|tattoos|withtattoos|scars|withscars)", RegexOptions.IgnoreCase);

        private static readonly Regex PronounNumberRegex =
            new(@"&pronoun\|(?<plural>[^\|]+)\|(?<singular>[^&]+)&", RegexOptions.IgnoreCase);

        private static readonly Regex AAnRegex =
            new(@"&(\!{0,1})a_an\[((?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\]", RegexOptions.IgnoreCase);

        private static readonly Regex AAnPluralRegex =
            new(@"&\?a_an\[(?<inner>(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\]", RegexOptions.IgnoreCase);

        // Example: %eyecolour[&his eyes are @][2-3:&his % eyes are @][1:&his single eye is @][0:&he has no eyes, only empty sockets]
        private static readonly Regex BodypartSpecificRegex =
            new(@"\%(?<characteristic>\w+)(?:\[(?<normal>(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\])(?:\[(?<alt1low>\d+)(?:\-(?<alt1high>\d+))?\:(?<alt1>(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\])?(?:\[(?<alt2low>\d+)(?:\-(?<alt2high>\d+))?\:(?<alt2>(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\])?(?:\[(?<alt3low>\d+)(?:\-(?<alt3high>\d+))?\:(?<alt3>(?:[^\[\]]|(?<open>\[)|(?<-open>\]))*(?(open)(?!)))\])?", RegexOptions.IgnoreCase);

        private static Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetDefinition(string definition,
            IEnumerable<Tuple<ICharacteristicDefinition, ICharacteristicValue>> characteristics) {
            var type = CharacteristicDescriptionType.Normal;
            var selection =
                characteristics.Select(x => x.Item1)
                    .FirstOrDefault(x => x.Pattern.IsMatch(definition) && !BasicCharacteristicRegex.IsMatch(definition));
            if (selection != null) {
                return Tuple.Create(selection, type);
            }
            if (BasicCharacteristicRegex.IsMatch(definition)) {
                selection =
                    characteristics.Select(x => x.Item1)
                        .FirstOrDefault(
                            x =>
                                x.Pattern.IsMatch(
                                    BasicCharacteristicRegex.Match(definition).Groups[1].Value.ToLowerInvariant()));
                type = BasicCharacteristicRegex.Match(definition).Groups[2].Value.Equals("basic",
                    StringComparison.InvariantCultureIgnoreCase)
                    ? CharacteristicDescriptionType.Basic
                    : CharacteristicDescriptionType.Fancy;
            }
            else {
                return Tuple.Create((ICharacteristicDefinition) null, CharacteristicDescriptionType.Normal);
            }

            return Tuple.Create(selection, type);
        }

        private static string DescribeCharacteristic(ICharacteristicValue value,
            CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal) {
            switch (type) {
                case CharacteristicDescriptionType.Basic:
                    return value.GetBasicValue;
                case CharacteristicDescriptionType.Fancy:
                    return value.GetFancyValue;
                case CharacteristicDescriptionType.Normal:
                    return value.GetValue;
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool ContainsPlural<T>(this T owner, string description, IPerceiver voyeur,
            bool ignoreObscurers = false) where T : IHaveCharacteristics {
            // First of all, check to see if there are any replacable characteristic values that are plurals
            var matches = CharacteristicRegex.Matches(description);
            return matches.Cast<Match>()
                       .Where(match => match.Groups[1].Value.Length <= 0)
                       .Select(match => owner.GetCharacteristicDefinition(match.Groups[2].Value))
                       .Where(x => x.Item2 == CharacteristicDescriptionType.Normal)
                       .SelectNotNull(type => type.Item1)
                       .Select(type => owner.GetCharacteristic(type, voyeur))
                       .Where(characteristic => characteristic != null)
                       .Any(characteristic => characteristic.Pluralisation == PluralisationType.Plural) || ParseCharacteristics(owner, description, voyeur, ignoreObscurers).ContainsPlural();

            // Otherwise just parse the text and check it out
        }

        private static bool ContainsPlural(string description,
            IEnumerable<Tuple<ICharacteristicDefinition, ICharacteristicValue>> characteristics, Gendering gender,
            IFuturemud gameworld, IRace race = null, ICulture culture = null, IEthnicity ethnicity = null, int age = 0, double height = 0.0) {
            // First of all, check to see if there are any replacable characteristic values that are plurals
            var matches = CharacteristicRegex.Matches(description);
            if (
                matches.Cast<Match>()
                    .Where(match => match.Groups[1].Value.Length <= 0)
                    .Select(match => GetDefinition(match.Groups[2].Value, characteristics))
                    .Where(x => x.Item2 == CharacteristicDescriptionType.Normal)
                    .Select(
                        type => characteristics.Where(x => x.Item1 == type.Item1).Select(x => x.Item2).FirstOrDefault())
                    .Where(characteristic => characteristic != null)
                    .Any(characteristic => characteristic.Pluralisation == PluralisationType.Plural)) {
                return true;
            }

            // Otherwise just parse the text and check it out
            return
                ParseCharacteristicsAbsolute(description, characteristics, gender, gameworld, race, culture, ethnicity, age, height)
                    .ContainsPlural();
        }

        /// <summary>
        ///     Parses a description given the supplied characteristics ignoring all obscurers and changers
        /// </summary>
        /// <param name="description"></param>
        /// <param name="characteristics"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string ParseCharacteristicsAbsolute(string description,
            IEnumerable<Tuple<ICharacteristicDefinition, ICharacteristicValue>> characteristics, Gendering gender,
            IFuturemud gameworld, IRace race = null, ICulture culture = null, IEthnicity ethnicity = null, int age = 0, double height = 0.0) {
            if (description == null) {
                Console.WriteLine("Null description passed to IHaveCharacteristics.ParseDescriptionAbsolute");
                return "";
            }
            description = AAnPluralRegex.Replace(description,
                m => ContainsPlural(m.Groups["inner"].Value, characteristics, gender, gameworld, race, culture, ethnicity, age,
                    height)
                    ? ParseCharacteristicsAbsolute(m.Groups["inner"].Value, characteristics, gender, gameworld, race, culture, ethnicity, age,
                        height)
                    : ParseCharacteristicsAbsolute(m.Groups["inner"].Value, characteristics, gender, gameworld, race, culture, ethnicity, age,
                        height).A_An());

            description = PronounNumberRegex.Replace(description, m =>{
                switch (gender.Enum)
                {
                    case Gender.Indeterminate:
                    case Gender.NonBinary:
                        return m.Groups["plural"].Value;
                }

                return m.Groups["singular"].Value;
            });

            description = BodypartSpecificRegex.Replace(description, m => m.Groups["normal"].Value.Replace("@", $"${m.Groups["characteristic"].Value}"));

            description = CharacteristicRegex.Replace(description, m => {
                    // Check for ! modifier, which means this is an "Inner" one, to be passed down to a later parser
                    if (m.Groups[1].Value.FirstOrDefault() == '!') {
                        return "$" + m.Groups[1].Value.Substring(1) + m.Groups[2].Value +
                               (!string.IsNullOrEmpty(m.Groups[3].Value)
                                   ? "[" +
                                     ParseCharacteristicsAbsolute(m.Groups[3].Value, characteristics, gender, gameworld,
                                                                  race, culture,  ethnicity, age, height) + "]" +
                                     (!string.IsNullOrEmpty(m.Groups[4].Value)
                                         ? "[" +
                                           ParseCharacteristicsAbsolute(m.Groups[4].Value, characteristics, gender,
                                                                        gameworld, race, culture,  ethnicity, age, height) + "]"
                                         : "")
                                   : "");
                    }

                    Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> type;
                    ICharacteristicValue characteristic;
                    if (m.Groups[2].Value.Equals("height")) {
                        type =
                            new Tuple<ICharacteristicDefinition, CharacteristicDescriptionType>(
                                gameworld.RelativeHeightDescriptors.Ranges.First().Value.Definition,
                                CharacteristicDescriptionType.Normal);
                        characteristic =
                            gameworld.RelativeHeightDescriptors.Ranges.First(x => type.Item1.IsDefaultValue(x.Value))
                                .Value;
                    }
                    else {
                        type = GetDefinition(m.Groups[2].Value, characteristics);
                        characteristic =
                            characteristics.Where(x => x.Item1 == type.Item1).Select(x => x.Item2).FirstOrDefault();
                    }


                    // Check for ? modifier, which means this is a "not null or default" check
                    if (m.Groups[1].Value == "?") {
                        if ((type.Item1 == null) || type.Item1.IsDefaultValue(characteristic)) {
                            return ParseCharacteristicsAbsolute(m.Groups[4].Value, characteristics, gender, gameworld,
                                                                race, culture, ethnicity, age, height);
                        }

                        return ParseCharacteristicsAbsolute(m.Groups[3].Value, characteristics, gender, gameworld, race,
                            culture, ethnicity, age, height);
                    }

                    // Else we just have a "not obscured" regular variable
                    if (type.Item1 == null) {
                        return m.Groups[0].Value;
                    }

                                                          if (!string.IsNullOrEmpty(m.Groups[3].Value)) {
                                                              return
                                                                  ParseCharacteristicsAbsolute(
                                                                      m.Groups[3].Value.Replace("@",
                                                                                                type.Item2 == CharacteristicDescriptionType.Normal
                                                                                                    ? characteristic.GetValue
                                                                                                    : (type.Item2 == CharacteristicDescriptionType.Basic
                                                                                                        ? characteristic.GetBasicValue
                                                                                                        : characteristic.GetFancyValue)), characteristics, gender, gameworld, race,
                                                                      culture, ethnicity, age, height);
                                                          }
                                                          return string.IsNullOrEmpty(m.Groups[4].Value)
                        ? DescribeCharacteristic(characteristic, type.Item2)
                        : "";
                }
            );

            description = ExtraVariableRegex.Replace(description, m => {
                if (m.Groups[1].Value == "!") {
                    return "&" + m.Groups[2].Value;
                }

                switch (m.Groups[2].Value)
                {
                    case "he":
                        return gender.Subjective();
                    case "him":
                        return gender.Objective();
                    case "his":
                        return gender.Possessive();
                    case "himself":
                        return gender.Reflexive();
                    case "male":
                        return gender.GenderClass();
                    case "race":
                        return race?.Name.ToLowerInvariant() ?? "unknown";
                    case "culture":
                        return culture?.Name.ToLowerInvariant() ?? "unknown";
                    case "ethnicity":
                        return ethnicity?.Name.ToLowerInvariant() ?? "unknown";
                    case "ethnicgroup":
                        return ethnicity?.EthnicGroup.ToLowerInvariant() ?? "unknown";
                    case "personword":
                        return culture?.PersonWord(gender.Enum).ToLowerInvariant() ?? "unknown";
                    case "age":
                        return race?.AgeCategory(age).DescribeEnum(true).ToLowerInvariant() ?? "adult";
                    case "height":
                        return gameworld.UnitManager.Describe(height, UnitType.Length,
                            gameworld.UnitManager.Units.First().System);
                    case "tattoos":
                    case "withtattoos":
                        return string.Empty;
                    case "scars":
                    case "withscars":
                        return string.Empty;
                    default:
                        return m.Groups[0].Value;
                }
            });

            description = AAnRegex.Replace(description, m => {
                switch (m.Groups[1].Value) {
                    case "!":
                        return "&a_an[" + m.Groups[2].Value + "]";
                    default:
                        return m.Groups[2].Value.A_An();
                }
            });

            return description;
        }

        public static string ParseCharacteristics<T>(this T owner, string description, IPerceiver voyeur,
            bool ignoreObscurers = false) where T : IHaveCharacteristics {
            if (description == null) {
                Console.WriteLine("Null description passed to IHaveCharacteristics.ParseCharacteristics");
                return "";
            }

            description = AAnPluralRegex.Replace(description,
                m => ContainsPlural(owner, m.Groups["inner"].Value, voyeur, ignoreObscurers)
                    ? ParseCharacteristics(owner, m.Groups["inner"].Value, voyeur, ignoreObscurers)
                    : ParseCharacteristics(owner, m.Groups["inner"].Value, voyeur, ignoreObscurers).A_An());

            var ownerPerc = owner as IPerceivable;
            var ownerChargen = owner as ICharacterTemplate;
            var gender = ownerPerc is not null ? ownerPerc.ApparentGender(voyeur) : Gendering.Get(ownerChargen.SelectedGender);
            description = PronounNumberRegex.Replace(description, m => {
                switch (gender.Enum)
                {
                    case Gender.Indeterminate:
                    case Gender.NonBinary:
                        return m.Groups["plural"].Value;
                }

                return m.Groups["singular"].Value;
            });

            description = BodypartSpecificRegex.Replace(description, m => {
                var ownerbody = (owner as IBody) ?? (owner as IHaveABody)?.Body;
                if (ownerbody == null)
                {
                    return m.Value;
                }

                var type = owner.GetCharacteristicDefinition(m.Groups["characteristic"].Value);
                if (type == null)
                {
                    return m.Value;
                }

                if (!(type.Item1 is IBodypartSpecificCharacteristicDefinition bodypartSpecificCharacteristicDefinition))
                {
                    return m.Value;
                }

                var ownercount = ownerbody.Bodyparts.Count(x => x.Shape == bodypartSpecificCharacteristicDefinition.TargetShape);
                string text;
                if (ownercount == bodypartSpecificCharacteristicDefinition.OrdinaryCount)
                {
                    text = m.Groups["normal"].Value;
                }
                else
                {
                    if (m.Groups["alt1low"].Length > 0 && int.Parse(m.Groups["alt1low"].Value) <= ownercount && (m.Groups["alt1high"].Length == 0 || int.Parse(m.Groups["alt1high"].Value) >= ownercount))
                    {
                        text = m.Groups["alt1"].Value;
                    }
                    else if (m.Groups["alt2low"].Length > 0 && int.Parse(m.Groups["alt2low"].Value) <= ownercount && (m.Groups["alt2high"].Length == 0 || int.Parse(m.Groups["alt2high"].Value) >= ownercount))
                    {
                        text = m.Groups["alt2"].Value;
                    }
                    else if (m.Groups["alt3low"].Length > 0 && int.Parse(m.Groups["alt3low"].Value) <= ownercount && (m.Groups["alt3high"].Length == 0 || int.Parse(m.Groups["alt3high"].Value) >= ownercount))
                    {
                        text = m.Groups["alt3"].Value;
                    }
                    else
                    {
                        text = m.Groups["normal"].Value;
                    }
                }

                return text
                .Replace("@", $"${m.Groups["characteristic"].Value}")
                .Replace("%", ownercount.ToString("N0", voyeur))
                .Replace("*", ownercount.ToWordyNumber())
                ;
            });

            description = CharacteristicRegex.Replace(description, m => {
                    // Check for ! modifier, which means this is an "Inner" one, to be passed down to a later parser
                    if (m.Groups[1].Value.FirstOrDefault() == '!') {
                        return "$" + m.Groups[1].Value.Substring(1) + m.Groups[2].Value +
                               (!string.IsNullOrEmpty(m.Groups[3].Value)
                                   ? "[" + owner.ParseCharacteristics(m.Groups[3].Value, voyeur, ignoreObscurers) + "]" +
                                     (!string.IsNullOrEmpty(m.Groups[4].Value)
                                         ? "[" + owner.ParseCharacteristics(m.Groups[4].Value, voyeur, ignoreObscurers) +
                                           "]"
                                         : "")
                                   : "");
                    }

                                                          var type = owner.GetCharacteristicDefinition(m.Groups[2].Value);
                    // Check for ? modifier, which means this is a "not null or default" check
                    if (m.Groups[1].Value == "?") {
                        if ((type.Item1 == null) ||
                            type.Item1.IsDefaultValue(owner.GetCharacteristic(type.Item1, voyeur))) {
                            return owner.ParseCharacteristics(m.Groups[4].Value, voyeur);
                        }

                        return owner.ParseCharacteristics(m.Groups[3].Value, voyeur);
                    }

                    // Else we just have a "not obscured" regular variable
                    if (type.Item1 == null) {
                        return m.Groups[0].Value;
                    }

                                                          var obscurer = owner.GetObscurer(type.Item1, voyeur);

                    if ((obscurer == null) || ignoreObscurers) {
                        return !string.IsNullOrEmpty(m.Groups[3].Value)
                            ? owner.ParseCharacteristics(
                                m.Groups[3].Value.Replace("@",
                                                          owner.DescribeCharacteristic(type.Item1, voyeur, type.Item2)), voyeur,
                                ignoreObscurers)
                            : (string.IsNullOrEmpty(m.Groups[4].Value)
                                ? owner.DescribeCharacteristic(type.Item1, voyeur, type.Item2)
                                : "");
                    }

                                                          return
                        owner.ParseCharacteristics(
                            m.Groups[4].Value.Replace("@", obscurer.DescribeCharacteristic(type.Item1, voyeur))
                                .Replace("$", obscurer.Parent.Name.ToLowerInvariant())
                                .Replace("*", obscurer.Parent.HowSeen(voyeur, colour: false)), voyeur);
                }
            );

            description = ExtraVariableRegex.Replace(description, m => {
                if (m.Groups[1].Value == "!") {
                    return "&" + m.Groups[2].Value;
                }
                                                         
                switch (m.Groups[2].Value) {
                    case "he":
                        return gender.Subjective();
                    case "him":
                        return gender.Objective();
                    case "his":
                        return gender.Possessive();
                    case "himself":
                        return gender.Reflexive();
                    case "height":
                        var character = owner as ICharacter ?? (owner as IBody)?.Actor;
                        return character != null
                            ? voyeur.Gameworld.UnitManager.Describe(character.Height, UnitType.Length, voyeur)
                            : "unknown";
                    case "male":
                        return gender.GenderClass();
                    case "race":
                        var race = owner as IHaveRace;
                        return race?.Race?.Name.ToLowerInvariant() ?? "unknown";
                    case "culture":
                        var culture = owner as IHaveCulture;
                        return culture?.Culture?.Name.ToLowerInvariant() ?? "unknown";
                    case "ethnicity":
                        var ethnicity = owner as IHaveRace;
                        return ethnicity?.Ethnicity?.Name.ToLowerInvariant() ?? "unknown";
                    case "ethnicgroup":
                        ethnicity = owner as IHaveRace;
                        return ethnicity?.Ethnicity?.EthnicGroup.ToLowerInvariant() ?? "unknown";
                    case "personword":
                        culture = owner as IHaveCulture;
                        return culture?.Culture.PersonWord(gender.Enum).ToLowerInvariant() ?? "unknown";
                    case "age":
                        race = owner as IHaveRace;
                        return race?.AgeCategory.DescribeEnum(true).ToLowerInvariant() ?? "adult";
                    case "tattoos":
                        character = owner as ICharacter ?? (owner as IBody)?.Actor;
                        if (character != null)
                        {
                            var tattoos = character.Body.Tattoos
                            .Where(x => character.Body.ExposedBodyparts.Contains(x.Bodypart))
                            .OrderByDescending(x => x.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (tattoos.Any(x => x.TattooTemplate.HasSpecialTattooCharacteristicOverride))
                            {
                                return tattoos.First(x => x.TattooTemplate.HasSpecialTattooCharacteristicOverride).TattooTemplate.SpecialTattooCharacteristicOverride(false).SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 3 || tattoos.Any(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)))
                            {
                                return character.Gameworld.GetStaticString("TattooSDescInked").SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 10 || tattoos.Count(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)) > 2)
                            {
                                return character.Gameworld.GetStaticString("TattooSDescHeavilyInked").SpaceIfNotEmpty();
                            }
                        }

                        var chargen = owner as ICharacterTemplate;
                        if (chargen != null)
                        {
                            var tattoos = chargen.SelectedDisfigurements
                            .Where(x => x.Disfigurement is ITattooTemplate)
                            .Select(x => (Tattoo: (ITattooTemplate)x.Disfigurement, x.Bodypart))
                            .OrderByDescending(x => x.Tattoo.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (tattoos.Any(x => x.Tattoo.HasSpecialTattooCharacteristicOverride))
                            {
                                return tattoos.First(x => x.Tattoo.HasSpecialTattooCharacteristicOverride).Tattoo.SpecialTattooCharacteristicOverride(false).SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 3 || tattoos.Any(x => x.Tattoo.Size >= (chargen.SelectedRace?.ModifiedSize(x.Bodypart) ?? x.Bodypart.Size)))
                            {
                                return chargen.Gameworld.GetStaticString("TattooSDescInked").SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 10 || tattoos.Count(x => x.Tattoo.Size >= (chargen.SelectedRace?.ModifiedSize(x.Bodypart) ?? x.Bodypart.Size)) > 2)
                            {
                                return chargen.Gameworld.GetStaticString("TattooSDescHeavilyInked").SpaceIfNotEmpty();
                            }
                        }
                        return string.Empty;
                    case "withtattoos":
                        character = owner as ICharacter ?? (owner as IBody)?.Actor;
                        if (character != null)
                        {
                            var tattoos = character.Body.Tattoos
                            .Where(x => character.Body.ExposedBodyparts.Contains(x.Bodypart))
                            .OrderByDescending(x => x.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (tattoos.Any(x => x.TattooTemplate.HasSpecialTattooCharacteristicOverride))
                            {
                                return tattoos.First(x => x.TattooTemplate.HasSpecialTattooCharacteristicOverride).TattooTemplate.SpecialTattooCharacteristicOverride(true).SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 3 || tattoos.Any(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)))
                            {
                                return character.Gameworld.GetStaticString("TattooSDescInkedWith").SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 10 || tattoos.Count(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)) > 2)
                            {
                                return character.Gameworld.GetStaticString("TattooSDescHeavilyInkedWith").SpaceIfNotEmpty();
                            }
                        }
                        chargen = owner as ICharacterTemplate;
                        if (chargen != null)
                        {
                            var tattoos = chargen.SelectedDisfigurements
                            .Where(x => x.Disfigurement is ITattooTemplate)
                            .Select(x => (Tattoo: (ITattooTemplate)x.Disfigurement, x.Bodypart))
                            .OrderByDescending(x => x.Tattoo.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (tattoos.Any(x => x.Tattoo.HasSpecialTattooCharacteristicOverride))
                            {
                                return tattoos.First(x => x.Tattoo.HasSpecialTattooCharacteristicOverride).Tattoo.SpecialTattooCharacteristicOverride(true).SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 3 || tattoos.Any(x => x.Tattoo.Size >= (chargen.SelectedRace?.ModifiedSize(x.Bodypart) ?? x.Bodypart.Size)))
                            {
                                return chargen.Gameworld.GetStaticString("TattooSDescInkedWith").SpaceIfNotEmpty();
                            }

                            if (tattoos.Count > 10 || tattoos.Count(x => x.Tattoo.Size >= (chargen.SelectedRace?.ModifiedSize(x.Bodypart) ?? x.Bodypart.Size)) > 2)
                            {
                                return chargen.Gameworld.GetStaticString("TattooSDescHeavilyInkedWith").SpaceIfNotEmpty();
                            }
                        }
                        return string.Empty;
                    case "scars":
                        character = owner as ICharacter ?? (owner as IBody)?.Actor;
                        if (character != null)
                        {
                            var scars = character.Body.Scars
                            .Where(x => character.Body.ExposedBodyparts.Contains(x.Bodypart))
                            .OrderByDescending(x => x.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (scars.Any(x => x.ScarTemplate.HasSpecialScarCharacteristicOverride))
                            {
                                return scars.First(x => x.ScarTemplate.HasSpecialScarCharacteristicOverride).ScarTemplate.SpecialScarCharacteristicOverride(false).SpaceIfNotEmpty();
                            }

                            if (scars.Count > 3 || scars.Any(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)))
                            {
                                return character.Gameworld.GetStaticString("ScarSDescAdditionScarred").SpaceIfNotEmpty();
                            }

                            if (scars.Count > 10 || scars.Count(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)) > 2)
                            {
                                return character.Gameworld.GetStaticString("ScarSDescAdditionHeavilyScarred").SpaceIfNotEmpty();
                            }
                        }

                        chargen = owner as ICharacterTemplate;
                        if (chargen != null)
                        {
                            var scars = chargen.SelectedDisfigurements
                            .Where(x => x.Disfigurement is IScarTemplate)
                            .Select(x => (Scar: (IScarTemplate)x.Disfigurement, x.Bodypart))
                            .OrderByDescending(x => x.Bodypart.Size.ChangeSize(x.Scar.SizeSteps))
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (scars.Any(x => x.Scar.HasSpecialScarCharacteristicOverride))
                            {
                                return scars.First(x => x.Scar.HasSpecialScarCharacteristicOverride).Scar.SpecialScarCharacteristicOverride(false).SpaceIfNotEmpty();
                            }

                            if (scars.Count > 3)
                            {
                                return chargen.Gameworld.GetStaticString("ScarSDescAdditionScarred").SpaceIfNotEmpty();
                            }

                            if (scars.Count > 10)
                            {
                                return chargen.Gameworld.GetStaticString("ScarSDescAdditionHeavilyScarred").SpaceIfNotEmpty();
                            }
                        }
                        return string.Empty;
                    case "withscars":
                        character = owner as ICharacter ?? (owner as IBody)?.Actor;
                        if (character != null)
                        {
                            var scars = character.Body.Scars
                            .Where(x => character.Body.ExposedBodyparts.Contains(x.Bodypart))
                            .OrderByDescending(x => x.Size)
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (scars.Any(x => x.ScarTemplate.HasSpecialScarCharacteristicOverride))
                            {
                                return scars.First(x => x.ScarTemplate.HasSpecialScarCharacteristicOverride).ScarTemplate.SpecialScarCharacteristicOverride(true).SpaceIfNotEmpty();
                            }

                            if (scars.Count > 3 || scars.Any(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)))
                            {
                                return character.Gameworld.GetStaticString("ScarSDescAdditionScarredWith").SpaceIfNotEmpty();
                            }

                            if (scars.Count > 10 || scars.Count(x => x.Size >= character.Race.ModifiedSize(x.Bodypart)) > 2)
                            {
                                return character.Gameworld.GetStaticString("ScarSDescAdditionHeavilyScarredWith").SpaceIfNotEmpty();
                            }
                        }

                        chargen = owner as ICharacterTemplate;
                        if (chargen != null)
                        {
                            var scars = chargen.SelectedDisfigurements
                            .Where(x => x.Disfigurement is IScarTemplate)
                            .Select(x => (Scar: (IScarTemplate)x.Disfigurement, x.Bodypart))
                            .OrderByDescending(x => x.Bodypart.Size.ChangeSize(x.Scar.SizeSteps))
                            .ThenByDescending(x => x.Bodypart.IsVital)
                            .ToList();

                            if (scars.Any(x => x.Scar.HasSpecialScarCharacteristicOverride))
                            {
                                return scars.First(x => x.Scar.HasSpecialScarCharacteristicOverride).Scar.SpecialScarCharacteristicOverride(true).SpaceIfNotEmpty();
                            }

                            if (scars.Count > 3)
                            {
                                return chargen.Gameworld.GetStaticString("ScarSDescAdditionScarredWith").SpaceIfNotEmpty();
                            }

                            if (scars.Count > 10)
                            {
                                return chargen.Gameworld.GetStaticString("ScarSDescAdditionHeavilyScarredWith").SpaceIfNotEmpty();
                            }
                        }
                        return string.Empty;
                    default:
                        return m.Groups[0].Value;
                }
            });

            description = AAnRegex.Replace(description, m => {
                switch (m.Groups[1].Value) {
                    case "!":
                        return "&a_an[" + m.Groups[2].Value + "]";
                }
                return m.Groups[2].Value.A_An();
            });

            return description;
        }
    }
}