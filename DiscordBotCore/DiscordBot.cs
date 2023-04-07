using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudSharp.Framework;
using Newtonsoft.Json;
using Serilog;

namespace Discord_Bot
{
    public partial class DiscordBot {
        private static DiscordBot _instance;

        public static DiscordBot Instance => _instance ??= new DiscordBot();

        private readonly List<TcpConnection> _tcpConnections = new List<TcpConnection>();
        public IEnumerable<TcpConnection> TCPConnections => _tcpConnections;
        private readonly Dictionary<ulong, CachedDiscordRequest> _cachedDiscordRequests = new ();
        public Dictionary<ulong, CachedDiscordRequest> CachedDiscordRequests => _cachedDiscordRequests;
        
        private async Task TcpListenerAsync() {
            var tcp = new TcpListener(IPAddress.Any, _tcpClientPort);
            tcp.Start();
            while (true) {
                foreach (var conn in _tcpConnections.Where(x => !x.Closing).ToList()) {
                    await conn.CheckIncomingAsync();
                }

                if (tcp.Pending()) {
                    try {
                        var newClient = await tcp.AcceptTcpClientAsync();
                        _tcpConnections.Add(new TcpConnection(newClient, _client, this));
                    }
                    catch (SocketException) {
                    }
                }

                foreach (var conn in _tcpConnections.Where(x => x.Closing).ToList()) {
                    _tcpConnections.Remove(conn);
                }

                await Task.Delay(100);
            }
        }

