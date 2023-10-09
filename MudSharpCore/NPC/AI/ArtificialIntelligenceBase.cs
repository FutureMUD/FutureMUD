using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Database;

namespace MudSharp.NPC.AI;

public abstract class ArtificialIntelligenceBase : SaveableItem, IArtificialIntelligence
{
	private static readonly Dictionary<string, Func<ArtificialIntelligence, IFuturemud, IArtificialIntelligence>>
		_AITypeLoaders = new();

	protected ArtificialIntelligenceBase(ArtificialIntelligence ai, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = ai.Id;
		_name = ai.Name;
		AIType = ai.Type;
		RawXmlDefinition = ai.Definition;
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

	public static IArtificialIntelligence LoadIntelligence(ArtificialIntelligence ai, IFuturemud gameworld)
	{
		return _AITypeLoaders[ai.Type](ai, gameworld);
	}

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
	protected abstract string TypeHelpText { get; }
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
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Type: {AIType.ColourValue()}");
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