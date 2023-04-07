using System;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using ExpressionEngine;

namespace MudSharp.CharacterCreation.Resources;

public abstract class ChargenResourceBase : FrameworkItem, IChargenResource
{
	private readonly Expression _maximumResourceExpression;

	private IChargenResource _maximumResourceReference;

	protected ChargenResourceBase(ChargenResource resource)
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
	}

	public static IChargenResource LoadFromDatabase(ChargenResource resource)
	{
		switch (resource.Type)
		{
			case "Simple":
				return new SimpleChargenResource(resource);
			case "Regenerating":
				return new RegeneratingChargenResource(resource);
			case "Playtime":
				return new TotalPlaytimeResource(resource);
			default:
				throw new NotSupportedException(
					"Unsupported ChargenResource type in ChargeResourceBase.LoadFromDatabase.");
		}
	}

	#region IChargenResource Members

	public void PerformPostLoadUpdate(ChargenResource resource, IFuturemud gameworld)
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

		return (int)_maximumResourceExpression.Evaluate();
	}

	#endregion
}