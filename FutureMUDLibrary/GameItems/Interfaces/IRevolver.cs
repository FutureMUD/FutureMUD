using System.Collections.Generic;
using MudSharp.GameItems;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public interface IRevolver : IRangedWeapon, IOpenable
{
    int Chambers { get; }
    int CurrentChamber { get; }
    IEnumerable<IGameItem> CylinderContents { get; }
    bool CanLoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
    string WhyCannotLoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
    void LoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);
    void Spin(int positions);
    void Spin();
}
