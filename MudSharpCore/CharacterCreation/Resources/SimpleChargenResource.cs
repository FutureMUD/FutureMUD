using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using System;
using ExpressionEngine;

namespace MudSharp.CharacterCreation.Resources;

/// <summary>
///     Simple resources that do not regenerate or change except when awarded, e.g. RPP
/// </summary>
public class SimpleChargenResource : ChargenResourceBase
{
	public SimpleChargenResource(IFuturemud gameworld, ChargenResource resource) : base(gameworld, resource)
	{
	}

	public SimpleChargenResource(IFuturemud gameworld, string name, string plural, string alias) : base(gameworld, name, plural, alias)
	{
		MaximumNumberAwardedPerAward = 1;
		MinimumTimeBetweenAwards = TimeSpan.FromDays(14);
		MaximumResourceExpression = new Expression("-1");
		DoDatabaseInsert("Simple");
	}

	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		// Do nothing
	}

	/// <inheritdoc />
	public override bool DisplayChangesOnLogin => true;

	/// <inheritdoc />
	public override string TypeName => "Simple";
}