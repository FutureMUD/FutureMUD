using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MudSharp.Models
{
    public partial class VariableDefinition
    {
        public string OwnerTypeDefinition { get; set; }
        public string Property { get; set; }
        public string ContainedTypeDefinition { get; set; }

        [NotMapped]
        public long OwnerType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(OwnerTypeDefinition);
            set => OwnerTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }

        [NotMapped]
        public long ContainedType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(ContainedTypeDefinition);
            set => ContainedTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }
    }
}
