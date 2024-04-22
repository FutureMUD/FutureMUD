using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes
{
	public class MarketGoodWeightGameItemComponentProto : GameItemComponentProto
	{
		public override string TypeDescription => "MarketGoodWeight";

		private readonly Dictionary<long, decimal> _marketMultipliers = new();

		public IReadOnlyDictionary<long,decimal> MarketMultipliers => _marketMultipliers;

		#region Constructors
		protected MarketGoodWeightGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "MarketGoodWeight")
		{
		}

		protected MarketGoodWeightGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
		{
		}

		protected override void LoadFromXml(XElement root)
		{
			foreach (var item in root.Element("Multipliers").Elements("Multiplier"))
			{
				_marketMultipliers.Add(long.Parse(item.Attribute("category").Value), decimal.Parse(item.Attribute("value").Value));
			}
		}
		#endregion

		#region Saving
		protected override string SaveToXml()
		{
			return new XElement("Definition",
					new XElement("Multipliers",
						from item in _marketMultipliers
						select new XElement("Multiplier",
							new XAttribute("category", item.Key),
							new XAttribute("value", item.Value)
						)
					)
				).ToString();
		}
		#endregion

		#region Component Instance Initialising Functions
		public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
		{
			return new MarketGoodWeightGameItemComponent(this, parent, temporary);
		}

		public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
		{
			return new MarketGoodWeightGameItemComponent(component, this, parent);
		}
		#endregion

		#region Initialisation Tasks
		public static void RegisterComponentInitialiser(GameItemComponentManager manager)
		{
			manager.AddBuilderLoader("MarketGoodWeight".ToLowerInvariant(), true, (gameworld, account) => new MarketGoodWeightGameItemComponentProto(gameworld, account));
			manager.AddDatabaseLoader("MarketGoodWeight", (proto, gameworld) => new MarketGoodWeightGameItemComponentProto(proto, gameworld));
			manager.AddTypeHelpInfo(
				"MarketGoodWeight",
				$"Specifies a multiplier for good satisfaction for #B[Market Categories]#0 for this item".SubstituteANSIColour(),
				BuildingHelpText
			);
		}

		public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
		{
			return CreateNewRevision(initiator, (proto, gameworld) => new MarketGoodWeightGameItemComponentProto(proto, gameworld));
		}
		#endregion

		#region Building Commands

		private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3multiplier <category> <%>#0 - sets the multiplier of a category
	#3remove <category>#0 - removes the multiplier for a category";

		public override string ShowBuildingHelp => BuildingHelpText;

		public override bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant().CollapseString())
			{
				case "multiplier":
					return BuildingCommandMultiplier(actor, command);
				case "remove":
					return BuildingCommandRemove(actor, command);
				default:
					return base.BuildingCommand(actor, command);
			}
		}

		private bool BuildingCommandRemove(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which market category would you like to remove a multiplier for?");
				return false;
			}

			var category = Gameworld.MarketCategories.GetByIdOrName(command.SafeRemainingArgument);
			if (category is null)
			{
				actor.OutputHandler.Send("There is no such market category.");
				return false;
			}

			if (_marketMultipliers.Remove(category.Id))
			{
				actor.OutputHandler.Send($"This item does not offer any multiplier for the {category.Name.ColourName()} market category.");
				return false;
			}

			actor.OutputHandler.Send($"This item will no longer offer any multiplier for the {category.Name.ColourName()} market category.");
			Changed = true;
			return true;
		}

		private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which market category would you like to set a multiplier for?");
				return false;
			}

			var category = Gameworld.MarketCategories.GetByIdOrName(command.PopSpeech());
			if (category is null)
			{
				actor.OutputHandler.Send("There is no such market category.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send($"What multiplier do you want to set for the {category.Name.ColourName()} category?");
				return false;
			}

			if (!command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var value))
			{
				actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
				return false;
			}

			_marketMultipliers.Remove(category.Id);
			_marketMultipliers.Add(category.Id, value);
			Changed = true;
			actor.OutputHandler.Send($"This item will now multiply the weight for the {category.Name.ColourName()} category by {value.ToString("P2", actor).ColourValue()}.");
			return true;
		}

		#endregion

		public override string ComponentDescriptionOLC(ICharacter actor)
		{
			return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis component multipliers how much the item is considered to fulfil a population need for a market category when purchased by NPCs compared to the base amount, with the following multipliers:\n\n{4}",
				"MarketGoodWeight Game Item Component".Colour(Telnet.Cyan),
				Id,
				RevisionNumber,
				Name,
				MarketMultipliers
					.Select(x => (Category: Gameworld.MarketCategories.Get(x.Key), Multiplier: x.Value))
					.Select(x => $"{x.Category.Name.ColourName()}: {x.Multiplier.ToString("P2", actor).ColourValue()}")
					.ListToLines(true)
				);
		}
	}
}
