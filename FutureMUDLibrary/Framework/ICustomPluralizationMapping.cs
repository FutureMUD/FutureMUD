namespace MudSharp.Framework
{
    public interface ICustomPluralizationMapping
    {
        //
        // Summary:
        //     Adds singular and plural forms of a word to the System.Data.Entity.Design.PluralizationServices.ICustomPluralizationMapping
        //     object.
        //
        // Parameters:
        //   singular:
        //     The singular version of the word added to the System.Data.Entity.Design.PluralizationServices.ICustomPluralizationMapping
        //     object.
        //
        //   plural:
        //     The plural version of the word added to the System.Data.Entity.Design.PluralizationServices.ICustomPluralizationMapping
        //     object.
        void AddWord(string singular, string plural);
    }
}