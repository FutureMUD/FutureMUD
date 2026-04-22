namespace MudSharp.Models
{
    public partial class CharacterBody
    {
        public long CharacterId { get; set; }
        public long BodyId { get; set; }
        public string Alias { get; set; }
        public int SortOrder { get; set; }
        public bool AllowVoluntarySwitch { get; set; }
        public long? CanVoluntarilySwitchProgId { get; set; }
        public long? WhyCannotVoluntarilySwitchProgId { get; set; }

        public virtual Body Body { get; set; }
        public virtual Character Character { get; set; }
        public virtual FutureProg CanVoluntarilySwitchProg { get; set; }
        public virtual FutureProg WhyCannotVoluntarilySwitchProg { get; set; }
    }
}
