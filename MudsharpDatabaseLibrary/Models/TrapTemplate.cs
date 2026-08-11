#nullable enable

namespace MudSharp.Models;

public partial class TrapTemplate
{
	public long Id { get; set; }
	public int RevisionNumber { get; set; }
	public long EditableItemId { get; set; }
	public string Name { get; set; } = null!;
	public string Definition { get; set; } = null!;

	public virtual EditableItem EditableItem { get; set; } = null!;
}
