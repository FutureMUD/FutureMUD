using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Work.Projects.MaterialRequirements;

public abstract class MaterialRequirementBase : SaveableItem, IProjectMaterialRequirement
{
	public sealed override string FrameworkItemType => "ProjectMaterialRequirement";

	protected MaterialRequirementBase(Models.ProjectMaterialRequirement requirement, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = requirement.Id;
		_name = requirement.Name;
		Description = requirement.Description;
		IsMandatoryForProjectCompletion = requirement.IsMandatoryForProjectCompletion;
	}

	protected MaterialRequirementBase(IFuturemud gameworld, IProjectPhase phase, string type)
	{
		Gameworld = gameworld;
		_name = phase.MaterialRequirements.Select(x => x.Name).NameOrAppendNumberToName("Material");
		Description = "Things required to complete the project.";
		IsMandatoryForProjectCompletion = true;

		using (new FMDB())
		{
			var dbitem = new Models.ProjectMaterialRequirement();
			FMDB.Context.ProjectMaterialRequirements.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.IsMandatoryForProjectCompletion = IsMandatoryForProjectCompletion;
			dbitem.ProjectPhaseId = phase.Id;
			dbitem.Type = type;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected MaterialRequirementBase(MaterialRequirementBase rhs, IProjectPhase newPhase, string type)
	{
		Gameworld = rhs.Gameworld;
		if (newPhase.MaterialRequirements.Contains(rhs))
		{
			_name = newPhase.MaterialRequirements.Select(x => x.Name).NameOrAppendNumberToName(rhs.Name);
		}
		else
		{
			_name = rhs.Name;
		}

		Description = rhs.Description;
		IsMandatoryForProjectCompletion = rhs.IsMandatoryForProjectCompletion;

		using (new FMDB())
		{
			var dbitem = new Models.ProjectMaterialRequirement();
			FMDB.Context.ProjectMaterialRequirements.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.IsMandatoryForProjectCompletion = IsMandatoryForProjectCompletion;
			dbitem.ProjectPhaseId = newPhase.Id;
			dbitem.Type = type;
			dbitem.Definition = rhs.SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected abstract XElement SaveDefinition();

	public override void Save()
	{
		var dbitem = FMDB.Context.ProjectMaterialRequirements.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.IsMandatoryForProjectCompletion = IsMandatoryForProjectCompletion;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	public bool IsMandatoryForProjectCompletion { get; protected set; }

	public abstract bool ItemCounts(IGameItem item);

	public abstract void SupplyItem(ICharacter actor, IGameItem item, IActiveProject project);

	public abstract void PeekSupplyItem(ICharacter actor, IGameItem item, IActiveProject project);

	public abstract string DescribeQuantity(ICharacter actor);

	public abstract double QuantityRequired { get; }
	public string Description { get; protected set; }

	private IInventoryPlanTemplate _inventoryPlanTemplate;

	public IInventoryPlan GetPlanForCharacter(ICharacter actor)
	{
		if (_inventoryPlanTemplate == null)
		{
			_inventoryPlanTemplate = new InventoryPlanTemplate(Gameworld,
				new[]
				{
					new InventoryPlanPhaseTemplate(1, new[]
					{
						LocateMaterialAction()
					})
				}
			);
		}

		return _inventoryPlanTemplate.CreatePlan(actor);
	}

	protected abstract IInventoryPlanAction LocateMaterialAction();

	public void Delete()
	{
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProjectMaterialRequirements.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProjectMaterialRequirements.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public abstract IProjectMaterialRequirement Duplicate(IProjectPhase newPhase);

	protected virtual string HelpText => @"You can use the following options with this material requirement:

	#3show#0 - shows detailed information about this material requirement
	#3name <name>#0 - renames this requirement
	#3description <description>#0 - sets the description";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command, phase);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "show":
			case "view":
				return BuildingCommandShow(actor, command, phase);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	protected abstract bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase);

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this material requirement?");
			return false;
		}

		Description = command.SafeRemainingArgument.SubstituteANSIColour().ProperSentences().Fullstop();
		actor.OutputHandler.Send(
			$"You change the description of material requirement {Name.ColourValue()} to {Description}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give o this material requirement?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (phase.MaterialRequirements.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a material requirement with that name. Names must be unique per phase.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the material requirement {_name.ColourValue()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public virtual (bool Truth, string Error) CanSubmit()
	{
		return (true, string.Empty);
	}

	public abstract string Show(ICharacter actor);
	public abstract string ShowToPlayer(ICharacter actor);
}