#nullable enable

using System.Threading;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

/// <summary>
/// Routes live media feeds over the existing computer-network reachability model. It carries no playback backlog:
/// packets are forwarded only while a valid source, route, subscription and destination output all exist.
/// </summary>
public sealed class ComputerMediaNetworkService : IComputerMediaNetworkService
{
	private const string MediaApplicationId = "media";
	private readonly object _sync = new();
	private readonly IFuturemud _gameworld;
	private readonly Dictionary<IComputerMediaConfigurationHost, List<MediaSubscriptionConfiguration>>
		_transientSubscriptions = [];
	private long _nextTransientSubscriptionId;

	public ComputerMediaNetworkService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
		var mediaChannel = _gameworld.MediaChannelService;
		if (mediaChannel is not null)
		{
			mediaChannel.PacketDelivered += HandlePacketDelivered;
		}
	}

	public IEnumerable<ComputerMediaFeedInfo> GetFeeds(IComputerHost host)
	{
		if (host is not IComputerMediaConfigurationHost configurationHost)
		{
			return Enumerable.Empty<ComputerMediaFeedInfo>();
		}

		return configurationHost.MediaFeeds
			.Select(feed => new ComputerMediaFeedInfo(feed.FeedName, feed.InputName, feed.IsPublic,
				feed.AllowedAccountIds, IsFeedActive(host, feed)))
			.OrderBy(x => x.FeedName)
			.ToList();
	}

	public IEnumerable<ComputerMediaSubscriptionInfo> GetSubscriptions(IComputerHost host)
	{
		if (host is not IComputerMediaConfigurationHost configurationHost)
		{
			return Enumerable.Empty<ComputerMediaSubscriptionInfo>();
		}

		return configurationHost.MediaSubscriptions
			.Select(subscription => new ComputerMediaSubscriptionInfo(subscription.SubscriptionName,
				subscription.SourceAddress, subscription.FeedName, subscription.OutputName, subscription.AccountId,
				IsSubscriptionActive(configurationHost, subscription), true))
			.Concat(GetTransientSubscriptions(configurationHost)
				.Select(subscription => new ComputerMediaSubscriptionInfo(subscription.SubscriptionName,
					subscription.SourceAddress, subscription.FeedName, subscription.OutputName, subscription.AccountId,
					IsSubscriptionActive(configurationHost, subscription), false)))
			.OrderBy(x => x.SubscriptionName)
			.ToList();
	}

	public bool PublishFeed(IComputerHost host, string input, string feedName, bool isPublic, out string error)
	{
		error = string.Empty;
		if (host is not IComputerMediaConfigurationHost configurationHost)
		{
			error = "Media feeds require a persistent physical computer host.";
			return false;
		}

		if (!TryNormaliseFeedName(feedName, out var normalisedFeedName, out error))
		{
			return false;
		}

		if (!_gameworld.ComputerMediaService.TryResolveMediaInput(host, input, out _, out error))
		{
			return false;
		}

		var existing = configurationHost.MediaFeeds.FirstOrDefault(x =>
			x.FeedName.Equals(normalisedFeedName, StringComparison.InvariantCultureIgnoreCase));
		return configurationHost.UpsertMediaFeed(new MediaFeedConfiguration(normalisedFeedName, input.Trim(), isPublic,
			existing?.AllowedAccountIds ?? Array.Empty<long>()), out error);
	}

	public bool SetFeedAcl(IComputerHost host, string feedName, string accountAddress, bool add, out string error)
	{
		error = string.Empty;
		if (host is not IComputerMediaConfigurationHost configurationHost)
		{
			error = "Media feeds require a persistent physical computer host.";
			return false;
		}

		var feed = configurationHost.MediaFeeds.FirstOrDefault(x =>
			x.FeedName.Equals(feedName.Trim(), StringComparison.InvariantCultureIgnoreCase));
		if (feed is null)
		{
			error = "There is no media feed with that name on this host.";
			return false;
		}

		var account = _gameworld.ComputerNetworkIdentityService.FindAccount(host, accountAddress, out error);
		if (account is null)
		{
			return false;
		}

		var accountIds = feed.AllowedAccountIds.ToHashSet();
		if (add)
		{
			accountIds.Add(account.Id);
		}
		else if (!accountIds.Remove(account.Id))
		{
			error = $"{account.Address.ColourName()} is not on that feed's access list.";
			return false;
		}

		return configurationHost.UpsertMediaFeed(feed with { AllowedAccountIds = accountIds.OrderBy(x => x).ToList() },
			out error);
	}

	public bool SubscribeFeed(IComputerHost subscriberHost, string hostAddress, string feedName, string output,
		IComputerNetworkAccount? account, string? savedSubscriptionName, IComputerTerminalSession? session,
		out string subscriptionName, out string error)
	{
		subscriptionName = string.Empty;
		error = string.Empty;
		if (subscriberHost is not IComputerMediaConfigurationHost configurationHost)
		{
			error = "Media subscriptions require a persistent physical computer host.";
			return false;
		}

		if (!_gameworld.ComputerMediaService.GetMediaOutputs(subscriberHost)
			.Any(x => x.Equals(output, StringComparison.InvariantCultureIgnoreCase)))
		{
			error = "There is no powered, connected media output with that name.";
			return false;
		}

		var summary = ResolveMediaHost(subscriberHost, hostAddress, session);
		if (summary is null || !summary.Host.IsNetworkServiceEnabled(MediaApplicationId))
		{
			error = $"There is no reachable host advertising the Media service at {hostAddress.ColourName()}.";
			return false;
		}

		if (summary.Host is not IComputerMediaConfigurationHost sourceConfigurationHost)
		{
			error = "That media service host does not have persistent feed configuration.";
			return false;
		}

		var feed = sourceConfigurationHost.MediaFeeds.FirstOrDefault(x =>
			x.FeedName.Equals(feedName.Trim(), StringComparison.InvariantCultureIgnoreCase));
		if (feed is null || !IsFeedActive(summary.Host, feed))
		{
			error = "That media feed is not currently available.";
			return false;
		}

		if (!ValidateFeedAccess(subscriberHost, feed, account, out error))
		{
			return false;
		}

		var sourceHostItemId = summary.Host.OwnerHostItemId;
		if (sourceHostItemId is not > 0L)
		{
			error = "That media service does not have a stable host identity.";
			return false;
		}

		var isSavedSubscription = !string.IsNullOrWhiteSpace(savedSubscriptionName);
		var requestedName = isSavedSubscription
			? savedSubscriptionName!.Trim()
			: $"live-{Interlocked.Increment(ref _nextTransientSubscriptionId)}";
		if (configurationHost.MediaSubscriptions.Any(x =>
			x.SubscriptionName.Equals(requestedName, StringComparison.InvariantCultureIgnoreCase)) ||
		    GetTransientSubscriptions(configurationHost).Any(x =>
				x.SubscriptionName.Equals(requestedName, StringComparison.InvariantCultureIgnoreCase)))
		{
			error = "A media subscription with that name already exists. Unsubscribe it before replacing it.";
			return false;
		}

		var configuration = new MediaSubscriptionConfiguration(requestedName, sourceHostItemId.Value,
			summary.CanonicalAddress, feed.FeedName, output.Trim(), feed.IsPublic ? null : account!.Id, true);
		if (isSavedSubscription && !configurationHost.UpsertMediaSubscription(configuration, out error))
		{
			return false;
		}

		if (!isSavedSubscription)
		{
			AddTransientSubscription(configurationHost, configuration);
		}

		subscriptionName = requestedName;
		return true;
	}

	public bool UnsubscribeFeed(IComputerHost subscriberHost, string subscriptionName, out string error)
	{
		if (subscriberHost is not IComputerMediaConfigurationHost configurationHost)
		{
			error = "Media subscriptions require a persistent physical computer host.";
			return false;
		}

		if (configurationHost.MediaSubscriptions.Any(x =>
			x.SubscriptionName.Equals(subscriptionName.Trim(), StringComparison.InvariantCultureIgnoreCase)))
		{
			return configurationHost.RemoveMediaSubscription(subscriptionName, out error);
		}

		if (RemoveTransientSubscription(configurationHost, subscriptionName))
		{
			error = string.Empty;
			return true;
		}

		error = "There is no media subscription with that name on this host.";
		return false;
	}

	public void InterruptSubscriptions(IComputerHost host)
	{
		if (host is not IComputerMediaConfigurationHost configurationHost)
		{
			return;
		}

		lock (_sync)
		{
			_transientSubscriptions.Remove(configurationHost);
		}
	}

	public bool SubscribeFromProgram(IComputerHost subscriberHost, string addressAndFeed, string output,
		bool savedSubscription, out string error)
	{
		error = string.Empty;
		if (!TrySplitAddressAndFeed(addressAndFeed, out var hostAddress, out var feedName, out error))
		{
			return false;
		}

		if (savedSubscription)
		{
			if (subscriberHost is not IComputerMediaConfigurationHost configurationHost)
			{
				error = "Saved media subscriptions require a persistent physical computer host.";
				return false;
			}

			var saved = configurationHost.MediaSubscriptions.FirstOrDefault(x =>
				x.SourceAddress.Equals(hostAddress, StringComparison.InvariantCultureIgnoreCase) &&
				x.FeedName.Equals(feedName, StringComparison.InvariantCultureIgnoreCase) &&
				x.OutputName.Equals(output, StringComparison.InvariantCultureIgnoreCase));
			if (saved is null || saved.AccountId is not > 0L)
			{
				error = "There is no saved private media subscription matching that source, feed and output.";
				return false;
			}

			var account = _gameworld.ComputerNetworkIdentityService.GetAccount(subscriberHost, saved.AccountId.Value,
				out error);
			if (account is null)
			{
				return false;
			}

			var sourceSummary = ResolveMediaHost(subscriberHost, hostAddress, null);
			if (sourceSummary?.Host is not IComputerMediaConfigurationHost sourceConfigurationHost ||
			    !sourceSummary.Host.IsNetworkServiceEnabled(MediaApplicationId))
			{
				error = "The saved subscription's media service is not currently reachable.";
				return false;
			}

			var feed = sourceConfigurationHost.MediaFeeds.FirstOrDefault(x =>
				x.FeedName.Equals(feedName, StringComparison.InvariantCultureIgnoreCase));
			if (feed is null || !ValidateFeedAccess(subscriberHost, feed, account, out error))
			{
				return false;
			}

			return true;
		}

		return SubscribeFeed(subscriberHost, hostAddress, feedName, output, null, null, null, out _, out error);
	}

	public IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId)
	{
		if (!applicationId.EqualTo(MediaApplicationId))
		{
			return Enumerable.Empty<string>();
		}

		var feeds = GetFeeds(host).ToList();
		var publicFeeds = feeds.Where(x => x.IsPublic).Select(x => x.FeedName).OrderBy(x => x).ToList();
		var privateCount = feeds.Count(x => !x.IsPublic);
		return publicFeeds
			.Select(x => $"Public feed: {x}")
			.Concat(privateCount > 0 ? [$"{privateCount} private feed(s)"] : [])
			.ToList();
	}

	private void HandlePacketDelivered(IMediaSink sink, MediaPacket packet)
	{
		if (sink is not IComputerMediaInterface sourceInterface ||
		    sourceInterface.ConnectedHost is not IComputerMediaConfigurationHost sourceHost ||
		    !sourceHost.Powered || !sourceHost.IsNetworkServiceEnabled(MediaApplicationId))
		{
			return;
		}

		var feeds = sourceHost.MediaFeeds
			.Where(feed => IsFeedActive(sourceHost, feed))
			.Where(feed => _gameworld.ComputerMediaService.TryResolveMediaInput(sourceHost, feed.InputName,
				out var endpoint, out _) && endpoint == sink.MediaInputEndpoint)
			.ToList();
		if (!feeds.Any())
		{
			return;
		}

		foreach (var feed in feeds)
		{
			foreach (var subscriberHost in GetConfigurationHosts())
			{
				if (!subscriberHost.Powered)
				{
					continue;
				}

				foreach (var subscription in subscriberHost.MediaSubscriptions
					         .Concat(GetTransientSubscriptions(subscriberHost))
					         .Where(x => x.Enabled)
					         .Where(x => x.SourceHostItemId == sourceHost.OwnerHostItemId)
					         .Where(x => x.FeedName.Equals(feed.FeedName, StringComparison.InvariantCultureIgnoreCase))
					         .ToList())
				{
					if (!CanDeliver(subscriberHost, sourceHost, feed, subscription))
					{
						continue;
					}

					_gameworld.ComputerMediaService.PublishToOutput(subscriberHost, subscription.OutputName, packet,
						out _);
				}
			}
		}
	}

	private bool CanDeliver(IComputerMediaConfigurationHost subscriberHost, IComputerMediaConfigurationHost sourceHost,
		MediaFeedConfiguration feed, MediaSubscriptionConfiguration subscription)
	{
		if (!IsReachable(subscriberHost, sourceHost) || !sourceHost.IsNetworkServiceEnabled(MediaApplicationId))
		{
			return false;
		}

		if (feed.IsPublic)
		{
			return true;
		}

		if (subscription.AccountId is not > 0L)
		{
			return false;
		}

		var account = _gameworld.ComputerNetworkIdentityService.GetAccount(subscriberHost, subscription.AccountId.Value,
			out _);
		return account is not null && account.Enabled && feed.AllowedAccountIds.Contains(account.Id);
	}

	private bool IsFeedActive(IComputerHost host, MediaFeedConfiguration feed)
	{
		return host.Powered && host.IsNetworkServiceEnabled(MediaApplicationId) &&
		       _gameworld.ComputerMediaService.TryResolveMediaInput(host, feed.InputName, out _, out _);
	}

	private bool IsSubscriptionActive(IComputerMediaConfigurationHost subscriberHost, MediaSubscriptionConfiguration subscription)
	{
		var sourceHost = GetConfigurationHosts()
			.FirstOrDefault(x => x.OwnerHostItemId == subscription.SourceHostItemId);
		if (sourceHost is null || !subscriberHost.Powered || !sourceHost.Powered ||
		    !IsReachable(subscriberHost, sourceHost))
		{
			return false;
		}

		var feed = sourceHost.MediaFeeds.FirstOrDefault(x =>
			x.FeedName.Equals(subscription.FeedName, StringComparison.InvariantCultureIgnoreCase));
		return feed is not null && IsFeedActive(sourceHost, feed) &&
		       _gameworld.ComputerMediaService.GetMediaOutputs(subscriberHost)
			       .Any(x => x.Equals(subscription.OutputName, StringComparison.InvariantCultureIgnoreCase)) &&
		       CanDeliver(subscriberHost, sourceHost, feed, subscription);
	}

	private bool ValidateFeedAccess(IComputerHost subscriberHost, MediaFeedConfiguration feed,
		IComputerNetworkAccount? suppliedAccount, out string error)
	{
		error = string.Empty;
		if (feed.IsPublic)
		{
			return true;
		}

		if (suppliedAccount is null)
		{
			error = "That is a private media feed. Log in with a network account on its access list first.";
			return false;
		}

		var account = _gameworld.ComputerNetworkIdentityService.GetAccount(subscriberHost, suppliedAccount.Id, out error);
		if (account is null)
		{
			return false;
		}

		if (!feed.AllowedAccountIds.Contains(account.Id))
		{
			error = "That network account is not on this feed's access list.";
			return false;
		}

		return true;
	}

	private ComputerNetworkHostSummary? ResolveMediaHost(IComputerHost sourceHost, string identifier,
		IComputerTerminalSession? session)
	{
		return _gameworld.ComputerExecutionService.ResolveReachableHost(sourceHost, identifier, session);
	}

	private bool IsReachable(IComputerHost sourceHost, IComputerHost targetHost)
	{
		return sourceHost.OwnerHostItemId == targetHost.OwnerHostItemId ||
		       _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			       .Any(x => x.Host.OwnerHostItemId == targetHost.OwnerHostItemId);
	}

	private IEnumerable<IComputerMediaConfigurationHost> GetConfigurationHosts()
	{
		return _gameworld.Items
			.SelectMany(item => item.Components.OfType<IComputerMediaConfigurationHost>())
			.ToList();
	}

	private IEnumerable<MediaSubscriptionConfiguration> GetTransientSubscriptions(
		IComputerMediaConfigurationHost host)
	{
		lock (_sync)
		{
			return _transientSubscriptions.TryGetValue(host, out var subscriptions)
				? subscriptions.ToList()
				: [];
		}
	}

	private void AddTransientSubscription(IComputerMediaConfigurationHost host,
		MediaSubscriptionConfiguration subscription)
	{
		lock (_sync)
		{
			if (!_transientSubscriptions.TryGetValue(host, out var subscriptions))
			{
				subscriptions = [];
				_transientSubscriptions[host] = subscriptions;
			}

			subscriptions.Add(subscription);
		}
	}

	private bool RemoveTransientSubscription(IComputerMediaConfigurationHost host, string subscriptionName)
	{
		lock (_sync)
		{
			if (!_transientSubscriptions.TryGetValue(host, out var subscriptions))
			{
				return false;
			}

			var removed = subscriptions.RemoveAll(x =>
				x.SubscriptionName.Equals(subscriptionName.Trim(), StringComparison.InvariantCultureIgnoreCase)) > 0;
			if (subscriptions.Count == 0)
			{
				_transientSubscriptions.Remove(host);
			}

			return removed;
		}
	}

	private static bool TryNormaliseFeedName(string feedName, out string normalised, out string error)
	{
		normalised = feedName.Trim();
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(normalised) || normalised.Contains('/') || normalised.Contains('@'))
		{
			error = "Media feed names must be non-empty and cannot contain / or @.";
			return false;
		}

		return true;
	}

	private static bool TrySplitAddressAndFeed(string addressAndFeed, out string hostAddress, out string feedName,
		out string error)
	{
		hostAddress = string.Empty;
		feedName = string.Empty;
		error = string.Empty;
		var index = addressAndFeed.LastIndexOf('/');
		if (index <= 0 || index >= addressAndFeed.Length - 1)
		{
			error = "Media feed addresses must be in the form host-address/feed.";
			return false;
		}

		hostAddress = addressAndFeed[..index].Trim();
		feedName = addressAndFeed[(index + 1)..].Trim();
		return TryNormaliseFeedName(feedName, out feedName, out error) && !string.IsNullOrWhiteSpace(hostAddress);
	}

}
