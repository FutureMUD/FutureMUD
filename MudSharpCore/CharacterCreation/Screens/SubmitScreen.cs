﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation.Screens;

public class SubmitScreenStoryboard : ChargenScreenStoryboard
{
	private SubmitScreenStoryboard()
	{
	}

	public SubmitScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		AutomaticallyApproveAllApplicationsBelow =
			(PermissionLevel)int.Parse(definition.Element("AutomaticallyApproveAllApplicationsBelow")?.Value ?? "-1");
	}

	protected override string StoryboardName => "Submit";

	public override ChargenStage Stage => ChargenStage.Submit;

	public PermissionLevel AutomaticallyApproveAllApplicationsBelow { get; private set; }

	public override string HelpText => $@"{BaseHelpText}
	#3approve <level>#0 - automatically approve all apps of this level of below";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
				new XElement("AutomaticallyApproveAllApplicationsBelow", (int)AutomaticallyApproveAllApplicationsBelow)
			).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.Submit,
			new ChargenScreenStoryboardFactory("Submit", (game, dbitem) => new SubmitScreenStoryboard(game, dbitem)),
			"Submit",
			"Hardcoded application submission screen",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SubmitScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This is a hard-coded screen that allows a person to submit their character."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Automatically Approve All Apps Below: {AutomaticallyApproveAllApplicationsBelow.Describe().ColourValue()}");
		return sb.ToString();
	}

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "approve":
				return BuildingCommandApprove(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandApprove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What level of approval authority should applications at or below be automatically approved? The valid selections are {Enum.GetValues<PermissionLevel>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<PermissionLevel>(out var value))
		{
			actor.OutputHandler.Send($"That is not a valid value. The valid selections are {Enum.GetValues<PermissionLevel>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (value > actor.Account.Authority.Level)
		{
			actor.OutputHandler.Send("You can't set the minimum approval level higher than your own permission level.");
			return false;
		}

		AutomaticallyApproveAllApplicationsBelow = value;
		Changed = true;
		actor.OutputHandler.Send($"All applications with a minimum approval level of {value.Describe().ColourValue()} or less will now be automatically approved.");
		return true;
	}

	#endregion

	internal class SubmitScreen : ChargenScreen
	{
		private SubmitScreenStoryboard Storyboard;

		internal SubmitScreen(IChargen chargen, SubmitScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.Submit;

		public override string Display()
		{
			var costs = Chargen.ApplicationCosts;
			if (costs.Any(x => Chargen.Account.AccountResources[x.Key] < x.Value))
			{
				Chargen.SetStage(ChargenStage.Menu);
				return
					$"You may not submit this application because it costs more {costs.Where(x => Chargen.Account.AccountResources[x.Key] < x.Value).Select(x => x.Key.PluralName).ListToString()} than you have. You must go back and fix up your application first.\n{Chargen.Display()}";
			}

			if (!Chargen.CanSubmit)
			{
				Chargen.SetStage(ChargenStage.Menu);
				return
					$"You have not yet completed all stages, and so cannot submit your application!\n{Chargen.Display()}";
			}

			using (new FMDB())
			{
				var activeCount =
					(from character in FMDB.Context.Characters
					 where character.AccountId == Account.Id
					 where character.Status == (int)CharacterStatus.Active
					 select character).Count();

				var submittedCount = (from chargen in FMDB.Context.Chargens
				                      where chargen.AccountId == Account.Id
				                      where chargen.Status == (int)ChargenState.Submitted
				                      select chargen).Count();

				if (Account.ActiveCharactersAllowed <= activeCount + submittedCount)
				{
					Chargen.SetStage(ChargenStage.Menu);
					return
						$"Your account is only allowed {Account.ActiveCharactersAllowed} active or submitted {(Account.ActiveCharactersAllowed == 1 ? "character" : "characters")}, and you already have {activeCount + submittedCount}.";
				}
			}

			return
				$"{Chargen.DisplayForReview(Chargen.Account, PermissionLevel.Player)}\nDo you want to submit your application? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				State = ChargenScreenState.Complete;
				return "You submit your application.\n";
			}

			Chargen.SetStage(ChargenStage.Menu);
			return "You decline to submit your application\n\n" + Chargen.CurrentScreen.Display();
		}
	}
}