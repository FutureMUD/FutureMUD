using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MudSharp.Models
{
    public partial class VariableValue
    {
        public string ReferenceTypeDefinition { get; set; }
        public long ReferenceId { get; set; }
        public string ReferenceProperty { get; set; }
        public string ValueDefinition { get; set; }
        public string ValueTypeDefinition { get; set; }

        [NotMapped]
        public long ReferenceType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(ReferenceTypeDefinition);
            set => ReferenceTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }

        [NotMapped]
        public long ValueType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(ValueTypeDefinition);
            set => ValueTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }
    }
}
