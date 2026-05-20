using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class AgricultureFieldProfile
	{
		public AgricultureFieldProfile()
		{
			AgricultureFields = new HashSet<AgricultureField>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Definition { get; set; }

		public virtual ICollection<AgricultureField> AgricultureFields { get; set; }
	}

	public partial class AgricultureCropDefinition
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string Definition { get; set; }
	}

	public partial class AgricultureHerdDefinition
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Definition { get; set; }
		public long? NpcTemplateId { get; set; }
		public int? NpcTemplateRevisionNumber { get; set; }

		public virtual NpcTemplate NpcTemplate { get; set; }
	}

	public partial class AgricultureWoodlandDefinition
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string WoodlandType { get; set; }
		public string Definition { get; set; }
	}

	public partial class AgricultureOperation
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int OperationType { get; set; }
		public int TargetType { get; set; }
		public int RequiredUse { get; set; }
		public int ResultUse { get; set; }
		public long ProjectId { get; set; }
		public int ProjectRevisionNumber { get; set; }
		public long? CompletionProgId { get; set; }
		public string Definition { get; set; }

		public virtual FutureProg CompletionProg { get; set; }
		public virtual Project Project { get; set; }
	}

	public partial class AgricultureField
	{
		public AgricultureField()
		{
			AgricultureFieldHerds = new HashSet<AgricultureFieldHerd>();
			AgricultureProjectContexts = new HashSet<AgricultureProjectContext>();
		}

		public long Id { get; set; }
		public long CellId { get; set; }
		public long ProfileId { get; set; }
		public int CurrentUse { get; set; }
		public int Moisture { get; set; }
		public int Drainage { get; set; }
		public int Nutrients { get; set; }
		public int Salinity { get; set; }
		public int Topsoil { get; set; }
		public int Tilth { get; set; }
		public int Rockiness { get; set; }
		public int Weeds { get; set; }
		public int Pests { get; set; }
		public int Fence { get; set; }
		public int Pasture { get; set; }
		public int Condition { get; set; }
		public string LastTickMudDateTime { get; set; }
		public string Definition { get; set; }

		public virtual Cell Cell { get; set; }
		public virtual AgricultureFieldProfile Profile { get; set; }
		public virtual AgricultureFieldCrop AgricultureFieldCrop { get; set; }
		public virtual AgricultureFieldWoodland AgricultureFieldWoodland { get; set; }
		public virtual ICollection<AgricultureFieldHerd> AgricultureFieldHerds { get; set; }
		public virtual ICollection<AgricultureProjectContext> AgricultureProjectContexts { get; set; }
	}

	public partial class AgricultureFieldCrop
	{
		public long AgricultureFieldId { get; set; }
		public long CropDefinitionId { get; set; }
		public int Stage { get; set; }
		public int GrowthDays { get; set; }
		public int Health { get; set; }
		public int YieldPotential { get; set; }
		public string PlantedMudDateTime { get; set; }
		public string Definition { get; set; }

		public virtual AgricultureCropDefinition CropDefinition { get; set; }
		public virtual AgricultureField AgricultureField { get; set; }
	}

	public partial class AgricultureFieldHerd
	{
		public long Id { get; set; }
		public long AgricultureFieldId { get; set; }
		public long HerdDefinitionId { get; set; }
		public int HeadCount { get; set; }
		public double Condition { get; set; }
		public string Definition { get; set; }

		public virtual AgricultureField AgricultureField { get; set; }
		public virtual AgricultureHerdDefinition HerdDefinition { get; set; }
	}

	public partial class AgricultureFieldWoodland
	{
		public long AgricultureFieldId { get; set; }
		public long WoodlandDefinitionId { get; set; }
		public int GrowthDays { get; set; }
		public int Health { get; set; }
		public int YieldPotential { get; set; }
		public string PlantedMudDateTime { get; set; }
		public string Definition { get; set; }

		public virtual AgricultureField AgricultureField { get; set; }
		public virtual AgricultureWoodlandDefinition WoodlandDefinition { get; set; }
	}

	public partial class AgricultureProjectContext
	{
		public long ActiveProjectId { get; set; }
		public long AgricultureFieldId { get; set; }
		public long OperationId { get; set; }
		public int TargetType { get; set; }
		public long? TargetId { get; set; }
		public string TargetText { get; set; }
		public long? ActorId { get; set; }
		public string Definition { get; set; }

		public virtual ActiveProject ActiveProject { get; set; }
		public virtual AgricultureField AgricultureField { get; set; }
		public virtual AgricultureOperation Operation { get; set; }
		public virtual Character Actor { get; set; }
	}
}
