using System;
using MudSharp.Character;

namespace MudSharp.Discord
{
    public interface IDiscordConnection : IDisposable
    {
        void SendMessageFromProg(ulong channel, string title, string message);
        void NotifyPetition(string account, string message, string location);
        void NotifyCharacterSubmission(string account, string name, long id);
        void NotifyShutdown(string shutdownAccount);
        void NotifyCharacterApproval(string account, string name, string approver);
        void NotifyCharacterRejection(string account, string name, string reviewer);
        void NotifyCrash(string crashMessage);
        void NotifyBadEcho(string badEcho);
        void NotifyAdmins(string echo);
        void NotifyDeath(ICharacter who);
        void NotifyCustomChannel(ulong channel, string header, string echo);
        void NotifyInGameChannelUsed(string channel, ulong discordChannel, string author, string text);
        bool OpenTcpConnection();
        void CloseTcpConnection();
        void HandleMessages();
        void HandleBroadcast(string message);
        void NotifyEnforcement(string subtype, ulong discordChannel, string otherText);
    }
}