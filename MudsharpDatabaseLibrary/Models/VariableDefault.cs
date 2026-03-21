using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MudSharp.Models
{
    public partial class VariableDefault
    {
        public string OwnerTypeDefinition { get; set; }
        public string Property { get; set; }
        public string DefaultValue { get; set; }

        [NotMapped]
        public long OwnerType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(OwnerTypeDefinition);
            set => OwnerTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }
    }
}
