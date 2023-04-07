using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Health.Surgery;

public class SurgicalProcedureFactory
{
	private SurgicalProcedureFactory()
	{
	}

	public static SurgicalProcedureFactory Instance { get; } = new();
	
	public ISurgicalProcedure CreateProcedureFromBuilderInput(IFuturemud gameworld, string name, string gerund,
		IBodyPrototype body, string school, IKnowledge knowledge, SurgicalProcedureType type)
	{
		switch (type)
		{
			case SurgicalProcedureType.Triage:
				return new TriageProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.DetailedExamination:
				return new MedicalExaminationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.InvasiveProcedureFinalisation:
				return new InvasiveProcedureFinalisation(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.ExploratorySurgery:
				return new ExploratorySurgery(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.Amputation:
				return new AmputationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.Replantation:
				return new ReplantationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.Cannulation:
				return new CannulationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.TraumaControl:
				return new TraumaControlProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.OrganExtraction:
				return new OrganExtractionProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.OrganTransplant:
				return new OrganTransplantProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.Decannulation:
				return new DecannulationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.OrganStabilisation:
				return new OrganStabilisationProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.SurgicalBoneSetting:
				return new SurgicalSettingProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.InstallImplant:
				return new InstallImplantProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.RemoveImplant:
				return new RemoveImplantProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.ConfigureImplantPower:
				return new ConfigureImplantPowerProcedure(gameworld, name, gerund, body, school, knowledge);
			case SurgicalProcedureType.ConfigureImplantInterface:
				return new ConfigureImplantInterfaceProcedure(gameworld, name, gerund, body, school, knowledge);
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public ISurgicalProcedure LoadProcedure(IFuturemud gameworld, MudSharp.Models.SurgicalProcedure procedure)
	{
		switch ((SurgicalProcedureType)procedure.Procedure)
		{
			case SurgicalProcedureType.DetailedExamination:
				return new MedicalExaminationProcedure(procedure, gameworld);
			case SurgicalProcedureType.Triage:
				return new TriageProcedure(procedure, gameworld);
			case SurgicalProcedureType.ExploratorySurgery:
				return new ExploratorySurgery(procedure, gameworld);
			case SurgicalProcedureType.InvasiveProcedureFinalisation:
				return new InvasiveProcedureFinalisation(procedure, gameworld);
			case SurgicalProcedureType.Amputation:
				return new AmputationProcedure(procedure, gameworld);
			case SurgicalProcedureType.Replantation:
				return new ReplantationProcedure(procedure, gameworld);
			case SurgicalProcedureType.TraumaControl:
				return new TraumaControlProcedure(procedure, gameworld);
			case SurgicalProcedureType.Cannulation:
				return new CannulationProcedure(procedure, gameworld);
			case SurgicalProcedureType.OrganExtraction:
				return new OrganExtractionProcedure(procedure, gameworld);
			case SurgicalProcedureType.OrganTransplant:
				return new OrganTransplantProcedure(procedure, gameworld);
			case SurgicalProcedureType.Decannulation:
				return new DecannulationProcedure(procedure, gameworld);
			case SurgicalProcedureType.OrganStabilisation:
				return new OrganStabilisationProcedure(procedure, gameworld);
			case SurgicalProcedureType.SurgicalBoneSetting:
				return new SurgicalSettingProcedure(procedure, gameworld);
			case SurgicalProcedureType.InstallImplant:
				return new InstallImplantProcedure(procedure, gameworld);
			case SurgicalProcedureType.RemoveImplant:
				return new RemoveImplantProcedure(procedure, gameworld);
			case SurgicalProcedureType.ConfigureImplantPower:
				return new ConfigureImplantPowerProcedure(procedure, gameworld);
			case SurgicalProcedureType.ConfigureImplantInterface:
				return new ConfigureImplantInterfaceProcedure(procedure, gameworld);
			default:
				throw new NotImplementedException();
		}
	}
}