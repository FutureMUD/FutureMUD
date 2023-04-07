namespace MudSharp.Framework
{
    public interface IPluralizationService
    {
        //
        // Summary:
        //     Pluralize a word using the service.
        //
        // Parameters:
        //   word:
        //     The word to pluralize.
        //
        // Returns:
        //     The pluralized word
        string Pluralize(string word);

        //
        // Summary:
        //     Singularize a word using the service.
        //
        // Parameters:
        //   word:
        //     The word to singularize.
        //
        // Returns:
        //     The singularized word.
        string Singularize(string word);
    }
}