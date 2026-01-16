#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Arenas;

/// <summary>
///     Marks a player as preparing for an arena event.
/// </summary>
public sealed class ArenaPreparingEffect : Effect, INoQuitEffect, INoTimeOutEffect
{
	private readonly long _arenaEventId;
	private IArenaEvent? _arenaEvent;

	public ArenaPreparingEffect(ICharacter owner, IArenaEvent arenaEvent) : base(owner)
	{
		if (owner is null)
		{
			throw new ArgumentNullException(nameof(owner));
		}

		_arenaEvent = arenaEvent ?? throw new ArgumentNullException(nameof(arenaEvent));
		_arenaEventId = arenaEvent.Id;
	}

	private ArenaPreparingEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		if (root is null)
		{
			throw new ArgumentNullException(nameof(root));
		}

		_arenaEventId = long.Parse(root.Element("ArenaEventId")?.Value
		                          ?? throw new ArgumentException("ArenaEventId element missing."));
		_arenaEvent = ResolveEvent();
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ArenaPreparing", (effect, owner) => new ArenaPreparingEffect(effect, owner));
	}

	public long ArenaEventId => _arenaEventId;

	public IArenaEvent? ArenaEvent => ResolveEvent();

	protected override string SpecificEffectType => "ArenaPreparing";

	public override bool SavingEffect => true;

	public string NoQuitReason =>
		$"You cannot quit while you are preparing for {DescribeEventName()}.";

	public string NoTimeOutReason =>
		$"You cannot time out while you are preparing for {DescribeEventName()}.";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Preparing for {DescribeEventName()}.";
	}

	internal bool Matches(IArenaEvent arenaEvent)
	{
		return arenaEvent is not null && arenaEvent.Id == _arenaEventId;
	}

	internal void AttachToEvent(IArenaEvent arenaEvent)
	{
		if (arenaEvent is null || arenaEvent.Id != _arenaEventId)
		{
			return;
		}

		_arenaEvent = arenaEvent;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition", new XElement("ArenaEventId", _arenaEventId));
	}

	private IArenaEvent? ResolveEvent()
	{
		if (_arenaEvent is not null)
		{
			return _arenaEvent;
		}

		_arenaEvent = Gameworld?.CombatArenas.SelectMany(x => x.ActiveEvents)
			.FirstOrDefault(x => x.Id == _arenaEventId);
		return _arenaEvent;
	}

	private string DescribeEventName()
	{
		var arenaEvent = ResolveEvent();
		if (arenaEvent is null)
		{
			return $"arena event #{_arenaEventId}";
		}

		return arenaEvent.Name;
	}
}
