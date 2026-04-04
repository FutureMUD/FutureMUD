using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Disfigurements
{
    public interface ITattoo : IDisfigurement, IKeyworded
    {
        ITattooTemplate TattooTemplate { get; }
        ICharacter Tattooist { get; }
        double TattooistSkill { get; set; }
        MudDateTime TimeOfInscription { get; set; }
        XElement SaveToXml();
        double CompletionPercentage { get; set; }
        IReadOnlyDictionary<string, ITattooTextValue> TextValues { get; }
        bool HasUnreadableCopyPenalty { get; }
        string ShortDescriptionFor(IPerceiver voyeur);
        string FullDescriptionFor(IPerceiver voyeur);
    }
}
