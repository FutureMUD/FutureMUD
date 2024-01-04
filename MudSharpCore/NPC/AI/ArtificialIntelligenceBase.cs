using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Database;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public abstract class ArtificialIntelligenceBase : SaveableItem, IArtificialIntelligence
{
	private static readonly Dictionary<string, Func<ArtificialIntelligence, IFuturemud, IArtificialIntelligence>>
		_AITypeLoaders = new();

	private static readonly Dictionary<string, Func<IFuturemud, string, IArtificialIntelligence>> _aiBuilderLoaders =
		new(StringComparer.InvariantCultureIgnoreCase);

	private static readonly Dictionary<string, string> _aiBuilderTypeHelps =
		new(StringComparer.InvariantCultureIgnoreCase);

	public virtual bool IsReadyToBeUsed => true;

	protected ArtificialIntelligenceBase(ArtificialIntelligence ai, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = ai.Id;
		_name = ai.Name;
		AIType = ai.Type;
		RawXmlDefinition = ai.Definition;
	}

	protected ArtificialIntelligenceBase(IFuturemud gameworld, string name, string type)
	{
		Gameworld = gameworld;
		_name = name;
		AIType = type;
	}

	/// <summary>
	/// This constructor is only to be used to make a disposable dummy instance for reading type help
	/// </summary>
	protected ArtificialIntelligenceBase()
	{

	}

	protected void DatabaseInitialise()
	{
		RawXmlDefinition = SaveToXml();
		using (new FMDB())
		{
			var dbitem = new Models.ArtificialIntelligence
			{
				Name = _name,
				Type = AIType,
				Definition = SaveToXml(),
			};
			FMDB.Context.ArtificialIntelligences.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IArtificialIntelligence Clone(string newName)
	{
		using (new FMDB())
		{
			var dbnew = new ArtificialIntelligence
			{
				Name = newName,
				Type = AIType,
				Definition = SaveToXml()
			};
			FMDB.Context.ArtificialIntelligences.Add(dbnew);
			FMDB.Context.SaveChanges();

			return LoadIntelligence(dbnew, Gameworld);
		}
	}

	public string RawXmlDefinition { get; protected set; }

	public string AIType { get; }

	public virtual bool CountsAsAggressive => false;

	#region Overrides of Item

	public sealed override string FrameworkItemType => "AI";

	#endregion

	protected static void RegisterAIType(string type,
		Func<ArtificialIntelligence, IFuturemud, IArtificialIntelligence> func)
	{
		_AITypeLoaders.Add(type, func);
	}

	protected static void RegisterAIBuilderInformation(string type,
		Func<IFuturemud, string, IArtificialIntelligence> loader, string help)
	{
		_aiBuilderLoaders.Add(type, loader);
		_aiBuilderTypeHelps.Add(type, help);
	}

	public static IArtificialIntelligence LoadIntelligence(ArtificialIntelligence ai, IFuturemud gameworld)
	{
		return _AITypeLoaders[ai.Type](ai, gameworld);
	}
#nullable enable
	public static IArtificialIntelligence? LoadFromBuilderInput(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which type of AI did you want to create? The valid types are:\n\n{_aiBuilderLoaders.Keys.Select(x => x.ColourName()).ListToLines(true)}");
			return null;
		}

		var type = command.PopSpeech();
		if (!_aiBuilderLoaders.ContainsKey(type))
		{
			actor.OutputHandler.Send($"That is not a valid AI type. The valid types are:\n\n{_aiBuilderLoaders.Keys.Select(x => x.ColourName()).ListToLines(true)}");
			return null;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new AI?");
			return null;
		}

		var name = command.PopSpeech().TitleCase();
		if (actor.Gameworld.AIs.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an AI with the name {name.ColourName()}. Names must be unique.");
			return null;
		}

		return _aiBuilderLoaders[type](actor.Gameworld, name);
	}
#nullable restore
	public static void SetupAI()
	{
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.IsSubclassOf(typeof(ArtificialIntelligenceBase))))
		{
			var method = type.GetMethod("RegisterLoader", BindingFlags.Public | BindingFlags.Static);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}
	}

	public bool IsGenerallyAble(ICharacter ch, bool ignoreMovement = false)
	{
		if (!ch.State.IsAble())
		{
			return false;
		}

		if (ch.Movement != null && !ignoreMovement)
		{
			return false;
		}

		if (!ch.CanMove() && !ignoreMovement)
		{
			return false;
		}

		return true;
	}

	#region IHandleEvents Members

	public abstract bool HandleEvent(EventType type, params dynamic[] arguments);

	public abstract bool HandlesEvent(params EventType[] types);

	public bool InstallHook(IHook hook)
	{
		return false;
	}

	public bool RemoveHook(IHook hook)
	{
		return false;
	}

	public bool HooksChanged
	{
		get => false;
		set { }
	}

	#endregion

	#region Implementation of IEditableItem
	protected virtual string TypeHelpText => "There is no type help for this type";
	public string HelpText => $@"You can use the following options to edit this AI:

	#3name <name>#0 - renames this AI
{TypeHelpText}";

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this AI?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.AIs.Any(x => x.Name.EqualTo(name))) {

			actor.OutputHandler.Send($"There is already an AI called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this AI from {_name.ColourName()} to {name.ColourName()}.");
		Changed = true;
		_name = name;
		return true;
	}

	/// <inheritdoc />
	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Definition:\n");
		sb.AppendLine(RawXmlDefinition.ColourCommand());
		return sb.ToString();
	}

	#endregion

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.ArtificialIntelligences.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveToXml();
		Changed = false;
	}

	protected abstract string SaveToXml();
}