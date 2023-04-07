namespace MudSharp.Framework.Save {
    public interface ISaveable : IHaveFuturemud {
        /// <summary>
        ///     Indicates that the ISavable has changed since it was last saved
        /// </summary>
        bool Changed { get; set; }

        /// <summary>
        ///     Tells the object to perform whatever save action it needs to do
        /// </summary>
        void Save();
    }
}