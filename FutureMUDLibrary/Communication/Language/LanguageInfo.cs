using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language {
    [Flags]
    public enum LanguageOutputStyle {
        Normal = 1 << 0,

        /// <summary>
        ///     Return only the (possibly scrambled or error) text, with no language/accent preamble, such as used in emote inline
        ///     speech
        /// </summary>
        TextOnly = 1 << 1,

        /// <summary>
        ///     Do not append accent information
        /// </summary>
        NoAccent = 1 << 2
    }

    public enum LanguageForm {
        Spoken,
        Written,
        Psychic
    }

    public enum LanguagePerceptionResult
    {
        Success,
        CannotHear,
        ListenFailure,
        HearFailure,
        PartialSuccess,
        UnknownLanguage,
        MutuallyIntelligableLanguage
    }

    /// <summary>
    ///     Base class that contains information about a fragment of information that is language-based
    /// </summary>
    public abstract class LanguageInfo {
        protected Difficulty _ratedDifficulty;

        /// <summary>
        ///     The raw text that was being communicated linguistically
        /// </summary>
        protected string _rawText;

        protected LanguageInfo(ILanguage language, string text, Outcome originoutcome, IPerceivable origin,
            IPerceivable proxy = null) {
            Language = language;
            _ratedDifficulty = Language.Model.RateDifficulty(new ExplodedString(text));
            Origin = origin;
            OriginOutcome = originoutcome;
            Proxy = proxy;
            _rawText = text.ProperSentences().NormaliseSpacing().Trim();
        }

        public abstract LanguageForm Form { get; }

        /// <summary>
        ///     The IPerceivable from whence this fragment of language originated. The true origin.
        /// </summary>
        public IPerceivable Origin { get; protected set; }

        /// <summary>
        ///     The IPerceivable from whence this fragment of language appears to originate. For example, a radio item.
        /// </summary>
        public IPerceivable Proxy { get; protected set; }

        /// <summary>
        ///     The language in which this fragment is encoded
        /// </summary>
        public ILanguage Language { get; protected set; }

        public Outcome OriginOutcome { get; protected set; }

        public abstract string ParseFor(ILanguagePerceiver perceiver);
    }

    public class SpokenLanguageInfo : LanguageInfo {
        public SpokenLanguageInfo(ILanguage language, IAccent accent, AudioVolume volume, string text,
            Outcome originoutcome, IPerceivable origin, IPerceivable target, IPerceivable proxy = null)
            : base(language, text, originoutcome, origin, proxy) {
            Accent = accent;
            Volume = volume;
            Target = target;
        }

        public SpokenLanguageInfo(SpokenLanguageInfo rhs, AudioVolume newVolume, IPerceivable proxy)
            : base(rhs.Language, rhs._rawText, rhs.OriginOutcome, rhs.Origin, proxy) {
            Accent = rhs.Accent;
            Volume = newVolume;
            Target = rhs.Target;
        }

        public IAccent Accent { get; protected set; }
        public AudioVolume Volume { get; protected set; }
        public IPerceivable Target { get; protected set; }

        public override LanguageForm Form => LanguageForm.Spoken;

        protected (LanguagePerceptionResult Result, double ListenObfuscation, double LanguageObfuscation) GetPerceptionResult(ILanguagePerceiver perceiver) {
            if (perceiver == null) {
                return (LanguagePerceptionResult.Success, 0.0, 0.0);
            }

            var proximity = perceiver.IsSelf(Origin) || perceiver.IsSelf(Target)
                ? Proximity.Intimate
                : perceiver.GetProximity(Proxy);

            if (perceiver.IsSelf(Origin) || perceiver.IsSelf(Proxy) ||
                ((perceiver as ICharacter)?.IsAdministrator() == true)) {
                return (LanguagePerceptionResult.Success, 0.0, 0.0);
            }

            if (!perceiver.CanHear(Proxy)) {
                return (LanguagePerceptionResult.CannotHear, 1.0, 0.0);
            }

            var listenResult = perceiver.Gameworld.GetCheck(CheckType.LanguageListenCheck)
                .Check(perceiver, perceiver.Location.LocalAudioDifficulty(perceiver, Volume, proximity), Proxy);
            if (listenResult == Outcome.MajorFail) {
                return (LanguagePerceptionResult.ListenFailure, 1.0, 0.0);
            }
            
            var accentDifficulty = perceiver.AccentDifficulty(Accent);
            if (OriginOutcome.IsFail()) {
                accentDifficulty = accentDifficulty.StageUp(1);
            }

            var hearCheckResult = perceiver.Gameworld.GetCheck(CheckType.SpokenLanguageHearCheck)
                .Check(perceiver, (Difficulty) Math.Min((int) accentDifficulty, (int) _ratedDifficulty),
                    Language.LinkedTrait, Origin);
            var originalResult = hearCheckResult.Outcome;
            var usedOtherLanguage = false;
            if (hearCheckResult.IsFail() && !perceiver.Languages.Contains(Language)) {
                foreach (var language in perceiver.Languages.Except(Language)
                    .Where(x => x.MutualIntelligability(Language) != Difficulty.Impossible).ToList()) {
                    hearCheckResult = perceiver.Gameworld.GetCheck(CheckType.SpokenLanguageHearCheck)
                        .Check(perceiver, (Difficulty)Math.Max((int)accentDifficulty, Math.Max((int)_ratedDifficulty, (int)language.MutualIntelligability(Language))),
                               language.LinkedTrait, Origin);
                    usedOtherLanguage = true;
                    if (hearCheckResult.IsPass()) {
                        break;
                    }
                }
            }

            if (hearCheckResult.Outcome == Outcome.MajorFail && !perceiver.Languages.Contains(Language)) {
                return (LanguagePerceptionResult.UnknownLanguage, 0.0, 1.0);
            }

            var hearResult = originalResult.CheckDegrees() > hearCheckResult.Outcome.CheckDegrees() ? originalResult : hearCheckResult.Outcome;

            if (hearResult == Outcome.MajorFail) {
                return (usedOtherLanguage ? LanguagePerceptionResult.UnknownLanguage : LanguagePerceptionResult.HearFailure, 0.0, 1.0);
            }

            // Having established that the perceiver can both hear and understand to some extent, we must work out a ratio of word obfuscation
            var listenRatio = Language.LanguageObfuscationFactor * listenResult.FailureDegrees();
            var languageRatio = Language.LanguageObfuscationFactor *
                                (usedOtherLanguage
                                    ? Math.Min(1, 3 - hearResult.CheckDegrees())
                                    : hearResult.FailureDegrees());
            var ratio = listenRatio + languageRatio;
            if (ratio >= 1.0) {
                return (usedOtherLanguage ? LanguagePerceptionResult.UnknownLanguage : LanguagePerceptionResult.HearFailure, 0.0, 1.0);
            }

            if (usedOtherLanguage) {
                return (LanguagePerceptionResult.MutuallyIntelligableLanguage, listenRatio, languageRatio);
            }

            return ratio > 0
                ? (LanguagePerceptionResult.PartialSuccess, listenRatio, languageRatio)
                : (LanguagePerceptionResult.Success, 0.0, 0.0);
        }

        public override string ParseFor(ILanguagePerceiver perceiver) {
            var result = GetPerceptionResult(perceiver);
            switch (result.Result) {
                case LanguagePerceptionResult.CannotHear:
                    return " something that you cannot hear.";
                case LanguagePerceptionResult.UnknownLanguage:
                    return $" something in {Language.UnknownLanguageSpokenDescription}.";
                case LanguagePerceptionResult.HearFailure:
                    return
                        $" something in {Language.Name.TitleCase()} {(perceiver.Accents.Contains(Accent) || ((perceiver as ICharacter)?.IsAdministrator() ?? false) ? Accent.AccentSuffix : Accent.VagueSuffix)}, but you cannot understand {Origin.ApparentGender(perceiver).Objective()}.";
                case LanguagePerceptionResult.ListenFailure:
                    return " something that you cannot make out.";
                case LanguagePerceptionResult.MutuallyIntelligableLanguage:
                    return $", in {Language.Name.TitleCase()}, \n{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText), Math.Min(result.ListenObfuscation, 1.0), Math.Min(result.LanguageObfuscation, 1.0)).Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                case LanguagePerceptionResult.PartialSuccess:
                    return
                        $", in {Language.Name.TitleCase()} {(perceiver.Accents.Contains(Accent) || ((perceiver as ICharacter)?.IsAdministrator() ?? false) ? Accent.AccentSuffix : Accent.VagueSuffix)}, \n{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText), Math.Min(result.ListenObfuscation, 1.0), Math.Min(result.LanguageObfuscation, 1.0)).Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                case LanguagePerceptionResult.Success:
                    return
                        $", in {Language.Name.TitleCase()} {(perceiver.Accents.Contains(Accent) || ((perceiver as ICharacter)?.IsAdministrator() ?? false) ? Accent.AccentSuffix : Accent.VagueSuffix)}, \n{_rawText.Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                default:
                    throw new ApplicationException("Unknown LanguagePerceptionResult in SpokenLanguageInfo.ParseFor");
            }
        }
    }

    public class EmoteSpokenLanguageInfo : SpokenLanguageInfo {
        public EmoteSpokenLanguageInfo(ILanguage language, IAccent accent, AudioVolume volume, string text,
            Outcome originoutcome, IPerceivable origin, IPerceivable target, IPerceivable proxy = null)
            : base(language, accent, volume, text, originoutcome, origin, target, proxy) {
        }

        public override string ParseFor(ILanguagePerceiver perceiver) {
            var result = GetPerceptionResult(perceiver);
            switch (result.Item1) {
                case LanguagePerceptionResult.CannotHear:
                    return "*** something that you cannot hear ***".ColourBold(Telnet.Cyan);
                case LanguagePerceptionResult.HearFailure:
                    return "*** something that you don't quite understand ***"
                        .FluentTagMXP("send",
                            $"href='look' hint='Language: {Language.Name.TitleCase()}, Accent: {(perceiver.Accents.Contains(Accent) ? Accent.AccentSuffix : Accent.VagueSuffix)}'");
                case LanguagePerceptionResult.ListenFailure:
                    return "*** something that you cannot make out ***".ColourBold(Telnet.Cyan);
                case LanguagePerceptionResult.UnknownLanguage:
                    return $"*** something in {Language.UnknownLanguageSpokenDescription} ***".ColourBold(Telnet.Cyan);
                case LanguagePerceptionResult.MutuallyIntelligableLanguage:
                    return
                        $"\"{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText.ProperSentences()), Math.Min(result.Item2, 1.0)).Fullstop()}\""
                            .FluentTagMXP("send",
                                          $"href='look' hint='Language: {Language.Name.TitleCase()}'");
                case LanguagePerceptionResult.PartialSuccess:
                    return
                        $"\"{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText.ProperSentences()), Math.Min(result.Item2, 1.0)).Fullstop()}\""
                            .FluentTagMXP("send",
                                $"href='look' hint='Language: {Language.Name.TitleCase()}, Accent: {(perceiver.Accents.Contains(Accent) ? Accent.AccentSuffix : Accent.VagueSuffix)}'");

                case LanguagePerceptionResult.Success:
                    return $"\"{_rawText.ProperSentences()}\""
                        .FluentTagMXP("send",
                            $"href='look' hint='Language: {Language.Name.TitleCase()}, Accent: {(perceiver.Accents.Contains(Accent) ? Accent.AccentSuffix : Accent.VagueSuffix)}'");
                default:
                    throw new ApplicationException(
                        "Unknown LanguagePerceptionResult in EmoteSpokenLanguageInfo.ParseFor");
            }
        }
    }

    public class PsychicLanguageInfo : LanguageInfo
    {
        public PsychicLanguageInfo(ILanguage language, IAccent accent, string text, Outcome originoutcome, IPerceivable origin, IPerceivable proxy = null, double senderScramble = 0.0) : base(language, text, originoutcome, origin, proxy)
        {
            SenderScramble = senderScramble;
            Accent = accent;
        }

        public IAccent Accent { get; protected set; }

        public double SenderScramble { get; protected set; }

        public override LanguageForm Form => LanguageForm.Psychic;

        public override string ParseFor(ILanguagePerceiver perceiver)
        {
            var result = GetPerceptionResult(perceiver);
            var ratio = Math.Max(SenderScramble, result.LanguageObfuscation);
            var accentPortion = Accent == null ? "" : $" {(perceiver.Accents.Contains(Accent) || ((perceiver as ICharacter)?.IsAdministrator() ?? false) ? Accent.AccentSuffix : Accent.VagueSuffix)}";
            switch (result.Result) {
                case LanguagePerceptionResult.UnknownLanguage:
                    return $" something in {Language.UnknownLanguageSpokenDescription}.";
                case LanguagePerceptionResult.HearFailure:
                    return
                        $" something in {Language.Name.TitleCase()}{accentPortion}, but you cannot understand {Origin.ApparentGender(perceiver).Objective()}.";
                case LanguagePerceptionResult.MutuallyIntelligableLanguage:
                    return $", in {Language.Name.TitleCase()}, \n{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText), ratio).Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                case LanguagePerceptionResult.PartialSuccess:
                    return
                        $", in {Language.Name.TitleCase()}{accentPortion}, \n{perceiver.Gameworld.LanguageScrambler.Scramble(new ExplodedString(_rawText), ratio).Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                case LanguagePerceptionResult.Success:
                    return
                        $", in {Language.Name.TitleCase()}{accentPortion}, \n{_rawText.Fullstop().DoubleQuotes().Wrap(perceiver.InnerLineFormatLength, "   ")}";
                default:
                    throw new ApplicationException("Unknown LanguagePerceptionResult in PsychicLanguageInfo.ParseFor");
            }
        }

        protected (LanguagePerceptionResult Result, double LanguageObfuscation) GetPerceptionResult(ILanguagePerceiver perceiver)
        {
            if (perceiver == null)
            {
                return (LanguagePerceptionResult.Success, 0.0);
            }

            if (perceiver.IsSelf(Origin) || perceiver.IsSelf(Proxy) ||
                ((perceiver as ICharacter)?.IsAdministrator() == true))
            {
                return (LanguagePerceptionResult.Success, 0.0);
            }

            var accentDifficulty = Accent != null ? perceiver.AccentDifficulty(Accent) : Difficulty.Automatic;
            if (OriginOutcome.IsFail())
            {
                accentDifficulty = accentDifficulty.StageUp(1);
            }

            var hearCheckResult = perceiver.Gameworld.GetCheck(CheckType.PsychicLanguageHearCheck)
                .Check(perceiver, accentDifficulty,
                    Language.LinkedTrait, Origin);
            var originalResult = hearCheckResult.Outcome;
            var usedOtherLanguage = false;
            if (hearCheckResult.IsFail() && !perceiver.Languages.Contains(Language))
            {
                foreach (var language in perceiver.Languages.Except(Language)
                    .Where(x => x.MutualIntelligability(Language) != Difficulty.Impossible).ToList())
                {
                    hearCheckResult = perceiver.Gameworld.GetCheck(CheckType.PsychicLanguageHearCheck)
                        .Check(perceiver, (Difficulty)Math.Max((int)accentDifficulty, (int)language.MutualIntelligability(Language)),
                               language.LinkedTrait, Origin);
                    usedOtherLanguage = true;
                    if (hearCheckResult.IsPass())
                    {
                        break;
                    }
                }
            }

            if (hearCheckResult.Outcome == Outcome.MajorFail && !perceiver.Languages.Contains(Language))
            {
                return (LanguagePerceptionResult.UnknownLanguage, 1.0);
            }

            var hearResult = originalResult.CheckDegrees() > hearCheckResult.Outcome.CheckDegrees() ? originalResult : hearCheckResult.Outcome;

            if (hearResult == Outcome.MajorFail)
            {
                return (usedOtherLanguage ? LanguagePerceptionResult.UnknownLanguage : LanguagePerceptionResult.HearFailure, 1.0);
            }

            // Having established that the perceiver can both hear and understand to some extent, we must work out a ratio of word obfuscation
            var languageRatio = Language.LanguageObfuscationFactor *
                                (usedOtherLanguage
                                    ? Math.Min(1, 3 - hearResult.CheckDegrees())
                                    : hearResult.FailureDegrees());
            if (languageRatio >= 1.0)
            {
                return (usedOtherLanguage ? LanguagePerceptionResult.UnknownLanguage : LanguagePerceptionResult.HearFailure, 1.0);
            }

            if (usedOtherLanguage)
            {
                return (LanguagePerceptionResult.MutuallyIntelligableLanguage, languageRatio);
            }

            return languageRatio > 0
                ? (LanguagePerceptionResult.PartialSuccess, languageRatio)
                : (LanguagePerceptionResult.Success, 0.0);
        }
    }
}