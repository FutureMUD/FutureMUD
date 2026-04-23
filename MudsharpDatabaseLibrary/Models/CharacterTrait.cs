namespace MudSharp.Models
{
    public partial class CharacterTrait
    {
        public long CharacterId { get; set; }
        public long TraitDefinitionId { get; set; }
        public double Value { get; set; }
        public double AdditionalValue { get; set; }

        public virtual Character Character { get; set; }
        public virtual TraitDefinition TraitDefinition { get; set; }
    }
}
