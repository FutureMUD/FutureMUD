using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Powers;

public abstract class MagicPowerBase : SaveableItem, IMagicPower
{

	protected MagicPowerBase(Models.MagicPower power, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = power.Id;
		_name = power.Name;
		Blurb = power.Blurb;
		School = Gameworld.MagicSchools.Get(power.MagicSchoolId);
		_showHelpText = power.ShowHelp;
		var root = XElement.Parse(power.Definition);
		var element = root.Element("CanInvokePowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no CanInvokePowerProg in the definition XML for power {Id} ({Name}).");
		}

		CanInvokePowerProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("WhyCantInvokePowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no WhyCantInvokePowerProg in the definition XML for power {Id} ({Name}).");
		}

		WhyCantInvokePowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("InvocationCosts");
		if (element != null)
		{
			foreach (var verbElement in element.Element("Verbs")?.Elements("Verb") ?? Enumerable.Empty<XElement>())
			{
				var verb = verbElement.Attribute("verb")?.Value ?? throw new ApplicationException(
					$"There was no verb attribute in the InvocationCosts of a Verb element in the definition XML for power {Id} ({Name}).");
				foreach (var sub in verbElement.Elements())
				{
					var which = sub.Attribute("resource")?.Value;
					if (string.IsNullOrWhiteSpace(which))
					{
						throw new ApplicationException(
							$"There was no resource attribute in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					var resource = long.TryParse(which, out value)
						? gameworld.MagicResources.Get(value)
						: gameworld.MagicResources.GetByName(which);
					if (resource == null)
					{
						throw new ApplicationException(
							$"Could not load the resource referred to by '{which}' in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					if (!double.TryParse(sub.Value, out var dvalue))
					{
						throw new ApplicationException(
							$"Could not convert the amount in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					InvocationCosts.Add(verb, (resource, dvalue));
				}
			}
		}
	}

	public sealed override string FrameworkItemType => "MagicPower";

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		return $"Magic Power {Id} - {Name}";
	}

	public ICharacter AcquireTarget(ICharacter owner, string targetText, MagicPowerDistance distance)
	{
		switch (distance)
		{
			case MagicPowerDistance.AnyConnectedMind:
				return owner.CombinedEffectsOfType<ConnectMindEffect>()
				            .Where(x => x.School == School)
				            .Select(x => x.TargetCharacter)
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.AnyConnectedMindOrConnectedTo:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Where(x => x.School == School)
				            .Select(x => x.TargetCharacter)
				            .Concat(owner.CombinedEffectsOfType<MindConnectedToEffect>().Where(x => x.School == School)
				                         .Select(x => x.OriginatorCharacter))
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameLocationOnly:
				return owner.Location.Characters
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.AdjacentLocationsOnly:
				return owner.Location.Characters
				            .Except(owner)
				            .Concat(owner.Location.ExitsFor(owner).Select(x => x.Destination)
				                         .SelectMany(x => x.Characters))
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameAreaOnly:
				return owner.Location.Areas.Any()
					? owner.Location.Areas
					       .SelectMany(x => x.Cells.SelectMany(y => y.Characters))
					       .Except(owner)
					       .Where(x => TargetFilterFunction(owner, x))
					       .GetFromItemListByKeyword(targetText, owner)
					: owner.Location.Characters
					       .Where(x => TargetFilterFunction(owner, x))
					       .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameZoneOnly:
				return owner.Location.Zone.Cells
				            .SelectMany(x => x.Characters)
				            .Except(owner)
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameShardOnly:
				return owner.Location.Shard.Cells
				            .SelectMany(x => x.Characters)
				            .Except(owner)
				            .Where(x => TargetFilterFunction(owner, x))
				            .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SamePlaneOnly:
				// TODO
				return Gameworld.Shards
				                .SelectMany(x => x.Cells.SelectMany(y => y.Characters))
				                .Except(owner)
				                .Where(x => TargetFilterFunction(owner, x))
				                .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SeenTargetOnly:
				return owner.SeenTargets
				            .OfType<ICharacter>()
				            .Concat(owner.Location.Characters)
				            .Where(x => TargetFilterFunction(owner, x)).GetFromItemListByKeyword(targetText, owner);
		}

		throw new ApplicationException("Unknown MagicPowerDistance in MagicPowerBase.AcquireTarget");
	}

	protected virtual bool TargetFilterFunction(ICharacter owner, ICharacter target)
	{
		return true;
	}

	public bool TargetIsInRange(ICharacter owner, ICharacter target, MagicPowerDistance distance)
	{
		switch (distance)
		{
			case MagicPowerDistance.AnyConnectedMind:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == target);
			case MagicPowerDistance.AnyConnectedMindOrConnectedTo:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == target) ||
				       owner.CombinedEffectsOfType<MindConnectedToEffect>().Any(x => x.OriginatorCharacter == target)
					;
			case MagicPowerDistance.SameLocationOnly:
				return owner.Location == target.Location;
			case MagicPowerDistance.AdjacentLocationsOnly:
				return owner.Location.CellsInVicinity(1, x => true, x => true)
				            .Any(x => x == target.Location);
			case MagicPowerDistance.SameAreaOnly:
				return owner.Location.Areas.Any(x => target.Location.Areas.Contains(x)) ||
				       owner.Location == target.Location;
			case MagicPowerDistance.SameZoneOnly:
				return owner.Location.Zone == target.Location.Zone;
			case MagicPowerDistance.SameShardOnly:
				return owner.Location.Shard == target.Location.Shard;
			case MagicPowerDistance.SamePlaneOnly:
				return true; // TODO
			case MagicPowerDistance.SeenTargetOnly:
				return owner.SeenTargets.Concat(owner.Location.Characters.Except(owner).Where(x => owner.CanSee(x)))
				            .Contains(target);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	#region Implementation of IMagicPower

	public IMagicSchool School { get; protected set; }

	public abstract string PowerType { get; }

	protected string _showHelpText;

	public virtual string ShowHelp(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.GetLineWithTitle(voyeur.InnerLineFormatLength, voyeur.Account.UseUnicode, Telnet.BoldMagenta,
			Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"School: {School.Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(_showHelpText.SubstituteANSIColour().Wrap(voyeur.InnerLineFormatLength));
		if (InvocationCosts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Costs to Use:");
			foreach (var verb in InvocationCosts)
			{
				if (verb.Value.Any())
				{
					sb.AppendLine(
						$"\t{verb.Key.Proper().Colour(School.PowerListColour)}: {verb.Value.Select(x => $"{x.Cost.ToString("N2", voyeur)} {x.Resource.ShortName}".ColourValue()).ListToString()}");
				}
				else
				{
					sb.AppendLine($"\t{verb.Key.Proper().Colour(School.PowerListColour)}: {"None".ColourValue()}");
				}
			}
		}

		return sb.ToString();
	}

	public abstract void UseCommand(ICharacter actor, string verb, StringStack command);

	protected bool HandleGeneralUseRestrictions(ICharacter actor)
	{
		if (actor.Movement != null)
		{
			actor.OutputHandler.Send("You cannot use that power while you are moving.");
			return false;
		}

		if (!actor.State.IsAble())
		{
			actor.OutputHandler.Send($"You cannot use that power while you are {actor.State.Describe()}.");
			return false;
		}

		if (actor.IsEngagedInMelee)
		{
			actor.OutputHandler.Send("You cannot use that power while you are in melee combat!");
			return false;
		}

		var (truth, blocks) = actor.IsBlocked("general");
		if (truth)
		{
			actor.OutputHandler.Send($"You cannot use that power because you are {blocks}.");
			return false;
		}

		return true;
	}

	public (bool Truth, IMagicResource missing) CanAffordToInvokePower(ICharacter actor, string verb)
	{
		var costs = InvocationCosts[verb];
		foreach (var (resource, cost) in costs)
		{
			if (actor.MagicResourceAmounts.FirstOrDefault(x => x.Key == resource).Value <= cost)
			{
				return (false, resource);
			}
		}

		return (true, null);
	}

	public void ConsumePowerCosts(ICharacter actor, string verb)
	{
		foreach (var (resource, cost) in InvocationCosts[verb])
		{
			actor.UseResource(resource, cost);
		}
	}

	public string Blurb { get; protected set; }
	public abstract IEnumerable<string> Verbs { get; }
	public IFutureProg CanInvokePowerProg { get; private set; }
	public IFutureProg WhyCantInvokePowerProg { get; private set; }

	public CollectionDictionary<string, (IMagicResource Resource, double Cost)> InvocationCosts { get; } = new();

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	public string Show(ICharacter actor)
	{
		throw new NotImplementedException();
	}

	public override void Save()
	{
		Changed = false;
		throw new NotImplementedException();
	}
	#endregion
}