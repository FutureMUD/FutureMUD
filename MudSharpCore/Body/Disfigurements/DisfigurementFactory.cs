using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.Disfigurements;

public static class DisfigurementFactory
{
    public static IDisfigurementTemplate LoadTemplate(MudSharp.Models.DisfigurementTemplate template,
        IFuturemud gameworld)
    {
        return template.Type switch
        {
            "Tattoo" => new TattooTemplate(template, gameworld),
            "Scar" => null,
            _ => throw new ApplicationException(
                "Invalid disfigurement template type in DisfigurementFactory.LoadTemplate: " + template.Type)
        };
    }
}
