#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic.Gathering;

namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private bool BuildingCommandGatheringLand(ICharacter actor, StringStack command)
	{
		MagicGatheringMethodDefinition? method = FindGatheringMethod(command.PopSpeech());
		if (method?.Kind != MagicGatheringMethodKind.Land)
		{
			actor.OutputHandler.Send("Specify an existing Land gathering method. See gather help.");
			return false;
		}
		string setting = command.PopForSwitch();
		MagicGatheringMethodDefinition updated;
		try
		{
			switch (setting)
			{
				case "source":
					updated = EditLandSource(method, command, false);
					break;
				case "collateral":
					updated = EditLandSource(method, command, true);
					break;
				case "damage":
					updated = method with { LandDamagePerDestinationUnit = PositiveNumber(command, "Land scar per mana") };
					break;
				case "pressure":
					updated = method with { LandPressurePerDestinationUnit = NonNegativeNumber(command, "Land pressure per mana") };
					break;
				case "damageprog":
					updated = method with { LandDamageProgId = ProgId(command, "Land damage") };
					break;
				case "pressureprog":
					updated = method with { LandPressureProgId = ProgId(command, "Land pressure") };
					break;
				case "crophealth":
					updated = method with { CropHealthCostPerDestinationUnit = NonNegativeNumber(command, "crop health cost") };
					break;
				case "woodlandhealth":
					updated = method with { WoodlandHealthCostPerDestinationUnit = NonNegativeNumber(command, "woodland health cost") };
					break;
				case "crophealthprog":
					updated = method with { CropHealthCostProgId = ProgId(command, "crop health cost") };
					break;
				case "woodlandhealthprog":
					updated = method with { WoodlandHealthCostProgId = ProgId(command, "woodland health cost") };
					break;
				case "message":
					updated = EditLandMessage(method, command);
					break;
				default:
					throw new InvalidOperationException("Use gather land <method> source|collateral|damage|pressure|crophealth|woodlandhealth|message ...");
			}
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send(ex.Message);
			return false;
		}
		ReplaceGatheringMethod(updated with { StructuralVersion = checked(method.StructuralVersion + 1) });
		Changed = true;
		actor.OutputHandler.Send($"Updated destructive Land method {method.Name.ColourName()}; live precommit actions will revalidate.");
		return true;
	}

	private MagicGatheringMethodDefinition EditLandSource(MagicGatheringMethodDefinition method,
		StringStack command, bool collateral)
	{
		string first = command.PopForSwitch();
		List<MagicLandSourceDefinition> entries = method.LandSources.ToList();
		if (first == "add")
		{
			if (entries.Count >= 16) throw new InvalidOperationException("A Land method supports at most sixteen combined sources and collateral entries.");
			string selector = ReadLandSourceSelector(command);
			if (!MagicGatheringService.TryCanonicalLandSelector(selector, out selector))
			{
				throw new InvalidOperationException("Specify ambient:<resource-id>, forage:<yield-key>, crop, woodland or pasture.");
			}
			double ratio = collateral ? NonNegativeNumber(command, "collateral units per mana")
				: PositiveNumber(command, "source units per mana");
			entries.Add(new(Guid.NewGuid(), selector, ratio, collateral));
			return method with { LandSources = entries.ToArray() };
		}
		if (collateral) throw new InvalidOperationException("Use gather land <method> collateral add <selector> <units-per-mana>.");
		if (!int.TryParse(first, NumberStyles.None, CultureInfo.InvariantCulture, out int index) ||
		    index < 1 || index > entries.Count)
		{
			throw new InvalidOperationException("Specify a valid one-based Land source entry index.");
		}
		MagicLandSourceDefinition entry = entries[index - 1];
		switch (command.PopForSwitch())
		{
			case "remove":
				entries.RemoveAt(index - 1);
				break;
			case "move":
				if (!int.TryParse(command.SafeRemainingArgument, NumberStyles.None, CultureInfo.InvariantCulture,
				    out int destination) || destination < 1 || destination > entries.Count)
				{
					throw new InvalidOperationException("Specify a destination index within the current source list.");
				}
				entries.RemoveAt(index - 1);
				entries.Insert(destination - 1, entry);
				break;
			case "ratio":
				entries[index - 1] = entry with { UnitsPerDestinationUnit = entry.IsCollateral
					? NonNegativeNumber(command, "collateral units per mana")
					: PositiveNumber(command, "source units per mana") };
				break;
			case "ratioprog":
				entries[index - 1] = entry with { RatioProgId = ProgId(command, "Land source ratio") };
				break;
			case "optional":
				if (entry.IsCollateral) throw new InvalidOperationException("Mandatory collateral cannot be optional.");
				string value = command.SafeRemainingArgument;
				if (!value.EqualToAny("on", "off")) throw new InvalidOperationException("Use optional on|off.");
				entries[index - 1] = entry with { AllowAbsent = value.EqualTo("on") };
				break;
			default:
				throw new InvalidOperationException("Use source <index> remove|move|ratio|ratioprog|optional ...");
		}
		return method with { LandSources = entries.ToArray() };
	}

	private string ReadLandSourceSelector(StringStack command)
	{
		string kind = command.PopSpeech();
		if (kind.EqualTo("ambient"))
		{
			string name = command.PopSpeech();
			IMagicResource? resource = Gameworld.MagicResources.GetByIdOrName(name);
			if (resource is null || !resource.ResourceType.HasFlag(MagicResourceType.LocationResource))
			{
				throw new InvalidOperationException("Specify an existing location-capable ambient resource.");
			}
			return $"ambient:{resource.Id}";
		}
		if (kind.EqualTo("forage")) return $"forage:{command.PopSpeech()}";
		return kind;
	}

	private static MagicGatheringMethodDefinition EditLandMessage(MagicGatheringMethodDefinition method,
		StringStack command)
	{
		string target = command.PopForSwitch();
		string text = command.SafeRemainingArgument;
		if (text.Length > 1000) throw new InvalidOperationException("A gathering message must be at most 1,000 characters.");
		return target switch
		{
			"actorstart" => method with { LandActorStartEmote = text },
			"observerstart" => method with { LandObserverStartEmote = text },
			"actorcomplete" => method with { LandActorCompleteEmote = text },
			"observercomplete" => method with { LandObserverCompleteEmote = text },
			"actorcancel" => method with { LandActorCancelEmote = text },
			"observercancel" => method with { LandObserverCancelEmote = text },
			_ => throw new InvalidOperationException("Use message actorstart|observerstart|actorcomplete|observercomplete|actorcancel|observercancel <emote>.")
		};
	}
}
