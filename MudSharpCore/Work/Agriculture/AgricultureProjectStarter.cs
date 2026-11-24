#nullable enable

using MudSharp.Models;
using MudSharp.Work.Projects.ConcreteTypes;

namespace MudSharp.Work.Agriculture;

internal static class AgricultureProjectStarter
{
	public static bool TryStart(ICharacter? actor, IAgricultureField? field, IAgricultureOperation? operation,
		IFrameworkItem? target, out string reason)
	{
		if (actor is null || field is null || operation is null)
		{
			reason = "A character, field and agriculture operation are required.";
			return false;
		}

		if (actor.Location != field.Room)
		{
			reason = "You must be at the field to start work there.";
			return false;
		}

		if (!field.CanBeginOperation(actor, operation, target, out reason))
		{
			return false;
		}

		var project = operation.Project;
		if (project is null)
		{
			reason = "That agriculture operation has no project template configured.";
			return false;
		}

		if (!project.CanInitiateProject(actor, hasFieldContext: true))
		{
			reason = project.WhyCannotInitiateProject(actor, hasFieldContext: true);
			return false;
		}

		var context = new AgricultureProjectContext
		{
			AgricultureFieldId = field.Id,
			OperationId = operation.Id,
			TargetType = (int)operation.TargetType,
			TargetId = target?.Id,
			TargetText = target?.Name ?? string.Empty,
			ActorId = CharacterInstanceIdentityComparer.IdentityId(actor),
			Definition = "<Context />"
		};
		var active = new ActiveLocalProject(project, actor, context);
		actor.Gameworld.Add(active);
		actor.Gameworld.SaveManager.Flush();
		reason = string.Empty;
		return true;
	}
}
