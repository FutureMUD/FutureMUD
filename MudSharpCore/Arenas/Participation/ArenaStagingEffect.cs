#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;

namespace MudSharp.Arenas;

/// <summary>
///     Tracks arena signups waiting in staging and withdraws them if they leave during registration.
/// </summary>
public sealed class ArenaStagingEffect : Effect
{
	private readonly long _arenaEventId;
	private IArenaEvent? _arenaEvent;

	public ArenaStagingEffect(ICharacter owner, IArenaEvent arenaEvent) : base(owner)
	{
		if (owner is null)
		{
			throw new ArgumentNullException(nameof(owner));
		}

		_arenaEvent = arenaEvent ?? throw new ArgumentNullException(nameof(arenaEvent));
		_arenaEventId = arenaEvent.Id;
		SubscribeEvents();
	}

	private ArenaStagingEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		if (root is null)
		{
			throw new ArgumentNullException(nameof(root));
		}

		_arenaEventId = long.Parse(root.Element("ArenaEventId")?.Value
		                          ?? throw new ArgumentException("ArenaEventId element missing."));
		_arenaEvent = ResolveEvent();
		SubscribeEvents();
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ArenaStaging", (effect, owner) => new ArenaStagingEffect(effect, owner));
	}

	public long ArenaEventId => _arenaEventId;

	public IArenaEvent? ArenaEvent => ResolveEvent();

	protected override string SpecificEffectType => "ArenaStaging";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Staging for {DescribeEventName()}.";
	}

	public override void Login()
	{
		SubscribeEvents();
		EvaluateStatus();
	}

	public override void RemovalEffect()
	{
		UnsubscribeEvents();
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

	private void SubscribeEvents()
	{
		if (Owner is not ICharacter character)
		{
			return;
		}

		character.OnLocationChanged -= Character_OnLocationChanged;
		character.OnLocationChanged += Character_OnLocationChanged;
	}

	private void UnsubscribeEvents()
	{
		if (Owner is not ICharacter character)
		{
			return;
		}

		character.OnLocationChanged -= Character_OnLocationChanged;
	}

	private void Character_OnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		EvaluateStatus();
	}

	private void EvaluateStatus()
	{
		if (Owner is not ICharacter character)
		{
			return;
		}

		var arenaEvent = ResolveEvent();
		if (arenaEvent is null)
		{
			character.RemoveEffect(this, true);
			return;
		}

		if (arenaEvent.State != ArenaEventState.RegistrationOpen)
		{
			character.RemoveEffect(this, true);
			return;
		}

		if (character.Location is null)
		{
			return;
		}

		if (arenaEvent.Arena.WaitingCells.Contains(character.Location))
		{
			return;
		}

		try
		{
			arenaEvent.Withdraw(character);
			character.OutputHandler.Send(
				$"You leave the staging area and are withdrawn from {arenaEvent.Name.ColourName()}.".ColourError());
		}
		catch (Exception ex)
		{
			character.OutputHandler.Send(ex.Message.ColourError());
		}

		character.RemoveEffect(this, true);
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
