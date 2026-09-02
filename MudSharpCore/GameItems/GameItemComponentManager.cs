#nullable enable

using MudSharp.Accounts;
using MudSharp.GameItems.Prototypes;
using System.Reflection;

namespace MudSharp.GameItems;

internal sealed record GameItemComponentRegistrationAuditEntry(
	string CanonicalDatabaseType,
	string? PrimaryBuilderType,
	IReadOnlyList<string> BuilderAliases,
	string PrototypeClass,
	string Description,
	GameItemComponentTypeTechnology Technology,
	IReadOnlyList<string> ComponentCapabilities,
	IReadOnlyList<string> ExclusiveCapabilities,
	IReadOnlyList<string> RequiredSiblingCapabilities,
	bool HasBuilderLoader,
	bool HasDatabaseLoader,
	bool HasHelp,
	bool HasContextDependentRequirements);

public class GameItemComponentManager : IGameItemComponentManager
{
	private sealed class RegistrationAuditBuilder(Type prototypeType)
	{
		public Type PrototypeType { get; } = prototypeType;
		public List<string> BuilderAliases { get; } = [];
		public string? PrimaryBuilderType { get; set; }
		public string? CanonicalDatabaseType { get; set; }
		public GameItemComponentTypeHelpInfo? HelpInfo { get; set; }
	}

	private readonly List<string> _primaryTypes = [];
	private readonly Dictionary<string, Func<IFuturemud, IAccount, IGameItemComponentProto>>
		_registeredComponentProtos = [];
	private readonly Dictionary<string,
		Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto>>
		_registeredDatabaseLoaders = [];
	private readonly List<GameItemComponentTypeHelpInfo> _typeHelpInfo = [];
	private readonly Dictionary<Type, RegistrationAuditBuilder> _registrationAuditBuilders = [];
	private Type? _registeringPrototypeType;

	public GameItemComponentManager()
	{
		foreach (var type in Assembly.GetExecutingAssembly()
		                             .GetTypes()
		                             .Where(x => x.IsSubclassOf(typeof(GameItemComponentProto)))
		                             .OrderBy(x => x.FullName, StringComparer.Ordinal))
		{
			var method = type.GetMethod("RegisterComponentInitialiser", BindingFlags.Static | BindingFlags.Public);
			if (method is null)
			{
				continue;
			}

			_registeringPrototypeType = type;
			_registrationAuditBuilders[type] = new RegistrationAuditBuilder(type);
			try
			{
				method.Invoke(null, new object[] { this });
			}
			finally
			{
				_registeringPrototypeType = null;
			}
		}
	}

	public IEnumerable<string> PrimaryTypes => _primaryTypes;
	public IEnumerable<GameItemComponentTypeHelpInfo> TypeHelpInfo => _typeHelpInfo;

	internal IReadOnlyList<GameItemComponentRegistrationAuditEntry> RegistrationAuditEntries =>
		_registrationAuditBuilders.Values
		                          .Where(x => !string.IsNullOrWhiteSpace(x.CanonicalDatabaseType))
		                          .Select(CreateAuditEntry)
		                          .OrderBy(x => x.CanonicalDatabaseType, StringComparer.OrdinalIgnoreCase)
		                          .ToList();

	public void AddTypeHelpInfo(string name, string blurb, string help)
	{
		AddTypeHelpInfo(name, blurb, help, GameItemComponentTypeTechnology.None);
	}

	public void AddModernTypeHelpInfo(string name, string blurb, string help)
	{
		AddTypeHelpInfo(name, blurb, help, GameItemComponentTypeTechnology.Modern);
	}

	public void AddFuturisticTypeHelpInfo(string name, string blurb, string help)
	{
		AddTypeHelpInfo(name, blurb, help, GameItemComponentTypeTechnology.Futuristic);
	}

	private void AddTypeHelpInfo(string name, string blurb, string help,
		GameItemComponentTypeTechnology technology)
	{
		var helpInfo = new GameItemComponentTypeHelpInfo(name, blurb, help, technology);
		_typeHelpInfo.Add(helpInfo);
		if (CurrentAuditBuilder is not null)
		{
			CurrentAuditBuilder.HelpInfo = helpInfo;
		}
	}

	public IEnumerable<GameItemComponentTypeHelpInfo> GetTypeHelpInfo(bool showModern, bool showFuturistic)
	{
		return _typeHelpInfo.Where(x =>
			(showModern || !x.IsModern) &&
			(showFuturistic || !x.IsFuturistic));
	}

