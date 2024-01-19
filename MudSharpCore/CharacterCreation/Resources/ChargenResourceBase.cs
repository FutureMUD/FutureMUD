using System;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using ExpressionEngine;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation.Resources;

public abstract class ChargenResourceBase : FrameworkItem, IChargenResource
{
	public sealed override string FrameworkItemType => "ChargenResource";

	private readonly Expression _maximumResourceExpression;

	private IChargenResource _maximumResourceReference;
	protected IFutureProg ControlProg { get; private set; }

	protected ChargenResourceBase(IFuturemud gameworld, ChargenResource resource)
	{
		_id = resource.Id;
		_name = resource.Name;
		PluralName = resource.PluralName;
		Alias = resource.Alias;
		MinimumTimeBetweenAwards = TimeSpan.FromMinutes(resource.MinimumTimeBetweenAwards);
		MaximumNumberAwardedPerAward = resource.MaximumNumberAwardedPerAward;
		PermissionLevelRequiredToAward = (PermissionLevel)resource.PermissionLevelRequiredToAward;
		PermissionLevelRequiredToCircumventMinimumTime =
			(PermissionLevel)resource.PermissionLevelRequiredToCircumventMinimumTime;
		ShowToPlayerInScore = resource.ShowToPlayerInScore;
		TextDisplayedToPlayerOnAward = resource.TextDisplayedToPlayerOnAward;
		TextDisplayedToPlayerOnDeduct = resource.TextDisplayedToPlayerOnDeduct;
		_maximumResourceExpression = new Expression(resource.MaximumResourceFormula);
		ControlProg = gameworld.FutureProgs.Get(resource.ControlProgId ?? 0);
	}

	public static IChargenResource LoadFromDatabase(IFuturemud gameworld, ChargenResource resource)
	{
		switch (resource.Type)
		{
			case "Simple":
				return new SimpleChargenResource(gameworld, resource);
			case "Regenerating":
				return new RegeneratingChargenResource(gameworld, resource);
			case "Playtime":
				return new TotalPlaytimeResource(gameworld, resource);
			case "Realtime":
				return new RealtimeRegeneratingResource(gameworld, resource);
			default:
				throw new NotSupportedException(
					"Unsupported ChargenResource type in ChargeResourceBase.LoadFromDatabase.");
		}
	}

	#region IChargenResource Members

	public virtual void PerformPostLoadUpdate(ChargenResource resource, IFuturemud gameworld)
	{
		if (resource.MaximumResourceId.HasValue)
		{
			_maximumResourceReference = gameworld.ChargenResources.Get(resource.MaximumResourceId.Value);
		}
	}

	public string PluralName { get; protected set; }

	public string Alias { get; protected set; }

	public TimeSpan MinimumTimeBetweenAwards { get; protected set; }

	public double MaximumNumberAwardedPerAward { get; protected set; }

	public PermissionLevel PermissionLevelRequiredToAward { get; protected set; }

	public PermissionLevel PermissionLevelRequiredToCircumventMinimumTime { get; protected set; }

	public abstract void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes);

	public bool ShowToPlayerInScore { get; protected set; }

	public string TextDisplayedToPlayerOnAward { get; protected set; }

	public string TextDisplayedToPlayerOnDeduct { get; protected set; }

	public int GetMaximum(IAccount account)
	{
		if (_maximumResourceReference != null)
		{
			_maximumResourceExpression.Parameters["variable"] = account.AccountResources[_maximumResourceReference];
		}

		return Convert.ToInt32(_maximumResourceExpression.Evaluate());
	}

	#endregion
}