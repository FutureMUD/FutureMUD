namespace MudSharp.Framework;

public abstract class FrameworkItem : IFrameworkItem
{
	protected long _id;
	protected string _name;

	public virtual string Name => _name;

	public virtual long Id
	{
		get => _id;
		set => _id = value;
	}

	public abstract string FrameworkItemType { get; }

	public override string ToString()
	{
		return $"{FrameworkItemType} #{_id:N0} - {Name ?? "Unnamed"}";
	}
}