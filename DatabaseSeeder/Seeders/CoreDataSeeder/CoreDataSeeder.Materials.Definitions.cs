#nullable enable

using Humanizer;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System;
using TimeOfDay = MudSharp.Celestial.TimeOfDay;
using TimeZoneInfo = MudSharp.Models.TimeZoneInfo;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
    internal class ResidueInformation
    {
        public ResidueInformation(string sdesc, string desc, string colour = "white", string? solvent = null,
            double ratio = 1.0)
        {
            ResidueSdesc = sdesc;
            ResidueDesc = desc;
            ResidueColour = colour;
            Solvent = solvent;
            SolventRatio = ratio;
        }

        public string ResidueSdesc { get; set; }
        public string ResidueDesc { get; set; }
        public string ResidueColour { get; set; }
        public string? Solvent { get; set; }
        public double SolventRatio { get; set; }
    }
}
