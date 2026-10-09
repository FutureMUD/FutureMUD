# Static Configuration Security

`show config <setting>` and `editstaticconfig` use one case-insensitive restricted-setting list in `MudSharpCore/Commands/StaticConfigurationAccess.cs`. The implementor permission level is named `PermissionLevel.Founder` in code.

Only implementors can view or edit these settings:

| Setting | Reason |
| --- | --- |
| `EmailServer` | Complete email transport configuration, including account details, secret references, and legacy inline SMTP passwords. |
| `EngineUpdateBinariesPath` | Selects the filesystem directory overwritten by engine update extraction; only the owner may redirect this path. |
| `DiscordAuthToken` | Authentication secret sent to the Discord bridge. |
| `DiscordBotIpAddress` | Controls where the Discord authentication secret is sent. |
| `DiscordBotPort` | Controls which service receives the Discord authentication secret. |
| `GPT_Secret_Key` | OpenAI API credential. |
| `Gemini_Secret_Key` | Google Gemini API credential. |
| `Anthropic_API_Key` | Anthropic API credential. |

Other administrators can still see each setting's name. Viewing a restricted setting or listing configurations for editing displays `Redacted for Security` in red instead of fetching its value. Attempting to edit a restricted setting displays the same placeholder and refuses entry to the editor. Permission is checked again on editor submission, so losing implementor permission while editing prevents the change from being saved.

Implementors retain the existing value display and editor workflow. Other static settings retain their existing command permissions. Runtime consumers, database storage, seeded defaults, and setup workflows are unchanged. Add future sensitive static settings to the same list; name matching deliberately does not use keyword guesses.
