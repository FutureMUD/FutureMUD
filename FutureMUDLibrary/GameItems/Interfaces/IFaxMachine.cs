#nullable enable
using MudSharp.Character;
using MudSharp.Communication;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IFaxMachine : ITelephone
{
    bool CanReceiveFaxes { get; }
    bool CanSendFax(ICharacter actor, string number, IReadable document, out string error);
    bool SendFax(ICharacter actor, string number, IReadable document, out string error);
    void ReceiveFax(string senderNumber, IReadOnlyCollection<ICanBeRead> document);
}
