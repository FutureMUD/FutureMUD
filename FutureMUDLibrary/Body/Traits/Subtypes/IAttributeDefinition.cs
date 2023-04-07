namespace MudSharp.Body.Traits.Subtypes {
    public interface IAttributeDefinition : ITraitDefinition {
        string Alias { get; }
        string ChargenBlurb { get; }

        int DisplayOrder { get; }
        bool DisplayAsSubAttribute { get; }
        bool ShowInScoreCommand { get; }
        bool ShowInAttributeCommand { get; }
    }
}