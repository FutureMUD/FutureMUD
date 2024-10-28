using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Products;
internal class NPCProduct : BaseProduct
{
	internal class NPCProductData : ICraftProductData
	{
		/// <inheritdoc />
		public IPerceivable Perceivable => new PerceivableGroup(from item in CharacterTemplates select new DummyPerceivable(item.SelectedSdesc));

		public List<SimpleCharacterTemplate> CharacterTemplates { get; set; }

		public INPCTemplate NPCTemplate { get; set; }

		public IFutureProg OnLoadProg { get; set; }

		/// <inheritdoc />
		public XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("NPCTemplate", NPCTemplate.Id),
				new XElement("OnLoadProg", OnLoadProg?.Id ?? 0),
				new XElement("NPCS", from item in CharacterTemplates select item.SaveToXml())
			);
		}

		/// <inheritdoc />
		public void FinaliseLoadTimeTasks()
		{
			// Do nothing
		}

		/// <inheritdoc />
		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			foreach (var template in CharacterTemplates)
			{
				var newCharacter = new NPC.NPC(location.Gameworld, template with { SelectedStartingLocation = location }, NPCTemplate)
				{
					RoomLayer = layer
				};
				NPCTemplate.OnLoadProg?.Execute(newCharacter);
				OnLoadProg?.Execute(newCharacter);
				if (newCharacter.Location.IsSwimmingLayer(newCharacter.RoomLayer) && newCharacter.Race.CanSwim)
				{
					newCharacter.PositionState = PositionSwimming.Instance;
				}
				else if (newCharacter.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && newCharacter.CanFly().Truth)
				{
					newCharacter.PositionState = PositionFlying.Instance;
				}

				location.Login(newCharacter);
				newCharacter.HandleEvent(EventType.NPCOnGameLoadFinished, newCharacter);
			}
		}

		/// <inheritdoc />
		public void Delete()
		{
		}

		/// <inheritdoc />
		public void Quit()
		{
		}
	}

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("NPCProduct",
			(product, craft, game) => new NPCProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("npc",
			(craft, game, fail) => new NPCProduct(craft, game, fail));
	}

	/// <inheritdoc />
	public NPCProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		_npcId = long.Parse(root.Element("Template").Value);
		OnLoadProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnLoadProg").Value));
	}

	/// <inheritdoc />
	public NPCProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Quantity = 1;
	}

	/// <inheritdoc />
	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality)
	{
		var templates = new List<SimpleCharacterTemplate>();
		for (var i = 0; i < Quantity; i++)
		{
			templates.Add((SimpleCharacterTemplate)NPCTemplate.GetCharacterTemplate());
		}
		return new NPCProductData { 
			CharacterTemplates = templates, 
			NPCTemplate = NPCTemplate,
			OnLoadProg = OnLoadProg
		};
	}

	/// <inheritdoc />
	public override bool IsValid()
	{
		return _npcId != 0 && NPCTemplate?.Status == RevisionStatus.Current;
	}

	/// <inheritdoc />
	public override string WhyNotValid()
	{
		if (_npcId == 0)
		{
			return "You must first set an NPC for this product to load.";
		}

		var npc = NPCTemplate;
		if (npc is null)
		{
			return "Couldn't find the NPC associated with the set ID. It might have been deleted.";
		}

		if (npc.Status != RevisionStatus.Current)
		{
			return $"The NPC Template {npc.ReferenceDescription(new DummyPerceiver())} is not approved for use.";
		}

		return "You can't do that for an unknown reason.";
	}

	/// <inheritdoc />
	public override string HowSeen(IPerceiver voyeur)
	{
		return $"{Quantity.ToString("N0", voyeur)}x {NPCTemplate?.ReferenceDescription(voyeur)}{(OnLoadProg is not null ? $" [OnLoad: {OnLoadProg.MXPClickableFunctionName()}]" : "")}";
	}

	/// <inheritdoc />
	public override string ProductType => "NPCProduct";

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return 
			new XElement("Definition",
				new XElement("Quantity", Quantity),
				new XElement("Template", NPCTemplate?.Id ?? 0),
				new XElement("OnLoadProg", OnLoadProg?.Id ?? 0)
			)
			.ToString();
	}

	private long _npcId;

	public INPCTemplate NPCTemplate => Gameworld.NpcTemplates.Get(_npcId);

	public int Quantity { get; set; }

	public IFutureProg OnLoadProg { get; set; }

	/// <inheritdoc />
	public override string Name => $"{Quantity}x {NPCTemplate?.ReferenceDescription(new DummyPerceiver())}{(OnLoadProg is not null ? $" [OnLoad: {OnLoadProg.MXPClickableFunctionName()}]" : "")}";

	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3quantity <number>#0 - the number of NPCs to load
	#3template <id>#0 - the NPC Template to load
	#3prog <prog>#0 - a prog to execute on the loaded NPC
	#3prog none#0 - clears any on load prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "quantity":
				return BuildingCommandQuantity(actor, command);
			case "template":
			case "npc":
			case "mob":
				return BuildingCommandTemplate(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
		}
		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog to run on the loaded NPC or use #3none#0 to remove it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			OnLoadProg = null;
			ProductChanged = true;
			actor.OutputHandler.Send("This product will no longer execute any prog on the loaded NPCs.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Void, [ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnLoadProg = prog;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now execute the {prog.MXPClickableFunctionName()} prog after loading the NPCs.");
		return true;
	}

	private bool BuildingCommandTemplate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which NPC Template would you like to use?");
			return false;
		}

		var template = Gameworld.NpcTemplates.GetByRevisableId(command.SafeRemainingArgument);
		if (template is null)
		{
			actor.OutputHandler.Send("There is no such NPC Template.");
			return false;
		}

		if (template.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"The NPC Template {template.EditHeader()} is not approved for use.");
			return false;
		}

		_npcId = template.Id;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now produce the {template.EditHeader()} NPC.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of this item to be loaded.");
			return false;
		}

		Quantity = value;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now produce {Quantity} of the target NPC.");
		return true;
	}

}
