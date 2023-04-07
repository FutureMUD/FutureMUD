namespace MudSharp.Framework
{
    public interface IFutureMUDPluralizationService : IPluralizationService {
        bool IsPlural(string word);
        bool IsSingular(string word);
    }
}