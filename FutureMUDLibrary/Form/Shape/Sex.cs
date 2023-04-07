namespace MudSharp.Form.Shape {
    public abstract class Gendering {
        public abstract string Name { get; }

        public abstract Gender Enum { get; }

        /// <summary>
        ///     Subjective gendered pronoun:
        ///     he, she, it
        /// </summary>
        /// <param name="proper">Whether the pronoun is capitalised.</param>
        public abstract string Subjective(bool proper = false);

        /// <summary>
        ///     Objective gendered pronoun:
        ///     him, her, it
        /// </summary>
        /// <param name="proper">Whether the pronoun is capitalised.</param>
        public abstract string Objective(bool proper = false);

        /// <summary>
        ///     Determined posessive gendered pronoun:
        ///     his, her, its
        /// </summary>
        /// <param name="proper">Whether the pronoun is capitalised.</param>
        public abstract string Possessive(bool proper = false);

        /// <summary>
        ///     Possessive gendered pronoun:
        ///     his, hers, its
        /// </summary>
        /// <param name="proper">Whether the pronoun is capitalised.</param>
        public abstract string GeneralPossessive(bool proper = false);

        /// <summary>
        ///     Adjective of the Gender's Class - e.g. male, female, neuter
        /// </summary>
        /// <param name="proper"></param>
        /// <returns></returns>
        public abstract string GenderClass(bool proper = false);

        /// <summary>
        ///     Reflexive gendered pronoun:
        ///     himself, herself, yourself
        /// </summary>
        /// <param name="proper"></param>
        /// <returns></returns>
        public abstract string Reflexive(bool proper = false);

        /// <summary>
        /// Either Is or Are matching with the main pronoun (e.g. he is, they are)
        /// </summary>
        /// <param name="proper"></param>
        /// <returns></returns>
        public virtual string Is(bool proper = false)
        {
            return proper ? "Is" : "is";
        }

        /// <summary>
        /// Either Has or Have, matching with the main pronoun (e.g. he has, they have)
        /// </summary>
        /// <param name="proper"></param>
        /// <returns></returns>
        public virtual string Has(bool proper = false)
        {
            return proper ? "Has" : "has";
        }

        public virtual bool UseThirdPersonVerbForms => false;

        /// <summary>
        ///     Gets a reference to the Sex indicated by the SexEnum argument.
        /// </summary>
        public static Gendering Get(Gender sex) {
            switch (sex) {
                case Gender.Male:
                    return Male.Instance;
                case Gender.Female:
                    return Female.Instance;
                case Gender.Neuter:
                    return Neuter.Instance;
                case Gender.NonBinary:
                    return NonBinary.Instance;
                default:
                    return Indeterminate.Instance;
            }
        }

        public static Gendering Get(string sex) {
            switch (sex.ToLowerInvariant()) {
                case "male":
                    return Male.Instance;
                case "female":
                    return Female.Instance;
                case "neuter":
                    return Neuter.Instance;
                case "non-binary":
                case "nonbinary":
                case "non binary":
                case "nb":
                    return NonBinary.Instance;
                default:
                    return Indeterminate.Instance;
            }
        }
    }

    public class Male : Gendering {
        private Male() {
        }

        public static Male Instance { get; } = new();

        public override string Name => "male";

        public override Gender Enum => Gender.Male;

        public override bool UseThirdPersonVerbForms => true;

        public override string Subjective(bool proper = false) {
            return proper ? "He" : "he";
        }

        public override string Objective(bool proper = false) {
            return proper ? "Him" : "him";
        }

        public override string Possessive(bool proper = false) {
            return proper ? "His" : "his";
        }

        public override string GeneralPossessive(bool proper = false) {
            return proper ? "His" : "his";
        }

        public override string GenderClass(bool proper = false) {
            return proper ? "Male" : "male";
        }

        public override string Reflexive(bool proper = false) {
            return proper ? "Himself" : "himself";
        }
    }

    public class Female : Gendering {
        private Female() {
        }

        public static Female Instance { get; } = new();

        public override string Name => "female";

        public override Gender Enum => Gender.Female;

        public override bool UseThirdPersonVerbForms => true;

        public override string Subjective(bool proper = false) {
            return proper ? "She" : "she";
        }

        public override string Objective(bool proper = false) {
            return proper ? "Her" : "her";
        }

        public override string Possessive(bool proper = false) {
            return proper ? "Her" : "her";
        }

        public override string GeneralPossessive(bool proper = false) {
            return proper ? "Hers" : "hers";
        }

        public override string GenderClass(bool proper = false) {
            return proper ? "Female" : "female";
        }

        public override string Reflexive(bool proper = false) {
            return proper ? "Herself" : "herself";
        }
    }

    public class Neuter : Gendering {
        private Neuter() {
        }

        public static Neuter Instance { get; } = new();

        public override string Name => "neuter";

        public override Gender Enum => Gender.Neuter;

        public override bool UseThirdPersonVerbForms => true;

        public override string Subjective(bool proper = false) {
            return proper ? "It" : "it";
        }

        public override string Objective(bool proper = false) {
            return proper ? "It" : "it";
        }

        public override string Possessive(bool proper = false) {
            return proper ? "Its" : "its";
        }

        public override string GeneralPossessive(bool proper = false) {
            return proper ? "Its" : "its";
        }

        public override string GenderClass(bool proper = false) {
            return proper ? "Neuter" : "neuter";
        }

        public override string Reflexive(bool proper = false) {
            return proper ? "Itself" : "itself";
        }
    }

    public class Indeterminate : Gendering {
        private Indeterminate() {
        }

        public static Indeterminate Instance { get; } = new();

        public override string Name => "indeterminate";

        public override Gender Enum => Gender.Indeterminate;

        public override string Subjective(bool proper = false) {
            return proper ? "They" : "they";
        }

        public override string Objective(bool proper = false) {
            return proper ? "Them" : "them";
        }

        public override string Possessive(bool proper = false) {
            return proper ? "Their" : "their";
        }

        public override string GeneralPossessive(bool proper = false) {
            return proper ? "Theirs" : "theirs";
        }

        public override string GenderClass(bool proper = false) {
            return proper ? "Indeterminately-gendered" : "indeterminately-gendered";
        }

        public override string Reflexive(bool proper = false) {
            return proper ? "Themselves" : "themselves";
        }

        public override string Is(bool proper = false)
        {
            return proper ? "Are" : "are";
        }

        public override string Has(bool proper = false)
        {
            return proper ? "Have" : "have";
        }
    }

    // Using Indeterminate as the base for now, until something points out I am a cishet fascist and need to change these up.
    public class NonBinary : Gendering
    {
        private NonBinary() {
        }

        public static NonBinary Instance { get; } = new();

        public override string Name => "non-binary";

        public override Gender Enum => Gender.NonBinary;

        public override string Subjective(bool proper = false)
        {
            return proper ? "They" : "they";
        }

        public override string Objective(bool proper = false)
        {
            return proper ? "Them" : "them";
        }

        public override string Possessive(bool proper = false)
        {
            return proper ? "Their" : "their";
        }

        public override string GeneralPossessive(bool proper = false)
        {
            return proper ? "Theirs" : "theirs";
        }

        public override string GenderClass(bool proper = false)
        {
            return proper ? "Individual" : "individual";
        }

        public override string Reflexive(bool proper = false)
        {
            return proper ? "Themselves" : "themselves";
        }

        public override string Is(bool proper = false)
        {
            return proper ? "Are" : "are";
        }

        public override string Has(bool proper = false)
        {
            return proper ? "Have" : "have";
        }
    }

}