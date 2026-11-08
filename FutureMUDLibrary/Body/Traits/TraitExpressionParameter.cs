namespace MudSharp.Body.Traits
{
    public class TraitExpressionParameter
    {
        public ITraitDefinition Trait { get; set; }
        /// <summary>Reads the evaluation target instead of the owner; never participates in learning.</summary>
        public bool IsTarget { get; set; }
        /// <summary>Reads the stored trait value without merits, effects or equipment modifiers.</summary>
        public bool UseRawValue { get; set; }
        public bool CanImprove { get; set; }
        public bool CanBranch { get; set; }
    }
}
