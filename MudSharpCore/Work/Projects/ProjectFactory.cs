using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.Work.Projects.ConcreteTypes;
using MudSharp.Work.Projects.Impacts;
using MudSharp.Work.Projects.LabourRequirements;
using MudSharp.Work.Projects.MaterialRequirements;

namespace MudSharp.Work.Projects;

public static class ProjectFactory
{
	public static IProject CreateProject(IAccount builder, IFuturemud gameworld, string type)
	{
		switch (type.ToLowerInvariant())
		{
			case "personal":
				return new PersonalProject(builder);
			case "local":
				return new LocalProject(builder);
		}

		return null;
	}

	public static IProject LoadProject(MudSharp.Models.Project project, IFuturemud gameworld)
	{
		switch (project.Type)
		{
			case "personal":
				return new PersonalProject(project, gameworld);
			case "local":
				return new LocalProject(project, gameworld);
		}

		throw new NotImplementedException(
			$"The ProjectType type '{project.Type}' is not yet defined in ProjectFactory.LoadProject");
	}

	public static IEnumerable<string> ValidProjectTypes => new[] { "personal", "local" };

	public static IActiveProject LoadActiveProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld)
	{
		return gameworld.Projects.Get(project.ProjectId, project.ProjectRevisionNumber).LoadActiveProject(project);
	}

	public static IProjectLabourRequirement CreateLabour(IProjectPhase phase, IFuturemud gameworld, string type,
		string name)
	{
		switch (type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleProjectLabour(gameworld, phase, name);
			case "endless":
				return new EndlessProjectLabour(gameworld, phase, name);
			case "supervision":
				return new SupervisionProjectLabour(gameworld, phase, name);
		}

		return null;
	}

	public static IEnumerable<string> ValidLabourTypes => new[] { "simple", "endless", "supervision" };

	public static IProjectLabourRequirement LoadLabour(Models.ProjectLabourRequirement labour, IFuturemud gameworld)
	{
		switch (labour.Type)
		{
			case "simple":
				return new SimpleProjectLabour(labour, gameworld);
			case "endless":
				return new EndlessProjectLabour(labour, gameworld);
			case "supervision":
				return new SupervisionProjectLabour(labour, gameworld);
		}

		throw new NotImplementedException(
			$"The ProjectLabourRequirement type '{labour.Type}' is not yet defined in ProjectFactory.LoadLabour");
	}

	public static IProjectMaterialRequirement CreateMaterial(IProjectPhase phase, IFuturemud gameworld, string type)
	{
		switch (type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleProjectMaterial(gameworld, phase);
			case "commodity":
				return new CommodityProjectMaterial(gameworld, phase);
		}

		return null;
	}

	public static IEnumerable<string> ValidMaterialTypes => new[] { "simple", "commodity" };

	public static IProjectMaterialRequirement LoadMaterial(Models.ProjectMaterialRequirement material,
		IFuturemud gameworld)
	{
		switch (material.Type)
		{
			case "simple":
				return new SimpleProjectMaterial(material, gameworld);
			case "commodity":
				return new CommodityProjectMaterial(material, gameworld);
		}

		throw new NotImplementedException(
			$"The ProjectMaterialRequirement type '{material.Type}' is not yet defined in ProjectFactory.LoadMaterial");
	}

	public static IProjectAction CreateAction(IProjectPhase phase, IFuturemud gameworld, string type)
	{
		switch (type.ToLowerInvariant())
		{
			case "prog":
				return new Actions.ProgAction(phase, gameworld);
			case "skilluse":
				return new Actions.SkillUseAction(phase, gameworld);
		}

		return null;
	}

	public static IEnumerable<string> ValidActionTypes => new[] { "prog", "skilluse" };

	public static IProjectAction LoadAction(Models.ProjectAction action, IFuturemud gameworld)
	{
		switch (action.Type.ToLowerInvariant())
		{
			case "prog":
				return new Actions.ProgAction(action, gameworld);
			case "skilluse":
				return new Actions.SkillUseAction(action, gameworld);
		}

		throw new NotImplementedException(
			$"The ProjectAction type '{action.Type}' is not yet defined in ProjectFactory.LoadAction");
	}

	public static ILabourImpact CreateImpact(IProjectLabourRequirement requirement, IFuturemud gameworld, string type,
		string name)
	{
		switch (type.ToLowerInvariant())
		{
			case "trait":
				return new TraitImpact(requirement, name);
			case "healing":
				return new HealingImpact(requirement, name);
			case "job":
				return new JobEffortImpact(requirement, name);
			case "cap":
				return new TraitCapImpact(requirement, name);
		}

		return null;
	}

	public static IEnumerable<string> ValidImpactTypes => new[] { "trait", "healing", "job", "cap" };

	public static ILabourImpact LoadImpact(Models.ProjectLabourImpact impact, IFuturemud gameworld)
	{
		switch (impact.Type.ToLowerInvariant())
		{
			case "trait":
				return new TraitImpact(impact, gameworld);
			case "healing":
				return new HealingImpact(impact, gameworld);
			case "jobeffort":
				return new JobEffortImpact(impact, gameworld);
			case "cap":
				return new TraitCapImpact(impact, gameworld);
		}

		throw new NotImplementedException(
			$"The ProjectLabourImpact type '{impact.Type}' is not yet defined in ProjectFactory.LoadImpact");
	}
}