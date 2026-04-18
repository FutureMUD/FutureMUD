#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static bool HasMissingAnimalDisfigurementTemplates(FuturemudDatabaseContext context)
    {
        return RaceTemplates.Values.Any(template =>
            SeederDisfigurementTemplateUtilities.HasMissingDefinitions(
                context,
                template.TattooTemplates,
                template.ScarTemplates));
    }

    private void SeedAnimalDisfigurementTemplates(
        IEnumerable<AnimalRaceTemplate> templates,
        IReadOnlyDictionary<string, BodyProto> bodyLookup)
    {
        foreach (AnimalRaceTemplate template in templates)
        {
            if (!HasDisfigurementDefinitions(template))
            {
                continue;
            }

            if (!bodyLookup.TryGetValue(template.BodyKey, out BodyProto? body))
            {
                throw new InvalidOperationException(
                    $"Could not resolve body key {template.BodyKey} while seeding disfigurements for animal race {template.Name}.");
            }

            SeederDisfigurementTemplateUtilities.SeedTemplates(
                _context,
                body,
                template.TattooTemplates,
                template.ScarTemplates);
        }
    }

    private void SeedExistingAnimalDisfigurementTemplates()
    {
        SeedAnimalDisfigurementTemplates(RaceTemplates.Values, BuildExistingAnimalDisfigurementBodyLookup());
    }

    private Dictionary<string, BodyProto> BuildExistingAnimalDisfigurementBodyLookup()
    {
        Dictionary<string, BodyProto> lookup = new(StringComparer.OrdinalIgnoreCase);

        void AddBody(string key, string bodyName)
        {
            BodyProto? body = _context.BodyProtos.FirstOrDefault(x => x.Name == bodyName);
            if (body is not null)
            {
                lookup[key] = body;
            }
        }

        AddBody("Ungulate", "Ungulate");
        AddBody("Toed Quadruped", "Toed Quadruped");
        AddBody("Avian", "Avian");
        AddBody("Serpentine", "Serpentine");
        AddBody("Piscine", "Piscine");
        AddBody("Decapod", "Decapod");
        AddBody("Malacostracan", "Malacostracan");
        AddBody("Cephalopod", "Cephalopod");
        AddBody("Jellyfish", "Jellyfish");
        AddBody("Pinniped", "Pinniped");
        AddBody("Cetacean", "Cetacean");
        AddBody("Vermiform", "Vermiform");
        AddBody("Insectoid", "Insectoid");
        AddBody("Winged Insectoid", "Winged Insectoid");
        AddBody("Arachnid", "Arachnid");
        AddBody("Scorpion", "Scorpion");
        AddBody("Reptilian", "Reptilian");
        AddBody("Anuran", "Anuran");

        return lookup;
    }

    private static bool HasDisfigurementDefinitions(AnimalRaceTemplate template)
    {
        return template.TattooTemplates?.Any() == true || template.ScarTemplates?.Any() == true;
    }
}
