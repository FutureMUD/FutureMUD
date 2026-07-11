using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Agriculture;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Projects.Actions;

public class AgricultureOperationAction : BaseAction
{
	public AgricultureOperationAction(ProjectAction action, IFuturemud gameworld) : base(action, gameworld)
	{
	}

	public AgricultureOperationAction(IProjectPhase phase, IFuturemud gameworld) : base(phase, gameworld, "agriculture")
	{
		Description = "Apply an agriculture operation stored against this active project.";
		Changed = true;
	}

	public AgricultureOperationAction(AgricultureOperationAction rhs, IProjectPhase newPhase) : base(rhs, newPhase, "agriculture")
	{
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Action");
	}

	public override void CompleteAction(IActiveProject project)
	{
		using (new FMDB())
		{
			var context = FMDB.Context.AgricultureProjectContexts.Find(project.Id);
			if (context == null)
			{
				project.CharacterOwner?.OutputHandler.Send(
					"The agriculture project completed, but no agriculture context was found.");
				return;
			}

			var field = Gameworld.AgricultureFields.Get(context.AgricultureFieldId);
			var operation = Gameworld.AgricultureOperations.Get(context.OperationId);
			var target = ResolveTarget(context);
			if (field == null || operation == null)
			{
				project.CharacterOwner?.OutputHandler.Send(
					"The agriculture project completed, but its field or operation could not be found.");
				FMDB.Context.AgricultureProjectContexts.Remove(context);
				FMDB.Context.SaveChanges();
				return;
			}

			var actor = ResolveCompletionActor(project, context);
			var outcome = AgricultureProjectSkillTracker.OutcomeFor(context.Definition);
			var existingItems = new HashSet<IGameItem>(field.Cell.GameItems, ReferenceEqualityComparer.Instance);
			if (field.ApplyOperation(operation, target, actor, false, outcome, out var result))
			{
				if (project.CharacterOwner is not null)
				{
					ItemOwnershipService.AssignOwner(
						field.Cell.GameItems.Where(x => !existingItems.Contains(x)),
						project.CharacterOwner);
				}
				field.Cell.Handle(result);
			}
			else
			{
				field.Cell.Handle($"The {operation.Name.ColourName()} agriculture operation could not be applied: {result}");
			}

			FMDB.Context.AgricultureProjectContexts.Remove(context);
			FMDB.Context.SaveChanges();
		}
	}

	private ICharacter ResolveCompletionActor(IActiveProject project, AgricultureProjectContext context)
	{
		if (context.ActorId.HasValue)
		{
			var actor = Gameworld.TryGetCharacter(context.ActorId.Value, true);
			if (actor != null)
			{
				return actor;
			}
		}

		var worker = project.ActiveLabour.Select(x => x.Character).FirstOrDefault(x => x != null);
		return worker ?? project.CharacterOwner;
	}

	private IFrameworkItem ResolveTarget(AgricultureProjectContext context)
	{
		return (AgricultureTargetType)context.TargetType switch
		{
			AgricultureTargetType.Crop when context.TargetId.HasValue => Gameworld.AgricultureCropDefinitions.Get(context.TargetId.Value),
			AgricultureTargetType.Herd when context.TargetId.HasValue => Gameworld.AgricultureHerdDefinitions.Get(context.TargetId.Value),
			AgricultureTargetType.Woodland when context.TargetId.HasValue => Gameworld.AgricultureWoodlandDefinitions.Get(context.TargetId.Value),
			_ => null
		};
	}

	public override IProjectAction Duplicate(IProjectPhase newPhase)
	{
		return new AgricultureOperationAction(this, newPhase);
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append($"[{Name}] Agriculture Operation - {Description}");
		sb.Append(" ");
		sb.Append("Uses AgricultureProjectContext on the active project.".ColourCommand());
		return sb.ToString();
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Description;
	}

	protected override string HelpText => $@"{base.HelpText}

This action applies the agriculture operation stored on the active project's agriculture context.";
}
