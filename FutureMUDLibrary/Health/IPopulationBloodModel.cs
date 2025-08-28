using System.Collections.Generic;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Health;

public interface IPopulationBloodModel : IEditableItem
{
    IBloodtype GetBloodType(ICharacterTemplate character);
    IEnumerable<(IBloodtype Bloodtype, double Weight)> BloodTypes { get; }
    IBloodModel? BloodModel { get; }
}

