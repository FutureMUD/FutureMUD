using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class SpecialApplicationScreenStoryboard : ChargenScreenStoryboard
{
	private SpecialApplicationScreenStoryboard()
	{
	}

	protected SpecialApplicationScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(
		dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value.SubstituteANSIColour();
		AutomaticallyDoShortApplicationForAccountFirst = bool.Parse(definition.Element("AutomaticallyDoShortApplicationForAccountFirst")?.Value ?? "true");
	}

	protected override string StoryboardName => "SpecialApplication";

	public string Blurb { get; protected set; }

	public bool AutomaticallyDoShortApplicationForAccountFirst { get; protected set; }

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("AutomaticallyDoShortApplicationForAccountFirst", AutomaticallyDoShortApplicationForAccountFirst)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SpecialApplication,
			new ChargenScreenStoryboardFactory("SpecialApplication",
				(game, dbitem) => new SpecialApplicationScreenStoryboard(game, dbitem)),
			"SpecialApplication",
			"Select whether this is a special or normal application",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen allows people to select whether they are making a regular application (rules apply), a short application (skip some complex decisions) or a special application (rules relaxed). You can set up other decisions in chargen to check the IsSpecialApplication property of the chargen to otherwise ignore restrictions - the seeder sets things up like this to begin with."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		var resource = Gameworld.ChargenResources.Get(Gameworld.GetStaticLong("SpecialApplicationResource"));
		if (resource is null)
		{
			sb.AppendLine($"Special Application Cost: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				$"Special Application Cost: #2{Gameworld.GetStaticInt("SpecialApplicationCost").ToString("N0")} {resource.Alias}#0"
					.SubstituteANSIColour());
		}
		sb.AppendLine($"Account First Gets Short Mode: {AutomaticallyDoShortApplicationForAccountFirst.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class SpecialApplicationScreen : ChargenScreen
	{
		protected SpecialApplicationScreenStoryboard Storyboard;

		internal SpecialApplicationScreen(IChargen chargen, SpecialApplicationScreenStoryboard storyboard) : base(
			chargen, storyboard)
		{
			Storyboard = storyboard;
			using (new FMDB())
			{
				if (FMDB.Context.Characters.Count(x => x.AccountId == chargen.Account.Id) == 0)
				{
					if (storyboard.AutomaticallyDoShortApplicationForAccountFirst)
					{
						Chargen.ApplicationType = ApplicationType.Simple;
					}
					else
					{
						Chargen.ApplicationType = ApplicationType.Special;
					}
					State = ChargenScreenState.Complete;
				}
			}
			// TODO - automatically complete if account is banned from submitting special applications
		}

		#region Overrides of ChargenScreen

		public override ChargenStage AssociatedStage => ChargenStage.SpecialApplication;

		public override string Display()
		{
			return
				$@"{"Application Type".Colour(Telnet.Cyan)}

{Storyboard.Blurb.Wrap(Account.InnerLineFormatLength)}

Please enter your choice. Type {"simple".ColourCommand()}, {"normal".ColourCommand()} or {"special".ColourCommand()}.";
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			State = ChargenScreenState.Complete;

			if ("short".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.ApplicationType = ApplicationType.Simple;
				return "\n\nThis application is now a simple application.";
			}

			if ("normal".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.ApplicationType = ApplicationType.Normal;
				return "\n\nThis application is now a normal application.";
			}

			if ("special".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.ApplicationType = ApplicationType.Special;
				return "\n\nThis application is now a special application.";
			}

			return $"The valid choices are {"simple".ColourCommand()}, {"normal".ColourCommand()} or {"special".ColourCommand()}.";
		}

		#endregion
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3autofirst#0 - toggles automatic short mode for first applications";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "autofirst":
			case "auto":
			case "first":
				return BuildingCommandAutoFirst(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandAutoFirst(ICharacter actor)
	{
		AutomaticallyDoShortApplicationForAccountFirst = !AutomaticallyDoShortApplicationForAccountFirst;
		Changed = true;
		actor.OutputHandler.Send($"First time applications on new accounts will {AutomaticallyDoShortApplicationForAccountFirst.NowNoLonger()} be forced to use short mode.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		Blurb = text;
		Changed = true;
		handler.Send($"You set the blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion

	#region Overrides of ChargenScreenStoryboard

	public override ChargenStage Stage => ChargenStage.SpecialApplication;

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		if (chargen.ApplicationType != ApplicationType.Special)
		{
			return Enumerable.Empty<(IChargenResource, int )>();
		}

		var resource = Gameworld.ChargenResources.Get(Gameworld.GetStaticLong("SpecialApplicationResource"));
		if (resource == null)
		{
			return Enumerable.Empty<(IChargenResource, int )>();
		}

		return new[] { (resource, Gameworld.GetStaticInt("SpecialApplicationCost")) };
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SpecialApplicationScreen(chargen, this);
	}

	#endregion
}