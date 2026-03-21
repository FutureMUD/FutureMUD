using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MudSharp.Models
{
    public partial class FutureProgsParameter
    {
        public long FutureProgId { get; set; }
        public int ParameterIndex { get; set; }
        public string ParameterTypeDefinition { get; set; }
        public string ParameterName { get; set; }

        public virtual FutureProg FutureProg { get; set; }

        [NotMapped]
        public long ParameterType
        {
            get => ProgVariableTypeStorageConverter.ToLegacyLong(ParameterTypeDefinition);
            set => ParameterTypeDefinition = ProgVariableTypeStorageConverter.FromLegacyLong(value);
        }
    }
}
