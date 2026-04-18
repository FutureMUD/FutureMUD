#nullable enable

namespace MudSharp.Framework.Save;

/// <summary>
/// Represents a boot-loaded object that must defer character-dependent resolution until characters can be safely materialised.
/// </summary>
public interface IPostCharacterLoadFinalisable
{
    void FinaliseLoading();
}
