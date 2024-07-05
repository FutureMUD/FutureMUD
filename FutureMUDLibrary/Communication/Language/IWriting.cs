using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language {

    [Flags]
    public enum WritingStyleDescriptors {
        None = 0,
        Neat = 1 << 0,
        Untidy = 1 << 1,
        Messy = 1 << 2,
        Childish = 1 << 3,
        Cursive = 1 << 4,
        Looped = 1 << 5,
        Crude = 1 << 6,
        Stylised = 1 << 7,
        Block = 1 << 8,
        Precise = 1 << 9,
        SemiCursive = 1 << 10,
        MachinePrinted = 1 << 11,
        Italic = 1 << 12,
        Plain = 1 << 13,
        Unembellished = 1 << 14,
        Ostentatious = 1 << 15,
        Ornate = 1 << 16,
        Clear = 1 << 17,
        Sloppy = 1 << 18
    }

    public static class WritingExtensions {
        public static WritingStyleDescriptors Parse(this WritingStyleDescriptors orig, string text) {
            return Enum.GetValues(typeof(WritingStyleDescriptors)).OfType<WritingStyleDescriptors>().FirstOrDefault(x => x.Describe().EqualTo(text));
        }

        public static string Describe(this WritingStyleDescriptors type) {
            switch (type) {
                case WritingStyleDescriptors.None:
                    return "text";
                case WritingStyleDescriptors.Neat:
                    return "neat";
                case WritingStyleDescriptors.Untidy:
                    return "untidy";
                case WritingStyleDescriptors.Messy:
                    return "messy";
                case WritingStyleDescriptors.Childish:
                    return "childish";
                case WritingStyleDescriptors.Cursive:
                    return "cursive";
                case WritingStyleDescriptors.SemiCursive:
                    return "semi-cursive";
                case WritingStyleDescriptors.Looped:
                    return "looped";
                case WritingStyleDescriptors.Crude:
                    return "crude";
                case WritingStyleDescriptors.Stylised:
                    return "stylised";
                case WritingStyleDescriptors.Block:
                    return "block";
                case WritingStyleDescriptors.Precise:
                    return "precise";
                case WritingStyleDescriptors.MachinePrinted:
                    return "machine print";
                case WritingStyleDescriptors.Italic:
                    return "italic";
                case WritingStyleDescriptors.Plain:
                    return "plain";
                case WritingStyleDescriptors.Unembellished:
                    return "unembellished";
                case WritingStyleDescriptors.Ostentatious:
                    return "ostentatious";
                case WritingStyleDescriptors.Ornate:
                    return "ornate";
                case WritingStyleDescriptors.Clear:
                    return "clear";
                case WritingStyleDescriptors.Sloppy:
                    return "sloppy";
            }
            var flags = type.GetFlags().OfType<WritingStyleDescriptors>().ToList();
            return flags.OrderByDescending(x => x.IsModifierDescriptor()).Select(x => x.Describe()).ListToString(conjunction: "", twoItemJoiner: ", ");
        }

        public static string DescribeModifiers(this WritingStyleDescriptors type, string conjunction = "and", string twoItemJoiner = " and ") {
            var flags = type.GetFlags().OfType<WritingStyleDescriptors>().ToList();
            return flags.Where(x => x.IsModifierDescriptor()).Select(x => x.Describe()).ListToString(conjunction: conjunction, twoItemJoiner: twoItemJoiner);
        }

        public static bool IsModifierDescriptor(this WritingStyleDescriptors type) {
            switch (type) {
                case WritingStyleDescriptors.Neat:
                case WritingStyleDescriptors.Untidy:
                case WritingStyleDescriptors.Messy:
                case WritingStyleDescriptors.Childish:
                case WritingStyleDescriptors.Looped:
                case WritingStyleDescriptors.Stylised:
                case WritingStyleDescriptors.Precise:
                case WritingStyleDescriptors.Crude:
                case WritingStyleDescriptors.Unembellished:
                case WritingStyleDescriptors.Ostentatious:
                case WritingStyleDescriptors.Ornate:
                case WritingStyleDescriptors.Clear:
                case WritingStyleDescriptors.Sloppy:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMachineDescriptor(this WritingStyleDescriptors type) {
            switch (type) {
                case WritingStyleDescriptors.MachinePrinted:
                case WritingStyleDescriptors.Italic:
                    return true;
                default:
                    return false;
            }
        }

        public static double MinimumHandwritingSkill(this WritingStyleDescriptors type) {
            switch (type) {
                case WritingStyleDescriptors.Crude:
                case WritingStyleDescriptors.Childish:
                    return 0.0;
                case WritingStyleDescriptors.Untidy:
                case WritingStyleDescriptors.Messy:
                case WritingStyleDescriptors.Sloppy:
                    return 15.0;
                case WritingStyleDescriptors.Neat:
                case WritingStyleDescriptors.Unembellished:
                case WritingStyleDescriptors.Clear:
                    return 40.0;
                case WritingStyleDescriptors.Looped:
                case WritingStyleDescriptors.Stylised:
                case WritingStyleDescriptors.Precise:
                case WritingStyleDescriptors.Ostentatious:
                case WritingStyleDescriptors.Ornate:
                    return 60.0;
                case WritingStyleDescriptors.Block:
                case WritingStyleDescriptors.Plain:
                    return 0.0;
                case WritingStyleDescriptors.Cursive:
                case WritingStyleDescriptors.SemiCursive:
                    return 25.0;
                default:
                    return 0.0;
            }
        }
    }

    public interface IWriting : ICanBeRead
    {
        WritingStyleDescriptors Style { get; }
        ILanguage Language { get; }
        IScript Script { get; }
        ICharacter TrueAuthor { get; }
        double HandwritingSkill { get; }
        double LiteracySkill { get; }
        double ForgerySkill { get; }
        double LanguageSkill { get; }
        IWriting Copy();
        IColour WritingColour { get; }

        Difficulty WritingDifficulty(ICharacter actor)
        {
	        var difficulty = Difficulty.Trivial;

	        if (!actor.Languages.Contains(Language))
	        {
		        var mutual = actor.Languages.Select(x => x.MutualIntelligability(Language)).DefaultIfEmpty(Difficulty.Impossible).Min();
		        if (mutual == Difficulty.Impossible)
		        {
			        return Difficulty.Impossible;
		        }

		        difficulty = mutual;
	        }

	        // Stage up difficulty based on writing style
	        if (Style.HasFlag(WritingStyleDescriptors.SemiCursive))
	        {
		        difficulty = difficulty.StageUp(1);
	        }
	        else if (Style.HasFlag(WritingStyleDescriptors.Cursive))
	        {
		        difficulty = difficulty.StageUp(2);
	        }
	        else if (Style.HasFlag(WritingStyleDescriptors.Stylised))
	        {
		        difficulty = difficulty.StageUp(3);
	        }

	        if (DocumentLength > 1000)
	        {
		        difficulty = difficulty.StageUp(1);
	        }

	        if (LanguageSkill < Gameworld.GetStaticDouble("ReadDifficultyLanguageSkillThreshold"))
	        {
		        difficulty = difficulty.StageUp(1);
	        }

	        if (LiteracySkill < Gameworld.GetStaticDouble("ReadDifficultyLiteracySkillThreshold"))
	        {
		        difficulty = difficulty.StageUp(1);
	        }

	        if (HandwritingSkill < Gameworld.GetStaticDouble("ReadDifficultyHandwritingSkillThreshold"))
	        {
		        difficulty = difficulty.StageUp(1);
	        }

	        return difficulty;
		}
    }
}
