namespace MudClientBlazor.Services;

public sealed class LoginAliasSettings
{
	public string Label { get; set; } = "Login";
	public string InitialCommand { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public bool IsEnabled { get; set; } = true;

	public LoginAliasSettings Clone()
	{
		return new LoginAliasSettings
		{
			Label = Label,
			InitialCommand = InitialCommand,
			Username = Username,
			Password = Password,
			IsEnabled = IsEnabled
		};
	}
}

public sealed class QuickAliasBinding
{
	public string Id { get; set; } = string.Empty;
	public string Label { get; set; } = string.Empty;
	public string Command { get; set; } = string.Empty;
	public bool IsEnabled { get; set; } = true;

	public QuickAliasBinding Clone()
	{
		return new QuickAliasBinding
		{
			Id = Id,
			Label = Label,
			Command = Command,
			IsEnabled = IsEnabled
		};
	}
}

public sealed class QuickAliasSettings
{
	public const int CurrentVersion = 1;
	public const int AliasCount = 10;

	public int Version { get; set; } = CurrentVersion;
	public LoginAliasSettings Login { get; set; } = CreateDefaultLogin();
	public List<QuickAliasBinding> Aliases { get; set; } = new();

	public QuickAliasSettings Clone()
	{
		return new QuickAliasSettings
		{
			Version = Version,
			Login = Login.Clone(),
			Aliases = Aliases.Select(alias => alias.Clone()).ToList()
		};
	}

	public static QuickAliasSettings CreateDefault()
	{
		return new QuickAliasSettings
		{
			Login = CreateDefaultLogin(),
			Aliases = CreateDefaultAliases().Select(alias => alias.Clone()).ToList()
		};
	}

	public static LoginAliasSettings CreateDefaultLogin()
	{
		return new LoginAliasSettings
		{
			Label = "Login",
			IsEnabled = true
		};
	}

	public static IReadOnlyList<QuickAliasBinding> CreateDefaultAliases()
	{
		return Enumerable.Range(1, AliasCount)
			.Select(index => new QuickAliasBinding
			{
				Id = $"alias-{index}",
				Label = $"Alias {index}",
				IsEnabled = true
			})
			.ToArray();
	}

	public static QuickAliasSettings Normalize(QuickAliasSettings? settings)
	{
		if (settings == null)
		{
			return CreateDefault();
		}

		var savedById = (settings.Aliases ?? [])
			.Where(alias => !string.IsNullOrWhiteSpace(alias.Id))
			.GroupBy(alias => alias.Id, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

		var normalized = new QuickAliasSettings
		{
			Version = CurrentVersion,
			Login = NormalizeLogin(settings.Login)
		};

		foreach (var defaultAlias in CreateDefaultAliases())
		{
			savedById.TryGetValue(defaultAlias.Id, out var savedAlias);
			normalized.Aliases.Add(NormalizeAlias(savedAlias, defaultAlias));
		}

		return normalized;
	}

	public static QuickAliasSettings CreatePersistentCopy(QuickAliasSettings settings)
	{
		ArgumentNullException.ThrowIfNull(settings);

		var persistent = Normalize(settings);
		persistent.Login.Password = string.Empty;
		return persistent;
	}

	private static LoginAliasSettings NormalizeLogin(LoginAliasSettings? savedLogin)
	{
		var defaultLogin = CreateDefaultLogin();
		if (savedLogin == null)
		{
			return defaultLogin;
		}

		return new LoginAliasSettings
		{
			Label = string.IsNullOrWhiteSpace(savedLogin.Label) ? defaultLogin.Label : savedLogin.Label.Trim(),
			InitialCommand = savedLogin.InitialCommand ?? string.Empty,
			Username = savedLogin.Username ?? string.Empty,
			Password = savedLogin.Password ?? string.Empty,
			IsEnabled = savedLogin.IsEnabled
		};
	}

	private static QuickAliasBinding NormalizeAlias(QuickAliasBinding? savedAlias, QuickAliasBinding defaultAlias)
	{
		return new QuickAliasBinding
		{
			Id = defaultAlias.Id,
			Label = string.IsNullOrWhiteSpace(savedAlias?.Label) ? defaultAlias.Label : savedAlias.Label.Trim(),
			Command = savedAlias?.Command ?? defaultAlias.Command,
			IsEnabled = savedAlias?.IsEnabled ?? defaultAlias.IsEnabled
		};
	}
}
