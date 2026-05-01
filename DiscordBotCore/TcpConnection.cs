using DSharpPlus;
using DSharpPlus.Entities;
using MudSharp.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot;

public class TcpConnection
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _networkStream;
    private static readonly byte[] ByteSeparators = { 1 };
    private readonly List<byte> _incomingBytes = new();
    private readonly DiscordClient _discordClient;
    private readonly DiscordBot _discordBot;
    public bool TcpClientAuthenticated { get; private set; }
    public bool Closing { get; private set; }

    public TcpConnection(TcpClient tcpClient, DiscordClient discordClient, DiscordBot discordBot)
    {
        _tcpClient = tcpClient;
        _networkStream = _tcpClient.GetStream();
        _discordClient = discordClient;
        _discordBot = discordBot;
    }

    public async Task CheckIncomingAsync()
    {
        try
        {
            if (!_networkStream.DataAvailable)
            {
                return;
            }

            byte[] inputBuffer = new byte[4096];
            int bytes = _networkStream.Read(inputBuffer, 0, 4096);
            if (bytes == 0)
            {
                Closing = true;
                _tcpClient?.Close();
                _networkStream?.Close();
                TcpClientAuthenticated = false;
                return;
            }

            _incomingBytes.AddRange(inputBuffer.Take(bytes));
            while (TryReadIncomingCommand(out byte[] command))
            {
                if (command.Length == 0)
                {
                    continue;
                }

                await HandleTcpCommandAsync(Encoding.Unicode.GetString(command));
            }
        }
        catch (SocketException e)
        {
            Closing = true;
            _tcpClient?.Close();
            _networkStream?.Close();
            TcpClientAuthenticated = false;
            Log.Information($"TCP Connection encountered a SocketException: {e.Message}");
        }
#if DEBUG
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
#endif
    }

    private bool TryReadIncomingCommand(out byte[] command)
    {
        int delimiterIndex = _incomingBytes.IndexOf(ByteSeparators[0]);
        if (delimiterIndex == -1)
        {
            command = Array.Empty<byte>();
            return false;
        }

        command = _incomingBytes.GetRange(0, delimiterIndex).ToArray();
        _incomingBytes.RemoveRange(0, delimiterIndex + 1);
        return true;
    }

    private async Task HandleTcpCommandAsync(string getString)
    {

        Console.WriteLine($"TCP Communique: {getString}");
        StringStack ss = new(getString);
        switch (ss.Pop())
        {
            case "request":
                await HandleTcpCommandRequest(ss);
                return;
            case "login":
                await HandleTcpCommandLoginAsync(ss.RemainingArgument);
                return;
            case "shutdown":
                await HandleTcpCommandShutdownAsync(ss.RemainingArgument);
                return;
            case "crash":
                await HandleTcpCommandCrashAsync(ss.RemainingArgument);
                return;
            case "chargen":
                await HandleTcpCommandChargenAsync(ss);
                return;
            case "chargen_approved":
                await HandleTcpCommandChargenApprovedAsync(ss);
                return;
            case "chargen_rejected":
                await HandleTcpCommandChargenRejectedAsync(ss);
                return;
            case "petition":
                await HandleTcpCommandPetitionAsync(ss);
                return;
            case "sendmessage":
                await HandleTcpCommandSendMessageAsync(ss);
                return;
            case "broadcast":
                await HanddleTcpCommandBroadcast(ss.RemainingArgument);
                return;
            case "badecho":
                await HandleTcpCommandBadEchoAsync(ss.RemainingArgument);
                return;
            case "notifyadmins":
                await HandleTcpCommandNotifyAdmins(ss.RemainingArgument);
                return;
            case "notifydeath":
                await HandleTcpCommandNotifyDeath(ss);
                return;
            case "enforcement":
                await HandleTcpCommandEnforcement(ss);
                return;
            case "custom":
                await HandleTcpCommandCustomMessage(ss);
                return;
            case "ingamechannel":
                await HandleTcpCommandInGameChannel(ss);
                return;
            case "progerror":
                await HandleProgError(ss);
                return;
            default:
                return;
        }
    }

    private async Task HandleProgError(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        long progId = long.Parse(ss.Pop());
        string progName = ss.PopSpeech();
        string errorText = ss.SafeRemainingArgument;

        DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle($"Error In Prog #{progId} ({progName})")
                           .AddField("progID", progId.ToString("N0"))
                           .AddField("progName", progName)
                           .WithDescription(errorText);

        DiscordEmbed embed = embedBuilder.Build();
        await _discordBot.AnnounceToDebugChannel(embed);
    }

    private async Task HandleTcpCommandInGameChannel(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        string channelName = ss.PopSpeech();
        ulong discordChannel = ulong.Parse(ss.Pop());
        string author = ss.PopSpeech();
        string text = ss.SafeRemainingArgument;
        DiscordChannel channel = await _discordClient.GetChannelAsync(discordChannel);
        await channel.SendMessageAsync($"**[{channelName}: {author}]** {text}");
    }

    private async Task HandleTcpCommandCustomMessage(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        ulong channelid = ulong.Parse(ss.Pop());
        DiscordChannel channel = await _discordClient.GetChannelAsync(channelid);
        string header = ss.PopSpeech();
        string message = ss.SafeRemainingArgument;
        await channel.SendMessageAsync($"**{header}**\n\n```{message}```");
    }

    private async Task HandleTcpCommandEnforcement(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        string subtype = ss.Pop();
        ulong channelid = ulong.Parse(ss.Pop());
        DiscordChannel channel = await _discordClient.GetChannelAsync(channelid);
        string authority = ss.PopSpeech();
        string criminalid = ss.Pop();
        string criminalname = ss.PopSpeech();
        switch (subtype)
        {
            case "enforcement":
                string patrol = ss.PopSpeech();
                string crime = ss.PopSpeech();
                string location = ss.PopSpeech();
                string action = ss.PopSpeech();
                await channel.SendMessageAsync($"Enforcement Action in **{authority}** at **{location}**:\n\tThe **{patrol}** patrol is enforcing **{crime}** against **{criminalname}** ({criminalid}) - *{action}*");
                return;
            case "conviction":
                crime = ss.PopSpeech();
                string result = ss.PopSpeech();
                await channel.SendMessageAsync($"Conviction Recorded in **{authority}**:\n\t**{criminalname}** ({criminalid}) was convicted of **{crime}** with a punishment of **{result}**");
                return;
            case "returnfrombail":
                await channel.SendMessageAsync($"**{criminalname}** ({criminalid}) has returned from bail in **{authority}**");
                return;
            case "release":
                await channel.SendMessageAsync($"**{criminalname}** ({criminalid}) has been released from custody in **{authority}**");
                return;
            case "imprisonment":
                string length = ss.PopSpeech();
                await channel.SendMessageAsync($"**{criminalname}** ({criminalid}) has been sentenced to **{length}** imprisonment in **{authority}**");
                return;
            case "incarceration":
                await channel.SendMessageAsync($"**{criminalname}** ({criminalid}) has been incarcerated awaiting trial in **{authority}**");
                return;
        }
    }

    private async Task HandleTcpCommandRequest(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        if (!ulong.TryParse(ss.Pop(), out ulong request))
        {
            Log.Warning("Received malformed Discord TCP request response without a valid request id.");
            return;
        }

        if (!_discordBot.CachedDiscordRequests.TryRemove(request, out CachedDiscordRequest cachedRequest))
        {
            Log.Warning("Received Discord TCP response for unknown request id {RequestId}.", request);
            return;
        }

        await cachedRequest.OnResponseAction(ss.RemainingArgument, cachedRequest.Context);
    }

    private async Task HandleTcpCommandNotifyDeath(StringStack ss)
    {
        long whoId = long.Parse(ss.Pop());
        string accountName = ss.Pop();
        long whereId = long.Parse(ss.Pop());
        string roomLayer = ss.PopSpeech();
        string where = ss.PopSpeech();
        string whoDesc = ss.PopSpeech();
        string whoName = ss.PopSpeech();

        if (!TcpClientAuthenticated)
        {
            return;
        }

        DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle("Player Character Death")
                           .WithDescription($"**{accountName}**'s character \"**{whoName}**\" (**{whoDesc}**, #{whoId:N}) has died at **{where}** ({roomLayer}, {whereId:N})");

        DiscordEmbed embed = embedBuilder.Build();

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandNotifyAdmins(string ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Admin Notification")
                    .WithDescription($"Engine sent admin notification:\n```{ss}```")
                    .Build()
            ;

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandBadEchoAsync(string remainingArgument)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Bad Echo")
                    .WithDescription($"Perception engine reported a bad echo:\n```{remainingArgument}```")
                    .Build()
            ;

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HanddleTcpCommandBroadcast(string remainingArgument)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("System Broadcast")
                    .WithDescription($"```[System Message] {remainingArgument}```")
                    .Build()
            ;

        await _discordBot.AnnounceToDiscord(embed);
    }

    private async Task HandleTcpCommandSendMessageAsync(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        ulong channelId = ulong.Parse(ss.Pop());
        string title = ss.PopSpeech();
        string message = ss.RemainingArgument[..Math.Min(ss.RemainingArgument.Length, 2048)];
        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle(title)
                    .WithDescription(message)
                    .Build();

        await _discordBot.AnnounceToChannel(channelId, embed);
    }

    private async Task HandleTcpCommandChargenRejectedAsync(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }
        string account = ss.PopSpeech();
        string approver = ss.PopSpeech();
        string name = ss.RemainingArgument;
        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Character Rejected")
                    .WithDescription($"Account \"{account}\" had application \"{name}\" rejected by \"{approver}\".")
                    .Build()
            ;

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandChargenApprovedAsync(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }
        string account = ss.PopSpeech();
        string approver = ss.PopSpeech();
        string name = ss.RemainingArgument;
        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Character Approved")
                    .WithDescription($"Account \"{account}\" had application \"{name}\" approved by \"{approver}\".")
                    .Build()
            ;

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandChargenAsync(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        string account = ss.Pop();
        long id = long.Parse(ss.Pop());
        string name = ss.RemainingArgument;


        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Character Submission")
                    .WithDescription($"Account \"{account}\" has submitted a character \"{name}\" (ID #{id}), which is now awaiting approval.")
                    .Build()
            ;

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandPetitionAsync(StringStack ss)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        string who = ss.Pop();
        string where = ss.PopSpeech();
        string message = ss.RemainingArgument[..Math.Min(ss.RemainingArgument.Length, 1900)];

        DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                      .WithTitle("Petition")
                      .WithDescription($"Mud Petition from {who.Proper()}\n\n```{message}```");
        builder.AddField("Location", where);

        DiscordEmbed embed = builder.Build();

        await _discordBot.AnnounceToAdminChannel(embed);
    }

    private async Task HandleTcpCommandCrashAsync(string ssRemainingArgument)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        DiscordEmbed embed = new DiscordEmbedBuilder()
                    .WithTitle("Crash Info")
                    .WithDescription($"```{ssRemainingArgument[..Math.Min(ssRemainingArgument.Length, 2040)]}```")
                    .Build()
            ;
        await _discordBot.AnnounceToDebugChannel(embed);
        await _discordBot.AnnounceToDiscord($"Unfortunately, **{_discordBot.GameName}** has crashed. It should automatically reboot within the next few minutes.");
    }

    private async Task HandleTcpCommandShutdownAsync(string shutdownAccount)
    {
        if (!TcpClientAuthenticated)
        {
            return;
        }

        (string account, bool reboot) = DiscordBotProtocol.ParseShutdownNotification(shutdownAccount);
        Closing = true;
        _tcpClient?.Close();
        _networkStream?.Close();
        TcpClientAuthenticated = false;
        if (reboot)
        {
            await _discordBot.AnnounceToDiscord($"{_discordBot.GameName} has been shutdown by {account}. It should be back in a couple of minutes.");
            return;
        }

        await _discordBot.AnnounceToDiscord($"{_discordBot.GameName} has been shutdown by {account}, and it has been told not to reboot automatically.");
    }

    private async Task HandleTcpCommandLoginAsync(string getString)
    {
        StringStack ss = new(getString);
        string auth = ss.Pop();
        if (auth.EqualTo(_discordBot.ServerAuth))
        {
            TcpClientAuthenticated = true;
            _tcpClient.Client.Send(DiscordBotProtocol.EncodeCommandForEngine("authsuccess"));
            await _discordBot.AnnounceToDiscord($"I have successfully heard from {_discordBot.GameName}, and I'm now ready to report on what it's up to!");
            if (!ss.IsFinished)
            {
                await _discordBot.CheckVersionInfo(ss.RemainingArgument);
            }
        }
        else
        {
            _tcpClient.Client.Send(DiscordBotProtocol.EncodeCommandForEngine("authfailure"));
        }
    }

    public async Task SendTcpCommand(string command)
    {
        try
        {
            byte[] bytes = DiscordBotProtocol.EncodeCommandForEngine(command);
            await _tcpClient.Client.SendAsync(bytes, SocketFlags.None);
        }
        catch (SocketException e)
        {
            Closing = true;
            _tcpClient?.Close();
            _networkStream?.Close();
            TcpClientAuthenticated = false;
            Log.Information($"TCP Connection encountered a SocketException: {e.Message}");
        }
    }
}
