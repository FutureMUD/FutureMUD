using MudSharp.Character;
using MudSharp.Form.Characteristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace MudSharp.Effects.Interfaces
{
	public interface IChangeCharacteristicEffect : IEffectSubtype
	{
		bool ChangesCharacteristic(ICharacteristicDefinition characteristic);
		ICharacteristicValue GetChangedCharacteristic(ICharacteristicDefinition characteristic);

	}
}
