using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EditableItem
    {
        public EditableItem()
        {
            CellOverlayPackages = new HashSet<CellOverlayPackage>();
            Crafts = new HashSet<Craft>();
            DisfigurementTemplates = new HashSet<DisfigurementTemplate>();
            ForagableProfiles = new HashSet<ForagableProfile>();
            Foragables = new HashSet<Foragable>();
            GameItemComponentProtos = new HashSet<GameItemComponentProto>();
            GameItemProtos = new HashSet<GameItemProto>();
            Npctemplates = new HashSet<NpcTemplate>();
            Projects = new HashSet<Project>();
        }

        public long Id { get; set; }
        public int RevisionNumber { get; set; }
        public int RevisionStatus { get; set; }
        public long BuilderAccountId { get; set; }
        public long? ReviewerAccountId { get; set; }
        public string BuilderComment { get; set; }
        public string ReviewerComment { get; set; }
        public DateTime BuilderDate { get; set; }
        public DateTime? ReviewerDate { get; set; }
        public DateTime? ObsoleteDate { get; set; }

        public virtual ICollection<CellOverlayPackage> CellOverlayPackages { get; set; }
        public virtual ICollection<Craft> Crafts { get; set; }
        public virtual ICollection<DisfigurementTemplate> DisfigurementTemplates { get; set; }
        public virtual ICollection<ForagableProfile> ForagableProfiles { get; set; }
        public virtual ICollection<Foragable> Foragables { get; set; }
        public virtual ICollection<GameItemComponentProto> GameItemComponentProtos { get; set; }
        public virtual ICollection<GameItemProto> GameItemProtos { get; set; }
        public virtual ICollection<NpcTemplate> Npctemplates { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
