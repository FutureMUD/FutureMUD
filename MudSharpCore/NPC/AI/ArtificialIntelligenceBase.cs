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

namespace MudSharp.NPC.AI;

public abstract class ArtificialIntelligenceBase : FrameworkItem, IArtificialIntelligence
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

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; }

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

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
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
}