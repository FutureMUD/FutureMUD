using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Form.Characteristics;

public static class CharacteristicDefinitionFactory
{
	public static ICharacteristicDefinition LoadDefinition(MudSharp.Models.CharacteristicDefinition definition,
		IFuturemud gameworld)
	{
		if (definition.ParentId != null)
		{
			switch (definition.Model)
			{
				case "bodypart":
					return new BodypartSpecificClientCharacteristicDefinition(definition, gameworld);
				default:
					return new ClientCharacteristicDefinition(definition, gameworld);
			}
		}

		switch (definition.Model)
		{
			case "standard":
				return new CharacteristicDefinition(definition, gameworld);
			case "bodypart":
				return new BodypartSpecificCharacteristicDefinition(definition, gameworld);
			default:
				return new CharacteristicDefinition(definition, gameworld);
		}
	}
}