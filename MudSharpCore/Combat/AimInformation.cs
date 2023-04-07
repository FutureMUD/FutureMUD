using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.Combat;

public class AimInformation : IAimInformation
{
	public IPerceiver Shooter { get; set; }
	public IPerceiver Target { get; set; }
	private double _aimPercentage;

	public double AimPercentage
	{
		get => _aimPercentage;
		set => _aimPercentage = Math.Max(0.0, Math.Min(value, 1.0));
	}

	public IEnumerable<ICellExit> Path { get; set; }
	public IRangedWeapon Weapon { get; set; }

	public event EventHandler AimInvalidated;

	public AimInformation(IPerceiver target, IPerceiver shooter, IEnumerable<ICellExit> path, IRangedWeapon weapon)
	{
		Shooter = shooter;
		Target = target;
		Path = path;
		Weapon = weapon;
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		if (Target != null)
		{
			Target.OnQuit += Target_NoLongerValid;
			Target.OnDeleted += Target_NoLongerValid;
			Target.OnLocationChanged += Target_OnLocationChanged;
			if (Target is IMortalPerceiver mp)
			{
				mp.OnDeath += Target_NoLongerValid;
			}

			if (Target is IGameItem gi)
			{
				gi.OnRemovedFromLocation += Target_NoLongerValid;
			}
		}

		Weapon.Parent.OnDeath += WeaponDestroyed;
		Weapon.Parent.OnDeleted += WeaponLost;
		Weapon.Parent.OnQuit += WeaponLost;
		if (Shooter is ICharacter characterOwner)
		{
			characterOwner.OnMoved += CharacterOwner_OnMoved;
			characterOwner.Body.OnInventoryChange += WeaponInventoryStateChange;
		}

		RegisterPathEvents();
	}

