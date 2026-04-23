namespace MudSharp.Models
{
    public partial class CharacterBodySource
    {
        public long CharacterId { get; set; }
        public int SourceType { get; set; }
        public long SourceId { get; set; }
        public string SourceKey { get; set; }
        public long BodyId { get; set; }

        public virtual Body Body { get; set; }
        public virtual Character Character { get; set; }
    }
}
