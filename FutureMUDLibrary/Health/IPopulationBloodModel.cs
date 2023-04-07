using MudSharp.CharacterCreation;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health
{
    public interface IPopulationBloodModel : IFrameworkItem
    {
        IBloodtype GetBloodType(ICharacterTemplate character);
        IBloodModel BloodModel {get;}
    }
}
