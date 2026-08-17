namespace MudSharp.Models;

public class LootTable
{
	public long Id { get; set; }
	public int RevisionNumber { get; set; }
	public long EditableItemId { get; set; }
	public string Name { get; set; }
	public string Definition { get; set; }
	public int AlgorithmVersion { get; set; }

	public virtual EditableItem EditableItem { get; set; }
}
