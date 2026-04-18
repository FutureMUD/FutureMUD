using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot;

public class CustomCommandHandler
{
    public IEnumerable<(string Phrase, Command Binding)> CustomBindings { get; init; }
    public IEnumerable<string> StringPrefixes { get; init; }

    public async Task HandleCommandAsync(DiscordClient sender, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot || e.Author.IsSystem == true)
        {
            return;
        }

        int num = e.Message.GetMentionPrefixLength(sender.CurrentUser);
        if (StringPrefixes.Any())
        {
            foreach (string strPrefix in StringPrefixes)
            {
                if (num != -1)
                {
                    break;
                }

                if (string.IsNullOrEmpty(strPrefix))
                {
                    continue;
                }

                num = e.Message.GetStringPrefixLength(strPrefix, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        if (num == -1)
        {
            return;
        }

        string prefix = e.Message.Content[..num];
        string str = e.Message.Content[num..];
        CommandsNextExtension cnext = sender.GetCommandsNext();

        CommandContext ctx;
        foreach ((string phrase, Command binding) in CustomBindings)
        {
            if (str.StartsWith(phrase, StringComparison.InvariantCultureIgnoreCase))
            {
                ctx = cnext.CreateContext(e.Message, prefix, binding, string.Empty);
                await cnext.ExecuteCommandAsync(ctx);
                return;
            }
        }

        Command command = cnext.FindCommand(str, out string args);
        if (command == null)
        {
            return;
        }


        ctx = cnext.CreateContext(e.Message, prefix, command, args);
        await cnext.ExecuteCommandAsync(ctx);
    }
}