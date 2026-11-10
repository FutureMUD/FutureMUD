using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudSharp.Framework;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Bot;

public partial class DiscordBot
{
    private static DiscordBot _instance;

    public static DiscordBot Instance => _instance ??= new DiscordBot();

    private readonly List<TcpConnection> _tcpConnections = new();
    public IEnumerable<TcpConnection> TCPConnections => _tcpConnections;
    private readonly CachedDiscordRequestCache _cachedDiscordRequests = new();
    public CachedDiscordRequestCache CachedDiscordRequests => _cachedDiscordRequests;

    private async Task TcpListenerAsync()
    {
        TcpListener tcp = new(IPAddress.Parse(_listenAddress), _tcpClientPort);
        tcp.Start();
        while (true)
        {
            await CachedDiscordRequests.SweepExpiredAsync();
            foreach (TcpConnection conn in _tcpConnections.Where(x => !x.Closing).ToList())
            {
                await conn.CheckIncomingAsync();
            }

            if (tcp.Pending())
            {
                try
                {
                    TcpClient newClient = await tcp.AcceptTcpClientAsync();
                    _tcpConnections.Add(new TcpConnection(newClient, _client, this));
                }
                catch (SocketException)
                {
                }
            }

            foreach (TcpConnection conn in _tcpConnections.Where(x => x.Closing).ToList())
            {
                _tcpConnections.Remove(conn);
            }

            await Task.Delay(100);
        }
    }

