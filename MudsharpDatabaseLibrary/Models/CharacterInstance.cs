using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class CharacterInstance
{
	public CharacterInstance()
	{
		AnchoredInstances = new HashSet<CharacterInstance>();
	}

	public long Id { get; set; }
	public long CharacterId { get; set; }
	public long BodyId { get; set; }
	public long? PrimaryCharacterId { get; set; }
	public long? EmbodiedBodyId { get; set; }
	public string InstanceName { get; set; }
	public int InstanceKind { get; set; }
	public int ControlPolicy { get; set; }
	public int DeathPolicy { get; set; }
	public int PerceptionPolicy { get; set; }
	public int PersistencePolicy { get; set; }
	public long? LocationId { get; set; }
	public long? CurrentProjectId { get; set; }
	public long? CurrentProjectLabourId { get; set; }
	public double CurrentProjectHours { get; set; }
	public double CurrentProjectProjectHours { get; set; }
	public int RoomLayer { get; set; }
	public decimal? RoutePosition { get; set; }
	public int PositionId { get; set; }
	public int PositionModifier { get; set; }
	public long? PositionTargetId { get; set; }
	public string PositionTargetType { get; set; }
	public string PositionEmote { get; set; }
	public int State { get; set; }
	public int Status { get; set; }
	public bool IsPrimary { get; set; }
	public bool IsEmbodied { get; set; }
	public bool IsControllable { get; set; }
	public long? AnchorInstanceId { get; set; }
	public int? CreatedBySourceType { get; set; }
	public long? CreatedBySourceId { get; set; }
	public string CreatedBySourceKey { get; set; }
	public DateTime CreatedDateTime { get; set; }
	public DateTime? ExpiryDateTime { get; set; }
	public string EffectData { get; set; }

	public virtual Character Character { get; set; }
	public virtual Body Body { get; set; }
	public virtual Cell Location { get; set; }
	public virtual CharacterInstance AnchorInstance { get; set; }
	public virtual ICollection<CharacterInstance> AnchoredInstances { get; set; }
}