	private void CharacterOwner_OnMoved(object sender, Movement.MoveEventArgs e)
	{
		var oldDistance = Path.Count();
		RecalculatePath();

		// Can't aim at the target any more
		if ((!Path.Any() && Shooter.Location != Target.Location) || !Shooter.CanSee(Target))
		{
			Shooter.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					"You stop aiming $2 at $1=0 because you have lost sight of &1 due to movement.", Shooter, Shooter,
					Target, Weapon.Parent)), OutputRange.Personal);
			Shooter.OutputHandler.Handle(new EmoteOutput(
				new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
				flags: OutputFlags.SuppressSource));
			ReleaseEvents();
		}

		if (Shooter is IPerceivableHaveTraits iphtShooter)
		{
			var outcomes = Shooter.Gameworld.GetCheck(CheckType.KeepAimTargetMoved).MultiDifficultyCheck(iphtShooter,
				Weapon.AimDifficulty, Target?.Cover?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic, Target,
				Weapon.WeaponType.FireTrait);
			var worstOutcome = outcomes.Item1.Outcome.Worst(outcomes.Item2);
			if (worstOutcome.IsFail())
			{
				Shooter.OutputHandler.Send("You lose some of your aim as you move.");
				AimPercentage -= 0.33 * worstOutcome.FailureDegrees();
			}
		}
	}

	private void RegisterPathEvents()
	{
		foreach (var exit in Path)
		{
			if (exit.Exit.Door != null)
			{
				exit.Exit.Door.OnClose += Door_OnClose;
				exit.Exit.Door.OnRemovedFromExit += Door_OnRemovedFromExit;
				exit.Exit.Door.Parent.OnDeath += Door_Destroyed;
				exit.Exit.Door.Parent.OnDeleted += Door_Destroyed;
				exit.Exit.Door.Parent.OnQuit += Door_Destroyed;
			}
		}
	}

	private void RecalculatePath()
	{
		ReleasePathEvents();
		Path = Shooter.PathBetween(Target, Weapon.WeaponType.DefaultRangeInRooms, false, false, true);
		if (Path.Any())
		{
			RegisterPathEvents();
		}
	}

	protected void WeaponDestroyed(IPerceivable perceivable)
	{
		Shooter.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because $2 has been destroyed!", Shooter, Shooter,
				Target, Weapon.Parent)));
		ReleaseEvents();
	}

	protected void WeaponLost(IPerceivable perceivable)
	{
		Shooter.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because #0 no longer have|has $2.", Shooter, Shooter,
				Target, Weapon.Parent)));
		ReleaseEvents();
	}

	protected void WeaponInventoryStateChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == Weapon.Parent && newState != InventoryState.Wielded)
		{
			Shooter.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because #0 are|is no longer wielding $2.",
					Shooter, Shooter, Target, Weapon.Parent)));
			ReleaseEvents();
		}
	}

	private void Target_OnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		if (exit != null)
		{
			Shooter.OutputHandler.Send($"Your target has moved to {exit.OutboundDirectionDescription}.");
		}

		var oldDistance = Path.Count();
		RecalculatePath();

		// Can't aim at the target any more
		if ((!Path.Any() && Shooter.Location != Target.Location) || !Shooter.CanSee(Target))
		{
			Shooter.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					$"You stop aiming $2 at $1=0 because you have lost sight of &1 due to movement.", Shooter, Shooter,
					Target, Weapon.Parent)), OutputRange.Personal);
			Shooter.OutputHandler.Handle(new EmoteOutput(
				new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
				flags: OutputFlags.SuppressSource));
			ReleaseEvents();
		}

		var movingTowards = oldDistance > Path.Count();
		if (Shooter is IPerceivableHaveTraits iphtShooter)
		{
			var outcomes = Shooter.Gameworld.GetCheck(CheckType.KeepAimTargetMoved).MultiDifficultyCheck(iphtShooter,
				Weapon.AimDifficulty,
				(Target as IPerceiver)?.Cover?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic, Target,
				Weapon.WeaponType.FireTrait);
			var worstOutcome = outcomes.Item1.Outcome.Worst(outcomes.Item2);
			if (worstOutcome.IsFail())
			{
				Shooter.OutputHandler.Send("You lose some of your aim as your target moves.");
				AimPercentage -= (movingTowards ? 0.15 : 0.33) * worstOutcome.FailureDegrees();
			}
		}
	}

	private void Door_Destroyed(IPerceivable owner)
	{
		var item = owner as IGameItem;
		item.OnDeath -= Door_Destroyed;
		item.OnDeleted -= Door_Destroyed;
		item.OnQuit -= Door_Destroyed;
		var door = item.GetItemType<IDoor>();
		Door_OnRemovedFromExit(door);
	}

	private void Door_OnRemovedFromExit(IDoor door)
	{
		door.OnRemovedFromExit -= Door_OnRemovedFromExit;
		door.OnClose -= Door_OnClose;
	}

	private void Door_OnClose(IOpenable openable)
	{
		var door = openable as IDoor;
		if (!door.CanFireThrough)
		{
			RecalculatePath();
			if ((!Path.Any() && Shooter.Location != Target.Location) || !Shooter.CanSee(Target))
			{
				Shooter.OutputHandler.Handle(
					new EmoteOutput(new Emote(
						$"You stop aiming $2 at $1=0 because you have lost sight of &1 due to a door closing.", Shooter,
						Shooter, Target, Weapon.Parent)), OutputRange.Personal);
				Shooter.OutputHandler.Handle(new EmoteOutput(
					new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
					flags: OutputFlags.SuppressSource));
				ReleaseEvents();
			}
		}
	}

	private void Target_NoLongerValid(IPerceivable owner)
	{
		Shooter.OutputHandler.Handle(
			new EmoteOutput(new Emote("You stop aiming $2 at $1=0 because you have lost sight of &1.", Shooter, Shooter,
				Target, Weapon.Parent)), OutputRange.Personal);
		Shooter.OutputHandler.Handle(new EmoteOutput(
			new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
			flags: OutputFlags.SuppressSource));
		ReleaseEvents();
	}

	private void ReleasePathEvents()
	{
		foreach (var exit in Path)
		{
			if (exit.Exit.Door != null)
			{
				exit.Exit.Door.OnClose -= Door_OnClose;
				exit.Exit.Door.OnRemovedFromExit -= Door_OnRemovedFromExit;
				exit.Exit.Door.Parent.OnDeath -= Door_Destroyed;
				exit.Exit.Door.Parent.OnDeleted -= Door_Destroyed;
				exit.Exit.Door.Parent.OnQuit -= Door_Destroyed;
			}
		}
	}

	private bool _eventsReleased;

	public void ReleaseEvents()
	{
		if (!_eventsReleased)
		{
			_eventsReleased = true;
			AimInvalidated?.Invoke(this, null);
			if (Target != null)
			{
				Target.OnQuit -= Target_NoLongerValid;
				Target.OnDeleted -= Target_NoLongerValid;
				Target.OnLocationChanged -= Target_OnLocationChanged;
				if (Target is IMortalPerceiver mp)
				{
					mp.OnDeath -= Target_NoLongerValid;
				}

				if (Target is IGameItem gi)
				{
					gi.OnRemovedFromLocation -= Target_NoLongerValid;
				}
			}

			Weapon.Parent.OnDeath -= WeaponDestroyed;
			Weapon.Parent.OnDeleted -= WeaponLost;
			Weapon.Parent.OnQuit -= WeaponLost;
			if (Shooter is ICharacter characterOwner)
			{
				characterOwner.OnMoved -= CharacterOwner_OnMoved;
				characterOwner.Body.OnInventoryChange -= WeaponInventoryStateChange;
			}

			ReleasePathEvents();
		}
	}

	~AimInformation()
	{
		if (!_eventsReleased)
		{
			ReleaseEvents();
		}
	}
}