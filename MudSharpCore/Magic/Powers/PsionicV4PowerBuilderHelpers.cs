#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

internal static class PsionicV4PowerBuilderHelpers
{
	public static IMagicPower? BuildWithSkill(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command, Func<ITraitDefinition, IMagicPower> builder)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
			return null;
		}

		var skill = gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("There is no such skill or attribute.");
			return null;
		}

		return builder(skill);
	}
}
