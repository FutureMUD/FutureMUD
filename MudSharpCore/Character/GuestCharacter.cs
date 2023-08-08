using System;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Character;

public class GuestCharacter : Character
{
	private static ICell _guestLoungeCell;

	public GuestCharacter(ICharacterTemplate template, IFuturemud gameworld) : base(gameworld, template)
	{
		Register(new NonPlayerOutputHandler());
	}

	public GuestCharacter(MudSharp.Models.Character character, IFuturemud gameworld, bool temporary = false)
		: base(character, gameworld, temporary)
	{
		Register(new NonPlayerOutputHandler());
		State = CharacterState.Awake;
		SetPosition(PositionStanding.Instance, PositionModifier.None, null, null);
	}

	public static ICell GuestLoungeCell
	{
		get
		{
			if (_guestLoungeCell == null)
			{
				var gameworld = Futuremud.Games.First();
				try
				{
					_guestLoungeCell =
						gameworld.Cells.Get(gameworld.GetStaticLong("GuestLoungeCell"));
					if (_guestLoungeCell is not null)
					{
						_guestLoungeCell.CellProposedForDeletion -= GuestLoungeCell_CellProposedForDeletion;
						_guestLoungeCell.CellProposedForDeletion += GuestLoungeCell_CellProposedForDeletion;
					}
				}
				catch (Exception)
				{
					return null;
				}
			}

			return _guestLoungeCell;
		}
	}

	private static void GuestLoungeCell_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason("That room is the guest lounge cell");
	}

	public static ICharacter GetRandomGuestCharacter(IFuturemud gameworld)
	{
		return gameworld.Guests.Where(x => !gameworld.Characters.Contains(x)).GetRandomElement();
	}

	public override object DatabaseInsert()
	{
		var result = base.DatabaseInsert();
		var dbitem = new Guest();
		FMDB.Context.Guests.Add(dbitem);
		dbitem.Character = (MudSharp.Models.Character)result;
		return result;
	}

	#region Overrides of Character

	public override bool IsGuest => true;

	#region Overrides of Character

	public override IGameItem Die()
	{
		// Guest avatars don't die
		OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					"@ are|is slain, and explode|explodes into a million tiny shards of light before disappearing completely.",
					this)));
		OutputHandler.Send("\n");
		OutputHandler.QuietMode = true;
		Location?.Leave(this);
		Quit();
		Controller.SetContext(_nextContext);
		_nextContext = null;
		Location = null;
		OutputHandler.QuietMode = false;
		return null;
	}

	#region Overrides of Character

	public override bool Quit(bool silent = false)
	{
		OutputHandler.QuietMode = true;
		State = CharacterState.Awake;
		SetPosition(PositionStanding.Instance, PositionModifier.None, null, null);
		Body.HeldBreathTime = TimeSpan.Zero;
		foreach (var part in Body.SeveredRoots.ToList())
		{
			Body.RestoreBodypart(part);
		}

		Body.Sober();
		Body.CureAllWounds();
		Body.CurrentStamina = Body.MaximumStamina;
		Body.CurrentBloodVolumeLitres = Body.TotalBloodVolumeLitres;
		Body.EndHealthTick();
		base.Quit(silent);
		foreach (var item in Body.ExternalItems)
		{
			item.RemoveAllEffects();
		}

		EffectHandler.RemoveAllEffects();
		Body.RemoveAllEffects();
		OutputHandler.QuietMode = false;
		return true;
	}

	#endregion

	#endregion

	#endregion
}