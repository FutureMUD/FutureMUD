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

    /// <summary>
    /// Normalises a proposed builder-facing name before the generic editable-item rename workflow validates it.
    /// Concrete types with names backed by something other than <see cref="_name"/> can override this method.
    /// </summary>
    internal virtual bool TryNormaliseNameForBulkRename(string proposedName, out string normalisedName,
        out string error)
    {
		// Conventional editable records historically normalise their names to title case.
		// Concrete types with a deliberately raw or computed name override this behaviour.
		normalisedName = proposedName.Trim().TitleCase();
        if (string.IsNullOrWhiteSpace(normalisedName))
        {
            error = "Names cannot be blank.";
            return false;
        }

        if (normalisedName.All(char.IsDigit))
        {
            error = "Names cannot be entirely numeric, because numeric input is reserved for IDs.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>
    /// Applies a name which has already passed the generic editable-item rename validation.
    /// </summary>
    internal virtual void SetNameFromValidatedBulkRename(string name)
    {
        _name = name;
        if (this is MudSharp.Framework.Save.ISaveable saveable)
        {
            saveable.Changed = true;
        }
    }

    public override string ToString()
    {
        return $"{FrameworkItemType} #{_id:N0} - {Name ?? "Unnamed"}";
    }
}
