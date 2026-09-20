namespace MudSharp.Framework.Save
{
    public interface ISaveable : IHaveFuturemud
    {
        /// <summary>
        ///     Indicates that the ISavable has changed since it was last saved
        /// </summary>
        bool Changed { get; set; }

        /// <summary>
        ///     Tells the object to perform whatever save action it needs to do
        /// </summary>
        void Save();
    }

    /// <summary>
    ///     Implemented by saveable owners that clear specialised dirty state while staging a database save.
    ///     The save manager invokes this hook after this owner's Save method was attempted and the enclosing
    ///     database commit failed, so that the owner can restore that state for a later retry. Implementations
    ///     must not throw.
    /// </summary>
    public interface IRecoverableSaveFailure
    {
        void RecoverFromSaveFailure();
    }
}