    public DiscordBot()
    {
        using (StreamReader reader = new(new FileStream("settings.json", FileMode.Open)))
        {
            try
            {
                if (!DiscordBotSettingsStore.TryLoadSettings(reader.ReadToEnd(), out var settings))
                {
                    Console.WriteLine("Discord bot settings.json is malformed.");
                }
                else
                {
                    _tcpClientPort = settings.Port;
                    _listenAddress = settings.ListenAddress;
                    _botToken = settings.Token;
                    _botPrefixes.AddRange(settings.Prefixes);
                    ServerAuth = settings.ServerAuth;
                    _announceChannelId = settings.AnnounceChannelId;
                    _adminAnnounceChannelId = settings.AdminAnnounceChannelId;
                    _debugAnnounceChannelId = settings.DebugAnnounceChannelId;
                    GameName = settings.GameName;
                    _authorisedUsers.AddRange(settings.AdminUsers);
                    _ownerUsers.AddRange(settings.OwnerUserIds);
                    _customGlobalReactions.AddRange(settings.CustomGlobalReactions);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Discord bot settings.json could not be read.");
            }
        }

        using (StreamReader reader = new(new FileStream("accountlinks.data", FileMode.OpenOrCreate)))
        {
            try
            {
                List<string> lines = new();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                DiscordBotAccountLinkLoadResult result = DiscordBotSettingsStore.LoadAccountLinks(lines);
                DetailedUserSettings.AddRange(result.Settings);
                if (result.InvalidLineCount > 0)
                {
                    Console.WriteLine($"Ignored {result.InvalidLineCount:N0} malformed Discord account-link entries.");
                }
            }
            catch (Exception)
            {
            }
        }
        using (StreamReader reader = new(new FileStream("lastversion.config", FileMode.OpenOrCreate)))
        {
            try
            {
                _lastVersion = reader.ReadLine();
            }
            catch (Exception)
            {

            }
        }
    }

    private readonly List<CustomGlobalReaction> _customGlobalReactions = new();
    private int _tcpClientPort;
    private string _listenAddress = IPAddress.Loopback.ToString();
    private string _botToken;
    private readonly List<string> _botPrefixes = new();
    public string ServerAuth { get; private set; }
    private ulong _announceChannelId;
    private ulong _adminAnnounceChannelId;
    private ulong _debugAnnounceChannelId;
    private string _lastVersion;
    private DiscordClient _client;
    private CommandsNextExtension _commands;
    private SlashCommandsExtension _slashCommands;
    private IServiceProvider _services;
    public string GameName { get; private set; }
    public DiscordChannel AnnounceChannel { get; private set; }
    public DiscordChannel AdminAnnounceChannel { get; private set; }
    public DiscordChannel DebugAnnounceChannel { get; private set; }
    public List<DetailedUserSetting> DetailedUserSettings { get; } = new List<DetailedUserSetting>();
    public CustomCommandHandler CustomCommandHandler { get; private set; }

    private readonly List<ulong> _authorisedUsers = new();
    private readonly List<ulong> _ownerUsers = new();
    private readonly SemaphoreSlim _botSettingsGate = new(1, 1);
    private readonly SemaphoreSlim _accountLinksGate = new(1, 1);
    private readonly DiscordBotAtomicFileWriter _atomicFileWriter = new();

    public bool IsAuthorisedUser(DiscordUser user)
    {
        return _authorisedUsers.Contains(user.Id);
    }

    public bool IsOwner(DiscordUser user)
    {
        return DiscordRemoteCommandPolicy.IsOwner(user.Id, _ownerUsers, _authorisedUsers);
    }

    public bool TryGetAuthenticatedConnection(out TcpConnection connection)
    {
        connection = _tcpConnections.FirstOrDefault(x => x.TcpClientAuthenticated && !x.Closing);
        return connection != null;
    }

    internal async Task<bool> TryCacheDiscordRequestAsync(CachedDiscordRequest request)
    {
        if (await CachedDiscordRequests.TryAddRequestAsync(request))
        {
            return true;
        }

        await SendPrivateCommandResponseAsync(request.Context,
            "There are too many outstanding bot requests. Please try again shortly.");
        return false;
    }

    public async Task RunBotAsync()
    {
        Log.Logger = new LoggerConfiguration()
                     .WriteTo.Console()
                     .CreateLogger();
        ILoggerFactory logFactory = new LoggerFactory().AddSerilog();

        _client = new DiscordClient(new DiscordConfiguration
        {
            Token = _botToken,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.DirectMessageReactions |
                      DiscordIntents.MessageContents |
                      DiscordIntents.DirectMessages |
                      DiscordIntents.GuildMessages |
                      DiscordIntents.GuildMessageReactions |
                      DiscordIntents.Guilds,
            LoggerFactory = logFactory,
#if DEBUG
            MinimumLogLevel = LogLevel.Debug,
#endif
        });

        _services = new ServiceCollection()
                    .AddSingleton(_client)
                    .BuildServiceProvider();

        _commands = _client.UseCommandsNext(new CommandsNextConfiguration
        {
            CaseSensitive = false,
            Services = _services,
            EnableDms = true,
            EnableMentionPrefix = true,
            StringPrefixes = _botPrefixes,
            EnableDefaultHelp = false,
            UseDefaultCommandHandler = false
        });

        _commands.RegisterCommands(Assembly.GetExecutingAssembly());
        _slashCommands = _client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = _services
        });
        _slashCommands.RegisterCommands(Assembly.GetExecutingAssembly());
        _client.ComponentInteractionCreated += RemoteCommandComponents.HandleAsync;
        CustomCommandHandler = new CustomCommandHandler
        {
            StringPrefixes = _botPrefixes,
            CustomBindings = new List<(string Phrase, Command Binding)>
            {
                ("I love you", _commands.RegisteredCommands["love"]),
                ("<3", _commands.RegisteredCommands["love"]),
                ("I <3 you", _commands.RegisteredCommands["love"]),
                ("I love u", _commands.RegisteredCommands["love"]),
                ("I <3 u", _commands.RegisteredCommands["love"]),
                ("luf", _commands.RegisteredCommands["love"]),
                ("lov", _commands.RegisteredCommands["love"]),
                ("luv", _commands.RegisteredCommands["love"]),
                ("I luf you", _commands.RegisteredCommands["love"]),
                ("I luf u", _commands.RegisteredCommands["love"]),
                ("I luv you", _commands.RegisteredCommands["love"]),
                ("I luv u", _commands.RegisteredCommands["love"]),
                ("ich liebe dich", _commands.RegisteredCommands["love"]),
                ("te amo", _commands.RegisteredCommands["love"]),
                ("wo ai ni", _commands.RegisteredCommands["love"]),
                ("我愛你", _commands.RegisteredCommands["love"]),
                ("i wub u", _commands.RegisteredCommands["love"]),
                ("I wuv u", _commands.RegisteredCommands["love"]),
                ("I wuv you", _commands.RegisteredCommands["love"]),
                ("I wub you", _commands.RegisteredCommands["love"]),
                ("thank", _commands.RegisteredCommands["thanks"]),
                ("thank you", _commands.RegisteredCommands["thanks"]),
                ("ty", _commands.RegisteredCommands["thanks"]),
                ("thanks!", _commands.RegisteredCommands["thanks"]),
                ("good bot", _commands.RegisteredCommands["thanks"]),
                ("ty bot", _commands.RegisteredCommands["thanks"]),
                ("thanks bot", _commands.RegisteredCommands["thanks"]),
                ("thank you bot", _commands.RegisteredCommands["thanks"]),
                ("thankyou", _commands.RegisteredCommands["thanks"]),
                ("thankyou bot", _commands.RegisteredCommands["thanks"]),
                ("gud bot", _commands.RegisteredCommands["thanks"]),
                ("danke", _commands.RegisteredCommands["thanks"]),
                ("danke bot", _commands.RegisteredCommands["thanks"]),
                ("bedankt", _commands.RegisteredCommands["thanks"]),
                ("bedankt bot", _commands.RegisteredCommands["thanks"]),
                ("merci", _commands.RegisteredCommands["thanks"]),
                ("merci bot", _commands.RegisteredCommands["thanks"])
            }
        };

