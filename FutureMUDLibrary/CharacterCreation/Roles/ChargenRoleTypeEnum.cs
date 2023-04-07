namespace MudSharp.CharacterCreation.Roles {
    public enum ChargenRoleType {
        /// <summary>
        ///     The ChargenRole is a Class selection. Will appear in SCORE
        /// </summary>
        Class,

        /// <summary>
        ///     The ChargenRole is a Subclass selection. Will appear in SCORE
        /// </summary>
        Subclass,

        /// <summary>
        ///     The ChargenRole is a Professional selection
        /// </summary>
        Profession,

        /// <summary>
        ///     The ChargenRole is a family role
        /// </summary>
        Family,

        /// <summary>
        ///     The ChargenRole is a story/plot role
        /// </summary>
        Story,

        /// <summary>
        ///     The ChargenRole has to do with a starting location
        /// </summary>
        StartingLocation,

        Childhood,

        Education,

        ImmediatePast
    }
}