using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework.Monitoring;

public class DuplicationMonitor : IHaveFuturemud
{
    public IFuturemud Gameworld { get; set; }

    public DuplicationMonitor(IFuturemud gameworld)
    {
        Gameworld = gameworld;
    }

    public void AuditCharacters()
    {
        using StreamWriter writer = new($"Duplication Audit - {DateTime.UtcNow:yyyyMMMMddhhmmss}");
        Gameworld.SaveManager.Flush();
        foreach (ICharacter actor in Gameworld.Actors)
        {
            List<ICharacter> duplicates = Gameworld.Actors.Where(x => x.Id == actor.Id && !ReferenceEquals(x, actor)).ToList();
            if (duplicates.Any())
            {
                writer.WriteLine("Audit found duplicate Character in Actors list:");
                writer.WriteLine();
                writer.WriteLine("-----------------------Original------------------------------");
                writer.WriteLine(actor.DebugInfo());
                writer.WriteLine(actor.ShowScore(actor));
                writer.WriteLine(actor.ShowHealth(actor));
                foreach (ICharacter duplicate in duplicates)
                {
                    writer.WriteLine("-----------------------Duplicate-----------------------------");
                    writer.WriteLine(duplicate.DebugInfo());
                    writer.WriteLine(duplicate.ShowScore(actor));
                    writer.WriteLine(duplicate.ShowHealth(actor));
                }

                writer.WriteLine("-----------------------End------------------------------");
                writer.WriteLine();
            }

            duplicates = Gameworld.CachedActors.Where(x => x.Id == actor.Id && !ReferenceEquals(x, actor)).ToList();
            if (duplicates.Any())
            {
                writer.WriteLine("Audit found duplicate Character in CachedActors list:");
                writer.WriteLine();
                writer.WriteLine("-----------------------Original------------------------------");
                writer.WriteLine(actor.DebugInfo());
                writer.WriteLine(actor.ShowScore(actor));
                writer.WriteLine(actor.ShowHealth(actor));
                foreach (ICharacter duplicate in duplicates)
                {
                    writer.WriteLine("-----------------------Duplicate-----------------------------");
                    writer.WriteLine(duplicate.DebugInfo());
                    writer.WriteLine(duplicate.ShowScore(actor));
                    writer.WriteLine(duplicate.ShowHealth(actor));
                }

                writer.WriteLine("-----------------------End------------------------------");
                writer.WriteLine();
            }
        }
    }
}