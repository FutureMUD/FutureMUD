#nullable enable
using MudSharp.Communication.Language;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;

namespace MudSharp.Construction.Grids;

public interface ITelephoneCall
{
    string Number { get; }
    ITelephone Caller { get; }
    IReadOnlyCollection<ITelephone> Participants { get; }
    IReadOnlyCollection<ITelephone> RingingPhones { get; }
    bool IsConnected { get; }
    bool IsRinging { get; }
    void RelayTransmission(ITelephone source, SpokenLanguageInfo spokenLanguage);
    void RelayDigits(ITelephone source, string digits);
}
