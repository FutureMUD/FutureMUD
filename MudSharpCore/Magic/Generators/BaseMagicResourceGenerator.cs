using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Magic.Generators;

public abstract class BaseMagicResourceGenerator : SaveableItem, IMagicResourceRegenerator
{
	private Dictionary<IHaveMagicResource, HeartbeatManagerDelegate> _delegates =
		new();

	protected BaseMagicResourceGenerator(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
	}

	protected BaseMagicResourceGenerator(Models.MagicGenerator generator, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = generator.Id;
		_name = generator.Name;
	}

	public static IMagicResourceRegenerator LoadFromDatabase(Models.MagicGenerator generator, IFuturemud gameworld)
	{
		switch (generator.Type)
		{
			case "linear":
				return new LinearTimeBasedGenerator(generator, gameworld);
			case "state":
				return new StateGenerator(generator, gameworld);
		}

		throw new ApplicationException("Invalid MagicGenerator type: " + generator.Type);
	}

	public static IMagicResourceRegenerator LoadFromBuilderInput(ICharacter actor, StringStack command)
	{
		var type = command.PopSpeech().ToLowerInvariant();
		switch (type)
		{
			case "linear":
			case "state":
				break;
			default:
				actor.OutputHandler.Send("The valid types are #3linear#0 and #3state#0.".SubstituteANSIColour());
				return null;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What magic resource should this regenerator be tied to?");
			return null;
		}

		var resource = actor.Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such magic resource.");
			return null;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your regenerator?");
			return null;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MagicResourceRegenerators.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a regenerator with that name. Names must be unique.");
			return null;
		}

		switch (type)
		{
			case "linear":
				return new LinearTimeBasedGenerator(actor.Gameworld, name, resource);
			case "state":
				return new StateGenerator(actor.Gameworld, name, resource);
		}

		return null;
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "MagicResourceGenerator";

	#endregion

	#region Implementation of IMagicResourceRegenerator

	public abstract string RegeneratorTypeName { get; }

	public HeartbeatManagerDelegate GetOnMinuteDelegate(IHaveMagicResource thing)
	{
		if (!_delegates.ContainsKey(thing))
		{
			_delegates[thing] = InternalGetOnMinuteDelegate(thing);
		}

		return _delegates[thing];
	}

	protected abstract HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing);

	public abstract IEnumerable<IMagicResource> GeneratedResources { get; }

	public abstract IMagicResourceRegenerator Clone(string name);

	protected abstract XElement SaveDefinition();

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.MagicGenerators.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	private string HelpText => @$"You can use the following options with this command:

	#3name <name>#0 - rename this regenerator
{SubtypeHelpText}";

	protected abstract string SubtypeHelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this regenerator to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicResourceRegenerators.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a regenerator with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this regenerator from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public abstract string Show(ICharacter actor);

	#endregion
}