using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.RPG.Checks;
using System.Text;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A SimpleHearingProfile is a class implementing IHearingProfile that provides a flat, constant difficulty for
///     various proximities for the location
/// </summary>
public class SimpleHearingProfile : HearingProfile
{
	private readonly Dictionary<Tuple<AudioVolume, Proximity>, Difficulty> _difficultyMap =
		new();

        private Difficulty _defaultDifficulty;

        public SimpleHearingProfile(MudSharp.Models.HearingProfile profile)
                : base(profile)
        {
        }

        public SimpleHearingProfile(IFuturemud game, string name)
                : base(game, name, "Simple")
        {
                _defaultDifficulty = Difficulty.Automatic;
        }

        public override string FrameworkItemType => "SimpleHearingProfile";

        public override string Type => "Simple";

        public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
        {
                return _difficultyMap.ContainsKey(Tuple.Create(volume, proximity))
                        ? _difficultyMap[Tuple.Create(volume, proximity)]
                        : _defaultDifficulty;
        }

        protected override string SaveDefinition()
        {
                return new XElement("Definition",
                        new XElement("DefaultDifficulty", (int)_defaultDifficulty),
                        new XElement("Difficulties",
                                from item in _difficultyMap
                                select new XElement("Difficulty",
                                        new XAttribute("Volume", (int)item.Key.Item1),
                                        new XAttribute("Proximity", (int)item.Key.Item2),
                                        (int)item.Value)))
                        .ToString();
        }

        public override string HelpText => base.HelpText + @"

        #3default <difficulty>#0 - sets the default difficulty
        #3difficulty <volume> <proximity> <difficulty>#0 - sets a difficulty for a volume/proximity
        #3difficulty <volume> <proximity>#0 - removes a difficulty";

        public override bool BuildingCommand(ICharacter actor, StringStack command)
        {
                switch (command.Peek()?.ToLowerInvariant())
                {
                        case "default":
                                command.Pop();
                                return BuildingCommandDefault(actor, command);
                        case "difficulty":
                        case "diff":
                                command.Pop();
                                return BuildingCommandDifficulty(actor, command);
                        default:
                                return base.BuildingCommand(actor, command);
                }
        }

        private bool BuildingCommandDefault(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send("Which difficulty should be the default?");
                        return false;
                }

                if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var diff))
                {
                        actor.OutputHandler.Send($"{command.SafeRemainingArgument.ColourCommand()} is not a valid difficulty.");
                        return false;
                }

                _defaultDifficulty = diff;
                Changed = true;
                actor.OutputHandler.Send($"Default difficulty set to {diff.Describe().ColourValue()}.");
                return true;
        }

        private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
        {
                if (command.CountRemainingArguments() < 2)
                {
                        actor.OutputHandler.Send("You must specify a volume and proximity, and optionally a difficulty (or none to remove)." );
                        return false;
                }

                if (!command.PopSpeech().TryParseEnum<AudioVolume>(out var volume))
                {
                        actor.OutputHandler.Send("That is not a valid volume.");
                        return false;
                }

                if (!command.PopSpeech().TryParseEnum<Proximity>(out var prox))
                {
                        actor.OutputHandler.Send("That is not a valid proximity.");
                        return false;
                }

                if (command.IsFinished)
                {
                        if (_difficultyMap.Remove(Tuple.Create(volume, prox)))
                        {
                                Changed = true;
                                actor.OutputHandler.Send("Difficulty removed.");
                                return true;
                        }

                        actor.OutputHandler.Send("There was no such difficulty to remove.");
                        return false;
                }

                if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var diff))
                {
                        actor.OutputHandler.Send("That is not a valid difficulty.");
                        return false;
                }

                _difficultyMap[Tuple.Create(volume, prox)] = diff;
                Changed = true;
                actor.OutputHandler.Send($"Difficulty for {volume.Describe()} / {prox.Describe()} set to {diff.Describe().ColourValue()}.");
                return true;
        }

        public override string Show(ICharacter actor)
        {
                var sb = new StringBuilder();
                sb.Append(base.Show(actor));
                sb.AppendLine($"Default Difficulty: {_defaultDifficulty.Describe().ColourValue()}");
                foreach (var item in _difficultyMap)
                {
                        sb.AppendLine($"{item.Key.Item1.Describe(),-15} {item.Key.Item2.Describe(),-15} : {item.Value.Describe().ColourValue()}");
                }
                return sb.ToString();
        }

	public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		var root = XElement.Parse(profile.Definition);
		var element = root.Element("DefaultDifficulty");
		if (element != null)
		{
			_defaultDifficulty = (Difficulty)Convert.ToInt32(element.Value);
		}
		else
		{
			_defaultDifficulty = Difficulty.Automatic;
		}

		element = root.Element("Difficulties");
		if (element != null)
		{
			foreach (var sub in element.Elements("Difficulty"))
			{
				_difficultyMap.Add(
					Tuple.Create((AudioVolume)Convert.ToInt32(sub.Attribute("Volume").Value),
						(Proximity)Convert.ToInt32(sub.Attribute("Proximity").Value)),
					(Difficulty)Convert.ToInt32(sub.Value));
			}
		}
	}
}