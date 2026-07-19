#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class PortalTopologySpellEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("portalnetwork", (root, spell) => new PortalTopologySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("portalnetwork", BuilderFactory,
			"Creates or links a durable portal/rune topology endpoint",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new PortalTopologySpellEffect(new XElement("Effect",
			new XAttribute("type", "portalnetwork"),
			new XElement("NetworkId", 0L),
			new XElement("EndpointKey", new XCData("rune")),
			new XElement("AnchorMode", (int)MagicPortalTopologyAnchorMode.TargetRoom),
			new XElement("LinkEndpointKey", new XCData(string.Empty)),
			new XElement("ReplaceExisting", true),
			new XElement("Permanent", false)
		), spell), string.Empty);
	}

	protected PortalTopologySpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		NetworkId = long.Parse(root.Element("NetworkId")?.Value ?? "0");
		EndpointKey = root.Element("EndpointKey")?.Value ?? "rune";
		AnchorMode = (MagicPortalTopologyAnchorMode)int.Parse(root.Element("AnchorMode")?.Value ?? ((int)MagicPortalTopologyAnchorMode.TargetRoom).ToString());
		LinkEndpointKey = root.Element("LinkEndpointKey")?.Value ?? string.Empty;
		ReplaceExisting = bool.Parse(root.Element("ReplaceExisting")?.Value ?? "true");
		Permanent = bool.Parse(root.Element("Permanent")?.Value ?? "false");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public long NetworkId { get; private set; }
	public string EndpointKey { get; private set; }
	public MagicPortalTopologyAnchorMode AnchorMode { get; private set; }
	public string LinkEndpointKey { get; private set; }
	public bool ReplaceExisting { get; private set; }
	public bool Permanent { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "portalnetwork"),
			new XElement("NetworkId", NetworkId),
			new XElement("EndpointKey", new XCData(EndpointKey)),
			new XElement("AnchorMode", (int)AnchorMode),
			new XElement("LinkEndpointKey", new XCData(LinkEndpointKey)),
			new XElement("ReplaceExisting", ReplaceExisting),
			new XElement("Permanent", Permanent)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => AnchorMode != MagicPortalTopologyAnchorMode.CasterRoom;

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "character" or "room" or "rooms" or "item" or "item&room" or "character&room" or "perceivable";
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var network = Gameworld.MagicPortalNetworks.Get(NetworkId);
		if (network is null)
		{
			return null;
		}

		ICell? cell = null;
		IGameItem? item = null;
		var endpointType = MagicPortalEndpointType.Cell;
		switch (AnchorMode)
		{
			case MagicPortalTopologyAnchorMode.CasterRoom:
				cell = caster.Location;
				break;
			case MagicPortalTopologyAnchorMode.TargetRoom:
				cell = target as ICell ??
				       additionalParameters.FirstOrDefault(x => x.ParameterName.EqualTo("room"))?.Item as ICell;
				break;
			case MagicPortalTopologyAnchorMode.TargetItem:
				item = target as IGameItem;
				endpointType = MagicPortalEndpointType.Item;
				break;
			default:
				return null;
		}

		if (endpointType == MagicPortalEndpointType.Cell && cell is null)
		{
			return null;
		}

		if (endpointType == MagicPortalEndpointType.Item && item is null)
		{
			return null;
		}

		var existingEndpoint = network.Endpoints.FirstOrDefault(x => x.Key.EqualTo(EndpointKey));
		if (existingEndpoint is not null && !Permanent)
		{
			return null;
		}

		IMagicPortalEndpoint? linkTarget = null;
		if (!string.IsNullOrWhiteSpace(LinkEndpointKey))
		{
			if (LinkEndpointKey.EqualTo(EndpointKey))
			{
				return null;
			}

			linkTarget = network.Endpoints.FirstOrDefault(x => x.Key.EqualTo(LinkEndpointKey));
			if (linkTarget is null)
			{
				return null;
			}
		}

		var service = new MagicPortalTopologyService();
		var endpoint = service.CreateOrUpdateEndpoint(caster, network, EndpointKey, EndpointKey.TitleCase(),
			endpointType, cell, item, ReplaceExisting, Spell.Id, out _);
		if (endpoint is null)
		{
			return null;
		}

		var createdEndpoints = new List<long>();
		var createdLinks = new List<long>();
		if (existingEndpoint is null)
		{
			createdEndpoints.Add(endpoint.Id);
		}

		if (!string.IsNullOrWhiteSpace(LinkEndpointKey))
		{
			var existingLink = network is MagicPortalNetwork concreteNetwork &&
			                   endpoint is MagicPortalEndpoint concreteEndpoint &&
			                   linkTarget is MagicPortalEndpoint concreteTarget
				? concreteNetwork.LinkBetween(concreteEndpoint, concreteTarget)
				: null;

			if (existingLink is null)
			{
				var link = service.CreateLink(caster, network, endpoint, linkTarget!, Spell.Id, out _);
				if (link is null)
				{
					if (createdEndpoints.Any())
					{
						service.DeleteSpellCreatedTopology(Gameworld, createdEndpoints, createdLinks);
					}

					return null;
				}

				createdLinks.Add(link.Id);
			}
		}

		var effectOwner = target ?? (IPerceivable?)cell ?? item;
		if (effectOwner is null)
		{
			return null;
		}

		return new SpellPortalTopologyEffect(effectOwner, parent, createdEndpoints, createdLinks, Permanent);
	}

	public IMagicSpellEffectTemplate Clone() => new PortalTopologySpellEffect(SaveToXml(), Spell);

	public const string HelpText = @"You can use the following options with this effect:

	#3network <id|name>#0 - sets the durable portal network to edit
	#3key <key>#0 - sets the endpoint key this spell creates or updates
	#3anchor caster#0 - creates the endpoint in the caster's room
	#3anchor room#0 - creates the endpoint at the target room
	#3anchor item#0 - creates the endpoint on the target item while directly placed
	#3link none|<endpoint key>#0 - optionally links the created endpoint to an existing endpoint
	#3replace#0 - toggles whether a permanent cast can update an existing endpoint key
	#3permanent#0 - toggles whether spell expiry cleans up created topology";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "network":
				return BuildingCommandNetwork(actor, command);
			case "key":
				return BuildingCommandKey(actor, command);
			case "anchor":
				return BuildingCommandAnchor(actor, command);
			case "link":
				return BuildingCommandLink(actor, command);
			case "replace":
				ReplaceExisting = !ReplaceExisting;
				break;
			case "permanent":
				Permanent = !Permanent;
				break;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send("The portal topology effect has been updated.");
		return true;
	}

	private bool BuildingCommandNetwork(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which portal network should this effect use?");
			return false;
		}

		var network = Gameworld.MagicPortalNetworks.GetByIdOrName(command.SafeRemainingArgument);
		if (network is null)
		{
			actor.OutputHandler.Send("There is no such magic portal network.");
			return false;
		}

		NetworkId = network.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now edit the {network.Name.ColourName()} portal network.");
		return true;
	}

	private bool BuildingCommandKey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What endpoint key should this effect create or update?");
			return false;
		}

		EndpointKey = command.SafeRemainingArgument.ToLowerInvariant();
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now create or update endpoint key {EndpointKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandAnchor(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "caster":
			case "casterroom":
			case "here":
				AnchorMode = MagicPortalTopologyAnchorMode.CasterRoom;
				break;
			case "room":
			case "targetroom":
				AnchorMode = MagicPortalTopologyAnchorMode.TargetRoom;
				break;
			case "item":
			case "targetitem":
				AnchorMode = MagicPortalTopologyAnchorMode.TargetItem;
				break;
			default:
				actor.OutputHandler.Send("Valid anchor modes are #3caster#0, #3room#0, and #3item#0.".SubstituteANSIColour());
				return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect now uses the {AnchorMode.DescribeEnum().ColourName()} anchor mode.");
		return true;
	}

	private bool BuildingCommandLink(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which existing endpoint key should be linked to this endpoint, or #3none#0?".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			LinkEndpointKey = string.Empty;
		}
		else
		{
			LinkEndpointKey = command.SafeRemainingArgument.ToLowerInvariant();
		}

		Spell.Changed = true;
		actor.OutputHandler.Send(string.IsNullOrWhiteSpace(LinkEndpointKey)
			? "This effect will not create a link."
			: $"This effect will link its endpoint to {LinkEndpointKey.ColourCommand()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var network = Gameworld.MagicPortalNetworks.Get(NetworkId);
		return SpellEffectPresentation.Describe(actor, "Portal Network",
			("Network", network?.Name.ColourName() ?? "None".ColourError()),
			("Endpoint Key", EndpointKey.ColourCommand()),
			("Anchor", AnchorMode.DescribeEnum().ColourName()),
			("Link", string.IsNullOrWhiteSpace(LinkEndpointKey) ? "none".ColourValue() : LinkEndpointKey.ColourCommand()),
			("Replace Existing", ReplaceExisting.ToColouredString()),
			("Permanent", Permanent.ToColouredString())
		);
	}
}
