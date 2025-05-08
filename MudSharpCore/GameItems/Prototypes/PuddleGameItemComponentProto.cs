using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
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
using MudSharp.Construction;
using System.Numerics;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.GameItems.Groups;

namespace MudSharp.GameItems.Prototypes
{
	public class PuddleGameItemComponentProto : GameItemComponentProto
	{
		public override string TypeDescription => "Puddle";

		public override bool ReadOnly => true;
		public override bool PreventManualLoad => true;

		public static IGameItemProto ItemPrototype { get; set; }
		public static IGameItemGroup PuddleGroup { get; set; }
		public static IGameItemGroup BloodGroup { get; set; }
		public static IGameItemGroup ResidueGroup { get; set; }

		public static void InitialiseItemType(IFuturemud gameworld)
		{
			ItemPrototype = gameworld.ItemProtos.LastOrDefault(x => x.IsItemType<PuddleGameItemComponentProto>());
			if (ItemPrototype == null)
			{
				var comp = new PuddleGameItemComponentProto(gameworld, "Puddle", null);
				gameworld.Add(comp);
				comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
				var proto = new GameItemProto(gameworld, null);
				gameworld.Add(proto);
				proto.AddComponent(comp);
				proto.Weight = 0;
				proto.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
				proto.ReadOnly = true;
				ItemPrototype = proto;
			}

			// Stacking Groups
			if (gameworld.ItemGroups.All(x => !x.Name.EqualTo("Puddles")))
			{
				using (new FMDB())
				{
					var dbgroup = new Models.ItemGroup
					{
						Name = "Puddles",
						Keywords = "puddle liquid"
					};
					FMDB.Context.ItemGroups.Add(dbgroup);

					var dbform = new Models.ItemGroupForm
					{
						ItemGroup = dbgroup,
						Type = "Simple",
						Definition = new XElement("Definition",
							new XElement("Description", new XCData("There are numerous puddles of liquid around the room.")),
							new XElement("RoomDescription", new XCData("#6There are numerous puddles of liquid here.#0")),
							new XElement("ItemName", new XCData("puddle"))
						).ToString()
					};
					FMDB.Context.ItemGroupForms.Add(dbform);
					FMDB.Context.SaveChanges();

					PuddleGroup = new GameItemGroup(dbgroup, gameworld);
					gameworld.Add(PuddleGroup);

					
				}
			}
			else
			{
				PuddleGroup = gameworld.ItemGroups.First(x => x.Name.EqualTo("Puddles"));
			}

			if (gameworld.ItemGroups.All(x => !x.Name.EqualTo("Blood Splatters")))
			{
				using (new FMDB())
				{
					var dbblood = new Models.ItemGroup
					{
						Name = "Blood Splatters",
						Keywords = "blood splatter puddle"
					};
					FMDB.Context.ItemGroups.Add(dbblood);
					var dbbloodform = new Models.ItemGroupForm
					{
						ItemGroup = dbblood,
						Type = "Simple",
						Definition = new XElement("Definition",
							new XElement("Description", new XCData("There are numerous splatters of blood around the room.")),
							new XElement("RoomDescription", new XCData("#9There are numerous splatters of blood here.#0")),
							new XElement("ItemName", new XCData("splatter"))
						).ToString()
					};
					FMDB.Context.ItemGroupForms.Add(dbbloodform);
					FMDB.Context.SaveChanges();

					BloodGroup = new GameItemGroup(dbblood, gameworld);
					gameworld.Add(BloodGroup);
				}
			}
			else
			{
				BloodGroup = gameworld.ItemGroups.First(x => x.Name.EqualTo("Blood Splatters"));
			}

			if (gameworld.ItemGroups.All(x => !x.Name.EqualTo("Dried Liquid Residue")))
			{
				using (new FMDB())
				{
					var dbblood = new Models.ItemGroup
					{
						Name = "Dried Liquid Residue",
						Keywords = "dried residue liquid splatter"
					};
					FMDB.Context.ItemGroups.Add(dbblood);
					var dbbloodform = new Models.ItemGroupForm
					{
						ItemGroup = dbblood,
						Type = "Simple",
						Definition = new XElement("Definition",
							new XElement("Description", new XCData("There are numerous dried residues of liquid splatters here.")),
							new XElement("RoomDescription", new XCData("#6There are numerous dried residues of liquid splatters here.#0")),
							new XElement("ItemName", new XCData("residue"))
						).ToString()
					};
					FMDB.Context.ItemGroupForms.Add(dbbloodform);
					FMDB.Context.SaveChanges();

					ResidueGroup = new GameItemGroup(dbblood, gameworld);
					gameworld.Add(ResidueGroup);
				}
			}
			else
			{
				ResidueGroup = gameworld.ItemGroups.First(x => x.Name.EqualTo("Dried Liquid Residue"));
			}
		}