        public DiscordBot() {
            using (var reader = new StreamReader(new FileStream("settings.json", FileMode.Open))) {
                try {
                    DiscordBotSetttings settings =
                        JsonConvert.DeserializeObject<DiscordBotSetttings>(reader.ReadToEnd());
                    _tcpClientPort = settings.Port;
                    _botToken = settings.Token;
                    _botPrefixes.AddRange(settings.Prefixes);
                    ServerAuth = settings.ServerAuth;
                    _announceChannelId = settings.AnnounceChannelId;
                    _adminAnnounceChannelId = settings.AdminAnnounceChannelId;
                    _debugAnnounceChannelId = settings.DebugAnnounceChannelId;
                    GameName = settings.GameName;
                    _authorisedUsers.AddRange(settings.AdminUsers);
                    _customGlobalReactions.AddRange(settings.CustomGlobalReactions);
                }
                catch (Exception e) {
                    Console.WriteLine("");
                }
            }

            using (var reader = new StreamReader(new FileStream("accountlinks.data", FileMode.OpenOrCreate)))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var setting = new DetailedUserSetting(line);
                        DetailedUserSettings.Add(setting);
                    }
                }
                catch (Exception)
                {

                }
            }

            using (var reader = new StreamReader(new FileStream("lastversion.config", FileMode.OpenOrCreate)))
            {
                try
                {
                    _lastVersion = reader.ReadLine();
                }
                catch (Exception e)
                {

                }
            }
        }

        private readonly List<CustomGlobalReaction> _customGlobalReactions = new List<CustomGlobalReaction>();
        private int _tcpClientPort;
        private string _botToken;
        private readonly List<string> _botPrefixes = new List<string>();
        public string ServerAuth { get; private set; }
        private ulong _announceChannelId;
        private ulong _adminAnnounceChannelId;
        private ulong _debugAnnounceChannelId;
        private string _lastVersion;
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private IServiceProvider _services;
        public string GameName { get; private set; }
        public DiscordChannel AnnounceChannel { get; private set; }
        public DiscordChannel AdminAnnounceChannel { get; private set; }
        public DiscordChannel DebugAnnounceChannel { get; private set; }
        public List<DetailedUserSetting> DetailedUserSettings { get; } = new List<DetailedUserSetting>();
        public CustomCommandHandler CustomCommandHandler { get; private set; }

        private readonly List<ulong> _authorisedUsers = new List<ulong>();

        public bool IsAuthorisedUser(DiscordUser user) {
            return _authorisedUsers.Contains(user.Id);
        }

        public async Task RunBotAsync() {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();    
            var logFactory = new LoggerFactory().AddSerilog();

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
        
        public async Task AnnounceToDiscord(string message) {
            try {
                if (AnnounceChannel != null) {
                    await AnnounceChannel.SendMessageAsync(message);
                }
            }
            catch (Exception e) {
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

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
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
                var emoji = DiscordEmoji.FromUnicode("⚠️");
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
                var emoji = DiscordEmoji.FromUnicode("⚠️");
                await e.Context.Message.CreateReactionAsync(emoji);
                if (arge.ParamName == null)
                {
                    var argument =
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
                await using (var writer = new StreamWriter(new FileStream("lastversion.config", FileMode.OpenOrCreate)))
                {
                    await writer.WriteLineAsync(newVersionInfo);
                }
                await AnnounceToDiscord($"Version **{newVersionInfo}** is now live.");
            }
        }

        public async Task<string> GetMudStatusAsync() {
            if (!_tcpConnections.Any()) {
                return $"I'm not currently connected to **{GameName}**. Probably safe to presume it's down.";
            }

            return $"I am currently connected to **{GameName}**.";
        }

        public async Task AddAuthorisedUser(ulong who)
        {
            _authorisedUsers.Add(who);
            await using (var writer = new StreamWriter(new FileStream("settings.json", FileMode.OpenOrCreate)))
            {
                try
                {
                    await writer.WriteAsync(JsonConvert.SerializeObject(new DiscordBotSetttings
                    {
                        Port = _tcpClientPort,
                        Token = _botToken,
                        Prefixes = _botPrefixes.ToList(),
                        ServerAuth = ServerAuth,
                        AnnounceChannelId = _announceChannelId,
                        AdminAnnounceChannelId = _adminAnnounceChannelId,
                        DebugAnnounceChannelId = _debugAnnounceChannelId,
                        GameName = GameName,
                        AdminUsers = _authorisedUsers.ToList(),
                        CustomGlobalReactions = _customGlobalReactions.ToList()
                    }));
                }
                catch (Exception e)
                {
                }
            }
            using (var reader = new StreamReader(new FileStream("settings.json", FileMode.Open))) {
                try {
                    DiscordBotSetttings settings =
                        JsonConvert.DeserializeObject<DiscordBotSetttings>(reader.ReadToEnd());
                    _tcpClientPort = settings.Port;
                    _botToken = settings.Token;
                    _botPrefixes.AddRange(settings.Prefixes);
                    ServerAuth = settings.ServerAuth;
                    _announceChannelId = settings.AnnounceChannelId;
                    _adminAnnounceChannelId = settings.AdminAnnounceChannelId;
                    _debugAnnounceChannelId = settings.DebugAnnounceChannelId;
                    GameName = settings.GameName;
                    _authorisedUsers.AddRange(settings.AdminUsers);
                    _customGlobalReactions.AddRange(settings.CustomGlobalReactions);
                }
                catch (Exception e) {
                    Console.WriteLine("");
                }
            }
        }

        public async Task AnnounceToAdminChannel(string message) {
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

        public async Task AnnounceToAdminChannel(DiscordEmbed embed) {
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
                var channel = await _client.GetChannelAsync(channelId);
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

        public async Task AskMudToShutdown(long userid, bool stop){
             if (!_tcpConnections.Any(x => x.TcpClientAuthenticated))
            {
                return;
            }

             await _tcpConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"shutdown {userid} {stop}");
        }

        public async Task AskMudToBroadcast(string message)
        {
            if (!_tcpConnections.Any(x => x.TcpClientAuthenticated))
            {
                return;
            }

            await _tcpConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"broadcast {message}");
        }

        public async Task SaveRegistrationConfig()
        {
            await using (var writer = new StreamWriter(new FileStream("accountlinks.data", FileMode.Open, FileAccess.ReadWrite)))
            {
                foreach (var config in DetailedUserSettings)
                {
                    await writer.WriteLineAsync(config.SaveToConfig());
                }
            }
        }
    }
}
