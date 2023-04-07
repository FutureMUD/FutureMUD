using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class Searching : CharacterAction
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression
	{
		get
		{
			if (_effectDurationDiceExpression == null)
			{
				_effectDurationDiceExpression = Futuremud.Games.First()
				                                         .GetStaticConfiguration(
					                                         "SearchingEffectDurationDiceExpression");
			}

			return _effectDurationDiceExpression;
		}
	}

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	public Difficulty CurrentDifficulty { get; private set; }
	private readonly Dictionary<IPerceivable, int> _searchHitsDictionary = new();

	public Searching(ICharacter owner) : base(owner)
	{
		CurrentDifficulty = Difficulty.Impossible.StageDown(1);
		Action = SearchAction;
		ActionDescription = "searching the area";
		_blocks.Add("general");
		_blocks.Add("movement");
		CancelEmoteString = $"@ stop|stops searching the area";
		WhyCannotMoveEmoteString = $"@ cannot move because #0 are|is searching the area";
		LDescAddendum = "searching the area";
		SetupEventHandlers();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Searching the area at {CurrentDifficulty.Describe().Colour(Telnet.Cyan)} difficulty.";
	}

	protected override string SpecificEffectType => "Searching";

	public override void ExpireEffect()
	{
		if ((bool?)ApplicabilityProg?.Execute(Owner, null, null) ?? true)
		{
			Action(Owner);
		}

		CharacterOwner.Reschedule(this, EffectDuration);
	}

	private void SearchAction(IPerceivable dummy)
	{
		var check = Gameworld.GetCheck(CheckType.ActiveSearchCheck);
		var hiders = CharacterOwner.Location.Characters.Except(CharacterOwner).Where(x =>
			x.EffectsOfType<IHideEffect>().Any() && !CharacterOwner.AffectedBy<SawHider>(x)).ToList();
		var hiddenItems = CharacterOwner.Location.LayerGameItems(CharacterOwner.RoomLayer).Where(x =>
			x.EffectsOfType<IItemHiddenEffect>().Any() && !CharacterOwner.AffectedBy<SawHiddenItem>(x)).ToList();
		var hitsPerSkill = Gameworld.GetStaticDouble("ActiveSearchRequiredHitsPerSkill");
		CharacterOwner.OutputHandler.Send("You continue searching the area...");
		var result = check.Check(CharacterOwner, CurrentDifficulty);

		foreach (var tch in hiders)
		{
			if (!_searchHitsDictionary.ContainsKey(tch))
			{
				var skill = tch.EffectsOfType<IHideEffect>().Select(x => x.EffectiveHideSkill).Max();
				_searchHitsDictionary[tch] = (int)(skill * hitsPerSkill);
			}

			if ((_searchHitsDictionary[tch] -= result.SuccessDegrees()) <= 0)
			{
				if (!CharacterOwner.CanSee(tch, PerceiveIgnoreFlags.IgnoreHiding))
				{
					continue;
				}

				CharacterOwner.AddEffect(new SawHider(CharacterOwner, tch), TimeSpan.FromSeconds(30));
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote("You find $0!", CharacterOwner, tch)));
			}
		}

		foreach (var item in hiddenItems)
		{
			if (!_searchHitsDictionary.ContainsKey(item))
			{
				var skill = item.EffectsOfType<IItemHiddenEffect>().Select(x => x.EffectiveHideSkill).Max();
				_searchHitsDictionary[item] = (int)(skill * hitsPerSkill);
			}

			if ((_searchHitsDictionary[item] -= result.SuccessDegrees()) <= 0 || item.EffectsOfType<IItemHiddenEffect>()
				    .Any(x => x.KnewOriginalHidingPlace(CharacterOwner)))
			{
				if (!CharacterOwner.CanSee(item, PerceiveIgnoreFlags.IgnoreHiding))
				{
					continue;
				}

				CharacterOwner.AddEffect(new SawHiddenItem(CharacterOwner, item), TimeSpan.FromSeconds(30));
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote("You find $0!", CharacterOwner, item)));
			}
		}

		var newDifficulty = CurrentDifficulty.StageDown(1);
		if (newDifficulty >= CharacterOwner.Location.SpotDifficulty(CharacterOwner))
		{
			CurrentDifficulty = newDifficulty;
		}
	}
}