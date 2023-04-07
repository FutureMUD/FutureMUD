using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Body.Disfigurements;

public static class DisfigurementFactory
{
	public static IDisfigurementTemplate LoadTemplate(MudSharp.Models.DisfigurementTemplate template,
		IFuturemud gameworld)
	{
		switch (template.Type)
		{
			case "Tattoo":
				return new TattooTemplate(template, gameworld);
		}

		throw new ApplicationException("Invalid disfigurement template type in DisfigurementFactory.LoadTemplate: " +
		                               template.Type);
	}
}