	public void AddBuilderLoader(string name, bool primary,
		Func<IFuturemud, IAccount, IGameItemComponentProto> initialiser)
	{
		name = name.ToLowerInvariant();
		if (CurrentAuditBuilder is not null)
		{
			if (!CurrentAuditBuilder.BuilderAliases.Contains(name, StringComparer.OrdinalIgnoreCase))
			{
				CurrentAuditBuilder.BuilderAliases.Add(name);
			}

			if (primary)
			{
				CurrentAuditBuilder.PrimaryBuilderType = name;
			}
		}

		if (_registeredComponentProtos.ContainsKey(name))
		{
			if (!primary)
			{
				return;
			}

			if (_primaryTypes.Contains(name))
			{
				throw new ArgumentException(
					$"A primary game item component builder loader named {name} is already registered.");
			}

			_registeredComponentProtos[name] = initialiser;
			_primaryTypes.Add(name);
			return;
		}

		_registeredComponentProtos.Add(name, initialiser);
		if (primary)
		{
			_primaryTypes.Add(name);
		}
	}

	public void AddDatabaseLoader(string name,
		Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto> initialiser)
	{
		if (CurrentAuditBuilder is not null)
		{
			CurrentAuditBuilder.CanonicalDatabaseType = name;
		}

		_registeredDatabaseLoaders.Add(name, initialiser);
	}

	public IGameItemComponentProto? GetProto(string name, IFuturemud gameworld, IAccount account)
	{
		var proto = _registeredComponentProtos.TryGetValue(name.ToLowerInvariant(), out var output)
			? output(gameworld, account)
			: null;
		if (proto is not null)
		{
			// This line lets component constructors persist after the derived type has been initialised.
			gameworld.SaveManager.Flush();
		}

		return proto;
	}

	public IGameItemComponentProto? GetProto(MudSharp.Models.GameItemComponentProto dbproto, IFuturemud gameworld)
	{
		return _registeredDatabaseLoaders.TryGetValue(dbproto.Type, out var output)
			? output(dbproto, gameworld)
			: null;
	}

	private RegistrationAuditBuilder? CurrentAuditBuilder => _registeringPrototypeType is not null &&
		_registrationAuditBuilders.TryGetValue(_registeringPrototypeType, out var builder)
			? builder
			: null;

	private static GameItemComponentRegistrationAuditEntry CreateAuditEntry(RegistrationAuditBuilder builder)
	{
		var componentMarker = typeof(IGameItemComponentPrototype<>);
		var exclusiveMarker = typeof(IExclusiveGameItemComponentPrototype<>);
		var interfaces = builder.PrototypeType.GetInterfaces();
		var componentCapabilities = interfaces
		                            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == componentMarker)
		                            .Select(x => x.GetGenericArguments()[0].Name)
		                            .Distinct(StringComparer.Ordinal)
		                            .OrderBy(x => x, StringComparer.Ordinal)
		                            .ToList();
		var exclusiveCapabilities = interfaces
		                            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == exclusiveMarker)
		                            .Select(x => x.GetGenericArguments()[0].Name)
		                            .Distinct(StringComparer.Ordinal)
		                            .OrderBy(x => x, StringComparer.Ordinal)
		                            .ToList();
		var requiredCapabilities = builder.PrototypeType
		                                  .GetFields(BindingFlags.Static | BindingFlags.Public |
		                                             BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
		                                  .Where(x => typeof(IEnumerable<GameItemComponentPrototypeRequirement>)
		                                      .IsAssignableFrom(x.FieldType))
		                                  .SelectMany(x =>
		                                      x.GetValue(null) as IEnumerable<GameItemComponentPrototypeRequirement> ??
		                                      [])
		                                  .Select(x => x.Capability.Name)
		                                  .Distinct(StringComparer.Ordinal)
		                                  .OrderBy(x => x, StringComparer.Ordinal)
		                                  .ToList();
		var hasContextDependentRequirements =
			typeof(IGameItemComponentPrototypeRequirementProvider).IsAssignableFrom(builder.PrototypeType) &&
			requiredCapabilities.Count == 0;

		return new GameItemComponentRegistrationAuditEntry(
			builder.CanonicalDatabaseType!,
			builder.PrimaryBuilderType,
			builder.BuilderAliases.OrderBy(x => x, StringComparer.Ordinal).ToList(),
			builder.PrototypeType.FullName ?? builder.PrototypeType.Name,
			builder.HelpInfo?.Blurb ?? string.Empty,
			builder.HelpInfo?.Technology ?? GameItemComponentTypeTechnology.None,
			componentCapabilities,
			exclusiveCapabilities,
			requiredCapabilities,
			builder.BuilderAliases.Count > 0,
			true,
			builder.HelpInfo is not null,
			hasContextDependentRequirements);
	}
}