		#region Constructors

		private PuddleGameItemComponentProto(IFuturemud gameworld, string name, IAccount originator) : base(gameworld, name, originator)
		{
			Description = "Turns an item into a puddle";
			DoDatabaseInsert("Puddle");
		}

		protected PuddleGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
		{
		}

		protected override void LoadFromXml(XElement root)
		{
		}
		#endregion

		#region Saving
		protected override string SaveToXml()
		{
			return new XElement("Definition").ToString();
		}
		#endregion

		#region Component Instance Initialising Functions
		public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
		{
			return new PuddleGameItemComponent(this, parent, temporary);
		}

		public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
		{
			return new PuddleGameItemComponent(component, this, parent);
		}
		#endregion

		#region Initialisation Tasks
		public static void RegisterComponentInitialiser(GameItemComponentManager manager)
		{
			manager.AddDatabaseLoader("Puddle", (proto, gameworld) => new PuddleGameItemComponentProto(proto, gameworld));
			manager.AddTypeHelpInfo(
				"Puddle",
				$"Marks an item as a {"[system-generated]".Colour(Telnet.Green)} puddle of liquid. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
				"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
			);
		}

		public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
		{
			throw new NotSupportedException("Puddles should not be edited.");
		}
		#endregion

		public override string ComponentDescriptionOLC(ICharacter actor)
		{
			return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a puddle, a kind of system-generated temporary liquid container.",
				"Puddle Game Item Component".Colour(Telnet.Cyan),
				Id,
				RevisionNumber,
				Name
				);
		}

		public static IGameItem CreateNewPuddle(LiquidMixture mixture)
		{
			var newItem = ItemPrototype.CreateNew();
			var puddleItem = newItem.GetItemType<PuddleGameItemComponent>();
			puddleItem.LiquidMixture = mixture.Clone();
			newItem.Login();
			newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
			return newItem;
		}

		public static void CreateNewPuddle(LiquidMixture mixture, ICell location, RoomLayer layer,
			IPerceivable referenceItem)
		{
			if (location is null || location.IsSwimmingLayer(layer))
			{
				return;
			}

			var newItem = ItemPrototype.CreateNew();
			var puddleItem = newItem.GetItemType<PuddleGameItemComponent>();
			puddleItem.LiquidMixture = mixture.Clone();
			location.Gameworld.Add(newItem);
			newItem.RoomLayer = layer;
			location.Insert(newItem, true);
			newItem.PositionTarget = referenceItem;
			newItem.PositionModifier = Body.Position.PositionModifier.Around;
			newItem.Login();
			newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		}

		public static void TopUpOrCreateNewPuddle(LiquidMixture mixture, ICell location, RoomLayer layer,
			IPerceivable referenceItem)
		{
			var puddle = location.LayerGameItems(layer)
			                     .SelectNotNull(x => x.GetItemType<PuddleGameItemComponent>())
			                     .FirstOrDefault(x => x.Parent.PositionTarget == referenceItem);
			if (puddle is null)
			{
				if (location.Gameworld.GetStaticBool("PuddlesEnabled"))
				{
					CreateNewPuddle(mixture, location, layer, referenceItem);
				}

				return;
			}

			puddle.MergeLiquid(mixture, null, "");
		}
	}
}
