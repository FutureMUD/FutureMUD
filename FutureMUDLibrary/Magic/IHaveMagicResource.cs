using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Magic
{
    public interface IHaveMagicResource
    {
        IEnumerable<IMagicResource> MagicResources { get; }
        IReadOnlyDictionary<IMagicResource, double> MagicResourceAmounts { get; }

        bool CanUseResource(IMagicResource resource, double amount);

        /// <summary>
        /// Uses a magic resource and returns a result if successful
        /// </summary>
        /// <param name="resource">The resource to consume</param>
        /// <param name="amount">The amount of the resource to consume</param>
        /// <returns>True if the IHaveMagicResource had enough of the resource to pay for it, false if it didn't.</returns>
        bool UseResource(IMagicResource resource, double amount);
        void AddResource(IMagicResource resource, double amount);
        IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators { get; }
        void AddMagicResourceGenerator(IMagicResourceRegenerator generator);
        void RemoveMagicResourceGenerator(IMagicResourceRegenerator generator);
    }
}
