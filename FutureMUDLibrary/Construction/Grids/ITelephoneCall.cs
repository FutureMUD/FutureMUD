#nullable enable
using System.Collections.Generic;
using MudSharp.Communication.Language;
using MudSharp.GameItems.Interfaces;

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
}