        _client.MessageCreated += CustomCommandHandler.HandleCommandAsync;
        _commands.CommandErrored += Commands_CommandErrored;

        _client.UseInteractivity(new InteractivityConfiguration
        {
            PollBehaviour = PollBehaviour.DeleteEmojis,
            Timeout = TimeSpan.FromMinutes(5)
        });

        _client.Ready += ClientOnReady;
        await _client.ConnectAsync();
        await TcpListenerAsync();
        await Task.Delay(-1);
    }

    private async Task ClientOnReady(DiscordClient sender, ReadyEventArgs e)
    {
        AnnounceChannel = await _client.GetChannelAsync(_announceChannelId);
        AdminAnnounceChannel = await _client.GetChannelAsync(_adminAnnounceChannelId);
        DebugAnnounceChannel = await _client.GetChannelAsync(_debugAnnounceChannelId);
    }

    public async Task AnnounceToDiscord(string message)
    {
        try
        {
            if (AnnounceChannel != null)
            {
                await AnnounceChannel.SendMessageAsync(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing messages: " + e);
        }
    }

    public async Task AnnounceToDiscord(DiscordEmbed embed)
    {
        try
        {
            if (AnnounceChannel != null)
            {
                await AnnounceChannel.SendMessageAsync(embed);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing admin messages: " + e);
        }
    }

    private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        DiscordEmbedBuilder embed;
        // let's check if the error is a result of lack
        // of required permissions
        if (e.Exception is ChecksFailedException ex)
        {
            // yes, the user lacks required permissions, 
            // let them know

            DiscordEmoji emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
            await e.Context.Message.CreateReactionAsync(emoji);
            // let's wrap the response into an embed
            embed = new DiscordEmbedBuilder
            {
                Title = "Access denied",
                Description = $"{emoji} You do not have the permissions required to execute this command.",
                Color = new DiscordColor(0xFF0000) // red
            };
            await e.Context.RespondAsync("", embed: embed);
        }
        else if (e.Exception is CommandNotFoundException cnex)
        {
            DiscordEmoji emoji = DiscordEmoji.FromUnicode("⚠️");
            await e.Context.Message.CreateReactionAsync(emoji);
            // let's wrap the response into an embed
            embed = new DiscordEmbedBuilder
            {
                Title = "Command Not Found",
                Description = $"{emoji} That is not a valid command. Please see the HELP command for a list of commands that you can use.",
                Color = DiscordColor.Yellow
            };
            await e.Context.RespondAsync("", embed: embed);
        }
        else if (e.Exception is ArgumentException arge)
        {
            DiscordEmoji emoji = DiscordEmoji.FromUnicode("⚠️");
            await e.Context.Message.CreateReactionAsync(emoji);
            if (arge.ParamName == null)
            {
                string argument =
                    $"{e.Command.Name} {e.Command.Overloads.First().Arguments.Select(x => x.IsOptional ? $"[<{x.Name}>]" : $"<{x.Name}>").ListToCommaSeparatedValues(" ")}";
                embed = new DiscordEmbedBuilder
                {
                    Title = $"Problem with {e.Command.Name} command",
                    Description = $"{emoji} {arge.Message}\n\nThe correct usage of the **{e.Command.Name}** command is:\n\n`{argument}`",
                    Color = DiscordColor.Yellow
                };
            }
            else
            {
                embed = new DiscordEmbedBuilder
                {
                    Title = "Invalid Arguments",
                    Description = $"{emoji} {arge.Message}",
                    Color = DiscordColor.Yellow
                };
            }

            await e.Context.RespondAsync("", embed: embed);
        }
    }

    public async Task CheckVersionInfo(string newVersionInfo)
    {
        if (!string.Equals(newVersionInfo, _lastVersion))
        {
            _lastVersion = newVersionInfo;
            await using (StreamWriter writer = new(new FileStream("lastversion.config", FileMode.Create)))
            {
                await writer.WriteLineAsync(newVersionInfo);
            }
            await AnnounceToDiscord($"Version **{newVersionInfo}** is now live.");
        }
    }

    public Task<string> GetMudStatusAsync()
    {
        if (!TCPConnections.Any(x => x.TcpClientAuthenticated && !x.Closing))
        {
            return Task.FromResult($"I'm not currently connected to **{GameName}**. Probably safe to presume it's down.");
        }

        return Task.FromResult($"I am currently connected to **{GameName}**.");
    }

    public async Task AddAuthorisedUser(ulong who)
    {
		await _botSettingsGate.WaitAsync();
		try
		{
			var added = false;
			if (!_authorisedUsers.Contains(who))
			{
				_authorisedUsers.Add(who);
				added = true;
			}
			try
			{
				await WriteBotSettingsAsync();
			}
			catch
			{
				if (added)
				{
					_authorisedUsers.Remove(who);
				}
				throw;
			}
		}
		finally
		{
			_botSettingsGate.Release();
		}
    }

	internal async Task<bool> RemoveAuthorisedUserAsync(ulong who)
	{
		await _botSettingsGate.WaitAsync();
		try
		{
			_authorisedUsers.Remove(who);
			_ownerUsers.Remove(who);
			await WriteBotSettingsAsync();
			return true;
		}
		catch (Exception e)
		{
			Log.Warning("Could not persist bot administrator revocation for {DiscordUserId} ({ExceptionType}).", who, e.GetType().Name);
			return false;
		}
		finally
		{
			_botSettingsGate.Release();
		}
	}

	private Task WriteBotSettingsAsync()
	{
		var settings = new DiscordBotSetttings
		{
			Port = _tcpClientPort,
			ListenAddress = _listenAddress,
			Token = _botToken,
			Prefixes = _botPrefixes.ToList(),
			ServerAuth = ServerAuth,
			AnnounceChannelId = _announceChannelId,
			AdminAnnounceChannelId = _adminAnnounceChannelId,
			DebugAnnounceChannelId = _debugAnnounceChannelId,
			GameName = GameName,
			AdminUsers = _authorisedUsers.ToList(),
			OwnerUserIds = _ownerUsers.ToList(),
			CustomGlobalReactions = _customGlobalReactions.ToList()
		};
		return _atomicFileWriter.WriteAsync("settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
	}

    public async Task AnnounceToAdminChannel(string message)
    {
        try
        {
            if (AdminAnnounceChannel != null)
            {
                await AdminAnnounceChannel.SendMessageAsync(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing admin messages: " + e);
        }
    }

    public async Task AnnounceToAdminChannel(DiscordEmbed embed)
    {
        try
        {
            if (AdminAnnounceChannel != null)
            {
                await AdminAnnounceChannel.SendMessageAsync(embed);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing admin messages: " + e);
        }
    }

    public async Task AnnounceToDebugChannel(string message)
    {
        try
        {
            if (DebugAnnounceChannel != null)
            {
                await DebugAnnounceChannel.SendMessageAsync(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing debug messages: " + e);
        }
    }

    public async Task AnnounceToDebugChannel(DiscordEmbed embed)
    {
        try
        {
            if (DebugAnnounceChannel != null)
            {
                await DebugAnnounceChannel.SendMessageAsync(embed);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing debug messages: " + e);
        }
    }

    public async Task AnnounceToChannel(ulong channelId, DiscordEmbed embed)
    {
        try
        {
            DiscordChannel channel = await _client.GetChannelAsync(channelId);
            if (channel != null)
            {
                await channel.SendMessageAsync(embed);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Had an exception in announcing admin messages: " + e);
        }
    }

    public async Task AskMudToShutdown(long userid, bool reboot)
    {
        if (!TryGetAuthenticatedConnection(out TcpConnection connection))
        {
            return;
        }

        await connection.SendTcpCommand($"shutdown {userid} {reboot}");
    }

    public async Task AskMudToBroadcast(long accountId, string message)
    {
        if (!TryGetAuthenticatedConnection(out TcpConnection connection))
        {
            return;
        }

        await connection.SendTcpCommand($"broadcast {accountId} {message}");
    }

    public async Task<bool> ReplaceAccountLinkAsync(DetailedUserSetting setting)
    {
		await _accountLinksGate.WaitAsync();
		try
		{
			DetailedUserSettings.RemoveAll(x => x.MudAccountId == setting.MudAccountId || x.DiscordUserId == setting.DiscordUserId);
			DetailedUserSettings.Add(setting);
			await WriteRegistrationConfigAsync();
			return true;
		}
		catch (Exception e)
		{
			Log.Warning("Could not persist Discord account link for {DiscordUserId} ({ExceptionType}).", setting.DiscordUserId, e.GetType().Name);
			return false;
		}
		finally
		{
			_accountLinksGate.Release();
		}
    }

	public async Task SaveRegistrationConfig()
	{
		await _accountLinksGate.WaitAsync();
		try
		{
			await WriteRegistrationConfigAsync();
		}
		finally
		{
			_accountLinksGate.Release();
		}
	}

	internal async Task<bool> RemoveAccountLinkAsync(ulong discordUserId)
	{
		await _accountLinksGate.WaitAsync();
		try
		{
			var removed = DetailedUserSettings.RemoveAll(x => x.DiscordUserId == discordUserId);
			if (removed > 0)
			{
				await WriteRegistrationConfigAsync();
			}
			return true;
		}
		catch (Exception e)
		{
			Log.Warning("Could not persist Discord account unlink for {DiscordUserId} ({ExceptionType}).", discordUserId, e.GetType().Name);
			return false;
		}
		finally
		{
			_accountLinksGate.Release();
		}
	}

	private Task WriteRegistrationConfigAsync()
	{
		var configLines = DiscordBotSettingsStore.SaveAccountLinks(DetailedUserSettings.ToArray()).ToArray();
		var content = configLines.Length == 0 ? "" : string.Join(Environment.NewLine, configLines) + Environment.NewLine;
		return _atomicFileWriter.WriteAsync("accountlinks.data", content);
	}
}
